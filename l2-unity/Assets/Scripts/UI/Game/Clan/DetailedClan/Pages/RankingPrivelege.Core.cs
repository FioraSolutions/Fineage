using FMOD.Studio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class RankingPrivelege : AbstractClanContent
{
    public RankingPrivelege(DataProviderClanInfo dataProvider)
    {
        _dataProvider = dataProvider;
        _creatorTableWindows = new CreatorTableWindows();
        _creatorTableWindows.OnRowClicked += SelectRank;
    }

    public void LoadAsset(Func<string, VisualTreeAsset> loaderFunc)
    {
        _loaderFunc = loaderFunc;
    }

    public void PreShow(PledgePowerGradeListDto rankingList, VisualElement detailedInfoElement, PledgeShowMemberListAllDto packetAll)
    {
        _rankingList = rankingList;
        content = LoadContent(content, detailedInfoElement);
        ClearClick(content);
        ClearContent(content);
        if (_creatorTableWindows != null) _creatorTableWindows.DestroyTable();

        Show(rankingList, detailedInfoElement, packetAll);
    }

    public void Show(PledgePowerGradeListDto rankingList, VisualElement detailedInfoElement, PledgeShowMemberListAllDto packetAll)
    {
        VisualElement page = ToolTipsUtils.CloneOne(template);
        VisualElement master_table_content_result = page.Q<VisualElement>(master_table_content);
        Button editButton = page.Q<Button>(button_edit_auth);
        editButton?.RegisterCallback<ClickEvent>(evt => OnClickEdit(evt));

        CreateTable(rankingList , master_table_content_result, _loaderFunc);

        if (content != null & page != null)
        {
            content.Add(page);
        }
    }
}
