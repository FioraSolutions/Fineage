using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public abstract class AbstractGetCache
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
    public bool TryGetBodyArmorParts(
        Armor armor,
        CharacterRaceAnimation raceId,
        ItemSlot bodySlot,
        out BodyArmorMeshPart[] parts)
    {
        parts = Array.Empty<BodyArmorMeshPart>();
        L2ArmorPiece piece = GetArmorPiece(armor, raceId);
        if (piece == null)
            return false;

        parts = BuildBodyArmorParts(bodySlot, piece);
        return parts.Length > 0;
    }

    L2ArmorPiece GetArmorPiece(Armor armor, CharacterRaceAnimation raceId)
    {
        if (armor == null)
            return null;

        if (armor.Armorgrp == null || armor.Armorgrp.FirstModel == null)
            return null;

        byte raceIndex = (byte)raceId;
        if (raceIndex >= armor.Armorgrp.FirstModel.Length)
            return null;

        string model = armor.Armorgrp.FirstModel[raceIndex];
        if (string.IsNullOrEmpty(model))
            return null;

        if (!_armors.TryGetValue(model, out var l2Armor))
            return null;

        if (l2Armor.baseModel == null)
            return null;

        string textureName = GetValidTextureName(armor, raceId);
        if (textureName == null)
            return null;

        if (!TryGetMaterials(l2Armor, textureName, out var firstMaterial, out var allMaterials))
            return null;

        return new L2ArmorPiece(l2Armor.baseModel, firstMaterial, l2Armor.allModels, allMaterials);
    }

    static BodyArmorMeshPart[] BuildBodyArmorParts(ItemSlot bodySlot, L2ArmorPiece piece)
    {
        if (bodySlot == ItemSlot.fullarmor)
            return BuildFullArmorParts(piece);

        if (piece.baseArmorModel == null || piece.material == null)
            return Array.Empty<BodyArmorMeshPart>();

        return new[]
        {
            new BodyArmorMeshPart(piece.baseArmorModel, piece.material, bodySlot)
        };
    }

    static BodyArmorMeshPart[] BuildFullArmorParts(L2ArmorPiece piece)
    {
        GameObject[] models = piece.baseAllModels;
        if (models == null || models.Length == 0)
        {
            if (piece.baseArmorModel == null)
                return Array.Empty<BodyArmorMeshPart>();
            models = new[] { piece.baseArmorModel };
        }

        Material[] materials = piece.allMaterials;
        var list = new List<BodyArmorMeshPart>(models.Length);
        for (int i = 0; i < models.Length; i++)
        {
            GameObject mesh = models[i];
            if (mesh == null)
                continue;

            Material mat = null;
            if (materials != null && i < materials.Length)
                mat = materials[i];
            if (mat == null && materials != null && materials.Length > 0)
                mat = materials[0];
            if (mat == null)
                mat = piece.material;
            if (mat == null)
                continue;

            ItemSlot partSlot = i == 0 ? ItemSlot.chest : ItemSlot.legs;
            list.Add(new BodyArmorMeshPart(mesh, mat, partSlot));
        }

        return list.ToArray();
    }

    private string GetValidTextureName(Armor armor, CharacterRaceAnimation raceId)
    {
        byte raceIndex = (byte)raceId;
        string texture = null;
        string[] textureArray = armor.Armorgrp.FirstTexture;
        if (textureArray != null && raceIndex < textureArray.Length)
            texture = textureArray[raceIndex];

        // Onepiece robes: FirstTexture used to be wrongly parsed as "None" (legs slot).
        // Fall back to the first real entry in AllTextures for this race.
        if (IsMissingArmorToken(texture) &&
            armor.Armorgrp.AllTextures != null &&
            raceIndex < armor.Armorgrp.AllTextures.Count)
        {
            List<string> all = armor.Armorgrp.AllTextures[raceIndex];
            if (all != null)
            {
                for (int i = 0; i < all.Count; i++)
                {
                    if (!IsMissingArmorToken(all[i]))
                    {
                        texture = all[i];
                        break;
                    }
                }
            }
        }

        return IsMissingArmorToken(texture) ? null : texture;
    }

    static bool IsMissingArmorToken(string token)
    {
        return string.IsNullOrEmpty(token) ||
               string.Equals(token, "None", StringComparison.OrdinalIgnoreCase);
    }

    private bool TryGetMaterials(L2Armor l2Armor, string textureName, out Material firstMaterial, out Material[] allMaterials)
    {
        firstMaterial = null;
        allMaterials = null;

        if (l2Armor.materials == null)
            l2Armor.materials = new Dictionary<string, Material>();

        l2Armor.materials.TryGetValue(textureName, out firstMaterial);
        if (firstMaterial == null)
        {
            firstMaterial = LoadArmorMaterial(textureName);
            if (firstMaterial != null)
                l2Armor.materials[textureName] = firstMaterial;
        }

        if (l2Armor.allMaterials != null)
            l2Armor.allMaterials.TryGetValue(textureName, out allMaterials);

        if (firstMaterial == null)
        {
            // Debug.LogWarning($"Can't find armor material for model: {textureName} in ModelTable");
            return false;
        }

        // Onepiece robes ship texture={t00;[None]} — cache may omit allMaterials when the
        // first LoadArmorMaterial failed earlier. Synthesize so EquipFullArmor still dresses.
        if (allMaterials == null || allMaterials.Length == 0)
        {
            allMaterials = new[] { firstMaterial };
            if (l2Armor.allMaterials == null)
                l2Armor.allMaterials = new Dictionary<string, Material[]>();
            l2Armor.allMaterials[textureName] = allMaterials;
        }

        return true;
    }

    protected Material LoadArmorMaterial(string texture)
    {
        if (string.IsNullOrEmpty(texture))
            return null;
        if (string.Equals(texture, "None", StringComparison.OrdinalIgnoreCase))
            return null;

        string[] parts = texture.Split('.');
        if (parts.Length < 2)
            return null;

        string folder = parts[0];
        string file = parts[1];
        // Dat packs use lowercase packages (mmagic.mmagic_m007_t90_u); Resources
        // folders are PascalCase (MMagic/Materials/MMagic_m007_t90_u). Unity
        // Resources.Load is case-sensitive even on macOS APFS.
        string canonFolder = CanonicalSysTexturePackage(folder);
        string canonFile = file;
        if (!string.IsNullOrEmpty(folder) &&
            file.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
            canonFile = canonFolder + file.Substring(folder.Length);

        string[] folders =
        {
            folder,
            canonFolder,
            folder.Replace("NPCs", "Npcs"),
            folder.Replace("Npcs", "NPCs")
        };
        string[] files = { file, canonFile };

        for (int f = 0; f < folders.Length; f++)
        {
            string fold = folders[f];
            if (string.IsNullOrEmpty(fold))
                continue;
            for (int n = 0; n < files.Length; n++)
            {
                string asset = files[n];
                if (string.IsNullOrEmpty(asset))
                    continue;
                string[] candidates =
                {
                    $"Data/SysTextures/{fold}/Materials/{asset}",
                    $"Data/SysTextures/{fold}/Materials/{fold}.{asset}",
                    $"Data/SysTextures/{fold}/Materials/{texture}",
                    // Fresh NPC imports often drop .mat next to the TGA, not under Materials/.
                    $"Data/SysTextures/{fold}/{asset}",
                    $"Data/SysTextures/{fold}/{texture}"
                };

                for (int i = 0; i < candidates.Length; i++)
                {
                    Material material = Resources.Load<Material>(candidates[i]);
                    if (material != null)
                        return material;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Maps armorgrp/weapongrp SysTextures package names to on-disk Resources folders.
    /// </summary>
    protected static string CanonicalSysTexturePackage(string folder)
    {
        if (string.IsNullOrEmpty(folder))
            return folder;
        switch (folder.ToLowerInvariant())
        {
            case "mmagic": return "MMagic";
            case "fmagic": return "FMagic";
            case "mfighter": return "MFighter";
            case "ffighter": return "FFighter";
            case "melf": return "MElf";
            case "felf": return "FElf";
            case "mdarkelf": return "MDarkElf";
            case "fdarkelf": return "FDarkElf";
            case "mdwarf": return "MDwarf";
            case "fdwarf": return "FDwarf";
            case "morc": return "MOrc";
            case "forc": return "FOrc";
            case "mshaman": return "MShaman";
            case "fshaman": return "FShaman";
            case "dropitemstex": return "DropItemsTex";
            case "lineagenpcstex": return "LineageNpcsTex";
            case "lineageweaponstex": return "LineageWeaponsTex";
            default:
                return char.ToUpperInvariant(folder[0]) + folder.Substring(1);
        }
    }

    protected GameObject LoadUkxPrefab(string model)
    {
        if (string.IsNullOrEmpty(model))
            return null;

        string[] parts = model.Split('.');
        if (parts.Length < 2)
            return null;

        string pkg = parts[0];
        string asset = parts[1];

        GameObject go = Resources.Load<GameObject>($"Data/Animations/{pkg}/{asset}");
        if (go != null)
            return go;

        go = Resources.Load<GameObject>($"Data/Animations/{pkg}/{asset}/{asset}");
        if (go != null)
            return go;

        go = Resources.Load<GameObject>($"Data/StaticMeshes/{pkg}/{asset}");
        if (go != null)
            return go;

        return Resources.Load<GameObject>($"Data/StaticMeshes/{pkg}/{asset}/{asset}");
    }


    public GameObject GetWeaponById(int itemId)
    {
        Weapon weapon = TablePacketServices.Items.GetWeapon(itemId);
        if (weapon == null)
        {
            // Debug.LogWarning($"Can't find weapon {itemId} in ItemTable");
        }

        return GetWeapon(weapon.Weapongrp.Model);
    }

    public GameObject GetEtcById(int itemId)
    {
        EtcItem etcItem = TablePacketServices.Items.GetEtcItem(itemId);
        if (etcItem == null)
        {
            // Debug.LogWarning($"Can't find etcItem  {itemId} in ItemTable");
        }

        return GetEtcItem(etcItem.EtcItemgrp.Model);
    }

    public GameObject GetWeapon(string model)
    {
        if (!_weapons.ContainsKey(model))
        {
            // Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        GameObject go = _weapons[model];
        if (go == null)
        {
            // Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        return go;
    }

    public GameObject GetEtcItem(string model)
    {
        if (!_etcItems.ContainsKey(model))
        {
            // Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        GameObject go = _etcItems[model];
        if (go == null)
        {
            // Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        return go;
    }

    public GameObject TryGetCachedItemModel(string model)
    {
        if (string.IsNullOrEmpty(model))
            return null;
        if (_weapons != null && _weapons.TryGetValue(model, out GameObject weapon) && weapon != null)
            return weapon;
        if (_etcItems != null && _etcItems.TryGetValue(model, out GameObject etc) && etc != null)
            return etc;
        if (_loadedModels != null && _loadedModels.TryGetValue(model, out GameObject extra) && extra != null)
            return extra;
        return null;
    }

    /// <summary>
    /// Cached weapon/etc prefab, or a lazily loaded Animations/StaticMeshes UKX.
    /// </summary>
    public GameObject GetOrLoadModel(string model)
    {
        if (string.IsNullOrEmpty(model) ||
            model.Equals("None", StringComparison.OrdinalIgnoreCase))
            return null;

        GameObject cached = TryGetCachedItemModel(model);
        if (cached != null)
            return cached;

        GameObject loaded = LoadUkxPrefab(model);
        if (loaded == null)
            return null;

        if (_loadedModels == null)
            _loadedModels = new Dictionary<string, GameObject>();
        _loadedModels[model] = loaded;
        return loaded;
    }

    public GameObject GetDropPrefab(int itemId)
    {
        if (App.GameContainer != null)
            return App.GameContainer.Resolve<ItemDropPrefabLoader>().Resolve(itemId);

        Abstractgrp grp = ResolveDropGrp(itemId);
        if (grp != null && !string.IsNullOrEmpty(grp.DropModel))
        {
            GameObject drop = GetOrLoadModel(grp.DropModel);
            if (drop != null)
                return drop;
        }

        string equip = grp is Weapongrp weapon ? weapon.Model
            : grp is EtcItemgrp etc ? etc.Model
            : null;
        return GetOrLoadModel(equip);
    }

    static Abstractgrp ResolveDropGrp(int itemId)
    {
        ItemTable items = TablePacketServices.Items;
        if (items == null)
            return null;
        Weapon weapon = items.GetWeapon(itemId);
        if (weapon != null && weapon.Weapongrp != null)
            return weapon.Weapongrp;
        Armor armor = items.GetArmor(itemId);
        if (armor != null && armor.Armorgrp != null)
            return armor.Armorgrp;
        EtcItem etc = items.GetEtcItem(itemId);
        return etc != null ? etc.EtcItemgrp : null;
    }

    public GameObject GetContainer(CharacterRaceAnimation raceId, EntityType entityType)
    {
        GameObject go = null;

        switch (entityType)
        {
            case EntityType.User:
                go = _userContainers[(byte)raceId];
                break;
            case EntityType.Player:
                go = _playerContainers[(byte)raceId];
                break;
            case EntityType.Pawn:
                go = _pawnContainers[(byte)raceId];
                break;
        }

        if (go == null)
        {
            // Debug.LogError($"Can't find container for race {raceId} and entity type {entityType}");
        }

        return go;
    }

    public GameObject GetFace(CharacterRaceAnimation raceId, byte face) {
        GameObject go = _faces[(byte)raceId, face];
        if (go == null) {
            // Debug.LogError($"Can't find face {face} for race {raceId} at index {raceId},{face}");
        }

        return go;
    }

    public GameObject GetHair(CharacterRaceAnimation raceId, byte hairStyle, byte hairColor, bool bh) {
        byte index = (byte)(hairStyle * 8 + hairColor * 2);
        index = GetRemappedIndex(raceId, index);
        if (bh) {
            index += 1;
        }

        //Debug.Log($"Loading hair[{index}] Race:{raceId} Model:{hairStyle}_{hairColor}_{(bh ? "bh" : "ah")}");

        GameObject go = _hair[(byte)raceId, index];
        if (go == null) {
            // Debug.LogError($"Can't find hairstyle {hairStyle} haircolor {hairColor} for race {raceId} at index {raceId},{index}");
        }

        return go;
    }

    private byte GetRemappedIndex(CharacterRaceAnimation raceId, byte index) {
        if (CharacterRaceAnimation.FFighter == raceId) {
            return index switch {
                8 => 6,
                10 => 8,
                12 => 10,
                _ => index
            };
        }
        return index;
    }

    public GameObject GetNpc(string meshname)
    {
        L2Npc npc = null;
        _npcs.TryGetValue(meshname, out npc);
        
        if (npc == null)
        {
            // Debug.LogWarning($"Can't find npc {meshname} model in ModelTable.");
            return null;
        }

        AddMaterial(npc);
        
        return npc.baseModel;
    }

    private void AddMaterial(L2Npc npc)
    {
        if (npc.allMaterials.Count == 0 || npc.baseModel == null)
            return;

        Material[] materials = npc.allMaterials.Values.First();
        if (materials == null || materials.Length == 0)
            return;

        SkinnedMeshRenderer[] renderers = npc.baseModel.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        SkinnedMeshRenderer fallback = null;
        int fallbackSlots = -1;
        bool assigned = false;
        int emptyCount = 0;
        for (int r = 0; r < renderers.Length; r++)
        {
            if (renderers[r] == null)
                continue;
            int slots = renderers[r].sharedMaterials != null ? renderers[r].sharedMaterials.Length : 0;
            if (slots == 0)
                emptyCount++;
        }

        int filledEmpty = 0;
        for (int r = 0; r < renderers.Length; r++)
        {
            SkinnedMeshRenderer renderer = renderers[r];
            if (renderer == null)
                continue;
            int slotCount = renderer.sharedMaterials != null ? renderer.sharedMaterials.Length : 0;
            if (slotCount > fallbackSlots)
            {
                fallback = renderer;
                fallbackSlots = slotCount;
            }
            // Fresh PSK→FBX imports often arrive with zero material slots
            // (materialImportMode=None). One mesh → dump the full npcgrp set;
            // several empty SMRs → one texture each in order.
            if (slotCount == 0)
            {
                if (emptyCount <= 1)
                    renderer.sharedMaterials = materials;
                else if (filledEmpty < materials.Length && materials[filledEmpty] != null)
                {
                    renderer.sharedMaterial = materials[filledEmpty];
                    filledEmpty++;
                }
                else
                    continue;
                renderer.enabled = true;
                assigned = true;
                continue;
            }
            if (slotCount >= materials.Length && ApplyNpcMaterials(renderer, materials))
                assigned = true;
        }

        if (!assigned && fallback != null)
            ApplyNpcMaterials(fallback, materials);
    }

    static bool ApplyNpcMaterials(SkinnedMeshRenderer renderer, Material[] materials)
    {
        if (renderer == null || materials == null || materials.Length == 0)
            return false;

        Material[] slots = renderer.sharedMaterials;
        if (slots == null || slots.Length == 0)
        {
            renderer.sharedMaterials = materials;
            renderer.enabled = true;
            return true;
        }

        int count = slots.Length < materials.Length ? slots.Length : materials.Length;
        Material[] next = new Material[slots.Length];
        bool changed = false;
        for (int i = 0; i < slots.Length; i++)
            next[i] = slots[i];
        for (int i = 0; i < count; i++)
        {
            if (materials[i] == null || next[i] == materials[i])
                continue;
            next[i] = materials[i];
            changed = true;
        }

        if (changed)
            renderer.sharedMaterials = next;
        return changed || count > 0;
    }

 
}
