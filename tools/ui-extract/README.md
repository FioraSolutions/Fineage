# ui-extract

Extract Interlude `Interface.xdat` window layouts to text JSON for `l2-unity/Docs/ui-original`.

- `extract_interface_xdat.py` — parse an Interlude (ct0) `Interface.xdat` and write one JSON file per window.
- `inventory_textures.py` — map Unreal texture names in those JSON files to files already under `l2-unity/Assets`.

See `l2-unity/Docs/ui-original/README.md` for the full source list, how to decode `sysstring-e.dat`, and how the character-select lobby was recovered from `nwindow.dll`.

Do not commit `Interface.xdat`, `nwindow.dll`, decoded `.dat`, or `.utx` files.
