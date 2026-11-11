using System;
using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public partial class AudioManager : MonoBehaviour
{
    public void PlayMonsterSound(MonsterSoundEvent monsterSoundEvent, string npcClassName, Vector3 position)
    {
        if (_isDisabled) return;

        string eventKey = monsterSoundEvent.ToString().ToLower();
        EventReference er = RuntimeManager.PathToEventReference("event:/MonSound/" + npcClassName + "/" + eventKey);
        if (!er.IsNull)
        {
            PlaySound(er, position);
        }
    }

    public void PlayCharacterSound(CharacterSoundEvent characterSoundEvent, CharacterRaceSound characterRace, Vector3 position)
    {
        if (_isDisabled) return;

        string eventKey = characterSoundEvent.ToString();
        EventReference er = RuntimeManager.PathToEventReference("event:/ChrSound/" + characterRace + "/" + characterRace + "_" + eventKey);
        if (!er.IsNull)
        {
            PlaySound(er, position);
        }
    }

    public void PlayUISound(string soundName)
    {
        if (_isDisabled) return;

        EventReference er = RuntimeManager.PathToEventReference("event:/InterfaceSound/" + soundName);
        if (!er.IsNull)
        {
            PlaySound(er);
        }
    }
}
