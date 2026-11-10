using UnityEngine;
using UnityEngine.UIElements;

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

    public InventorySlot(int position, VisualElement slotElement, L2Tab tab, SlotType slotType , bool rightMouseup, bool leftMouseUp = false)
    : base(position, slotElement, slotType, leftMouseUp, rightMouseup)
    {
        _currentTab = tab;
        _empty = true;

        if (_slotClickSoundManipulator == null)
        {
            _slotClickSoundManipulator = new SlotClickSoundManipulator(_slotElement);
            _slotElement.AddManipulator(_slotClickSoundManipulator);
        }
       
    }

    public InventorySlot(int position, VisualElement slotElement, SlotType slotType)
    : base(position, slotElement, slotType, true, false)
    {
        _empty = true;
    }
 




    private Product _product;
    private ItemInstance _itemInstance;
    public ItemInstance ItemInstance { get { return _itemInstance; } }
    public Product Product { get { return _product; } }





    public void AssignItemAtIndexImage(ItemInstance item , int indexImage)
    {
        _slotElement.RemoveFromClassList("empty");

        AddDataItem(item);
        _itemInstance = item;
        _count = item.Count;
        _objectId = item.ObjectId;
        _id = item.ItemId;
        _remainingTime = item.RemainingTime;
        _enchantLevel = item.EnchantLevel;
        _empty = false;
        RefreshEnchantMark();

        if (_slotElement != null)
        {
            AddImageAtIndex(this , indexImage);
            AddTooltip(item);
            _slotDragManipulator.enabled = true;
        }
        else
        {
            // Debug.Log("Не критическая ошибка не смогли найти InventorySlot>SlotElement");
        }
    }




    // Alpha channel unused for now; keep path shared with AddImage.


    public bool IsEmpty { get { return _empty; } }
    float _useClickAt;


    void RefreshEnchantMark()
    {
        if (_slotElement == null)
            return;
        Label mark = _slotElement.Q<Label>("EnchantMark");
        if (_enchantLevel <= 0)
        {
            if (mark != null)
                mark.RemoveFromHierarchy();
            return;
        }

        if (mark == null)
        {
            mark = new Label { name = "EnchantMark", pickingMode = PickingMode.Ignore };
            mark.style.position = UnityEngine.UIElements.Position.Absolute;
            mark.style.left = 1;
            mark.style.top = 0;
            mark.style.fontSize = 10;
            mark.style.color = new StyleColor(new Color(1f, 0.92f, 0.45f));
            mark.style.unityFontStyleAndWeight = FontStyle.Bold;
            _slotElement.Add(mark);
        }

        mark.text = "+" + _enchantLevel;
    }




}
