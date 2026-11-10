

public partial class L2RibbonEmitter
{
    private void EnsureMesh()
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (_mesh == null)
        {
            _mesh = new Mesh { name = "L2RibbonStrip" };
            _mesh.MarkDynamic();
        }

        if (_meshFilter != null && _meshFilter.sharedMesh != _mesh)
        {
            _meshFilter.sharedMesh = _mesh;
        }
    }

    private Transform ResolveSearchRoot()
    {
        if (OwnerTarget != null)
        {
            return OwnerTarget;
        }

        if (FollowTarget != null)
        {
            return FollowTarget;
        }

        return null;
    }

    static Entity ResolveOwnerEntity(Transform searchRoot)
    {
        if (searchRoot == null)
        {
            return null;
        }

        return searchRoot.GetComponentInParent<Entity>();
    }

    private void ResolveBladeAnchors()
    {
        if (_swordTip != null && _swordBase != null && _weaponRoot != null)
        {
            return;
        }

        Transform searchRoot = ResolveSearchRoot();
        Transform weapon = _weaponRoot;
        Entity owner = ResolveOwnerEntity(searchRoot);

        if (weapon == null && owner != null)
        {
            weapon = owner.GetWeaponTransform();
        }

        Transform[] points = owner != null ? owner.GetSwordBasePoints() : null;
        if (points != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == null)
                {
                    continue;
                }

                if (_swordBase == null && points[i].name == _swordBaseName)
                {
                    _swordBase = points[i];
                }

                if (_swordTip == null && points[i].name == _swordTipName)
                {
                    _swordTip = points[i];
                }
            }
        }

        if (weapon == null && searchRoot != null)
        {
            Transform bone = searchRoot.FindRecursive(_autoBoneName);
            if (bone != null)
            {
                // weapon_* usually under Weapon_R_Bone
                for (int i = 0; i < bone.childCount; i++)
                {
                    Transform child = bone.GetChild(i);
                    if (child.name.StartsWith("weapon_"))
                    {
                        weapon = child;
                        break;
                    }
                }

                if (weapon == null)
                {
                    weapon = bone;
                }
            }
        }

        Transform tipRoot = weapon != null ? weapon : searchRoot;
        if (tipRoot != null)
        {
            if (_swordTip == null)
            {
                _swordTip = tipRoot.Find(_swordTipName) ?? tipRoot.FindRecursive(_swordTipName);
            }

            if (_swordBase == null)
            {
                _swordBase = tipRoot.Find(_swordBaseName) ?? tipRoot.FindRecursive(_swordBaseName);
            }
        }

        if (_sampleBone == null && !string.IsNullOrEmpty(_autoBoneName) && searchRoot != null)
        {
            _sampleBone = searchRoot.FindRecursive(_autoBoneName);
        }

        if (_sampleBone == null && weapon != null)
        {
            _sampleBone = weapon;
        }

        _weaponRoot = weapon != null ? weapon : tipRoot;

        float markerLen = (_swordTip != null && _swordBase != null)
            ? Vector3.Distance(_swordTip.position, _swordBase.position)
            : 0f;
        float meshLen = 0f;
        if (_weaponRoot != null &&
            L2FxRibbonGetPoint.TryGetMeshBladeEnds(_weaponRoot, out Vector3 meshTip, out Vector3 meshHilt))
        {
            meshLen = Vector3.Distance(meshTip, meshHilt);
        }

        Log(
            $"ResolveBlade search='{(searchRoot != null ? searchRoot.name : "null")}' " +
            $"weapon='{(weapon != null ? weapon.name : "null")}' " +
            $"tip='{(_swordTip != null ? FullPath(_swordTip) : "null")}' " +
            $"base='{(_swordBase != null ? FullPath(_swordBase) : "null")}' " +
            $"bone='{(_sampleBone != null ? _sampleBone.name : "null")}' " +
            $"markerLen={markerLen:F3} meshLen={meshLen:F3}");
    }

    private static string FullPath(Transform t)
    {
        if (t == null)
        {
            return "null";
        }

        string path = t.name;
        Transform p = t.parent;
        int guard = 0;
        while (p != null && guard++ < 8)
        {
            path = p.name + "/" + path;
            p = p.parent;
        }

        return path;
    }

    private void ApplyOpacityToMaterial()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        UpdateShaderFloat("_Opacity", _opacity);
    }

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
