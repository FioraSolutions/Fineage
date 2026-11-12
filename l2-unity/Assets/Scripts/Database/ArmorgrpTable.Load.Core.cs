using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class ArmorgrpTable
{
    public static void Bind(ArmorgrpTable table) {
        if (table != null)
            _instance = table;
    }

    public static void Unbind() => _instance = null;

    public void Initialize() {
        ReadArmorGrpDat();
        ReadArmorInterlude();
    }
}
