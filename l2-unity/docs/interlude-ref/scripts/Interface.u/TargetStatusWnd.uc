class TargetStatusWnd extends UIScript;

var bool m_bExpand;
var int m_TargetLevel;
var int m_targetID;
var bool m_bShow;

function OnLoad()
{
	RegisterEvent(980);
	RegisterEvent(990);
	RegisterEvent(190);
	RegisterEvent(210);
	RegisterEvent(200);
	RegisterEvent(220);
	RegisterEvent(300);
	SetExpandMode(false);
	m_bShow = false;
	m_targetID = -1;
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
	SetExpandMode(m_bExpand);
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x18
	if(__NFUN_154__(Event_ID, 980))
	{
		HandleTargetUpdate();		
	}
	else
	{
		// End:0x4A
		if(__NFUN_154__(Event_ID, 990))
		{
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd");			
		}
		else
		{
			// End:0x67
			if(__NFUN_154__(Event_ID, 300))
			{
				HandleReceiveTargetLevelDiff(param);				
			}
			else
			{
				// End:0x81
				if(__NFUN_154__(Event_ID, 190))
				{
					HandleUpdateHPMP(param);					
				}
				else
				{
					// End:0x9B
					if(__NFUN_154__(Event_ID, 210))
					{
						HandleUpdateHPMP(param);						
					}
					else
					{
						// End:0xB5
						if(__NFUN_154__(Event_ID, 200))
						{
							HandleUpdateHPMP(param);							
						}
						else
						{
							// End:0xCC
							if(__NFUN_154__(Event_ID, 220))
							{
								HandleUpdateHPMP(param);
							}
						}
					}
				}
			}
		}
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1D
		case "btnClose":
			OnCloseButton();
			// End:0x20
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnCloseButton()
{
	RequestTargetCancel();
	PlayConsoleSound(6);
	return;
}

function HandleUpdateHPMP(string param)
{
	local int ServerID;

	// End:0x38
	if(m_bShow)
	{
		ParseInt(param, "ServerID", ServerID);
		// End:0x38
		if(__NFUN_154__(m_targetID, ServerID))
		{
			HandleTargetUpdate();
		}
	}
	return;
}

function HandleReceiveTargetLevelDiff(string param)
{
	ParseInt(param, "LevelDiff", m_TargetLevel);
	return;
}

function HandleTargetUpdate()
{
	local Rect rectWnd;
	local string strTmp;
	local int targetID, PlayerID, PetID, clanType, clanNameValue;

	local bool bIsServerObject, bIsHPShowableNPC;
	local string Name, RankName;
	local Color TargetNameColor;
	local int ServerObjectNameID;
	local UIEventManager.EServerObjectType ServerObjectType;
	local bool bShowHPBar, bShowMPBar, bShowPledgeInfo, bShowPledgeTex, bShowPledgeAllianceTex;

	local string PledgeName, PledgeAllianceName;
	local Texture PledgeCrestTexture, PledgeAllianceCrestTexture;
	local Color PledgeNameColor, PledgeAllianceNameColor;
	local bool bShowNpcInfo;
	local array<int> arrNpcInfo;
	local bool IsTargetChanged;
	local UserInfo Info;
	local Color WhiteColor;

	WhiteColor.R = 0;
	WhiteColor.G = 0;
	WhiteColor.B = 0;
	targetID = Class'NWindow.UIDATA_TARGET'.static.GetTargetID();
	// End:0x69
	if(__NFUN_150__(targetID, 1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd");
		return;
	}
	// End:0x80
	if(__NFUN_155__(m_targetID, targetID))
	{
		IsTargetChanged = true;
	}
	m_targetID = targetID;
	GetTargetInfo(Info);
	rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect("TargetStatusWnd");
	PledgeName = GetSystemString(431);
	PledgeAllianceName = GetSystemString(591);
	PledgeNameColor.R = 128;
	PledgeNameColor.G = 128;
	PledgeNameColor.B = 128;
	PledgeAllianceNameColor.R = 128;
	PledgeAllianceNameColor.G = 128;
	PledgeAllianceNameColor.B = 128;
	TargetNameColor = Class'NWindow.UIDATA_TARGET'.static.GetTargetNameColor(m_TargetLevel);
	bIsServerObject = Class'NWindow.UIDATA_TARGET'.static.IsServerObject();
	// End:0x28A
	if(bIsServerObject)
	{
		ServerObjectNameID = Class'NWindow.UIDATA_STATICOBJECT'.static.GetServerObjectNameID(m_targetID);
		// End:0x1AC
		if(__NFUN_151__(ServerObjectNameID, 0))
		{
			Name = Class'NWindow.UIDATA_STATICOBJECT'.static.GetStaticObjectName(ServerObjectNameID);
			RankName = "";
		}
		Class'NWindow.UIAPI_NAMECTRL'.static.SetName("TargetStatusWnd.UserName", Name, 0, 2);
		Class'NWindow.UIAPI_NAMECTRL'.static.SetName("TargetStatusWnd.RankName", RankName, 0, 2);
		ServerObjectType = Class'NWindow.UIDATA_STATICOBJECT'.static.GetServerObjectType(m_targetID);
		// End:0x287
		if(__NFUN_154__(int(ServerObjectType), int(2)))
		{
			// End:0x287
			if(Class'NWindow.UIDATA_STATICOBJECT'.static.GetStaticObjectShowHP(m_targetID))
			{
				bShowHPBar = true;
				UpdateHPBar(Class'NWindow.UIDATA_STATICOBJECT'.static.GetServerObjectHP(m_targetID), Class'NWindow.UIDATA_STATICOBJECT'.static.GetServerObjectMaxHP(m_targetID));
			}
		}		
	}
	else
	{
		// End:0x372
		if(__NFUN_150__(__NFUN_125__(Info.Name), 1))
		{
			Name = Class'NWindow.UIDATA_PARTY'.static.GetMemberName(m_targetID);
			RankName = "";
			Debug(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("m_TargetID", string(m_targetID)), ", info.Name : "), Info.Name), ", Name : "), Name));
			Class'NWindow.UIAPI_NAMECTRL'.static.SetName("TargetStatusWnd.UserName", Name, 0, 2);
			Class'NWindow.UIAPI_NAMECTRL'.static.SetName("TargetStatusWnd.RankName", RankName, 0, 2);			
		}
		else
		{
			PlayerID = Class'NWindow.UIDATA_PLAYER'.static.GetPlayerID();
			PetID = Class'NWindow.UIDATA_PET'.static.GetPetID();
			bIsHPShowableNPC = Class'NWindow.UIDATA_TARGET'.static.IsHPShowableNPC();
			// End:0x5A6
			if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_130__(__NFUN_130__(Info.bNpc, __NFUN_129__(Info.bPet)), Info.bCanBeAttacked), __NFUN_130__(__NFUN_151__(PlayerID, 0), __NFUN_154__(m_targetID, PlayerID))), __NFUN_130__(__NFUN_130__(Info.bNpc, Info.bPet), __NFUN_154__(m_targetID, PetID))), __NFUN_130__(Info.bNpc, bIsHPShowableNPC)))
			{
				// End:0x4AC
				if(IsAllWhiteID(Info.nClassID))
				{
					Name = Info.Name;
					RankName = "";
					Class'NWindow.UIAPI_NAMECTRL'.static.SetName("TargetStatusWnd.UserName", Name, 0, 2);					
				}
				else
				{
					Name = Info.Name;
					RankName = "";
					Class'NWindow.UIAPI_NAMECTRL'.static.SetNameWithColor("TargetStatusWnd.UserName", Name, 0, 2, TargetNameColor);
					Class'NWindow.UIAPI_NAMECTRL'.static.SetName("TargetStatusWnd.RankName", RankName, 0, 2);
					bShowHPBar = true;
					UpdateHPBar(Info.nCurHP, Info.nMaxHP);
					// End:0x5A3
					if(__NFUN_129__(__NFUN_130__(__NFUN_130__(Info.bNpc, __NFUN_129__(Info.bPet)), Info.bCanBeAttacked)))
					{
						bShowMPBar = true;
						UpdateMPBar(Info.nCurMP, Info.nMaxMP);
					}
				}				
			}
			else
			{
				Name = Info.Name;
				// End:0x5CF
				if(Info.bNpc)
				{
					RankName = "";					
				}
				else
				{
					RankName = GetUserRankString(Info.nUserRank);
				}
				Class'NWindow.UIAPI_NAMECTRL'.static.SetName("TargetStatusWnd.UserName", Name, 0, 2);
				Class'NWindow.UIAPI_NAMECTRL'.static.SetName("TargetStatusWnd.RankName", RankName, 0, 2);
			}
			// End:0x925
			if(m_bExpand)
			{
				// End:0x6A0
				if(Info.bNpc)
				{
					// End:0x69D
					if(Class'NWindow.UIDATA_NPC'.static.GetNpcProperty(Info.nClassID, arrNpcInfo))
					{
						bShowNpcInfo = true;
						// End:0x69D
						if(IsTargetChanged)
						{
							UpdateNpcInfoTree(arrNpcInfo);
						}
					}					
				}
				else
				{
					bShowPledgeInfo = true;
					// End:0x925
					if(__NFUN_151__(Info.nClanID, 0))
					{
						PledgeName = Class'NWindow.UIDATA_CLAN'.static.GetName(Info.nClanID);
						PledgeNameColor.R = 176;
						PledgeNameColor.G = 152;
						PledgeNameColor.B = 121;
						// End:0x7EF
						if(__NFUN_130__(__NFUN_130__(__NFUN_123__(PledgeName, ""), Class'NWindow.UIDATA_USER'.static.GetClanType(m_targetID, clanType)), Class'NWindow.UIDATA_CLAN'.static.GetNameValue(Info.nClanID, clanNameValue)))
						{
							// End:0x784
							if(__NFUN_154__(clanType, -1))
							{
								PledgeNameColor.R = 209;
								PledgeNameColor.G = 167;
								PledgeNameColor.B = 2;								
							}
							else
							{
								// End:0x7BB
								if(__NFUN_151__(clanNameValue, 0))
								{
									PledgeNameColor.R = 0;
									PledgeNameColor.G = 130;
									PledgeNameColor.B = byte(255);									
								}
								else
								{
									// End:0x7EF
									if(__NFUN_150__(clanNameValue, 0))
									{
										PledgeNameColor.R = byte(255);
										PledgeNameColor.G = 0;
										PledgeNameColor.B = 0;
									}
								}
							}
						}
						// End:0x84F
						if(Class'NWindow.UIDATA_CLAN'.static.GetCrestTexture(Info.nClanID, PledgeCrestTexture))
						{
							bShowPledgeTex = true;
							Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTextureWithObject("TargetStatusWnd.texPledgeCrest", PledgeCrestTexture);							
						}
						else
						{
							bShowPledgeTex = false;
						}
						strTmp = Class'NWindow.UIDATA_CLAN'.static.GetAllianceName(Info.nClanID);
						// End:0x925
						if(__NFUN_151__(__NFUN_125__(strTmp), 0))
						{
							PledgeAllianceName = strTmp;
							PledgeAllianceNameColor.R = 176;
							PledgeAllianceNameColor.G = 155;
							PledgeAllianceNameColor.B = 121;
							// End:0x91D
							if(Class'NWindow.UIDATA_CLAN'.static.GetAllianceCrestTexture(Info.nClanID, PledgeAllianceCrestTexture))
							{
								bShowPledgeAllianceTex = true;
								Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTextureWithObject("TargetStatusWnd.texPledgeAllianceCrest", PledgeAllianceCrestTexture);								
							}
							else
							{
								bShowPledgeAllianceTex = false;
							}
						}
					}
				}
			}
		}
	}
	// End:0x976
	if(__NFUN_129__(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("TargetStatusWnd")))
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd");
		SetExpandMode(m_bExpand);
	}
	// End:0x9A8
	if(bShowHPBar)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.barHP");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.barHP");
	}
	// End:0xA00
	if(bShowMPBar)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.barMP");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.barMP");
	}
	// End:0xDE4
	if(bShowPledgeInfo)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.txtPledge");
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.txtAlliance");
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.txtPledgeName");
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.txtPledgeAllianceName");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("TargetStatusWnd.txtPledgeName", PledgeName);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("TargetStatusWnd.txtPledgeAllianceName", PledgeAllianceName);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("TargetStatusWnd.txtPledgeName", PledgeNameColor);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor("TargetStatusWnd.txtPledgeAllianceName", PledgeAllianceNameColor);
		// End:0xC4A
		if(bShowPledgeTex)
		{
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.texPledgeCrest");
			Class'NWindow.UIAPI_WINDOW'.static.MoveTo("TargetStatusWnd.txtPledgeName", __NFUN_146__(rectWnd.nX, 63), __NFUN_146__(rectWnd.nY, 43));			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.texPledgeCrest");
			Class'NWindow.UIAPI_WINDOW'.static.MoveTo("TargetStatusWnd.txtPledgeName", __NFUN_146__(rectWnd.nX, 45), __NFUN_146__(rectWnd.nY, 43));
		}
		// End:0xD58
		if(bShowPledgeAllianceTex)
		{
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.texPledgeAllianceCrest");
			Class'NWindow.UIAPI_WINDOW'.static.MoveTo("TargetStatusWnd.txtPledgeAllianceName", __NFUN_146__(rectWnd.nX, 63), __NFUN_146__(rectWnd.nY, 59));			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.texPledgeAllianceCrest");
			Class'NWindow.UIAPI_WINDOW'.static.MoveTo("TargetStatusWnd.txtPledgeAllianceName", __NFUN_146__(rectWnd.nX, 45), __NFUN_146__(rectWnd.nY, 59));
		}		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.txtPledge");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.texPledgeCrest");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.txtPledgeName");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.txtAlliance");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.texPledgeAllianceCrest");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.txtPledgeAllianceName");
	}
	// End:0xF61
	if(bShowNpcInfo)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.NpcInfo");
		Class'NWindow.UIAPI_TREECTRL'.static.ShowScrollBar("TargetStatusWnd.NpcInfo", false);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.NpcInfo");
	}
	return;
}

function OnFrameExpandClick(bool bIsExpand)
{
	SetExpandMode(bIsExpand);
	return;
}

function SetExpandMode(bool bExpand)
{
	m_bExpand = bExpand;
	m_targetID = -1;
	HandleTargetUpdate();
	// End:0x7D
	if(bExpand)
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.BackTex");
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.BackExpTex");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("TargetStatusWnd.BackTex");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("TargetStatusWnd.BackExpTex");
	}
	return;
}

function UpdateHPBar(int HP, int maxHP)
{
	Class'NWindow.UIAPI_BARCTRL'.static.SetValue("TargetStatusWnd.barHP", maxHP, HP);
	return;
}

function UpdateMPBar(int MP, int maxMP)
{
	Class'NWindow.UIAPI_BARCTRL'.static.SetValue("TargetStatusWnd.barMP", maxMP, MP);
	return;
}

function UpdateNpcInfoTree(array<int> arrNpcInfo)
{
	local int i, SkillID, SkillLevel;
	local string strNodeName;
	local XMLTreeNodeInfo infNode;
	local XMLTreeNodeItemInfo infNodeItem;
	local XMLTreeNodeInfo infNodeClear;
	local XMLTreeNodeItemInfo infNodeItemClear;

	Class'NWindow.UIAPI_TREECTRL'.static.Clear("TargetStatusWnd.NpcInfo");
	infNode.strName = "root";
	strNodeName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("TargetStatusWnd.NpcInfo", "", infNode);
	// End:0xB6
	if(__NFUN_150__(__NFUN_125__(strNodeName), 1))
	{
		Debug(__NFUN_112__("ERROR: Can't insert root node. Name: ", infNode.strName));
		return;
	}
	i = 0;
	J0xBD:

	// End:0x2FA [Loop If]
	if(__NFUN_150__(i, arrNpcInfo.Length))
	{
		SkillID = arrNpcInfo[i];
		SkillLevel = arrNpcInfo[__NFUN_146__(i, 1)];
		infNode = infNodeClear;
		infNode.nOffSetX = __NFUN_144__(int(__NFUN_173__(float(__NFUN_145__(i, 2)), float(8))), 18);
		// End:0x162
		if(__NFUN_180__(__NFUN_173__(float(__NFUN_145__(i, 2)), float(8)), float(0)))
		{
			// End:0x153
			if(__NFUN_151__(i, 0))
			{
				infNode.nOffSetY = 3;				
			}
			else
			{
				infNode.nOffSetY = 0;
			}			
		}
		else
		{
			infNode.nOffSetY = -15;
		}
		infNode.strName = __NFUN_112__("", string(__NFUN_145__(i, 2)));
		infNode.bShowButton = 0;
		infNode.ToolTip = SetNpcInfoTooltip(SkillID, SkillLevel);
		strNodeName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode("TargetStatusWnd.NpcInfo", "root", infNode);
		// End:0x22B
		if(__NFUN_150__(__NFUN_125__(strNodeName), 1))
		{
			__NFUN_231__(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
			return;
		}
		infNode.ToolTip.DrawList.Remove(0, infNode.ToolTip.DrawList.Length);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 2;
		infNodeItem.u_nTextureWidth = 15;
		infNodeItem.u_nTextureHeight = 15;
		infNodeItem.u_nTextureUWidth = 32;
		infNodeItem.u_nTextureUHeight = 32;
		infNodeItem.u_strTexture = Class'NWindow.UIDATA_SKILL'.static.GetIconName(SkillID, SkillLevel);
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem("TargetStatusWnd.NpcInfo", strNodeName, infNodeItem);
		__NFUN_161__(i, 2);
		// [Loop Continue]
		goto J0xBD;
	}
	return;
}

function CustomTooltip SetNpcInfoTooltip(int Id, int Level)
{
	local CustomTooltip ToolTip;
	local DrawItemInfo Info, infoClear;
	local ItemInfo item;

	item.Name = Class'NWindow.UIDATA_SKILL'.static.GetName(Id, Level);
	item.Description = Class'NWindow.UIDATA_SKILL'.static.GetDescription(Id, Level);
	ToolTip.DrawList.Length = 1;
	Info = infoClear;
	Info.eType = 1;
	Info.t_bDrawOneLine = true;
	Info.t_strText = item.Name;
	ToolTip.DrawList[0] = Info;
	// End:0x171
	if(__NFUN_151__(__NFUN_125__(item.Description), 0))
	{
		ToolTip.MinimumWidth = 144;
		ToolTip.DrawList.Length = 2;
		Info = infoClear;
		Info.eType = 1;
		Info.nOffSetY = 6;
		Info.bLineBreak = true;
		Info.t_color.R = 178;
		Info.t_color.G = 190;
		Info.t_color.B = 207;
		Info.t_color.A = byte(255);
		Info.t_strText = item.Description;
		ToolTip.DrawList[1] = Info;
	}
	return ToolTip;
	return;
}

function bool IsAllWhiteID(int m_targetID)
{
	local bool bIsAllWhiteName;

	bIsAllWhiteName = false;
	switch(m_targetID)
	{
		// End:0x17
		case 12778:
		// End:0x1F
		case 13031:
		// End:0x27
		case 13032:
		// End:0x2F
		case 13033:
		// End:0x37
		case 13034:
		// End:0x4A
		case 13035:
			bIsAllWhiteName = true;
			// End:0x4D
			break;
		// End:0xFFFF
		default:
			break;
	}
	return bIsAllWhiteName;
	return;
}
