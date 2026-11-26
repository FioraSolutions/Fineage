class RecipeBuyManufactureWnd extends UIScript;

const RECIPEWND_MAX_MP_WIDTH = 165.0f;

var int m_merchantID;
var int m_RecipeID;
var int m_SuccessRate;
var int m_Adena;
var int m_MaxMP;

function OnLoad()
{
	RegisterEvent(800);
	RegisterEvent(210);
	RegisterEvent(2600);
	RegisterEvent(2610);
	return;
}

function OnEvent(int Event_ID, string param)
{
	local Rect rectWnd;
	local int ServerID, MPValue, MerchantID, RecipeID, currentMP, maxMP,
		MakingResult, Adena;

	// End:0x1AD
	if(__NFUN_154__(Event_ID, 800))
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RecipeBuyListWnd");
		Clear();
		rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect("RecipeBuyListWnd");
		Class'NWindow.UIAPI_WINDOW'.static.MoveTo("RecipeBuyManufactureWnd", rectWnd.nX, rectWnd.nY);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("RecipeBuyManufactureWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("RecipeBuyManufactureWnd");
		ParseInt(param, "MerchantID", MerchantID);
		ParseInt(param, "RecipeID", RecipeID);
		ParseInt(param, "CurrentMP", currentMP);
		ParseInt(param, "MaxMP", maxMP);
		ParseInt(param, "MakingResult", MakingResult);
		ParseInt(param, "Adena", Adena);
		ReceiveRecipeShopSellList(MerchantID, RecipeID, currentMP, maxMP, MakingResult, Adena);		
	}
	else
	{
		// End:0x218
		if(__NFUN_154__(Event_ID, 210))
		{
			ParseInt(param, "ServerID", ServerID);
			ParseInt(param, "CurrentMP", MPValue);
			// End:0x215
			if(__NFUN_130__(__NFUN_154__(m_merchantID, ServerID), __NFUN_151__(m_merchantID, 0)))
			{
				SetMPBar(MPValue);
			}			
		}
		else
		{
			// End:0x243
			if(__NFUN_132__(__NFUN_154__(Event_ID, 2600), __NFUN_154__(Event_ID, 2610)))
			{
				HandleInventoryItem(param);
			}
		}
	}
	return;
}

function OnClickButton(string strID)
{
	local string param;

	switch(strID)
	{
		// End:0x1D
		case "btnClose":
			CloseWindow();
			// End:0x11F
			break;
		// End:0x46
		case "btnPrev":
			Class'NWindow.RecipeAPI'.static.RequestRecipeShopSellList(m_merchantID);
			CloseWindow();
			// End:0x11F
			break;
		// End:0xE8
		case "btnRecipeTree":
			// End:0x9A
			if(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("RecipeTreeWnd"))
			{
				Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RecipeTreeWnd");				
			}
			else
			{
				ParamAdd(param, "RecipeID", string(m_RecipeID));
				ParamAdd(param, "SuccessRate", string(m_SuccessRate));
				ExecuteEvent(810, param);
			}
			// End:0x11F
			break;
		// End:0x11C
		case "btnManufacture":
			Class'NWindow.RecipeAPI'.static.RequestRecipeShopMakeDo(m_merchantID, m_RecipeID, m_Adena);
			// End:0x11F
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function CloseWindow()
{
	Clear();
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RecipeBuyManufactureWnd");
	PlayConsoleSound(6);
	return;
}

function Clear()
{
	m_merchantID = 0;
	m_RecipeID = 0;
	m_SuccessRate = 0;
	m_Adena = 0;
	m_MaxMP = 0;
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("RecipeBuyManufactureWnd.ItemWnd");
	return;
}

function ReceiveRecipeShopSellList(int MerchantID, int RecipeID, int currentMP, int maxMP, int MakingResult, int Adena)
{
	local int i;
	local string strTmp;
	local int nTmp, ProductID, ProductNum;
	local string ItemName;
	local ParamStack param;
	local ItemInfo infItem;

	m_merchantID = MerchantID;
	m_RecipeID = RecipeID;
	m_SuccessRate = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeSuccessRate(RecipeID);
	m_Adena = Adena;
	m_MaxMP = maxMP;
	strTmp = __NFUN_112__(__NFUN_112__(GetSystemString(663), " - "), Class'NWindow.UIDATA_USER'.static.GetUserName(MerchantID));
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("RecipeBuyManufactureWnd", strTmp);
	ProductID = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeProductID(RecipeID);
	strTmp = Class'NWindow.UIDATA_ITEM'.static.GetItemTextureName(ProductID);
	Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("RecipeBuyManufactureWnd.texItem", strTmp);
	ItemName = MakeFullItemName(ProductID);
	nTmp = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeCrystalType(RecipeID);
	strTmp = GetItemGradeString(nTmp);
	// End:0x168
	if(__NFUN_151__(__NFUN_125__(strTmp), 0))
	{
		strTmp = __NFUN_112__(__NFUN_112__("`", strTmp), "`");
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeBuyManufactureWnd.txtName", __NFUN_112__(__NFUN_112__(ItemName, " "), strTmp));
	nTmp = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeMpConsume(RecipeID);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeBuyManufactureWnd.txtMPConsume", __NFUN_112__("", string(nTmp)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeBuyManufactureWnd.txtSuccessRate", __NFUN_112__(string(m_SuccessRate), "%"));
	ProductNum = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeProductNum(RecipeID);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeBuyManufactureWnd.txtResultValue", __NFUN_112__("", string(ProductNum)));
	SetMPBar(currentMP);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeBuyManufactureWnd.txtCountValue", __NFUN_112__("", string(GetInventoryItemCount(ProductID))));
	strTmp = "";
	// End:0x328
	if(__NFUN_154__(MakingResult, 0))
	{
		strTmp = MakeFullSystemMsg(GetSystemMessage(960), ItemName, "");		
	}
	else
	{
		// End:0x35A
		if(__NFUN_154__(MakingResult, 1))
		{
			strTmp = MakeFullSystemMsg(GetSystemMessage(959), ItemName, __NFUN_112__("", string(ProductNum)));
		}
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeBuyManufactureWnd.txtMsg", strTmp);
	param = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeMaterialItem(RecipeID);
	nTmp = param.GetInt();
	i = 0;
	J0x3C4:

	// End:0x527 [Loop If]
	if(__NFUN_150__(i, nTmp))
	{
		infItem.ClassID = param.GetInt();
		infItem.Reserved = param.GetInt();
		infItem.Name = Class'NWindow.UIDATA_ITEM'.static.GetItemName(infItem.ClassID);
		infItem.AdditionalName = Class'NWindow.UIDATA_ITEM'.static.GetItemAdditionalName(infItem.ClassID);
		infItem.IconName = Class'NWindow.UIDATA_ITEM'.static.GetItemTextureName(infItem.ClassID);
		infItem.Description = Class'NWindow.UIDATA_ITEM'.static.GetItemDescription(infItem.ClassID);
		infItem.ItemNum = GetInventoryItemCount(infItem.ClassID);
		// End:0x4DB
		if(__NFUN_151__(infItem.Reserved, infItem.ItemNum))
		{
			infItem.bDisabled = true;			
		}
		else
		{
			infItem.bDisabled = false;
		}
		Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("RecipeBuyManufactureWnd.ItemWnd", infItem);
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x3C4;
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
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("RecipeBuyManufactureWnd.texMPBar", nMPWidth, 12);
	return;
}

function HandleInventoryItem(string param)
{
	local int ClassID, idx;
	local ItemInfo infItem;

	// End:0x12B
	if(ParseInt(param, "classID", ClassID))
	{
		idx = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("RecipeBuyManufactureWnd.ItemWnd", ClassID);
		// End:0x12B
		if(__NFUN_151__(idx, -1))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("RecipeBuyManufactureWnd.ItemWnd", idx, infItem);
			infItem.ItemNum = GetInventoryItemCount(infItem.ClassID);
			// End:0xE4
			if(__NFUN_151__(infItem.Reserved, infItem.ItemNum))
			{
				infItem.bDisabled = true;				
			}
			else
			{
				infItem.bDisabled = false;
			}
			Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("RecipeBuyManufactureWnd.ItemWnd", idx, infItem);
		}
	}
	return;
}
