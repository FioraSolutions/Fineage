using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>Code-built Options panel (Audio + Video) opened from the System menu.</summary>
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

        Add(Header("Options"));
        Add(Header("Sound"));
        Add(VolumeSlider("Master", GameSettings.Master, v => GameSettings.Master = v));
        Add(VolumeSlider("Music", GameSettings.Music, v => GameSettings.Music = v));
        Add(VolumeSlider("Effects", GameSettings.Sfx, v => GameSettings.Sfx = v));
        Add(VolumeSlider("Ambient", GameSettings.Ambient, v => GameSettings.Ambient = v));
        var mute = new Toggle("Mute all") { value = GameSettings.Mute };
        mute.RegisterValueChangedCallback(e => GameSettings.Mute = e.newValue);
        Tint(mute); Add(mute);

        Add(Header("Video"));
        var choices = new List<string>();
        int sel = 0;
        for (int i = 0; i < GameSettings.Resolutions.Length; i++)
        {
            var r = GameSettings.Resolutions[i];
            choices.Add(r.x + " x " + r.y);
            if (r.x == Screen.width && r.y == Screen.height) sel = i;
        }
        var res = new DropdownField("Resolution", choices, sel);
        Tint(res); Add(res);
        var full = new Toggle("Fullscreen") { value = Screen.fullScreen };
        Tint(full); Add(full);

        var row = new VisualElement(); row.style.flexDirection = FlexDirection.Row; row.style.justifyContent = Justify.FlexEnd; row.style.marginTop = 8;
        var apply = new Button(() => { var r = GameSettings.Resolutions[Mathf.Max(0, res.index)]; GameSettings.ApplyResolution(r.x, r.y, full.value); }) { text = "Apply" };
        var close = new Button(() => RemoveFromHierarchy()) { text = "Close" };
        row.Add(apply); row.Add(close); Add(row);
    }

    static Label Header(string t)
    {
        var l = new Label(t); l.style.color = Gold; l.style.unityFontStyleAndWeight = FontStyle.Bold; l.style.marginTop = 4; l.style.marginBottom = 2;
        return l;
    }

    static void Tint(VisualElement e) { e.style.color = Color.white; var lbl = e.Q<Label>(); if (lbl != null) { lbl.style.color = Color.white; lbl.style.minWidth = 80; } }

    static VisualElement VolumeSlider(string name, float value, System.Action<float> set)
    {
        var s = new Slider(name, 0f, 1f) { value = value, showInputField = false };
        s.RegisterValueChangedCallback(e => set(e.newValue));
        Tint(s);
        return s;
    }
}
