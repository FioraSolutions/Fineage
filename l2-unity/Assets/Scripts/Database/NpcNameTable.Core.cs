using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class NpcNameTable
{
    public static void Bind(NpcNameTable table) {
        if (table != null)
            _instance = table;
    }

    public void Initialize() {
        ReadNpcgrps();
    }
}
