using UnityEngine;
using UnityEngine.UIElements;

public partial class MapPanner 
{
    private VisualElement _viewport;
    private VisualElement _map;
    private VisualElement _marker;

    private Vector2 lastPointer;
    private Vector2 offset; 

    private bool dragging;

    public const float ViewWidthPixels = 328f;
    public const float ViewHeightPixels = 328f;
    public static float MapWidthPixels => InterludeWorldMapBounds.PixelWidth;
    public static float MapHeightPixels => InterludeWorldMapBounds.PixelHeight;

    const float VIEW_W = ViewWidthPixels;
    const float VIEW_H = ViewHeightPixels;
    float MAP_W => MapWidthPixels;
    float MAP_H => MapHeightPixels;
    private Button _buttonLocation;
    private bool _isDisabled = false;
    private bool _isManualUpdate = false;

    private Vector2 _mapPos = Vector2.zero;
    // Pixel size of the map image, used verbatim by PlaceMarkerOnMap.
    Vector2 MapPixelSize => new Vector2(MapWidthPixels, MapHeightPixels);



}
