using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
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
