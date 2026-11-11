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

    private const float VIEW_W = 294f;
    private const float VIEW_H = 300f;
    private const float MAP_W = 1812f;
    private const float MAP_H = 2620f;
    public const float MapWidthPixels = MAP_W;
    public const float MapHeightPixels = MAP_H;
    private Button _buttonLocation;
    private bool _isDisabled = false;
    private bool _isManualUpdate = false;

    private Vector2 _mapPos = Vector2.zero;
    // Pixel size of the map image, used verbatim by PlaceMarkerOnMap.
    Vector2 mapSizeD = new Vector2(MAP_W, MAP_H);

    // L2 world rectangle covered by the 1812x2620 world map art. Calibrated against the town icon
    // positions on the art using the real teleporter coordinates of Aden, Giran, Oren, Dion,
    // Goddard, Rune, Orc Village, Dwarven Village and Talking Island (least squares: residual
    // ~9 px on x, ~12 px on y). The art's x axis follows L2 x and its y axis follows L2 y.
    public const float MapL2MinX = -125577f;
    public const float MapL2MaxX = 196500f;
    public const float MapL2MinY = -264300f;
    public const float MapL2MaxY = 267788f;



}
