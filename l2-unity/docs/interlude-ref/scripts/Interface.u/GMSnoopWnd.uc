class GMSnoopWnd extends UICommonAPI;

const MAX_GMSnoop = 4;

var int m_SnoopIDList[4];
var WindowHandle m_hSnoopWndList[4];
var TextListBoxHandle m_hSnoopChatWndList[4];
var ButtonHandle m_hCancelButtonList[4];
var CheckBoxHandle m_hCheckBox[28];

function OnLoad()
{
	local int i, j;

	i = 0;
	J0x07:

	// End:0x124 [Loop If]
	if(__NFUN_150__(i, 4))
	{
		m_hSnoopWndList[i] = GetHandle(__NFUN_112__("SnoopWnd", string(__NFUN_146__(i, 1))));
		m_hSnoopChatWndList[i] = TextListBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("SnoopWnd", string(__NFUN_146__(i, 1))), ".Chat")));
		m_hCancelButtonList[i] = ButtonHandle(GetHandle(__NFUN_112__(__NFUN_112__("SnoopWnd", string(__NFUN_146__(i, 1))), ".CancelButton")));
		j = 0;
		J0xB6:

		// End:0x11A [Loop If]
		if(__NFUN_150__(j, 7))
		{
			m_hCheckBox[__NFUN_146__(__NFUN_144__(i, 7), j)] = CheckBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__(__NFUN_112__("SnoopWnd", string(__NFUN_146__(i, 1))), ".CheckBox"), string(j))));
			__NFUN_163__(j);
			// [Loop Continue]
			goto J0xB6;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	RegisterEvent(2410);
	ClearAllSnoop();
	return;
}

function OnShow()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x32 [Loop If]
	if(__NFUN_150__(i, 4))
	{
		m_hSnoopWndList[i].HideWindow();
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function OnHide()
{
	return;
}

function OnClickButtonWithHandle(ButtonHandle a_ButtonHandle)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x57 [Loop If]
	if(__NFUN_150__(i, 4))
	{
		// End:0x4D
		if(__NFUN_114__(a_ButtonHandle, m_hCancelButtonList[i]))
		{
			Class'NWindow.GMAPI'.static.RequestSnoopEnd(m_SnoopIDList[i]);
			ClearSnoop(i);
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 2410:
			HandleGMSnoop(a_Param);
			// End:0x20
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleGMSnoop(string a_Param)
{
	local int SnoopID;
	local string SnoopName;
	local int CreatureID, Type, SnoopIndex;
	local string CharacterName, chatMessage;
	local WindowHandle hSnoopWnd;
	local TextListBoxHandle hSnoopChatWnd;

	ParseInt(a_Param, "SnoopID", SnoopID);
	ParseString(a_Param, "CharacterName", CharacterName);
	// End:0x71
	if(__NFUN_129__(GetSnoopWnd(SnoopID, hSnoopWnd, hSnoopChatWnd, SnoopIndex, CharacterName)))
	{
		DialogShow(1, GetSystemMessage(802));
		return;
	}
	ParseString(a_Param, "SnoopName", SnoopName);
	ParseInt(a_Param, "CreatureID", CreatureID);
	ParseInt(a_Param, "Type", Type);
	ParseString(a_Param, "ChatMessage", chatMessage);
	// End:0xFE
	if(__NFUN_129__(m_hOwnerWnd.IsShowWindow()))
	{
		m_hOwnerWnd.ShowWindow();
	}
	hSnoopWnd.ShowWindow();
	// End:0x152
	if(__NFUN_129__(IsFiltered(byte(Type), SnoopIndex)))
	{
		hSnoopChatWnd.AddString(__NFUN_112__(__NFUN_112__(CharacterName, ": "), chatMessage), GetColorByType(byte(Type)));
	}
	return;
}

function ClearAllSnoop()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x28 [Loop If]
	if(__NFUN_150__(i, 4))
	{
		ClearSnoop(i);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function ClearSnoop(int a_SnoopIndex)
{
	local int i;
	local bool AllHidden;

	// End:0x0D
	if(__NFUN_151__(0, a_SnoopIndex))
	{
		return;
	}
	// End:0x1B
	if(__NFUN_152__(4, a_SnoopIndex))
	{
		return;
	}
	m_SnoopIDList[a_SnoopIndex] = -1;
	m_hSnoopWndList[a_SnoopIndex].HideWindow();
	m_hSnoopChatWndList[a_SnoopIndex].HideWindow();
	AllHidden = true;
	i = 0;
	J0x65:

	// End:0x9B [Loop If]
	if(__NFUN_150__(i, 4))
	{
		// End:0x91
		if(__NFUN_155__(-1, m_SnoopIDList[a_SnoopIndex]))
		{
			AllHidden = false;
			// [Explicit Break]
			goto J0x9B;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x65;
	}
	J0x9B:

	// End:0xB3
	if(AllHidden)
	{
		m_hOwnerWnd.HideWindow();
	}
	return;
}

function bool GetSnoopWnd(int a_SnoopID, out WindowHandle a_hSnoopWnd, out TextListBoxHandle a_hSnoopChatWnd, out int a_SnoopIndex, string a_CharacterName)
{
	local int SnoopIndex;

	SnoopIndex = GetSnoopIndexByID(a_SnoopID);
	// End:0x4F
	if(__NFUN_155__(-1, SnoopIndex))
	{
		a_hSnoopWnd = m_hSnoopWndList[SnoopIndex];
		a_hSnoopChatWnd = m_hSnoopChatWndList[SnoopIndex];
		a_SnoopIndex = SnoopIndex;
		return true;
	}
	SnoopIndex = GetSnoopIndexByID(-1);
	// End:0xD7
	if(__NFUN_155__(-1, SnoopIndex))
	{
		m_SnoopIDList[SnoopIndex] = a_SnoopID;
		a_hSnoopWnd = m_hSnoopWndList[SnoopIndex];
		a_hSnoopChatWnd = m_hSnoopChatWndList[SnoopIndex];
		a_SnoopIndex = SnoopIndex;
		a_hSnoopWnd.SetWindowTitle(__NFUN_112__(__NFUN_112__(GetSystemString(693), " - "), a_CharacterName));
		return true;
	}
	return false;
	return;
}

function int GetSnoopIndexByID(int a_SnoopID)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x38 [Loop If]
	if(__NFUN_150__(i, 4))
	{
		// End:0x2E
		if(__NFUN_154__(a_SnoopID, m_SnoopIDList[i]))
		{
			return i;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return -1;
	return;
}

function Color GetColorByType(UIEventManager.EChatType a_Type)
{
	local Color ResultColor;

	ResultColor.A = byte(255);
	switch(a_Type)
	{
		// End:0x47
		case 1:
			ResultColor.R = byte(255);
			ResultColor.G = 114;
			ResultColor.B = 0;
			// End:0x16E
			break;
		// End:0x78
		case 3:
			ResultColor.R = 0;
			ResultColor.G = byte(255);
			ResultColor.B = 0;
			// End:0x16E
			break;
		// End:0xA9
		case 4:
			ResultColor.R = 125;
			ResultColor.G = 119;
			ResultColor.B = byte(255);
			// End:0x16E
			break;
		// End:0xDC
		case 2:
			ResultColor.R = byte(255);
			ResultColor.G = 0;
			ResultColor.B = byte(255);
			// End:0x16E
			break;
		// End:0x10B
		case 8:
			ResultColor.R = 234;
			ResultColor.G = 165;
			ResultColor.B = 245;
			// End:0x16E
			break;
		// End:0x13C
		case 9:
			ResultColor.R = 119;
			ResultColor.G = byte(255);
			ResultColor.B = 153;
			// End:0x16E
			break;
		// End:0x16B
		case 0:
			ResultColor.R = 220;
			ResultColor.G = 220;
			ResultColor.B = 220;
			// End:0x16E
			break;
		// End:0xFFFF
		default:
			break;
	}
	return ResultColor;
	return;
}

function bool IsFiltered(UIEventManager.EChatType a_Type, int a_SnoopIndex)
{
	switch(a_Type)
	{
		// End:0x2D
		case 8:
			// End:0x2A
			if(m_hCheckBox[__NFUN_144__(a_SnoopIndex, 7)].IsChecked())
			{
				return false;
			}
			// End:0x12B
			break;
		// End:0x56
		case 4:
			// End:0x53
			if(m_hCheckBox[__NFUN_146__(__NFUN_144__(a_SnoopIndex, 7), 1)].IsChecked())
			{
				return false;
			}
			// End:0x12B
			break;
		// End:0x80
		case 3:
			// End:0x7D
			if(m_hCheckBox[__NFUN_146__(__NFUN_144__(a_SnoopIndex, 7), 2)].IsChecked())
			{
				return false;
			}
			// End:0x12B
			break;
		// End:0xAA
		case 1:
			// End:0xA7
			if(m_hCheckBox[__NFUN_146__(__NFUN_144__(a_SnoopIndex, 7), 3)].IsChecked())
			{
				return false;
			}
			// End:0x12B
			break;
		// End:0xD4
		case 2:
			// End:0xD1
			if(m_hCheckBox[__NFUN_146__(__NFUN_144__(a_SnoopIndex, 7), 4)].IsChecked())
			{
				return false;
			}
			// End:0x12B
			break;
		// End:0xFE
		case 0:
			// End:0xFB
			if(m_hCheckBox[__NFUN_146__(__NFUN_144__(a_SnoopIndex, 7), 5)].IsChecked())
			{
				return false;
			}
			// End:0x12B
			break;
		// End:0x128
		case 9:
			// End:0x125
			if(m_hCheckBox[__NFUN_146__(__NFUN_144__(a_SnoopIndex, 7), 6)].IsChecked())
			{
				return false;
			}
			// End:0x12B
			break;
		// End:0xFFFF
		default:
			break;
	}
	return true;
	return;
}
