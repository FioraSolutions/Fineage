class ShopWnd extends UICommonAPI;

const DIALOG_TOP_TO_BOTTOM = 111;
const DIALOG_BOTTOM_TO_TOP = 222;
const DIALOG_PREVIEW = 333;

enum ShopType
{
	ShopNone,                       // 0
	ShopBuy,                        // 1
	ShopSell,                       // 2
	ShopPreview                     // 3
};

var string m_WindowName;
var ShopWnd.ShopType m_shopType;
var int m_merchantID;
var int m_npcID;
var INT64 m_currentPrice;

function OnLoad()
{
	RegisterEvent(2080);
	RegisterEvent(2090);
	RegisterEvent(1710);
	return;
}

function Clear()
{
	m_shopType = 0;
	m_merchantID = -1;
	m_npcID = -1;
	m_currentPrice = Int2Int64(0);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear(__NFUN_112__(m_WindowName, ".TopList"));
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear(__NFUN_112__(m_WindowName, ".BottomList"));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".PriceText"), "0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString(__NFUN_112__(m_WindowName, ".PriceText"), "");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".AdenaText"), "0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString(__NFUN_112__(m_WindowName, ".AdenaText"), "");
	Class'NWindow.UIAPI_INVENWEIGHT'.static.ZeroWeight(__NFUN_112__(m_WindowName, ".InvenWeight"));
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x1D
		case 2080:
			HandleOpenWindow(param);
			// End:0x4A
			break;
		// End:0x33
		case 2090:
			HandleAddItem(param);
			// End:0x4A
			break;
		// End:0x44
		case 1710:
			HandleDialogOK();
			// End:0x4A
			break;
		// End:0xFFFF
		default:
			// End:0x4A
			break;
			break;
	}
	return;
}

function OnClickButton(string ControlName)
{
	local int Index;

	// End:0x4C
	if(__NFUN_122__(ControlName, "UpButton"))
	{
		Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedNum(__NFUN_112__(m_WindowName, ".BottomList"));
		MoveItemBottomToTop(Index, false);		
	}
	else
	{
		// End:0x97
		if(__NFUN_122__(ControlName, "DownButton"))
		{
			Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedNum(__NFUN_112__(m_WindowName, ".TopList"));
			MoveItemTopToBottom(Index, false);			
		}
		else
		{
			// End:0xB4
			if(__NFUN_122__(ControlName, "OKButton"))
			{
				HandleOKButton();				
			}
			else
			{
				// End:0xDD
				if(__NFUN_122__(ControlName, "CancelButton"))
				{
					Clear();
					HideWindow(m_WindowName);
				}
			}
		}
	}
	return;
}

function OnDBClickItem(string ControlName, int Index)
{
	// End:0x22
	if(__NFUN_122__(ControlName, "TopList"))
	{
		MoveItemTopToBottom(Index, false);		
	}
	else
	{
		// End:0x44
		if(__NFUN_122__(ControlName, "BottomList"))
		{
			MoveItemBottomToTop(Index, false);
		}
	}
	return;
}

function OnDropItem(string strID, ItemInfo Info, int X, int Y)
{
	local int Index;

	// End:0x89
	if(__NFUN_130__(__NFUN_122__(strID, "TopList"), __NFUN_122__(Info.DragSrcName, "BottomList")))
	{
		Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".BottomList"), Info.ClassID);
		// End:0x86
		if(__NFUN_153__(Index, 0))
		{
			MoveItemBottomToTop(Index, __NFUN_151__(Info.AllItemCount, 0));
		}		
	}
	else
	{
		// End:0x10C
		if(__NFUN_130__(__NFUN_122__(strID, "BottomList"), __NFUN_122__(Info.DragSrcName, "TopList")))
		{
			Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".TopList"), Info.ClassID);
			// End:0x10C
			if(__NFUN_153__(Index, 0))
			{
				MoveItemTopToBottom(Index, __NFUN_151__(Info.AllItemCount, 0));
			}
		}
	}
	return;
}

function MoveItemTopToBottom(int Index, bool bAllItem)
{
	local int bottomIndex;
	local ItemInfo Info, bottomInfo;

	// End:0x321
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".TopList"), Index, Info))
	{
		// End:0xD6
		if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bAllItem), IsStackableItem(Info.ConsumeType)), __NFUN_155__(Info.ItemNum, 1)))
		{
			DialogSetID(111);
			DialogSetReservedInt(Info.ClassID);
			DialogSetDefaultOK();
			// End:0xA0
			if(__NFUN_154__(int(m_shopType), int(2)))
			{
				DialogSetParamInt(Info.ItemNum);				
			}
			else
			{
				DialogSetParamInt(-1);
			}
			DialogSetDefaultOK();
			DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name, ""));			
		}
		else
		{
			Info.bShowCount = false;
			// End:0x209
			if(__NFUN_154__(int(m_shopType), int(2)))
			{
				bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".BottomList"), Info.ClassID);
				// End:0x1B9
				if(__NFUN_130__(__NFUN_153__(bottomIndex, 0), IsStackableItem(Info.ConsumeType)))
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".BottomList"), bottomIndex, bottomInfo);
					__NFUN_161__(bottomInfo.ItemNum, Info.ItemNum);
					Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem(__NFUN_112__(m_WindowName, ".BottomList"), bottomIndex, bottomInfo);					
				}
				else
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__(m_WindowName, ".BottomList"), Info);
				}
				Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem(__NFUN_112__(m_WindowName, ".TopList"), Index);				
			}
			else
			{
				// End:0x28A
				if(__NFUN_154__(int(m_shopType), int(1)))
				{
					Info.ItemNum = 1;
					Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__(m_WindowName, ".BottomList"), Info);
					Class'NWindow.UIAPI_INVENWEIGHT'.static.AddWeight(__NFUN_112__(m_WindowName, ".InvenWeight"), __NFUN_144__(Info.Weight, Info.ItemNum));					
				}
				else
				{
					// End:0x301
					if(__NFUN_154__(int(m_shopType), int(3)))
					{
						bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".BottomList"), Info.ClassID);
						Info.ItemNum = 1;
						Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__(m_WindowName, ".BottomList"), Info);
					}
				}
			}
			AddPrice(Int64Mul(Info.Price, Info.ItemNum));
		}
	}
	return;
}

function MoveItemBottomToTop(int Index, bool bAllItem)
{
	local ItemInfo Info, info2;
	local int bottomIndex;

	// End:0x221
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".BottomList"), Index, Info))
	{
		// End:0xA9
		if(__NFUN_130__(__NFUN_129__(bAllItem), IsStackableItem(Info.ConsumeType)))
		{
			DialogSetID(222);
			DialogSetDefaultOK();
			DialogSetReservedInt(Info.ClassID);
			DialogSetParamInt(Info.ItemNum);
			DialogSetDefaultOK();
			DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name, ""));			
		}
		else
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem(__NFUN_112__(m_WindowName, ".BottomList"), Index);
			// End:0x1B5
			if(__NFUN_154__(int(m_shopType), int(2)))
			{
				bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID(__NFUN_112__(m_WindowName, ".TopList"), Info.ServerID);
				// End:0x148
				if(__NFUN_154__(bottomIndex, -1))
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__(m_WindowName, ".TopList"), Info);					
				}
				else
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".TopList"), bottomIndex, info2);
					__NFUN_161__(info2.ItemNum, Info.ItemNum);
					Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem(__NFUN_112__(m_WindowName, ".TopList"), bottomIndex, info2);
				}				
			}
			else
			{
				// End:0x1FF
				if(__NFUN_154__(int(m_shopType), int(1)))
				{
					Class'NWindow.UIAPI_INVENWEIGHT'.static.ReduceWeight(__NFUN_112__(m_WindowName, ".InvenWeight"), __NFUN_144__(Info.Weight, Info.ItemNum));
				}
			}
			AddPrice(Int64Mul(__NFUN_143__(Info.Price), Info.ItemNum));
		}
	}
	return;
}

function HandleDialogOK()
{
	local int Id, Num, ClassID, Index, topIndex;

	local ItemInfo Info, topInfo;
	local string param;

	// End:0x630
	if(DialogIsMine())
	{
		Id = DialogGetID();
		Num = int(DialogGetString());
		ClassID = DialogGetReservedInt();
		// End:0x2CA
		if(__NFUN_130__(__NFUN_154__(Id, 111), __NFUN_151__(Num, 0)))
		{
			topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".TopList"), ClassID);
			// End:0x2C7
			if(__NFUN_153__(topIndex, 0))
			{
				Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".TopList"), topIndex, topInfo);
				// End:0xCF
				if(__NFUN_154__(int(m_shopType), int(2)))
				{
					Num = __NFUN_249__(Num, topInfo.ItemNum);
				}
				Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".BottomList"), ClassID);
				// End:0x191
				if(__NFUN_153__(Index, 0))
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".BottomList"), Index, Info);
					__NFUN_161__(Info.ItemNum, Num);
					Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem(__NFUN_112__(m_WindowName, ".BottomList"), Index, Info);
					AddPrice(Int64Mul(Num, Info.Price));					
				}
				else
				{
					Info = topInfo;
					Info.ItemNum = Num;
					Info.bShowCount = false;
					Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__(m_WindowName, ".BottomList"), Info);
					AddPrice(Int64Mul(Num, Info.Price));
				}
				// End:0x282
				if(__NFUN_154__(int(m_shopType), int(2)))
				{
					__NFUN_162__(topInfo.ItemNum, Num);
					// End:0x255
					if(__NFUN_152__(topInfo.ItemNum, 0))
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem(__NFUN_112__(m_WindowName, ".TopList"), topIndex);						
					}
					else
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem(__NFUN_112__(m_WindowName, ".TopList"), topIndex, topInfo);
					}					
				}
				else
				{
					// End:0x2C7
					if(__NFUN_154__(int(m_shopType), int(1)))
					{
						Class'NWindow.UIAPI_INVENWEIGHT'.static.AddWeight(__NFUN_112__(m_WindowName, ".InvenWeight"), __NFUN_144__(Info.Weight, Num));
					}
				}
			}			
		}
		else
		{
			// End:0x522
			if(__NFUN_130__(__NFUN_154__(Id, 222), __NFUN_151__(Num, 0)))
			{
				Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".BottomList"), ClassID);
				// End:0x51F
				if(__NFUN_153__(Index, 0))
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".BottomList"), Index, Info);
					__NFUN_162__(Info.ItemNum, Num);
					// End:0x39A
					if(__NFUN_151__(Info.ItemNum, 0))
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem(__NFUN_112__(m_WindowName, ".BottomList"), Index, Info);						
					}
					else
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem(__NFUN_112__(m_WindowName, ".BottomList"), Index);
					}
					// End:0x4BD
					if(__NFUN_154__(int(m_shopType), int(2)))
					{
						topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".TopList"), ClassID);
						// End:0x485
						if(__NFUN_130__(__NFUN_153__(topIndex, 0), IsStackableItem(Info.ConsumeType)))
						{
							Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".TopList"), topIndex, topInfo);
							__NFUN_161__(topInfo.ItemNum, Num);
							Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem(__NFUN_112__(m_WindowName, ".TopList"), topIndex, topInfo);							
						}
						else
						{
							Info.ItemNum = Num;
							Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__(m_WindowName, ".TopList"), Info);
						}						
					}
					else
					{
						// End:0x502
						if(__NFUN_154__(int(m_shopType), int(1)))
						{
							Class'NWindow.UIAPI_INVENWEIGHT'.static.ReduceWeight(__NFUN_112__(m_WindowName, ".InvenWeight"), __NFUN_144__(Info.Weight, Num));
						}
					}
					AddPrice(Int64Mul(__NFUN_143__(Num), Info.Price));
				}				
			}
			else
			{
				// End:0x630
				if(__NFUN_154__(Id, 333))
				{
					Num = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum(__NFUN_112__(m_WindowName, ".BottomList"));
					// End:0x630
					if(__NFUN_151__(Num, 0))
					{
						ParamAdd(param, "merchant", string(m_merchantID));
						ParamAdd(param, "npc", string(m_npcID));
						ParamAdd(param, "num", string(Num));
						Index = 0;
						J0x5B6:

						// End:0x625 [Loop If]
						if(__NFUN_150__(Index, Num))
						{
							Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".BottomList"), Index, Info);
							ParamAdd(param, __NFUN_112__("classID", string(Index)), string(Info.ClassID));
							__NFUN_163__(Index);
							// [Loop Continue]
							goto J0x5B6;
						}
						RequestPreviewItem(param);
					}
				}
			}
		}
	}
	return;
}

function HandleOpenWindow(string param)
{
	local string Type;
	local int Adena;
	local string adenaString;
	local WindowHandle m_inventoryWnd;

	m_inventoryWnd = GetHandle("InventoryWnd");
	Clear();
	ParseString(param, "type", Type);
	ParseInt(param, "merchant", m_merchantID);
	ParseInt(param, "adena", Adena);
	// End:0x81
	if(__NFUN_122__(Type, "buy"))
	{
		m_shopType = 1;		
	}
	else
	{
		// End:0x9C
		if(__NFUN_122__(Type, "sell"))
		{
			m_shopType = 2;			
		}
		else
		{
			// End:0xBA
			if(__NFUN_122__(Type, "preview"))
			{
				m_shopType = 3;				
			}
			else
			{
				m_shopType = 0;
			}
		}
	}
	adenaString = MakeCostString(string(Adena));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".AdenaText"), adenaString);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString(__NFUN_112__(m_WindowName, ".AdenaText"), ConvertNumToText(string(Adena)));
	// End:0x14C
	if(m_inventoryWnd.IsShowWindow())
	{
		m_inventoryWnd.HideWindow();
	}
	ShowWindow(m_WindowName);
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus(m_WindowName);
	// End:0x28E
	if(__NFUN_122__(Type, "buy"))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType(__NFUN_112__(m_WindowName, ".TopList"), "InventoryPrice1HideEnchantStackable");
		Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType(__NFUN_112__(m_WindowName, ".BottomList"), "InventoryPrice1");
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle(m_WindowName, 136);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".TopText"), GetSystemString(137));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".BottomText"), GetSystemString(139));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".PriceConstText"), GetSystemString(142));		
	}
	else
	{
		// End:0x39E
		if(__NFUN_122__(Type, "sell"))
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType(__NFUN_112__(m_WindowName, ".TopList"), "InventoryPrice2");
			Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType(__NFUN_112__(m_WindowName, ".BottomList"), "InventoryPrice2");
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle(m_WindowName, 136);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".TopText"), GetSystemString(138));
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".BottomText"), GetSystemString(137));
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".PriceConstText"), GetSystemString(143));			
		}
		else
		{
			// End:0x4E3
			if(__NFUN_122__(Type, "preview"))
			{
				ParseInt(param, "npc", m_npcID);
				Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType(__NFUN_112__(m_WindowName, ".TopList"), "InventoryPrice1HideEnchantStackable");
				Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType(__NFUN_112__(m_WindowName, ".BottomList"), "InventoryPrice1");
				Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle(m_WindowName, 847);
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".TopText"), GetSystemString(811));
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".BottomText"), GetSystemString(812));
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".PriceConstText"), GetSystemString(813));
			}
		}
	}
	return;
}

function HandleAddItem(string param)
{
	local ItemInfo Info;

	ParamToItemInfo(param, Info);
	// End:0x3F
	if(__NFUN_130__(__NFUN_154__(int(m_shopType), int(1)), __NFUN_151__(Info.ItemNum, 0)))
	{
		Info.bShowCount = true;
	}
	Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__(m_WindowName, ".TopList"), Info);
	return;
}

function AddPrice(INT64 Price)
{
	local string Adena;

	m_currentPrice = Int64Add(m_currentPrice, Price);
	// End:0x45
	if(__NFUN_132__(__NFUN_150__(m_currentPrice.nLeft, 0), __NFUN_150__(m_currentPrice.nRight, 0)))
	{
		m_currentPrice = Int2Int64(0);
	}
	Adena = MakeCostStringInt64(m_currentPrice);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".PriceText"), Adena);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString(__NFUN_112__(m_WindowName, ".PriceText"), ConvertNumToText(Int64ToString(m_currentPrice)));
	return;
}

function HandleOKButton()
{
	local string param;
	local int topCount, bottomCount, topIndex, bottomIndex;
	local ItemInfo topInfo, bottomInfo;
	local int limitedItemCount;

	bottomCount = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum(__NFUN_112__(m_WindowName, ".BottomList"));
	// End:0x267
	if(__NFUN_154__(int(m_shopType), int(1)))
	{
		topCount = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum(__NFUN_112__(m_WindowName, ".TopList"));
		topIndex = 0;
		J0x66:

		// End:0x189 [Loop If]
		if(__NFUN_150__(topIndex, topCount))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".TopList"), topIndex, topInfo);
			// End:0x17F
			if(__NFUN_151__(topInfo.ItemNum, 0))
			{
				limitedItemCount = 0;
				bottomCount = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum(__NFUN_112__(m_WindowName, ".BottomList"));
				bottomIndex = 0;
				J0xE6:

				// End:0x156 [Loop If]
				if(__NFUN_150__(bottomIndex, bottomCount))
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".BottomList"), bottomIndex, bottomInfo);
					// End:0x14C
					if(__NFUN_154__(bottomInfo.ClassID, topInfo.ClassID))
					{
						__NFUN_161__(limitedItemCount, bottomInfo.ItemNum);
					}
					__NFUN_163__(bottomIndex);
					// [Loop Continue]
					goto J0xE6;
				}
				// End:0x17F
				if(__NFUN_151__(limitedItemCount, topInfo.ItemNum))
				{
					DialogShow(4, GetSystemMessage(1338));
					return;
				}
			}
			__NFUN_163__(topIndex);
			// [Loop Continue]
			goto J0x66;
		}
		ParamAdd(param, "merchant", string(m_merchantID));
		ParamAdd(param, "num", string(bottomCount));
		bottomIndex = 0;
		J0x1C3:

		// End:0x259 [Loop If]
		if(__NFUN_150__(bottomIndex, bottomCount))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".BottomList"), bottomIndex, bottomInfo);
			ParamAdd(param, __NFUN_112__("classID", string(bottomIndex)), string(bottomInfo.ClassID));
			ParamAdd(param, __NFUN_112__("count", string(bottomIndex)), string(bottomInfo.ItemNum));
			__NFUN_163__(bottomIndex);
			// [Loop Continue]
			goto J0x1C3;
		}
		RequestBuyItem(param);		
	}
	else
	{
		// End:0x37F
		if(__NFUN_154__(int(m_shopType), int(2)))
		{
			ParamAdd(param, "merchant", string(m_merchantID));
			ParamAdd(param, "num", string(bottomCount));
			bottomIndex = 0;
			J0x2B1:

			// End:0x371 [Loop If]
			if(__NFUN_150__(bottomIndex, bottomCount))
			{
				Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".BottomList"), bottomIndex, bottomInfo);
				ParamAdd(param, __NFUN_112__("serverID", string(bottomIndex)), string(bottomInfo.ServerID));
				ParamAdd(param, __NFUN_112__("classID", string(bottomIndex)), string(bottomInfo.ClassID));
				ParamAdd(param, __NFUN_112__("count", string(bottomIndex)), string(bottomInfo.ItemNum));
				__NFUN_163__(bottomIndex);
				// [Loop Continue]
				goto J0x2B1;
			}
			RequestSellItem(param);			
		}
		else
		{
			// End:0x3B8
			if(__NFUN_154__(int(m_shopType), int(3)))
			{
				// End:0x3B8
				if(__NFUN_151__(bottomCount, 0))
				{
					DialogSetID(333);
					DialogShow(4, GetSystemMessage(1157));
				}
			}
		}
	}
	HideWindow(m_WindowName);
	return;
}

defaultproperties
{
	m_WindowName="ShopWnd"
}