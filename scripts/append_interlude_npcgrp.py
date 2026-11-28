#!/usr/bin/env python3
"""Append the Interlude npcgrp / npcname rows that Npcgrp_Classic.txt / NpcName_Classic-eu.txt lack.

Source of truth: l2-unity/docs/interlude-ref/dats/npcgrp.tsv and npcname-e.tsv (decrypted Interlude dats).
Only ids that are absent from the Classic files are appended; existing Classic rows are never touched.
Only fields the Interlude dat really carries are written: class_name, mesh_name, property_list,
npc_speed, attack_sound1, defense_sound1, damage_sound, attack_effect (and id/name/nick/nickcolor for names).
Fields it does not carry (collision, org_hp/mp, npc_type, hpshowable, social, texture lists) are omitted, not guessed.
Idempotent: a second run appends nothing.
"""
import json
import os
import sys

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "l2-unity")
DATS = os.path.join(ROOT, "docs", "interlude-ref", "dats")
META = os.path.join(ROOT, "Assets", "StreamingAssets", "Data", "Meta")
GRP = os.path.join(META, "Npcgrp_Classic.txt")
NAME = os.path.join(META, "NpcName_Classic-eu.txt")


def tsv(path):
    with open(path, encoding="utf-8") as f:
        rows = [l.rstrip("\n").split("\t") for l in f]
    return rows[1:]


def arr(cell):
    v = json.loads(cell)
    return "{" + ";".join("[%s]" % s for s in v if s != "") + "}"


def existing_ids(path, key):
    ids = set()
    with open(path, encoding="utf-8") as f:
        for line in f:
            for part in line.split("\t"):
                if part.startswith(key + "="):
                    ids.add(part.split("=", 1)[1].strip())
    return ids


def main():
    have_grp = existing_ids(GRP, "npc_id")
    have_name = existing_ids(NAME, "id")
    grp_lines, name_lines = [], []
    for r in tsv(os.path.join(DATS, "npcgrp.tsv")):
        npc_id, cls, mesh = r[0], r[1], r[2]
        if npc_id in have_grp:
            continue
        props = ";".join(str(x) for x in json.loads(r[8]))
        speed = float(r[9])
        parts = [
            "npc_begin", "npc_id=" + npc_id, "class_name=[%s]" % cls,
            "mesh_name=[%s]" % (mesh if mesh else "None"),
            "property_list={%s}" % props, "npc_speed=%s" % repr(speed),
            "attack_sound1=" + arr(r[13]), "defense_sound1=" + arr(r[15]),
            "damage_sound=" + arr(r[17]), "attack_effect=[%s]" % r[23], "npc_end",
        ]
        grp_lines.append("\t".join(parts))
    for r in tsv(os.path.join(DATS, "npcname-e.tsv")):
        npc_id, name, desc, rgb = r[0], r[1], r[2], json.loads(r[3])
        if npc_id in have_name:
            continue
        color = "%02X%02X%02XFF" % tuple(rgb)
        name_lines.append("\t".join([
            "npc_begin", "id=" + npc_id, "name=[%s]" % name, "nick=[%s]" % desc,
            "nickcolor=" + color, "npc_end"]))
    for path, lines in ((GRP, grp_lines), (NAME, name_lines)):
        if lines:
            with open(path, "a", encoding="utf-8", newline="\n") as f:
                f.write("\n".join(lines) + "\n")
    print("appended npcgrp=%d npcname=%d" % (len(grp_lines), len(name_lines)))


if __name__ == "__main__":
    sys.exit(main())
