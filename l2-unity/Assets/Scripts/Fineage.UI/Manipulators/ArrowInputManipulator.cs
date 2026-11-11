using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ArrowInputManipulator : PointerManipulator
{
    private string _labelValue;
    private Label _label;
    private string[] _values;
    private TextField _textField;
    private Button _leftArrow;
    private Button _rightArrow;
    private Action<int, string> _onArrowClick;
    private int _index;
    private int _defaultIndex;

    public String Value { get { return _textField.value; } }
    public int Index { get { return _index; } }










}
