class ToolTip extends UICommonAPI;

const TOOLTIP_MINIMUM_WIDTH = 144;
const TOOLTIP_SETITEM_MAX = 3;

var CustomTooltip m_Tooltip;
var DrawItemInfo m_Info;

function OnLoad()
{
	RegisterEvent(2920);
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x1D
		case 2920:
			HandleRequestTooltipInfo(param);
			// End:0x20
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleRequestTooltipInfo(string param)
{
	local string TooltipType;
	local int SourceType;
	local UIEventManager.ETooltipSourceType eSourceType;

	ClearTooltip();
	// End:0x2A
	if(__NFUN_129__(ParseString(param, "TooltipType", TooltipType)))
	{
		return;
	}
	// End:0x4D
	if(__NFUN_129__(ParseInt(param, "SourceType", SourceType)))
	{
		return;
	}
	eSourceType = byte(SourceType);
	// End:0x7E
	if(__NFUN_122__(TooltipType, "Text"))
	{
		ReturnTooltip_NTT_TEXT(param, eSourceType, false);		
	}
	else
	{
		// End:0xA9
		if(__NFUN_122__(TooltipType, "Description"))
		{
			ReturnTooltip_NTT_TEXT(param, eSourceType, true);			
		}
		else
		{
			// End:0xCE
			if(__NFUN_122__(TooltipType, "Action"))
			{
				ReturnTooltip_NTT_ACTION(param, eSourceType);				
			}
			else
			{
				// End:0xF2
				if(__NFUN_122__(TooltipType, "Skill"))
				{
					ReturnTooltip_NTT_SKILL(param, eSourceType);					
				}
				else
				{
					// End:0x11B
					if(__NFUN_122__(TooltipType, "NormalItem"))
					{
						ReturnTooltip_NTT_NORMALITEM(param, eSourceType);						
					}
					else
					{
						// End:0x142
						if(__NFUN_122__(TooltipType, "Shortcut"))
						{
							ReturnTooltip_NTT_SHORTCUT(param, eSourceType);							
						}
						else
						{
							// End:0x16F
							if(__NFUN_122__(TooltipType, "AbnormalStatus"))
							{
								ReturnTooltip_NTT_ABNORMALSTATUS(param, eSourceType);								
							}
							else
							{
								// End:0x19F
								if(__NFUN_122__(TooltipType, "RecipeManufacture"))
								{
									ReturnTooltip_NTT_RECIPE_MANUFACTURE(param, eSourceType);									
								}
								else
								{
									// End:0x1C5
									if(__NFUN_122__(TooltipType, "Recipe"))
									{
										ReturnTooltip_NTT_RECIPE(param, eSourceType, false);										
									}
									else
									{
										// End:0x1F0
										if(__NFUN_122__(TooltipType, "RecipePrice"))
										{
											ReturnTooltip_NTT_RECIPE(param, eSourceType, true);											
										}
										else
										{
											// End:0x2D8
											if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_122__(TooltipType, "Inventory"), __NFUN_122__(TooltipType, "InventoryPrice1")), __NFUN_122__(TooltipType, "InventoryPrice2")), __NFUN_122__(TooltipType, "InventoryPrice1HideEnchant")), __NFUN_122__(TooltipType, "InventoryPrice1HideEnchantStackable")), __NFUN_122__(TooltipType, "InventoryPrice2PrivateShop")))
											{
												ReturnTooltip_NTT_ITEM(param, TooltipType, eSourceType);												
											}
											else
											{
												// End:0x301
												if(__NFUN_122__(TooltipType, "PartyMatch"))
												{
													ReturnTooltip_NTT_PARTYMATCH(param, eSourceType);													
												}
												else
												{
													// End:0x329
													if(__NFUN_122__(TooltipType, "QuestInfo"))
													{
														ReturnTooltip_NTT_QUESTINFO(param, eSourceType);														
													}
													else
													{
														// End:0x351
														if(__NFUN_122__(TooltipType, "QuestList"))
														{
															ReturnTooltip_NTT_QUESTLIST(param, eSourceType);															
														}
														else
														{
															// End:0x378
															if(__NFUN_122__(TooltipType, "RaidList"))
															{
																ReturnTooltip_NTT_RAIDLIST(param, eSourceType);																
															}
															else
															{
																// End:0x39F
																if(__NFUN_122__(TooltipType, "ClanInfo"))
																{
																	ReturnTooltip_NTT_CLANINFO(param, eSourceType);																	
																}
																else
																{
																	// End:0x45D
																	if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_122__(TooltipType, "ManorSeedInfo"), __NFUN_122__(TooltipType, "ManorCropInfo")), __NFUN_122__(TooltipType, "ManorSeedSetting")), __NFUN_122__(TooltipType, "ManorCropSetting")), __NFUN_122__(TooltipType, "ManorDefaultInfo")), __NFUN_122__(TooltipType, "ManorCropSell")))
																	{
																		ReturnTooltip_NTT_MANOR(param, TooltipType, eSourceType);
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
					}
				}
			}
		}
	}
	return;
}

function bool IsEnchantableItem(UIEventManager.EItemParamType Type)
{
	return __NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_154__(int(Type), int(0)), __NFUN_154__(int(Type), int(1))), __NFUN_154__(int(Type), int(3))), __NFUN_154__(int(Type), int(2)));
	return;
}

function ClearTooltip()
{
	m_Tooltip.SimpleLineCount = 0;
	m_Tooltip.MinimumWidth = 0;
	m_Tooltip.DrawList.Remove(0, m_Tooltip.DrawList.Length);
	return;
}

function StartItem()
{
	local DrawItemInfo infoClear;

	m_Info = infoClear;
	return;
}

function EndItem()
{
	m_Tooltip.DrawList.Length = __NFUN_146__(m_Tooltip.DrawList.Length, 1);
	m_Tooltip.DrawList[__NFUN_147__(m_Tooltip.DrawList.Length, 1)] = m_Info;
	return;
}

function ReturnTooltip_NTT_TEXT(string param, UIEventManager.ETooltipSourceType eSourceType, bool bDesc)
{
	local string strText;
	local int Id;

	// End:0x156
	if(__NFUN_154__(int(eSourceType), int(0)))
	{
		// End:0xFB
		if(ParseString(param, "Text", strText))
		{
			// End:0xF8
			if(__NFUN_151__(__NFUN_125__(strText), 0))
			{
				// End:0xC2
				if(bDesc)
				{
					m_Tooltip.MinimumWidth = 144;
					StartItem();
					m_Info.eType = 1;
					m_Info.t_color.R = 178;
					m_Info.t_color.G = 190;
					m_Info.t_color.B = 207;
					m_Info.t_color.A = byte(255);
					m_Info.t_strText = strText;
					EndItem();					
				}
				else
				{
					StartItem();
					m_Info.eType = 1;
					m_Info.t_bDrawOneLine = true;
					m_Info.t_strText = strText;
					EndItem();
				}
			}			
		}
		else
		{
			// End:0x153
			if(ParseInt(param, "ID", Id))
			{
				// End:0x153
				if(__NFUN_151__(Id, 0))
				{
					StartItem();
					m_Info.eType = 1;
					m_Info.t_bDrawOneLine = true;
					m_Info.t_ID = Id;
					EndItem();
				}
			}
		}		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_ITEM(string param, string TooltipType, UIEventManager.ETooltipSourceType eSourceType)
{
	local ItemInfo item;
	local UIEventManager.EItemType EItemType;
	local UIEventManager.EEtcItemType EEtcItemType;
	local bool bLargeWidth;
	local string SlotString, strTmp;
	local int nTmp, idx;
	local string ItemName;
	local int Quality, ColorR, ColorG, ColorB;
	local string strDesc1, strDesc2, strDesc3;
	local array<int> arrID;
	local int SetID, ClassID;
	local string strAdena, strAdenaComma;
	local Color AdenaColor;

	// End:0x16BB
	if(__NFUN_154__(int(eSourceType), int(1)))
	{
		ParamToItemInfo(param, item);
		EItemType = byte(item.ItemType);
		EEtcItemType = byte(item.ItemSubType);
		ItemName = Class'NWindow.UIDATA_ITEM'.static.GetRefineryItemName(item.Name, item.RefineryOp1, item.RefineryOp2);
		// End:0xD9
		if(__NFUN_130__(__NFUN_123__(TooltipType, "InventoryPrice1HideEnchant"), __NFUN_123__(TooltipType, "InventoryPrice1HideEnchantStackable")))
		{
			AddTooltipItemEnchant(item);
		}
		AddTooltipItemName(ItemName, item);
		AddTooltipItemGrade(item);
		// End:0x12E
		if(__NFUN_123__(TooltipType, "InventoryPrice1HideEnchantStackable"))
		{
			AddTooltipItemCount(item);
		}
		// End:0x1B3
		if(__NFUN_154__(item.ClassID, 57))
		{
			m_Tooltip.SimpleLineCount = 2;
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.bLineBreak = true;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_strText = __NFUN_112__(__NFUN_112__("(", ConvertNumToText(string(item.ItemNum))), ")");
			EndItem();
		}
		// End:0x365
		if(__NFUN_132__(__NFUN_132__(__NFUN_122__(TooltipType, "InventoryPrice1"), __NFUN_122__(TooltipType, "InventoryPrice1HideEnchant")), __NFUN_122__(TooltipType, "InventoryPrice1HideEnchantStackable")))
		{
			strAdena = string(item.Price);
			strAdenaComma = MakeCostString(strAdena);
			AdenaColor = GetNumericColor(strAdenaComma);
			AddTooltipItemOption(322, __NFUN_112__(strAdenaComma, " "), true, true, false);
			SetTooltipItemColor(int(AdenaColor.R), int(AdenaColor.G), int(AdenaColor.B), 0);
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color = AdenaColor;
			m_Info.t_ID = 469;
			EndItem();
			m_Tooltip.SimpleLineCount = 2;
			// End:0x365
			if(__NFUN_151__(item.Price, 0))
			{
				m_Tooltip.SimpleLineCount = 3;
				AddTooltipItemOption(0, __NFUN_112__(__NFUN_112__("(", ConvertNumToText(strAdena)), ")"), false, true, false);
				SetTooltipItemColor(int(AdenaColor.R), int(AdenaColor.G), int(AdenaColor.B), 0);
			}
		}
		// End:0x608
		if(__NFUN_132__(__NFUN_122__(TooltipType, "InventoryPrice2"), __NFUN_122__(TooltipType, "InventoryPrice2PrivateShop")))
		{
			strAdena = string(item.Price);
			strAdenaComma = MakeCostString(strAdena);
			AdenaColor = GetNumericColor(strAdenaComma);
			AddTooltipItemOption2(322, 468, true, true, false);
			SetTooltipItemColor(int(AdenaColor.R), int(AdenaColor.G), int(AdenaColor.B), 0);
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color = AdenaColor;
			m_Info.t_strText = __NFUN_112__(__NFUN_112__(" ", strAdenaComma), " ");
			EndItem();
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color = AdenaColor;
			m_Info.t_ID = 469;
			EndItem();
			m_Tooltip.SimpleLineCount = 2;
			// End:0x608
			if(__NFUN_151__(item.Price, 0))
			{
				m_Tooltip.SimpleLineCount = 3;
				StartItem();
				m_Info.eType = 1;
				m_Info.nOffSetY = 6;
				m_Info.bLineBreak = true;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_color = AdenaColor;
				m_Info.t_strText = "(";
				EndItem();
				StartItem();
				m_Info.eType = 1;
				m_Info.nOffSetY = 6;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_color = AdenaColor;
				m_Info.t_ID = 468;
				EndItem();
				StartItem();
				m_Info.eType = 1;
				m_Info.nOffSetY = 6;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_color = AdenaColor;
				m_Info.t_strText = __NFUN_112__(__NFUN_112__(" ", ConvertNumToText(strAdena)), ")");
				EndItem();
			}
		}
		// End:0x66D
		if(__NFUN_122__(TooltipType, "InventoryPrice2PrivateShop"))
		{
			// End:0x66D
			if(__NFUN_130__(IsStackableItem(item.ConsumeType), __NFUN_151__(item.Reserved, 0)))
			{
				AddTooltipItemOption(808, string(item.Reserved), true, true, false);
			}
		}
		SlotString = GetSlotTypeString(item.ItemType, item.SlotBitType, item.ArmorType);
		switch(EItemType)
		{
			// End:0xAFB
			case 0:
				bLargeWidth = true;
				strTmp = GetWeaponTypeString(item.WeaponType);
				// End:0x6EB
				if(__NFUN_151__(__NFUN_125__(strTmp), 0))
				{
					AddTooltipItemOption(0, __NFUN_112__(__NFUN_112__(strTmp, " / "), SlotString), false, true, false);
				}
				AddTooltipItemBlank(12);
				AddTooltipItemOption(1489, "", true, false, false);
				SetTooltipItemColor(255, 255, 255, 0);
				AddTooltipItemOption(94, string(GetPhysicalDamage(item.WeaponType, item.SlotBitType, item.CrystalType, item.Enchanted, item.PhysicalDamage)), true, true, false);
				AddTooltipItemOption(98, string(GetMagicalDamage(item.WeaponType, item.SlotBitType, item.CrystalType, item.Enchanted, item.MagicalDamage)), true, true, false);
				AddTooltipItemOption(111, GetAttackSpeedString(item.AttackSpeed), true, true, false);
				// End:0x7E5
				if(__NFUN_151__(item.SoulshotCount, 0))
				{
					AddTooltipItemOption(404, __NFUN_112__("X ", string(item.SoulshotCount)), true, true, false);
				}
				// End:0x815
				if(__NFUN_151__(item.SpiritshotCount, 0))
				{
					AddTooltipItemOption(496, __NFUN_112__("X ", string(item.SpiritshotCount)), true, true, false);
				}
				AddTooltipItemOption(52, string(item.Weight), true, true, false);
				// End:0x856
				if(__NFUN_155__(item.MpConsume, 0))
				{
					AddTooltipItemOption(320, string(item.MpConsume), true, true, false);
				}
				// End:0xAF8
				if(__NFUN_132__(__NFUN_155__(item.RefineryOp1, 0), __NFUN_155__(item.RefineryOp2, 0)))
				{
					AddTooltipItemBlank(12);
					AddTooltipItemOption(1490, "", true, false, false);
					SetTooltipItemColor(255, 255, 255, 0);
					// End:0x8E6
					if(__NFUN_155__(item.RefineryOp2, 0))
					{
						Quality = Class'NWindow.UIDATA_REFINERYOPTION'.static.GetQuality(item.RefineryOp2);
						GetRefineryColor(Quality, ColorR, ColorG, ColorB);
					}
					// End:0x9CF
					if(__NFUN_155__(item.RefineryOp1, 0))
					{
						strDesc1 = "";
						strDesc2 = "";
						strDesc3 = "";
						// End:0x9CF
						if(Class'NWindow.UIDATA_REFINERYOPTION'.static.GetOptionDescription(item.RefineryOp1, strDesc1, strDesc2, strDesc3))
						{
							// End:0x96B
							if(__NFUN_151__(__NFUN_125__(strDesc1), 0))
							{
								AddTooltipItemOption(0, strDesc1, false, true, false);
								SetTooltipItemColor(ColorR, ColorG, ColorB, 0);
							}
							// End:0x99D
							if(__NFUN_151__(__NFUN_125__(strDesc2), 0))
							{
								AddTooltipItemOption(0, strDesc2, false, true, false);
								SetTooltipItemColor(ColorR, ColorG, ColorB, 0);
							}
							// End:0x9CF
							if(__NFUN_151__(__NFUN_125__(strDesc3), 0))
							{
								AddTooltipItemOption(0, strDesc3, false, true, false);
								SetTooltipItemColor(ColorR, ColorG, ColorB, 0);
							}
						}
					}
					// End:0xAB8
					if(__NFUN_155__(item.RefineryOp2, 0))
					{
						strDesc1 = "";
						strDesc2 = "";
						strDesc3 = "";
						// End:0xAB8
						if(Class'NWindow.UIDATA_REFINERYOPTION'.static.GetOptionDescription(item.RefineryOp2, strDesc1, strDesc2, strDesc3))
						{
							// End:0xA54
							if(__NFUN_151__(__NFUN_125__(strDesc1), 0))
							{
								AddTooltipItemOption(0, strDesc1, false, true, false);
								SetTooltipItemColor(ColorR, ColorG, ColorB, 0);
							}
							// End:0xA86
							if(__NFUN_151__(__NFUN_125__(strDesc2), 0))
							{
								AddTooltipItemOption(0, strDesc2, false, true, false);
								SetTooltipItemColor(ColorR, ColorG, ColorB, 0);
							}
							// End:0xAB8
							if(__NFUN_151__(__NFUN_125__(strDesc3), 0))
							{
								AddTooltipItemOption(0, strDesc3, false, true, false);
								SetTooltipItemColor(ColorR, ColorG, ColorB, 0);
							}
						}
					}
					AddTooltipItemOption(1491, "", true, false, false);
					SetTooltipItemColor(ColorR, ColorG, ColorB, 0);
					// End:0xAF8
					if(__NFUN_151__(__NFUN_125__(item.Description), 0))
					{
						AddTooltipItemBlank(12);
					}
				}
				// End:0xEBE
				break;
			// End:0xCA6
			case 1:
				bLargeWidth = true;
				// End:0xBAB
				if(__NFUN_132__(__NFUN_154__(item.SlotBitType, 256), __NFUN_154__(item.SlotBitType, 128)))
				{
					AddTooltipItemOption(95, string(GetShieldDefense(item.CrystalType, item.Enchanted, item.ShieldDefense)), true, true, false);
					AddTooltipItemOption(317, string(item.ShieldDefenseRate), true, true, false);
					AddTooltipItemOption(97, string(item.AvoidModify), true, true, false);
					AddTooltipItemOption(52, string(item.Weight), true, true, false);					
				}
				else
				{
					// End:0xC3F
					if(IsMagicalArmor(item.ClassID))
					{
						// End:0xBDA
						if(__NFUN_151__(__NFUN_125__(SlotString), 0))
						{
							AddTooltipItemOption(0, SlotString, false, true, false);
						}
						AddTooltipItemOption(388, string(item.MpBonus), true, true, false);
						AddTooltipItemOption(95, string(GetPhysicalDefense(item.CrystalType, item.Enchanted, item.PhysicalDefense)), true, true, false);
						AddTooltipItemOption(52, string(item.Weight), true, true, false);						
					}
					else
					{
						// End:0xC5B
						if(__NFUN_151__(__NFUN_125__(SlotString), 0))
						{
							AddTooltipItemOption(0, SlotString, false, true, false);
						}
						AddTooltipItemOption(95, string(GetPhysicalDefense(item.CrystalType, item.Enchanted, item.PhysicalDefense)), true, true, false);
						AddTooltipItemOption(52, string(item.Weight), true, true, false);
					}
				}
				// End:0xEBE
				break;
			// End:0xD1A
			case 2:
				bLargeWidth = true;
				// End:0xCCF
				if(__NFUN_151__(__NFUN_125__(SlotString), 0))
				{
					AddTooltipItemOption(0, SlotString, false, true, false);
				}
				AddTooltipItemOption(99, string(GetMagicalDefense(item.CrystalType, item.Enchanted, item.MagicalDefense)), true, true, false);
				AddTooltipItemOption(52, string(item.Weight), true, true, false);
				// End:0xEBE
				break;
			// End:0xD46
			case 3:
				bLargeWidth = true;
				// End:0xD43
				if(__NFUN_151__(__NFUN_125__(SlotString), 0))
				{
					AddTooltipItemOption(0, SlotString, false, true, false);
				}
				// End:0xEBE
				break;
			// End:0xEBB
			case 5:
				bLargeWidth = true;
				// End:0xDB9
				if(__NFUN_154__(int(EEtcItemType), int(7)))
				{
					// End:0xD81
					if(__NFUN_154__(item.Damaged, 0))
					{
						nTmp = 971;						
					}
					else
					{
						nTmp = 970;
					}
					AddTooltipItemOption2(969, nTmp, true, true, false);
					AddTooltipItemOption(88, string(item.Enchanted), true, true, false);					
				}
				else
				{
					// End:0xDE6
					if(__NFUN_154__(int(EEtcItemType), int(15)))
					{
						AddTooltipItemOption(972, string(item.Enchanted), true, true, false);						
					}
					else
					{
						// End:0xE3B
						if(__NFUN_154__(int(EEtcItemType), int(13)))
						{
							AddTooltipItemOption(670, string(item.Blessed), true, true, false);
							AddTooltipItemOption(671, GetLottoString(item.Enchanted, item.Damaged), true, true, false);							
						}
						else
						{
							// End:0xEA1
							if(__NFUN_154__(int(EEtcItemType), int(14)))
							{
								AddTooltipItemOption(670, string(item.Enchanted), true, true, false);
								AddTooltipItemOption(671, GetRaceTicketString(item.Blessed), true, true, false);
								AddTooltipItemOption(744, string(__NFUN_144__(item.Damaged, 100)), true, true, false);
							}
						}
					}
				}
				AddTooltipItemOption(52, string(item.Weight), true, true, false);
				// End:0xEBE
				break;
			// End:0xFFFF
			default:
				break;
		}
		// End:0x10CC
		if(__NFUN_130__(__NFUN_153__(item.CurrentDurability, 0), __NFUN_151__(item.Durability, 0)))
		{
			bLargeWidth = true;
			AddTooltipItemBlank(12);
			AddTooltipItemOption(1492, "", true, false, false);
			SetTooltipItemColor(255, 255, 255, 0);
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.bLineBreak = true;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color.R = 163;
			m_Info.t_color.G = 163;
			m_Info.t_color.B = 163;
			m_Info.t_color.A = byte(255);
			m_Info.t_ID = 1493;
			EndItem();
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.t_bDrawOneLine = true;
			// End:0x1023
			if(__NFUN_152__(__NFUN_146__(item.CurrentDurability, 1), 5))
			{
				m_Info.t_color.R = byte(255);
				m_Info.t_color.G = 0;
				m_Info.t_color.B = 0;				
			}
			else
			{
				m_Info.t_color.R = 176;
				m_Info.t_color.G = 155;
				m_Info.t_color.B = 121;
			}
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = __NFUN_112__(__NFUN_112__(__NFUN_112__(" ", string(item.CurrentDurability)), "/"), string(item.Durability));
			EndItem();
			AddTooltipItemOption(1491, "", true, false, false);
			// End:0x10CC
			if(__NFUN_151__(__NFUN_125__(item.Description), 0))
			{
				AddTooltipItemBlank(12);
			}
		}
		// End:0x1178
		if(__NFUN_151__(__NFUN_125__(item.Description), 0))
		{
			bLargeWidth = true;
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.bLineBreak = true;
			m_Info.t_color.R = 178;
			m_Info.t_color.G = 190;
			m_Info.t_color.B = 207;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = item.Description;
			EndItem();
		}
		// End:0x16B8
		if(__NFUN_151__(item.ClassID, 0))
		{
			idx = 0;
			J0x118F:

			// End:0x1535 [Loop If]
			if(__NFUN_150__(idx, 3))
			{
				Class'NWindow.UIDATA_ITEM'.static.GetSetItemIDList(item.ClassID, idx, arrID);
				SetID = 0;
				J0x11C5:

				// End:0x1384 [Loop If]
				if(__NFUN_150__(SetID, arrID.Length))
				{
					bLargeWidth = true;
					ClassID = arrID[SetID];
					// End:0x137A
					if(__NFUN_155__(item.ClassID, ClassID))
					{
						strTmp = Class'NWindow.UIDATA_ITEM'.static.GetItemName(ClassID);
						// End:0x137A
						if(__NFUN_151__(__NFUN_125__(strTmp), 0))
						{
							StartItem();
							m_Info.eType = 1;
							m_Info.nOffSetY = 6;
							m_Info.bLineBreak = true;
							m_Info.t_bDrawOneLine = true;
							m_Info.t_color.R = 112;
							m_Info.t_color.G = 115;
							m_Info.t_color.B = 123;
							m_Info.t_color.A = byte(255);
							m_Info.t_strText = strTmp;
							ParamAdd(m_Info.Condition, "Type", "Equip");
							ParamAdd(m_Info.Condition, "ServerID", string(item.ServerID));
							ParamAdd(m_Info.Condition, "EquipID", string(ClassID));
							ParamAdd(m_Info.Condition, "NormalColor", "112,115,123");
							ParamAdd(m_Info.Condition, "EnableColor", "176,185,205");
							EndItem();
						}
					}
					__NFUN_165__(SetID);
					// [Loop Continue]
					goto J0x11C5;
				}
				strTmp = Class'NWindow.UIDATA_ITEM'.static.GetSetItemEffectDescription(item.ClassID, idx);
				// End:0x152B
				if(__NFUN_151__(__NFUN_125__(strTmp), 0))
				{
					bLargeWidth = true;
					StartItem();
					m_Info.eType = 1;
					m_Info.nOffSetY = 6;
					m_Info.bLineBreak = true;
					m_Info.t_color.R = 128;
					m_Info.t_color.G = 127;
					m_Info.t_color.B = 103;
					m_Info.t_color.A = byte(255);
					m_Info.t_strText = strTmp;
					ParamAdd(m_Info.Condition, "Type", "SetEffect");
					ParamAdd(m_Info.Condition, "ServerID", string(item.ServerID));
					ParamAdd(m_Info.Condition, "ClassID", string(item.ClassID));
					ParamAdd(m_Info.Condition, "EffectID", string(idx));
					ParamAdd(m_Info.Condition, "NormalColor", "128,127,103");
					ParamAdd(m_Info.Condition, "EnableColor", "183,178,122");
					EndItem();
				}
				__NFUN_165__(idx);
				// [Loop Continue]
				goto J0x118F;
			}
			strTmp = Class'NWindow.UIDATA_ITEM'.static.GetSetItemEnchantEffectDescription(item.ClassID);
			// End:0x16B8
			if(__NFUN_151__(__NFUN_125__(strTmp), 0))
			{
				bLargeWidth = true;
				StartItem();
				m_Info.eType = 1;
				m_Info.nOffSetY = 6;
				m_Info.bLineBreak = true;
				m_Info.t_color.R = 74;
				m_Info.t_color.G = 92;
				m_Info.t_color.B = 104;
				m_Info.t_color.A = byte(255);
				m_Info.t_strText = strTmp;
				ParamAdd(m_Info.Condition, "Type", "EnchantEffect");
				ParamAdd(m_Info.Condition, "ServerID", string(item.ServerID));
				ParamAdd(m_Info.Condition, "ClassID", string(item.ClassID));
				ParamAdd(m_Info.Condition, "NormalColor", "74,92,104");
				ParamAdd(m_Info.Condition, "EnableColor", "111,146,169");
				EndItem();
			}
		}		
	}
	else
	{
		return;
	}
	// End:0x16D3
	if(bLargeWidth)
	{
		m_Tooltip.MinimumWidth = 144;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_ACTION(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local ItemInfo item;

	// End:0x149
	if(__NFUN_154__(int(eSourceType), int(1)))
	{
		ParseString(param, "Name", item.Name);
		ParseString(param, "Description", item.Description);
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_strText = item.Name;
		EndItem();
		// End:0x146
		if(__NFUN_151__(__NFUN_125__(item.Description), 0))
		{
			m_Tooltip.MinimumWidth = 144;
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.t_bDrawOneLine = false;
			m_Info.bLineBreak = true;
			m_Info.t_color.R = 178;
			m_Info.t_color.G = 190;
			m_Info.t_color.B = 207;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = item.Description;
			EndItem();
		}		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_SKILL(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local ItemInfo item;
	local UIEventManager.EItemParamType EItemParamType;
	local UIEventManager.EShortCutItemType eShortCutType;
	local int nTmp, SkillLevel;

	// End:0x569
	if(__NFUN_154__(int(eSourceType), int(1)))
	{
		ParseString(param, "Name", item.Name);
		ParseString(param, "AdditionalName", item.AdditionalName);
		ParseString(param, "Description", item.Description);
		ParseInt(param, "ClassID", item.ClassID);
		ParseInt(param, "Level", item.Level);
		eShortCutType = byte(item.ItemSubType);
		EItemParamType = byte(item.ItemType);
		SkillLevel = item.Level;
		m_Tooltip.MinimumWidth = 144;
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_strText = item.Name;
		EndItem();
		// End:0x1F7
		if(__NFUN_151__(__NFUN_125__(item.AdditionalName), 0))
		{
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetX = 5;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color.R = byte(255);
			m_Info.t_color.G = 217;
			m_Info.t_color.B = 105;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = item.AdditionalName;
			EndItem();
			SkillLevel = Class'NWindow.UIDATA_SKILL'.static.GetEnchantSkillLevel(item.ClassID, item.Level);
		}
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_strText = " ";
		EndItem();
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 163;
		m_Info.t_color.G = 163;
		m_Info.t_color.B = 163;
		m_Info.t_color.A = byte(255);
		m_Info.t_ID = 88;
		EndItem();
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 176;
		m_Info.t_color.G = 155;
		m_Info.t_color.B = 121;
		m_Info.t_color.A = byte(255);
		m_Info.t_strText = __NFUN_112__(" ", string(SkillLevel));
		EndItem();
		StartItem();
		m_Info.eType = 1;
		m_Info.nOffSetY = 6;
		m_Info.bLineBreak = true;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 176;
		m_Info.t_color.G = 155;
		m_Info.t_color.B = 121;
		m_Info.t_color.A = byte(255);
		m_Info.t_strText = Class'NWindow.UIDATA_SKILL'.static.GetOperateType(item.ClassID, item.Level);
		EndItem();
		nTmp = Class'NWindow.UIDATA_SKILL'.static.GetHpConsume(item.ClassID, item.Level);
		// End:0x430
		if(__NFUN_151__(nTmp, 0))
		{
			AddTooltipItemOption(1195, string(nTmp), true, true, false);
		}
		nTmp = Class'NWindow.UIDATA_SKILL'.static.GetMpConsume(item.ClassID, item.Level);
		// End:0x479
		if(__NFUN_151__(nTmp, 0))
		{
			AddTooltipItemOption(320, string(nTmp), true, true, false);
		}
		nTmp = Class'NWindow.UIDATA_SKILL'.static.GetCastRange(item.ClassID, item.Level);
		// End:0x4C2
		if(__NFUN_153__(nTmp, 0))
		{
			AddTooltipItemOption(321, string(nTmp), true, true, false);
		}
		// End:0x566
		if(__NFUN_151__(__NFUN_125__(item.Description), 0))
		{
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.bLineBreak = true;
			m_Info.t_color.R = 178;
			m_Info.t_color.G = 190;
			m_Info.t_color.B = 207;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = item.Description;
			EndItem();
		}		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_ABNORMALSTATUS(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local ItemInfo item;
	local UIEventManager.EItemParamType EItemParamType;
	local UIEventManager.EShortCutItemType eShortCutType;

	// End:0x601
	if(__NFUN_154__(int(eSourceType), int(1)))
	{
		ParseString(param, "Name", item.Name);
		ParseString(param, "AdditionalName", item.AdditionalName);
		ParseString(param, "Description", item.Description);
		ParseInt(param, "ClassID", item.ClassID);
		ParseInt(param, "Level", item.Level);
		ParseInt(param, "Reserved", item.Reserved);
		eShortCutType = byte(item.ItemSubType);
		EItemParamType = byte(item.ItemType);
		m_Tooltip.MinimumWidth = 144;
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_strText = item.Name;
		EndItem();
		// End:0x20B
		if(__NFUN_151__(__NFUN_125__(item.AdditionalName), 0))
		{
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetX = 5;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color.R = byte(255);
			m_Info.t_color.G = 217;
			m_Info.t_color.B = 105;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = item.AdditionalName;
			EndItem();
			item.Level = Class'NWindow.UIDATA_SKILL'.static.GetEnchantSkillLevel(item.ClassID, item.Level);
		}
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_strText = " ";
		EndItem();
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 163;
		m_Info.t_color.G = 163;
		m_Info.t_color.B = 163;
		m_Info.t_color.A = byte(255);
		m_Info.t_ID = 88;
		EndItem();
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 176;
		m_Info.t_color.G = 155;
		m_Info.t_color.B = 121;
		m_Info.t_color.A = byte(255);
		m_Info.t_strText = __NFUN_112__(" ", string(item.Level));
		EndItem();
		// End:0x55A
		if(__NFUN_130__(__NFUN_129__(IsDebuff(item.ClassID, item.Level)), __NFUN_153__(item.Reserved, 0)))
		{
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.bLineBreak = true;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color.R = 163;
			m_Info.t_color.G = 163;
			m_Info.t_color.B = 163;
			m_Info.t_color.A = byte(255);
			m_Info.t_ID = 1199;
			EndItem();
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color.R = 163;
			m_Info.t_color.G = 163;
			m_Info.t_color.B = 163;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = " : ";
			EndItem();
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color.R = 176;
			m_Info.t_color.G = 155;
			m_Info.t_color.B = 121;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = MakeBuffTimeStr(item.Reserved);
			ParamAdd(m_Info.Condition, "Type", "RemainTime");
			EndItem();
		}
		// End:0x5FE
		if(__NFUN_151__(__NFUN_125__(item.Description), 0))
		{
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.bLineBreak = true;
			m_Info.t_color.R = 178;
			m_Info.t_color.G = 190;
			m_Info.t_color.B = 207;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = item.Description;
			EndItem();
		}		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_NORMALITEM(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local ItemInfo item;

	// End:0x168
	if(__NFUN_154__(int(eSourceType), int(1)))
	{
		ParseString(param, "Name", item.Name);
		ParseString(param, "Description", item.Description);
		ParseString(param, "AdditionalName", item.AdditionalName);
		ParseInt(param, "CrystalType", item.CrystalType);
		AddTooltipItemName(item.Name, item);
		AddTooltipItemGrade(item);
		// End:0x165
		if(__NFUN_151__(__NFUN_125__(item.Description), 0))
		{
			m_Tooltip.MinimumWidth = 144;
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.bLineBreak = true;
			m_Info.t_color.R = 178;
			m_Info.t_color.G = 190;
			m_Info.t_color.B = 207;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = item.Description;
			EndItem();
		}		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_RECIPE(string param, UIEventManager.ETooltipSourceType eSourceType, bool bShowPrice)
{
	local ItemInfo item;
	local string strAdena, strAdenaComma;
	local Color AdenaColor;

	// End:0x2D5
	if(__NFUN_154__(int(eSourceType), int(1)))
	{
		ParseString(param, "Name", item.Name);
		ParseString(param, "Description", item.Description);
		ParseString(param, "AdditionalName", item.AdditionalName);
		ParseInt(param, "CrystalType", item.CrystalType);
		ParseInt(param, "Weight", item.Weight);
		ParseInt(param, "Price", item.Price);
		AddTooltipItemName(item.Name, item);
		AddTooltipItemGrade(item);
		// End:0x20A
		if(bShowPrice)
		{
			strAdena = string(item.Price);
			strAdenaComma = MakeCostString(strAdena);
			AdenaColor = GetNumericColor(strAdenaComma);
			AddTooltipItemOption(641, __NFUN_112__(strAdenaComma, " "), true, true, false);
			SetTooltipItemColor(int(AdenaColor.R), int(AdenaColor.G), int(AdenaColor.B), 0);
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color = AdenaColor;
			m_Info.t_ID = 469;
			EndItem();
			AddTooltipItemOption(0, __NFUN_112__(__NFUN_112__("(", ConvertNumToText(strAdena)), ")"), false, true, false);
			SetTooltipItemColor(int(AdenaColor.R), int(AdenaColor.G), int(AdenaColor.B), 0);
		}
		AddTooltipItemOption(52, string(item.Weight), true, true, false);
		// End:0x2D2
		if(__NFUN_151__(__NFUN_125__(item.Description), 0))
		{
			m_Tooltip.MinimumWidth = 144;
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.bLineBreak = true;
			m_Info.t_color.R = 178;
			m_Info.t_color.G = 190;
			m_Info.t_color.B = 207;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = item.Description;
			EndItem();
		}		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_SHORTCUT(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local ItemInfo item;
	local UIEventManager.EItemParamType EItemParamType;
	local UIEventManager.EShortCutItemType eShortCutType;
	local string ItemName;

	// End:0x5B3
	if(__NFUN_154__(int(eSourceType), int(1)))
	{
		ParseString(param, "Name", item.Name);
		ParseString(param, "AdditionalName", item.AdditionalName);
		ParseInt(param, "ClassID", item.ClassID);
		ParseInt(param, "Level", item.Level);
		ParseInt(param, "Reserved", item.Reserved);
		ParseInt(param, "Enchanted", item.Enchanted);
		ParseInt(param, "ItemType", item.ItemType);
		ParseInt(param, "ItemSubType", item.ItemSubType);
		ParseInt(param, "CrystalType", item.CrystalType);
		ParseInt(param, "ConsumeType", item.ConsumeType);
		ParseInt(param, "RefineryOp1", item.RefineryOp1);
		ParseInt(param, "RefineryOp2", item.RefineryOp2);
		ParseInt(param, "ItemNum", item.ItemNum);
		ParseInt(param, "MpConsume", item.MpConsume);
		eShortCutType = byte(item.ItemSubType);
		EItemParamType = byte(item.ItemType);
		ItemName = Class'NWindow.UIDATA_ITEM'.static.GetRefineryItemName(item.Name, item.RefineryOp1, item.RefineryOp2);
		switch(eShortCutType)
		{
			// End:0x267
			case 1:
				AddTooltipItemEnchant(item);
				AddTooltipItemName(ItemName, item);
				AddTooltipItemGrade(item);
				AddTooltipItemCount(item);
				// End:0x5B0
				break;
			// End:0x565
			case 2:
				StartItem();
				m_Info.eType = 1;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_strText = ItemName;
				EndItem();
				// End:0x376
				if(__NFUN_151__(__NFUN_125__(item.AdditionalName), 0))
				{
					StartItem();
					m_Info.eType = 1;
					m_Info.nOffSetX = 5;
					m_Info.t_bDrawOneLine = true;
					m_Info.t_color.R = byte(255);
					m_Info.t_color.G = 217;
					m_Info.t_color.B = 105;
					m_Info.t_color.A = byte(255);
					m_Info.t_strText = item.AdditionalName;
					EndItem();
					item.Level = Class'NWindow.UIDATA_SKILL'.static.GetEnchantSkillLevel(item.ClassID, item.Level);
				}
				StartItem();
				m_Info.eType = 1;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_strText = " ";
				EndItem();
				StartItem();
				m_Info.eType = 1;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_color.R = 163;
				m_Info.t_color.G = 163;
				m_Info.t_color.B = 163;
				m_Info.t_color.A = byte(255);
				m_Info.t_ID = 88;
				EndItem();
				StartItem();
				m_Info.eType = 1;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_color.R = 176;
				m_Info.t_color.G = 155;
				m_Info.t_color.B = 121;
				m_Info.t_color.A = byte(255);
				m_Info.t_strText = __NFUN_112__(" ", string(item.Level));
				EndItem();
				StartItem();
				m_Info.eType = 1;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_strText = " (";
				EndItem();
				StartItem();
				m_Info.eType = 1;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_ID = 91;
				EndItem();
				StartItem();
				m_Info.eType = 1;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_strText = __NFUN_112__(__NFUN_112__(":", string(item.MpConsume)), ")");
				EndItem();
				// End:0x5B0
				break;
			// End:0x56A
			case 3:
			// End:0x56F
			case 4:
			// End:0x5AD
			case 5:
				StartItem();
				m_Info.eType = 1;
				m_Info.t_bDrawOneLine = true;
				m_Info.t_strText = ItemName;
				EndItem();
				// End:0x5B0
				break;
			// End:0xFFFF
			default:
				break;
		}		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_RECIPE_MANUFACTURE(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local ItemInfo item;

	// End:0x1D9
	if(__NFUN_154__(int(eSourceType), int(1)))
	{
		ParseString(param, "Name", item.Name);
		ParseString(param, "Description", item.Description);
		ParseString(param, "AdditionalName", item.AdditionalName);
		ParseInt(param, "Reserved", item.Reserved);
		ParseInt(param, "CrystalType", item.CrystalType);
		ParseInt(param, "ItemNum", item.ItemNum);
		m_Tooltip.MinimumWidth = 144;
		AddTooltipItemName(item.Name, item);
		AddTooltipItemGrade(item);
		AddTooltipItemOption(736, string(item.Reserved), true, true, false);
		AddTooltipItemOption(737, string(item.ItemNum), true, true, false);
		// End:0x1D6
		if(__NFUN_151__(__NFUN_125__(item.Description), 0))
		{
			StartItem();
			m_Info.eType = 1;
			m_Info.nOffSetY = 6;
			m_Info.bLineBreak = true;
			m_Info.t_color.R = 178;
			m_Info.t_color.G = 190;
			m_Info.t_color.B = 207;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = item.Description;
			EndItem();
		}		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_CLANINFO(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local LVDataRecord Record;

	// End:0x4B
	if(__NFUN_154__(int(eSourceType), int(2)))
	{
		ParamToRecord(param, Record);
		AddTooltipItemOption(391, GetClassType(int(Record.LVDataList[2].szData)), true, true, true);		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_PARTYMATCH(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local LVDataRecord Record;

	// End:0x4A
	if(__NFUN_154__(int(eSourceType), int(2)))
	{
		ParamToRecord(param, Record);
		AddTooltipItemOption(391, GetClassType(int(Record.LVDataList[1].szData)), true, true, true);		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_QUESTLIST(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local LVDataRecord Record;
	local int nTmp;

	// End:0x9A
	if(__NFUN_154__(int(eSourceType), int(2)))
	{
		ParamToRecord(param, Record);
		AddTooltipItemOption(1200, Record.LVDataList[0].szData, true, true, true);
		switch(Record.LVDataList[3].nReserved1)
		{
			// End:0x57
			case 0:
			// End:0x6A
			case 2:
				nTmp = 861;
				// End:0x84
				break;
			// End:0x6E
			case 1:
			// End:0x81
			case 3:
				nTmp = 862;
				// End:0x84
				break;
			// End:0xFFFF
			default:
				break;
		}
		AddTooltipItemOption2(1202, nTmp, true, true, false);		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_RAIDLIST(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local LVDataRecord Record;

	// End:0xC9
	if(__NFUN_154__(int(eSourceType), int(2)))
	{
		ParamToRecord(param, Record);
		// End:0x34
		if(__NFUN_150__(__NFUN_125__(Record.szReserved), 1))
		{
			return;
		}
		m_Tooltip.MinimumWidth = 144;
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = false;
		m_Info.t_color.R = 178;
		m_Info.t_color.G = 190;
		m_Info.t_color.B = 207;
		m_Info.t_color.A = byte(255);
		m_Info.t_strText = Record.szReserved;
		EndItem();		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_QUESTINFO(string param, UIEventManager.ETooltipSourceType eSourceType)
{
	local LVDataRecord Record;
	local int nTmp, Width1, Width2, Height;

	// End:0x224
	if(__NFUN_154__(int(eSourceType), int(2)))
	{
		ParamToRecord(param, Record);
		AddTooltipItemOption(1200, Record.LVDataList[0].szData, true, true, true);
		AddTooltipItemOption(1201, Record.LVDataList[1].szData, true, true, false);
		GetTextSize(__NFUN_112__(__NFUN_112__(GetSystemString(1200), " : "), Record.LVDataList[0].szData), Width1, Height);
		GetTextSize(__NFUN_112__(__NFUN_112__(GetSystemString(1201), " : "), Record.LVDataList[1].szData), Width2, Height);
		// End:0xE2
		if(__NFUN_151__(Width2, Width1))
		{
			Width1 = Width2;
		}
		// End:0xF6
		if(__NFUN_151__(144, Width1))
		{
			Width1 = 144;
		}
		m_Tooltip.MinimumWidth = __NFUN_146__(Width1, 30);
		AddTooltipItemOption(922, Record.LVDataList[2].szData, true, true, false);
		switch(Record.LVDataList[3].nReserved1)
		{
			// End:0x142
			case 0:
			// End:0x155
			case 2:
				nTmp = 861;
				// End:0x16F
				break;
			// End:0x159
			case 1:
			// End:0x16C
			case 3:
				nTmp = 862;
				// End:0x16F
				break;
			// End:0xFFFF
			default:
				break;
		}
		AddTooltipItemOption2(1202, nTmp, true, true, false);
		StartItem();
		m_Info.eType = 1;
		m_Info.nOffSetY = 6;
		m_Info.t_bDrawOneLine = false;
		m_Info.bLineBreak = true;
		m_Info.t_color.R = 178;
		m_Info.t_color.G = 190;
		m_Info.t_color.B = 207;
		m_Info.t_color.A = byte(255);
		m_Info.t_strText = Record.szReserved;
		EndItem();		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function ReturnTooltip_NTT_MANOR(string param, string TooltipType, UIEventManager.ETooltipSourceType eSourceType)
{
	local LVDataRecord Record;
	local int idx1, idx2, idx3;

	// End:0x1E4
	if(__NFUN_154__(int(eSourceType), int(2)))
	{
		ParamToRecord(param, Record);
		// End:0x54
		if(__NFUN_122__(TooltipType, "ManorSeedInfo"))
		{
			idx1 = 4;
			idx2 = 5;
			idx3 = 6;			
		}
		else
		{
			// End:0x88
			if(__NFUN_122__(TooltipType, "ManorCropInfo"))
			{
				idx1 = 5;
				idx2 = 6;
				idx3 = 7;				
			}
			else
			{
				// End:0xBF
				if(__NFUN_122__(TooltipType, "ManorSeedSetting"))
				{
					idx1 = 7;
					idx2 = 8;
					idx3 = 9;					
				}
				else
				{
					// End:0xF6
					if(__NFUN_122__(TooltipType, "ManorCropSetting"))
					{
						idx1 = 9;
						idx2 = 10;
						idx3 = 11;						
					}
					else
					{
						// End:0x12C
						if(__NFUN_122__(TooltipType, "ManorDefaultInfo"))
						{
							idx1 = 1;
							idx2 = 4;
							idx3 = 5;							
						}
						else
						{
							// End:0x15D
							if(__NFUN_122__(TooltipType, "ManorCropSell"))
							{
								idx1 = 7;
								idx2 = 8;
								idx3 = 9;
							}
						}
					}
				}
			}
		}
		AddTooltipItemOption(0, Record.LVDataList[0].szData, false, true, true);
		AddTooltipItemOption(537, Record.LVDataList[idx1].szData, true, true, false);
		AddTooltipItemOption(1134, Record.LVDataList[idx2].szData, true, true, false);
		AddTooltipItemOption(1135, Record.LVDataList[idx3].szData, true, true, false);		
	}
	else
	{
		return;
	}
	ReturnTooltipInfo(m_Tooltip);
	return;
}

function AddTooltipItemOption(int TitleID, string Content, bool bTitle, bool bContent, bool IamFirst)
{
	// End:0xAE
	if(bTitle)
	{
		StartItem();
		m_Info.eType = 1;
		// End:0x34
		if(__NFUN_129__(IamFirst))
		{
			m_Info.nOffSetY = 6;
		}
		m_Info.bLineBreak = true;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 163;
		m_Info.t_color.G = 163;
		m_Info.t_color.B = 163;
		m_Info.t_color.A = byte(255);
		m_Info.t_ID = TitleID;
		EndItem();
	}
	// End:0x208
	if(bContent)
	{
		// End:0x158
		if(bTitle)
		{
			StartItem();
			m_Info.eType = 1;
			// End:0xEB
			if(__NFUN_129__(IamFirst))
			{
				m_Info.nOffSetY = 6;
			}
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color.R = 163;
			m_Info.t_color.G = 163;
			m_Info.t_color.B = 163;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = " : ";
			EndItem();
		}
		StartItem();
		m_Info.eType = 1;
		// End:0x183
		if(__NFUN_129__(IamFirst))
		{
			m_Info.nOffSetY = 6;
		}
		// End:0x19B
		if(__NFUN_129__(bTitle))
		{
			m_Info.bLineBreak = true;
		}
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 176;
		m_Info.t_color.G = 155;
		m_Info.t_color.B = 121;
		m_Info.t_color.A = byte(255);
		m_Info.t_strText = Content;
		EndItem();
	}
	return;
}

function AddTooltipItemOption2(int TitleID, int ContentID, bool bTitle, bool bContent, bool IamFirst)
{
	// End:0xAE
	if(bTitle)
	{
		StartItem();
		m_Info.eType = 1;
		// End:0x34
		if(__NFUN_129__(IamFirst))
		{
			m_Info.nOffSetY = 6;
		}
		m_Info.bLineBreak = true;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 163;
		m_Info.t_color.G = 163;
		m_Info.t_color.B = 163;
		m_Info.t_color.A = byte(255);
		m_Info.t_ID = TitleID;
		EndItem();
	}
	// End:0x208
	if(bContent)
	{
		// End:0x158
		if(bTitle)
		{
			StartItem();
			m_Info.eType = 1;
			// End:0xEB
			if(__NFUN_129__(IamFirst))
			{
				m_Info.nOffSetY = 6;
			}
			m_Info.t_bDrawOneLine = true;
			m_Info.t_color.R = 163;
			m_Info.t_color.G = 163;
			m_Info.t_color.B = 163;
			m_Info.t_color.A = byte(255);
			m_Info.t_strText = " : ";
			EndItem();
		}
		StartItem();
		m_Info.eType = 1;
		// End:0x183
		if(__NFUN_129__(IamFirst))
		{
			m_Info.nOffSetY = 6;
		}
		// End:0x19B
		if(__NFUN_129__(bTitle))
		{
			m_Info.bLineBreak = true;
		}
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 176;
		m_Info.t_color.G = 155;
		m_Info.t_color.B = 121;
		m_Info.t_color.A = byte(255);
		m_Info.t_ID = ContentID;
		EndItem();
	}
	return;
}

function SetTooltipItemColor(int R, int G, int B, int offset)
{
	local int idx;

	idx = __NFUN_147__(__NFUN_147__(m_Tooltip.DrawList.Length, 1), offset);
	m_Tooltip.DrawList[idx].t_color.R = byte(R);
	m_Tooltip.DrawList[idx].t_color.G = byte(G);
	m_Tooltip.DrawList[idx].t_color.B = byte(B);
	m_Tooltip.DrawList[idx].t_color.A = byte(255);
	return;
}

function AddTooltipItemBlank(int Height)
{
	StartItem();
	m_Info.eType = 0;
	m_Info.b_nHeight = Height;
	EndItem();
	return;
}

function AddTooltipItemEnchant(ItemInfo item)
{
	local UIEventManager.EItemParamType EItemParamType;

	EItemParamType = byte(item.ItemType);
	// End:0xC3
	if(__NFUN_130__(__NFUN_151__(item.Enchanted, 0), IsEnchantableItem(EItemParamType)))
	{
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = 176;
		m_Info.t_color.G = 155;
		m_Info.t_color.B = 121;
		m_Info.t_color.A = byte(255);
		m_Info.t_strText = __NFUN_112__(__NFUN_112__("+", string(item.Enchanted)), " ");
		EndItem();
	}
	return;
}

function AddTooltipItemName(string Name, ItemInfo item)
{
	StartItem();
	m_Info.eType = 1;
	m_Info.t_bDrawOneLine = true;
	m_Info.t_strText = Name;
	EndItem();
	// End:0xD4
	if(__NFUN_151__(__NFUN_125__(item.AdditionalName), 0))
	{
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_color.R = byte(255);
		m_Info.t_color.G = 217;
		m_Info.t_color.B = 105;
		m_Info.t_color.A = byte(255);
		m_Info.t_strText = __NFUN_112__(" ", item.AdditionalName);
		EndItem();
	}
	return;
}

function AddTooltipItemGrade(ItemInfo item)
{
	local string strTmp;

	strTmp = GetItemGradeString(item.CrystalType);
	// End:0x97
	if(__NFUN_151__(__NFUN_125__(strTmp), 0))
	{
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_strText = " ";
		EndItem();
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_strText = __NFUN_112__(__NFUN_112__("`", strTmp), "`");
		EndItem();
	}
	return;
}

function AddTooltipItemCount(ItemInfo item)
{
	// End:0x61
	if(IsStackableItem(item.ConsumeType))
	{
		StartItem();
		m_Info.eType = 1;
		m_Info.t_bDrawOneLine = true;
		m_Info.t_strText = __NFUN_112__(__NFUN_112__(" (", MakeCostString(string(item.ItemNum))), ")");
		EndItem();
	}
	return;
}

function GetRefineryColor(int Quality, out int R, out int G, out int B)
{
	switch(Quality)
	{
		// End:0x26
		case 1:
			R = 187;
			G = 181;
			B = 138;
			// End:0xA4
			break;
		// End:0x46
		case 2:
			R = 132;
			G = 174;
			B = 216;
			// End:0xA4
			break;
		// End:0x66
		case 3:
			R = 193;
			G = 112;
			B = 202;
			// End:0xA4
			break;
		// End:0x86
		case 4:
			R = 225;
			G = 109;
			B = 109;
			// End:0xA4
			break;
		// End:0xFFFF
		default:
			R = 187;
			G = 181;
			B = 138;
			// End:0xA4
			break;
			break;
	}
	return;
}
