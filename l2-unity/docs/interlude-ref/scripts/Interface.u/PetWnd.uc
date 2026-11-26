class PetWnd extends UICommonAPI;

const PET_EQUIPPEDTEXTURE_NAME = "l2ui_ch3.PetWnd.petitem_click";
const DIALOG_PETNAME = 1111;
const DIALOG_GIVEITEMTOPET = 2222;
const NPET_SMALLBARSIZE = 85;
const NPET_LARGEBARSIZE = 206;
const NPET_BARHEIGHT = 12;

var int m_PetID;
var bool m_bShowNameBtn;
var string m_LastInputPetName;

function OnLoad()
{
	RegisterEvent(1710);
	RegisterEvent(250);
	RegisterEvent(1010);
	RegisterEvent(1020);
	RegisterEvent(1030);
	RegisterEvent(1130);
	RegisterEvent(1320);
	RegisterEvent(1330);
	RegisterEvent(1060);
	RegisterEvent(1070);
	RegisterEvent(1080);
	RegisterEvent(1900);
	m_bShowNameBtn = true;
	HideScrollBar();
	return;
}

function OnShow()
{
	Class'NWindow.PetAPI'.static.RequestPetInventoryItemList();
	Class'NWindow.ActionAPI'.static.RequestPetActionList();
	return;
}

function OnDropItem(string strTarget, ItemInfo Info, int X, int Y)
{
	// End:0xFA
	if(__NFUN_130__(__NFUN_122__(strTarget, "PetInvenWnd"), __NFUN_122__(Info.DragSrcName, "InventoryItem")))
	{
		// End:0xE0
		if(__NFUN_130__(IsStackableItem(Info.ConsumeType), __NFUN_151__(Info.ItemNum, 1)))
		{
			// End:0x92
			if(__NFUN_151__(Info.AllItemCount, 0))
			{
				Class'NWindow.PetAPI'.static.RequestGiveItemToPet(Info.ServerID, Info.AllItemCount);				
			}
			else
			{
				DialogSetID(2222);
				DialogSetReservedInt(Info.ServerID);
				DialogSetParamInt(Info.ItemNum);
				DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), Info.Name));
			}			
		}
		else
		{
			Class'NWindow.PetAPI'.static.RequestGiveItemToPet(Info.ServerID, 1);
		}
	}
	return;
}

function HandleLanguageChanged()
{
	Class'NWindow.PetAPI'.static.RequestPetInventoryItemList();
	Class'NWindow.ActionAPI'.static.RequestPetActionList();
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x15
	if(__NFUN_154__(Event_ID, 250))
	{
		HandlePetInfoUpdate();		
	}
	else
	{
		// End:0x2D
		if(__NFUN_154__(Event_ID, 1130))
		{
			HandlePetSummonedStatusClose();			
		}
		else
		{
			// End:0x45
			if(__NFUN_154__(Event_ID, 1010))
			{
				HandlePetShow();				
			}
			else
			{
				// End:0x62
				if(__NFUN_154__(Event_ID, 1020))
				{
					HandlePetShowNameBtn(param);					
				}
				else
				{
					// End:0x7F
					if(__NFUN_154__(Event_ID, 1030))
					{
						HandleRegPetName(param);						
					}
					else
					{
						// End:0x97
						if(__NFUN_154__(Event_ID, 1710))
						{
							HandleDialogOK();							
						}
						else
						{
							// End:0xAF
							if(__NFUN_154__(Event_ID, 1320))
							{
								HandleActionPetListStart();								
							}
							else
							{
								// End:0xCC
								if(__NFUN_154__(Event_ID, 1330))
								{
									HandleActionPetList(param);									
								}
								else
								{
									// End:0xE4
									if(__NFUN_154__(Event_ID, 1060))
									{
										HandlePetInventoryItemStart();										
									}
									else
									{
										// End:0x101
										if(__NFUN_154__(Event_ID, 1070))
										{
											HandlePetInventoryItemList(param);											
										}
										else
										{
											// End:0x11E
											if(__NFUN_154__(Event_ID, 1080))
											{
												HandlePetInventoryItemUpdate(param);												
											}
											else
											{
												// End:0x133
												if(__NFUN_154__(Event_ID, 1900))
												{
													HandleLanguageChanged();
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
		}
	}
	return;
}

function HandleDialogOK()
{
	local int Id, ServerID, Number;

	// End:0x80
	if(DialogIsMine())
	{
		Id = DialogGetID();
		ServerID = DialogGetReservedInt();
		Number = int(DialogGetString());
		// End:0x58
		if(__NFUN_154__(Id, 1111))
		{
			m_LastInputPetName = DialogGetString();
			RequestChangePetName(m_LastInputPetName);			
		}
		else
		{
			// End:0x80
			if(__NFUN_154__(Id, 2222))
			{
				Class'NWindow.PetAPI'.static.RequestGiveItemToPet(ServerID, Number);
			}
		}
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1C
		case "btnName":
			OnNameClick();
			// End:0x1F
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnNameClick()
{
	DialogSetID(1111);
	DialogShow(2, GetSystemMessage(535));
	return;
}

function OnClickItem(string strID, int Index)
{
	local ItemInfo infItem;

	// End:0x78
	if(__NFUN_130__(__NFUN_122__(strID, "PetActionWnd"), __NFUN_151__(Index, -1)))
	{
		// End:0x78
		if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PetWnd.PetWnd_Action.PetActionWnd", Index, infItem))
		{
			DoAction(infItem.ClassID);
		}
	}
	return;
}

function OnDBClickItem(string strID, int Index)
{
	local ItemInfo infItem;

	// End:0x82
	if(__NFUN_130__(__NFUN_122__(strID, "PetInvenWnd"), __NFUN_151__(Index, -1)))
	{
		// End:0x82
		if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PetWnd.PetWnd_Inventory.PetInvenWnd", Index, infItem))
		{
			Class'NWindow.PetAPI'.static.RequestPetUseItem(infItem.ServerID);
		}
	}
	return;
}

function OnRClickItem(string strID, int Index)
{
	local ItemInfo infItem;

	// End:0x82
	if(__NFUN_130__(__NFUN_122__(strID, "PetInvenWnd"), __NFUN_151__(Index, -1)))
	{
		// End:0x82
		if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("PetWnd.PetWnd_Inventory.PetInvenWnd", Index, infItem))
		{
			Class'NWindow.PetAPI'.static.RequestPetUseItem(infItem.ServerID);
		}
	}
	return;
}

function HideScrollBar()
{
	Class'NWindow.UIAPI_ITEMWINDOW'.static.ShowScrollBar("PetWnd.PetWnd_Action.PetActionWnd", false);
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtLvName", "");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtHP", "0/0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtMP", "0/0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtSP", "0");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtExp", "0%");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtFatigue", "0%");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtWeight", "0%");
	UpdateHPBar(0, 0);
	UpdateMPBar(0, 0);
	UpdateFatigueBar(0, 0);
	UpdateEXPBar(0, 0);
	UpdateWeightBar(0, 0);
	return;
}

function ClearActionWnd()
{
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("PetWnd.PetWnd_Action.PetActionWnd");
	return;
}

function ClearInvenWnd()
{
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("PetWnd.PetWnd_Inventory.PetInvenWnd");
	return;
}

function HandleRegPetName(string param)
{
	local int MsgNo;
	local Color MsgColor;

	ParseInt(param, "ErrMsgNo", MsgNo);
	MsgColor.R = 176;
	MsgColor.G = 155;
	MsgColor.B = 121;
	MsgColor.A = byte(255);
	AddSystemMessage(GetSystemMessage(MsgNo), MsgColor);
	DialogShow(2, GetSystemMessage(535));
	// End:0x90
	if(__NFUN_154__(MsgNo, 80))
	{
		DialogSetString(m_LastInputPetName);
	}
	return;
}

function HandlePetShowNameBtn(string param)
{
	local int ShowFlag;

	ParseInt(param, "Show", ShowFlag);
	// End:0x2B
	if(__NFUN_154__(ShowFlag, 1))
	{
		SetVisibleNameBtn(true);		
	}
	else
	{
		SetVisibleNameBtn(false);
	}
	return;
}

function SetVisibleNameBtn(bool bShow)
{
	// End:0x2B
	if(bShow)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PetWnd.btnName");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PetWnd.btnName");
	}
	m_bShowNameBtn = bShow;
	return;
}

function HandlePetSummonedStatusClose()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PetWnd");
	PlayConsoleSound(6);
	return;
}

function HandlePetInfoUpdate()
{
	local string Name;
	local int HP, maxHP, MP, maxMP, Fatigue, MaxFatigue,
		CarryWeight, CarringWeight, SP, Level;

	local float fExpRate, fTmp;
	local int PhysicalAttack, PhysicalDefense, HitRate, CriticalRate, PhysicalAttackSpeed, MagicalAttack,
		MagicDefense, PhysicalAvoid, MovingSpeed, MagicCastingSpeed, SoulShotCosume,
		SpiritShotConsume;

	local PetInfo Info;

	// End:0x1A3
	if(GetPetInfo(Info))
	{
		m_PetID = Info.nID;
		Name = Info.Name;
		SP = Info.nSP;
		Level = Info.nLevel;
		HP = Info.nCurHP;
		maxHP = Info.nMaxHP;
		MP = Info.nCurMP;
		maxMP = Info.nMaxMP;
		CarryWeight = Info.nCarryWeight;
		CarringWeight = Info.nCarringWeight;
		Fatigue = Info.nFatigue;
		MaxFatigue = Info.nMaxFatigue;
		fExpRate = Class'NWindow.UIDATA_PET'.static.GetPetEXPRate();
		PhysicalAttack = Info.nPhysicalAttack;
		PhysicalDefense = Info.nPhysicalDefense;
		HitRate = Info.nHitRate;
		CriticalRate = Info.nCriticalRate;
		PhysicalAttackSpeed = Info.nPhysicalAttackSpeed;
		MagicalAttack = Info.nMagicalAttack;
		MagicDefense = Info.nMagicDefense;
		PhysicalAvoid = Info.nPhysicalAvoid;
		MovingSpeed = Info.nMovingSpeed;
		MagicCastingSpeed = Info.nMagicCastingSpeed;
		SoulShotCosume = Info.nSoulShotCosume;
		SpiritShotConsume = Info.nSpiritShotConsume;
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtLvName", __NFUN_112__(__NFUN_112__(string(Level), " "), Name));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtHP", __NFUN_112__(__NFUN_112__(string(HP), "/"), string(maxHP)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtMP", __NFUN_112__(__NFUN_112__(string(MP), "/"), string(maxMP)));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtSP", string(SP));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtExp", __NFUN_112__(string(fExpRate), "%"));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtPhysicalAttack", string(PhysicalAttack));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtPhysicalDefense", string(PhysicalDefense));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtHitRate", string(HitRate));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtCriticalRate", string(CriticalRate));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtPhysicalAttackSpeed", string(PhysicalAttackSpeed));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtMagicalAttack", string(MagicalAttack));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtMagicDefense", string(MagicDefense));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtPhysicalAvoid", string(PhysicalAvoid));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtMovingSpeed", string(MovingSpeed));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtMagicCastingSpeed", string(MagicCastingSpeed));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtSoulShotCosume", string(SoulShotCosume));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtSpiritShotConsume", string(SpiritShotConsume));
	fTmp = __NFUN_172__(__NFUN_171__(100.0000000, float(Fatigue)), float(MaxFatigue));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtFatigue", __NFUN_112__(string(fTmp), "%"));
	fTmp = __NFUN_172__(__NFUN_171__(100.0000000, float(CarringWeight)), float(CarryWeight));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PetWnd.txtWeight", __NFUN_112__(string(fTmp), "%"));
	UpdateHPBar(HP, maxHP);
	UpdateMPBar(MP, maxMP);
	UpdateEXPBar(int(fExpRate), 100);
	UpdateFatigueBar(Fatigue, MaxFatigue);
	UpdateWeightBar(CarringWeight, CarryWeight);
	return;
}

function HandlePetShow()
{
	Clear();
	HandlePetInfoUpdate();
	PlayConsoleSound(5);
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PetWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PetWnd");
	SetVisibleNameBtn(m_bShowNameBtn);
	return;
}

function UpdateHPBar(int Value, int MaxValue)
{
	local int Size;

	Size = 0;
	// End:0x3F
	if(__NFUN_151__(MaxValue, 0))
	{
		Size = 85;
		// End:0x3F
		if(__NFUN_150__(Value, MaxValue))
		{
			Size = __NFUN_145__(__NFUN_144__(85, Value), MaxValue);
		}
	}
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("PetWnd.texHP", Size, 12);
	return;
}

function UpdateMPBar(int Value, int MaxValue)
{
	local int Size;

	Size = 0;
	// End:0x3F
	if(__NFUN_151__(MaxValue, 0))
	{
		Size = 85;
		// End:0x3F
		if(__NFUN_150__(Value, MaxValue))
		{
			Size = __NFUN_145__(__NFUN_144__(85, Value), MaxValue);
		}
	}
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("PetWnd.texMP", Size, 12);
	return;
}

function UpdateEXPBar(int Value, int MaxValue)
{
	local int Size;

	Size = 0;
	// End:0x3F
	if(__NFUN_151__(MaxValue, 0))
	{
		Size = 206;
		// End:0x3F
		if(__NFUN_150__(Value, MaxValue))
		{
			Size = __NFUN_145__(__NFUN_144__(206, Value), MaxValue);
		}
	}
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("PetWnd.texEXP", Size, 12);
	return;
}

function UpdateFatigueBar(int Value, int MaxValue)
{
	local int Size;

	Size = 0;
	// End:0x3F
	if(__NFUN_151__(MaxValue, 0))
	{
		Size = 206;
		// End:0x3F
		if(__NFUN_150__(Value, MaxValue))
		{
			Size = __NFUN_145__(__NFUN_144__(206, Value), MaxValue);
		}
	}
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("PetWnd.texFatigue", Size, 12);
	return;
}

function UpdateWeightBar(int Value, int MaxValue)
{
	local int Size;
	local float fTmp;
	local string strName;

	Size = 0;
	// End:0x17B
	if(__NFUN_151__(MaxValue, 0))
	{
		fTmp = __NFUN_172__(__NFUN_171__(100.0000000, float(Value)), float(MaxValue));
		// End:0x6F
		if(__NFUN_178__(fTmp, 50.0000000))
		{
			strName = "L2UI_CH3.PlayerStatusWnd.ps_weightbar1";			
		}
		else
		{
			// End:0xC0
			if(__NFUN_130__(__NFUN_177__(fTmp, 50.0000000), __NFUN_178__(fTmp, 66.6600037)))
			{
				strName = "L2UI_CH3.PlayerStatusWnd.ps_weightbar2";				
			}
			else
			{
				// End:0x111
				if(__NFUN_130__(__NFUN_177__(fTmp, 66.6600037), __NFUN_178__(fTmp, 80.0000000)))
				{
					strName = "L2UI_CH3.PlayerStatusWnd.ps_weightbar3";					
				}
				else
				{
					// End:0x14E
					if(__NFUN_177__(fTmp, 80.0000000))
					{
						strName = "L2UI_CH3.PlayerStatusWnd.ps_weightbar4";
					}
				}
			}
		}
		Size = 85;
		// End:0x17B
		if(__NFUN_150__(Value, MaxValue))
		{
			Size = __NFUN_145__(__NFUN_144__(85, Value), MaxValue);
		}
	}
	Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("PetWnd.texWeight", strName);
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("PetWnd.texWeight", Size, 12);
	return;
}

function HandleActionPetListStart()
{
	ClearActionWnd();
	return;
}

function HandleActionPetList(string param)
{
	local int tmp;
	local UIEventManager.EActionCategory Type;
	local int ActionID;
	local string strActionName, strIconName, strDescription, strCommand;
	local ItemInfo infItem;

	ParseInt(param, "Type", tmp);
	ParseInt(param, "ActionID", ActionID);
	ParseString(param, "Name", strActionName);
	ParseString(param, "IconName", strIconName);
	ParseString(param, "Description", strDescription);
	ParseString(param, "Command", strCommand);
	infItem.ClassID = ActionID;
	infItem.Name = strActionName;
	infItem.IconName = strIconName;
	infItem.Description = strDescription;
	infItem.ItemSubType = int(3);
	infItem.MacroCommand = strCommand;
	Type = byte(tmp);
	// End:0x149
	if(__NFUN_154__(int(Type), int(4)))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PetWnd.PetWnd_Action.PetActionWnd", infItem);
	}
	return;
}

function HandlePetInventoryItemStart()
{
	ClearInvenWnd();
	return;
}

function HandlePetInventoryItemList(string param)
{
	local ItemInfo infItem;

	ParamToItemInfo(param, infItem);
	// End:0x48
	if(infItem.bEquipped)
	{
		infItem.ForeTexture = "l2ui_ch3.PetWnd.petitem_click";
	}
	Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PetWnd.PetWnd_Inventory.PetInvenWnd", infItem);
	return;
}

function HandlePetInventoryItemUpdate(string param)
{
	local ItemInfo infItem;
	local int tmp, Index;
	local UIEventManager.EInventoryUpdateType WorkType;

	ParamToItemInfo(param, infItem);
	ParseInt(param, "WorkType", tmp);
	WorkType = byte(tmp);
	// End:0x49
	if(__NFUN_150__(infItem.ClassID, 1))
	{
		return;
	}
	// End:0x81
	if(infItem.bEquipped)
	{
		infItem.ForeTexture = "l2ui_ch3.PetWnd.petitem_click";
	}
	switch(WorkType)
	{
		// End:0xC9
		case 1:
			Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("PetWnd.PetWnd_Inventory.PetInvenWnd", infItem);
			// End:0x1F5
			break;
		// End:0x160
		case 2:
			Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PetWnd.PetWnd_Inventory.PetInvenWnd", infItem.ClassID);
			// End:0x11F
			if(__NFUN_150__(Index, 0))
			{
				return;
			}
			Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("PetWnd.PetWnd_Inventory.PetInvenWnd", Index, infItem);
			// End:0x1F5
			break;
		// End:0x1F2
		case 3:
			Index = Class'NWindow.UIAPI_ITEMWINDOW'.static.FindItemWithClassID("PetWnd.PetWnd_Inventory.PetInvenWnd", infItem.ClassID);
			// End:0x1B6
			if(__NFUN_150__(Index, 0))
			{
				return;
			}
			Class'NWindow.UIAPI_ITEMWINDOW'.static.DeleteItem("PetWnd.PetWnd_Inventory.PetInvenWnd", Index);
			// End:0x1F5
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}
