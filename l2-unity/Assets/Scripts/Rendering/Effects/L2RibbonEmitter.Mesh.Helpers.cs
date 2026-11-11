using System.Collections.Generic;
using UnityEngine;

public partial class L2RibbonEmitter
{
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
}
