

public partial class InventoryTab
{
    public override void SelectSlot(int slotPosition)
    {
        if (_selectedSlot != -1)
        {
            //It happens that the panel will become smaller and the index may be larger than the current panel, so you need to check
            if (ArrayUtils.IsValidIndexArray(_inventorySlots, _selectedSlot)){
                _inventorySlots[_selectedSlot].UnSelect();
            }

        }
        _inventorySlots[slotPosition].SetSelected();
        _selectedSlot = slotPosition;
    }

    public void UnSelectSlot()
    {
        if (_selectedSlot != -1)
        {
            _inventorySlots[_selectedSlot].UnSelect();
        }
    }

    public void ResetSelectSlot()
    {
        _selectedSlot = -1;
    }

    protected override void OnGeometryChanged()
    {
    }

    protected override void OnSwitchTab()
    {
        if (InventoryPacketServices.Window.SwitchTab(this))
        {
            AudioPacketServices.Audio.PlayUISound("window_open");
        }
    }

    protected override void RegisterAutoScrollEvent()
    {
    }

    public void SetSelectedSlot(int selected)
    {
        _selectedSlot = selected;
    }

    public void SetFilterCategories(List<ItemCategory> filteredCategories)
    {
        _filteredCategories = filteredCategories;
    }

}
