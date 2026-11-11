using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class SkillbarSlot : L2ClickableSlot
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
    private IVisualElementScheduledItem _autoShotPulse;

}
