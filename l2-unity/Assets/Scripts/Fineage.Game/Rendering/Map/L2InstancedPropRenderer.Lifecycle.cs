using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2InstancedPropRenderer : MonoBehaviour
{
    private void OnEnable()
    {
        _transform = transform;
        BuildCells();
    }

    private void LateUpdate()
    {
        if (_cells == null || _cells.Count == 0)
        {
            return;
        }

        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        float maxDistance = _maxDrawDistance > 0f ? _maxDrawDistance : camera.farClipPlane;
        Vector3 cameraPos = camera.transform.position;
        Matrix4x4 world = _transform.localToWorldMatrix;
        CullAndDraw(camera, world, maxDistance, cameraPos);
    }
}
