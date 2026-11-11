using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class L2Tab
{
    public virtual void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader , bool initEmpty , bool isSwitchTab)
    {
        _windowEle = chatWindowEle;

        if (tabContainer != null)
        {
            _tabContainer = tabContainer;
            _tabHeader = tabHeader;
            _scrollView = tabContainer.Q<ScrollView>("ScrollView");

            if(_scrollView != null)
            {
                _scroller = _scrollView.verticalScroller;
            }
            

            tabHeader.AddManipulator(new ButtonClickSoundManipulator(tabHeader));
            //use inventory window
            //no use dealer window
            if (isSwitchTab)
            {
                tabHeader.RegisterCallback<MouseDownEvent>(evt => {
                    OnSwitchTab();
                }, TrickleDown.TrickleDown);
            }


            RegisterAutoScrollEvent();
            RegisterPlayerScrollEvent();
        }
    }

    protected void AdjustScrollValue(int direction)
    {
        if (_scrollView == null || _scroller == null) return;

        float contentHeight = _scrollView.contentContainer.worldBound.height;
        float viewportHeight = _scrollView.worldBound.height;

        if (contentHeight <= viewportHeight) return; // No need to scroll if content fits in viewport

        float scrollRange = contentHeight - viewportHeight;
        float stepSize = _scrollStepSize / scrollRange;
        float newValue = direction * (_scroller.value + stepSize) * _scroller.highValue;
        _scroller.value = Mathf.Clamp(newValue, 0, _scroller.highValue);
    }

    protected virtual void OnSwitchTab() { }

    protected virtual void RegisterAutoScrollEvent() { }

    private void RegisterPlayerScrollEvent()
    {
        if (_scroller != null)
        {
            var highBtn = _scroller.Q<RepeatButton>("unity-high-button");
            var lowBtn = _scroller.Q<RepeatButton>("unity-low-button");
            var dragger = _scroller.Q<VisualElement>("unity-drag-container");

            highBtn.RegisterCallback<MouseUpEvent>(evt => {
                AdjustScrollValue(1);
                VerifyScrollValue();
            });
            lowBtn.RegisterCallback<MouseUpEvent>(evt => {
                AdjustScrollValue(-1);
                VerifyScrollValue();
            });

            highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
            lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));

            dragger.RegisterCallback<MouseUpEvent>(evt => {
                VerifyScrollValue();
            });

            dragger.RegisterCallback<WheelEvent>(evt => {
                VerifyScrollValue();
            });

            _windowEle.RegisterCallback<GeometryChangedEvent>(evt => {
                OnGeometryChanged();
            });

            _scrollView.RegisterCallback<WheelEvent>(evt => {
                int direction = evt.delta.y > 0 ? 1 : -1;
                AdjustScrollValue(direction);
                VerifyScrollValue();
                evt.StopPropagation();
            });
        }
        
    }
}
