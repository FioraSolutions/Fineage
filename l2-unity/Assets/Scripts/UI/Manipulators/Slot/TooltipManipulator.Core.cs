using UnityEngine.UIElements;
using UnityEngine;

public partial class TooltipManipulator : PointerManipulator
{
    public TooltipManipulator(VisualElement target, string text)
    {
        this.target = target;
        _text = text;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerEnterEvent>(PointerInHandler);
        target.RegisterCallback<MouseOverEvent>(PointerOverHandler);
        target.RegisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerEnterEvent>(PointerInHandler);
        target.UnregisterCallback<MouseOverEvent>(PointerOverHandler);
        target.UnregisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    protected virtual void OnPointerEnterTooltip(PointerEnterEvent evt)
    {
        if (target != null && HudCorePacketServices.ToolTip != null && _text.Length > 0)
            HudCorePacketServices.ToolTip.UpdateTooltip(_text, target);
    }

    private void PointerInHandler(PointerEnterEvent evt) => OnPointerEnterTooltip(evt);
}
