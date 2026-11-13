using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Dark Elf village sits under the 20_18 heightmap. That sheet is what you
/// fall through and get thrown out of after a scroll: the controller is inside
/// the mesh, so it sinks and then pops upward. While the player is under the
/// cave mouth the sheet stays off, every frame, so a scene load cannot turn it
/// back on. Static-mesh floors and the tunnel stay.
/// </summary>
public sealed class L2CaveTerrainGate : MonoBehaviour
{
    // Village floor is about -90. The north mouth is about +30.
    const float CaveLipUnityY = 45f;
    const string TerrainName = "20_18";

    struct Piece
    {
        public Renderer Renderer;
        public Collider Collider;
        public Terrain Terrain;
    }

    readonly List<Piece> _pieces = new List<Piece>();
    bool _cave;

    void LateUpdate()
    {
        if (!SceneManager.GetSceneByName(TerrainName).isLoaded)
            return;

        if (_pieces.Count == 0)
            TryBind();
        if (_pieces.Count == 0)
            return;

        float y = SampleY();
        bool inCave = y < CaveLipUnityY;
        if (inCave == _cave && !inCave)
            return;

        _cave = inCave;
        for (int i = 0; i < _pieces.Count; i++)
        {
            Piece piece = _pieces[i];
            if (piece.Renderer != null)
                piece.Renderer.enabled = !inCave;
            if (piece.Collider != null)
                piece.Collider.enabled = !inCave;
            if (piece.Terrain != null)
            {
                piece.Terrain.enabled = !inCave;
                piece.Terrain.drawHeightmap = !inCave;
            }
        }
    }

    static float SampleY()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null)
            return player.transform.position.y;

        Camera cam = Camera.main;
        return cam != null ? cam.transform.position.y : CaveLipUnityY + 1f;
    }

    void TryBind()
    {
        var seen = new HashSet<int>();
        MeshRenderer[] meshes = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < meshes.Length; i++)
        {
            MeshRenderer renderer = meshes[i];
            if (renderer == null || renderer.gameObject.name != TerrainName)
                continue;
            if (!seen.Add(renderer.gameObject.GetInstanceID()))
                continue;
            _pieces.Add(new Piece
            {
                Renderer = renderer,
                Collider = renderer.GetComponent<Collider>()
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
                Collider = terrain.GetComponent<Collider>()
            });
        }
    }
}
