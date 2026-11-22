class PCCafeEventWnd extends UICommonAPI;

var int m_TotalPoint;
var int m_AddPoint;
var int m_PeriodType;
var int m_RemainTime;
var int m_PointType;
var WindowHandle HelpButton;

function OnLoad()
{
	RegisterEvent(1910);
	HelpButton = GetHandle("PCCafeEventWnd.HelpButton");
	HideWindow("PCCafeEventWnd.PointAddTextBox");
	return;
}

function OnClickButton(string a_ButtonID)
{
	switch(a_ButtonID)
	{
		// End:0x1F
		case "HelpButton":
			OnClickHelpButton();
			// End:0x22
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
		// End:0x1D
		case 1910:
			HandlePCCafePointInfo(a_Param);
			// End:0x20
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickHelpButton()
{
	return;
}

function HandlePCCafePointInfo(string a_Param)
{
	ParseInt(a_Param, "TotalPoint", m_TotalPoint);
	ParseInt(a_Param, "AddPoint", m_AddPoint);
	ParseInt(a_Param, "PeriodType", m_PeriodType);
	ParseInt(a_Param, "RemainTime", m_RemainTime);
	ParseInt(a_Param, "PointType", m_PointType);
	Refresh();
	return;
}

function bool IsPCCafeEventOpened()
{
	// End:0x0D
	if(__NFUN_150__(0, m_PeriodType))
	{
		return true;
	}
	return false;
	return;
}

function OnEnterState(name a_PreStateName)
{
	Refresh();
	return;
}

function Refresh()
{
	local Color TextColor;
	local string AddPointText;

	// End:0x321
	if(IsPCCafeEventOpened())
	{
		ShowWindow("PCCafeEventWnd");
		HelpButton.SetTooltipCustomType(SetTooltip(GetHelpButtonTooltipText()));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PCCafeEventWnd.PointTextBox", MakeCostString(string(m_TotalPoint)));
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha("PCCafeEventWnd.PointAddTextBox", 0);
		// End:0x31E
		if(__NFUN_155__(0, m_AddPoint))
		{
			// End:0xD4
			if(__NFUN_150__(0, m_AddPoint))
			{
				AddPointText = __NFUN_112__("+", MakeCostString(string(m_AddPoint)));				
			}
			else
			{
				AddPointText = MakeCostString(string(m_AddPoint));
			}
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PCCafeEventWnd.PointAddTextBox", AddPointText);
			switch(m_PointType)
			{
				// End:0x154
				case 0:
					TextColor.R = byte(255);
					TextColor.G = byte(255);
					TextColor.B = 0;
					// End:0x1BA
					break;
				// End:0x184
				case 1:
					TextColor.R = byte(255);
					TextColor.G = 0;
					TextColor.B = 0;
					// End:0x1BA
					break;
				// End:0x1B7
				case 2:
					TextColor.R = 0;
					TextColor.G = byte(255);
					TextColor.B = byte(255);
					// End:0x1BA
					break;
				// End:0xFFFF
				default:
					break;
			}
			Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("PCCafeEventWnd.PointAddTextBox", TextColor);
			Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("PCCafeEventWnd.PointAddTextBox", "PCCafeEventWnd", "TopRight", "TopRight", -5, 41);
			Class'NWindow.UIAPI_WINDOW'.static.ClearAnchor("PCCafeEventWnd.PointAddTextBox");
			Class'NWindow.UIAPI_WINDOW'.static.Move("PCCafeEventWnd.PointAddTextBox", 0, -18, 1.0000000);
			Class'NWindow.UIAPI_WINDOW'.static.SetAlpha("PCCafeEventWnd.PointAddTextBox", 255);
			Class'NWindow.UIAPI_WINDOW'.static.SetAlpha("PCCafeEventWnd.PointAddTextBox", 0, 0.8000000);
			m_AddPoint = 0;
		}		
	}
	else
	{
		HideWindow("PCCafeEventWnd");
	}
	return;
}

function string GetHelpButtonTooltipText()
{
	local string TooltipSystemMsg;

	// End:0x1F
	if(__NFUN_154__(1, m_PeriodType))
	{
		TooltipSystemMsg = GetSystemMessage(1705);		
	}
	else
	{
		// End:0x3F
		if(__NFUN_154__(2, m_PeriodType))
		{
			TooltipSystemMsg = GetSystemMessage(1706);			
		}
		else
		{
			return "";
		}
	}
	return MakeFullSystemMsg(TooltipSystemMsg, string(m_RemainTime), "");
	return;
}

function CustomTooltip SetTooltip(string Text)
{
	local CustomTooltip ToolTip;
	local DrawItemInfo Info;

	ToolTip.MinimumWidth = 144;
	ToolTip.DrawList.Length = 1;
	Info.eType = 1;
	Info.t_bDrawOneLine = true;
	Info.t_color.R = 178;
	Info.t_color.G = 190;
	Info.t_color.B = 207;
	Info.t_color.A = byte(255);
	Info.t_strText = Text;
	ToolTip.DrawList[0] = Info;
	return ToolTip;
	return;
}
