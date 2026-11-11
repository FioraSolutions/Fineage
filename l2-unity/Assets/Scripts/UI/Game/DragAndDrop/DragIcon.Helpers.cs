using System;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public partial class DragIcon : L2PopupWindow
{
    public void SetBackground(Background baackground)
    {
        icon.style.backgroundImage = baackground;
    }

    public void NewPosition(Vector2 position)
    {
        if(position != null)
        {
            if (!_isShow)
            {
                ShowWindow();
                _isShow = true;
            }
            icon.transform.position = position;
        }
    }

    public void ResetPosition()
    {
        icon.transform.position = Vector2.zero;
        if (_isShow)
        {
            _isShow = false;
            HideWindow();
        }
        
    }

    public void BringToFront1()
    {
        BringToFront();
    }

    public void RegisterClickWindow(VisualElement contentId)
    {

        if (contentId == null)
        {
            // Debug.LogError(contentId + " can't be found.");
            return;
        }

        contentId.RegisterCallback<MouseDownEvent>(evt => {
            //Debug.Log("Evennt cliiikkk 1");
            BringToFront();

        }, TrickleDown.TrickleDown);

    }
}
