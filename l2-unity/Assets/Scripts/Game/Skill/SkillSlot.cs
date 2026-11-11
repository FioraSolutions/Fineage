using Newtonsoft.Json.Bson;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillSlot : L2DraggableSlot
{
    private SkillInstance _skillInstance { get;  set; }
    private bool _empty = false;
    public event Action<int> EventLeftClick;
    private bool _isPassive = false;
    private VisualElement _reuseElement;
    private VisualElement _rechargeMaskBg;
















}
