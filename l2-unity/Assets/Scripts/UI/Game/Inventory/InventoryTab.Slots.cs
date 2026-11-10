

public partial class InventoryTab
{
    public void UpdateInventorySlots(InventorySlot[] inventory)
    {
        _inventorySlots = null;
        _inventorySlots = inventory;
    }

    private void CreateEmptyInventory(bool initEmty)
    {
        if (_contentContainer != null && initEmty)
        {

            _contentContainer.Clear();

            int slotCount = InventoryWindow.PLAYER_INVENTORY_SIZE;
            _inventorySlots = new InventorySlot[slotCount];
            //SlotType slotType = GetSlotType(MainTab);

            VisualTreeAsset template = InventoryPacketServices.Window.InventorySlotTemplate;
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                if (i == _inventorySlots.Length - 1)
                {
                    VisualElement disabledElement = CreateVisualElementDisabled(template);
                    _contentContainer.Add(disabledElement);
                }
                else
                {
                    VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(template, _contentContainer);
                    _inventorySlots[i] = CreateInventorySlot(i, slotElement, SlotType.Inventory);
                }

            }
            UpdateInventorySlots(_inventorySlots);
        }

    }

    private VisualElement CreateVisualElementDisabled(VisualTreeAsset template)
    {
        VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(template, null);
        slotElement.AddToClassList("inventory-slot");
        slotElement.AddToClassList("disabled");
        return slotElement;
    }

    private InventorySlot CreateInventorySlot(int i , VisualElement slotElement , SlotType slotType)
    {
        return new InventorySlot(i, slotElement, this, slotType , false);
    }

    private void UpdatePadSlots()
    {
        int slotCount = InventoryPacketServices.Window.SlotCount;

        // Interlude InventoryWnd bag is 6-wide; Classic expand used 9/12.
        int rowLength = InventoryWindow.BagColumns;
        if (InventoryPacketServices.Window != null && InventoryPacketServices.Window.Expanded)
            rowLength = 12;

        int padSlot = 0;
        if (slotCount < 8 * rowLength)
        {
            padSlot = 8 * rowLength - slotCount;
        }
        else if (slotCount % rowLength != 0)
        {
            padSlot = rowLength - slotCount % rowLength;
        }

        VisualTreeAsset template = InventoryPacketServices.Window.InventorySlotTemplate;
        for (int i = 0; i < padSlot; i++)
        {
            VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(template, _contentContainer);
            slotElement.AddToClassList("inventory-slot");
            slotElement.AddToClassList("disabled");
        }
    }

    public void SetItemList(List<ItemInstance> allItems)
    {

        if(allItems != null && allItems.Count > 0)
        {
            AddDataInventory(allItems);
            AddAllEmptyInventory(allItems.Count);
        }
        else
        {
            AddAllEmptyInventory(0);
        }
    }

    private void AddDataInventory(List<ItemInstance> allItems)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            ItemInstance item = allItems[i];
            item.SetSlot(i);
            //Debug.Log("AssignItem Set Inventory>>>> " + item.ItemId + " ObjectId " + item.ObjectId + " Add Slot " + item.Slot);
            _inventorySlots[i].AssignItem(item);
        }
    }

    public void AddAllEmptyInventory(int size)
    {
        int start_i = size;

        for (int i = start_i; i < _inventorySlots.Length; i++)
        {
            var slot = _inventorySlots[i];

            if (slot != null)
            {
                slot.AssignEmpty();
            }
               
        }
    }

    public InventorySlot GetInventorySlot(int objectId)
    {

        return _inventorySlots?
                               .Where(slot => slot != null && slot.ObjectId == objectId)
                               .FirstOrDefault();
    }

    public ItemInstance GetInventorySlotByPosition(int position)
    {
        if(ArrayUtils.IsValidIndexArray(_inventorySlots, position))
        {
            return _inventorySlots[position].ItemInstance;
        }
       return null;
    }

    public int GetEmptySlot()
    {
        InventorySlot slot =  _inventorySlots?
                              .Where(slot => slot != null && slot.Id == 0)
                              .FirstOrDefault();
        return slot.Position;
    }

    public InventorySlot GetEmptyInventorySlot()
    {
        return  _inventorySlots?
                              .Where(slot => slot != null && slot.Id == 0)
                              .FirstOrDefault();
    }

    public InventorySlot GetFreeSlot()
    {
        //return _inventorySlots.FirstOrDefault(slot => slot.ObjectId == 0);

        return _inventorySlots?
                               .Where(slot => slot.IsEmpty)
                               .FirstOrDefault();
    }

}
