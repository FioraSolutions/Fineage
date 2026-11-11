using System;
using UnityEngine;
using System.Collections.Generic;

public partial class FNManagerLight
{
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (_debugLog)
            {
                // Debug.LogWarning($"{LOG_TAG} duplicate on '{name}' — Destroy(this), keep existing Instance");
            }

            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsurePool();
        if (_debugLog)
        {
            // Debug.Log(
                // $"{LOG_TAG} Awake ok on '{name}' id={GetInstanceID()} " +
                // $"fallback intensity={_intensity:F2} range={_rangeMeters:F3}m duration={_durationSeconds:F3}s pool={_poolSize}");
        }
    }

    public static FNManagerLight Ensure()
    {
        if (Instance != null)
        {
            return Instance;
        }

        // Debug.LogWarning($"{LOG_TAG} Instance was null — creating runtime GameObject");
        GameObject go = new GameObject(nameof(FNManagerLight));
        return go.AddComponent<FNManagerLight>();
    }
}
