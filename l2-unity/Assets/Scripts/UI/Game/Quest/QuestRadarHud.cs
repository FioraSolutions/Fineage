using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Compass drawn inside the extracted RadarContainerWnd (102x102, top-right).
/// The radar has no map texture. Heading, north, self, and the quest blip are the compass marks.
/// </summary>
public sealed class QuestRadarHud : MonoBehaviour
{
    const string ArrowPath = "Data/UI/Map/Radar/radar_arrow";
    const string NorthPath = "Data/UI/Map/Radar/radar_n";
    const string SelfPath = "Data/UI/Map/Radar/radar_myself";
    const string TargetPath = "Data/UI/Map/Radar/radar_target";
    const float NorthSizePx = 16f;
    const float SelfSizePx = 16f;
    const float TargetSizePx = 8f;

    VisualElement _host;
    VisualElement _compass;
    VisualElement _arrow;
    VisualElement _north;
    VisualElement _target;
    VisualElement _self;
    float _sizePx;
    bool _assetsFailed;
    bool _ready;

    float RimRadiusPx => _sizePx * 0.5f - NorthSizePx * 0.5f - 2f;

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
        AnchorNorthFixed();

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
            float cx = (_sizePx * 0.5f) + Mathf.Sin(rel) * r;
            float cy = (_sizePx * 0.5f) - Mathf.Cos(rel) * r;
            _target.style.display = DisplayStyle.Flex;
            _target.style.left = cx - TargetSizePx * 0.5f;
            _target.style.top = cy - TargetSizePx * 0.5f;
        }
        else
        {
            _target.style.display = DisplayStyle.None;
        }
    }

    void AnchorNorthFixed()
    {
        if (_north == null)
            return;
        float top = (_sizePx - NorthSizePx) * 0.5f - RimRadiusPx;
        _north.style.left = (_sizePx - NorthSizePx) * 0.5f;
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

        Texture2D arrow = Load(ArrowPath);
        Texture2D north = Load(NorthPath);
        Texture2D self = Load(SelfPath);
        Texture2D target = Load(TargetPath);
        if (arrow == null || north == null || self == null || target == null)
        {
            _assetsFailed = true;
            return false;
        }

        _host = new VisualElement { name = "RadarContainerWnd" };
        _host.pickingMode = PickingMode.Ignore;
        root.Add(_host);
        if (!InterludeWindowLayout.Apply(_host, "RadarContainerWnd"))
        {
            _assetsFailed = true;
            return false;
        }

        _compass = _host.Q<VisualElement>("Radar");
        if (_compass == null)
        {
            Debug.LogError("[QuestRadar] Radar is missing from RadarContainerWnd.");
            _assetsFailed = true;
            return false;
        }

        _sizePx = _compass.resolvedStyle.width;
        if (float.IsNaN(_sizePx) || _sizePx <= 1f)
            _sizePx = _compass.style.width.value.value;
        if (_sizePx <= 1f)
        {
            Debug.LogError("[QuestRadar] Radar has no positive size in the extracted layout.");
            _assetsFailed = true;
            return false;
        }

        _self = Image(self, SelfSizePx, (_sizePx - SelfSizePx) * 0.5f, (_sizePx - SelfSizePx) * 0.5f);
        _compass.Add(_self);

        _arrow = Image(arrow, _sizePx, 0f, 0f);
        _arrow.style.display = DisplayStyle.None;
        _arrow.style.transformOrigin = new StyleTransformOrigin(
            new TransformOrigin(Length.Percent(50), Length.Percent(50)));
        _compass.Add(_arrow);

        _north = Image(north, NorthSizePx, 0f, 0f);
        _compass.Add(_north);
        AnchorNorthFixed();

        _target = Image(target, TargetSizePx, 0f, 0f);
        _target.style.display = DisplayStyle.None;
        _compass.Add(_target);

        VisualElement icons = new VisualElement { name = "RadarWnd" };
        icons.pickingMode = PickingMode.Ignore;
        root.Add(icons);
        if (InterludeWindowLayout.Apply(icons, "RadarWnd"))
            ShowOnlyIcon(icons, "icon1");

        _host.style.display = DisplayStyle.None;
        _ready = true;
        return true;
    }

    static void ShowOnlyIcon(VisualElement icons, string visibleName)
    {
        icons.Query<VisualElement>(className: "il-xdat").ForEach(element =>
        {
            if (element == icons || element.name == "RadarWnd")
                return;
            element.style.display = element.name == visibleName ? DisplayStyle.Flex : DisplayStyle.None;
        });
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
            Debug.LogError("[QuestRadar] missing texture path='" + path + "'.");
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
