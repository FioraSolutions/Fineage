

public abstract partial class L2Window
{
    public virtual void HideWindow()
    {
        try
        {
            _isWindowHidden = true;
            _windowEle.style.display = DisplayStyle.None;
            _mouseOverDetection.Disable();
        }
        catch (Exception)
        {
            // Debug.LogException(ex);
        }

    }

    public virtual void OnlyHideWindow()
    {
        _windowEle.style.display = DisplayStyle.None;
    }

    public virtual void ShowWindow()
    {
        try
        {
            _isWindowHidden = false;
            _windowEle.style.display = DisplayStyle.Flex;
            _mouseOverDetection.Enable();
        }
        catch (Exception)
        {
            // Debug.LogException(ex);
        }

    }

    public virtual void ToggleHideWindow()
    {
        if (_isWindowHidden)
        {
            ShowWindow();
        }
        else
        {
            HideWindow();
        }
    }

    public virtual void BringToFront()
    {
    }

    public virtual void SendToBack()
    {
    }

}
