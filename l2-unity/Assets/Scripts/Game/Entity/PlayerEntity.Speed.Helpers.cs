using UnityEngine;
using VContainer;

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

        PlayerAppearance playerApperance = _appearance as PlayerAppearance;
        bool twoHanded = _gear != null && _gear.IsTwoHandedEquipped();
        int baseClass = playerApperance != null ? playerApperance.BaseClass : 0;
        int sex = playerApperance != null ? playerApperance.Sex : 0;
        float anim_converted = CharTemplateRegistry.GetRunSpeed(baseClass, sex, serverValue, twoHanded);
        AnimationPacketServices.PlayerAnimation?.SetRunSpeed(anim_converted);

        _lastServerRunSpeed = serverValue;
        return converted;
    }

    public void RefreshRunSpeed()
    {
        PlayerAppearance playerApperance = _appearance as PlayerAppearance;
        bool twoHanded = _gear != null && _gear.IsTwoHandedEquipped();
        int baseClass = playerApperance != null ? playerApperance.BaseClass : 0;
        int sex = playerApperance != null ? playerApperance.Sex : 0;
        float anim_converted = CharTemplateRegistry.GetRunSpeed(baseClass, sex, _lastServerRunSpeed, twoHanded);
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

        PlayerAppearance playerApperance = _appearance as PlayerAppearance;
        bool twoHanded = _gear != null && _gear.IsTwoHandedEquipped();
        int baseClass = playerApperance != null ? playerApperance.BaseClass : 0;
        int sex = playerApperance != null ? playerApperance.Sex : 0;
        float anim_converted = CharTemplateRegistry.GetWalkSpeed(baseClass, sex, serverValue, twoHanded);
        AnimationPacketServices.PlayerAnimation?.SetWalkSpeed(anim_converted);

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
