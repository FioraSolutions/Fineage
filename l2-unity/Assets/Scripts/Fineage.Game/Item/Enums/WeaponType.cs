public enum WeaponType : byte {
    none, //shield
    hand,
    sword,
    bigword,
    blunt,
    bigblunt,
    bow,
    arrow,
    dagger,
    fist,
    dual,
    dualfist,
    pole,
    shield,
    staff,
    mbook
}

public enum WeaponMaterial : byte
{
    None, //shield
    Wood,
    Bone
}

// hands: 244 Elven fighter fists

public class WeaponMaterialTypeParser
{
    public static WeaponMaterial Parce(string materils)
    {
        switch (materils)
        {
            case "wood":
                return WeaponMaterial.Wood;
            case "bone":
                return WeaponMaterial.Bone;
            default:
                return WeaponMaterial.None;
        }
    }
}
public class WeaponTypeParser {
    public static string GetWeaponAnim(WeaponType weaponType) {
        switch (weaponType) {
            case WeaponType.none:
                return "1HS";
            case WeaponType.fist:
            case WeaponType.hand:
                return "hand";
            case WeaponType.sword:
            case WeaponType.blunt:
            case WeaponType.dagger:
                return "1HS";
            case WeaponType.bigword:
            case WeaponType.bigblunt:
                return "2HS";
            case WeaponType.dual:
            case WeaponType.dualfist:
                return "dual";
            case WeaponType.pole:
            case WeaponType.staff:
                return "pole";
            case WeaponType.bow:
                return "bow";
            case WeaponType.mbook:
                // Interlude has no book swing set. Books attack with the one-hand clip.
                return "1HS";
            default:
                return "hand";
        }
    }

    /// <summary>
    /// Interlude weapongrp columns: handness (grip) and weapon_type (kind).
    /// 0 shield, 1 sword, 2 blunt, 3 dagger, 4 pole, 5 fist, 6 bow, 7 book, 8 dual.
    /// Handness 2 is two-hand, 4 is pole/staff, 5 is bow, 7 is fists.
    /// </summary>
    public static WeaponType FromInterlude(int handness, int weaponType)
    {
        switch (weaponType)
        {
            case 0:
                return WeaponType.shield;
            case 1:
                return handness == 2 ? WeaponType.bigword : WeaponType.sword;
            case 2:
                if (handness == 2)
                    return WeaponType.bigblunt;
                if (handness == 4)
                    return WeaponType.staff;
                return WeaponType.blunt;
            case 3:
                return WeaponType.dagger;
            case 4:
                return WeaponType.pole;
            case 5:
                return WeaponType.fist;
            case 6:
                return WeaponType.bow;
            case 7:
                return WeaponType.mbook;
            case 8:
                return WeaponType.dual;
            case 10:
                return WeaponType.pole;
            default:
                return ParseByHandness(handness);
        }
    }

    public static WeaponType Parse(string type) {
        switch (type) {
            case "sword":
                return WeaponType.sword;
            case "twohandsword":
                return WeaponType.bigword;
            case "blunt":
                return WeaponType.blunt;
            case "twohandblunt":
                return WeaponType.bigblunt;
            case "dagger":
            case "buster":
                return WeaponType.dagger;
            case "bow":
                return WeaponType.bow;
            case "pole":
                return WeaponType.pole;
            case "staff":
                return WeaponType.staff;
            case "fist":
                // Shields in Classic weapongrp often use fist + handness 0;
                // WeaponClassifier promotes those to shield.
                return WeaponType.fist;
            case "dual":
                return WeaponType.dual;
            case "dualfist":
                return WeaponType.dualfist;
            case "arrow":
                return WeaponType.arrow;
            default:
                return WeaponType.hand;
        }
    }

    public static string WeaponTypeName(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.sword: // For 2H sword check at handness
                return "Sword";
            case WeaponType.blunt:
                return "Blunt";
            case WeaponType.pole:
                return "Pole";
            case WeaponType.dagger:
                return "Dagger";
            case WeaponType.dual:
                return "Dual Sword";
            case WeaponType.bow:
                return "Bow";
            case WeaponType.hand: // shield or hand check at handness
                return "Hand";
            case WeaponType.fist:
                return "Fist";
            case WeaponType.staff:
                return "Staff";
            case WeaponType.mbook:
                return "Magic book";
            default:
                return "None";
        }
    }


    public static int WeaponRange(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.none:
                return  40;
            case WeaponType.sword:
                return 40;
            case WeaponType.blunt:
                return 40;
            case WeaponType.dagger:
                return 40;
            case WeaponType.bow:
                return 500;
            case WeaponType.pole:
                return 66;
            case WeaponType.fist:
                return 40;
            case WeaponType.dualfist:
                return 40;
            case WeaponType.bigword:
                return 40;
            default:
                return 40;
        }
    }

        public static WeaponType ParseByHandness(int handness) {
        switch (handness) {
            case 0:
                return WeaponType.none;
            case 1:
                return WeaponType.sword;
            case 2:
                return WeaponType.bigword;
            case 3:
                // Interlude dual swords are handness 3, weapon_type 8.
                return WeaponType.dual;
            case 4:
                return WeaponType.pole;
            case 5:
                return WeaponType.bow;
            case 6: // special
                return WeaponType.hand;
            case 7:
                return WeaponType.fist;
            case 8: // arbalet
                return WeaponType.hand;
            case 9: // rapier
                return WeaponType.hand;
            case 10: // dualdagger
                return WeaponType.hand;
            case 12: 
                return WeaponType.dagger;
            case 14: //staff
                return WeaponType.pole;
            default:
                return WeaponType.hand;

        }
    }
}
