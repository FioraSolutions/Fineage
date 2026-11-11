using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public sealed partial class ParticleGroupJobPump : MonoBehaviour
{
    static void Ensure()
    {
        if (_instance != null)
            return;

        var go = new GameObject(nameof(ParticleGroupJobPump));
        DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave;
        _instance = go.AddComponent<ParticleGroupJobPump>();
    }

    void FixedUpdate()
    {
        int count = Queued.Count;
        if (count == 0)
            return;

        JobHandle combined = default;
        bool any = false;
        for (int i = 0; i < count; i++)
        {
            ParticleGroup group = Queued[i].group;
            if (group == null)
                continue;

            JobHandle handle = group.ConsumeExpireHandle();
            combined = any ? JobHandle.CombineDependencies(combined, handle) : handle;
            any = true;
        }

        combined.Complete();

        for (int i = 0; i < count; i++)
        {
            ParticleGroup group = Queued[i].group;
            if (group == null)
                continue;

            group.ApplyExpireAndSpawn(Queued[i].now);
        }

        Queued.Clear();
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
        Queued.Clear();
    }
}
