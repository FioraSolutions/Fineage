using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ArrowInputManipulator : PointerManipulator
{
    private void OnLeftArrowClick(ClickEvent e)
    {
        if (_values == null || _values.Length == 0)
        {
            // Debug.Log("You can't use the arrow , no character found");
            return;
        }

        if (--_index < 0)
        {
            _index = _values.Length - 1;
        }

        _textField.value = _values[_index];

        _onArrowClick(_index, _textField.value);
    }

    private void OnRightArrowClick(ClickEvent e)
    {
        if (_values == null || _values.Length == 0)
        {
            // Debug.Log("You can't use the arrow , no character found");
            return;
        }

        if (++_index > _values.Length - 1)
        {
            _index = 0;
        }

        _textField.value = _values[_index];

        _onArrowClick(_index, _textField.value);
    }

    public void SelectIndex(int index)
    {
        if (index < 0 || index >= _values.Length)
        {
            // Debug.LogWarning($"Trying to select wrong index [{index}] for arrow manipulator.");
            return;
        }

        _index = index;
        _textField.value = _values[_index];
    }

    public void ClearInput()
    {
        _index = -1;
        _textField.value = "";
    }

    public void ResetInput()
    {
        _index = 0;
        _textField.value = _values[_index];
    }
}
