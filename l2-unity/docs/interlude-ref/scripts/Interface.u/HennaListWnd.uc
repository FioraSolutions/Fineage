class HennaListWnd extends UICommonAPI;

const FEE_OFFSET_Y_EQUIP = -13;
const FEE_OFFSET_Y_UNEQUIP = -12;
const HENNA_EQUIP = 1;
const HENNA_UNEQUIP = 2;

var int m_iState;
var int m_iRootNameLength;

function OnLoad()
{
	RegisterEvent(1640);
	RegisterEvent(1650);
	RegisterEvent(1670);
	RegisterEvent(1680);
	return;
}

function OnClickButton(string strID)
{
	local string strHennaID;

	switch(strID)
	{
		// End:0xFFFF
		default:
			strHennaID = __NFUN_127__(strID, __NFUN_146__(m_iRootNameLength, 1));
			// End:0x3A
			if(__NFUN_154__(m_iState, 1))
			{
				RequestHennaItemInfo(int(strHennaID));				
			}
			else
			{
				// End:0x53
				if(__NFUN_154__(m_iState, 2))
				{
					RequestHennaUnEquipInfo(int(strHennaID));
				}
			}
			// End:0x56
			break;
			break;
	}
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_TREECTRL'.static.Clear("HennaListWnd.HennaListTree");
	return;
}

function OnEvent(int Event_ID, string param)
{
	local int iAdena;
	local string strName, strIconName, strDescription;
	local int iHennaID, iClassID, iNum, iFee;

	switch(Event_ID)
	{
		// End:0x41
		case 1640:
			m_iState = 1;
			Clear();
			ParseInt(param, "Adena", iAdena);
			ShowHennaListWnd(iAdena);
			// End:0x160
			break;
		// End:0x49
		case 1650:
		// End:0x122
		case 1680:
			ParseString(param, "Name", strName);
			ParseString(param, "Description", strDescription);
			ParseString(param, "IconName", strIconName);
			ParseInt(param, "HennaID", iHennaID);
			ParseInt(param, "ClassID", iClassID);
			ParseInt(param, "NumOfItem", iNum);
			ParseInt(param, "Fee", iFee);
			AddHennaListItem(strName, strIconName, strDescription, iFee, iHennaID);
			// End:0x160
			break;
		// End:0x15D
		case 1670:
			m_iState = 2;
			Clear();
			ParseInt(param, "Adena", iAdena);
			ShowHennaListWnd(iAdena);
			// End:0x160
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ShowHennaListWnd(int iAdena)
{
	local XMLTreeNodeInfo infNode;
	local string strTmp;

	// End:0x66
	if(__NFUN_154__(m_iState, 1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("HennaListWnd", GetSystemString(651));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaListWnd.txtList", GetSystemString(659));		
	}
	else
	{
		// End:0xCA
		if(__NFUN_154__(m_iState, 2))
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("HennaListWnd", GetSystemString(652));
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaListWnd.txtList", GetSystemString(660));
		}
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("HennaListWnd.txtAdena", MakeCostString(__NFUN_112__("", string(iAdena))));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("HennaListWnd.txtAdena", ConvertNumToText(__NFUN_112__("", string(iAdena))));
	infNode.strName = "HennaListRoot";
	infNode.nOffSetX = 7;
	infNode.nOffSetY = -3;
	strTmp = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("HennaListWnd.HennaListTree", "", infNode);
	// End:0x1EF
	if(__NFUN_150__(__NFUN_125__(strTmp), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert root node. Name: ", infNode.strName));
		return;
	}
	m_iRootNameLength = __NFUN_125__(infNode.strName);
	ShowWindow("HennaListWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("HennaListWnd");
	return;
}

function AddHennaListItem(string strName, string strIconName, string strDescription, int iFee, int iHennaID)
{
	local XMLTreeNodeInfo infNode;
	local XMLTreeNodeItemInfo infNodeItem;
	local XMLTreeNodeInfo infNodeClear;
	local XMLTreeNodeItemInfo infNodeItemClear;
	local string strRetName, strAdenaComma;

	infNode = infNodeClear;
	infNode.strName = __NFUN_112__("", string(iHennaID));
	infNode.bShowButton = 0;
	infNode.nTexExpandedOffSetX = -7;
	infNode.nTexExpandedOffSetY = 8;
	infNode.nTexExpandedHeight = 46;
	infNode.nTexExpandedRightWidth = 0;
	infNode.nTexExpandedLeftUWidth = 32;
	infNode.nTexExpandedLeftUHeight = 40;
	infNode.strTexExpandedLeft = "L2UI_CH3.etc.IconSelect2";
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("HennaListWnd.HennaListTree", "HennaListRoot", infNode);
	// End:0x12A
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 2;
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = 15;
	infNodeItem.u_nTextureWidth = 32;
	infNodeItem.u_nTextureHeight = 32;
	infNodeItem.u_strTexture = strIconName;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("HennaListWnd.HennaListTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = strName;
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 5;
	// End:0x217
	if(__NFUN_154__(m_iState, 1))
	{
		infNodeItem.nOffSetY = 17;		
	}
	else
	{
		// End:0x230
		if(__NFUN_154__(m_iState, 2))
		{
			infNodeItem.nOffSetY = 10;
		}
	}
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("HennaListWnd.HennaListTree", strRetName, infNodeItem);
	// End:0x305
	if(__NFUN_154__(m_iState, 2))
	{
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		infNodeItem.t_strText = strDescription;
		infNodeItem.bLineBreak = true;
		infNodeItem.t_bDrawOneLine = true;
		infNodeItem.nOffSetX = 37;
		infNodeItem.nOffSetY = -24;
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("HennaListWnd.HennaListTree", strRetName, infNodeItem);
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = __NFUN_112__(GetSystemString(637), " : ");
	infNodeItem.bLineBreak = true;
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 37;
	// End:0x37F
	if(__NFUN_154__(m_iState, 1))
	{
		infNodeItem.nOffSetY = -13;		
	}
	else
	{
		// End:0x39B
		if(__NFUN_154__(m_iState, 2))
		{
			infNodeItem.nOffSetY = -12;
		}
	}
	infNodeItem.t_color.R = 168;
	infNodeItem.t_color.G = 168;
	infNodeItem.t_color.B = 168;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("HennaListWnd.HennaListTree", strRetName, infNodeItem);
	strAdenaComma = MakeCostString(__NFUN_112__("", string(iFee)));
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = strAdenaComma;
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 0;
	// End:0x490
	if(__NFUN_154__(m_iState, 1))
	{
		infNodeItem.nOffSetY = -13;		
	}
	else
	{
		// End:0x4AC
		if(__NFUN_154__(m_iState, 2))
		{
			infNodeItem.nOffSetY = -12;
		}
	}
	infNodeItem.t_color = GetNumericColor(strAdenaComma);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("HennaListWnd.HennaListTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSystemString(469);
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 5;
	// End:0x55D
	if(__NFUN_154__(m_iState, 1))
	{
		infNodeItem.nOffSetY = -13;		
	}
	else
	{
		// End:0x579
		if(__NFUN_154__(m_iState, 2))
		{
			infNodeItem.nOffSetY = -12;
		}
	}
	infNodeItem.t_color.R = byte(255);
	infNodeItem.t_color.G = byte(255);
	infNodeItem.t_color.B = 0;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("HennaListWnd.HennaListTree", strRetName, infNodeItem);
	return;
}
