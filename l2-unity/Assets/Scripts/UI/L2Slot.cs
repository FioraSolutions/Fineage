using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2Slot
{
    [SerializeField] protected int _id;
    protected int _level;
    [SerializeField] protected int _position;
    [SerializeField] protected SlotType _slotType;
    protected string _name;
    protected string _description;
    protected string _icon;
    protected int _enchantLevel;
    protected VisualElement _slotElement;
    protected VisualElement _slotBg;
    protected VisualElement _slotDisabled;
    protected TooltipManipulator _tooltipManipulator;
    protected SlotHoverDetectManipulator _hoverManipulator;

    public int EnchantLevel { get { return _enchantLevel; } set { _enchantLevel = value; } }
    public int Id { get { return _id; } set { _id = value; } }

    public int Level { get { return _level; } set { _level = value; } }
    public int Position { get { return _position; } set { _position = value; } }
    public SlotType Type { get { return _slotType; } set { _slotType = value; } }
    public string Name { get { return _name; } set { _name = value; } }
    public string Description { get { return _description; } set { _description = value; } }
    public string Icon { get { return _icon; } set { _icon = value; } }
    public VisualElement SlotBg { get { return _slotBg; } }
    public VisualElement SlotElement { get { return _slotElement; } }



}
