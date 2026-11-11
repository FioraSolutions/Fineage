using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class ParticleGroupGpuDrawer
{
    public const string ShaderTag = "L2FxGpuInstancing";
    public const string ShaderTagOn = "On";

    private static readonly int SlotsBufferId = Shader.PropertyToID("_L2FxParticleSlots");
    private static readonly int SlotStride = Marshal.SizeOf<L2FxParticleInstance>();

    private ComputeBuffer _slotsBuffer;
    private MaterialPropertyBlock _properties;






}
