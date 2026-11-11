using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ModelTable;
using static Unity.Burst.Intrinsics.Arm;
using static UnityEngine.Rendering.DebugUI;
using System;

public partial class Gear
{
    private Transform[] RefreshBone(Transform[] allBone)
    {
        allBone[0] = GetShieldBone();
        allBone[1] = GetLeftHandBone();
        allBone[2] = GetLeftHandBone();
        allBone[3] = GetRightHandBone();
        return allBone;
    }

    protected virtual Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = transform.FindRecursive("Bow Bone");
        }
        return _leftHandBone;
    }

    public Transform GetSpineBone()
    {
        if (_spineBone == null)
        {
            _spineBone = transform.FindRecursive("Bip01_Spine1");
        }
        return _spineBone;
    }

    public Transform GetRightUpperArm()
    {
        if (_rightUpperArm == null)
        {
            _rightUpperArm = transform.FindRecursive("Bip01_R_UpperArm");
        }
        return _rightUpperArm;
    }
}
