using System;

public partial class QuestWindow
{
    public void AddData(List<QuestInstance> quests)
    {
        //if (_questsList != null) _questsList.Clear();

        repeatQuests = quests.Where(q => q.IsRepeat()).ToList();
        nonRepeatQuests = quests.Where(q => !q.IsRepeat()).ToList();

        _questTabPanelRepeat.AddElementsToContent(repeatQuests);
        _questTabPanelSingle.AddElementsToContent(nonRepeatQuests);

        if (!IsSuccess())
        {
            HideDetailedInfo();
        } 
    }

    private void ToggleDetailedInfo(List<QuestInstance> list)
    {
        if (list == null || list.Count == 0)
        {
            _detailedInfoElement.style.display = DisplayStyle.None;
        }
    }

    private void AddTestRewardData()
    {
        _questsList.Add(new OtherModel(new ModelQuestDemoReward("XP", "10,000", "etc_exp_point_i00")));
        _questsList.Add(new OtherModel(new ModelQuestDemoReward("Unknown Reward", "Undecided", "etc_pi_gift_box_i04")));
    }

}
