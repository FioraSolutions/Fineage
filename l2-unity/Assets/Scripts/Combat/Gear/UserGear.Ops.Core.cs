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
}
