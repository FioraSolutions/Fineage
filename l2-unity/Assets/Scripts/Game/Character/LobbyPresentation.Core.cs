using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class LobbyPresentation
{
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
}
