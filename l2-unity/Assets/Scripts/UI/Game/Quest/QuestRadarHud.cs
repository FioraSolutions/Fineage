using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude top-right CH3 radar (L2UI_CH3 Radar package). Heading-up world map inside the
/// static <c>radar_back</c> frame, fixed north marker, quest arrow/target, and zoom controls.
/// </summary>
public sealed class QuestRadarHud : MonoBehaviour
{
    const string BackPath = "Data/UI/Map/Radar/radar_back";
    const string MapPath = "Data/UI/Map/WorldMap";
    const string ArrowPath = "Data/UI/Map/Radar/radar_arrow";
    const string NorthPath = "Data/UI/Map/Radar/radar_n";
    const string SelfPath = "Data/UI/Map/Radar/radar_myself";
    const string TargetPath = "Data/UI/Map/Radar/radar_target";
    const string ZoomInPath = "Data/UI/Map/Radar/mapbutton_zoomin1";
    const string ZoomInOverPath = "Data/UI/Map/Radar/mapbutton_zoomin1_over";
    const string ZoomOutPath = "Data/UI/Map/Radar/mapbutton_zoomout1";
    const string ZoomOutOverPath = "Data/UI/Map/Radar/mapbutton_zoomout1_over";
    const float SizePx = 128f;
    const float MarginPx = 12f;
    const float NorthSizePx = 16f;
    const float SelfSizePx = 16f;
    const float TargetSizePx = 8f;
    const float RimRadiusPx = 54f;
    const float MapSizePx = 108f;
    const float ZoomBtnSize = 12f;

    VisualElement _host;
    VisualElement _mapRotator;
    VisualElement _frame;
    VisualElement _arrow;
    VisualElement _north;
    VisualElement _target;
    VisualElement _self;
    Button _zoomIn;
    Button _zoomOut;
    float _mapZoom = 1f;
    bool _assetsFailed;
    bool _ready;

    void OnDestroy()
    {
        if (_host != null && _host.parent != null)
            _host.RemoveFromHierarchy();
    }

    void LateUpdate()
    {
        if (!EnsureUi())
            return;

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null)
        {
            _host.style.display = DisplayStyle.None;
            return;
        }

        _host.style.display = DisplayStyle.Flex;

        Vector3 fwd = ResolveRadarForward(player);
        float faceAng = Mathf.Atan2(fwd.z, fwd.x);
        if (_mapRotator != null)
            _mapRotator.transform.rotation = Quaternion.Euler(0f, 0f, faceAng * Mathf.Rad2Deg);
        AnchorNorthFixed();
        SyncMapScroll(player);

        if (!QuestDirectionSight.TryActive(out QuestDirectionTarget target, out _, out Vector3 playerL2, out float planarUu))
        {
            _arrow.style.display = DisplayStyle.None;
            _target.style.display = DisplayStyle.None;
            return;
        }

        float toAng = Mathf.Atan2(target.X - playerL2.x, target.Y - playerL2.y);
        float rel = toAng - faceAng;

        _arrow.style.display = DisplayStyle.Flex;
        _arrow.transform.rotation = Quaternion.Euler(0f, 0f, -rel * Mathf.Rad2Deg);

        if (planarUu < QuestDirectionSight.RadarRadiusUu)
        {
            float r = (planarUu / QuestDirectionSight.RadarRadiusUu) * RimRadiusPx;
            float cx = (SizePx * 0.5f) + Mathf.Sin(rel) * r;
            float cy = (SizePx * 0.5f) - Mathf.Cos(rel) * r;
            _target.style.display = DisplayStyle.Flex;
            _target.style.left = cx - TargetSizePx * 0.5f;
            _target.style.top = cy - TargetSizePx * 0.5f;
        }
        else
        {
            _target.style.display = DisplayStyle.None;
        }
    }

    void SyncMapScroll(PlayerEntity player)
    {
        if (_mapRotator == null)
            return;
        VisualElement map = _mapRotator.Q<VisualElement>("QuestRadarMap");
        if (map == null)
            return;

        Vector3 pos = player.transform.position;
        var l2 = VectorUtils.ConvertPosUnityToL2j(pos);
        Vector2 mapPx = DetailedMapProjection.ProjectWorldMapPixel(l2.x, l2.y);
        float scaledW = DetailedMapProjection.WorldMapPixelWidth * _mapZoom;
        float scaledH = DetailedMapProjection.WorldMapPixelHeight * _mapZoom;
        map.style.width = scaledW;
        map.style.height = scaledH;
        float cx = SizePx * 0.5f;
        float left = cx - mapPx.x * _mapZoom;
        float top = cx - mapPx.y * _mapZoom;
        map.style.left = left;
        map.style.top = top;
    }

    void AnchorNorthFixed()
    {
        if (_north == null)
            return;
        float top = (SizePx - NorthSizePx) * 0.5f - RimRadiusPx;
        _north.style.left = (SizePx - NorthSizePx) * 0.5f;
        _north.style.top = Mathf.Max(0f, top);
        _north.style.unityBackgroundImageTintColor = Color.white;
    }

    bool EnsureUi()
    {
        if (_ready)
            return true;
        if (_assetsFailed)
            return false;

        L2GameUI ui = L2GameUI.Instance;
        if (ui == null)
            return false;
        UIDocument document = ui.GetComponent<UIDocument>();
        VisualElement root = document != null ? document.rootVisualElement : null;
        if (root == null)
            return false;

        Texture2D back = Load(BackPath);
        Texture2D map = Load(MapPath);
        Texture2D arrow = Load(ArrowPath);
        Texture2D north = Load(NorthPath);
        Texture2D self = Load(SelfPath);
        Texture2D target = Load(TargetPath);
        L2InterludeHudSkinPipeline.ValidateTextures(
            InterludeHudWindowCatalog.RadarWnd,
            nameof(QuestRadarHud));
        L2InterludeHudSkinPipeline.ValidateRadarZoomTextures(nameof(QuestRadarHud));
        if (back == null || map == null || arrow == null || north == null || self == null || target == null)
        {
            _assetsFailed = true;
            return false;
        }

        _host = new VisualElement { name = "QuestRadar" };
        _host.pickingMode = PickingMode.Ignore;
        _host.style.position = Position.Absolute;
        _host.style.right = MarginPx;
        _host.style.top = MarginPx;
        _host.style.width = SizePx;
        _host.style.height = SizePx + ZoomBtnSize;
        _host.style.display = DisplayStyle.None;
        root.Add(_host);

        var mapClip = new VisualElement { name = "QuestRadarMapClip" };
        mapClip.pickingMode = PickingMode.Ignore;
        mapClip.style.position = Position.Absolute;
        mapClip.style.left = (SizePx - MapSizePx) * 0.5f;
        mapClip.style.top = (SizePx - MapSizePx) * 0.5f;
        mapClip.style.width = MapSizePx;
        mapClip.style.height = MapSizePx;
        mapClip.style.overflow = Overflow.Hidden;
        mapClip.style.borderTopLeftRadius = mapClip.style.borderTopRightRadius =
            mapClip.style.borderBottomLeftRadius = mapClip.style.borderBottomRightRadius = Length.Percent(50);
        _host.Add(mapClip);

        _mapRotator = new VisualElement { name = "QuestRadarMapRotator" };
        _mapRotator.pickingMode = PickingMode.Ignore;
        _mapRotator.style.position = Position.Absolute;
        _mapRotator.style.left = 0;
        _mapRotator.style.top = 0;
        _mapRotator.style.width = MapSizePx;
        _mapRotator.style.height = MapSizePx;
        _mapRotator.style.transformOrigin = new StyleTransformOrigin(
            new TransformOrigin(Length.Percent(50), Length.Percent(50)));
        mapClip.Add(_mapRotator);

        var mapLayer = new VisualElement { name = "QuestRadarMap" };
        mapLayer.pickingMode = PickingMode.Ignore;
        mapLayer.style.position = Position.Absolute;
        mapLayer.style.backgroundImage = new StyleBackground(map);
        mapLayer.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        _mapRotator.Add(mapLayer);

        _frame = new VisualElement { name = "QuestRadarFrame" };
        _frame.pickingMode = PickingMode.Ignore;
        _frame.style.position = Position.Absolute;
        _frame.style.left = 0;
        _frame.style.top = 0;
        _frame.style.width = SizePx;
        _frame.style.height = SizePx;
        _frame.style.backgroundImage = new StyleBackground(back);
        _host.Add(_frame);

        _self = Image(self, SelfSizePx, (SizePx - SelfSizePx) * 0.5f, (SizePx - SelfSizePx) * 0.5f);
        _host.Add(_self);

        _arrow = Image(arrow, SizePx, 0f, 0f);
        _arrow.style.display = DisplayStyle.None;
        _host.Add(_arrow);

        _north = Image(north, NorthSizePx, 0f, 0f);
        _host.Add(_north);
        AnchorNorthFixed();

        _target = Image(target, TargetSizePx, 0f, 0f);
        _target.style.display = DisplayStyle.None;
        _host.Add(_target);

        _zoomIn = ZoomButton(ZoomInPath, ZoomInOverPath, SizePx - ZoomBtnSize * 2f, SizePx + 2f);
        _zoomOut = ZoomButton(ZoomOutPath, ZoomOutOverPath, SizePx - ZoomBtnSize, SizePx + 2f);
        _zoomIn.clicked += () => _mapZoom = Mathf.Clamp(_mapZoom * 1.15f, 0.6f, 2.2f);
        _zoomOut.clicked += () => _mapZoom = Mathf.Clamp(_mapZoom / 1.15f, 0.6f, 2.2f);
        _host.Add(_zoomIn);
        _host.Add(_zoomOut);

        _ready = true;
        return true;
    }

    static Button ZoomButton(string normal, string hover, float left, float top)
    {
        Texture2D tex = Resources.Load<Texture2D>(normal);
        var btn = new Button { pickingMode = PickingMode.Position };
        btn.style.position = Position.Absolute;
        btn.style.left = left;
        btn.style.top = top;
        btn.style.width = ZoomBtnSize;
        btn.style.height = ZoomBtnSize;
        btn.style.marginLeft = 0;
        btn.style.marginRight = 0;
        btn.style.marginTop = 0;
        btn.style.marginBottom = 0;
        btn.style.paddingLeft = 0;
        btn.style.paddingRight = 0;
        btn.style.paddingTop = 0;
        btn.style.paddingBottom = 0;
        btn.style.borderTopWidth = btn.style.borderBottomWidth =
            btn.style.borderLeftWidth = btn.style.borderRightWidth = 0;
        btn.style.backgroundColor = Color.clear;
        if (tex != null)
            btn.style.backgroundImage = new StyleBackground(tex);
        else
            btn.style.display = DisplayStyle.None;

        Texture2D over = Resources.Load<Texture2D>(hover);
        if (over != null)
        {
            btn.RegisterCallback<MouseEnterEvent>(_ =>
            {
                if (tex != null)
                    btn.style.backgroundImage = new StyleBackground(over);
            });
            btn.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                if (tex != null)
                    btn.style.backgroundImage = new StyleBackground(tex);
            });
        }

        return btn;
    }

    static VisualElement Image(Texture2D tex, float size, float left, float top)
    {
        var el = new VisualElement();
        el.pickingMode = PickingMode.Ignore;
        el.style.position = Position.Absolute;
        el.style.left = left;
        el.style.top = top;
        el.style.width = size;
        el.style.height = size;
        el.style.backgroundImage = new StyleBackground(tex);
        return el;
    }

    static Texture2D Load(string path)
    {
        var tex = Resources.Load<Texture2D>(path);
        if (tex == null)
            Debug.LogError($"[QuestRadar] missing texture path='{path}'. Import L2UI_CH3 Radar from Interlude client.");
        return tex;
    }

    static Vector3 ResolveRadarForward(PlayerEntity player)
    {
        CameraController camera = CameraPacketServices.Controller;
        Vector3 fwd = camera != null ? camera.transform.forward : player.transform.forward;
        fwd.y = 0f;
        if (fwd.sqrMagnitude < 1e-6f)
            fwd = player.transform.forward;
        fwd.y = 0f;
        return fwd.sqrMagnitude > 1e-6f ? fwd.normalized : Vector3.right;
    }
}