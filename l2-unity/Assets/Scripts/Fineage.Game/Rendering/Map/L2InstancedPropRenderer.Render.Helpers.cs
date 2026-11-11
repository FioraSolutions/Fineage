using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2InstancedPropRenderer : MonoBehaviour
{
    private void BuildCells()
    {
        _cells = new List<Cell>();
        if (_propSet == null || _propSet.batches == null || _cellSize <= 0f)
        {
            return;
        }

        float invCell = 1f / _cellSize;
        var buckets = new Dictionary<CellKey, List<Matrix4x4>>();
        var meta = new Dictionary<CellKey, BatchMeta>();

        for (int b = 0; b < _propSet.batches.Length; b++)
        {
            L2InstancedPropBatch batch = _propSet.batches[b];
            if (batch == null || batch.mesh == null || batch.material == null || batch.matrices == null)
            {
                continue;
            }

            if (!batch.material.enableInstancing)
            {
                batch.material.enableInstancing = true;
            }

            float meshRadius = batch.mesh.bounds.extents.magnitude;
            for (int i = 0; i < batch.matrices.Length; i++)
            {
                Vector3 pos = batch.matrices[i].GetColumn(3);
                int cx = Mathf.FloorToInt(pos.x * invCell);
                int cz = Mathf.FloorToInt(pos.z * invCell);
                var key = new CellKey(b, cx, cz);
                if (!buckets.TryGetValue(key, out List<Matrix4x4> list))
                {
                    list = new List<Matrix4x4>(64);
                    buckets.Add(key, list);
                    meta.Add(key, new BatchMeta
                    {
                        mesh = batch.mesh,
                        material = batch.material,
                        shadows = batch.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off,
                        meshRadius = meshRadius
                    });
                }

                list.Add(batch.matrices[i]);
            }
        }

        foreach (var pair in buckets)
        {
            CellKey key = pair.Key;
            List<Matrix4x4> matrices = pair.Value;
            BatchMeta info = meta[key];
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < matrices.Count; i++)
            {
                Vector3 p = matrices[i].GetColumn(3);
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }

            Vector3 center = (min + max) * 0.5f;
            float radius = Vector3.Distance(min, max) * 0.5f + info.meshRadius;
            _cells.Add(new Cell
            {
                mesh = info.mesh,
                material = info.material,
                shadows = info.shadows,
                center = center,
                radius = radius,
                matrices = matrices.ToArray()
            });
        }
    }
}
