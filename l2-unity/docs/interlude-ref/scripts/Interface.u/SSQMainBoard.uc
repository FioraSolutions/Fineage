class SSQMainBoard extends UIScript;

const NC_PARTYMEMBER_MAX = 9;
const SSQR_STATUS = 1;
const SSQR_MAINEVENT = 2;
const SSQR_SEALSTATUS = 3;
const SSQR_PREINFO = 4;
const SSQT_NONE = 0;
const SSQT_DUSK = 1;
const SSQT_DAWN = 2;
const SSQE_TIMEATTACK = 0;
const SSQS_NONE = 0;
const SSQS_GREED = 1;
const SSQS_REVEAL = 2;
const SSQS_STRIFE = 3;

struct SSQStatusInfo
{
	var int m_nSSQStatus;
	var int m_nSSQTeam;
	var int m_nSelectedSeal;
	var int m_nContribution;
	var int m_nTeam1HuntingMark;
	var int m_nTeam2HuntingMark;
	var int m_nTeam1MainEventMark;
	var int m_nTeam2MainEventMark;
	var int m_nTeam1Per;
	var int m_nTeam2Per;
	var int m_nTeam1TotalMark;
	var int m_nTeam2TotalMark;
	var int m_nPeriod;
	var int m_nMsgNum1;
	var int m_nMsgNum2;
	var int m_nSealStoneAdena;
};

struct SSQPreStatusInfo
{
	var int m_nWinner;
	var int m_nRoomNum;
	var array<int> m_nSealNumArray;
	var array<int> m_nWinnerArray;
	var array<int> m_nMsgArray;
};

struct SSQMainEventInfo
{
	var int m_nSSQStatus;
	var int m_nEventType;
	var int m_nEventNo;
	var int m_nWinPoint;
	var int m_nTeam1Score;
	var int m_nTeam2Score;
	var string m_Team1MemberName[9];
	var string m_Team2MemberName[9];
};

var SSQStatusInfo g_sinfo;
var SSQPreStatusInfo g_sinfopre;
var bool m_bShowPreInfo;
var bool m_bRequest_SealStatus;
var bool m_bRequest_MainEvent;

function OnLoad()
{
	RegisterEvent(740);
	RegisterEvent(770);
	RegisterEvent(750);
	RegisterEvent(760);
	m_bRequest_SealStatus = false;
	m_bRequest_MainEvent = false;
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtSS", __NFUN_112__(GetSystemString(833), " - "));
	m_bShowPreInfo = false;
	SetSSQStatus();
	return;
}

function OnShow()
{
	Class'NWindow.UIAPI_TABCTRL'.static.SetTopOrder("SSQMainBoard.TabCtrl", 0, true);
	PlayConsoleSound(5);
	SetSSQStatus();
	return;
}

function OnHide()
{
	m_bRequest_SealStatus = false;
	m_bRequest_MainEvent = false;
	m_bShowPreInfo = false;
	Class'NWindow.UIAPI_TREECTRL'.static.Clear("SSQMainBoard.me_MainTree");
	Class'NWindow.UIAPI_TREECTRL'.static.Clear("SSQMainBoard.ss_MainTree");
	return;
}

function OnEvent(int Event_ID, string param)
{
	local int i, j, k, L;
	local string strTmp;
	local int m_nSSQStatus, m_nNeedPoint1, m_nNeedPoint2, sealnum, m_nSealID, m_nOwnerTeamID,
		m_nTeam1Mark, m_nTeam2Mark;

	local SSQMainEventInfo Info;
	local int eventnum, nEventType, roomnum, team1num, team2num;

	// End:0x284
	if(__NFUN_154__(Event_ID, 740))
	{
		ParseInt(param, "SuccessRate", g_sinfo.m_nSSQStatus);
		ParseInt(param, "Period", g_sinfo.m_nPeriod);
		ParseInt(param, "MsgNum1", g_sinfo.m_nMsgNum1);
		ParseInt(param, "MsgNum2", g_sinfo.m_nMsgNum2);
		ParseInt(param, "SSQTeam", g_sinfo.m_nSSQTeam);
		ParseInt(param, "SelectedSeal", g_sinfo.m_nSelectedSeal);
		ParseInt(param, "Contribution", g_sinfo.m_nContribution);
		ParseInt(param, "SealStoneAdena", g_sinfo.m_nSealStoneAdena);
		ParseInt(param, "Team1HuntingMark", g_sinfo.m_nTeam1HuntingMark);
		ParseInt(param, "Team1MainEventMark", g_sinfo.m_nTeam1MainEventMark);
		ParseInt(param, "Team2HuntingMark", g_sinfo.m_nTeam2HuntingMark);
		ParseInt(param, "Team2MainEventMark", g_sinfo.m_nTeam2MainEventMark);
		ParseInt(param, "Team1Per", g_sinfo.m_nTeam1Per);
		ParseInt(param, "Team2Per", g_sinfo.m_nTeam2Per);
		ParseInt(param, "Team1TotalMark", g_sinfo.m_nTeam1TotalMark);
		ParseInt(param, "Team2TotalMark", g_sinfo.m_nTeam2TotalMark);
		SetSSQStatusInfo();
		SetSSQPreInfo();
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SSQMainBoard");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("SSQMainBoard");		
	}
	else
	{
		// End:0x3E7
		if(__NFUN_154__(Event_ID, 770))
		{
			ClearSSQPreInfo();
			ParseInt(param, "Winner", g_sinfopre.m_nWinner);
			ParseInt(param, "RoomNum", g_sinfopre.m_nRoomNum);
			i = 0;
			J0x2DB:

			// End:0x3DE [Loop If]
			if(__NFUN_150__(i, g_sinfopre.m_nRoomNum))
			{
				g_sinfopre.m_nSealNumArray.Insert(g_sinfopre.m_nSealNumArray.Length, 1);
				ParseInt(param, __NFUN_112__("SealNum_", string(i)), g_sinfopre.m_nSealNumArray[__NFUN_147__(g_sinfopre.m_nSealNumArray.Length, 1)]);
				g_sinfopre.m_nWinnerArray.Insert(g_sinfopre.m_nWinnerArray.Length, 1);
				ParseInt(param, __NFUN_112__("Winner_", string(i)), g_sinfopre.m_nWinnerArray[__NFUN_147__(g_sinfopre.m_nWinnerArray.Length, 1)]);
				g_sinfopre.m_nMsgArray.Insert(g_sinfopre.m_nMsgArray.Length, 1);
				ParseInt(param, __NFUN_112__("Msg_", string(i)), g_sinfopre.m_nMsgArray[__NFUN_147__(g_sinfopre.m_nMsgArray.Length, 1)]);
				__NFUN_165__(i);
				// [Loop Continue]
				goto J0x2DB;
			}
			SetSSQPreInfo();			
		}
		else
		{
			// End:0x763
			if(__NFUN_154__(Event_ID, 750))
			{
				ParseInt(param, "SSQStatus", m_nSSQStatus);
				Info.m_nSSQStatus = m_nSSQStatus;
				ParseInt(param, "EventNum", eventnum);
				i = 0;
				J0x442:

				// End:0x760 [Loop If]
				if(__NFUN_150__(i, eventnum))
				{
					ParseInt(param, __NFUN_112__("EventType_", string(i)), nEventType);
					Info.m_nEventType = nEventType;
					ParseInt(param, __NFUN_112__("RoomNum_", string(i)), roomnum);
					j = 0;
					J0x4B0:

					// End:0x756 [Loop If]
					if(__NFUN_150__(j, roomnum))
					{
						ParseInt(param, __NFUN_112__(__NFUN_112__(__NFUN_112__("EventNo_", string(i)), "_"), string(j)), Info.m_nEventNo);
						ParseInt(param, __NFUN_112__(__NFUN_112__(__NFUN_112__("WinPoint_", string(i)), "_"), string(j)), Info.m_nWinPoint);
						ParseInt(param, __NFUN_112__(__NFUN_112__(__NFUN_112__("Team2Score_", string(i)), "_"), string(j)), Info.m_nTeam2Score);
						ParseInt(param, __NFUN_112__(__NFUN_112__(__NFUN_112__("Team2Num_", string(i)), "_"), string(j)), team2num);
						k = 0;
						J0x59E:

						// End:0x621 [Loop If]
						if(__NFUN_150__(k, team2num))
						{
							ParseString(param, __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Team2MemberName_", string(i)), "_"), string(j)), "_"), string(k)), strTmp);
							// End:0x617
							if(__NFUN_151__(__NFUN_125__(strTmp), 0))
							{
								Info.m_Team2MemberName[k] = strTmp;
							}
							__NFUN_165__(k);
							// [Loop Continue]
							goto J0x59E;
						}
						ParseInt(param, __NFUN_112__(__NFUN_112__(__NFUN_112__("Team1Score_", string(i)), "_"), string(j)), Info.m_nTeam1Score);
						ParseInt(param, __NFUN_112__(__NFUN_112__(__NFUN_112__("Team1Num_", string(i)), "_"), string(j)), team1num);
						L = 0;
						J0x693:

						// End:0x716 [Loop If]
						if(__NFUN_150__(L, team1num))
						{
							ParseString(param, __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Team1MemberName_", string(i)), "_"), string(j)), "_"), string(L)), strTmp);
							// End:0x70C
							if(__NFUN_151__(__NFUN_125__(strTmp), 0))
							{
								Info.m_Team1MemberName[L] = strTmp;
							}
							__NFUN_165__(L);
							// [Loop Continue]
							goto J0x693;
						}
						AddSSQMainEvent(Info);
						ClearSSQMainEventInfo(Info);
						Info.m_nSSQStatus = m_nSSQStatus;
						Info.m_nEventType = nEventType;
						__NFUN_165__(j);
						// [Loop Continue]
						goto J0x4B0;
					}
					__NFUN_165__(i);
					// [Loop Continue]
					goto J0x442;
				}				
			}
			else
			{
				// End:0x8BA
				if(__NFUN_154__(Event_ID, 760))
				{
					ParseInt(param, "SSQStatus", m_nSSQStatus);
					ParseInt(param, "NeedPoint1", m_nNeedPoint1);
					ParseInt(param, "NeedPoint2", m_nNeedPoint2);
					ParseInt(param, "SealNum", sealnum);
					i = 0;
					J0x7E5:

					// End:0x8BA [Loop If]
					if(__NFUN_150__(i, sealnum))
					{
						ParseInt(param, __NFUN_112__("SealID_", string(i)), m_nSealID);
						ParseInt(param, __NFUN_112__("OwnerTeamID_", string(i)), m_nOwnerTeamID);
						ParseInt(param, __NFUN_112__("Team2Mark_", string(i)), m_nTeam2Mark);
						ParseInt(param, __NFUN_112__("Team1Mark_", string(i)), m_nTeam1Mark);
						AddSSQSealStatus(m_nSSQStatus, m_nNeedPoint1, m_nNeedPoint2, m_nSealID, m_nOwnerTeamID, m_nTeam1Mark, m_nTeam2Mark);
						__NFUN_165__(i);
						// [Loop Continue]
						goto J0x7E5;
					}
				}
			}
		}
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x46
		case "s_btnRenew":
			// End:0x33
			if(m_bShowPreInfo)
			{
				Class'NWindow.SSQAPI'.static.RequestSSQStatus(4);				
			}
			else
			{
				Class'NWindow.SSQAPI'.static.RequestSSQStatus(1);
			}
			// End:0x126
			break;
		// End:0x89
		case "s_btnPreview":
			m_bShowPreInfo = __NFUN_129__(m_bShowPreInfo);
			// End:0x80
			if(m_bShowPreInfo)
			{
				Class'NWindow.SSQAPI'.static.RequestSSQStatus(4);
			}
			SetSSQStatus();
			// End:0x126
			break;
		// End:0xA2
		case "ss_btnRenew":
			ShowSSQSealStatus();
			// End:0x126
			break;
		// End:0xBB
		case "me_btnRenew":
			ShowSSQMainEvent();
			// End:0x126
			break;
		// End:0xD1
		case "TabCtrl0":
			SetSSQStatus();
			// End:0x126
			break;
		// End:0xFA
		case "TabCtrl1":
			// End:0xF7
			if(__NFUN_129__(m_bRequest_MainEvent))
			{
				ShowSSQMainEvent();
				m_bRequest_MainEvent = true;
			}
			// End:0x126
			break;
		// End:0x123
		case "TabCtrl2":
			// End:0x120
			if(__NFUN_129__(m_bRequest_SealStatus))
			{
				ShowSSQSealStatus();
				m_bRequest_SealStatus = true;
			}
			// End:0x126
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function SetSSQStatus()
{
	// End:0x9E
	if(m_bShowPreInfo)
	{
		Class'NWindow.UIAPI_BUTTON'.static.SetButtonName("SSQMainBoard.s_btnPreview", 939);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SSQMainBoard.SSQStatusWnd_Preview");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SSQMainBoard.SSQStatusWnd_Status");		
	}
	else
	{
		Class'NWindow.UIAPI_BUTTON'.static.SetButtonName("SSQMainBoard.s_btnPreview", 937);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SSQMainBoard.SSQStatusWnd_Status");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SSQMainBoard.SSQStatusWnd_Preview");
	}
	return;
}

function SetSSQStatusInfo()
{
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtTime", __NFUN_112__(__NFUN_112__(string(g_sinfo.m_nPeriod), " "), GetSystemString(934)));
	// End:0x8B
	if(__NFUN_151__(g_sinfo.m_nMsgNum1, 0))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtSta1", GetSystemMessage(g_sinfo.m_nMsgNum1));		
	}
	else
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtSta1", "");
	}
	// End:0xFA
	if(__NFUN_151__(g_sinfo.m_nMsgNum2, 0))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtSta2", GetSystemMessage(g_sinfo.m_nMsgNum2));		
	}
	else
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtSta2", "");
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtMyTeamName", GetSSQTeamName(g_sinfo.m_nSSQTeam));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtMySealName", GetSSQSealName(g_sinfo.m_nSelectedSeal));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtMySealStoneCount", __NFUN_112__(string(g_sinfo.m_nContribution), GetSystemString(932)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtMySealStoneCountAdena", __NFUN_112__(__NFUN_112__(__NFUN_112__("(", string(g_sinfo.m_nSealStoneAdena)), GetSystemString(933)), ")"));
	// End:0x28A
	if(__NFUN_154__(g_sinfo.m_nSSQStatus, 3))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtAllStaCur", __NFUN_112__(" - ", GetSystemString(838)));		
	}
	else
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtAllStaCur", __NFUN_112__(" - ", GetSystemString(837)));
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtAllDawn", GetSSQTeamName(2));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtAllDusk", GetSSQTeamName(1));
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("SSQMainBoard.texDawnValue", int(__NFUN_172__(__NFUN_171__(float(g_sinfo.m_nTeam2Per), 150.0000000), 100.0000000)), 11);
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("SSQMainBoard.texDuskValue", int(__NFUN_172__(__NFUN_171__(float(g_sinfo.m_nTeam1Per), 150.0000000), 100.0000000)), 11);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtPointDawn", GetSSQTeamName(2));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtPointDusk", GetSSQTeamName(1));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtPointDawn1", __NFUN_112__("", string(g_sinfo.m_nTeam2HuntingMark)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtPointDawn2", __NFUN_112__("", string(g_sinfo.m_nTeam2MainEventMark)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtPointDawn3", __NFUN_112__("", string(g_sinfo.m_nTeam2TotalMark)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtPointDusk1", __NFUN_112__("", string(g_sinfo.m_nTeam1HuntingMark)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtPointDusk2", __NFUN_112__("", string(g_sinfo.m_nTeam1MainEventMark)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.txtPointDusk3", __NFUN_112__("", string(g_sinfo.m_nTeam1TotalMark)));
	return;
}

function ClearSSQPreInfo()
{
	g_sinfopre.m_nWinner = 0;
	g_sinfopre.m_nRoomNum = 0;
	g_sinfopre.m_nSealNumArray.Remove(0, g_sinfopre.m_nSealNumArray.Length);
	g_sinfopre.m_nWinnerArray.Remove(0, g_sinfopre.m_nWinnerArray.Length);
	g_sinfopre.m_nMsgArray.Remove(0, g_sinfopre.m_nMsgArray.Length);
	return;
}

function SetSSQPreInfo()
{
	local string strTmp;

	// End:0x63
	if(__NFUN_154__(g_sinfopre.m_nWinner, 1))
	{
		strTmp = GetSSQTeamName(1);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.pre_txtWinTeam", __NFUN_112__(__NFUN_112__(strTmp, " "), GetSystemString(828)));		
	}
	else
	{
		// End:0xC8
		if(__NFUN_154__(g_sinfopre.m_nWinner, 2))
		{
			strTmp = GetSSQTeamName(2);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.pre_txtWinTeam", __NFUN_112__(__NFUN_112__(strTmp, " "), GetSystemString(828)));			
		}
		else
		{
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.pre_txtWinTeam", "");
		}
	}
	// End:0x127
	if(__NFUN_155__(g_sinfopre.m_nWinner, 0))
	{
		strTmp = MakeFullSystemMsg(GetSystemMessage(1288), strTmp, "");		
	}
	else
	{
		strTmp = GetSystemMessage(1293);
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SSQMainBoard.pre_txtWinText", strTmp);
	AddSSQPreInfoSealStatus();
	return;
}

function AddSSQPreInfoSealStatus()
{
	local int i, nSealNum, nWinner, nMsgNum;
	local XMLTreeNodeInfo infNode;
	local XMLTreeNodeItemInfo infNodeItem;
	local XMLTreeNodeInfo infNodeClear;
	local XMLTreeNodeItemInfo infNodeItemClear;
	local string strRetName;

	Class'NWindow.UIAPI_TREECTRL'.static.Clear("SSQMainBoard.pre_MainTree");
	// End:0x6A
	if(__NFUN_150__(g_sinfopre.m_nSealNumArray.Length, 1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SSQMainBoard.pre_MainTree");
		return;		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SSQMainBoard.pre_MainTree");
	}
	infNode.strName = "root";
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SSQMainBoard.pre_MainTree", "", infNode);
	// End:0x124
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert root node. Name: ", infNode.strName));
		return;
	}
	infNode = infNodeClear;
	infNode.strName = "node";
	infNode.nOffSetX = 2;
	infNode.nOffSetY = 3;
	infNode.bShowButton = 0;
	infNode.bDrawBackground = 1;
	infNode.bTexBackHighlight = 1;
	infNode.nTexBackHighlightHeight = 17;
	infNode.nTexBackWidth = 240;
	infNode.nTexBackUWidth = 211;
	infNode.nTexBackOffSetX = -3;
	infNode.nTexBackOffSetY = -4;
	infNode.nTexBackOffSetBottom = 2;
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SSQMainBoard.pre_MainTree", "root", infNode);
	// End:0x250
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	i = 0;
	J0x257:

	// End:0x5CE [Loop If]
	if(__NFUN_150__(i, g_sinfopre.m_nSealNumArray.Length))
	{
		nSealNum = g_sinfopre.m_nSealNumArray[i];
		nWinner = g_sinfopre.m_nWinnerArray[i];
		nMsgNum = g_sinfopre.m_nMsgArray[i];
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		infNodeItem.t_strText = __NFUN_112__(GetSSQSealName(nSealNum), " : ");
		infNodeItem.nOffSetX = 4;
		infNodeItem.nOffSetY = 0;
		infNodeItem.t_color.R = 128;
		infNodeItem.t_color.G = 128;
		infNodeItem.t_color.B = 128;
		infNodeItem.t_color.A = byte(255);
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.pre_MainTree", strRetName, infNodeItem);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		// End:0x3B2
		if(__NFUN_154__(nWinner, 1))
		{
			infNodeItem.t_strText = GetSSQTeamName(1);			
		}
		else
		{
			// End:0x3D4
			if(__NFUN_154__(nWinner, 2))
			{
				infNodeItem.t_strText = GetSSQTeamName(2);				
			}
			else
			{
				infNodeItem.t_strText = GetSystemString(936);
			}
		}
		infNodeItem.nOffSetX = 0;
		infNodeItem.nOffSetY = 0;
		infNodeItem.t_color.R = 176;
		infNodeItem.t_color.G = 155;
		infNodeItem.t_color.B = 121;
		infNodeItem.t_color.A = byte(255);
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.pre_MainTree", strRetName, infNodeItem);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		infNodeItem.t_strText = GetSystemMessage(nMsgNum);
		infNodeItem.bLineBreak = true;
		infNodeItem.nOffSetX = 8;
		infNodeItem.nOffSetY = 6;
		infNodeItem.t_color.R = 128;
		infNodeItem.t_color.G = 128;
		infNodeItem.t_color.B = 128;
		infNodeItem.t_color.A = byte(255);
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.pre_MainTree", strRetName, infNodeItem);
		// End:0x5C4
		if(__NFUN_155__(i, __NFUN_147__(g_sinfopre.m_nSealNumArray.Length, 1)))
		{
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 0;
			infNodeItem.b_nHeight = 20;
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.pre_MainTree", strRetName, infNodeItem);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x257;
	}
	return;
}

function ClearSSQMainEventInfo(out SSQMainEventInfo Info)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x43 [Loop If]
	if(__NFUN_150__(i, 9))
	{
		Info.m_Team1MemberName[i] = "";
		Info.m_Team2MemberName[i] = "";
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function ShowSSQMainEvent()
{
	local XMLTreeNodeInfo infNode;
	local string strRetName;

	Class'NWindow.UIAPI_TREECTRL'.static.Clear("SSQMainBoard.me_MainTree");
	infNode.strName = "root";
	infNode.nOffSetX = 3;
	infNode.nOffSetY = 5;
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SSQMainBoard.me_MainTree", "", infNode);
	// End:0xD2
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert root node. Name: ", infNode.strName));
		return;
	}
	Class'NWindow.SSQAPI'.static.RequestSSQStatus(2);
	return;
}

function AddSSQMainEvent(SSQMainEventInfo Info)
{
	local int i;
	local XMLTreeNodeInfo infNode;
	local XMLTreeNodeItemInfo infNodeItem;
	local XMLTreeNodeInfo infNodeClear;
	local XMLTreeNodeItemInfo infNodeItemClear;
	local string strRetName, strNodeName, strTmp;

	strNodeName = __NFUN_112__("root.", string(Info.m_nEventType));
	// End:0x5A
	if(Class'NWindow.UIAPI_TREECTRL'.static.IsNodeNameExist("SSQMainBoard.me_MainTree", strNodeName))
	{
		strRetName = strNodeName;		
	}
	else
	{
		infNode = infNodeClear;
		infNode.strName = __NFUN_112__("", string(Info.m_nEventType));
		infNode.bShowButton = 1;
		infNode.nTexBtnWidth = 14;
		infNode.nTexBtnHeight = 14;
		infNode.strTexBtnExpand = "L2UI_CH3.QUESTWND.QuestWndPlusBtn";
		infNode.strTexBtnCollapse = "L2UI_CH3.QUESTWND.QuestWndMinusBtn";
		infNode.strTexBtnExpand_Over = "L2UI_CH3.QUESTWND.QuestWndPlusBtn_over";
		infNode.strTexBtnCollapse_Over = "L2UI_CH3.QUESTWND.QuestWndMinusBtn_over";
		infNode.nTexExpandedOffSetY = 1;
		infNode.nTexExpandedHeight = 13;
		infNode.nTexExpandedRightWidth = 32;
		infNode.nTexExpandedLeftUWidth = 16;
		infNode.nTexExpandedLeftUHeight = 13;
		infNode.nTexExpandedRightUWidth = 32;
		infNode.nTexExpandedRightUHeight = 13;
		infNode.strTexExpandedLeft = "L2UI_CH3.ListCtrl.TextSelect";
		infNode.strTexExpandedRight = "L2UI_CH3.ListCtrl.TextSelect2";
		strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SSQMainBoard.me_MainTree", "root", infNode);
		// End:0x294
		if(__NFUN_150__(__NFUN_125__(strRetName), 1))
		{
			Debug(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
			return;
		}
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		// End:0x2D5
		if(__NFUN_154__(Info.m_nEventType, 0))
		{
			infNodeItem.t_strText = GetSystemString(845);			
		}
		else
		{
			infNodeItem.t_strText = GetSystemString(27);
		}
		infNodeItem.nOffSetX = 4;
		infNodeItem.nOffSetY = 2;
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 0;
		infNodeItem.b_nHeight = 8;
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	}
	infNode = infNodeClear;
	infNode.strName = __NFUN_112__("", string(Info.m_nEventNo));
	infNode.nOffSetX = 7;
	infNode.nOffSetY = 0;
	infNode.bShowButton = 1;
	infNode.nTexBtnWidth = 14;
	infNode.nTexBtnHeight = 14;
	infNode.strTexBtnExpand = "L2UI_CH3.QUESTWND.QuestWndDownBtn";
	infNode.strTexBtnCollapse = "L2UI_CH3.QUESTWND.QuestWndUpBtn";
	infNode.strTexBtnExpand_Over = "L2UI_CH3.QUESTWND.QuestWndDownBtn_over";
	infNode.strTexBtnCollapse_Over = "L2UI_CH3.QUESTWND.QuestWndUpBtn_over";
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SSQMainBoard.me_MainTree", strRetName, infNode);
	// End:0x528
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		__NFUN_231__(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSSQTimeAttackEventRoomName(Info.m_nEventNo);
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 2;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	// End:0x633
	if(__NFUN_154__(Info.m_nSSQStatus, 1))
	{
		infNodeItem.t_strText = GetSystemString(829);		
	}
	else
	{
		// End:0x681
		if(__NFUN_151__(Info.m_nTeam1Score, Info.m_nTeam2Score))
		{
			infNodeItem.t_strText = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", GetSystemString(923)), " "), GetSystemString(828)), ")");			
		}
		else
		{
			// End:0x6CF
			if(__NFUN_150__(Info.m_nTeam1Score, Info.m_nTeam2Score))
			{
				infNodeItem.t_strText = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", GetSystemString(924)), " "), GetSystemString(828)), ")");				
			}
			else
			{
				infNodeItem.t_strText = __NFUN_112__(__NFUN_112__("(", GetSystemString(846)), ")");
			}
		}
	}
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 2;
	infNodeItem.t_color.R = 176;
	infNodeItem.t_color.G = 155;
	infNodeItem.t_color.B = 121;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = __NFUN_112__(GetSystemString(831), " : ");
	infNodeItem.bLineBreak = true;
	infNodeItem.nOffSetX = 19;
	infNodeItem.nOffSetY = 4;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = __NFUN_112__("", string(Info.m_nWinPoint));
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = 4;
	infNodeItem.t_color.R = 176;
	infNodeItem.t_color.G = 155;
	infNodeItem.t_color.B = 121;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 0;
	infNodeItem.b_nHeight = 8;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNode = infNodeClear;
	infNode.strName = "member";
	infNode.nOffSetX = 2;
	infNode.nOffSetY = 0;
	infNode.bShowButton = 0;
	infNode.bDrawBackground = 1;
	infNode.bTexBackHighlight = 1;
	infNode.nTexBackHighlightHeight = 16;
	infNode.nTexBackWidth = 218;
	infNode.nTexBackUWidth = 211;
	infNode.nTexBackOffSetX = 0;
	infNode.nTexBackOffSetY = -3;
	infNode.nTexBackOffSetBottom = -2;
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SSQMainBoard.me_MainTree", strRetName, infNode);
	// End:0xAAA
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSSQTeamName(2);
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 0;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSystemString(830);
	infNodeItem.bLineBreak = true;
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 4;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = __NFUN_112__("", string(Info.m_nTeam1Score));
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 4;
	infNodeItem.t_color.R = 176;
	infNodeItem.t_color.G = 155;
	infNodeItem.t_color.B = 121;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSystemString(832);
	infNodeItem.bLineBreak = true;
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 4;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	i = 0;
	J0xDE0:

	// End:0xEE5 [Loop If]
	if(__NFUN_150__(i, 9))
	{
		strTmp = Info.m_Team1MemberName[i];
		// End:0xEDB
		if(__NFUN_151__(__NFUN_125__(strTmp), 0))
		{
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 1;
			infNodeItem.t_strText = strTmp;
			infNodeItem.bLineBreak = true;
			infNodeItem.nOffSetX = 5;
			infNodeItem.nOffSetY = 4;
			infNodeItem.t_color.R = 176;
			infNodeItem.t_color.G = 155;
			infNodeItem.t_color.B = 121;
			infNodeItem.t_color.A = byte(255);
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0xDE0;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 0;
	infNodeItem.b_nHeight = 20;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSSQTeamName(1);
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 0;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSystemString(830);
	infNodeItem.bLineBreak = true;
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 4;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = __NFUN_112__("", string(Info.m_nTeam2Score));
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 4;
	infNodeItem.t_color.R = 176;
	infNodeItem.t_color.G = 155;
	infNodeItem.t_color.B = 121;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSystemString(832);
	infNodeItem.bLineBreak = true;
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = 4;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	i = 0;
	J0x1272:

	// End:0x1377 [Loop If]
	if(__NFUN_150__(i, 9))
	{
		strTmp = Info.m_Team2MemberName[i];
		// End:0x136D
		if(__NFUN_151__(__NFUN_125__(strTmp), 0))
		{
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 1;
			infNodeItem.t_strText = strTmp;
			infNodeItem.bLineBreak = true;
			infNodeItem.nOffSetX = 5;
			infNodeItem.nOffSetY = 4;
			infNodeItem.t_color.R = 176;
			infNodeItem.t_color.G = 155;
			infNodeItem.t_color.B = 121;
			infNodeItem.t_color.A = byte(255);
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x1272;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 0;
	infNodeItem.b_nHeight = 4;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.me_MainTree", strRetName, infNodeItem);
	return;
}

function ShowSSQSealStatus()
{
	local XMLTreeNodeInfo infNode;
	local string strRetName;

	Class'NWindow.UIAPI_TREECTRL'.static.Clear("SSQMainBoard.ss_MainTree");
	infNode.strName = "root";
	infNode.nOffSetX = 3;
	infNode.nOffSetY = 5;
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SSQMainBoard.ss_MainTree", "", infNode);
	// End:0xD2
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert root node. Name: ", infNode.strName));
		return;
	}
	// End:0x118
	if(__NFUN_154__(g_sinfo.m_nMsgNum1, 1183))
	{
		AddSSQSealStatus(1, 10, 35, 1, 0, 0, 0);
		AddSSQSealStatus(1, 10, 35, 2, 0, 0, 0);
		AddSSQSealStatus(1, 10, 35, 3, 0, 0, 0);		
	}
	else
	{
		Class'NWindow.SSQAPI'.static.RequestSSQStatus(3);
	}
	return;
}

function AddSSQSealStatus(int m_nSSQStatus, int m_nNeedPoint1, int m_nNeedPoint2, int m_nSealID, int m_nOwnerTeamID, int m_nTeam1Mark, int m_nTeam2Mark)
{
	local int i, nMax, nStrID, nNeedPoint, nTmp;

	local float fBarX, fBarWidth;
	local int nWidth, nHeight;
	local XMLTreeNodeInfo infNode;
	local XMLTreeNodeItemInfo infNodeItem;
	local XMLTreeNodeInfo infNodeClear;
	local XMLTreeNodeItemInfo infNodeItemClear;
	local string strRetName, strTmp;

	strTmp = GetSSQSealName(m_nSealID);
	infNode = infNodeClear;
	infNode.strName = __NFUN_112__("", string(m_nSealID));
	infNode.bShowButton = 1;
	infNode.nTexBtnWidth = 14;
	infNode.nTexBtnHeight = 14;
	infNode.strTexBtnExpand = "L2UI_CH3.QUESTWND.QuestWndPlusBtn";
	infNode.strTexBtnCollapse = "L2UI_CH3.QUESTWND.QuestWndMinusBtn";
	infNode.strTexBtnExpand_Over = "L2UI_CH3.QUESTWND.QuestWndPlusBtn_over";
	infNode.strTexBtnCollapse_Over = "L2UI_CH3.QUESTWND.QuestWndMinusBtn_over";
	infNode.nTexExpandedOffSetY = 1;
	infNode.nTexExpandedHeight = 13;
	infNode.nTexExpandedRightWidth = 32;
	infNode.nTexExpandedLeftUWidth = 16;
	infNode.nTexExpandedLeftUHeight = 13;
	infNode.nTexExpandedRightUWidth = 32;
	infNode.nTexExpandedRightUHeight = 13;
	infNode.strTexExpandedLeft = "L2UI_CH3.ListCtrl.TextSelect";
	infNode.strTexExpandedRight = "L2UI_CH3.ListCtrl.TextSelect2";
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SSQMainBoard.ss_MainTree", "root", infNode);
	// End:0x246
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = strTmp;
	infNodeItem.nOffSetX = 4;
	infNodeItem.nOffSetY = 2;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSystemString(823);
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = 5;
	infNodeItem.bLineBreak = true;
	infNodeItem.bStopMouseFocus = true;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSSQTeamName(m_nOwnerTeamID);
	infNodeItem.nOffSetX = 4;
	infNodeItem.nOffSetY = 5;
	infNodeItem.t_color.R = 176;
	infNodeItem.t_color.G = 155;
	infNodeItem.t_color.B = 121;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSSQTeamName(2);
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = 7;
	infNodeItem.bLineBreak = true;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	fBarX = 80.0000000;
	fBarWidth = 140.0000000;
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 2;
	infNodeItem.nOffSetX = 2;
	infNodeItem.nOffSetY = 7;
	infNodeItem.u_nTextureWidth = int(fBarWidth);
	infNodeItem.u_nTextureHeight = 11;
	infNodeItem.u_nTextureUWidth = 8;
	infNodeItem.u_nTextureUHeight = 11;
	infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_bar2back";
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	// End:0x623
	if(__NFUN_154__(m_nOwnerTeamID, 2))
	{
		nNeedPoint = m_nNeedPoint1;		
	}
	else
	{
		nNeedPoint = m_nNeedPoint2;
	}
	// End:0x81C
	if(__NFUN_151__(m_nTeam1Mark, nNeedPoint))
	{
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 2;
		infNodeItem.nOffSetX = int(__NFUN_169__(fBarWidth));
		infNodeItem.nOffSetY = 7;
		infNodeItem.u_nTextureWidth = int(__NFUN_171__(fBarWidth, __NFUN_172__(float(nNeedPoint), 100.0000000)));
		nTmp = infNodeItem.u_nTextureWidth;
		infNodeItem.u_nTextureHeight = 11;
		infNodeItem.u_nTextureUWidth = 8;
		infNodeItem.u_nTextureUHeight = 11;
		infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_bar21";
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 2;
		infNodeItem.nOffSetX = 0;
		infNodeItem.nOffSetY = 7;
		infNodeItem.u_nTextureWidth = int(__NFUN_171__(fBarWidth, __NFUN_172__(float(__NFUN_147__(m_nTeam1Mark, nNeedPoint)), 100.0000000)));
		nTmp = __NFUN_146__(nTmp, infNodeItem.u_nTextureWidth);
		infNodeItem.u_nTextureHeight = 11;
		infNodeItem.u_nTextureUWidth = 8;
		infNodeItem.u_nTextureUHeight = 11;
		infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_bar22";
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);		
	}
	else
	{
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 2;
		infNodeItem.nOffSetX = int(__NFUN_169__(fBarWidth));
		infNodeItem.nOffSetY = 7;
		infNodeItem.u_nTextureWidth = int(__NFUN_171__(fBarWidth, __NFUN_172__(float(m_nTeam1Mark), 100.0000000)));
		nTmp = infNodeItem.u_nTextureWidth;
		infNodeItem.u_nTextureHeight = 11;
		infNodeItem.u_nTextureUWidth = 8;
		infNodeItem.u_nTextureUHeight = 11;
		infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_bar21";
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 2;
	infNodeItem.nOffSetX = int(__NFUN_174__(float(__NFUN_143__(nTmp)), __NFUN_171__(fBarWidth, __NFUN_172__(float(nNeedPoint), 100.0000000))));
	infNodeItem.nOffSetY = 7;
	infNodeItem.u_nTextureWidth = 1;
	nTmp = int(__NFUN_174__(__NFUN_171__(fBarWidth, __NFUN_172__(float(nNeedPoint), 100.0000000)), float(1)));
	infNodeItem.u_nTextureHeight = 11;
	infNodeItem.u_nTextureUWidth = 1;
	infNodeItem.u_nTextureUHeight = 11;
	infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_barline";
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = __NFUN_112__(string(m_nTeam1Mark), "%");
	GetTextSize(infNodeItem.t_strText, nWidth, nHeight);
	infNodeItem.nOffSetX = int(__NFUN_175__(__NFUN_174__(float(__NFUN_143__(nTmp)), __NFUN_172__(fBarWidth, float(2))), float(__NFUN_145__(nWidth, 2))));
	infNodeItem.nOffSetY = 8;
	infNodeItem.t_color.R = byte(255);
	infNodeItem.t_color.G = byte(255);
	infNodeItem.t_color.B = byte(255);
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSSQTeamName(1);
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = 6;
	infNodeItem.bLineBreak = true;
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 2;
	infNodeItem.nOffSetX = 2;
	infNodeItem.nOffSetY = 6;
	infNodeItem.u_nTextureWidth = int(fBarWidth);
	infNodeItem.u_nTextureHeight = 11;
	infNodeItem.u_nTextureUWidth = 8;
	infNodeItem.u_nTextureUHeight = 11;
	infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_bar1back";
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	// End:0xCBE
	if(__NFUN_154__(m_nOwnerTeamID, 1))
	{
		nNeedPoint = m_nNeedPoint1;		
	}
	else
	{
		nNeedPoint = m_nNeedPoint2;
	}
	// End:0xEB7
	if(__NFUN_151__(m_nTeam2Mark, nNeedPoint))
	{
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 2;
		infNodeItem.nOffSetX = int(__NFUN_169__(fBarWidth));
		infNodeItem.nOffSetY = 6;
		infNodeItem.u_nTextureWidth = int(__NFUN_171__(fBarWidth, __NFUN_172__(float(nNeedPoint), 100.0000000)));
		nTmp = infNodeItem.u_nTextureWidth;
		infNodeItem.u_nTextureHeight = 11;
		infNodeItem.u_nTextureUWidth = 8;
		infNodeItem.u_nTextureUHeight = 11;
		infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_bar11";
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 2;
		infNodeItem.nOffSetX = 0;
		infNodeItem.nOffSetY = 6;
		infNodeItem.u_nTextureWidth = int(__NFUN_171__(fBarWidth, __NFUN_172__(float(__NFUN_147__(m_nTeam2Mark, nNeedPoint)), 100.0000000)));
		nTmp = __NFUN_146__(nTmp, infNodeItem.u_nTextureWidth);
		infNodeItem.u_nTextureHeight = 11;
		infNodeItem.u_nTextureUWidth = 8;
		infNodeItem.u_nTextureUHeight = 11;
		infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_bar12";
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);		
	}
	else
	{
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 2;
		infNodeItem.nOffSetX = int(__NFUN_169__(fBarWidth));
		infNodeItem.nOffSetY = 6;
		infNodeItem.u_nTextureWidth = int(__NFUN_171__(fBarWidth, __NFUN_172__(float(m_nTeam2Mark), 100.0000000)));
		nTmp = infNodeItem.u_nTextureWidth;
		infNodeItem.u_nTextureHeight = 11;
		infNodeItem.u_nTextureUWidth = 8;
		infNodeItem.u_nTextureUHeight = 11;
		infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_bar11";
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 2;
	infNodeItem.nOffSetX = int(__NFUN_174__(float(__NFUN_143__(nTmp)), __NFUN_171__(fBarWidth, __NFUN_172__(float(nNeedPoint), 100.0000000))));
	infNodeItem.nOffSetY = 6;
	infNodeItem.u_nTextureWidth = 1;
	nTmp = int(__NFUN_174__(__NFUN_171__(fBarWidth, __NFUN_172__(float(nNeedPoint), 100.0000000)), float(1)));
	infNodeItem.u_nTextureHeight = 11;
	infNodeItem.u_nTextureUWidth = 1;
	infNodeItem.u_nTextureUHeight = 11;
	infNodeItem.u_strTexture = "L2UI_CH3.SSQWnd.ssq_barline";
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = __NFUN_112__(string(m_nTeam2Mark), "%");
	GetTextSize(infNodeItem.t_strText, nWidth, nHeight);
	infNodeItem.nOffSetX = int(__NFUN_175__(__NFUN_174__(float(__NFUN_143__(nTmp)), __NFUN_172__(fBarWidth, float(2))), float(__NFUN_145__(nWidth, 2))));
	infNodeItem.nOffSetY = 6;
	infNodeItem.t_color.R = byte(255);
	infNodeItem.t_color.G = byte(255);
	infNodeItem.t_color.B = byte(255);
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 0;
	infNodeItem.b_nHeight = 12;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNode = infNodeClear;
	infNode.strName = "desc";
	infNode.bShowButton = 0;
	infNode.bDrawBackground = 1;
	infNode.bTexBackHighlight = 1;
	infNode.nTexBackHighlightHeight = 18;
	infNode.nTexBackWidth = 218;
	infNode.nTexBackUWidth = 211;
	infNode.nTexBackOffSetX = -4;
	infNode.nTexBackOffSetY = -3;
	infNode.nTexBackOffSetBottom = -3;
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SSQMainBoard.ss_MainTree", strRetName, infNode);
	// End:0x1317
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSSQSealDesc(m_nSealID);
	infNodeItem.t_color.R = 128;
	infNodeItem.t_color.G = 128;
	infNodeItem.t_color.B = 128;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 0;
	infNodeItem.b_nHeight = 18;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	// End:0x143B
	if(__NFUN_154__(m_nSealID, 1))
	{
		nMax = 16;
		nStrID = 941;		
	}
	else
	{
		// End:0x145D
		if(__NFUN_154__(m_nSealID, 2))
		{
			nMax = 12;
			nStrID = 957;			
		}
		else
		{
			nMax = 0;
		}
	}
	i = 0;
	J0x146B:

	// End:0x16D1 [Loop If]
	if(__NFUN_150__(i, nMax))
	{
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		infNodeItem.t_strText = GetSystemString(__NFUN_146__(nStrID, i));
		infNodeItem.bLineBreak = true;
		infNodeItem.nOffSetY = 6;
		infNodeItem.t_color.R = 128;
		infNodeItem.t_color.G = 128;
		infNodeItem.t_color.B = 128;
		infNodeItem.t_color.A = byte(255);
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		infNodeItem.t_strText = ":";
		infNodeItem.bLineBreak = true;
		infNodeItem.nOffSetY = 6;
		infNodeItem.t_color.R = 128;
		infNodeItem.t_color.G = 128;
		infNodeItem.t_color.B = 128;
		infNodeItem.t_color.A = byte(255);
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		infNodeItem.t_strText = GetSystemString(__NFUN_146__(__NFUN_146__(nStrID, i), 1));
		infNodeItem.nOffSetY = 6;
		infNodeItem.t_color.R = 176;
		infNodeItem.t_color.G = 155;
		infNodeItem.t_color.B = 121;
		infNodeItem.t_color.A = byte(255);
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
		__NFUN_161__(i, 2);
		// [Loop Continue]
		goto J0x146B;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 0;
	infNodeItem.b_nHeight = 6;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SSQMainBoard.ss_MainTree", strRetName, infNodeItem);
	return;
}

function string GetSSQSealName(int nID)
{
	local int nStrID;

	// End:0x19
	if(__NFUN_154__(nID, 1))
	{
		nStrID = 816;		
	}
	else
	{
		// End:0x33
		if(__NFUN_154__(nID, 2))
		{
			nStrID = 817;			
		}
		else
		{
			// End:0x4D
			if(__NFUN_154__(nID, 3))
			{
				nStrID = 818;				
			}
			else
			{
				nStrID = 27;
			}
		}
	}
	return GetSystemString(nStrID);
	return;
}

function string GetSSQTeamName(int nID)
{
	local int nStrID;

	// End:0x19
	if(__NFUN_154__(nID, 1))
	{
		nStrID = 815;		
	}
	else
	{
		// End:0x33
		if(__NFUN_154__(nID, 2))
		{
			nStrID = 814;			
		}
		else
		{
			nStrID = 27;
		}
	}
	return GetSystemString(nStrID);
	return;
}

function string GetSSQSealDesc(int nID)
{
	local int nStrID;

	// End:0x19
	if(__NFUN_154__(nID, 1))
	{
		nStrID = 1178;		
	}
	else
	{
		// End:0x33
		if(__NFUN_154__(nID, 2))
		{
			nStrID = 1179;			
		}
		else
		{
			// End:0x4D
			if(__NFUN_154__(nID, 3))
			{
				nStrID = 1180;				
			}
			else
			{
				nStrID = 27;
			}
		}
	}
	return GetSystemMessage(nStrID);
	return;
}

function string GetSSQTimeAttackEventRoomName(int nID)
{
	local int nStrID;

	// End:0x19
	if(__NFUN_154__(nID, 1))
	{
		nStrID = 819;		
	}
	else
	{
		// End:0x33
		if(__NFUN_154__(nID, 2))
		{
			nStrID = 820;			
		}
		else
		{
			// End:0x4D
			if(__NFUN_154__(nID, 3))
			{
				nStrID = 821;				
			}
			else
			{
				// End:0x67
				if(__NFUN_154__(nID, 4))
				{
					nStrID = 844;					
				}
				else
				{
					// End:0x81
					if(__NFUN_154__(nID, 5))
					{
						nStrID = 822;						
					}
					else
					{
						nStrID = 27;
					}
				}
			}
		}
	}
	return GetSystemString(nStrID);
	return;
}
