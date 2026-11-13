using System;
using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public partial class AudioManager : MonoBehaviour
{
    public void PlayEquipSound(string soundName)
    {
        if (_isDisabled) return;

        EventReference er = RuntimeManager.PathToEventReference("event:/ItemSound/" + soundName);
        if (!er.IsNull)
        {
            PlaySound(er);
        }
    }

    public void PlayStepSound(string surfaceTag, Vector3 position)
    {
        PlayStepSound(surfaceTag, position, true);
    }

    public void PlayStepSound(string surfaceTag, Vector3 position, bool running)
    {
        if (_isDisabled) return;

        string eventKey;
        if (!running)
            eventKey = "default_walk";
        else
        {
            string tag = string.IsNullOrEmpty(surfaceTag) ? "" : surfaceTag.ToLowerInvariant();
            switch (tag)
            {
                case "dirt":
                case "stone":
                case "wood":
                case "grass":
                    eventKey = tag + "_run";
                    break;
                default:
                    eventKey = "default_run";
                    break;
            }
        }

        EventReference er = RuntimeManager.PathToEventReference("event:/StepSound/" + eventKey);
        if (!er.IsNull)
        {
            PlaySound(er, position);
        }
    }

    public void PlayHitSound(bool criticalHit, Vector3 position)
    {
        // FMOD path stays off — Unity Resources (same pattern as SkillCastSound / MonsterCombatSound).
        if (HitImpactSound.Play(criticalHit))
            return;

        if (_isDisabled) return;

        EventReference er;
        if (criticalHit)
        {
            er = RuntimeManager.PathToEventReference("event:/SkillSound/critical_hit");
        }
        else
        {
            er = RuntimeManager.PathToEventReference("event:/ItemSound/armor_hit_underwear");
        }

        if (!er.IsNull)
        {
            PlaySound(er, position);
        }
    }
}
