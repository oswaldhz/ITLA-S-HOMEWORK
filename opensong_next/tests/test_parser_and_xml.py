import xml.etree.ElementTree as ET

from opensong_next.core import parser
from opensong_next.core.xml_io import SongDocument, export_song, import_song


def test_classic_parser_sections_and_slides():
    raw = """
    [V1]
    [C]Amazing [G]grace||
    How [C]sweet the [F]sound
    [C]That saved a [G]wretch like [C]me

    [C]
    [F]I once was [C]lost|| but now am [G]found
    """

    song = parser.parse_classic(raw.strip(), title="Amazing Grace")

    assert song.title == "Amazing Grace"
    assert len(song.sections) == 2

    verse = song.sections[0]
    assert verse.label == "V1"
    assert len(verse.slides) == 2
    assert verse.slides[0].lines[0].startswith("[C]Amazing")

    chorus = song.sections[1]
    assert chorus.label == "C"
    assert chorus.slides[0].lines[0].startswith("[F]I once")


def test_transpose_line_and_song():
    line = "[C]Amazing [G]grace"
    transposed = parser.transpose_line(line, 2)
    assert "[D]" in transposed
    assert "[A]" in transposed

    song = parser.parse_classic("[V]\n[C]Test line")
    shifted = parser.transpose_song(song, -2)
    assert shifted.sections[0].slides[0].lines[0].startswith("[A#]")


def test_xml_round_trip():
    body = "[V1]\n[C]Line one||Line two"
    parsed = parser.parse_classic(body, title="Sample")
    doc = SongDocument(title="Sample", author="John Doe", key="C", lyrics=parsed)

    xml_str = export_song(doc)
    root = ET.fromstring(xml_str)
    assert root.findtext("title") == "Sample"

    imported = import_song(xml_str)
    assert imported.title == "Sample"
    assert imported.author == "John Doe"
    assert imported.key == "C"
    assert imported.lyrics
    assert imported.lyrics.sections[0].label == "V1"
    assert imported.lyrics.sections[0].slides[0].lines[0] == "[C]Line one"

