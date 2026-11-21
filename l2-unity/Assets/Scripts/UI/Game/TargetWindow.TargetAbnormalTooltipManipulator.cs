using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Rich abnormal tooltip (name, level, description, remaining time) for target effect icons.
/// </summary>
public sealed class TargetAbnormalTooltipManipulator : TooltipManipulator
{
    readonly int _skillId;
    readonly int _wireValue;
    float _endsAt;

    public TargetAbnormalTooltipManipulator(VisualElement target, int skillId, int wireValue, int durationSeconds)
        : base(target, string.Empty)
    {
        _skillId = skillId;
        _wireValue = wireValue;
        _endsAt = durationSeconds > 0 ? Time.time + durationSeconds : 0f;
        RefreshText();
    }

    public void SyncRemaining(float endsAt)
    {
        _endsAt = endsAt;
        RefreshText();
    }

    void RefreshText()
    {
        int remaining = _endsAt > 0f ? Mathf.Max(0, Mathf.CeilToInt(_endsAt - Time.time)) : 0;
        string text = AbnormalEffectTooltipBuilder.Build(_skillId, _wireValue, remaining, includeRemainingTime: true);
        SetText(text);
    }

    protected override void RegisterCallbacksOnTarget()
    {
        base.RegisterCallbacksOnTarget();
        if (target != null)
            target.RegisterCallback<PointerEnterEvent>(_ => RefreshText());
    }
}