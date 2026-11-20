using System;
using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public partial class AudioManager : MonoBehaviour
{
    public void PlayMonsterSound(MonsterSoundEvent monsterSoundEvent, string npcClassName, Vector3 position) =>
        Svc.PlayMonsterSoundFmod(monsterSoundEvent, npcClassName, position);

    public void PlayCharacterSound(CharacterSoundEvent characterSoundEvent, CharacterRaceSound characterRace, Vector3 position) =>
        Svc.PlayCharacterSound(characterSoundEvent, characterRace, position);

    public void PlayUISound(string soundName) => Svc.PlayUiSound(soundName);
}