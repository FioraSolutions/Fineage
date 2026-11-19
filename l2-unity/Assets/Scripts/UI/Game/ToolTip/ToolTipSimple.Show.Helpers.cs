using System;
using System.Collections;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections.Generic;
using UnityEngine;

public partial class ToolTipSimple : L2PopupWindow, IToolTips
{
    private void SetIdElement(VisualElement item, int position , int type)
    {
        item.name = position + "_"+ type.ToString();
    }

    private IEnumerator EndNewPosition(VisualElement ve)
    {
        ForceHide();
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator AllHide()
    {
        ForceHide();
        yield return new WaitForEndOfFrame();
    }

    public void ForceHide()
    {
        TooltipLifecycle.Unwatch(ForceHide);
        if (_windowEle != null)
            _windowEle.style.display = DisplayStyle.None;
        if (_selectShow != null && _showToolTip != null)
            _showToolTip.Hide(_selectShow);
        _selectShow = null;
    }

    private IEnumerator StartNewPosition(VisualElement ve)
    {
        string[] ids = GetUniquePosition(ve);
        TemplateContainer template = SwitchToolTip(ids , false);
        if(template != null)
        {
            _windowEle.style.display = DisplayStyle.Flex;
            _contentInside.MarkDirtyRepaint();

            AddData(ids, template);
            bool isBelow = GetForceBelow(ids);
            _selectShow = ve;
            TooltipLifecycle.WatchSource(ve, ForceHide);
            //esle position layout != 0 <--- (EndNewPosition add new Vector2(0,0))
            //This means the layout did not return to the base, most likely this is an error. Restarting the transition to a new position
            //if (_windowEle.worldBound.height != 0)
            if (_contentInside.worldBound.height != 0)
            {
                _showToolTip.Show(_selectShow , isBelow);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
