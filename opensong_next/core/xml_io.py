"""OpenSong compatible XML import/export helpers."""

from __future__ import annotations

import xml.etree.ElementTree as ET
from dataclasses import dataclass

from .parser import ParsedSong, Section, Slide


@dataclass
class SongDocument:
    title: str
    author: str | None = None
    key: str | None = None
    ccli: str | None = None
    lyrics: ParsedSong | None = None


def export_song(song: SongDocument) -> str:
    root = ET.Element("song")
    ET.SubElement(root, "title").text = song.title
    if song.author:
        ET.SubElement(root, "author").text = song.author
    if song.key:
        ET.SubElement(root, "key").text = song.key
    if song.ccli:
        ET.SubElement(root, "ccli").text = song.ccli

    lyrics_el = ET.SubElement(root, "lyrics")

    if song.lyrics:
        for section in song.lyrics.sections:
            verse_el = ET.SubElement(lyrics_el, "verse", name=section.label.lower())
            slide_parts = []
            for slide in section.slides:
                slide_parts.append("\n".join(slide.lines))
            verse_el.text = "||".join(slide_parts)

    return ET.tostring(root, encoding="unicode")


def import_song(xml_content: str) -> SongDocument:
    root = ET.fromstring(xml_content)
    title = (root.findtext("title") or "").strip()
    author = root.findtext("author")
    key = root.findtext("key")
    ccli = root.findtext("ccli")

    sections: list[Section] = []
    lyrics_el = root.find("lyrics")
    if lyrics_el is not None:
        for verse_el in lyrics_el.findall("verse"):
            label = (verse_el.attrib.get("name") or "verse").upper()
            text = verse_el.text or ""
            slides_raw = text.split("||") if text else []
            section = Section(label=label)
            for slide_chunk in slides_raw:
                slide = Slide(lines=[line for line in slide_chunk.split("\n") if line])
                section.slides.append(slide)
            sections.append(section)

    parsed = ParsedSong(title=title, sections=sections)
    return SongDocument(title=title, author=author, key=key, ccli=ccli, lyrics=parsed)

