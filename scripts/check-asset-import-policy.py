#!/usr/bin/env python3
"""Verify l2-unity texture/audio .meta files match ImportPolicy rules (CI gate)."""

from __future__ import annotations

import argparse
import json
import sys
from collections import Counter
from pathlib import Path

_REPO = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(_REPO / "scripts" / "lib"))

from asset_import_policy import (  # noqa: E402
    L2_UNITY,
    apply_audio_meta,
    apply_texture_meta,
    audio_meta_matches,
    build_audio_expectation,
    build_texture_expectation,
    iter_policy_audio_paths,
    iter_policy_texture_paths,
    load_rules,
    posix_path,
    resolve_texture_rule,
    texture_meta_matches,
)


def check_required_readable(rules: dict, errors: list[str]) -> None:
    for entry in rules.get("requiredReadableAssets", []):
        rel = entry["assetPath"]
        path = L2_UNITY / rel
        if not path.is_file():
            errors.append(f"required readable asset missing: {rel} ({entry['reason']})")
            continue
        text = path.read_text(encoding="utf-8")
        field = entry["yamlField"]
        expected = int(entry["expectedValue"])
        match = None
        for line in text.splitlines():
            if line.startswith(f"  {field}:"):
                match = int(line.split(":", 1)[1].strip())
                break
        if match != expected:
            errors.append(
                f"{rel}: expected {field}={expected} for {entry['reason']}, got {match}"
            )


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--apply", action="store_true", help="Rewrite .meta files to match policy")
    parser.add_argument("--report", metavar="PATH", help="Write JSON counts per rule id")
    args = parser.parse_args()

    rules = load_rules()
    errors: list[str] = []
    texture_counts: Counter[str] = Counter()
    audio_counts: Counter[str] = Counter()
    touched_textures = 0
    touched_audio = 0

    if args.apply:
        check_required_readable(rules, errors)

    for asset in iter_policy_texture_paths():
        rel = posix_path(asset)
        if rel.endswith(".asset"):
            continue
        meta_path = asset.with_suffix(asset.suffix + ".meta")
        if not meta_path.is_file():
            errors.append(f"missing texture meta: {rel}.meta")
            continue
        expectation = build_texture_expectation(rel, rules, asset)
        if expectation is None:
            errors.append(f"no texture import rule matched: {rel}")
            continue
        texture_counts[expectation.rule_id] += 1
        text = meta_path.read_text(encoding="utf-8")
        if "TextureImporter:" not in text:
            errors.append(f"not a TextureImporter meta: {rel}.meta")
            continue
        if args.apply:
            updated = apply_texture_meta(text, expectation)
            if updated != text:
                meta_path.write_text(updated, encoding="utf-8", newline="\n")
                touched_textures += 1
        elif not texture_meta_matches(text, expectation):
            errors.append(f"texture meta drift: {rel} (rule {expectation.rule_id})")

    # Embedded readable texture asset (not governed by TextureImporter).
    for asset in iter_policy_texture_paths():
        if not str(asset).endswith("sh2_frame_positions.asset"):
            continue
        rel = posix_path(asset)
        text = asset.read_text(encoding="utf-8")
        texture_counts["runtime-data-texture"] += 1
        if "m_IsReadable: 1" not in text:
            if args.apply:
                if "m_IsReadable: 0" in text:
                    text = text.replace("m_IsReadable: 0", "m_IsReadable: 1", 1)
                    asset.write_text(text, encoding="utf-8", newline="\n")
                    touched_textures += 1
            else:
                errors.append(f"{rel}: m_IsReadable must be 1 (L2VertMeshFrameBank)")

    for audio in iter_policy_audio_paths():
        rel = posix_path(audio)
        meta_path = audio.with_suffix(audio.suffix + ".meta")
        if not meta_path.is_file():
            errors.append(f"missing audio meta: {rel}.meta")
            continue
        try:
            expectation = build_audio_expectation(rel, rules, audio)
        except ValueError as exc:
            errors.append(str(exc))
            continue
        audio_counts[expectation.rule_id] += 1
        text = meta_path.read_text(encoding="utf-8")
        if args.apply:
            updated = apply_audio_meta(text, expectation)
            if updated != text:
                meta_path.write_text(updated, encoding="utf-8", newline="\n")
                touched_audio += 1
        elif not audio_meta_matches(text, expectation):
            errors.append(f"audio meta drift: {rel} (rule {expectation.rule_id})")

    if not args.apply:
        check_required_readable(rules, errors)

    report = {
        "texturesByRule": dict(texture_counts),
        "audioByRule": dict(audio_counts),
        "textureAssets": sum(texture_counts.values()),
        "audioAssets": sum(audio_counts.values()),
        "touchedTextures": touched_textures,
        "touchedAudio": touched_audio,
        "errors": len(errors),
    }

    if args.report:
        Path(args.report).write_text(json.dumps(report, indent=2, sort_keys=True) + "\n", encoding="utf-8")

    if args.apply:
        print(json.dumps(report, indent=2, sort_keys=True))
        return 0

    if errors:
        print(f"asset import policy: {len(errors)} mismatch(es)", file=sys.stderr)
        for line in errors[:40]:
            print(line, file=sys.stderr)
        if len(errors) > 40:
            print(f"... and {len(errors) - 40} more", file=sys.stderr)
        return 1

    print(
        "asset import policy OK: "
        f"{report['textureAssets']} textures, {report['audioAssets']} audio "
        f"({len(texture_counts)} texture rules, {len(audio_counts)} audio rules active)"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())