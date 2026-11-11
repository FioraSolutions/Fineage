using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractSkillbar
{
    protected VisualElement _windowEle;
    protected VisualElement _skillbarWindow;
    private int _skillbarIndex;
    protected int _page;
    protected bool _horizontalBar;
    protected List<VisualElement> _slotAnchors;
    protected List<SkillbarSlot> _barSlots;
    protected ArrowInputManipulator _arrowInputManipulator;

    public int Page { get { return _page; } }
    public int SkillbarIndex { get { return _skillbarIndex; } }





    public List<SkillbarSlot> BarSlots
    {
        get { return _barSlots; }
    }





    protected abstract void UpdateVisuals();

    /// <summary>Re-applies per-slot marks (armed shots) without rebuilding the bar.</summary>



}
