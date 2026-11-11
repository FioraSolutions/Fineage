using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class EffectManager : MonoBehaviour
{
    private void WireGamePacketServices()
    {
        GamePacketServices.Bind(animations: _animations);
    }

    Transform EnsureActiveEffectsContainer()
    {
        if (_activeEffectsContainer != null)
            return _activeEffectsContainer;

        Transform existing = transform.Find("ActiveEffects");
        if (existing != null)
        {
            _activeEffectsContainer = existing;
            return _activeEffectsContainer;
        }

        var go = new GameObject("ActiveEffects");
        go.transform.SetParent(transform, false);
        _activeEffectsContainer = go.transform;
        return _activeEffectsContainer;
    }
}
