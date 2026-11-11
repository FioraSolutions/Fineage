using UnityEngine;
using UnityEngine.UIElements;

public partial class MouseOverDetectionManipulator : PointerManipulator
{
    public MouseOverDetectionManipulator(VisualElement target)
    {
        this.target = target;
        _ui = SystemUIPacketServices.Ui != null ? SystemUIPacketServices.Ui : LobbyUiPacketServices.LoginUi;
    }

    public void RefreshTargetElement(VisualElement newTarget)
    {
        if (target != null) UnregisterCallbacksFromTarget();
        target = newTarget;
        if (target != null) RegisterCallbacksOnTarget();
    }

    public void SetSubElement(VisualElement subElement) => _subElement = subElement;

    public void Enable() => _enabled = true;

    public void Disable()
    {
        _enabled = false;
        if (_overThisManipulator)
        {
           // Debug.Log("PointerLeaveHandler> leave1 unblock 2");
            UnBlock();
        }

    }

    public void SetBlock(bool isBlock) { if (isBlock) Block(); else UnBlock(); }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerEnterEvent>(PointerEnterHandler);
        target.RegisterCallback<PointerOverEvent>(PointerOverHandler);
        target.RegisterCallback<PointerLeaveEvent>(PointerLeaveHandler);
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);

        if (_subElement != null)
        {
            _subElement.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            _subElement.RegisterCallback<PointerLeaveEvent>(PointerLeaveHandler);
            _subElement.RegisterCallback<PointerOverEvent>(PointerOverHandler);
        }
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerEnterEvent>(PointerEnterHandler);
        target.UnregisterCallback<PointerOverEvent>(PointerOverHandler);
        target.UnregisterCallback<PointerLeaveEvent>(PointerLeaveHandler);
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        if (_subElement != null)
        {
            _subElement.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            _subElement.UnregisterCallback<PointerLeaveEvent>(PointerLeaveHandler);
            _subElement.UnregisterCallback<PointerOverEvent>(PointerOverHandler);
        }
    }
}
