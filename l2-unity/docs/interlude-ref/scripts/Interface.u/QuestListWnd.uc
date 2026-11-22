class QuestListWnd extends UIScript;

var WindowHandle Me;
var ListCtrlHandle lstQuest;
var TextureHandle QuestTooltip;

function OnLoad()
{
	RegisterEvent(2840);
	RegisterEvent(2850);
	Me = GetHandle("QuestListWnd");
	QuestTooltip = TextureHandle(GetHandle("QuestListWnd.QuestTooltip"));
	lstQuest = ListCtrlHandle(GetHandle("QuestListWnd.lstQuest"));
	InitQuestTooltip();
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x18
		case 2840:
			HandleQuestInfoStart();
			// End:0x31
			break;
		// End:0x2E
		case 2850:
			HandleQuestInfo(param);
			// End:0x31
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1B
		case "btnLoc":
			ShowQuestTarget();
			// End:0x1E
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleQuestInfoStart()
{
	lstQuest.DeleteAllItem();
	Me.ShowWindow();
	Me.SetFocus();
	return;
}

function HandleQuestInfo(string param)
{
	local int QuestID;
	local string QuestName, QuestRequirement, QuestLevel;
	local int QuestType;
	local string QuestNpcName, QuestDecription;
	local LVDataRecord Record;

	ParseInt(param, "QuestID", QuestID);
	ParseString(param, "QuestName", QuestName);
	ParseString(param, "QuestRequirement", QuestRequirement);
	ParseString(param, "QuestLevel", QuestLevel);
	ParseInt(param, "QuestType", QuestType);
	ParseString(param, "QuestNpcName", QuestNpcName);
	ParseString(param, "QuestDecription", QuestDecription);
	Record.LVDataList.Length = 5;
	Record.LVDataList[0].szData = QuestName;
	Record.LVDataList[1].szData = QuestRequirement;
	Record.LVDataList[2].szData = QuestLevel;
	Record.LVDataList[3].nTextureWidth = 16;
	Record.LVDataList[3].nTextureHeight = 16;
	switch(QuestType)
	{
		// End:0x155
		case 0:
		// End:0x196
		case 2:
			Record.LVDataList[3].szTexture = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_1";
			// End:0x1DE
			break;
		// End:0x19A
		case 1:
		// End:0x1DB
		case 3:
			Record.LVDataList[3].szTexture = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_2";
			// End:0x1DE
			break;
		// End:0xFFFF
		default:
			break;
	}
	Record.LVDataList[3].szData = string(QuestType);
	Record.LVDataList[4].szData = QuestNpcName;
	Record.szReserved = QuestDecription;
	Record.nReserved1 = QuestID;
	lstQuest.InsertRecord(Record);
	return;
}

function ShowQuestTarget()
{
	local int idx, QuestID, NpcID;
	local string strTargetName;
	local Vector vTargetPos;
	local LVDataRecord Record;

	idx = lstQuest.GetSelectedIndex();
	// End:0x4E
	if(__NFUN_151__(idx, -1))
	{
		Record = lstQuest.GetRecord(idx);
		QuestID = Record.nReserved1;
	}
	// End:0x176
	if(__NFUN_151__(QuestID, 0))
	{
		NpcID = Class'NWindow.UIDATA_QUEST'.static.GetStartNPCID(QuestID, 1);
		strTargetName = Class'NWindow.UIDATA_NPC'.static.GetNPCName(NpcID);
		vTargetPos = Class'NWindow.UIDATA_QUEST'.static.GetStartNPCLoc(QuestID, 1);
		Debug(__NFUN_112__("QuestID=", string(QuestID)));
		Debug(__NFUN_112__("strTargetName=", strTargetName));
		Debug(__NFUN_112__("vTargetPos.x=", string(vTargetPos.X)));
		Debug(__NFUN_112__("vTargetPos.y=", string(vTargetPos.Y)));
		Debug(__NFUN_112__("vTargetPos.z=", string(vTargetPos.Z)));
		// End:0x176
		if(__NFUN_151__(__NFUN_125__(strTargetName), 0))
		{
			Class'NWindow.QuestAPI'.static.SetQuestTargetInfo(true, true, true, strTargetName, vTargetPos, QuestID);
		}
	}
	return;
}

function InitQuestTooltip()
{
	local CustomTooltip TooltipInfo;

	TooltipInfo.DrawList.Length = 4;
	TooltipInfo.DrawList[0].eType = 2;
	TooltipInfo.DrawList[0].u_nTextureWidth = 16;
	TooltipInfo.DrawList[0].u_nTextureHeight = 16;
	TooltipInfo.DrawList[0].u_strTexture = "L2UI_CH3.QuestWnd.QuestWndInfoIcon_1";
	TooltipInfo.DrawList[1].eType = 1;
	TooltipInfo.DrawList[1].nOffSetX = 5;
	TooltipInfo.DrawList[1].t_bDrawOneLine = true;
	TooltipInfo.DrawList[1].t_strText = GetSystemString(861);
	TooltipInfo.DrawList[2].eType = 2;
	TooltipInfo.DrawList[2].nOffSetY = 2;
	TooltipInfo.DrawList[2].u_nTextureWidth = 16;
	TooltipInfo.DrawList[2].u_nTextureHeight = 16;
	TooltipInfo.DrawList[2].u_strTexture = "L2UI_CH3.QuestWnd.QuestWndInfoIcon_2";
	TooltipInfo.DrawList[2].bLineBreak = true;
	TooltipInfo.DrawList[3].eType = 1;
	TooltipInfo.DrawList[3].nOffSetY = 2;
	TooltipInfo.DrawList[3].nOffSetX = 5;
	TooltipInfo.DrawList[3].t_bDrawOneLine = true;
	TooltipInfo.DrawList[3].t_strText = GetSystemString(862);
	QuestTooltip.SetTooltipCustomType(TooltipInfo);
	return;
}
