class RecipeBuyListWnd extends UIScript;

const RECIPEWND_MAX_MP_WIDTH = 165.0f;

var int m_merchantID;
var int m_MaxMP;

function OnLoad()
{
	RegisterEvent(780);
	RegisterEvent(790);
	RegisterEvent(210);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeBuyListWnd.txtAdena", "0");
	return;
}

function OnEvent(int Event_ID, string param)
{
	local Rect rectWnd;
	local int ServerID, MPValue, currentMP, maxMP, Adena, RecipeID,
		CanbeMade, MakingFee;

	// End:0x13A
	if(__NFUN_154__(Event_ID, 780))
	{
		Clear();
		rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect("RecipeBuyManufactureWnd");
		Class'NWindow.UIAPI_WINDOW'.static.MoveTo("RecipeBuyListWnd", rectWnd.nX, rectWnd.nY);
		ParseInt(param, "ServerID", ServerID);
		ParseInt(param, "CurrentMP", currentMP);
		ParseInt(param, "MaxMP", maxMP);
		ParseInt(param, "Adena", Adena);
		ReceiveRecipeShopSellList(ServerID, currentMP, maxMP, Adena);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("RecipeBuyListWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("RecipeBuyListWnd");		
	}
	else
	{
		// End:0x1B1
		if(__NFUN_154__(Event_ID, 790))
		{
			ParseInt(param, "RecipeID", RecipeID);
			ParseInt(param, "CanbeMade", CanbeMade);
			ParseInt(param, "MakingFee", MakingFee);
			AddRecipeShopSellItem(RecipeID, CanbeMade, MakingFee);			
		}
		else
		{
			// End:0x219
			if(__NFUN_154__(Event_ID, 210))
			{
				ParseInt(param, "ServerID", ServerID);
				ParseInt(param, "CurrentMP", MPValue);
				// End:0x219
				if(__NFUN_130__(__NFUN_154__(m_merchantID, ServerID), __NFUN_151__(m_merchantID, 0)))
				{
					SetMPBar(MPValue);
				}
			}
		}
	}
	return;
}

function OnClickButton(string strID)
{
	local string strRecipeID;

	switch(strID)
	{
		// End:0x1D
		case "btnClose":
			CloseWindow();
			// End:0x4D
			break;
		// End:0xFFFF
		default:
			strRecipeID = __NFUN_127__(strID, 5);
			Class'NWindow.RecipeAPI'.static.RequestRecipeShopMakeInfo(m_merchantID, int(strRecipeID));
			// End:0x4D
			break;
			break;
	}
	return;
}

function CloseWindow()
{
	Clear();
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RecipeBuyListWnd");
	PlayConsoleSound(6);
	return;
}

function Clear()
{
	m_merchantID = 0;
	m_MaxMP = 0;
	Class'NWindow.UIAPI_TREECTRL'.static.Clear("RecipeBuyListWnd.MainTree");
	return;
}

function ReceiveRecipeShopSellList(int ServerID, int currentMP, int maxMP, int Adena)
{
	local string strTmp;
	local XMLTreeNodeInfo infNode;

	m_merchantID = ServerID;
	m_MaxMP = maxMP;
	strTmp = __NFUN_112__(__NFUN_112__(GetSystemString(663), " - "), Class'NWindow.UIDATA_USER'.static.GetUserName(ServerID));
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("RecipeBuyListWnd", strTmp);
	SetMPBar(currentMP);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeBuyListWnd.txtAdena", MakeCostString(__NFUN_112__("", string(Adena))));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("RecipeBuyListWnd.txtAdena", ConvertNumToText(__NFUN_112__("", string(Adena))));
	infNode.strName = "root";
	infNode.nOffSetX = 7;
	infNode.nOffSetY = 7;
	strTmp = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("RecipeBuyListWnd.MainTree", "", infNode);
	// End:0x195
	if(__NFUN_150__(__NFUN_125__(strTmp), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert root node. Name: ", infNode.strName));
		return;
	}
	return;
}

function SetMPBar(int currentMP)
{
	local int nTmp, nMPWidth;

	nTmp = __NFUN_144__(165, currentMP);
	nMPWidth = __NFUN_145__(nTmp, m_MaxMP);
	// End:0x3A
	if(__NFUN_177__(float(nMPWidth), 165.0000000))
	{
		nMPWidth = 165;
	}
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("RecipeBuyListWnd.texMPBar", nMPWidth, 12);
	return;
}

function AddRecipeShopSellItem(int RecipeID, int CanbeMade, int MakingFee)
{
	local string strTmp;
	local XMLTreeNodeInfo infNode;
	local XMLTreeNodeItemInfo infNodeItem;
	local XMLTreeNodeInfo infNodeClear;
	local XMLTreeNodeItemInfo infNodeItemClear;
	local string strRetName;
	local int ProductID;
	local string AdenaComma, strName, strDescription;

	ProductID = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeProductID(RecipeID);
	strName = Class'NWindow.UIDATA_ITEM'.static.GetItemName(ProductID);
	strDescription = Class'NWindow.UIDATA_ITEM'.static.GetItemDescription(ProductID);
	infNode = infNodeClear;
	infNode.strName = __NFUN_112__("", string(RecipeID));
	infNode.bShowButton = 0;
	infNode.ToolTip = SetTooltip(strName, strDescription, MakingFee);
	infNode.bFollowCursor = true;
	infNode.nTexExpandedOffSetX = -7;
	infNode.nTexExpandedOffSetY = -3;
	infNode.nTexExpandedHeight = 46;
	infNode.nTexExpandedRightWidth = 0;
	infNode.nTexExpandedLeftUWidth = 32;
	infNode.nTexExpandedLeftUHeight = 40;
	infNode.strTexExpandedLeft = "L2UI_CH3.etc.IconSelect2";
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("RecipeBuyListWnd.MainTree", "root", infNode);
	// End:0x19A
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		__NFUN_231__(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	infNode.ToolTip.DrawList.Remove(0, infNode.ToolTip.DrawList.Length);
	strTmp = Class'NWindow.UIDATA_ITEM'.static.GetItemTextureName(ProductID);
	// End:0x1FE
	if(__NFUN_150__(__NFUN_125__(strTmp), 1))
	{
		strTmp = "Default.BlackTexture";
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 2;
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = 4;
	infNodeItem.u_nTextureWidth = 32;
	infNodeItem.u_nTextureHeight = 32;
	infNodeItem.u_strTexture = strTmp;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = strName;
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 10;
	infNodeItem.nOffSetY = 0;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSystemString(641);
	infNodeItem.bLineBreak = true;
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 42;
	infNodeItem.nOffSetY = -22;
	infNodeItem.t_color.R = 168;
	infNodeItem.t_color.G = 168;
	infNodeItem.t_color.B = 168;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = " : ";
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = -22;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	AdenaComma = MakeCostString(__NFUN_112__("", string(MakingFee)));
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = AdenaComma;
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = -22;
	infNodeItem.t_color = GetNumericColor(AdenaComma);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSystemString(469);
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 5;
	infNodeItem.nOffSetY = -22;
	infNodeItem.t_color.R = byte(255);
	infNodeItem.t_color.G = byte(255);
	infNodeItem.t_color.B = 0;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = GetSystemString(642);
	infNodeItem.bLineBreak = true;
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 42;
	infNodeItem.nOffSetY = -8;
	infNodeItem.t_color.R = 168;
	infNodeItem.t_color.G = 168;
	infNodeItem.t_color.B = 168;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = " : ";
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = -8;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = __NFUN_112__(string(Class'NWindow.UIDATA_RECIPE'.static.GetRecipeSuccessRate(RecipeID)), "%");
	infNodeItem.t_bDrawOneLine = true;
	infNodeItem.nOffSetX = 0;
	infNodeItem.nOffSetY = -8;
	infNodeItem.t_color.R = 176;
	infNodeItem.t_color.G = 155;
	infNodeItem.t_color.B = 121;
	infNodeItem.t_color.A = byte(255);
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 0;
	infNodeItem.bStopMouseFocus = true;
	infNodeItem.b_nHeight = 10;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("RecipeBuyListWnd.MainTree", strRetName, infNodeItem);
	return;
}

function CustomTooltip SetTooltip(string Name, string Description, int MakingFee)
{
	local CustomTooltip ToolTip;
	local DrawItemInfo Info, infoClear;
	local string AdenaComma;

	AdenaComma = MakeCostString(__NFUN_112__("", string(MakingFee)));
	ToolTip.DrawList.Length = 4;
	Info = infoClear;
	Info.eType = 1;
	Info.t_bDrawOneLine = true;
	Info.t_strText = Name;
	ToolTip.DrawList[0] = Info;
	Info = infoClear;
	Info.eType = 1;
	Info.nOffSetY = 6;
	Info.bLineBreak = true;
	Info.t_bDrawOneLine = true;
	Info.t_color.R = 163;
	Info.t_color.G = 163;
	Info.t_color.B = 163;
	Info.t_color.A = byte(255);
	Info.t_strText = __NFUN_112__(GetSystemString(322), " : ");
	ToolTip.DrawList[1] = Info;
	Info = infoClear;
	Info.eType = 1;
	Info.nOffSetY = 6;
	Info.t_bDrawOneLine = true;
	Info.t_color = GetNumericColor(AdenaComma);
	Info.t_strText = __NFUN_112__(__NFUN_112__(AdenaComma, " "), GetSystemString(469));
	ToolTip.DrawList[2] = Info;
	Info = infoClear;
	Info.eType = 1;
	Info.nOffSetY = 6;
	Info.bLineBreak = true;
	Info.t_bDrawOneLine = true;
	Info.t_color = GetNumericColor(AdenaComma);
	Info.t_strText = __NFUN_112__(__NFUN_112__("(", ConvertNumToText(string(MakingFee))), ")");
	ToolTip.DrawList[3] = Info;
	return ToolTip;
	return;
}
