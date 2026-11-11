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
}
