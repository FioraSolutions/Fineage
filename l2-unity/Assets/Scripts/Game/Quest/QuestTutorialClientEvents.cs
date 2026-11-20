using System.Collections.Generic;

public static class QuestTutorialClientEvents
{
    static readonly HashSet<int> _enabled = new();

    public static void SetEnabled(int eventId, bool on)
    {
        if (eventId <= 0)
            return;
        if (on)
            _enabled.Add(eventId);
        else
            _enabled.Remove(eventId);
    }

    public static bool IsEnabled(int eventId) => eventId > 0 && _enabled.Contains(eventId);
}