using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class ParticleGroupGpuDrawer
{
    void EnsureBuffer(int count)
    {
        if (_slotsBuffer != null && _slotsBuffer.count >= count)
            return;

        _slotsBuffer?.Release();
        int size = Mathf.NextPowerOfTwo(Mathf.Max(count, 8));
        _slotsBuffer = new ComputeBuffer(size, SlotStride);
    }

    public void Release()
    {
        _slotsBuffer?.Release();
        _slotsBuffer = null;
        _properties = null;
    }

    static Bounds TransformBounds(Bounds local, Matrix4x4 matrix)
    {
        Vector3 center = matrix.MultiplyPoint3x4(local.center);
        Vector3 ext = local.extents;
        Vector3 axisX = matrix.MultiplyVector(new Vector3(ext.x, 0f, 0f));
        Vector3 axisY = matrix.MultiplyVector(new Vector3(0f, ext.y, 0f));
        Vector3 axisZ = matrix.MultiplyVector(new Vector3(0f, 0f, ext.z));
        Vector3 worldExt = new Vector3(
            Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x),
            Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y),
            Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z));
        return new Bounds(center, worldExt * 2f);
    }
}
