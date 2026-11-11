using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
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
}
