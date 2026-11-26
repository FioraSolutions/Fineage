class ClanDrawerWnd extends UIScript;

const c_maxranklimit = 100;
const changelineval1 = 23;

var string m_state;
var int m_clanType;
var int m_clanWarListPage;
var int m_currentEditGradeID;
var string m_currentName;
var string m_myName;
var int m_currentMaster;
var string currentMasterName;
var WindowHandle Clan3_OrgIcon[8];

function OnLoad()
{
	RegisterEvent(350);
	RegisterEvent(360);
	RegisterEvent(370);
	RegisterEvent(380);
	RegisterEvent(430);
	RegisterEvent(460);
	RegisterEvent(490);
	RegisterEvent(500);
	RegisterEvent(160);
	RegisterEvent(470);
	InitHandle();
	InitializeGradeComboBox();
	HideAll();
	m_clanWarListPage = -1;
	return;
}

function InitHandle()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x58 [Loop If]
	if(__NFUN_150__(i, 8))
	{
		Clan3_OrgIcon[i] = GetHandle(__NFUN_112__("ClanDrawerWnd.Clan3_OrgIcon", string(__NFUN_146__(i, 1))));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function OnShow()
{
	local ClanWnd script;

	script = ClanWnd(GetScript("ClanWnd"));
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_ChangeMemberNameBtn");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");
	// End:0x2A8
	if(__NFUN_154__(script.m_bClanMaster, 0))
	{
		// End:0x18F
		if(__NFUN_154__(script.m_bNickName, 0))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberNameBtn");
		}
		// End:0x1DC
		if(__NFUN_154__(script.m_bGrade, 0))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");
		}
		// End:0x260
		if(__NFUN_154__(script.m_bManageMaster, 0))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
		}
		// End:0x2A8
		if(__NFUN_154__(script.m_bOustMember, 0))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");
		}
	}
	return;
}

function Clear()
{
	m_state = "";
	m_clanType = -1;
	m_clanWarListPage = -1;
	m_currentEditGradeID = -1;
	m_currentName = "";
	return;
}

function SetStateAndShow(string State)
{
	local ClanWnd script;
	local int i;
	local string string1, string2, string3, string4;

	m_state = State;
	// End:0x4C
	if(__NFUN_129__(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("ClanDrawerWnd")))
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd");
	}
	HideAll();
	// End:0x18F
	if(__NFUN_122__(m_state, "ClanMemberInfoState"))
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.ClanMemberInfoWnd");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberNameWnd");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeNameWnd");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_AssignApprenticeWnd");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodWnd");		
	}
	else
	{
		// End:0x2DA
		if(__NFUN_122__(m_state, "ClanMemberAuthState"))
		{
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.ClanMemberAuthWnd");
			i = 0;
			J0x1E5:

			// End:0x231 [Loop If]
			if(__NFUN_152__(i, 9))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable(__NFUN_112__("ClanDrawerWnd.Clan2_Check10", string(i)), true);
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x1E5;
			}
			i = 0;
			J0x238:

			// End:0x284 [Loop If]
			if(__NFUN_152__(i, 5))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable(__NFUN_112__("ClanDrawerWnd.Clan2_Check20", string(i)), true);
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x238;
			}
			i = 0;
			J0x28B:

			// End:0x2D7 [Loop If]
			if(__NFUN_152__(i, 8))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable(__NFUN_112__("ClanDrawerWnd.Clan2_Check30", string(i)), true);
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x28B;
			}			
		}
		else
		{
			// End:0x326
			if(__NFUN_122__(m_state, "ClanInfoState"))
			{
				Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.ClanInfoWnd");
				InitializeClanInfoWnd();				
			}
			else
			{
				// End:0x37B
				if(__NFUN_122__(m_state, "ClanAuthManageWndState"))
				{
					Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.ClanAuthManageWnd");					
				}
				else
				{
					// End:0x53B
					if(__NFUN_122__(m_state, "ClanEmblemManageWndState"))
					{
						Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.ClanEmblemManageWnd");
						string1 = __NFUN_128__(GetSystemMessage(211), 23);
						string2 = __NFUN_234__(GetSystemMessage(211), __NFUN_147__(__NFUN_125__(GetSystemMessage(211)), 23));
						string3 = __NFUN_128__(GetSystemMessage(1478), 23);
						string4 = __NFUN_234__(GetSystemMessage(1478), __NFUN_147__(__NFUN_125__(GetSystemMessage(1478)), 23));
						Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan7_ManageEmb1Text1", string1);
						Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan7_ManageEmb1Text2", string2);
						Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan7_ManageEmb2Text1", string3);
						Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan7_ManageEmb2Text2", string4);
						script = ClanWnd(GetScript("ClanWnd"));						
					}
					else
					{
						// End:0x5C5
						if(__NFUN_122__(m_state, "ClanWarManagementWndState"))
						{
							Class'NWindow.UIAPI_TABCTRL'.static.SetTopOrder("ClanDrawerWnd.ClanWarTabCtrl", 0, true);
							Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.ClanWarManagementWnd");							
						}
						else
						{
							// End:0x616
							if(__NFUN_122__(m_state, "ClanAuthEditWndState"))
							{
								Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.ClanAuthEditWnd");								
							}
							else
							{
								// End:0x65C
								if(__NFUN_122__(m_state, "ClanHeroWndState"))
								{
									Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.ClanHeroWnd");
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

function HideAll()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.ClanMemberInfoWnd");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.ClanMemberAuthWnd");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.ClanInfoWnd");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.ClanPenaltyWnd");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.ClanWarManagementWnd");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.ClanAuthManageWnd");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.ClanAuthEditWnd");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.ClanEmblemManageWnd");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.ClanHeroWnd");
	return;
}

function OnClickButton(string strID)
{
	local LVDataRecord Record;
	local int i;
	local ClanWnd script;

	script = ClanWnd(GetScript("ClanWnd"));
	// End:0x82
	if(__NFUN_122__(strID, "Clan1_AskJoinPartyBtn"))
	{
		RequestInviteParty(Class'NWindow.UIAPI_TEXTBOX'.static.GetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberName"));		
	}
	else
	{
		// End:0x1D1
		if(__NFUN_122__(strID, "Clan1_ChangeMemberNameBtn"))
		{
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.Clan1_ChangeMemberNameWnd");
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeNameWnd");
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_AssignApprenticeWnd");
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodWnd");
			Class'NWindow.UIAPI_EDITBOX'.static.SetString("ClanDrawerWnd.Clan1_ChangeNameTextEditbox", "");			
		}
		else
		{
			// End:0x2E5
			if(__NFUN_122__(strID, "Clan1_ChangeMemberGradeBtn"))
			{
				Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeNameWnd");
				Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberNameWnd");
				Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodWnd");
				Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_AssignApprenticeWnd");				
			}
			else
			{
				// End:0x3FF
				if(__NFUN_122__(strID, "Clan1_ChangeBanishBtn"))
				{
					Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberNameWnd");
					Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeNameWnd");
					Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_AssignApprenticeWnd");
					RequestClanExpelMember(m_clanType, Class'NWindow.UIAPI_TEXTBOX'.static.GetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberName"));					
				}
				else
				{
					// End:0x518
					if(__NFUN_122__(strID, "Clan1_AssignApprenticeBtn"))
					{
						Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.Clan1_AssignApprenticeWnd");
						Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberNameWnd");
						Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodWnd");
						Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeNameWnd");
						InitializeAcademyList();						
					}
					else
					{
						// End:0x630
						if(__NFUN_122__(strID, "Clan1_ChangeMemberKHOpen"))
						{
							Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodWnd");
							Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_AssignApprenticeWnd");
							Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberNameWnd");
							Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeNameWnd");
							KnighthoodCombobox();							
						}
						else
						{
							// End:0x6E0
							if(__NFUN_122__(strID, "Clan1_DeleteApprenticeBtn"))
							{
								RequestClanDeletePupil(Class'NWindow.UIAPI_TEXTBOX'.static.GetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberName"), Class'NWindow.UIAPI_TEXTBOX'.static.GetText("ClanDrawerWnd.Clan1_CurrentSelectedApprentice"));
								RecallCurrentMemberInfo();								
							}
							else
							{
								// End:0x700
								if(__NFUN_122__(strID, "Clan1_OKBtn"))
								{
									HideWindow();									
								}
								else
								{
									// End:0x820
									if(__NFUN_122__(strID, "Clan1_ChangeNameAssignBtn"))
									{
										RequestClanChangeNickName(Class'NWindow.UIAPI_TEXTBOX'.static.GetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberName"), Class'NWindow.UIAPI_EDITBOX'.static.GetString("ClanDrawerWnd.Clan1_ChangeNameTextEditbox"));
										Class'NWindow.UIAPI_EDITBOX'.static.SetString("ClanDrawerWnd.Clan1_ChangeNameTextEditbox", "");
										Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberNameWnd");
										RecallCurrentMemberInfo();										
									}
									else
									{
										// End:0x908
										if(__NFUN_122__(strID, "Clan1_ChangeNameDeleteBtn"))
										{
											RequestClanChangeNickName(Class'NWindow.UIAPI_TEXTBOX'.static.GetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberName"), "");
											Class'NWindow.UIAPI_EDITBOX'.static.SetString("ClanDrawerWnd.Clan1_ChangeNameTextEditbox", "");
											Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberNameWnd");
											RecallCurrentMemberInfo();											
										}
										else
										{
											// End:0xA7A
											if(__NFUN_122__(strID, "Clan1_ChangeMemberGradeAssignBtn"))
											{
												// End:0x9EA
												if(__NFUN_152__(Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("ClanDrawerWnd.Clan1_MemberGradeList"), 5))
												{
													RequestClanChangeGrade(Class'NWindow.UIAPI_TEXTBOX'.static.GetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberName"), Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("ClanDrawerWnd.Clan1_MemberGradeList"));													
												}
												else
												{
													RequestClanChangeGrade(Class'NWindow.UIAPI_TEXTBOX'.static.GetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberName"), getCurrentGradebyClanType());
												}
												Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeNameWnd");
												RecallCurrentMemberInfo();												
											}
											else
											{
												// End:0xB92
												if(__NFUN_122__(strID, "Clan1_ApprenticeAssignBtn"))
												{
													i = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ClanDrawerWnd.Clan1_AssignApprenticeList");
													// End:0xB57
													if(__NFUN_130__(__NFUN_153__(i, 0), __NFUN_123__(m_currentName, "")))
													{
														Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ClanDrawerWnd.Clan1_AssignApprenticeList", i);
														RequestClanAssignPupil(m_currentName, Record.LVDataList[0].szData);
													}
													Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_AssignApprenticeWnd");													
												}
												else
												{
													// End:0xC04
													if(__NFUN_122__(strID, "Clan1_ChangeMemberKnightHoodBtn"))
													{
														proc_swapmember();
														Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodWnd");														
													}
													else
													{
														// End:0xC58
														if(__NFUN_122__(strID, "Clan1_Cancel1"))
														{
															Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberNameWnd");															
														}
														else
														{
															// End:0xCB1
															if(__NFUN_122__(strID, "Clan1_Cancel2"))
															{
																Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeNameWnd");																
															}
															else
															{
																// End:0xD05
																if(__NFUN_122__(strID, "Clan1_Cancel3"))
																{
																	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_AssignApprenticeWnd");																	
																}
																else
																{
																	// End:0xD5F
																	if(__NFUN_122__(strID, "Clan1_Cancel4"))
																	{
																		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodWnd");																		
																	}
																	else
																	{
																		// End:0xD83
																		if(__NFUN_122__(strID, "Clan7_RegEmbBtn"))
																		{
																			RequestClanRegisterCrest();																			
																		}
																		else
																		{
																			// End:0xDA6
																			if(__NFUN_122__(strID, "Clan7_RmEmbBtn"))
																			{
																				RequestClanUnregisterCrest();																				
																			}
																			else
																			{
																				// End:0xDCB
																				if(__NFUN_122__(strID, "Clan7_RegEmb2Btn"))
																				{
																					RequestClanRegisterEmblem();																					
																				}
																				else
																				{
																					// End:0xDEF
																					if(__NFUN_122__(strID, "Clan7_RmEmb2Btn"))
																					{
																						RequestClanUnregisterEmblem();																						
																					}
																					else
																					{
																						// End:0xE17
																						if(__NFUN_122__(strID, "Clan8_CancelWar1Btn"))
																						{
																							HandleCancelWar1();																							
																						}
																						else
																						{
																							// End:0xE40
																							if(__NFUN_122__(strID, "Clan8_DeclareWar1Btn"))
																							{
																								HandleDeclareWar();																								
																							}
																							else
																							{
																								// End:0xE68
																								if(__NFUN_122__(strID, "Clan8_CancelWar2Btn"))
																								{
																									HandleCancelWar2();																									
																								}
																								else
																								{
																									// End:0xE96
																									if(__NFUN_122__(strID, "Clan8_ViewMoreBtn"))
																									{
																										RequestClanWarList(__NFUN_163__(m_clanWarListPage), 1);																										
																									}
																									else
																									{
																										// End:0xEB6
																										if(__NFUN_122__(strID, "Clan2_OKBtn"))
																										{
																											HideWindow();																											
																										}
																										else
																										{
																											// End:0xED6
																											if(__NFUN_122__(strID, "Clan3_OKBtn"))
																											{
																												HideWindow();																												
																											}
																											else
																											{
																												// End:0xEF6
																												if(__NFUN_122__(strID, "Clan4_OKBtn"))
																												{
																													HideWindow();																													
																												}
																												else
																												{
																													// End:0xF16
																													if(__NFUN_122__(strID, "Clan5_OKBtn"))
																													{
																														HideWindow();																														
																													}
																													else
																													{
																														// End:0xF36
																														if(__NFUN_122__(strID, "Clan7_OKBtn"))
																														{
																															HideWindow();																															
																														}
																														else
																														{
																															// End:0xF58
																															if(__NFUN_122__(strID, "ClanWar_OKBtn"))
																															{
																																HideWindow();																																
																															}
																															else
																															{
																																// End:0xF7C
																																if(__NFUN_122__(strID, "Clan5_ManageBtn"))
																																{
																																	EditAuthGrade();																																	
																																}
																																else
																																{
																																	// End:0xFA1
																																	if(__NFUN_122__(strID, "Clan5_ManageBtn2"))
																																	{
																																		EditAuthGrade2();																																		
																																	}
																																	else
																																	{
																																		// End:0xFE2
																																		if(__NFUN_122__(strID, "Clan6_ApplyBtn"))
																																		{
																																			ApplyEditGrade();
																																			SetStateAndShow("ClanAuthManageWndState");																																			
																																		}
																																		else
																																		{
																																			// End:0x101E
																																			if(__NFUN_122__(strID, "Clan6_CancelBtn"))
																																			{
																																				SetStateAndShow("ClanAuthManageWndState");																																				
																																			}
																																			else
																																			{
																																				// End:0x1044
																																				if(__NFUN_122__(strID, "ClanWarTabCtrl0"))
																																				{
																																					RequestClanWarList(0, 0);																																					
																																				}
																																				else
																																				{
																																					// End:0x106E
																																					if(__NFUN_122__(strID, "ClanWarTabCtrl1"))
																																					{
																																						RequestClanWarList(m_clanWarListPage, 1);																																						
																																					}
																																					else
																																					{
																																						// End:0x1129
																																						if(__NFUN_122__(strID, "Clan1_ChangeNameAssignNobBtn"))
																																						{
																																							RequestClanChangeNickName(script.m_myName, Class'NWindow.UIAPI_EDITBOX'.static.GetString("ClanDrawerWnd.Clan1_ChangeNobNameTextEditbox"));
																																							Class'NWindow.UIAPI_EDITBOX'.static.SetString("ClanDrawerWnd.Clan1_ChangeNobNameTextEditbox", "");																																							
																																						}
																																						else
																																						{
																																							// End:0x114E
																																							if(__NFUN_122__(strID, "Clan1_NobCancel1"))
																																							{
																																								HideWindow();																																								
																																							}
																																							else
																																							{
																																								// End:0x118C
																																								if(__NFUN_122__(strID, "Clan1_ChangeNameDeleteNobBtn"))
																																								{
																																									RequestClanChangeNickName(script.m_myName, "");
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
						}
					}
				}
			}
		}
	}
	return;
}

function OnDBClickListCtrlRecord(string ListCtrlID)
{
	local int i;
	local LVDataRecord Record;

	// End:0x24
	if(__NFUN_122__(ListCtrlID, "Clan5_AuthListCtrl"))
	{
		EditAuthGrade();
	}
	// End:0x49
	if(__NFUN_122__(ListCtrlID, "Clan5_AuthListCtrl2"))
	{
		EditAuthGrade2();
	}
	// End:0x15F
	if(__NFUN_122__(ListCtrlID, "Clan1_AssignApprenticeList"))
	{
		i = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ClanDrawerWnd.Clan1_AssignApprenticeList");
		// End:0x127
		if(__NFUN_130__(__NFUN_153__(i, 0), __NFUN_123__(m_currentName, "")))
		{
			Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ClanDrawerWnd.Clan1_AssignApprenticeList", i);
			RequestClanAssignPupil(m_currentName, Record.LVDataList[0].szData);
		}
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd.Clan1_AssignApprenticeWnd");
	}
	return;
}

function RecallCurrentMemberInfo()
{
	local ClanWnd script;

	script = ClanWnd(GetScript("ClanWnd"));
	RequestClanMemberInfo(script.G_CurrentRecord, script.G_CurrentSzData);
	SetStateAndShow("ClanMemberInfoState");
	return;
}

function OnClickCheckBox(string CheckBoxID)
{
	local string CheckboxNum, CheckboxName;
	local bool CheckedStat;
	local int i;

	CheckboxName = __NFUN_128__(CheckBoxID, 12);
	// End:0x1AB
	if(__NFUN_122__(CheckboxName, "Clan6_Check1"))
	{
		CheckboxNum = __NFUN_234__(CheckBoxID, 2);
		// End:0x13A
		if(__NFUN_122__(CheckboxNum, "00"))
		{
			CheckedStat = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ClanDrawerWnd.Clan6_Check100");
			switch(CheckedStat)
			{
				// End:0xDA
				case true:
					i = 0;
					J0x8B:

					// End:0xD7 [Loop If]
					if(__NFUN_152__(i, 9))
					{
						Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan6_Check10", string(i)), true);
						__NFUN_163__(i);
						// [Loop Continue]
						goto J0x8B;
					}
					// End:0x137
					break;
				// End:0x134
				case false:
					i = 0;
					J0xE5:

					// End:0x131 [Loop If]
					if(__NFUN_152__(i, 9))
					{
						Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan6_Check10", string(i)), false);
						__NFUN_163__(i);
						// [Loop Continue]
						goto J0xE5;
					}
					// End:0x137
					break;
				// End:0xFFFF
				default:
					break;
			}			
		}
		else
		{
			// End:0x17D
			if(__NFUN_242__(count_all_check("10", 9), true))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check100", true);				
			}
			else
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check100", false);
			}
		}
	}
	// End:0x347
	if(__NFUN_122__(CheckboxName, "Clan6_Check2"))
	{
		CheckboxNum = __NFUN_234__(CheckBoxID, 2);
		// End:0x2D6
		if(__NFUN_122__(CheckboxNum, "00"))
		{
			CheckedStat = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ClanDrawerWnd.Clan6_Check200");
			switch(CheckedStat)
			{
				// End:0x276
				case true:
					i = 0;
					J0x227:

					// End:0x273 [Loop If]
					if(__NFUN_152__(i, 5))
					{
						Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan6_Check20", string(i)), true);
						__NFUN_163__(i);
						// [Loop Continue]
						goto J0x227;
					}
					// End:0x2D3
					break;
				// End:0x2D0
				case false:
					i = 0;
					J0x281:

					// End:0x2CD [Loop If]
					if(__NFUN_152__(i, 5))
					{
						Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan6_Check20", string(i)), false);
						__NFUN_163__(i);
						// [Loop Continue]
						goto J0x281;
					}
					// End:0x2D3
					break;
				// End:0xFFFF
				default:
					break;
			}			
		}
		else
		{
			// End:0x319
			if(__NFUN_242__(count_all_check("20", 8), true))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check200", true);				
			}
			else
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check200", false);
			}
		}
	}
	// End:0x4E3
	if(__NFUN_122__(CheckboxName, "Clan6_Check3"))
	{
		CheckboxNum = __NFUN_234__(CheckBoxID, 2);
		// End:0x472
		if(__NFUN_122__(CheckboxNum, "00"))
		{
			CheckedStat = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ClanDrawerWnd.Clan6_Check300");
			switch(CheckedStat)
			{
				// End:0x412
				case true:
					i = 0;
					J0x3C3:

					// End:0x40F [Loop If]
					if(__NFUN_152__(i, 9))
					{
						Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan6_Check30", string(i)), true);
						__NFUN_163__(i);
						// [Loop Continue]
						goto J0x3C3;
					}
					// End:0x46F
					break;
				// End:0x46C
				case false:
					i = 0;
					J0x41D:

					// End:0x469 [Loop If]
					if(__NFUN_152__(i, 9))
					{
						Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan6_Check30", string(i)), false);
						__NFUN_163__(i);
						// [Loop Continue]
						goto J0x41D;
					}
					// End:0x46F
					break;
				// End:0xFFFF
				default:
					break;
			}			
		}
		else
		{
			// End:0x4B5
			if(__NFUN_242__(count_all_check("30", 9), true))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check300", true);				
			}
			else
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check300", false);
			}
		}
	}
	return;
}

function bool count_all_check(string numString, int TotalNum)
{
	local bool checkall, currentcheck;
	local int i;

	checkall = false;
	i = 1;
	J0x0F:

	// End:0x7D [Loop If]
	if(__NFUN_152__(i, TotalNum))
	{
		currentcheck = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked(__NFUN_112__(__NFUN_112__("ClanDrawerWnd.Clan6_Check", numString), string(i)));
		// End:0x73
		if(__NFUN_242__(currentcheck, true))
		{
			checkall = true;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x0F;
	}
	return checkall;
	return;
}

function bool count_all_check2(string numString, int TotalNum)
{
	local bool checkall, currentcheck;
	local int i;

	checkall = false;
	i = 1;
	J0x0F:

	// End:0x7D [Loop If]
	if(__NFUN_152__(i, TotalNum))
	{
		currentcheck = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked(__NFUN_112__(__NFUN_112__("ClanDrawerWnd.Clan2_Check", numString), string(i)));
		// End:0x73
		if(__NFUN_242__(currentcheck, true))
		{
			checkall = true;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x0F;
	}
	return checkall;
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 350:
			HandleClanAuthGradeList(a_Param);
			// End:0xF6
			break;
		// End:0x33
		case 460:
			HandleClanWarList(a_Param);
			// End:0xF6
			break;
		// End:0x46
		case 360:
			HandleCrestChange(a_Param);
		// End:0x5C
		case 430:
			HandleClanMemberInfo(a_Param);
			// End:0xF6
			break;
		// End:0x72
		case 490:
			HandleSkillList(a_Param);
			// End:0xF6
			break;
		// End:0x88
		case 500:
			HandleSkillListAdd(a_Param);
			// End:0xF6
			break;
		// End:0x9E
		case 370:
			HandleClanAuth(a_Param);
			// End:0xF6
			break;
		// End:0xB4
		case 380:
			HandleClanAuthMember(a_Param);
			// End:0xF6
			break;
		// End:0xDA
		case 160:
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd");
			// End:0xF6
			break;
		// End:0xF0
		case 470:
			HandleClearWarList(a_Param);
			// End:0xF6
			break;
		// End:0xFFFF
		default:
			// End:0xF6
			break;
			break;
	}
	return;
}

function HandleClanAuthGradeList(string a_Param)
{
	local int Count, Id, members, i;
	local LVDataRecord Record;
	local LVData Data;

	Record.LVDataList.Length = 2;
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ClanDrawerWnd.Clan5_AuthListCtrl");
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ClanDrawerWnd.Clan5_AuthListCtrl2");
	ParseInt(a_Param, "Count", Count);
	i = 0;
	J0x8F:

	// End:0x184 [Loop If]
	if(__NFUN_150__(i, 5))
	{
		ParseInt(a_Param, __NFUN_112__("GradeID", string(i)), Id);
		ParseInt(a_Param, __NFUN_112__("GradeMemberCount", string(i)), members);
		Data.szData = GetStringByGradeID(Id);
		Record.LVDataList[0] = Data;
		Data.szData = string(members);
		Record.LVDataList[1] = Data;
		Record.nReserved1 = Id;
		Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ClanDrawerWnd.Clan5_AuthListCtrl", Record);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x8F;
	}
	i = 5;
	J0x18C:

	// End:0x282 [Loop If]
	if(__NFUN_150__(i, 9))
	{
		ParseInt(a_Param, __NFUN_112__("GradeID", string(i)), Id);
		ParseInt(a_Param, __NFUN_112__("GradeMemberCount", string(i)), members);
		Data.szData = GetStringByGradeID(Id);
		Record.LVDataList[0] = Data;
		Data.szData = string(members);
		Record.LVDataList[1] = Data;
		Record.nReserved1 = Id;
		Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ClanDrawerWnd.Clan5_AuthListCtrl2", Record);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x18C;
	}
	Class'NWindow.UIAPI_LISTCTRL'.static.SetSelectedIndex("ClanDrawerWnd.Clan5_AuthListCtrl", 0, true);
	Class'NWindow.UIAPI_LISTCTRL'.static.SetSelectedIndex("ClanDrawerWnd.Clan5_AuthListCtrl2", 0, true);
	return;
}

function HandleClanWarList(string a_Param)
{
	local string sClanName;
	local int Type, period;
	local LVDataRecord Record;
	local int Page;

	ParseInt(a_Param, "Page", Page);
	ParseString(a_Param, "ClanName", sClanName);
	ParseInt(a_Param, "Type", Type);
	ParseInt(a_Param, "Period", period);
	Record.LVDataList.Length = 3;
	Record.LVDataList[0].szData = sClanName;
	Record.LVDataList[1].szData = GetWarStateString(Type);
	Record.LVDataList[2].szData = string(period);
	// End:0x13F
	if(__NFUN_132__(__NFUN_154__(Type, 0), __NFUN_154__(Type, 2)))
	{
		Class'NWindow.UIAPI_TABCTRL'.static.SetTopOrder("ClanDrawerWnd.ClanWarTabCtrl", 0, true);
		Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ClanDrawerWnd.Clan8_DeclaredListCtrl", Record);		
	}
	else
	{
		Class'NWindow.UIAPI_TABCTRL'.static.SetTopOrder("ClanDrawerWnd.ClanWarTabCtrl", 1, true);
		m_clanWarListPage = Page;
		Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ClanDrawerWnd.Clan8_GotDeclaredListCtrl", Record);
	}
	return;
}

function HandleClanMemberInfo(string a_Param)
{
	local string nickname;
	local int gradeID;
	local string organization, MasterName;
	local ClanWnd script;
	local string organizationtext;

	script = ClanWnd(GetScript("ClanWnd"));
	ParseInt(a_Param, "ClanType", m_clanType);
	ParseString(a_Param, "Name", m_currentName);
	ParseString(a_Param, "NickName", nickname);
	ParseInt(a_Param, "GradeID", gradeID);
	ParseString(a_Param, "OrderName", organization);
	ParseString(a_Param, "MasterName", MasterName);
	currentMasterName = MasterName;
	// End:0xD9
	if(__NFUN_122__(MasterName, ""))
	{
		MasterName = GetSystemString(27);
	}
	organizationtext = __NFUN_168__(__NFUN_168__(getClanOrderString(m_clanType), "-"), organization);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberName", m_currentName);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberSName", nickname);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberGrade", GetStringByGradeID(gradeID));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberOrderName", organizationtext);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_CurrentSelectedApprentice", MasterName);
	// End:0x2C8
	if(__NFUN_122__(script.m_CurrentclanMasterReal, m_currentName))
	{
		// End:0x2C8
		if(__NFUN_154__(script.m_currentShowIndex, 0))
		{
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberGrade", GetSystemString(342));
		}
	}
	// End:0x398
	if(__NFUN_154__(m_clanType, -1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_CurrentSelectedApprenticeTitle", GetSystemString(1332));		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
		// End:0x487
		if(__NFUN_123__(currentMasterName, ""))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
		}
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_CurrentSelectedApprenticeTitle", GetSystemString(1431));
	}
	script.resetBtnShowHide();
	CheckandCompareMyNameandDisableThings();
	return;
}

function CheckandCompareMyNameandDisableThings()
{
	local ClanWnd script;
	local UserInfo UserInfo;

	GetPlayerInfo(UserInfo);
	m_myName = UserInfo.Name;
	script = ClanWnd(GetScript("ClanWnd"));
	// End:0x1F3
	if(__NFUN_151__(script.m_bClanMaster, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_ChangeMemberKHOpen");
		// End:0x180
		if(__NFUN_123__(currentMasterName, ""))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
			// End:0x17D
			if(__NFUN_242__(script.G_CurrentAlias, true))
			{
				Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
			}			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
		}		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberKHOpen");
		Proc_AuthValidation();
		// End:0x3F4
		if(__NFUN_150__(script.GetClanTypeFromIndex(script.m_currentShowIndex), script.m_myClanType))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
			// End:0x3BC
			if(__NFUN_154__(m_clanType, -1))
			{				
			}
			else
			{
				Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberNameBtn");
			}
		}
		// End:0x555
		if(__NFUN_151__(script.m_myClanType, 1))
		{
			// End:0x555
			if(__NFUN_155__(script.GetClanTypeFromIndex(script.m_currentShowIndex), 0))
			{
				// End:0x464
				if(__NFUN_154__(__NFUN_147__(script.m_myClanType, script.GetClanTypeFromIndex(script.m_currentShowIndex)), 1))
				{
					Proc_AuthValidation();
				}
				// End:0x4A1
				if(__NFUN_154__(__NFUN_147__(script.m_myClanType, script.GetClanTypeFromIndex(script.m_currentShowIndex)), 1000))
				{
					Proc_AuthValidation();
				}
				// End:0x4DB
				if(__NFUN_154__(__NFUN_147__(script.m_myClanType, script.GetClanTypeFromIndex(script.m_currentShowIndex)), 100))
				{
					Proc_AuthValidation();
				}
				// End:0x518
				if(__NFUN_154__(__NFUN_147__(script.m_myClanType, script.GetClanTypeFromIndex(script.m_currentShowIndex)), 999))
				{
					Proc_AuthValidation();
				}
				// End:0x555
				if(__NFUN_154__(__NFUN_147__(script.m_myClanType, script.GetClanTypeFromIndex(script.m_currentShowIndex)), 1001))
				{
					Proc_AuthValidation();
				}
			}
		}
		// End:0x6B6
		if(__NFUN_242__(script.G_CurrentAlias, true))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberNameBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberKHOpen");
		}
		// End:0x81A
		if(__NFUN_122__(script.m_CurrentclanMasterReal, m_currentName))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberNameBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberKHOpen");
		}
	}
	// End:0x8CD
	if(__NFUN_122__(m_currentName, m_myName))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberKHOpen");
	}
	// End:0x985
	if(__NFUN_154__(m_clanType, -1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
	}
	return;
}

function Proc_AuthValidation()
{
	local ClanWnd script;

	script = ClanWnd(GetScript("ClanWnd"));
	// End:0xD0
	if(__NFUN_154__(script.m_bNickName, 0))
	{
		// End:0x95
		if(__NFUN_132__(__NFUN_242__(script.G_IamHero, true), __NFUN_242__(script.G_IamNobless, true)))
		{
			Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_ChangeMemberNameBtn");			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberNameBtn");
		}		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_ChangeMemberNameBtn");
	}
	// End:0x158
	if(__NFUN_154__(script.m_bGrade, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_ChangeMemberGradeBtn");
	}
	// End:0x218
	if(__NFUN_154__(script.m_bManageMaster, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");		
	}
	else
	{
		// End:0x297
		if(__NFUN_123__(currentMasterName, ""))
		{
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_AssignApprenticeBtn");
			Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_DeleteApprenticeBtn");
		}
	}
	// End:0x352
	if(__NFUN_154__(script.m_bOustMember, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_ChangeBanishBtn");
	}
	return;
}

function HandleCrestChange(string param)
{
	local ClanWnd script;

	// End:0x7F
	if(__NFUN_122__(m_state, "ClanEmblemManageWndState"))
	{
		script = ClanWnd(GetScript("ClanWnd"));
		Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTextureWithClanCrest("ClanDrawerWnd.ClanCrestTextureCtrl", script.m_clanID);
	}
	return;
}

function HandleSkillList(string a_Param)
{
	local int Count, i, Id, Level;

	ParseInt(a_Param, "Count", Count);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("ClanDrawerWnd.ClanSkillWnd");
	i = 0;
	J0x49:

	// End:0xB9 [Loop If]
	if(__NFUN_150__(i, Count))
	{
		ParseInt(a_Param, __NFUN_112__("SkillID", string(i)), Id);
		ParseInt(a_Param, __NFUN_112__("SkillLevel", string(i)), Level);
		AddSkill(Id, Level);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x49;
	}
	return;
}

function HandleSkillListAdd(string a_Param)
{
	local int Id, Level, i, Count;
	local ItemInfo Info;

	ParseInt(a_Param, "SkillID", Id);
	ParseInt(a_Param, "SkillLevel", Level);
	Count = Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItemNum("ClanDrawerWnd.ClanSkillWnd");
	i = 0;
	J0x6D:

	// End:0xD2 [Loop If]
	if(__NFUN_150__(i, Count))
	{
		Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("ClanDrawerWnd.ClanSkillWnd", i, Info);
		// End:0xC8
		if(__NFUN_154__(Info.ClassID, Id))
		{
			// [Explicit Break]
			goto J0xD2;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x6D;
	}
	J0xD2:

	// End:0xF9
	if(__NFUN_150__(i, Count))
	{
		ReplaceSkill(i, Id, Level);		
	}
	else
	{
		AddSkill(Id, Level);
	}
	return;
}

function HandleCancelWar1()
{
	local LVDataRecord Record;
	local int Index;

	Index = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ClanDrawerWnd.Clan8_DeclaredListCtrl");
	// End:0xC6
	if(__NFUN_153__(Index, 0))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ClanDrawerWnd.Clan8_DeclaredListCtrl", Index);
		RequestClanWithdrawWarWithClanName(Record.LVDataList[0].szData);
		RequestClanWarList(0, 0);
		SetStateAndShow("ClanWarManagementWndState");
	}
	return;
}

function HandleDeclareWar()
{
	local LVDataRecord Record;
	local int Index;

	Index = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ClanDrawerWnd.Clan8_GotDeclaredListCtrl");
	// End:0xAF
	if(__NFUN_153__(Index, 0))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ClanDrawerWnd.Clan8_GotDeclaredListCtrl", Index);
		RequestClanDeclareWarWidhClanName(Record.LVDataList[0].szData);
		RequestClanWarList(m_clanWarListPage, 1);
	}
	return;
}

function HandleCancelWar2()
{
	local LVDataRecord Record;
	local int Index;

	Index = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ClanDrawerWnd.Clan8_GotDeclaredListCtrl");
	// End:0x111
	if(__NFUN_153__(Index, 0))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ClanDrawerWnd.Clan8_GotDeclaredListCtrl", Index);
		RequestClanWithdrawWarWithClanName(Record.LVDataList[0].szData);
		RequestClanWarList(m_clanWarListPage, 1);
		Class'NWindow.UIAPI_TABCTRL'.static.SetTopOrder("ClanDrawerWnd.ClanWarTabCtrl", 1, true);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ClanDrawerWnd.ClanWarManagementWnd");
	}
	return;
}

function HandleClanAuth(string a_Param)
{
	local int gradeID, Command;
	local array<int> powers;
	local int i, Index;

	ParseInt(a_Param, "GradeID", gradeID);
	ParseInt(a_Param, "Command", Command);
	powers.Length = 32;
	i = 0;
	J0x42:

	// End:0x83 [Loop If]
	if(__NFUN_150__(i, 32))
	{
		ParseInt(a_Param, __NFUN_112__("PowerValue", string(i)), powers[i]);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x42;
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan6_CurrentSelectedRankName", __NFUN_112__(GetStringByGradeID(gradeID), GetSystemString(1376)));
	Index = 1;
	i = 1;
	J0xE5:

	// End:0x13F [Loop If]
	if(__NFUN_152__(i, 9))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan6_Check10", string(i)), bool(powers[__NFUN_165__(Index)]));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xE5;
	}
	i = 1;
	J0x146:

	// End:0x1A0 [Loop If]
	if(__NFUN_152__(i, 5))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan6_Check20", string(i)), bool(powers[__NFUN_165__(Index)]));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x146;
	}
	i = 1;
	J0x1A7:

	// End:0x201 [Loop If]
	if(__NFUN_152__(i, 8))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan6_Check30", string(i)), bool(powers[__NFUN_165__(Index)]));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1A7;
	}
	// End:0x244
	if(__NFUN_242__(count_all_check("10", 9), true))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check100", true);		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check100", false);
	}
	// End:0x2B5
	if(__NFUN_242__(count_all_check("20", 5), true))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check200", true);		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check200", false);
	}
	// End:0x326
	if(__NFUN_242__(count_all_check("30", 8), true))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check300", true);		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan6_Check300", false);
	}
	// End:0x369
	if(__NFUN_154__(gradeID, 9))
	{
		disableAcademyAuth();		
	}
	else
	{
		resetAcademyAuth();
	}
	return;
}

function HandleClanAuthMember(string a_Param)
{
	local ClanWnd script;
	local int gradeID;
	local string sName;
	local array<int> powers;
	local int i, Index;

	script = ClanWnd(GetScript("ClanWnd"));
	ParseInt(a_Param, "Grade", gradeID);
	ParseString(a_Param, "Name", sName);
	powers.Length = 32;
	i = 0;
	J0x57:

	// End:0x98 [Loop If]
	if(__NFUN_150__(i, 32))
	{
		ParseInt(a_Param, __NFUN_112__("PowerValue", string(i)), powers[i]);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x57;
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan2_CurrentSelectedMemberName", __NFUN_168__(__NFUN_168__(sName, "-"), GetStringByGradeID(gradeID)));
	Index = 1;
	i = 1;
	J0xFB:

	// End:0x155 [Loop If]
	if(__NFUN_152__(i, 9))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan2_Check10", string(i)), bool(powers[__NFUN_165__(Index)]));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xFB;
	}
	i = 1;
	J0x15C:

	// End:0x1B6 [Loop If]
	if(__NFUN_152__(i, 5))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan2_Check20", string(i)), bool(powers[__NFUN_165__(Index)]));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x15C;
	}
	i = 1;
	J0x1BD:

	// End:0x217 [Loop If]
	if(__NFUN_152__(i, 8))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan2_Check30", string(i)), bool(powers[__NFUN_165__(Index)]));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1BD;
	}
	// End:0x25A
	if(__NFUN_242__(count_all_check2("10", 9), true))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan2_Check100", true);		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan2_Check100", false);
	}
	// End:0x2CB
	if(__NFUN_242__(count_all_check2("20", 5), true))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan2_Check200", true);		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan2_Check200", false);
	}
	// End:0x33C
	if(__NFUN_242__(count_all_check2("30", 8), true))
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan2_Check300", true);		
	}
	else
	{
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ClanDrawerWnd.Clan2_Check300", false);
	}
	// End:0x493
	if(__NFUN_122__(script.m_myName, sName))
	{
		// End:0x3C8
		if(__NFUN_242__(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ClanDrawerWnd.Clan2_Check101"), true))
		{
			script.m_bJoin = 1;			
		}
		else
		{
			script.m_bJoin = 0;
		}
		// End:0x41E
		if(__NFUN_242__(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ClanDrawerWnd.Clan2_Check107"), true))
		{
			script.m_bCrest = 1;			
		}
		else
		{
			script.m_bCrest = 0;
		}
		// End:0x474
		if(__NFUN_242__(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ClanDrawerWnd.Clan2_Check105"), true))
		{
			script.m_bWar = 1;			
		}
		else
		{
			script.m_bWar = 0;
		}
		script.resetBtnShowHide();
	}
	// End:0x5F9
	if(__NFUN_122__(script.m_CurrentclanMasterReal, sName))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan2_CurrentSelectedMemberName", __NFUN_168__(__NFUN_168__(sName, "-"), GetSystemString(342)));
		i = 0;
		J0x507:

		// End:0x553 [Loop If]
		if(__NFUN_152__(i, 9))
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan2_Check10", string(i)), true);
			__NFUN_163__(i);
			// [Loop Continue]
			goto J0x507;
		}
		i = 0;
		J0x55A:

		// End:0x5A6 [Loop If]
		if(__NFUN_152__(i, 5))
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan2_Check20", string(i)), true);
			__NFUN_163__(i);
			// [Loop Continue]
			goto J0x55A;
		}
		i = 0;
		J0x5AD:

		// End:0x5F9 [Loop If]
		if(__NFUN_152__(i, 8))
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck(__NFUN_112__("ClanDrawerWnd.Clan2_Check30", string(i)), true);
			__NFUN_163__(i);
			// [Loop Continue]
			goto J0x5AD;
		}
	}
	return;
}

function HandleClearWarList(string a_Param)
{
	local int Condition;

	// End:0x99
	if(ParseInt(a_Param, "Condition", Condition))
	{
		// End:0x61
		if(__NFUN_154__(Condition, 0))
		{
			Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ClanDrawerWnd.Clan8_DeclaredListCtrl");			
		}
		else
		{
			Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ClanDrawerWnd.Clan8_GotDeclaredListCtrl");
		}
	}
	return;
}

function string GetStringByGradeID(int gradeID)
{
	local int stringIndex;

	stringIndex = -1;
	Debug(__NFUN_168__("gradeID", string(gradeID)));
	// End:0x3C
	if(__NFUN_154__(gradeID, 1))
	{
		stringIndex = 1406;		
	}
	else
	{
		// End:0x56
		if(__NFUN_154__(gradeID, 2))
		{
			stringIndex = 1407;			
		}
		else
		{
			// End:0x70
			if(__NFUN_154__(gradeID, 3))
			{
				stringIndex = 1408;				
			}
			else
			{
				// End:0x8A
				if(__NFUN_154__(gradeID, 4))
				{
					stringIndex = 1409;					
				}
				else
				{
					// End:0xA4
					if(__NFUN_154__(gradeID, 5))
					{
						stringIndex = 1410;						
					}
					else
					{
						// End:0xBE
						if(__NFUN_154__(gradeID, 6))
						{
							stringIndex = 1411;							
						}
						else
						{
							// End:0xD8
							if(__NFUN_154__(gradeID, 7))
							{
								stringIndex = 1412;								
							}
							else
							{
								// End:0xF2
								if(__NFUN_154__(gradeID, 8))
								{
									stringIndex = 1413;									
								}
								else
								{
									// End:0x109
									if(__NFUN_154__(gradeID, 9))
									{
										stringIndex = 1414;
									}
								}
							}
						}
					}
				}
			}
		}
	}
	// End:0x127
	if(__NFUN_155__(stringIndex, -1))
	{
		return GetSystemString(stringIndex);		
	}
	else
	{
		return "";
	}
	return;
}

function InitializeAcademyList()
{
	local ClanWnd script;
	local int i;
	local LVDataRecord Record;

	Record.LVDataList.Length = 3;
	script = ClanWnd(GetScript("ClanWnd"));
	InitializeClan1_AssignApprenticeList();
	i = 0;
	J0x35:

	// End:0x271 [Loop If]
	if(__NFUN_150__(i, script.m_memberList[script.GetIndexFromType(-1)].m_array.Length))
	{
		// End:0x267
		if(__NFUN_154__(script.m_memberList[script.GetIndexFromType(-1)].m_array[i].bHaveMaster, 0))
		{
			Record.LVDataList[0].szData = script.m_memberList[script.GetIndexFromType(-1)].m_array[i].sName;
			Record.LVDataList[1].szData = string(script.m_memberList[script.GetIndexFromType(-1)].m_array[i].Level);
			Record.nReserved1 = script.m_memberList[script.GetIndexFromType(-1)].m_array[i].clanType;
			Record.LVDataList[2].szData = string(script.m_memberList[script.GetIndexFromType(-1)].m_array[i].ClassID);
			Record.LVDataList[2].szTexture = GetClassIconName(script.m_memberList[script.GetIndexFromType(-1)].m_array[i].ClassID);
			Record.LVDataList[2].nTextureWidth = 11;
			Record.LVDataList[2].nTextureHeight = 11;
			Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ClanDrawerWnd.Clan1_AssignApprenticeList", Record);
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x35;
	}
	return;
}

function InitializeClan1_AssignApprenticeList()
{
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ClanDrawerWnd.Clan1_AssignApprenticeList");
	return;
}

function InitializeClanInfoWnd()
{
	local Color Blue, Red, DarkYellow;
	local ClanWnd script;
	local int i;
	local string ClanNameVal, ClanRankStr, ToolTip;
	local int clanType;

	Blue.R = 126;
	Blue.G = 158;
	Blue.B = 245;
	Red.R = 200;
	Red.G = 50;
	Red.B = 80;
	DarkYellow.R = 175;
	DarkYellow.G = 152;
	DarkYellow.B = 120;
	script = ClanWnd(GetScript("ClanWnd"));
	ClanNameVal = __NFUN_168__(string(script.m_clanNameValue), GetSystemString(1442));
	reset_clan_org();
	// End:0xE0
	if(__NFUN_154__(script.m_clanRank, 0))
	{
		ClanRankStr = GetSystemString(1374);		
	}
	else
	{
		// End:0x11B
		if(__NFUN_152__(script.m_clanRank, 100))
		{
			ClanRankStr = __NFUN_168__(GetSystemString(1375), string(script.m_clanRank));			
		}
		else
		{
			ClanRankStr = GetSystemString(1374);
		}
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan3_ClanName", script.m_clanName);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan3_ClanPoint", ClanNameVal);
	// End:0x1E4
	if(__NFUN_154__(script.m_clanNameValue, 0))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("ClanDrawerWnd.Clan3_ClanPoint", DarkYellow);		
	}
	else
	{
		// End:0x22E
		if(__NFUN_150__(script.m_clanNameValue, 0))
		{
			Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("ClanDrawerWnd.Clan3_ClanPoint", Red);			
		}
		else
		{
			// End:0x275
			if(__NFUN_151__(script.m_clanNameValue, 0))
			{
				Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("ClanDrawerWnd.Clan3_ClanPoint", Blue);
			}
		}
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan3_ClanRanking", ClanRankStr);
	i = 0;
	J0x2B1:

	// End:0x4E8 [Loop If]
	if(__NFUN_150__(i, 8))
	{
		// End:0x4DE
		if(__NFUN_123__(script.m_memberList[i].m_sName, ""))
		{
			clanType = script.GetClanTypeFromIndex(i);
			// End:0x356
			if(__NFUN_154__(clanType, 0))
			{
				ToolTip = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(script.m_memberList[i].m_sName, "\\n"), GetSystemString(342)), " : "), script.m_memberList[i].m_sMasterName);
			}
			// End:0x384
			if(__NFUN_154__(clanType, -1))
			{
				ToolTip = script.m_memberList[i].m_sName;
			}
			// End:0x3F2
			if(__NFUN_132__(__NFUN_154__(clanType, 100), __NFUN_154__(clanType, 200)))
			{
				ToolTip = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(script.m_memberList[i].m_sName, "\\n"), GetSystemString(1438)), " : "), script.m_memberList[i].m_sMasterName);
			}
			// End:0x488
			if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_154__(clanType, 1001), __NFUN_154__(clanType, 1002)), __NFUN_154__(clanType, 2001)), __NFUN_154__(clanType, 2002)))
			{
				ToolTip = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(script.m_memberList[i].m_sName, "\\n"), GetSystemString(1433)), " : "), script.m_memberList[i].m_sMasterName);
			}
			// End:0x4DE
			if(__NFUN_123__(ToolTip, ""))
			{
				Clan3_OrgIcon[i].ShowWindow();
				Clan3_OrgIcon[i].EnableWindow();
				Clan3_OrgIcon[i].SetTooltipCustomType(SetTooltip(ToolTip));
			}
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x2B1;
	}
	return;
}

function InitializeGradeComboBox()
{
	local int i;

	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("ClanDrawerWnd.Clan1_MemberGradeList");
	Class'NWindow.UIAPI_COMBOBOX'.static.AddString("ClanDrawerWnd.Clan1_MemberGradeList", GetSystemString(1451));
	i = 1;
	J0x7A:

	// End:0xCF [Loop If]
	if(__NFUN_150__(i, 6))
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.AddString("ClanDrawerWnd.Clan1_MemberGradeList", GetStringByGradeID(i));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x7A;
	}
	return;
}

function KnighthoodCombobox()
{
	local ClanWnd script;
	local int i;

	script = ClanWnd(GetScript("ClanWnd"));
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("ClanDrawerWnd.Clan1_targetknighthoodcombobox");
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("ClanDrawerWnd.Clan1_targetknighthoodmember");
	Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("ClanDrawerWnd.Clan1_targetknighthoodcombobox", GetSystemString(1465), 0);
	Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("ClanDrawerWnd.Clan1_targetknighthoodmember", GetSystemString(1466), 0);
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_targetknighthoodmember");
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodBtn");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodTXT1", MakeFullSystemMsg(GetSystemMessage(1906), m_currentName, ""));
	i = 0;
	J0x1F9:

	// End:0x2B1 [Loop If]
	if(__NFUN_150__(i, script.8))
	{
		// End:0x2A7
		if(__NFUN_123__(script.m_memberList[i].m_sName, ""))
		{
			// End:0x2A7
			if(__NFUN_155__(script.GetClanTypeFromIndex(i), -1))
			{
				Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("ClanDrawerWnd.Clan1_targetknighthoodcombobox", script.m_memberList[i].m_sName, i);
			}
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1F9;
	}
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("ClanDrawerWnd.Clan1_targetknighthoodcombobox", 0);
	return;
}

function swapTargetSelect(int clanNo)
{
	local ClanWnd script;
	local int i;

	script = ClanWnd(GetScript("ClanWnd"));
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("ClanDrawerWnd.Clan1_targetknighthoodmember");
	Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("ClanDrawerWnd.Clan1_targetknighthoodmember", GetSystemString(1466), 0);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodTXT1", MakeFullSystemMsg(GetSystemMessage(1907), m_currentName, ""));
	i = 0;
	J0xFA:

	// End:0x1E2 [Loop If]
	if(__NFUN_152__(i, script.m_memberList[clanNo].m_array.Length))
	{
		// End:0x1D8
		if(__NFUN_123__(script.m_memberList[clanNo].m_array[i].sName, script.m_CurrentclanMasterReal))
		{
			Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("ClanDrawerWnd.Clan1_targetknighthoodmember", script.m_memberList[clanNo].m_array[i].sName, script.m_memberList[clanNo].m_array[i].clanType);
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xFA;
	}
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("ClanDrawerWnd.Clan1_targetknighthoodmember", 0);
	return;
}

function proc_swapmember()
{
	local int currentindexnew1, currentindexnew2;
	local string currentstring1, currentstring2;
	local int Type, clantype1;
	local ClanWnd script;

	script = ClanWnd(GetScript("ClanWnd"));
	currentindexnew1 = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("ClanDrawerWnd.Clan1_targetknighthoodcombobox");
	currentstring1 = Class'NWindow.UIAPI_COMBOBOX'.static.GetString("ClanDrawerWnd.Clan1_targetknighthoodcombobox", currentindexnew1);
	clantype1 = script.GetClanTypeFromIndex(Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("ClanDrawerWnd.Clan1_targetknighthoodcombobox", currentindexnew1));
	currentindexnew2 = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("ClanDrawerWnd.Clan1_targetknighthoodmember");
	currentstring2 = Class'NWindow.UIAPI_COMBOBOX'.static.GetString("ClanDrawerWnd.Clan1_targetknighthoodmember", currentindexnew2);
	// End:0x198
	if(__NFUN_154__(currentindexnew2, 0))
	{
		Type = 0;		
	}
	else
	{
		Type = 1;
	}
	// End:0x1C3
	if(__NFUN_154__(Type, 1))
	{
		RequestClanReorganizeMember(1, m_currentName, clantype1, currentstring2);		
	}
	else
	{
		// End:0x1E1
		if(__NFUN_154__(Type, 0))
		{
			RequestClanReorganizeMember(0, m_currentName, clantype1, "");
		}
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ClanDrawerWnd.Clan1_CurrentSelectedMemberOrderName", __NFUN_168__(__NFUN_168__(getClanOrderString(clantype1), "-"), currentstring1));
	return;
}

function OnComboBoxItemSelected(string strID, int Index)
{
	local int selectval;

	// End:0xF6
	if(__NFUN_122__(strID, "Clan1_targetknighthoodcombobox"))
	{
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_targetknighthoodmember");
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("ClanDrawerWnd.Clan1_ChangeMemberKnightHoodBtn");
		selectval = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("ClanDrawerWnd.Clan1_targetknighthoodcombobox", Index);
		swapTargetSelect(selectval);
	}
	return;
}

function HideWindow()
{
	local ClanWnd script;

	script = ClanWnd(GetScript("ClanWnd"));
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ClanDrawerWnd");
	script.ResetOpeningVariables();
	return;
}

function ApplyEditGrade()
{
	local array<int> powers;
	local int i, Index;

	powers.Length = 32;
	powers[0] = 0;
	Index = 1;
	i = 1;
	J0x20:

	// End:0x82 [Loop If]
	if(__NFUN_152__(i, 9))
	{
		// End:0x71
		if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked(__NFUN_112__("ClanDrawerWnd.Clan6_Check10", string(i))))
		{
			powers[Index] = 1;
		}
		__NFUN_163__(Index);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x20;
	}
	i = 1;
	J0x89:

	// End:0xEB [Loop If]
	if(__NFUN_152__(i, 5))
	{
		// End:0xDA
		if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked(__NFUN_112__("ClanDrawerWnd.Clan6_Check20", string(i))))
		{
			powers[Index] = 1;
		}
		__NFUN_163__(Index);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x89;
	}
	i = 1;
	J0xF2:

	// End:0x154 [Loop If]
	if(__NFUN_152__(i, 8))
	{
		// End:0x143
		if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked(__NFUN_112__("ClanDrawerWnd.Clan6_Check30", string(i))))
		{
			powers[Index] = 1;
		}
		__NFUN_163__(Index);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xF2;
	}
	RequestEditClanAuth(m_currentEditGradeID, powers);
	return;
}

function EditAuthGrade()
{
	local int Index;
	local LVDataRecord Record;

	Index = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ClanDrawerWnd.Clan5_AuthListCtrl");
	// End:0xBD
	if(__NFUN_153__(Index, 0))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ClanDrawerWnd.Clan5_AuthListCtrl", Index);
		RequestClanAuth(Record.nReserved1);
		m_currentEditGradeID = Record.nReserved1;
		SetStateAndShow("ClanAuthEditWndState");		
	}
	else
	{
		SetStateAndShow("ClanAuthManageWndState");
	}
	return;
}

function EditAuthGrade2()
{
	local int Index;
	local LVDataRecord Record;

	Index = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ClanDrawerWnd.Clan5_AuthListCtrl2");
	// End:0xBF
	if(__NFUN_153__(Index, 0))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ClanDrawerWnd.Clan5_AuthListCtrl2", Index);
		RequestClanAuth(Record.nReserved1);
		m_currentEditGradeID = Record.nReserved1;
		SetStateAndShow("ClanAuthEditWndState");		
	}
	else
	{
		SetStateAndShow("ClanAuthManageWndState");
	}
	return;
}

function string GetWarStateString(int State)
{
	// End:0x1A
	if(__NFUN_154__(State, 0))
	{
		return GetSystemString(1429);		
	}
	else
	{
		// End:0x34
		if(__NFUN_154__(State, 1))
		{
			return GetSystemString(1430);			
		}
		else
		{
			// End:0x4C
			if(__NFUN_154__(State, 2))
			{
				return GetSystemString(1367);
			}
		}
	}
	return "Error";
	return;
}

function AddSkill(int Id, int Level)
{
	local ItemInfo Info;

	Info.ClassID = Id;
	Info.Level = Level;
	Info.Name = Class'NWindow.UIDATA_SKILL'.static.GetName(Info.ClassID, Info.Level);
	Info.IconName = Class'NWindow.UIDATA_SKILL'.static.GetIconName(Info.ClassID, Info.Level);
	Info.Description = Class'NWindow.UIDATA_SKILL'.static.GetDescription(Info.ClassID, Info.Level);
	Info.AdditionalName = Class'NWindow.UIDATA_SKILL'.static.GetEnchantName(Info.ClassID, Info.Level);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("ClanDrawerWnd.ClanSkillWnd", Info);
	return;
}

function ReplaceSkill(int Index, int Id, int Level)
{
	local ItemInfo Info;

	Info.ClassID = Id;
	Info.Level = Level;
	Info.Name = Class'NWindow.UIDATA_SKILL'.static.GetName(Info.ClassID, Info.Level);
	Info.IconName = Class'NWindow.UIDATA_SKILL'.static.GetIconName(Info.ClassID, Info.Level);
	Info.Description = Class'NWindow.UIDATA_SKILL'.static.GetDescription(Info.ClassID, Info.Level);
	Info.AdditionalName = Class'NWindow.UIDATA_SKILL'.static.GetEnchantName(Info.ClassID, Info.Level);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.SetItem("ClanDrawerWnd.ClanSkillWnd", Index, Info);
	return;
}

function string getClanOrderString(int gradeID)
{
	local int stringIndex;

	stringIndex = -1;
	// End:0x24
	if(__NFUN_154__(gradeID, 0))
	{
		stringIndex = 1399;		
	}
	else
	{
		// End:0x3E
		if(__NFUN_154__(gradeID, 100))
		{
			stringIndex = 1400;			
		}
		else
		{
			// End:0x58
			if(__NFUN_154__(gradeID, 200))
			{
				stringIndex = 1401;				
			}
			else
			{
				// End:0x75
				if(__NFUN_154__(gradeID, 1001))
				{
					stringIndex = 1402;					
				}
				else
				{
					// End:0x92
					if(__NFUN_154__(gradeID, 1002))
					{
						stringIndex = 1403;						
					}
					else
					{
						// End:0xAF
						if(__NFUN_154__(gradeID, 2001))
						{
							stringIndex = 1404;							
						}
						else
						{
							// End:0xCC
							if(__NFUN_154__(gradeID, 2002))
							{
								stringIndex = 1405;								
							}
							else
							{
								// End:0xE6
								if(__NFUN_154__(gradeID, -1))
								{
									stringIndex = 1419;
								}
							}
						}
					}
				}
			}
		}
	}
	// End:0x104
	if(__NFUN_155__(stringIndex, -1))
	{
		return GetSystemString(stringIndex);		
	}
	else
	{
		return "";
	}
	return;
}

function reset_clan_org()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x64 [Loop If]
	if(__NFUN_150__(i, 8))
	{
		Clan3_OrgIcon[i].HideWindow();
		Clan3_OrgIcon[i].DisableWindow();
		Clan3_OrgIcon[i].SetTooltipCustomType(SetTooltip(""));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function disableAcademyAuth()
{
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check100", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check101", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check102", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check106", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check104", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check105", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check107", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check108", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check109", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check110", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check200", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check203", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check204", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check205", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check300", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check303", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check302", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check305", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check306", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check307", true);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check308", true);
	return;
}

function resetAcademyAuth()
{
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check100", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check101", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check102", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check106", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check104", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check105", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check107", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check108", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check109", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check109", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check200", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check203", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check204", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check205", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check300", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check303", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check302", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check305", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check306", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check307", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ClanDrawerWnd.Clan6_Check308", false);
	return;
}

function int getCurrentGradebyClanType()
{
	local int GradeNum;

	switch(m_clanType)
	{
		// End:0x16
		case 0:
			GradeNum = 6;
			// End:0x98
			break;
		// End:0x26
		case 100:
			GradeNum = 7;
			// End:0x98
			break;
		// End:0x36
		case 200:
			GradeNum = 7;
			// End:0x98
			break;
		// End:0x49
		case 1001:
			GradeNum = 8;
			// End:0x98
			break;
		// End:0x5C
		case 1002:
			GradeNum = 8;
			// End:0x98
			break;
		// End:0x6F
		case 2001:
			GradeNum = 8;
			// End:0x98
			break;
		// End:0x82
		case 2002:
			GradeNum = 8;
			// End:0x98
			break;
		// End:0x95
		case -1:
			GradeNum = 9;
			// End:0x98
			break;
		// End:0xFFFF
		default:
			break;
	}
	return GradeNum;
	return;
}

function CustomTooltip SetTooltip(string Text)
{
	local CustomTooltip ToolTip;
	local DrawItemInfo Info;

	ToolTip.MinimumWidth = 144;
	ToolTip.DrawList.Length = 1;
	Info.eType = 1;
	Info.t_color.R = 178;
	Info.t_color.G = 190;
	Info.t_color.B = 207;
	Info.t_color.A = byte(255);
	Info.t_strText = Text;
	ToolTip.DrawList[0] = Info;
	return ToolTip;
	return;
}
