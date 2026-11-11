using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public abstract partial class AbstractGetCache
{
    public static int RACE_COUNT = Enum.GetValues(typeof(CharacterRaceAnimation)).Length;
    protected static int FACE_COUNT = 6;
    protected static int HAIR_STYLE_COUNT = 6;
    protected static int HAIR_COLOR_COUNT = 4;

    protected Dictionary<string, GameObject> _weapons;
    protected Dictionary<string, GameObject> _etcItems;
    protected Dictionary<string, GameObject> _loadedModels;
    protected Dictionary<string, L2Npc> _npcs;
    protected Dictionary<string, L2Armor> _armors;

    protected GameObject[] _playerContainers;
    protected GameObject[] _userContainers;
    protected GameObject[] _pawnContainers;
    protected GameObject[,] _faces;
    protected GameObject[,] _hair;

    /// <summary>One skinned mesh + material for a body region (chest/legs/gloves/feet).</summary>
    public readonly struct BodyArmorMeshPart
    {
        public GameObject Mesh { get; }
        public Material Material { get; }
        public ItemSlot PartSlot { get; }

        public BodyArmorMeshPart(GameObject mesh, Material material, ItemSlot partSlot)
        {
            Mesh = mesh;
            Material = material;
            PartSlot = partSlot;
        }
    }

    class L2ArmorPiece
    {
        public GameObject baseArmorModel;
        public Material material;
        public GameObject[] baseAllModels;
        public Material[] allMaterials;

        public L2ArmorPiece(
            GameObject baseArmorModel,
            Material material,
            GameObject[] baseAllModels,
            Material[] allMaterials)
        {
            this.baseArmorModel = baseArmorModel;
            this.material = material;
            this.baseAllModels = baseAllModels;
            this.allMaterials = allMaterials;
        }
    }

    protected class L2Armor
    {
        public GameObject baseModel;
        public GameObject[] allModels;
        public Dictionary<string, Material> materials;
        public Dictionary<string, Material[]> allMaterials;
    }

    protected class L2Npc
    {
        public L2Npc(GameObject baseModel , Dictionary<string, Material[]> materials)
        {
            this.baseModel = baseModel;
            this.allMaterials = materials;
        }
        public GameObject baseModel;
        public Dictionary<string, Material[]> allMaterials;
    }

    // -------
    // Getters
    // -------

    /// <summary>
    /// Resolve cached mesh/material parts for a body armor item. Gear code should use this,
    /// not the private L2ArmorPiece cache shape.
    /// </summary>
}
