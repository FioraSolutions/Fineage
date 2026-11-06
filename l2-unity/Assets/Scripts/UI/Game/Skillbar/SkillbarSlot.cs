using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public class SkillbarSlot : L2ClickableSlot
{
    private ButtonClickSoundManipulator _buttonClickSoundManipulator;
    private L2Slot _innerSlot;
    private Shortcut _shortcut;
    private int _skillbarId;
    private int _slot;
    private int _itemId;
    private VisualElement _keyElement;
    private VisualElement _reuseElement;
    private VisualElement _rechargeMaskBg;
    private VisualElement _autoShotMask;

    public SkillbarSlot(VisualElement slotElement, int position, int skillbarId, int slot) : base(slotElement, position, SlotType.SkillBar, true, false)
    {
        _slotElement = slotElement;
        _position = position;
        _skillbarId = skillbarId;
        _slot = slot;
        _keyElement = _slotElement.Q<VisualElement>("Key");
        _reuseElement = _slotElement.Q<VisualElement>("ReuseBg");
        _rechargeMaskBg = _slotElement.Q<VisualElement>("RechargeMaskBg");
        // UITK Button.Clickable steals pointer capture from SlotDragManipulator and leaves the
        // floating skill icon stuck to the cursor. Clicks go through L2ClickableSlot callbacks.
        if (_slotElement is Button button)
            button.clickable = null;
    }


    public VisualElement GetReuseElement()
    {
        return _reuseElement;
    }


    public VisualElement GetRechargeMaskElement()
    {
        return _rechargeMaskBg;
    }

    public Shortcut Shortcut
    {
        get { return _shortcut; }
    }

    public void AssignShortcut(Shortcut shortcut)
    {
        ClearInnerSlot();
        // Do not ClearManipulators() here — that nulls SkillbarSlot._hoverManipulator while
        // MouseUp handlers stay registered → NRE in HandleSlotClickUp and dead clicks.
        if (_buttonClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_buttonClickSoundManipulator);
            _buttonClickSoundManipulator = null;
        }

        _shortcut = shortcut;
        _itemId = 0;
        // Re-assert: UITK may reattach Clickable on Button targets.
        if (_slotElement is Button button)
            button.clickable = null;
        _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        _slotElement.AddManipulator(_buttonClickSoundManipulator);

        switch (shortcut.Type)
        {
            case Shortcut.TYPE_ACTION:
                AssignAction(shortcut.Id);
                break;
            case Shortcut.TYPE_ITEM:
                AssignItem(shortcut.Id);
                break;
            case Shortcut.TYPE_MACRO:
                break;
            case Shortcut.TYPE_RECIPE:
                break;
            case Shortcut.TYPE_SKILL:
                AssignSkill(shortcut.Id , shortcut.Level);
                break;
        }
    }

    void ClearInnerSlot()
    {
        if (_innerSlot == null)
            return;

        if (_innerSlot is L2DraggableSlot)
            _innerSlot.ClearManipulators();
        else if (_innerSlot is L2ClickableSlot clickable)
            clickable.ClearManipulators();
        else
            _innerSlot.ClearManipulators();

        _innerSlot = null;
    }

    public void AssignItem(int objectId)
    {
        ClearInnerSlot();
        // The bar can restore shortcuts before the inventory is bound (skillbar init runs early).
        PlayerInventory inventory = SkillbarPacketServices.Inventory;
        ItemInstance item = inventory != null ? inventory.FindByAllInventory(objectId) : null;
        _itemId = item != null ? item.ItemId : 0;
        _innerSlot = new InventorySlot(_position, _slotElement, SlotType.SkillBar);
        ((InventorySlot)_innerSlot).AssignItem(item);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        UpdateInputInfo();
        RefreshMarks();
    }

    public void AssignSkill(int objectId , int level)
    {
        ClearInnerSlot();
        _itemId = 0;
        _innerSlot = new SkillSlot(_slotElement, _position, SlotType.SkillBar);
        ((SkillSlot)_innerSlot).AssignSkill(objectId , level , false);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        UpdateInputInfo();
        RefreshMarks();
    }

    public void AssignAction(int objectId)
    {
        ClearInnerSlot();
        _itemId = 0;
        _innerSlot = new ActionSlot(_slotElement, _position, SlotType.SkillBar);
        ((ActionSlot)_innerSlot).AssignAction((ActionType)objectId);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        UpdateInputInfo();
        RefreshMarks();
    }

    private void UpdateInputInfo()
    {
        _slotElement.RemoveFromClassList("empty");

        string key = SkillbarPacketServices.Shortcuts.GetKeybindForShortcut(_skillbarId, _slot);

        //Debug.Log("UpdateInputInfo поиску по KeyImageTable");
        Texture2D inputTexture = KeyImageTable.Instance.LoadTextureByKey(key);

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

    /// <summary>Toggles auto-use when the shortcut is a shot. Returns true when the click was consumed.</summary>
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

    /// <summary>
    /// The shot template id for this slot. Bar shortcuts store the inventory objectId, so the
    /// carried item is resolved back to its template; a stale shortcut with no item resolves to 0.
    /// The resolved id wins over the cached one: when a shot stack is used up and re-bought the
    /// objectId changes, and the old cache would point at a template nobody carries.
    /// </summary>
    int ShotItemId()
    {
        if (_shortcut != null)
        {
            PlayerInventory inventory = SkillbarPacketServices.Inventory;
            ItemInstance item = inventory != null ? inventory.FindByAllInventory(_shortcut.Id) : null;
            if (item != null)
                return item.ItemId;
        }
        return _itemId;
    }

    /// <summary>Armed-shot marker: a subtle highlight so the player can see auto-use is on.</summary>
    public void RefreshMarks()
    {
        int itemId = ShotItemId();
        bool armed = itemId > 0 && NetAutoShot.IsEnabled(itemId);

        if (!armed)
        {
            if (_autoShotMask != null)
                _autoShotMask.style.display = DisplayStyle.None;
            _slotElement?.RemoveFromClassList("auto-shot-on");
            return;
        }

        _slotElement?.AddToClassList("auto-shot-on");
        if (_autoShotMask == null)
        {
            _autoShotMask = new VisualElement { pickingMode = PickingMode.Ignore };
            _autoShotMask.style.position = UnityEngine.UIElements.Position.Absolute;
            _autoShotMask.style.left = 0;
            _autoShotMask.style.top = 0;
            _autoShotMask.style.right = 0;
            _autoShotMask.style.bottom = 0;
            _autoShotMask.style.borderLeftWidth = 1;
            _autoShotMask.style.borderRightWidth = 1;
            _autoShotMask.style.borderTopWidth = 1;
            _autoShotMask.style.borderBottomWidth = 1;
            _autoShotMask.style.borderLeftColor = new Color(1f, 0.85f, 0.25f, 0.9f);
            _autoShotMask.style.borderRightColor = new Color(1f, 0.85f, 0.25f, 0.9f);
            _autoShotMask.style.borderTopColor = new Color(1f, 0.85f, 0.25f, 0.9f);
            _autoShotMask.style.borderBottomColor = new Color(1f, 0.85f, 0.25f, 0.9f);
            _slotElement.Add(_autoShotMask);
        }

        _autoShotMask.style.display = DisplayStyle.Flex;
    }

    protected override void HandleMiddleClick()
    {
    }
}