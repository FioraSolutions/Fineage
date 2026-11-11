#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class LobbyCleanGate
{
    static bool BelongsToWorldEntity(Transform t)
    {
        for (Transform cur = t; cur != null; cur = cur.parent)
        {
            if (cur.GetComponent<Entity>() != null)
                return true;
            if (cur.GetComponent<PlayerController>() != null)
                return true;
        }
        return false;
    }

    static string BuildPath(Transform t)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(64);
        for (Transform cur = t; cur != null; cur = cur.parent)
        {
            if (sb.Length > 0)
                sb.Insert(0, '/');
            sb.Insert(0, cur.name);
        }
        return sb.ToString();
    }
}
#endif
