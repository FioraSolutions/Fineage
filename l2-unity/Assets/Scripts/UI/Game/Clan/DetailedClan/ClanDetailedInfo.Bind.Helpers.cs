using FMOD.Studio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanDetailedInfo
{
    private void OnClickHide(int id)
    {
        _showPanel = -1;
    }

    private void OnClickApplyPrivilege(int useRank)
    {
        List<SettingCheckBox> listSelect = _createPanelCheckBox.GetSelectAllCheckBoxs();
        int newPrivileges = GetCalcNewRank(listSelect);

        if(newPrivileges != -1)
        {
            ZoneCommands.Send(new RequestPledgePowerCommand(useRank, 2, newPrivileges));

        }


    }

    private void OnSwitchSubWindow(int id)
    {
        PledgeReceivePowerInfoDto powerInfo = new PledgeReceivePowerInfoDto();
        powerInfo.PowerGrade = id;
        _privilegesInfoContent.PreShow(powerInfo, _detailedInfoElement);
    }

    private int GetCalcNewRank(List<SettingCheckBox> list)
    {
        if(list.Count == 1)
        {
            return list[0].GetData();
        }

        int[] dataArray = list.Where(cb => cb.GetData() != 0)
                               .Select(cb => cb.GetData())
                               .ToArray();

        return ByteUtils.CombineFlags(dataArray);
    }
}
