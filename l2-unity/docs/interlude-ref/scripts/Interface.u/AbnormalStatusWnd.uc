class AbnormalStatusWnd extends UIScript;

const NSTATUSICON_FRAMESIZE = 12;
const NSTATUSICON_MAXCOL = 12;

var int m_NormalStatusRow;
var int m_EtcStatusRow;
var int m_ShortStatusRow;
var bool m_bOnCurState;

function OnLoad()
{
	RegisterEvent(950);
	RegisterEvent(960);
	RegisterEvent(970);
	RegisterEvent(40);
	RegisterEvent(50);
	RegisterEvent(310);
	m_NormalStatusRow = -1;
	m_EtcStatusRow = -1;
	m_ShortStatusRow = -1;
	m_bOnCurState = false;
	return;
}

function OnEnterState(name a_PreStateName)
{
	m_bOnCurState = true;
	UpdateWindowSize();
	return;
}

function OnExitState(name a_NextStateName)
{
	m_bOnCurState = false;
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x1D
	if(__NFUN_154__(Event_ID, 950))
	{
		HandleAddNormalStatus(param);		
	}
	else
	{
		// End:0x3A
		if(__NFUN_154__(Event_ID, 960))
		{
			HandleAddEtcStatus(param);			
		}
		else
		{
			// End:0x57
			if(__NFUN_154__(Event_ID, 970))
			{
				HandleAddShortStatus(param);				
			}
			else
			{
				// End:0x6C
				if(__NFUN_154__(Event_ID, 40))
				{
					HandleRestart();					
				}
				else
				{
					// End:0x81
					if(__NFUN_154__(Event_ID, 50))
					{
						HandleDie();						
					}
					else
					{
						// End:0x96
						if(__NFUN_154__(Event_ID, 310))
						{
							HandleShowReplayQuitDialogBox();
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
	ClearAll();
	return;
}

function HandleDie()
{
	ClearStatus(false, false);
	ClearStatus(false, true);
	return;
}

function HandleShowReplayQuitDialogBox()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("AbnormalStatusWnd");
	return;
}

function OnShow()
{
	local int RowCount;

	RowCount = Class'NWindow.UIAPI_STATUSICONCTRL'.static.GetRowCount("AbnormalStatusWnd.StatusIcon");
	// End:0x60
	if(__NFUN_150__(RowCount, 1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("AbnormalStatusWnd");
	}
	return;
}

function ClearStatus(bool bEtcItem, bool bShortItem)
{
	local int i, j, RowCount, RowCountTmp, ColCount;

	local StatusIconInfo Info;

	// End:0x25
	if(__NFUN_130__(__NFUN_242__(bEtcItem, false), __NFUN_242__(bShortItem, false)))
	{
		m_NormalStatusRow = -1;
	}
	// End:0x4A
	if(__NFUN_130__(__NFUN_242__(bEtcItem, true), __NFUN_242__(bShortItem, false)))
	{
		m_EtcStatusRow = -1;
	}
	// End:0x6F
	if(__NFUN_130__(__NFUN_242__(bEtcItem, false), __NFUN_242__(bShortItem, true)))
	{
		m_ShortStatusRow = -1;
	}
	RowCount = Class'NWindow.UIAPI_STATUSICONCTRL'.static.GetRowCount("AbnormalStatusWnd.StatusIcon");
	i = 0;
	J0xA9:

	// End:0x229 [Loop If]
	if(__NFUN_150__(i, RowCount))
	{
		ColCount = Class'NWindow.UIAPI_STATUSICONCTRL'.static.GetColCount("AbnormalStatusWnd.StatusIcon", i);
		j = 0;
		J0xF7:

		// End:0x21F [Loop If]
		if(__NFUN_150__(j, ColCount))
		{
			Class'NWindow.UIAPI_STATUSICONCTRL'.static.GetItem("AbnormalStatusWnd.StatusIcon", i, j, Info);
			// End:0x215
			if(__NFUN_151__(Info.ClassID, 0))
			{
				// End:0x215
				if(__NFUN_130__(__NFUN_242__(Info.bEtcItem, bEtcItem), __NFUN_242__(Info.bShortItem, bShortItem)))
				{
					Class'NWindow.UIAPI_STATUSICONCTRL'.static.DelItem("AbnormalStatusWnd.StatusIcon", i, j);
					__NFUN_166__(j);
					__NFUN_166__(ColCount);
					RowCountTmp = Class'NWindow.UIAPI_STATUSICONCTRL'.static.GetRowCount("AbnormalStatusWnd.StatusIcon");
					// End:0x215
					if(__NFUN_155__(RowCountTmp, RowCount))
					{
						__NFUN_166__(i);
						__NFUN_166__(RowCount);
					}
				}
			}
			__NFUN_165__(j);
			// [Loop Continue]
			goto J0xF7;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0xA9;
	}
	return;
}

function ClearAll()
{
	ClearStatus(false, false);
	ClearStatus(true, false);
	ClearStatus(false, true);
	return;
}

function HandleAddNormalStatus(string param)
{
	local int i, Max, BuffCnt;
	local StatusIconInfo Info;

	ClearStatus(false, false);
	Info.Size = 24;
	Info.BackTex = "L2UI.EtcWndBack.AbnormalBack";
	Info.bShow = true;
	Info.bEtcItem = false;
	Info.bShortItem = false;
	BuffCnt = 0;
	ParseInt(param, "Max", Max);
	i = 0;
	J0x88:

	// End:0x235 [Loop If]
	if(__NFUN_150__(i, Max))
	{
		ParseInt(param, __NFUN_112__("SkillID_", string(i)), Info.ClassID);
		ParseInt(param, __NFUN_112__("SkillLevel_", string(i)), Info.Level);
		ParseInt(param, __NFUN_112__("RemainTime_", string(i)), Info.RemainTime);
		ParseString(param, __NFUN_112__("Name_", string(i)), Info.Name);
		ParseString(param, __NFUN_112__("IconName_", string(i)), Info.IconName);
		ParseString(param, __NFUN_112__("Description_", string(i)), Info.Description);
		// End:0x22B
		if(__NFUN_151__(Info.ClassID, 0))
		{
			// End:0x1ED
			if(__NFUN_180__(__NFUN_173__(float(BuffCnt), float(12)), float(0)))
			{
				__NFUN_165__(m_NormalStatusRow);
				Class'NWindow.UIAPI_STATUSICONCTRL'.static.InsertRow("AbnormalStatusWnd.StatusIcon", m_NormalStatusRow);
			}
			Class'NWindow.UIAPI_STATUSICONCTRL'.static.AddCol("AbnormalStatusWnd.StatusIcon", m_NormalStatusRow, Info);
			__NFUN_165__(BuffCnt);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x88;
	}
	// End:0x252
	if(__NFUN_151__(m_EtcStatusRow, -1))
	{
		m_EtcStatusRow = __NFUN_146__(m_NormalStatusRow, 1);
	}
	// End:0x26F
	if(__NFUN_151__(m_ShortStatusRow, -1))
	{
		m_ShortStatusRow = __NFUN_146__(m_NormalStatusRow, 1);
	}
	UpdateWindowSize();
	return;
}

function HandleAddEtcStatus(string param)
{
	local int i, Max, BuffCnt, CurRow;
	local bool bNewRow;
	local StatusIconInfo Info;

	ClearStatus(true, false);
	Info.Size = 24;
	Info.BackTex = "L2UI.EtcWndBack.AbnormalBack";
	Info.bShow = true;
	Info.bEtcItem = true;
	Info.bShortItem = false;
	// End:0x8A
	if(__NFUN_151__(m_ShortStatusRow, -1))
	{
		bNewRow = false;
		CurRow = m_ShortStatusRow;		
	}
	else
	{
		bNewRow = true;
		CurRow = m_NormalStatusRow;
	}
	BuffCnt = 0;
	ParseInt(param, "Max", Max);
	i = 0;
	J0xC0:

	// End:0x27B [Loop If]
	if(__NFUN_150__(i, Max))
	{
		ParseInt(param, __NFUN_112__("SkillID_", string(i)), Info.ClassID);
		ParseInt(param, __NFUN_112__("SkillLevel_", string(i)), Info.Level);
		ParseInt(param, __NFUN_112__("RemainTime_", string(i)), Info.RemainTime);
		ParseString(param, __NFUN_112__("Name_", string(i)), Info.Name);
		ParseString(param, __NFUN_112__("IconName_", string(i)), Info.IconName);
		ParseString(param, __NFUN_112__("Description_", string(i)), Info.Description);
		// End:0x271
		if(__NFUN_151__(Info.ClassID, 0))
		{
			// End:0x228
			if(bNewRow)
			{
				bNewRow = __NFUN_129__(bNewRow);
				__NFUN_165__(CurRow);
				Class'NWindow.UIAPI_STATUSICONCTRL'.static.InsertRow("AbnormalStatusWnd.StatusIcon", CurRow);
			}
			Class'NWindow.UIAPI_STATUSICONCTRL'.static.AddCol("AbnormalStatusWnd.StatusIcon", CurRow, Info);
			m_EtcStatusRow = CurRow;
			__NFUN_165__(BuffCnt);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0xC0;
	}
	UpdateWindowSize();
	return;
}

function HandleAddShortStatus(string param)
{
	local int i, Max, BuffCnt, CurRow, CurCol;

	local bool bNewRow;
	local StatusIconInfo Info;

	ClearStatus(false, true);
	Info.Size = 24;
	Info.BackTex = "L2UI.EtcWndBack.AbnormalBack";
	Info.bShow = true;
	Info.bEtcItem = false;
	Info.bShortItem = true;
	CurCol = -1;
	// End:0x95
	if(__NFUN_151__(m_EtcStatusRow, -1))
	{
		bNewRow = false;
		CurRow = m_EtcStatusRow;		
	}
	else
	{
		bNewRow = true;
		CurRow = m_NormalStatusRow;
	}
	BuffCnt = 0;
	ParseInt(param, "Max", Max);
	i = 0;
	J0xCB:

	// End:0x292 [Loop If]
	if(__NFUN_150__(i, Max))
	{
		ParseInt(param, __NFUN_112__("SkillID_", string(i)), Info.ClassID);
		ParseInt(param, __NFUN_112__("SkillLevel_", string(i)), Info.Level);
		ParseInt(param, __NFUN_112__("RemainTime_", string(i)), Info.RemainTime);
		ParseString(param, __NFUN_112__("Name_", string(i)), Info.Name);
		ParseString(param, __NFUN_112__("IconName_", string(i)), Info.IconName);
		ParseString(param, __NFUN_112__("Description_", string(i)), Info.Description);
		// End:0x288
		if(__NFUN_151__(Info.ClassID, 0))
		{
			// End:0x233
			if(bNewRow)
			{
				bNewRow = __NFUN_129__(bNewRow);
				__NFUN_165__(CurRow);
				Class'NWindow.UIAPI_STATUSICONCTRL'.static.InsertRow("AbnormalStatusWnd.StatusIcon", CurRow);
			}
			__NFUN_165__(CurCol);
			Class'NWindow.UIAPI_STATUSICONCTRL'.static.InsertCol("AbnormalStatusWnd.StatusIcon", CurRow, CurCol, Info);
			m_ShortStatusRow = CurRow;
			__NFUN_165__(BuffCnt);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0xCB;
	}
	UpdateWindowSize();
	return;
}

function UpdateWindowSize()
{
	local int RowCount;
	local Rect rectWnd;

	RowCount = Class'NWindow.UIAPI_STATUSICONCTRL'.static.GetRowCount("AbnormalStatusWnd.StatusIcon");
	// End:0x12C
	if(__NFUN_151__(RowCount, 0))
	{
		// End:0x6C
		if(m_bOnCurState)
		{
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("AbnormalStatusWnd");			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("AbnormalStatusWnd");
		}
		rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect("AbnormalStatusWnd.StatusIcon");
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("AbnormalStatusWnd", __NFUN_146__(rectWnd.nWidth, 12), rectWnd.nHeight);
		Class'NWindow.UIAPI_WINDOW'.static.SetFrameSize("AbnormalStatusWnd", 12, rectWnd.nHeight);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("AbnormalStatusWnd");
	}
	return;
}
