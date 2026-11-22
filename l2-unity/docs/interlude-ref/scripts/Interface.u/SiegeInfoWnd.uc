class SiegeInfoWnd extends UICommonAPI;

var bool m_bShow;
var int m_CastleID;
var string m_CastleName;
var int m_PlayerClanID;
var bool m_IsCastleOwner;
var int m_SiegeTime;
var array<int> m_SelectableTimeArray;
var bool m_IsExistMyClanIDinAttackSide;
var bool m_IsExistMyClanIDinDefenseSide;
var int m_AcceptedClan;
var int m_WaitingClan;
var int m_DialogClanID;
var int m_DialogSelectedTimeID;
var WindowHandle m_wndTop;
var TabHandle TabCtrl;
var TextBoxHandle txtCastleName;
var TextBoxHandle txtOwnerName;
var TextBoxHandle txtClanName;
var TextBoxHandle txtAllianceName;
var TextureHandle texClan;
var TextureHandle texAlliance;
var TextBoxHandle txtCurTime;
var TextBoxHandle txtSiegeTime;
var ComboBoxHandle cboTime;
var ListCtrlHandle lstAttackClan;
var TextBoxHandle txtAttackCount;
var ButtonHandle btnAttackApply;
var ButtonHandle btnAttackCancel;
var ListCtrlHandle lstDefenseClan;
var TextBoxHandle txtDefenseCount;
var ButtonHandle btnDefenseApply;
var ButtonHandle btnDefenseCancel;
var ButtonHandle btnDefenseReject;
var ButtonHandle btnDefenseConfirm;

function OnLoad()
{
	RegisterEvent(1710);
	RegisterEvent(1450);
	RegisterEvent(1460);
	RegisterEvent(1470);
	RegisterEvent(1480);
	RegisterEvent(1490);
	m_bShow = false;
	m_CastleID = 0;
	m_CastleName = "";
	m_SiegeTime = 0;
	m_AcceptedClan = 0;
	m_WaitingClan = 0;
	m_wndTop = GetHandle("SiegeInfoWnd");
	TabCtrl = TabHandle(GetHandle("SiegeInfoWnd.TabCtrl"));
	txtCastleName = TextBoxHandle(GetHandle("SiegeInfoWnd.txtCastleName"));
	txtOwnerName = TextBoxHandle(GetHandle("SiegeInfoWnd.txtOwnerName"));
	txtClanName = TextBoxHandle(GetHandle("SiegeInfoWnd.txtClanName"));
	txtAllianceName = TextBoxHandle(GetHandle("SiegeInfoWnd.txtAllianceName"));
	texClan = TextureHandle(GetHandle("SiegeInfoWnd.texClan"));
	texAlliance = TextureHandle(GetHandle("SiegeInfoWnd.texAlliance"));
	txtCurTime = TextBoxHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Date.txtCurTime"));
	txtSiegeTime = TextBoxHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Date.txtSiegeTime"));
	cboTime = ComboBoxHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Date.cboTime"));
	lstAttackClan = ListCtrlHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party1.lstClan"));
	txtAttackCount = TextBoxHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party1.txtCount"));
	btnAttackApply = ButtonHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party1.btnAttackApply"));
	btnAttackCancel = ButtonHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party1.btnAttackCancel"));
	lstDefenseClan = ListCtrlHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party2.lstClan"));
	txtDefenseCount = TextBoxHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party2.txtCount"));
	btnDefenseApply = ButtonHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party2.btnDefenseApply"));
	btnDefenseCancel = ButtonHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party2.btnDefenseCancel"));
	btnDefenseReject = ButtonHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party2.btnDefenseReject"));
	btnDefenseConfirm = ButtonHandle(GetHandle("SiegeInfoWnd.SiegeInfoWnd_Party2.btnDefenseConfirm"));
	UpdateAttackCount();
	UpdateDefenseCount();
	return;
}

function OnShow()
{
	m_bShow = true;
	return;
}

function OnHide()
{
	m_bShow = false;
	return;
}

function OnEnterState(name a_PreStateName)
{
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x1D
	if(__NFUN_154__(Event_ID, 1450))
	{
		HandleSiegeInfo(param);		
	}
	else
	{
		// End:0x3A
		if(__NFUN_154__(Event_ID, 1460))
		{
			HandleSiegeInfoClanListStart(param);			
		}
		else
		{
			// End:0x57
			if(__NFUN_154__(Event_ID, 1470))
			{
				HandleSiegeInfoClanList(param);				
			}
			else
			{
				// End:0x74
				if(__NFUN_154__(Event_ID, 1480))
				{
					HandleSiegeInfoClanListEnd(param);					
				}
				else
				{
					// End:0x91
					if(__NFUN_154__(Event_ID, 1490))
					{
						HandleSiegeInfoSelectableTime(param);						
					}
					else
					{
						// End:0x1F0
						if(__NFUN_154__(Event_ID, 1710))
						{
							// End:0x1F0
							if(DialogIsMine())
							{
								// End:0xCE
								if(__NFUN_154__(DialogGetID(), 1))
								{
									Class'NWindow.SiegeAPI'.static.RequestJoinCastleSiege(m_CastleID, 1, 1);									
								}
								else
								{
									// End:0xF4
									if(__NFUN_154__(DialogGetID(), 2))
									{
										Class'NWindow.SiegeAPI'.static.RequestJoinCastleSiege(m_CastleID, 1, 0);										
									}
									else
									{
										// End:0x11A
										if(__NFUN_154__(DialogGetID(), 3))
										{
											Class'NWindow.SiegeAPI'.static.RequestJoinCastleSiege(m_CastleID, 0, 1);											
										}
										else
										{
											// End:0x140
											if(__NFUN_154__(DialogGetID(), 4))
											{
												Class'NWindow.SiegeAPI'.static.RequestJoinCastleSiege(m_CastleID, 0, 0);												
											}
											else
											{
												// End:0x17C
												if(__NFUN_154__(DialogGetID(), 5))
												{
													// End:0x179
													if(__NFUN_151__(m_DialogClanID, 0))
													{
														Class'NWindow.SiegeAPI'.static.RequestConfirmCastleSiegeWaitingList(m_CastleID, m_DialogClanID, 0);
														m_DialogClanID = 0;
													}													
												}
												else
												{
													// End:0x1B8
													if(__NFUN_154__(DialogGetID(), 6))
													{
														// End:0x1B5
														if(__NFUN_151__(m_DialogClanID, 0))
														{
															Class'NWindow.SiegeAPI'.static.RequestConfirmCastleSiegeWaitingList(m_CastleID, m_DialogClanID, 1);
															m_DialogClanID = 0;
														}														
													}
													else
													{
														// End:0x1F0
														if(__NFUN_154__(DialogGetID(), 7))
														{
															// End:0x1F0
															if(__NFUN_151__(m_DialogSelectedTimeID, 0))
															{
																Class'NWindow.SiegeAPI'.static.RequestSetCastleSiegeTime(m_CastleID, m_DialogSelectedTimeID);
																m_DialogSelectedTimeID = 0;
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

function OnComboBoxItemSelected(string strID, int Index)
{
	// End:0x69
	if(__NFUN_122__(strID, "cboTime"))
	{
		// End:0x69
		if(__NFUN_151__(Index, 0))
		{
			m_DialogSelectedTimeID = m_SelectableTimeArray[__NFUN_147__(Index, 1)];
			DialogShow(0, MakeFullSystemMsg(GetSystemMessage(663), cboTime.GetString(Index), ""));
			DialogSetID(7);
		}
	}
	return;
}

function ClearInfo()
{
	local Rect rectWnd;

	rectWnd = m_wndTop.GetRect();
	m_CastleID = 0;
	m_CastleName = "";
	m_IsCastleOwner = false;
	m_SiegeTime = 0;
	txtCurTime.SetText("");
	txtSiegeTime.SetText("");
	txtCastleName.SetText("");
	txtOwnerName.SetText(GetSystemString(595));
	txtClanName.SetText("");
	txtAllianceName.SetText("");
	texClan.SetTexture("");
	texAlliance.SetTexture("");
	txtClanName.MoveTo(__NFUN_146__(rectWnd.nX, 80), __NFUN_146__(rectWnd.nY, 86));
	txtAllianceName.MoveTo(__NFUN_146__(rectWnd.nX, 80), __NFUN_146__(rectWnd.nY, 102));
	TabCtrl.SetTopOrder(0, true);
	ClearTimeCombo();
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x23
		case "btnAttackApply":
			OnAttackApplyClick();
			// End:0xFA
			break;
		// End:0x40
		case "btnAttackCancel":
			OnAttackCancelClick();
			// End:0xFA
			break;
		// End:0x5D
		case "btnDefenseApply":
			OnDefenseApplyClick();
			// End:0xFA
			break;
		// End:0x7B
		case "btnDefenseCancel":
			OnDefenseCancelClick();
			// End:0xFA
			break;
		// End:0x99
		case "btnDefenseReject":
			OnDefenseRejectClick();
			// End:0xFA
			break;
		// End:0xB8
		case "btnDefenseConfirm":
			OnDefenseConfirmClick();
			// End:0xFA
			break;
		// End:0xCB
		case "TabCtrl0":
			OnTabCtrl0Click();
		// End:0xE1
		case "TabCtrl1":
			OnTabCtrl1Click();
			// End:0xFA
			break;
		// End:0xF7
		case "TabCtrl2":
			OnTabCtrl2Click();
			// End:0xFA
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnAttackApplyClick()
{
	DialogShow(0, MakeFullSystemMsg(GetSystemMessage(667), m_CastleName, ""));
	DialogSetID(1);
	return;
}

function OnAttackCancelClick()
{
	DialogShow(0, MakeFullSystemMsg(GetSystemMessage(669), m_CastleName, ""));
	DialogSetID(2);
	return;
}

function OnDefenseApplyClick()
{
	DialogShow(0, MakeFullSystemMsg(GetSystemMessage(668), m_CastleName, ""));
	DialogSetID(3);
	return;
}

function OnDefenseCancelClick()
{
	DialogShow(0, MakeFullSystemMsg(GetSystemMessage(669), m_CastleName, ""));
	DialogSetID(4);
	return;
}

function OnDefenseRejectClick()
{
	local int idx, clanID;
	local string ClanName;
	local int Status;
	local UIEventManager.ECastleSiegeDefenderType DefenderType;
	local LVDataRecord Record;

	idx = lstDefenseClan.GetSelectedIndex();
	// End:0xE5
	if(__NFUN_151__(idx, -1))
	{
		Record = lstDefenseClan.GetRecord(idx);
		clanID = Record.nReserved1;
		Status = Record.nReserved2;
		DefenderType = byte(Status);
		// End:0xE5
		if(__NFUN_151__(clanID, 0))
		{
			// End:0xE5
			if(__NFUN_132__(__NFUN_154__(int(DefenderType), int(2)), __NFUN_154__(int(DefenderType), int(3))))
			{
				ClanName = Class'NWindow.UIDATA_CLAN'.static.GetName(clanID);
				m_DialogClanID = clanID;
				DialogShow(0, MakeFullSystemMsg(GetSystemMessage(670), ClanName, ""));
				DialogSetID(5);
			}
		}
	}
	return;
}

function OnDefenseConfirmClick()
{
	local int idx, clanID;
	local string ClanName;
	local int Status;
	local UIEventManager.ECastleSiegeDefenderType DefenderType;
	local LVDataRecord Record;

	idx = lstDefenseClan.GetSelectedIndex();
	// End:0xD3
	if(__NFUN_151__(idx, -1))
	{
		Record = lstDefenseClan.GetRecord(idx);
		clanID = Record.nReserved1;
		Status = Record.nReserved2;
		DefenderType = byte(Status);
		// End:0xD3
		if(__NFUN_151__(clanID, 0))
		{
			// End:0xD3
			if(__NFUN_154__(int(DefenderType), int(2)))
			{
				ClanName = Class'NWindow.UIDATA_CLAN'.static.GetName(clanID);
				m_DialogClanID = clanID;
				DialogShow(0, MakeFullSystemMsg(GetSystemMessage(671), ClanName, ""));
				DialogSetID(6);
			}
		}
	}
	return;
}

function OnTabCtrl0Click()
{
	UpdateTimeCombo();
	return;
}

function OnTabCtrl1Click()
{
	ClearAttackButton();
	Class'NWindow.SiegeAPI'.static.RequestCastleSiegeAttackerList(m_CastleID);
	return;
}

function OnTabCtrl2Click()
{
	ClearDefenseButton();
	Class'NWindow.SiegeAPI'.static.RequestCastleSiegeDefenderList(m_CastleID);
	return;
}

function HandleSiegeInfo(string param)
{
	local Rect rectWnd;
	local int castleID, IsOwner;
	local string OwnerName;
	local int clanID;
	local string ClanName;
	local int AllianceID;
	local string AllianceName;
	local int NowTime, SiegeTime;
	local string CastleName;
	local Texture ClanCrestTexture, AllianceCrestTexture;

	ClearInfo();
	rectWnd = m_wndTop.GetRect();
	ParseInt(param, "CastleID", castleID);
	ParseInt(param, "IsOwner", IsOwner);
	// End:0x61
	if(__NFUN_154__(IsOwner, 1))
	{
		m_IsCastleOwner = true;
	}
	ParseString(param, "OwnerName", OwnerName);
	ParseInt(param, "ClanID", clanID);
	ParseString(param, "ClanName", ClanName);
	ParseInt(param, "AllianceID", AllianceID);
	ParseString(param, "AllianceName", AllianceName);
	ParseInt(param, "NowTime", NowTime);
	ParseInt(param, "SiegeTime", SiegeTime);
	m_SiegeTime = SiegeTime;
	CastleName = GetCastleName(castleID);
	m_CastleID = castleID;
	m_CastleName = CastleName;
	txtCastleName.SetText(CastleName);
	// End:0x183
	if(__NFUN_151__(__NFUN_125__(OwnerName), 0))
	{
		txtOwnerName.SetText(OwnerName);
	}
	// End:0x1A4
	if(__NFUN_151__(__NFUN_125__(ClanName), 0))
	{
		txtClanName.SetText(ClanName);
	}
	// End:0x1C8
	if(__NFUN_151__(__NFUN_125__(AllianceName), 0))
	{
		txtAllianceName.SetText(AllianceName);		
	}
	else
	{
		// End:0x1EF
		if(__NFUN_151__(__NFUN_125__(ClanName), 0))
		{
			txtAllianceName.SetText(GetSystemString(591));
		}
	}
	// End:0x255
	if(__NFUN_151__(clanID, 0))
	{
		// End:0x255
		if(Class'NWindow.UIDATA_CLAN'.static.GetCrestTexture(clanID, ClanCrestTexture))
		{
			texClan.SetTextureWithObject(ClanCrestTexture);
			txtClanName.MoveTo(__NFUN_146__(rectWnd.nX, 100), __NFUN_146__(rectWnd.nY, 86));
		}
	}
	// End:0x2BB
	if(__NFUN_151__(AllianceID, 0))
	{
		// End:0x2BB
		if(Class'NWindow.UIDATA_CLAN'.static.GetAllianceCrestTexture(clanID, AllianceCrestTexture))
		{
			texAlliance.SetTextureWithObject(AllianceCrestTexture);
			txtAllianceName.MoveTo(__NFUN_146__(rectWnd.nX, 100), __NFUN_146__(rectWnd.nY, 102));
		}
	}
	// End:0x2E0
	if(__NFUN_151__(NowTime, 0))
	{
		txtCurTime.SetText(ConvertTimetoStr(NowTime));
	}
	// End:0x308
	if(__NFUN_151__(SiegeTime, 0))
	{
		txtSiegeTime.SetText(ConvertTimetoStr(SiegeTime));		
	}
	else
	{
		// End:0x32D
		if(__NFUN_129__(m_IsCastleOwner))
		{
			txtSiegeTime.SetText(GetSystemString(584));
		}
	}
	m_wndTop.ShowWindow();
	m_wndTop.SetFocus();
	UpdateTimeCombo();
	return;
}

function HandleSiegeInfoClanListStart(string param)
{
	local int Type;
	local UserInfo infUser;

	m_PlayerClanID = 0;
	// End:0x25
	if(GetPlayerInfo(infUser))
	{
		m_PlayerClanID = infUser.nClanID;
	}
	// End:0xAB
	if(ParseInt(param, "Type", Type))
	{
		// End:0x6F
		if(__NFUN_154__(Type, 0))
		{
			lstAttackClan.DeleteAllItem();
			m_IsExistMyClanIDinAttackSide = false;
			UpdateAttackCount();
			ClearAttackButton();			
		}
		else
		{
			// End:0xAB
			if(__NFUN_154__(Type, 1))
			{
				lstDefenseClan.DeleteAllItem();
				m_IsExistMyClanIDinDefenseSide = false;
				m_AcceptedClan = 0;
				m_WaitingClan = 0;
				UpdateDefenseCount();
				ClearDefenseButton();
			}
		}
	}
	return;
}

function HandleSiegeInfoClanList(string param)
{
	local int Type, clanID;
	local string ClanName, AllianceName;
	local int AllianceID, Status;
	local UIEventManager.ECastleSiegeDefenderType DefenderType;
	local LVDataRecord Record;
	local Texture texClan, texAlliance;

	Record.LVDataList.Length = 2;
	// End:0x547
	if(ParseInt(param, "Type", Type))
	{
		ParseInt(param, "ClanID", clanID);
		ParseString(param, "ClanName", ClanName);
		ParseInt(param, "AllianceID", AllianceID);
		ParseString(param, "AllianceName", AllianceName);
		ParseInt(param, "Status", Status);
		// End:0xB8
		if(__NFUN_150__(clanID, 1))
		{
			return;
		}
		// End:0x319
		if(__NFUN_154__(Type, 0))
		{
			// End:0xE7
			if(__NFUN_130__(__NFUN_151__(m_PlayerClanID, 0), __NFUN_154__(clanID, m_PlayerClanID)))
			{
				m_IsExistMyClanIDinAttackSide = true;
			}
			Record.LVDataList[0].szData = ClanName;
			// End:0x1EC
			if(Class'NWindow.UIDATA_CLAN'.static.GetCrestTexture(clanID, texClan))
			{
				Record.LVDataList[0].arrTexture.Length = 1;
				Record.LVDataList[0].arrTexture[0].objTex = texClan;
				Record.LVDataList[0].arrTexture[0].X = 6;
				Record.LVDataList[0].arrTexture[0].Y = 0;
				Record.LVDataList[0].arrTexture[0].Width = 16;
				Record.LVDataList[0].arrTexture[0].Height = 12;
				Record.LVDataList[0].arrTexture[0].U = 0;
				Record.LVDataList[0].arrTexture[0].V = 4;
			}
			Record.LVDataList[1].szData = AllianceName;
			// End:0x2FC
			if(__NFUN_151__(AllianceID, 0))
			{
				// End:0x2FC
				if(Class'NWindow.UIDATA_CLAN'.static.GetAllianceCrestTexture(clanID, texAlliance))
				{
					Record.LVDataList[1].arrTexture.Length = 1;
					Record.LVDataList[1].arrTexture[0].objTex = texAlliance;
					Record.LVDataList[1].arrTexture[0].X = 6;
					Record.LVDataList[1].arrTexture[0].Y = 0;
					Record.LVDataList[1].arrTexture[0].Width = 8;
					Record.LVDataList[1].arrTexture[0].Height = 12;
					Record.LVDataList[1].arrTexture[0].U = 0;
					Record.LVDataList[1].arrTexture[0].V = 4;
				}
			}
			lstAttackClan.InsertRecord(Record);
			UpdateAttackCount();			
		}
		else
		{
			// End:0x547
			if(__NFUN_154__(Type, 1))
			{
				// End:0x348
				if(__NFUN_130__(__NFUN_151__(m_PlayerClanID, 0), __NFUN_154__(clanID, m_PlayerClanID)))
				{
					m_IsExistMyClanIDinDefenseSide = true;
				}
				Record.nReserved1 = clanID;
				Record.nReserved2 = Status;
				Record.LVDataList[0].szData = ClanName;
				// End:0x46D
				if(Class'NWindow.UIDATA_CLAN'.static.GetCrestTexture(clanID, texClan))
				{
					Record.LVDataList[0].arrTexture.Length = 1;
					Record.LVDataList[0].arrTexture[0].objTex = texClan;
					Record.LVDataList[0].arrTexture[0].X = 6;
					Record.LVDataList[0].arrTexture[0].Y = 0;
					Record.LVDataList[0].arrTexture[0].Width = 16;
					Record.LVDataList[0].arrTexture[0].Height = 12;
					Record.LVDataList[0].arrTexture[0].U = 0;
					Record.LVDataList[0].arrTexture[0].V = 4;
				}
				DefenderType = byte(Status);
				switch(DefenderType)
				{
					// End:0x4AD
					case 1:
						Record.LVDataList[1].szData = GetSystemString(588);
						__NFUN_165__(m_WaitingClan);
						// End:0x52D
						break;
					// End:0x4D9
					case 2:
						Record.LVDataList[1].szData = GetSystemString(568);
						__NFUN_165__(m_WaitingClan);
						// End:0x52D
						break;
					// End:0x505
					case 3:
						Record.LVDataList[1].szData = GetSystemString(567);
						__NFUN_165__(m_AcceptedClan);
						// End:0x52D
						break;
					// End:0x52A
					case 4:
						Record.LVDataList[1].szData = GetSystemString(579);
						// End:0x52D
						break;
					// End:0xFFFF
					default:
						break;
				}
				lstDefenseClan.InsertRecord(Record);
				UpdateDefenseCount();
			}
		}
	}
	return;
}

function HandleSiegeInfoClanListEnd(string param)
{
	local int Type;

	// End:0x3E
	if(ParseInt(param, "Type", Type))
	{
		// End:0x2D
		if(__NFUN_154__(Type, 0))
		{
			UpdateAttackButton();			
		}
		else
		{
			// End:0x3E
			if(__NFUN_154__(Type, 1))
			{
				UpdateDefenseButton();
			}
		}
	}
	return;
}

function HandleSiegeInfoSelectableTime(string param)
{
	local int TimeID;
	local string TimeString;

	// End:0x6D
	if(ParseInt(param, "TimeID", TimeID))
	{
		// End:0x6D
		if(__NFUN_151__(TimeID, 0))
		{
			TimeString = ConvertTimetoStr(TimeID);
			cboTime.AddString(TimeString);
			m_SelectableTimeArray.Insert(m_SelectableTimeArray.Length, 1);
			m_SelectableTimeArray[__NFUN_147__(m_SelectableTimeArray.Length, 1)] = TimeID;
		}
	}
	return;
}

function UpdateAttackCount()
{
	txtAttackCount.SetText(__NFUN_112__(__NFUN_112__(GetSystemString(576), " : "), string(lstAttackClan.GetRecordCount())));
	return;
}

function UpdateDefenseCount()
{
	txtDefenseCount.SetText(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetSystemString(577), "/"), GetSystemString(578)), " : "), string(m_AcceptedClan)), "/"), string(m_WaitingClan)));
	return;
}

function UpdateTimeCombo()
{
	// End:0x28
	if(__NFUN_130__(m_IsCastleOwner, __NFUN_150__(m_SiegeTime, 1)))
	{
		cboTime.ShowWindow();		
	}
	else
	{
		cboTime.HideWindow();
	}
	return;
}

function UpdateAttackButton()
{
	// End:0x35
	if(__NFUN_129__(m_IsCastleOwner))
	{
		// End:0x26
		if(m_IsExistMyClanIDinAttackSide)
		{
			btnAttackCancel.ShowWindow();			
		}
		else
		{
			btnAttackApply.ShowWindow();
		}
	}
	return;
}

function UpdateDefenseButton()
{
	// End:0x38
	if(__NFUN_129__(m_IsCastleOwner))
	{
		// End:0x26
		if(m_IsExistMyClanIDinDefenseSide)
		{
			btnDefenseCancel.ShowWindow();			
		}
		else
		{
			btnDefenseApply.ShowWindow();
		}		
	}
	else
	{
		btnDefenseReject.ShowWindow();
		btnDefenseConfirm.ShowWindow();
	}
	return;
}

function ClearTimeCombo()
{
	m_SelectableTimeArray.Remove(0, m_SelectableTimeArray.Length);
	cboTime.Clear();
	cboTime.SYS_AddString(585);
	cboTime.SetSelectedNum(0);
	cboTime.HideWindow();
	return;
}

function ClearAttackButton()
{
	btnAttackApply.HideWindow();
	btnAttackCancel.HideWindow();
	return;
}

function ClearDefenseButton()
{
	btnDefenseApply.HideWindow();
	btnDefenseCancel.HideWindow();
	btnDefenseReject.HideWindow();
	btnDefenseConfirm.HideWindow();
	return;
}
