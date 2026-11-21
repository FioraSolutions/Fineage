using System.Text;
using UnityEngine.UIElements;
using UnityEngine;

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
        RefreshAbnormalTooltip(skillId, level);
    }

    void RefreshAbnormalTooltip(int skillId, int level)
    {
        if (_element == null || skillId <= 0)
            return;

        int safeLevel = level > 0 ? level : 1;
        SkillNameData nameData = TablePacketServices.SkillNames?.GetName(skillId, safeLevel);
        if (nameData == null && TablePacketServices.SkillNames == null)
            return;

        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(nameData?.Name))
            sb.Append(nameData.Name);
        if (safeLevel > 0)
        {
            if (sb.Length > 0)
                sb.Append(' ');
            sb.Append("Lv.").Append(safeLevel);
        }
        if (!string.IsNullOrEmpty(nameData?.Desc))
        {
            if (sb.Length > 0)
                sb.Append('\n');
            sb.Append(nameData.Desc);
        }

        string tip = sb.Length > 0 ? sb.ToString() : ("Skill " + skillId + " Lv." + safeLevel);
        if (_tooltipManipulator != null)
        {
            _element.RemoveManipulator(_tooltipManipulator);
            _tooltipManipulator = null;
        }

        _tooltipManipulator = new TooltipManipulator(_element, tip);
        _element.AddManipulator(_tooltipManipulator);
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
