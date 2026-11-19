using UnityEngine;

public partial class Gear
{
    public void SyncEnchantGlows()
    {
        PlayerAppearance appearance = ResolveAppearance();
        int weaponEnchant = appearance != null ? appearance.WeaponEnchant : 0;
        int leftEnchant = SlotEnchant(appearance, 8);

        ApplyHandGlow(GetRightHandBone(), weaponName, false, weaponEnchant);
        if (_leftHandShield != null)
            ApplyHandGlow(GetShieldBone(), shieldName, true, leftEnchant);
        else
            ApplyHandGlow(GetLeftHandBone(), weaponName, true, leftEnchant != 0 ? leftEnchant : weaponEnchant);
    }

    protected PlayerAppearance ResolveAppearance()
    {
        Entity entity = GetComponent<Entity>() ?? GetComponentInParent<Entity>();
        return entity != null ? entity.Appearance as PlayerAppearance : null;
    }

    protected static int SlotEnchant(PlayerAppearance appearance, int slot)
    {
        if (appearance == null || appearance.SlotEnchant == null)
            return 0;
        if (slot < 0 || slot >= appearance.SlotEnchant.Length)
            return 0;
        return appearance.SlotEnchant[slot];
    }

    void ApplyHandGlow(Transform bone, string prefix, bool leftHand, int enchantLevel)
    {
        Transform hand = FindPrefixedChild(bone, prefix);
        if (hand == null)
            return;

        int itemId = ItemIdFromPrefixedName(hand.name, prefix);
        Weapon weapon = itemId != 0 && TablePacketServices.Items != null
            ? TablePacketServices.Items.GetWeapon(itemId)
            : null;
        EnchantGlowPlacement placement = EnchantGlowPlacement.None;
        if (weapon != null && weapon.Weapongrp != null)
        {
            placement = EnchantGlow.PickHand(
                weapon.Weapongrp.EnchantedRight,
                weapon.Weapongrp.EnchantedLeft,
                leftHand);
        }

        ItemEnchantGlowView view = hand.GetComponent<ItemEnchantGlowView>();
        if (view == null)
            view = hand.gameObject.AddComponent<ItemEnchantGlowView>();
        view.ApplyWeapon(placement, enchantLevel);
    }

    static Transform FindPrefixedChild(Transform bone, string prefix)
    {
        if (bone == null)
            return null;
        for (int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);
            if (child.name.StartsWith(prefix, System.StringComparison.Ordinal))
                return child;
        }
        return null;
    }

    static int ItemIdFromPrefixedName(string name, string prefix)
    {
        if (string.IsNullOrEmpty(name) || !name.StartsWith(prefix, System.StringComparison.Ordinal))
            return 0;
        return int.TryParse(name.Substring(prefix.Length), out int id) ? id : 0;
    }
}
