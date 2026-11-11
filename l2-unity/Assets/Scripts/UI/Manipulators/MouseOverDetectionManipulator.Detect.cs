using UnityEngine;
using UnityEngine.UIElements;

public partial class MouseOverDetectionManipulator : PointerManipulator
{
    private bool IsVisibleAndContains(Vector2 posScreen, VisualElement el)
    {

        if (el == null) return false;
        var panel = el.panel;
        if (panel == null) return false;

 
        Vector2 panelPos = new Vector2(posScreen.x, Screen.height - posScreen.y);


        if (el.worldBound.Contains(posScreen))
            return true;

        var picked = panel.Pick(panelPos); 
        if (picked == null) return false;

        return picked == el || el.Contains(picked);
    }

    private void PointerEnterHandler(PointerEnterEvent evt)
    {
        //Debug.Log("PointerLeaveHandler> enter enter");
        if (_enabled && (_subElement == null || !IsVisibleAndContains(evt.position, _subElement)))
        {
            //Debug.Log("PointerLeaveHandler> enter enter block");
            Block();
        }

    }

    private void PointerOverHandler(PointerOverEvent evt)
    {
        //Debug.Log("PointerLeaveHandler> over enter");
        if (_enabled && (_subElement == null || !IsVisibleAndContains(evt.position, _subElement)))
        {
            //Debug.Log("PointerLeaveHandler> over enter block");
            Block();
        }

        else if (_enabled && _subElement != null && IsVisibleAndContains(evt.position, _subElement))
        {
           // Debug.Log("PointerLeaveHandler> over enter block");
            Block();
        }

    }

    private void PointerLeaveHandler(PointerLeaveEvent evt)
    {
       
        //Debug.Log("PointerLeaveHandler> leave enter");
        //if(_subElement != null)
        //{
          //  Debug.Log("name level 1 subelement " + _subElement.name + "Reuslt " + IsVisibleAndContains(evt.position, _subElement) + " evt " + evt.target.ToString());
         //   Debug.Log("name level 2 target element " + target.name + " Reuslt " + IsVisibleAndContains(evt.position, target) + " evt " + evt.target.ToString());
        //}

        if (_isSelectDropfield == true) return;
        if (!_enabled) return;

        Vector2 mouse = PollInput.MousePosition;
        if (!IsVisibleAndContains(mouse, target) && !IsVisibleAndContains(mouse, _subElement))
        {
            UnBlock();
            //Debug.Log("PointerLeaveHandler> leave unblock");
        }

    }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        //Debug.Log("name 1 subelement " + _subElement.name + "Reuslt " + IsVisibleAndContains(evt.position, _subElement));
        //Debug.Log("name 2 target element " + target.name + " Reuslt " + IsVisibleAndContains(evt.position, target)); 

        //Debug.Log("name 1 subelement new " + _subElement.name + "Reuslt " + IsVisibleAndContains(PollInput.MousePosition, _subElement));
        //Debug.Log("name 1 subelement new " + _subElement.name + "Reuslt " + IsVisibleAndContains(PollInput.MousePosition, target));

        Vector2 mouse = PollInput.MousePosition;
        if (_enabled && (IsVisibleAndContains(mouse, _subElement) || IsVisibleAndContains(mouse, target)))
            Block();
        else
        {
            //Debug.Log("PointerDownHandler> leave1 unblock");
            UnBlock();
        }

    }

    public void SetBlockDropfieldStatus()
    {
        _isSelectDropfield = true;
        Block();
    }

    public void SetUnBlockDropfieldStatus()
    {
        _isSelectDropfield = false;
        UnBlock();
    }

    private void Block()
    {
        _ui.MouseOverUI = true;
        _overThisManipulator = true;
    }

    private void UnBlock()
    {
        _ui.MouseOverUI = false;
        _overThisManipulator = false;
        //Debug.Log("PointerLeaveHandler> leave1 unblock root");
    }
}
