using UnityEngine;

public static partial class L2ActorPointLights
{
    static bool _sceneActorBootstrapLogged;
    public static void RegisterLocalLight(L2ActorLocalLight light)
    {
        if (light == null || Lights.Contains(light))
            return;

        Lights.Add(light);
        _rewriteActors = true;
    }

    public static void UnregisterLocalLight(L2ActorLocalLight light)
    {
        if (light == null)
            return;

        if (Lights.Remove(light))
            _rewriteActors = true;
    }

    public static void TryRegisterActorRoot(Transform root)
    {
        if (root == null)
            return;

        int rootId = root.GetInstanceID();
        if (GroupsByRoot.ContainsKey(rootId))
            return;

        if (!TryAdoptAnimatorRoot(root))
            return;

        _rewriteActors = true;
    }

    public static void UnregisterActorRoot(Transform root)
    {
        if (root == null)
            return;

        int rootId = root.GetInstanceID();
        if (!GroupsByRoot.TryGetValue(rootId, out ActorGroup group))
            return;

        GroupsByRoot.Remove(rootId);
        Groups.Remove(group);
        _rewriteActors = true;
    }
}