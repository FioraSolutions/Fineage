using UnityEngine;

public partial class L2RibbonEmitter
{
    private float ResolveScaleRatioUnity()
    {
        return _scaleRatio * Mathf.Max(0.0001f, _l2ToUnityScale);
    }

    private Transform ResolveSpace()
    {
        if (!_storeInOwnerLocal)
        {
            return null;
        }

        if (OwnerTarget != null)
        {
            return OwnerTarget;
        }

        if (FollowTarget != null)
        {
            return FollowTarget;
        }

        return transform;
    }

    private Vector3 ResolveSheetPivot(Transform space)
    {
        Vector3 pivotWorld;
        if (_weaponRoot != null)
        {
            pivotWorld = _weaponRoot.position;
        }
        else if (_points.Count > 0)
        {
            Vector3 midLocal = L2FxRibbonGetPoint.EdgeMid(_points[0].A2, _points[0].A3);
            return midLocal;
        }
        else
        {
            pivotWorld = transform.position;
        }

        return space != null ? space.InverseTransformPoint(pivotWorld) : pivotWorld;
    }
}
