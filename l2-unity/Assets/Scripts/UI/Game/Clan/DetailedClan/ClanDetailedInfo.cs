using FMOD.Studio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanDetailedInfo
{

    private const string _templateNameClanInfo = "Data/UI/_Elements/Game/Clan/DetailedContent/ClanInfoContent";
    private const string _templateNamePrivilegesInfo = "Data/UI/_Elements/Game/Clan/DetailedContent/PrivilegesInfoContent";
    private const string _templateNameRankingPrivelege = "Data/UI/_Elements/Game/Clan/DetailedContent/RankingPrivilege";
    private const string _templateNameMemberInfo = "Data/UI/_Elements/Game/Clan/DetailedContent/MemberInfoContent";

    private DataProviderClanInfo _dataProvider;
    private MemberInfoContent _memberInfoContent;

    private PrivilegesInfoContent _privilegesInfoContent;
    private RankingPrivelege _rankingPrivilege;
    private ClanInfoContent _clanInfoContent;
    private ICreatorPanelCheckBox _createPanelCheckBox;
    private ICreatorSkillsPanel _creatorSkillsPanel;
    private VisualElement _detailedInfoElement;

    private int _showPanel = -1;


   public void LoadAssets(Func<string, VisualTreeAsset> loaderFunc)
   {
        _memberInfoContent.template = loaderFunc(_templateNameMemberInfo);
        _privilegesInfoContent.template = loaderFunc(_templateNamePrivilegesInfo);
        _clanInfoContent.template = loaderFunc(_templateNameClanInfo);
        _rankingPrivilege.template = loaderFunc(_templateNameRankingPrivelege);

        _memberInfoContent.LoadAsset(loaderFunc);
        _createPanelCheckBox.LoadAsset(loaderFunc);
        _creatorSkillsPanel.LoadAsset(loaderFunc);
        _rankingPrivilege.LoadAsset(loaderFunc);

    }




    private string _selectDismiss = "";







}
