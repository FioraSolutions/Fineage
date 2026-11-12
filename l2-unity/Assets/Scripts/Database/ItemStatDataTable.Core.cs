using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

public partial class ItemStatDataTable
{
    public static void Bind(ItemStatDataTable table) {
        if (table != null)
            _instance = table;
    }

    public static void Unbind() => _instance = null;

    public void Initialize() {
        ReadItemStatDataDat();
    }
}
