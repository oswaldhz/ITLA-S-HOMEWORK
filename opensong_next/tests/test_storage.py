from opensong_next.storage.db import (
    BibleBook,
    BibleVersion,
    Database,
    ServiceSet,
    SetItem,
    Song,
    SongVersion,
)


def test_schema_and_repositories(tmp_path):
    db_file = tmp_path / "test.db"
    db = Database(db_file)

    song = db.songs.add(Song(title="Amazing Grace", author="Newton"))
    version = db.versions.add(SongVersion(song_id=song.id, content="[V]\n[C]Amazing grace"))

    found_song = db.songs.get(song.id)
    assert found_song and found_song.title == "Amazing Grace"

    service_set = db.sets.add(ServiceSet(name="Sunday"))
    item = db.set_items.add(SetItem(set_id=service_set.id, song_version_id=version.id, position=1))

    items = db.set_items.list_for_set(service_set.id)
    assert items and items[0].song_version_id == version.id

    kjv = db.bibles.add_version(BibleVersion(name="King James Version", abbreviation="KJV", language="en"))
    db.bibles.add_book(BibleBook(bible_version_id=kjv.id, name="Genesis", order_index=1, chapters=50))

    versions = db.bibles.list_versions()
    assert versions and versions[0].abbreviation == "KJV"

    books = db.bibles.list_books_for_version(kjv.id)
    assert books and books[0].name == "Genesis"

    db.close()

