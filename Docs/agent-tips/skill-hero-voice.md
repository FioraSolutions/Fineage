# Hero skill voice ("читает скилл")

## Source (Interlude client)
- Package: `Lineage II/sounds/chrsound.uax` (+ `chrsound2.uax` is NPC/fishing, not hero skill voice).
- Skill SFX stay in `skillsound*.uax` → `Resources/Data/Sounds/SkillSound/*_{cast,shot,explotion}`.
- Hero *voice* clips are named `{F|M}_{Race}_{Kind}.wav`, e.g. `M_HMagician_Shot`, `F_Elf_Element`.

## Kinds
Human / Elf / Dark Elf: `Black`, `White`, `Element`, `Call`, `Sub`, `Shot`, `Throw`, `Notarget`.
Dwarf / Orc: `Type_A`, `Type_B`, `Type_C`, `Shot`, `Throw`, `Notarget`.

Exact per-skill map is in encrypted `system/skillsoundgrp.dat` (Lineage2Ver413). Until decrypted, `SkillCastSound` picks Kind from skill name keywords.

## Fineage wiring
- Clips: `l2-unity/Assets/Resources/Data/Sounds/SkillVoice/`
- Code: `SkillCastSound.OnMagicSkillUse` plays SFX + voice via `AudioSource.PlayClipAtPoint`.
- Needs Unity audio on (`ProjectSettings/AudioManager.asset` `m_DisableAudio: 0`) and `AudioListener` on Main Camera.
- Race prefix from `Entity.RaceId` (`CharacterRaceAnimation`).

## Extract
```
umodel -export -sounds -out=... -path=<sounds_dir> chrsound.uax
# keep only names matching ^[FM]_
```

## 2026-10-30 fix (male human mage silent + self-heal silent)
- Cause A: `PlayClipAtPoint` is 3D; L2 camera distance mutes short chrsound clips. Voice (and SFX) now play via DontDestroy 2D `AudioSource` (`spatialBlend=0`).
- Cause B: permanent `_missing` HashSet + Enter Play Mode without domain reload poisoned loads after SkillVoice was added mid-session. Misses now retry after 2s.
- Cause C: Battle Heal / Group Heal / Elemental Heal had no `*_cast` stems — aliased to `heal` / `greater_heal` via skillId map + name normalize.
- Cause D: prefab `RaceId` default FElf; `ResolveRaceForVoice` re-resolves from Appearance+class so Magician gets `M_HMagician_*`.
