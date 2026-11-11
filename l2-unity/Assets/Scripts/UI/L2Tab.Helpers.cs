using UnityEngine.UIElements;
using UnityEngine;

public abstract partial class L2Tab
{
    protected virtual void OnGeometryChanged() { }

    private void VerifyScrollValue()
    {
        if (_scroller.highValue > 0 && _scroller.value == _scroller.highValue || _scroller.highValue == 0 && _scroller.value == 0)
        {
            _autoscroll = true;
        }
        else
        {
            _autoscroll = false;
        }
    }

    public virtual void SelectSlot(int slot) { }

    public void SetTabName(string tabName)
    {
        _tabName = tabName;
    }

    public void SetAutoScroll(bool autoscroll)
    {
        _autoscroll = autoscroll;
    }
}
