using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Char-select / create presentation: owned by the unloadable Lobby hierarchy,
/// drawn only on layer <c>CharSelect</c>. Gameplay cameras omit that layer.
/// Primary cleanup is Menu / l2_lobby unload — not Destroy sweeps.
/// </summary>
public static class LobbyPresentation
{
    public const string LayerName = "CharSelect";
    public const string RootName = "LobbyPresentationRoot";

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
    /// Parent under Menu's Lobby → LobbyPresentationRoot (scene unload removes it).
    /// </summary>
    public static Transform GetOrCreateRoot()
    {
        Transform lobby = FindLobbyTransform();
        if (lobby == null)
        {
            Debug.LogWarning("[LobbyPresentation] Lobby root missing; creating orphan root in Menu.");
            GameObject orphan = new GameObject(RootName);
            ApplyLayer(orphan);
            MoveToMenuIfPossible(orphan);
            return orphan.transform;
        }

        Transform existing = lobby.Find(RootName);
        if (existing != null)
            return existing;

        GameObject root = new GameObject(RootName);
        root.transform.SetParent(lobby, false);
        ApplyLayer(root);
        return root.transform;
    }

    public static void Host(GameObject presentationRoot)
    {
        if (presentationRoot == null)
            return;

        Transform stage = GetOrCreateRoot();
        presentationRoot.transform.SetParent(stage, true);
        ApplyLayer(presentationRoot);
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

    static Transform FindLobbyTransform()
    {
        CharacterCreator creator = CharacterCreator.Instance;
        if (creator != null)
            return creator.transform;

        GameObject lobby = GameObject.Find("Lobby");
        return lobby != null ? lobby.transform : null;
    }

    static void MoveToMenuIfPossible(GameObject go)
    {
        Scene menu = SceneManager.GetSceneByName("Menu");
        if (menu.IsValid() && menu.isLoaded)
            SceneManager.MoveGameObjectToScene(go, menu);
    }

    static void SetLayerRecursively(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        for (int i = 0; i < root.childCount; i++)
            SetLayerRecursively(root.GetChild(i), layer);
    }
}
