using UnityEngine;
using UnityEngine.UIElements;

public partial class MouseOverDetectionManipulator : PointerManipulator
{
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
