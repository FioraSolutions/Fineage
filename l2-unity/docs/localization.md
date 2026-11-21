# Localization (Unity client)

Mirrors [Fineage-Backend `docs/localization.md`](https://github.com/FioraSolutions/Fineage-Backend/blob/dev/docs/localization.md). The server localizes text it composes; the client localizes UI and client-side fallbacks.

## Catalog layout

```
l2-unity/Assets/Resources/Data/l10n/locales.json     → shipped locale tags
l2-unity/Assets/Resources/Data/l10n/<locale>/strings.json
```

Keys match the backend (`chat.*`, `combat.*`, `ui.*`, `sysmsg.<id>`, `name.item.<id>` when added). English (`en`) is the only shipped locale. Add another language by:

1. Appending the tag to `locales.json`.
2. Adding `Data/l10n/<tag>/strings.json` (copy `en` and translate values; keep keys and `{0}` placeholders).
3. Adding the same tag folder on the server under `data/l10n/<tag>/` (see backend doc).

No packet or code changes are required for a new UI language.

## Lookup

`L10n.Get(key)` / `LocalizationCatalog.Get(key, locale)`:

1. Row in the active locale.
2. If missing and locale is not `en`: **warning** `event=l10n_fallback key=… locale=… fallback=en`, then English.
3. If English is missing: **error** `event=l10n_missing key=… locale=en`. The player sees the **key** (no silent fallback).

`L10n.Format(key, args…)` uses invariant `string.Format` like the server.

## Player language

Stored in `PlayerPrefs` (`opt.locale`, default `en`). Exposed in the login **Options** panel (and the in-game Options panel from the system menu).

Sent on the wire (synced with backend `Fineage.Net.Contract`):

| Packet | Tail |
|--------|------|
| `Auth` `0x0201` | After `registerIfMissing`: UTF-8 tag (`en`, `ru`, `pt-br`). Omitted only on legacy builds (`Language = null`). |
| `EnterWorld` `0x0303` | Entire payload is the tag, or empty for stored account language. |

## UI binding

- UXML: class `l10n` on `Label` / `Button` / `Toggle`; the `text` (or toggle `label`) field holds the **key** until `L10nUiBinder.Bind(root)` runs (from `L2Window.InitWindow`).
- Code-built UI: `L10nUiBinder.SetText(element, key)`.
- Window titles: `windows-name-label` with a key in `text`, or `AttributeOverrides` with `ui.window.*` keys.

## System messages

- Non-empty `SystemMessage.text` from the server is shown as sent (already localized).
- Empty `text`: client resolves `sysmsg.<InterludeId>` from `strings.json` and applies wire `Params` to `{0}` … slots (`NetSystemLines`).

## Fonts (Cyrillic / CJK)

Interlude UI uses `Data/UI/Font/tahoma` (Latin). For Cyrillic and CJK:

1. Ship a **fallback chain** on UI Toolkit text (`-unity-font-definition` or theme `FontDefinition` with fallbacks): e.g. Latin Tahoma → Noto Sans → Noto Sans CJK SC.
2. Keep one logical “game UI” style in USS; bind fallback fonts per script range so mixed chat (Latin names + localized lines) renders without tofu.
3. Do **not** commit original retail `.dat` / `.u` font tables; add open fonts under `Resources/Data/UI/Font/` and reference them in USS.
4. Nameplates / HTML windows may need separate fallbacks if they use different font assets.

Import tooling (`scripts/import-client-l10n-sysmsg.py`) can seed `sysmsg.*` rows from local `systemmsg-*.dat` under `ref/` (gitignored); output is JSON only — never commit original game binaries.

## Import from Interlude `.dat` (optional)

With `ref/l2-interlude-client/system/systemmsg-e.dat` present locally:

```bash
python3 scripts/import-client-l10n-sysmsg.py \
  --dat ref/l2-interlude-client/system/systemmsg-e.dat \
  --out l2-unity/Assets/Resources/Data/l10n/en/strings.json \
  --merge
```

Review the diff; the script writes `sysmsg.<id>` keys. Commit only the generated JSON.