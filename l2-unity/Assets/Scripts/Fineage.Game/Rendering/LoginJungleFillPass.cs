using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Draws Jungle03 as a fullscreen far-plane fill before the login camera opaques.
/// Keeps mist/canopy in every pixel the 3D props don't cover (retail Camera25 look).
/// </summary>
[DisallowMultipleComponent]
public sealed class LoginJungleFillPass : MonoBehaviour
{
    Material _material;
    Camera _camera;
    bool _hooked;

    public void Bind(Material material)
    {
        _material = material;
        _camera = GetComponent<Camera>();
        Hook();
    }

    void OnEnable()
    {
        Hook();
    }

    void OnDisable()
    {
        Unhook();
    }

    void OnDestroy()
    {
        Unhook();
    }

    void Hook()
    {
        if (_hooked)
            return;
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
        _hooked = true;
    }

    void Unhook()
    {
        if (!_hooked)
            return;
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
        _hooked = false;
    }

    void OnBeginCamera(ScriptableRenderContext context, Camera camera)
    {
        if (_material == null || _camera == null || camera != _camera)
            return;
        if (!camera.enabled || camera.cameraType != CameraType.Game && camera.cameraType != CameraType.VR)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("LoginJungleFill");
        cmd.DrawProcedural(Matrix4x4.identity, _material, 0, MeshTopology.Triangles, 3, 1);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
