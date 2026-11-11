using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public sealed partial class ParticleGroupJobPump : MonoBehaviour
{
    public static void Enqueue(ParticleGroup group, float now)
    {
        if (group == null)
            return;

        Ensure();
        for (int i = 0; i < Queued.Count; i++)
        {
            if (Queued[i].group != group)
                continue;
            Queued[i] = new QueuedGroup { group = group, now = now };
            return;
        }

        Queued.Add(new QueuedGroup { group = group, now = now });
    }

    public static void Remove(ParticleGroup group)
    {
        if (group == null || Queued.Count == 0)
            return;

        for (int i = Queued.Count - 1; i >= 0; i--)
        {
            if (Queued[i].group == group)
                Queued.RemoveAt(i);
        }
    }
}
