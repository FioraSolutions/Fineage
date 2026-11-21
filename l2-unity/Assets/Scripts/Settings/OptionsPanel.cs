using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

/// <summary>Code-built Options panel (Audio + Video + Language) opened from the System / Login menu.</summary>
public class OptionsPanel : VisualElement
{
    static readonly Color Gold = new Color(0.83f, 0.74f, 0.55f);

    public OptionsPanel()
    {
        style.position = Position.Absolute;
        style.left = Length.Percent(50); style.top = Length.Percent(50);
        style.width = 300; style.marginLeft = -150; style.marginTop = -150;
        style.backgroundColor = new Color(0.05f, 0.05f, 0.06f, 0.94f);
        style.borderTopWidth = style.borderBottomWidth = style.borderLeftWidth = style.borderRightWidth = 1;
        var bc = new Color(0.45f, 0.38f, 0.25f);
        style.borderTopColor = style.borderBottomColor = style.borderLeftColor = style.borderRightColor = bc;
        style.paddingLeft = style.paddingRight = 10; style.paddingTop = style.paddingBottom = 8;
        pickingMode = PickingMode.Position;

        Add(Header("ui.options.title"));
        Add(Header("ui.options.sound"));
        Add(VolumeSlider("ui.options.master", GameSettings.Master, v => GameSettings.Master = v));
        Add(VolumeSlider("ui.options.music", GameSettings.Music, v => GameSettings.Music = v));
        Add(VolumeSlider("ui.options.effects", GameSettings.Sfx, v => GameSettings.Sfx = v));
        Add(VolumeSlider("ui.options.ambient", GameSettings.Ambient, v => GameSettings.Ambient = v));
        var mute = new Toggle { value = GameSettings.Mute };
        L10nUiBinder.SetText(mute, "ui.options.mute_all");
        mute.RegisterValueChangedCallback(e => GameSettings.Mute = e.newValue);
        Tint(mute); Add(mute);

        Add(Header("ui.options.video"));
        var choices = new List<string>();
        int sel = 0;
        for (int i = 0; i < GameSettings.Resolutions.Length; i++)
        {
            var r = GameSettings.Resolutions[i];
            choices.Add(r.x + " x " + r.y);
            if (r.x == Screen.width && r.y == Screen.height) sel = i;
        }
        var res = new DropdownField(L10n.Get("ui.options.resolution"), choices, sel);
        Tint(res); Add(res);
        var full = new Toggle { value = Screen.fullScreen };
        L10nUiBinder.SetText(full, "ui.options.fullscreen");
        Tint(full); Add(full);

        var qualityNames = new List<string>(QualitySettings.names);
        var quality = new DropdownField(L10n.Get("ui.options.graphics_quality"), qualityNames, GameSettings.GraphicsQualityLevelIndex);
        quality.RegisterValueChangedCallback(e =>
        {
            int idx = qualityNames.IndexOf(e.newValue);
            if (idx >= 0)
                GameSettings.GraphicsQualityLevelIndex = idx;
        });
        Tint(quality);
        Add(quality);

        var localeChoices = new List<string>(PlayerLanguage.AvailableTags);
        int localeIndex = Mathf.Max(0, localeChoices.IndexOf(PlayerLanguage.Tag));
        var language = new DropdownField(L10n.Get("ui.options.language"), localeChoices, localeIndex);
        language.RegisterValueChangedCallback(e => PlayerLanguage.SetTag(e.newValue));
        Tint(language);
        Add(language);

        var row = new VisualElement(); row.style.flexDirection = FlexDirection.Row; row.style.justifyContent = Justify.FlexEnd; row.style.marginTop = 8;
        var apply = new Button(() => { var r = GameSettings.Resolutions[Mathf.Max(0, res.index)]; GameSettings.ApplyResolution(r.x, r.y, full.value); });
        L10nUiBinder.SetText(apply, "ui.options.apply");
        var close = new Button(() => RemoveFromHierarchy());
        L10nUiBinder.SetText(close, "ui.options.close");
        row.Add(apply); row.Add(close); Add(row);

        L10n.LocaleChanged += OnLocaleChanged;
        RegisterCallback<DetachFromPanelEvent>(_ => L10n.LocaleChanged -= OnLocaleChanged);
    }

    void OnLocaleChanged()
    {
        L10nUiBinder.Bind(this);
    }

    Label Header(string key)
    {
        var l = new Label(L10n.Get(key));
        l.AddToClassList("l10n");
        l.userData = key;
        l.style.color = Gold;
        l.style.unityFontStyleAndWeight = FontStyle.Bold;
        l.style.marginTop = 4;
        l.style.marginBottom = 2;
        return l;
    }

    static void Tint(VisualElement e) { e.style.color = Color.white; var lbl = e.Q<Label>(); if (lbl != null) { lbl.style.color = Color.white; lbl.style.minWidth = 80; } }

    static VisualElement VolumeSlider(string labelKey, float value, System.Action<float> set)
    {
        var s = new Slider(L10n.Get(labelKey), 0f, 1f) { value = value, showInputField = false };
        s.AddToClassList("l10n");
        s.userData = labelKey;
        s.RegisterValueChangedCallback(e => set(e.newValue));
        Tint(s);
        return s;
    }
}