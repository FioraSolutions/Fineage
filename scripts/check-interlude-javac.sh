#!/usr/bin/env bash
# javac the Interlude extract tools. ExportLevelHeight's pattern-variable scope
# error shipped because nothing compiled these sources. Cruma-backed tools need
# CRUMA_JAR; the vendored extractor tools always compile.
set -euo pipefail

repo_root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$repo_root"

if ! command -v javac >/dev/null 2>&1; then
  echo "check-interlude-javac: javac is not on PATH" >&2
  exit 1
fi

work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

lzo_jar="$work/lzo-core-1.0.6.jar"
curl -fsSL -o "$lzo_jar" \
  https://repo1.maven.org/maven2/org/anarres/lzo/lzo-core/1.0.6/lzo-core-1.0.6.jar

lib_sources=()
while IFS= read -r src; do
  lib_sources+=("$src")
done < <(find tools/interlude/L2TerrainExtractor/src/main/java -name '*.java' ! -name 'L2TerrainExtractor.java' | sort)

if [[ ${#lib_sources[@]} -eq 0 ]]; then
  echo "check-interlude-javac: no extractor sources" >&2
  exit 1
fi

mkdir -p "$work/classes" "$work/tools" "$work/extract"
javac --release 17 -encoding UTF-8 -d "$work/classes" -cp "$lzo_jar" "${lib_sources[@]}"

shopt -s nullglob
tool_sources=(tools/interlude/*.java)
extract_plain=()
extract_cruma=()
for src in tools/interlude-extract/*.java; do
  if grep -q 'com\.rost\.cruma' "$src"; then
    extract_cruma+=("$src")
  else
    extract_plain+=("$src")
  fi
done
shopt -u nullglob

if [[ ${#tool_sources[@]} -eq 0 ]]; then
  echo "check-interlude-javac: no tools/interlude/*.java" >&2
  exit 1
fi
javac --release 17 -encoding UTF-8 -d "$work/tools" -cp "$work/classes:$lzo_jar" "${tool_sources[@]}"

# findMip parses UTexture tails; a silent wrong offset ships bad heights. The
# synthetic layouts must all pass before any real package is touched.
java -cp "$work/classes:$work/tools" TextureTailSelfTest

if [[ ${#extract_plain[@]} -eq 0 ]]; then
  echo "check-interlude-javac: no tools/interlude-extract/*.java without Cruma" >&2
  exit 1
fi
javac --release 17 -encoding UTF-8 -d "$work/extract" -cp "$work/classes:$lzo_jar" "${extract_plain[@]}"

if [[ ${#extract_cruma[@]} -gt 0 ]]; then
  if [[ -n "${CRUMA_JAR:-}" && -f "$CRUMA_JAR" ]]; then
    javac --release 17 -encoding UTF-8 -d "$work/extract" -cp "$work/classes:$lzo_jar:$CRUMA_JAR" "${extract_cruma[@]}"
    echo "javac: cruma tools ${extract_cruma[*]}"
  else
    echo "check-interlude-javac: CRUMA_JAR unset, skipped ${#extract_cruma[@]} files that import com.rost.cruma" >&2
  fi
fi

echo "javac: tools/interlude/*.java and tools/interlude-extract (extractor classpath) OK"
