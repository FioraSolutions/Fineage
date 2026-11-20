using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// General tooltip owner lifecycle: hide when the hovered element is destroyed, detached,
/// or hidden, or when the window that owns it is closed / display:none.
/// Not tied to a specific window (inventory, warehouse, …).
/// </summary>
public static class TooltipLifecycle
{
    struct Watch
    {
        public VisualElement Source;
        public Action Hide;
    }

    static readonly List<Watch> Watches = new List<Watch>();

    public static void WatchSource(VisualElement source, Action hide)
    {
        if (hide == null)
            return;
        Unwatch(hide);
        if (source != null)
            source.RegisterCallback<DetachFromPanelEvent>(OnDetach);
        Watches.Add(new Watch { Source = source, Hide = hide });
    }

    public static void Unwatch(Action hide)
    {
        for (int i = Watches.Count - 1; i >= 0; i--)
        {
            if (Watches[i].Hide != hide)
                continue;
            VisualElement source = Watches[i].Source;
            if (source != null)
                source.UnregisterCallback<DetachFromPanelEvent>(OnDetach);
            Watches.RemoveAt(i);
        }
    }

    public static void HideIfOwnerClosed(VisualElement windowRoot)
    {
        if (windowRoot == null)
            return;
        InvokeMatching(source => source == null || IsUnder(windowRoot, source) || !IsInteractable(source));
    }

    public static void HideIfSourceGone()
    {
        InvokeMatching(source => !IsInteractable(source));
    }

    public static bool IsInteractable(VisualElement element)
    {
        if (element == null || element.panel == null)
            return false;
        for (VisualElement p = element; p != null; p = p.parent)
        {
            if (!p.visible)
                return false;
            if (p.resolvedStyle.display == DisplayStyle.None)
                return false;
            if (p.resolvedStyle.visibility == Visibility.Hidden)
                return false;
        }
        return true;
    }

    static void OnDetach(DetachFromPanelEvent evt)
    {
        VisualElement target = evt.currentTarget as VisualElement;
        InvokeMatching(source => source == null || source == target);
    }

    static void InvokeMatching(Func<VisualElement, bool> match)
    {
        for (int i = Watches.Count - 1; i >= 0; i--)
        {
            Watch watch = Watches[i];
            if (!match(watch.Source))
                continue;
            if (watch.Source != null)
                watch.Source.UnregisterCallback<DetachFromPanelEvent>(OnDetach);
            Watches.RemoveAt(i);
            watch.Hide?.Invoke();
        }
    }

    static bool IsUnder(VisualElement root, VisualElement element)
    {
        for (VisualElement p = element; p != null; p = p.parent)
        {
            if (p == root)
                return true;
        }
        return false;
    }
}
