"""Normalize Fineage.Net.Contract sources for Unity (C# 9 / netstandard2.1).

Backend contract code may use newer C# (file-scoped namespaces, collection
expressions). Client copies under l2-unity must compile in Unity's compiler.
"""

from __future__ import annotations

import re
from pathlib import Path

FILE_SCOPED_NAMESPACE = re.compile(r"^namespace\s+([^;{]+);\s*$", re.MULTILINE)

# Property array default `= []` (C# 12 collection expression).
ARRAY_PROP_EMPTY_COLLECTION = re.compile(
    r"(public\s+(?:[\w.]+\[\]\s+\w+\s+\{\s*get;\s*set;\s*\}\s*=\s*))\[\]\s*;",
    re.MULTILINE,
)


def normalize_newlines(text: str) -> str:
    return text.replace("\r\n", "\n").replace("\r", "\n")


def _indent_body_lines(body: str) -> str:
  lines = body.split("\n")
  out: list[str] = []
  for line in lines:
      if line.strip() == "":
          out.append("")
      else:
          out.append("    " + line)
  return "\n".join(out)


def expand_file_scoped_namespace(text: str) -> str:
    """Convert `namespace X;` to a braced namespace block."""
    match = FILE_SCOPED_NAMESPACE.search(text)
    if not match:
        return text

    ns = match.group(1).strip()
    before = text[: match.start()]
    after = text[match.end() :]
    if after.startswith("\n"):
        after = after[1:]
    body = _indent_body_lines(after.rstrip("\n"))
    if body:
        return f"{before}namespace {ns}\n{{\n{body}\n}}\n"
    return f"{before}namespace {ns}\n{{\n}}\n"


def fix_empty_collection_expression_defaults(text: str) -> str:
    """`public T[] Prop { get; set; } = [];` → `System.Array.Empty<T>()`."""

    def repl(match: re.Match[str]) -> str:
        prefix = match.group(1)
        type_match = re.search(r"public\s+([\w.]+)\[\]", prefix)
        if not type_match:
            return match.group(0).replace("= [];", "= System.Array.Empty<object>();")
        elem_type = type_match.group(1)
        return f"{prefix}System.Array.Empty<{elem_type}>();"

    return ARRAY_PROP_EMPTY_COLLECTION.sub(repl, text)


COALESCE_Member_PROP = re.compile(r"m\.(\w+)\s*\?\?\s*\[\]")
METHOD_PARAM_MSG = re.compile(r"\((\w+NetMsg)\s+m\)")


def _class_body(lookup: str, class_name: str) -> str | None:
    needle = f"public sealed class {class_name}"
    idx = lookup.find(needle)
    if idx < 0:
        return None
    brace = lookup.find("{", idx)
    if brace < 0:
        return None
    depth = 0
    for i in range(brace, len(lookup)):
        ch = lookup[i]
        if ch == "{":
            depth += 1
        elif ch == "}":
            depth -= 1
            if depth == 0:
                return lookup[brace : i + 1]
    return None


def _elem_type_for_prop(prop: str, msg_type: str | None, lookup: str) -> str | None:
    scope = lookup
    if msg_type:
        body = _class_body(lookup, msg_type)
        if body:
            scope = body
    pat = re.compile(rf"public\s+([\w.]+)\[\]\s+{re.escape(prop)}\s")
    match = pat.search(scope)
    return match.group(1) if match else None


def fix_null_coalesce_empty_collection(text: str, *, lookup_corpus: str = "") -> str:
    """`m.SomeArrayProp ?? []` → `m.SomeArrayProp ?? System.Array.Empty<T>()`."""

    lookup = lookup_corpus if lookup_corpus else text
    lines = text.split("\n")
    current_msg: str | None = None

    def repl_line(line: str) -> str:
        nonlocal current_msg
        match = METHOD_PARAM_MSG.search(line)
        if match:
            current_msg = match.group(1)
        if "?? []" not in line:
            return line
        out = line
        for prop_match in COALESCE_Member_PROP.finditer(line):
            prop = prop_match.group(1)
            elem = _elem_type_for_prop(prop, current_msg, lookup)
            if elem:
                out = out.replace(f"m.{prop} ?? []", f"m.{prop} ?? System.Array.Empty<{elem}>()")
        return out

    return "\n".join(repl_line(line) for line in lines)


def transform_for_unity_cs9(text: str, *, lookup_corpus: str = "") -> str:
    text = normalize_newlines(text)
    text = expand_file_scoped_namespace(text)
    text = fix_empty_collection_expression_defaults(text)
    text = fix_null_coalesce_empty_collection(text, lookup_corpus=lookup_corpus)
    return text


def build_contract_lookup_corpus(source: Path, rel_paths: tuple[str, ...]) -> str:
    chunks: list[str] = []
    for rel in rel_paths:
        path = source / rel
        if path.is_file():
            try:
                chunks.append(path.read_text(encoding="utf-8-sig", errors="replace"))
            except OSError:
                continue
    return "\n".join(chunks)


def prepare_contract_bytes(data: bytes, *, lookup_corpus: str = "") -> bytes:
    text = data.decode("utf-8-sig")
    return transform_for_unity_cs9(text, lookup_corpus=lookup_corpus).encode("utf-8")


def scan_assets_cs10_plus(assets_root: Path, *, repo_root: Path | None = None) -> list[str]:
    """Return human-readable violations under l2-unity/Assets/**/*.cs."""
    errors: list[str] = []
    if not assets_root.is_dir():
        return errors

    repo_root = repo_root or assets_root.parent.parent

    skip_prefixes = ("Plugins/", "Plugins\\")

    for path in sorted(assets_root.rglob("*.cs")):
        try:
            rel = path.relative_to(repo_root)
        except ValueError:
            rel = path
        rel_posix = rel.as_posix()
        if rel_posix.startswith(skip_prefixes) or "/Plugins/" in rel_posix:
            continue
        try:
            text = normalize_newlines(path.read_text(encoding="utf-8-sig", errors="replace"))
        except OSError as exc:
            errors.append(f"{rel}: read failed: {exc}")
            continue

        for line_no, line in enumerate(text.split("\n"), start=1):
            stripped = line.strip()
            if stripped.startswith("//") or stripped.startswith("*") or stripped.startswith("///"):
                continue
            if re.match(r"^\s*namespace\s+[^;{]+;\s*$", line):
                errors.append(f"{rel}:{line_no}: file-scoped namespace (C# 10+)")
            if re.search(r"\{\s*get;\s*set;\s*\}\s*=\s*\[\]\s*;", line):
                errors.append(f"{rel}:{line_no}: collection expression `= []` (C# 12+)")
            if re.search(r"\?\?\s*\[\]", line):
                errors.append(f"{rel}:{line_no}: collection expression `?? []` (C# 12+)")
            if re.search(
                r"(?<![.\w])required\s+(?:public\s+|private\s+|protected\s+|internal\s+)?"
                r"[\w.<>,\[\]?]+\s+\w+\s*\{",
                line,
            ):
                errors.append(f"{rel}:{line_no}: required member (C# 11+)")
            if re.search(r"(=\s*|return\s+)\"\"\"", line):
                errors.append(f"{rel}:{line_no}: raw string literal (C# 11+)")
            if re.match(
                r"^\s*public\s+(?:sealed\s+|abstract\s+|partial\s+)?class\s+\w+\([^)]+\)\s*[:{]",
                line,
            ):
                errors.append(f"{rel}:{line_no}: primary constructor on class (C# 12+)")

    return errors