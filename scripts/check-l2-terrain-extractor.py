#!/usr/bin/env python3
"""CI: vendored L2TerrainExtractor must be complete and compile (fatJar)."""

from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
EXTRACTOR = REPO / "tools/interlude/L2TerrainExtractor"
CACHE_JAVA = EXTRACTOR / "src/main/java/io/github/l2terrain/cache/TerrainDataCache.java"


def main() -> int:
    if not CACHE_JAVA.is_file():
        print(f"ERROR missing {CACHE_JAVA}", file=sys.stderr)
        return 1
    ignored = subprocess.run(
        ["git", "check-ignore", "-q", str(CACHE_JAVA)],
        cwd=REPO,
        capture_output=True,
    )
    if ignored.returncode == 0:
        print(
            f"ERROR {CACHE_JAVA} is gitignored — fix .gitignore so Java cache/ sources are tracked",
            file=sys.stderr,
        )
        return 1
    gradlew = EXTRACTOR / "gradlew"
    wrapper_jar = EXTRACTOR / "gradle/wrapper/gradle-wrapper.jar"
    if gradlew.is_file() and wrapper_jar.is_file():
        cmd = [str(gradlew), "fatJar", "--no-daemon", "-q"]
    else:
        # CI installs Gradle 8.5 via gradle/actions/setup-gradle when the wrapper jar is not vendored.
        cmd = ["gradle", "fatJar", "--no-daemon", "-q"]
    r = subprocess.run(cmd, cwd=EXTRACTOR)
    if r.returncode != 0:
        print("ERROR L2TerrainExtractor fatJar failed", file=sys.stderr)
        return r.returncode
    jar_dir = EXTRACTOR / "build/libs"
    jars = list(jar_dir.glob("*-all.jar")) if jar_dir.is_dir() else []
    if not jars:
        print(f"ERROR no *-all.jar under {jar_dir}", file=sys.stderr)
        return 1
    print(f"ok L2TerrainExtractor fatJar -> {jars[0].name}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())