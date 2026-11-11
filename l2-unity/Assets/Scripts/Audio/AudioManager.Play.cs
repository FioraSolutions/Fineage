using System;

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

    public void PlayItemSound(ItemSoundEvent itemSoundEvent, Vector3 position)
    {
        if (_isDisabled) return;

        EventReference er;
        er = RuntimeManager.PathToEventReference("event:/ItemSound/" + itemSoundEvent.ToString());


        if (!er.IsNull)
        {
            PlaySound(er, position);
        }
    }

    public void PlaySound(EventReference sound, Vector3 postition)
    {
        if (_isDisabled) return;


        RuntimeManager.PlayOneShot(sound, postition);
    }

    public void PlaySound(EventReference sound)
    {
        if (_isDisabled) return;


        RuntimeManager.PlayOneShot(sound);
    }
}
