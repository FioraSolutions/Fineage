using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ChatWindow : L2Window
{
    private void OnDestroy()
    {
        Ready = false;
        if (_instance == this)
            _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Chat/ChatWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/Chat/ChatTab");
        _messageLabelTemplate = LoadAsset("Data/UI/_Elements/Game/Chat/MessageLabelTemplate");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        Ready = false;
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        InterludeWindowLayout.Apply(_windowEle, "ChatWnd");

        _chatInput = GetElementById("ChatInputField") as TextField;
        VisualElement edit = GetElementById("ChatEditBox");
        if (_chatInput == null || edit == null)
        {
            Debug.LogError("[ChatWindow] ChatInputField or ChatEditBox is missing from the extracted layout.");
        }
        else
        {
            if (_chatInput.parent != edit)
                edit.Add(_chatInput);
            _chatInput.style.position = Position.Absolute;
            _chatInput.style.left = 0f;
            _chatInput.style.top = 0f;
            _chatInput.style.right = 0f;
            _chatInput.style.bottom = 0f;
            _chatInput.style.marginLeft = 0f;
            _chatInput.style.marginRight = 0f;
            _chatInput.style.marginTop = 0f;
            _chatInput.style.marginBottom = 0f;
            _chatInput.RegisterCallback<FocusEvent>(OnChatInputFocus);
            _chatInput.RegisterCallback<BlurEvent>(OnChatInputBlur);
            int limit = InterludeWindowLayout.ControlMaxLength("ChatWnd", "ChatEditBox");
            if (limit <= 0)
                Debug.LogError("[ChatWindow] ChatEditBox has no maxLength in the extracted layout.");
            else
                _chatInput.maxLength = limit;
            _chatInput.AddManipulator(new BlinkingCursorManipulator(_chatInput));
            _chatInputContainer = edit;
        }

        // SystemMsgWnd sits above ChatWnd. Start before tabs so Flush can queue into it.
        SystemMsgWindow sysMsg = SystemMsgWindow.Ensure();
        sysMsg.AddWindow(root);
        sysMsg.DockAboveChat(_windowEle);

        CreateTabs();

        yield return new WaitForEndOfFrame();
        SystemMsgWindow.Ensure()?.SyncDock();
        yield return null;
        SystemMsgWindow.Ensure()?.SyncDock();
        L2InterludeHudSkinPipeline.ValidateTextures(
            InterludeHudWindowCatalog.ChatWnd,
            nameof(ChatWindow));
    }
}
