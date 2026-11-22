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
        if (!AudioWorldGate.ShouldPlayWorldSound(entityId, position))
            return false;

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

        if (TryPlayOfficialCharacterWaves(service, chrSoundBank, soundEvent, position, entityId))
            return true;

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
        if (!AudioWorldGate.ShouldPlayWorldSound(entityId, position))
            return false;

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

        if (TryPlayOfficialCharacterWaves(servicePlay, race.ToString(), soundEvent, position, entityId))
            return true;

        string path = AudioPathResolver.FmodChrSound(race, soundEvent);
        return servicePlay.PlayFmodEvent(path, AudioCategory.Sfx, position, entityId, soundEvent.ToString());
    }

    public static bool PlayMonster(MonsterSoundEvent soundEvent, string npcClassName, Vector3 position, int entityId)
    {
        AudioService service = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
        if (service == null)
            return false;

        if (position.sqrMagnitude <= 0.0001f)
        {
            service.LogCombatAudioError(
                "monster combat sound missing world position",
                entityId,
                AudioPathResolver.FmodMonsterSound(
                    AudioPathResolver.NormalizeMonsterSoundFolder(npcClassName),
                    soundEvent.ToString().ToLowerInvariant()),
                soundEvent.ToString());
            return false;
        }

        if (!AudioWorldGate.ShouldPlayWorldSound(entityId, position))
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

    public static bool PlayHitImpact(bool criticalHit, Vector3 position, int entityId) =>
        PlayHitImpact(criticalHit, position, entityId, attacker: null, target: null, shieldBlock: false);

    public static bool PlayHitImpact(
        bool criticalHit,
        Vector3 position,
        int entityId,
        Entity attacker,
        Entity target,
        bool shieldBlock = false)
    {
        if (!AudioWorldGate.ShouldPlayWorldSound(entityId, position))
            return false;

        AudioService service = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
        if (service == null)
            return false;

        if (attacker == null && !shieldBlock
            && CombatAudioHitContext.TryTake(out Entity pendingAttacker, out bool pendingCrit, out bool pendingShield))
        {
            attacker = pendingAttacker;
            shieldBlock = pendingShield;
            if (criticalHit == false)
                criticalHit = pendingCrit;
        }

        string eventPath;
        if (shieldBlock)
        {
            eventPath = CombatHitSoundResolver.ResolveShieldBlockFmodEventPath(target);
        }
        else
        {
            int weaponItemId = CombatHitSoundResolver.ResolveAttackerWeaponItemId(attacker);
            eventPath = CombatHitSoundResolver.ResolveFmodEventPath(
                criticalHit,
                attacker,
                target,
                weaponItemId);
        }

        if (string.IsNullOrEmpty(eventPath))
        {
            service.LogCombatAudioError(
                "hit impact event path empty",
                entityId,
                null,
                "HitImpact");
            return false;
        }

        if (!InterludeSoundTokenPath.IsCombatSfxEvent(eventPath))
        {
            service.LogCombatAudioError(
                "hit impact path is not combat SFX",
                entityId,
                eventPath,
                "HitImpact");
            return false;
        }

        return PlayCombatSfxEvent(service, eventPath, position, entityId, "HitImpact", volumeScale: 0.75f);
    }

    public static bool PlayItemSoundToken(string datToken, Vector3 position, int entityId)
    {
        if (!AudioWorldGate.ShouldPlayWorldSound(entityId, position))
            return false;

        if (!InterludeSoundTokenPath.TryFromDatToken(datToken, out string eventPath))
        {
            if (OfficialWaveTable.TryResolveDatToken(datToken, out string officialTokenPath, out string tokenError))
            {
                AudioService officialService = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
                if (officialService == null)
                    return false;
                return officialService.PlayClipAtWorld(officialTokenPath, position, AudioCategory.Sfx, 1f, atListener: false);
            }

            AudioService service = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
            service?.LogCombatAudioError(
                tokenError ?? ("item_sound token unresolved token=\"" + datToken + "\""),
                entityId,
                datToken,
                "WeaponSwing");
            return false;
        }

        if (InterludeSoundTokenPath.IsUiOrInterfaceEvent(eventPath))
        {
            AudioService service = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
            service?.LogCombatAudioError(
                "refused UI/interface weapon sound",
                entityId,
                eventPath,
                "WeaponSwing");
            return false;
        }

        AudioService play = RequiredAudioServices.RequireService(nameof(CharacterCombatAudioDispatch));
        if (play == null)
            return false;

        return PlayCombatSfxEvent(play, eventPath, position, entityId, "WeaponSwing", volumeScale: 1f);
    }

    static bool PlayCombatSfxEvent(
        AudioService service,
        string fmodEventPath,
        Vector3 position,
        int entityId,
        string eventLabel,
        float volumeScale)
    {
        InterludeFmodCombatSoundResolver.EnsureCombatBankForEvent(fmodEventPath);

        string officialPath = null;
        OfficialWaveTable.TryResolveDatToken(TokenFromFmodOrDat(fmodEventPath), out officialPath, out _);
        if (!string.IsNullOrEmpty(officialPath)
            && service.PlayClipAtWorld(officialPath, position, AudioCategory.Sfx, volumeScale, atListener: false))
            return true;

        if (service.PlayFmodEvent(fmodEventPath, AudioCategory.Sfx, position, entityId, eventLabel))
            return true;

        string resourcesPath = officialPath ?? InterludeSoundTokenPath.ResourcesPathFromFmodEvent(fmodEventPath);
        if (!string.IsNullOrEmpty(resourcesPath)
            && service.PlayClipAtWorld(resourcesPath, position, AudioCategory.Sfx, volumeScale, atListener: false))
            return true;

        service.LogCombatAudioError(
            "combat sound unresolved event=\"" + fmodEventPath + "\""
            + (string.IsNullOrEmpty(resourcesPath)
                ? " (no official Resources wave)"
                : " Resources=\"" + resourcesPath + "\""),
            entityId,
            fmodEventPath,
            eventLabel);
        return false;
    }

    static readonly System.Collections.Generic.Dictionary<string, int> ChrVariantCursor =
        new System.Collections.Generic.Dictionary<string, int>(System.StringComparer.Ordinal);

    static bool TryPlayOfficialCharacterWaves(
        AudioService service,
        string bank,
        CharacterSoundEvent soundEvent,
        Vector3 position,
        int entityId)
    {
        if (service == null)
            return false;
        if (!OfficialWaveTable.TryResolveCharacter(bank, soundEvent, out var entries, out string error))
            return false;

        OfficialWaveTable.Entry pick = entries[0];
        if (entries.Count > 1)
        {
            string key = bank + ":" + soundEvent;
            ChrVariantCursor.TryGetValue(key, out int i);
            pick = entries[i % entries.Count];
            ChrVariantCursor[key] = i + 1;
        }

        return service.PlayClipAtWorld(pick.ResourcesPath, position, AudioCategory.Sfx, 1f, atListener: false);
    }

    static string TokenFromFmodOrDat(string fmodOrToken)
    {
        if (string.IsNullOrEmpty(fmodOrToken))
            return fmodOrToken;
        if (fmodOrToken.StartsWith("event:/", System.StringComparison.OrdinalIgnoreCase))
        {
            string tail = fmodOrToken.Substring("event:/".Length);
            int slash = tail.IndexOf('/');
            if (slash > 0 && slash < tail.Length - 1)
                return tail.Substring(0, slash) + "." + tail.Substring(slash + 1).Replace('/', '.');
        }
        return fmodOrToken;
    }
}