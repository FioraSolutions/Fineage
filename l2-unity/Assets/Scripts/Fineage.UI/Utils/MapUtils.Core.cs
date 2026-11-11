using UnityEngine;
using UnityEngine.UIElements;

public static partial class MapUtils
{
    public static Vector2 WorldToMap(Vector3 worldPos, Vector3 cornerA, Vector3 cornerB, Vector2 mapSize, bool useXZ = false, bool invertYForUI = true)
    {
        // определяем min/max по осям
        float minX = Mathf.Min(cornerA.x, cornerB.x);
        float maxX = Mathf.Max(cornerA.x, cornerB.x);

        float minY = useXZ ? Mathf.Min(cornerA.z, cornerB.z) : Mathf.Min(cornerA.y, cornerB.y);
        float maxY = useXZ ? Mathf.Max(cornerA.z, cornerB.z) : Mathf.Max(cornerA.y, cornerB.y);

        float worldWidth = maxX - minX;
        float worldHeight = maxY - minY;

        const float EPS = 1e-6f;
        if (Mathf.Abs(worldWidth) < EPS) worldWidth = EPS;
        if (Mathf.Abs(worldHeight) < EPS) worldHeight = EPS;

        float wx = worldPos.x;
        float wy = useXZ ? worldPos.z : worldPos.y;

        // нормализация в [0,1]
        float nx = (wx - minX) / worldWidth;
        float ny = (wy - minY) / worldHeight;

        nx = Mathf.Clamp01(nx);
        ny = Mathf.Clamp01(ny);

        if (invertYForUI)
            ny = 1f - ny;

        // масштабируем на размеры UI
        float mapX = nx * mapSize.x;
        float mapY = ny * mapSize.y;

        return new Vector2(mapX, mapY);
    }
}
