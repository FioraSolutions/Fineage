using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public partial class AudioService
{
    static readonly HashSet<string> LoggedMissingFmodEvents = new HashSet<string>();

    public bool PlayFmodEvent(string eventPath, AudioCategory category, Vector3? position = null) =>
        PlayFmodEvent(eventPath, category, position, entityId: 0, eventLabel: "Sfx");

    public void PlayCharacterSound(CharacterSoundEvent soundEvent, CharacterRaceSound race, Vector3 position)
    {
        string path = AudioPathResolver.FmodChrSound(race, soundEvent);
        PlayFmodEvent(path, AudioCategory.Sfx, position);
    }

    public void PlayChrSoundBank(string chrSoundBank, CharacterSoundEvent soundEvent, Vector3 position) =>
        CharacterCombatAudioDispatch.PlayCharacterBank(chrSoundBank, soundEvent, position, entityId: 0);

    public void PlayMonsterSoundFmod(MonsterSoundEvent soundEvent, string npcClassName, Vector3 position)
    {
        string folder = AudioPathResolver.NormalizeMonsterSoundFolder(npcClassName);
        string key = soundEvent.ToString().ToLowerInvariant();
        string path = AudioPathResolver.FmodMonsterSound(folder, key);
        PlayFmodEvent(path, AudioCategory.Sfx, position);
    }

    public void PlayItemSound(ItemSoundEvent itemSoundEvent, Vector3 position)
    {
        string path = AudioPathResolver.FmodItemSound(itemSoundEvent);
        PlayFmodEvent(path, AudioCategory.Sfx, position);
    }

    public void PlayEquipSound(string soundName)
    {
        string path = AudioPathResolver.FmodItemSound(soundName);
        PlayFmodEvent(path, AudioCategory.Sfx);
    }

    public void PlayUiSound(string soundName)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            LogAudioError(OfficialWaveTable.FormatMissingWaveError(null, soundName, "empty UI sound"));
            return;
        }

        string resourcesPath = null;
        string error = null;
        if (soundName.IndexOf('.') >= 0)
        {
            OfficialWaveTable.TryResolveDatToken(soundName, out resourcesPath, out error);
        }
        else if (OfficialWaveTable.TryResolve(soundName, out OfficialWaveTable.Entry entry, out error))
        {
            resourcesPath = entry.ResourcesPath;
        }

        if (!string.IsNullOrEmpty(resourcesPath) && PlayClip2D(resourcesPath, AudioCategory.Ui))
            return;

        LogAudioError(error ?? OfficialWaveTable.FormatMissingWaveError(null, soundName, "UI sound"));
    }

    public void PlayStepSound(string surfaceTag, Vector3 position, bool running)
    {
        string eventKey = AudioPathResolver.StepEventKey(surfaceTag, running);
        string path = AudioPathResolver.FmodStepSound(eventKey);
        PlayFmodEvent(path, AudioCategory.Sfx, position);
    }
}