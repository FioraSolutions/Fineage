using System;

public partial class L2ToolTip : L2PopupWindow
{
    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Template/Tooltip");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _title = GetLabelById("Title");
    }

    public void UpdateTooltip(string title, VisualElement target)
    {
        _windowEle.style.left = -1000;
        _windowEle.style.opacity = 0;

        _tooltipTarget = target;

        ShowWindow();

        if (_updateStyleCoroutine != null)
        {
            StopCoroutine(_updateStyleCoroutine);
        }

        _updateStyleCoroutine = StartCoroutine(UpdateToolTipCoroutine(title, target));
    }

    IEnumerator UpdateToolTipCoroutine(string title, VisualElement target)
    {
        while (true)
        {
            _title.text = title;

            yield return new WaitForEndOfFrame();

            _windowEle.style.left = target.worldBound.x;
            _windowEle.style.top = target.worldBound.y - _windowEle.resolvedStyle.height;

            _windowEle.style.opacity = 1;
        }
    }

    public void HideWindow(VisualElement exitElement)
    {
        if (exitElement == _tooltipTarget)
        {
            base.HideWindow();

            if (_updateStyleCoroutine != null)
            {
                StopCoroutine(_updateStyleCoroutine);
            }
        }
    }
}
