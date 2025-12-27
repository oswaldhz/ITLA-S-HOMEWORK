from __future__ import annotations

import asyncio
from typing import Any

from fastapi import FastAPI, HTTPException, WebSocket, WebSocketDisconnect
from fastapi.responses import HTMLResponse

from opensong_next.core.presentation import ALLOWED_MODES, PresentationController

api_app = FastAPI(title="OpenSong Next", version="0.1.0")

_controller: PresentationController | None = None
_clients: set[WebSocket] = set()
_server_loop: asyncio.AbstractEventLoop | None = None


# Controller / loop attachment -------------------------------------------
def attach_controller(controller: PresentationController, loop: asyncio.AbstractEventLoop | None = None) -> None:
    global _controller, _server_loop
    _controller = controller
    if loop is not None:
        _server_loop = loop
    controller.add_listener(_schedule_broadcast)


def set_server_loop(loop: asyncio.AbstractEventLoop) -> None:
    global _server_loop
    _server_loop = loop


# Helpers ----------------------------------------------------------------
def _schedule_broadcast(status: dict[str, Any]) -> None:
    """Schedule a websocket broadcast on the server loop."""

    async def _send() -> None:
        await broadcast_status(status)

    if _server_loop and _server_loop.is_running():
        asyncio.run_coroutine_threadsafe(_send(), _server_loop)
    else:
        try:
            loop = asyncio.get_running_loop()
            loop.create_task(_send())
        except RuntimeError:
            # No running loop (likely called from GUI thread); skip
            pass


async def broadcast_status(status: dict[str, Any] | None = None) -> None:
    if status is None:
        status = _controller.status if _controller else {}
    stale: set[WebSocket] = set()
    for ws in list(_clients):
        try:
            await ws.send_json(status)
        except WebSocketDisconnect:
            stale.add(ws)
        except Exception:
            stale.add(ws)
    for ws in stale:
        _clients.discard(ws)


# Routes -----------------------------------------------------------------
@api_app.get("/health", summary="Health check")
async def health() -> dict[str, str]:
    return {"status": "ok"}


@api_app.get("/info", summary="Application metadata")
async def info() -> dict[str, str]:
    return {
        "name": api_app.title or "OpenSong Next",
        "version": api_app.version or "0.0.0",
        "description": "Control and projection companion service",
    }


@api_app.get("/status", summary="Current presentation status")
async def status() -> dict[str, Any]:
    if _controller is None:
        raise HTTPException(status_code=503, detail="Controller not ready")
    return _controller.status


@api_app.post("/next", summary="Advance to the next slide")
async def next_slide() -> dict[str, Any]:
    if _controller is None:
        raise HTTPException(status_code=503, detail="Controller not ready")
    return _controller.next()


@api_app.post("/prev", summary="Go back one slide")
async def previous_slide() -> dict[str, Any]:
    if _controller is None:
        raise HTTPException(status_code=503, detail="Controller not ready")
    return _controller.previous()


@api_app.post("/goto/{index}", summary="Jump to an absolute slide index")
async def goto_slide(index: int) -> dict[str, Any]:
    if _controller is None:
        raise HTTPException(status_code=503, detail="Controller not ready")
    return _controller.goto(index)


@api_app.post("/mode/{mode}", summary="Set projection mode")
async def set_mode(mode: str) -> dict[str, Any]:
    if mode not in ALLOWED_MODES:
        raise HTTPException(status_code=400, detail=f"Mode must be one of {sorted(ALLOWED_MODES)}")
    if _controller is None:
        raise HTTPException(status_code=503, detail="Controller not ready")
    return _controller.set_mode(mode)


_REMOTE_HTML = """
<!doctype html>
<html>
  <head>
    <meta charset=\"utf-8\" />
    <title>OpenSong Next Remote</title>
    <style>
      body { font-family: sans-serif; margin: 1rem; }
      #current { font-size: 1.4rem; margin-bottom: 0.5rem; }
      #next { color: #666; margin-bottom: 1rem; }
      ul { padding: 0; }
      li { list-style: none; padding: 0.2rem 0.4rem; }
      li.active { background: #eef; }
      button { margin-right: 0.5rem; }
    </style>
  </head>
  <body>
    <div id="current">Loading…</div>
    <div id="next"></div>
    <button onclick="send('prev')">◀ Prev</button>
    <button onclick="send('next')">Next ▶</button>
    <button onclick="setMode('normal')">N</button>
    <button onclick="setMode('black')">K</button>
    <button onclick="setMode('white')">W</button>
    <button onclick="setMode('logo')">L</button>
    <button onclick="setMode('freeze')">F</button>
    <h3>Set list</h3>
    <ul id="set"></ul>
    <script>
      const ws = new WebSocket((location.protocol === 'https:' ? 'wss://' : 'ws://') + location.host + '/ws/status');
      ws.onmessage = (event) => {
        const payload = JSON.parse(event.data);
        render(payload);
      };
      ws.onopen = () => ws.send(JSON.stringify({action: 'status'}));

      function send(action){ ws.send(JSON.stringify({action})); }
      function setMode(mode){ ws.send(JSON.stringify({action: 'mode', mode})); }

      function render(status){
        if(!status) return;
        const cur = status.current || {};
        const next = status.next || {};
        document.getElementById('current').innerText = `[${status.mode}] ${cur.song || ''} (${cur.section || ''})\n${(cur.lines||[]).join('\n')}`;
        document.getElementById('next').innerText = next.lines ? `Next: ${next.lines.join(' / ')}` : '';
        const list = document.getElementById('set');
        list.innerHTML = '';
        let offset = 0;
        (status.set || []).forEach(entry => {
          const li = document.createElement('li');
          li.innerText = `${entry.title} (${entry.count})`;
          if(status.current_index >= offset && status.current_index < offset + entry.count){
            li.classList.add('active');
          }
          li.onclick = () => ws.send(JSON.stringify({action:'goto', index: offset}));
          list.appendChild(li);
          offset += entry.count;
        });
      }
    </script>
  </body>
</html>
"""


@api_app.get("/", response_class=HTMLResponse, summary="Remote control UI")
async def remote_page() -> HTMLResponse:
    return HTMLResponse(_REMOTE_HTML)


@api_app.websocket("/ws/status")
async def ws_status(websocket: WebSocket) -> None:
    await websocket.accept()
    _clients.add(websocket)
    if _controller is not None:
        await websocket.send_json(_controller.status)
    try:
        while True:
            msg = await websocket.receive_json()
            if not isinstance(msg, dict):
                continue
            action = msg.get("action")
            if _controller is None:
                continue
            if action == "next":
                _controller.next()
            elif action == "prev":
                _controller.previous()
            elif action == "goto":
                index = msg.get("index")
                if isinstance(index, int):
                    _controller.goto(index)
            elif action == "mode":
                mode = msg.get("mode")
                if isinstance(mode, str) and mode in ALLOWED_MODES:
                    _controller.set_mode(mode)
            elif action == "status":
                await websocket.send_json(_controller.status)
    except WebSocketDisconnect:
        pass
    finally:
        _clients.discard(websocket)

