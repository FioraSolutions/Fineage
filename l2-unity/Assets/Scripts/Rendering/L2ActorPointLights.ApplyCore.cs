using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    static bool Usable(L2ActorLocalLight light)
    {
        return light != null && light.IsActive;
    }

    static bool AlreadyChosen(int index, int chosen)
    {
        for (int i = 0; i < chosen; i++)
        {
            if (Chosen[i] == index)
                return true;
        }
        return false;
    }

    static void Refresh()
    {
        int previousLights = Lights.Count;
        Lights.Clear();
        L2ActorLocalLight[] found = Object.FindObjectsByType<L2ActorLocalLight>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null)
                Lights.Add(found[i]);
        }

        if (Lights.Count != previousLights)
            _rewriteActors = true;

        PruneActors();
        AdoptActors();
    }

    static void PruneActors()
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

    static void AdoptActors()
    {
        Animator[] animators = Object.FindObjectsByType<Animator>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < animators.Length; i++)
        {
            Animator animator = animators[i];
            if (animator == null)
                continue;
            Transform root = animator.transform;
            int rootId = root.GetInstanceID();
            if (GroupsByRoot.ContainsKey(rootId))
                continue;
            if (animator.GetComponentInParent<Animator>() != animator)
                continue;

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
                    group.Root = root;
                    group.RootId = rootId;
                }

                group.Renderers.Add(renderer);
            }

            if (group == null)
                continue;
            GroupsByRoot.Add(rootId, group);
            Groups.Add(group);
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
