using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Char-select / create meshes live under Menu <c>Lobby/LobbyPresentationRoot</c>
/// on layer <c>CharSelect</c>. Ownership = Menu scene: <see cref="SceneManager.UnloadSceneAsync"/>
/// is the cleanup (Unity multi-scene / active-scene Instantiate rules).
/// </summary>
public static class LobbyPresentation
{
    public const string LayerName = "CharSelect";
    public const string ClickLayerName = "EntityClick";
    public const string RootName = "LobbyPresentationRoot";
    public const string ClickAreaName = "click_area";
    const string MenuSceneName = "Menu";

    static int _layer = int.MinValue;
    static int _clickLayer = int.MinValue;
    static Transform _root;

    public static int Layer
    {
        get
        {
            if (_layer == int.MinValue)
                _layer = LayerMask.NameToLayer(LayerName);
            return _layer;
        }
    }

    static int ClickLayer
    {
        get
        {
            if (_clickLayer == int.MinValue)
                _clickLayer = LayerMask.NameToLayer(ClickLayerName);
            return _clickLayer;
        }
    }

    public static Transform GetOrCreateRoot()
    {
        if (_root != null)
        {
            EnsureInMenuScene(_root.gameObject);
            return _root;
        }

        Transform lobby = ResolveLobbyTransform();
        if (lobby != null)
        {
            Transform existing = lobby.Find(RootName);
            if (existing != null)
            {
                _root = existing;
                EnsureInMenuScene(_root.gameObject);
                return _root;
            }

            GameObject rootGo = new GameObject(RootName);
            rootGo.transform.SetParent(lobby, false);
            ApplyLayer(rootGo);
            _root = rootGo.transform;
            EnsureInMenuScene(_root.gameObject);
            return _root;
        }

        Debug.LogWarning("[LobbyPresentation] Lobby missing; staging root in Menu scene.");
        GameObject orphan = new GameObject(RootName);
        ApplyLayer(orphan);
        EnsureInMenuScene(orphan);
        _root = orphan.transform;
        return _root;
    }

    public static void Host(GameObject presentationRoot)
    {
        if (presentationRoot == null)
            return;

        presentationRoot.transform.SetParent(GetOrCreateRoot(), true);
        ApplyLayer(presentationRoot);
        EnsureInMenuScene(GetOrCreateRoot().gameObject);
    }

    /// <summary>Drop cached root after Menu unload (Unity fake-nulls the Transform).</summary>
    public static void ForgetRoot()
    {
        _root = null;
    }

    public static void ApplyLayer(GameObject root)
    {
        int layer = Layer;
        if (root == null || layer < 0)
            return;
        ApplyLayersRecursive(root.transform, layer, ClickLayer);
    }

    public static void EnsureCameraSeesLobby(Camera cam)
    {
        int layer = Layer;
        if (cam == null || layer < 0)
            return;
        cam.cullingMask |= 1 << layer;
    }

    static void EnsureInMenuScene(GameObject go)
    {
        if (go == null)
            return;
        Scene menu = SceneManager.GetSceneByName(MenuSceneName);
        if (!menu.IsValid() || !menu.isLoaded)
            return;
        if (go.scene != menu)
            SceneManager.MoveGameObjectToScene(go, menu);
    }

    static Transform ResolveLobbyTransform()
    {
        CharacterCreator creator = LoginPacketServices.Creator;
        return creator != null ? creator.transform : null;
    }

    /// <summary>
    /// Paint CharSelect on the pawn tree, but keep <c>click_area</c> on EntityClick
    /// (CharacterSelector raycasts only that layer) and ensure its collider is on.
    /// </summary>
    static void ApplyLayersRecursive(Transform node, int charSelectLayer, int entityClickLayer)
    {
        if (node.name == ClickAreaName && entityClickLayer >= 0)
        {
            node.gameObject.layer = entityClickLayer;
            MeshCollider col = node.GetComponent<MeshCollider>();
            if (col != null)
                col.enabled = true;
        }
        else if (entityClickLayer < 0 || node.gameObject.layer != entityClickLayer)
        {
            node.gameObject.layer = charSelectLayer;
        }

        for (int i = 0; i < node.childCount; i++)
            ApplyLayersRecursive(node.GetChild(i), charSelectLayer, entityClickLayer);
    }
}
