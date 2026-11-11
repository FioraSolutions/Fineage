using UnityEngine;

public partial class UserGear : Gear
{
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
            _armorDresser.OnSyncMesh += OnSyncMesh;
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
}
