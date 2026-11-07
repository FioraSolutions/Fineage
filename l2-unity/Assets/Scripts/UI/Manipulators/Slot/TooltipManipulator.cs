

using UnityEngine;
using UnityEngine.UIElements;

public class TooltipManipulator : PointerManipulator
{

    private string _text;
    private bool _pointerOver;

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

    private void PointerOverHandler(MouseOverEvent evt)
    {
        _pointerOver = true;
    }

    private void PointerOutHandler(PointerOutEvent evt)
    {
        if(target != null & HudCorePacketServices.ToolTip != null)
        {
            _pointerOver = false;
            if (_text.Length > 0)
                HudCorePacketServices.ToolTip.HideWindow(target);
        }
        else
        {
            // Debug.Log("TooltipManipulator: PointerOutHandler не критическая ошибка!");
        }
     
    }

    public void SetText(string text)
    {
        _text = text;
    }

    public void Clear()
    {
        if (_pointerOver)
        {
            if(HudCorePacketServices.ToolTip != null)
            {
                HudCorePacketServices.ToolTip.HideWindow(target);
            }
            else
            {
                // Debug.Log("TooltipManipulator: Clear не критическая ошибка!");
            }
            
        }
    }
}
