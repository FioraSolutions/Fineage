using FMOD.Studio;
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

        if (position.sqrMagnitude <= 0.0001f)
        {
            LogCombatAudioError(
                "monster combat clip refused without world position",
                entityId,
                fmodPath,
                soundEvent.ToString());
            return false;
        }

        PlayClipAt(clip, position, AudioCategory.Sfx, MonsterCombatSound.Volume, atListener: false, clip.name);
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
        if (er.IsNull || !IsFmodEventAvailable(eventPath))
        {
            if (CombatTrace.Enabled)
                CombatTrace.Write("snd.fmod-missing", "event=" + eventPath + " label=" + eventLabel + " owner=" + entityId);
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

        AudioPlayTrace.Write(
            "play", Time.unscaledTime, eventPath, "Fmod", eventLabel, entityId, 0f, 0f, 1f, 0, "fmod");
        if (CombatTrace.Enabled)
            CombatTrace.Write("snd.fmod", "event=" + eventPath + " label=" + eventLabel + " owner=" + entityId +
                " sinceDieMs=" + CombatTrace.SinceDieMs(entityId));
        return true;
    }

    static bool IsFmodEventAvailable(string eventPath)
    {
        if (string.IsNullOrEmpty(eventPath))
            return false;

        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(eventPath, out EventDescription description);
        return result == FMOD.RESULT.OK && description.isValid();
    }
}