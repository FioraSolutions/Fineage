using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class SkillbarSlot
{
    private void UpdateInputInfo()
    {
        _slotElement.RemoveFromClassList("empty");

        string key = SkillbarPacketServices.Shortcuts.GetKeybindForShortcut(_skillbarId, _slot);

        //Debug.Log("UpdateInputInfo поиску по KeyImageTable");
        Texture2D inputTexture = TablePacketServices.KeyImages.LoadTextureByKey(key);

        if (inputTexture != null)
        {
            _keyElement.style.backgroundImage = inputTexture;
            _keyElement.style.width = inputTexture.width;
        }
    }

    public override void ClearManipulators()
    {
        base.ClearManipulators();

        if (_buttonClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_buttonClickSoundManipulator);
            _buttonClickSoundManipulator = null;
        }
    }

    protected override void HandleLeftClick()
    {
        if (_shortcut == null)
            return;

        // Interlude: a soulshot / spiritshot shortcut is an auto-use switch, not a "use one" button.
        // Clicking it arms automatic charging (the icon lights up) and clicking again disarms it.
        if (TryToggleAutoShot())
            return;

        // Debug.LogWarning($"Use bar slot {_position}.");
        SkillbarPacketServices.Shortcuts.UseShortcut(_shortcut);
    }

    protected override void HandleRightClick()
    {
        // The original client toggles shots with the left click; the right click does it too so the
        // gesture mouse players reach for first works, and it stays a no-op for every other shortcut.
        TryToggleAutoShot();
    }

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
