import asyncio
import signal
import sys
import threading
from typing import Optional

from PySide6.QtWidgets import QApplication

from opensong_next.api.server import api_app, attach_controller, set_server_loop
from opensong_next.core import parser
from opensong_next.core.presentation import PresentationController, slides_from_song
from opensong_next.ui.control_window import ControlWindow
from opensong_next.ui.output_window import OutputWindow


def _run_api_server(controller: PresentationController, host: str = "127.0.0.1", port: int = 8000) -> "uvicorn.Server":
    """Start FastAPI server in a background thread."""

    import uvicorn

    config = uvicorn.Config(api_app, host=host, port=port, log_level="info")
    server = uvicorn.Server(config)

    def _serve() -> None:
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        set_server_loop(loop)
        attach_controller(controller, loop)
        server.run()

    thread = threading.Thread(target=_serve, daemon=True)
    thread.start()
    return server


def main() -> int:
    app = QApplication(sys.argv)

    # Build a tiny sample set so the UI has something to render.
    body = """
    [V1]
    [C]Amazing [G]grace||How sweet the sound
    [C]That saved a [G]wretch like [C]me

    [C]
    [F]I once was [C]lost||but now am [G]found
    """.strip()
    sample_song = parser.parse_classic(body, title="Amazing Grace")
    controller = PresentationController([slides_from_song(sample_song)])

    api_server: Optional["uvicorn.Server"] = _run_api_server(controller)

    control_window = ControlWindow(controller)
    output_window = OutputWindow(controller)
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
