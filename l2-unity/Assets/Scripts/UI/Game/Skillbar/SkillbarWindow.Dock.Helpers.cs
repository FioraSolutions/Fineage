using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using VContainer;

public partial class SkillbarWindow
{
    float ResolveParentWidth()
    {
        if (_windowEle.parent != null && _windowEle.parent.resolvedStyle.width > 1f)
            return _windowEle.parent.resolvedStyle.width;
        return Screen.width;
    }

    float ResolveParentHeight()
    {
        if (_windowEle.parent != null && _windowEle.parent.resolvedStyle.height > 1f)
            return _windowEle.parent.resolvedStyle.height;
        return Screen.height;
    }

    public void PersistPosition()
    {
        if (_windowEle == null)
            return;

        float left = _windowEle.resolvedStyle.left;
        float top = _windowEle.resolvedStyle.top;
        if (float.IsNaN(left) || float.IsNaN(top))
            return;

        float parentWidth = ResolveParentWidth();
        float parentHeight = ResolveParentHeight();
        float barWidth = _windowEle.resolvedStyle.width > 1f ? _windowEle.resolvedStyle.width : 500f;
        float barHeight = _windowEle.resolvedStyle.height > 1f ? _windowEle.resolvedStyle.height : 46f;
        if (!IsOnScreen(left, top, barWidth, barHeight, parentWidth, parentHeight))
            return;

        PlayerPrefs.SetFloat(PrefLeft, left);
        PlayerPrefs.SetFloat(PrefTop, top);
        PlayerPrefs.Save();
    }
}
