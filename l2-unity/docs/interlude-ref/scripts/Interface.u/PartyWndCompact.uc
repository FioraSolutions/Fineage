class PartyWndCompact extends UICommonAPI;

const NSTATUSICON_MAXCOL = 12;
const NPARTYSTATUS_HEIGHT = 26;
const NPARTYSTATUS_MAXCOUNT = 8;

var bool m_bCompact;
var bool m_bBuff;
var bool m_partyleader;
var int m_arrID[8];
var int m_CurCount;
var int m_CurBf;
var int m_MasterID;
var WindowHandle m_wndTop;
var WindowHandle m_PartyOption;
var WindowHandle m_PartyStatus[8];
var TextureHandle m_ClassIcon[8];
var StatusIconHandle m_StatusIconBuff[8];
var StatusIconHandle m_StatusIconDeBuff[8];
var BarHandle m_BarCP[8];
var BarHandle m_BarHP[8];
var BarHandle m_BarMP[8];
var ButtonHandle btnBuff;

function OnLoad()
{
	local int idx;

	RegisterEvent(1000);
	RegisterEvent(1140);
	RegisterEvent(1150);
	RegisterEvent(1160);
	RegisterEvent(1170);
	RegisterEvent(1180);
	RegisterEvent(40);
	m_bCompact = false;
	m_bBuff = false;
	m_CurBf = 0;
	m_MasterID = 0;
	m_wndTop = GetHandle("PartyWndCompact");
	m_PartyOption = GetHandle("PartyWndOption");
	idx = 0;
	J0xA8:

	// End:0x2CD [Loop If]
	if(__NFUN_150__(idx, 8))
	{
		m_PartyStatus[idx] = GetHandle(__NFUN_112__("PartyWndCompact.PartyStatusWnd", string(idx)));
		m_ClassIcon[idx] = TextureHandle(GetHandle(__NFUN_112__(__NFUN_112__("PartyWndCompact.PartyStatusWnd", string(idx)), ".ClassIcon")));
		m_StatusIconBuff[idx] = StatusIconHandle(GetHandle(__NFUN_112__(__NFUN_112__("PartyWndCompact.PartyStatusWnd", string(idx)), ".StatusIconBuff")));
		m_StatusIconDeBuff[idx] = StatusIconHandle(GetHandle(__NFUN_112__(__NFUN_112__("PartyWndCompact.PartyStatusWnd", string(idx)), ".StatusIconDebuff")));
		m_BarCP[idx] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("PartyWndCompact.PartyStatusWnd", string(idx)), ".barCP")));
		m_BarHP[idx] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("PartyWndCompact.PartyStatusWnd", string(idx)), ".barHP")));
		m_BarMP[idx] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("PartyWndCompact.PartyStatusWnd", string(idx)), ".barMP")));
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0xA8;
	}
	btnBuff = ButtonHandle(GetHandle("PartyWndCompact.btnBuff"));
	idx = 0;
	J0x2FE:

	// End:0x3BC [Loop If]
	if(__NFUN_150__(idx, 8))
	{
		m_StatusIconBuff[idx].SetAnchor(__NFUN_112__("PartyWndCompact.PartyStatusWnd", string(idx)), "TopRight", "TopLeft", 1, 3);
		m_StatusIconDeBuff[idx].SetAnchor(__NFUN_112__("PartyWndCompact.PartyStatusWnd", string(idx)), "TopRight", "TopLeft", 1, 3);
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0x2FE;
	}
	m_ClassIcon[0].Move(0, 7);
	m_PartyOption.HideWindow();
	return;
}

function OnShow()
{
	SetBuffButtonTooltip();
	return;
}

function OnEnterState(name a_PreStateName)
{
	m_bCompact = false;
	m_bBuff = false;
	m_CurBf = 0;
	ResizeWnd();
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x1D
	if(__NFUN_154__(Event_ID, 1140))
	{
		HandlePartyAddParty(param);		
	}
	else
	{
		// End:0x3A
		if(__NFUN_154__(Event_ID, 1150))
		{
			HandlePartyUpdateParty(param);			
		}
		else
		{
			// End:0x57
			if(__NFUN_154__(Event_ID, 1160))
			{
				HandlePartyDeleteParty(param);				
			}
			else
			{
				// End:0x6F
				if(__NFUN_154__(Event_ID, 1170))
				{
					HandlePartyDeleteAllParty();					
				}
				else
				{
					// End:0x8C
					if(__NFUN_154__(Event_ID, 1180))
					{
						HandlePartySpelledList(param);						
					}
					else
					{
						// End:0xA9
						if(__NFUN_154__(Event_ID, 1000))
						{
							HandleShowBuffIcon(param);							
						}
						else
						{
							// End:0xBB
							if(__NFUN_154__(Event_ID, 40))
							{
								HandleRestart();
							}
						}
					}
				}
			}
		}
	}
	return;
}

function HandleRestart()
{
	Clear();
	return;
}

function Clear()
{
	local int idx;

	idx = 0;
	J0x07:

	// End:0x28 [Loop If]
	if(__NFUN_150__(idx, 8))
	{
		ClearStatus(idx);
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0x07;
	}
	m_CurCount = 0;
	ResizeWnd();
	return;
}

function ClearStatus(int idx)
{
	m_StatusIconBuff[idx].Clear();
	m_StatusIconDeBuff[idx].Clear();
	m_ClassIcon[idx].SetTexture("");
	UpdateCPBar(idx, 0, 0);
	UpdateHPBar(idx, 0, 0);
	UpdateMPBar(idx, 0, 0);
	m_arrID[idx] = 0;
	return;
}

function CopyStatus(int DesIndex, int SrcIndex)
{
	local int MaxValue, CurValue, Row, col, MaxRow, MaxCol;

	local StatusIconInfo Info;
	local CustomTooltip TooltipInfo, TooltipInfo2;

	m_arrID[DesIndex] = m_arrID[SrcIndex];
	m_PartyStatus[SrcIndex].GetTooltipCustomType(TooltipInfo);
	m_PartyStatus[DesIndex].SetTooltipCustomType(TooltipInfo);
	m_ClassIcon[DesIndex].SetTexture(m_ClassIcon[SrcIndex].GetTextureName());
	m_ClassIcon[SrcIndex].GetTooltipCustomType(TooltipInfo2);
	m_ClassIcon[DesIndex].SetTooltipCustomType(TooltipInfo2);
	m_BarCP[SrcIndex].GetValue(MaxValue, CurValue);
	m_BarCP[DesIndex].SetValue(MaxValue, CurValue);
	m_BarHP[SrcIndex].GetValue(MaxValue, CurValue);
	m_BarHP[DesIndex].SetValue(MaxValue, CurValue);
	m_BarMP[SrcIndex].GetValue(MaxValue, CurValue);
	m_BarMP[DesIndex].SetValue(MaxValue, CurValue);
	m_StatusIconBuff[DesIndex].Clear();
	MaxRow = m_StatusIconBuff[SrcIndex].GetRowCount();
	Row = 0;
	J0x19A:

	// End:0x24B [Loop If]
	if(__NFUN_150__(Row, MaxRow))
	{
		m_StatusIconBuff[DesIndex].AddRow();
		MaxCol = m_StatusIconBuff[SrcIndex].GetColCount(Row);
		col = 0;
		J0x1E5:

		// End:0x241 [Loop If]
		if(__NFUN_150__(col, MaxCol))
		{
			m_StatusIconBuff[SrcIndex].GetItem(Row, col, Info);
			m_StatusIconBuff[DesIndex].AddCol(Row, Info);
			__NFUN_165__(col);
			// [Loop Continue]
			goto J0x1E5;
		}
		__NFUN_165__(Row);
		// [Loop Continue]
		goto J0x19A;
	}
	m_StatusIconDeBuff[DesIndex].Clear();
	MaxRow = m_StatusIconDeBuff[SrcIndex].GetRowCount();
	Row = 0;
	J0x282:

	// End:0x333 [Loop If]
	if(__NFUN_150__(Row, MaxRow))
	{
		m_StatusIconDeBuff[DesIndex].AddRow();
		MaxCol = m_StatusIconDeBuff[SrcIndex].GetColCount(Row);
		col = 0;
		J0x2CD:

		// End:0x329 [Loop If]
		if(__NFUN_150__(col, MaxCol))
		{
			m_StatusIconDeBuff[SrcIndex].GetItem(Row, col, Info);
			m_StatusIconDeBuff[DesIndex].AddCol(Row, Info);
			__NFUN_165__(col);
			// [Loop Continue]
			goto J0x2CD;
		}
		__NFUN_165__(Row);
		// [Loop Continue]
		goto J0x282;
	}
	return;
}

function ResizeWnd()
{
	local int idx;
	local Rect rectWnd;
	local bool bOption;

	bOption = GetOptionBool("Game", "SmallPartyWnd");
	// End:0x107
	if(__NFUN_151__(m_CurCount, 0))
	{
		rectWnd = m_wndTop.GetRect();
		m_wndTop.SetWindowSize(rectWnd.nWidth, __NFUN_144__(26, m_CurCount));
		m_wndTop.SetResizeFrameSize(10, __NFUN_144__(26, m_CurCount));
		// End:0x99
		if(bOption)
		{
			m_wndTop.ShowWindow();			
		}
		else
		{
			m_wndTop.HideWindow();
		}
		idx = 0;
		J0xAF:

		// End:0x104 [Loop If]
		if(__NFUN_150__(idx, 8))
		{
			// End:0xE5
			if(__NFUN_152__(idx, __NFUN_147__(m_CurCount, 1)))
			{
				m_PartyStatus[idx].ShowWindow();
				// [Explicit Continue]
				goto J0xFA;
			}
			m_PartyStatus[idx].HideWindow();
			J0xFA:

			__NFUN_165__(idx);
			// [Loop Continue]
			goto J0xAF;
		}		
	}
	else
	{
		m_wndTop.HideWindow();
	}
	return;
}

function int FindPartyID(int Id)
{
	local int idx;

	idx = 0;
	J0x07:

	// End:0x38 [Loop If]
	if(__NFUN_150__(idx, 8))
	{
		// End:0x2E
		if(__NFUN_154__(m_arrID[idx], Id))
		{
			return idx;
		}
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0x07;
	}
	return -1;
	return;
}

function HandlePartyAddParty(string param)
{
	local int Id;

	ParseInt(param, "ID", Id);
	// End:0x53
	if(__NFUN_151__(Id, 0))
	{
		__NFUN_165__(m_CurCount);
		ResizeWnd();
		m_arrID[__NFUN_147__(m_CurCount, 1)] = Id;
		UpdateStatus(__NFUN_147__(m_CurCount, 1), param);
	}
	return;
}

function HandlePartyUpdateParty(string param)
{
	local int Id, idx;

	ParseInt(param, "ID", Id);
	// End:0x40
	if(__NFUN_151__(Id, 0))
	{
		idx = FindPartyID(Id);
		UpdateStatus(idx, param);
	}
	return;
}

function HandlePartyDeleteParty(string param)
{
	local int Id, idx, i;

	ParseInt(param, "ID", Id);
	// End:0x91
	if(__NFUN_151__(Id, 0))
	{
		idx = FindPartyID(Id);
		// End:0x91
		if(__NFUN_151__(idx, -1))
		{
			i = idx;
			J0x4A:

			// End:0x79 [Loop If]
			if(__NFUN_150__(i, __NFUN_147__(m_CurCount, 1)))
			{
				CopyStatus(i, __NFUN_146__(i, 1));
				__NFUN_165__(i);
				// [Loop Continue]
				goto J0x4A;
			}
			ClearStatus(m_CurCount);
			__NFUN_166__(m_CurCount);
			ResizeWnd();
		}
	}
	return;
}

function HandlePartyDeleteAllParty()
{
	Clear();
	return;
}

function UpdateStatus(int idx, string param)
{
	local string Name;
	local int Id, CP, maxCP, HP, maxHP, MP,
		maxMP, ClassID, MasterID;

	local CustomTooltip TooltipInfo, TooltipInfo2;

	// End:0x1B
	if(__NFUN_132__(__NFUN_150__(idx, 0), __NFUN_153__(idx, 8)))
	{
		return;
	}
	ParseString(param, "Name", Name);
	ParseInt(param, "ID", Id);
	ParseInt(param, "CurCP", CP);
	ParseInt(param, "MaxCP", maxCP);
	ParseInt(param, "CurHP", HP);
	ParseInt(param, "MaxHP", maxHP);
	ParseInt(param, "CurMP", MP);
	ParseInt(param, "MaxMP", maxMP);
	ParseInt(param, "ClassID", ClassID);
	// End:0x110
	if(ParseInt(param, "MasterID", MasterID))
	{
		m_MasterID = MasterID;
	}
	TooltipInfo.DrawList.Length = 1;
	TooltipInfo.DrawList[0].eType = 1;
	TooltipInfo.DrawList[0].t_bDrawOneLine = true;
	// End:0x192
	if(__NFUN_130__(__NFUN_151__(m_MasterID, 0), __NFUN_154__(m_MasterID, Id)))
	{
		TooltipInfo.DrawList[0].t_strText = __NFUN_112__(__NFUN_112__(__NFUN_112__(Name, "("), GetSystemString(408)), ")");		
	}
	else
	{
		TooltipInfo.DrawList[0].t_strText = Name;
	}
	m_PartyStatus[idx].SetTooltipCustomType(TooltipInfo);
	m_ClassIcon[idx].SetTexture(GetClassIconName(ClassID));
	TooltipInfo2.DrawList.Length = 2;
	TooltipInfo2.DrawList[0].eType = 1;
	TooltipInfo2.DrawList[0].t_bDrawOneLine = true;
	// End:0x266
	if(__NFUN_130__(__NFUN_151__(m_MasterID, 0), __NFUN_154__(m_MasterID, Id)))
	{
		TooltipInfo2.DrawList[0].t_strText = __NFUN_112__(__NFUN_112__(__NFUN_112__(Name, "("), GetSystemString(408)), ")");		
	}
	else
	{
		TooltipInfo2.DrawList[0].t_strText = Name;
	}
	TooltipInfo2.DrawList[1].eType = 1;
	TooltipInfo2.DrawList[1].nOffSetY = 2;
	TooltipInfo2.DrawList[1].t_bDrawOneLine = true;
	TooltipInfo2.DrawList[1].bLineBreak = true;
	TooltipInfo2.DrawList[1].t_strText = __NFUN_112__(__NFUN_112__(GetClassStr(ClassID), " - "), GetClassType(ClassID));
	TooltipInfo2.DrawList[1].t_color.R = 128;
	TooltipInfo2.DrawList[1].t_color.G = 128;
	TooltipInfo2.DrawList[1].t_color.B = 128;
	TooltipInfo2.DrawList[1].t_color.A = byte(255);
	m_ClassIcon[idx].SetTooltipCustomType(TooltipInfo2);
	UpdateCPBar(idx, CP, maxCP);
	UpdateHPBar(idx, HP, maxHP);
	UpdateMPBar(idx, MP, maxMP);
	return;
}

function HandlePartySpelledList(string param)
{
	local int i, idx, Id, Max, BuffCnt, BuffCurRow,
		DeBuffCnt, DeBuffCurRow;

	local StatusIconInfo Info;

	DeBuffCurRow = -1;
	BuffCurRow = -1;
	ParseInt(param, "ID", Id);
	// End:0x37
	if(__NFUN_150__(Id, 1))
	{
		return;
	}
	idx = FindPartyID(Id);
	// End:0x55
	if(__NFUN_150__(idx, 0))
	{
		return;
	}
	m_StatusIconBuff[idx].Clear();
	m_StatusIconDeBuff[idx].Clear();
	Info.Size = 10;
	Info.bShow = true;
	ParseInt(param, "Max", Max);
	i = 0;
	J0xB5:

	// End:0x22B [Loop If]
	if(__NFUN_150__(i, Max))
	{
		ParseInt(param, __NFUN_112__("SkillID_", string(i)), Info.ClassID);
		ParseInt(param, __NFUN_112__("Level_", string(i)), Info.Level);
		// End:0x221
		if(__NFUN_151__(Info.ClassID, 0))
		{
			Info.IconName = Class'NWindow.UIDATA_SKILL'.static.GetIconName(Info.ClassID, Info.Level);
			// End:0x1CA
			if(__NFUN_242__(IsDebuff(Info.ClassID, Info.Level), true))
			{
				// End:0x1A1
				if(__NFUN_180__(__NFUN_173__(float(DeBuffCnt), float(12)), float(0)))
				{
					__NFUN_165__(DeBuffCurRow);
					m_StatusIconDeBuff[idx].AddRow();
				}
				m_StatusIconDeBuff[idx].AddCol(DeBuffCurRow, Info);
				__NFUN_165__(DeBuffCnt);
				// [Explicit Continue]
				goto J0x221;
			}
			// End:0x1FB
			if(__NFUN_180__(__NFUN_173__(float(BuffCnt), float(12)), float(0)))
			{
				__NFUN_165__(BuffCurRow);
				m_StatusIconBuff[idx].AddRow();
			}
			m_StatusIconBuff[idx].AddCol(BuffCurRow, Info);
			__NFUN_165__(BuffCnt);
		}
		J0x221:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0xB5;
	}
	UpdateBuff();
	return;
}

function HandleShowBuffIcon(string param)
{
	local int nShow;

	ParseInt(param, "Show", nShow);
	m_CurBf = __NFUN_146__(m_CurBf, 1);
	// End:0x37
	if(__NFUN_151__(m_CurBf, 2))
	{
		m_CurBf = 0;
	}
	SetBuffButtonTooltip();
	switch(m_CurBf)
	{
		// End:0x51
		case 1:
			UpdateBuff();
			// End:0x73
			break;
		// End:0x5F
		case 2:
			UpdateBuff();
			// End:0x73
			break;
		// End:0x70
		case 0:
			m_CurBf = 0;
			UpdateBuff();
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickButton(string strID)
{
	local PartyWnd script;

	script = PartyWnd(GetScript("PartyWnd"));
	switch(strID)
	{
		// End:0x46
		case "btnBuff":
			OnBuffButton();
			script.OnBuffButton();
			// End:0x5D
			break;
		// End:0x5A
		case "btnOption":
			OnOpenPartyWndOption();
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnOpenPartyWndOption()
{
	local PartyWndOption script;

	script = PartyWndOption(GetScript("PartyWndOption"));
	script.ShowPartyWndOption();
	m_PartyOption.SetAnchor("PartyWndCompact.PartyStatusWnd0", "TopRight", "TopLeft", 5, 5);
	return;
}

function OnBuffButton()
{
	m_CurBf = __NFUN_146__(m_CurBf, 1);
	// End:0x21
	if(__NFUN_151__(m_CurBf, 2))
	{
		m_CurBf = 0;
	}
	SetBuffButtonTooltip();
	UpdateBuff();
	return;
}

function UpdateBuff()
{
	local int idx;

	// End:0x55
	if(__NFUN_154__(m_CurBf, 1))
	{
		idx = 0;
		J0x12:

		// End:0x52 [Loop If]
		if(__NFUN_150__(idx, 8))
		{
			m_StatusIconBuff[idx].ShowWindow();
			m_StatusIconDeBuff[idx].HideWindow();
			__NFUN_165__(idx);
			// [Loop Continue]
			goto J0x12;
		}		
	}
	else
	{
		// End:0xAB
		if(__NFUN_154__(m_CurBf, 2))
		{
			idx = 0;
			J0x68:

			// End:0xA8 [Loop If]
			if(__NFUN_150__(idx, 8))
			{
				m_StatusIconBuff[idx].HideWindow();
				m_StatusIconDeBuff[idx].ShowWindow();
				__NFUN_165__(idx);
				// [Loop Continue]
				goto J0x68;
			}			
		}
		else
		{
			idx = 0;
			J0xB2:

			// End:0xF2 [Loop If]
			if(__NFUN_150__(idx, 8))
			{
				m_StatusIconBuff[idx].HideWindow();
				m_StatusIconDeBuff[idx].HideWindow();
				__NFUN_165__(idx);
				// [Loop Continue]
				goto J0xB2;
			}
		}
	}
	return;
}

function UpdateCPBar(int idx, int Value, int MaxValue)
{
	m_BarCP[idx].SetValue(MaxValue, Value);
	return;
}

function UpdateHPBar(int idx, int Value, int MaxValue)
{
	m_BarHP[idx].SetValue(MaxValue, Value);
	return;
}

function UpdateMPBar(int idx, int Value, int MaxValue)
{
	m_BarMP[idx].SetValue(MaxValue, Value);
	return;
}

function OnLButtonDown(WindowHandle a_WindowHandle, int X, int Y)
{
	local Rect rectWnd;
	local int idx;

	rectWnd = m_wndTop.GetRect();
	// End:0x59
	if(__NFUN_151__(X, __NFUN_146__(rectWnd.nX, 30)))
	{
		idx = __NFUN_145__(__NFUN_147__(Y, rectWnd.nY), 26);
		RequestTargetUser(m_arrID[idx]);
	}
	return;
}

function OnRButtonUp(int X, int Y)
{
	local Rect rectWnd;
	local UserInfo UserInfo;
	local int idx;

	rectWnd = m_wndTop.GetRect();
	// End:0x71
	if(__NFUN_151__(X, __NFUN_146__(rectWnd.nX, 30)))
	{
		// End:0x71
		if(GetPlayerInfo(UserInfo))
		{
			idx = __NFUN_145__(__NFUN_147__(Y, rectWnd.nY), 26);
			RequestAssist(m_arrID[idx], UserInfo.Loc);
		}
	}
	return;
}

function SetBuffButtonTooltip()
{
	local int idx;

	switch(m_CurBf)
	{
		// End:0x19
		case 0:
			idx = 1496;
			// End:0x41
			break;
		// End:0x2B
		case 1:
			idx = 1497;
			// End:0x41
			break;
		// End:0x3E
		case 2:
			idx = 1498;
			// End:0x41
			break;
		// End:0xFFFF
		default:
			break;
	}
	btnBuff.SetTooltipCustomType(MakeTooltipSimpleText(GetSystemString(idx)));
	return;
}
