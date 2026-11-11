using System;
using System.Reflection.Emit;
using System.Xml;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

public partial class ToolTipSkill
{
    private Vector2 checkBound(Vector2 newPoint , float element)
    {
        return  new Vector2(newPoint.x, newPoint.y + element);
    }

    public void ResetPosition(Vector2 vector2)
    {
        _content.transform.position = vector2;
        SendToBack();
    }

    public void NewPosition(Vector2 newPoint, float sdfig, bool forceBelow)
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
