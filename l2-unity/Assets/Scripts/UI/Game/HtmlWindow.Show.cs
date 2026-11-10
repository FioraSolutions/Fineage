

public partial class HtmlWindow
{
    public void Show()
    {
        ApplyNormalWindowMode();
        RenderHtml(_normalHtml, "");
        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        yield return null;

        _windowEle.style.display = DisplayStyle.Flex;

        if (_mouseOverDetection != null)
            _mouseOverDetection.Enable();

        BringToFront();
    }

}
