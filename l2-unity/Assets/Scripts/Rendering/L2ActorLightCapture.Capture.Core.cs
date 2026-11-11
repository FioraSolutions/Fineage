using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static partial class L2ActorLightCapture
{
    static bool IsKaterina(EntityIdentity identity)
    {
        if (identity.NpcId == KaterinaNpcId)
            return true;
        string name = identity.Name;
        if (string.IsNullOrEmpty(name))
            return false;
        return name.IndexOf("katerina", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("ekaterin", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
