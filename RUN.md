# Fineage — локальный запуск до экрана логина

## Требования

- Unity **6000.0.68f1** (Apple Silicon) — pinned в `ProjectSettings/ProjectVersion.txt`
- Проект: `l2-unity/`
- Диагностические дампы: `l2-unity/logs/diag/` (не Windows `C:\unity\...`)

## Baseline (версии)

| Что | Сейчас | Заметка |
|---|---|---|
| Editor | 6000.0.68f1 | Последний патч ветки 6.0 — **6000.0.85f1**; 6.0 LTS EOL с 16.10.2026. Рекомендуемый LTS — **6.3** (6000.3.x). Прыжок 6.0→6.3 отдельно, не вслепую. |
| URP / ShaderGraph / VFX | 17.0.4 | Идут с Editor 6000.0.x |
| Input System | 1.18.0 | OK |
| VContainer | 1.19.0 | Актуальный релиз |
| UnityMeshSimplifier | commit `d741912` | Закреплён; не `master` floating |

## На что опирается «декомпил» (в репо)

Полного исходника клиента L2 **нет**. Есть три слоя:

1. **VFX / emitters** — `.uc` (UE particle) в `Assets/Resources/Data/Effects/` + общая математика в `Assets/Resources/Data/Shaders/Skills/Common/Decompile_Common/` (правило: чинить формулы там, не хаками в эффектах). Гайд: `l2-unity/docs/metrics/L2_UC_TO_UNITY_AI_GUIDE.md`.
2. **Сеть / бизнес-логика** — C# под L2J (Acis/Mobius Interlude), пакеты в `Assets/Scripts/Networking/`. Ориентир — поведение оригинала + заметки [Guide-L2Unity](https://github.com/gawric/Guide-L2Unity), не полный декомпил EXE.
3. **Внешние дампы автора** — раньше пути `C:\unity\l2 client\decompile\...` (Frida/move logs, bridge extract, umodel). Теперь: `logs/diag/` и опционально `External/decompile` / env `L2_DECOMPILE_ROOT`.

Архитектура Unity-клиента: `Di/` (VContainer) → `Networking/` (Login/Game + Interlude protocol) → `Game/` + `Controller/` + `UI/` (UI Toolkit).

## Экран логина (без сервера)

1. Открой `l2-unity` в Unity 6000.0.68f1 (первый импорт может занять долго).
2. Сцена **`Assets/Resources/Scenes/Menu.unity`** → Play.
3. Должен появиться UI логина (логин/пароль).  
   Кнопка Login ещё ничего полезного не сделает без login-сервера на `127.0.0.1:2106`.

Опционально: drag&drop `l2_lobby` в Menu (как в README gawric) для фона лобби.

## Заглушка login-сокета (позже)

```bash
cd tools/login-stub
dotnet run
```

Слушает `127.0.0.1:2106`, принимает TCP. Init/Blowfish/auth — следующий шаг под твой C# сервер.
