using UnityEngine;
using UnityEngine.UIElements;

public partial class DataCell : L2Slot
{
    public DataCell(int idSkill , VisualElement element  ,  VisualElement labelTemplate, VisualElement slot  , int position) :
        base(slot, position, SlotType.BuffPanel)
    {
        _idSkill = idSkill;
        _element = element;
        _isBusy = false;
        _position = position;
        _label = labelTemplate.Q<Label>("LabelText");
        _labelShadow = labelTemplate.Q<Label>("LabelTextShadow");
        _labelTemplate = labelTemplate;
        _skillInstance = new SkillInstance(idSkill , 0 , false , false);
        RegisterLabelChange(_label);
    }

    public void ResetData()
    {
        SetSkillId(-1);
        SetLevel(-1);
        SetBusy(false);
        SetText("");
        ResetDurationSmooth();
        _activeTime = 0;
        _startTime = 0;
        _lastRemaningTime = 0;
        _skillInstance.RefreshData(-1, -1, false, false);
        _skill = null;
    }

    public void SetText(string text)
    {
        _label.text = text;
    }

    public void RefreshData(int skillId , bool isbusy ,int level)
    {
        _skill = TablePacketServices.SkillGrps.GetSkill(skillId , level);
        SetSkillId(skillId);
        SetLevel(level);
        SetBusy(isbusy);
        _skillInstance.RefreshData(skillId , level , IsPassiveSkill() , false);
        if (isbusy != false) SetIcon(skillId, level);
    }

    public SkillInstance GetSkillInstance()
    {
        return _skillInstance;
    }

    public int GetSkillId()
    {
        return _idSkill;
    }

    public void SetSkillId(int skillId)
    {
        _idSkill = skillId;
        _skillInstance.RefreshData(skillId , _level , IsPassiveSkill() , false);
    }

    public void SetLevel(int level)
    {
        _level = level;
        _skillInstance.RefreshData(_idSkill, level, IsPassiveSkill(), false);
    }

    public void ResetDurationSmooth()
    {
        _durationSmooth = 0;
    }

    public int GetLevel()
    {
        return _level;
    }

    public bool IsBusy()
    {
        return _isBusy;
    }

    public void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
    }

    public bool IsPassiveSkill()
    {
        if (_skill != null)
        {
            if (_skill.OperateType == 2) return true;
        }
        
        return false;
    }
}
