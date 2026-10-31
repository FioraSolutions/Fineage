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
    public const string RootName = "LobbyPresentationRoot";
    const string MenuSceneName = "Menu";

    static int _layer = int.MinValue;
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
        // Child inherits parent scene; still pin explicitly if parent was wrong.
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
        SetLayerRecursively(root.transform, layer);
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
        // CharacterCreator lives on the Menu Lobby object.
        CharacterCreator creator = CharacterCreator.Instance;
        return creator != null ? creator.transform : null;
    }

    static void SetLayerRecursively(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        for (int i = 0; i < root.childCount; i++)
            SetLayerRecursively(root.GetChild(i), layer);
    }
}
