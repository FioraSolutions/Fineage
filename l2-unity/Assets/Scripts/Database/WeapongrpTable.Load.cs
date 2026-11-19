using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class WeapongrpTable
{
    public static void Bind(WeapongrpTable table) {
        if (table != null)
            _instance = table;
    }

    public static void Unbind() => _instance = null;

    public void Initialize() {
        string glowPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/EnchantGlow_interlude.txt");
        if (!EnchantGlow.TryLoadFile(glowPath))
            EnchantGlow.LoadText(EnchantGlow.DefaultLevels);
        ReadWeaponGrpDat();
        ReadWeaponInterlude();
    }
}
