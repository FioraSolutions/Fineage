class SkillTrainInfoWnd extends UICommonAPI;

const NORMAL_SKILL = 0;
const FISHING_SKILL = 1;
const CLAN_SKILL = 2;
const ENCHANT_SKILL = 3;
const OFFSET_X_ICON_TEXTURE = 0;
const OFFSET_Y_ICON_TEXTURE = 4;
const OFFSET_Y_SECONDLINE = -14;
const OFFSET_Y_MPCONSUME = 3;
const OFFSET_Y_CASTRANGE = 0;
const OFFSET_Y_SP = 120;

var int m_iType;
var int m_iID;
var int m_iLevel;

function OnLoad()
{
	RegisterEvent(2040);
	RegisterEvent(2050);
	RegisterEvent(2060);
	return;
}

function OnClickButton(string strBtnID)
{
	switch(strBtnID)
	{
		// End:0x1D
		case "btnLearn":
			OnLearn();
			// End:0x67
			break;
		// End:0x64
		case "btnGoBackList":
			HideWindow("SkillTrainInfoWnd");
			ShowWindowWithFocus("SkillTrainListWnd");
			// End:0x67
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnLearn()
{
	switch(m_iType)
	{
		// End:0x0B
		case 0:
		// End:0x0F
		case 1:
		// End:0x2C
		case 2:
			RequestAcquireSkill(m_iID, m_iLevel, m_iType);
			// End:0x47
			break;
		// End:0x44
		case 3:
			RequestExEnchantSkill(m_iID, m_iLevel);
			// End:0x47
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnEvent(int Event_ID, string param)
{
	local int iType;
	local string strIconName, strName;
	local int iID, iLevel, iSPConsume;
	local INT64 iEXPConsume;
	local string strDescription, strOperateType;
	local int iMPConsume, iCastRange, iNumOfItem;
	local string strEnchantName, strEnchantDesc;
	local int iPercent;

	switch(Event_ID)
	{
		// End:0x204
		case 2040:
			ParseInt(param, "Type", iType);
			ParseString(param, "strIconName", strIconName);
			ParseString(param, "strName", strName);
			ParseInt(param, "iID", iID);
			ParseInt(param, "iLevel", iLevel);
			ParseString(param, "strOperateType", strOperateType);
			ParseInt(param, "iMPConsume", iMPConsume);
			ParseInt(param, "iCastRange", iCastRange);
			ParseInt(param, "iSPConsume", iSPConsume);
			ParseString(param, "strDescription", strDescription);
			ParseInt64(param, "iEXPConsume", iEXPConsume);
			ParseString(param, "strEnchantName", strEnchantName);
			ParseString(param, "strEnchantDesc", strEnchantDesc);
			ParseInt(param, "iPercent", iPercent);
			m_iType = iType;
			m_iID = iID;
			m_iLevel = iLevel;
			ShowSkillTrainInfoWnd();
			AddSkillTrainInfo(strIconName, strName, iID, iLevel, strOperateType, iMPConsume, iCastRange, strDescription, iSPConsume, iEXPConsume, strEnchantName, strEnchantDesc, iPercent);
			// End:0x2BF
			break;
		// End:0x27C
		case 2060:
			ParseString(param, "strIconName", strIconName);
			ParseString(param, "strName", strName);
			ParseInt(param, "iNumOfItem", iNumOfItem);
			AddSkillTrainInfoExtend(strIconName, strName, iNumOfItem);
			ShowNeedItems();
			// End:0x2BF
			break;
		// End:0x2BC
		case 2050:
			// End:0x2B9
			if(IsShowWindow("SkillTrainInfoWnd"))
			{
				HideWindow("SkillTrainInfoWnd");
			}
			// End:0x2BF
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ShowSkillTrainInfoWnd()
{
	local int iWindowTitle, iSPIdx;
	local UserInfo infoPlayer;
	local int iPlayerSP;
	local INT64 iPlayerEXP, iLevelEXP, iResultEXP;
	local string strEXP;

	GetPlayerInfo(infoPlayer);
	switch(m_iType)
	{
		// End:0x16
		case 0:
		// End:0x1A
		case 1:
		// End:0x55
		case 3:
			iWindowTitle = 477;
			iSPIdx = 92;
			iPlayerSP = infoPlayer.nSP;
			iPlayerEXP = infoPlayer.nCurExp;
			// End:0x8C
			break;
		// End:0x89
		case 2:
			iWindowTitle = 1436;
			iSPIdx = 1372;
			iPlayerSP = GetClanNameValue(infoPlayer.nClanID);
			// End:0x8C
			break;
		// End:0xFFFF
		default:
			break;
	}
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle("SkillTrainInfoWnd", iWindowTitle);
	// End:0x16B
	if(__NFUN_154__(m_iType, 3))
	{
		SetBackTex("L2UI_CH3.SkillTrainWnd.skillenchant_back");
		iLevelEXP = GetExpByPlayerLevel(infoPlayer.nLevel);
		iResultEXP = Int64SubtractBfromA(iPlayerEXP, iLevelEXP);
		strEXP = Int64ToString(iResultEXP);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndEnchant.txtEXP", strEXP);		
	}
	else
	{
		SetBackTex("L2UI_CH3.SkillTrainWnd.SkillTrain2");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndNormal.txtSPString", GetSystemString(iSPIdx));
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("SkillTrainInfoWnd.txtSP", iPlayerSP);
	ShowWindowWithFocus("SkillTrainInfoWnd");
	return;
}

function AddSkillTrainInfo(string strIconName, string strName, int iID, int iLevel, string strOperateType, int iMPConsume, int iCastRange, string strDescription, int iSPConsume, INT64 iEXPConsume, string strEnchantName, string strEnchantDesc, int iPercent)
{
	local string strEXPConsume;

	Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("SkillTrainInfoWnd.texIcon", strIconName);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.txtName", strName);
	// End:0x1D2
	if(__NFUN_154__(m_iType, 3))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndEnchant.txtEnchantName", strEnchantName);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndEnchant.txtDescription", strEnchantDesc);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("SkillTrainInfoWnd.SubWndEnchant.txtProbabilityOfSuccess", iPercent);
		strEXPConsume = Int64ToString(iEXPConsume);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndEnchant.txtNeedEXP", strEXPConsume);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("SkillTrainInfoWnd.SubWndEnchant.txtNeedSP", iSPConsume);		
	}
	else
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("SkillTrainInfoWnd.txtLevel", iLevel);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.txtOperateType", strOperateType);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("SkillTrainInfoWnd.txtMP", iMPConsume);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndNormal.txtDescription", strDescription);
		switch(m_iType)
		{
			// End:0x2B3
			case 0:
			// End:0x304
			case 1:
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndNormal.txtNeedSPString", GetSystemString(365));
				// End:0x359
				break;
			// End:0x356
			case 2:
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndNormal.txtNeedSPString", GetSystemString(1437));
				// End:0x359
				break;
			// End:0xFFFF
			default:
				break;
		}
		Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("SkillTrainInfoWnd.SubWndNormal.txtNeedSP", iSPConsume);
	}
	// End:0x431
	if(__NFUN_153__(iCastRange, 0))
	{
		ShowWindow("SkillTrainInfoWnd.txtCastRangeString");
		ShowWindow("SkillTrainInfoWnd.txtColoneCastRange");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("SkillTrainInfoWnd.txtCastRange", iCastRange);		
	}
	else
	{
		HideWindow("SkillTrainInfoWnd.txtCastRangeString");
		HideWindow("SkillTrainInfoWnd.txtColoneCastRange");
		HideWindow("SkillTrainInfoWnd.txtCastRange");
	}
	return;
}

function AddSkillTrainInfoExtend(string strIconName, string strName, int iNumOfItem)
{
	// End:0xA9
	if(__NFUN_154__(m_iType, 3))
	{
		Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("SkillTrainInfoWnd.SubWndEnchant.texNeedItemIcon", strIconName);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndEnchant.txtNeedItemName", __NFUN_112__(__NFUN_112__(strName, " X "), string(iNumOfItem)));		
	}
	else
	{
		Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("SkillTrainInfoWnd.SubWndNormal.texNeedItemIcon", strIconName);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SkillTrainInfoWnd.SubWndNormal.txtNeedItemName", __NFUN_112__(__NFUN_112__(strName, " X "), string(iNumOfItem)));
	}
	return;
}

function OnShow()
{
	switch(m_iType)
	{
		// End:0x0B
		case 0:
		// End:0x0F
		case 1:
		// End:0xD0
		case 2:
			HideWindow("SkillTrainInfoWnd.SubWndEnchant");
			ShowWindow("SkillTrainInfoWnd.SubWndNormal");
			HideWindow("SkillTrainInfoWnd.SubWndNormal.texNeedItemIcon");
			HideWindow("SkillTrainInfoWnd.SubWndNormal.txtNeedItemName");
			// End:0x196
			break;
		// End:0x193
		case 3:
			HideWindow("SkillTrainInfoWnd.SubWndNormal");
			ShowWindow("SkillTrainInfoWnd.SubWndEnchant");
			HideWindow("SkillTrainInfoWnd.SubWndEnchant.texNeedItemIcon");
			HideWindow("SkillTrainInfoWnd.SubWndEnchant.txtNeedItemName");
			// End:0x196
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ShowNeedItems()
{
	// End:0x7D
	if(__NFUN_154__(m_iType, 3))
	{
		ShowWindow("SkillTrainInfoWnd.SubWndEnchant.texNeedItemIcon");
		ShowWindow("SkillTrainInfoWnd.SubWndEnchant.txtNeedItemName");		
	}
	else
	{
		ShowWindow("SkillTrainInfoWnd.SubWndNormal.texNeedItemIcon");
		ShowWindow("SkillTrainInfoWnd.SubWndNormal.txtNeedItemName");
	}
	return;
}

function SetBackTex(string strFile)
{
	Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("SkillTrainInfoWnd.texBack", strFile);
	return;
}
