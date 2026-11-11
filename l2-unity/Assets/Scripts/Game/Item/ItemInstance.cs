using FMOD.Studio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR;

public partial class ItemInstance : AbstractServerItem
{
    [SerializeField] AbstractItem _itemData;
    [SerializeField] private int _objectId;
    [SerializeField] private int _itemId;
    [SerializeField] private ItemLocation _location;
    [SerializeField] private int _slot;
    [SerializeField] private int _count;
    [SerializeField] private ItemCategory _category;
    [SerializeField] private bool _equipped;
    [SerializeField] private ItemSlot _bodyPart;
    [SerializeField] private int _enchantLevel;
    [SerializeField] private long _remainingTime;
    [SerializeField] private int _lastChange;
    private bool  _requiredItems = false;
    private int  _requiredCount = 0;

    public AbstractItem ItemData { get { return _itemData; } }
    public int ObjectId { get { return _objectId; } }
    public int ItemId { get { return _itemId; } }
    public ItemLocation Location { get { return _location; } }
    public bool Equipped { get { return _equipped; } }
    public int Slot { get { return _slot; } }
    public int Count { get { return _count; } }
    public ItemCategory Category { get { return _category; } }
    public ItemSlot BodyPart { get { return _bodyPart; } }
    public int EnchantLevel { get { return _enchantLevel; } }
    public long RemainingTime { get { return _remainingTime; } }
    public int LastChange { get { return _lastChange; } set { _lastChange = value; } }










    
    //We pass the IsBow bool so that when the server receives information about what needs to be changed,
    //it doesn't select the first element in its LR hands. This is usually the left hand, which usually contains arrows,
    //so we specifically select only the bow from the right hand.
}
