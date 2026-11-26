class WarehouseWnd extends UICommonAPI;

const KEEPING_PRICE = 30;
const DEFAULT_MAX_COUNT = 200;
const DIALOG_TOP_TO_BOTTOM = 111;
const DIALOG_BOTTOM_TO_TOP = 222;

enum WarehouseCategory
{
	WC_None,                        // 0
	WC_Private,                     // 1
	WC_Clan,                        // 2
	WC_Castle,                      // 3
	WC_Etc                          // 4
};

enum WarehouseType
{
	WT_Deposit,                     // 0
	WT_Withdraw                     // 1
};

var WarehouseWnd.WarehouseCategory m_category;
var WarehouseWnd.WarehouseType m_type;
var int m_maxPrivateCount;
var string m_WindowName;
var ItemWindowHandle m_topList;
var ItemWindowHandle m_bottomList;

function OnLoad()
{
	RegisterEvent(2100);
	RegisterEvent(2110);
	RegisterEvent(2070);
	RegisterEvent(1710);
	InitHandle();
	return;
}

function InitHandle()
{
	m_topList = ItemWindowHandle(GetHandle("TopList"));
	m_bottomList = ItemWindowHandle(GetHandle("BottomList"));
	return;
}

function Clear()
{
	m_type = 0;
	m_category = 0;
	m_topList.Clear();
	m_bottomList.Clear();
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".PriceText"), "0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString(__NFUN_112__(m_WindowName, ".PriceText"), "");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".AdenaText"), "0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString(__NFUN_112__(m_WindowName, ".AdenaText"), "");
	Class'NWindow.UIAPI_INVENWEIGHT'.static.ZeroWeight("WarehouseWnd.InvenWeight");
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x1D
		case 2100:
			HandleOpenWindow(param);
			// End:0x60
			break;
		// End:0x33
		case 2110:
			HandleAddItem(param);
			// End:0x60
			break;
		// End:0x49
		case 2070:
			HandleSetMaxCount(param);
			// End:0x60
			break;
		// End:0x5A
		case 1710:
			HandleDialogOK();
			// End:0x60
			break;
		// End:0xFFFF
		default:
			// End:0x60
			break;
			break;
	}
	return;
}

function OnClickButton(string ControlName)
{
	local int Index;

	// End:0x38
	if(__NFUN_122__(ControlName, "UpButton"))
	{
		Index = m_bottomList.GetSelectedNum();
		MoveItemBottomToTop(Index, false);		
	}
	else
	{
		// End:0x72
		if(__NFUN_122__(ControlName, "DownButton"))
		{
			Index = m_topList.GetSelectedNum();
			MoveItemTopToBottom(Index, false);			
		}
		else
		{
			// End:0x8F
			if(__NFUN_122__(ControlName, "OKButton"))
			{
				HandleOKButton();				
			}
			else
			{
				// End:0xB8
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

function OnClickItem(string ControlName, int Index)
{
	local WindowHandle m_dialogWnd;

	m_dialogWnd = GetHandle("DialogBox");
	// End:0x5C
	if(__NFUN_122__(ControlName, "TopList"))
	{
		// End:0x5C
		if(__NFUN_130__(DialogIsMine(), m_dialogWnd.IsShowWindow()))
		{
			DialogHide();
			m_dialogWnd.HideWindow();
		}
	}
	return;
}

function OnDropItem(string strID, ItemInfo Info, int X, int Y)
{
	local int Index;

	// End:0x75
	if(__NFUN_130__(__NFUN_122__(strID, "TopList"), __NFUN_122__(Info.DragSrcName, "BottomList")))
	{
		Index = m_bottomList.FindItemWithClassID(Info.ClassID);
		// End:0x72
		if(__NFUN_153__(Index, 0))
		{
			MoveItemBottomToTop(Index, __NFUN_151__(Info.AllItemCount, 0));
		}		
	}
	else
	{
		// End:0xE7
		if(__NFUN_130__(__NFUN_122__(strID, "BottomList"), __NFUN_122__(Info.DragSrcName, "TopList")))
		{
			Index = m_topList.FindItemWithClassID(Info.ClassID);
			// End:0xE7
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
	local ItemInfo topInfo, bottomInfo;
	local int bottomIndex;

	// End:0x1B2
	if(m_topList.GetItem(Index, topInfo))
	{
		// End:0xA1
		if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bAllItem), IsStackableItem(topInfo.ConsumeType)), __NFUN_151__(topInfo.ItemNum, 1)))
		{
			DialogSetID(111);
			DialogSetReservedInt(topInfo.ClassID);
			DialogSetParamInt(topInfo.ItemNum);
			DialogSetDefaultOK();
			DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), topInfo.Name, ""));			
		}
		else
		{
			bottomIndex = m_bottomList.FindItemWithClassID(topInfo.ClassID);
			// End:0x12F
			if(__NFUN_130__(__NFUN_155__(bottomIndex, -1), IsStackableItem(topInfo.ConsumeType)))
			{
				m_bottomList.GetItem(bottomIndex, bottomInfo);
				__NFUN_161__(bottomInfo.ItemNum, topInfo.ItemNum);
				m_bottomList.SetItem(bottomIndex, bottomInfo);				
			}
			else
			{
				m_bottomList.AddItem(topInfo);
			}
			m_topList.DeleteItem(Index);
			// End:0x1A6
			if(__NFUN_154__(int(m_type), int(1)))
			{
				Class'NWindow.UIAPI_INVENWEIGHT'.static.AddWeight("WarehouseWnd.InvenWeight", __NFUN_144__(topInfo.ItemNum, topInfo.Weight));
			}
			AdjustPrice();
			AdjustCount();
		}
	}
	return;
}

function MoveItemBottomToTop(int Index, bool bAllItem)
{
	local ItemInfo bottomInfo, topInfo;
	local int topIndex;

	// End:0x1B2
	if(m_bottomList.GetItem(Index, bottomInfo))
	{
		// End:0xA1
		if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bAllItem), IsStackableItem(bottomInfo.ConsumeType)), __NFUN_151__(bottomInfo.ItemNum, 1)))
		{
			DialogSetID(222);
			DialogSetReservedInt(bottomInfo.ClassID);
			DialogSetParamInt(bottomInfo.ItemNum);
			DialogSetDefaultOK();
			DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), bottomInfo.Name, ""));			
		}
		else
		{
			topIndex = m_topList.FindItemWithClassID(bottomInfo.ClassID);
			// End:0x12F
			if(__NFUN_130__(__NFUN_155__(topIndex, -1), IsStackableItem(bottomInfo.ConsumeType)))
			{
				m_topList.GetItem(topIndex, topInfo);
				__NFUN_161__(topInfo.ItemNum, bottomInfo.ItemNum);
				m_topList.SetItem(topIndex, topInfo);				
			}
			else
			{
				m_topList.AddItem(bottomInfo);
			}
			m_bottomList.DeleteItem(Index);
			// End:0x1A6
			if(__NFUN_154__(int(m_type), int(1)))
			{
				Class'NWindow.UIAPI_INVENWEIGHT'.static.ReduceWeight("WarehouseWnd.InvenWeight", __NFUN_144__(bottomInfo.ItemNum, bottomInfo.Weight));
			}
			AdjustPrice();
			AdjustCount();
		}
	}
	return;
}

function HandleDialogOK()
{
	local int Id, Num, ClassID, Index, topIndex;

	local ItemInfo Info, topInfo;

	// End:0x377
	if(DialogIsMine())
	{
		Id = DialogGetID();
		Num = int(DialogGetString());
		ClassID = DialogGetReservedInt();
		// End:0x1D0
		if(__NFUN_130__(__NFUN_154__(Id, 111), __NFUN_151__(Num, 0)))
		{
			topIndex = m_topList.FindItemWithClassID(ClassID);
			// End:0x1CD
			if(__NFUN_153__(topIndex, 0))
			{
				m_topList.GetItem(topIndex, topInfo);
				Index = m_bottomList.FindItemWithClassID(ClassID);
				// End:0xF1
				if(__NFUN_153__(Index, 0))
				{
					m_bottomList.GetItem(Index, Info);
					__NFUN_161__(Info.ItemNum, Num);
					m_bottomList.SetItem(Index, Info);					
				}
				else
				{
					Info = topInfo;
					Info.ItemNum = Num;
					Info.bShowCount = false;
					m_bottomList.AddItem(Info);
				}
				// End:0x17C
				if(__NFUN_154__(int(m_type), int(1)))
				{
					Class'NWindow.UIAPI_INVENWEIGHT'.static.AddWeight("WarehouseWnd.InvenWeight", __NFUN_144__(Info.ItemNum, Info.Weight));
				}
				__NFUN_162__(topInfo.ItemNum, Num);
				// End:0x1B4
				if(__NFUN_152__(topInfo.ItemNum, 0))
				{
					m_topList.DeleteItem(topIndex);					
				}
				else
				{
					m_topList.SetItem(topIndex, topInfo);
				}
			}			
		}
		else
		{
			// End:0x36B
			if(__NFUN_130__(__NFUN_154__(Id, 222), __NFUN_151__(Num, 0)))
			{
				Index = m_bottomList.FindItemWithClassID(ClassID);
				// End:0x36B
				if(__NFUN_153__(Index, 0))
				{
					m_bottomList.GetItem(Index, Info);
					__NFUN_162__(Info.ItemNum, Num);
					// End:0x264
					if(__NFUN_151__(Info.ItemNum, 0))
					{
						m_bottomList.SetItem(Index, Info);						
					}
					else
					{
						m_bottomList.DeleteItem(Index);
					}
					topIndex = m_topList.FindItemWithClassID(ClassID);
					// End:0x2F8
					if(__NFUN_130__(__NFUN_153__(topIndex, 0), IsStackableItem(Info.ConsumeType)))
					{
						m_topList.GetItem(topIndex, topInfo);
						__NFUN_161__(topInfo.ItemNum, Num);
						m_topList.SetItem(topIndex, topInfo);						
					}
					else
					{
						Info.ItemNum = Num;
						m_topList.AddItem(Info);
					}
					// End:0x36B
					if(__NFUN_154__(int(m_type), int(1)))
					{
						Class'NWindow.UIAPI_INVENWEIGHT'.static.ReduceWeight("WarehouseWnd.InvenWeight", __NFUN_144__(Info.ItemNum, Info.Weight));
					}
				}
			}
		}
		AdjustPrice();
		AdjustCount();
	}
	return;
}

function HandleOpenWindow(string param)
{
	local string Type;
	local int Adena, tmpInt;
	local string adenaString;
	local WindowHandle m_inventoryWnd;

	m_inventoryWnd = GetHandle("InventoryWnd");
	Clear();
	ParseString(param, "type", Type);
	ParseInt(param, "category", tmpInt);
	m_category = byte(tmpInt);
	ParseInt(param, "adena", Adena);
	switch(m_category)
	{
		// End:0x9C
		case 1:
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle(m_WindowName, 1216);
			// End:0x102
			break;
		// End:0xBD
		case 2:
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle(m_WindowName, 1217);
			// End:0x102
			break;
		// End:0xDE
		case 3:
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle(m_WindowName, 1218);
			// End:0x102
			break;
		// End:0xFC
		case 4:
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle(m_WindowName, 131);
			// End:0x102
			break;
		// End:0xFFFF
		default:
			// End:0x102
			break;
			break;
	}
	// End:0x120
	if(__NFUN_122__(Type, "deposit"))
	{
		m_type = 0;		
	}
	else
	{
		// End:0x13C
		if(__NFUN_122__(Type, "withdraw"))
		{
			m_type = 1;
		}
	}
	adenaString = MakeCostString(string(Adena));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".AdenaText"), adenaString);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString(__NFUN_112__(m_WindowName, ".AdenaText"), ConvertNumToText(string(Adena)));
	// End:0x1C6
	if(m_inventoryWnd.IsShowWindow())
	{
		m_inventoryWnd.HideWindow();
	}
	ShowWindow(m_WindowName);
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus(m_WindowName);
	// End:0x286
	if(__NFUN_154__(int(m_type), int(0)))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".TopText"), GetSystemString(138));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".BottomText"), GetSystemString(132));
		ShowWindow(__NFUN_112__(m_WindowName, ".BottomCountText"));
		HideWindow(__NFUN_112__(m_WindowName, ".TopCountText"));		
	}
	else
	{
		// End:0x324
		if(__NFUN_154__(int(m_type), int(1)))
		{
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".TopText"), GetSystemString(132));
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".BottomText"), GetSystemString(133));
			ShowWindow(__NFUN_112__(m_WindowName, ".TopCountText"));
			HideWindow(__NFUN_112__(m_WindowName, ".BottomCountText"));
		}
	}
	return;
}

function HandleAddItem(string param)
{
	local ItemInfo Info;

	ParamToItemInfo(param, Info);
	m_topList.AddItem(Info);
	AdjustCount();
	return;
}

function AdjustPrice()
{
	local string Adena;
	local int Count;

	// End:0x96
	if(__NFUN_154__(int(m_type), int(0)))
	{
		Count = m_bottomList.GetItemNum();
		Adena = MakeCostString(string(__NFUN_144__(Count, 30)));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".PriceText"), Adena);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString(__NFUN_112__(m_WindowName, ".PriceText"), ConvertNumToText(string(__NFUN_144__(Count, 30))));
	}
	return;
}

function AdjustCount()
{
	local int Num, maxNum;

	// End:0x1E
	if(__NFUN_154__(int(m_category), int(1)))
	{
		maxNum = m_maxPrivateCount;		
	}
	else
	{
		maxNum = 200;
	}
	// End:0x95
	if(__NFUN_154__(int(m_type), int(0)))
	{
		Num = m_bottomList.GetItemNum();
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".BottomCountText"), __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", string(Num)), "/"), string(maxNum)), ")"));		
	}
	else
	{
		// End:0xFE
		if(__NFUN_154__(int(m_type), int(1)))
		{
			Num = m_topList.GetItemNum();
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".TopCountText"), __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", string(Num)), "/"), string(maxNum)), ")"));
		}
	}
	return;
}

function HandleOKButton()
{
	local string param;
	local int bottomCount, bottomIndex;
	local ItemInfo bottomInfo;

	bottomCount = m_bottomList.GetItemNum();
	// End:0xD0
	if(__NFUN_154__(int(m_type), int(0)))
	{
		ParamAdd(param, "num", string(bottomCount));
		bottomIndex = 0;
		J0x43:

		// End:0xC2 [Loop If]
		if(__NFUN_150__(bottomIndex, bottomCount))
		{
			m_bottomList.GetItem(bottomIndex, bottomInfo);
			ParamAdd(param, __NFUN_112__("dbID", string(bottomIndex)), string(bottomInfo.Reserved));
			ParamAdd(param, __NFUN_112__("count", string(bottomIndex)), string(bottomInfo.ItemNum));
			__NFUN_163__(bottomIndex);
			// [Loop Continue]
			goto J0x43;
		}
		RequestWarehouseDeposit(param);		
	}
	else
	{
		// End:0x188
		if(__NFUN_154__(int(m_type), int(1)))
		{
			ParamAdd(param, "num", string(bottomCount));
			bottomIndex = 0;
			J0xFE:

			// End:0x17D [Loop If]
			if(__NFUN_150__(bottomIndex, bottomCount))
			{
				m_bottomList.GetItem(bottomIndex, bottomInfo);
				ParamAdd(param, __NFUN_112__("dbID", string(bottomIndex)), string(bottomInfo.Reserved));
				ParamAdd(param, __NFUN_112__("count", string(bottomIndex)), string(bottomInfo.ItemNum));
				__NFUN_163__(bottomIndex);
				// [Loop Continue]
				goto J0xFE;
			}
			RequestWarehouseWithdraw(param);
		}
	}
	HideWindow(m_WindowName);
	return;
}

function HandleSetMaxCount(string param)
{
	ParseInt(param, "warehousePrivate", m_maxPrivateCount);
	return;
}

defaultproperties
{
	m_WindowName="WarehouseWnd"
}