
using UnityEngine;
using UnityEngine.UIElements;

public partial class TooltipManipulator : PointerManipulator
{
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
