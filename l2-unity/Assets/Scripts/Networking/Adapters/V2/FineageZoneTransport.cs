using System;
using UnityEngine;

/// <summary>
/// Routes <see cref="IZoneTransport"/> calls based on <see cref="FineageWireConfig.Current"/>.
/// Keeps protocol mode checks out of GameManager, GameClient, and LoginWindow.
/// </summary>
public sealed class FineageZoneTransport : IZoneTransport
{
    public event Action OnDisconnected;

    public bool BypassesInterludeLogin => FineageWireConfig.IsZoneV2;

    public bool AllowsInterludeGameSocketAuth => !FineageWireConfig.IsZoneV2;

    public bool Connected
    {
        get
        {
            ZoneV2Client zone = ActiveZoneClient;
            return zone != null && zone.Connected;
        }
    }

    public int SessionObjectId
    {
        get
        {
            ZoneV2Client zone = ActiveZoneClient;
            return zone != null ? zone.SessionObjectId : 0;
        }
    }

    public bool TrySendGameCommand(INetworkCommand command)
    {
        if (!FineageWireConfig.IsZoneV2)
            return false;

        MoveToCommand move = command as MoveToCommand;
        if (move != null)
        {
            ZoneV2Client zone = ZoneV2Client.Ensure();
            zone.SendMove(move.From, move.To);
            return true;
        }

        ClickActionCommand action = command as ClickActionCommand;
        if (action != null)
        {
            World world = WorldApplyIncomingPacketServices.GameWorld;
            if (world != null)
            {
                Entity clicked = world.GetEntityNoLockSync(action.ObjectId);
                if (clicked is ItemEntity)
                {
                    ZoneV2Client.Ensure().SendPickupRequest(action.ObjectId);
                    return true;
                }
            }

            ZoneV2Client zone = ZoneV2Client.Ensure();
            zone.SendSelectTarget(action.ObjectId);
            TargetManager targets = ControllerPacketServices.Targets;
            if (targets != null && targets.IsAttackTargetSet())
                zone.SendAttackRequest(action.ObjectId);
            return true;
        }

        ZonePickupRequestCommand pickup = command as ZonePickupRequestCommand;
        if (pickup != null)
        {
            ZoneV2Client.Ensure().SendPickupRequest(pickup.GroundObjectId);
            return true;
        }

        ZoneGrantItemCommand grant = command as ZoneGrantItemCommand;
        if (grant != null)
        {
            ZoneV2Client.Ensure().SendGrantItemRequest(grant.ItemId, grant.Count);
            return true;
        }

        UseItemCommand useItem = command as UseItemCommand;
        if (useItem != null)
        {
            ZoneV2Client.Ensure().SendUseItem(useItem.ObjectId);
            return true;
        }

        UnequipItemCommand unequip = command as UnequipItemCommand;
        if (unequip != null)
        {
            ZoneV2Client.Ensure().SendUnequipItem(unequip.PaperdollSlot);
            return true;
        }

        RequestBypassToServerCommand bypass = command as RequestBypassToServerCommand;
        int giveItemId;
        int giveCount;
        if (bypass != null && ZoneV2GiveCommand.TryParse(bypass.Bypass, out giveItemId, out giveCount))
        {
            ZoneV2Client.Ensure().SendGiveItem(giveItemId, giveCount);
            return true;
        }

        // Other Interlude game commands are no-ops on ZoneV2 until wired.
        return true;
    }

    public void BeginSession(string account, string password)
    {
        if (!FineageWireConfig.IsZoneV2)
            return;

        ZoneV2Client.Ensure().BeginSession(account, password);
    }

    public void SendMove(Vector3 fromUnity, Vector3 toUnity)
    {
        if (!FineageWireConfig.IsZoneV2)
            return;

        ZoneV2Client.Ensure().SendMove(fromUnity, toUnity);
    }

    public void Disconnect()
    {
        if (!FineageWireConfig.IsZoneV2)
            return;

        ZoneV2Client zone = ZoneV2Client.Instance;
        if (zone != null)
            zone.Disconnect();
    }

    private static ZoneV2Client ActiveZoneClient
    {
        get { return FineageWireConfig.IsZoneV2 ? ZoneV2Client.Instance : null; }
    }
}
