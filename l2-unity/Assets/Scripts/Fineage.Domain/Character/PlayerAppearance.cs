using System;
using UnityEngine;

[System.Serializable]
public class PlayerAppearance : Appearance
{
    public int Race;
    public int Sex;
    public int Face;
    public int BaseClass;
    public bool Running = true;
    public int HairStyle;
    public int HairColor;
    public int Chest;
    public int Legs;
    public int Gloves;
    public int Feet;
    public int WeaponEnchant;
    /// <summary>Per-paperdoll-slot enchant (same 17-wide order as UserInfo paperdoll).</summary>
    public int[] SlotEnchant = new int[17];
    public bool PaperdollReady;
    public PaperDollSelection PaperDoll { get; } = new();

    public byte FaceByte { get { return Convert.ToByte(Face); } }
    public byte HairStyleByte { get { return Convert.ToByte(HairStyle); } }
    public byte HairColorByte { get { return Convert.ToByte(HairColor); } }
}
