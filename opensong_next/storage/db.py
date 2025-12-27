"""SQLite storage layer for OpenSong Next.

This module defines a minimal schema plus repository helpers for songs,
song versions, service sets, and Bible metadata.  The goal is to keep the
API light-weight while still providing enough structure for the rest of the
application to build upon.
"""

from __future__ import annotations

import contextlib
import sqlite3
from dataclasses import dataclass
from pathlib import Path
from typing import Iterator, Optional

DB_FILENAME = "opensong_next.db"


def _connect(db_path: Path | str | None = None) -> sqlite3.Connection:
    path = Path(db_path) if db_path else Path(DB_FILENAME)
    conn = sqlite3.connect(path)
    conn.row_factory = sqlite3.Row
    return conn


def initialize_schema(conn: sqlite3.Connection) -> None:
    """Create all required tables if they do not exist."""

    conn.executescript(
        """
        PRAGMA foreign_keys = ON;

        CREATE TABLE IF NOT EXISTS songs (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            title TEXT NOT NULL,
            author TEXT,
            ccli TEXT,
            theme TEXT,
            created_at TEXT DEFAULT CURRENT_TIMESTAMP
        );

        CREATE TABLE IF NOT EXISTS song_versions (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            song_id INTEGER NOT NULL REFERENCES songs(id) ON DELETE CASCADE,
            label TEXT DEFAULT 'default',
            key_signature TEXT,
            tempo INTEGER,
            time_signature TEXT,
            content TEXT NOT NULL,
            created_at TEXT DEFAULT CURRENT_TIMESTAMP,
            updated_at TEXT DEFAULT CURRENT_TIMESTAMP
        );

        CREATE TRIGGER IF NOT EXISTS trg_song_versions_updated
        AFTER UPDATE ON song_versions
        BEGIN
            UPDATE song_versions SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
        END;

        CREATE TABLE IF NOT EXISTS sets (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            created_at TEXT DEFAULT CURRENT_TIMESTAMP
        );

        CREATE TABLE IF NOT EXISTS set_items (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            set_id INTEGER NOT NULL REFERENCES sets(id) ON DELETE CASCADE,
            song_version_id INTEGER NOT NULL REFERENCES song_versions(id) ON DELETE CASCADE,
            position INTEGER NOT NULL,
            notes TEXT
        );

        CREATE TABLE IF NOT EXISTS bible_versions (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            abbreviation TEXT,
            language TEXT
        );

        CREATE TABLE IF NOT EXISTS bible_books (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            bible_version_id INTEGER NOT NULL REFERENCES bible_versions(id) ON DELETE CASCADE,
            name TEXT NOT NULL,
            order_index INTEGER NOT NULL,
            chapters INTEGER
        );
        """
    )
    conn.commit()


@dataclass
class Song:
    title: str
    id: Optional[int] = None
    author: str | None = None
    ccli: str | None = None
    theme: str | None = None


@dataclass
class SongVersion:
    song_id: int
    content: str
    id: Optional[int] = None
    label: str = "default"
    key_signature: str | None = None
    tempo: int | None = None
    time_signature: str | None = None


@dataclass
class ServiceSet:
    name: str
    id: Optional[int] = None


@dataclass
class SetItem:
    set_id: int
    song_version_id: int
    position: int
    id: Optional[int] = None
    notes: str | None = None


@dataclass
class BibleVersion:
    name: str
    id: Optional[int] = None
    abbreviation: str | None = None
    language: str | None = None


@dataclass
class BibleBook:
    bible_version_id: int
    name: str
    order_index: int
    id: Optional[int] = None
    chapters: int | None = None


class RepositoryBase:
    def __init__(self, conn: sqlite3.Connection):
        self.conn = conn

    @contextlib.contextmanager
    def _cursor(self) -> Iterator[sqlite3.Cursor]:
        cur = self.conn.cursor()
        try:
            yield cur
            self.conn.commit()
        finally:
            cur.close()


class SongRepository(RepositoryBase):
    def add(self, song: Song) -> Song:
        with self._cursor() as cur:
            cur.execute(
                "INSERT INTO songs (title, author, ccli, theme) VALUES (?, ?, ?, ?)",
                (song.title, song.author, song.ccli, song.theme),
            )
            song.id = cur.lastrowid
        return song

    def get(self, song_id: int) -> Optional[Song]:
        row = self.conn.execute("SELECT * FROM songs WHERE id = ?", (song_id,)).fetchone()
        if not row:
            return None
        return Song(
            id=row["id"],
            title=row["title"],
            author=row["author"],
            ccli=row["ccli"],
            theme=row["theme"],
        )

    def list(self) -> list[Song]:
        rows = self.conn.execute("SELECT * FROM songs ORDER BY title").fetchall()
        return [Song(id=row["id"], title=row["title"], author=row["author"], ccli=row["ccli"], theme=row["theme"]) for row in rows]


class SongVersionRepository(RepositoryBase):
    def add(self, version: SongVersion) -> SongVersion:
        with self._cursor() as cur:
            cur.execute(
                (
                    "INSERT INTO song_versions (song_id, label, key_signature, tempo, time_signature, content) "
                    "VALUES (?, ?, ?, ?, ?, ?)"
                ),
                (
                    version.song_id,
                    version.label,
                    version.key_signature,
                    version.tempo,
                    version.time_signature,
                    version.content,
                ),
            )
            version.id = cur.lastrowid
        return version

    def list_for_song(self, song_id: int) -> list[SongVersion]:
        rows = self.conn.execute(
            "SELECT * FROM song_versions WHERE song_id = ? ORDER BY id",
            (song_id,),
        ).fetchall()
        return [
            SongVersion(
                id=row["id"],
                song_id=row["song_id"],
                label=row["label"],
                key_signature=row["key_signature"],
                tempo=row["tempo"],
                time_signature=row["time_signature"],
                content=row["content"],
            )
            for row in rows
        ]


class SetRepository(RepositoryBase):
    def add(self, service_set: ServiceSet) -> ServiceSet:
        with self._cursor() as cur:
            cur.execute("INSERT INTO sets (name) VALUES (?)", (service_set.name,))
            service_set.id = cur.lastrowid
        return service_set

    def list(self) -> list[ServiceSet]:
        rows = self.conn.execute("SELECT * FROM sets ORDER BY created_at DESC").fetchall()
        return [ServiceSet(id=row["id"], name=row["name"]) for row in rows]


class SetItemRepository(RepositoryBase):
    def add(self, item: SetItem) -> SetItem:
        with self._cursor() as cur:
            cur.execute(
                "INSERT INTO set_items (set_id, song_version_id, position, notes) VALUES (?, ?, ?, ?)",
                (item.set_id, item.song_version_id, item.position, item.notes),
            )
            item.id = cur.lastrowid
        return item

    def list_for_set(self, set_id: int) -> list[SetItem]:
        rows = self.conn.execute(
            "SELECT * FROM set_items WHERE set_id = ? ORDER BY position",
            (set_id,),
        ).fetchall()
        return [
            SetItem(
                id=row["id"],
                set_id=row["set_id"],
                song_version_id=row["song_version_id"],
                position=row["position"],
                notes=row["notes"],
            )
            for row in rows
        ]


class BibleRepository(RepositoryBase):
    def add_version(self, version: BibleVersion) -> BibleVersion:
        with self._cursor() as cur:
            cur.execute(
                "INSERT INTO bible_versions (name, abbreviation, language) VALUES (?, ?, ?)",
                (version.name, version.abbreviation, version.language),
            )
            version.id = cur.lastrowid
        return version

    def add_book(self, book: BibleBook) -> BibleBook:
        with self._cursor() as cur:
            cur.execute(
                "INSERT INTO bible_books (bible_version_id, name, order_index, chapters) VALUES (?, ?, ?, ?)",
                (book.bible_version_id, book.name, book.order_index, book.chapters),
            )
            book.id = cur.lastrowid
        return book

    def list_versions(self) -> list[BibleVersion]:
        rows = self.conn.execute("SELECT * FROM bible_versions ORDER BY name").fetchall()
        return [
            BibleVersion(
                id=row["id"],
                name=row["name"],
                abbreviation=row["abbreviation"],
                language=row["language"],
            )
            for row in rows
        ]

    def list_books_for_version(self, version_id: int) -> list[BibleBook]:
        rows = self.conn.execute(
            "SELECT * FROM bible_books WHERE bible_version_id = ? ORDER BY order_index",
            (version_id,),
        ).fetchall()
        return [
            BibleBook(
                id=row["id"],
                bible_version_id=row["bible_version_id"],
                name=row["name"],
                order_index=row["order_index"],
                chapters=row["chapters"],
            )
            for row in rows
        ]


class Database:
    """Convenience wrapper bundling a connection and repositories."""

    def __init__(self, db_path: Path | str | None = None):
        self.conn = _connect(db_path)
        initialize_schema(self.conn)
        self.songs = SongRepository(self.conn)
        self.versions = SongVersionRepository(self.conn)
        self.sets = SetRepository(self.conn)
        self.set_items = SetItemRepository(self.conn)
        self.bibles = BibleRepository(self.conn)

    def close(self) -> None:
        self.conn.close()

