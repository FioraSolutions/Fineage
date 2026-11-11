using UnityEngine;
using UnityEngine.UIElements;

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

    public bool IsPointerOverPanel()
    {
        if (_rootElement == null || _rootElement.panel == null)
            return false;
        Vector2 m = PollInput.MousePosition;
        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(_rootElement.panel, new Vector2(m.x, Screen.height - m.y));
        VisualElement picked = _rootElement.panel.Pick(panelPos);
        return picked != null && !IsBackgroundContainer(picked);
    }

    bool IsBackgroundContainer(VisualElement ve)
    {
        if (ve == null) return true;

        // Loading overlay / layout spacers must never swallow world input.
        // pick=UI-container/UIContainer/Spacer was blocking click-to-move and WASD.
        for (VisualElement p = ve; p != null; p = p.parent)
        {
            if (p == _loadingElement || p.name == "Loading")
                return true;
            if (p.name == "Spacer"
                && (p.parent == _rootVisualContainer
                    || (p.parent != null && (p.parent.name == "UIContainer" || p.parent.name == "UI-container"))))
                return true;
            if (p == _rootElement)
                break;
        }

        // Full-screen transparent wrappers (window roots/containers) must not swallow world clicks.
        Rect pb = _rootElement.worldBound;
        Rect b = ve.worldBound;
        if (pb.width > 0 && pb.height > 0 && b.width >= pb.width * 0.9f && b.height >= pb.height * 0.9f)
            return true;
        return ve == _rootElement
            || ve == _rootVisualContainer
            || ve == _popupVisualContainer
            || ve == _tooltipVisualContainer
            || ve == _slotVisualContainer
            || ve == _rootElement.parent;
    }

    public void BlurFocus()
    {
        if (_focusedElement != null)
        {
            _focusedElement.Blur();
        }
    }
}
