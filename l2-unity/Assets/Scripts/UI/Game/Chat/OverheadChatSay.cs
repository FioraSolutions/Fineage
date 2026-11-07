using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Short-lived say text above a pawn's head (CreatureSay ObjectId). Chat log stays in
/// <see cref="ChatWindow"/>; this is the missing world bubble for #521.
/// </summary>
public static class OverheadChatSay
{
    const float LifetimeSec = 4f;
    const float HeadOffsetMeters = 2.2f;
    const int FontSize = 16;
    const int MaxChars = 80;

    static Canvas _canvas;
    static Font _font;

    public static void Show(int objectId, string text, int chatType)
    {
        if (objectId <= 0 || string.IsNullOrEmpty(text))
            return;
        if (chatType == NetChat.Announcement || chatType == NetChat.CriticalAnnounce)
            return;

        World world = WorldApplyPacketServices.GameWorld ?? GamePacketServices.GameWorld;
        if (world == null)
            return;

        Entity entity = world.GetEntityNoLockSync(objectId);
        if (entity == null || entity.transform == null)
            return;

        string shown = text.Length > MaxChars ? text.Substring(0, MaxChars) + "…" : text;
        Color color;
        if (!ColorUtility.TryParseHtmlString(NetChat.ColorFor(chatType), out color))
            color = Color.white;

        EnsureCanvas();
        GameObject go = new GameObject("OverheadSay");
        go.transform.SetParent(_canvas.transform, false);
        Text label = go.AddComponent<Text>();
        label.font = _font;
        label.fontSize = FontSize;
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.raycastTarget = false;
        label.text = shown;
        label.color = color;

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(420f, 48f);
        go.AddComponent<OverheadChatSayRunner>().Begin(entity.transform, LifetimeSec, HeadOffsetMeters);
    }

    static void EnsureCanvas()
    {
        if (_canvas != null)
            return;

        GameObject root = GameObject.Find("OverheadChatCanvas");
        if (root == null)
        {
            root = new GameObject("OverheadChatCanvas");
            Object.DontDestroyOnLoad(root);
        }

        _canvas = root.GetComponent<Canvas>();
        if (_canvas == null)
            _canvas = root.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 40;

        if (root.GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
        }

        if (root.GetComponent<GraphicRaycaster>() == null)
            root.AddComponent<GraphicRaycaster>();

        if (_font == null)
            _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (_font == null)
            _font = Font.CreateDynamicFontFromOSFont("Arial", FontSize);
    }
}

/// <summary>Follows the speaker in screen space until the bubble expires.</summary>
public sealed class OverheadChatSayRunner : MonoBehaviour
{
    Transform _follow;
    float _endAt;
    float _headOffset;
    Text _label;
    Color _baseColor;

    public void Begin(Transform follow, float lifetimeSec, float headOffsetMeters)
    {
        _follow = follow;
        _endAt = Time.time + lifetimeSec;
        _headOffset = headOffsetMeters;
        _label = GetComponent<Text>();
        _baseColor = _label != null ? _label.color : Color.white;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        Camera cam = Camera.main;
        while (Time.time < _endAt && _follow != null)
        {
            if (cam == null)
                cam = Camera.main;
            if (cam != null)
            {
                Vector3 world = _follow.position + Vector3.up * _headOffset;
                Vector3 screen = cam.WorldToScreenPoint(world);
                if (screen.z > 0f)
                {
                    transform.position = screen;
                    float left = _endAt - Time.time;
                    if (left < 1f && _label != null)
                    {
                        Color c = _baseColor;
                        c.a = Mathf.Clamp01(left);
                        _label.color = c;
                    }
                }
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}
