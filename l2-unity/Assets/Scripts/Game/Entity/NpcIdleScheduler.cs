using UnityEngine;

/// <summary>Per-NPC timing for occasional SpWait / social idles (shared SMBs cannot hold state).</summary>
public sealed class NpcIdleScheduler : MonoBehaviour
{
    const float MinSpecialIdleSeconds = 22f;
    const float MaxSpecialIdleSeconds = 88f;

    float _nextSpecialIdleAt;

    void Awake()
    {
        Reschedule();
    }

    public static NpcIdleScheduler Ensure(Entity entity)
    {
        if (entity == null)
            return null;
        NpcIdleScheduler sched = entity.GetComponent<NpcIdleScheduler>();
        if (sched == null)
            sched = entity.gameObject.AddComponent<NpcIdleScheduler>();
        return sched;
    }

    public bool TryTriggerSpecialIdle()
    {
        return Time.time >= _nextSpecialIdleAt;
    }

    public void OnSpecialIdleTriggered()
    {
        Reschedule();
    }

    public void Reschedule()
    {
        _nextSpecialIdleAt = Time.time + Random.Range(MinSpecialIdleSeconds, MaxSpecialIdleSeconds);
    }
}