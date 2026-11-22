class RecipeBookWnd extends UICommonAPI;

var int m_ItemCount;
var array<int> m_arrItem;
var int m_BookType;
var int m_ItemMaxCount_Dwarf;
var int m_ItemMaxCount_Normal;
var int m_DeleteItemID;

function OnLoad()
{
	RegisterEvent(820);
	RegisterEvent(830);
	RegisterEvent(2070);
	RegisterEvent(1710);
	return;
}

function OnEvent(int Event_ID, string param)
{
	local Rect rectWnd;
	local int RecipeAddBookItem;

	// End:0x11B
	if(__NFUN_154__(Event_ID, 820))
	{
		Clear();
		rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect("RecipeManufactureWnd");
		Class'NWindow.UIAPI_WINDOW'.static.MoveTo("RecipeBookWnd", rectWnd.nX, rectWnd.nY);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("RecipeBookWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("RecipeBookWnd");
		ParseInt(param, "Type", m_BookType);
		// End:0xF5
		if(__NFUN_154__(m_BookType, 1))
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle("RecipeBookWnd", 1214);			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle("RecipeBookWnd", 1215);
		}		
	}
	else
	{
		// End:0x152
		if(__NFUN_154__(Event_ID, 830))
		{
			ParseInt(param, "RecipeID", RecipeAddBookItem);
			AddRecipeBookItem(RecipeAddBookItem);			
		}
		else
		{
			// End:0x1A6
			if(__NFUN_154__(Event_ID, 2070))
			{
				ParseInt(param, "recipe", m_ItemMaxCount_Normal);
				ParseInt(param, "dwarvenRecipe", m_ItemMaxCount_Dwarf);
				SetItemCount(m_ItemCount);				
			}
			else
			{
				// End:0x1D2
				if(__NFUN_154__(Event_ID, 1710))
				{
					// End:0x1D2
					if(DialogIsMine())
					{
						Class'NWindow.RecipeAPI'.static.RequestRecipeItemDelete(m_DeleteItemID);
					}
				}
			}
		}
	}
	return;
}

function OnDBClickItem(string strID, int Index)
{
	// End:0x41
	if(__NFUN_130__(__NFUN_122__(strID, "RecipeItem"), __NFUN_151__(m_ItemCount, Index)))
	{
		Class'NWindow.RecipeAPI'.static.RequestRecipeItemMakeInfo(m_arrItem[Index]);
	}
	return;
}

function OnDropItem(string strID, ItemInfo infItem, int X, int Y)
{
	// End:0x1F
	if(__NFUN_122__(strID, "btnTrash"))
	{
		DeleteItem(infItem);
	}
	return;
}

function OnClickButton(string strID)
{
	local ItemInfo infItem;

	switch(strID)
	{
		// End:0x53
		case "btnTrash":
			// End:0x50
			if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedItem("RecipeBookWnd.RecipeItem", infItem))
			{
				DeleteItem(infItem);
			}
			// End:0x56
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function Clear()
{
	SetItemCount(0);
	m_arrItem.Remove(0, m_arrItem.Length);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("RecipeBookWnd.RecipeItem");
	return;
}

function AddRecipeBookItem(int RecipeID)
{
	local ItemInfo infItem;
	local int ProductID, Index;

	ProductID = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeProductID(RecipeID);
	Index = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeIndex(RecipeID);
	infItem.ClassID = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeClassID(RecipeID);
	infItem.Level = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeLevel(RecipeID);
	infItem.ServerID = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeIndex(RecipeID);
	infItem.ItemSubType = int(5);
	infItem.Name = Class'NWindow.UIDATA_ITEM'.static.GetItemName(infItem.ClassID);
	infItem.Description = Class'NWindow.UIDATA_ITEM'.static.GetItemDescription(infItem.ClassID);
	infItem.Weight = Class'NWindow.UIDATA_ITEM'.static.GetItemWeight(infItem.ClassID);
	infItem.IconName = Class'NWindow.UIDATA_ITEM'.static.GetItemTextureName(ProductID);
	infItem.CrystalType = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeCrystalType(RecipeID);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("RecipeBookWnd.RecipeItem", infItem);
	m_arrItem.Insert(m_arrItem.Length, 1);
	m_arrItem[__NFUN_147__(m_arrItem.Length, 1)] = Index;
	__NFUN_165__(m_ItemCount);
	SetItemCount(m_ItemCount);
	return;
}

function SetItemCount(int MaxCount)
{
	local int nTmp;

	m_ItemCount = MaxCount;
	// End:0x24
	if(__NFUN_154__(m_BookType, 1))
	{
		nTmp = m_ItemMaxCount_Normal;		
	}
	else
	{
		nTmp = m_ItemMaxCount_Dwarf;
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeBookWnd.txtCount", __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", string(m_ItemCount)), "/"), string(nTmp)), ")"));
	return;
}

function DeleteItem(ItemInfo infItem)
{
	local string strMsg;

	strMsg = MakeFullSystemMsg(GetSystemMessage(74), infItem.Name, "");
	m_DeleteItemID = infItem.ServerID;
	DialogShow(4, strMsg);
	return;
}
