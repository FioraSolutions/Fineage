using UnityEngine;
using UnityEngine.UIElements;
using System;

public abstract partial class L2UI : MonoBehaviour
{
    protected virtual void Update()
    {
        if (_rootElement != null && !float.IsNaN(_rootElement.resolvedStyle.width) && _uiLoaded == false)
        {
            try
            {
                LoadUI();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                // Always mark loaded so a one-shot failure doesn't spam every frame.
                _uiLoaded = true;
            }
        }
        else
        {
            var doc = GetComponent<UIDocument>();
            if (doc != null)
                _rootElement = doc.rootVisualElement;
        }

        if (_uiLoaded && _rootElement != null && _rootElement.focusController != null)
        {
            _focusedElement = _rootElement.focusController.focusedElement;
            HealStuckMouseOver();
        }
    }

    void HealStuckMouseOver()
    {
        if (!_mouseOverUI || _rootElement == null || _rootElement.panel == null)
            return;
        if (PollInput.LeftMouseHeld || PollInput.RightMouseHeld)
            return;
        if (!IsPointerOverPanel())
            _mouseOverUI = false;
    }
}
