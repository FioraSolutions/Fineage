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
}
