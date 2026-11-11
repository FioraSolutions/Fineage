using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    public static void AppendCapture(StringBuilder text)
    {
        if (Time.unscaledTime >= _nextRefresh)
            Refresh();
        text.Append(FullReport());
    }
}
