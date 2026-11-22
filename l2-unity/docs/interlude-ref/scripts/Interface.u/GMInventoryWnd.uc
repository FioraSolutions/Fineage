class GMInventoryWnd extends InventoryWnd;

struct GMHennaInfo
{
	var int HennaID;
	var int IsActive;
};

var bool bShow;
var int m_ObservingUserInvenLimit;
var int m_Adena;
var bool m_HasLEar;
var bool m_HasLFinger;
var array<GMHennaInfo> m_HennaInfoList;

function OnLoad()
{
	local WindowHandle hCrystallizeButton, hTrashButton, hInvenWeight;

	RegisterEvent(2401);
	RegisterEvent(2402);
	RegisterEvent(2403);
	RegisterEvent(2404);
	InitHandle();
	bShow = false;
	m_hOwnerWnd.SetWindowTitle(" ");
	hCrystallizeButton = GetHandle("CrystallizeButton");
	hTrashButton = GetHandle("TrashButton");
	hInvenWeight = GetHandle("InvenWeight");
	hCrystallizeButton.HideWindow();
	hTrashButton.HideWindow();
	hInvenWeight.HideWindow();
	return;
}

function OnShow()
{
	SetAdenaText();
	return;
}

function ShowInventory(string a_Param)
{
	// End:0x0E
	if(__NFUN_122__(a_Param, ""))
	{
		return;
	}
	// End:0x37
	if(bShow)
	{
		HandleClear();
		m_hOwnerWnd.HideWindow();
		bShow = false;		
	}
	else
	{
		Class'NWindow.GMAPI'.static.RequestGMCommand(5, a_Param);
		bShow = true;
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 2401:
			HandleGMObservingInventoryAddItem(a_Param);
			// End:0x62
			break;
		// End:0x33
		case 2402:
			HandleGMObservingInventoryClear(a_Param);
			// End:0x62
			break;
		// End:0x49
		case 2403:
			HandleGMAddHennaInfo(a_Param);
			// End:0x62
			break;
		// End:0x5F
		case 2404:
			HandleGMUpdateHennaInfo(a_Param);
			// End:0x62
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleGMObservingInventoryAddItem(string a_Param)
{
	HandleAddItem(a_Param);
	SetItemCount();
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
			// End:0x72
			if(__NFUN_154__(Info.ClassID, 57))
			{
				SetAdena(Info.ItemNum);
			}
			m_invenItem.AddItem(Info);
		}
	}
	return;
}

function SetAdena(int a_Adena)
{
	m_Adena = a_Adena;
	SetAdenaText();
	return;
}

function SetAdenaText()
{
	local string adenaString;

	adenaString = MakeCostString(string(m_Adena));
	m_hAdenaTextBox.SetText(adenaString);
	m_hAdenaTextBox.SetTooltipString(ConvertNumToText(string(m_Adena)));
	return;
}

function int GetMyInventoryLimit()
{
	return m_ObservingUserInvenLimit;
	return;
}

function HandleGMObservingInventoryClear(string a_Param)
{
	HandleClear();
	ParseInt(a_Param, "InvenLimit", m_ObservingUserInvenLimit);
	m_hOwnerWnd.ShowWindow();
	m_hOwnerWnd.SetFocus();
	return;
}

function HandleGMAddHennaInfo(string a_Param)
{
	m_HennaInfoList.Length = __NFUN_146__(m_HennaInfoList.Length, 1);
	ParseInt(a_Param, "ID", m_HennaInfoList[__NFUN_147__(m_HennaInfoList.Length, 1)].HennaID);
	ParseInt(a_Param, "bActive", m_HennaInfoList[__NFUN_147__(m_HennaInfoList.Length, 1)].IsActive);
	UpdateHennaInfo();
	return;
}

function HandleGMUpdateHennaInfo(string a_Param)
{
	m_HennaInfoList.Length = 0;
	return;
}

function OnDropItem(string strTarget, ItemInfo Info, int X, int Y)
{
	return;
}

function OnDropItemSource(string strTarget, ItemInfo Info)
{
	return;
}

function OnDBClickItem(string strID, int Index)
{
	return;
}

function OnRClickItem(string strID, int Index)
{
	return;
}

function EquipItemUpdate(ItemInfo a_Info)
{
	local ItemWindowHandle hItemWnd;

	super.EquipItemUpdate(a_Info);
	switch(a_Info.SlotBitType)
	{
		// End:0x1C
		case 2:
		// End:0x21
		case 4:
		// End:0x60
		case 6:
			// End:0x4F
			if(__NFUN_154__(0, m_equipItem[8].GetItemNum()))
			{
				hItemWnd = m_equipItem[8];				
			}
			else
			{
				hItemWnd = m_equipItem[9];
			}
			// End:0xAC
			break;
		// End:0x65
		case 16:
		// End:0x6A
		case 32:
		// End:0xA9
		case 48:
			// End:0x98
			if(__NFUN_154__(0, m_equipItem[13].GetItemNum()))
			{
				hItemWnd = m_equipItem[13];				
			}
			else
			{
				hItemWnd = m_equipItem[14];
			}
			// End:0xAC
			break;
		// End:0xFFFF
		default:
			break;
	}
	// End:0xDA
	if(__NFUN_119__(none, hItemWnd))
	{
		hItemWnd.Clear();
		hItemWnd.AddItem(a_Info);
	}
	return;
}

function int IsLOrREar(int a_ServerID)
{
	return 0;
	return;
}

function int IsLOrRFinger(int a_ServerID)
{
	return 0;
	return;
}

function UpdateHennaInfo()
{
	local int i;
	local ItemInfo HennaItemInfo;

	m_hHennaItemWindow.Clear();
	m_hHennaItemWindow.SetRow(m_HennaInfoList.Length);
	i = 0;
	J0x2B:

	// End:0x11F [Loop If]
	if(__NFUN_150__(i, m_HennaInfoList.Length))
	{
		// End:0x6C
		if(__NFUN_129__(Class'NWindow.UIDATA_HENNA'.static.GetItemName(m_HennaInfoList[i].HennaID, HennaItemInfo.Name)))
		{
			// [Explicit Break]
			goto J0x11F;
		}
		// End:0x9D
		if(__NFUN_129__(Class'NWindow.UIDATA_HENNA'.static.GetDescription(m_HennaInfoList[i].HennaID, HennaItemInfo.Description)))
		{
			// [Explicit Break]
			goto J0x11F;
		}
		// End:0xCE
		if(__NFUN_129__(Class'NWindow.UIDATA_HENNA'.static.GetIconTex(m_HennaInfoList[i].HennaID, HennaItemInfo.IconName)))
		{
			// [Explicit Break]
			goto J0x11F;
		}
		// End:0xF4
		if(__NFUN_154__(0, m_HennaInfoList[i].IsActive))
		{
			HennaItemInfo.bDisabled = true;			
		}
		else
		{
			HennaItemInfo.bDisabled = false;
		}
		m_hHennaItemWindow.AddItem(HennaItemInfo);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x2B;
	}
	J0x11F:

	return;
}

defaultproperties
{
	m_WindowName="GMInventoryWnd"
}