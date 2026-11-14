using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dark Elf start sits under the 20_18 heightmap. The sheet is a rock wall
/// through the village and across the stair out. Hide the drawing while the
/// body is inside. Keep only the upward faces of the collider so the stair
/// floor stays and the vertical cut does not block the exit.
/// </summary>
public sealed class L2CaveTerrainGate : MonoBehaviour
{
    const string TerrainName = "20_18";
    const float WalkableNormalY = 0.45f;

    struct Piece
    {
        public Renderer Renderer;
        public MeshCollider Collider;
        public Mesh Source;
        public Mesh Walkable;
        public Terrain Terrain;
    }

    readonly List<Piece> _pieces = new List<Piece>();
    bool _cave;

    void LateUpdate()
    {
        if (_pieces.Count == 0)
            TryBind();
        if (_pieces.Count == 0)
            return;

        bool draw = WantsSurface();
        if (draw == !_cave && _cave)
            return;

        _cave = !draw;
        for (int i = 0; i < _pieces.Count; i++)
        {
            Piece piece = _pieces[i];
            if (piece.Renderer != null)
            {
                piece.Renderer.forceRenderingOff = !draw;
                piece.Renderer.enabled = draw;
            }
            if (piece.Terrain != null)
            {
                piece.Terrain.drawHeightmap = draw;
                piece.Terrain.enabled = true;
            }
            if (piece.Collider == null)
                continue;
            piece.Collider.enabled = true;
            Mesh want = !draw && piece.Walkable != null ? piece.Walkable : piece.Source;
            if (want != null && piece.Collider.sharedMesh != want)
                piece.Collider.sharedMesh = want;
        }
    }

    static bool WantsSurface()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null)
            return false;
        Vector3 feet = player.transform.position;
        if (WorldPlacement.IsDarkElfCave(feet))
            return false;
        Camera cam = Camera.main;
        if (cam != null && cam.transform.position.y < 15f
            && WorldPlacement.IsDarkElfCave(new Vector3(cam.transform.position.x, 0f, cam.transform.position.z)))
            return false;
        return true;
    }

    void TryBind()
    {
        var seen = new HashSet<int>();
        MeshRenderer[] meshes = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < meshes.Length; i++)
        {
            MeshRenderer renderer = meshes[i];
            if (renderer == null || !IsSheet(renderer))
                continue;
            if (!seen.Add(renderer.gameObject.GetInstanceID()))
                continue;
            MeshCollider collider = renderer.GetComponent<MeshCollider>();
            Mesh source = collider != null ? collider.sharedMesh : null;
            _pieces.Add(new Piece
            {
                Renderer = renderer,
                Collider = collider,
                Source = source,
                Walkable = WalkableSheet(source)
            });
        }

        Terrain[] terrains = FindObjectsByType<Terrain>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            if (terrain == null || terrain.gameObject.name != TerrainName)
                continue;
            if (!seen.Add(terrain.gameObject.GetInstanceID()))
                continue;
            _pieces.Add(new Piece
            {
                Terrain = terrain,
                Collider = terrain.GetComponent<MeshCollider>()
            });
        }
    }

    static bool IsSheet(Renderer renderer)
    {
        if (renderer.gameObject.name == TerrainName)
            return true;
        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        Mesh mesh = filter != null ? filter.sharedMesh : null;
        return mesh != null && mesh.name == "20_18_Mesh";
    }

    static Mesh WalkableSheet(Mesh source)
    {
        if (source == null || !source.isReadable)
            return null;
        var kept = new List<int>();
        int subs = source.subMeshCount;
        Vector3[] verts = source.vertices;
        for (int s = 0; s < subs; s++)
        {
            int[] tris = source.GetTriangles(s);
            for (int i = 0; i + 2 < tris.Length; i += 3)
            {
                Vector3 a = verts[tris[i]];
                Vector3 b = verts[tris[i + 1]];
                Vector3 c = verts[tris[i + 2]];
                Vector3 n = Vector3.Cross(b - a, c - a);
                if (n.sqrMagnitude < 1e-12f)
                    continue;
                if (n.y / Mathf.Sqrt(n.sqrMagnitude) < WalkableNormalY)
                    continue;
                kept.Add(tris[i]);
                kept.Add(tris[i + 1]);
                kept.Add(tris[i + 2]);
            }
        }
        if (kept.Count == 0)
            return null;
        var mesh = new Mesh { name = source.name + "_walk" };
        if (source.vertexCount > 65000)
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = verts;
        mesh.triangles = kept.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }
}
