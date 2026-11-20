using System;
using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public partial class AudioManager : MonoBehaviour
{
    public void PlayEquipSound(string soundName) => Svc.PlayEquipSound(soundName);

    public void PlayStepSound(string surfaceTag, Vector3 position) =>
        Svc.PlayStepSound(surfaceTag, position, false);

    public void PlayStepSound(string surfaceTag, Vector3 position, bool running) =>
        Svc.PlayStepSound(surfaceTag, position, running);

    public void PlayHitSound(bool criticalHit, Vector3 position)
    {
        if (_isDisabled)
            return;
        Svc.PlayHitImpact(criticalHit, position);
    }
}