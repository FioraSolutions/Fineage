using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HomeProjectileTrailVelocityProvider
{
    public void RetargetBindingsTo(Transform searchRoot)
    {
        if (searchRoot == null || _bindings == null)
        {
            return;
        }

        for (int i = 0; i < _bindings.Count; i++)
        {
            TrailBinding binding = _bindings[i];
            if (binding == null)
            {
                continue;
            }

            string tailName = binding.tailRoot != null ? binding.tailRoot.name : "SpriteEmitter2";
            string coreName = string.IsNullOrEmpty(binding.velocitySourceName)
                ? "SpriteEmitter5"
                : binding.velocitySourceName;
            binding.tailRoot = FindChildByName(searchRoot, tailName);
            binding.velocitySource = FindChildByName(searchRoot, coreName);
            binding.localSpaceReference = binding.tailRoot;
            binding.targetRenderers = null;
            binding.hasLastPosition = false;
            binding.history = null;
            binding.baseLocalScales = null;
        }
    }

    [SerializeField] private float _minimumAxisRange = 0.002f;
    [SerializeField] private float _smoothing = 18f;
    [SerializeField] private bool _debugLogs;
    [SerializeField] private List<TrailBinding> _bindings = new List<TrailBinding>();

    public void CopySettingsFrom(HomeProjectileTrailVelocityProvider other)
    {
        if (other == null)
        {
            return;
        }

        _minimumAxisRange = other._minimumAxisRange;
        _smoothing = other._smoothing;
        _debugLogs = other._debugLogs;
        _bindings = new List<TrailBinding>();
        if (other._bindings == null)
        {
            return;
        }

        for (int i = 0; i < other._bindings.Count; i++)
        {
            TrailBinding source = other._bindings[i];
            if (source == null)
            {
                continue;
            }

            _bindings.Add(CloneBinding(source));
        }
    }

    public static void MoveFromRootToFlightRoot(Transform root, Transform flightRoot)
    {
        if (root == null || flightRoot == null)
        {
            return;
        }

        HomeProjectileTrailVelocityProvider source = root.GetComponent<HomeProjectileTrailVelocityProvider>();
        if (source == null)
        {
            return;
        }

        HomeProjectileTrailVelocityProvider dest = flightRoot.GetComponent<HomeProjectileTrailVelocityProvider>();
        if (dest == null)
        {
            dest = flightRoot.gameObject.AddComponent<HomeProjectileTrailVelocityProvider>();
        }

        dest.CopySettingsFrom(source);
        dest.RetargetBindingsTo(flightRoot);
        dest.ResetRuntimeState();
        source.enabled = false;
        UnityEngine.Object.Destroy(source);
    }
}
