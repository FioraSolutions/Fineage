using System;
using UnityEngine;

public static partial class LocomotionGait
{
    internal static void FindBones(Transform root, out Transform pelvis, out Transform leftFoot, out Transform rightFoot)
    {
        pelvis = null;
        leftFoot = null;
        rightFoot = null;
        if (root == null)
            return;

        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        int pelvisScore = -1;
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t == null)
                continue;
            string n = t.name;
            if (string.IsNullOrEmpty(n))
                continue;

            int score = PelvisScore(n);
            if (score > pelvisScore)
            {
                pelvisScore = score;
                pelvis = t;
            }

            if (!IsFootName(n))
                continue;
            int side = FootSide(n);
            if (side < 0 && leftFoot == null)
                leftFoot = t;
            else if (side > 0 && rightFoot == null)
                rightFoot = t;
            else if (side == 0)
            {
                if (leftFoot == null)
                    leftFoot = t;
                else if (rightFoot == null && t != leftFoot)
                    rightFoot = t;
            }
        }
    }

    static int PelvisScore(string name)
    {
        // Exact L2 "Bip01" / common pelvis names. Do not match "Bip01 Spine" or "hip twist".
        if (name.Equals("Bip01", StringComparison.OrdinalIgnoreCase)
            || name.Equals("pelvis", StringComparison.OrdinalIgnoreCase)
            || name.Equals("hips", StringComparison.OrdinalIgnoreCase))
            return 3;
        if (name.Equals("hip", StringComparison.OrdinalIgnoreCase))
            return 2;
        if (name.IndexOf("pelvis", StringComparison.OrdinalIgnoreCase) >= 0)
            return 1;
        return -1;
    }

    static bool IsFootName(string name)
    {
        if (name.IndexOf("toe", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("hand", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("finger", StringComparison.OrdinalIgnoreCase) >= 0)
            return false;
        return name.IndexOf("foot", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("ankle", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static int FootSide(string name)
    {
        string n = name.ToLowerInvariant();
        if (n.Contains("left") || n.Contains(" l ") || n.Contains(" l.") || n.Contains(" l_")
            || n.StartsWith("l ") || n.Contains(".l") || n.Contains("_l") || n.EndsWith(" l")
            || n.Contains("bip01 l"))
            return -1;
        if (n.Contains("right") || n.Contains(" r ") || n.Contains(" r.") || n.Contains(" r_")
            || n.StartsWith("r ") || n.Contains(".r") || n.Contains("_r") || n.EndsWith(" r")
            || n.Contains("bip01 r"))
            return 1;
        return 0;
    }
}
