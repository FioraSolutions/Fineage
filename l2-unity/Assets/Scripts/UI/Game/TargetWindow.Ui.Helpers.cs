using System;

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TargetWindow : L2Window
{
    private void TooogleHide(bool isHideHpBar)
    {
        if (isHideHpBar){
            HideBar();
        }
        else{
            ShowBar();
        }
    }

    private void HideBar()
    {
        _HPBarBG.style.display = DisplayStyle.None;
        _HPBar.style.display = DisplayStyle.None;
    }

    private void ShowBar()
    {
        _HPBarBG.style.display = DisplayStyle.Flex;
        _HPBar.style.display = DisplayStyle.Flex;
    }
}
