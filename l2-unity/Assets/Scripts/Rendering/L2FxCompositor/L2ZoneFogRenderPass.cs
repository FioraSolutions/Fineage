using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Applies the zone distance fog to the camera color after the world is drawn.
/// URP Lit does not read _L2Fog*, and calling L2_LinearFog from each world shader
/// leaves those Lit materials sharp. This pass is the only fog path.
/// </summary>
public sealed class L2ZoneFogRenderPass : ScriptableRenderPass
{
    static readonly int SourceId = Shader.PropertyToID("_L2ZoneFogSource");
    static readonly int ZBufferId = Shader.PropertyToID("_L2ZBufferParams");
    static readonly int ReversedId = Shader.PropertyToID("_L2ReversedZ");
    static readonly int FarId = Shader.PropertyToID("_L2FogFarClip");
    static readonly ProfilingSampler Sampler = new ProfilingSampler("L2 Zone Fog");

    readonly Material _material;
    RTHandle _copy;

    sealed class PassData
    {
        public TextureHandle color;
        public TextureHandle depth;
        public Vector4 zBuffer;
        public float reversed;
        public float farClip;
        public L2ZoneFogRenderPass pass;
    }

    public L2ZoneFogRenderPass(Material material)
    {
        _material = material;
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        ConfigureInput(ScriptableRenderPassInput.Depth);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_material == null)
            return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        Camera camera = cameraData.camera;
        if (camera == null)
            return;
        CameraType type = camera.cameraType;
        if (type == CameraType.Preview || type == CameraType.Reflection)
            return;

        UniversalResourceData resources = frameData.Get<UniversalResourceData>();
        TextureHandle color = resources.activeColorTexture;
        if (!color.IsValid())
            return;

        TextureHandle depth = resources.cameraDepthTexture;
        if (!depth.IsValid())
            depth = resources.activeDepthTexture;
        if (!depth.IsValid())
            return;

        RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
        desc.depthBufferBits = 0;
        desc.msaaSamples = 1;
        RenderingUtils.ReAllocateIfNeeded(
            ref _copy,
            desc,
            FilterMode.Bilinear,
            TextureWrapMode.Clamp,
            name: "_L2ZoneFogCopy");
        if (_copy == null)
            return;

        using (IUnsafeRenderGraphBuilder builder =
               renderGraph.AddUnsafePass<PassData>("L2 Zone Fog", out PassData passData, Sampler))
        {
            float near = camera.nearClipPlane;
            float far = camera.farClipPlane;
            if (near < 1e-4f)
                near = 0.3f;
            if (far <= near + 1e-3f)
                far = near + 1000f;
            bool reversedZ = SystemInfo.usesReversedZBuffer;
            passData.color = color;
            passData.depth = depth;
            passData.zBuffer = L2ZoneFogMath.ZBufferParams(near, far, reversedZ);
            passData.reversed = reversedZ ? 1f : 0f;
            passData.farClip = far;
            passData.pass = this;
            builder.UseTexture(color, AccessFlags.ReadWrite);
            builder.UseTexture(depth, AccessFlags.Read);
            builder.AllowPassCulling(false);
            builder.AllowGlobalStateModification(true);
            builder.SetRenderFunc(static (PassData data, UnsafeGraphContext context) =>
            {
                CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                data.pass.Apply(cmd, data.color, data.depth, data.zBuffer, data.reversed, data.farClip);
            });
        }
    }

    void Apply(CommandBuffer cmd, TextureHandle color, TextureHandle depth, Vector4 zBuffer, float reversed, float farClip)
    {
        RTHandle colorRt = (RTHandle)color;
        RTHandle depthRt = (RTHandle)depth;
        if (colorRt == null || _copy == null || _material == null)
            return;

        cmd.SetGlobalTexture(SourceId, colorRt.nameID);
        Draw(_copy, 0);
        cmd.SetGlobalTexture(SourceId, _copy.nameID);
        if (depthRt != null)
            cmd.SetGlobalTexture("_CameraDepthTexture", depthRt.nameID);
        // Unity's per-camera _ZBufferParams is not bound for this procedural draw.
        // Left at 0, LinearEyeDepth is infinite and the pass fogs every pixel.
        cmd.SetGlobalVector(ZBufferId, zBuffer);
        cmd.SetGlobalFloat(ReversedId, reversed);
        cmd.SetGlobalFloat(FarId, farClip);
        Draw(colorRt, 1);

        void Draw(RTHandle destination, int pass)
        {
            RenderTexture destRt = destination.rt;
            int w = destRt != null ? destRt.width : 1;
            int h = destRt != null ? destRt.height : 1;
            cmd.SetRenderTarget(destination.nameID);
            cmd.SetViewport(new Rect(0f, 0f, w, h));
            cmd.DrawProcedural(Matrix4x4.identity, _material, pass, MeshTopology.Triangles, 3, 1);
        }
    }
}
