using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClickManager
{
    public void OnClickToMove(RaycastHit hit)
    {
        if (CastMoveLock.IsLocked)
        {
            return;
        }

        _lastClickPosition = hit.point;

        StopFollow();
        SendPacketMoveToLocation(_lastClickPosition);

        if (Targets != null)
        {
            Targets.ClearAttackTarget();
        }

        float angle = Vector3.Angle(hit.normal, Vector3.up);
        if (angle < 85f)
        {
            PlaceLocator(_lastClickPosition);
        }
        else
        {
            HideLocator();
        }
    }

    private void StopFollow()
    {
        if (PlayerStatePacketServices.PlayerState == null)
        {
            return;
        }

        PlayerStatePacketServices.PlayerState.Follow = null;
        PlayerStatePacketServices.PlayerState.IsMoveToPawn = false;
    }

    private void SendPacketMoveToLocation(Vector3 lastClickPosition)
    {
        IZoneTransport game = ZoneCommands.Transport;
        if (game == null)
            return;

        if (!TryResolveMoveOrigin(out Vector3 from))
            return;

        ZoneCommands.Send(new MoveToCommand(from, lastClickPosition));
    }
}
