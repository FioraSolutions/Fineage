# Fineage — локальный запуск до экрана логина

## Требования

- Unity **6000.3.27f1** LTS (Apple Silicon) — pinned в `ProjectSettings/ProjectVersion.txt`
- Проект: `l2-unity/`
- Диагностические дампы: `l2-unity/logs/diag/` (не Windows `C:\unity\...`)

## Baseline (версии)

| Что | Сейчас | Заметка |
|---|---|---|
| Editor | **6000.3.27f1** (LTS) | Ветка `chore/unity-6.3`. Было 6000.0.68f1. |
| URP / ShaderGraph / VFX | **17.3.0** | Линия Editor 6000.3.x |
| Input System | **1.20.1** | |
| ProBuilder | **6.1.3** | |
| VContainer | 1.19.0 | Актуальный релиз |
| UnityMeshSimplifier | commit `d741912` | Закреплён; не `master` floating |

Первый открытие в 6.3: дождаться Package Manager resolve + реимпорт; `packages-lock.json` пересоберёт Editor.

## На что опирается «декомпил» (в репо)

Полного исходника клиента L2 **нет**. Есть три слоя:

1. **VFX / emitters** — `.uc` (UE particle) в `Assets/Resources/Data/Effects/` + общая математика в `Assets/Resources/Data/Shaders/Skills/Common/Decompile_Common/` (правило: чинить формулы там, не хаками в эффектах). Гайд: `l2-unity/docs/metrics/L2_UC_TO_UNITY_AI_GUIDE.md`.
2. **Сеть / бизнес-логика** — C# под L2J (Acis/Mobius Interlude), пакеты в `Assets/Scripts/Networking/`. Ориентир — поведение оригинала + заметки [Guide-L2Unity](https://github.com/gawric/Guide-L2Unity), не полный декомпил EXE.
3. **Внешние дампы автора** — раньше пути `C:\unity\l2 client\decompile\...` (Frida/move logs, bridge extract, umodel). Теперь: `logs/diag/` и опционально `External/decompile` / env `L2_DECOMPILE_ROOT`.

Архитектура Unity-клиента: `Di/` (VContainer) → `Networking/` (Login/Game + Interlude protocol) → `Game/` + `Controller/` + `UI/` (UI Toolkit).

## Экран логина (без сервера)

1. Открой `l2-unity` в Unity **6000.3.27f1** (первый импорт / package resolve может занять долго).
2. Сцена **`Assets/Resources/Scenes/Menu.unity`** → Play.
3. Должен появиться UI логина (логин/пароль).  
   Кнопка Login ещё ничего полезного не сделает без login-сервера на `127.0.0.1:2106`.

Опционально: drag&drop `l2_lobby` в Menu (как в README gawric) для фона лобби.

## Login stub → экран персонажа

```bash
cd tools/login-stub
dotnet run          # 127.0.0.1:2106 (login) + :7777 (game)
# dotnet run -- --selftest
```

1. `dotnet run` в `tools/login-stub` (login `:2106`, game `:7777`).
2. Menu.unity → Play → Login (любой логин/пароль).
3. AutoAgree/Confirm → char select → **Start** на **Test**.
4. Грузится Talking Island `17_25`, клик по земле — бег (stub эхает `CharMoveToLocation`).

Позже заменить stub на свой C# login/game сервер (тот же wire).

## Логи

Кратко: `l2-unity/docs/LOGGING.md`.

- Lobby/login: Console `[LOBBY]` + `l2-unity/logs/diag/lobby-flow.txt`
- Stub: `[STUB]` + `tools/login-stub/logs/stub.log`; `dotnet run -- --verbose` или `FINEAGE_STUB_DEBUG=1`
- Zone: `[ZONE]` + `tools/zone-server/logs/zone.log`; `--verbose` / `FINEAGE_ZONE_DEBUG=1`
- Debug в клиенте: `FineageLog.DebugEnabled = true` или define `FINEAGE_LOG_DEBUG`
- Если `~/Library/Logs/Unity/Editor.log` раздулся: `: > ~/Library/Logs/Unity/Editor.log`
