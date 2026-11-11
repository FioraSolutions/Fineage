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
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/Chat/ChatTabHeader");
        _messageLabelTemplate = LoadAsset("Data/UI/_Elements/Game/Chat/MessageLabelTemplate");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        Ready = false;
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var diagonalResizeHandle = GetElementByClass("resize-diag");

        DiagonalResizeManipulator diagonalResizeManipulator = new DiagonalResizeManipulator(
            diagonalResizeHandle,
            _windowEle,
            _chatWindowMinWidth,
            _chatWindowMaxWidth,
            _chatWindowMinHeight,
            _chatWindowMaxHeight,
            14.5f,
            2f);

        diagonalResizeHandle.AddManipulator(diagonalResizeManipulator);

        _chatInput = (TextField)GetElementById("ChatInputField");
        _chatInput.RegisterCallback<FocusEvent>(OnChatInputFocus);
        _chatInput.RegisterCallback<BlurEvent>(OnChatInputBlur);
        int limit = _chatInputCharacterLimit;
        _chatInput.maxLength = limit;

        var enlargeTextBtn = (Button)GetElementById("EnlargeTextBtn");
        enlargeTextBtn.AddManipulator(new ButtonClickSoundManipulator(enlargeTextBtn));

        var chatOptionsBtn = (Button)GetElementById("ChatOptionsBtn");
        chatOptionsBtn.AddManipulator(new ButtonClickSoundManipulator(chatOptionsBtn));

        _chatInput.AddManipulator(new BlinkingCursorManipulator(_chatInput));

        _chatInputContainer = GetElementById("InnerBar");

        // Interlude SystemMsgWnd above ChatWnd (C4+). Start before tabs so Flush can queue into it.
        SystemMsgWindow sysMsg = SystemMsgWindow.Ensure();
        sysMsg.AddWindow(root);
        sysMsg.DockAboveChat(_windowEle);

        CreateTabs();

        yield return new WaitForEndOfFrame();
        diagonalResizeManipulator.SnapSize();
        // SystemMsgWnd must match ChatWnd width after SnapSize/layout.
        SystemMsgWindow.Ensure()?.SyncDock();
        yield return null;
        SystemMsgWindow.Ensure()?.SyncDock();
    }
}
