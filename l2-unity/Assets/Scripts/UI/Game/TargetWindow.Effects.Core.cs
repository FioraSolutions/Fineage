using UnityEngine;
using UnityEngine.UIElements;

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
            if (cell?.Timer == null)
                continue;
            int remaining = Mathf.Max(0, Mathf.CeilToInt(cell.EndsAt - now));
            cell.Timer.text = remaining > 0 ? remaining.ToString() : "";
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

    void BindEffects(TargetData targetData)
    {
        if (_effectsRow == null)
            return;
        if (targetData?.Identity == null)
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
            return;
        }

        int painted = 0;
        for (int i = 0; i < list.Count; i++)
        {
            EffectHolder effect = list[i];
            if (effect == null || effect.Id <= 0)
                continue;
            if (TryAddEffectIcon(effect.Id, effect.Value, effect.Duration))
                painted++;
        }

        _effectsRow.style.display = painted > 0 ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
