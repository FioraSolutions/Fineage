using UnityEngine;

public partial class UserGear : Gear
{
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

        // Naked chest/legs only needed when replacing a full-body set.
        Armor[] nakedArmor = null;
        GameObject[] nakedMeshes = null;
        if (visual.Slot == ItemSlot.chest || visual.Slot == ItemSlot.legs)
        {
            GetDefaultGoWithArmorModel(
                ItemSlot.fullarmor,
                out nakedArmor,
                out nakedMeshes,
                (int)_raceId);
        }

        _armorDresser.ApplySingle(
            visual.Armor,
            visual.Slot,
            armorMesh,
            nakedArmor,
            nakedMeshes);
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
}
