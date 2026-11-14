using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static bool IsHairRenderer(SkinnedMeshRenderer renderer)
    {
        if (renderer == null || string.IsNullOrEmpty(renderer.name))
            return false;
        string n = renderer.name;
        return n.IndexOf("_ah", System.StringComparison.OrdinalIgnoreCase) >= 0
            || n.IndexOf("_bh", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

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
    /// Blender renames a colliding right-thumb bone to Bip01_L_Finger01.001.
    /// It stays parented under Bip01_R_Finger0, so mirror the side from the parent.
    /// </summary>
    static bool TryFindDuplicateBone(Dictionary<string, Transform> skeleton, Transform src, out Transform donor)
    {
        donor = null;
        if (src == null)
            return false;
        int dot = src.name.LastIndexOf('.');
        if (dot <= 0)
            return false;
        string tail = src.name.Substring(dot + 1);
        for (int i = 0; i < tail.Length; i++)
        {
            if (!char.IsDigit(tail[i]))
                return false;
        }

        string baseName = src.name.Substring(0, dot);
        string parent = src.parent != null ? src.parent.name : "";
        bool parentRight = SideOf(parent) > 0;
        bool parentLeft = SideOf(parent) < 0;
        bool baseRight = SideOf(baseName) > 0;
        bool baseLeft = SideOf(baseName) < 0;
        string candidate = baseName;
        if (parentRight && baseLeft)
            candidate = SwapSide(baseName, toRight: true);
        else if (parentLeft && baseRight)
            candidate = SwapSide(baseName, toRight: false);
        return TryFindBone(skeleton, candidate, out donor);
    }

    static int SideOf(string name)
    {
        if (string.IsNullOrEmpty(name))
            return 0;
        if (name.IndexOf("_R_", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf(" R ", StringComparison.OrdinalIgnoreCase) >= 0)
            return 1;
        if (name.IndexOf("_L_", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf(" L ", StringComparison.OrdinalIgnoreCase) >= 0)
            return -1;
        return 0;
    }

    static string SwapSide(string name, bool toRight)
    {
        if (toRight)
            return name.Replace("_L_", "_R_").Replace("_l_", "_r_").Replace(" L ", " R ");
        return name.Replace("_R_", "_L_").Replace("_r_", "_l_").Replace(" R ", " L ");
    }

    /// <summary>
    /// Parent a hair chain that shares no bone names with the body onto Bip01_head.
    /// FElf and FDarkElf back-hair are skinned to hair01, which is the head joint
    /// under another name. Human and male-elf back-hair are skinned to Bip01_head
    /// and never reach this. The chain root belongs on the head with a zero local
    /// offset; preserving its world pose leaves the braid in front of the face.
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
            if (bone == head || bone.IsChildOf(head))
                continue;

            // hair01 is authored on top of Bip01_head: same rest translation and
            // the same axes as the body head (FElf and FDarkElf). Human and male
            // elf hair is weighted to Bip01_head itself, so it never gets here.
            // Keeping the chain's world pose parents it to a head the wait clip
            // has already turned, which swings the braid in front of the face.
            bone.SetParent(head, false);
            bone.localPosition = Vector3.zero;
            bone.localRotation = Quaternion.identity;
            bone.localScale = Vector3.one;

            if (!attached)
            {
                renderer.rootBone = bone;
                attached = true;
            }
        }

        if (attached)
            renderer.updateWhenOffscreen = true;
    }

    static bool TryPlaceHairOnHeadBind(
        SkinnedMeshRenderer renderer,
        Transform bone,
        Transform head,
        SkinnedMeshRenderer skeletonMesh)
    {
        if (!TryBindWorld(renderer, bone, out Matrix4x4 hairWorld))
            return false;
        if (!TryBindWorld(skeletonMesh, head, out Matrix4x4 headWorld))
            return false;

        Matrix4x4 local = headWorld.inverse * hairWorld;
        if (!TryDecomposeHairLocal(local, out Vector3 pos, out Quaternion rot, out Vector3 scale))
            return false;

        bone.SetParent(head, false);
        bone.localPosition = pos;
        bone.localRotation = rot;
        bone.localScale = SanitizeHairScale(scale);
        return true;
    }

    static void PlaceHairOnHeadLive(Transform bone, Transform head)
    {
        Vector3 worldPos = bone.position;
        Quaternion worldRot = bone.rotation;
        Vector3 lossy = bone.lossyScale;
        bone.SetParent(head, false);
        bone.position = worldPos;
        bone.rotation = worldRot;
        Vector3 parentScale = head.lossyScale;
        bone.localScale = SanitizeHairScale(new Vector3(
            DivideScale(lossy.x, parentScale.x),
            DivideScale(lossy.y, parentScale.y),
            DivideScale(lossy.z, parentScale.z)));
    }

    static bool TryDecomposeHairLocal(Matrix4x4 local, out Vector3 pos, out Quaternion rot, out Vector3 scale)
    {
        pos = local.GetColumn(3);
        rot = Quaternion.identity;
        Vector3 x = local.GetColumn(0);
        Vector3 y = local.GetColumn(1);
        Vector3 z = local.GetColumn(2);
        scale = new Vector3(x.magnitude, y.magnitude, z.magnitude);
        if (pos.sqrMagnitude > 4f)
            return false;
        if (scale.x < 1e-5f || scale.y < 1e-5f || scale.z < 1e-5f)
            return false;

        x /= scale.x;
        y /= scale.y;
        z /= scale.z;
        if (Vector3.Dot(Vector3.Cross(x, y), z) < 0f)
        {
            scale.x = -scale.x;
            x = -x;
        }

        Matrix4x4 ortho = Matrix4x4.identity;
        ortho.SetColumn(0, new Vector4(x.x, x.y, x.z, 0f));
        ortho.SetColumn(1, new Vector4(y.x, y.y, y.z, 0f));
        ortho.SetColumn(2, new Vector4(z.x, z.y, z.z, 0f));
        rot = ortho.rotation;
        return !float.IsNaN(rot.x) && !float.IsNaN(rot.y) && !float.IsNaN(rot.z) && !float.IsNaN(rot.w);
    }

    static float DivideScale(float value, float parent)
    {
        return Mathf.Abs(parent) < 0.0001f ? value : value / parent;
    }

    static Vector3 SanitizeHairScale(Vector3 scale)
    {
        return new Vector3(SanitizeHairAxis(scale.x), SanitizeHairAxis(scale.y), SanitizeHairAxis(scale.z));
    }

    static float SanitizeHairAxis(float value)
    {
        float sign = value < 0f ? -1f : 1f;
        float magnitude = Mathf.Abs(value);
        if (magnitude < 0.25f || magnitude > 4f)
            magnitude = 1f;
        return sign * magnitude;
    }

    static bool RebindRigidToHead(
        SkinnedMeshRenderer renderer,
        Transform[] mapped,
        Transform head,
        SkinnedMeshRenderer skeletonMesh)
    {
        Mesh src = renderer.sharedMesh;
        if (src == null || !TryBoneBindLocal(skeletonMesh, head, out Matrix4x4 headBind))
            return false;

        Mesh mesh = src.name.EndsWith("_headbind", StringComparison.Ordinal) ? src : UnityEngine.Object.Instantiate(src);
        mesh.name = src.name.EndsWith("_headbind", StringComparison.Ordinal) ? src.name : src.name + "_headbind";
        Matrix4x4[] poses = mesh.bindposes;
        Matrix4x4 pose = headBind.inverse * HairToSkeleton(renderer, skeletonMesh);
        int n = poses.Length < mapped.Length ? poses.Length : mapped.Length;
        for (int i = 0; i < n; i++)
            poses[i] = pose;
        mesh.bindposes = poses;
        for (int i = 0; i < mapped.Length; i++)
            mapped[i] = head;
        renderer.sharedMesh = mesh;
        renderer.bones = mapped;
        renderer.rootBone = head;
        renderer.updateWhenOffscreen = true;
        return true;
    }

    static Matrix4x4 HairToSkeleton(SkinnedMeshRenderer hair, SkinnedMeshRenderer skeletonMesh)
    {
        if (hair == null || skeletonMesh == null)
            return Matrix4x4.identity;
        return skeletonMesh.transform.worldToLocalMatrix * hair.transform.localToWorldMatrix;
    }

    /// <summary>
    /// Copy each mapped body bone's bindpose onto the hair mesh. Keeps neck and
    /// head weights, drops the hair file's own bone axes.
    /// </summary>
    static bool RetargetBindposes(
        SkinnedMeshRenderer renderer,
        Transform[] mapped,
        SkinnedMeshRenderer skeletonMesh)
    {
        Mesh src = renderer.sharedMesh;
        if (src == null || skeletonMesh == null || mapped == null)
            return false;
        Matrix4x4[] poses = src.bindposes;
        if (poses == null || poses.Length == 0)
            return false;

        Matrix4x4 delta = HairToSkeleton(renderer, skeletonMesh);
        var next = new Matrix4x4[poses.Length];
        for (int i = 0; i < poses.Length; i++)
            next[i] = poses[i];
        int n = poses.Length < mapped.Length ? poses.Length : mapped.Length;
        int replaced = 0;
        for (int i = 0; i < n; i++)
        {
            if (mapped[i] == null)
                continue;
            if (!TryBoneBindLocal(skeletonMesh, mapped[i], out Matrix4x4 boneBind))
                continue;
            next[i] = boneBind.inverse * delta;
            replaced++;
        }
        if (replaced == 0)
            return false;

        bool tagged = src.name.EndsWith("_retarget", StringComparison.Ordinal);
        Mesh mesh = tagged ? src : UnityEngine.Object.Instantiate(src);
        mesh.name = tagged ? src.name : src.name + "_retarget";
        mesh.bindposes = next;
        Transform srcRoot = renderer.rootBone;
        Transform[] srcBones = renderer.bones;
        Transform newRoot = null;
        if (srcRoot != null && srcBones != null)
        {
            int bones = srcBones.Length < mapped.Length ? srcBones.Length : mapped.Length;
            for (int i = 0; i < bones; i++)
            {
                if (srcBones[i] != srcRoot)
                    continue;
                newRoot = mapped[i];
                break;
            }
        }
        renderer.sharedMesh = mesh;
        renderer.bones = mapped;
        if (newRoot != null)
            renderer.rootBone = newRoot;
        renderer.updateWhenOffscreen = true;
        return true;
    }

    static bool RetargetToRest(
        SkinnedMeshRenderer renderer,
        Transform[] srcBones,
        Transform[] mapped,
        Dictionary<Transform, Matrix4x4> restLocal,
        Transform animRoot)
    {
        Mesh src = renderer != null ? renderer.sharedMesh : null;
        if (src == null || srcBones == null || mapped == null || restLocal == null || restLocal.Count == 0 || animRoot == null)
            return false;

        bool already = src.name.EndsWith("_rest", StringComparison.Ordinal);
        if (!already)
        {
            Matrix4x4[] poses = src.bindposes;
            if (poses == null || poses.Length == 0)
                return false;

            var next = new Matrix4x4[poses.Length];
            for (int i = 0; i < poses.Length; i++)
                next[i] = poses[i];

            int n = poses.Length < srcBones.Length ? poses.Length : srcBones.Length;
            if (mapped.Length < n)
                n = mapped.Length;
            Matrix4x4 rootNow = animRoot.localToWorldMatrix;
            int replaced = 0;
            for (int i = 0; i < n; i++)
            {
                Transform srcBone = srcBones[i];
                Transform dst = mapped[i];
                if (srcBone == null || dst == null)
                    continue;
                if (!restLocal.TryGetValue(dst, out Matrix4x4 localBind))
                    continue;
                Matrix4x4 rest = rootNow * localBind;
                next[i] = rest.inverse * srcBone.localToWorldMatrix * poses[i];
                replaced++;
            }
            if (replaced == 0)
                return false;

            Mesh mesh = UnityEngine.Object.Instantiate(src);
            mesh.name = src.name + "_rest";
            mesh.bindposes = next;
            renderer.sharedMesh = mesh;
        }

        renderer.bones = mapped;
        // Skin in the mesh's own space. A skull (or any swapped bone) as rootBone
        // adds that bone's translation on top of the bind and the face floats.
        renderer.rootBone = null;
        renderer.updateWhenOffscreen = true;
        return true;
    }

    static bool IsSkeletonRootName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        return name.Equals("Bip01", StringComparison.OrdinalIgnoreCase)
            || name.Equals("bip01", StringComparison.OrdinalIgnoreCase)
            || name.Equals("Bip01_Pelvis", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Bone that actually carries the vertices. The skin lists every cluster,
    /// including zero-weight ones, so the bone array alone cannot tell orc
    /// (weight on Bip01) from a human (weight on Bip01_head).
    /// </summary>
    static int DominantBone(Mesh mesh)
    {
        if (mesh == null || mesh.bindposes == null)
            return -1;
        BoneWeight[] weights = mesh.boneWeights;
        if (weights == null || weights.Length == 0)
            return -1;
        int n = mesh.bindposes.Length;
        var sum = new float[n];
        for (int i = 0; i < weights.Length; i++)
        {
            BoneWeight w = weights[i];
            if (w.boneIndex0 >= 0 && w.boneIndex0 < n) sum[w.boneIndex0] += w.weight0;
            if (w.boneIndex1 >= 0 && w.boneIndex1 < n) sum[w.boneIndex1] += w.weight1;
            if (w.boneIndex2 >= 0 && w.boneIndex2 < n) sum[w.boneIndex2] += w.weight2;
            if (w.boneIndex3 >= 0 && w.boneIndex3 < n) sum[w.boneIndex3] += w.weight3;
        }
        int best = -1;
        float bestW = 0f;
        for (int i = 0; i < n; i++)
        {
            if (sum[i] > bestW)
            {
                bestW = sum[i];
                best = i;
            }
        }
        return best;
    }

    static bool TryDriveRootWeightedFaceFromHead(
        SkinnedMeshRenderer renderer,
        Transform[] srcBones,
        Transform[] mapped,
        Transform bodyHead)
    {
        Mesh src = renderer != null ? renderer.sharedMesh : null;
        if (src == null || srcBones == null || mapped == null || bodyHead == null)
            return false;
        int dom = DominantBone(src);
        if (dom < 0 || dom >= srcBones.Length || srcBones[dom] == null)
            return false;
        if (!IsSkeletonRootName(srcBones[dom].name))
            return false;

        Transform faceHead = null;
        for (int i = 0; i < srcBones.Length; i++)
        {
            if (IsSkullBone(srcBones[i]))
            {
                faceHead = srcBones[i];
                break;
            }
        }
        if (faceHead == null)
            return false;

        bool tagged = src.name.EndsWith("_headbind", StringComparison.Ordinal);
        if (!tagged)
        {
            Mesh mesh = UnityEngine.Object.Instantiate(src);
            mesh.name = src.name + "_headbind";
            Matrix4x4[] poses = mesh.bindposes;
            if (poses == null || dom >= poses.Length)
                return false;
            poses[dom] = faceHead.worldToLocalMatrix * srcBones[dom].localToWorldMatrix * poses[dom];
            mesh.bindposes = poses;
            renderer.sharedMesh = mesh;
        }

        mapped[dom] = bodyHead;
        renderer.bones = mapped;
        renderer.rootBone = bodyHead;
        renderer.updateWhenOffscreen = true;
        return true;
    }

    static bool IsSkullBone(Transform bone)
    {
        if (bone == null || string.IsNullOrEmpty(bone.name))
            return false;
        string n = bone.name;
        return n.Equals("Bip01_head", StringComparison.OrdinalIgnoreCase)
            || n.Equals("Bip01 Head", StringComparison.OrdinalIgnoreCase)
            || n.Equals("Bip01_Head", StringComparison.OrdinalIgnoreCase);
    }

    static bool TryBoneBindLocal(SkinnedMeshRenderer smr, Transform bone, out Matrix4x4 local)
    {
        local = Matrix4x4.identity;
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
            local = poses[i].inverse;
            return true;
        }
        return false;
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
