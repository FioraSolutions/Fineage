using UnityEngine;

public partial class UserGear
{
    public void SyncArmorEnchantGlows()
    {
        if (_armorDresser == null)
            return;
        PlayerAppearance appearance = ResolveAppearance();
        _armorDresser.ForEachWorn((slot, armor, mesh) =>
        {
            if (mesh == null)
                return;
            int itemId = armor != null ? armor.Id : 0;
            bool naked = itemId == ItemTable.NAKED_CHEST || itemId == ItemTable.NAKED_LEGS ||
                itemId == ItemTable.NAKED_GLOVES || itemId == ItemTable.NAKED_BOOTS;
            int enchant = naked ? 0 : SlotEnchant(appearance, PaperdollIndex(slot));
            ItemEnchantGlowView view = mesh.GetComponent<ItemEnchantGlowView>();
            if (enchant <= 0)
            {
                if (view != null)
                    view.Clear();
                return;
            }
            if (view == null)
                view = mesh.AddComponent<ItemEnchantGlowView>();
            view.ApplyArmor(enchant);
        });
    }

    static int PaperdollIndex(ItemSlot slot)
    {
        switch (slot)
        {
            case ItemSlot.gloves:
                return 9;
            case ItemSlot.chest:
            case ItemSlot.fullarmor:
                return 10;
            case ItemSlot.legs:
                return 11;
            case ItemSlot.feet:
                return 12;
            case ItemSlot.head:
                return 6;
            default:
                return -1;
        }
    }
}
