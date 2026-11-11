using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    private void HandleTimeLeftUpdated(int skillId, float newTimeLeft, VisualElement elementToUpdate)
    {
        if (elementToUpdate != null)
        {
            Label label = (Label)elementToUpdate;
            label.text = TimeUtils.FormatTime(newTimeLeft);
        }
    }

    private bool GetForceBelow(string[] ids)
    {
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);

        switch (type)
        {
            case (int)SlotType.BuffPanel:
                return true;

            default: return false;
        }
    }

    private void SetSimpleSingleToolTip(Product product , TemplateContainer template)
    {
        if (product != null)
        {
            IDataTips text = ToolTipManager.GetInstance().GetProductText(product);
            var _descriptedText = (Label)template.Q<VisualElement>(null, "DescriptedLabel");
            var _nameText = (Label)template.Q<VisualElement>(null, "Heading");

            _descriptedText.style.fontSize = 12;
            _nameText.style.paddingLeft = 0;
            _descriptedText.style.color = ToolTipsUtils.GetColorPrice(text.GetPrice());

            var icon = template.Q<VisualElement>(null, "Icon");
            var groubBoxIcon = template.Q<VisualElement>(null, "Grow");

            SetIcon(icon , groubBoxIcon , null);
            string decription = text.GetPrice();

            if (!string.IsNullOrEmpty(decription) && decription.Equals("0") != true)
            {
                SetDataToolTip(_nameText, _descriptedText, text.GetName(), "Price: " + ToolTipsUtils.ConvertToPrice(Int32.Parse(decription)) + " Adena");
            }
            else
            {
                SetDataToolTip(_nameText, _descriptedText, text.GetName(), "");
            }
            
        }
        else
        {
            _windowEle.style.display = DisplayStyle.None;
            _showToolTip.Hide(null);
        }
    }

    private VisualElement SetSkillToolTip(SkillInstance skillInstance, TemplateContainer template , float leftTime = 0 )
    {
        IDataTips data = ToolTipManager.GetInstance().GetProductText(skillInstance);
       return  _dataProvider.SetDataSkillInTemplate(template, skillInstance, data , leftTime);
    }
}
