using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class ParticleGroupGpuDrawer
{
    public static bool SupportsGpuInstancing(Material material)
    {
        return material != null &&
               material.GetTag(ShaderTag, false, string.Empty) == ShaderTagOn;
    }

    public void Draw(
        Mesh mesh,
        Material[] materials,
        int layer,
        int rendererPriority,
        NativeArray<L2FxParticleInstance> packedSlots,
        NativeArray<Matrix4x4> matrices,
        int packed)
    {
        if (mesh == null || materials == null || packed <= 0 ||
            !packedSlots.IsCreated || !matrices.IsCreated)
            return;

        EnsureBuffer(packed);
        _slotsBuffer.SetData(packedSlots, 0, 0, packed);
        _properties ??= new MaterialPropertyBlock();
        _properties.Clear();
        _properties.SetBuffer(SlotsBufferId, _slotsBuffer);

        Bounds worldBounds = TransformBounds(mesh.bounds, matrices[0]);
        for (int i = 1; i < packed; i++)
            worldBounds.Encapsulate(TransformBounds(mesh.bounds, matrices[i]));
        worldBounds.Expand(8f);

        for (int submesh = 0; submesh < materials.Length; submesh++)
        {
            Material material = materials[submesh];
            if (material == null)
                continue;

            _properties ??= new MaterialPropertyBlock();
            _properties.Clear();
            _properties.SetBuffer(SlotsBufferId, _slotsBuffer);

            if (L2FxCompositorRuntime.PreferGpuQueue)
            {
                // Matrices must be copied — NativeArray may be reused next frame.
                var matrixCopy = new Matrix4x4[packed];
                for (int i = 0; i < packed; i++)
                    matrixCopy[i] = matrices[i];

                var propsCopy = new MaterialPropertyBlock();
                propsCopy.SetBuffer(SlotsBufferId, _slotsBuffer);

                L2FxGpuDrawQueue.Enqueue(new L2FxGpuDrawQueue.DrawItem
                {
                    Mesh = mesh,
                    Material = material,
                    Submesh = submesh,
                    Layer = layer,
                    RendererPriority = rendererPriority,
                    WorldBounds = worldBounds,
                    SlotsBuffer = _slotsBuffer,
                    Matrices = matrixCopy,
                    InstanceCount = packed,
                    Properties = propsCopy
                });
                continue;
            }

            var rp = new RenderParams(material)
            {
                worldBounds = worldBounds,
                layer = layer,
                rendererPriority = rendererPriority,
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = false,
                matProps = _properties
            };
            Graphics.RenderMeshInstanced(rp, mesh, submesh, matrices, packed);
        }
    }
}
