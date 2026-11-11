using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    static string FullReport()
    {
        var text = new StringBuilder();
        text.Append("pointSlots=").Append(Max);
        text.Append(" localLights=").Append(Lights.Count);
        text.Append(" actors=").Append(Groups.Count).Append('\n');
        text.Append("client_c227=").Append(Vec(ClientC227)).Append('\n');
        text.Append("note=ready is light.color * intensity. The vertex adds ready * NdotL / (a + b*meters), then saturates with ambient and sun.\n");
        text.Append("note=contrib is that term at NdotL=1. color0_ceiling adds sun NdotL=1 as well, so it is a ceiling, not one real vertex.\n");
        text.Append("note=whiteTex is saturate(color0 * 2), the modulate2x result on a white texel.\n");

        for (int i = 0; i < Lights.Count; i++)
        {
            L2ActorLocalLight light = Lights[i];
            if (light == null)
                continue;
            Color src = light.SourceColor;
            Color ready = light.ReadyColor;
            text.Append("light ").Append(i).Append(' ').Append(light.name);
            text.Append(" active=").Append(light.IsActive ? 1 : 0);
            text.Append(" pos=").Append(Vec(light.WorldPosition));
            text.Append(" unityColor=").Append(Vec(src));
            text.Append(" intensity=").Append(Num(light.Intensity));
            text.Append(" ready=").Append(Vec(ready));
            text.Append(" a=").Append(Num(light.constantAttenuation));
            text.Append(" b=").Append(Num(light.linearAttenuation));
            text.Append(" radius=").Append(Num(light.radius));
            text.Append(" unityRange=").Append(Num(light.UnityRange));
            text.Append('\n');
        }

        Camera cam = Camera.main;
        for (int i = 0; i < Groups.Count; i++)
        {
            ActorGroup group = Groups[i];
            if (group.Root == null)
                continue;
            if (cam != null && (group.Origin - cam.transform.position).sqrMagnitude > 40f * 40f)
                continue;
            AppendActor(text, group, cam);
        }

        return text.ToString();
    }

    static void AppendActor(StringBuilder text, ActorGroup group, Camera cam)
    {
        Vector3 origin = group.Origin;
        int chosen = Select(origin);
        Color ambient = Shader.GetGlobalColor("_L2ActorAmbientHalf");
        Color plane = Shader.GetGlobalColor("_L2HsvActorLight");
        float sunScale = Shader.GetGlobalFloat("_L2ActorDirectionalScale");
        if (sunScale <= 0f)
            sunScale = 0.7f;
        Vector3 sun = new Vector3(plane.r * sunScale, plane.g * sunScale, plane.b * sunScale);
        Vector3 sum = Vector3.zero;

        text.Append("actor ").Append(group.Root.name);
        text.Append(" origin=").Append(Vec(origin));
        if (cam != null)
            text.Append(" camDist=").Append(Num(Vector3.Distance(origin, cam.transform.position)));
        text.Append(" renderers=").Append(group.Renderers.Count);
        text.Append(" slots=").Append(chosen);
        text.Append(" c251=").Append(Vec(ambient));
        text.Append(" c250=").Append(Vec(sun));
        text.Append('\n');

        float nearestDist = 0f;
        for (int s = 0; s < chosen; s++)
        {
            L2ActorLocalLight light = Lights[Chosen[s]];
            float dist = Vector3.Distance(origin, light.WorldPosition);
            if (s == 0)
                nearestDist = dist;
            float atten = 1f / Mathf.Max(light.constantAttenuation + light.linearAttenuation * dist, 1e-5f);
            Color ready = light.ReadyColor;
            Vector3 contrib = new Vector3(ready.r, ready.g, ready.b) * atten;
            sum += contrib;
            text.Append("  slot").Append(s).Append(' ').Append(light.name);
            text.Append(" d=").Append(Num(dist)).Append("m");
            text.Append(" uu=").Append(Num(dist * 52.5f));
            text.Append(" intensity=").Append(Num(light.Intensity));
            text.Append(" ready=").Append(Vec(ready));
            text.Append(" atten=").Append(Num(atten));
            text.Append(" contrib=").Append(Vec(contrib));
            text.Append('\n');
        }

        Vector3 color0 = new Vector3(
            Mathf.Clamp01(ambient.r + sun.x + sum.x),
            Mathf.Clamp01(ambient.g + sun.y + sum.y),
            Mathf.Clamp01(ambient.b + sun.z + sum.z));
        Vector3 white = new Vector3(
            Mathf.Clamp01(color0.x * 2f),
            Mathf.Clamp01(color0.y * 2f),
            Mathf.Clamp01(color0.z * 2f));
        text.Append("  sumNdotl1=").Append(Vec(sum));
        text.Append(" color0_ceiling=").Append(Vec(color0));
        text.Append(" whiteTex=").Append(Vec(white));
        if (chosen > 0)
        {
            float clientAtten = 1f / Mathf.Max(0.431085169f + 0.021897854f * nearestDist, 1e-5f);
            text.Append(" clientOneAtNearest=").Append(Vec(ClientC227 * clientAtten));
        }
        text.Append('\n');
    }
}
