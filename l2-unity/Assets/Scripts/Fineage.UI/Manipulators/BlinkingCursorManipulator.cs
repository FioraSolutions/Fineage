using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BlinkingCursorManipulator : PointerManipulator {
    private IVisualElementScheduledItem _scheduler;

    public BlinkingCursorManipulator(VisualElement target) {
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget() {
        _scheduler = target.schedule.Execute(() => {
            if (target.ClassListContains("transparent-cursor"))
                target.RemoveFromClassList("transparent-cursor");
            else
                target.AddToClassList("transparent-cursor");
        }).Every(500);
        _scheduler.Pause();

        target.RegisterCallback<FocusEvent>(OnFocusEvent);
        target.RegisterCallback<BlurEvent>(OnBlurEvent);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<FocusEvent>(OnFocusEvent);
        target.UnregisterCallback<BlurEvent>(OnBlurEvent);
        _scheduler.Pause();
    }

    void OnFocusEvent(FocusEvent evt) {
        target.RemoveFromClassList("transparent-cursor");
        _scheduler.Resume();
    }

    void OnBlurEvent(BlurEvent evt) {
        target.RemoveFromClassList("transparent-cursor");
        _scheduler.Pause();
    }
}
