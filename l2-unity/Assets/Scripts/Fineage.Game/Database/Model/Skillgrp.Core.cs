using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Skillgrp
{
    public string GetActiveSkillNameOrNot()
    {
        if (_operate_type == 1 | _operate_type == 0)
        {
            return _activeSkillName;
        }
        else if (_operate_type == 2)
        {
            return _passiveSkillName;
        }
        else if (_operate_type == 3)
        {
            return _selfSkillName;
        }
        return "";
    }
}
