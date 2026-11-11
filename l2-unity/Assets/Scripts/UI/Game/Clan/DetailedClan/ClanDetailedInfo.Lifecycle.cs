using FMOD.Studio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanDetailedInfo
{
    public void  SetDetailedInfoElement(VisualElement detailedInfoElement)
    {
        _detailedInfoElement = detailedInfoElement;
    }

    public ClanDetailedInfo(DataProviderClanInfo dataProvider)
    {
        _dataProvider = dataProvider;

        _createPanelCheckBox = new CreatePanelCheckBoxWindows();
        _creatorSkillsPanel = new CreatorSkillsPanel();

        _memberInfoContent = new MemberInfoContent(_dataProvider);
        _memberInfoContent.OnClickHide += OnClickHide;
        _memberInfoContent.OnOutsideClickRank += OnClickOkRank;
        _memberInfoContent.OnOutsideClickTitle += OnClickOkTitle;
        _memberInfoContent.OnOutsideClickDeleteTitle += OnDeleteTitle;
        _memberInfoContent.OnOutsideClickDismiss += OnOkDismiss;


        _privilegesInfoContent = new PrivilegesInfoContent(_dataProvider , _createPanelCheckBox);
        _privilegesInfoContent.OnClickHide += OnClickHide;
        _privilegesInfoContent.OnClickApply += OnClickApplyPrivilege;

        _clanInfoContent = new ClanInfoContent(_dataProvider , _creatorSkillsPanel);
        _clanInfoContent.OnClickHide += OnClickHide;

        _rankingPrivilege = new RankingPrivelege(_dataProvider);
        _rankingPrivilege.OnClickHide += OnClickHide;
        _rankingPrivilege.OnSwitchRank += OnSwitchSubWindow;
    }

    public void UpdateDetailedInfo(object packet, VisualElement detailedInfoElement, PledgeShowMemberListAllDto packetAll)
    {
        switch (packet)
        {
            case PledgeReceiveMemberInfoDto memberInfo:
                _memberInfoContent.PreShow(memberInfo, detailedInfoElement, packetAll);
                _showPanel = 0;
                break;

            case PledgeReceivePowerInfoDto powerInfo:
                //if leader max access
                if (powerInfo.Name == packetAll.SubPledgeLeaderName) powerInfo.PowerGradeByRank = ClanPrivileges.CP_ALL;

                _privilegesInfoContent.PreShow(powerInfo, detailedInfoElement);
                _showPanel = 1;
                break;
            case PledgeClanInfoDto clanInfo:
                _clanInfoContent.PreShow(clanInfo, detailedInfoElement, packetAll);
                _showPanel = 2;
                break;
            case PledgePowerGradeListDto authInfo:
                _rankingPrivilege.PreShow(authInfo, detailedInfoElement, packetAll);
                _showPanel = 3;
                break;
            case ManagePledgePowerDto pladgePower:
                _privilegesInfoContent.PreShow(pladgePower, detailedInfoElement);
                _showPanel = 1;
                break;
            default:
                // Handle unknown packet types if necessary
                break;
        }
    }

    public int GetShowPanel()
    {
        return _showPanel;
    }

    private void OnClickOkRank(string memberName, int rank)
    {
    }

    private void OnClickOkTitle(string memberName, string title)
    {
        ZoneCommands.Send(new RequestGiveNickNameCommand(memberName, title));

        //Debug.Log("Click Ok OnClickOkTitle memberName " + memberName + " Rank " + title);
    }

    private void OnDeleteTitle(string memberName)
    {
        ZoneCommands.Send(new RequestGiveNickNameCommand(memberName, ""));
    }
}
