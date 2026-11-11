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
        if (_isDisabled) return;

        string eventKey;
        surfaceTag = surfaceTag.ToLower();

        switch (surfaceTag)
        {
            case "dirt":
                eventKey = surfaceTag + "_run";
                break;
            case "stone":
                eventKey = surfaceTag + "_run";
                break;
            case "wood":
                eventKey = surfaceTag + "_run";
                break;
            default:
                eventKey = "default_run";
                break;

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
