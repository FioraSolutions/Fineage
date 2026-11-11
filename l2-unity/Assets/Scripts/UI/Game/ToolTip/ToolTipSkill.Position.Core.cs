using System;
using System.Reflection.Emit;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public partial class ToolTipSkill
{
    private void CalcNewPosition(VisualElement ve)
    {
        _windowEle.style.display = DisplayStyle.Flex;
        
        _showToolTip.Show(ve , false);
    }

    private void ResetElement(VisualElement ve)
    {
        _heightContent = _windowEle.worldBound.height;
        _showToolTip.Hide(ve);
        _windowEle.style.display = DisplayStyle.None;
    }

    public void NewPosition(Vector2 newPoint , float sdfig)
    {

        var testPoint = checkBound(newPoint, _heightContent);
        if (!SkillLearnPacketServices.SkillList.IsWindowContain(testPoint))
        {
            float width = _heightContent;
            float newddfig = width;
            //2px border 
            float sdfig1 = sdfig + 2;
            float new_y = newPoint.y - newddfig;
            float new_y2 = new_y - sdfig1;
            Vector2 reversePoint = new Vector2(newPoint.x, new_y2);
            _content.transform.position = reversePoint;
        }
        else
        {
            //2px border
            float new_y = newPoint.y + 2;
            Vector2 reversePoint = new Vector2(newPoint.x, new_y);
            _content.transform.position = reversePoint;
        }
        BringToFront();
    }
}
