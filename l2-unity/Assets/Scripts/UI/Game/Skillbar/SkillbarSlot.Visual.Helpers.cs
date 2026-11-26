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

        // Shine waits for the zone AutoSoulShot packet; do not flip it locally.
        NetAutoShot.Toggle(itemId);
        return true;
    }

    int ShotItemId()
    {
        if (_shortcut != null)
        {
            PlayerInventory inventory = SkillbarPacketServices.Inventory;
            ItemInstance item = null;
            string error = null;
            if (inventory != null && !inventory.TryResolveShortcut(_shortcut.Id, _shortcut.Level, out item, out error))
                item = null;
            if (!string.IsNullOrEmpty(error))
                Debug.LogError("[Inventory] " + error);
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

        // Same ToggleEffect shine as a running toggle skill. Driven only by the zone's
        // AutoSoulShot state — never by a local click. Inventory slots do not call this.
        ToggleSkillShine.Apply(_slotElement, armed, ref _autoShotMask, ref _autoShotPulse);
        if (armed)
            _slotElement?.AddToClassList("auto-shot-on");
        else
            _slotElement?.RemoveFromClassList("auto-shot-on");
    }

    protected override void HandleMiddleClick()
    {
    }
}
