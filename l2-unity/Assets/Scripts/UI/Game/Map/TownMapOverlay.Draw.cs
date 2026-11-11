using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class TownMapOverlay
{
    static Vector2 WorldToMapPixel(float l2X, float l2Y)
    {
        float nx = Mathf.InverseLerp(MapPanner.MapL2MinX, MapPanner.MapL2MaxX, l2X);
        float ny = Mathf.InverseLerp(MapPanner.MapL2MinY, MapPanner.MapL2MaxY, l2Y);
        return new Vector2(nx * MapPanner.MapWidthPixels, ny * MapPanner.MapHeightPixels);
    }

    public void Open(Town town)
    {
        if (town == null || _layer == null)
        {
            return;
        }

        _openTown = town;

        if (_layerImage != null)
        {
            _layerImage.style.backgroundImage =
                new StyleBackground(Resources.Load<Texture2D>(town.texturePath));
        }

        if (_layerTitle != null)
        {
            _layerTitle.text = town.title;
        }

        _layer.style.display = DisplayStyle.Flex;
        UpdateMarker();
    }

    public void Close()
    {
        _openTown = null;
        if (_layer != null)
        {
            _layer.style.display = DisplayStyle.None;
        }
    }

    public void UpdateMarker()
    {
        if (_openTown == null || _layerImage == null || _layerMarker == null)
        {
            return;
        }

        Vector3 worldPos = ControllerPacketServices.Player != null
            ? ControllerPacketServices.Player.GetPlayerPosition()
            : Vector3.zero;

        float l2X = worldPos.z * 52.5f;
        float l2Y = worldPos.x * 52.5f;

        float w = _layerImage.layout.width;
        float h = _layerImage.layout.height;
        if (w <= 0f || h <= 0f)
        {
            return;
        }

        float nx = Mathf.Clamp01(Mathf.InverseLerp(_openTown.MinX, _openTown.MaxX, l2X));
        float ny = Mathf.Clamp01(Mathf.InverseLerp(_openTown.MinY, _openTown.MaxY, l2Y));

        _layerMarker.style.left = nx * w - _layerMarker.layout.width * 0.5f;
        _layerMarker.style.top = ny * h - _layerMarker.layout.height * 0.5f;
    }
}
