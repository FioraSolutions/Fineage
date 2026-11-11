using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VContainer;

public partial class HtmlWindow : L2PopupWindow
{
    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/HtmlWindow");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        VisualElement dragArea = GetElementByClass("drag-area");

        _content = GetElementByClass("content");
        _scrollView = _windowEle.Q<ScrollView>();
        _communityTopButtons = _windowEle.Q<VisualElement>("CommunityTopButtons");

        if (_content == null)
        {
            // Debug.LogError("HtmlWindow: content element not found.");
            return;
        }

        if (_communityTopButtons != null)
            _communityTopButtons.style.display = DisplayStyle.None;

        _titleLabel = _windowEle.Q<Label>("windows-name-label");

        _content.style.display = DisplayStyle.Flex;
        _content.style.flexDirection = FlexDirection.Column;
        _content.style.alignItems = Align.Stretch;
        _content.style.justifyContent = Justify.FlexStart;
        _content.style.width = Length.Percent(100);
        _content.style.flexGrow = 1;
        _content.style.flexShrink = 0;

        if (dragArea != null)
            dragArea.AddManipulator(new DragManipulator(dragArea, _windowEle));

        RegisterCloseWindowEvent("btn-close-frame");

        if (dragArea != null)
            RegisterClickWindowEvent(_windowEle, dragArea);

        RegisterCommunityButtons();

        if (!_normalWindowPositionInitialized && _root != null)
        {
            OnCenterScreen(_root);
            _normalWindowPositionInitialized = true;
        }
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();
    }

    bool CanSend(long cooldownMs)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (now < _flood)
            return false;

        _flood = now + cooldownMs;
        return true;
    }
}
