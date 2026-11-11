using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    static int Select(Vector3 origin)
    {
        int chosen = 0;
        for (int slot = 0; slot < PassCount; slot++)
        {
            int best = -1;
            float bestDist = float.MaxValue;
            for (int i = 0; i < Lights.Count; i++)
            {
                L2ActorLocalLight light = Lights[i];
                if (!Usable(light) || AlreadyChosen(i, chosen))
                    continue;
                float radius = light.radius;
                float dist = (light.WorldPosition - origin).sqrMagnitude;
                if (dist > radius * radius || dist >= bestDist)
                    continue;
                best = i;
                bestDist = dist;
            }

            if (best < 0)
                break;
            Chosen[chosen] = best;
            chosen++;
        }

        return chosen;
    }

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
