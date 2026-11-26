class InventoryWnd extends UICommonAPI;

const DIALOG_USE_RECIPE = 1111;
const DIALOG_POPUP = 2222;
const DIALOG_DROPITEM = 3333;
const DIALOG_DROPITEM_ASKCOUNT = 4444;
const DIALOG_DROPITEM_ALL = 5555;
const DIALOG_DESTROYITEM = 6666;
const DIALOG_DESTROYITEM_ALL = 7777;
const DIALOG_DESTROYITEM_ASKCOUNT = 8888;
const DIALOG_CRYSTALLIZE = 9999;
const DIALOG_DROPITEM_PETASKCOUNT = 10000;
const EQUIPITEM_Underwear = 0;
const EQUIPITEM_Head = 1;
const EQUIPITEM_Hair = 2;
const EQUIPITEM_Hair2 = 3;
const EQUIPITEM_Neck = 4;
const EQUIPITEM_RHand = 5;
const EQUIPITEM_Chest = 6;
const EQUIPITEM_LHand = 7;
const EQUIPITEM_REar = 8;
const EQUIPITEM_LEar = 9;
const EQUIPITEM_Gloves = 10;
const EQUIPITEM_Legs = 11;
const EQUIPITEM_Feet = 12;
const EQUIPITEM_RFinger = 13;
const EQUIPITEM_LFinger = 14;
const EQUIPITEM_Max = 15;

var string m_WindowName;
var ItemWindowHandle m_invenItem;
var ItemWindowHandle m_questItem;
var ItemWindowHandle m_equipItem[15];
var ItemWindowHandle m_hHennaItemWindow;
var TextBoxHandle m_hAdenaTextBox;
var array<int> m_itemOrder;
var Vector m_clickLocation;
var array<ItemInfo> m_EarItemList;
var array<ItemInfo> m_FingerItemLIst;

function OnLoad()
{
	RegisterEvent(2570);
	RegisterEvent(2580);
	RegisterEvent(2590);
	RegisterEvent(2600);
	RegisterEvent(2610);
	RegisterEvent(2620);
	RegisterEvent(2630);
	RegisterEvent(2631);
	RegisterEvent(260);
	RegisterEvent(180);
	RegisterEvent(1710);
	InitHandle();
	return;
}

function InitHandle()
{
	m_invenItem = ItemWindowHandle(GetHandle(__NFUN_112__(m_WindowName, ".InventoryItem")));
	m_questItem = ItemWindowHandle(GetHandle(__NFUN_112__(m_WindowName, ".QuestItem")));
	m_hAdenaTextBox = TextBoxHandle(GetHandle(__NFUN_112__(m_WindowName, ".AdenaText")));
	m_equipItem[0] = ItemWindowHandle(GetHandle("EquipItem_Underwear"));
	m_equipItem[1] = ItemWindowHandle(GetHandle("EquipItem_Head"));
	m_equipItem[2] = ItemWindowHandle(GetHandle("EquipItem_Hair"));
	m_equipItem[3] = ItemWindowHandle(GetHandle("EquipItem_Hair2"));
	m_equipItem[4] = ItemWindowHandle(GetHandle("EquipItem_Neck"));
	m_equipItem[5] = ItemWindowHandle(GetHandle("EquipItem_RHand"));
	m_equipItem[6] = ItemWindowHandle(GetHandle("EquipItem_Chest"));
	m_equipItem[7] = ItemWindowHandle(GetHandle("EquipItem_LHand"));
	m_equipItem[8] = ItemWindowHandle(GetHandle("EquipItem_REar"));
	m_equipItem[9] = ItemWindowHandle(GetHandle("EquipItem_LEar"));
	m_equipItem[10] = ItemWindowHandle(GetHandle("EquipItem_Gloves"));
	m_equipItem[11] = ItemWindowHandle(GetHandle("EquipItem_Legs"));
	m_equipItem[12] = ItemWindowHandle(GetHandle("EquipItem_Feet"));
	m_equipItem[13] = ItemWindowHandle(GetHandle("EquipItem_RFinger"));
	m_equipItem[14] = ItemWindowHandle(GetHandle("EquipItem_LFinger"));
	m_equipItem[7].SetDisableTex("L2UI.InventoryWnd.Icon_dualcap");
	m_equipItem[1].SetDisableTex("L2UI.InventoryWnd.Icon_dualcap");
	m_equipItem[10].SetDisableTex("L2UI.InventoryWnd.Icon_dualcap");
	m_equipItem[11].SetDisableTex("L2UI.InventoryWnd.Icon_dualcap");
	m_equipItem[12].SetDisableTex("L2UI.InventoryWnd.Icon_dualcap");
	m_equipItem[3].SetDisableTex("L2UI.InventoryWnd.Icon_dualcap");
	m_hHennaItemWindow = ItemWindowHandle(GetHandle("HennaItem"));
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x18
		case 2570:
			HandleClear();
			// End:0xD9
			break;
		// End:0x29
		case 2580:
			HandleOpenWindow();
			// End:0xD9
			break;
		// End:0x3A
		case 2590:
			HandleHideWindow();
			// End:0xD9
			break;
		// End:0x50
		case 2600:
			HandleAddItem(param);
			// End:0xD9
			break;
		// End:0x66
		case 2610:
			HandleUpdateItem(param);
			// End:0xD9
			break;
		// End:0x77
		case 2620:
			HandleItemListEnd();
			// End:0xD9
			break;
		// End:0x8D
		case 2630:
			HandleAddHennaInfo(param);
			// End:0xD9
			break;
		// End:0xA3
		case 260:
			HandleUpdateHennaInfo(param);
			// End:0xD9
			break;
		// End:0xB4
		case 2631:
			HandleToggleWindow();
			// End:0xD9
			break;
		// End:0xC5
		case 1710:
			HandleDialogOK();
			// End:0xD9
			break;
		// End:0xD3
		case 180:
			HandleUpdateUserInfo();
			// End:0xD9
			break;
		// End:0xFFFF
		default:
			// End:0xD9
			break;
			break;
	}
	return;
}

function OnShow()
{
	// End:0x36
	if(Class'NWindow.UIDATA_PLAYER'.static.HasCrystallizeAbility())
	{
		ShowWindow(__NFUN_112__(m_WindowName, ".CrystallizeButton"));		
	}
	else
	{
		HideWindow(__NFUN_112__(m_WindowName, ".CrystallizeButton"));
	}
	SetAdenaText();
	SetItemCount();
	UpdateHennaInfo();
	return;
}

function OnHide()
{
	SaveItemOrder();
	return;
}

function OnDBClickItemWithHandle(ItemWindowHandle a_hItemWindow, int Index)
{
	UseItem(a_hItemWindow, Index);
	return;
}

function OnRClickItemWithHandle(ItemWindowHandle a_hItemWindow, int Index)
{
	UseItem(a_hItemWindow, Index);
	return;
}

function OnSelectItemWithHandle(ItemWindowHandle a_hItemWindow, int a_Index)
{
	local int i;

	// End:0x11
	if(__NFUN_114__(a_hItemWindow, m_invenItem))
	{
		return;
	}
	// End:0x22
	if(__NFUN_114__(a_hItemWindow, m_questItem))
	{
		return;
	}
	i = 0;
	J0x29:

	// End:0x69 [Loop If]
	if(__NFUN_150__(i, 15))
	{
		// End:0x5F
		if(__NFUN_119__(a_hItemWindow, m_equipItem[i]))
		{
			m_equipItem[i].ClearSelect();
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x29;
	}
	return;
}

function OnDropItem(string strTarget, ItemInfo Info, int X, int Y)
{
	local int toIndex, fromIndex;

	// End:0x61
	if(__NFUN_129__(__NFUN_132__(__NFUN_132__(__NFUN_122__(Info.DragSrcName, "InventoryItem"), __NFUN_122__(Info.DragSrcName, "QuestItem")), __NFUN_155__(-1, __NFUN_126__(Info.DragSrcName, "EquipItem")))))
	{
		return;
	}
	// End:0x2EE
	if(__NFUN_122__(strTarget, "InventoryItem"))
	{
		// End:0x1AF
		if(__NFUN_122__(Info.DragSrcName, "InventoryItem"))
		{
			toIndex = m_invenItem.GetIndexAt(X, Y, 1, 1);
			// End:0x15F
			if(__NFUN_153__(toIndex, 0))
			{
				fromIndex = m_invenItem.FindItemWithServerID(Info.ServerID);
				// End:0x15C
				if(__NFUN_155__(toIndex, fromIndex))
				{
					J0xF2:

					// End:0x127 [Loop If]
					if(__NFUN_150__(fromIndex, toIndex))
					{
						m_invenItem.SwapItems(fromIndex, __NFUN_146__(fromIndex, 1));
						__NFUN_163__(fromIndex);
						// [Loop Continue]
						goto J0xF2;
					}
					J0x127:

					// End:0x15C [Loop If]
					if(__NFUN_150__(toIndex, fromIndex))
					{
						m_invenItem.SwapItems(fromIndex, __NFUN_147__(fromIndex, 1));
						__NFUN_164__(fromIndex);
						// [Loop Continue]
						goto J0x127;
					}
				}				
			}
			else
			{
				fromIndex = m_invenItem.GetItemNum();
				J0x174:

				// End:0x1AC [Loop If]
				if(__NFUN_150__(toIndex, __NFUN_147__(fromIndex, 1)))
				{
					m_invenItem.SwapItems(toIndex, __NFUN_146__(toIndex, 1));
					__NFUN_163__(toIndex);
					// [Loop Continue]
					goto J0x174;
				}
			}			
		}
		else
		{
			// End:0x1ED
			if(__NFUN_155__(-1, __NFUN_126__(Info.DragSrcName, "EquipItem")))
			{
				RequestUnequipItem(Info.ServerID, Info.SlotBitType);				
			}
			else
			{
				// End:0x2EB
				if(__NFUN_122__(Info.DragSrcName, "PetInvenWnd"))
				{
					// End:0x2D0
					if(__NFUN_130__(IsStackableItem(Info.ConsumeType), __NFUN_151__(Info.ItemNum, 1)))
					{
						// End:0x282
						if(__NFUN_151__(Info.AllItemCount, 0))
						{
							// End:0x27F
							if(CheckItemLimit(Info.ClassID, Info.AllItemCount))
							{
								Class'NWindow.PetAPI'.static.RequestGetItemFromPet(Info.ServerID, Info.AllItemCount, false);
							}							
						}
						else
						{
							DialogSetID(10000);
							DialogSetReservedInt(Info.ServerID);
							DialogSetParamInt(Info.ItemNum);
							DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name));
						}						
					}
					else
					{
						Class'NWindow.PetAPI'.static.RequestGetItemFromPet(Info.ServerID, 1, false);
					}
				}
			}
		}		
	}
	else
	{
		// End:0x434
		if(__NFUN_122__(strTarget, "QuestItem"))
		{
			// End:0x431
			if(__NFUN_122__(Info.DragSrcName, "QuestItem"))
			{
				toIndex = m_questItem.GetIndexAt(X, Y, 1, 1);
				// End:0x3E4
				if(__NFUN_153__(toIndex, 0))
				{
					fromIndex = m_questItem.FindItemWithServerID(Info.ServerID);
					// End:0x3E1
					if(__NFUN_155__(toIndex, fromIndex))
					{
						J0x377:

						// End:0x3AC [Loop If]
						if(__NFUN_150__(fromIndex, toIndex))
						{
							m_questItem.SwapItems(fromIndex, __NFUN_146__(fromIndex, 1));
							__NFUN_163__(fromIndex);
							// [Loop Continue]
							goto J0x377;
						}
						J0x3AC:

						// End:0x3E1 [Loop If]
						if(__NFUN_150__(toIndex, fromIndex))
						{
							m_questItem.SwapItems(fromIndex, __NFUN_147__(fromIndex, 1));
							__NFUN_164__(fromIndex);
							// [Loop Continue]
							goto J0x3AC;
						}
					}					
				}
				else
				{
					fromIndex = m_invenItem.GetItemNum();
					J0x3F9:

					// End:0x431 [Loop If]
					if(__NFUN_150__(toIndex, __NFUN_147__(fromIndex, 1)))
					{
						m_invenItem.SwapItems(toIndex, __NFUN_146__(toIndex, 1));
						__NFUN_163__(toIndex);
						// [Loop Continue]
						goto J0x3F9;
					}
				}
			}			
		}
		else
		{
			// End:0x4D8
			if(__NFUN_155__(-1, __NFUN_126__(strTarget, "EquipItem")))
			{
				// End:0x48A
				if(__NFUN_122__(Info.DragSrcName, "PetInvenWnd"))
				{
					Class'NWindow.PetAPI'.static.RequestGetItemFromPet(Info.ServerID, 1, true);					
				}
				else
				{
					// End:0x4AE
					if(__NFUN_155__(-1, __NFUN_126__(Info.DragSrcName, "EquipItem")))
					{						
					}
					else
					{
						// End:0x4D5
						if(__NFUN_155__(int(byte(Info.ItemType)), int(5)))
						{
							RequestUseItem(Info.ServerID);
						}
					}
				}				
			}
			else
			{
				// End:0x600
				if(__NFUN_122__(strTarget, "TrashButton"))
				{
					// End:0x5C2
					if(__NFUN_130__(IsStackableItem(Info.ConsumeType), __NFUN_151__(Info.ItemNum, 1)))
					{
						// End:0x574
						if(__NFUN_151__(Info.AllItemCount, 0))
						{
							DialogSetID(7777);
							DialogSetReservedInt(Info.ServerID);
							DialogSetReservedInt2(Info.AllItemCount);
							DialogShow(4, MakeFullSystemMsg(GetSystemMessage(74), Info.Name, ""));							
						}
						else
						{
							DialogSetID(8888);
							DialogSetReservedInt(Info.ServerID);
							DialogSetParamInt(Info.ItemNum);
							DialogShow(6, MakeFullSystemMsg(GetSystemMessage(73), Info.Name));
						}						
					}
					else
					{
						DialogSetID(6666);
						DialogSetReservedInt(Info.ServerID);
						DialogShow(4, MakeFullSystemMsg(GetSystemMessage(74), Info.Name));
					}					
				}
				else
				{
					// End:0x6CC
					if(__NFUN_122__(strTarget, "CrystallizeButton"))
					{
						// End:0x6CC
						if(__NFUN_132__(__NFUN_122__(Info.DragSrcName, "InventoryItem"), __NFUN_155__(-1, __NFUN_126__(Info.DragSrcName, "EquipItem"))))
						{
							// End:0x6CC
							if(__NFUN_130__(Class'NWindow.UIDATA_PLAYER'.static.HasCrystallizeAbility(), Class'NWindow.UIDATA_ITEM'.static.IsCrystallizable(Info.ClassID)))
							{
								DialogSetID(9999);
								DialogSetReservedInt(Info.ServerID);
								DialogShow(4, MakeFullSystemMsg(GetSystemMessage(336), Info.Name));
							}
						}
					}
				}
			}
		}
	}
	return;
}

function OnDropItemSource(string strTarget, ItemInfo Info)
{
	// End:0x1A6
	if(__NFUN_122__(strTarget, "Console"))
	{
		// End:0x1A6
		if(__NFUN_132__(__NFUN_132__(__NFUN_122__(Info.DragSrcName, "InventoryItem"), __NFUN_122__(Info.DragSrcName, "QuestItem")), __NFUN_155__(-1, __NFUN_126__(Info.DragSrcName, "EquipItem"))))
		{
			m_clickLocation = GetClickLocation();
			// End:0x160
			if(__NFUN_130__(IsStackableItem(Info.ConsumeType), __NFUN_151__(Info.ItemNum, 1)))
			{
				// End:0x10A
				if(__NFUN_151__(Info.AllItemCount, 0))
				{
					DialogHide();
					DialogSetID(5555);
					DialogSetReservedInt(Info.ServerID);
					DialogSetReservedInt2(Info.AllItemCount);
					DialogShow(4, MakeFullSystemMsg(GetSystemMessage(1833), Info.Name, ""));					
				}
				else
				{
					DialogHide();
					DialogSetID(4444);
					DialogSetReservedInt(Info.ServerID);
					DialogSetParamInt(Info.ItemNum);
					DialogShow(6, MakeFullSystemMsg(GetSystemMessage(71), Info.Name, ""));
				}				
			}
			else
			{
				DialogHide();
				DialogSetID(3333);
				DialogSetReservedInt(Info.ServerID);
				DialogShow(4, MakeFullSystemMsg(GetSystemMessage(400), Info.Name, ""));
			}
		}
	}
	return;
}

function bool IsEquipItem(out ItemInfo Info)
{
	return Info.bEquipped;
	return;
}

function bool IsQuestItem(out ItemInfo Info)
{
	return __NFUN_154__(int(byte(Info.ItemType)), int(3));
	return;
}

function HandleClear()
{
	EquipItemClear();
	m_invenItem.Clear();
	m_questItem.Clear();
	m_EarItemList.Length = 0;
	m_FingerItemLIst.Length = 0;
	return;
}

function int EquipItemGetItemNum()
{
	local int i, ItemNum;

	i = 0;
	J0x07:

	// End:0x57 [Loop If]
	if(__NFUN_150__(i, 15))
	{
		// End:0x4D
		if(m_equipItem[i].IsEnableWindow())
		{
			ItemNum = __NFUN_146__(ItemNum, m_equipItem[i].GetItemNum());
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return ItemNum;
	return;
}

function EquipItemClear()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x32 [Loop If]
	if(__NFUN_150__(i, 15))
	{
		m_equipItem[i].Clear();
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function bool EquipItemFind(int a_ServerID)
{
	local int i, Index;

	i = 0;
	J0x07:

	// End:0x4E [Loop If]
	if(__NFUN_150__(i, 15))
	{
		Index = m_equipItem[i].FindItemWithServerID(a_ServerID);
		// End:0x44
		if(__NFUN_155__(-1, Index))
		{
			return true;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return false;
	return;
}

function EquipItemDelete(int a_ServerID)
{
	local int i, Index;
	local ItemInfo TheItemInfo;

	i = 0;
	J0x07:

	// End:0xD7 [Loop If]
	if(__NFUN_150__(i, 15))
	{
		Index = m_equipItem[i].FindItemWithServerID(a_ServerID);
		// End:0xCD
		if(__NFUN_155__(-1, Index))
		{
			m_equipItem[i].Clear();
			// End:0xCD
			if(__NFUN_154__(i, 7))
			{
				// End:0xCD
				if(m_equipItem[5].GetItem(0, TheItemInfo))
				{
					// End:0xCD
					if(__NFUN_154__(TheItemInfo.SlotBitType, 16384))
					{
						m_equipItem[7].Clear();
						m_equipItem[7].AddItem(TheItemInfo);
						m_equipItem[7].DisableWindow();
					}
				}
			}
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function EarItemUpdate()
{
	local int i, LEarIndex, REarIndex;

	LEarIndex = -1;
	REarIndex = -1;
	i = 0;
	J0x1D:

	// End:0x8D [Loop If]
	if(__NFUN_150__(i, m_EarItemList.Length))
	{
		switch(IsLOrREar(m_EarItemList[i].ServerID))
		{
			// End:0x5B
			case -1:
				LEarIndex = i;
				// End:0x83
				break;
			// End:0x6E
			case 0:
				m_EarItemList.Remove(i, 1);
				// End:0x83
				break;
			// End:0x80
			case 1:
				REarIndex = i;
				// End:0x83
				break;
			// End:0xFFFF
			default:
				break;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1D;
	}
	// End:0xCB
	if(__NFUN_155__(-1, LEarIndex))
	{
		m_equipItem[9].Clear();
		m_equipItem[9].AddItem(m_EarItemList[LEarIndex]);
	}
	// End:0x109
	if(__NFUN_155__(-1, REarIndex))
	{
		m_equipItem[8].Clear();
		m_equipItem[8].AddItem(m_EarItemList[REarIndex]);
	}
	return;
}

function FingerItemUpdate()
{
	local int i, LFingerIndex, RFingerIndex;

	LFingerIndex = -1;
	RFingerIndex = -1;
	i = 0;
	J0x1D:

	// End:0x8D [Loop If]
	if(__NFUN_150__(i, m_FingerItemLIst.Length))
	{
		switch(IsLOrRFinger(m_FingerItemLIst[i].ServerID))
		{
			// End:0x5B
			case -1:
				LFingerIndex = i;
				// End:0x83
				break;
			// End:0x6E
			case 0:
				m_FingerItemLIst.Remove(i, 1);
				// End:0x83
				break;
			// End:0x80
			case 1:
				RFingerIndex = i;
				// End:0x83
				break;
			// End:0xFFFF
			default:
				break;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1D;
	}
	// End:0xCB
	if(__NFUN_155__(-1, LFingerIndex))
	{
		m_equipItem[14].Clear();
		m_equipItem[14].AddItem(m_FingerItemLIst[LFingerIndex]);
	}
	// End:0x109
	if(__NFUN_155__(-1, RFingerIndex))
	{
		m_equipItem[13].Clear();
		m_equipItem[13].AddItem(m_FingerItemLIst[RFingerIndex]);
	}
	return;
}

function EquipItemUpdate(ItemInfo a_Info)
{
	local ItemWindowHandle hItemWnd;
	local ItemInfo TheItemInfo;
	local bool ClearLHand;
	local ItemInfo RHand, LHand, Legs, Gloves, Feet, Hair2;

	local int i;
	local bool Found;

	switch(a_Info.SlotBitType)
	{
		// End:0x20
		case 1:
			hItemWnd = m_equipItem[0];
			// End:0x650
			break;
		// End:0x25
		case 2:
		// End:0x2A
		case 4:
		// End:0xB7
		case 6:
			i = 0;
			J0x36:

			// End:0x72 [Loop If]
			if(__NFUN_150__(i, m_EarItemList.Length))
			{
				// End:0x68
				if(__NFUN_154__(m_EarItemList[i].ServerID, a_Info.ServerID))
				{
					// [Explicit Break]
					goto J0x72;
				}
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x36;
			}
			J0x72:

			// End:0xA7
			if(__NFUN_154__(i, m_EarItemList.Length))
			{
				m_EarItemList.Length = __NFUN_146__(m_EarItemList.Length, 1);
				m_EarItemList[__NFUN_147__(m_EarItemList.Length, 1)] = a_Info;
			}
			hItemWnd = none;
			EarItemUpdate();
			// End:0x650
			break;
		// End:0xCD
		case 8:
			hItemWnd = m_equipItem[4];
			// End:0x650
			break;
		// End:0xD2
		case 16:
		// End:0xD7
		case 32:
		// End:0x164
		case 48:
			i = 0;
			J0xE3:

			// End:0x11F [Loop If]
			if(__NFUN_150__(i, m_FingerItemLIst.Length))
			{
				// End:0x115
				if(__NFUN_154__(m_FingerItemLIst[i].ServerID, a_Info.ServerID))
				{
					// [Explicit Break]
					goto J0x11F;
				}
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0xE3;
			}
			J0x11F:

			// End:0x154
			if(__NFUN_154__(i, m_FingerItemLIst.Length))
			{
				m_FingerItemLIst.Length = __NFUN_146__(m_FingerItemLIst.Length, 1);
				m_FingerItemLIst[__NFUN_147__(m_FingerItemLIst.Length, 1)] = a_Info;
			}
			hItemWnd = none;
			FingerItemUpdate();
			// End:0x650
			break;
		// End:0x188
		case 64:
			hItemWnd = m_equipItem[1];
			hItemWnd.EnableWindow();
			// End:0x650
			break;
		// End:0x19E
		case 128:
			hItemWnd = m_equipItem[5];
			// End:0x650
			break;
		// End:0x1C6
		case 256:
			hItemWnd = m_equipItem[7];
			hItemWnd.EnableWindow();
			// End:0x650
			break;
		// End:0x1EE
		case 512:
			hItemWnd = m_equipItem[10];
			hItemWnd.EnableWindow();
			// End:0x650
			break;
		// End:0x207
		case 1024:
			hItemWnd = m_equipItem[6];
			// End:0x650
			break;
		// End:0x22F
		case 2048:
			hItemWnd = m_equipItem[11];
			hItemWnd.EnableWindow();
			// End:0x650
			break;
		// End:0x257
		case 4096:
			hItemWnd = m_equipItem[12];
			hItemWnd.EnableWindow();
			// End:0x650
			break;
		// End:0x26F
		case 8192:
			hItemWnd = m_equipItem[0];
			// End:0x650
			break;
		// End:0x3C2
		case 16384:
			hItemWnd = m_equipItem[5];
			ClearLHand = true;
			// End:0x2CC
			if(IsBowOrFishingRod(a_Info))
			{
				// End:0x2CC
				if(m_equipItem[7].GetItem(0, TheItemInfo))
				{
					// End:0x2CC
					if(IsArrow(TheItemInfo))
					{
						ClearLHand = false;
					}
				}
			}
			// End:0x3BF
			if(ClearLHand)
			{
				// End:0x384
				if(__NFUN_155__(__NFUN_125__(a_Info.IconNameEx1), 0))
				{
					RHand = a_Info;
					LHand = a_Info;
					RHand.IconIndex = 1;
					LHand.IconIndex = 2;
					m_equipItem[5].Clear();
					m_equipItem[5].AddItem(RHand);
					m_equipItem[7].Clear();
					m_equipItem[7].AddItem(LHand);
					m_equipItem[7].DisableWindow();
					hItemWnd = none;					
				}
				else
				{
					m_equipItem[7].Clear();
					m_equipItem[7].AddItem(a_Info);
					m_equipItem[7].DisableWindow();
				}
			}
			// End:0x650
			break;
		// End:0x436
		case 32768:
			hItemWnd = m_equipItem[6];
			Legs = a_Info;
			Legs.IconName = a_Info.IconNameEx2;
			m_equipItem[11].Clear();
			m_equipItem[11].AddItem(Legs);
			m_equipItem[11].DisableWindow();
			// End:0x650
			break;
		// End:0x44F
		case 65536:
			hItemWnd = m_equipItem[2];
			// End:0x650
			break;
		// End:0x5D1
		case 131072:
			hItemWnd = m_equipItem[6];
			Hair2 = a_Info;
			Gloves = a_Info;
			Legs = a_Info;
			Feet = a_Info;
			Hair2.IconName = a_Info.IconNameEx1;
			Gloves.IconName = a_Info.IconNameEx2;
			Legs.IconName = a_Info.IconNameEx3;
			Feet.IconName = a_Info.IconNameEx4;
			m_equipItem[1].Clear();
			m_equipItem[1].AddItem(Hair2);
			m_equipItem[1].DisableWindow();
			m_equipItem[10].Clear();
			m_equipItem[10].AddItem(Gloves);
			m_equipItem[10].DisableWindow();
			m_equipItem[11].Clear();
			m_equipItem[11].AddItem(Legs);
			m_equipItem[11].DisableWindow();
			m_equipItem[12].Clear();
			m_equipItem[12].AddItem(Feet);
			m_equipItem[12].DisableWindow();
			// End:0x650
			break;
		// End:0x5F9
		case 262144:
			hItemWnd = m_equipItem[3];
			hItemWnd.EnableWindow();
			// End:0x650
			break;
		// End:0x64D
		case 524288:
			hItemWnd = m_equipItem[2];
			m_equipItem[3].Clear();
			m_equipItem[3].AddItem(a_Info);
			m_equipItem[3].DisableWindow();
			// End:0x650
			break;
		// End:0xFFFF
		default:
			break;
	}
	// End:0x67E
	if(__NFUN_119__(none, hItemWnd))
	{
		hItemWnd.Clear();
		hItemWnd.AddItem(a_Info);
	}
	return;
}

function HandleOpenWindow()
{
	LoadItemOrder();
	ShowWindow(m_WindowName);
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus(m_WindowName);
	return;
}

function HandleHideWindow()
{
	HideWindow(m_WindowName);
	return;
}

function HandleAddItem(string param)
{
	local ItemInfo Info;

	ParamToItemInfo(param, Info);
	// End:0x2C
	if(IsEquipItem(Info))
	{
		EquipItemUpdate(Info);		
	}
	else
	{
		// End:0x51
		if(IsQuestItem(Info))
		{
			m_questItem.AddItem(Info);			
		}
		else
		{
			m_invenItem.AddItem(Info);
		}
	}
	return;
}

function HandleUpdateItem(string param)
{
	local string Type;
	local ItemInfo Info;
	local int Index;

	ParseString(param, "type", Type);
	ParamToItemInfo(param, Info);
	// End:0x11F
	if(__NFUN_122__(Type, "add"))
	{
		// End:0x51
		if(IsEquipItem(Info))
		{
			EquipItemUpdate(Info);			
		}
		else
		{
			// End:0xBF
			if(IsQuestItem(Info))
			{
				m_questItem.AddItem(Info);
				Index = __NFUN_147__(m_questItem.GetItemNum(), 1);
				J0x8B:

				// End:0xBC [Loop If]
				if(__NFUN_151__(Index, 0))
				{
					m_questItem.SwapItems(__NFUN_147__(Index, 1), Index);
					__NFUN_164__(Index);
					// [Loop Continue]
					goto J0x8B;
				}				
			}
			else
			{
				m_invenItem.AddItem(Info);
				Index = __NFUN_147__(m_invenItem.GetItemNum(), 1);
				J0xEB:

				// End:0x11C [Loop If]
				if(__NFUN_151__(Index, 0))
				{
					m_invenItem.SwapItems(__NFUN_147__(Index, 1), Index);
					__NFUN_164__(Index);
					// [Loop Continue]
					goto J0xEB;
				}
			}
		}		
	}
	else
	{
		// End:0x2E9
		if(__NFUN_122__(Type, "update"))
		{
			// End:0x1B0
			if(IsEquipItem(Info))
			{
				// End:0x160
				if(EquipItemFind(Info.ServerID))
				{
					EquipItemUpdate(Info);					
				}
				else
				{
					Index = m_invenItem.FindItemWithServerID(Info.ServerID);
					// End:0x1A2
					if(__NFUN_155__(Index, -1))
					{
						m_invenItem.DeleteItem(Index);
					}
					EquipItemUpdate(Info);
				}				
			}
			else
			{
				// End:0x22F
				if(IsQuestItem(Info))
				{
					Index = m_questItem.FindItemWithServerID(Info.ServerID);
					// End:0x208
					if(__NFUN_155__(Index, -1))
					{
						m_questItem.SetItem(Index, Info);						
					}
					else
					{
						EquipItemDelete(Info.ServerID);
						m_questItem.AddItem(Info);
					}					
				}
				else
				{
					Index = m_invenItem.FindItemWithServerID(Info.ServerID);
					// End:0x279
					if(__NFUN_155__(Index, -1))
					{
						m_invenItem.SetItem(Index, Info);						
					}
					else
					{
						EquipItemDelete(Info.ServerID);
						m_invenItem.AddItem(Info);
						Index = __NFUN_147__(m_invenItem.GetItemNum(), 1);
						J0x2B5:

						// End:0x2E6 [Loop If]
						if(__NFUN_151__(Index, 0))
						{
							m_invenItem.SwapItems(__NFUN_147__(Index, 1), Index);
							__NFUN_164__(Index);
							// [Loop Continue]
							goto J0x2B5;
						}
					}
				}
			}			
		}
		else
		{
			// End:0x393
			if(__NFUN_122__(Type, "delete"))
			{
				// End:0x31C
				if(IsEquipItem(Info))
				{
					EquipItemDelete(Info.ServerID);					
				}
				else
				{
					// End:0x360
					if(IsQuestItem(Info))
					{
						Index = m_questItem.FindItemWithServerID(Info.ServerID);
						m_questItem.DeleteItem(Index);						
					}
					else
					{
						Index = m_invenItem.FindItemWithServerID(Info.ServerID);
						m_invenItem.DeleteItem(Index);
					}
				}
			}
		}
	}
	SetAdenaText();
	SetItemCount();
	return;
}

function HandleItemListEnd()
{
	SetAdenaText();
	SetItemCount();
	OrderItem();
	return;
}

function HandleAddHennaInfo(string param)
{
	UpdateHennaInfo();
	return;
}

function HandleUpdateHennaInfo(string param)
{
	UpdateHennaInfo();
	return;
}

function UpdateHennaInfo()
{
	local int i, HennaInfoCount, HennaID, IsActive;
	local ItemInfo HennaItemInfo;
	local UserInfo PlayerInfo;
	local int ClassStep;

	// End:0x66
	if(GetPlayerInfo(PlayerInfo))
	{
		ClassStep = GetClassStep(PlayerInfo.nSubClass);
		switch(ClassStep)
		{
			// End:0x2F
			case 1:
			// End:0x34
			case 2:
			// End:0x50
			case 3:
				m_hHennaItemWindow.SetRow(ClassStep);
				// End:0x66
				break;
			// End:0xFFFF
			default:
				m_hHennaItemWindow.SetRow(0);
				// End:0x66
				break;
				break;
		}
	}
	m_hHennaItemWindow.Clear();
	HennaInfoCount = Class'NWindow.HennaAPI'.static.GetHennaInfoCount();
	// End:0xA4
	if(__NFUN_151__(HennaInfoCount, ClassStep))
	{
		HennaInfoCount = ClassStep;
	}
	i = 0;
	J0xAB:

	// End:0x193 [Loop If]
	if(__NFUN_150__(i, HennaInfoCount))
	{
		// End:0x189
		if(Class'NWindow.HennaAPI'.static.GetHennaInfo(i, HennaID, IsActive))
		{
			// End:0x101
			if(__NFUN_129__(Class'NWindow.UIDATA_HENNA'.static.GetItemName(HennaID, HennaItemInfo.Name)))
			{
				// [Explicit Break]
				goto J0x193;
			}
			// End:0x127
			if(__NFUN_129__(Class'NWindow.UIDATA_HENNA'.static.GetDescription(HennaID, HennaItemInfo.Description)))
			{
				// [Explicit Break]
				goto J0x193;
			}
			// End:0x14D
			if(__NFUN_129__(Class'NWindow.UIDATA_HENNA'.static.GetIconTex(HennaID, HennaItemInfo.IconName)))
			{
				// [Explicit Break]
				goto J0x193;
			}
			// End:0x168
			if(__NFUN_154__(0, IsActive))
			{
				HennaItemInfo.bDisabled = true;				
			}
			else
			{
				HennaItemInfo.bDisabled = false;
			}
			m_hHennaItemWindow.AddItem(HennaItemInfo);
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xAB;
	}
	J0x193:

	return;
}

function SetAdenaText()
{
	local string adenaString;

	adenaString = MakeCostString(string(GetAdena()));
	m_hAdenaTextBox.SetText(adenaString);
	m_hAdenaTextBox.SetTooltipString(ConvertNumToText(string(GetAdena())));
	return;
}

function UseItem(ItemWindowHandle a_hItemWindow, int Index)
{
	local ItemInfo Info;

	// End:0xB1
	if(a_hItemWindow.GetItem(Index, Info))
	{
		// End:0x5B
		if(Info.bRecipe)
		{
			DialogSetReservedInt(Info.ServerID);
			DialogSetID(1111);
			DialogShow(4, GetSystemMessage(798));			
		}
		else
		{
			// End:0xA1
			if(__NFUN_151__(Info.PopMsgNum, 0))
			{
				DialogSetID(2222);
				DialogSetReservedInt(Info.ServerID);
				DialogShow(4, GetSystemMessage(Info.PopMsgNum));				
			}
			else
			{
				RequestUseItem(Info.ServerID);
			}
		}
	}
	return;
}

function SaveItemOrder()
{
	local ItemInfo Info;
	local int i;

	m_itemOrder.Length = m_invenItem.GetItemNum();
	i = 0;
	J0x1D:

	// End:0x66 [Loop If]
	if(__NFUN_150__(i, m_itemOrder.Length))
	{
		m_invenItem.GetItem(i, Info);
		m_itemOrder[i] = Info.ClassID;
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1D;
	}
	SaveInventoryOrder(m_itemOrder);
	return;
}

function LoadItemOrder()
{
	LoadInventoryOrder(m_itemOrder);
	return;
}

function OrderItem()
{
	local int newItemIndex, ItemNum, itemIndex, orderIndex;
	local ItemInfo Info;
	local bool matched;

	newItemIndex = 0;
	ItemNum = m_invenItem.GetItemNum();
	itemIndex = 0;
	J0x23:

	// End:0xCE [Loop If]
	if(__NFUN_150__(itemIndex, ItemNum))
	{
		m_invenItem.GetItem(itemIndex, Info);
		matched = false;
		orderIndex = 0;
		J0x5A:

		// End:0x99 [Loop If]
		if(__NFUN_150__(orderIndex, m_itemOrder.Length))
		{
			// End:0x8F
			if(__NFUN_154__(Info.ClassID, m_itemOrder[orderIndex]))
			{
				matched = true;
				// [Explicit Break]
				goto J0x99;
			}
			__NFUN_163__(orderIndex);
			// [Loop Continue]
			goto J0x5A;
		}
		J0x99:

		// End:0xC4
		if(__NFUN_129__(matched))
		{
			m_invenItem.SwapItems(itemIndex, newItemIndex);
			__NFUN_163__(newItemIndex);
		}
		__NFUN_163__(itemIndex);
		// [Loop Continue]
		goto J0x23;
	}
	orderIndex = 0;
	J0xD5:

	// End:0x165 [Loop If]
	if(__NFUN_150__(orderIndex, m_itemOrder.Length))
	{
		itemIndex = 0;
		J0xEC:

		// End:0x15B [Loop If]
		if(__NFUN_150__(itemIndex, ItemNum))
		{
			m_invenItem.GetItem(itemIndex, Info);
			// End:0x151
			if(__NFUN_154__(Info.ClassID, m_itemOrder[orderIndex]))
			{
				m_invenItem.SwapItems(itemIndex, newItemIndex);
				__NFUN_163__(newItemIndex);
				// [Explicit Break]
				goto J0x15B;
			}
			__NFUN_163__(itemIndex);
			// [Loop Continue]
			goto J0xEC;
		}
		J0x15B:

		__NFUN_163__(orderIndex);
		// [Loop Continue]
		goto J0xD5;
	}
	return;
}

function int GetMyInventoryLimit()
{
	return Class'NWindow.UIDATA_PLAYER'.static.GetInventoryLimit();
	return;
}

function SetItemCount()
{
	local int limit, Count;
	local TextBoxHandle Handle;

	Count = __NFUN_146__(__NFUN_146__(m_invenItem.GetItemNum(), m_questItem.GetItemNum()), EquipItemGetItemNum());
	limit = GetMyInventoryLimit();
	Handle = TextBoxHandle(GetHandle(__NFUN_112__(m_WindowName, ".ItemCount")));
	Handle.SetText(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", string(Count)), "/"), string(limit)), ")"));
	return;
}

function HandleDialogOK()
{
	local int Id, Reserved, reserved2, Number;

	// End:0x1B5
	if(DialogIsMine())
	{
		Id = DialogGetID();
		Reserved = DialogGetReservedInt();
		reserved2 = DialogGetReservedInt2();
		Number = int(DialogGetString());
		// End:0x69
		if(__NFUN_132__(__NFUN_154__(Id, 1111), __NFUN_154__(Id, 2222)))
		{
			RequestUseItem(Reserved);			
		}
		else
		{
			// End:0x8C
			if(__NFUN_154__(Id, 3333))
			{
				RequestDropItem(Reserved, 1, m_clickLocation);				
			}
			else
			{
				// End:0xC5
				if(__NFUN_154__(Id, 4444))
				{
					// End:0xAD
					if(__NFUN_154__(Number, 0))
					{
						Number = 1;
					}
					RequestDropItem(Reserved, Number, m_clickLocation);					
				}
				else
				{
					// End:0xEC
					if(__NFUN_154__(Id, 5555))
					{
						RequestDropItem(Reserved, reserved2, m_clickLocation);						
					}
					else
					{
						// End:0x112
						if(__NFUN_154__(Id, 6666))
						{
							RequestDestroyItem(Reserved, 1);
							PlayConsoleSound(4);							
						}
						else
						{
							// End:0x13C
							if(__NFUN_154__(Id, 8888))
							{
								RequestDestroyItem(Reserved, Number);
								PlayConsoleSound(4);								
							}
							else
							{
								// End:0x166
								if(__NFUN_154__(Id, 7777))
								{
									RequestDestroyItem(Reserved, reserved2);
									PlayConsoleSound(4);									
								}
								else
								{
									// End:0x18C
									if(__NFUN_154__(Id, 9999))
									{
										RequestCrystallizeItem(Reserved, 1);
										PlayConsoleSound(4);										
									}
									else
									{
										// End:0x1B5
										if(__NFUN_154__(Id, 10000))
										{
											Class'NWindow.PetAPI'.static.RequestGetItemFromPet(Reserved, Number, false);
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
	return;
}

function HandleUpdateUserInfo()
{
	// End:0x1E
	if(m_hOwnerWnd.IsShowWindow())
	{
		EarItemUpdate();
		FingerItemUpdate();
	}
	return;
}

function HandleToggleWindow()
{
	// End:0x2C
	if(m_hOwnerWnd.IsShowWindow())
	{
		m_hOwnerWnd.HideWindow();
		PlayConsoleSound(14);		
	}
	else
	{
		// End:0x52
		if(IsShowInventoryWndUponEvent())
		{
			RequestItemList();
			m_hOwnerWnd.ShowWindow();
			PlayConsoleSound(13);
		}
	}
	return;
}

function bool IsShowInventoryWndUponEvent()
{
	local WindowHandle m_warehouseWnd, m_privateShopWnd, m_tradeWnd, m_shopWnd, m_multiSellWnd, m_deliverWnd;

	local PrivateShopWnd m_scriptPrivateShopWnd;

	m_warehouseWnd = GetHandle("WarehouseWnd");
	m_privateShopWnd = GetHandle("PrivateShopWnd");
	m_tradeWnd = GetHandle("TradeWnd");
	m_shopWnd = GetHandle("ShopWnd");
	m_multiSellWnd = GetHandle("MultiSellWnd");
	m_deliverWnd = GetHandle("DeliverWnd");
	m_scriptPrivateShopWnd = PrivateShopWnd(GetScript("PrivateShopWnd"));
	// End:0xC8
	if(m_warehouseWnd.IsShowWindow())
	{
		return false;
	}
	// End:0xDC
	if(m_warehouseWnd.IsShowWindow())
	{
		return false;
	}
	// End:0xF0
	if(m_tradeWnd.IsShowWindow())
	{
		return false;
	}
	// End:0x104
	if(m_shopWnd.IsShowWindow())
	{
		return false;
	}
	// End:0x118
	if(m_multiSellWnd.IsShowWindow())
	{
		return false;
	}
	// End:0x12C
	if(m_deliverWnd.IsShowWindow())
	{
		return false;
	}
	// End:0x15B
	if(__NFUN_130__(m_privateShopWnd.IsShowWindow(), __NFUN_154__(int(m_scriptPrivateShopWnd.m_type), int(2))))
	{
		return false;
	}
	return true;
	return;
}

function int IsLOrREar(int a_ServerID)
{
	local int LEar, REar, LFinger, RFinger;

	GetAccessoryServerID(LEar, REar, LFinger, RFinger);
	// End:0x32
	if(__NFUN_154__(a_ServerID, LEar))
	{
		return -1;		
	}
	else
	{
		// End:0x46
		if(__NFUN_154__(a_ServerID, REar))
		{
			return 1;			
		}
		else
		{
			return 0;
		}
	}
	return;
}

function int IsLOrRFinger(int a_ServerID)
{
	local int LEar, REar, LFinger, RFinger;

	GetAccessoryServerID(LEar, REar, LFinger, RFinger);
	// End:0x32
	if(__NFUN_154__(a_ServerID, LFinger))
	{
		return -1;		
	}
	else
	{
		// End:0x46
		if(__NFUN_154__(a_ServerID, RFinger))
		{
			return 1;			
		}
		else
		{
			return 0;
		}
	}
	return;
}

function bool IsBowOrFishingRod(ItemInfo a_Info)
{
	// End:0x26
	if(__NFUN_132__(__NFUN_154__(6, a_Info.WeaponType), __NFUN_154__(10, a_Info.WeaponType)))
	{
		return true;
	}
	return false;
	return;
}

function bool IsArrow(ItemInfo a_Info)
{
	return a_Info.bArrow;
	return;
}

defaultproperties
{
	m_WindowName="InventoryWnd"
}