using FMODUnity;
using UnityEngine;

public partial class AudioService
{
    public bool PlayFmodEvent(string eventPath, AudioCategory category, Vector3? position = null)
    {
        if (_disabled || string.IsNullOrEmpty(eventPath))
            return false;

        EventReference er = RuntimeManager.PathToEventReference(eventPath);
        if (er.IsNull)
        {
            LogAudioError("AudioService: missing FMOD event id=\"" + eventPath + "\"");
            return false;
        }

        if (position.HasValue)
            RuntimeManager.PlayOneShot(er, position.Value);
        else
            RuntimeManager.PlayOneShot(er);

        return true;
    }

    public void PlayCharacterSound(CharacterSoundEvent soundEvent, CharacterRaceSound race, Vector3 position)
    {
        string path = AudioPathResolver.FmodChrSound(race, soundEvent);
        PlayFmodEvent(path, AudioCategory.Sfx, position);
    }

    public void PlayMonsterSoundFmod(MonsterSoundEvent soundEvent, string npcClassName, Vector3 position)
    {
        string key = soundEvent.ToString().ToLowerInvariant();
        string path = AudioPathResolver.FmodMonsterSound(npcClassName, key);
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
        string path = AudioPathResolver.FmodInterfaceSound(soundName);
        PlayFmodEvent(path, AudioCategory.Ui);
    }

    public void PlayStepSound(string surfaceTag, Vector3 position, bool running)
    {
        string eventKey = AudioPathResolver.StepEventKey(surfaceTag, running);
        string path = AudioPathResolver.FmodStepSound(eventKey);
        PlayFmodEvent(path, AudioCategory.Sfx, position);
    }
}