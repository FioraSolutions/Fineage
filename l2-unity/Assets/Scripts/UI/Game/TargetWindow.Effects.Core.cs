using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public partial class TargetWindow
{
    void TickEffectTimers()
    {
        if (_effectCells.Count == 0)
            return;
        float now = Time.time;
        for (int i = 0; i < _effectCells.Count; i++)
        {
            TargetEffectCell cell = _effectCells[i];
            cell?.Tooltip?.SyncRemaining(cell.EndsAt);
        }
    }

    public void RefreshEffectsIfSelected(int objectId)
    {
        TargetManager targets = TargetPacketServices.Targets;
        if (targets == null || !targets.HasTarget())
            return;
        TargetData data = targets.Target;
        if (data?.Identity == null || data.Identity.Id != objectId)
            return;
        _effectsObjectId = -1;
        _effectsSignature = 0;
        BindEffects(data);
    }

    /// <summary>
    /// Interlude TargetStatusWnd carries abnormal rows only for monsters / NPCs. A player target
    /// (self or another character) is name + HP chrome only: buffs travel through PartySpelled /
    /// the owner's BufferPanel, never through the target window.
    /// </summary>
    static bool CanShowTargetEffects(TargetData targetData)
    {
        EntityIdentity identity = targetData?.Identity;
        if (identity == null)
            return false;
        if (identity.Owned)
            return false;
        return identity.EntityType == EntityType.Monster || identity.EntityType == EntityType.NPC;
    }

    void BindEffects(TargetData targetData)
    {
        if (_effectsRow == null)
            return;
        if (targetData?.Identity == null || !CanShowTargetEffects(targetData))
        {
            ClearEffects();
            return;
        }

        Entity entity = targetData.GetEntity();
        var list = entity != null ? entity.TargetEffects : null;
        int objectId = targetData.Identity.Id;
        int signature = EffectsSignature(list);
        if (objectId == _effectsObjectId && signature == _effectsSignature)
            return;

        _effectsObjectId = objectId;
        _effectsSignature = signature;
        _effectsRow.Clear();
        _effectCells.Clear();

        if (list == null || list.Count == 0)
        {
            _effectsRow.style.display = DisplayStyle.None;
            RefreshPlateHeight();
            return;
        }

        int painted = 0;
        var seen = new System.Collections.Generic.HashSet<int>();
        var duplicateIds = new System.Collections.Generic.List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            EffectHolder effect = list[i];
            if (effect == null || effect.Id <= 0)
                continue;
            if (!seen.Add(effect.Id))
            {
                duplicateIds.Add(effect.Id);
                continue;
            }
            if (TryAddEffectIcon(effect.Id, effect.Value, effect.Duration))
                painted++;
        }

        if (duplicateIds.Count > 0)
        {
            Debug.LogWarning(
                "[TargetWindow] duplicate abnormal skill ids from server npcId=" +
                (targetData.Identity != null ? targetData.Identity.NpcId : 0) +
                " objectId=" + objectId +
                " ids=" + string.Join(",", duplicateIds));
        }

        _effectsRow.style.display = painted > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        RefreshPlateHeight();
    }

    /// <summary>
    /// The SmallWnd plate is 46px; it swaps to the 76px SmallWindow2 trio only while the NpcInfo
    /// property tree is open with icons. The abnormal strip below the plate never changes it.
    /// </summary>
    void RefreshPlateHeight()
    {
        if (_windowEle == null)
            return;

        bool hasProperties = _expanded
            && _propertiesRow != null
            && _propertiesRow.childCount > 0;

        if (hasProperties)
            _windowEle.AddToClassList("expanded");
        else
            _windowEle.RemoveFromClassList("expanded");
    }
}
