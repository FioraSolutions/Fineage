# Interlude extract

One-shot catalog of the official Interlude client: DAT tables, Unreal package indexes, PE packer notes, and skill-visual resolution.

```bash
python3 tools/interlude-extract/extract_all.py \
  --client "$HOME/Downloads/L2_Interlude_Client/Lineage II" \
  --extract-root "$HOME/Projects/Fineage-ref-extracted" \
  --docs l2-unity/Docs/interlude-ref
```

Committed output is text only (`l2-unity/Docs/interlude-ref`). Decrypted packages, PNG, and WAV stay under `--extract-root` (outside git).

| Script | Role |
|--------|------|
| `extract_all.py` | Entry point: DAT decrypt/parse, package decrypt/index, PE inspect, skill visuals, README |
| `ddf_parser.py` | l2asm DDF reader (ASCF/CNTR, UNICODE, MTX/MAT, ENBBY) |
| `decrypt.py` | Lineage2Ver413 DAT via l2encdec; Ver111/121 XOR packages |
| `package_index.py` | UE2 name/import/export index |
| `inspect_binaries.py` | PE section/entropy packer ID and string dump |
| `UScriptDump` | UELib decompiler for `.u` scripts (Interface, Engine, …) |
| `ddf/Interlude/*.ddf` | Community DAT schemas (two Interlude corrections: huntingzone `ASCF extra`, logongrp floats) |

`l2encdec` is not committed; use the Darwin binary already under `tools/interlude-dat/l2encdec-bin/`.
