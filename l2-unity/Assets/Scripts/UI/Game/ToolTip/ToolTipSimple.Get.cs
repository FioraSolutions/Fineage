using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    private TemplateContainer GetProductContainer(Product product)
    {
        switch (product.GetTypeItem())
        {
            case EnumType2.TYPE2_WEAPON:
                return SwitchToWeapon();
            case EnumType2.TYPE2_ACCESSORY:
            case EnumType2.TYPE2_OTHER:
                return SwitchToAccessories();
            case EnumType2.TYPE2_SHIELD_ARMOR:
                return SwitchToArmor();
        }
        return null;
    }

    private TemplateContainer GetInventoryContainer(ItemInstance item)
    {
        switch (item.Category)
        {
            case ItemCategory.Weapon:
                return SwitchToWeapon();
            case ItemCategory.Jewel:
            case ItemCategory.Item:
                return SwitchToAccessories();
            case ItemCategory.RequiredItem:
                return SwitchToRequiredAccessories();
            case ItemCategory.ShieldArmor:
                return SwitchToArmor();
        }
        return null;
    }

    private TemplateContainer GetSkillContainer(SkillInstance item)
    {
        return SwitchToSkill();
    }

    public TemplateContainer GetContainer(ItemInstance item)
    {
        switch (item.Category)
        {
            case ItemCategory.Weapon:
                return SwitchToWeapon();
            case ItemCategory.Jewel:
            case ItemCategory.Item:
                return SwitchToAccessories();
            case ItemCategory.ShieldArmor:
                return SwitchToArmor();
        }
        return null;
    }

    private TemplateContainer GetGearContainer(GearItem gearItem)
    {
        ItemInstance assignItem = GetGearTooltipItem(gearItem);
        if (assignItem == null)
            return null;

        if (assignItem.BodyPart == ItemSlot.fullarmor)
        {
            if (gearItem.GetSlotType() != ItemSlot.legs)
                return GetInventoryContainer(assignItem);
        }
        else if (assignItem.BodyPart == ItemSlot.lrhand)
        {
            if (gearItem.GetSlotType() != ItemSlot.lhand)
                return GetInventoryContainer(assignItem);
        }
        else
        {
            return GetInventoryContainer(assignItem);
        }

        return null;
    }

    private static ItemInstance GetGearTooltipItem(GearItem gearItem)
    {
        if (gearItem == null || gearItem.GetItemId() == 0)
            return null;
        PlayerInventory inventory = InventoryPacketServices.Inventory;
        return inventory != null ? inventory.GetItemEquip(gearItem.GetObjectId()) : null;
    }

    private TemplateContainer GetGearStringContainer(GearItem gearItem)
    {
        ItemInstance assignItem = GetGearTooltipItem(gearItem);
        if (assignItem == null)
            return null;

        if (assignItem.BodyPart == ItemSlot.fullarmor)
        {
            if (gearItem.GetSlotType() != ItemSlot.legs)
                return SwitchToString();
        }
        else if (assignItem.BodyPart == ItemSlot.lrhand)
        {
            if (gearItem.GetSlotType() != ItemSlot.lhand)
                return SwitchToString();
        }
        else
        {
            return SwitchToString();
        }

        return null;
    }

    public object GetDataClickLeft(int type, int position)
    {
        switch ((SlotType)type)
        {
            case SlotType.PriceSell:
                return ToolTipManager.GetInstance().FindProductInSellList(position);

            case SlotType.PriceBuy:
                return ToolTipManager.GetInstance().FindProductInBuyList(position);

            case SlotType.Inventory:
                return InventoryPacketServices.Window.GetItemByPosition(position);

            case SlotType.Enchant:
                return ItemEnchantPacketServices.Enchant.GetItemByPosition(position);

            case SlotType.Multisell:
                return MultiSellPacketServices.MultiSell.GetItemByPosition(position);

            case SlotType.Recipe:
                return CraftPacketServices.RecipeBook.GetRecipeInstanceByPosition(position);
            case SlotType.RecipeCraftItem:
                return CraftPacketServices.Crafting.GetRequiredInstanceByPosition(position);
            case SlotType.SkillWindow:
                return SkillLearnPacketServices.SkillList.GetSkillInstanceBySkillId(position);

            case SlotType.Gear:
                return GetGearTooltipItem(InventoryPacketServices.Window != null
                    ? InventoryPacketServices.Window.GetGearPosition(position)
                    : null);

            default:
                return null;
        }
    }

}
