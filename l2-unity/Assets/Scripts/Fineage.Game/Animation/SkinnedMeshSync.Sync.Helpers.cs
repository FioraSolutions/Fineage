using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static bool IsFaceOrHairRenderer(SkinnedMeshRenderer renderer)
    {
        if (renderer == null)
            return false;
        string n = renderer.name;
        if (string.IsNullOrEmpty(n))
            return false;
        // MOrc_m000_f / MOrc_f_0 / MOrc_m000_m00_bh / MOrc_h_0_0_bh
        return n.IndexOf("_f", System.StringComparison.OrdinalIgnoreCase) >= 0
            || n.IndexOf("_h_", System.StringComparison.OrdinalIgnoreCase) >= 0
            || n.IndexOf("_bh", System.StringComparison.OrdinalIgnoreCase) >= 0
            || n.IndexOf("_ah", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static bool TryFindBoneAlias(Dictionary<string, Transform> skeleton, string name, out Transform donor)
    {
        donor = null;
        if (string.IsNullOrEmpty(name))
            return false;
        if (name.EndsWith("_end", System.StringComparison.OrdinalIgnoreCase))
        {
            string trimmed = name.Substring(0, name.Length - 4);
            if (TryFindBone(skeleton, trimmed, out donor))
                return true;
        }
        // Spaces vs underscores (NPC clergy style vs player underscore style).
        string alt = name.IndexOf(' ') >= 0
            ? name.Replace(' ', '_')
            : name.Replace('_', ' ');
        return !string.Equals(alt, name, System.StringComparison.Ordinal)
            && TryFindBone(skeleton, alt, out donor);
    }

    /// <summary>
    /// Parent a hair chain that shares no bone names with the body onto Bip01_head,
    /// keeping the bind-pose offset so the mesh does not orbit the neck.
    /// </summary>
    static void AttachForeignHairChain(
        SkinnedMeshRenderer renderer,
        Transform head,
        SkinnedMeshRenderer skeletonMesh)
    {
        Transform[] srcBones = renderer != null ? renderer.bones : null;
        if (srcBones == null || head == null)
            return;

        HashSet<Transform> skinned = new HashSet<Transform>();
        for (int i = 0; i < srcBones.Length; i++)
        {
            if (srcBones[i] != null)
                skinned.Add(srcBones[i]);
        }

        bool attached = false;
        for (int i = 0; i < srcBones.Length; i++)
        {
            Transform bone = srcBones[i];
            if (bone == null || skinned.Contains(bone.parent))
                continue;
            if (bone.name.StartsWith("Bip01", System.StringComparison.OrdinalIgnoreCase))
                continue;

            if (TryBindWorld(renderer, bone, out Matrix4x4 hairBind)
                && TryBindWorld(skeletonMesh, head, out Matrix4x4 headBind))
            {
                Matrix4x4 local = headBind.inverse * hairBind;
                bone.SetParent(head, false);
                bone.localPosition = local.GetColumn(3);
                bone.localRotation = local.rotation;
                bone.localScale = new Vector3(
                    local.GetColumn(0).magnitude,
                    local.GetColumn(1).magnitude,
                    local.GetColumn(2).magnitude);
            }
            else
            {
                bone.SetParent(head, true);
            }

            if (!attached)
            {
                renderer.rootBone = bone;
                attached = true;
            }
        }

        if (attached)
            renderer.updateWhenOffscreen = true;
    }

    static bool TryBindWorld(SkinnedMeshRenderer smr, Transform bone, out Matrix4x4 world)
    {
        world = Matrix4x4.identity;
        Mesh mesh = smr != null ? smr.sharedMesh : null;
        if (mesh == null || bone == null)
            return false;

        Transform[] bones = smr.bones;
        Matrix4x4[] poses = mesh.bindposes;
        if (bones == null || poses == null)
            return false;

        int count = bones.Length < poses.Length ? bones.Length : poses.Length;
        for (int i = 0; i < count; i++)
        {
            if (bones[i] != bone)
                continue;
            world = smr.transform.localToWorldMatrix * poses[i].inverse;
            return true;
        }

        return false;
    }
}
