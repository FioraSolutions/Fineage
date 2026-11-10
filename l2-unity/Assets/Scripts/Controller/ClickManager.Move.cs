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

    static bool TryResolveMoveOrigin(out Vector3 from)
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null)
        {
            from = player.transform.position;
            return true;
        }

        PlayerController controller = ControllerPacketServices.Player;
        if (controller != null)
        {
            from = controller.transform.position;
            return true;
        }

        from = default;
        return false;
    }

    public void PlaceLocator(Vector3 position)
    {
        EnsureLocator();
        if (_locator == null)
        {
            Debug.LogWarning("[MoveMark] locator prefab not loaded");
            return;
        }

        ClickMoveLocator mark = _locator.GetComponent<ClickMoveLocator>();
        if (mark != null)
        {
            mark.PlaceAt(position);
            return;
        }

        _locator.SetActive(true);
        _locator.transform.position = position;
    }

    public void HideLocator()
    {
        if (_locator == null)
            return;

        _locator.SetActive(false);
    }

}
