"""Presentation state and navigation helpers for OpenSong Next.

This module centralizes slide management so that the Qt UI and the FastAPI
server can share a single source of truth.  It exposes a small set of
navigation helpers plus a serializable status payload used by the REST and
WebSocket APIs.
"""
from __future__ import annotations

import threading
from dataclasses import dataclass, field
from typing import Callable, Iterable, List, Optional

from .parser import ParsedSong


ALLOWED_MODES = {"normal", "black", "white", "logo", "freeze"}


@dataclass
class SlideContent:
    """Renderable slide content."""

    song_title: str
    section: str
    lines: List[str] = field(default_factory=list)
    background: str | None = None

    def to_payload(self) -> dict[str, object]:
        return {
            "song": self.song_title,
            "section": self.section,
            "lines": self.lines,
            "background": self.background,
        }


@dataclass
class SetEntry:
    """Group of slides associated with a single song."""

    title: str
    slides: List[SlideContent]
    background: str | None = None

    def to_payload(self, offset: int) -> dict[str, object]:
        return {
            "title": self.title,
            "count": len(self.slides),
            "offset": offset,
            "background": self.background,
        }


def slides_from_song(song: ParsedSong, *, default_background: str | None = None) -> SetEntry:
    """Flatten a :class:`ParsedSong` into slide-friendly structures."""

    slides: list[SlideContent] = []
    for section in song.sections:
        for slide in section.slides:
            slides.append(
                SlideContent(
                    song_title=song.title or "Untitled",
                    section=section.label,
                    lines=list(slide.lines),
                    background=default_background,
                )
            )
    return SetEntry(title=song.title or "Untitled", slides=slides, background=default_background)


class PresentationController:
    """Stateful slide navigator shared between UI and API layers."""

    def __init__(self, set_entries: Iterable[SetEntry]):
        self._set_entries = list(set_entries)
        self._slides: list[SlideContent] = []
        for entry in self._set_entries:
            for slide in entry.slides:
                if slide.background is None:
                    slide.background = entry.background
                self._slides.append(slide)

        self._index = 0 if self._slides else -1
        self._mode = "normal"
        self._listeners: list[Callable[[dict[str, object]], None]] = []
        self._lock = threading.RLock()

    # Listener management -------------------------------------------------
    def add_listener(self, callback: Callable[[dict[str, object]], None]) -> None:
        """Register a synchronous listener for status updates."""

        with self._lock:
            if callback not in self._listeners:
                self._listeners.append(callback)

    def _emit(self) -> None:
        status = self.status
        for listener in list(self._listeners):
            try:
                listener(status)
            except Exception:
                # Listener errors should not crash the UI/API loop
                continue

    # Properties ----------------------------------------------------------
    @property
    def index(self) -> int:
        with self._lock:
            return self._index

    @property
    def mode(self) -> str:
        with self._lock:
            return self._mode

    @property
    def slides(self) -> list[SlideContent]:
        return self._slides

    @property
    def current_slide(self) -> Optional[SlideContent]:
        with self._lock:
            if 0 <= self._index < len(self._slides):
                return self._slides[self._index]
            return None

    @property
    def next_slide(self) -> Optional[SlideContent]:
        with self._lock:
            nxt = self._index + 1
            if 0 <= nxt < len(self._slides):
                return self._slides[nxt]
            return None

    @property
    def status(self) -> dict[str, object]:
        with self._lock:
            offset = 0
            set_payload = []
            for entry in self._set_entries:
                set_payload.append(entry.to_payload(offset))
                offset += len(entry.slides)

            return {
                "mode": self._mode,
                "current_index": self._index,
                "total": len(self._slides),
                "current": self.current_slide.to_payload() if self.current_slide else None,
                "next": self.next_slide.to_payload() if self.next_slide else None,
                "set": set_payload,
            }

    # Navigation ----------------------------------------------------------
    def next(self) -> dict[str, object]:
        with self._lock:
            if self._index + 1 < len(self._slides):
                self._index += 1
        self._emit()
        return self.status

    def previous(self) -> dict[str, object]:
        with self._lock:
            if self._index > 0:
                self._index -= 1
        self._emit()
        return self.status

    def goto(self, index: int) -> dict[str, object]:
        with self._lock:
            if 0 <= index < len(self._slides):
                self._index = index
        self._emit()
        return self.status

    def set_mode(self, mode: str) -> dict[str, object]:
        if mode not in ALLOWED_MODES:
            raise ValueError(f"Unsupported mode: {mode}")
        with self._lock:
            self._mode = mode
        self._emit()
        return self.status

    def reset(self) -> dict[str, object]:
        with self._lock:
            self._index = 0 if self._slides else -1
            self._mode = "normal"
        self._emit()
        return self.status

