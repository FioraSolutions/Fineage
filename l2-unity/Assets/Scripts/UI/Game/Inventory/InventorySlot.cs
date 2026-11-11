using UnityEngine.UIElements;
using UnityEngine;

public partial class InventorySlot : L2DraggableSlot
{
    protected L2Tab _currentTab;
    private int _count;
    private long _remainingTime;
    private SlotClickSoundManipulator _slotClickSoundManipulator;
    private int _objectId;
    private ItemCategory _itemCategory;
    protected bool _empty = true;

    public int Count { get { return _count; } }
    public long RemainingTime { get { return _remainingTime; } }
    public ItemCategory ItemCategory { get { return _itemCategory; } }
    public int ObjectId { get { return _objectId; } }
    public int ItemId { get { return _id; } }


 




    private Product _product;
    private ItemInstance _itemInstance;
    public ItemInstance ItemInstance { get { return _itemInstance; } }
    public Product Product { get { return _product; } }









    // Alpha channel unused for now; keep path shared with AddImage.


    public bool IsEmpty { get { return _empty; } }
    float _useClickAt;






}
