public partial class InventorySlot
{
    public override void ClearManipulators()
    {
        base.ClearManipulators();

        if (_slotClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_slotClickSoundManipulator);
            _slotClickSoundManipulator = null;
        }
    }

    protected bool TryGiveToEnchant()
    {
        if (_empty || _objectId <= 0)
            return false;
        EnchantWindow window = ItemEnchantPacketServices.Enchant;
        if (window == null || !window.AcceptsTarget)
            return false;
        window.AddEnchantItem(_objectId);
        return true;
    }

    protected override void HandleLeftClick()
    {
        if (_currentTab != null)
        {
            _currentTab.SelectSlot(_position);
        }

        if (_empty)
            return;

        if (TryGiveToEnchant())
            return;

        if (Time.time - _useClickAt < 0.3f)
            UseItem();
        _useClickAt = Time.time;
    }
}
