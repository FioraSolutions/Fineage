using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class TownMapOverlay
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


    public bool IsOpen => _openTown != null;

    /// <summary>Starter town containing the given L2 XY, or null if outside every plan square.</summary>

    /// <summary>Open the plan for the hero's current starter village when they stand inside one.</summary>




}
