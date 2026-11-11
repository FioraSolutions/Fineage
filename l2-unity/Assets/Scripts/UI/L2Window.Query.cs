

using NUnit.Framework.Internal;
using System;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract partial class L2Window
{
    protected Label GetLabelById(string id)
    {
        VisualElement e = GetElementById(id);
        if (e == null)
        {
            return null;
        }

        return (Label)e;
    }

    protected VisualElement GetElementById(string id)
    {
        var btn = _windowEle.Q<VisualElement>(id);
        if (btn == null)
        {
            // Debug.LogError(id + " can't be found.");
            return null;
        }

        return btn;
    }

    public VisualElement GetElementByClass(string className)
    {
        var btn = _windowEle.Q<VisualElement>(null, className);
        if (btn == null)
        {
            // Debug.LogError(className + " can't be found.");
            return null;
        }

        return btn;
    }

    public bool MouseOverThisWindow()
    {
        return _mouseOverDetection.MouseOver;
    }

    protected void SetEnableMouseOver(bool isBlock)
    {
        _mouseOverDetection.SetBlock(isBlock);
    }

    protected void SetBlockStatusMouseOver()
    {
        _mouseOverDetection.SetBlockDropfieldStatus();
    }

    protected void SetUnBlockStatusMouseOver()
    {
        _mouseOverDetection.SetUnBlockDropfieldStatus();
    }

}
