using UnityEngine;
using UnityEngine.UIElements;

public partial class DataCell : L2Slot
{
    public void ShowCell(bool show)
    {
        if (show)
        {
            
            _element.style.display = DisplayStyle.Flex;
            _labelTemplate.style.display = DisplayStyle.Flex;
        }
        else
        {
            _element.style.display = DisplayStyle.None;
            _labelTemplate.style.display = DisplayStyle.None;
        }
        
    }

    public bool IsHideElements()
    {
        float opacity = _element.resolvedStyle.opacity;

        if (opacity >= 1 & _switchHide != true)
        {
            _switchHide = true;
            _switchShow = false;
            _durationSmooth = 0;
        }
        
        return _switchHide;
    }

    public bool IsShowElements()
    {
        if (_element.resolvedStyle.opacity <= 0.4f & _switchShow != true)
        {
            _switchShow = true;
            _switchHide = false;
            _durationSmooth = 0;
        }

        return _switchShow;
    }

    public void SetTimeDeltaTime(float deltatime)
    {
        _durationSmooth += deltatime;
    }

    public float GetSmoothTime()
    {
        return _durationSmooth;
    }

    public VisualElement GetElement()
    {
        return _element;
    }

    public int GetPosition()
    {
        return _position;
    }

    public void SetIcon(int skillId, int level)
    {
        if (_skill == null)
            _skill = TablePacketServices.SkillGrps.GetSkill(skillId, level);
        VisualElement foundChild = _element.Query<VisualElement>(name: "SlotBg").First();
        if (foundChild != null && _skill != null)
            L2SlotIconView.SetSkillIcon(foundChild, _skill.Icon);
    }

    public float GetRemainingTime(float time)
    {
        float elapsedTime = time - _startTime;
        float timeEnd = _activeTime - elapsedTime;
        _lastRemaningTime = Mathf.Max(0, timeEnd);
        return _lastRemaningTime;
    }

    public float GetLastRemainingTime()
    {
        return _lastRemaningTime;
    }

    public void SetActiveTime(float activeTime)
    {
        _activeTime  = activeTime;
        _startTime = Time.time;
    }

    public int GetIntervalUse()
    {
        return Mathf.RoundToInt(_activeTime * _intervalUse);
    }

    private void RegisterLabelChange(Label label)
    {
        label.RegisterValueChangedCallback(evt =>
        {
            _label.text = evt.newValue;
            _labelShadow.text = evt.newValue;
        });
    }
}
