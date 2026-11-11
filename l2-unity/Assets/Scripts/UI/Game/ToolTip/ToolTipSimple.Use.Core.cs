using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    public void EventLeftClickSlot(VisualElement ve)
    {
        if (ve == null || _windowEle == null)
            return;

        string[] ids = GetUniquePosition(ve);
        if (ids == null || ids.Length < 2)
            return;

        var template = SwitchToolTip(ids, true);

        if(template != null)
        {
            int position = Int32.Parse(ids[0]);
            int type = Int32.Parse(ids[1]);

            object data = GetDataClickLeft(type, position);

            if(data != null)
            {
                AddData(data, template);
                _selectShow = ve;
                _windowEle.style.display = DisplayStyle.Flex;
                _showToolTip.Show(ve, GetForceBelow(ids));
                return;
            }
        }

        // Nothing to describe (empty bag / gear slot): the click must not leave a stale tooltip.
        // Only the panel is hidden here — Hide() also drops the buff countdown subscriptions of
        // whatever tooltip the pointer was on, and a click on an empty cell must not do that.
        _windowEle.style.display = DisplayStyle.None;
    }

    private void AddData(object data , TemplateContainer template)
    {
        if(data.GetType() == typeof(Product))
        {
            Product product = (Product)data;
            UseProduct(product, template);
        }else if (data.GetType() == typeof(ItemInstance))
        {
            ItemInstance item = (ItemInstance)data;
            UseItem(item, template);
        }else if (data.GetType() == typeof(SkillInstance))
        {
            SkillInstance item = (SkillInstance)data;
            UseSkill( item, template);
        }
  
    }
}
