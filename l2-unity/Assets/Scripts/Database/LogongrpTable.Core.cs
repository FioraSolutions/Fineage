using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public partial class LogongrpTable
{
    public static void Bind(LogongrpTable table) {
        if (table != null)
            _instance = table;
    }
}
