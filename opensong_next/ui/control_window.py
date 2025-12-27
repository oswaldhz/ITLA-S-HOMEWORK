from __future__ import annotations

from typing import Optional

from PySide6.QtCore import Qt
from PySide6.QtGui import QFont
from PySide6.QtWidgets import (
    QLabel,
    QListWidget,
    QListWidgetItem,
    QMainWindow,
    QPushButton,
    QShortcut,
    QVBoxLayout,
    QWidget,
)

from opensong_next.core.presentation import PresentationController


class ControlWindow(QMainWindow):
    """Controller surface for navigating a running set."""

    def __init__(self, controller: PresentationController) -> None:
        super().__init__()
        self.controller = controller
        self.setWindowTitle("OpenSong Next - Control")
        self.setMinimumWidth(420)

        self._set_list: Optional[QListWidget] = None
        self._current_label: Optional[QLabel] = None
        self._next_label: Optional[QLabel] = None
        self._stage_label: Optional[QLabel] = None

        self._build_ui()
        self.controller.add_listener(lambda status: self._update_ui(status))
        self._update_ui(self.controller.status)
        self._wire_shortcuts()

    def _build_ui(self) -> None:
        container = QWidget(self)
        layout = QVBoxLayout(container)

        self._current_label = QLabel("Current slide", parent=container)
        self._current_label.setAlignment(Qt.AlignLeft | Qt.AlignTop)
        self._current_label.setWordWrap(True)
        self._current_label.setFont(QFont("Sans", 14))
        layout.addWidget(self._current_label)

        self._next_label = QLabel("Next slide", parent=container)
        self._next_label.setAlignment(Qt.AlignLeft | Qt.AlignTop)
        self._next_label.setWordWrap(True)
        self._next_label.setFont(QFont("Sans", 12))
        layout.addWidget(self._next_label)

        stage_title = QLabel("Stage view (current / next)", parent=container)
        stage_title.setAlignment(Qt.AlignLeft)
        layout.addWidget(stage_title)

        self._stage_label = QLabel("--", parent=container)
        self._stage_label.setWordWrap(True)
        self._stage_label.setFont(QFont("Monospace", 11))
        layout.addWidget(self._stage_label)

        layout.addWidget(QLabel("Set list", parent=container))
        self._set_list = QListWidget(parent=container)
        self._set_list.itemDoubleClicked.connect(self._jump_to_item)
        layout.addWidget(self._set_list)

        nav_row = QWidget(parent=container)
        nav_layout = QVBoxLayout(nav_row)
        next_btn = QPushButton("Next ▶", parent=nav_row)
        next_btn.clicked.connect(lambda: self.controller.next())
        prev_btn = QPushButton("◀ Previous", parent=nav_row)
        prev_btn.clicked.connect(lambda: self.controller.previous())
        nav_layout.addWidget(next_btn)
        nav_layout.addWidget(prev_btn)
        nav_row.setLayout(nav_layout)
        layout.addWidget(nav_row)

        container.setLayout(layout)
        self.setCentralWidget(container)

    def _wire_shortcuts(self) -> None:
        # Navigation shortcuts
        QShortcut(Qt.Key_Right, self, activated=lambda: self.controller.next())
        QShortcut(Qt.Key_Down, self, activated=lambda: self.controller.next())
        QShortcut(Qt.Key_Space, self, activated=lambda: self.controller.next())
        QShortcut(Qt.Key_Left, self, activated=lambda: self.controller.previous())
        QShortcut(Qt.Key_Up, self, activated=lambda: self.controller.previous())
        QShortcut(Qt.Key_Backspace, self, activated=lambda: self.controller.previous())

        # Mode shortcuts (N/K/W/L/F)
        QShortcut(Qt.Key_N, self, activated=lambda: self.controller.set_mode("normal"))
        QShortcut(Qt.Key_K, self, activated=lambda: self.controller.set_mode("black"))
        QShortcut(Qt.Key_W, self, activated=lambda: self.controller.set_mode("white"))
        QShortcut(Qt.Key_L, self, activated=lambda: self.controller.set_mode("logo"))
        QShortcut(Qt.Key_F, self, activated=lambda: self.controller.set_mode("freeze"))

    # UI updates ---------------------------------------------------------
    def _update_ui(self, status: dict[str, object]) -> None:
        current = status.get("current") if isinstance(status, dict) else None
        nxt = status.get("next") if isinstance(status, dict) else None
        mode = status.get("mode") if isinstance(status, dict) else ""

        if self._current_label is not None:
            if current:
                lines = "\n".join(current.get("lines", []))
                self._current_label.setText(f"[{mode}] {current.get('song', '')} ({current.get('section', '')})\n{lines}")
            else:
                self._current_label.setText("No slide loaded")

        if self._next_label is not None:
            if nxt:
                lines = "\n".join(nxt.get("lines", []))
                self._next_label.setText(f"Next: {nxt.get('song', '')} ({nxt.get('section', '')})\n{lines}")
            else:
                self._next_label.setText("End of set")

        if self._set_list is not None:
            self._refresh_set_items(status)

        if self._stage_label is not None:
            current_lines = " | ".join(current.get("lines", [])) if current else "--"
            next_lines = " | ".join(nxt.get("lines", [])) if nxt else "--"
            self._stage_label.setText(f"NOW: {current_lines}\nNEXT: {next_lines}")

    def _refresh_set_items(self, status: dict[str, object]) -> None:
        assert self._set_list is not None
        self._set_list.blockSignals(True)
        self._set_list.clear()
        offset = 0
        current_index = status.get("current_index", -1)
        for entry in status.get("set", []):
            title = entry.get("title", "")
            count = entry.get("count", 0)
            label = f"{title} ({count} slides)"
            item = QListWidgetItem(label)
            item.setData(Qt.UserRole, offset)
            self._set_list.addItem(item)

            # Highlight the active song block
            if current_index is not None and offset <= current_index < offset + count:
                item.setSelected(True)
            offset += count
        self._set_list.blockSignals(False)

    def _jump_to_item(self, item: QListWidgetItem) -> None:
        index = item.data(Qt.UserRole)
        if isinstance(index, int):
            self.controller.goto(index)
