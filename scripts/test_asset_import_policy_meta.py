#!/usr/bin/env python3
"""Unit tests for TextureImporter platformSettings rewriting (CI)."""

from __future__ import annotations

import sys
from pathlib import Path

_REPO = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(_REPO / "scripts" / "lib"))

from asset_import_policy import (  # noqa: E402
    TextureExpectation,
    _parse_platform_blocks,
    _replace_platform_settings,
    apply_texture_meta,
    read_texture_meta_values,
    texture_meta_matches,
)


def _sample_expectation() -> TextureExpectation:
    platforms = {
        "Standalone": {
            "name": "Standalone",
            "overridden": True,
            "format": "DXT5",
            "compression": "Compressed",
            "maxTextureSize": 2048,
        },
        "Android": {
            "name": "Android",
            "overridden": True,
            "format": "ETC2_RGBA8",
            "compression": "Compressed",
            "maxTextureSize": 2048,
        },
        "iPhone": {
            "name": "iPhone",
            "overridden": True,
            "format": "ASTC_4x4",
            "compression": "Compressed",
            "maxTextureSize": 2048,
        },
    }
    return TextureExpectation(
        rule_id="world-alpha-cutout",
        readable=False,
        mipmaps=True,
        streaming_mipmaps=True,
        mip_maps_preserve_coverage=True,
        filter_mode=None,
        npot_scale=None,
        wrap_mode=None,
        platforms=platforms,
    )


def _minimal_meta(platform_settings_body: str) -> str:
    return (
        "fileFormatVersion: 2\n"
        "guid: deadbeefdeadbeefdeadbeefdeadbeef\n"
        "TextureImporter:\n"
        "  mipmaps:\n"
        "    enableMipMap: 0\n"
        "    mipMapsPreserveCoverage: 0\n"
        "  isReadable: 1\n"
        "  streamingMipmaps: 0\n"
        f"  platformSettings:{platform_settings_body}\n"
        "  spriteSheet:\n"
        "    sprites: []\n"
    )


def test_empty_platform_settings_list_replaced() -> None:
    meta = _minimal_meta(" []\n")
    expectation = _sample_expectation()
    updated = apply_texture_meta(meta, expectation)
    assert "platformSettings: []" not in updated
    blocks = _parse_platform_blocks(updated)
    assert set(blocks.keys()) == {"Standalone", "Android", "iPhone"}
    assert texture_meta_matches(updated, expectation)


def test_ios_alias_matches_iphone_rule() -> None:
    meta = _minimal_meta("\n")
    expectation = _sample_expectation()
    meta = apply_texture_meta(meta, expectation)
    meta = meta.replace("buildTarget: iPhone", "buildTarget: iOS", 1)
    assert texture_meta_matches(meta, expectation)
    current = read_texture_meta_values(meta)
    assert "iOS" in current["platforms"]


def main() -> int:
    test_empty_platform_settings_list_replaced()
    test_ios_alias_matches_iphone_rule()
    print("asset_import_policy meta tests OK")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())