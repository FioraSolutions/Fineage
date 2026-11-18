using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections;

public partial class SkillbarSlot
{
    bool TryToggleAutoShot()
    {
        if (_shortcut == null || _shortcut.Type != Shortcut.TYPE_ITEM)
            return false;

        int itemId = ShotItemId();
        if (!NetAutoShot.IsShotItem(itemId))
            return false;

        NetAutoShot.Toggle(itemId);
        RefreshMarks();
        return true;
    }

    int ShotItemId()
    {
        if (_shortcut != null)
        {
            PlayerInventory inventory = SkillbarPacketServices.Inventory;
            ItemInstance item = inventory != null ? inventory.FindByAllInventory(_shortcut.Id) : null;
            if (item == null && inventory != null && _shortcut.Level > 0)
                item = inventory.GetItemByItemId(_shortcut.Level);
            if (item != null)
                return item.ItemId;
            if (_shortcut.Level > 0)
                return _shortcut.Level;
        }
        return _itemId;
    }

    public void RefreshMarks()
    {
        RefreshAutoShotMark();
        RefreshToggleMark();
    }

    /// <summary>
    /// Interlude toggle marker: a running toggle (operate_type 3) shows the animated ToggleEffect shine over its
    /// icon on the bar until the zone drops its abnormal.
    /// </summary>
    void RefreshToggleMark()
    {
        int skillId = 0;
        int level = 0;
        if (_shortcut != null && _shortcut.Type == Shortcut.TYPE_SKILL)
        {
            skillId = _shortcut.Id;
            level = _shortcut.Level;
        }

        ToggleSkillShine.Apply(_slotElement, skillId, level, ref _toggleShine, ref _togglePulse);
        if (_slotElement != null && skillId > 0 && SkillToggleState.IsActive(skillId))
            _slotElement.AddToClassList("toggle-on");
        else
            _slotElement?.RemoveFromClassList("toggle-on");
    }

    void RefreshAutoShotMark()
    {
        int itemId = ShotItemId();
        bool armed = itemId > 0 && NetAutoShot.IsEnabled(itemId);

        if (!armed)
        {
            StopAutoShotWave();
            if (_autoShotMask != null)
                _autoShotMask.style.display = DisplayStyle.None;
            _slotElement?.RemoveFromClassList("auto-shot-on");
            return;
        }

        _slotElement?.AddToClassList("auto-shot-on");
        AutoShotSlotGlow.EnsureOverlay(_slotElement, ref _autoShotMask);
        if (_autoShotPulse == null)
            _autoShotPulse = AutoShotSlotGlow.StartWave(_autoShotMask);
    }

    void StopAutoShotWave()
    {
        if (_autoShotPulse != null)
        {
            _autoShotPulse.Pause();
            _autoShotPulse = null;
        }
    }

    protected override void HandleMiddleClick()
    {
    }
}
