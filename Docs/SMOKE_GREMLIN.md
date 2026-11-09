# Gremlin vertical smoke (#129 / #138)

End-to-end path on Fineage Net (Interlude): enter TI → see Gremlin → equipped starter weapon → (optional) kill / die → village revive.

## Automated (Backend)

```bash
cd Fineage-Backend
dotnet run --project src/Fineage.Zone -- --selftest
```

Expect `selftest OK`. The zone acceptance handoff covers:

- Login → ZoneAuth → EnterWorld burst (`UserInfo`, `ItemList`, self `EntitySpawn`, welcome `SystemMessage`)
- Visible NPCs in the enter knownlist (includes Gremlin near Cedric / TI start)
- Starter weapon equipped in paperdoll R-hand (sword `219` / staff `177`)
- A short `MoveInput` ack

Child feature tickets: #130 datapack, #132 spawn, #133 starter equip, #134 death→respawn, #135 client equip.

## Manual (Unity client)

1. Run Zone with starter maps (local default `ti_cedric` is enough for Cedric gremlins; whole-world: `FINEAGE_ZONE_INCLUDE_ENABLED_MAPS=1`).
2. Login → create/select Human Fighter or Mystic → Enter World.
3. Confirm a **Gremlin** is visible near the TI / Cedric start.
4. Open inventory: starter weapon is **equipped** (right hand).
5. Target Gremlin, melee until death (or die to it) → use revive-to-village → land near TI restart.
6. Chat say shows in the log **and** briefly above the head (#521).

## Admin GM shop (Mobius HTML, KuK only)

Zone must run with `FINEAGE_ZONE_ADMIN=1` (default in Backend `./scripts/run-local-net.sh start`). Character name must be in Backend `data/admin-characters.json` (default **KuK**).

1. Chat `//gmshop` → Mobius `gmshops.htm` (same window as NPC HTML).
2. Jewels → Boss Jewels → buy RB jewelry (buylist `9917`, price 0).
3. Also: `//setlevel 80`, `//setclass <id>`, `//give <itemId> [count]`, `//buff` / `//mbuff` / `//fbuff` / `//buff_clear`.

## Related landings (combat playtest)

- #522 HP bar / freeze / mage SFX — closed on `dev` (#616–#618, Backend #249)
- #465 sit / walk-run / socials — closed (#620–#621)
