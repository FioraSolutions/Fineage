using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ToolTipSkill : L2PopupWindow, IToolTips
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _showToolTip = new ShowToolTip(this);
        }
        else
        {
            Destroy(this);
        }
    }
}
