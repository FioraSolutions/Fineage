using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class L2UI : MonoBehaviour
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



    /// <summary>
    /// MouseOverUI is a global flag toggled by enter/leave manipulators. When a window is hidden
    /// or rebuilt under the cursor, PointerLeave never fires and the flag sticks true, killing all
    /// world clicks (targeting/move) until restart. Re-validate against a real panel pick.
    /// </summary>

    /// <summary>Fresh check: is the cursor over a real (non-background) UI Toolkit element right now.</summary>







}
