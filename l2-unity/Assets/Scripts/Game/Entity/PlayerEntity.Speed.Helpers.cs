using UnityEngine;
using VContainer;
using System;

public partial class PlayerEntity
{
    public override float UpdateRunSpeed(float serverValue)
    {
        if (serverValue <= 0.001f)
            return UnityRunSpeedMeters;

        float converted = base.UpdateRunSpeed(serverValue);
        PlayerController player = LocalPlayer;
        if (player != null)
            player.UpdateRunSpeed(converted);

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
        PlayerController player = LocalPlayer;
        if (player != null)
            player.UpdateWalkSpeed(converted);

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

        PlayerController player = LocalPlayer;
        if (player != null)
            player.Running = running;

        if (HudCorePacketServices.CharacterInfo != null)
            HudCorePacketServices.CharacterInfo.UpdateValues();
    }
}