class TradeWnd extends UICommonAPI;

const DIALOG_ID_TRADE_REQUEST = 0;
const DIALOG_ID_ITEM_NUMBER = 1;

function OnLoad()
{
	RegisterEvent(1710);
	RegisterEvent(1720);
	RegisterEvent(1950);
	RegisterEvent(1960);
	RegisterEvent(1970);
	RegisterEvent(1980);
	RegisterEvent(1990);
	RegisterEvent(2000);
	return;
}

function OnSendPacketWhenHiding()
{
	RequestTradeDone(false);
	return;
}

function OnHide()
{
	Clear();
	return;
}

function OnEvent(int EventID, string param)
{
	switch(EventID)
	{
		// End:0x1D
		case 1950:
			HandleStartTrade(param);
			// End:0xB3
			break;
		// End:0x33
		case 1960:
			HandleTradeAddItem(param);
			// End:0xB3
			break;
		// End:0x49
		case 1970:
			HandleTradeDone(param);
			// End:0xB3
			break;
		// End:0x5F
		case 1980:
			HandleTradeOtherOK(param);
			// End:0xB3
			break;
		// End:0x75
		case 1990:
			HandleTradeUpdateInventoryItem(param);
			// End:0xB3
			break;
		// End:0x8B
		case 2000:
			HandleReceiveStartTrade(param);
			// End:0xB3
			break;
		// End:0x9C
		case 1710:
			HandleDialogOK();
			// End:0xB3
			break;
		// End:0xAD
		case 1720:
			HandleDialogCancel();
			// End:0xB3
			break;
		// End:0xFFFF
		default:
			// End:0xB3
			break;
			break;
	}
	return;
}

function OnClickButton(string ControlName)
{
	// End:0x3F
	if(__NFUN_122__(ControlName, "OKButton"))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.SetFaded("TradeWnd.MyList", true);
		RequestTradeDone(true);		
	}
	else
	{
		// End:0x61
		if(__NFUN_122__(ControlName, "CancelButton"))
		{
			RequestTradeDone(false);			
		}
		else
		{
			// End:0x7D
			if(__NFUN_122__(ControlName, "MoveButton"))
			{
				HandleMoveButton();
			}
		}
	}
	return;
}

function OnDBClickItem(string ControlName, int Index)
{
	local ItemInfo Info;

	// End:0xCF
	if(__NFUN_122__(ControlName, "InventoryList"))
	{
		// End:0xCF
		if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("TradeWnd.InventoryList", Index, Info))
		{
			// End:0xBE
			if(__NFUN_130__(IsStackableItem(Info.ConsumeType), __NFUN_155__(Info.ItemNum, 1)))
			{
				DialogSetID(1);
				DialogSetReservedInt(Info.ServerID);
				DialogSetParamInt(Info.ItemNum);
				DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name, ""));				
			}
			else
			{
				RequestAddTradeItem(Info.ServerID, 1);
			}
		}
	}
	return;
}

function OnDropItem(string strID, ItemInfo Info, int X, int Y)
{
	// End:0xF3
	if(__NFUN_130__(__NFUN_122__(strID, "MyList"), __NFUN_122__(Info.DragSrcName, "InventoryList")))
	{
		// End:0xE2
		if(IsStackableItem(Info.ConsumeType))
		{
			// End:0x72
			if(__NFUN_151__(Info.AllItemCount, 0))
			{
				RequestAddTradeItem(Info.ServerID, Info.AllItemCount);				
			}
			else
			{
				// End:0x96
				if(__NFUN_154__(Info.ItemNum, 1))
				{
					RequestAddTradeItem(Info.ServerID, 1);					
				}
				else
				{
					DialogSetID(1);
					DialogSetReservedInt(Info.ServerID);
					DialogSetParamInt(Info.ItemNum);
					DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name, ""));
				}
			}			
		}
		else
		{
			RequestAddTradeItem(Info.ServerID, 1);
		}
	}
	return;
}

function MoveToMyList(int Index, int Num)
{
	local ItemInfo Info;

	// End:0x49
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("TradeWnd.InventoryList", Index, Info))
	{
		RequestAddTradeItem(Info.ServerID, Num);
	}
	return;
}

function HandleMoveButton()
{
	local int selected;
	local ItemInfo Info;

	selected = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedNum("TradeWnd.InventoryList");
	// End:0xD1
	if(__NFUN_153__(selected, 0))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("TradeWnd.InventoryList", selected, Info);
		// End:0x88
		if(__NFUN_154__(Info.ItemNum, 1))
		{
			MoveToMyList(selected, 1);			
		}
		else
		{
			DialogSetID(1);
			DialogSetReservedInt(Info.ServerID);
			DialogSetParamInt(Info.ItemNum);
			DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name, ""));
		}
	}
	return;
}

function HandleStartTrade(string param)
{
	local int targetID;
	local UserInfo TargetInfo;
	local string ClanName;
	local WindowHandle m_inventoryWnd, m_warehouseWnd, m_privateShopWnd, m_shopWnd, m_multiSellWnd;

	m_inventoryWnd = GetHandle("InventoryWnd");
	m_warehouseWnd = GetHandle("WarehouseWnd");
	m_privateShopWnd = GetHandle("PrivateShopWnd");
	m_shopWnd = GetHandle("ShopWnd");
	m_multiSellWnd = GetHandle("MultiSellWnd");
	// End:0xA0
	if(m_inventoryWnd.IsShowWindow())
	{
		m_inventoryWnd.HideWindow();
	}
	// End:0xC1
	if(m_warehouseWnd.IsShowWindow())
	{
		m_warehouseWnd.HideWindow();
	}
	// End:0xE2
	if(m_privateShopWnd.IsShowWindow())
	{
		m_privateShopWnd.HideWindow();
	}
	// End:0x103
	if(m_shopWnd.IsShowWindow())
	{
		m_shopWnd.HideWindow();
	}
	// End:0x124
	if(m_multiSellWnd.IsShowWindow())
	{
		m_multiSellWnd.HideWindow();
	}
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TradeWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("TradeWnd");
	ParseInt(param, "targetId", targetID);
	// End:0x21E
	if(__NFUN_151__(targetID, 0))
	{
		GetUserInfo(targetID, TargetInfo);
		// End:0x1F0
		if(__NFUN_151__(TargetInfo.nClanID, 0))
		{
			ClanName = GetClanName(TargetInfo.nClanID);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("TradeWnd.Targetname", __NFUN_112__(__NFUN_112__(TargetInfo.Name, " - "), ClanName));			
		}
		else
		{
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("TradeWnd.Targetname", TargetInfo.Name);
		}
	}
	return;
}

function HandleTradeAddItem(string param)
{
	local string strDest;
	local ItemInfo ItemInfo, tempInfo;
	local int Index;

	ParseString(param, "destination", strDest);
	ParamToItemInfo(param, ItemInfo);
	// End:0x67
	if(__NFUN_122__(strDest, "inventoryList"))
	{
		strDest = "TradeWnd.InventoryList";		
	}
	else
	{
		// End:0xCE
		if(__NFUN_122__(strDest, "myList"))
		{
			strDest = "TradeWnd.MyList";
			Class'NWindow.UIAPI_INVENWEIGHT'.static.ReduceWeight("TradeWnd.InvenWeight", __NFUN_144__(ItemInfo.ItemNum, ItemInfo.Weight));			
		}
		else
		{
			// End:0x138
			if(__NFUN_122__(strDest, "otherList"))
			{
				strDest = "TradeWnd.OtherList";
				Class'NWindow.UIAPI_INVENWEIGHT'.static.AddWeight("TradeWnd.InvenWeight", __NFUN_144__(ItemInfo.ItemNum, ItemInfo.Weight));
			}
		}
	}
	Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID(strDest, ItemInfo.ServerID);
	// End:0x1CF
	if(__NFUN_153__(Index, 0))
	{
		// End:0x1CC
		if(IsStackableItem(ItemInfo.ConsumeType))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(strDest, Index, tempInfo);
			__NFUN_161__(ItemInfo.ItemNum, tempInfo.ItemNum);
			Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem(strDest, Index, ItemInfo);
		}		
	}
	else
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(strDest, ItemInfo);
	}
	return;
}

function HandleTradeDone(string param)
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TradeWnd");
	return;
}

function HandleTradeOtherOK(string param)
{
	Class'NWindow.UIAPI_ITEMWINDOW'.static.SetFaded("TradeWnd.OtherList", true);
	return;
}

function HandleTradeUpdateInventoryItem(string param)
{
	local ItemInfo Info;
	local string Type;
	local int Index;

	ParseString(param, "type", Type);
	ParamToItemInfo(param, Info);
	// End:0x64
	if(__NFUN_122__(Type, "add"))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("TradeWnd.InventoryList", Info);		
	}
	else
	{
		// End:0xEC
		if(__NFUN_122__(Type, "update"))
		{
			Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("TradeWnd.InventoryList", Info.ServerID);
			// End:0xE9
			if(__NFUN_153__(Index, 0))
			{
				Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("TradeWnd.InventoryList", Index, Info);
			}			
		}
		else
		{
			// End:0x16C
			if(__NFUN_122__(Type, "delete"))
			{
				Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("TradeWnd.InventoryList", Info.ServerID);
				// End:0x16C
				if(__NFUN_153__(Index, 0))
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("TradeWnd.InventoryList", Index);
				}
			}
		}
	}
	return;
}

function HandleReceiveStartTrade(string param)
{
	local int targetID;
	local UserInfo Info;

	ParseInt(param, "targetID", targetID);
	// End:0x72
	if(__NFUN_130__(__NFUN_151__(targetID, 0), GetUserInfo(targetID, Info)))
	{
		DialogSetID(0);
		DialogSetParamInt(__NFUN_144__(10, 1000));
		DialogShow(7, MakeFullSystemMsg(GetSystemMessage(100), Info.Name, ""));
	}
	return;
}

function HandleDialogOK()
{
	local int ServerID, Num;

	// End:0x55
	if(DialogIsMine())
	{
		// End:0x1F
		if(__NFUN_154__(DialogGetID(), 0))
		{
			AnswerTradeRequest(true);			
		}
		else
		{
			// End:0x55
			if(__NFUN_154__(DialogGetID(), 1))
			{
				ServerID = DialogGetReservedInt();
				Num = int(DialogGetString());
				RequestAddTradeItem(ServerID, Num);
			}
		}
	}
	return;
}

function HandleDialogCancel()
{
	// End:0x1C
	if(DialogIsMine())
	{
		// End:0x1C
		if(__NFUN_154__(DialogGetID(), 0))
		{
			AnswerTradeRequest(false);
		}
	}
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("TradeWnd.InventoryList");
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("TradeWnd.MyList");
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("TradeWnd.OtherList");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("TradeWnd.TargetName", "");
	Class'NWindow.UIAPI_INVENWEIGHT'.static.ZeroWeight("TradeWnd.InvenWeight");
	return;
}
