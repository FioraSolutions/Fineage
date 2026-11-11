using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    static string NearestReport()
    {
        Camera cam = Camera.main;
        Vector3 origin = cam != null ? cam.transform.position : Vector3.zero;
        ActorGroup nearest = null;
        float best = float.MaxValue;
        for (int i = 0; i < Groups.Count; i++)
        {
            ActorGroup group = Groups[i];
            if (group.Root == null)
                continue;
            float dist = (group.Origin - origin).sqrMagnitude;
            if (dist >= best)
                continue;
            best = dist;
            nearest = group;
        }

        var text = new StringBuilder();
        text.Append("[ActorPoint] localLights=").Append(Lights.Count);
        text.Append(" actors=").Append(Groups.Count);
        if (nearest == null)
        {
            text.Append(" no actor renderer");
            return text.ToString();
        }

        AppendActor(text, nearest, cam);
        return text.ToString();
    }
}
