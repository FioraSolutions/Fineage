using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interlude draws a black say box above the speaker. L2UI has no balloon texture for it;
/// the client fills a black rect and prints the line in the say color. Whisper and
/// announcements stay in the chat log only.
/// </summary>
public static class OverheadChatSay
{
    const float LifetimeSec = 5f;
    const int FontSize = 13;
    const int MaxChars = 80;

    static Canvas _canvas;
    static Font _font;
    static Sprite _pixel;
    static readonly Dictionary<int, OverheadChatSayRunner> _live = new Dictionary<int, OverheadChatSayRunner>();

    public static void Show(int objectId, string text, int chatType)
    {
        if (objectId <= 0 || string.IsNullOrEmpty(text))
            return;
        if (!ShowsOverHead(chatType))
            return;

        World world = WorldApplyPacketServices.GameWorld ?? GamePacketServices.GameWorld;
        Entity entity = world != null ? world.GetEntityNoLockSync(objectId) : null;
        if (entity == null)
        {
            PlayerEntity local = DomainPacketServices.LocalPlayer;
            if (local != null && local.Identity != null && local.Identity.Id == objectId)
                entity = local;
        }
        if (entity == null || entity.transform == null)
            return;

        string shown = text.Length > MaxChars ? text.Substring(0, MaxChars) + "…" : text;
        Color color;
        if (!ColorUtility.TryParseHtmlString(NetChat.ColorFor(chatType), out color))
            color = Color.white;

        EnsureCanvas();
        if (_live.TryGetValue(objectId, out OverheadChatSayRunner previous) && previous != null)
            Object.Destroy(previous.gameObject);

        GameObject go = new GameObject("OverheadSay");
        go.transform.SetParent(_canvas.transform, false);

        Image plate = go.AddComponent<Image>();
        plate.sprite = Pixel();
        plate.type = Image.Type.Simple;
        plate.color = new Color(0f, 0f, 0f, 0.86f);
        plate.raycastTarget = false;

        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        Text label = textGo.AddComponent<Text>();
        label.font = _font;
        label.fontSize = FontSize;
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.raycastTarget = false;
        label.supportRichText = false;
        label.text = shown;
        label.color = color;

        RectTransform textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(6f, 3f);
        textRect.offsetMax = new Vector2(-6f, -3f);

        float textWidth = Mathf.Max(1f, label.preferredWidth);
        float width = Mathf.Clamp(textWidth + 14f, 36f, 280f);
        int lines = Mathf.Clamp(Mathf.CeilToInt(textWidth / (width - 14f)), 1, 3);
        float height = lines * (FontSize + 2f) + 8f;
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 0f);
        rect.sizeDelta = new Vector2(width, height);

        GameObject tail = new GameObject("Tail");
        tail.transform.SetParent(go.transform, false);
        Image tailImage = tail.AddComponent<Image>();
        tailImage.sprite = Pixel();
        tailImage.color = plate.color;
        tailImage.raycastTarget = false;
        RectTransform tailRect = tail.GetComponent<RectTransform>();
        tailRect.anchorMin = new Vector2(0.5f, 0f);
        tailRect.anchorMax = new Vector2(0.5f, 0f);
        tailRect.pivot = new Vector2(0.5f, 1f);
        tailRect.sizeDelta = new Vector2(8f, 5f);
        tailRect.anchoredPosition = Vector2.zero;

        OverheadChatSayRunner runner = go.AddComponent<OverheadChatSayRunner>();
        _live[objectId] = runner;
        runner.Begin(entity, objectId, LifetimeSec);
    }

    static bool ShowsOverHead(int chatType)
    {
        return chatType == NetChat.All
            || chatType == NetChat.Shout
            || chatType == NetChat.Party
            || chatType == NetChat.Clan
            || chatType == NetChat.Alliance;
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

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        if (scaler != null)
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        if (_font == null)
            _font = Resources.Load<Font>("Data/UI/Assets/Font/tahoma");
        if (_font == null)
            _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (_font == null)
            _font = Font.CreateDynamicFontFromOSFont("Arial", FontSize);
    }

    static Sprite Pixel()
    {
        if (_pixel != null)
            return _pixel;
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        _pixel = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        return _pixel;
    }

    internal static void Release(int objectId, OverheadChatSayRunner runner)
    {
        if (_live.TryGetValue(objectId, out OverheadChatSayRunner current) && current == runner)
            _live.Remove(objectId);
    }
}

/// <summary>Follows the speaker in screen space until the box expires.</summary>
public sealed class OverheadChatSayRunner : MonoBehaviour
{
    Entity _entity;
    int _objectId;
    float _endAt;
    Image _plate;
    Image _tail;
    Text _label;
    Color _plateColor;
    Color _textColor;

    public void Begin(Entity entity, int objectId, float lifetimeSec)
    {
        _entity = entity;
        _objectId = objectId;
        _endAt = Time.time + lifetimeSec;
        _plate = GetComponent<Image>();
        _label = GetComponentInChildren<Text>();
        Image[] images = GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] != _plate)
                _tail = images[i];
        }
        _plateColor = _plate != null ? _plate.color : new Color(0f, 0f, 0f, 0.86f);
        _textColor = _label != null ? _label.color : Color.white;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        Camera cam = Camera.main;
        while (Time.time < _endAt && _entity != null)
        {
            if (cam == null)
                cam = Camera.main;
            if (cam != null)
            {
                Vector2 screenBottom = default;
                NameplatesManager plates = NameplatePacketServices.Nameplates;
                bool onScreen = plates != null
                    && plates.TryResolveOverheadSayScreenPos(cam, _entity, _entity.transform, out screenBottom);
                if (!onScreen)
                {
                    Vector3 world = L2OverheadAnchor.GetWorldPos(_entity, _entity.transform);
                    Vector3 screen = cam.WorldToScreenPoint(world);
                    if (screen.z <= 0f)
                    {
                        yield return null;
                        continue;
                    }

                    float lineH = L2NameplateScreenLayout.FallbackLineHeightPx;
                    screenBottom = L2NameplateScreenLayout.ResolveSayBottomLeft(
                        cam.pixelHeight, screen.x, screen.y, lineH);
                    onScreen = true;
                }

                if (onScreen)
                {
                    transform.position = new Vector3(screenBottom.x, screenBottom.y, 0f);
                    float left = _endAt - Time.time;
                    float alpha = left < 0.6f ? Mathf.Clamp01(left / 0.6f) : 1f;
                    if (_plate != null)
                    {
                        Color c = _plateColor;
                        c.a *= alpha;
                        _plate.color = c;
                    }
                    if (_tail != null)
                    {
                        Color c = _plateColor;
                        c.a *= alpha;
                        _tail.color = c;
                    }
                    if (_label != null)
                    {
                        Color c = _textColor;
                        c.a *= alpha;
                        _label.color = c;
                    }
                }
            }
            yield return null;
        }

        OverheadChatSay.Release(_objectId, this);
        Destroy(gameObject);
    }
}
