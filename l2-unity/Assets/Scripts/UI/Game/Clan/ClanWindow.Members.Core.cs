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
}
