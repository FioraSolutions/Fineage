using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2InstancedPropRenderer : MonoBehaviour
{
    private void DrawCell(Cell cell, Matrix4x4 world)
    {
        int remaining = cell.matrices.Length;
        int offset = 0;
        while (remaining > 0)
        {
            int count = remaining < DrawChunk ? remaining : DrawChunk;
            for (int i = 0; i < count; i++)
            {
                _drawBuffer[i] = world * cell.matrices[offset + i];
            }

            Graphics.DrawMeshInstanced(
                cell.mesh,
                0,
                cell.material,
                _drawBuffer,
                count,
                null,
                cell.shadows,
                _receiveShadows,
                _layer);

            offset += count;
            remaining -= count;
        }
    }
}
