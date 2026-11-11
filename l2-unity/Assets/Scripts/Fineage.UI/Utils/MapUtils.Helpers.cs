using UnityEngine;
using UnityEngine.UIElements;

public static partial class MapUtils
{
    public static bool PlaceMarkerOnMap(ref Vector2 offset , VisualElement map, VisualElement marker, Vector2 mapPos, Vector2 mapSizeFromWorldToMap,
                     float markerPivotX = 0.5f, float markerPivotY = 0.5f, bool worldYIsBottom = false)
    {
        float uiMapW = map.layout.width;
        float uiMapH = map.layout.height;

        // защитные проверки
        if (uiMapW <= 0f || uiMapH <= 0f) return false;
        if (mapSizeFromWorldToMap.x <= 0f || mapSizeFromWorldToMap.y <= 0f) return false;

        float sx = uiMapW / mapSizeFromWorldToMap.x;
        float sy = uiMapH / mapSizeFromWorldToMap.y;

        float x = mapPos.x * sx;
        float y = mapPos.y * sy;

        if (worldYIsBottom)
        {
            // инвертируем Y, если world Y отсчитывается снизу
            y = uiMapH - y;
        }

        float mw = marker.layout.width;
        float mh = marker.layout.height;

        float left = x - mw * markerPivotX;
        float top = y - mh * markerPivotY;

        // --- проверка: если текущее положение уже равно целевому, ничего не делаем ---
        // используем небольшую толерантность для сравнения float
        const float eps = 0.5f; // подберите по необходимости (пиксели)
        float currentLeft = marker.layout.x;
        float currentTop = marker.layout.y;

        if (Mathf.Abs(currentLeft - left) <= eps && Mathf.Abs(currentTop - top) <= eps)
        {
            return false;
        }

        marker.style.left = left;
        marker.style.top = top;

        return true;
    }

    public static void PanViewportToMarker(ref Vector2 offset , VisualElement map, VisualElement viewport, VisualElement marker, float padding = 0f)
    {
        float mapW = map.layout.width;
        float mapH = map.layout.height;
        float viewW = viewport.layout.width;
        float viewH = viewport.layout.height;


        float markerCenterX = marker.layout.x + marker.layout.width * 0.5f;
        float markerCenterY = marker.layout.y + marker.layout.height * 0.5f;


        float desiredX = markerCenterX - viewW * 0.5f + Mathf.Sign(markerCenterX - viewW * 0.5f) * padding;
        float desiredY = markerCenterY - viewH * 0.5f + Mathf.Sign(markerCenterY - viewH * 0.5f) * padding;


        float maxOffsetX = Mathf.Max(0f, mapW - viewW);
        float maxOffsetY = Mathf.Max(0f, mapH - viewH);

        desiredX = Mathf.Clamp(desiredX, 0f, maxOffsetX);
        desiredY = Mathf.Clamp(desiredY, 0f, maxOffsetY);

        offset.x = desiredX;
        offset.y = desiredY;

        map.style.left = -desiredX;
        map.style.top = -desiredY;
    }
}
