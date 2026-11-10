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

    /// <summary>
    /// Pivot for arc sheets — weapon root (hand) in the same space as stored a2/a3.
    /// Falls back to previous sample mid if weapon is missing.
    /// </summary>
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

    private Vector3 ResolveWidthAxisWorld(Transform bone)
    {
        switch (_widthAxisSource)
        {
            case WidthAxisSource.TransformUp:
                return bone.up;
            case WidthAxisSource.TransformForward:
                return bone.forward;
            case WidthAxisSource.CustomLocal:
                return bone.TransformDirection(_customWidthAxisLocal.sqrMagnitude > 1e-8f
                    ? _customWidthAxisLocal.normalized
                    : Vector3.right);
            default:
                return bone.right;
        }
    }

    private bool TryResolveBladeWorldEnds(out Vector3 tipWorld, out Vector3 hiltWorld, out string source)
    {
        tipWorld = default;
        hiltWorld = default;
        source = "none";

        bool haveMarkers = _swordTip != null && _swordBase != null;
        Vector3 markerTip = haveMarkers ? _swordTip.position : default;
        Vector3 markerHilt = haveMarkers ? _swordBase.position : default;
        float markerLenSq = haveMarkers ? (markerTip - markerHilt).sqrMagnitude : 0f;

        bool haveMesh = false;
        Vector3 meshTip = default;
        Vector3 meshHilt = default;
        Transform meshRoot = _weaponRoot;
        if (meshRoot == null && _swordTip != null)
        {
            meshRoot = _swordTip.parent != null ? _swordTip.parent : _swordTip;
        }

        if (_preferMeshBladeEnds && meshRoot != null &&
            L2FxRibbonGetPoint.TryGetMeshBladeEnds(meshRoot, out meshTip, out meshHilt))
        {
            haveMesh = true;
        }

        // Mesh bounds and Sword_Tip/Base often have opposite tip/hilt naming
        // (SwordSetup historically swapped GO names). Alternating sources flips
        // a2/a3 every frame (logs: dTip≈bladeLen, dMid≈0) and tears the ribbon.
        // Once mesh ends resolve, stick to them.
        if (haveMesh)
        {
            tipWorld = meshTip;
            hiltWorld = meshHilt;
            source = "MeshBounds";
            return true;
        }

        if (haveMarkers && markerLenSq > 1e-10f)
        {
            tipWorld = markerTip;
            hiltWorld = markerHilt;
            source = "SwordMarkers";
            return true;
        }

        return false;
    }

    private bool TryBuildEdges(out L2FxRibbonGetPoint.Edges edges)
    {
        edges = default;
        bool wantBlade = _sampleMode == SampleMode.AutoBladeEnds || _sampleMode == SampleMode.BladeEndsOnly;
        if (wantBlade && TryResolveBladeWorldEnds(out Vector3 tip, out Vector3 hilt, out string bladeSource))
        {
            edges = L2FxRibbonGetPoint.GetPointFromBladeEnds(tip, hilt, _edgeRatio);
            _lastSampleSource = bladeSource;
            _lastBladeResolveSource = bladeSource;
            _lastTipWorld = tip;
            _lastBaseWorld = hilt;
            return edges.A4 > 1e-5f;
        }

        if (_sampleMode == SampleMode.BladeEndsOnly)
        {
            _lastSampleSource = "BladeEndsMissing";
            return false;
        }

        Transform bone = _sampleBone;
        if (bone == null)
        {
            if (FollowTarget != null)
            {
                bone = FollowTarget;
            }
            else if (OwnerTarget != null)
            {
                bone = OwnerTarget;
            }
            else
            {
                bone = transform;
            }
        }

        edges = L2FxRibbonGetPoint.GetPointCoordSys1(
            bone.position,
            ResolveWidthAxisWorld(bone),
            ResolveScaleRatioUnity(),
            _edgeRatio);
        _lastSampleSource = "BoneNormal/" + bone.name;
        _lastBladeResolveSource = _lastSampleSource;
        _lastTipWorld = edges.A2;
        _lastBaseWorld = edges.A3;
        return true;
    }

    /// <summary>
    /// MeshBounds tip/hilt are fixed local AABB ends — do not re-stabilize (false tip↔hilt swaps on large arcs).
    /// Stabilize only for markers / bone fallback where naming can flip.
    /// </summary>
    private bool ShouldStabilizeEdgePolarity()
    {
        if (string.IsNullOrEmpty(_lastSampleSource))
        {
            return true;
        }

        return _lastSampleSource.IndexOf("MeshBounds", System.StringComparison.OrdinalIgnoreCase) < 0;
    }
}
