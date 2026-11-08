using UnityEngine;

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
    ArmorVisualResolver _armorVisuals;

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
            // Unknown paperdoll id → naked for that slot (avoid gingerbread torso).
            if (itemId != 0)
                ResetSlotToNaked(slot, itemId);
            return;
        }

        ItemSlot slotArmor = ResolveEquipSlot(armor, slot);
        if (_armorDresser.IsArmorEquipped(armor, slotArmor))
            return;

        ArmorVisual visual = ArmorVisuals.Resolve(armor, _raceId);
        if (visual.IsMissing)
        {
            ResetSlotToNaked(slotArmor, itemId);
            return;
        }

        if (visual.Slot == ItemSlot.fullarmor)
            ApplyFullArmor(visual);
        else
            ApplySingleArmor(visual);
    }

    /// <summary>
    /// CharInfo / spawn paperdoll: chest/legs/gloves/feet. Full-body chest skips legs.
    /// </summary>
    public void SyncEquippedArmor(PlayerAppearance appearance)
    {
        if (appearance == null)
            return;

        int chestId = appearance.Chest != 0 ? appearance.Chest : ItemTable.NAKED_CHEST;
        int glovesId = appearance.Gloves != 0 ? appearance.Gloves : ItemTable.NAKED_GLOVES;
        int feetId = appearance.Feet != 0 ? appearance.Feet : ItemTable.NAKED_BOOTS;

        Armor chestArmor = TablePacketServices.Items.GetArmor(chestId);
        bool fullBody = chestArmor != null &&
            ArmorVisualResolver.ResolveBodySlot(chestArmor) == ItemSlot.fullarmor;

        EquipArmor(chestId, fullBody ? ItemSlot.fullarmor : ItemSlot.chest);

        if (!fullBody)
        {
            int legsId = appearance.Legs != 0 ? appearance.Legs : ItemTable.NAKED_LEGS;
            EquipArmor(legsId, ItemSlot.legs);
        }

        EquipArmor(glovesId, ItemSlot.gloves);
        EquipArmor(feetId, ItemSlot.feet);
    }

    ArmorVisualResolver ArmorVisuals =>
        _armorVisuals ??= ArmorVisualResolver.FromTables();

    static ItemSlot ResolveEquipSlot(Armor armor, ItemSlot fallback)
    {
        ItemSlot body = ArmorVisualResolver.ResolveBodySlot(armor);
        if (ArmorBodySlots.IsBody(body))
            return body;
        return ArmorBodySlots.Normalize(fallback);
    }

    void ApplyFullArmor(ArmorVisual visual)
    {
        Armor armor = visual.Armor;
        GameObject goChest = null;
        GameObject goLegs = null;

        for (int i = 0; i < visual.Parts.Length; i++)
        {
            ArmorVisualPart part = visual.Parts[i];
            GameObject mesh = CreateArmorMesh(part.Mesh, part.Material);
            if (mesh == null)
                continue;

            if (part.PartSlot == ItemSlot.legs)
                goLegs = mesh;
            else
                goChest = mesh;
        }

        if (goChest == null)
        {
            ResetSlotToNaked(ItemSlot.fullarmor, visual.ItemId);
            return;
        }

        _armorDresser.ApplyFullBody(armor, goChest, goLegs);
    }

    void ApplySingleArmor(ArmorVisual visual)
    {
        if (visual.Parts.Length == 0)
        {
            ResetSlotToNaked(visual.Slot, visual.ItemId);
            return;
        }

        ArmorVisualPart part = visual.Parts[0];
        GameObject armorMesh = CreateArmorMesh(part.Mesh, part.Material);
        if (armorMesh == null)
        {
            ResetSlotToNaked(visual.Slot, visual.ItemId);
            return;
        }

        GetDefaultGoWithArmorModel(
            ItemSlot.fullarmor,
            out Armor[] defaultArmor,
            out GameObject[] listArmorPiece,
            (int)_raceId);
        _armorDresser.ApplySingle(
            visual.Armor,
            visual.Slot,
            armorMesh,
            defaultArmor,
            listArmorPiece);
    }

    void ResetSlotToNaked(ItemSlot slotArmor, int itemId)
    {
        EquipmentMeshLog.Once("armor", itemId);
        GetDefaultGoWithArmorModel(
            slotArmor,
            out Armor[] defaultArmor,
            out GameObject[] listArmorPiece,
            (int)_raceId);
        _armorDresser.ApplyNaked(slotArmor, defaultArmor, listArmorPiece);
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
        IPoolManager pool = ObjectPoolPacketServices.Pool;
        if (pool != null)
        {
            if (!pool.ReturnToPool(ObjectType.Armor , go))
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

    public void AddUserGearLink(GameObject face, GameObject hair1, GameObject hair2) {
        Face = face;
        Hair1 = hair1;
        Hair2 = hair2;
    }

}
