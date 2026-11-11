using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClickManager
{
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
