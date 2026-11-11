using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillbarWindow
{
    public void ApplyPosition()
    {
        if (_windowEle == null)
            return;

        _windowEle.style.translate = StyleKeyword.Null;
        _windowEle.style.right = StyleKeyword.Null;

        float parentWidth = ResolveParentWidth();
        float parentHeight = ResolveParentHeight();
        float barWidth = _windowEle.resolvedStyle.width;
        if (barWidth <= 1f)
            barWidth = 500f;
        float barHeight = _windowEle.resolvedStyle.height;
        if (barHeight <= 1f)
            barHeight = 46f;

        if (PlayerPrefs.HasKey(PrefLeft) && PlayerPrefs.HasKey(PrefTop))
        {
            float left = PlayerPrefs.GetFloat(PrefLeft);
            float top = PlayerPrefs.GetFloat(PrefTop);
            // Stale prefs (e.g. top past panel bottom after resize) hide the bar entirely.
            if (IsOnScreen(left, top, barWidth, barHeight, parentWidth, parentHeight))
            {
                _windowEle.style.bottom = StyleKeyword.Null;
                _windowEle.style.left = left;
                _windowEle.style.top = top;
                return;
            }

            PlayerPrefs.DeleteKey(PrefLeft);
            PlayerPrefs.DeleteKey(PrefTop);
            PlayerPrefs.Save();
        }

        DockDefault(parentWidth, barWidth);
    }

    static bool IsOnScreen(float left, float top, float barWidth, float barHeight, float parentWidth, float parentHeight)
    {
        if (float.IsNaN(left) || float.IsNaN(top) || parentWidth <= 1f || parentHeight <= 1f)
            return false;
        // Keep at least half the bar inside the panel so it stays findable/draggable.
        float visibleW = Mathf.Min(left + barWidth, parentWidth) - Mathf.Max(left, 0f);
        float visibleH = Mathf.Min(top + barHeight, parentHeight) - Mathf.Max(top, 0f);
        return visibleW >= barWidth * 0.5f && visibleH >= barHeight * 0.5f;
    }

    void DockDefault(float parentWidth, float barWidth)
    {
        _windowEle.style.left = Mathf.Max(0f, parentWidth - MenuDockWidth - barWidth);
        _windowEle.style.top = StyleKeyword.Null;
        _windowEle.style.bottom = 0;
    }
}
