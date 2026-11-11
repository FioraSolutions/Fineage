using UnityEngine;
using UnityEngine.UIElements;

public partial class CreateScroller
{
    protected void AdjustScrollValue(ScrollView scrollView, Scroller scroller, int direction)
    {
        if (scrollView == null || scroller == null) return;

        float contentHeight = scrollView.contentContainer.worldBound.height;
        float viewportHeight = scrollView.worldBound.height;

        if (contentHeight <= viewportHeight) return; // No need to scroll if content fits in viewport

        float scrollRange = contentHeight - viewportHeight;
        float stepSize = _scrollStepSize / scrollRange;
        float newValue = direction * (scroller.value + stepSize) * scroller.highValue;
        scroller.value = Mathf.Clamp(newValue, 0, scroller.highValue);
    }

    private void VerifyScrollValue(Scroller scroller)
    {
        if (scroller.highValue > 0 && scroller.value == scroller.highValue || scroller.highValue == 0 && scroller.value == 0)
        {
            _autoscroll = true;
        }
        else
        {
            _autoscroll = false;
        }
    }
}
