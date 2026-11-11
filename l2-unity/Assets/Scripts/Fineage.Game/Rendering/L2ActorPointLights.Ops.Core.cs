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
}
