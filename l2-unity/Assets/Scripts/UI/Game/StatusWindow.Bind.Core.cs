using System;

public partial class StatusWindow
{
    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var statusWindowDragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(statusWindowDragArea, _windowEle);
        statusWindowDragArea.AddManipulator(drag);

        var horizontalResizeHandle = GetElementByClass("hor-resize-handle");
        HorizontalResizeManipulator horizontalResize = new HorizontalResizeManipulator(
            horizontalResizeHandle, _windowEle, _statusWindowMinWidth, _statusWindowMaxWidth);
        horizontalResizeHandle.AddManipulator(horizontalResize);

        _windowEle.RegisterCallback<ClickEvent>(OnStatusWindowClicked);

        _nameLabel = (Label)GetElementById("PlayerNameText");
        if(_nameLabel == null) {
            // Debug.LogError("Status window PlayerNameText is null.");
        }

        _levelLabel = (Label)GetElementById("LevelText");
        if(_levelLabel == null) {
            // Debug.LogError("Status window LevelText is null.");
        }

        _CPTextLabel = (Label)GetElementById("CPText");
        if(_CPTextLabel == null) {
            // Debug.LogError("Status window CPText is null.");
        }

        _HPTextLabel = (Label)GetElementById("HPText");
        if(_HPTextLabel == null) {
            // Debug.LogError("Status window Hp text is null.");
        }

        _MPTextLabel = (Label)GetElementById("MPText");
        if(_MPTextLabel == null) {
            // Debug.LogError("Status window MPText is null.");
        }

        _CPBarBG = GetElementById("CPBarBG");
        if(_CPBarBG == null) {
            // Debug.LogError("Status window CPBarBG is null");
        }

        _CPBar = GetElementById("CPBar");
        if(_CPBar == null) {
            // Debug.LogError("Status window CPBar is null");
        }

        _HPBar = GetElementById("HPBar");
        if(_HPBar == null) {
            // Debug.LogError("Status window HPBar is null");
        }

        _HPBarBG = GetElementById("HPBarBG");
        if(_HPBarBG == null) {
            // Debug.LogError("Status window HPBarBG is null");
        }

        _MPBarBG = GetElementById("MPBarBG");
        if(_MPBarBG == null) {
            // Debug.LogError("Status window MPBarBG is null");
        }

        _EXPBar = GetElementById("EXPBar");
        _EXPBarBG = GetElementById("EXPBarBG");
        _EXPTextLabel = (Label)GetElementById("EXPText");

        _MPBar = GetElementById("MPBar");
        if(_MPBar == null) {
            // Debug.LogError("Status windowar MPBar is null");
        }
    }

    private void OnStatusWindowClicked(ClickEvent evt)
    {
        if (evt.button != 0)
        {
            return;
        }

        SelectPlayerAsTarget();
        evt.StopPropagation();
    }

    private void SelectPlayerAsTarget()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null || TargetPacketServices.Targets == null)
        {
            return;
        }

        // Zone SelectTarget for self does not fan TargetSelected — set the local target window here.
        TargetPacketServices.Targets.SetTarget(
            new ObjectData(player.gameObject),
            "#ffffff");
        SendSelfTargetAction(player);
    }
}
