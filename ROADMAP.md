# Fineage roadmap

Живая карта: что уже есть, что дальше, как резать на параллельные задачи.
Доска: [FioraSolutions/Fineage Project #7](https://github.com/orgs/FioraSolutions/projects/7)  
Issues + milestones в репо — источник правды для агентов/людей.

## Как пользоваться

1. Бери issue с доски → Status **In Progress** → ветка `feat/<issue-slug>` (лучше отдельный worktree).
2. Одна issue ≈ один агент / один PR в `dev`.
3. Версии (`v0.x`) = milestone: закрываем milestone, когда Done все его issues (или сознательно выкинули).

## Сделано (baseline)

| Что | Где |
|-----|-----|
| Unity **6000.3.27f1** + URP 17.3 | `main` / `dev` |
| Login stub → CharSelect → EnterWorld → run | `tools/login-stub` |
| Fix run-in-place без UserInfo | `dev` |
| Portable diag paths | `L2DiagPaths` |

В работе / на ветках (ещё не обязательно в `dev`):

| Что | Ветка / issue |
|-----|----------------|
| Domain facade над Interlude | `feat/domain-wire-facade` → #3 |
| ZoneServer v2 skeleton (MessagePack) | `feat/zone-server-skeleton` → #4 |

---

## Версии

### v0.1 — Playable stub ✅

Цель: локально зайти и побегать на TI.

- [x] Login + game stub
- [x] Enter world + move echo
- [x] Unity 6.3 LTS

### v0.2 — Domain wire

Цель: клиентский мир не знает Interlude opcodes — только domain events. Interlude = adapter.

| Параллель | Задачи |
|-----------|--------|
| A | Domain events + sink (#3) |
| B | Interlude mappers Move/Stop/Knownlist/UserInfo |
| C | Client feature-flag `WireBackend=Interlude\|V2` |
| D | Docs: как добавить новый domain event |

### v0.3 — Own server MVP

Цель: свой C# login+zone без L2J; клиент ходит на v2 (хотя бы enter+move).

| Параллель | Задачи |
|-----------|--------|
| A | ZoneServer skeleton (#4) → merge |
| B | LoginServer v2 (auth + session key) |
| C | Client V2 codec (MessagePack/protobuf) |
| D | Smoke script: stub vs zoneserver |
| E | TLS / length-prefix framing |

### v0.4 — World presence

Цель: не пустой мир — другие сущности, статы, knownlist.

| Параллель | Задачи |
|-----------|--------|
| A | UserInfo / StatsChanged end-to-end |
| B | CharInfo spawn (other players) |
| C | NpcInfo spawn (static NPCs TI) |
| D | Server AOI grid (simple) |
| E | DeleteObject / leave knownlist |

### v0.5 — Combat loop

Цель: ударить моба / скилл / смерть / рес.

| Параллель | Задачи |
|-----------|--------|
| A | Attack + AutoAttack packets → domain |
| B | Server damage / HP sync |
| C | Die / Revive |
| D | MagicSkillUse minimal |
| E | Target select |

### v0.6 — Inventory & social UI

| Параллель | Задачи |
|-----------|--------|
| A | Inventory list + equip |
| B | Drop / pickup (есть старые ветки — сверить) |
| C | Chat say / system messages |
| D | Party/trade (есть `feat/party-and-trade` — audit) |

### v0.7 — Scale & harden

| Параллель | Задачи |
|-----------|--------|
| A | Tick loop + interest management |
| B | Client queue backpressure / buffer pool |
| C | Observability (metrics, structured logs) |
| D | CI: client compile + zoneserver selftest |
| E | Load smoke (N fake clients) |

---

## Зависимости (не параллелить слепо)

```
v0.1 ✅
  └─ v0.2 Domain ──┬─► v0.3 ZoneServer + client V2
                   └─► (Interlude stub ещё жив для регрессии)
                         └─► v0.4 Presence ─► v0.5 Combat ─► v0.6 UI
                                              └─► v0.7 Scale (можно куски раньше)
```

**Жёстко:** v0.3 client V2 опирается на v0.2 domain.  
**Мягко:** server skeleton (#4) можно мержить параллельно с facade, клиентский dual-adapter — после facade.
