using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class LobbyPresentation
{
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
