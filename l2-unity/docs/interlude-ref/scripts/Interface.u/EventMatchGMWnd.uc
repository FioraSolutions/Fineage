class EventMatchGMWnd extends UICommonAPI;

const TIMERID_CountDown = 1;

var HtmlHandle m_hCommandHtml;
var WindowHandle m_hEventMatchGMCommandWnd;
var WindowHandle m_hEventMatchGMFenceWnd;
var ButtonHandle m_hCreateEventMatchButton;
var ButtonHandle m_hSetTeam1LeaderButton;
var ButtonHandle m_hLockTeam1Button;
var ButtonHandle m_hSetTeam2LeaderButton;
var ButtonHandle m_hLockTeam2Button;
var ButtonHandle m_hPauseButton;
var ButtonHandle m_hStartButton;
var ButtonHandle m_hSetScoreButton;
var ButtonHandle m_hSendAnnounceButton;
var ButtonHandle m_hShowCommandWndButton;
var ButtonHandle m_hSendGameEndMsgButton;
var ButtonHandle m_hSetFenceButton;
var ButtonHandle m_hTeam1FirecrackerButton;
var ButtonHandle m_hTeam2FirecrackerButton;
var EditBoxHandle m_hTeam1NameEditBox;
var EditBoxHandle m_hTeam2NameEditBox;
var EditBoxHandle m_hTeam1LeaderNameEditBox;
var EditBoxHandle m_hTeam2LeaderNameEditBox;
var EditBoxHandle m_hOptionFileEditBox;
var EditBoxHandle m_hCommandFileEditBox;
var EditBoxHandle m_hTeam1ScoreEditBox;
var EditBoxHandle m_hTeam2ScoreEditBox;
var EditBoxHandle m_hAnnounceEditBox;
var TextBoxHandle m_hMatchIDTextBox;
var ListCtrlHandle m_hTeam1ListCtrl;
var ListCtrlHandle m_hTeam2ListCtrl;
var int m_CountDown;
var int m_MatchID;
var bool m_Team1Locked;
var bool m_Team2Locked;
var bool m_Paused;
var string m_Team1Name;
var string m_Team2Name;

function OnLoad()
{
	RegisterEvent(2180);
	RegisterEvent(2190);
	RegisterEvent(2250);
	RegisterEvent(2210);
	__NFUN_113__('HidingState');
	m_hEventMatchGMCommandWnd = GetHandle("EventMatchGMCommandWnd");
	m_hCommandHtml = HtmlHandle(GetHandle("EventMatchGMCommandWnd.HtmlCtrl"));
	m_hEventMatchGMFenceWnd = GetHandle("EventMatchGMFenceWnd");
	m_hCreateEventMatchButton = ButtonHandle(GetHandle("CreateEventMatchButton"));
	m_hSetTeam1LeaderButton = ButtonHandle(GetHandle("SetTeam1LeaderButton"));
	m_hLockTeam1Button = ButtonHandle(GetHandle("LockTeam1Button"));
	m_hSetTeam2LeaderButton = ButtonHandle(GetHandle("SetTeam2LeaderButton"));
	m_hLockTeam2Button = ButtonHandle(GetHandle("LockTeam2Button"));
	m_hPauseButton = ButtonHandle(GetHandle("PauseButton"));
	m_hStartButton = ButtonHandle(GetHandle("StartButton"));
	m_hSetScoreButton = ButtonHandle(GetHandle("SetScoreButton"));
	m_hSendAnnounceButton = ButtonHandle(GetHandle("SendAnnounceButton"));
	m_hShowCommandWndButton = ButtonHandle(GetHandle("ShowCommandWndButton"));
	m_hSendGameEndMsgButton = ButtonHandle(GetHandle("SendGameEndMsgButton"));
	m_hSetFenceButton = ButtonHandle(GetHandle("SetFenceButton"));
	m_hTeam1FirecrackerButton = ButtonHandle(GetHandle("Team1FirecrackerButton"));
	m_hTeam2FirecrackerButton = ButtonHandle(GetHandle("Team2FirecrackerButton"));
	m_hTeam1NameEditBox = EditBoxHandle(GetHandle("Team1NameEditBox"));
	m_hTeam2NameEditBox = EditBoxHandle(GetHandle("Team2NameEditBox"));
	m_hTeam1LeaderNameEditBox = EditBoxHandle(GetHandle("Team1LeaderNameEditBox"));
	m_hTeam2LeaderNameEditBox = EditBoxHandle(GetHandle("Team2LeaderNameEditBox"));
	m_hOptionFileEditBox = EditBoxHandle(GetHandle("OptionFileEditBox"));
	m_hCommandFileEditBox = EditBoxHandle(GetHandle("CommandFileEditBox"));
	m_hTeam1ScoreEditBox = EditBoxHandle(GetHandle("Team1ScoreEditBox"));
	m_hTeam2ScoreEditBox = EditBoxHandle(GetHandle("Team2ScoreEditBox"));
	m_hAnnounceEditBox = EditBoxHandle(GetHandle("AnnounceEditBox"));
	m_hMatchIDTextBox = TextBoxHandle(GetHandle("MatchIDTextBox"));
	m_hTeam1ListCtrl = ListCtrlHandle(GetHandle("Team1ListCtrl"));
	m_hTeam2ListCtrl = ListCtrlHandle(GetHandle("Team2ListCtrl"));
	return;
}

function OnClickButtonWithHandle(ButtonHandle a_ButtonHandle)
{
	switch(a_ButtonHandle)
	{
		// End:0x18
		case m_hCreateEventMatchButton:
			OnClickCreateEventMatchButton();
			// End:0xF8
			break;
		// End:0x29
		case m_hSetTeam1LeaderButton:
			OnClickSetTeam1LeaderButton();
			// End:0xF8
			break;
		// End:0x3A
		case m_hLockTeam1Button:
			OnClickLockTeam1Button();
			// End:0xF8
			break;
		// End:0x4B
		case m_hSetTeam2LeaderButton:
			OnClickSetTeam2LeaderButton();
			// End:0xF8
			break;
		// End:0x5C
		case m_hLockTeam2Button:
			OnClickLockTeam2Button();
			// End:0xF8
			break;
		// End:0x6D
		case m_hPauseButton:
			OnClickPauseButton();
			// End:0xF8
			break;
		// End:0x7E
		case m_hStartButton:
			OnClickStartButton();
			// End:0xF8
			break;
		// End:0x8F
		case m_hSetScoreButton:
			OnClickSetScoreButton();
			// End:0xF8
			break;
		// End:0xA0
		case m_hSendAnnounceButton:
			OnClickSendAnnounceButton();
			// End:0xF8
			break;
		// End:0xB1
		case m_hShowCommandWndButton:
			OnClickShowCommandWndButton();
			// End:0xF8
			break;
		// End:0xC2
		case m_hSendGameEndMsgButton:
			OnClickSendGameEngMsgButton();
			// End:0xF8
			break;
		// End:0xD3
		case m_hSetFenceButton:
			OnClickSetFenceButton();
			// End:0xF8
			break;
		// End:0xE4
		case m_hTeam1FirecrackerButton:
			OnClickTeam1FirecrackerButton();
			// End:0xF8
			break;
		// End:0xF5
		case m_hTeam2FirecrackerButton:
			OnClickTeam2FirecrackerButton();
			// End:0xF8
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x18
		case 2180:
			HandleShowEventMatchGMWnd();
			// End:0x5D
			break;
		// End:0x2E
		case 2190:
			HandleEventMatchCreated(a_Param);
			// End:0x5D
			break;
		// End:0x44
		case 2250:
			HandleEventMatchUpdateTeamInfo(a_Param);
			// End:0x5D
			break;
		// End:0x5A
		case 2210:
			HandleEventMatchManage(a_Param);
			// End:0x5D
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnHide()
{
	__NFUN_113__('HidingState');
	return;
}

function OnClickCreateEventMatchButton()
{
	return;
}

function OnClickSetTeam1LeaderButton()
{
	return;
}

function OnClickLockTeam1Button()
{
	return;
}

function OnClickSetTeam2LeaderButton()
{
	return;
}

function OnClickLockTeam2Button()
{
	return;
}

function OnClickPauseButton()
{
	return;
}

function OnClickStartButton()
{
	return;
}

function OnClickSetScoreButton()
{
	return;
}

function OnClickSendAnnounceButton()
{
	return;
}

function OnClickShowCommandWndButton()
{
	local string CommandFileName;
	local int Count;
	local string HtmlString;
	local int i;
	local string CommandString;

	CommandFileName = m_hCommandFileEditBox.GetString();
	// End:0x36
	if(__NFUN_122__(CommandFileName, ""))
	{
		DialogShow(5, GetSystemMessage(1415));
		return;
	}
	RefreshINI(CommandFileName);
	// End:0x6A
	if(__NFUN_129__(GetINIInt("Cmd", "CmdCnt", Count, CommandFileName)))
	{
		Count = 0;
	}
	HtmlString = "<html><body>";
	i = 0;
	J0x85:

	// End:0xFE [Loop If]
	if(__NFUN_150__(i, Count))
	{
		// End:0xF4
		if(GetINIString("Cmd", __NFUN_112__("Cmd", string(i)), CommandString, CommandFileName))
		{
			HtmlString = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(HtmlString, "<a cmd = \""), CommandString), "\">"), CommandString), "</a><br1>");
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x85;
	}
	HtmlString = __NFUN_112__(HtmlString, "</body></html>");
	m_hEventMatchGMCommandWnd.ShowWindow();
	m_hEventMatchGMCommandWnd.SetFocus();
	m_hCommandHtml.LoadHtmlFromString(HtmlString);
	return;
}

function OnClickSendGameEngMsgButton()
{
	return;
}

function OnClickSetFenceButton()
{
	return;
}

function OnClickTeam1FirecrackerButton()
{
	return;
}

function OnClickTeam2FirecrackerButton()
{
	return;
}

function NotifyFenceInfo(Vector a_Position, int a_XLength, int a_YLength)
{
	return;
}

function HandleShowEventMatchGMWnd()
{
	return;
}

function HandleEventMatchCreated(string a_Param)
{
	return;
}

function HandleEventMatchUpdateTeamInfo(string a_Param)
{
	local int TeamID, i, PartyMemberCount;
	local LVDataRecord Record;
	local EventMatchUserData UserData;
	local ListCtrlHandle hTeamListCtrl;

	// End:0x1A7
	if(ParseInt(a_Param, "TeamID", TeamID))
	{
		PartyMemberCount = Class'NWindow.EventMatchAPI'.static.GetPartyMemberCount(TeamID);
		switch(TeamID)
		{
			// End:0x84
			case 0:
				m_hLockTeam1Button.EnableWindow();
				// End:0x76
				if(__NFUN_155__(0, PartyMemberCount))
				{
					m_hLockTeam1Button.SetButtonName(1072);
					m_Team1Locked = true;
				}
				hTeamListCtrl = m_hTeam1ListCtrl;
				// End:0xD2
				break;
			// End:0xCC
			case 1:
				m_hLockTeam2Button.EnableWindow();
				// End:0xBE
				if(__NFUN_155__(0, PartyMemberCount))
				{
					m_hLockTeam2Button.SetButtonName(1072);
					m_Team2Locked = true;
				}
				hTeamListCtrl = m_hTeam2ListCtrl;
				// End:0xD2
				break;
			// End:0xFFFF
			default:
				// End:0xD2
				break;
				break;
		}
		hTeamListCtrl.DeleteAllItem();
		Record.LVDataList.Length = 3;
		i = 0;
		J0xF6:

		// End:0x1A1 [Loop If]
		if(__NFUN_150__(i, PartyMemberCount))
		{
			// End:0x197
			if(Class'NWindow.EventMatchAPI'.static.GetUserData(TeamID, i, UserData))
			{
				Record.LVDataList[0].szData = UserData.UserName;
				Record.LVDataList[1].szData = string(UserData.UserLv);
				Record.LVDataList[2].szData = GetClassType(UserData.UserClass);
				hTeamListCtrl.InsertRecord(Record);
			}
			__NFUN_163__(i);
			// [Loop Continue]
			goto J0xF6;
		}
		RefreshLockStatus();
	}
	return;
}

function RefreshLockStatus()
{
	return;
}

function StartCountDown()
{
	return;
}

function bool ApplySkillRule(string a_OptionFile)
{
	local int DefaultAllow;
	local string Command;
	local int i, Count, Id;

	// End:0x3F
	if(__NFUN_129__(GetINIBool("Skill", "DefaultAllow", DefaultAllow, a_OptionFile)))
	{
		DialogShow(5, GetSystemMessage(1425));
		return false;
	}
	Command = __NFUN_168__("//eventmatch skill_rule", string(m_MatchID));
	// End:0x8D
	if(__NFUN_154__(1, DefaultAllow))
	{
		Command = __NFUN_168__(Command, "allow_all");		
	}
	else
	{
		Command = __NFUN_168__(Command, "deny_all");
	}
	// End:0xD4
	if(__NFUN_129__(GetINIInt("Skill", "ExpSkillCnt", Count, a_OptionFile)))
	{
		Count = 0;
	}
	i = 0;
	J0xDB:

	// End:0x189 [Loop If]
	if(__NFUN_150__(i, Count))
	{
		// End:0x13D
		if(__NFUN_129__(GetINIBool("Skill", __NFUN_112__("ExpSkillID", string(i)), Id, a_OptionFile)))
		{
			DialogShow(5, MakeFullSystemMsg(GetSystemMessage(1427), string(i)));
			return false;
		}
		// End:0x15B
		if(__NFUN_154__(1, DefaultAllow))
		{
			Command = __NFUN_168__(Command, "D");			
		}
		else
		{
			Command = __NFUN_168__(Command, "A");
		}
		Command = __NFUN_112__(Command, string(Id));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xDB;
	}
	ExecuteCommand(Command);
	return true;
	return;
}

function bool ApplyItemRule(string a_OptionFile)
{
	local int DefaultAllow;
	local string Command;
	local int i, Count, Id;

	// End:0x3E
	if(__NFUN_129__(GetINIBool("Item", "DefaultAllow", DefaultAllow, a_OptionFile)))
	{
		DialogShow(5, GetSystemMessage(1425));
		return false;
	}
	Command = __NFUN_168__("//eventmatch item_rule", string(m_MatchID));
	// End:0x8B
	if(__NFUN_154__(1, DefaultAllow))
	{
		Command = __NFUN_168__(Command, "allow_all");		
	}
	else
	{
		Command = __NFUN_168__(Command, "deny_all");
	}
	// End:0xD0
	if(__NFUN_129__(GetINIInt("Item", "ExpItemCnt", Count, a_OptionFile)))
	{
		Count = 0;
	}
	i = 0;
	J0xD7:

	// End:0x183 [Loop If]
	if(__NFUN_150__(i, Count))
	{
		// End:0x137
		if(__NFUN_129__(GetINIInt("Item", __NFUN_112__("ExpItemID", string(i)), Id, a_OptionFile)))
		{
			DialogShow(5, MakeFullSystemMsg(GetSystemMessage(1427), string(i)));
			return false;
		}
		// End:0x155
		if(__NFUN_154__(1, DefaultAllow))
		{
			Command = __NFUN_168__(Command, "D");			
		}
		else
		{
			Command = __NFUN_168__(Command, "A");
		}
		Command = __NFUN_112__(Command, string(Id));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xD7;
	}
	ExecuteCommand(Command);
	return true;
	return;
}

function bool ApplyBuffRule()
{
	local string OptionFile, Command;
	local int i, Count, Level, Id;

	OptionFile = m_hOptionFileEditBox.GetString();
	// End:0x4E
	if(__NFUN_129__(GetINIInt("Buff", "BuffCnt", Count, OptionFile)))
	{
		DialogShow(5, GetSystemMessage(1422));
		return false;
	}
	// End:0x5B
	if(__NFUN_153__(0, Count))
	{
		return true;
	}
	Command = __NFUN_168__("//eventmatch useskill", string(m_MatchID));
	i = 0;
	J0x88:

	// End:0x15A [Loop If]
	if(__NFUN_150__(i, Count))
	{
		// End:0xE5
		if(__NFUN_129__(GetINIInt("Buff", __NFUN_112__("BuffID", string(i)), Id, OptionFile)))
		{
			DialogShow(5, MakeFullSystemMsg(GetSystemMessage(1423), string(i)));
			return false;
		}
		// End:0x133
		if(__NFUN_129__(GetINIInt("Buff", __NFUN_112__("BuffLv", string(i)), Level, OptionFile)))
		{
			DialogShow(5, MakeFullSystemMsg(GetSystemMessage(1424), string(i)));
			return false;
		}
		Command = __NFUN_168__(__NFUN_168__(Command, string(Id)), string(Level));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x88;
	}
	ExecuteCommand(Command);
	return true;
	return;
}

function bool CheckBuffRule(string a_OptionFile)
{
	local int i, Count, Level, Id;

	// End:0x39
	if(__NFUN_129__(GetINIInt("Buff", "BuffCnt", Count, a_OptionFile)))
	{
		DialogShow(5, GetSystemMessage(1422));
		return false;
	}
	i = 0;
	J0x40:

	// End:0xF5 [Loop If]
	if(__NFUN_150__(i, Count))
	{
		// End:0x9D
		if(__NFUN_129__(GetINIInt("Buff", __NFUN_112__("BuffID", string(i)), Id, a_OptionFile)))
		{
			DialogShow(5, MakeFullSystemMsg(GetSystemMessage(1423), string(i)));
			return false;
		}
		// End:0xEB
		if(__NFUN_129__(GetINIInt("Buff", __NFUN_112__("BuffLv", string(i)), Level, a_OptionFile)))
		{
			DialogShow(5, MakeFullSystemMsg(GetSystemMessage(1424), string(i)));
			return false;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x40;
	}
	return true;
	return;
}

function SetFence()
{
	// End:0x5E
	if(__NFUN_122__(m_hSetFenceButton.GetButtonName(), GetSystemString(1081)))
	{
		m_hSetFenceButton.SetButtonName(1082);
		ExecuteCommand(__NFUN_168__(__NFUN_168__("//eventmatch fence", string(m_MatchID)), "2"));		
	}
	else
	{
		// End:0xB9
		if(__NFUN_122__(m_hSetFenceButton.GetButtonName(), GetSystemString(1082)))
		{
			m_hSetFenceButton.SetButtonName(1081);
			ExecuteCommand(__NFUN_168__(__NFUN_168__("//eventmatch fence", string(m_MatchID)), "1"));
		}
	}
	return;
}

function RemoveEventMatch()
{
	m_Team1Name = "";
	m_Team2Name = "";
	ExecuteCommand(__NFUN_168__("//eventmatch remove", string(m_MatchID)));
	__NFUN_113__('WaitingState');
	return;
}

function SetScore()
{
	ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch score", string(m_MatchID)), m_hTeam1ScoreEditBox.GetString()), m_hTeam2ScoreEditBox.GetString()));
	return;
}

function SendAnnounce()
{
	ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch msg", string(m_MatchID)), string(0)), "\""), m_hAnnounceEditBox.GetString()), "\""));
	m_hAnnounceEditBox.Clear();
	return;
}

function SendGameEndMsg()
{
	ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch msg", string(m_MatchID)), string(3)), "NULL"));
	return;
}

function Firecracker(int a_TeamID)
{
	local int PartyMemberCount, i;
	local EventMatchUserData UserData;

	PartyMemberCount = Class'NWindow.EventMatchAPI'.static.GetPartyMemberCount(a_TeamID);
	i = 0;
	J0x21:

	// End:0x87 [Loop If]
	if(__NFUN_150__(i, PartyMemberCount))
	{
		// End:0x7D
		if(Class'NWindow.EventMatchAPI'.static.GetUserData(a_TeamID, i, UserData))
		{
			ExecuteCommand(__NFUN_168__("//eventmatch firecracker", UserData.UserName));
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x21;
	}
	return;
}

function HandleEventMatchManage(string a_Param)
{
	local int MatchID, MatchStat, FenceStat;
	local string Team1Name, Team2Name;
	local int Team1Stat, Team2Stat;

	ParseInt(a_Param, "MatchID", MatchID);
	ParseInt(a_Param, "MatchStat", MatchStat);
	ParseInt(a_Param, "FenceStat", FenceStat);
	ParseString(a_Param, "Team1Name", Team1Name);
	ParseString(a_Param, "Team2Name", Team2Name);
	ParseInt(a_Param, "Team1Stat", Team1Stat);
	ParseInt(a_Param, "Team2Stat", Team2Stat);
	SetMatchID(MatchID);
	switch(MatchStat)
	{
		// End:0xDB
		case 1:
			__NFUN_113__('CreatedState');
			// End:0x10A
			break;
		// End:0xF1
		case 2:
			__NFUN_113__('GamingState');
			SetPause(false);
			// End:0x10A
			break;
		// End:0x107
		case 3:
			__NFUN_113__('GamingState');
			SetPause(true);
			// End:0x10A
			break;
		// End:0xFFFF
		default:
			break;
	}
	switch(FenceStat)
	{
		// End:0x115
		case 0:
		// End:0x130
		case 1:
			m_hSetFenceButton.SetButtonName(1081);
			// End:0x14F
			break;
		// End:0x14C
		case 2:
			m_hSetFenceButton.SetButtonName(1082);
			// End:0x14F
			break;
		// End:0xFFFF
		default:
			break;
	}
	// End:0x179
	if(__NFUN_154__(0, Team1Stat))
	{
		m_hLockTeam1Button.SetButtonName(1071);
		m_Team1Locked = false;		
	}
	else
	{
		m_hLockTeam1Button.SetButtonName(1072);
		m_Team1Locked = true;
	}
	// End:0x1BF
	if(__NFUN_154__(0, Team2Stat))
	{
		m_hLockTeam2Button.SetButtonName(1071);
		m_Team2Locked = false;		
	}
	else
	{
		m_hLockTeam2Button.SetButtonName(1072);
		m_Team2Locked = true;
	}
	m_hTeam1NameEditBox.SetString(Team1Name);
	m_hTeam2NameEditBox.SetString(Team2Name);
	m_hTeam1ListCtrl.DeleteAllItem();
	m_hTeam2ListCtrl.DeleteAllItem();
	return;
}

function SetPause(bool a_Pause, optional bool a_SendToServer)
{
	Debug(__NFUN_168__(__NFUN_168__(__NFUN_168__("SetPause", string(a_Pause)), "m_Paused"), string(m_Paused)));
	// End:0x43
	if(__NFUN_242__(m_Paused, a_Pause))
	{
		return;
	}
	// End:0x8F
	if(a_Pause)
	{
		m_hPauseButton.SetButtonName(1074);
		// End:0x8C
		if(a_SendToServer)
		{
			ExecuteCommand(__NFUN_168__("//eventmatch pause", string(m_MatchID)));
		}		
	}
	else
	{
		m_hPauseButton.SetButtonName(1073);
		// End:0xCF
		if(a_SendToServer)
		{
			ExecuteCommand(__NFUN_168__("//eventmatch start", string(m_MatchID)));
		}
	}
	m_Paused = a_Pause;
	return;
}

function SetMatchID(int a_MatchID)
{
	m_MatchID = a_MatchID;
	// End:0x2E
	if(__NFUN_154__(-1, m_MatchID))
	{
		m_hMatchIDTextBox.SetText("");		
	}
	else
	{
		m_hMatchIDTextBox.SetText(string(m_MatchID));
	}
	return;
}

state HidingState
{
	function BeginState()
	{
		m_hOwnerWnd.HideWindow();
		return;
	}

	function EndState()
	{
		m_hOwnerWnd.ShowWindow();
		return;
	}

	function HandleShowEventMatchGMWnd()
	{
		__NFUN_113__('WaitingState');
		return;
	}
	stop;
}

state WaitingState
{
	function BeginState()
	{
		SetMatchID(-1);
		m_hLockTeam1Button.SetButtonName(1071);
		m_hLockTeam2Button.SetButtonName(1071);
		m_hTeam1ListCtrl.DeleteAllItem();
		m_hTeam2ListCtrl.DeleteAllItem();
		m_Team1Locked = false;
		m_Team2Locked = false;
		SetPause(false);
		m_hCreateEventMatchButton.SetButtonName(1068);
		m_hCreateEventMatchButton.EnableWindow();
		m_hSetTeam1LeaderButton.DisableWindow();
		m_hLockTeam1Button.DisableWindow();
		m_hSetTeam2LeaderButton.DisableWindow();
		m_hLockTeam2Button.DisableWindow();
		m_hPauseButton.DisableWindow();
		m_hStartButton.DisableWindow();
		m_hSetScoreButton.DisableWindow();
		m_hSendAnnounceButton.DisableWindow();
		m_hSendGameEndMsgButton.DisableWindow();
		m_hSetFenceButton.DisableWindow();
		m_hTeam1FirecrackerButton.DisableWindow();
		m_hTeam2FirecrackerButton.DisableWindow();
		return;
	}

	function OnClickCreateEventMatchButton()
	{
		local string Team1Name, Team2Name;

		Team1Name = m_hTeam1NameEditBox.GetString();
		// End:0x36
		if(__NFUN_122__(Team1Name, ""))
		{
			DialogShow(5, GetSystemMessage(1418));
			return;
		}
		Team2Name = m_hTeam2NameEditBox.GetString();
		// End:0x6C
		if(__NFUN_122__(Team2Name, ""))
		{
			DialogShow(5, GetSystemMessage(1419));
			return;
		}
		// End:0x90
		if(__NFUN_122__(Team1Name, Team2Name))
		{
			DialogShow(5, GetSystemMessage(1420));
			return;
		}
		m_Team1Name = Team1Name;
		m_Team2Name = Team2Name;
		m_hEventMatchGMFenceWnd.ShowWindow();
		m_hEventMatchGMFenceWnd.SetFocus();
		return;
	}

	function NotifyFenceInfo(Vector a_Position, int a_XLength, int a_YLength)
	{
		ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__(__NFUN_168__(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch create 1", m_Team1Name), m_Team2Name), string(int(a_Position.X))), string(int(a_Position.Y))), string(int(a_Position.Z))), string(a_XLength)), string(a_YLength)));
		__NFUN_113__('CreatingState');
		return;
	}
	stop;
}

state CreatingState
{
	function BeginState()
	{
		m_hCreateEventMatchButton.SetButtonName(1099);
		m_hCreateEventMatchButton.DisableWindow();
		return;
	}

	function HandleEventMatchCreated(string a_Param)
	{
		local int MatchID;

		// End:0x2E
		if(ParseInt(a_Param, "MatchID", MatchID))
		{
			SetMatchID(MatchID);
			__NFUN_113__('CreatedState');
		}
		return;
	}
	stop;
}

state CreatedState
{
	function BeginState()
	{
		m_hCreateEventMatchButton.SetButtonName(1069);
		m_hStartButton.SetButtonName(1075);
		m_hCreateEventMatchButton.EnableWindow();
		m_hSetTeam1LeaderButton.EnableWindow();
		m_hLockTeam1Button.EnableWindow();
		m_hSetTeam2LeaderButton.EnableWindow();
		m_hLockTeam2Button.EnableWindow();
		m_hPauseButton.DisableWindow();
		RefreshLockStatus();
		m_hSetScoreButton.EnableWindow();
		m_hSendAnnounceButton.EnableWindow();
		m_hSendGameEndMsgButton.EnableWindow();
		m_hSetFenceButton.EnableWindow();
		m_hTeam1FirecrackerButton.EnableWindow();
		m_hTeam2FirecrackerButton.EnableWindow();
		SetPause(false);
		return;
	}

	function OnClickCreateEventMatchButton()
	{
		RemoveEventMatch();
		return;
	}

	function OnClickSetTeam1LeaderButton()
	{
		local UserInfo TargetInfo;

		// End:0x27
		if(GetTargetInfo(TargetInfo))
		{
			m_hTeam1LeaderNameEditBox.SetString(TargetInfo.Name);
		}
		return;
	}

	function OnClickLockTeam1Button()
	{
		// End:0x66
		if(m_Team1Locked)
		{
			ExecuteCommand(__NFUN_168__(__NFUN_168__("//eventmatch unlock", string(m_MatchID)), "1"));
			m_hLockTeam1Button.SetButtonName(1071);
			m_Team1Locked = false;
			m_hTeam1ListCtrl.DeleteAllItem();
			RefreshLockStatus();			
		}
		else
		{
			ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch leader", string(m_MatchID)), "1"), m_hTeam1LeaderNameEditBox.GetString()));
			ExecuteCommand(__NFUN_168__(__NFUN_168__("//eventmatch lock", string(m_MatchID)), "1"));
		}
		return;
	}

	function OnClickSetTeam2LeaderButton()
	{
		local UserInfo TargetInfo;

		// End:0x27
		if(GetTargetInfo(TargetInfo))
		{
			m_hTeam2LeaderNameEditBox.SetString(TargetInfo.Name);
		}
		return;
	}

	function OnClickLockTeam2Button()
	{
		// End:0x66
		if(m_Team2Locked)
		{
			ExecuteCommand(__NFUN_168__(__NFUN_168__("//eventmatch unlock", string(m_MatchID)), "2"));
			m_hLockTeam2Button.SetButtonName(1071);
			m_Team2Locked = false;
			m_hTeam2ListCtrl.DeleteAllItem();
			RefreshLockStatus();			
		}
		else
		{
			ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch leader", string(m_MatchID)), "2"), m_hTeam2LeaderNameEditBox.GetString()));
			ExecuteCommand(__NFUN_168__(__NFUN_168__("//eventmatch lock", string(m_MatchID)), "2"));
		}
		return;
	}

	function RefreshLockStatus()
	{
		// End:0x26
		if(__NFUN_130__(m_Team1Locked, m_Team2Locked))
		{
			m_hStartButton.EnableWindow();			
		}
		else
		{
			m_hStartButton.DisableWindow();
		}
		return;
	}

	function OnClickSetFenceButton()
	{
		SetFence();
		return;
	}

	function OnClickStartButton()
	{
		local string OptionFile;

		OptionFile = m_hOptionFileEditBox.GetString();
		// End:0x36
		if(__NFUN_122__(OptionFile, ""))
		{
			DialogShow(5, GetSystemMessage(1421));
			return;
		}
		RefreshINI(OptionFile);
		// End:0x53
		if(__NFUN_129__(ApplySkillRule(OptionFile)))
		{
			return;
		}
		// End:0x65
		if(__NFUN_129__(ApplyItemRule(OptionFile)))
		{
			return;
		}
		// End:0x77
		if(__NFUN_129__(CheckBuffRule(OptionFile)))
		{
			return;
		}
		__NFUN_113__('CountDownState');
		return;
	}

	function OnClickSetScoreButton()
	{
		SetScore();
		return;
	}

	function OnClickSendAnnounceButton()
	{
		SendAnnounce();
		return;
	}

	function OnClickSendGameEngMsgButton()
	{
		SendGameEndMsg();
		return;
	}

	function OnClickTeam1FirecrackerButton()
	{
		Firecracker(0);
		return;
	}

	function OnClickTeam2FirecrackerButton()
	{
		Firecracker(1);
		return;
	}

	function OnCompleteEditBox(string a_EditBoxID)
	{
		// End:0x21
		if(__NFUN_122__(a_EditBoxID, "AnnounceEditBox"))
		{
			SendAnnounce();
		}
		return;
	}
	stop;
}

state CountDownState
{
	function BeginState()
	{
		m_hStartButton.DisableWindow();
		m_hStartButton.SetButtonName(1102);
		m_hOwnerWnd.SetTimer(1, 1000);
		m_CountDown = 4;
		ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch msg", string(m_MatchID)), string(8)), "NULL"));
		return;
	}

	function OnClickCreateEventMatchButton()
	{
		RemoveEventMatch();
		return;
	}

	function OnClickSetFenceButton()
	{
		SetFence();
		return;
	}

	function OnTimer(int a_TimerID)
	{
		switch(a_TimerID)
		{
			// End:0x199
			case 1:
				switch(m_CountDown)
				{
					// End:0x51
					case 4:
						ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch msg", string(m_MatchID)), string(7)), "NULL"));
						m_CountDown = 3;
						// End:0x196
						break;
					// End:0x90
					case 3:
						ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch msg", string(m_MatchID)), string(6)), "NULL"));
						m_CountDown = 2;
						// End:0x196
						break;
					// End:0xCE
					case 2:
						ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch msg", string(m_MatchID)), string(5)), "NULL"));
						m_CountDown = 1;
						// End:0x196
						break;
					// End:0x10B
					case 1:
						ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch msg", string(m_MatchID)), string(4)), "NULL"));
						m_CountDown = 0;
						// End:0x196
						break;
					// End:0x175
					case 0:
						ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch msg", string(m_MatchID)), string(2)), "NULL"));
						// End:0x14B
						if(__NFUN_129__(ApplyBuffRule()))
						{
							return;
						}
						ExecuteCommand(__NFUN_168__("//eventmatch start", string(m_MatchID)));
						__NFUN_113__('GamingState');
					// End:0xFFFF
					default:
						m_hOwnerWnd.KillTimer(1);
						m_CountDown = -1;
						// End:0x196
						break;
						break;
				}
				// End:0x19C
				break;
			// End:0xFFFF
			default:
				break;
		}
		return;
	}

	function OnClickSetScoreButton()
	{
		SetScore();
		return;
	}

	function OnClickSendAnnounceButton()
	{
		SendAnnounce();
		return;
	}

	function OnClickSendGameEngMsgButton()
	{
		SendGameEndMsg();
		return;
	}

	function OnClickTeam1FirecrackerButton()
	{
		Firecracker(0);
		return;
	}

	function OnClickTeam2FirecrackerButton()
	{
		Firecracker(1);
		return;
	}

	function OnCompleteEditBox(string a_EditBoxID)
	{
		// End:0x21
		if(__NFUN_122__(a_EditBoxID, "AnnounceEditBox"))
		{
			SendAnnounce();
		}
		return;
	}
	stop;
}

state GamingState
{
	function BeginState()
	{
		m_hPauseButton.SetButtonName(1073);
		m_hStartButton.SetButtonName(1076);
		m_hCreateEventMatchButton.EnableWindow();
		m_hSetTeam1LeaderButton.DisableWindow();
		m_hLockTeam1Button.DisableWindow();
		m_hSetTeam2LeaderButton.DisableWindow();
		m_hLockTeam2Button.DisableWindow();
		m_hPauseButton.EnableWindow();
		m_hStartButton.EnableWindow();
		m_hSetScoreButton.EnableWindow();
		m_hSendAnnounceButton.EnableWindow();
		m_hSendGameEndMsgButton.EnableWindow();
		m_hSetFenceButton.EnableWindow();
		m_hTeam1FirecrackerButton.EnableWindow();
		m_hTeam2FirecrackerButton.EnableWindow();
		return;
	}

	function OnClickCreateEventMatchButton()
	{
		RemoveEventMatch();
		return;
	}

	function OnClickStartButton()
	{
		ExecuteCommand(__NFUN_168__("//eventmatch dispelall", string(m_MatchID)));
		ExecuteCommand(__NFUN_168__("//eventmatch end", string(m_MatchID)));
		ExecuteCommand(__NFUN_168__(__NFUN_168__(__NFUN_168__("//eventmatch msg", string(m_MatchID)), string(1)), "NULL"));
		__NFUN_113__('CreatedState');
		return;
	}

	function OnClickSetFenceButton()
	{
		SetFence();
		return;
	}

	function OnClickPauseButton()
	{
		SetPause(__NFUN_129__(m_Paused), true);
		return;
	}

	function OnClickSetScoreButton()
	{
		SetScore();
		return;
	}

	function OnClickSendAnnounceButton()
	{
		SendAnnounce();
		return;
	}

	function OnClickSendGameEngMsgButton()
	{
		SendGameEndMsg();
		return;
	}

	function OnClickTeam1FirecrackerButton()
	{
		Firecracker(0);
		return;
	}

	function OnClickTeam2FirecrackerButton()
	{
		Firecracker(1);
		return;
	}

	function OnCompleteEditBox(string a_EditBoxID)
	{
		// End:0x21
		if(__NFUN_122__(a_EditBoxID, "AnnounceEditBox"))
		{
			SendAnnounce();
		}
		return;
	}
	stop;
}
