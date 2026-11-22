class HennaInfoWnd extends UIScript;

const HENNA_EQUIP = 1;
const HENNA_UNEQUIP = 2;

var int m_iState;
var int m_iHennaID;

function OnLoad()
{
	RegisterEvent(1660);
	RegisterEvent(1690);
	return;
}

function OnClickButton(string strID)
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("HennaInfoWnd");
	switch(strID)
	{
		// End:0x59
		case "btnPrev":
			// End:0x44
			if(__NFUN_154__(m_iState, 1))
			{
				RequestHennaItemList();				
			}
			else
			{
				// End:0x56
				if(__NFUN_154__(m_iState, 2))
				{
					RequestHennaUnEquipList();
				}
			}
			// End:0x99
			break;
		// End:0x96
		case "btnOK":
			// End:0x7C
			if(__NFUN_154__(m_iState, 1))
			{
				RequestHennaEquip(m_iHennaID);				
			}
			else
			{
				// End:0x93
				if(__NFUN_154__(m_iState, 2))
				{
					RequestHennaUnEquip(m_iHennaID);
				}
			}
			// End:0x99
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnShow()
{
	// End:0x96
	if(__NFUN_154__(m_iState, 1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("HennaInfoWnd", GetSystemString(651));
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("HennaInfoWnd.HennaInfoWndUnEquip");
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("HennaInfoWnd.HennaInfoWndEquip");		
	}
	else
	{
		// End:0x12D
		if(__NFUN_154__(m_iState, 2))
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("HennaInfoWnd", GetSystemString(652));
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("HennaInfoWnd.HennaInfoWndEquip");
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("HennaInfoWnd.HennaInfoWndUnEquip");			
		}
		else
		{
			Debug("¿¡·¯¿¡·¯ ÀÌ»óÀÌ»ó~~");
		}
	}
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x24
		case 1660:
			m_iState = 1;
			ShowHennaInfoWnd(param);
			// End:0x45
			break;
		// End:0x42
		case 1690:
			m_iState = 2;
			ShowHennaInfoWnd(param);
			// End:0x45
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ShowHennaInfoWnd(string param)
{
	local string strAdenaComma;
	local int iAdena;
	local string strDyeName, strDyeIconName;
	local int iHennaID, iClassID, iNum, iFee;
	local string strTattooName, strTattooAddName, strTattooIconName;
	local int iINTnow, iINTchange, iSTRnow, iSTRchange, iCONnow, iCONchange,
		iMENnow, iMENchange, iDEXnow, iDEXchange, iWITnow,
		iWITchange;

	local Color col;

	ParseInt(param, "Adena", iAdena);
	ParseString(param, "DyeIconName", strDyeIconName);
	ParseString(param, "DyeName", strDyeName);
	ParseInt(param, "HennaID", iHennaID);
	ParseInt(param, "ClassID", iClassID);
	ParseInt(param, "NumOfItem", iNum);
	ParseInt(param, "Fee", iFee);
	ParseString(param, "TattooIconName", strTattooIconName);
	ParseString(param, "TattooName", strTattooName);
	ParseString(param, "TattooAddName", strTattooAddName);
	ParseInt(param, "INTnow", iINTnow);
	ParseInt(param, "INTchange", iINTchange);
	ParseInt(param, "STRnow", iSTRnow);
	ParseInt(param, "STRchange", iSTRchange);
	ParseInt(param, "CONnow", iCONnow);
	ParseInt(param, "CONchange", iCONchange);
	ParseInt(param, "MENnow", iMENnow);
	ParseInt(param, "MENchange", iMENchange);
	ParseInt(param, "DEXnow", iDEXnow);
	ParseInt(param, "DEXchange", iDEXchange);
	ParseInt(param, "WITnow", iWITnow);
	ParseInt(param, "WITchange", iWITchange);
	m_iHennaID = iHennaID;
	// End:0x552
	if(__NFUN_154__(m_iState, 1))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtDyeInfo", GetSystemString(638));
		Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("HennaInfoWnd.textureDyeIconName", strDyeIconName);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtDyeName", strDyeName);
		col.R = 168;
		col.G = 168;
		col.B = 168;
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtFee", __NFUN_112__(GetSystemString(637), " : "));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("HennaInfoWnd.txtFee", col);
		strAdenaComma = MakeCostString(__NFUN_112__("", string(iFee)));
		col = GetNumericColor(strAdenaComma);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtAdena", strAdenaComma);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("HennaInfoWnd.txtAdena", col);
		col.R = byte(255);
		col.G = byte(255);
		col.B = 0;
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtAdenaString", GetSystemString(469));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("HennaInfoWnd.txtAdenaString", col);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtTattooInfo", GetSystemString(639));
		Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("HennaInfoWnd.textureTattooIconName", strTattooIconName);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtTattooName", strTattooName);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtTattooAddName", strTattooAddName);		
	}
	else
	{
		// End:0x8C8
		if(__NFUN_154__(m_iState, 2))
		{
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtTattooInfoUnEquip", GetSystemString(639));
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("HennaInfoWnd.textureTattooIconNameUnEquip", strTattooIconName);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtTattooNameUnEquip", __NFUN_112__(__NFUN_112__(GetSystemString(652), ":"), strTattooName));
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtTattooAddNameUnEquip", strTattooAddName);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtDyeInfoUnEquip", GetSystemString(638));
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("HennaInfoWnd.textureDyeIconNameUnEquip", strDyeIconName);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtDyeNameUnEquip", strDyeName);
			col.R = 168;
			col.G = 168;
			col.B = 168;
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtFeeUnEquip", __NFUN_112__(GetSystemString(637), " : "));
			Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("HennaInfoWnd.txtFeeUnEquip", col);
			strAdenaComma = MakeCostString(__NFUN_112__("", string(iFee)));
			col = GetNumericColor(strAdenaComma);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtAdenaUnEquip", strAdenaComma);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("HennaInfoWnd.txtAdenaUnEquip", col);
			col.R = byte(255);
			col.G = byte(255);
			col.B = 0;
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtAdenaStringUnEquip", GetSystemString(469));
			Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("HennaInfoWnd.txtAdenaStringUnEquip", col);
		}
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtSTRBefore", iSTRnow);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtSTRAfter", iSTRchange);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtDEXBefore", iDEXnow);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtDEXAfter", iDEXchange);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtCONBefore", iCONnow);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtCONAfter", iCONchange);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtINTBefore", iINTnow);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtINTAfter", iINTchange);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtWITBefore", iWITnow);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtWITAfter", iWITchange);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtMENBefore", iMENnow);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("HennaInfoWnd.txtMENAfter", iMENchange);
	strAdenaComma = MakeCostString(__NFUN_112__("", string(iAdena)));
	col = GetNumericColor(strAdenaComma);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaInfoWnd.txtHaveAdena", strAdenaComma);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("HennaInfoWnd.txtHaveAdena", ConvertNumToText(__NFUN_112__("", string(iAdena))));
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("HennaListWnd");
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("HennaInfoWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("HennaInfoWnd");
	return;
}
