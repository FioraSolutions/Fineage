using System;
using UnityEngine;
using VContainer;

public partial class PlayerEntity : Entity
{
    protected override void OnDeath()
    {
        base.OnDeath();
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.DEAD);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _characterAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }

    public string GetCurrentAnimName()
    {
        return _gear.WeaponAnim;
    }

    public GameObject GetGoEtcItem()
    {
        return _gear.GetGoEtcItem();
    }

    public float TargetDistance()
    {
        Vector3 startPos = GetPositionRightHand();
        Transform target = Target;

        return VectorUtils.Distance2D(startPos , target.position);
    }

    public void SetProceduralSpinePose(Vector3 rotation)
    {
        try
        {
            Transform bone = _gear.GetSpineBone();
            SpineProceduralPacketServices.Controller?.SetBoneMod(bone, new BoneModification(rotation, Vector3.zero, 1.0f));
        }
        catch (Exception)
        {
            // Debug.LogError("");
        }


    }

    public void SetProceduralRightUpperArmPose(Vector3 rotation)
    {
        try
        {
            Transform upperArm = _gear.GetRightUpperArm();
            SpineProceduralPacketServices.Controller?.SetBoneMod(upperArm, new BoneModification(rotation, Vector3.zero, 1.0f));
        }
        catch (Exception)
        {
            // Debug.LogError("");
        }

    }

    public void RemoveProceduralPose()
    {
        Transform bone = _gear.GetSpineBone();
        Transform upperArm = _gear.GetRightUpperArm();
        SpineProceduralController procedural = SpineProceduralPacketServices.Controller;
        if (procedural == null)
            return;

        procedural.RemoveBoneMod(bone);
        procedural.RemoveBoneMod(upperArm);
    }
}
