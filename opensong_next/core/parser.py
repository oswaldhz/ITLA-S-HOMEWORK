"""Parsing utilities for OpenSong classic text format.

The classic format uses section headers like ``[V]`` (verse) or ``[C]``
followed by chord/lyric lines.  Slide breaks are indicated by ``||`` on a
dedicated line or embedded at the end of a line.
"""

from __future__ import annotations

import re
from dataclasses import dataclass, field
from typing import Iterator


SECTION_RE = re.compile(r"^\[(?P<label>[A-Za-z0-9]+)\]\s*$")
SLIDE_BREAK = "||"

CHORD_STEPS = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"]
ENHARMONICS = {"Db": "C#", "Eb": "D#", "Gb": "F#", "Ab": "G#", "Bb": "A#"}
CHORD_INDEX = {name: idx for idx, name in enumerate(CHORD_STEPS)}


@dataclass
class Slide:
    lines: list[str] = field(default_factory=list)


@dataclass
class Section:
    label: str
    slides: list[Slide] = field(default_factory=list)

    def add_line(self, line: str) -> None:
        if not self.slides:
            self.slides.append(Slide())
        self.slides[-1].lines.append(line)

    def add_slide(self) -> None:
        if not self.slides or self.slides[-1].lines:
            self.slides.append(Slide())


@dataclass
class ParsedSong:
    title: str | None
    sections: list[Section]

    def iter_lines(self) -> Iterator[str]:
        for section in self.sections:
            for slide in section.slides:
                for line in slide.lines:
                    yield line


def parse_classic(text: str, *, title: str | None = None) -> ParsedSong:
    """Parse a classic-format string into structured sections and slides."""

    sections: list[Section] = []
    current: Section | None = None

    for raw_line in text.splitlines():
        line = raw_line.rstrip()
        stripped = line.strip()
        if not line:
            continue

        match = SECTION_RE.match(stripped)
        if match:
            current = Section(label=match.group("label"))
            sections.append(current)
            continue

        if current is None:
            current = Section(label="BODY")
            sections.append(current)

        if SLIDE_BREAK in stripped:
            before, after = line.split(SLIDE_BREAK, 1)
            before = before.strip()
            after = after.strip()
            if before:
                current.add_line(before)
            current.add_slide()
            if after:
                current.add_line(after)
            continue

        current.add_line(stripped)

    return ParsedSong(title=title, sections=sections)


def transpose_chord(chord: str, semitones: int) -> str:
    """Transpose a single chord name by the given number of semitones."""

    root_match = re.match(r"([A-G](?:#|b)?)(.*)", chord)
    if not root_match:
        return chord

    root, suffix = root_match.groups()
    root = ENHARMONICS.get(root, root)
    idx = CHORD_INDEX.get(root)
    if idx is None:
        return chord

    new_idx = (idx + semitones) % len(CHORD_STEPS)
    return CHORD_STEPS[new_idx] + suffix


def _replace_inline(match: re.Match[str], semitones: int) -> str:
    chord = match.group(1)
    return f"[{transpose_chord(chord, semitones)}]"


def transpose_line(line: str, semitones: int) -> str:
    """Transpose all inline chords (``[C]``) contained in a line."""

    return re.sub(r"\[([^\]]+)\]", lambda m: _replace_inline(m, semitones), line)


def transpose_song(song: ParsedSong, semitones: int) -> ParsedSong:
    """Return a new ParsedSong with chords transposed."""

    new_sections: list[Section] = []
    for section in song.sections:
        new_section = Section(label=section.label)
        for slide in section.slides:
            new_slide = Slide()
            for line in slide.lines:
                new_slide.lines.append(transpose_line(line, semitones))
            new_section.slides.append(new_slide)
        new_sections.append(new_section)

    return ParsedSong(title=song.title, sections=new_sections)

