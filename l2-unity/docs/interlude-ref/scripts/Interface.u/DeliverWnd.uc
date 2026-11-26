class DeliverWnd extends UICommonAPI;

const DIALOG_TOP_TO_BOTTOM = 111;
const DIALOG_BOTTOM_TO_TOP = 222;

var int m_targetID;

function OnLoad()
{
	RegisterEvent(2160);
	RegisterEvent(2170);
	RegisterEvent(1710);
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("DeliverWnd.TopList");
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("DeliverWnd.BottomList");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("DeliverWnd.PriceText", "0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("DeliverWnd.PriceText", "");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("DeliverWnd.AdenaText", "0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("DeliverWnd.AdenaText", "");
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x1D
		case 2160:
			HandleOpenWindow(param);
			// End:0x4A
			break;
		// End:0x33
		case 2170:
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

	// End:0x4F
	if(__NFUN_122__(ControlName, "UpButton"))
	{
		Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedNum("DeliverWnd.BottomList");
		MoveItemBottomToTop(Index, 0);		
	}
	else
	{
		// End:0x9D
		if(__NFUN_122__(ControlName, "DownButton"))
		{
			Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedNum("DeliverWnd.TopList");
			MoveItemTopToBottom(Index, 0);			
		}
		else
		{
			// End:0xBA
			if(__NFUN_122__(ControlName, "OKButton"))
			{
				HandleOKButton();				
			}
			else
			{
				// End:0xEA
				if(__NFUN_122__(ControlName, "CancelButton"))
				{
					Clear();
					HideWindow("DeliverWnd");
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
		MoveItemTopToBottom(Index, 0);		
	}
	else
	{
		// End:0x44
		if(__NFUN_122__(ControlName, "BottomList"))
		{
			MoveItemBottomToTop(Index, 0);
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

	// End:0x89
	if(__NFUN_130__(__NFUN_122__(strID, "TopList"), __NFUN_122__(Info.DragSrcName, "BottomList")))
	{
		Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("DeliverWnd.BottomList", Info.ClassID);
		// End:0x86
		if(__NFUN_153__(Index, 0))
		{
			MoveItemBottomToTop(Index, Info.AllItemCount);
		}		
	}
	else
	{
		// End:0x10C
		if(__NFUN_130__(__NFUN_122__(strID, "BottomList"), __NFUN_122__(Info.DragSrcName, "TopList")))
		{
			Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("DeliverWnd.TopList", Info.ClassID);
			// End:0x10C
			if(__NFUN_153__(Index, 0))
			{
				MoveItemTopToBottom(Index, Info.AllItemCount);
			}
		}
	}
	return;
}

function MoveItemTopToBottom(int Index, int AllItemCount)
{
	local ItemInfo Info;

	// End:0x12B
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("DeliverWnd.TopList", Index, Info))
	{
		// End:0xB9
		if(IsStackableItem(Info.ConsumeType))
		{
			// End:0x66
			if(__NFUN_151__(AllItemCount, 0))
			{
				ItemTopToBottom(Info.ClassID, AllItemCount);				
			}
			else
			{
				DialogSetID(111);
				DialogSetReservedInt(Info.ClassID);
				DialogSetParamInt(Info.ItemNum);
				DialogSetDefaultOK();
				DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name, ""));
			}			
		}
		else
		{
			Info.ItemNum = 1;
			Info.bShowCount = false;
			Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("DeliverWnd.BottomList", Info);
			Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("DeliverWnd.TopList", Index);
			AdjustPrice();
		}
	}
	return;
}

function MoveItemBottomToTop(int Index, int AllItemCount)
{
	local ItemInfo Info;

	// End:0x115
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("DeliverWnd.BottomList", Index, Info))
	{
		// End:0xBC
		if(IsStackableItem(Info.ConsumeType))
		{
			// End:0x69
			if(__NFUN_151__(AllItemCount, 0))
			{
				ItemBottomToTop(Info.ClassID, AllItemCount);				
			}
			else
			{
				DialogSetID(222);
				DialogSetReservedInt(Info.ClassID);
				DialogSetParamInt(Info.ItemNum);
				DialogSetDefaultOK();
				DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name, ""));
			}			
		}
		else
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("DeliverWnd.BottomList", Index);
			Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("DeliverWnd.TopList", Info);
			AdjustPrice();
		}
	}
	return;
}

function HandleDialogOK()
{
	local int Id, Num, ClassID;

	// End:0x70
	if(DialogIsMine())
	{
		Id = DialogGetID();
		Num = int(DialogGetString());
		ClassID = DialogGetReservedInt();
		// End:0x4E
		if(__NFUN_154__(Id, 111))
		{
			ItemTopToBottom(ClassID, Num);			
		}
		else
		{
			// End:0x6A
			if(__NFUN_154__(Id, 222))
			{
				ItemBottomToTop(ClassID, Num);
			}
		}
		AdjustPrice();
	}
	return;
}

function ItemTopToBottom(int ClassID, int Num)
{
	local int Index, topIndex;
	local ItemInfo Info, topInfo;

	topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("DeliverWnd.TopList", ClassID);
	// End:0x1E2
	if(__NFUN_153__(topIndex, 0))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("DeliverWnd.TopList", topIndex, topInfo);
		Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("DeliverWnd.BottomList", ClassID);
		// End:0x116
		if(__NFUN_153__(Index, 0))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("DeliverWnd.BottomList", Index, Info);
			__NFUN_161__(Info.ItemNum, Num);
			Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("DeliverWnd.BottomList", Index, Info);			
		}
		else
		{
			Info = topInfo;
			Info.ItemNum = Num;
			Info.bShowCount = false;
			Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("DeliverWnd.BottomList", Info);
		}
		__NFUN_162__(topInfo.ItemNum, Num);
		// End:0x1B5
		if(__NFUN_152__(topInfo.ItemNum, 0))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("DeliverWnd.TopList", topIndex);			
		}
		else
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("DeliverWnd.TopList", topIndex, topInfo);
		}
	}
	return;
}

function ItemBottomToTop(int ClassID, int Num)
{
	local int Index, topIndex;
	local ItemInfo Info, topInfo;

	Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("DeliverWnd.BottomList", ClassID);
	// End:0x1DF
	if(__NFUN_153__(Index, 0))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("DeliverWnd.BottomList", Index, Info);
		__NFUN_162__(Info.ItemNum, Num);
		// End:0xC0
		if(__NFUN_151__(Info.ItemNum, 0))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("DeliverWnd.BottomList", Index, Info);			
		}
		else
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("DeliverWnd.BottomList", Index);
		}
		topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("DeliverWnd.TopList", ClassID);
		// End:0x1A7
		if(__NFUN_130__(__NFUN_153__(topIndex, 0), IsStackableItem(Info.ConsumeType)))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("DeliverWnd.TopList", topIndex, topInfo);
			__NFUN_161__(topInfo.ItemNum, Num);
			Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("DeliverWnd.TopList", topIndex, topInfo);			
		}
		else
		{
			Info.ItemNum = Num;
			Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("DeliverWnd.TopList", Info);
		}
	}
	return;
}

function HandleOpenWindow(string param)
{
	local int Adena;
	local string adenaString;
	local WindowHandle m_inventoryWnd;

	m_inventoryWnd = GetHandle("InventoryWnd");
	Clear();
	ParseInt(param, "adena", Adena);
	ParseInt(param, "destinationID", m_targetID);
	adenaString = MakeCostString(string(Adena));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("DeliverWnd.AdenaText", adenaString);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("DeliverWnd.AdenaText", ConvertNumToText(string(Adena)));
	// End:0xE6
	if(m_inventoryWnd.IsShowWindow())
	{
		m_inventoryWnd.HideWindow();
	}
	ShowWindow("DeliverWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("DeliverWnd");
	return;
}

function HandleAddItem(string param)
{
	local ItemInfo Info;

	ParamToItemInfo(param, Info);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("DeliverWnd.TopList", Info);
	return;
}

function AdjustPrice()
{
	local string Adena;
	local int Count;

	Count = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("DeliverWnd.BottomList");
	Adena = MakeCostString(string(__NFUN_144__(Count, 1000)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("DeliverWnd.PriceText", Adena);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("DeliverWnd.PriceText", ConvertNumToText(string(__NFUN_144__(Count, 1000))));
	return;
}

function HandleOKButton()
{
	local string param;
	local int Count, Index;
	local ItemInfo ItemInfo;

	Count = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("DeliverWnd.BottomList");
	ParamAdd(param, "targetID", string(m_targetID));
	ParamAdd(param, "num", string(Count));
	Index = 0;
	J0x66:

	// End:0xFC [Loop If]
	if(__NFUN_150__(Index, Count))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("DeliverWnd.BottomList", Index, ItemInfo);
		ParamAdd(param, __NFUN_112__("dbID", string(Index)), string(ItemInfo.Reserved));
		ParamAdd(param, __NFUN_112__("count", string(Index)), string(ItemInfo.ItemNum));
		__NFUN_163__(Index);
		// [Loop Continue]
		goto J0x66;
	}
	RequestPackageSend(param);
	HideWindow("DeliverWnd");
	return;
}
