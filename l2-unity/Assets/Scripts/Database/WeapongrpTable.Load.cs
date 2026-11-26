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

#if UNITY_EDITOR
    /// <summary>
    /// Loads Interlude weapongrp from StreamingAssets for EditMode audits without hitting
    /// <see cref="Instance"/>'s empty non-DI fallback.
    /// </summary>
    public static WeapongrpTable TryLoadForEditModeTests()
    {
        Unbind();
        var table = new WeapongrpTable();
        table.Initialize();
        if (table.WeaponGrps == null || table.WeaponGrps.Count == 0)
        {
            Unbind();
            return null;
        }

        Bind(table);
        return table;
    }

    public static void UnloadForEditModeTests() => Unbind();
#endif

    public void Initialize() {
        string glowPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/EnchantGlow_interlude.txt");
        if (!EnchantGlow.TryLoadFile(glowPath))
            EnchantGlow.LoadText(EnchantGlow.DefaultLevels);
        ReadWeaponGrpDat();
        ReadWeaponInterlude();
    }
}
