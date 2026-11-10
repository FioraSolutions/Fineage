using FMOD.Studio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR;

public partial class ItemInstance
{
    public bool EqualsBodyPart(ItemSlot bodyPartSource , bool isBow = false )
    {
 
        if (IsLRHand(bodyPartSource , isBow))
        {
            return true;
        }

        if (IsFullPlate(bodyPartSource))
        {
            return true;
        }

        if (_bodyPart == bodyPartSource)
        {
            return true;
        }

        return false;
    }



    private bool IsLRHand(ItemSlot bodyPartSource , bool isBow)
    {
        if (isBow)
        {
            // Event Bow delete left hand arrow
            if (ShouldDeleteArrowWhenBowUnequipped())
                return false;

            // Event Bow Replace -> Sword
            if (ShouldReplaceBowWithSword(bodyPartSource))
                return true;

            // Event Sword -> Replace Bow
            return ShouldReplaceSwordWithBow(isBow);
        }

        if (_bodyPart == ItemSlot.lrhand)
        {
            if (IsTwoHandedDelArrow(bodyPartSource))
            {
                return false;
            }

            if (IsRightHand(bodyPartSource))
            {
                return true;
            }

        }else if (bodyPartSource == ItemSlot.lrhand)
        {
            //Event Replace Arrow need delete no replace
            if (IsArrow()) return false;

            if (_bodyPart == ItemSlot.rhand | _bodyPart == ItemSlot.lhand)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTwoHandedDelArrow(ItemSlot bodyPartSource) =>
      _bodyPart == ItemSlot.lrhand && bodyPartSource == ItemSlot.lhand;

    private bool IsRightHand(ItemSlot bodyPartSource) =>
        _bodyPart == ItemSlot.lrhand && bodyPartSource == ItemSlot.rhand;

    private bool ShouldDeleteArrowWhenBowUnequipped()
    {
        return !IsBow() && _bodyPart == ItemSlot.lrhand;
    }

    private bool ShouldReplaceBowWithSword(ItemSlot bodyPartSource)
    {
        return !IsBow() && bodyPartSource == ItemSlot.lrhand && _bodyPart == ItemSlot.rhand;
    }

    private bool ShouldReplaceSwordWithBow(bool isBow)
    {
        return isBow == IsBow();
    }

    private bool IsFullPlate(ItemSlot bodyPartSource)
    {
        if (_bodyPart == ItemSlot.chest | _bodyPart == ItemSlot.legs)
        {
            if (bodyPartSource == ItemSlot.fullarmor)
            {
                return true;
            }
        }
        else if (bodyPartSource == ItemSlot.chest | bodyPartSource == ItemSlot.legs)
        {
            if (_bodyPart == ItemSlot.fullarmor)
            {
                return true;
            }
        }

        return false;
    }

    public string GetName()
    {
        switch (_category)
        {
            case ItemCategory.Weapon:
            case ItemCategory.ShieldArmor:
            case ItemCategory.Jewel:
            case ItemCategory.Item:
            case ItemCategory.Adena:
            case ItemCategory.RequiredItem:
                return TablePacketServices.ItemNames.GetItemName(_itemId)?.DisplayName ?? "";

            default:
                return "";
        }
    }

    public Texture2D GetGradeTexture()
    {
        if (Category == ItemCategory.Weapon)
        {
            Weapongrp weapon = TablePacketServices.WeaponGrps.GetWeapon(_itemId);
            return GetGradeImage(weapon.Grade);
        }
        else if (Category == ItemCategory.ShieldArmor | Category == ItemCategory.Jewel)
        {
            Armorgrp armor = TablePacketServices.ArmorGrps.GetArmor(_itemId);
            Weapongrp weapon = TablePacketServices.WeaponGrps.GetWeapon(_itemId);
            if(armor != null) return GetGradeImage(armor.Grade);
            if(weapon != null) return GetGradeImage(weapon.Grade);
        }

        return null;
    }

    public ItemGrade GetItemGrade()
    {
        return _itemData.Itemgrp.Grade;
    }


}
