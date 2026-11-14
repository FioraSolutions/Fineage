using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Town doorways are a solid wall with an invisible portal sheet in front.
/// Cut that rectangle out of the baked brush mesh so the passage is open.
/// </summary>
public sealed class L2BrushPortal : MonoBehaviour
{
    struct Portal
    {
        public int Axis;
        public float Plane;
        public int A;
        public int B;
        public float A0, A1, B0, B1;
    }

    struct Vert
    {
        public Vector3 P;
        public Vector3 N;
        public Vector2 Uv;
        public float Get(int axis)
        {
            if (axis == 0) return P.x;
            if (axis == 1) return P.y;
            return P.z;
        }
    }

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
        MeshFilter[] filters = FindObjectsByType<MeshFilter>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < filters.Length; i++)
        {
            MeshFilter filter = filters[i];
            if (filter == null || filter.gameObject.name != "BSP")
                continue;
            if (!_done.Add(filter.GetInstanceID()))
                continue;
            Open(filter);
        }
    }

    void Open(MeshFilter filter)
    {
        Mesh source = filter.sharedMesh;
        if (source == null || !source.isReadable)
            return;
        string tile = filter.gameObject.scene.name;
        TextAsset file = Resources.Load<TextAsset>("Data/Maps/" + tile + "/" + tile + ".portals");
        if (file == null || string.IsNullOrEmpty(file.text))
            return;
        List<Portal> portals = Parse(file.text);
        if (portals.Count == 0)
            return;

        var verts = new List<Vector3>();
        var norms = new List<Vector3>();
        var uvs = new List<Vector2>();
        source.GetVertices(verts);
        source.GetNormals(norms);
        source.GetUVs(0, uvs);
        var built = new List<Vert>(verts.Count);
        for (int i = 0; i < verts.Count; i++)
        {
            built.Add(new Vert
            {
                P = verts[i],
                N = i < norms.Count ? norms[i] : Vector3.up,
                Uv = i < uvs.Count ? uvs[i] : Vector2.zero
            });
        }

        var tris = new List<int>[source.subMeshCount];
        bool changed = false;
        for (int s = 0; s < source.subMeshCount; s++)
        {
            int[] src = source.GetTriangles(s);
            var dst = new List<int>(src.Length);
            for (int i = 0; i + 2 < src.Length; i += 3)
            {
                Vert a = built[src[i]];
                Vert b = built[src[i + 1]];
                Vert c = built[src[i + 2]];
                var piece = new List<Vert>(3) { a, b, c };
                var parts = new List<List<Vert>>(4) { piece };
                for (int p = 0; p < portals.Count; p++)
                    parts = Cut(parts, portals[p]);
                if (parts.Count == 1 && parts[0].Count == 3
                    && Same(parts[0][0], a) && Same(parts[0][1], b) && Same(parts[0][2], c))
                {
                    dst.Add(src[i]);
                    dst.Add(src[i + 1]);
                    dst.Add(src[i + 2]);
                    continue;
                }
                changed = true;
                for (int k = 0; k < parts.Count; k++)
                    AddFan(built, dst, parts[k]);
            }
            tris[s] = dst;
        }
        if (!changed)
            return;

        var mesh = new Mesh { name = source.name + "_open", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        var pv = new Vector3[built.Count];
        var pn = new Vector3[built.Count];
        var pu = new Vector2[built.Count];
        for (int i = 0; i < built.Count; i++)
        {
            pv[i] = built[i].P;
            pn[i] = built[i].N;
            pu[i] = built[i].Uv;
        }
        mesh.vertices = pv;
        mesh.normals = pn;
        mesh.uv = pu;
        mesh.subMeshCount = tris.Length;
        for (int s = 0; s < tris.Length; s++)
            mesh.SetTriangles(tris[s], s);
        mesh.RecalculateBounds();
        filter.sharedMesh = mesh;
        MeshCollider col = filter.GetComponent<MeshCollider>();
        if (col != null)
            col.sharedMesh = mesh;
    }

    static bool Same(Vert a, Vert b)
    {
        return (a.P - b.P).sqrMagnitude < 1e-8f;
    }

    static void AddFan(List<Vert> built, List<int> dst, List<Vert> poly)
    {
        if (poly == null || poly.Count < 3)
            return;
        int s0 = built.Count;
        for (int i = 0; i < poly.Count; i++)
            built.Add(poly[i]);
        for (int i = 1; i < poly.Count - 1; i++)
        {
            dst.Add(s0);
            dst.Add(s0 + i);
            dst.Add(s0 + i + 1);
        }
    }

    static List<List<Vert>> Cut(List<List<Vert>> parts, Portal portal)
    {
        var next = new List<List<Vert>>(parts.Count);
        for (int i = 0; i < parts.Count; i++)
        {
            List<Vert> poly = parts[i];
            if (!Coplanar(poly, portal))
            {
                next.Add(poly);
                continue;
            }
            List<List<Vert>> outside = Outside(poly, portal);
            if (outside.Count == 0)
                continue;
            next.AddRange(outside);
        }
        return next;
    }

    static bool Coplanar(List<Vert> poly, Portal portal)
    {
        float min = float.MaxValue;
        float max = float.MinValue;
        for (int i = 0; i < poly.Count; i++)
        {
            float c = poly[i].Get(portal.Axis);
            if (c < min) min = c;
            if (c > max) max = c;
        }
        if (max - min > 0.2f)
            return false;
        float mid = (min + max) * 0.5f;
        return Mathf.Abs(mid - portal.Plane) < 0.25f;
    }

    static List<List<Vert>> Outside(List<Vert> poly, Portal portal)
    {
        var result = new List<List<Vert>>(4);
        // Left of the hole, right of the hole, and the bands above and below between them.
        Add(result, Clip(poly, portal, (u, v) => u <= portal.A0));
        Add(result, Clip(poly, portal, (u, v) => u >= portal.A1));
        Add(result, Clip(poly, portal, (u, v) => u >= portal.A0 && u <= portal.A1 && v <= portal.B0));
        Add(result, Clip(poly, portal, (u, v) => u >= portal.A0 && u <= portal.A1 && v >= portal.B1));
        return result;
    }

    static void Add(List<List<Vert>> result, List<Vert> poly)
    {
        if (poly != null && poly.Count >= 3)
            result.Add(poly);
    }

    static List<Vert> Clip(List<Vert> poly, Portal portal, System.Func<float, float, bool> inside)
    {
        // Clip against the four edges by intersecting with the half-spaces one at a time
        // is not available here: `inside` is the final region, so clip with its bounding lines.
        // Regions used above are each a single half-plane or a box. Handle the box as three clips.
        return ClipBox(poly, portal, inside);
    }

    static List<Vert> ClipBox(List<Vert> poly, Portal portal, System.Func<float, float, bool> inside)
    {
        // Sample the region by clipping to every half-plane that contains it.
        // The four call sites are the only ones, so match them explicitly.
        bool left = inside(-1e6f, 0f) && !inside(1e6f, 0f);
        bool right = inside(1e6f, 0f) && !inside(-1e6f, 0f);
        bool below = inside((portal.A0 + portal.A1) * 0.5f, -1e6f);
        bool above = inside((portal.A0 + portal.A1) * 0.5f, 1e6f);
        List<Vert> cur = poly;
        if (left)
            cur = Half(cur, portal, true, portal.A0, true);
        else if (right)
            cur = Half(cur, portal, true, portal.A1, false);
        else
        {
            cur = Half(cur, portal, true, portal.A0, false);
            cur = Half(cur, portal, true, portal.A1, true);
            if (below)
                cur = Half(cur, portal, false, portal.B0, true);
            else if (above)
                cur = Half(cur, portal, false, portal.B1, false);
        }
        return cur;
    }

    static List<Vert> Half(List<Vert> poly, Portal portal, bool useA, float limit, bool keepLess)
    {
        if (poly == null || poly.Count == 0)
            return poly;
        var next = new List<Vert>(poly.Count + 1);
        for (int i = 0; i < poly.Count; i++)
        {
            Vert cur = poly[i];
            Vert prev = poly[(i + poly.Count - 1) % poly.Count];
            float ca = Coord(cur, portal, useA);
            float pa = Coord(prev, portal, useA);
            bool cin = keepLess ? ca <= limit : ca >= limit;
            bool pin = keepLess ? pa <= limit : pa >= limit;
            if (cin != pin)
            {
                float d = ca - pa;
                float t = Mathf.Abs(d) < 1e-8f ? 0f : (limit - pa) / d;
                next.Add(Lerp(prev, cur, t));
            }
            if (cin)
                next.Add(cur);
        }
        return next;
    }

    static float Coord(Vert v, Portal portal, bool useA)
    {
        return v.Get(useA ? portal.A : portal.B);
    }

    static Vert Lerp(Vert a, Vert b, float t)
    {
        return new Vert
        {
            P = Vector3.LerpUnclamped(a.P, b.P, t),
            N = Vector3.LerpUnclamped(a.N, b.N, t),
            Uv = Vector2.LerpUnclamped(a.Uv, b.Uv, t)
        };
    }

    static List<Portal> Parse(string text)
    {
        var list = new List<Portal>();
        string[] lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.Length == 0)
                continue;
            string[] p = line.Split(' ');
            if (p.Length < 8)
                continue;
            list.Add(new Portal
            {
                Axis = int.Parse(p[0], CultureInfo.InvariantCulture),
                Plane = float.Parse(p[1], CultureInfo.InvariantCulture),
                A = int.Parse(p[2], CultureInfo.InvariantCulture),
                B = int.Parse(p[3], CultureInfo.InvariantCulture),
                A0 = float.Parse(p[4], CultureInfo.InvariantCulture),
                A1 = float.Parse(p[5], CultureInfo.InvariantCulture),
                B0 = float.Parse(p[6], CultureInfo.InvariantCulture),
                B1 = float.Parse(p[7], CultureInfo.InvariantCulture)
            });
        }
        return list;
    }
}
