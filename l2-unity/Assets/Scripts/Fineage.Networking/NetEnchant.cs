using System;
using System.Collections.Generic;

public static partial class NetEnchant
{
    public const int ResultCancel = 0;
    public const int ResultSuccess = 1;
    public const int ResultKeep = 2;
    public const int ResultReset = 3;
    public const int ResultCrystallize = 4;

    public struct Session
    {
        public bool Open;
        public int ScrollObjectId;
        public int TargetObjectId;
    }

    public struct View
    {
        public bool CloseWindow;
        public bool RemoveItem;
        public bool ShowEnchant;
        public int ObjectId;
        public int EnchantLevel;
        public int CrystalItemId;
        public int CrystalCount;
    }


    /// <summary>The scroll itself is not a target. A closed window ignores the click.</summary>



    /// <summary>
    /// Success, keep, and reset write the level from the result. Crystallize removes the target.
    /// Crystals arrive on the following <c>InventoryUpdate</c>.
    /// </summary>


    /// <summary>
    /// EtcItemgrp types that open the enchant window. Property-increase scrolls
    /// (<c>scrl_inc_enchant_prop_*</c>) are not included.
    /// </summary>

    /// <summary>These <c>Error</c> 1099 codes clear the selected scroll. The window should close.</summary>

    /// <summary>Readable line for an enchant rejection. A server sentence wins when the body included one.</summary>
}
