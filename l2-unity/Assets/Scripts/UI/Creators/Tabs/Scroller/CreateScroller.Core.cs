using UnityEngine;
using UnityEngine.UIElements;

public partial class CreateScroller
{
    public void Start(VisualElement _tabContainer)
    {
        if (_tabContainer != null)
        {

            ScrollView scrollView = _tabContainer.Q<ScrollView>("ScrollView");

            if (scrollView != null)
            {
                Scroller scroller = scrollView.verticalScroller;
                RegisterPlayerScrollEvent(scrollView, scroller);
            }
        }
    }

    public void RegisterPlayerScrollEvent(ScrollView scrollView, Scroller scroller)
    {
        if (scroller != null)
        {
            var highBtn = scroller.Q<RepeatButton>("unity-high-button");
            var lowBtn = scroller.Q<RepeatButton>("unity-low-button");
            var dragger = scroller.Q<VisualElement>("unity-drag-container");

            highBtn.RegisterCallback<MouseUpEvent>(evt => {
                AdjustScrollValue(scrollView, scroller, 1);
                VerifyScrollValue(scroller);
            });
            lowBtn.RegisterCallback<MouseUpEvent>(evt => {
                AdjustScrollValue(scrollView, scroller, -1);
                VerifyScrollValue(scroller);
            });

            highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
            lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));

            dragger.RegisterCallback<MouseUpEvent>(evt => {
                VerifyScrollValue(scroller);
            });

            dragger.RegisterCallback<WheelEvent>(evt => {
                VerifyScrollValue(scroller);
            });

            // _windowEle.RegisterCallback<GeometryChangedEvent>(evt => {
            //     OnGeometryChanged();
            // });

            scrollView.RegisterCallback<WheelEvent>(evt => {
                int direction = evt.delta.y > 0 ? 1 : -1;
                AdjustScrollValue(scrollView, scroller, direction);
                VerifyScrollValue(scroller);
                evt.StopPropagation();
            });
        }

    }
}
