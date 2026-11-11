using System;
using UnityEngine;

public partial class CameraController
{
    public void SetHeading(float heading)
    {
        _x = heading;
    }

    Vector3 LookAtPoint()
    {
        float boneOffset = 0f;
        if (_stickToBone && _rootBone != null)
        {
            boneOffset = _rootBone.position.y - _target.position.y - _rootBoneHeight;
        }

        float aboveHead = L2HeightOffsetMeters * WorldScale;
        CharacterController cc = FindCharacterController();
        if (cc != null && cc.height > 0.1f)
        {
            Vector3 localTop = cc.center + Vector3.up * (cc.height * 0.5f);
            return cc.transform.TransformPoint(localTop) + _camOffset + Vector3.up * (aboveHead + _lookHeightAdjust + boneOffset);
        }

        float lookHeight = _resolvedPlayerHeight + aboveHead + _lookHeightAdjust;
        return _target.position + _camOffset + Vector3.up * (lookHeight + boneOffset);
    }

    float ResolvePlayerHeight()
    {
        CharacterController cc = FindCharacterController();
        if (cc != null && cc.height > 0.1f)
        {
            return cc.height;
        }

        return _unityPlayerHeight;
    }
}
