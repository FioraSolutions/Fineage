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
