using UnityEngine.UIElements;
using UnityEngine;

public partial class DataCell : L2Slot
{
    private bool _isBusy;
    private int _idSkill;
    private VisualElement _element;
    private Skillgrp _skill;
    private float _activeTime = 0;
    private float _startTime;

    //Smooth movement counter
    private bool _switchHide = false;
    private bool _switchShow = false;
    private float _durationSmooth = 0;
    //procent of the total
    public float _intervalUse = 0.25f;
    private Label _label;
    private Label _labelShadow;
    private VisualElement _labelTemplate;
    private SkillInstance _skillInstance;
    private float _lastRemaningTime = 0;




}
