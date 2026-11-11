using UnityEngine;
using UnityEngine.Diagnostics;

public partial class MonsterGear : Gear
{
    public Transform GetTailBone()
    {
        if (tailBone == null)
        {
            tailBone = FindRecursiveBone(BONE_TAIL);
        }
        return tailBone;
    }

    public Transform GetHeadBone()
    {
        if (headBone == null)
        {
            headBone = FindRecursiveBone(BONE_HEAD);
        }
        return headBone;
    }

    public Transform GetClavicledRBone()
    {
        if (clavicleRBone == null)
        {
            clavicleRBone = FindRecursiveBone(BONE_CLAVICLE_R_NAME);
        }
        return clavicleRBone;
    }
}
