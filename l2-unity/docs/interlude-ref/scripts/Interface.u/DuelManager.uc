class DuelManager extends UICommonAPI;

const DIALOG_ASK_START = 1111;
const MAX_PARTY_NUM = 9;
const NDUELSTATUS_HEIGHT = 46;

var int m_memberInfo[9];
var WindowHandle m_TopWnd;
var WindowHandle m_StatusWnd[9];
var NameCtrlHandle m_PlayerName[9];
var TextureHandle m_ClassIcon[9];
var BarHandle m_BarCP[9];
var BarHandle m_BarHP[9];
var BarHandle m_BarMP[9];
var bool m_bDuelState;

function OnLoad()
{
	local int idx;

	RegisterEvent(2700);
	RegisterEvent(2710);
	RegisterEvent(2720);
	RegisterEvent(2730);
	RegisterEvent(2740);
	RegisterEvent(2750);
	RegisterEvent(1710);
	RegisterEvent(1720);
	m_TopWnd = GetHandle("DuelManager");
	idx = 0;
	J0x78:

	// End:0x22C [Loop If]
	if(__NFUN_150__(idx, 9))
	{
		m_StatusWnd[idx] = GetHandle(__NFUN_112__("DuelManager.PartyStatusWnd", string(idx)));
		m_PlayerName[idx] = NameCtrlHandle(GetHandle(__NFUN_112__(__NFUN_112__("DuelManager.PartyStatusWnd", string(idx)), ".PlayerName")));
		m_ClassIcon[idx] = TextureHandle(GetHandle(__NFUN_112__(__NFUN_112__("DuelManager.PartyStatusWnd", string(idx)), ".ClassIcon")));
		m_BarCP[idx] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("DuelManager.PartyStatusWnd", string(idx)), ".barCP")));
		m_BarHP[idx] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("DuelManager.PartyStatusWnd", string(idx)), ".barHP")));
		m_BarMP[idx] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("DuelManager.PartyStatusWnd", string(idx)), ".barMP")));
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0x78;
	}
	Clear();
	m_bDuelState = false;
	return;
}

function OnEvent(int EventID, string param)
{
	local Color White;

	White.R = byte(255);
	White.G = byte(255);
	White.B = byte(255);
	switch(EventID)
	{
		// End:0x4A
		case 2700:
			HandleDuelAskStart(param);
			// End:0xD9
			break;
		// End:0x5D
		case 2710:
			m_bDuelState = true;
			// End:0xD9
			break;
		// End:0x68
		case 2720:
			// End:0xD9
			break;
		// End:0x90
		case 2730:
			m_bDuelState = false;
			Clear();
			m_TopWnd.HideWindow();
			// End:0xD9
			break;
		// End:0xA6
		case 2740:
			HandleUpdateUserInfo(param);
			// End:0xD9
			break;
		// End:0xB1
		case 2750:
			// End:0xD9
			break;
		// End:0xC2
		case 1710:
			HandleDialogOK();
			// End:0xD9
			break;
		// End:0xD3
		case 1720:
			HandleDialogCancel();
			// End:0xD9
			break;
		// End:0xFFFF
		default:
			// End:0xD9
			break;
			break;
	}
	return;
}

function HandleDuelAskStart(string param)
{
	local string sName;
	local int Type, messageNum;
	local bool bOption;

	bOption = GetOptionBool("Game", "IsRejectingDuel");
	ParseString(param, "userName", sName);
	ParseInt(param, "type", Type);
	// End:0x77
	if(__NFUN_242__(bOption, true))
	{
		RequestDuelAnswerStart(Type, int(bOption), 0);		
	}
	else
	{
		// End:0x90
		if(__NFUN_154__(Type, 0))
		{
			messageNum = 1938;			
		}
		else
		{
			// End:0xA6
			if(__NFUN_154__(Type, 1))
			{
				messageNum = 1939;
			}
		}
		DialogSetReservedInt(Type);
		DialogSetParamInt(__NFUN_144__(10, 1000));
		DialogSetID(1111);
		DialogShow(7, MakeFullSystemMsg(GetSystemMessage(messageNum), sName));
	}
	return;
}

function HandleDialogOK()
{
	local int dialogID;
	local bool bOption;

	// End:0x5D
	if(DialogIsMine())
	{
		dialogID = DialogGetID();
		// End:0x5D
		if(__NFUN_154__(dialogID, 1111))
		{
			bOption = GetOptionBool("Game", "IsRejectingDuel");
			RequestDuelAnswerStart(DialogGetReservedInt(), int(bOption), 1);
		}
	}
	return;
}

function HandleDialogCancel()
{
	local int dialogID;
	local bool bOption;

	// End:0x5D
	if(DialogIsMine())
	{
		dialogID = DialogGetID();
		// End:0x5D
		if(__NFUN_154__(dialogID, 1111))
		{
			bOption = GetOptionBool("Game", "IsRejectingDuel");
			RequestDuelAnswerStart(DialogGetReservedInt(), int(bOption), 0);
		}
	}
	return;
}

function HandleUpdateUserInfo(string param)
{
	local string sName;
	local int Id, ClassID, Level, currentHP, maxHP, currentMP,
		maxMP, currentCP, maxCP, i;

	local bool bFound;

	// End:0x0D
	if(__NFUN_129__(m_bDuelState))
	{
		return;
	}
	ParseString(param, "userName", sName);
	ParseInt(param, "ID", Id);
	ParseInt(param, "class", ClassID);
	ParseInt(param, "level", Level);
	ParseInt(param, "currentHP", currentHP);
	ParseInt(param, "maxHP", maxHP);
	ParseInt(param, "currentMP", currentMP);
	ParseInt(param, "maxMP", maxMP);
	ParseInt(param, "currentCP", currentCP);
	ParseInt(param, "maxCP", maxCP);
	bFound = false;
	i = 0;
	J0x10E:

	// End:0x15B [Loop If]
	if(__NFUN_150__(i, 9))
	{
		// End:0x14E
		if(__NFUN_155__(m_memberInfo[i], 0))
		{
			// End:0x14B
			if(__NFUN_154__(Id, m_memberInfo[i]))
			{
				bFound = true;
				// [Explicit Break]
				goto J0x15B;
			}
			// [Explicit Continue]
			goto J0x151;
		}
		// [Explicit Break]
		goto J0x15B;
		J0x151:

		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x10E;
	}
	J0x15B:

	// End:0x19A
	if(__NFUN_129__(bFound))
	{
		m_memberInfo[i] = Id;
		m_StatusWnd[i].ShowWindow();
		Resize(__NFUN_146__(i, 1));
	}
	m_TopWnd.ShowWindow();
	m_PlayerName[i].SetName(sName, 0, 2);
	m_ClassIcon[i].SetTexture(GetClassIconName(ClassID));
	m_ClassIcon[i].SetTooltipCustomType(MakeTooltipSimpleText(__NFUN_112__(__NFUN_112__(GetClassStr(ClassID), " - "), GetClassType(ClassID))));
	m_BarCP[i].SetValue(maxCP, currentCP);
	m_BarHP[i].SetValue(maxHP, currentHP);
	m_BarMP[i].SetValue(maxMP, currentMP);
	return;
}

function Clear()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x3F [Loop If]
	if(__NFUN_150__(i, 9))
	{
		m_StatusWnd[i].HideWindow();
		m_memberInfo[i] = 0;
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function Resize(int Count)
{
	local Rect entireRect, statusWndRect;

	entireRect = m_TopWnd.GetRect();
	statusWndRect = m_StatusWnd[0].GetRect();
	m_TopWnd.SetWindowSize(entireRect.nWidth, __NFUN_144__(statusWndRect.nHeight, Count));
	m_TopWnd.SetResizeFrameSize(10, __NFUN_144__(statusWndRect.nHeight, Count));
	return;
}

function OnLButtonDown(WindowHandle a_WindowHandle, int X, int Y)
{
	local Rect rectWnd;
	local int idx;

	rectWnd = m_TopWnd.GetRect();
	// End:0x59
	if(__NFUN_151__(X, __NFUN_146__(rectWnd.nX, 13)))
	{
		idx = __NFUN_145__(__NFUN_147__(Y, rectWnd.nY), 46);
		RequestTargetUser(m_memberInfo[idx]);
	}
	return;
}
