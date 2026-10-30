using UnityEngine;
using UnityEngine.UIElements;

public abstract class L2UI : MonoBehaviour
{
    private VisualElement _rootElement;

    [SerializeField] private bool _uiLoaded = false;
    [SerializeField] private Focusable _focusedElement;
    [SerializeField] private bool _mouseOverUI = false;
    [SerializeField] private VisualElement _loadingElement;

    protected VisualElement _rootVisualContainer;
    protected VisualElement _popupVisualContainer;
    protected VisualElement _tooltipVisualContainer;
    protected VisualElement _slotVisualContainer;

    public bool MouseOverUI { get { return _mouseOverUI; } set { _mouseOverUI = value; } }
    public bool UILoaded { get { return _uiLoaded; } set { _uiLoaded = value; } }
    public VisualElement RootElement { get { return _rootElement; } }


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

        if (_uiLoaded)
        {
            _focusedElement = _rootElement.focusController.focusedElement;
            HealStuckMouseOver();
        }
    }

    /// <summary>
    /// MouseOverUI is a global flag toggled by enter/leave manipulators. When a window is hidden
    /// or rebuilt under the cursor, PointerLeave never fires and the flag sticks true, killing all
    /// world clicks (targeting/move) until restart. Re-validate against a real panel pick.
    /// </summary>
    void HealStuckMouseOver()
    {
        if (!_mouseOverUI || _rootElement == null || _rootElement.panel == null)
            return;
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            return;
        if (!IsPointerOverPanel())
            _mouseOverUI = false;
    }

    /// <summary>Fresh check: is the cursor over a real (non-background) UI Toolkit element right now.</summary>
    public bool IsPointerOverPanel()
    {
        if (_rootElement == null || _rootElement.panel == null)
            return false;
        Vector3 m = Input.mousePosition;
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

    protected virtual void LoadUI()
    {
        _rootVisualContainer = _rootElement.Q<VisualElement>("UIContainer");
        _popupVisualContainer = _rootElement.Q<VisualElement>("UIContainerPopup");
        _tooltipVisualContainer = _rootElement.Q<VisualElement>("UIContainerTooltip");
        _slotVisualContainer = _rootElement.Q<VisualElement>("UIContainerSlot");
        _loadingElement = _rootElement.Q<VisualElement>("Loading");
    }

    public bool IsWindowContain(Vector2 vector2)
    {
        return _rootVisualContainer.worldBound.Contains(vector2);
    }

    public void StartLoading()
    {
        if (_loadingElement != null)
        {
            _loadingElement.style.display = DisplayStyle.Flex;
        }
    }

    public void StopLoading()
    {
        if (_loadingElement != null)
        {
            _loadingElement.style.display = DisplayStyle.None;
        }
    }

    public void SetHudVisible(bool visible)
    {
        if (_rootElement == null)
        {
            UIDocument document = GetComponent<UIDocument>();
            if (document != null)
            {
                _rootElement = document.rootVisualElement;
            }
        }

        if (_rootElement == null)
        {
            return;
        }

        _rootElement.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
        _rootElement.pickingMode = visible ? PickingMode.Position : PickingMode.Ignore;
    }
}
