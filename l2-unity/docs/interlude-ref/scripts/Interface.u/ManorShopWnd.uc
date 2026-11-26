class ManorShopWnd extends ShopWnd;

function OnLoad()
{
	RegisterEvent(2680);
	RegisterEvent(2690);
	RegisterEvent(1710);
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x1D
		case 2680:
			HandleOpenWindow(param);
			// End:0x4A
			break;
		// End:0x33
		case 2690:
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

function MoveItemTopToBottom(int Index, bool bAllItem)
{
	local int bottomIndex;
	local ItemInfo Info, bottomInfo;

	// End:0x415
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".TopList"), Index, Info))
	{
		// End:0xE2
		if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bAllItem), IsStackableItem(Info.ConsumeType)), __NFUN_155__(Info.ItemNum, 1)))
		{
			DialogSetID(111);
			DialogSetReservedInt(Info.ClassID);
			// End:0xAC
			if(__NFUN_132__(__NFUN_154__(int(m_shopType), int(2)), __NFUN_154__(int(m_shopType), int(1))))
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
			// End:0x215
			if(__NFUN_154__(int(m_shopType), int(2)))
			{
				bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".BottomList"), Info.ClassID);
				// End:0x1C5
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
				// End:0x37E
				if(__NFUN_154__(int(m_shopType), int(1)))
				{
					bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID(__NFUN_112__(m_WindowName, ".BottomList"), Info.ClassID);
					// End:0x2EB
					if(__NFUN_130__(__NFUN_153__(bottomIndex, 0), IsStackableItem(Info.ConsumeType)))
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem(__NFUN_112__(m_WindowName, ".BottomList"), bottomIndex, bottomInfo);
						__NFUN_161__(bottomInfo.ItemNum, Info.ItemNum);
						Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem(__NFUN_112__(m_WindowName, ".BottomList"), bottomIndex, bottomInfo);						
					}
					else
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__(m_WindowName, ".BottomList"), Info);
						Class'NWindow.UIAPI_INVENWEIGHT'.static.AddWeight(__NFUN_112__(m_WindowName, ".InvenWeight"), __NFUN_144__(Info.Weight, Info.ItemNum));
					}
					// End:0x37B
					if(bAllItem)
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem(__NFUN_112__(m_WindowName, ".TopList"), Index);
					}					
				}
				else
				{
					// End:0x3F5
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

function HandleOpenWindow(string param)
{
	super.HandleOpenWindow(param);
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitle(m_WindowName, 738);
	Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType(__NFUN_112__(m_WindowName, ".TopList"), "InventoryPrice1HideEnchant");
	return;
}

function HandleAddItem(string param)
{
	local ItemInfo Info;

	ParamToItemInfo(param, Info);
	Info.bShowCount = false;
	Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__(m_WindowName, ".TopList"), Info);
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
		RequestBuySeed(param);		
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
			RequestProcureCrop(param);			
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
	m_WindowName="ManorShopWnd"
}