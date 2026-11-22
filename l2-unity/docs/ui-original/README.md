# Original Interlude UI window layouts

These JSON files are the pixel-level layout of the retail Interlude UI, extracted so Unity work can follow the AGENTS.md rule: **original layout + original textures, pixel-perfect**. They are text only. Do not commit `Interface.xdat`, `Interface.u`, `nwindow.dll`, or `.utx` files.

## Source of truth

| Window | Retail source | Notes |
| --- | --- | --- |
| StatusWnd, TargetStatusWnd, ChatWnd, AbnormalStatusWnd, MinimapWnd, MinimapWnd_Expand, TownMapWnd, RadarContainerWnd (RadarMapWnd), RadarWnd | `system/interface.xdat` | Unencrypted Interlude xdat. Schema is acmi `xdat_editor_schema` **ct0** ([github.com/acmi/xdat_editor_schema](https://github.com/acmi/xdat_editor_schema)). |
| Character select lobby (NCLobbyWnd / Function / Start, Name/HP/MP/Exp card, Start/Create/Delete/Re-Login) | `system/nwindow.dll` | **Not in Interface.xdat.** Hardcoded in native C++. Coordinates below come from `OnCreate` / `OnPaint` in the retail Interlude DLL (see `CharacterSelect.json`). |

There is no Interlude `CharacterSelectWnd` (or `LobbyWnd`) record in `interface.xdat`. The closest xdat texture reuse is `L2UI_CH3.LobbyWnd.lobby_menuback` on `BenchMarkFunctionWnd`, which is a different window.

Button and tab labels are Interlude `sysstring-e.dat` ids (Lineage2Ver413). Chat tabs are `All`, `+Trade`, `#Party`, `@Clan`, `$Alliance` (sysstring 144, 355, 188, 128, 559). Lobby buttons are `Create` (158), `Delete` (159), `Start` (160), `Re-Login` (884).

## How to re-extract

Retail client (Mac path used here):

`~/Downloads/L2_Interlude_Client/Lineage II`

or `ref/l2-interlude-client/Lineage II` in this repo (do not commit those binaries).

```bash
# 1. Interface.xdat → window JSON
#    Interlude Interface.xdat is already plain (no Lineage2Ver header).
python3 tools/ui-extract/extract_interface_xdat.py \
  "/path/to/Lineage II/system/interface.xdat" \
  -o l2-unity/Docs/ui-original \
  --sysstring /tmp/sysstring-e.dec \
  --all

# 2. Decode sysstring-e.dat (Lineage2Ver413) first, same tool already used for skillsoundgrp:
#    tools/interlude-dat/l2encdec-bin/l2encdec-1.5.0-Darwin/l2encdec \
#      -c decode -p 413 -l "…/system/sysstring-e.dat" -o /tmp/sysstring-e.dec
#    or: npx --yes l2encdec -c decode -p 413 --legacy sysstring-e.dat sysstring-e.dec
#    Decoded sysstring is a binary table (u32 count, then id + compact-int string).
#    Do not commit the decoded file; the window JSON already has resolved labels.

# 3. Inventory which textures already exist in the Unity project
python3 tools/ui-extract/inventory_textures.py \
  --docs l2-unity/Docs/ui-original \
  --assets l2-unity/Assets \
  -o l2-unity/Docs/ui-original/textures.json
```

`CharacterSelect.json` is recovered from `nwindow.dll` (functions and evidence addresses are in that file). Re-disassemble those RVAs if a later client build is used; the Interlude retail `nwindow.dll` used here is 3,825,684 bytes, PE ImageBase `0x10000000`.

`Interface.u` is Lineage2Ver111 (XOR `0xAC` after the 28-byte header). It holds UnrealScript *behavior* (`ChatWnd`, `StatusWnd`, …), not the xdat layout.

## How to read a window JSON

- `controls[]` is a parent-first flattening of the tree. Each entry has `name`, `type`, `parent`, `x`, `y`, `width`, `height`, anchor points (`anchorThisPoint` → `anchorParentPoint`, `relativeTo`), textures, and (for text) `textAlign`, `fontType`, `fontColor`, `sysstringText`.
- `x`/`y` are the xdat `anchor` offsets, not always a top-left absolute position. Interlude places a control by lining up `anchorThisPoint` on the control with `anchorParentPoint` on `relativeTo` (or the parent window), then applying `x`/`y`.
- A **negative or zero** width/height in xdat means “stretch with the parent”: typically `parentSize + thisValue` (so width `-26` grows when the window grows). Do not treat those as 26px-wide bars.
- `wndDefPos` is the default on-screen placement (`alignment` + `x`/`y` + optional width). A value of `-9999` means “unspecified / use the window’s own size”.
- Interlude `TextBox` has an integer `fontType`, not a font name (later chronicles added `fontName`). Almost every text box uses the default `-9999`. The only exception in these windows is `StatusWnd_LevelTextBox` (`fontType` 6, color `#FFB09B79`, centered).
- `raw` is the full ct0 object (including unnamed `unk*` fields) for anything the flat view omits.

## Not extracted

Login is not in this folder. There is no `LoginWnd`, `AuthWnd`, or `NCLoginFunctionWnd` document in `docs/ui-original`, and the character-select notes from `nwindow.dll` do not include the login form. Do not guess its coordinates, size, textures, or control tree. Leave the current login screen alone until that window is extracted.

Lobby **screen placement** is not extracted either. `CharacterSelect.json` recovers the window-relative controls from `nwindow.dll` `OnCreate` / `OnPaint` (the 96×23 buttons, the 256×128 `select1_back` card, the bars). `wndDefPos` is null. The same note says the three windows (`NCLobbyFunctionWnd`, `NCLobbyStartWnd`, `NCLobbyWnd_CharInfo`) are positioned at runtime from the resolution, and those screen anchors were not recovered. Do not invent them, and do not apply the layout asset for these windows until that placement is extracted. The character-select tree stays as it is.

## Window notes

- **StatusWnd** is 176×84 at `TOP_LEFT` (0,0). Left / center / right frame slices, level box, CP/HP/MP/Exp bars (`L2UI_CH3.PlayerStatusWnd.ps_*`), `UserName`. The right-edge texture is `ps_sizecontrol1` (the Interlude resize grip). Buff icons are a separate window, **AbnormalStatusWnd** (38×26 at `TOP_LEFT` x=176), which only contains a `StatusIcon` control — icons themselves are created at runtime.
- **TargetStatusWnd** is 176×46 at `TOP_CENTER`. Frame slices use `L2UI_CH3.SmallWnd.SmallWindow_Back*`. HP/MP bars are `L2UI_CH3.SmallWnd.HpBar` / `MpBar`. Effect / info controls live in this tree (see the JSON).
- **ChatWnd** is 348×187 at `BOTTOM_LEFT`. Background is four slices (`Chatting_Back1..4`). The edit box is `ChatEditBox` at (39, −6) size 303×16, type `CHAT`, max length 100. Tabs (`ChatTabCtrl`) sit on a 320×23 strip at (23, −23) with each tab **64×23**, textures `Chatting_Tab2` (normal) / `Chatting_Tab1` (selected). The tab strip background in xdat is `L2UI_CH3.Minimap.MapWnd_back_max` (yes, a minimap texture).
- **RadarMapWnd** (`RadarContainerWnd` in xdat) is a **102×102** `Radar` control at `TOP_RIGHT` (0,0). It has **no map texture**. The engine draws the compass (oriented in the world plane). `RadarWnd` is a separate 16×16 icon stack at `TOP_RIGHT` (−10, 85) with `L2UI_CH3.Radar.icon1`–`icon9`.
- **MinimapWnd** is the world map, 334×413, `L2UI_CH3.Minimap.MapBack`, map view 328×328 at (3, 51). Bottom buttons: Current Loc., Party Member, Target Loc., Enlarge Map. Top-right: Find, World Info. `MinimapWnd_Expand` is the fullscreen variant. `TownMapWnd` is the 334×354 town map (`L2UI.MinimapWnd.MinimapBack`).
- **CharacterSelect** (lobby): window-relative geometry only. `NCLobbyFunctionWnd` 120×138 with `lobby_menuback`, buttons Create (13,20,96,23), Delete (13,45,96,23), Re-Login (13,95,96,23) using `BigButton`. `NCLobbyStartWnd` 140×49 with `lobby_startbtnback`, Start button (13,10,116,31) using `BigButton2`. The char-info card background is `select1_back` (not the in-game `myinfo_back`). HP/MP bars are 85×12 at (35,55) and (35,69); Exp bar is 206×12 at (35,83). Screen placement of these three windows is not in the extract (see Not extracted).

## Textures already in the Unity project

`textures.json` lists every referenced Unreal name, whether a file already exists under `l2-unity/Assets`, and the path(s). `textures-missing.txt` is the missing list only. Do not commit `.utx`. Import missing textures from `systextures/L2UI_CH3.utx` and `systextures/l2ui.utx` with the same names the JSON uses.

## Textures in the Unity project

`textures-map.json` lists every Unreal texture name used by these layouts and the Unity path under `l2-unity/Assets`. `textures-missing.txt` is empty when every name is present.

New Interlude UI textures were imported at the original `.utx` USize×VSize (for example `ps_hpbar` is 8×16, `BigButton` is 128×32). Some older assets in the project were cropped to a widget size; do not recrop the originals. Non-map UI uses point filtering and no mipmaps; files under `UI/Map` follow the ImportPolicy `ui-map` rule (bilinear, uncompressed RGBA32 on Standalone).

