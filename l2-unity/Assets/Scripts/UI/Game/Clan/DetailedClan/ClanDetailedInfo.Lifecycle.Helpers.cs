using FMOD.Studio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanDetailedInfo
{
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
