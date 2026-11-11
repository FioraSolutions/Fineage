using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorVerticalScrollWindows : ICreatorVerticalScroll
{
    public void AddOtherData(List<OtherModel> allItems)
    {
        if (_content != null && allItems != null)
        {
            _content.Clear();
            for (int i = 0; i < allItems.Count; i++)
            {
                object model = allItems[i].GetOtherModel();
                VisualElement slotElement = CloneTemplate(_templateItems);

                switch (model)
                {
                    case AcquireData acquireData:
                        _dataProvider.AddLearnSkill(acquireData.GetId(), acquireData.GetCost(), acquireData.GetValue1(), slotElement);
                        slotElement.RegisterCallback<MouseDownEvent>(evt => HandleClickDown(evt, acquireData), TrickleDown.TrickleDown);
                        break;

                    case ModelQuestDemoReward questData:
                        _dataProvider.AddRewardItem(questData.NameReward, questData.DecReward, questData.Icon, slotElement);
                        break;
                    default:
                        // Debug.LogWarning($"Unknown model type: {model?.GetType().Name}");
                        break;
                }

                _content.Add(slotElement);
            }
        }
    }

    public void HandleClickDown(MouseDownEvent evt , AcquireData item)
    {
        if (evt.button == 0)
        {
            OnLeftClick(item.GetId(), ItemCategory.None , -1);
        }
    }

    private void OnLeftClick(int itemId, ItemCategory category, int position)
    {
        EventLeftClick?.Invoke(itemId, category, position);
    }
}
