using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

/// <summary>
/// Uploads up to five local lights into actor vertex color.
/// Each character gets the five nearest torches that reach him.
/// A camera-wide fallback covers a pawn for the moment before his renderer is found.
/// </summary>
public static partial class L2ActorPointLights
{
    public const int Max = 5;

    // Снимок комнаты применил один цвет. Слоты 1–4 остаются нулём. Шейдер их тоже не складывает.
    const int PassCount = 1;
    const float RefreshSec = 1f;

    static readonly List<L2ActorLocalLight> Lights = new List<L2ActorLocalLight>(32);
    static readonly List<ActorGroup> Groups = new List<ActorGroup>(32);
    static readonly Dictionary<int, ActorGroup> GroupsByRoot = new Dictionary<int, ActorGroup>(64);
    static readonly int[] Chosen = new int[Max];

    static readonly Vector4[] GlobalPos = new Vector4[Max];
    static readonly Vector4[] GlobalColor = new Vector4[Max];
    static readonly Vector4[] GlobalAtten = new Vector4[Max];

    static readonly int PosId = Shader.PropertyToID("_L2ActorPointPos");
    static readonly int ColorId = Shader.PropertyToID("_L2ActorPointColor");
    static readonly int AttenId = Shader.PropertyToID("_L2ActorPointAtten");

    static float _nextRefresh = -1f;
    static float _nextLog = -1f;
    static bool _rewriteActors;
    const float MoveEpsilonSqr = 0.25f;

    // Captured indoor fire, monster search 20260929_165643.
    static readonly Vector3 ClientC227 = new Vector3(0.130504623f, 0.105057523f, 0.0745209977f);

    public static void Push()
    {
        if (Time.unscaledTime >= _nextRefresh)
        {
            Refresh();
            _nextRefresh = Time.unscaledTime + RefreshSec;
        }

        Vector3 origin = Vector3.zero;
        Camera cam = Camera.main;
        if (cam != null)
            origin = cam.transform.position;
        Fill(origin, GlobalPos, GlobalColor, GlobalAtten);
        Shader.SetGlobalVectorArray(PosId, GlobalPos);
        Shader.SetGlobalVectorArray(ColorId, GlobalColor);
        Shader.SetGlobalVectorArray(AttenId, GlobalAtten);

        for (int g = 0; g < Groups.Count; g++)
        {
            ActorGroup group = Groups[g];
            if (group.Root == null)
                continue;

            Vector3 actorOrigin = group.Root.position;
            bool moved = (actorOrigin - group.AppliedOrigin).sqrMagnitude >= MoveEpsilonSqr;
            if (!_rewriteActors && group.Applied && !moved && (group.AppliedChosen > 0 || !NearLight(actorOrigin)))
                continue;

            int chosen = Fill(actorOrigin, group.Pos, group.Color, group.Atten);
            if (!_rewriteActors && group.Applied && !moved && chosen == group.AppliedChosen)
                continue;

            for (int r = 0; r < group.Renderers.Count; r++)
            {
                Renderer renderer = group.Renderers[r];
                if (renderer == null)
                    continue;
                renderer.GetPropertyBlock(group.Block);
                group.Block.SetVectorArray(PosId, group.Pos);
                group.Block.SetVectorArray(ColorId, group.Color);
                group.Block.SetVectorArray(AttenId, group.Atten);
                renderer.SetPropertyBlock(group.Block);
            }

            group.Applied = true;
            group.AppliedChosen = chosen;
            group.AppliedOrigin = actorOrigin;
        }

        _rewriteActors = false;

        if (Time.unscaledTime >= _nextLog)
        {
            _nextLog = Time.unscaledTime + 2f;
            // Debug.Log(NearestReport());
        }
    }

    public static void AppendCapture(StringBuilder text)
    {
        if (Time.unscaledTime >= _nextRefresh)
            Refresh();
        text.Append(FullReport());
    }

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
