using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    static int Fill(Vector3 origin, Vector4[] pos, Vector4[] color, Vector4[] atten)
    {
        int chosen = Select(origin);

        for (int i = 0; i < Max; i++)
        {
            if (i >= chosen)
            {
                pos[i] = Vector4.zero;
                color[i] = Vector4.zero;
                atten[i] = Vector4.zero;
                continue;
            }

            L2ActorLocalLight light = Lights[Chosen[i]];
            Vector3 p = light.WorldPosition;
            Color c = light.ReadyColor;
            pos[i] = new Vector4(p.x, p.y, p.z, 1f);
            color[i] = new Vector4(c.r, c.g, c.b, 1f);
            atten[i] = new Vector4(light.constantAttenuation, light.linearAttenuation, light.radius, 0f);
        }

        return chosen;
    }

    static bool NearLight(Vector3 origin)
    {
        for (int i = 0; i < Lights.Count; i++)
        {
            L2ActorLocalLight light = Lights[i];
            if (!Usable(light))
                continue;
            float reach = light.radius + 0.5f;
            if ((light.WorldPosition - origin).sqrMagnitude <= reach * reach)
                return true;
        }

        return false;
    }
}
