using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Town detail maps for the world map window.
///
/// Interlude ships one 512x512 regional plan per starter town inside <c>systextures/town_map.utx</c>
/// (<c>town_map_elf_t00</c>, <c>town_map_darkelf_t00</c>, <c>town_map_dwarf_t00</c>,
/// <c>town_map_orc_t00</c>, <c>town_map_talking_t00</c>). The original client shows a "+" button
/// next to the town icon and swaps the viewport to that plan; the same art is imported under
/// <c>Data/UI/Map/Town</c> here.
///
/// The plan art is a region around the town rather than a 1:1 street survey, so the L2 rectangle is
/// centred on the town's teleporter landing point with a fixed half-size. The pin is then exact
/// inside that square.
/// </summary>
public sealed class TownMapOverlay
{
    public sealed class Town
    {
        public string id;
        public string title;
        public string texturePath;
        public float l2X;
        public float l2Y;
        public float halfSize = 3200f;

        public float MinX => l2X - halfSize;
        public float MaxX => l2X + halfSize;
        public float MinY => l2Y - halfSize;
        public float MaxY => l2Y + halfSize;
    }

    // Landing points from data/teleports.json (Elven Village / Dark Elven / Dwarven / Orc /
    // Talking Island). Dark Elven has no gatekeeper entry in that file, so its centre is the
    // village spawn used by the classic client.
    static readonly Town[] Towns =
    {
        new Town
        {
            id = "elf", title = "Elven Village", texturePath = "Data/UI/Map/Town/town_map_elf_t00",
            l2X = 46890f, l2Y = 51531f,
        },
        new Town
        {
            id = "darkelf", title = "Dark Elven Village", texturePath = "Data/UI/Map/Town/town_map_darkelf_t00",
            l2X = 27000f, l2Y = 11000f,
        },
        new Town
        {
            id = "dwarf", title = "Dwarven Village", texturePath = "Data/UI/Map/Town/town_map_dwarf_t00",
            l2X = 115120f, l2Y = -178224f,
        },
        new Town
        {
            id = "orc", title = "Orc Village", texturePath = "Data/UI/Map/Town/town_map_orc_t00",
            l2X = -45186f, l2Y = -112459f,
        },
        new Town
        {
            id = "talking", title = "Talking Island Village", texturePath = "Data/UI/Map/Town/town_map_talking_t00",
            l2X = -84141f, l2Y = 244623f,
        },
    };

    readonly VisualElement _map;
    readonly VisualElement _layer;
    readonly VisualElement _layerImage;
    readonly VisualElement _layerMarker;
    readonly Label _layerTitle;

    Town _openTown;

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

    public bool IsOpen => _openTown != null;

    /// <summary>Starter town containing the given L2 XY, or null if outside every plan square.</summary>
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

    /// <summary>Open the plan for the hero's current starter village when they stand inside one.</summary>
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

    static Vector2 WorldToMapPixel(float l2X, float l2Y)
    {
        float nx = Mathf.InverseLerp(MapPanner.MapL2MinX, MapPanner.MapL2MaxX, l2X);
        float ny = Mathf.InverseLerp(MapPanner.MapL2MinY, MapPanner.MapL2MaxY, l2Y);
        return new Vector2(nx * MapPanner.MapWidthPixels, ny * MapPanner.MapHeightPixels);
    }

    public void Open(Town town)
    {
        if (town == null || _layer == null)
        {
            return;
        }

        _openTown = town;

        if (_layerImage != null)
        {
            _layerImage.style.backgroundImage =
                new StyleBackground(Resources.Load<Texture2D>(town.texturePath));
        }

        if (_layerTitle != null)
        {
            _layerTitle.text = town.title;
        }

        _layer.style.display = DisplayStyle.Flex;
        UpdateMarker();
    }

    public void Close()
    {
        _openTown = null;
        if (_layer != null)
        {
            _layer.style.display = DisplayStyle.None;
        }
    }

    public void UpdateMarker()
    {
        if (_openTown == null || _layerImage == null || _layerMarker == null)
        {
            return;
        }

        Vector3 worldPos = ControllerPacketServices.Player != null
            ? ControllerPacketServices.Player.GetPlayerPosition()
            : Vector3.zero;

        float l2X = worldPos.z * 52.5f;
        float l2Y = worldPos.x * 52.5f;

        float w = _layerImage.layout.width;
        float h = _layerImage.layout.height;
        if (w <= 0f || h <= 0f)
        {
            return;
        }

        float nx = Mathf.Clamp01(Mathf.InverseLerp(_openTown.MinX, _openTown.MaxX, l2X));
        float ny = Mathf.Clamp01(Mathf.InverseLerp(_openTown.MinY, _openTown.MaxY, l2Y));

        _layerMarker.style.left = nx * w - _layerMarker.layout.width * 0.5f;
        _layerMarker.style.top = ny * h - _layerMarker.layout.height * 0.5f;
    }
}
