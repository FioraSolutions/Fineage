public enum ItemSlot : byte {
    none = 0,
    head = 1,
    chest = 2,
    legs = 3,
    fullarmor = 4,
    gloves = 5,
    feet = 6,
    lhand = 7,
    rhand = 8,
    lrhand = 9,
    rfinger = 10,
    lfinger = 11,
    lear = 12,
    rear = 13,
    neck = 14,
    underwear = 15,
    ring = 16,
    earring = 17,
    boots = 18,
    lhand_shield = 19,
    hat1 = 20,
    hat2 = 21,
    belt = 22,
    agation = 23,
    pendant = 24,
    alldress = 25,
    onepiece = 26,
}

public class ItemSlotParser {
    public static ItemSlot ParseBodyPart(string bodyPart) {
        switch (bodyPart) {
            case "artifact_a1":
                return ItemSlot.chest;
            case "artifact_a2":
                return ItemSlot.legs;
            case "artifact_a3":
                return ItemSlot.feet;
            case "head":
                return ItemSlot.head;
            case "artifactbook":
                return ItemSlot.gloves;
            case "rfinger":
                return ItemSlot.rfinger;
            case "lfinger":
                return ItemSlot.lfinger;
            case "rear":
                return ItemSlot.rear;
            case "lear":
                return ItemSlot.lear;
            case "neck":
                return ItemSlot.neck;
            case "onepiece":
                return ItemSlot.fullarmor;
            case "artifact_a7":
                return ItemSlot.rhand;
            case "artifact_a8":
                return ItemSlot.lhand_shield;
            case "lrhand":
                return ItemSlot.lrhand;
            case "alldress":
                return ItemSlot.alldress;
            default:
                return ItemSlot.feet;
        }
    }

    /// <summary>
    /// Official Interlude armorgrp body_part numbers, matched against Classic
    /// body_part strings on overlapping rows (earring=1, neck=3, ring=4,
    /// underwear=0, head=6, gloves=9, chest=10, legs=11, feet=12, cloak=13,
    /// full armor=15, formal wear=16, circlet=17, hair accessory=18, hair=19).
    /// Cloak has no ItemSlot in this client, so 13 maps to none.
    /// </summary>
    public static ItemSlot FromInterludeArmor(int bodyPartCode)
    {
        switch (bodyPartCode)
        {
            case 0:
                return ItemSlot.underwear;
            case 1:
                return ItemSlot.rear;
            case 3:
                return ItemSlot.neck;
            case 4:
                return ItemSlot.rfinger;
            case 6:
                return ItemSlot.head;
            case 9:
                return ItemSlot.gloves;
            case 10:
                return ItemSlot.chest;
            case 11:
                return ItemSlot.legs;
            case 12:
                return ItemSlot.feet;
            case 15:
                return ItemSlot.fullarmor;
            case 16:
                return ItemSlot.alldress;
            case 17:
                return ItemSlot.hat1;
            case 18:
                return ItemSlot.hat2;
            case 19:
                return ItemSlot.hat1;
            default:
                return ItemSlot.none;
        }
    }
}
