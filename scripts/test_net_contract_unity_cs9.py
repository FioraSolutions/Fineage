#!/usr/bin/env python3
"""Regression tests for backend→Unity C# 9 contract lowering."""

from __future__ import annotations

import sys
from pathlib import Path

_REPO = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(_REPO / "scripts"))

from net_contract_unity_cs9 import (  # noqa: E402
    fix_call_collection_expressions,
    prepare_contract_bytes,
    transform_for_unity_cs9,
)


def test_call_collection_expression_lowering() -> None:
    src = (
        "await WriteManyAsync(stream, [envelope], ct).ConfigureAwait(false);"
    )
    out = fix_call_collection_expressions(src)
    assert "new[] { envelope }" in out
    assert "[envelope]" not in out


def test_net_framing_write_async_roundtrip() -> None:
    backend = (
        "namespace Fineage.Net.Contract;\n"
        "public static class NetFraming\n"
        "{\n"
        "    public static async Task WriteAsync(Stream stream, PacketEnvelope envelope, CancellationToken ct)\n"
        "    {\n"
        "        await WriteManyAsync(stream, [envelope], ct).ConfigureAwait(false);\n"
        "    }\n"
        "}\n"
    )
    out = prepare_contract_bytes(
        backend.encode("utf-8"),
        lookup_corpus=backend,
        rel="Envelope/NetFraming.cs",
    ).decode("utf-8")
    assert "new[] { envelope }" in out
    assert "[envelope]" not in out


def main() -> int:
    test_call_collection_expression_lowering()
    test_net_framing_write_async_roundtrip()
    print("net_contract_unity_cs9 tests OK")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())