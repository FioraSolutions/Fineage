using UnityEngine;
using System;

public sealed partial class ItemDropWeaponAligner
{
    public static void StandUpright(GameObject visual)
    {
        if (visual == null)
            return;

        MeshFilter filter = visual.GetComponentInChildren<MeshFilter>();
        Mesh mesh = filter != null ? filter.sharedMesh : null;
        if (mesh == null)
            return;

        Bounds bounds = mesh.bounds;
        Vector3 size = bounds.size;
        int axis = 1;
        if (size.x >= size.y && size.x >= size.z)
            axis = 0;
        else if (size.z >= size.y && size.z >= size.x)
            axis = 2;

        Vector3 localUp = axis == 0 ? Vector3.right : axis == 2 ? Vector3.forward : Vector3.up;
        if (Mathf.Abs(bounds.max[axis]) < Mathf.Abs(bounds.min[axis]))
            localUp = -localUp;

        visual.transform.localRotation = Quaternion.FromToRotation(localUp, Vector3.up) *
                                         visual.transform.localRotation;
    }

    float DefaultWeaponWorldLength(int itemId)
    {
        Abstractgrp grp = _grp.ResolveGrp(itemId);
        int dropHeightUu = grp != null && grp.DropHeight > 0 ? grp.DropHeight : 12;
        float fromHeight = dropHeightUu * VectorUtils.L2UuToUnity * 4f * 0.8f;
        return Mathf.Clamp(fromHeight, 0.44f, 1.0f);
    }

    static float TryEquippedWeaponWorldLength(int itemId, out string source)
    {
        source = "none";
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null)
            return 0f;

        Gear gear = player.GetComponentInChildren<Gear>(true);
        if (gear == null)
            return 0f;

        Transform bone = gear.GetTransformRightHandBone();
        if (bone == null)
            return 0f;

        string idToken = itemId.ToString();
        for (int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);
            if (child.name.IndexOf(idToken, System.StringComparison.Ordinal) < 0)
                continue;
            float len = LongestMeshAxis(child.gameObject);
            if (len > 0.05f)
            {
                source = $"equippedChild:'{child.name}'";
                return len;
            }
        }

        source = "notEquipped";
        return 0f;
    }
}
