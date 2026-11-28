using UnityEngine;

public static partial class EntitySpawnShared
{
    public const string ClickAreaName = "click_area";

    /// <summary>
    /// Every NPC/monster must have a collider on <c>EntityClick</c> so <see cref="EntityPick"/> can
    /// select it — including fallback cubes that have no imported <c>click_area</c>.
    /// CharacterController stays on Default and must not be moved onto EntityClick.
    /// </summary>
    public static void EnsureEntityClickArea(GameObject go)
    {
        if (go == null)
            return;

        int layer = LayerMask.NameToLayer("EntityClick");
        if (layer < 0)
            return;

        Transform area = FindClickArea(go.transform);
        if (area == null)
        {
            var clickGo = new GameObject(ClickAreaName);
            clickGo.transform.SetParent(go.transform, false);
            clickGo.layer = layer;
            area = clickGo.transform;
        }
        else
        {
            SetLayer(area.gameObject, layer);
        }

        // click_area is a pick volume only. Imported prefabs keep a disabled cube MeshRenderer on it whose
        // material is not in the project; it must never draw, whatever later toggles Renderer.enabled.
        Renderer[] pickRenderers = area.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < pickRenderers.Length; i++)
        {
            if (pickRenderers[i] == null)
                continue;
            pickRenderers[i].enabled = false;
            pickRenderers[i].forceRenderingOff = true;
        }

        if (area.GetComponent<Collider>() == null)
            FitClickCollider(go, area.gameObject);
    }

    static Transform FindClickArea(Transform root)
    {
        if (root == null)
            return null;
        Transform named = root.Find(ClickAreaName);
        if (named != null)
            return named;
        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            Transform t = children[i];
            if (t == null || t == root)
                continue;
            if (t.name.IndexOf("click_area", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return t;
        }
        return null;
    }

    static void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;
        Transform[] children = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null)
                children[i].gameObject.layer = layer;
        }
    }

    static void FitClickCollider(GameObject owner, GameObject clickGo)
    {
        Bounds bounds;
        if (!TryWorldBounds(owner, out bounds))
        {
            var box = clickGo.AddComponent<BoxCollider>();
            box.size = new Vector3(0.8f, 1.0f, 0.8f);
            box.center = new Vector3(0f, 0.5f, 0f);
            box.isTrigger = true;
            return;
        }

        Vector3 localCenter = owner.transform.InverseTransformPoint(bounds.center);
        Vector3 worldSize = bounds.size;
        Vector3 lossy = owner.transform.lossyScale;
        Vector3 localSize = new Vector3(
            SafeDiv(worldSize.x, lossy.x),
            SafeDiv(worldSize.y, lossy.y),
            SafeDiv(worldSize.z, lossy.z));
        localSize.x = Mathf.Max(localSize.x, 0.4f);
        localSize.y = Mathf.Max(localSize.y, 0.4f);
        localSize.z = Mathf.Max(localSize.z, 0.4f);

        var collider = clickGo.AddComponent<BoxCollider>();
        collider.center = localCenter;
        collider.size = localSize;
        collider.isTrigger = true;
    }

    static bool TryWorldBounds(GameObject owner, out Bounds bounds)
    {
        bounds = new Bounds(owner.transform.position, Vector3.zero);
        bool any = false;
        CharacterController cc = owner.GetComponent<CharacterController>();
        if (cc != null)
        {
            bounds = cc.bounds;
            any = true;
        }
        Renderer[] renderers = owner.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null || r is ParticleSystemRenderer)
                continue;
            if (r.gameObject.name == ClickAreaName)
                continue;
            if (!any)
            {
                bounds = r.bounds;
                any = true;
            }
            else
                bounds.Encapsulate(r.bounds);
        }
        return any;
    }

    static float SafeDiv(float value, float scale)
    {
        float s = Mathf.Abs(scale) < 0.0001f ? 1f : scale;
        return value / s;
    }
}
