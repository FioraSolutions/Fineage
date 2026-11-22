using System.Collections.Generic;

public enum NpcVoicePriority : byte
{
    IdleAmbient = 0,
    SocialIdle = 1,
    Targeted = 2,
}

/// <summary>
/// Global concurrent ChrSound voice cap (Interlude throttles ambient NPC chatter in crowds).
/// </summary>
public static class NpcVoicePlaybackGate
{
    public const int DefaultMaxConcurrentVoices = 5;

    static readonly Dictionary<int, NpcVoicePriority> ActiveByEntity = new Dictionary<int, NpcVoicePriority>(16);
    static readonly object Sync = new object();

    public static int MaxConcurrent { get; set; } = DefaultMaxConcurrentVoices;

    public static void Reset()
    {
        lock (Sync)
            ActiveByEntity.Clear();
    }

    public static bool TryBegin(int entityId, NpcVoicePriority priority)
    {
        if (entityId == 0)
            return true;

        lock (Sync)
        {
            if (ActiveByEntity.TryGetValue(entityId, out NpcVoicePriority existing))
            {
                if (priority <= existing)
                    return true;
                ActiveByEntity[entityId] = priority;
                return true;
            }

            if (ActiveByEntity.Count >= MaxConcurrent)
            {
                if (!TryEvictLowestPriority(priority))
                    return false;
            }

            ActiveByEntity[entityId] = priority;
            return true;
        }
    }

    static bool TryEvictLowestPriority(NpcVoicePriority incoming)
    {
        int victimId = 0;
        NpcVoicePriority lowest = NpcVoicePriority.Targeted;
        foreach (KeyValuePair<int, NpcVoicePriority> kvp in ActiveByEntity)
        {
            if (kvp.Value <= lowest)
            {
                lowest = kvp.Value;
                victimId = kvp.Key;
            }
        }

        if (victimId == 0 || incoming <= lowest)
            return false;

        ActiveByEntity.Remove(victimId);
        return true;
    }

    public static void End(int entityId)
    {
        if (entityId == 0)
            return;
        lock (Sync)
            ActiveByEntity.Remove(entityId);
    }

    public static int ActiveCount
    {
        get
        {
            lock (Sync)
                return ActiveByEntity.Count;
        }
    }
}