

public partial class ToolTipSkill
{
    private void SetData(Skillgrp skillgrp, SkillNameData skillNameData)
    {
        string hp = skillgrp.HpConsume.ToString();
        string mp = skillgrp.MpConsume.ToString();
        string radius = skillgrp._cast_range.ToString();
        string desc = skillNameData.Desc;
        string time = skillgrp.ReuseDelay.ToString();
        string name = skillNameData.Name;
        string level = skillgrp.Level.ToString();
        string nameActiveSkill = GetActive(skillgrp.OperateType);

        if (desc.Equals("")) desc = TablePacketServices.SkillNames.GetName(skillgrp.Id, 1).Desc;


        SetDataTooTip(DataEmpty(hp), DataEmpty(mp), DataEmpty(radius), desc, DataEmpty(time), nameActiveSkill , name, level);
        L2SlotIconView.SetNamedIcon(_icon, skillgrp.Icon);
        HideEmptyData(DataEmpty(hp), DataEmpty(mp), DataEmpty(radius), desc, DataEmpty(time), nameActiveSkill, name, level);
        ShowHidenData(DataEmpty(hp), DataEmpty(mp), DataEmpty(radius), desc, DataEmpty(time), nameActiveSkill, name, level);
    }

    private string GetActive(int operate_type)
    {
        if(operate_type == 1)
        {
            return "Active Skill";
        }
        else if (operate_type == 2)
        {
            return "Passive Skill";
        }
        return "";
    }

    private string DataEmpty(string data)
    {
        if(data.Equals("0") | data.Equals("-1"))
        {
            return "";
        }
        return data;
    }
    private void SetDataTooTip(string hp , string mp , string radius , string descripted , string cast , string type , string name , string lvl)
    {
        if (!string.IsNullOrEmpty(hp))
        {
            if (!hp.Equals("0"))
            {
                _hpLabel.text = hp;
            }
            
        }

        if (!string.IsNullOrEmpty(mp))
        {
            if (!mp.Equals("0"))
            {
                _mpLabel.text = mp;
            }
                
        }

        if (!string.IsNullOrEmpty(radius))
        {
            if (!mp.Equals("-1"))
            {
                _rangeLabel.text = radius;
            }
               
        }

        if (!string.IsNullOrEmpty(descripted))
        {
            _descriptedLabel.text = descripted;
        }

        if (!string.IsNullOrEmpty(cast))
        {
            _castlabel.text = cast + " sec";
        }

        if (!string.IsNullOrEmpty(type))
        {
            _typeSkill.text = type;
        }

        if (!string.IsNullOrEmpty(name))
        {
            _nameSkill.text = name;
        }

        if (!string.IsNullOrEmpty(lvl))
        {
            _lvlSkill.text = lvl;
        }

    }

    private void HideEmptyData(string hp, string mp, string radius, string descripted, string cast, string type, string name, string lvl)
    {
        if (string.IsNullOrEmpty(hp))
        {
            _hpBlockText.style.display = DisplayStyle.None;
        }

        if (string.IsNullOrEmpty(mp))
        {
            _mpBlockText.style.display = DisplayStyle.None;
        }

        if (string.IsNullOrEmpty(radius))
        {
            _radiusBlockText.style.display = DisplayStyle.None;
        }

        if (string.IsNullOrEmpty(descripted))
        {
            _descriptedText.style.display = DisplayStyle.None;
        }

        if (string.IsNullOrEmpty(cast))
        {
            _castBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(type))
        {
           // _typeBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(name))
        {
           // _hpBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(lvl))
        {
           // _hpBlockText.style.display = DisplayStyle.None;
        }
    }

    private void ShowHidenData(string hp, string mp, string radius, string descripted, string cast, string type, string name, string lvl)
    {
        if (!string.IsNullOrEmpty(hp))
        {
            _hpBlockText.style.display = DisplayStyle.Flex;
        }

        if (!string.IsNullOrEmpty(mp))
        {
            _mpBlockText.style.display = DisplayStyle.Flex;
        }

        if (!string.IsNullOrEmpty(radius))
        {
            _radiusBlockText.style.display = DisplayStyle.Flex;
        }

        if (!string.IsNullOrEmpty(descripted))
        {
            _descriptedText.style.display = DisplayStyle.Flex;
        }

        if (!string.IsNullOrEmpty(cast))
        {
            _castBlockText.style.display = DisplayStyle.Flex;
        }
        //always given
        if (string.IsNullOrEmpty(type))
        {
            // _typeBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(name))
        {
            // _hpBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(lvl))
        {
            // _hpBlockText.style.display = DisplayStyle.None;
        }
    }

}
