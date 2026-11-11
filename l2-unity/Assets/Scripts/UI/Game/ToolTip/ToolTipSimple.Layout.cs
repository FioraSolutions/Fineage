using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    private string[] GetUniquePosition(VisualElement ve)
    {
        return  ve.name.Split('_');
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (evt.newRect.height == 0)
            return;

        if (_contentInside != null)
        {
            _heightContent = _contentInside.worldBound.height;
            _widtchContent = _contentInside.worldBound.width;
            _showToolTip.Show(_selectShow , _isForceBelow);
        }

    }

    public void NewPosition(Vector2 newPoint, float sdfig , bool isForceBelow)
    {
        _isForceBelow = isForceBelow;
        var highest = _calcPosition.HighestPoint(newPoint, _heightContent);
        bool insideRoot = SystemUIPacketServices.Ui.IsWindowContain(highest);
        Vector2 reversePoint = _calcPosition.GetNewPosition(newPoint, sdfig, _heightContent , insideRoot , isForceBelow);
        _content.transform.position = reversePoint;
        BringToFront();
    }

}
