import asyncio
import signal
import sys
import threading
from typing import Optional

from PySide6.QtCore import Qt
from PySide6.QtWidgets import QApplication, QLabel, QMainWindow, QVBoxLayout, QWidget

from opensong_next.api.server import api_app


class ControlWindow(QMainWindow):
    """Simple control window placeholder."""

    def __init__(self) -> None:
        super().__init__()
        self.setWindowTitle("OpenSong Next - Control")
        self._build_ui()

    def _build_ui(self) -> None:
        container = QWidget(self)
        label = QLabel("Control Surface", parent=container)
        label.setAlignment(Qt.AlignCenter)
        layout = QVBoxLayout(container)
        layout.addWidget(label)
        self.setCentralWidget(container)


class OutputWindow(QMainWindow):
    """Simple output/lyrics window placeholder."""

    def __init__(self) -> None:
        super().__init__()
        self.setWindowTitle("OpenSong Next - Output")
        self._build_ui()

    def _build_ui(self) -> None:
        container = QWidget(self)
        label = QLabel("Output Display", parent=container)
        label.setAlignment(Qt.AlignCenter)
        layout = QVBoxLayout(container)
        layout.addWidget(label)
        self.setCentralWidget(container)


def _run_api_server(host: str = "127.0.0.1", port: int = 8000) -> "uvicorn.Server":
    """Start FastAPI server in a background thread."""
    import uvicorn

    config = uvicorn.Config(api_app, host=host, port=port, log_level="info")
    server = uvicorn.Server(config)

    def _serve() -> None:
        asyncio.set_event_loop(asyncio.new_event_loop())
        server.run()

    thread = threading.Thread(target=_serve, daemon=True)
    thread.start()
    return server


def main() -> int:
    app = QApplication(sys.argv)
    api_server: Optional["uvicorn.Server"] = _run_api_server()

    control_window = ControlWindow()
    output_window = OutputWindow()
    control_window.show()
    output_window.show()

    def _stop_server() -> None:
        if api_server is not None:
            api_server.should_exit = True

    app.aboutToQuit.connect(_stop_server)

    signal.signal(signal.SIGINT, signal.SIG_DFL)
    return app.exec()


if __name__ == "__main__":
    raise SystemExit(main())
