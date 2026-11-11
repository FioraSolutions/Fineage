

using NUnit.Framework.Internal;
using System;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract partial class L2Window
{
    public void OnCenterScreen(VisualElement root)
    {
        _isCenter = false;
        UpdateCenter(root);
    }

    private void UpdateCenter(VisualElement root)
    {
        if (_isReg == false)
        {
            _isReg = true;
            _windowEle.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (_isCenter) return;

                _isCenter = true;

                float width = _windowEle.resolvedStyle.width;
                float height = _windowEle.resolvedStyle.height;

                if(width == 0 & height == 0)
                {
                    width = defaultWidth;
                    height = defaultHeight;
                }


                float parentWidth = root.resolvedStyle.width;
                float parentHeight = root.resolvedStyle.height;

                _windowEle.style.left = (parentWidth - width) / 2;
                _windowEle.style.top = (parentHeight - height) / 2;

            });
        }
    
    }

    public void AddWindow(VisualElement root)
    {
        // Subclasses often override Awake without calling base; Start may also
        // not have run yet when L2UI.LoadUI attaches windows on first Update.
        // Coroutines require an active host (SystemMsgWindow was created mid-load inactive).
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        EnsureAssetsLoaded();
        if (_windowTemplate == null)
        {
            return;
        }
        StartCoroutine(BuildWindow(root));
    }

    protected virtual void InitWindow(VisualElement root)
    {
        try
        {
            _root = root;
            _windowEle = _windowTemplate.Instantiate()[0];
            _mouseOverDetection = new MouseOverDetectionManipulator(_windowEle);
            _windowEle.AddManipulator(_mouseOverDetection);
            DisableEventOnOver(_windowEle);
            if (_isWindowHidden)
            {
                _windowEle.style.display = DisplayStyle.None;
                _mouseOverDetection.Disable();
            }

            root.Add(_windowEle);
        }
        catch (Exception)
        {
            // Debug.LogException(ex);
        }
 
    }

    protected void OnDropdownPointer(PointerDownEvent evt)
    {
        if (_listDropDown == null || _listDropDown.Count == 0)
        {
            evt.PreventDefault();
            evt.StopImmediatePropagation();
        }
    }

    protected void DisableEventOnOver(VisualElement _windowEle)
    {
        var dropdowns = _windowEle.Query<DropdownField>().ToList();

        if(dropdowns != null)
        {
            foreach (var dropdown in dropdowns)
            {
                DisablePointerEvents(dropdown);

                dropdown.RegisterCallback<PointerMoveEvent>(OnMoveEvent);
                dropdown.RegisterCallback<FocusOutEvent>(evt => OnMouseOutFocus(evt ,  dropdown));
                dropdown.RegisterCallback<PointerLeaveEvent>(OnLeaveEvent);
                dropdown.RegisterCallback<PointerDownEvent>(evt => ClickDropdownElement(evt, dropdown));

            }
        }
    
    }

    private void ClickDropdownElement(PointerDownEvent evt, DropdownField dropdown)
    {
        _IsCloseCreate = true;
    }

    private void OnClose(bool check)
    {
        onClose -= OnClose;
        SetUnBlockStatusMouseOver();
        _IsCloseCreate = false;
    }

    private void OnMoveEvent(PointerMoveEvent evt)
    {
       SetBlockStatusMouseOver();
    }

    private void OnMouseOutFocus(FocusOutEvent evt, DropdownField dropdown)
    {
        onClose += OnClose;
        _IsCloseCreate = false;
        dropDownUtils.SubscribeToDropdownOpenClose(dropdown.panel.visualTree, onOpen, onClose , ref _IsCloseCreate);
    }

    private void OnLeaveEvent(PointerLeaveEvent evt)
    {

        if (_IsCloseCreate == false)
        {
            SetUnBlockStatusMouseOver();
        }

    }

    private void DisablePointerEvents(VisualElement element)
    {
        element.RegisterCallback<PointerOverEvent>(evt => evt.StopPropagation());
        element.RegisterCallback<PointerOutEvent>(evt => evt.StopPropagation());
    }

}
