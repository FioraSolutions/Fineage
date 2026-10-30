using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Lobby select/create presentation shares Logongrp coordinates with the game map.
/// Host visuals in unloadable lobby scenes and on <c>LoginBG</c>; gameplay cameras
/// omit that layer (see <see cref="CameraController"/>). No Find/Destroy sweeps.
/// </summary>
public static class LobbyPresentation
{
    public const string LayerName = "LoginBG";
    const string MenuScene = "Menu";
    const string LobbyScene = "l2_lobby";

    static int _layer = int.MinValue;

    public static int Layer
    {
        get
        {
            if (_layer == int.MinValue)
                _layer = LayerMask.NameToLayer(LayerName);
            return _layer;
        }
    }

    /// <summary>
    /// Put a lobby-only root into Menu (preferred) or l2_lobby and mark LoginBG.
    /// Unloading those scenes removes the objects; DDOL Game must not own them.
    /// </summary>
    public static void HostInLobbyScene(GameObject root)
    {
        if (root == null)
            return;

        ApplyLayer(root);

        Scene menu = SceneManager.GetSceneByName(MenuScene);
        if (menu.IsValid() && menu.isLoaded)
        {
            SceneManager.MoveGameObjectToScene(root, menu);
            return;
        }

        Scene lobby = SceneManager.GetSceneByName(LobbyScene);
        if (lobby.IsValid() && lobby.isLoaded)
            SceneManager.MoveGameObjectToScene(root, lobby);
    }

    public static void ApplyLayer(GameObject root)
    {
        int layer = Layer;
        if (root == null || layer < 0)
            return;
        SetLayerRecursively(root.transform, layer);
    }

    public static void EnsureCameraSeesLobby(Camera cam)
    {
        int layer = Layer;
        if (cam == null || layer < 0)
            return;
        cam.cullingMask |= 1 << layer;
    }

    static void SetLayerRecursively(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        for (int i = 0; i < root.childCount; i++)
            SetLayerRecursively(root.GetChild(i), layer);
    }
}
