using System;
using System.Reflection.Emit;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

        if (desc.Equals(""))
        {
            SkillNameData fallback = SkillNames != null ? SkillNames.GetName(skillgrp.Id, 1) : null;
            if (fallback != null)
                desc = fallback.Desc;
        }


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
}
