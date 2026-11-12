using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public partial class NpcgrpTable
{
    public static void Bind(NpcgrpTable table) {
        if (table != null)
            _instance = table;
    }

    public static void Unbind() => _instance = null;

    public void Initialize() {
        Initialize(null);
    }

    public void Initialize(NpcDecoEffectsTable decoEffects) {
        _decoEffects = decoEffects;
        if (_decoEffects != null)
            _decoEffects.Initialize();
        ReadNpcgrps();
    }
}
