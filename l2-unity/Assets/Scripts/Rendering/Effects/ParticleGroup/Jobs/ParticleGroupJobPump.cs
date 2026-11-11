using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public sealed partial class ParticleGroupJobPump : MonoBehaviour
{
    struct QueuedGroup
    {
        public ParticleGroup group;
        public float now;
    }

    static ParticleGroupJobPump _instance;
    static readonly List<QueuedGroup> Queued = new List<QueuedGroup>(64);





}
