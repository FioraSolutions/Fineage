using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections;

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

        // Interlude / Mobius: left click (and the hotkey) uses the item once. Shot auto-use is
        // a right-click toggle that waits for the zone's AutoSoulShot before the shine appears.
        SkillbarPacketServices.Shortcuts.UseShortcut(_shortcut);
    }

    protected override void HandleRightClick()
    {
        // Official Interlude: right-clicking a soulshot / spiritshot / beast shot shortcut
        // sends RequestAutoSoulShot. Other shortcut types stay a no-op.
        TryToggleAutoShot();
    }
}
