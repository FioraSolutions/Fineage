#!/usr/bin/env python3
"""Classify a brush/BSP face reference by the package export's real class.

The reference is Package.Name or Package.Group.Name (group may itself contain
dots). The class comes from docs/interlude-ref/packages/exports.part*.tsv.
Texture and any subclass of Texture are texture pixels. Any other subclass of
Material (Shader, FinalBlend, Combiner, ColorModifier, TexPanner, ...) is a
material chain. The hierarchy is docs/interlude-ref/packages/classes.tsv.

An object that is not in the export table is absent. Two different classes for
the same package and object name are ambiguous. Neither is treated as a Texture.
"""
from __future__ import annotations

import csv
import re
from dataclasses import dataclass
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PACKAGES = REPO / "l2-unity" / "docs" / "interlude-ref" / "packages"
PROPS_REF = re.compile(r"\w+'([^']+)'")

KIND_TEXTURE = "texture"
KIND_MATERIAL = "material"
KIND_ABSENT = "absent"
KIND_OTHER = "other"
KIND_AMBIGUOUS = "ambiguous"


@dataclass(frozen=True)
class Face:
    package: str
    group: str
    name: str
    full: str
    ue_class: str
    kind: str

    def key(self) -> tuple[str, str, str]:
        return (self.package.lower(), self.group.lower(), self.name.lower())

    def format(self) -> str:
        cls = self.ue_class if self.ue_class else "absent"
        return (
            f"package={self.package} group={self.group} name={self.name} class={cls}"
        )


class ExportIndex:
    def __init__(self, packages: Path = PACKAGES) -> None:
        self.super_of: dict[str, str] = {}
        with (packages / "classes.tsv").open(encoding="utf-8") as fh:
            for row in csv.DictReader(fh, delimiter="\t"):
                self.super_of.setdefault(row["object"], row["super"])
        # (utx file lower, object lower) -> set of classes
        self.by_name: dict[tuple[str, str], set[str]] = {}
        self.package_files: set[str] = set()
        for part in sorted(packages.glob("exports.part*.tsv")):
            with part.open(encoding="utf-8") as fh:
                for row in csv.DictReader(fh, delimiter="\t"):
                    if row["ext"] != ".utx":
                        continue
                    file_name = row["file"].lower()
                    self.package_files.add(file_name)
                    key = (file_name, row["object"].lower())
                    self.by_name.setdefault(key, set()).add(row["class"])

    def has_package(self, package: str) -> bool:
        return (package.lower() + ".utx") in self.package_files

    def ancestors(self, ue_class: str) -> list[str]:
        out: list[str] = []
        seen: set[str] = set()
        current = ue_class
        while current and current not in seen and current != "Class":
            seen.add(current)
            out.append(current)
            current = self.super_of.get(current)
        return out

    def kind_of(self, ue_class: str) -> str:
        anc = self.ancestors(ue_class)
        if "Texture" in anc:
            return KIND_TEXTURE
        if "Material" in anc:
            return KIND_MATERIAL
        return KIND_OTHER


_INDEX: ExportIndex | None = None


def index() -> ExportIndex:
    global _INDEX
    if _INDEX is None:
        _INDEX = ExportIndex()
    return _INDEX


def parse_ref(full: str) -> tuple[str, str, str]:
    full = full.strip().strip("'\"")
    if "'" in full:
        # Class'Package.Group.Name' — keep the object path.
        full = full.split("'", 1)[1].rstrip("'")
    parts = [p for p in full.split(".") if p]
    if len(parts) < 2:
        raise ValueError(f"brush face reference is not Package.Name: {full}")
    group = ".".join(parts[1:-1])
    return parts[0], group, parts[-1]


def classify(full: str, idx: ExportIndex | None = None) -> Face:
    idx = idx or index()
    package, group, name = parse_ref(full)
    classes = idx.by_name.get((package.lower() + ".utx", name.lower()), set())
    canonical = f"{package}.{group}.{name}" if group else f"{package}.{name}"
    if not classes:
        return Face(package, group, name, canonical, "", KIND_ABSENT)
    if len(classes) != 1:
        listed = ",".join(sorted(classes))
        return Face(package, group, name, canonical, listed, KIND_AMBIGUOUS)
    ue_class = next(iter(classes))
    return Face(package, group, name, canonical, ue_class, idx.kind_of(ue_class))


def props_tokens(text: str) -> list[str]:
    return PROPS_REF.findall(text)


def resolve_props_token(token: str, current_package: str, idx: ExportIndex | None = None) -> Face:
    """A props reference is Package.Group.Name or Group.Name in the same package."""
    idx = idx or index()
    parts = [p for p in token.split(".") if p]
    if not parts:
        raise ValueError(f"empty material reference in props: {token!r}")
    if len(parts) >= 2 and idx.has_package(parts[0]):
        return classify(token, idx)
    if len(parts) == 1:
        return classify(f"{current_package}.{parts[0]}", idx)
    group = ".".join(parts[:-1])
    return classify(f"{current_package}.{group}.{parts[-1]}", idx)


def confirm_live(face: Face, rows: list[tuple[str, str, str]]) -> str | None:
    """rows are (name, class, group) from DescribeExports.

    Returns an error line when the live package does not contain this
    group path, or its class disagrees with the export table. None when
    the package and the table name the same class.
    """
    name_hits = [
        (n, cls, g) for n, cls, g in rows if n.lower() == face.name.lower()
    ]
    if face.group:
        chosen = [hit for hit in name_hits if hit[2].lower() == face.group.lower()]
    elif len(name_hits) == 1:
        # The brush reference is Package.Name. One export of that name is that object.
        chosen = name_hits
    else:
        chosen = [hit for hit in name_hits if hit[2] == ""]
    if not chosen:
        live = ", ".join(f"{cls}@{g or '(none)'}" for _, cls, g in name_hits) or "none"
        return (
            f"{face.format()} not in the package at this group path (live name hits: {live})"
        )
    live_classes = {cls for _, cls, _ in chosen}
    if len(live_classes) != 1:
        return f"{face.format()} ambiguous in the package: {sorted(live_classes)}"
    live = next(iter(live_classes))
    if face.kind == KIND_ABSENT:
        return f"{face.format()} absent from the export table; live class={live}"
    if live != face.ue_class:
        return f"{face.format()} export table disagrees with the package class={live}"
    return None
