using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class NetNpcWindows
{
    static void ShowQuests(NetQuestList list)
    {
        QuestWindow window = QuestPacketServices.Quest;
        if (window == null || list.Active == null)
            return;
        var quests = new List<QuestInstance>(list.Active.Length);
        for (int i = 0; i < list.Active.Length; i++)
        {
            NetQuestJournal row = list.Active[i];
            int flags = row.JournalWord != 0 ? row.JournalWord : row.Cond;
            if (flags == 0)
                flags = 1;
            quests.Add(new QuestInstance(row.QuestId, flags, row.Repeatable ? 1 : 0, row.Name));
        }

        try
        {
            // Enter-world also pushes QuestList to prefill the journal — never auto-open here.
            // MenuWindow toggles the window before RequestQuestList; AddData fills whatever is visible.
            window.AddData(quests);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] quest list: " + ex.Message);
        }
    }

    public static void ApplyQuestList(NetQuestList list) => ShowQuests(list);

    public static void ApplyAcquireSkillList(AcquireSkillListDto list)
    {
        SkillLearnWindow window = SkillLearnPacketServices.SkillLearn;
        if (window == null || list == null || list.AcquireList == null)
            return;
        Close();
        try
        {
            window.ShowWindow();
            window.AddData(list.AcquireList);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] acquire skill list: " + ex.Message);
        }
    }

    public static void ApplyAcquireSkillInfo(AcquireSkillInfoDto info)
    {
        DescriptionSkillWindow window = SkillLearnPacketServices.SkillDesc;
        if (window == null || info == null)
            return;
        try
        {
            window.ShowWindow();
            window.AddData(info);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] acquire skill info: " + ex.Message);
        }
    }

    public static void ApplyEnchantResult(NetEnchantResult result)
    {
        EnchantWindow window = ItemEnchantPacketServices.Enchant;
        try
        {
            window?.ApplyZoneResult(result);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] enchant result: " + ex.Message);
        }
    }

}
