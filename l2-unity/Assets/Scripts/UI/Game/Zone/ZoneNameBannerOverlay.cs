using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Center-top zone/town name banner (Interlude ExShowScreenMessage-style fade).</summary>
public sealed class ZoneNameBannerOverlay : MonoBehaviour
{
    const float FadeInSeconds = 0.35f;
    const float HoldSeconds = 2.5f;
    const float FadeOutSeconds = 0.8f;

    static ZoneNameBannerOverlay _instance;
    UIDocument _document;
    VisualElement _label;
    float _phaseEnd;
    int _phase;

    public static ZoneNameBannerOverlay Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        ZoneTransitionPacketServices.Bind(this);
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    void EnsureUi()
    {
        if (_label != null)
            return;
        _document = GetComponent<UIDocument>() ?? gameObject.AddComponent<UIDocument>();
        var root = new VisualElement { name = "ZoneNameBannerRoot" };
        root.style.position = Position.Absolute;
        root.style.left = 0;
        root.style.right = 0;
        root.style.top = 72;
        root.style.flexDirection = FlexDirection.Row;
        root.style.justifyContent = Justify.Center;
        root.pickingMode = PickingMode.Ignore;
        _document.rootVisualElement.Add(root);

        _label = new Label { name = "ZoneNameBanner" };
        _label.style.fontSize = 22;
        _label.style.unityFontStyleAndWeight = FontStyle.Bold;
        _label.style.color = new Color(0.95f, 0.92f, 0.75f, 0f);
        _label.style.unityTextAlign = TextAnchor.MiddleCenter;
        _label.style.display = DisplayStyle.None;
        root.Add(_label);
    }

    public void Show(string townName)
    {
        if (string.IsNullOrEmpty(townName))
        {
            Debug.LogError("[ZoneNameBanner] Show called with empty town name");
            return;
        }

        EnsureUi();
        _label.text = townName;
        _label.style.display = DisplayStyle.Flex;
        _phase = 0;
        _phaseEnd = Time.unscaledTime + FadeInSeconds;
        _label.style.color = new Color(0.95f, 0.92f, 0.75f, 0f);
    }

    void Update()
    {
        if (_label == null || _label.style.display == DisplayStyle.None)
            return;

        float now = Time.unscaledTime;
        if (now >= _phaseEnd)
        {
            _phase++;
            if (_phase == 1)
                _phaseEnd = now + HoldSeconds;
            else if (_phase == 2)
                _phaseEnd = now + FadeOutSeconds;
            else
            {
                _label.style.display = DisplayStyle.None;
                return;
            }
        }

        float t = Mathf.Clamp01(1f - (_phaseEnd - now) / PhaseDuration(_phase));
        float alpha = _phase switch
        {
            0 => t,
            1 => 1f,
            _ => 1f - t,
        };
        _label.style.color = new Color(0.95f, 0.92f, 0.75f, alpha);
    }

    static float PhaseDuration(int phase) =>
        phase switch
        {
            0 => FadeInSeconds,
            1 => HoldSeconds,
            _ => FadeOutSeconds,
        };
}