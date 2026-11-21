using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude top-right CH3 radar (L2UI_CH3 Radar package). Heading-up compass
/// with a quest-target needle and blip from the shared QuestDirection pipeline.
/// Mounted on the game UI document so it follows HUD visibility.
/// </summary>
public sealed class QuestRadarHud : MonoBehaviour
{
    const string BackPath = "Data/UI/Map/Radar/radar_back";
    const string ArrowPath = "Data/UI/Map/Radar/radar_arrow";
    const string NorthPath = "Data/UI/Map/Radar/radar_n";
    const string SelfPath = "Data/UI/Map/Radar/radar_myself";
    const string TargetPath = "Data/UI/Map/Radar/radar_target";
    const float SizePx = 128f;
    const float MarginPx = 12f;
    const float NorthSizePx = 16f;
    const float SelfSizePx = 16f;
    const float TargetSizePx = 8f;
    const float RimRadiusPx = 54f;

    VisualElement _host;
    VisualElement _disc;
    VisualElement _arrow;
    VisualElement _north;
    VisualElement _target;
    VisualElement _self;
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
        // Unity +X = L2 +Y (north), Unity +Z = L2 +X (east).
        float faceAng = Mathf.Atan2(fwd.z, fwd.x);
        if (_disc != null)
            _disc.transform.rotation = Quaternion.Euler(0f, 0f, faceAng * Mathf.Rad2Deg);
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

    void AnchorNorthFixed()
    {
        if (_north == null)
            return;
        float top = (SizePx - NorthSizePx) * 0.5f - RimRadiusPx;
        _north.style.left = (SizePx - NorthSizePx) * 0.5f;
        _north.style.top = Mathf.Max(0f, top);
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
        Texture2D arrow = Load(ArrowPath);
        Texture2D north = Load(NorthPath);
        Texture2D self = Load(SelfPath);
        Texture2D target = Load(TargetPath);
        if (back == null || arrow == null || north == null || self == null || target == null)
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
        _host.style.height = SizePx;
        _host.style.display = DisplayStyle.None;
        root.Add(_host);

        _disc = new VisualElement { name = "QuestRadarDisc" };
        _disc.pickingMode = PickingMode.Ignore;
        _disc.style.position = Position.Absolute;
        _disc.style.left = 0;
        _disc.style.top = 0;
        _disc.style.width = SizePx;
        _disc.style.height = SizePx;
        _disc.style.backgroundImage = new StyleBackground(back);
        _disc.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));
        _host.Add(_disc);

        _arrow = Image(arrow, SizePx, 0f, 0f);
        _arrow.style.display = DisplayStyle.None;
        _host.Add(_arrow);

        _north = Image(north, NorthSizePx, 0f, 0f);
        _host.Add(_north);
        AnchorNorthFixed();

        _target = Image(target, TargetSizePx, 0f, 0f);
        _target.style.display = DisplayStyle.None;
        _host.Add(_target);

        _self = Image(self, SelfSizePx, (SizePx - SelfSizePx) * 0.5f, (SizePx - SelfSizePx) * 0.5f);
        _host.Add(_self);

        _ready = true;
        return true;
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
            Debug.LogError($"[QuestRadar] missing texture path={path}. Import L2UI_CH3 Radar from Interlude client.");
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
