using System;
using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public partial class AudioManager : MonoBehaviour
{
    public void PlayItemSound(ItemSoundEvent itemSoundEvent, Vector3 position) =>
        Svc.PlayItemSound(itemSoundEvent, position);

    public void PlaySound(EventReference sound, Vector3 postition)
    {
        if (_isDisabled || sound.IsNull)
            return;
        RuntimeManager.PlayOneShot(sound, postition);
    }

    public void PlaySound(EventReference sound)
    {
        if (_isDisabled || sound.IsNull)
            return;
        RuntimeManager.PlayOneShot(sound);
    }
}