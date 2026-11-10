

public partial class TradeWindow
{
    public class ExchangeSlot : L2DraggableSlot
    {
        private int _count;
        private long _remainingTime;
        private SlotClickSoundManipulator _slotClickSoundManipulator;
        private int _objectId;
        private ItemCategory _itemCategory;
        protected bool _empty = true;

        private ItemInstance _itemInstance;
        public ItemInstance ItemInstance { get { return _itemInstance; } }
        public int Count { get { return _count; } }
        public long RemainingTime { get { return _remainingTime; } }
        public ItemCategory ItemCategory { get { return _itemCategory; } }
        public int ObjectId { get { return _objectId; } }
        public int ItemId { get { return _id; } }
        public bool IsEmpty { get { return _empty; } }

        public ExchangeSlot(int position, VisualElement slotElement, SlotType slotType)
            : base(position, slotElement, slotType, true, false)
        {
            _empty = true;

            if (_slotClickSoundManipulator == null)
            {
                _slotClickSoundManipulator = new SlotClickSoundManipulator(_slotElement);
                _slotElement.AddManipulator(_slotClickSoundManipulator);
            }
        }

        public void AssignEmpty()
        {
            _empty = true;
            _id = 0;
            _name = "Unknown";
            _description = "Unknown item.";
            _itemInstance = null;
            _objectId = 0;
            _enchantLevel = 0;

            if (_slotElement != null)
            {
                L2SlotIconView.PaintEmptyInventorySlot(SlotBg, _slotElement);
                if (_slotDragManipulator != null)
                    _slotDragManipulator.enabled = false;
            }
        }

        public void AssignItem(ItemInstance item)
        {
            _slotElement.RemoveFromClassList("empty");

            AddDataItem(item);
            _itemInstance = item;
            _count = item.Count;
            _objectId = item.ObjectId;
            _enchantLevel = item.EnchantLevel;
            _id = item.ItemId;
            _remainingTime = item.RemainingTime;
            _empty = false;

            if (_slotElement != null)
            {
                AddImage(this);
                AddTooltip(item);
                _slotDragManipulator.enabled = true;
            }
        }

        private void AddDataItem(ItemInstance item)
        {
            if (item.ItemData != null)
            {
                _id = item.ItemData.Id;
                _name = item.ItemData.ItemName.DisplayName;
                _description = item.ItemData.ItemName.Description;
                _icon = item.ItemData.Icon;
                _objectId = item.ObjectId;
                _itemCategory = item.Category;
            }
            else
            {
                // Debug.LogWarning($"Item data is null for item {item.ItemId}.");
                _id = 0;
                _name = "Unknown";
                _description = "Unknown item.";
                _icon = "";
                _objectId = -1;
                _itemCategory = ItemCategory.Item;
            }
        }

        private void AddImage(ExchangeSlot slot)
        {
            L2SlotIconView.SetItemIcon(SlotBg, _id);
        }

        private void AddTooltip(ItemInstance item)
        {
            if (_tooltipManipulator != null)
                _tooltipManipulator.SetText(L2ItemSlotTooltip.Format(_name, _count, item.Category));
        }

        public override void ClearManipulators()
        {
            base.ClearManipulators();

            if (_slotClickSoundManipulator != null)
            {
                _slotElement.RemoveManipulator(_slotClickSoundManipulator);
                _slotClickSoundManipulator = null;
            }
        }

        protected override void HandleLeftClick()
        {
            // При левом клике - удаляем предмет из обмена
            if (!_empty)
            {
                TradePacketServices.Trade.RemoveExchangeItem(_objectId);
            }
        }

        protected override void HandleRightClick()
        {
            // То же самое для правого клика
            if (!_empty)
            {
                TradePacketServices.Trade.RemoveExchangeItem(_objectId);
            }
        }

        protected override void HandleMiddleClick()
        {
            // Не используется в обмене
        }
    }

}
