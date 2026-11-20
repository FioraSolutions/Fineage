using UnityEngine;
using VContainer;
using System;

public partial class PlayerEntity
{
    public override float UpdateRunSpeed(float serverValue)
    {
        if (serverValue <= 0.001f)
            serverValue = 120f;
        float converted = base.UpdateRunSpeed(serverValue);
        // Apply planar speed first — anim NRE must not leave PlayerController at 0.
        PlayerController player = LocalPlayer;
        if (player != null)
            player.UpdateRunSpeed(converted);

        float anim_converted = global::LocomotionGait.Rate(
            AnimationPacketServices.PlayerAnimation != null
                ? AnimationPacketServices.PlayerAnimation.GetAnimator()
                : null,
            AnimationNames.RUN.Concat(WeaponSuffix()),
            converted,
            global::LocomotionGait.MinRunRate,
            global::LocomotionGait.MaxRate);
        AnimationPacketServices.PlayerAnimation?.SetRunSpeed(anim_converted);

        _lastServerRunSpeed = serverValue;
        return converted;
    }

    public void RefreshRunSpeed()
    {
        float unity = NumberUtils.ScaleToUnity(_lastServerRunSpeed > 0.001f ? _lastServerRunSpeed : 120f);
        float anim_converted = global::LocomotionGait.Rate(
            AnimationPacketServices.PlayerAnimation != null
                ? AnimationPacketServices.PlayerAnimation.GetAnimator()
                : null,
            AnimationNames.RUN.Concat(WeaponSuffix()),
            unity,
            global::LocomotionGait.MinRunRate,
            global::LocomotionGait.MaxRate);
        AnimationPacketServices.PlayerAnimation?.SetRunSpeed(anim_converted);
    }

    public override float UpdateWalkSpeed(float serverValue)
    {
        if (serverValue <= 0.001f)
            serverValue = 80f;
        float converted = base.UpdateWalkSpeed(serverValue);
        PlayerController player = LocalPlayer;
        if (player != null)
            player.UpdateWalkSpeed(converted);

        float anim_converted = global::LocomotionGait.Rate(
            AnimationPacketServices.PlayerAnimation != null
                ? AnimationPacketServices.PlayerAnimation.GetAnimator()
                : null,
            AnimationNames.WALK.Concat(WeaponSuffix()),
            converted,
            global::LocomotionGait.MinWalkRate,
            global::LocomotionGait.MaxRate);
        AnimationPacketServices.PlayerAnimation?.SetWalkSpeed(anim_converted);

        return converted;
    }

    string WeaponSuffix()
    {
        if (_gear != null && !string.IsNullOrEmpty(_gear.WeaponAnim))
            return _gear.WeaponAnim;
        return "hand";
    }

    public override void UpdateWaitType(WaitType moveType)
    {
        base.UpdateWaitType(moveType);
    }

    public override void UpdateMoveType(bool running)
    {
        base.UpdateMoveType(running);

        PlayerController player = LocalPlayer;
        if (player != null)
            player.Running = running;

        if (PlayerStatePacketServices.PlayerState != null)
        {
           // PlayerStatePacketServices.PlayerState.NotifyEvent(Event.MOVE_TYPE_UPDATED);
        }

        if (HudCorePacketServices.CharacterInfo != null)
        {
            HudCorePacketServices.CharacterInfo.UpdateValues();
        }
    }
}
