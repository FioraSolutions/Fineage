class PrivateShopWnd extends UICommonAPI;

const DIALOG_TOP_TO_BOTTOM = 111;
const DIALOG_BOTTOM_TO_TOP = 222;
const DIALOG_ASK_PRICE = 333;
const DIALOG_CONFIRM_PRICE = 444;
const DIALOG_EDIT_SHOP_MESSAGE = 555;
const DIALOG_CONFIRM_PRICE_FINAL = 666;

enum PrivateShopType
{
	PT_None,                        // 0
	PT_Buy,                         // 1
	PT_Sell,                        // 2
	PT_BuyList,                     // 3
	PT_SellList                     // 4
};

var int m_merchantID;
var PrivateShopWnd.PrivateShopType m_type;
var int m_buyMaxCount;
var int m_sellMaxCount;
var bool m_bBulk;

function OnLoad()
{
	RegisterEvent(2120);
	RegisterEvent(2130);
	RegisterEvent(2070);
	RegisterEvent(1710);
	m_merchantID = 0;
	m_buyMaxCount = 0;
	m_sellMaxCount = 0;
	return;
}

function OnSendPacketWhenHiding()
{
	RequestQuit();
	return;
}

function OnHide()
{
	local DialogBox DialogBox;

	DialogBox = DialogBox(GetScript("DialogBox"));
	// End:0x48
	if(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("DialogBox"))
	{
		DialogBox.HandleCancel();
	}
	Clear();
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x1D
		case 2120:
			HandleOpenWindow(param);
			// End:0x60
			break;
		// End:0x33
		case 2130:
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

	// End:0x53
	if(__NFUN_122__(ControlName, "UpButton"))
	{
		Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedNum("PrivateShopWnd.BottomList");
		MoveItemBottomToTop(Index, false);		
	}
	else
	{
		// End:0xA5
		if(__NFUN_122__(ControlName, "DownButton"))
		{
			Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetSelectedNum("PrivateShopWnd.TopList");
			MoveItemTopToBottom(Index, false);			
		}
		else
		{
			// End:0xC3
			if(__NFUN_122__(ControlName, "OKButton"))
			{
				HandleOKButton(true);				
			}
			else
			{
				// End:0xF8
				if(__NFUN_122__(ControlName, "StopButton"))
				{
					RequestQuit();
					HideWindow("PrivateShopWnd");					
				}
				else
				{
					// End:0x183
					if(__NFUN_122__(ControlName, "MessageButton"))
					{
						DialogSetEditBoxMaxLength(29);
						DialogSetID(555);
						// End:0x149
						if(__NFUN_154__(int(m_type), int(4)))
						{
							DialogSetString(GetPrivateShopMessage("sell"));							
						}
						else
						{
							// End:0x16A
							if(__NFUN_154__(int(m_type), int(3)))
							{
								DialogSetString(GetPrivateShopMessage("buy"));
							}
						}
						DialogSetDefaultOK();
						DialogShow(2, GetSystemMessage(334));
					}
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

	Index = -1;
	// End:0x11C
	if(__NFUN_130__(__NFUN_122__(strID, "TopList"), __NFUN_122__(Info.DragSrcName, "BottomList")))
	{
		// End:0x9A
		if(__NFUN_132__(__NFUN_154__(int(m_type), int(1)), __NFUN_154__(int(m_type), int(4))))
		{
			Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.BottomList", Info.ServerID);			
		}
		else
		{
			// End:0xF6
			if(__NFUN_132__(__NFUN_154__(int(m_type), int(2)), __NFUN_154__(int(m_type), int(3))))
			{
				Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.BottomList", Info.ClassID);
			}
		}
		// End:0x119
		if(__NFUN_153__(Index, 0))
		{
			MoveItemBottomToTop(Index, __NFUN_151__(Info.AllItemCount, 0));
		}		
	}
	else
	{
		// End:0x224
		if(__NFUN_130__(__NFUN_122__(strID, "BottomList"), __NFUN_122__(Info.DragSrcName, "TopList")))
		{
			// End:0x1A8
			if(__NFUN_132__(__NFUN_154__(int(m_type), int(1)), __NFUN_154__(int(m_type), int(4))))
			{
				Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.TopList", Info.ServerID);				
			}
			else
			{
				// End:0x201
				if(__NFUN_132__(__NFUN_154__(int(m_type), int(2)), __NFUN_154__(int(m_type), int(3))))
				{
					Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.TopList", Info.ClassID);
				}
			}
			// End:0x224
			if(__NFUN_153__(Index, 0))
			{
				MoveItemTopToBottom(Index, __NFUN_151__(Info.AllItemCount, 0));
			}
		}
	}
	return;
}

function Clear()
{
	m_type = 0;
	m_merchantID = -1;
	m_bBulk = false;
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("PrivateShopWnd.TopList");
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("PrivateShopWnd.BottomList");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.PriceText", "0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("PrivateShopWnd.PriceText", "");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.AdenaText", "0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("PrivateShopWnd.AdenaText", "");
	return;
}

function RequestQuit()
{
	// End:0x1E
	if(__NFUN_154__(int(m_type), int(3)))
	{
		RequestQuitPrivateShop("buy");		
	}
	else
	{
		// End:0x3A
		if(__NFUN_154__(int(m_type), int(4)))
		{
			RequestQuitPrivateShop("sell");
		}
	}
	return;
}

function MoveItemTopToBottom(int Index, bool bAllItem)
{
	local ItemInfo Info, bottomInfo;
	local int Num, i, bottomIndex;

	// End:0x4B1
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", Index, Info))
	{
		// End:0xA2
		if(__NFUN_154__(int(m_type), int(4)))
		{
			DialogSetID(333);
			DialogSetReservedInt(Info.ServerID);
			DialogSetReservedInt3(int(bAllItem));
			DialogSetEditType("number");
			DialogSetParamInt(-1);
			DialogSetDefaultOK();
			DialogShow(6, GetSystemMessage(322));			
		}
		else
		{
			// End:0x102
			if(__NFUN_154__(int(m_type), int(3)))
			{
				DialogSetID(333);
				DialogSetReservedInt(Info.ClassID);
				DialogSetEditType("number");
				DialogSetParamInt(-1);
				DialogSetDefaultOK();
				DialogShow(6, GetSystemMessage(585));				
			}
			else
			{
				// End:0x4B1
				if(__NFUN_132__(__NFUN_154__(int(m_type), int(2)), __NFUN_154__(int(m_type), int(1))))
				{
					// End:0x146
					if(__NFUN_130__(__NFUN_154__(int(m_type), int(2)), Info.bDisabled))
					{
						return;
					}
					// End:0x244
					if(__NFUN_130__(__NFUN_154__(int(m_type), int(1)), m_bBulk))
					{
						Num = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("PrivateShopWnd.TopList");
						i = 0;
						J0x195:

						// End:0x20E [Loop If]
						if(__NFUN_150__(i, Num))
						{
							Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", i, Info);
							Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.BottomList", Info);
							__NFUN_163__(i);
							// [Loop Continue]
							goto J0x195;
						}
						Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("PrivateShopWnd.TopList");
						AdjustPrice();
						AdjustCount();						
					}
					else
					{
						// End:0x2FC
						if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bAllItem), IsStackableItem(Info.ConsumeType)), __NFUN_151__(Info.ItemNum, 1)))
						{
							DialogSetID(111);
							// End:0x2A1
							if(__NFUN_154__(int(m_type), int(2)))
							{
								DialogSetReservedInt(Info.ClassID);								
							}
							else
							{
								// End:0x2C1
								if(__NFUN_154__(int(m_type), int(1)))
								{
									DialogSetReservedInt(Info.ServerID);
								}
							}
							DialogSetParamInt(Info.ItemNum);
							DialogSetDefaultOK();
							DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name, ""));							
						}
						else
						{
							// End:0x349
							if(__NFUN_154__(int(m_type), int(1)))
							{
								bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.BottomList", Info.ServerID);								
							}
							else
							{
								// End:0x393
								if(__NFUN_154__(int(m_type), int(2)))
								{
									bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.BottomList", Info.ClassID);
								}
							}
							// End:0x434
							if(__NFUN_130__(__NFUN_153__(bottomIndex, 0), IsStackableItem(Info.ConsumeType)))
							{
								Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
								__NFUN_161__(bottomInfo.ItemNum, Info.ItemNum);
								Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);								
							}
							else
							{
								Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.BottomList", Info);
							}
							Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.TopList", Index);
							AdjustPrice();
							AdjustCount();
						}
					}
					// End:0x4B1
					if(__NFUN_154__(int(m_type), int(1)))
					{
						AdjustWeight();
					}
				}
			}
		}
	}
	return;
}

function MoveItemBottomToTop(int Index, bool bAllItem)
{
	local ItemInfo Info, topInfo;
	local int stringIndex, Num, i, topIndex;

	// End:0x435
	if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", Index, Info))
	{
		// End:0x13B
		if(__NFUN_130__(__NFUN_154__(int(m_type), int(1)), m_bBulk))
		{
			Num = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("PrivateShopWnd.BottomList");
			i = 0;
			J0x89:

			// End:0x102 [Loop If]
			if(__NFUN_150__(i, Num))
			{
				Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", i, Info);
				Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.TopList", Info);
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x89;
			}
			Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("PrivateShopWnd.BottomList");
			AdjustPrice();
			AdjustCount();			
		}
		else
		{
			// End:0x26A
			if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bAllItem), IsStackableItem(Info.ConsumeType)), __NFUN_151__(Info.ItemNum, 1)))
			{
				DialogSetID(222);
				// End:0x1AA
				if(__NFUN_132__(__NFUN_154__(int(m_type), int(1)), __NFUN_154__(int(m_type), int(4))))
				{
					DialogSetReservedInt(Info.ServerID);					
				}
				else
				{
					// End:0x1DC
					if(__NFUN_132__(__NFUN_154__(int(m_type), int(2)), __NFUN_154__(int(m_type), int(3))))
					{
						DialogSetReservedInt(Info.ClassID);
					}
				}
				DialogSetParamInt(Info.ItemNum);
				switch(m_type)
				{
					// End:0x203
					case 4:
						stringIndex = 72;
						// End:0x23C
						break;
					// End:0x216
					case 3:
						stringIndex = 571;
						// End:0x23C
						break;
					// End:0x226
					case 2:
						stringIndex = 72;
						// End:0x23C
						break;
					// End:0x236
					case 1:
						stringIndex = 72;
						// End:0x23C
						break;
					// End:0xFFFF
					default:
						// End:0x23C
						break;
						break;
				}
				DialogSetDefaultOK();
				DialogShow(6, MakeFullSystemMsg(GetSystemMessage(stringIndex), Info.Name, ""));				
			}
			else
			{
				Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.BottomList", Index);
				// End:0x413
				if(__NFUN_155__(int(m_type), int(3)))
				{
					// End:0x305
					if(__NFUN_132__(__NFUN_154__(int(m_type), int(1)), __NFUN_154__(int(m_type), int(4))))
					{
						topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.TopList", Info.ServerID);						
					}
					else
					{
						// End:0x34C
						if(__NFUN_154__(int(m_type), int(2)))
						{
							topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.TopList", Info.ClassID);
						}
					}
					// End:0x3E7
					if(__NFUN_130__(__NFUN_153__(topIndex, 0), IsStackableItem(Info.ConsumeType)))
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
						__NFUN_161__(topInfo.ItemNum, Info.ItemNum);
						Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.TopList", topIndex, topInfo);						
					}
					else
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.TopList", Info);
					}
				}
				AdjustPrice();
				AdjustCount();
			}
		}
		// End:0x435
		if(__NFUN_154__(int(m_type), int(1)))
		{
			AdjustWeight();
		}
	}
	return;
}

function HandleDialogOK()
{
	local int Id, inputNum, itemID, bottomIndex, topIndex, i,
		allItem;

	local ItemInfo bottomInfo, topInfo;

	// End:0x17CC
	if(DialogIsMine())
	{
		Id = DialogGetID();
		inputNum = int(DialogGetString());
		itemID = DialogGetReservedInt();
		// End:0xAA7
		if(__NFUN_154__(int(m_type), int(4)))
		{
			// End:0x2AB
			if(__NFUN_130__(__NFUN_154__(Id, 111), __NFUN_151__(inputNum, 0)))
			{
				topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.TopList", itemID);
				// End:0x29C
				if(__NFUN_153__(topIndex, 0))
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
					bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.BottomList", itemID);
					Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
					// End:0x1B4
					if(__NFUN_130__(__NFUN_153__(bottomIndex, 0), IsStackableItem(bottomInfo.ConsumeType)))
					{
						bottomInfo.Price = DialogGetReservedInt2();
						__NFUN_161__(bottomInfo.ItemNum, __NFUN_249__(inputNum, topInfo.ItemNum));
						Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);						
					}
					else
					{
						bottomInfo = topInfo;
						bottomInfo.ItemNum = __NFUN_249__(inputNum, topInfo.ItemNum);
						bottomInfo.Price = DialogGetReservedInt2();
						Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.BottomList", bottomInfo);
					}
					__NFUN_162__(topInfo.ItemNum, inputNum);
					// End:0x26B
					if(__NFUN_152__(topInfo.ItemNum, 0))
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.TopList", topIndex);						
					}
					else
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.TopList", topIndex, topInfo);
					}
				}
				AdjustPrice();
				AdjustCount();				
			}
			else
			{
				// End:0x4F5
				if(__NFUN_130__(__NFUN_154__(Id, 222), __NFUN_151__(inputNum, 0)))
				{
					bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.BottomList", itemID);
					// End:0x4E6
					if(__NFUN_153__(bottomIndex, 0))
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
						topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.TopList", itemID);
						// End:0x40C
						if(__NFUN_130__(__NFUN_153__(topIndex, 0), IsStackableItem(bottomInfo.ConsumeType)))
						{
							Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
							__NFUN_161__(topInfo.ItemNum, __NFUN_249__(inputNum, bottomInfo.ItemNum));
							Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.TopList", topIndex, topInfo);							
						}
						else
						{
							topInfo = bottomInfo;
							topInfo.ItemNum = __NFUN_249__(inputNum, bottomInfo.ItemNum);
							Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.TopList", topInfo);
						}
						__NFUN_162__(bottomInfo.ItemNum, inputNum);
						// End:0x4B7
						if(__NFUN_151__(bottomInfo.ItemNum, 0))
						{
							Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);							
						}
						else
						{
							Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.BottomList", bottomIndex);
						}
					}
					AdjustPrice();
					AdjustCount();					
				}
				else
				{
					// End:0x77C
					if(__NFUN_154__(Id, 444))
					{
						topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.TopList", itemID);
						// End:0x779
						if(__NFUN_153__(topIndex, 0))
						{
							allItem = DialogGetReservedInt3();
							Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
							// End:0x60F
							if(__NFUN_130__(__NFUN_130__(__NFUN_154__(allItem, 0), IsStackableItem(topInfo.ConsumeType)), __NFUN_155__(topInfo.ItemNum, 1)))
							{
								DialogSetID(111);
								// End:0x5D4
								if(__NFUN_154__(topInfo.ItemNum, 0))
								{
									topInfo.ItemNum = 1;
								}
								DialogSetParamInt(topInfo.ItemNum);
								DialogSetDefaultOK();
								DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), topInfo.Name, ""));								
							}
							else
							{
								// End:0x626
								if(__NFUN_154__(allItem, 0))
								{
									topInfo.ItemNum = 1;
								}
								topInfo.Price = DialogGetReservedInt2();
								bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.BottomList", topInfo.ServerID);
								// End:0x712
								if(__NFUN_130__(__NFUN_153__(bottomIndex, 0), IsStackableItem(topInfo.ConsumeType)))
								{
									Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
									__NFUN_161__(topInfo.ItemNum, bottomInfo.ItemNum);
									Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.BottomList", bottomIndex, topInfo);									
								}
								else
								{
									Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.BottomList", topInfo);
								}
								Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.TopList", topIndex);
								AdjustPrice();
								AdjustCount();
							}
						}						
					}
					else
					{
						// End:0xA83
						if(__NFUN_130__(__NFUN_154__(Id, 333), __NFUN_151__(inputNum, 0)))
						{
							topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.TopList", itemID);
							// End:0xA80
							if(__NFUN_153__(topIndex, 0))
							{
								Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
								// End:0x82B
								if(__NFUN_153__(inputNum, 2000000000))
								{
									DialogShow(5, GetSystemMessage(1369));									
								}
								else
								{
									// End:0x882
									if(__NFUN_129__(IsProperPrice(topInfo, inputNum)))
									{
										DialogSetID(444);
										DialogSetReservedInt(topInfo.ServerID);
										DialogSetReservedInt2(inputNum);
										DialogSetDefaultOK();
										DialogShow(4, GetSystemMessage(569));										
									}
									else
									{
										allItem = DialogGetReservedInt3();
										// End:0x917
										if(__NFUN_130__(__NFUN_154__(allItem, 0), IsStackableItem(topInfo.ConsumeType)))
										{
											DialogSetID(111);
											DialogSetReservedInt(topInfo.ServerID);
											DialogSetReservedInt2(inputNum);
											DialogSetReservedInt3(allItem);
											DialogSetParamInt(topInfo.ItemNum);
											DialogSetDefaultOK();
											DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), topInfo.Name, ""));											
										}
										else
										{
											// End:0x92E
											if(__NFUN_154__(allItem, 0))
											{
												topInfo.ItemNum = 1;
											}
											topInfo.Price = inputNum;
											bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.BottomList", topInfo.ServerID);
											// End:0xA19
											if(__NFUN_130__(__NFUN_153__(bottomIndex, 0), IsStackableItem(topInfo.ConsumeType)))
											{
												Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
												__NFUN_161__(topInfo.ItemNum, bottomInfo.ItemNum);
												Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.BottomList", bottomIndex, topInfo);												
											}
											else
											{
												Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.BottomList", topInfo);
											}
											Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.TopList", topIndex);
											AdjustPrice();
											AdjustCount();
										}
									}
								}
							}							
						}
						else
						{
							// End:0xAA4
							if(__NFUN_154__(Id, 555))
							{
								SetPrivateShopMessage("sell", DialogGetString());
							}
						}
					}
				}
			}			
		}
		else
		{
			// End:0x115D
			if(__NFUN_154__(int(m_type), int(3)))
			{
				// End:0xDDD
				if(__NFUN_130__(__NFUN_154__(Id, 111), __NFUN_151__(inputNum, 0)))
				{
					topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.TopList", itemID);
					// End:0xDCE
					if(__NFUN_153__(topIndex, 0))
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
						bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.BottomList", itemID);
						Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
						// End:0xC1F
						if(__NFUN_130__(__NFUN_153__(bottomIndex, 0), IsStackableItem(bottomInfo.ConsumeType)))
						{
							bottomInfo.Price = DialogGetReservedInt2();
							__NFUN_161__(bottomInfo.ItemNum, inputNum);
							Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
							return;
						}
						// End:0xCE6
						if(__NFUN_153__(bottomIndex, 0))
						{
							i = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("PrivateShopWnd.BottomList");
							J0xC5A:

							// End:0xCE6 [Loop If]
							if(__NFUN_153__(i, 0))
							{
								Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", i, bottomInfo);
								// End:0xCDC
								if(__NFUN_154__(bottomInfo.ClassID, itemID))
								{
									Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.BottomList", i);
								}
								__NFUN_164__(i);
								// [Loop Continue]
								goto J0xC5A;
							}
						}
						// End:0xD57
						if(IsStackableItem(topInfo.ConsumeType))
						{
							bottomInfo = topInfo;
							bottomInfo.ItemNum = inputNum;
							bottomInfo.Price = DialogGetReservedInt2();
							Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.BottomList", bottomInfo);							
						}
						else
						{
							bottomInfo = topInfo;
							bottomInfo.ItemNum = 1;
							bottomInfo.Price = DialogGetReservedInt2();
							i = 0;
							J0xD86:

							// End:0xDCE [Loop If]
							if(__NFUN_150__(i, inputNum))
							{
								Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.BottomList", bottomInfo);
								__NFUN_163__(i);
								// [Loop Continue]
								goto J0xD86;
							}
						}
					}
					AdjustPrice();
					AdjustCount();					
				}
				else
				{
					// End:0xEF4
					if(__NFUN_130__(__NFUN_154__(Id, 222), __NFUN_151__(inputNum, 0)))
					{
						bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.BottomList", itemID);
						// End:0xEF1
						if(__NFUN_153__(bottomIndex, 0))
						{
							Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
							__NFUN_162__(bottomInfo.ItemNum, inputNum);
							// End:0xEC2
							if(__NFUN_151__(bottomInfo.ItemNum, 0))
							{
								Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);								
							}
							else
							{
								Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.BottomList", bottomIndex);
							}
						}						
					}
					else
					{
						// End:0xFD3
						if(__NFUN_154__(Id, 444))
						{
							topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.TopList", itemID);
							// End:0xFC4
							if(__NFUN_153__(topIndex, 0))
							{
								Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
								DialogSetID(111);
								DialogSetReservedInt(topInfo.ClassID);
								DialogSetParamInt(topInfo.ItemNum);
								DialogSetDefaultOK();
								DialogShow(6, MakeFullSystemMsg(GetSystemMessage(570), topInfo.Name, ""));
							}
							AdjustPrice();
							AdjustCount();							
						}
						else
						{
							// End:0x113A
							if(__NFUN_130__(__NFUN_154__(Id, 333), __NFUN_151__(inputNum, 0)))
							{
								topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.TopList", itemID);
								// End:0x1137
								if(__NFUN_153__(topIndex, 0))
								{
									Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
									// End:0x1082
									if(__NFUN_153__(inputNum, 2000000000))
									{
										DialogShow(5, GetSystemMessage(1369));										
									}
									else
									{
										// End:0x10D9
										if(__NFUN_129__(IsProperPrice(topInfo, inputNum)))
										{
											DialogSetID(444);
											DialogSetReservedInt(topInfo.ClassID);
											DialogSetReservedInt2(inputNum);
											DialogSetDefaultOK();
											DialogShow(4, GetSystemMessage(569));											
										}
										else
										{
											DialogSetID(111);
											DialogSetReservedInt(topInfo.ClassID);
											DialogSetReservedInt2(inputNum);
											DialogSetParamInt(topInfo.ItemNum);
											DialogSetDefaultOK();
											DialogShow(6, MakeFullSystemMsg(GetSystemMessage(570), topInfo.Name, ""));
										}
									}
								}								
							}
							else
							{
								// End:0x115A
								if(__NFUN_154__(Id, 555))
								{
									SetPrivateShopMessage("buy", DialogGetString());
								}
							}
						}
					}
				}				
			}
			else
			{
				// End:0x17CC
				if(__NFUN_132__(__NFUN_154__(int(m_type), int(1)), __NFUN_154__(int(m_type), int(2))))
				{
					// End:0x14A8
					if(__NFUN_130__(__NFUN_154__(Id, 111), __NFUN_151__(inputNum, 0)))
					{
						topIndex = -1;
						// End:0x11E8
						if(__NFUN_154__(int(m_type), int(1)))
						{
							topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.TopList", itemID);							
						}
						else
						{
							// End:0x122A
							if(__NFUN_154__(int(m_type), int(2)))
							{
								topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.TopList", itemID);
							}
						}
						// End:0x1499
						if(__NFUN_153__(topIndex, 0))
						{
							Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
							// End:0x12A2
							if(__NFUN_130__(__NFUN_154__(int(m_type), int(2)), __NFUN_150__(topInfo.Reserved, inputNum)))
							{
								DialogShow(5, GetSystemMessage(1036));								
							}
							else
							{
								// End:0x12EA
								if(__NFUN_154__(int(m_type), int(1)))
								{
									bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.BottomList", itemID);									
								}
								else
								{
									// End:0x132F
									if(__NFUN_154__(int(m_type), int(2)))
									{
										bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.BottomList", itemID);
									}
								}
								Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
								// End:0x13C2
								if(__NFUN_153__(bottomIndex, 0))
								{
									__NFUN_161__(bottomInfo.ItemNum, __NFUN_249__(inputNum, topInfo.ItemNum));
									Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);									
								}
								else
								{
									bottomInfo = topInfo;
									bottomInfo.ItemNum = __NFUN_249__(inputNum, topInfo.ItemNum);
									Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.BottomList", bottomInfo);
								}
								__NFUN_162__(topInfo.ItemNum, inputNum);
								// End:0x1468
								if(__NFUN_152__(topInfo.ItemNum, 0))
								{
									Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.TopList", topIndex);									
								}
								else
								{
									Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.TopList", topIndex, topInfo);
								}
							}
						}
						AdjustPrice();
						AdjustCount();						
					}
					else
					{
						// End:0x17A0
						if(__NFUN_130__(__NFUN_154__(Id, 222), __NFUN_151__(inputNum, 0)))
						{
							bottomIndex = -1;
							// End:0x1514
							if(__NFUN_154__(int(m_type), int(1)))
							{
								bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.BottomList", itemID);								
							}
							else
							{
								// End:0x1559
								if(__NFUN_154__(int(m_type), int(2)))
								{
									bottomIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.BottomList", itemID);
								}
							}
							// End:0x1791
							if(__NFUN_153__(bottomIndex, 0))
							{
								Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);
								topIndex = -1;
								// End:0x15E8
								if(__NFUN_154__(int(m_type), int(1)))
								{
									topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithServerID("PrivateShopWnd.TopList", itemID);									
								}
								else
								{
									// End:0x162A
									if(__NFUN_154__(int(m_type), int(2)))
									{
										topIndex = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PrivateShopWnd.TopList", itemID);
									}
								}
								// End:0x16B7
								if(__NFUN_153__(topIndex, 0))
								{
									Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.TopList", topIndex, topInfo);
									__NFUN_161__(topInfo.ItemNum, __NFUN_249__(inputNum, bottomInfo.ItemNum));
									Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.TopList", topIndex, topInfo);									
								}
								else
								{
									topInfo = bottomInfo;
									topInfo.ItemNum = __NFUN_249__(inputNum, bottomInfo.ItemNum);
									Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.TopList", topInfo);
								}
								__NFUN_162__(bottomInfo.ItemNum, inputNum);
								// End:0x1762
								if(__NFUN_151__(bottomInfo.ItemNum, 0))
								{
									Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PrivateShopWnd.BottomList", bottomIndex, bottomInfo);									
								}
								else
								{
									Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PrivateShopWnd.BottomList", bottomIndex);
								}
							}
							AdjustPrice();
							AdjustCount();							
						}
						else
						{
							// End:0x17B6
							if(__NFUN_154__(Id, 666))
							{
								HandleOKButton(false);
							}
						}
					}
					// End:0x17CC
					if(__NFUN_154__(int(m_type), int(1)))
					{
						AdjustWeight();
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
	local int Adena, bulk;
	local string adenaString;
	local UserInfo User;
	local WindowHandle m_inventoryWnd;

	m_inventoryWnd = GetHandle("InventoryWnd");
	Clear();
	ParseString(param, "type", Type);
	ParseInt(param, "adena", Adena);
	ParseInt(param, "userID", m_merchantID);
	ParseInt(param, "bulk", bulk);
	// End:0x91
	if(__NFUN_151__(bulk, 0))
	{
		m_bBulk = true;		
	}
	else
	{
		m_bBulk = false;
	}
	switch(Type)
	{
		// End:0xB3
		case "buy":
			m_type = 1;
			// End:0xFC
			break;
		// End:0xC7
		case "sell":
			m_type = 2;
			// End:0xFC
			break;
		// End:0xDE
		case "buyList":
			m_type = 3;
			// End:0xFC
			break;
		// End:0xF6
		case "sellList":
			m_type = 4;
			// End:0xFC
			break;
		// End:0xFFFF
		default:
			// End:0xFC
			break;
			break;
	}
	adenaString = MakeCostString(string(Adena));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.AdenaText", adenaString);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("PrivateShopWnd.AdenaText", ConvertNumToText(string(Adena)));
	// End:0x194
	if(m_inventoryWnd.IsShowWindow())
	{
		m_inventoryWnd.HideWindow();
	}
	ShowWindow("PrivateShopWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PrivateShopWnd");
	// End:0x3FA
	if(__NFUN_154__(int(m_type), int(3)))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType("PrivateShopWnd.TopList", "Inventory");
		Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType("PrivateShopWnd.BottomList", "InventoryPrice1");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.TopText", GetSystemString(1));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.BottomText", GetSystemString(502));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.PriceConstText", GetSystemString(142));
		Class'NWindow.UIAPI_BUTTON'.static.SetButtonName("PrivateShopWnd.OKButton", 428);
		ShowWindow("PrivateShopWnd.BottomCountText");
		ShowWindow("PrivateShopWnd.StopButton");
		ShowWindow("PrivateShopWnd.MessageButton");
		ShowWindow("PrivateShopWnd.OKButton");
		HideWindow("PrivateShopWnd.CheckBulk");
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("PrivateShopWnd", __NFUN_112__(__NFUN_112__(__NFUN_112__(GetSystemString(498), "("), GetSystemString(1434)), ")"));		
	}
	else
	{
		// End:0x68A
		if(__NFUN_154__(int(m_type), int(4)))
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType("PrivateShopWnd.TopList", "Inventory");
			Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType("PrivateShopWnd.BottomList", "InventoryPrice1");
			// End:0x4AF
			if(__NFUN_151__(bulk, 0))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("PrivateShopWnd.CheckBulk", true);				
			}
			else
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("PrivateShopWnd.CheckBulk", false);
			}
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.TopText", GetSystemString(1));
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.BottomText", GetSystemString(137));
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.PriceConstText", GetSystemString(143));
			Class'NWindow.UIAPI_BUTTON'.static.SetButtonName("PrivateShopWnd.OKButton", 428);
			ShowWindow("PrivateShopWnd.BottomCountText");
			ShowWindow("PrivateShopWnd.StopButton");
			ShowWindow("PrivateShopWnd.MessageButton");
			ShowWindow("PrivateShopWnd.OKButton");
			ShowWindow("PrivateShopWnd.CheckBulk");
			Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("PrivateShopWnd", __NFUN_112__(__NFUN_112__(__NFUN_112__(GetSystemString(498), "("), GetSystemString(1157)), ")"));			
		}
		else
		{
			// End:0x933
			if(__NFUN_154__(int(m_type), int(1)))
			{
				Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType("PrivateShopWnd.TopList", "InventoryPrice1");
				Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType("PrivateShopWnd.BottomList", "Inventory");
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.TopText", GetSystemString(137));
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.BottomText", GetSystemString(139));
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.PriceConstText", GetSystemString(142));
				Class'NWindow.UIAPI_BUTTON'.static.SetButtonName("PrivateShopWnd.OKButton", 140);
				HideWindow("PrivateShopWnd.BottomCountText");
				HideWindow("PrivateShopWnd.StopButton");
				HideWindow("PrivateShopWnd.MessageButton");
				ShowWindow("PrivateShopWnd.OKButton");
				HideWindow("PrivateShopWnd.CheckBulk");
				GetUserInfo(m_merchantID, User);
				// End:0x8E0
				if(__NFUN_151__(bulk, 0))
				{
					Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("PrivateShopWnd", __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetSystemString(498), "("), GetSystemString(1198)), ") - "), User.Name));					
				}
				else
				{
					Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("PrivateShopWnd", __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetSystemString(498), "("), GetSystemString(1157)), ") - "), User.Name));
				}				
			}
			else
			{
				// End:0xB89
				if(__NFUN_154__(int(m_type), int(2)))
				{
					Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType("PrivateShopWnd.TopList", "InventoryPrice2PrivateShop");
					Class'NWindow.UIAPI_WINDOW'.static.SetTooltipType("PrivateShopWnd.BottomList", "Inventory");
					Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.TopText", GetSystemString(503));
					Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.BottomText", GetSystemString(137));
					Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.PriceConstText", GetSystemString(143));
					Class'NWindow.UIAPI_BUTTON'.static.SetButtonName("PrivateShopWnd.OKButton", 140);
					HideWindow("PrivateShopWnd.BottomCountText");
					HideWindow("PrivateShopWnd.StopButton");
					HideWindow("PrivateShopWnd.MessageButton");
					ShowWindow("PrivateShopWnd.OKButton");
					HideWindow("PrivateShopWnd.CheckBulk");
					GetUserInfo(m_merchantID, User);
					Class'NWindow.UIAPI_WINDOW'.static.SetWindowTitleByText("PrivateShopWnd", __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetSystemString(498), "("), GetSystemString(1434)), ") - "), User.Name));
				}
			}
		}
	}
	return;
}

function HandleAddItem(string param)
{
	local ItemInfo Info;
	local string Target;

	ParseString(param, "target", Target);
	ParamToItemInfo(param, Info);
	// End:0x99
	if(__NFUN_122__(Target, "topList"))
	{
		// End:0x6A
		if(__NFUN_130__(__NFUN_154__(int(m_type), int(2)), __NFUN_154__(Info.ItemNum, 0)))
		{
			Info.bDisabled = true;
		}
		Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.TopList", Info);		
	}
	else
	{
		// End:0xDE
		if(__NFUN_122__(Target, "bottomList"))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PrivateShopWnd.BottomList", Info);
		}
	}
	AdjustPrice();
	AdjustCount();
	return;
}

function AdjustPrice()
{
	local string Adena;
	local int Count;
	local INT64 Price, addPrice64;
	local ItemInfo Info;

	Count = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("PrivateShopWnd.BottomList");
	Price = Int2Int64(0);
	addPrice64 = Int2Int64(0);
	J0x4A:

	// End:0xCC [Loop If]
	if(__NFUN_151__(Count, 0))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", __NFUN_147__(Count, 1), Info);
		addPrice64 = Int64Mul(Info.Price, Info.ItemNum);
		Price = Int64Add(Price, addPrice64);
		__NFUN_164__(Count);
		// [Loop Continue]
		goto J0x4A;
	}
	// End:0xFB
	if(__NFUN_132__(__NFUN_150__(Price.nLeft, 0), __NFUN_150__(Price.nRight, 0)))
	{
		Price = Int2Int64(0);
	}
	Adena = MakeCostStringInt64(Price);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.PriceText", Adena);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTooltipString("PrivateShopWnd.PriceText", ConvertNumToText(MakeCostStringInt64(Price)));
	return;
}

function AdjustCount()
{
	local int Num, maxNum;

	// End:0x9C
	if(__NFUN_154__(int(m_type), int(4)))
	{
		maxNum = m_sellMaxCount;
		Num = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("PrivateShopWnd.BottomList");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.BottomCountText", __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", string(Num)), "/"), string(maxNum)), ")"));		
	}
	else
	{
		// End:0x135
		if(__NFUN_154__(int(m_type), int(3)))
		{
			maxNum = m_buyMaxCount;
			Num = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("PrivateShopWnd.BottomList");
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PrivateShopWnd.BottomCountText", __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", string(Num)), "/"), string(maxNum)), ")"));
		}
	}
	return;
}

function AdjustWeight()
{
	local int Count, Weight;
	local ItemInfo Info;

	Class'NWindow.UIAPI_INVENWEIGHT'.static.ZeroWeight("PrivateShopWnd.InvenWeight");
	Count = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("PrivateShopWnd.BottomList");
	Weight = 0;
	J0x62:

	// End:0xCB [Loop If]
	if(__NFUN_151__(Count, 0))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", __NFUN_147__(Count, 1), Info);
		__NFUN_161__(Weight, __NFUN_144__(Info.Weight, Info.ItemNum));
		__NFUN_164__(Count);
		// [Loop Continue]
		goto J0x62;
	}
	Class'NWindow.UIAPI_INVENWEIGHT'.static.AddWeight("PrivateShopWnd.InvenWeight", Weight);
	return;
}

function HandleOKButton(bool bPriceCheck)
{
	local string param;
	local int ItemCount, itemIndex;
	local ItemInfo ItemInfo;

	ItemCount = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("PrivateShopWnd.BottomList");
	// End:0x192
	if(__NFUN_154__(int(m_type), int(4)))
	{
		// End:0x83
		if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("PrivateShopWnd.CheckBulk"))
		{
			ParamAdd(param, "bulk", "1");			
		}
		else
		{
			ParamAdd(param, "bulk", "0");
		}
		ParamAdd(param, "num", string(ItemCount));
		itemIndex = 0;
		J0xB5:

		// End:0x17A [Loop If]
		if(__NFUN_150__(itemIndex, ItemCount))
		{
			Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", itemIndex, ItemInfo);
			ParamAdd(param, __NFUN_112__("serverID", string(itemIndex)), string(ItemInfo.ServerID));
			ParamAdd(param, __NFUN_112__("count", string(itemIndex)), string(ItemInfo.ItemNum));
			ParamAdd(param, __NFUN_112__("price", string(itemIndex)), string(ItemInfo.Price));
			__NFUN_163__(itemIndex);
			// [Loop Continue]
			goto J0xB5;
		}
		SendPrivateShopList("sellList", param);		
	}
	else
	{
		// End:0x31B
		if(__NFUN_154__(int(m_type), int(1)))
		{
			ParamAdd(param, "merchantID", string(m_merchantID));
			ParamAdd(param, "num", string(ItemCount));
			itemIndex = 0;
			J0x1DE:

			// End:0x2CB [Loop If]
			if(__NFUN_150__(itemIndex, ItemCount))
			{
				Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", itemIndex, ItemInfo);
				// End:0x249
				if(__NFUN_130__(bPriceCheck, __NFUN_129__(IsProperPrice(ItemInfo, ItemInfo.Price))))
				{
					// [Explicit Break]
					goto J0x2CB;
				}
				ParamAdd(param, __NFUN_112__("serverID", string(itemIndex)), string(ItemInfo.ServerID));
				ParamAdd(param, __NFUN_112__("count", string(itemIndex)), string(ItemInfo.ItemNum));
				ParamAdd(param, __NFUN_112__("price", string(itemIndex)), string(ItemInfo.Price));
				__NFUN_163__(itemIndex);
				// [Loop Continue]
				goto J0x1DE;
			}
			J0x2CB:

			// End:0x308
			if(__NFUN_130__(bPriceCheck, __NFUN_150__(itemIndex, ItemCount)))
			{
				DialogSetID(666);
				DialogShow(4, GetSystemMessage(569));
				return;				
			}
			else
			{
				SendPrivateShopList("buy", param);
			}			
		}
		else
		{
			// End:0x478
			if(__NFUN_154__(int(m_type), int(3)))
			{
				ParamAdd(param, "num", string(ItemCount));
				itemIndex = 0;
				J0x349:

				// End:0x461 [Loop If]
				if(__NFUN_150__(itemIndex, ItemCount))
				{
					Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", itemIndex, ItemInfo);
					ParamAdd(param, __NFUN_112__("classID", string(itemIndex)), string(ItemInfo.ClassID));
					ParamAdd(param, __NFUN_112__("enchanted", string(itemIndex)), string(ItemInfo.Enchanted));
					ParamAdd(param, __NFUN_112__("damaged", string(itemIndex)), string(ItemInfo.Damaged));
					ParamAdd(param, __NFUN_112__("count", string(itemIndex)), string(ItemInfo.ItemNum));
					ParamAdd(param, __NFUN_112__("price", string(itemIndex)), string(ItemInfo.Price));
					__NFUN_163__(itemIndex);
					// [Loop Continue]
					goto J0x349;
				}
				SendPrivateShopList("buyList", param);				
			}
			else
			{
				// End:0x67C
				if(__NFUN_154__(int(m_type), int(2)))
				{
					ParamAdd(param, "merchantID", string(m_merchantID));
					ParamAdd(param, "num", string(ItemCount));
					itemIndex = 0;
					J0x4C4:

					// End:0x62E [Loop If]
					if(__NFUN_150__(itemIndex, ItemCount))
					{
						Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PrivateShopWnd.BottomList", itemIndex, ItemInfo);
						// End:0x52F
						if(__NFUN_130__(bPriceCheck, __NFUN_129__(IsProperPrice(ItemInfo, ItemInfo.Price))))
						{
							// [Explicit Break]
							goto J0x62E;
						}
						ParamAdd(param, __NFUN_112__("serverID", string(itemIndex)), string(ItemInfo.ServerID));
						ParamAdd(param, __NFUN_112__("classID", string(itemIndex)), string(ItemInfo.ClassID));
						ParamAdd(param, __NFUN_112__("enchanted", string(itemIndex)), string(ItemInfo.Enchanted));
						ParamAdd(param, __NFUN_112__("damaged", string(itemIndex)), string(ItemInfo.Damaged));
						ParamAdd(param, __NFUN_112__("count", string(itemIndex)), string(ItemInfo.ItemNum));
						ParamAdd(param, __NFUN_112__("price", string(itemIndex)), string(ItemInfo.Price));
						__NFUN_163__(itemIndex);
						// [Loop Continue]
						goto J0x4C4;
					}
					J0x62E:

					// End:0x66B
					if(__NFUN_130__(bPriceCheck, __NFUN_150__(itemIndex, ItemCount)))
					{
						DialogSetID(666);
						DialogShow(4, GetSystemMessage(569));
						return;						
					}
					else
					{
						SendPrivateShopList("sell", param);
					}
				}
			}
		}
	}
	HideWindow("PrivateShopWnd");
	Clear();
	return;
}

function HandleSetMaxCount(string param)
{
	ParseInt(param, "privateShopSell", m_sellMaxCount);
	ParseInt(param, "privateShopBuy", m_buyMaxCount);
	return;
}

function bool IsProperPrice(out ItemInfo Info, int Price)
{
	// End:0x46
	if(__NFUN_130__(__NFUN_151__(Info.DefaultPrice, 0), __NFUN_132__(__NFUN_152__(Price, __NFUN_145__(Info.DefaultPrice, 5)), __NFUN_153__(Price, __NFUN_144__(Info.DefaultPrice, 5)))))
	{
		return false;
	}
	return true;
	return;
}
