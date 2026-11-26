using UnityEngine;
using VContainer;
using System;

public partial class PlayerEntity
{
    /// <summary>
    /// Local hero locomotion floats live on <see cref="PlayerAnimationController"/> (child anim_player),
    /// not the root <see cref="NetworkAnimationController"/> used for remote users.
    /// </summary>
    protected override IAnimationController LocomotionAnimationController =>
        AnimationPacketServices.PlayerAnimation != null
            ? AnimationPacketServices.PlayerAnimation
            : _networkAnimationReceive;

    public override float UpdateRunSpeed(float serverValue)
    {
        if (serverValue <= 0.001f)
            return UnityRunSpeedMeters;

        float converted = base.UpdateRunSpeed(serverValue);
        PlayerController found = LocalPlayer;
        if (found != null)
        {
            IPlayerPawn player = found;
            player.UpdateRunSpeed(converted);
        }

        RefreshLocomotionAnimRates();
        _lastServerRunSpeed = serverValue;
        return converted;
    }

    public void RefreshRunSpeed()
    {
        if (_lastServerRunSpeed > 0.001f)
            UpdateRunSpeed(_lastServerRunSpeed);
        else
            RefreshLocomotionAnimRates();
    }

    public override float UpdateWalkSpeed(float serverValue)
    {
        if (serverValue <= 0.001f)
            return UnityWalkSpeedMeters;

        float converted = base.UpdateWalkSpeed(serverValue);
        PlayerController found = LocalPlayer;
        if (found != null)
        {
            IPlayerPawn player = found;
            player.UpdateWalkSpeed(converted);
        }

        RefreshLocomotionAnimRates();
        return converted;
    }

    public override void UpdateWaitType(WaitType moveType)
    {
        base.UpdateWaitType(moveType);
    }

    public override void UpdateMoveType(bool running)
    {
        base.UpdateMoveType(running);

        PlayerController found = LocalPlayer;
        if (found != null)
        {
            IPlayerPawn player = found;
            player.Running = running;
        }

        if (HudCorePacketServices.CharacterInfo != null)
            HudCorePacketServices.CharacterInfo.UpdateValues();
    }
}