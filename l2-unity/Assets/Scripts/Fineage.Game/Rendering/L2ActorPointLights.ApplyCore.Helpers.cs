using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    static void PruneActorGroups()
    {
        for (int i = Groups.Count - 1; i >= 0; i--)
        {
            ActorGroup group = Groups[i];
            if (group.Root != null)
                continue;
            GroupsByRoot.Remove(group.RootId);
            Groups.RemoveAt(i);
        }
    }

    static bool TryAdoptAnimatorRoot(Transform root)
    {
        if (root == null)
            return false;

        Animator animator = null;
        Animator[] animators = root.GetComponentsInChildren<Animator>(false);
        for (int i = 0; i < animators.Length; i++)
        {
            Animator candidate = animators[i];
            if (candidate == null || candidate.GetComponentInParent<Animator>() != candidate)
                continue;
            animator = candidate;
            break;
        }

        if (animator == null)
            return false;

        Transform actorRoot = animator.transform;
        int rootId = actorRoot.GetInstanceID();
        if (GroupsByRoot.ContainsKey(rootId))
            return false;

        SkinnedMeshRenderer[] skinned = animator.GetComponentsInChildren<SkinnedMeshRenderer>(false);
        ActorGroup group = null;
        for (int s = 0; s < skinned.Length; s++)
        {
            SkinnedMeshRenderer renderer = skinned[s];
            if (!IsActor(renderer))
                continue;
            if (group == null)
            {
                group = new ActorGroup();
                group.Root = actorRoot;
                group.RootId = rootId;
            }

            group.Renderers.Add(renderer);
        }

        if (group == null)
            return false;

        GroupsByRoot.Add(rootId, group);
        Groups.Add(group);
        return true;
    }

    static void BootstrapSceneActorsIfNeeded()
    {
        if (Groups.Count > 0)
            return;

        Animator[] animators = Object.FindObjectsByType<Animator>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < animators.Length; i++)
        {
            Animator animator = animators[i];
            if (animator == null || animator.GetComponentInParent<Animator>() != animator)
                continue;
            TryAdoptAnimatorRoot(animator.transform.root);
        }

        if (!_sceneActorBootstrapLogged && Groups.Count > 0)
        {
            _sceneActorBootstrapLogged = true;
            Debug.LogWarning(
                "[L2ActorPointLights] Bootstrapped actor groups from a one-time scene scan because no spawn registry entries exist yet.");
        }
    }

    static bool IsActor(Renderer renderer)
    {
        Material[] materials = renderer.sharedMaterials;
        for (int i = 0; i < materials.Length; i++)
        {
            Material material = materials[i];
            if (material == null || material.shader == null)
                continue;
            string name = material.shader.name;
            if (name == "L2/World/ActorModulate2X" || name == "L2/World/ActorEnvSpecular")
                return true;
        }
        return false;
    }
}
