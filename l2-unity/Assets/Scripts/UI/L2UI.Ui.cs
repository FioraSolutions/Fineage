using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class L2UI : MonoBehaviour
{
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
