using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    private TemplateContainer SwitchToolTip(string[] ids, bool isClickLeft)
    {
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);

        if (isClickLeft)
        {
            switch (type)
            {
                case (int)SlotType.PriceSell:
                    Product productSell = ToolTipManager.GetInstance().FindProductInSellList(position);
                    if (productSell != null) return GetProductContainer(productSell);
                    break;
                case (int)SlotType.PriceBuy:
                    Product productBuy = ToolTipManager.GetInstance().FindProductInBuyList(position);
                    if (productBuy != null) return GetProductContainer(productBuy);
                    break;

                case (int)SlotType.Inventory:
                    ItemInstance item = InventoryPacketServices.Window.GetItemByPosition(position);
                    if (item != null) return GetInventoryContainer(item);
                    break;
                case (int)SlotType.Enchant:
                    ItemInstance enchantItem = ItemEnchantPacketServices.Enchant.GetItemByPosition(position);
                    if (enchantItem != null) return GetInventoryContainer(enchantItem);
                    break;
                case (int)SlotType.Multisell:
                    ItemInstance multiSelltItem = MultiSellPacketServices.MultiSell.GetItemByPosition(position);
                    if (multiSelltItem != null) return GetInventoryContainer(multiSelltItem);
                    break;
                case (int)SlotType.Recipe:
                    ItemInstance receiptInstance = CraftPacketServices.RecipeBook.GetRecipeInstanceByPosition(position);
                    if (receiptInstance != null) return GetInventoryContainer(receiptInstance);
                    break;
                case (int)SlotType.RecipeCraftItem:
                    ItemInstance requiredItem = CraftPacketServices.Crafting.GetRequiredInstanceByPosition(position);
                    if (requiredItem != null) return GetInventoryContainer(requiredItem);
                    break;
                case (int)SlotType.Gear:
                    GearItem gearItem = InventoryPacketServices.Window.GetGearPosition(position);
                    return GetGearContainer(gearItem);
                case (int)SlotType.SkillWindow:
                    //It is not the position that is used, but the skill ID. We have many panels on this form, so we pass its ID to tooltips, and there is no need for it anymore.
                    int skillId  = position;
                    SkillInstance skillItem = SkillLearnPacketServices.SkillList.GetSkillInstanceBySkillId(skillId);
                    return GetSkillContainer(skillItem);
            }
        }
        else
        {
            switch (type)
            {
                case (int)SlotType.PriceSell:
                case (int)SlotType.PriceBuy:
                    return SwitchToSimple();
                case (int)SlotType.Inventory:
                    return SwitchToString();
                case (int)SlotType.Enchant:
                    return SwitchToString();
                case (int)SlotType.Multisell:
                    return SwitchToString();
                case (int)SlotType.Recipe:
                    return SwitchToString();
                case (int)SlotType.RecipeCraftItem:
                    return SwitchToString();
                case (int)SlotType.SkillBar:
                    return SwitchToString();
                case (int)SlotType.Gear:
                    GearItem gearItem = InventoryPacketServices.Window.GetGearPosition(position);
                    return GetGearStringContainer(gearItem);
                case (int)SlotType.SkillWindow:
                                       //It is not the position that is used, but the skill ID. We have many panels on this form, so we pass its ID to tooltips, and there is no need for it anymore.
                    int skillId  = position;
                    SkillInstance skillItem = SkillLearnPacketServices.SkillList.GetSkillInstanceBySkillId(skillId);
                    return GetSkillContainer(skillItem);
                case (int)SlotType.BuffPanel:
                    SkillInstance instance = AuxiliaryGameUIPacketServices.Buffer.GetCellByPosition(position);
                    return GetSkillContainer(instance);
            }
        }

        return null;
    }

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

    /// <summary>
    /// Resolves the worn item behind a gear cell, or null when the paperdoll cell is filled but the
    /// item is no longer in the equip map (a replace that has not been re-applied yet). Callers must
    /// tolerate null: dereferencing it here used to throw on a plain click on a worn slot.
    /// </summary>
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


    private TemplateContainer SwitchToSimple()
    {
        _content.Clear();
        var template = _windowTemplateSimple.CloneTree();
        _content.Add(template);
        _contentInside = template.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return template;
    }

  

    private TemplateContainer SwitchToString()
    {
        _content.Clear();
        var template = _windowTemplateString.CloneTree();
        _content.Add(template);
        _contentInside = template.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return template;
    }



    private TemplateContainer SwitchToWeapon()
    {
        _content.Clear();
        var weapon = _windowTemplateWeapon.CloneTree();
        _content.Add(weapon);
        _contentInside = weapon.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return weapon;
    }

    private TemplateContainer SwitchToSkill()
    {
        _content.Clear();
        var weapon = _windowTemplateSkill.CloneTree();
        _content.Add(weapon);
        _contentInside = weapon.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return weapon;
    }

    private TemplateContainer SwitchToAccessories()
    {
        _content.Clear();
        var _accessories = _windowTemplateAcccesories.CloneTree();
        _content.Add(_accessories);
        _contentInside = _accessories.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return _accessories;
    }

    private TemplateContainer SwitchToRequiredAccessories()
    {
        _content.Clear();
        var _accessories = _windowTemplateRequiredAcccesories.CloneTree();
        _content.Add(_accessories);
        _contentInside = _accessories.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return _accessories;
    }

    private TemplateContainer SwitchToArmor()
    {
        _content.Clear();
        var _accessories = _windowTemplateArmor.CloneTree();
        _content.Add(_accessories);
        _contentInside = _accessories.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return _accessories;
    }



    private string[] GetUniquePosition(VisualElement ve)
    {
        return  ve.name.Split('_');
    }

    private bool _isForceBelow = false;
    //experimental code 
    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (evt.newRect.height == 0)
            return;

        if (_contentInside != null)
        {
            _heightContent = _contentInside.worldBound.height;
            _widtchContent = _contentInside.worldBound.width;
            _showToolTip.Show(_selectShow , _isForceBelow);
        }

    }

 
    //experimental code 
    public void NewPosition(Vector2 newPoint, float sdfig , bool isForceBelow)
    {
        _isForceBelow = isForceBelow;
        var highest = _calcPosition.HighestPoint(newPoint, _heightContent);
        bool insideRoot = SystemUIPacketServices.Ui.IsWindowContain(highest);
        Vector2 reversePoint = _calcPosition.GetNewPosition(newPoint, sdfig, _heightContent , insideRoot , isForceBelow);
        _content.transform.position = reversePoint;
        BringToFront();
    }
}
