using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class SkillbarWindow : L2PopupWindow
{
    private void OnDestroy()
    {
        SkillToggleState.Changed -= OnToggleStateChanged;
        _instance = null;
    }

    void OnEnable()
    {
        SkillToggleState.Changed -= OnToggleStateChanged;
        SkillToggleState.Changed += OnToggleStateChanged;
        // Toggles may have switched while this window was hidden; repaint from the current set.
        OnToggleStateChanged();
    }

    void OnDisable()
    {
        SkillToggleState.Changed -= OnToggleStateChanged;
    }

    /// <summary>
    /// Mobius abnormal row changed: repaint the toggle shine on every bar slot so a running toggle
    /// (operate_type 3) shows the ToggleEffect marker and a switched-off one clears it.
    /// </summary>
    void OnToggleStateChanged()
    {
        if (_skillbars == null || !IsReady)
            return;
        _skillbars.ForEach(skillbar => skillbar.RefreshSlotMarks());
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillbarWindow");
        _skillbarHorizontalTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillBarHorizontal");
        _skillbarVerticalTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillBarVertical");
        _barSlotTemplate = LoadAsset("Data/UI/_Elements/Template/SkillbarSlot");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        RegisterClickWindowEvent(_windowEle, null);
        _skillbarContainerHorizontal = GetElementById("SkillbarContainerHorizontal");
        _skillbarContainerVertical = GetElementById("SkillbarContainerVertical");
        ToggleRotate();

        _skillbars = new List<AbstractSkillbar>();

        InitializeSkillbars();

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < _defaultSkillbarCount; i++)
        {
            AddSkillbar();
        }

        yield return new WaitForEndOfFrame();
        ApplyPosition();
        // Prefs / dock need resolved parent size — re-clamp after first layout pass.
        _windowEle.RegisterCallback<GeometryChangedEvent>(OnSkillbarGeometryChanged);
    }

    void OnSkillbarGeometryChanged(GeometryChangedEvent evt)
    {
        if (_windowEle == null)
            return;
        _windowEle.UnregisterCallback<GeometryChangedEvent>(OnSkillbarGeometryChanged);
        ApplyPosition();
    }
}
