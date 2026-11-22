class RecipeShopWnd extends UICommonAPI;

const RECIPESHOP_MAX_ITEM_SELL = 20;

var int m_BookItemCount;
var int m_ShopItemCount;
var array<int> m_arrBookItem;
var array<int> m_arrShopItem;
var int m_BookType;
var ItemInfo m_HandleItem;

function OnLoad()
{
	RegisterEvent(850);
	RegisterEvent(860);
	RegisterEvent(870);
	RegisterEvent(1710);
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x2A
		case "btnEnd":
			Class'NWindow.RecipeAPI'.static.RequestRecipeShopManageQuit();
			CloseWindow();
			// End:0xBE
			break;
		// End:0x6F
		case "btnMsg":
			DialogSetEditBoxMaxLength(29);
			DialogShow(2, GetSystemMessage(334));
			DialogSetID(0);
			DialogSetString(Class'NWindow.UIDATA_PLAYER'.static.GetRecipeShopMsg());
			// End:0xBE
			break;
		// End:0x8B
		case "btnStart":
			StartRecipeShop();
			CloseWindow();
			// End:0xBE
			break;
		// End:0xA2
		case "btnMoveUp":
			HandleMoveUpItem();
			// End:0xBE
			break;
		// End:0xBB
		case "btnMoveDown":
			HandleMoveDownItem();
			// End:0xBE
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnEvent(int Event_ID, string param)
{
	local string strPrice;
	local int RecipeID, CanbeMade, MakingFee, Price;
	local InventoryWnd InventoryWnd;

	InventoryWnd = InventoryWnd(GetScript("InventoryWnd"));
	// End:0xEC
	if(__NFUN_154__(Event_ID, 850))
	{
		Clear();
		InventoryWnd.LoadItemOrder();
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("RecipeShopWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("RecipeShopWnd");
		ParseInt(param, "Type", m_BookType);
		// End:0xC6
		if(__NFUN_154__(m_BookType, 1))
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle("RecipeShopWnd", 1212);			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle("RecipeShopWnd", 1213);
		}		
	}
	else
	{
		// End:0x123
		if(__NFUN_154__(Event_ID, 860))
		{
			ParseInt(param, "RecipeID", RecipeID);
			AddRecipeBookItem(RecipeID);			
		}
		else
		{
			// End:0x19A
			if(__NFUN_154__(Event_ID, 870))
			{
				ParseInt(param, "RecipeID", RecipeID);
				ParseInt(param, "CanbeMade", CanbeMade);
				ParseInt(param, "MakingFee", MakingFee);
				AddRecipeShopItem(RecipeID, CanbeMade, MakingFee);				
			}
			else
			{
				// End:0x256
				if(__NFUN_154__(Event_ID, 1710))
				{
					// End:0x256
					if(DialogIsMine())
					{
						// End:0x1D6
						if(__NFUN_154__(DialogGetID(), 0))
						{
							Class'NWindow.RecipeAPI'.static.RequestRecipeShopMessageSet(DialogGetString());							
						}
						else
						{
							// End:0x256
							if(__NFUN_154__(DialogGetID(), 1))
							{
								strPrice = DialogGetString();
								// End:0x250
								if(__NFUN_151__(__NFUN_125__(strPrice), 0))
								{
									Price = int(strPrice);
									// End:0x235
									if(__NFUN_153__(Price, 2000000000))
									{
										DialogSetID(2);
										DialogShow(4, GetSystemMessage(1369));										
									}
									else
									{
										m_HandleItem.Price = Price;
										UpdateShopItem(m_HandleItem);
									}
								}
								ClearHandleItem();
							}
						}
					}
				}
			}
		}
	}
	return;
}

function OnSendPacketWhenHiding()
{
	Class'NWindow.RecipeAPI'.static.RequestRecipeShopManageQuit();
	Clear();
	return;
}

function CloseWindow()
{
	Clear();
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RecipeShopWnd");
	PlayConsoleSound(6);
	return;
}

function OnDBClickItem(string strID, int Index)
{
	local int Max, i;
	local ItemInfo infItem, DeleteItem;

	ClearHandleItem();
	// End:0x151
	if(__NFUN_130__(__NFUN_122__(strID, "BookItemWnd"), __NFUN_151__(m_BookItemCount, Index)))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("RecipeShopWnd.BookItemWnd", Index, infItem);
		Max = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("RecipeShopWnd.ShopItemWnd");
		i = 0;
		J0x99:

		// End:0x10F [Loop If]
		if(__NFUN_150__(i, Max))
		{
			// End:0x105
			if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("RecipeShopWnd.ShopItemWnd", i, DeleteItem))
			{
				// End:0x105
				if(__NFUN_154__(DeleteItem.ClassID, infItem.ClassID))
				{
					DeleteShopItem(infItem);
					return;
				}
			}
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x99;
		}
		Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("RecipeShopWnd.BookItemWnd", Index, infItem);
		ShowShopItemAddDialog(infItem);		
	}
	else
	{
		// End:0x1B8
		if(__NFUN_130__(__NFUN_122__(strID, "ShopItemWnd"), __NFUN_151__(m_ShopItemCount, Index)))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("RecipeShopWnd.ShopItemWnd", Index, infItem);
			DeleteShopItem(infItem);
		}
	}
	return;
}

function OnDropItem(string strID, ItemInfo infItem, int X, int Y)
{
	// End:0x41
	if(__NFUN_122__(strID, "BookItemWnd"))
	{
		// End:0x3E
		if(__NFUN_122__(infItem.DragSrcName, "ShopItemWnd"))
		{
			DeleteShopItem(infItem);
		}		
	}
	else
	{
		// End:0x7F
		if(__NFUN_122__(strID, "ShopItemWnd"))
		{
			// End:0x7F
			if(__NFUN_122__(infItem.DragSrcName, "BookItemWnd"))
			{
				ShowShopItemAddDialog(infItem);
			}
		}
	}
	return;
}

function Clear()
{
	ClearHandleItem();
	m_BookItemCount = 0;
	m_ShopItemCount = 0;
	UpdateShopItemCount(0);
	m_arrBookItem.Remove(0, m_arrBookItem.Length);
	m_arrShopItem.Remove(0, m_arrShopItem.Length);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("RecipeShopWnd.BookItemWnd");
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("RecipeShopWnd.ShopItemWnd");
	return;
}

function ClearHandleItem()
{
	local ItemInfo ItemClear;

	m_HandleItem = ItemClear;
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
	infItem.Name = Class'NWindow.UIDATA_ITEM'.static.GetItemName(infItem.ClassID);
	infItem.Description = Class'NWindow.UIDATA_ITEM'.static.GetItemDescription(infItem.ClassID);
	infItem.Weight = Class'NWindow.UIDATA_ITEM'.static.GetItemWeight(infItem.ClassID);
	infItem.IconName = Class'NWindow.UIDATA_ITEM'.static.GetItemTextureName(ProductID);
	infItem.CrystalType = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeCrystalType(RecipeID);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("RecipeShopWnd.BookItemWnd", infItem);
	m_arrBookItem.Insert(m_arrBookItem.Length, 1);
	m_arrBookItem[__NFUN_147__(m_arrBookItem.Length, 1)] = Index;
	__NFUN_165__(m_BookItemCount);
	return;
}

function AddRecipeShopItem(int RecipeID, int CanbeMade, int MakingFee)
{
	local ItemInfo infItem;
	local int ProductID, Index;

	ProductID = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeProductID(RecipeID);
	Index = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeIndex(RecipeID);
	infItem.ClassID = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeClassID(RecipeID);
	infItem.Level = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeLevel(RecipeID);
	infItem.ServerID = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeIndex(RecipeID);
	infItem.Price = MakingFee;
	infItem.Reserved = CanbeMade;
	infItem.Name = Class'NWindow.UIDATA_ITEM'.static.GetItemName(infItem.ClassID);
	infItem.Description = Class'NWindow.UIDATA_ITEM'.static.GetItemDescription(infItem.ClassID);
	infItem.Weight = Class'NWindow.UIDATA_ITEM'.static.GetItemWeight(infItem.ClassID);
	infItem.IconName = Class'NWindow.UIDATA_ITEM'.static.GetItemTextureName(ProductID);
	infItem.CrystalType = Class'NWindow.UIDATA_RECIPE'.static.GetRecipeCrystalType(RecipeID);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("RecipeShopWnd.ShopItemWnd", infItem);
	m_arrShopItem.Insert(m_arrShopItem.Length, 1);
	m_arrShopItem[__NFUN_147__(m_arrShopItem.Length, 1)] = Index;
	__NFUN_165__(m_ShopItemCount);
	UpdateShopItemCount(m_ShopItemCount);
	return;
}

function ShowShopItemAddDialog(ItemInfo AddItem)
{
	m_HandleItem = AddItem;
	DialogSetID(1);
	DialogSetParamInt(-1);
	DialogSetDefaultOK();
	DialogShow(6, GetSystemMessage(963));
	return;
}

function UpdateShopItem(ItemInfo AddItem)
{
	local int i, Max;
	local ItemInfo infItem;
	local bool bDuplicated;

	bDuplicated = false;
	Max = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("RecipeShopWnd.ShopItemWnd");
	i = 0;
	J0x3F:

	// End:0xB3 [Loop If]
	if(__NFUN_150__(i, Max))
	{
		// End:0xA9
		if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("RecipeShopWnd.ShopItemWnd", i, infItem))
		{
			// End:0xA9
			if(__NFUN_154__(AddItem.ClassID, infItem.ClassID))
			{
				bDuplicated = true;
				// [Explicit Break]
				goto J0xB3;
			}
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x3F;
	}
	J0xB3:

	// End:0xFF
	if(__NFUN_129__(bDuplicated))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("RecipeShopWnd.ShopItemWnd", AddItem);
		__NFUN_165__(m_ShopItemCount);
		UpdateShopItemCount(m_ShopItemCount);
	}
	return;
}

function DeleteShopItem(ItemInfo DeleteItem)
{
	local int i, Max;
	local ItemInfo infItem;

	Max = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("RecipeShopWnd.ShopItemWnd");
	i = 0;
	J0x37:

	// End:0xE4 [Loop If]
	if(__NFUN_150__(i, Max))
	{
		// End:0xDA
		if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("RecipeShopWnd.ShopItemWnd", i, infItem))
		{
			// End:0xDA
			if(__NFUN_154__(DeleteItem.ClassID, infItem.ClassID))
			{
				Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("RecipeShopWnd.ShopItemWnd", i);
				__NFUN_166__(m_ShopItemCount);
				UpdateShopItemCount(m_ShopItemCount);
				// [Explicit Break]
				goto J0xE4;
			}
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x37;
	}
	J0xE4:

	return;
}

function UpdateShopItemCount(int Count)
{
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("RecipeShopWnd.txtCount", __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", string(Count)), "/"), string(20)), ")"));
	return;
}

function StartRecipeShop()
{
	local int i, Max;
	local ItemInfo infItem;
	local string param;
	local int ServerID, Price;

	Max = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("RecipeShopWnd.ShopItemWnd");
	ParamAdd(param, "Max", string(Max));
	i = 0;
	J0x4E:

	// End:0x115 [Loop If]
	if(__NFUN_150__(i, Max))
	{
		ServerID = 0;
		Price = 0;
		// End:0xC2
		if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("RecipeShopWnd.ShopItemWnd", i, infItem))
		{
			ServerID = infItem.ServerID;
			Price = infItem.Price;
		}
		ParamAdd(param, __NFUN_112__("ServerID_", string(i)), string(ServerID));
		ParamAdd(param, __NFUN_112__("Price_", string(i)), string(Price));
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x4E;
	}
	Class'NWindow.RecipeAPI'.static.RequestRecipeShopListSet(param);
	return;
}

function HandleMoveUpItem()
{
	local ItemInfo infItem;

	// End:0x3D
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedItem("RecipeShopWnd.ShopItemWnd", infItem))
	{
		DeleteShopItem(infItem);
	}
	return;
}

function HandleMoveDownItem()
{
	local ItemInfo infItem;

	// End:0x3D
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedItem("RecipeShopWnd.BookItemWnd", infItem))
	{
		ShowShopItemAddDialog(infItem);
	}
	return;
}
