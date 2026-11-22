class PartyWndOption extends UIScript;

var bool m_OptionShow;
var WindowHandle m_PartyOption;
var WindowHandle m_PartyWndBig;
var WindowHandle m_PartyWndSmall;

function OnLoad()
{
	m_OptionShow = false;
	m_PartyOption = GetHandle("PartyWndOption");
	m_PartyWndBig = GetHandle("PartyWnd");
	m_PartyWndSmall = GetHandle("PartyWndCompact");
	return;
}

function OnShow()
{
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ShowSmallPartyWndCheck", GetOptionBool("Game", "SmallPartyWnd"));
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PartyWndOption");
	m_OptionShow = true;
	return;
}

function OnClickCheckBox(string CheckBoxID)
{
	switch(CheckBoxID)
	{
		// End:0x25
		case "ShowSmallPartyWndCheck":
			// End:0x28
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function SwapBigandSmall()
{
	local PartyWnd script1;
	local PartyWndCompact Script2;

	script1 = PartyWnd(GetScript("PartyWnd"));
	Script2 = PartyWndCompact(GetScript("PartyWndCompact"));
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("PartyWndCompact", "PartyWnd", "TopLeft", "TopLeft", 0, 0);
	script1.ResizeWnd();
	Script2.ResizeWnd();
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0xA3
		case "okbtn":
			switch(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ShowSmallPartyWndCheck"))
			{
				// End:0x5D
				case true:
					SetOptionBool("Game", "SmallPartyWnd", true);
					// End:0x83
					break;
				// End:0x80
				case false:
					SetOptionBool("Game", "SmallPartyWnd", false);
					// End:0x83
					break;
				// End:0xFFFF
				default:
					break;
			}
			SwapBigandSmall();
			m_PartyOption.HideWindow();
			m_OptionShow = false;
			// End:0xA6
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ShowPartyWndOption()
{
	// End:0x26
	if(__NFUN_242__(m_OptionShow, false))
	{
		m_PartyOption.ShowWindow();
		m_OptionShow = true;		
	}
	else
	{
		m_PartyOption.HideWindow();
		m_OptionShow = false;
	}
	return;
}
