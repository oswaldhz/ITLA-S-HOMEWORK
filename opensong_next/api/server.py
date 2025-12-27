from fastapi import FastAPI

api_app = FastAPI(title="OpenSong Next", version="0.1.0")


@api_app.get("/health", summary="Health check")
async def health() -> dict[str, str]:
    """Simple health endpoint to verify the API server is running."""
    return {"status": "ok"}


@api_app.get("/info", summary="Application metadata")
async def info() -> dict[str, str]:
    """Return basic application metadata."""
    return {
        "name": api_app.title or "OpenSong Next",
        "version": api_app.version or "0.0.0",
        "description": "Control and projection companion service",
    }
