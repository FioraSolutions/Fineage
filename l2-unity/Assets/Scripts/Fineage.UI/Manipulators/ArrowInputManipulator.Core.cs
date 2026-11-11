using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ArrowInputManipulator : PointerManipulator
{
    public ArrowInputManipulator(VisualElement target, string label, string[] values, int defaultIndex, Action<int, string> onArrowClick)
    {
        _labelValue = label;
        this.target = target;
        if (_labelValue != null && _labelValue.Length > 0)
        {
            _label.text = label;
        }

        _values = values;
        _index = defaultIndex;

        if (defaultIndex != -1)
        {
            _textField.value = values[defaultIndex];
        }

        _onArrowClick = onArrowClick;
    }

    public void UpdateValues(string[] newVals)
    {
        _values = newVals;
        _index = -1;
        _textField.value = "";
    }

    private void LoadElements()
    {
        if (_labelValue != null && _labelValue.Length > 0)
        {
            _label = target.Q<Label>("Label");
        }
        _textField = target.Q<TextField>("DataField");
        _leftArrow = target.Q<Button>("LeftArrow");
        _rightArrow = target.Q<Button>("RightArrow");
    }

    protected override void RegisterCallbacksOnTarget()
    {
        LoadElements();
        _leftArrow.RegisterCallback<ClickEvent>(OnLeftArrowClick);
        _rightArrow.RegisterCallback<ClickEvent>(OnRightArrowClick);
        _leftArrow.AddManipulator(new ButtonClickSoundManipulator(_leftArrow));
        _rightArrow.AddManipulator(new ButtonClickSoundManipulator(_rightArrow));
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        _leftArrow.UnregisterCallback<ClickEvent>(OnLeftArrowClick);
        _rightArrow.UnregisterCallback<ClickEvent>(OnRightArrowClick);
    }
}
