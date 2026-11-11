using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemDropPresentationService
{
    void TickGlows()
    {
        if (_groundGlowByItemId.Count == 0)
            return;

        float now = Time.time;
        _expiredGlowIds.Clear();
        foreach (KeyValuePair<int, GroundGlowState> pair in _groundGlowByItemId)
        {
            if (pair.Value.EndTime <= now)
                _expiredGlowIds.Add(pair.Key);
        }

        for (int i = 0; i < _expiredGlowIds.Count; i++)
            StopGroundGlow(_expiredGlowIds[i]);
    }

    void StartGroundGlow(int itemObjectId, Transform parent, ItemDropVisualKind kind)
    {
        StopGroundGlow(itemObjectId);

        int glowId = ResolveGroundGlowEffectId(kind);
        float duration = ItemDropPresentationIds.GroundGlowDurationSeconds;
        if (glowId <= 0 && duration <= 0f)
            return;

        var state = new GroundGlowState();
        if (glowId > 0 && parent != null)
        {
            state.GlowAnchor = new GameObject("DropGroundGlow_" + itemObjectId);
            state.GlowAnchor.transform.SetParent(parent, false);
            state.GlowAnchor.transform.localPosition = Vector3.up * ItemDropPresentationIds.GroundGlowLocalY;
            PlayEffect(glowId, state.GlowAnchor.transform, "ground-glow");
            _layers.ApplyIgnoreRaycastLayer(state.GlowAnchor);
        }

        state.EndTime = duration > 0f ? Time.time + duration : float.PositiveInfinity;
        _groundGlowByItemId[itemObjectId] = state;
    }

    void StopGroundGlow(int itemObjectId)
    {
        if (!_groundGlowByItemId.TryGetValue(itemObjectId, out GroundGlowState state))
            return;

        if (state.GlowAnchor != null)
            UnityEngine.Object.Destroy(state.GlowAnchor);
        _groundGlowByItemId.Remove(itemObjectId);
    }

    void PlayLandBurst(int itemObjectId, Transform itemTransform)
    {
        StopLandBurst(itemObjectId);
        if (itemTransform == null || ItemDropPresentationIds.LandBurstEffectId <= 0)
            return;

        EffectManager effects = Effects;
        if (effects == null)
            return;

        var anchor = new GameObject("HitPointProxy");
        Vector3 grounded = WorldPlacement.Resolve(WorldPlacement.PlacementKind.ItemDrop, itemTransform.position);
        if (ItemDropPresentationIds.LandBurstLocalY != 0f)
            grounded += Vector3.up * ItemDropPresentationIds.LandBurstLocalY;
        anchor.transform.SetParent(itemTransform, true);
        anchor.transform.position = grounded;
        _landBurstByItemId[itemObjectId] = anchor;
        effects.PlayEffect(ItemDropPresentationIds.LandBurstEffectId, anchor.transform);
        _layers.ApplyIgnoreRaycastLayer(anchor);
    }
}
