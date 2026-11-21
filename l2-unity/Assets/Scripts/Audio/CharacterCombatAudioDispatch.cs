using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One combat/character SFX path: resolve event → backend (FMOD or Resources clip) → play.
/// Soulshot/cast paths stay on InterludeSfx; locomotion steps use ChrSound banks here too.
/// </summary>
public static class CharacterCombatAudioDispatch
{
    static readonly HashSet<string> LoggedUnresolvedMonsterSounds = new HashSet<string>();
    public static bool PlayCharacterBank(
        string chrSoundBank,
        CharacterSoundEvent soundEvent,
        Vector3 position,
        int entityId)
    {
        AudioService service = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
        if (service == null)
            return false;

        if (string.IsNullOrEmpty(chrSoundBank))
        {
            service.LogCombatAudioError(
                "ChrSound bank empty for " + soundEvent,
                entityId,
                chrSoundBankPath: null,
                soundEvent.ToString());
            return false;
        }

        string fmodBank = NpcChrSoundBankResolver.MapDamageBankToFmodBank(chrSoundBank);
        string path = AudioPathResolver.FmodChrSoundBank(fmodBank, soundEvent);
        return service.PlayFmodEvent(path, AudioCategory.Sfx, position, entityId, soundEvent.ToString());
    }

    public static bool PlayCharacterRace(
        CharacterSoundEvent soundEvent,
        CharacterRaceSound race,
        Vector3 position,
        int entityId)
    {
        if (race == CharacterRaceSound.Default)
        {
            AudioService service = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
            service?.LogCombatAudioError(
                "ChrSound race unset for " + soundEvent,
                entityId,
                AudioPathResolver.FmodChrSound(race, soundEvent),
                soundEvent.ToString());
            return false;
        }

        AudioService servicePlay = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
        if (servicePlay == null)
            return false;

        string path = AudioPathResolver.FmodChrSound(race, soundEvent);
        return servicePlay.PlayFmodEvent(path, AudioCategory.Sfx, position, entityId, soundEvent.ToString());
    }

    public static bool PlayMonster(MonsterSoundEvent soundEvent, string npcClassName, Vector3 position, int entityId)
    {
        AudioService service = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
        if (service == null)
            return false;

        if (string.IsNullOrEmpty(npcClassName))
        {
            service.LogCombatAudioError(
                "monster class name empty for " + soundEvent,
                entityId,
                null,
                soundEvent.ToString());
            return false;
        }

        string folder = AudioPathResolver.NormalizeMonsterSoundFolder(npcClassName);
        string key = soundEvent.ToString().ToLowerInvariant();
        string path = AudioPathResolver.FmodMonsterSound(folder, key);
        if (service.TryPlayFmodOrResourcesMonster(soundEvent, folder, position, entityId, path))
            return true;

        string logKey = folder + "/" + key;
        if (LoggedUnresolvedMonsterSounds.Add(logKey))
        {
            service.LogCombatAudioError(
                "monster combat sound unresolved",
                entityId,
                path,
                soundEvent.ToString());
        }

        return false;
    }

    public static bool PlayHitImpact(bool criticalHit, Vector3 position, int entityId)
    {
        AudioService service = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
        if (service == null)
            return false;

        string catalogId = criticalHit ? AudioSoundCatalog.IdHitCritical : AudioSoundCatalog.IdHitArmor;
        if (!AudioSoundCatalog.TryResolve(catalogId, out AudioPlaySpec spec))
        {
            service.LogCombatAudioError(
                AudioSoundCatalog.FormatUnknownIdError(catalogId),
                entityId,
                catalogId,
                "HitImpact");
            return false;
        }

        if (spec.Source == AudioSourceKind.FmodStudioEvent)
            return service.PlayFmodEvent(spec.Id, spec.Category, position, entityId, "HitImpact");

        return service.PlayFromSpec(spec, position, 0.75f);
    }
}