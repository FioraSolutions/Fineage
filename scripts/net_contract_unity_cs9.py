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

# C# 12 collection expression used as a call argument, e.g. `Foo(stream, [item], ct)`.
CALL_ARG_COLLECTION = re.compile(
    r"([,(]\s*)\[(?P<inner>[^\[\]]+)\](?=\s*[,)])"
)

USING_LINE = re.compile(r"^using\s+([\w.]+)\s*;\s*$", re.MULTILINE)

# Map identifier patterns in contract bodies to explicit `using` Unity requires (no implicit usings).
BCL_USING_RULES: tuple[tuple[re.Pattern[str], str], ...] = (
    (re.compile(r"\bReadOnlySpan\b"), "System"),
    (re.compile(r"\bSpan<"), "System"),
    (re.compile(r"\bMemory<"), "System"),
    (re.compile(r"\bArray\.Empty\b"), "System"),
    (re.compile(r"\bInvalidOperationException\b"), "System"),
    (re.compile(r"\bArgument(?:Exception|OutOfRangeException)?\b"), "System"),
    (re.compile(r"\bMath\."), "System"),
    (re.compile(r"\bBitConverter\b"), "System"),
    (re.compile(r"\bEnum\."), "System"),
    (re.compile(r"\bTask\b"), "System.Threading.Tasks"),
    (re.compile(r"\bValueTask\b"), "System.Threading.Tasks"),
    (re.compile(r"\bConfigureAwait\b"), "System.Threading.Tasks"),
    (re.compile(r"\bCancellationToken\b"), "System.Threading"),
    (re.compile(r"\bArrayPool\b"), "System.Buffers"),
    (re.compile(r"\bBinaryPrimitives\b"), "System.Buffers.Binary"),
    (re.compile(r"\bIReadOnlyList\b"), "System.Collections.Generic"),
    (re.compile(r"\bList<"), "System.Collections.Generic"),
    (re.compile(r"\bDictionary<"), "System.Collections.Generic"),
    (re.compile(r"\bIEnumerable<"), "System.Collections.Generic"),
    (re.compile(r"\bStream\b"), "System.IO"),
    (re.compile(r"\bMemoryStream\b"), "System.IO"),
    (re.compile(r"\bBinaryReader\b"), "System.IO"),
    (re.compile(r"\bBinaryWriter\b"), "System.IO"),
    (re.compile(r"\bEncoding\b"), "System.Text"),
    (re.compile(r"\bUTF8Encoding\b"), "System.Text"),
    (re.compile(r"\bStringBuilder\b"), "System.Text"),
)

USING_ORDER = (
    "System",
    "System.Buffers",
    "System.Buffers.Binary",
    "System.Collections.Generic",
    "System.IO",
    "System.Text",
    "System.Threading",
    "System.Threading.Tasks",
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


def fix_call_collection_expressions(text: str) -> str:
    """`Method(x, [item], y)` → `Method(x, new[] { item }, y)`."""

    def repl(match: re.Match[str]) -> str:
        inner = match.group("inner").strip()
        if not inner:
            return match.group(0)
        return f"{match.group(1)}new[] {{ {inner} }}"

    return CALL_ARG_COLLECTION.sub(repl, text)


def fix_null_coalesce_empty_brackets(text: str) -> str:
    """`expr ?? []` inside Encode* methods → typed `System.Array.Empty<T>()`."""
    encode_method = ""
    out: list[str] = []
    encode_types: dict[str, dict[str, str]] = {
        "EncodeEnchantSkillList": {"m.Skills": "EnchantSkillListEntryNet"},
        "EncodeEnchantSkillInfo": {"m.Requirements": "EnchantSkillRequirementNet"},
        "EncodeCursedWeaponList": {"m.ItemIds": "int"},
        "EncodeCursedWeaponStatus": {"m.Rows": "CursedWeaponStatusRowNet"},
    }

    for line in text.split("\n"):
        match = re.search(r"public static byte\[\] (Encode\w+)", line)
        if match:
            encode_method = match.group(1)
        if "?? []" in line:
            mapped = encode_types.get(encode_method, {})
            replaced = False
            for expr, elem_type in mapped.items():
                needle = f"{expr} ?? []"
                if needle in line:
                    line = line.replace(needle, f"{expr} ?? System.Array.Empty<{elem_type}>()")
                    replaced = True
                    break
            if not replaced:
                line = line.replace("?? []", "?? System.Array.Empty<object>()")
        out.append(line)
    return "\n".join(out)


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


def split_usings_and_body(text: str) -> tuple[list[str], str]:
    lines = normalize_newlines(text).split("\n")
    usings: list[str] = []
    index = 0
    while index < len(lines):
        match = USING_LINE.match(lines[index])
        if match:
            usings.append(match.group(1))
            index += 1
            continue
        if not lines[index].strip():
            if index + 1 < len(lines) and USING_LINE.match(lines[index + 1]):
                index += 1
                continue
        break
    body = "\n".join(lines[index:]).lstrip("\n")
    return usings, body


def required_bcl_usings(body: str) -> set[str]:
    needed: set[str] = set()
    for pattern, namespace in BCL_USING_RULES:
        if pattern.search(body):
            needed.add(namespace)
    return needed


def ensure_explicit_usings(text: str) -> str:
    """Emit every BCL namespace the file needs; Unity has no implicit/global usings."""
    usings, body = split_usings_and_body(text)
    merged = list(usings)
    present = set(usings)
    for namespace in required_bcl_usings(body):
        if namespace not in present:
            merged.append(namespace)
            present.add(namespace)

    if not merged:
        return body if body else text

    def sort_key(namespace: str) -> tuple[int, str]:
        try:
            return (USING_ORDER.index(namespace), namespace)
        except ValueError:
            return (len(USING_ORDER), namespace)

    block = "\n".join(f"using {ns};" for ns in sorted(set(merged), key=sort_key))
    if not body:
        return block + "\n"
    return f"{block}\n\n{body}"


def verify_explicit_usings(text: str, rel: str) -> list[str]:
    """After lowering, unqualified BCL types must resolve via explicit usings."""
    usings, body = split_usings_and_body(text)
    present = set(usings)
    errors: list[str] = []
    for pattern, namespace in BCL_USING_RULES:
        if not pattern.search(body):
            continue
        if namespace in present:
            continue
        fq_prefix = namespace + "."
        if re.search(rf"\b{re.escape(namespace)}\.", body):
            continue
        errors.append(
            f"{rel}: uses types from `{namespace}` but has no `using {namespace};` "
            "(Unity C# 9 has no implicit usings)"
        )
        break
    return errors


def transform_for_unity_cs9(text: str, *, lookup_corpus: str = "") -> str:
    text = normalize_newlines(text)
    text = expand_file_scoped_namespace(text)
    text = fix_empty_collection_expression_defaults(text)
    text = fix_null_coalesce_empty_collection(text, lookup_corpus=lookup_corpus)
    text = fix_null_coalesce_empty_brackets(text)
    text = fix_call_collection_expressions(text)
    text = ensure_explicit_usings(text)
    return text


def build_contract_lookup_corpus(texts) -> str:
    """Join the contract sources the sync is actually reading.

    Callers must pass the same bytes as ``read_source`` (the git ref when one
    is set). Reading the working tree here typed ``?? []`` as ``object``
    whenever that tree was behind the ref.
    """
    return "\n".join(text for text in texts if text)


def scan_source_cs10_plus_lines(text: str, rel: str) -> list[str]:
    """Violations that remain after ``transform_for_unity_cs9`` (sync must fail loudly)."""
    errors: list[str] = []
    for line_no, line in enumerate(normalize_newlines(text).split("\n"), start=1):
        stripped = line.strip()
        if stripped.startswith("//") or stripped.startswith("*") or stripped.startswith("///"):
            continue
        if re.match(r"^\s*namespace\s+[^;{]+;\s*$", line):
            errors.append(f"{rel}:{line_no}: file-scoped namespace (C# 10+)")
        if re.search(r"\{\s*get;\s*set;\s*\}\s*=\s*\[\]\s*;", line):
            errors.append(f"{rel}:{line_no}: collection expression `= []` (C# 12+)")
        if re.search(r"\?\?\s*\[\]", line):
            errors.append(f"{rel}:{line_no}: collection expression `?? []` (C# 12+)")
        if CALL_ARG_COLLECTION.search(line) and "new[]" not in line:
            errors.append(f"{rel}:{line_no}: collection expression call argument `[...]` (C# 12+)")
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


def prepare_contract_bytes(data: bytes, *, lookup_corpus: str = "", rel: str = "") -> bytes:
    text = data.decode("utf-8-sig")
    text = transform_for_unity_cs9(text, lookup_corpus=lookup_corpus)
    label = rel or "<contract>"
    violations = scan_source_cs10_plus_lines(text, label)
    violations.extend(verify_explicit_usings(text, label))
    if violations:
        raise ValueError(
            "backend Net contract cannot be lowered to Unity C# 9 "
            f"({label}):\n  " + "\n  ".join(violations)
        )
    return text.encode("utf-8")


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