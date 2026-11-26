# MinimapWnd game time HUD (original rects)

Source: `docs/ui-original/MinimapWnd.json` (lossless xdat extract).

| Control | x | y | w | h | Texture / notes |
|---------|---|---|---|---|-----------------|
| `texSun` | 7 | 57 | 25 | 25 | `L2UI_CH3.Minimap.Map_Sun` |
| `texMoon` | 7 | 57 | 25 | 25 | `L2UI_CH3.Minimap.Map_Moon` (same anchor as sun) |
| `txtGameTime` | 0 | 12 | 10×12 autosize | anchored TOP_RIGHT to `texSun` | `#FFDCDCDC`, Interlude AM/PM `H : MM` |

Fineage places these via `InterludeWindowLayout.Apply(_windowEle, "MinimapWnd")` on `MapWindow` (`MapWindow.uxml` root `MinimapWnd`).
Sun vs moon visibility follows `MinimapWnd.uc` `SelectSunOrMoon` (day 06:00–23:59).