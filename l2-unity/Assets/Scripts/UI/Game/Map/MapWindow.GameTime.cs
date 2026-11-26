using UnityEngine;
using UnityEngine.UIElements;

public partial class MapWindow
{
    WorldClock _subscribedClock;

    void SubscribeGameTimeClock()
    {
        UnsubscribeGameTimeClock();
        _subscribedClock = WorldClockPacketServices.Clock ?? WorldClock.EnsurePersistent();
        if (_subscribedClock == null)
        {
            Debug.LogError("[MapWindow] Cannot subscribe to game time: WorldClock missing.");
            return;
        }

        _subscribedClock.GameTimeLabelChanged += OnGameTimeLabelChanged;
        RefreshMinimapGameTime();
    }

    void UnsubscribeGameTimeClock()
    {
        if (_subscribedClock == null)
            return;
        _subscribedClock.GameTimeLabelChanged -= OnGameTimeLabelChanged;
        _subscribedClock = null;
    }

    void OnGameTimeLabelChanged(int hour24, int minute) => ApplyMinimapGameTime(hour24, minute);

    public void RefreshMinimapGameTime()
    {
        WorldClock clock = _subscribedClock ?? WorldClockPacketServices.Clock;
        if (clock == null || !clock.TryGetMobiusGameHourMinute(out int hour, out int minute))
            return;
        ApplyMinimapGameTime(hour, minute);
    }

    void ApplyMinimapGameTime(int hour24, int minute)
    {
        if (_windowEle == null)
            return;

        var label = GetElementById("txtGameTime") as Label;
        if (label == null)
        {
            Debug.LogError("[MapWindow] txtGameTime missing from MinimapWnd layout.");
            return;
        }

        label.text = InterludeMinimapGameTime.FormatHudLabel(hour24, minute);

        bool day = InterludeMinimapGameTime.IsDaylightHour(hour24);
        VisualElement sun = GetElementById("texSun");
        VisualElement moon = GetElementById("texMoon");
        if (sun == null || moon == null)
        {
            Debug.LogError("[MapWindow] texSun or texMoon missing from MinimapWnd layout.");
            return;
        }

        sun.style.display = day ? DisplayStyle.Flex : DisplayStyle.None;
        moon.style.display = day ? DisplayStyle.None : DisplayStyle.Flex;
    }

    void OnDestroy()
    {
        UnsubscribeGameTimeClock();
    }
}