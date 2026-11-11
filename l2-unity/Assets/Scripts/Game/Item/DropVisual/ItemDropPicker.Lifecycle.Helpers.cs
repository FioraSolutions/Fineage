using UnityEngine;

public sealed partial class ItemDropPicker
{
    bool TryGetGroundPoint(Ray ray, float distance, out Vector3 point)
    {
        point = default;
        int mask = GroundPickMask();
        if (mask == 0)
            return false;
        if (!Physics.Raycast(ray, out RaycastHit hit, distance, mask, QueryTriggerInteraction.Ignore))
            return false;
        point = hit.point;
        return true;
    }

    int GroundPickMask()
    {
        if (_groundPickMask >= 0)
            return _groundPickMask;

        _groundPickMask = 0;
        AddLayer(ref _groundPickMask, "Terrain");
        AddLayer(ref _groundPickMask, "StaticMesh");
        AddLayer(ref _groundPickMask, "Brush");
        AddLayer(ref _groundPickMask, "Default");
        AddLayer(ref _groundPickMask, "Unwalkable");
        AddLayer(ref _groundPickMask, "AllowWalk");
        AddLayer(ref _groundPickMask, "Obstacle");
        return _groundPickMask;
    }
}
