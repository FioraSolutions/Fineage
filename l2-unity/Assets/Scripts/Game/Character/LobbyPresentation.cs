using UnityEngine;

/// <summary>
/// Lobby select/create pawns share Logongrp world coordinates with Talking Island.
/// Keep them on <c>LoginBG</c> so only login cameras draw them; gameplay cameras exclude the layer.
/// </summary>
public static class LobbyPresentation
{
    public const string LayerName = "LoginBG";

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

    public static void ApplyLayer(GameObject root)
    {
        int layer = Layer;
        if (root == null || layer < 0)
            return;
        SetLayerRecursively(root.transform, layer);
    }

    public static void ExcludeFromWorldCameras()
    {
        int layer = Layer;
        if (layer < 0)
            return;

        Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < cameras.Length; i++)
        {
            Camera cam = cameras[i];
            if (cam == null || IsLoginCamera(cam))
                continue;
            cam.cullingMask &= ~(1 << layer);
        }
    }

    static bool IsLoginCamera(Camera cam)
    {
        // LoginCameraManager owns Login / CharSelect / race cameras in l2_lobby.
        Transform t = cam.transform;
        while (t != null)
        {
            string n = t.name;
            if (n == "Login" || n == "CharSelect" || n == "Human" || n == "Elf" ||
                n == "DarkElf" || n == "Orc" || n == "Dwarf" || n == "Cameras")
                return true;
            t = t.parent;
        }
        return false;
    }

    static void SetLayerRecursively(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        for (int i = 0; i < root.childCount; i++)
            SetLayerRecursively(root.GetChild(i), layer);
    }
}
