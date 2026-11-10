using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow
{
    public void AddClanData(PledgeShowMemberListAllDto packet )
    {
        _packet = packet;
        if (_windowEle == null || _dropdown == null)
        {
            return;
        }

        ApplyClanData(packet);
    }

    private void ApplyClanData(PledgeShowMemberListAllDto packet)
    {
        _dataProviderClanInfo.SetMasterClanInfo(_windowEle , packet);
        _listDropDown = _masterClan.SetDropdownList(_dropdown , packet.PledgeName);
        _masterClan.CreateMembersTable(packet.Members, _creatorTableWindows);
        SetDisabledButton(packet.SubPledgeLeaderName , packet.Members);
    }

    //not wornig 
    public void DeleteMemberData()
    {
        //_packet.Members.Clear();
        //_creatorTableWindows.UpdateTableData(_masterClan.GetEmptyData());
        //_masterClan.Up(_creatorTableWindows);
    }

    public void UpdateMemberData(PledgeShowMemberListUpdateDto packetUpdate)
    {
        ClanMember clanMember = new ClanMember(packetUpdate.MemberName, packetUpdate.Level, packetUpdate.ClassId, packetUpdate.Sex, packetUpdate.Race, packetUpdate.IsOnline, packetUpdate.PledgeType);
        _masterClan.UpdateMemberData(clanMember, _packet, _creatorTableWindows);
    }

    public void DeleteMemberData(PledgeShowMemberListDeleteDto packetDelete)
    {
        _masterClan.DeleteMemeberTable(packetDelete.MemberName, _packet, _creatorTableWindows);
    }

    public void AddMemberData(PledgeShowMemberListAddDto packetAdd)
    {
        _masterClan.AddMemberData(packetAdd.ClanMember, _packet, _creatorTableWindows);
    }

    public void UpdatePledge(PledgeInfoDto pledge)
    {
        _dataProviderClanInfo.UpdateClanInfo(_windowEle, pledge);
    }

    public void UpdateClanIdInfo(PledgeStatusChangedDto changed)
    {
        if(_packet != null & changed != null)
        {
            _packet.ClanId = changed.ClanId;
            _packet.CrestId = changed.CrestId;
            _packet.LeaderId = changed.LeaderId;
            _packet.AllyId = changed.AllyId;
            _packet.AllyCrestId = changed.AllyCrestId;

        }
    }



    public void SelectMember(int selectIndex, string select_text)
    {
        if (_detailedInfoElement.style.display == DisplayStyle.Flex)
        {


            switch (_detailedClan.GetShowPanel())
            {
                case 0:
                    OnClickShowMember(null);
                    break;

                case 1:
                    OnClickPrivileges(null);
                    break;
                default:
                    break;
            }
        }
    }

    private const string USS_STYLE_YELLOW = "button-label-yellow";
    private const string USS_STYLE_DISABLED = "button-label-disabled";
    private void SetDisabledButton(string leaderName, List<ClanMember> Members)
    {
        string userName = GetSelfName();
        if (userName == null || _labelEditAuth == null)
        {
            return;
        }

        ResetAllButtons();

        if (IsLeader(leaderName))
            EnabledLeaderButtons();
        else
        {
            if (Members.FirstOrDefault(m => m.MemberName == userName) == null)
                EnabledUnkButtons();
            else
                EnabledUserButtons();
        }
    }


    private void ResetAllButtons()
    {
        _labelEditAuth.AddToClassList(USS_STYLE_DISABLED);
        _labelMemberInfo.AddToClassList(USS_STYLE_DISABLED);
        _labelPrivileges.AddToClassList(USS_STYLE_DISABLED);
        _labelClanInfo.AddToClassList(USS_STYLE_DISABLED);
        _labelPenalty.AddToClassList(USS_STYLE_DISABLED);
        _labelLeave.AddToClassList(USS_STYLE_DISABLED);
        _labelClanEntry.AddToClassList(USS_STYLE_DISABLED);
        _labelInvite.AddToClassList(USS_STYLE_DISABLED);
    }


    private void EnabledLeaderButtons()
    {
        _labelEditAuth.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelMemberInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPrivileges.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelClanInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPenalty.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelLeave.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelInvite.RemoveFromClassList(USS_STYLE_DISABLED);


        _labelEditAuth.AddToClassList(USS_STYLE_YELLOW);
        _labelMemberInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelPrivileges.AddToClassList(USS_STYLE_YELLOW);
        _labelClanInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelPenalty.AddToClassList(USS_STYLE_YELLOW);
        _labelLeave.AddToClassList(USS_STYLE_YELLOW);
        _labelInvite.AddToClassList(USS_STYLE_YELLOW);

    }

    private void EnabledUserButtons()
    {
        _labelMemberInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPrivileges.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPenalty.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelClanInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelLeave.RemoveFromClassList(USS_STYLE_DISABLED);

        _labelMemberInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelPrivileges.AddToClassList(USS_STYLE_YELLOW);
        _labelPenalty.AddToClassList(USS_STYLE_YELLOW);
        _labelClanInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelLeave.AddToClassList(USS_STYLE_YELLOW);
    }

    private void EnabledUnkButtons()
    {

        _labelPenalty.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelLeave.RemoveFromClassList(USS_STYLE_DISABLED);

        _labelPenalty.AddToClassList(USS_STYLE_YELLOW);
        _labelLeave.AddToClassList(USS_STYLE_YELLOW);

    }

    private void OkLeave()
    {
        ZoneCommands.Send(new RequestWithdrawPledgeCommand());
        CancelEvent();
    }

    private void OkInvite()
    {
        TargetManager targets = TargetPacketServices.Targets;
        if (targets != null && targets.HasTarget() && targets.Target.GetEntity() != null)
        {
            int id = targets.Target != null ? targets.Target.GetEntity().Identity.Id : 0;

            ZoneCommands.Send(new RequestJoinPledgeCommand(id));
        }

        CancelEvent();
    }

    private void OnРЎancel()
    {
        CancelEvent();
    }
    private void CancelEvent()
    {
        SystemMessagePacketServices.Window.OnButtonOk -= OkLeave;
        SystemMessagePacketServices.Window.OnButtonClosed -= OnРЎancel;
    }


}
