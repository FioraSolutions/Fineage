

public partial class L2RibbonEmitter
{
    private void RebuildMesh()
    {
        EnsureMesh();
        int n = _points.Count;
        if (n < 2)
        {
            ClearMesh();
            return;
        }

        int vertCount = n * 2;
        int triCount = (n - 1) * 6;
        EnsureBuffers(vertCount, triCount);

        Transform space = ResolveSpace();
        Transform meshXf = transform;

        for (int i = 0; i < n; i++)
        {
            RibbonPoint p = _points[i];
            Vector3 a2World = space != null ? space.TransformPoint(p.A2) : p.A2;
            Vector3 a3World = space != null ? space.TransformPoint(p.A3) : p.A3;
            Vector3 a2Local = meshXf.InverseTransformPoint(a2World);
            Vector3 a3Local = meshXf.InverseTransformPoint(a3World);

            int v = i * 2;
            _verts[v] = a3Local;
            _verts[v + 1] = a2Local;

            float along = n <= 1 ? 0f : (float)i / (n - 1);
            _uvs[v] = new Vector2(0f, along);
            _uvs[v + 1] = new Vector2(1f, along);

            float alpha = _fadeAlphaAlongTrail
                ? Mathf.Lerp(_headAlpha, _tailAlpha, along)
                : 1f;
            Color c = new Color(1f, 1f, 1f, alpha);
            _colors[v] = c;
            _colors[v + 1] = c;
        }

        int t = 0;
        for (int i = 0; i < n - 1; i++)
        {
            int v = i * 2;
            _tris[t++] = v;
            _tris[t++] = v + 2;
            _tris[t++] = v + 1;

            _tris[t++] = v + 1;
            _tris[t++] = v + 2;
            _tris[t++] = v + 3;
        }

        _mesh.Clear(false);
        var vertList = new List<Vector3>(vertCount);
        var uvList = new List<Vector2>(vertCount);
        var colorList = new List<Color>(vertCount);
        var triList = new List<int>(triCount);
        for (int i = 0; i < vertCount; i++)
        {
            vertList.Add(_verts[i]);
            uvList.Add(_uvs[i]);
            colorList.Add(_colors[i]);
        }

        for (int i = 0; i < triCount; i++)
        {
            triList.Add(_tris[i]);
        }

        _mesh.SetVertices(vertList);
        _mesh.SetUVs(0, uvList);
        _mesh.SetColors(colorList);
        _mesh.SetTriangles(triList, 0, false);
        _mesh.RecalculateBounds();
        Bounds expanded = _mesh.bounds;
        expanded.Expand(1f);
        _mesh.bounds = expanded;

        if (_debugLog && _sampleAccepted <= 3)
        {
            Log(
                $"RebuildMesh verts={vertCount} tris={triCount / 3} " +
                $"local0={_verts[0]} local1={_verts[1]} bounds={_mesh.bounds}");
        }
    }

    private void EnsureBuffers(int vertCount, int triCount)
    {
        if (_verts == null || _verts.Length < vertCount)
        {
            _verts = new Vector3[vertCount];
            _uvs = new Vector2[vertCount];
            _colors = new Color[vertCount];
        }

        if (_tris == null || _tris.Length < triCount)
        {
            _tris = new int[triCount];
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_drawGizmos)
        {
            return;
        }

        if (_swordTip != null && _swordBase != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_swordTip.position, _swordBase.position);
            Gizmos.DrawSphere(_swordTip.position, 0.02f);
            Gizmos.DrawSphere(_swordBase.position, 0.02f);
        }

        if (_points == null || _points.Count == 0)
        {
            return;
        }

        Transform space = ResolveSpace();
        Gizmos.color = Color.cyan;
        for (int i = 0; i < _points.Count; i++)
        {
            Vector3 a2 = space != null ? space.TransformPoint(_points[i].A2) : _points[i].A2;
            Vector3 a3 = space != null ? space.TransformPoint(_points[i].A3) : _points[i].A3;
            Gizmos.DrawLine(a2, a3);
            if (i + 1 < _points.Count)
            {
                Vector3 a2n = space != null ? space.TransformPoint(_points[i + 1].A2) : _points[i + 1].A2;
                Vector3 a3n = space != null ? space.TransformPoint(_points[i + 1].A3) : _points[i + 1].A3;
                Gizmos.DrawLine(a2, a2n);
                Gizmos.DrawLine(a3, a3n);
            }
        }
    }
#endif
}
