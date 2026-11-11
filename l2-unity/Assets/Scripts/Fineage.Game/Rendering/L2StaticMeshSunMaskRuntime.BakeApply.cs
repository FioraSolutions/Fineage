using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed partial class L2StaticMeshSunMaskRuntime
{
    static void ApplyScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return;

        TextAsset asset = Resources.Load<TextAsset>(
            "Data/Maps/" + scene.name + "/Meta/StaticMeshSunMasksRemapped");
        if (asset == null)
            return;

        MapData data = JsonUtility.FromJson<MapData>(asset.text);
        if (data == null || data.actors == null)
            return;

        Shader.SetGlobalFloat(ForceSunVisibleId, 0f);
        Shader.SetGlobalFloat(SunMaskDebugId, ShowSunMaskDebug ? 1f : 0f);

        var filtersByMesh = new Dictionary<string, List<MeshFilter>>(StringComparer.OrdinalIgnoreCase);
        GameObject[] roots = scene.GetRootGameObjects();
        for (int r = 0; r < roots.Length; r++)
        {
            MeshFilter[] filters = roots[r].GetComponentsInChildren<MeshFilter>(true);
            for (int i = 0; i < filters.Length; i++)
            {
                MeshFilter filter = filters[i];
                if (!filtersByMesh.TryGetValue(filter.gameObject.name, out List<MeshFilter> list))
                {
                    list = new List<MeshFilter>();
                    filtersByMesh.Add(filter.gameObject.name, list);
                }
                list.Add(filter);
            }
        }

        var used = new HashSet<MeshFilter>();
        var combinedStreams = new Dictionary<Mesh, CombinedStreamData>();
        int applied = 0;
        int missingMeshName = 0;
        int positionMismatch = 0;
        int vertexMismatch = 0;
        var mismatchExamples = new List<string>();
        string bridgeStatus = "not in database";
        for (int i = 0; i < data.actors.Length; i++)
        {
            ActorData actor = data.actors[i];
            if (actor == null || string.IsNullOrEmpty(actor.visibility))
                continue;
            bool isBridge = string.Equals(
                actor.actor,
                "StaticMeshActor1",
                StringComparison.OrdinalIgnoreCase);
            if (!filtersByMesh.TryGetValue(actor.mesh, out List<MeshFilter> candidates))
            {
                missingMeshName++;
                if (isBridge)
                    bridgeStatus = "mesh name not found: " + actor.mesh;
                continue;
            }

            MeshFilter best = null;
            float bestDistance = float.PositiveInfinity;
            MeshFilter nearestAny = null;
            float nearestAnyDistance = float.PositiveInfinity;
            Vector3 expected = actor.UnityPosition;
            for (int c = 0; c < candidates.Count; c++)
            {
                MeshFilter candidate = candidates[c];
                if (used.Contains(candidate) || candidate.sharedMesh == null)
                    continue;
                float distance = Vector3.Distance(candidate.transform.position, expected);
                if (distance < nearestAnyDistance)
                {
                    nearestAny = candidate;
                    nearestAnyDistance = distance;
                }
                bool compatible =
                    candidate.sharedMesh.vertexCount == actor.vertexCount ||
                    TryGetCombinedVertexRange(
                        candidate,
                        actor.vertexCount,
                        out _,
                        out _);
                if (compatible && distance < bestDistance)
                {
                    best = candidate;
                    bestDistance = distance;
                }
            }

            if (best == null)
            {
                vertexMismatch++;
                if (mismatchExamples.Count < 8)
                {
                    mismatchExamples.Add(
                        actor.actor + "/" + actor.mesh +
                        " expected=" + actor.vertexCount +
                        " nearest=" +
                        (nearestAny != null
                            ? nearestAny.sharedMesh.name + ":" +
                                nearestAny.sharedMesh.vertexCount +
                                " distance=" + nearestAnyDistance.ToString("G6")
                            : "none") +
                        " candidates=" + candidates.Count);
                }
                if (isBridge)
                    bridgeStatus = "no scene mesh with " + actor.vertexCount + " vertices";
                continue;
            }
            if (bestDistance > 0.05f)
            {
                positionMismatch++;
                if (isBridge)
                    bridgeStatus = "position mismatch distance=" + bestDistance.ToString("G6");
                continue;
            }

            byte[] visibility = Convert.FromBase64String(actor.visibility);
            if (visibility.Length != actor.vertexCount)
                continue;

            bool direct = best.sharedMesh.vertexCount == actor.vertexCount;
            if (direct)
            {
                L2StaticMeshSunMaskRuntime component =
                    best.gameObject.AddComponent<L2StaticMeshSunMaskRuntime>();
                component.Initialize(best, visibility);
            }
            else if (!TryAddCombinedVisibility(
                best,
                visibility,
                combinedStreams,
                out string combinedError))
            {
                vertexMismatch++;
                if (mismatchExamples.Count < 8)
                {
                    mismatchExamples.Add(
                        actor.actor + "/" + actor.mesh +
                        " expected=" + actor.vertexCount +
                        " combined=" + best.sharedMesh.name + ":" +
                        best.sharedMesh.vertexCount +
                        " error=" + combinedError);
                }
                continue;
            }

            used.Add(best);
            applied++;
            if (isBridge)
                bridgeStatus = "applied distance=" + bestDistance.ToString("G6") +
                    " vertices=" + actor.vertexCount;
        }

        InstallCombinedStreams(combinedStreams);

        // Debug.Log("[L2StaticMeshSunMask] " + scene.name + ": applied " + applied +
            // " of " + data.actors.Length +
            // " missingName=" + missingMeshName +
            // " positionMismatch=" + positionMismatch +
            // " vertexMismatch=" + vertexMismatch +
            // " zeroMaskSunFallback=0" +
            // " sunMaskDebug=" + (ShowSunMaskDebug ? "1" : "0") +
            // "; bridge=" + bridgeStatus + "." +
            // (mismatchExamples.Count > 0
                // ? Environment.NewLine + "Mismatch examples: " +
                    // string.Join(" | ", mismatchExamples)
                // : ""));
    }
}
