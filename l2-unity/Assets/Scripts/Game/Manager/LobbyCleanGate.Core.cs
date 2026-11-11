#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class LobbyCleanGate
{
    public static Report Evaluate()
    {
        Report r = new Report();
        r.MenuLoaded = SceneManager.GetSceneByName("Menu").isLoaded;
        r.LobbyLoaded = SceneManager.GetSceneByName("l2_lobby").isLoaded;

        int charSelectLayer = LobbyPresentation.Layer;
        int faces = 0;
        int layerObjs = 0;
        Renderer[] renderers = Object.FindObjectsByType<Renderer>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer rend = renderers[i];
            if (rend == null || !rend.enabled || !rend.gameObject.activeInHierarchy)
                continue;
            string path = BuildPath(rend.transform);
            if (charSelectLayer >= 0 && rend.gameObject.layer == charSelectLayer)
                layerObjs++;
            if (!LooksLikeOrphanLobbyMesh(rend.transform, path))
                continue;
            faces++;
            if (faces <= 12)
            {
                FineageLog.Info(
                    FineageLogCategory.World,
                    "LobbyCleanGate FACE path=" + path
                    + " scene=" + rend.gameObject.scene.name
                    + " layer=" + LayerMask.LayerToName(rend.gameObject.layer)
                    + " pos=" + rend.transform.position);
            }
        }

        r.FaceLeftovers = faces;
        r.CharSelectLayerObjs = layerObjs;
        r.Ok = !r.MenuLoaded && !r.LobbyLoaded && faces == 0 && layerObjs == 0;
        r.Detail = "MenuLoaded=" + r.MenuLoaded
            + " LobbyLoaded=" + r.LobbyLoaded
            + " faceLeftovers=" + faces
            + " charSelectLayerObjs=" + layerObjs
            + " Camera.main=" + (Camera.main != null ? Camera.main.name : "null")
            + " Player=" + (DomainPacketServices.LocalPlayer != null ? DomainPacketServices.LocalPlayer.name : "null");
        return r;
    }

    static bool LooksLikeOrphanLobbyMesh(Transform t, string path)
    {
        if (BelongsToWorldEntity(t))
            return false;
        string p = path.ToLowerInvariant();
        // Real leftovers live under Game "Prefabs/{race}/..." catalog or lobby hosts.
        return p.StartsWith("prefabs/")
            || p.Contains("pawn")
            || p.Contains("/characters")
            || p.Contains("lobbypresentation")
            || p.Contains("selectablecharacter")
            || p.Contains("_f_")
            || p.Contains("_h_");
    }
}
#endif
