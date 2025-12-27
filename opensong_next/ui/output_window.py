from __future__ import annotations

from typing import Optional

from PySide6.QtCore import Qt
from PySide6.QtGui import QColor, QFont, QPalette
from PySide6.QtWidgets import QLabel, QMainWindow, QVBoxLayout, QWidget

from opensong_next.core.presentation import PresentationController


class OutputWindow(QMainWindow):
    """Projection window showing the currently selected slide."""

    def __init__(self, controller: PresentationController) -> None:
        super().__init__()
        self.controller = controller
        self.setWindowTitle("OpenSong Next - Output")

        self._slide_label: Optional[QLabel] = None
        self._next_label: Optional[QLabel] = None
        self._frozen_text: str | None = None

        self._build_ui()
        self.controller.add_listener(lambda status: self._render(status))
        self._render(self.controller.status)

    def _build_ui(self) -> None:
        container = QWidget(self)
        layout = QVBoxLayout(container)

        self._slide_label = QLabel("Slide output", parent=container)
        self._slide_label.setAlignment(Qt.AlignCenter)
        self._slide_label.setWordWrap(True)
        self._slide_label.setFont(QFont("Sans", 28, QFont.Bold))
        self._slide_label.setMargin(40)
        layout.addWidget(self._slide_label, stretch=2)

        self._next_label = QLabel("Upcoming", parent=container)
        self._next_label.setAlignment(Qt.AlignRight | Qt.AlignBottom)
        self._next_label.setWordWrap(True)
        self._next_label.setFont(QFont("Sans", 14))
        self._next_label.setMargin(16)
        layout.addWidget(self._next_label, stretch=1)

        container.setLayout(layout)
        self.setCentralWidget(container)

    def showEvent(self, event) -> None:  # type: ignore[override]
        super().showEvent(event)
        self._move_to_secondary()

    def _move_to_secondary(self) -> None:
        handle = self.windowHandle()
        if handle is None:
            return
        screens = self.windowHandle().screen().virtualSiblings() if handle.screen() else []
        if not screens:
            screens = self.screen().virtualSiblings() if self.screen() else []
        if screens and len(screens) > 1:
            target = screens[1]
            handle.setScreen(target)
            self.move(target.geometry().topLeft())
            self.showFullScreen()
        else:
            self.showMaximized()

    def _render(self, status: dict[str, object]) -> None:
        mode = status.get("mode") if isinstance(status, dict) else "normal"
        current = status.get("current") if isinstance(status, dict) else None
        nxt = status.get("next") if isinstance(status, dict) else None

        bg = None
        if current:
            bg = current.get("background")

        self._apply_mode(mode, bg)

        if self._slide_label is not None:
            if mode == "freeze" and self._frozen_text:
                self._slide_label.setText(self._frozen_text)
            elif current and mode not in {"black", "logo"}:
                lines = "\n".join(current.get("lines", []))
                self._slide_label.setText(lines)
                self._frozen_text = lines
            elif mode == "logo":
                self._slide_label.setText("OpenSong Next")
            elif mode == "black":
                self._slide_label.setText("")
                self._frozen_text = None
            else:
                self._slide_label.setText("No slide loaded")
                self._frozen_text = None

        if self._next_label is not None:
            if nxt:
                lines = " / ".join(nxt.get("lines", []))
                self._next_label.setText(f"Next: {lines}")
            else:
                self._next_label.setText("")

    def _apply_mode(self, mode: str, background: Optional[str]) -> None:
        palette = self.palette()
        if mode == "black":
            palette.setColor(QPalette.Window, QColor("black"))
            palette.setColor(QPalette.WindowText, QColor("white"))
            self._slide_label.setStyleSheet("color: white;")
        elif mode == "white":
            palette.setColor(QPalette.Window, QColor("white"))
            palette.setColor(QPalette.WindowText, QColor("black"))
            self._slide_label.setStyleSheet("color: black;")
        else:
            palette.setColor(QPalette.Window, QColor("#111"))
            palette.setColor(QPalette.WindowText, QColor("white"))
            self._slide_label.setStyleSheet("color: white;")

        if background and mode == "normal":
            self.setStyleSheet(
                f"QMainWindow {{background-image: url('{background}'); background-position: center; background-repeat: no-repeat; background-size: cover;}}"
            )
        else:
            self.setStyleSheet("")

        self.setPalette(palette)
        if self._next_label is not None:
            self._next_label.setStyleSheet("color: #ddd;")
