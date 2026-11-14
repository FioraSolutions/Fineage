using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Village door slabs are part of the building mesh, so the mesh collider
/// seals the arch. Drop those submeshes. Window cards and door trim stay.
/// </summary>
public sealed class L2DoorPassage : MonoBehaviour
{
    readonly HashSet<int> _done = new HashSet<int>();
    float _next;
    int _tries;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnScene;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnScene;
    }

    void OnScene(Scene scene, LoadSceneMode mode)
    {
        _tries = 0;
        _next = 0f;
    }

    void LateUpdate()
    {
        if (_tries > 8)
            return;
        if (Time.unscaledTime < _next)
            return;
        _next = Time.unscaledTime + 1.5f;
        _tries++;
        MeshCollider[] cols = FindObjectsByType<MeshCollider>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < cols.Length; i++)
            TryOpen(cols[i]);
    }

    void TryOpen(MeshCollider col)
    {
        if (col == null || !_done.Add(col.GetInstanceID()))
            return;

        MeshRenderer renderer = col.GetComponent<MeshRenderer>();
        Mesh source = col.sharedMesh;
        if (renderer == null || source == null)
            return;

        Material[] mats = renderer.sharedMaterials;
        int subs = source.subMeshCount;
        bool any = false;
        var drop = new bool[subs];
        for (int i = 0; i < subs; i++)
        {
            Material mat = i < mats.Length ? mats[i] : null;
            if (!IsDoorSlab(mat != null ? mat.name : null))
                continue;
            drop[i] = true;
            any = true;
        }
        if (!any)
            return;
        if (!source.isReadable)
        {
            _done.Remove(col.GetInstanceID());
            return;
        }

        var kept = new List<int>();
        for (int s = 0; s < subs; s++)
        {
            if (drop[s])
                continue;
            kept.AddRange(source.GetTriangles(s));
        }
        if (kept.Count == 0)
        {
            col.enabled = false;
            return;
        }

        var mesh = new Mesh { name = source.name + "_passage" };
        if (source.vertexCount > 65000)
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = source.vertices;
        mesh.triangles = kept.ToArray();
        mesh.RecalculateBounds();
        col.sharedMesh = mesh;

        MeshFilter filter = col.GetComponent<MeshFilter>();
        if (filter != null && filter.sharedMesh == source)
        {
            Mesh visual = Instantiate(source);
            visual.name = source.name + "_open";
            for (int s = 0; s < subs; s++)
            {
                if (drop[s])
                    visual.SetTriangles(System.Array.Empty<int>(), s);
            }
            filter.sharedMesh = visual;
        }
    }

    static bool IsDoorSlab(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        string n = name.ToLowerInvariant();
        if (n.Contains("deco") || n.Contains("frame") || n.Contains("nail") || n.Contains("lod"))
            return false;
        return n.Contains("door");
    }
}
