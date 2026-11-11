using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class TownMapOverlay
{
    public TownMapOverlay(VisualElement map, VisualElement layer)
    {
        _map = map;
        _layer = layer;
        _layerImage = layer?.Q<VisualElement>("town-map-image");
        _layerMarker = layer?.Q<VisualElement>("town-map-marker");
        _layerTitle = layer?.Q<Label>("town-map-title");

        VisualElement close = layer?.Q<VisualElement>("town-map-close");
        if (close != null)
        {
            close.RegisterCallback<ClickEvent>(_ => Close());
        }

        // The "+" entry point is parked: the first attempt drew a raw placeholder over the world
        // map art. Open()/Close() stay usable so the button can be reintroduced once the icon and
        // its placement are designed properly (#139).
    }

    public static Town FindTownAt(float l2X, float l2Y)
    {
        for (int i = 0; i < Towns.Length; i++)
        {
            Town town = Towns[i];
            if (l2X >= town.MinX && l2X <= town.MaxX &&
                l2Y >= town.MinY && l2Y <= town.MaxY)
                return town;
        }

        return null;
    }

    public bool TryOpenForPlayer()
    {
        PlayerController player = ControllerPacketServices.Player;
        if (player == null)
            return false;

        Vector3 worldPos = player.GetPlayerPosition();
        float l2X = worldPos.z * 52.5f;
        float l2Y = worldPos.x * 52.5f;
        Town town = FindTownAt(l2X, l2Y);
        if (town == null)
            return false;

        Open(town);
        return true;
    }
}
