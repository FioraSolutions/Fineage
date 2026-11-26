using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interlude draws the Attack hit list as floating text on the target (number, Miss, crit color).
/// There is no L2UI window for it — the client prints the line in screen space.
/// </summary>
public static class AttackHitLabel
{
    const float LifetimeSec = 0.9f;
    const int FontSize = 16;

    static Canvas _canvas;
    static Font _font;

    public static void Show(Entity target, AttackHitCue cue)
    {
        if (target == null || target.transform == null || string.IsNullOrEmpty(cue.Label))
            return;

        EnsureCanvas();
        if (_canvas == null || _font == null)
        {
            Debug.LogError("[Attack] hit label canvas or font missing label=" + cue.Label);
            return;
        }

        GameObject go = new GameObject("AttackHitLabel");
        go.transform.SetParent(_canvas.transform, false);
        Text label = go.AddComponent<Text>();
        label.font = _font;
        label.fontSize = cue.Kind == AttackHitLabelKind.Critical ? FontSize + 4 : FontSize;
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.raycastTarget = false;
        label.supportRichText = false;
        label.text = cue.Label;
        label.color = ColorFor(cue.Kind);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(Mathf.Max(48f, label.preferredWidth + 8f), FontSize + 10f);

        AttackHitLabelRunner runner = go.AddComponent<AttackHitLabelRunner>();
        runner.Begin(target, cue.Index, LifetimeSec);
    }

    public static Color ColorFor(AttackHitLabelKind kind)
    {
        switch (kind)
        {
            case AttackHitLabelKind.Critical:
                return new Color(1f, 0.28f, 0.16f, 1f);
            case AttackHitLabelKind.Shield:
                return new Color(0.55f, 0.78f, 1f, 1f);
            default:
                return Color.white;
        }
    }

    static void EnsureCanvas()
    {
        if (_canvas != null)
            return;

        GameObject root = new GameObject("AttackHitLabelCanvas");
        Object.DontDestroyOnLoad(root);
        _canvas = root.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 40;
        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        if (_font == null)
            _font = Resources.Load<Font>("Data/UI/Assets/Font/tahoma");
        if (_font == null)
            _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (_font == null)
            _font = Font.CreateDynamicFontFromOSFont("Arial", FontSize);
        if (_font == null)
            Debug.LogError("[Attack] hit label font missing (tahoma / Arial)");
    }
}

public sealed class AttackHitLabelRunner : MonoBehaviour
{
    Entity _target;
    int _index;
    float _endAt;
    float _rise;
    Text _label;
    Color _color;

    public void Begin(Entity target, int index, float lifetimeSec)
    {
        _target = target;
        _index = index;
        _endAt = Time.time + lifetimeSec;
        _label = GetComponent<Text>();
        _color = _label != null ? _label.color : Color.white;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        Camera cam = Camera.main;
        while (Time.time < _endAt && _target != null)
        {
            if (cam == null)
                cam = Camera.main;
            if (cam != null && _target.transform != null)
            {
                Vector3 world = _target.transform.position + Vector3.up * (1.4f + _rise);
                Vector3 screen = cam.WorldToScreenPoint(world);
                if (screen.z > 0f)
                {
                    float stack = _index * 18f;
                    transform.position = new Vector3(screen.x, screen.y + stack + _rise * 36f, 0f);
                    float left = _endAt - Time.time;
                    float alpha = left < 0.35f ? Mathf.Clamp01(left / 0.35f) : 1f;
                    if (_label != null)
                    {
                        Color c = _color;
                        c.a = alpha;
                        _label.color = c;
                    }
                }
            }

            _rise += Time.deltaTime * 0.55f;
            yield return null;
        }

        Destroy(gameObject);
    }
}
