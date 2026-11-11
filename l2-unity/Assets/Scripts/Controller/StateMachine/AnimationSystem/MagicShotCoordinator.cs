using System.Collections.Generic;
using UnityEngine;

public static partial class MagicShotCoordinator
{
    private struct ShotSession
    {
        public int CastStartMs;
        public float LastUpdatedTime;
    }

    private static readonly object Sync = new object();
    private static readonly Dictionary<int, ShotSession> SessionsByObjectId = new Dictionary<int, ShotSession>();
    private const float StaleSessionLifetimeSeconds = 30f;



}
