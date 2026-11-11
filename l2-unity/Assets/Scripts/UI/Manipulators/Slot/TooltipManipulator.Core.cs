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

    private void PointerInHandler(PointerEnterEvent evt)
    {
        if (target != null & HudCorePacketServices.ToolTip != null) {
            if (_text.Length > 0)
                HudCorePacketServices.ToolTip.UpdateTooltip(_text, target);
        }
        else
        {
            // Debug.Log("TooltipManipulator: PointerInHandler не критическая ошибка!");
        }

    }
}
