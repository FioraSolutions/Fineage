class SkillTrainListWnd extends UICommonAPI;

const NORMAL_SKILL = 0;
const FISHING_SKILL = 1;
const CLAN_SKILL = 2;
const ENCHANT_SKILL = 3;
const OFFSET_X_ICON_TEXTURE = 0;
const OFFSET_Y_ICON_TEXTURE = 4;
const OFFSET_Y_SECONDLINE = -14;

var int m_iType;
var int m_iState;
var int m_iRootNameLength;
var WindowHandle m_SkillTrainListWnd;

function OnLoad()
{
	RegisterEvent(2010);
	RegisterEvent(2020);
	RegisterEvent(2030);
	m_SkillTrainListWnd = GetHandle("SkillTrainListWnd.SkillTrainListTree");
	return;
}

function OnClickButton(string strItemID)
{
	local string strID_Level, strID, strLevel;
	local int iID, iLevel, iIdxComma, iLength;

	strID_Level = __NFUN_127__(strItemID, __NFUN_146__(m_iRootNameLength, 1));
	iLength = __NFUN_125__(strID_Level);
	iIdxComma = __NFUN_126__(strID_Level, ",");
	strID = __NFUN_128__(strID_Level, iIdxComma);
	strLevel = __NFUN_234__(strID_Level, __NFUN_147__(__NFUN_147__(iLength, iIdxComma), 1));
	iID = int(strID);
	iLevel = int(strLevel);
	switch(m_iType)
	{
		// End:0x85
		case 0:
		// End:0x89
		case 1:
		// End:0xA6
		case 2:
			RequestAcquireSkillInfo(iID, iLevel, m_iType);
			// End:0xC1
			break;
		// End:0xBE
		case 3:
			RequestExEnchantSkillInfo(iID, iLevel);
			// End:0xC1
			break;
		// End:0xFFFF
		default:
			break;
	}
	HideWindow("SkillTrainListWnd");
	m_SkillTrainListWnd.SetScrollPosition(0);
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_TREECTRL'.static.Clear("SkillTrainListWnd.SkillTrainListTree");
	return;
}

function OnEvent(int Event_ID, string param)
{
	local int iType;
	local string strIconName, strName;
	local int iID, iLevel, iSPConsume;
	local string strEnchantName;

	switch(Event_ID)
	{
		// End:0x79
		case 2010:
			ParseInt(param, "Type", iType);
			Clear();
			m_iType = iType;
			// End:0x6B
			if(IsShowWindow("SkillTrainInfoWnd"))
			{
				HideWindow("SkillTrainInfoWnd");
			}
			ShowSkillTrainListWnd(iType);
			// End:0x18A
			break;
		// End:0x147
		case 2030:
			ParseString(param, "strIconName", strIconName);
			ParseString(param, "strName", strName);
			ParseInt(param, "iID", iID);
			ParseInt(param, "iLevel", iLevel);
			ParseInt(param, "iSPConsume", iSPConsume);
			ParseString(param, "strEnchantName", strEnchantName);
			AddSkillTrainListItem(strIconName, strName, iID, iLevel, iSPConsume, strEnchantName);
			// End:0x18A
			break;
		// End:0x187
		case 2020:
			// End:0x184
			if(IsShowWindow("SkillTrainListWnd"))
			{
				HideWindow("SkillTrainListWnd");
			}
			// End:0x18A
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnShow()
{
	// End:0x53
	if(__NFUN_154__(m_iType, 3))
	{
		HideWindow("SkillTrainListWnd.txtSPString");
		HideWindow("SkillTrainListWnd.txtSP");		
	}
	else
	{
		ShowWindow("SkillTrainListWnd.txtSPString");
		ShowWindow("SkillTrainListWnd.txtSP");
	}
	return;
}

function ShowSkillTrainListWnd(int iType)
{
	local XMLTreeNodeInfo infNode;
	local string strTmp;
	local int iWindowTitle, iSPIdx;
	local UserInfo infoPlayer;
	local int iPlayerSP;

	GetPlayerInfo(infoPlayer);
	switch(m_iType)
	{
		// End:0x16
		case 0:
		// End:0x1A
		case 1:
		// End:0x45
		case 3:
			iWindowTitle = 477;
			iSPIdx = 92;
			iPlayerSP = infoPlayer.nSP;
			// End:0x7C
			break;
		// End:0x79
		case 2:
			iWindowTitle = 1436;
			iSPIdx = 1372;
			iPlayerSP = GetClanNameValue(infoPlayer.nClanID);
			// End:0x7C
			break;
		// End:0xFFFF
		default:
			break;
	}
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle("SkillTrainListWnd", iWindowTitle);
	// End:0x115
	if(__NFUN_155__(m_iType, 3))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainListWnd.txtSPString", GetSystemString(iSPIdx));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("SkillTrainListWnd.txtSP", iPlayerSP);
	}
	infNode.strName = "SkillTrainListRoot";
	infNode.nOffSetX = 7;
	infNode.nOffSetY = 0;
	strTmp = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SkillTrainListWnd.SkillTrainListTree", "", infNode);
	// End:0x1D7
	if(__NFUN_150__(__NFUN_125__(strTmp), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert root node. Name: ", infNode.strName));
		return;
	}
	m_iRootNameLength = __NFUN_125__(infNode.strName);
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SkillTrainListWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("SkillTrainListWnd");
	return;
}

function AddSkillTrainListItem(string strIconName, string strName, int iID, int iLevel, int iSPConsume, string strEnchantName)
{
	local XMLTreeNodeInfo infNode;
	local XMLTreeNodeItemInfo infNodeItem;
	local XMLTreeNodeInfo infNodeClear;
	local XMLTreeNodeItemInfo infNodeItemClear;
	local string strRetName;

	infNode = infNodeClear;
	infNode.strName = __NFUN_112__(__NFUN_112__(__NFUN_112__("", string(iID)), ","), string(iLevel));
	infNode.bShowButton = 0;
	infNode.nTexExpandedOffSetX = -7;
	infNode.nTexExpandedOffSetY = 2;
	infNode.nTexExpandedHeight = 38;
	infNode.nTexExpandedRightWidth = 0;
	infNode.nTexExpandedLeftUWidth = 32;
	infNode.nTexExpandedLeftUHeight = 38;
	infNode.strTexExpandedLeft = "L2UI_CH3.etc.IconSelect2";
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("SkillTrainListWnd.SkillTrainListTree", "SkillTrainListRoot", infNode);
	// End:0x147
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 2;
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = 4;
	infNodeItem.u_nTextureWidth = 34;
	infNodeItem.u_nTextureHeight = 34;
	infNodeItem.u_strTexture = "l2ui_ch3.InventoryWnd.Inventory_OutLine";
	InsertNodeItem(strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 2;
	infNodeItem.nOffSetX = -33;
	infNodeItem.nOffSetY = 4;
	infNodeItem.u_nTextureWidth = 35;
	infNodeItem.u_nTextureHeight = 35;
	infNodeItem.u_strTexture = "l2ui_ch3.InventoryWnd.Inventory_OutLine";
	InsertNodeItem(strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 2;
	infNodeItem.nOffSetX = -35;
	infNodeItem.nOffSetY = __NFUN_146__(4, 1);
	infNodeItem.u_nTextureWidth = 32;
	infNodeItem.u_nTextureHeight = 32;
	infNodeItem.u_strTexture = strIconName;
	InsertNodeItem(strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = strName;
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 3;
	infNodeItem.nOffSetY = 10;
	InsertNodeItem(strRetName, infNodeItem);
	switch(m_iType)
	{
		// End:0x345
		case 0:
		// End:0x349
		case 1:
		// End:0x641
		case 2:
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 1;
			infNodeItem.t_strText = GetSystemString(88);
			infNodeItem.bLineBreak = true;
			infNodeItem.t_bDrawOneLine = true;
			infNodeItem.nOffSetX = 37;
			infNodeItem.nOffSetY = -14;
			infNodeItem.t_color.R = 163;
			infNodeItem.t_color.G = 163;
			infNodeItem.t_color.B = 163;
			infNodeItem.t_color.A = byte(255);
			InsertNodeItem(strRetName, infNodeItem);
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 1;
			infNodeItem.t_strText = string(iLevel);
			infNodeItem.nOffSetX = 2;
			infNodeItem.nOffSetY = -14;
			infNodeItem.t_color.R = 176;
			infNodeItem.t_color.G = 155;
			infNodeItem.t_color.B = 121;
			infNodeItem.t_color.A = byte(255);
			InsertNodeItem(strRetName, infNodeItem);
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 1;
			switch(m_iType)
			{
				// End:0x4CE
				case 0:
				// End:0x4F2
				case 1:
					infNodeItem.t_strText = __NFUN_112__(GetSystemString(365), " : ");
					// End:0x51A
					break;
				// End:0x517
				case 2:
					infNodeItem.t_strText = __NFUN_112__(GetSystemString(1437), " : ");
					// End:0x51A
					break;
				// End:0xFFFF
				default:
					break;
			}
			infNodeItem.bLineBreak = true;
			infNodeItem.nOffSetX = 77;
			infNodeItem.nOffSetY = -14;
			infNodeItem.t_color.R = 163;
			infNodeItem.t_color.G = 163;
			infNodeItem.t_color.B = 163;
			infNodeItem.t_color.A = byte(255);
			InsertNodeItem(strRetName, infNodeItem);
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 1;
			infNodeItem.t_strText = string(iSPConsume);
			infNodeItem.nOffSetX = 0;
			infNodeItem.nOffSetY = -14;
			infNodeItem.t_color.R = 176;
			infNodeItem.t_color.G = 155;
			infNodeItem.t_color.B = 121;
			infNodeItem.t_color.A = byte(255);
			InsertNodeItem(strRetName, infNodeItem);
			// End:0x705
			break;
		// End:0x702
		case 3:
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 1;
			infNodeItem.bLineBreak = true;
			infNodeItem.t_bDrawOneLine = true;
			infNodeItem.t_strText = strEnchantName;
			infNodeItem.nOffSetX = 37;
			infNodeItem.nOffSetY = -14;
			infNodeItem.t_color.R = 176;
			infNodeItem.t_color.G = 155;
			infNodeItem.t_color.B = 121;
			infNodeItem.t_color.A = byte(255);
			InsertNodeItem(strRetName, infNodeItem);
			// End:0x705
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function InsertNodeItem(string strNodeName, XMLTreeNodeItemInfo infNodeItemName)
{
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("SkillTrainListWnd.SkillTrainListTree", strNodeName, infNodeItemName);
	return;
}
