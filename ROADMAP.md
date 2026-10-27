# Fineage roadmap

Живая карта: протокол/движок (Phase A) → контентные системы Interlude (Phase B+) → endgame.
Доска: [FioraSolutions/Fineage Project #7](https://github.com/orgs/FioraSolutions/projects/7)  
Issues + milestones в репо — источник правды для агентов/людей.

## Как агенты берут работу

1. Бери **открытый** issue со Status **Todo** → поставь **In Progress** → ветка `feat/<issue-number>-short-slug` (лучше отдельный worktree).
2. **Одна issue ≈ один PR** в `dev`. Не смешивай epic-скоуп в одном PR.
3. Читай **Depends on** / связанные epics в комментариях. Не стартуй контент-рантайм до schema/importer из `v0.8`.
4. Milestone (`v0.x` или named) закрываем, когда Done все его issues (или сознательно выкинули / перенесли).
5. «Все квесты / все мобы / все скиллы» = **пайплайн импорта + рантайм**, а не тысяча hand-authored issues. Epic описывает поверхность; child issues — первые конкретные PR.

---

## Sources of truth (что откуда брать)

Полного исходника клиента L2 **в репо нет**. Слои:

| Слой | Где | Что брать |
|------|-----|-----------|
| **Клиентские таблицы / grp** | `l2-unity/Assets/Scripts/Database/*Table.cs`, StreamingAssets Meta, UI в `Resources/Data/UI/` | Имена, иконки, skillgrp/npcgrp/itemname, quest name — отображение на клиенте |
| **Карты / геодата** | `StreamingAssets/Data/Maps/` (+ Geodata `16_24`…`17_25`), `Resources/Data/Maps/` | TI sample сейчас; остальные локации — импорт карт/гео, не выдумывать |
| **VFX / emitters** | `.uc` в `Resources/Data/Effects/`, формулы в `…/Decompile_Common/` | Визуал скиллов; чинить математику в Common, не хаками в эффектах. Гайд: `l2-unity/docs/metrics/L2_UC_TO_UNITY_AI_GUIDE.md` |
| **Сеть / opcodes Interlude** | `Assets/Scripts/Networking/` (+ Guide-L2Unity) | Поведение пакетов; ориентация на Acis/Mobius Interlude wire |
| **Серверная бизнес-логика** | **Не в git.** Ориентир: L2J **Acis / Mobius Interlude** datapack (skills XML, npc XML, HTML quests, items, multisell, spawns) — подключать как **внешний SoT** / submodule / `External/` | Квесты, AI мобов, формулы боя, Olympiad, Seven Seals, RB, clan/castle |
| **Внешние дампы автора** | `External/decompile` или env `L2_DECOMPILE_ROOT` (см. `.gitignore` → `l2-unity/External/`), diag: `logs/diag/` | Frida/move logs, umodel, bridge extract — **сейчас в рабочей копии отсутствует** |
| **Свои stubs** | `tools/login-stub`, `tools/zone-server` | Пока нет datapack-рантайма — минимальные hardcoded ответы |

**Правило:** не копировать тысячи XML в issues. Импортёр читает datapack → нормализованный Fineage schema → ZoneServer runtime. Клиентские grp/UI уже частично есть — сервер должен отдавать id, которые клиент умеет резолвить.

**Сейчас отсутствует в репо (нужно принести локально / submodule):**
- L2J/Mobius/Acis Interlude **datapack** (quests HTML, skills, npc, items, spawns)
- Полный **decompile** tree (`L2_DECOMPILE_ROOT` / `External/decompile`)
- Полный набор **карт** за пределами TI (`16_24`–`17_25`)

---

## Сделано (baseline)

| Что | Где |
|-----|-----|
| Unity **6000.3.27f1** + URP 17.3 | `main` / `dev` |
| Login stub → CharSelect → EnterWorld → run | `tools/login-stub` |
| Domain facade + Zone V2 client adapter | `dev` (PR #39 и др.) |
| ZoneServer v2 skeleton (MessagePack enter/move) | `tools/zone-server` |
| Portable diag paths | `L2DiagPaths` |
| ROADMAP Phase A (v0.1–v0.7) | merged via PR #37 на `main` |

---

## Phase A — Protocol / engine MVP (v0.1–v0.7)

Сохраняем как есть. Issues уже на доске Project #7.

### Матрица (Client / ZoneServer / Datapack / Tools)

| Ver | Client | ZoneServer | Datapack-content | Tools |
|-----|--------|------------|------------------|-------|
| **v0.1** ✅ Playable stub | Unity 6.3, enter+move | — (login-stub) | — | login-stub |
| **v0.2** Domain wire | Domain sink, Interlude mappers, WireBackend flag | — | — | docs domain event |
| **v0.3** Own server MVP | V2 codec enter+move | Zone skeleton, LoginServer v2, framing | — | smoke stub vs zone |
| **v0.4** World presence | CharInfo/UserInfo apply | NpcInfo sample, AOI, DeleteObject | static TI NPC ids (hardcode ok) | — |
| **v0.5** Combat loop | Attack/skill/target domain | melee HP, die/revive, 1 skill | 1 sample skill id | — |
| **v0.6** Inventory & social | inventory/equip UI, chat | item list packets | sample items | audit old branches |
| **v0.7** Scale & harden | RX backpressure | tick, metrics | — | CI, load smoke |

### Зависимости Phase A

```
v0.1 ✅
  └─ v0.2 Domain ──┬─► v0.3 ZoneServer + client V2
                   └─► (Interlude stub для регрессии)
                         └─► v0.4 Presence ─► v0.5 Combat ─► v0.6 UI
                                              └─► v0.7 Scale (куски раньше ok)
```

**Жёстко:** client V2 опирается на domain.  
**Мягко:** Phase B schema/importers можно начинать **параллельно** с v0.5–v0.7 (не блокирует combat MVP), но **runtime** контента ждёт стабильный ZoneServer + presence.

---

## Phase B+ — Content systems & game features

Цель: Interlude-поверхность (расы, скиллы, мобы, квесты, локации, оружие/энчант, Seven Seals, Olympiad, RB, clan/castle) через **пайплайны**, не ручной перенос каждой строки.

### Порядок фаз (dependency order)

```
Phase A (v0.2–v0.5 минимум)
    │
    ▼
v0.8  Content pipeline      ← schema + import + Zone datapack loader (FOUNDATION)
    │
    ├─► v0.9  Races & classes (+ subclasses later in same milestone)
    ├─► v0.10 Skills system
    ├─► v0.11 NPC & mobs
    ├─► v0.12 Items, weapons & enchant
    └─► v0.13 Locations & maps
            │
            ├─► v0.14 Quests engine        (нужны NPC + items + skills hooks)
            ├─► v0.17 Raid bosses          (нужны mobs + combat + loot)
            │
            ├─► v0.15 Seven Seals          (после quests/NPC hooks + seal state)
            ├─► v0.16 Olympiad             (после combat-complete + arenas)
            └─► v0.18 Clan & castle        (после presence + items + locations)
```

**Реалистичный staging (не «всё сразу»):**
1. Schema + один importer + smoke load **до** «all quests».
2. Одна раса/класс path (Human Fighter → Warrior) **до** all races.
3. Один sample RB **до** all RBs.
4. Один HTML-quest end-to-end **до** bulk import всех quests.
5. Enchant +0→+1 на одном weapon **до** full scroll matrix.

### Матрица Phase B+

| Milestone | Client | ZoneServer | Datapack-content | Tools |
|-----------|--------|------------|------------------|-------|
| **v0.8 Content pipeline** | id resolve против grp tables | DatapackRuntime load API | Fineage schema; import from L2J XML/HTML | `tools/datapack-import`, validate CI |
| **v0.9 Races & classes** | char create UI all races; subclass UI later | create/template, class change, subclass | char_templates, class trees | sample template dump |
| **v0.10 Skills** | SkillList/Learn UI, cast VFX bind | learn/use, effects, reuse | skills XML → schema | skill import + effect bind smoke |
| **v0.11 NPC & mobs** | NpcInfo/model spawn | spawn, AI stub→aggro, drops hook | npc XML + spawns | npc import; TI then all |
| **v0.12 Items & enchant** | Inventory/EnchantWindow | give/equip/enchant formulas | items/weapons XML | item import; enchant smoke |
| **v0.13 Locations & maps** | load extra map tiles | zone bounds, teleport, geodata move | map list + geodata packs | map/geodata import pipeline |
| **v0.14 Quests** | QuestList/HTML UI | quest state machine, rewards | HTML + quest scripts import | quest import; 1 E2E then bulk |
| **v0.15 Seven Seals** | seal UI / period info packets | seal periods, catacomb/necropolis access | seal schedule + related NPC/quests | period sim / admin |
| **v0.16 Olympiad** | olympiad UI / observe | stadiums, matches, points | olympiad config + NPC | match smoke 2 clients |
| **v0.17 Raid bosses** | RB nameplate / epic window if any | RB AI, windows, loot, respawn | RB definitions + spoil | 1 RB then bulk |
| **v0.18 Clan & castle** | Clan UI (уже заготовки) | clan create, pledge, siege loop | clan halls / castles datapack | siege smoke |

---

## Версии (детально)

### Phase A (кратко — без удаления прогресса)

#### v0.1 — Playable stub ✅
Локально зайти и побегать на TI. Login+game stub, enter+move, Unity 6.3.

#### v0.2 — Domain wire
Клиентский мир не знает opcodes — domain events; Interlude = adapter.

#### v0.3 — Own server MVP
Свой C# login+zone; клиент V2 enter+move.

#### v0.4 — World presence
UserInfo, CharInfo/NpcInfo, AOI, DeleteObject.

#### v0.5 — Combat loop
Attack, 1 skill, die/revive, target.

#### v0.6 — Inventory & social UI
Inventory/equip, drop/pickup audit, chat, party/trade audit.

#### v0.7 — Scale & harden
Tick, backpressure, metrics, CI, load smoke.

---

### Phase B+

#### v0.8 — Content pipeline
**Цель:** единый schema + import из L2J/Mobius datapack + загрузка в ZoneServer. Без этого «все квесты/мобы» не стартуют.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Tools | Repo layout `datapack/` или `External/l2j-datapack` + README SoT |
| B | Datapack | JSON/MessagePack schema: npc, skill, item, spawn, quest stub |
| C | Tools | Importer MVP: 1 npc + 1 skill + 1 item из XML |
| D | ZoneServer | `DatapackRuntime` load + query by id |
| E | Client/Tools | Smoke: server ids resolve against client grp tables |
| F | Infra | CI validate schema + sample pack |

#### v0.9 — Races & classes (+ subclasses)
**Цель:** сначала **одна** path Human Fighter; затем all races/base classes; subclasses — отдельный вертикальный срез в том же milestone.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Datapack | char templates / class tree import |
| B | ZoneServer | create character + starter gear/skills for 1 class |
| C | Client | char create: all race/sex/class options (UI already partially exists) |
| D | ZoneServer | class change / 2nd class quest hooks (depends v0.14 soft) |
| E | ZoneServer+Client | subclass certify / switch (после base classes) |

#### v0.10 — Skills system
**Цель:** pipeline «все скиллы» = import XML + runtime effects; не issue на каждый skill.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Tools | skills XML → schema importer |
| B | ZoneServer | SkillList, learn, use, reuse, mp/hp cost |
| C | Client | SkillLearn UI + MagicSkillUse domain (расширение v0.5) |
| D | Client | VFX bind via EffectGenerator / Decompile_Common |
| E | ZoneServer | buffs/debuffs / toggle minimal set → generalize |

#### v0.11 — NPC & mobs
**Цель:** sample set TI → bulk npc+spawn import; AI от stub к aggro/path.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Tools | npc XML + spawn import |
| B | ZoneServer | spawn knownlist NpcInfo from datapack |
| C | ZoneServer | aggro + basic AI + corpse/loot hook |
| D | Client | models via Npcgrp/Monster prefab pipeline |
| E | Tools | bulk validate all npc ids vs client |

#### v0.12 — Items, weapons & enchant
**Цель:** weapons/armor/etc import; EnchantWindow уже на клиенте — нужен server path.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Tools | items/weapons XML importer |
| B | ZoneServer | inventory give/equip/unequip (связь с v0.6) |
| C | ZoneServer | enchant scrolls + fail/success + crystalize hook |
| D | Client | EnchantWindow + EnchantResult packet path |
| E | Datapack | weapon grades / soulshots hooks smoke |

#### v0.13 — Locations & maps
**Цель:** выйти за TI; geodata move; teleports.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Tools | map/geodata pack import beyond `16_24`–`17_25` |
| B | Client | streaming load additional tiles |
| C | ZoneServer | zone definitions, restart points, teleports |
| D | ZoneServer | geodata collision for move validation |
| E | Datapack | town/field catalog smoke |

#### v0.14 — Quests engine
**Цель:** engine + 1 E2E quest; bulk = HTML/script import. **Не** issue на каждый quest.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Datapack | quest schema (states, items, npc talk) |
| B | Tools | HTML quest importer (Mobius/Acis style) |
| C | ZoneServer | talk/accept/progress/reward state machine |
| D | Client | QuestList + HtmlWindow path |
| E | Tools | bulk import + count/report; fix-forward failures |

#### v0.15 — Seven Seals
**Цель:** периоды печатей, доступ catacombs/necropolis, related quests/NPC.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Datapack | seal calendar + related npc/doors |
| B | ZoneServer | seal period state + ranking hooks |
| C | Client | seal info UI / packets |
| D | ZoneServer | dungeon access gates |
| E | Tools | admin force period for smoke |

#### v0.16 — Olympiad
**Цель:** non-classed matches first; then full olympiad cycle.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Datapack | stadiums / olympiad NPC config |
| B | ZoneServer | register / matchmake / fight / reward |
| C | Client | olympiad UI + observe packets |
| D | Tools | 2-client match smoke |
| E | ZoneServer | hero cycle / points (after basic matches) |

#### v0.17 — Raid bosses
**Цель:** 1 sample RB (e.g. TI-adjacent / classic Interlude RB) → all via datapack.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | Datapack | RB definitions (hp, skills, windows, loot) |
| B | ZoneServer | RB AI + damage windows + spoil/drop |
| C | Client | RB target/info presentation |
| D | ZoneServer | respawn / epic boss schedule |
| E | Tools | bulk RB import validate |

#### v0.18 — Clan & castle
**Цель:** clan create/pledge; castle siege loop как natural endgame social.

| Параллель | Слой | Задачи |
|-----------|------|--------|
| A | ZoneServer | clan create / invite / pledge update |
| B | Client | Clan UI wire (MasterClan и др. уже есть заготовки) |
| C | Datapack | castles / clan halls / doors |
| D | ZoneServer | siege registration + ownership |
| E | Tools | siege smoke checklist |

---

## Честная оценка масштаба

| Запрос пользователя | Что это значит в Fineage |
|---------------------|--------------------------|
| «все квесты» | Quest **engine** + HTML/datapack **importer** + runtime; тысячи квестов не заводятся issues |
| «все скиллы / мобы / оружия» | То же: XML→schema→server; клиентские grp уже частично есть |
| «все расы» | Char create + templates; потом subclass |
| «все локации» | Map/geodata pipeline за пределы TI |
| RB / Olympiad / Seven Seals | Отдельные milestones после combat + NPC + items |
| Server-side для всего | `DatapackRuntime` в ZoneServer, не login-stub forever |

---

## Связь с GitHub

| Milestone | Project #7 |
|-----------|------------|
| v0.1–v0.7 | Phase A issues (уже созданы) |
| v0.8–v0.18 | Epics + starter child issues (Content) |

Доска: https://github.com/orgs/FioraSolutions/projects/7
