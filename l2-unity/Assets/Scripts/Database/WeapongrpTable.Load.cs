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

    public void Initialize() {
        ReadWeaponGrpDat();
        ReadWeaponInterlude();
    }
}
