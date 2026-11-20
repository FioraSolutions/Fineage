"""Shared texture/audio import policy for l2-unity (.meta YAML + rules JSON)."""

from __future__ import annotations

import fnmatch
import json
import re
import subprocess
from dataclasses import dataclass
from pathlib import Path
from typing import Any

REPO_ROOT = Path(__file__).resolve().parents[2]
L2_UNITY = REPO_ROOT / "l2-unity"
RULES_PATH = L2_UNITY / "ImportPolicy" / "asset-import-rules.json"

TEXTURE_EXTENSIONS = {".png", ".jpg", ".jpeg", ".tga", ".psd", ".bmp", ".exr", ".gif"}
AUDIO_EXTENSIONS = {".wav", ".ogg", ".mp3", ".aiff", ".aif"}

TEXTURE_FORMAT_ID: dict[str, int] = {
    "RGBA32": 4,
    "DXT1": 10,
    "DXT5": 12,
    "BC4": 26,
    "BC5": 27,
    "BC6H": 24,
    "BC7": 25,
    "ASTC_4x4": 48,
    "ASTC_5x5": 49,
    "ASTC_6x6": 50,
    "ASTC_8x8": 51,
    "ETC2_RGBA8": 47,
}

TEXTURE_COMPRESSION_ID: dict[str, int] = {
    "Uncompressed": 0,
    "Compressed": 1,
    "CompressedHQ": 2,
}

FILTER_MODE_ID = {"Point": 0, "Bilinear": 1, "Trilinear": 2}
WRAP_MODE_ID = {"Repeat": 0, "Clamp": 1, "Mirror": 2, "MirrorOnce": 3}
NPOT_SCALE_ID = {"None": 0, "ToNearest": 1, "ToLarger": 2, "ToSmaller": 3}

AUDIO_LOAD_TYPE_ID = {
    "DecompressOnLoad": 0,
    "CompressedInMemory": 1,
    "Streaming": 2,
}

AUDIO_COMPRESSION_ID = {
    "PCM": 0,
    "Vorbis": 1,
    "ADPCM": 2,
}


@dataclass(frozen=True)
class TextureExpectation:
    rule_id: str
    readable: bool
    mipmaps: bool
    streaming_mipmaps: bool
    filter_mode: int | None
    npot_scale: int | None
    wrap_mode: int | None
    platforms: dict[str, dict[str, Any]]


@dataclass(frozen=True)
class AudioExpectation:
    rule_id: str
    load_type: int
    quality: float
    compression_format: int
    load_in_background: bool
    preload_audio_data: bool = False


def load_rules() -> dict[str, Any]:
    with RULES_PATH.open(encoding="utf-8") as handle:
        return json.load(handle)


def posix_path(path: Path) -> str:
    rel = path.relative_to(L2_UNITY).as_posix()
    if rel.endswith(".meta"):
        rel = rel[: -len(".meta")]
    return rel


def match_globs(asset_path: str, path_globs: list[str], filename_globs: list[str] | None) -> bool:
    if not any(fnmatch.fnmatch(asset_path, pattern) for pattern in path_globs):
        return False
    if filename_globs:
        name = asset_path.rsplit("/", 1)[-1]
        if not any(fnmatch.fnmatch(name, pattern) for pattern in filename_globs):
            return False
    return True


def resolve_texture_rule(asset_path: str, rules: dict[str, Any]) -> dict[str, Any] | None:
    for rule in rules["textureRules"]:
        if match_globs(asset_path, rule["pathGlobs"], rule.get("filenameGlobs")):
            return rule
    return None


def resolve_audio_rule(asset_path: str, rules: dict[str, Any]) -> dict[str, Any]:
    for rule in rules["audioRules"]:
        if match_globs(asset_path, rule["pathGlobs"], rule.get("filenameGlobs")):
            return rule
    raise ValueError(f"No audio import rule for asset: {asset_path}")


def _wav_duration_seconds(audio_path: Path) -> float | None:
    try:
        import wave

        with wave.open(str(audio_path), "rb") as handle:
            rate = handle.getframerate()
            frames = handle.getnframes()
            if rate <= 0:
                return None
            return frames / float(rate)
    except (wave.Error, OSError, ValueError):
        return None


def audio_duration_seconds(audio_path: Path) -> float | None:
    if not audio_path.is_file():
        return None
    ext = audio_path.suffix.lower()
    if ext == ".wav":
        duration = _wav_duration_seconds(audio_path)
        if duration is not None:
            return duration
    try:
        result = subprocess.run(
            [
                "ffprobe",
                "-v",
                "error",
                "-show_entries",
                "format=duration",
                "-of",
                "default=noprint_wrappers=1:nokey=1",
                str(audio_path),
            ],
            check=True,
            capture_output=True,
            text=True,
        )
        return float(result.stdout.strip())
    except (OSError, subprocess.CalledProcessError, ValueError):
        return None


def build_audio_expectation(asset_path: str, rules: dict[str, Any], audio_path: Path) -> AudioExpectation:
    rule = resolve_audio_rule(asset_path, rules)
    rule_id = rule["id"]
    load_name = rule["loadType"]
    quality = float(rule["quality"])
    compression = AUDIO_COMPRESSION_ID[rule["compressionFormat"]]
    load_bg = bool(rule.get("loadInBackground", False))

    overrides = rule.get("durationOverrides")
    if overrides:
        duration = audio_duration_seconds(audio_path)
        if duration is not None:
            stream_min = float(overrides["streamingMinSeconds"])
            mem_min = float(overrides["compressedInMemoryMinSeconds"])
            if duration >= stream_min:
                load_name = "Streaming"
                quality = float(overrides["streamingQuality"])
                load_bg = True
            elif duration >= mem_min:
                load_name = "CompressedInMemory"
                quality = float(overrides["compressedInMemoryQuality"])
                load_bg = False

    return AudioExpectation(
        rule_id=rule_id,
        load_type=AUDIO_LOAD_TYPE_ID[load_name],
        quality=quality,
        compression_format=compression,
        load_in_background=load_bg,
    )


def build_texture_expectation(asset_path: str, rules: dict[str, Any]) -> TextureExpectation | None:
    rule = resolve_texture_rule(asset_path, rules)
    if rule is None:
        return None
    return TextureExpectation(
        rule_id=rule["id"],
        readable=bool(rule["readable"]),
        mipmaps=bool(rule["mipmaps"]),
        streaming_mipmaps=bool(rule["streamingMipmaps"]),
        filter_mode=FILTER_MODE_ID.get(rule["filterMode"]) if rule.get("filterMode") else None,
        npot_scale=NPOT_SCALE_ID.get(rule["npotScale"]) if rule.get("npotScale") else None,
        wrap_mode=WRAP_MODE_ID.get(rule["wrapMode"]) if rule.get("wrapMode") else None,
        platforms={entry["name"]: entry for entry in rule["platforms"]},
    )


def _set_yaml_scalar(text: str, key: str, value: int | float, parent_indent: str = "  ") -> str:
    pattern = re.compile(rf"^{re.escape(parent_indent)}{re.escape(key)}: .*$", re.MULTILINE)
    replacement = f"{parent_indent}{key}: {value}"
    if pattern.search(text):
        return pattern.sub(replacement, text, count=1)
    # Insert after TextureImporter: line when missing (rare).
    anchor = re.search(r"^TextureImporter:\s*$", text, re.MULTILINE)
    if anchor:
        idx = anchor.end()
        return text[:idx] + f"\n{replacement}" + text[idx:]
    return text


def _parse_platform_blocks(text: str) -> dict[str, tuple[int, int]]:
    """Return buildTarget -> (start, end) line indexes of each platform block."""
    lines = text.splitlines(keepends=True)
    blocks: dict[str, tuple[int, int]] = {}
    for i, line in enumerate(lines):
        if line.strip() != "platformSettings:":
            continue
        j = i + 1
        while j < len(lines) and (lines[j].startswith("  -") or lines[j].startswith("    ")):
            if not lines[j].startswith("  - serializedVersion:"):
                j += 1
                continue
            start = j
            target: str | None = None
            j += 1
            while j < len(lines) and lines[j].startswith("    "):
                stripped = lines[j].strip()
                if stripped.startswith("buildTarget:"):
                    target = stripped.split(":", 1)[1].strip()
                j += 1
            if target is not None:
                blocks[target] = (start, j)
        break
    return blocks


def _render_platform_block(target: str, settings: dict[str, Any]) -> str:
    fmt = TEXTURE_FORMAT_ID[settings["format"]]
    compression = TEXTURE_COMPRESSION_ID[settings["compression"]]
    max_size = int(settings.get("maxTextureSize", 2048))
    overridden = 1 if settings.get("overridden", True) else 0
    return (
        "  - serializedVersion: 4\n"
        f"    buildTarget: {target}\n"
        f"    maxTextureSize: {max_size}\n"
        "    resizeAlgorithm: 0\n"
        f"    textureFormat: {fmt}\n"
        f"    textureCompression: {compression}\n"
        "    compressionQuality: 50\n"
        "    crunchedCompression: 0\n"
        "    allowsAlphaSplitting: 0\n"
        f"    overridden: {overridden}\n"
        "    ignorePlatformSupport: 0\n"
        "    androidETC2FallbackOverride: 0\n"
        "    forceMaximumCompressionQuality_BC6H_BC7: 0\n"
    )


def apply_texture_meta(text: str, expectation: TextureExpectation) -> str:
    text = _set_yaml_scalar(text, "isReadable", 1 if expectation.readable else 0)
    text = _set_yaml_scalar(text, "streamingMipmaps", 1 if expectation.streaming_mipmaps else 0, parent_indent="  ")
    text = _set_yaml_scalar(
        text, "enableMipMap", 1 if expectation.mipmaps else 0, parent_indent="    "
    )
    if expectation.filter_mode is not None:
        text = _set_yaml_scalar(text, "filterMode", expectation.filter_mode, parent_indent="    ")
    if expectation.npot_scale is not None:
        text = _set_yaml_scalar(text, "nPOTScale", expectation.npot_scale)
    if expectation.wrap_mode is not None:
        text = _set_yaml_scalar(text, "wrapU", expectation.wrap_mode, parent_indent="    ")
        text = _set_yaml_scalar(text, "wrapV", expectation.wrap_mode, parent_indent="    ")
        text = _set_yaml_scalar(text, "wrapW", expectation.wrap_mode, parent_indent="    ")

    lines = text.splitlines(keepends=True)
    blocks = _parse_platform_blocks(text)
    # Apply from the bottom up so indexes stay valid when replacing ranges.
    for target, settings in sorted(
        expectation.platforms.items(),
        key=lambda item: blocks.get(item[0], (0, 0))[0],
        reverse=True,
    ):
        new_block = _render_platform_block(target, settings)
        if target in blocks:
            start, end = blocks[target]
            lines[start:end] = [new_block]
            continue

        insert_at = None
        for idx, line in enumerate(lines):
            if line.startswith("  platformSettings:"):
                insert_at = idx + 1
                break
        if insert_at is None:
            raise ValueError("TextureImporter platformSettings section not found")
        lines.insert(insert_at, new_block)
    return "".join(lines)


def apply_audio_meta(text: str, expectation: AudioExpectation) -> str:
    text = _set_yaml_scalar(text, "loadType", expectation.load_type, parent_indent="    ")
    text = _set_yaml_scalar(text, "quality", expectation.quality, parent_indent="    ")
    text = _set_yaml_scalar(text, "compressionFormat", expectation.compression_format, parent_indent="    ")
    text = _set_yaml_scalar(text, "preloadAudioData", 0, parent_indent="    ")
    text = _set_yaml_scalar(text, "loadInBackground", 1 if expectation.load_in_background else 0)
    return text


def read_texture_meta_values(text: str) -> dict[str, Any]:
    readable_m = re.search(r"^  isReadable: (\d+)\s*$", text, re.MULTILINE)
    stream_m = re.search(r"^  streamingMipmaps: (\d+)\s*$", text, re.MULTILINE)
    mip_m = re.search(r"^    enableMipMap: (\d+)\s*$", text, re.MULTILINE)
    platforms: dict[str, dict[str, int]] = {}
    for target, (start, end) in _parse_platform_blocks(text).items():
        block = "".join(text.splitlines(keepends=True)[start:end])
        platforms[target] = {
            "textureFormat": int(re.search(r"textureFormat: (\-?\d+)", block).group(1)),
            "textureCompression": int(re.search(r"textureCompression: (\d+)", block).group(1)),
            "overridden": int(re.search(r"overridden: (\d+)", block).group(1)),
        }
    return {
        "isReadable": int(readable_m.group(1)) if readable_m else None,
        "streamingMipmaps": int(stream_m.group(1)) if stream_m else None,
        "enableMipMap": int(mip_m.group(1)) if mip_m else None,
        "platforms": platforms,
    }


def read_audio_meta_values(text: str) -> dict[str, Any]:
    load_m = re.search(r"^    loadType: (\d+)\s*$", text, re.MULTILINE)
    quality_m = re.search(r"^    quality: ([0-9.]+)\s*$", text, re.MULTILINE)
    fmt_m = re.search(r"^    compressionFormat: (\d+)\s*$", text, re.MULTILINE)
    bg_m = re.search(r"^  loadInBackground: (\d+)\s*$", text, re.MULTILINE)
    return {
        "loadType": int(load_m.group(1)) if load_m else None,
        "quality": float(quality_m.group(1)) if quality_m else None,
        "compressionFormat": int(fmt_m.group(1)) if fmt_m else None,
        "loadInBackground": int(bg_m.group(1)) if bg_m else None,
    }


def texture_meta_matches(text: str, expectation: TextureExpectation) -> bool:
    current = read_texture_meta_values(text)
    if current["isReadable"] != (1 if expectation.readable else 0):
        return False
    if current["streamingMipmaps"] != (1 if expectation.streaming_mipmaps else 0):
        return False
    if current["enableMipMap"] != (1 if expectation.mipmaps else 0):
        return False
    for target, settings in expectation.platforms.items():
        plat = current["platforms"].get(target)
        if plat is None:
            return False
        if plat["textureFormat"] != TEXTURE_FORMAT_ID[settings["format"]]:
            return False
        if plat["textureCompression"] != TEXTURE_COMPRESSION_ID[settings["compression"]]:
            return False
        if plat["overridden"] != (1 if settings.get("overridden", True) else 0):
            return False
    return True


def audio_meta_matches(text: str, expectation: AudioExpectation) -> bool:
    current = read_audio_meta_values(text)
    return (
        current["loadType"] == expectation.load_type
        and abs(current["quality"] - expectation.quality) < 0.001
        and current["compressionFormat"] == expectation.compression_format
        and current["loadInBackground"] == (1 if expectation.load_in_background else 0)
    )


def iter_policy_texture_paths() -> list[Path]:
    assets = L2_UNITY / "Assets"
    paths: list[Path] = []
    for path in assets.rglob("*"):
        if path.suffix.lower() in TEXTURE_EXTENSIONS and path.is_file():
            paths.append(path)
    # Embedded runtime-readable texture asset.
    embedded = (
        assets
        / "Resources/Data/StaticMeshes/LineageEffectsStaticmeshes/sh2_frames/sh2_frame_positions.asset"
    )
    if embedded.is_file():
        paths.append(embedded)
    return paths


def iter_policy_audio_paths() -> list[Path]:
    sounds = L2_UNITY / "Assets/Resources/Data/Sounds"
    return [p for p in sounds.rglob("*") if p.suffix.lower() in AUDIO_EXTENSIONS and p.is_file()]