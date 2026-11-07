using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ModelTable;

public class UserGear : Gear
{
    const string ModelChildName = "model";

    protected SkinnedMeshSync _skinnedMeshSync;
    [Header("Armors")]
    [Header("Meta")]

    [Header("Models")]
    [SerializeField] private GameObject _container;

    [System.NonSerialized] public GameObject Hair1;
    [System.NonSerialized] public GameObject Hair2;
    [System.NonSerialized] public GameObject Face;

    private CharacterArmorDresser _armorDresser;

    public override void Initialize(int ownderId, CharacterRaceAnimation raceId)
    {
        base.Initialize(ownderId, raceId);

        // Prefer the `model` child (lobby pawns / rebuilt race prefabs). Never parent gear
        // under the anim root itself — that hides SMRs from SkinnedMeshSync sibling scan.
        _container = ResolveGearContainer();

        if (_armorDresser == null)
        {
            _armorDresser = new CharacterArmorDresser(_container.transform);
            _armorDresser.OnDestroyGameObject += OnDestroyGameObject;
            _armorDresser.OnSyncMash += OnSyncMesh;
            _armorDresser.OnAddSyncMash += OnAddSyncMesh;
            _armorDresser.OnEquipArmor += OnEquipArmor;
        }

        // Anim + SkinnedMeshSync sit under model; resolve from the pawn root.
        _skinnedMeshSync = GetComponentInChildren<SkinnedMeshSync>(true);
    }

    GameObject ResolveGearContainer()
    {
        Transform model = transform.Find(ModelChildName);
        if (model != null)
            return model.gameObject;

        return gameObject;
    }

    /// <summary>Final bone remap after paperdoll equip (lobby + world).</summary>
    public void SyncSkinnedMeshes()
    {
        if (_skinnedMeshSync == null)
            _skinnedMeshSync = GetComponentInChildren<SkinnedMeshSync>(true);
        _skinnedMeshSync?.SyncMesh();
    }

    public void UnequipArmor(int itemId, ItemSlot slot)
    {
        int race = (int)_raceId;
        GetDefaultGoWithArmorModel(slot, out Armor[] defaultArmor, out GameObject[] listArmorPiece , (int)_raceId);

        if (listArmorPiece != null && listArmorPiece.Length > 0)
        {
            _armorDresser.UnequipArmorPiece(slot, itemId, defaultArmor, listArmorPiece);
        }

    }

    public void EquipArmor(int itemId, ItemSlot slot)
    {
        Armor armor = TablePacketServices.Items.GetArmor(itemId);
        if (armor == null)
        {
            // Unknown paperdoll id (e.g. starter kit piece not in client table): fall back to naked
            // for that slot so races don't spawn as gingerbread (head/gloves/boots only).
            if (itemId != 0)
            {
                Debug.LogWarning("[UserGear] EquipArmor missing ItemTable id=" + itemId +
                    " slot=" + slot + " race=" + _raceId + " — using naked default");
                GetDefaultGoWithArmorModel(slot, out Armor[] defaultArmor, out GameObject[] listArmorPiece, (int)_raceId);
                _armorDresser.EquipDefaultSlot(slot, defaultArmor, listArmorPiece);
            }
            return;
        }

        ItemSlot slotArmor = ResolveArmorSlot(armor, slot);
        // GearFlowLog.Info("EquipArmor id=" + itemId +
            // " askedSlot=" + slot +
            // " resolved=" + slotArmor +
            // " body=" + (armor.Armorgrp != null ? armor.Armorgrp.BodyPart.ToString() : "null"));
        if (ItemSlot.fullarmor != slotArmor) {
            EquipSingleArmor(armor, slotArmor, itemId);
        } else {
            EquipFullArmor(armor, slotArmor, itemId);
        }
    }

    /// <summary>
    /// CharInfo / spawn paperdoll: chest/legs/gloves/feet. Full-body chest skips legs.
    /// Old piece is replaced by <see cref="CharacterArmorDresser.EquipNewArmor"/>.
    /// </summary>
    public void SyncEquippedArmor(PlayerAppearance appearance)
    {
        if (appearance == null)
        {
            // GearFlowLog.Warn("SyncArmor abort appearance=null");
            return;
        }

        int chestId = appearance.Chest != 0 ? appearance.Chest : ItemTable.NAKED_CHEST;
        int glovesId = appearance.Gloves != 0 ? appearance.Gloves : ItemTable.NAKED_GLOVES;
        int feetId = appearance.Feet != 0 ? appearance.Feet : ItemTable.NAKED_BOOTS;

        Armor chestArmor = TablePacketServices.Items.GetArmor(chestId);
        bool fullBody = chestArmor != null && chestArmor.Armorgrp != null &&
            chestArmor.Armorgrp.BodyPart == ItemSlot.fullarmor;

        Debug.Log(
            "[Gear] SyncArmor race=" + _raceId +
            " chestId=" + chestId +
            " gloves=" + glovesId +
            " feet=" + feetId +
            " fullBody=" + fullBody +
            " bodyPart=" + (chestArmor?.Armorgrp != null ? chestArmor.Armorgrp.BodyPart.ToString() : "no-armor"));

        EquipArmor(chestId, fullBody ? ItemSlot.fullarmor : ItemSlot.chest);

        if (!fullBody)
        {
            int legsId = appearance.Legs != 0 ? appearance.Legs : ItemTable.NAKED_LEGS;
            EquipArmor(legsId, ItemSlot.legs);
        }

        EquipArmor(glovesId, ItemSlot.gloves);
        EquipArmor(feetId, ItemSlot.feet);
    }

    static ItemSlot ResolveArmorSlot(Armor armor, ItemSlot fallback)
    {
        ItemSlot body = armor.Armorgrp != null ? armor.Armorgrp.BodyPart : ItemSlot.none;
        body = ArmorDresserModel.GetExtendedArmorPart(body);
        if (body == ItemSlot.fullarmor || body == ItemSlot.chest || body == ItemSlot.legs ||
            body == ItemSlot.gloves || body == ItemSlot.feet)
            return body;
        return ArmorDresserModel.GetExtendedArmorPart(fallback);
    }

    private void EquipFullArmor(Armor armor, ItemSlot slotArmor, int itemId)
    {
        if (_armorDresser.IsArmorEquipped(armor, slotArmor))
        {
            Debug.Log("[Gear] EquipFullArmor SKIP already id=" + itemId + " race=" + _raceId);
            return;
        }

        L2ArmorPiece armorPiece = (L2ArmorPiece)LoadMesh(EquipmentCategory.FullArmor, itemId, (int)_raceId);
        if (!ValidateArmorPieceFullArmor(armorPiece, itemId))
        {
            string tex = armor?.Armorgrp?.FirstTexture != null &&
                         (byte)_raceId < armor.Armorgrp.FirstTexture.Length
                ? armor.Armorgrp.FirstTexture[(byte)_raceId]
                : "?";
            string model = armor?.Armorgrp?.FirstModel != null &&
                           (byte)_raceId < armor.Armorgrp.FirstModel.Length
                ? armor.Armorgrp.FirstModel[(byte)_raceId]
                : "?";
            Debug.LogWarning(
                "[Gear] EquipFullArmor INVALID id=" + itemId +
                " race=" + _raceId +
                " piece=" + (armorPiece != null) +
                " models=" + (armorPiece?.baseAllModels != null) +
                " mats=" + (armorPiece?.allMaterials != null) +
                " modelStr=" + model +
                " texStr=" + tex);
            ReportMissingArmorAndReset(armor, slotArmor, itemId);
            return;
        }

        try
        {
            GameObject[] listGo = CreateListArmorMesh(armorPiece.baseAllModels, armorPiece.allMaterials);
            int n = listGo != null ? listGo.Length : 0;
            int ok = 0;
            if (listGo != null)
            {
                for (int i = 0; i < listGo.Length; i++)
                    if (listGo[i] != null) ok++;
            }
            Debug.Log(
                "[Gear] EquipFullArmor APPLY id=" + itemId +
                " race=" + _raceId +
                " slots=" + n +
                " meshed=" + ok);

            // Magic onepiece robes (Major Arcana / Dark Crystal) only ship an upper mesh;
            // legs slot in armorgrp is None — still dress the chest piece.
            if (listGo != null && listGo.Length >= 1 && listGo[0] != null)
            {
                GameObject goChest = listGo[0];
                GameObject goLegs = listGo.Length > 1 ? listGo[1] : null;

                // refresh=true on the last piece so skinned mesh rebuilds once.
                if (goLegs != null)
                {
                    _armorDresser.SetFullArmor(false, armor, goChest, ItemSlot.chest);
                    _armorDresser.SetFullArmor(true, armor, goLegs, ItemSlot.legs);
                }
                else
                {
                    _armorDresser.SetFullArmor(true, armor, goChest, ItemSlot.chest);
                }
            }
            else
            {
                Debug.LogWarning("[Gear] EquipFullArmor no chest mesh id=" + itemId);
                ReportMissingArmorAndReset(armor, slotArmor, itemId);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[Gear] EquipFullArmor exception id=" + itemId + " " + ex.Message);
            ReportMissingArmorAndReset(armor, slotArmor, itemId);
        }
    }

    private void EquipSingleArmor(Armor armor , ItemSlot slotArmor , int itemId)
    {
        if (_armorDresser.IsArmorEquipped(armor, slotArmor))
        {
            // GearFlowLog.Info("EquipSingleArmor SKIP already equipped id=" + itemId + " slot=" + slotArmor);
            return;
        }

        L2ArmorPiece armorPiece = (L2ArmorPiece)LoadMesh(EquipmentCategory.Armor, itemId, (int)_raceId);
        if (!ValidateArmorPiece(armorPiece, itemId))
        {
            ReportMissingArmorAndReset(armor, slotArmor, itemId);
            return;
        }

        // GearFlowLog.Info("EquipSingleArmor APPLY id=" + itemId + " slot=" + slotArmor);

        try
        {
            GetDefaultGoWithArmorModel(ItemSlot.fullarmor, out Armor[] defaultArmor, out GameObject[] listArmorPiece , (int)_raceId);

            GameObject armorMesh = CreateArmorMesh(armorPiece.baseArmorModel, armorPiece.material);
            if (armorMesh != null)
            {
                _armorDresser.SetArmorPiece(armor, armorMesh, slotArmor , defaultArmor, listArmorPiece);

            }
            else
            {
                ReportMissingArmorAndReset(armor, slotArmor, itemId);
            }
        }
        catch (System.Exception)
        {
            ReportMissingArmorAndReset(armor, slotArmor, itemId);
        }
    }

    void ReportMissingArmorAndReset(Armor armor, ItemSlot slotArmor, int itemId)
    {
        EquipmentMeshLog.Once("armor", itemId);
        // string visual = GearFlowLog.ArmorVisual(armor, _raceId, itemId, slotArmor);
        // GearFlowLog.Info("Missing mesh/texture, reset to naked " + visual);

        GetDefaultGoWithArmorModel(slotArmor, out Armor[] defaultArmor, out GameObject[] listArmorPiece, (int)_raceId);
        _armorDresser.EquipDefaultSlot(slotArmor, defaultArmor, listArmorPiece);
    }

    /// <summary>
    /// Validates the armor piece data
    /// </summary>
    private bool ValidateArmorPiece(L2ArmorPiece armorPiece, int itemId)
    {
        if (armorPiece == null || armorPiece.baseArmorModel == null || armorPiece.material == null)
        {
            // Debug.LogWarning($"UserGear-> Invalid armor data for item {itemId}");
            return false;
        }
        return true;
    }

    private bool ValidateArmorPieceFullArmor(L2ArmorPiece armorPiece, int itemId)
    {
        if (armorPiece == null || armorPiece.baseAllModels == null)
            return false;

        // Magic onepiece: only the upper mesh is real; materials may be a single-slot array.
        for (int i = 0; i < armorPiece.baseAllModels.Length; i++)
        {
            if (armorPiece.baseAllModels[i] != null)
                return true;
        }

        return false;
    }
    
    private GameObject[] CreateListArmorMesh(GameObject[] baseListArmorModel, Material[] materials)
    {
        if (baseListArmorModel == null)
            return System.Array.Empty<GameObject>();

        GameObject[] listGo = new GameObject[baseListArmorModel.Length];
        for (int i = 0; i < baseListArmorModel.Length; i++)
        {
            GameObject baseArmorModel = baseListArmorModel[i];
            if (baseArmorModel == null)
                continue;
            Material material = materials != null && i < materials.Length ? materials[i] : null;
            if (material == null && materials != null && materials.Length > 0)
                material = materials[0];
            listGo[i] = CreateArmorMesh(baseArmorModel, material);
        }
        return listGo;
    }

    protected override Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = transform.FindRecursive("Weapon_L_Bone");
        }

        return _leftHandBone;
    }

    protected override Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = transform.FindRecursive("Weapon_R_Bone");
        }
        return _rightHandBone;
    }

    protected override Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = transform.FindRecursive("Shield_L_Bone");
        }
        return _shieldBone;
    }

    public void EquipHair(GameObject hair1Piece, GameObject hair2Piece)
    {
        EquipHairTest(hair1Piece, hair2Piece);
    }
    
    public void EquipHairTest(GameObject hair1Piece , GameObject hair2Piece)
    {
        if (Hair1 != null)
            DestroyImmediate(Hair1);
        if (Hair2 != null)
            DestroyImmediate(Hair2);

        Hair1 = null;
        Hair2 = null;
        var tr = _container.transform;
        Hair1 = hair1Piece;
        Hair2 = hair2Piece;
        if (Hair1 != null)
            Hair1.transform.SetParent(tr, false);
        if (Hair2 != null)
            Hair2.transform.SetParent(tr, false);

        _skinnedMeshSync?.SyncMesh();
    }

    public void EquipFace(GameObject facePiece)
    {
        if (Face != null)
        {
            Destroy(Face);
            //_torsoMeta = null;
        }
        var tr = _container.transform;
        Face = facePiece;
        Face.transform.SetParent(tr, false);

       _skinnedMeshSync?.SyncMesh();
    }

    public void OnDestroyGameObject(GameObject go)
    {
        if(ObjectPoolManager.Instance != null)
        {
            if (!ObjectPoolManager.Instance.ReturnToPool(ObjectType.Armor , go))
            {
                Destroy(go);
            }
        }
        else
        {
            Destroy(go);
        }

        //Debug.LogWarning("Запрос на удаление. Удаление состоялось размер " + _container.transform.childCount);
    }

    public void OnSyncMesh(int status)
    {
        //Debug.LogWarning("Запрос на удаление. Синхронизация начало");
        _skinnedMeshSync?.SyncMesh();
        //Debug.LogWarning("Запрос на удаление. Синхронизация конец");
    }

    public void OnAddSyncMesh(GameObject add) {
        _skinnedMeshSync?.AddObjectToQueue(add);
    }

    public void OnEquipArmor(int naked, ItemSlot slot) {
        EquipArmor(naked , slot);
    }

    public void AddUserGearLink(GameObject face, GameObject hair1, GameObject hair2) {
        Face = face;
        Hair1 = hair1;
        Hair2 = hair2;
    }

}
