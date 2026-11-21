using FMODUnity;
using UnityEngine;

public partial class AudioService
{
    public void LogCombatAudioError(string detail, int entityId, string chrSoundBankPath, string eventLabel)
    {
        string pathPart = string.IsNullOrEmpty(chrSoundBankPath) ? "" : " event=\"" + chrSoundBankPath + "\"";
        LogAudioError(
            "AudioService: combat sound " + eventLabel + " entityId=" + entityId + " — " + detail + pathPart);
    }

    public bool TryPlayFmodOrResourcesMonster(
        MonsterSoundEvent soundEvent,
        string npcClassName,
        Vector3 position,
        int entityId,
        string fmodPath)
    {
        if (!string.IsNullOrEmpty(fmodPath) && PlayFmodEvent(fmodPath, AudioCategory.Sfx, position, entityId, soundEvent.ToString()))
            return true;

        if (!MonsterCombatSound.TryLoadClip(soundEvent, npcClassName, out AudioClip clip) || clip == null)
            return false;

        if (position.sqrMagnitude > 0.0001f)
            PlayClipAt(clip, position, AudioCategory.Sfx, MonsterCombatSound.Volume, atListener: false, clip.name);
        else
            PlayLoadedClip2D(clip, AudioCategory.Sfx, MonsterCombatSound.Volume, clip.name);

        return true;
    }

    public bool PlayFmodEvent(
        string eventPath,
        AudioCategory category,
        Vector3? position,
        int entityId,
        string eventLabel)
    {
        if (_disabled || string.IsNullOrEmpty(eventPath))
            return false;

        EventReference er = RuntimeManager.PathToEventReference(eventPath);
        if (er.IsNull)
        {
            if (LoggedMissingFmodEvents.Add(eventPath))
            {
                LogCombatAudioError(
                    "missing FMOD event (GUID empty or bank not loaded)",
                    entityId,
                    eventPath,
                    eventLabel);
            }

            return false;
        }

        if (position.HasValue)
            RuntimeManager.PlayOneShot(er, position.Value);
        else
            RuntimeManager.PlayOneShot(er);

        return true;
    }
}