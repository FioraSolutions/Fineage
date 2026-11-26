class MinimapWnd_Expand extends UICommonAPI;

const N_MAX_MINI_MAP_RES_X = 1024;
const N_MAX_MINI_MAP_RES_Y = 1024;
const N_BUTTON_HEAD_AREA_BUFFER = 90;

var int m_PartyMemberCount;
var int m_PartyLocIndex;
var bool m_AdjustCursedLoc;
var int m_SSQStatus;
var bool m_bShowSSQType;
var bool m_bShowCurrentLocation;
var bool m_bShowGameTime;
var array<ResolutionInfo> ResolutionList;
var WindowHandle m_hMinimapWnd;
var WindowHandle MiniMapCtrl;

function OnLoad()
{
	m_PartyLocIndex = -1;
	m_PartyMemberCount = GetPartyMemberCount();
	RegisterEvent(520);
	RegisterEvent(1790);
	RegisterEvent(1800);
	RegisterEvent(1810);
	RegisterEvent(1820);
	RegisterEvent(1830);
	RegisterEvent(1840);
	RegisterEvent(1850);
	RegisterEvent(1860);
	RegisterEvent(2900);
	RegisterEvent(2420);
	RegisterEvent(1870);
	RegisterEvent(1880);
	RegisterEvent(1890);
	m_AdjustCursedLoc = false;
	GetResolutionList(ResolutionList);
	MiniMapCtrl = GetHandle("MinimapWnd_Expand.Minimap");
	m_hMinimapWnd = GetHandle("MinimapWnd_Expand");
	m_bShowSSQType = true;
	m_bShowCurrentLocation = true;
	m_bShowGameTime = true;
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 520:
			HandlePartyMemberChanged(a_Param);
			// End:0x1B1
			break;
		// End:0x33
		case 1790:
			HandleMinimapAddTarget(a_Param);
			// End:0x1B1
			break;
		// End:0x49
		case 1800:
			HandleMinimapDeleteTarget(a_Param);
			// End:0x1B1
			break;
		// End:0x5A
		case 1810:
			HandleMinimapDeleteAllTarget();
			// End:0x1B1
			break;
		// End:0x6B
		case 1820:
			HandleMinimapShowQuest();
			// End:0x1B1
			break;
		// End:0x7C
		case 1830:
			HandleMinimapHideQuest();
			// End:0x1B1
			break;
		// End:0x8E
		case 1840:
			AdjustMapToPlayerPosition(true);
			// End:0x1B1
			break;
		// End:0xC4
		case 1850:
			// End:0xB6
			if(__NFUN_129__(IsShowWindow("MinimapWnd_Expand")))
			{
				return;
			}
			HandleCursedWeaponList(a_Param);
			// End:0x1B1
			break;
		// End:0xFA
		case 1860:
			// End:0xEC
			if(__NFUN_129__(IsShowWindow("MinimapWnd_Expand")))
			{
				return;
			}
			HandleCursedWeaponLoctaion(a_Param);
			// End:0x1B1
			break;
		// End:0x10B
		case 2420:
			SetCurrentLocation();
			// End:0x1B1
			break;
		// End:0x142
		case 1870:
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("MinimapWnd_Expand.btnReduce");
			// End:0x1B1
			break;
		// End:0x179
		case 1880:
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MinimapWnd_Expand.btnReduce");
			// End:0x1B1
			break;
		// End:0x198
		case 1890:
			// End:0x195
			if(m_bShowGameTime)
			{
				HandleUpdateGameTime(a_Param);
			}
			// End:0x1B1
			break;
		// End:0x1AE
		case 2900:
			HandleResolutionChanged(a_Param);
			// End:0x1B1
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnShow()
{
	AdjustMapToPlayerPosition(true);
	Class'NWindow.AudioAPI'.static.PlaySound("interfacesound.Interface.map_open_01");
	SetSSQTypeText();
	SetCurrentLocation();
	CheckResolution();
	Class'NWindow.MiniMapAPI'.static.RequestCursedWeaponList();
	Class'NWindow.MiniMapAPI'.static.RequestCursedWeaponLocation();
	return;
}

function HandleResolutionChanged(string aParam)
{
	local int NewWidth, NewHeight;

	ParseInt(aParam, "NewWidth", NewWidth);
	ParseInt(aParam, "NewHeight", NewHeight);
	ResetMiniMapSize(NewWidth, NewHeight);
	return;
}

function SetSSQTypeText()
{
	local string SSQText;
	local MinimapWnd MinimapWndScript;

	MinimapWndScript = MinimapWnd(GetScript("MinimapWnd"));
	switch(MinimapWndScript.m_SSQStatus)
	{
		// End:0x45
		case 0:
			SSQText = GetSystemString(973);
			// End:0x92
			break;
		// End:0x5D
		case 1:
			SSQText = GetSystemString(974);
			// End:0x92
			break;
		// End:0x76
		case 2:
			SSQText = GetSystemString(975);
			// End:0x92
			break;
		// End:0x8F
		case 3:
			SSQText = GetSystemString(976);
			// End:0x92
			break;
		// End:0xFFFF
		default:
			break;
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("Minimapwnd_Expand.txtVarSSQType", SSQText);
	return;
}

function SetCurrentLocation()
{
	local string ZoneName;

	ZoneName = GetCurrentZoneName();
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("Minimapwnd_Expand.txtVarCurLoc", ZoneName);
	return;
}

function CheckResolution()
{
	local int CurrentMaxWidth, CurrentMaxHeight;

	Debug("MinimapExpand - Checkresolution");
	GetCurrentResolution(CurrentMaxWidth, CurrentMaxHeight);
	Debug(__NFUN_168__("ÇöÀç ÇØ»óµµX:", string(CurrentMaxWidth)));
	Debug(__NFUN_168__("ÇöÀç ÇØ»óµµY:", string(CurrentMaxHeight)));
	ResetMiniMapSize(CurrentMaxWidth, CurrentMaxHeight);
	return;
}

function ResetMiniMapSize(int CurrentWidth, int CurrentHeight)
{
	local int adjustedwidth, adjustedheight, MainMapWidth, MainMapHeight;

	Debug("MinimapExpandWnd - ResetMinimapSize");
	MainMapWidth = CurrentWidth;
	MainMapHeight = CurrentHeight;
	adjustedwidth = __NFUN_147__(CurrentWidth, __NFUN_145__(__NFUN_144__(CurrentWidth, 3), 100));
	adjustedheight = __NFUN_147__(CurrentHeight, 90);
	// End:0x93
	if(__NFUN_151__(CurrentWidth, 1024))
	{
		adjustedwidth = __NFUN_147__(1024, 8);
		MainMapWidth = 1024;
	}
	// End:0xBC
	if(__NFUN_151__(CurrentHeight, 1024))
	{
		adjustedheight = __NFUN_147__(1024, 90);
		MainMapHeight = 1024;
	}
	m_hMinimapWnd.SetWindowSize(MainMapWidth, MainMapHeight);
	MiniMapCtrl.SetWindowSize(adjustedwidth, adjustedheight);
	OnClickMyLocButton();
	return;
}

function OnHide()
{
	Class'NWindow.AudioAPI'.static.PlaySound("interfacesound.Interface.map_close_01");
	return;
}

function HandlePartyMemberChanged(string a_Param)
{
	ParseInt(a_Param, "PartyMemberCount", m_PartyMemberCount);
	return;
}

function HandleMinimapAddTarget(string a_Param)
{
	local Vector Loc;

	// End:0xB5
	if(__NFUN_130__(__NFUN_130__(ParseFloat(a_Param, "X", Loc.X), ParseFloat(a_Param, "Y", Loc.Y)), ParseFloat(a_Param, "Z", Loc.Z)))
	{
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AddTarget("MinimapWnd_Expand.Minimap", Loc);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd_Expand.Minimap", Loc, false, false);
	}
	return;
}

function HandleMinimapDeleteTarget(string a_Param)
{
	local Vector Loc;
	local int LocX, LocY, LocZ;

	// End:0xAB
	if(__NFUN_130__(__NFUN_130__(ParseInt(a_Param, "X", LocX), ParseInt(a_Param, "Y", LocY)), ParseInt(a_Param, "Z", LocZ)))
	{
		Loc.X = float(LocX);
		Loc.Y = float(LocY);
		Loc.Z = float(LocZ);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.DeleteTarget("MinimapWnd_Expand.Minimap", Loc);
	}
	return;
}

function HandleMinimapDeleteAllTarget()
{
	Class'NWindow.UIAPI_MINIMAPCTRL'.static.DeleteAllTarget("MinimapWnd_Expand.Minimap");
	return;
}

function HandleMinimapShowQuest()
{
	Debug("MinimapWnd_Expand.HandleMinimapShowQuest");
	Class'NWindow.UIAPI_MINIMAPCTRL'.static.SetShowQuest("MinimapWnd_Expand.Minimap", true);
	return;
}

function HandleMinimapHideQuest()
{
	Debug("MinimapWnd_Expand.HandleMinimapHideQuest");
	Class'NWindow.UIAPI_MINIMAPCTRL'.static.SetShowQuest("MinimapWnd_Expand.Minimap", false);
	return;
}

function RequestCursedWeaponLocation()
{
	Debug("MinimapWnd_Expand.RequestCursedweaponLocation");
	return;
}

function OnComboBoxItemSelected(string sName, int Index)
{
	local int CursedweaponComboBoxCurrentReservedData;

	// End:0x56
	if(__NFUN_122__(sName, "CursedComboBox"))
	{
		CursedweaponComboBoxCurrentReservedData = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("MinimapWnd_Expand.CursedComboBox", Index);
	}
	return;
}

function OnClickButton(string a_ButtonID)
{
	switch(a_ButtonID)
	{
		// End:0x21
		case "TargetButton":
			OnClickTargetButton();
			// End:0x102
			break;
		// End:0x3A
		case "MyLocButton":
			OnClickMyLocButton();
			// End:0x102
			break;
		// End:0x56
		case "PartyLocButton":
			OnClickPartyLocButton();
			// End:0x102
			break;
		// End:0x7C
		case "Pursuit":
			m_AdjustCursedLoc = true;
			Class'NWindow.MiniMapAPI'.static.RequestCursedWeaponLocation();
			// End:0x102
			break;
		// End:0x98
		case "CollapseButton":
			OnClickCollapseButton();
			// End:0x102
			break;
		// End:0xFF
		case "btnReduce":
			Class'NWindow.UIAPI_MINIMAPCTRL'.static.RequestReduceBtn("MinimapWnd_Expand.Minimap");
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MinimapWnd_Expand.btnReduce");
			// End:0x102
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickCollapseButton()
{
	local MinimapWnd MinimapWndScript;

	MinimapWndScript = MinimapWnd(GetScript("MinimapWnd"));
	MinimapWndScript.SetExpandState(false);
	m_hMinimapWnd.HideWindow();
	ShowWindowWithFocus("MinimapWnd");
	return;
}

function OnClickTargetButton()
{
	local Vector QuestLocation;

	// End:0x3D
	if(GetQuestLocation(QuestLocation))
	{
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd_Expand.Minimap", QuestLocation);
	}
	return;
}

function OnClickMyLocButton()
{
	AdjustMapToPlayerPosition(true);
	return;
}

function AdjustMapToPlayerPosition(bool a_ZoomToTownMap)
{
	local Vector PlayerPosition;

	PlayerPosition = GetPlayerPosition();
	Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd_Expand.Minimap", PlayerPosition, a_ZoomToTownMap);
	return;
}

function OnClickPartyLocButton()
{
	local Vector PartyMemberLocation;

	m_PartyMemberCount = GetPartyMemberCount();
	// End:0x19
	if(__NFUN_154__(0, m_PartyMemberCount))
	{
		return;
	}
	m_PartyLocIndex = int(__NFUN_173__(float(__NFUN_146__(m_PartyLocIndex, 1)), float(m_PartyMemberCount)));
	// End:0x77
	if(GetPartyMemberLocation(m_PartyLocIndex, PartyMemberLocation))
	{
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd_Expand.Minimap", PartyMemberLocation, false);
	}
	return;
}

function HandleCursedWeaponList(string param)
{
	local int Num, itemID, i;
	local string cursedName;

	ParseInt(param, "NUM", Num);
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("MinimapWnd_Expand.CursedComboBox");
	i = 0;
	J0x4D:

	// End:0x160 [Loop If]
	if(__NFUN_150__(i, __NFUN_146__(Num, 1)))
	{
		// End:0xA7
		if(__NFUN_154__(i, 0))
		{
			Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("MinimapWnd_Expand.CursedComboBox", GetSystemString(1463), 0);
		}
		ParseInt(param, __NFUN_112__("ID", string(__NFUN_147__(i, 1))), itemID);
		ParseString(param, __NFUN_112__("NAME", string(__NFUN_147__(i, 1))), cursedName);
		Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("MinimapWnd_Expand.CursedComboBox", cursedName, itemID);
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("MinimapWnd_Expand.CursedComboBox", 0);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x4D;
	}
	Class'NWindow.UIAPI_MINIMAPCTRL'.static.DeleteAllTarget("MinimapWnd_Expand.Minimap");
	return;
}

function HandleCursedWeaponLoctaion(string param)
{
	local int Num, itemID, itemID1, itemID2, isowndedo, isownded1,
		isownded2, X, Y, Z, i;

	local Vector CursedWeaponLoc1, CursedWeaponLoc2;
	local int CursedWeaponComboCurrentData;
	local string combocursedName, cursedName, cursedName1, cursedName2;
	local Vector cursedWeaponLocation;
	local bool combined;

	ParseInt(param, "NUM", Num);
	// End:0x88
	if(__NFUN_154__(Num, 0))
	{
		// End:0x59
		if(m_AdjustCursedLoc)
		{
			Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd_Expand.Minimap", GetPlayerPosition());
		}
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.DeleteAllCursedWeaponIcon("MinimapWnd_Expand.Minimap");
		return;		
	}
	else
	{
		i = 0;
		J0x8F:

		// End:0x27D [Loop If]
		if(__NFUN_150__(i, Num))
		{
			ParseInt(param, __NFUN_112__("ID", string(i)), itemID);
			ParseString(param, __NFUN_112__("NAME", string(i)), cursedName);
			ParseInt(param, __NFUN_112__("ISOWNED", string(i)), isowndedo);
			ParseInt(param, __NFUN_112__("X", string(i)), X);
			ParseInt(param, __NFUN_112__("Y", string(i)), Y);
			ParseInt(param, __NFUN_112__("Z", string(i)), Z);
			cursedWeaponLocation.X = float(X);
			cursedWeaponLocation.Y = float(Y);
			cursedWeaponLocation.Z = float(Z);
			__NFUN_226__(cursedWeaponLocation);
			switch(i)
			{
				// End:0x202
				case 0:
					itemID1 = itemID;
					cursedName1 = cursedName;
					isownded1 = isowndedo;
					CursedWeaponLoc1.X = cursedWeaponLocation.X;
					CursedWeaponLoc1.Y = cursedWeaponLocation.Y;
					CursedWeaponLoc1.Z = cursedWeaponLocation.Z;
					__NFUN_226__(CursedWeaponLoc1);
					// End:0x273
					break;
				// End:0x270
				case 1:
					itemID2 = itemID;
					cursedName2 = cursedName;
					isownded2 = isowndedo;
					CursedWeaponLoc2.X = cursedWeaponLocation.X;
					CursedWeaponLoc2.Y = cursedWeaponLocation.Y;
					CursedWeaponLoc2.Z = cursedWeaponLocation.Z;
					__NFUN_226__(CursedWeaponLoc2);
					// End:0x273
					break;
				// End:0xFFFF
				default:
					break;
			}
			__NFUN_163__(i);
			// [Loop Continue]
			goto J0x8F;
		}
	}
	// End:0x38F
	if(m_AdjustCursedLoc)
	{
		m_AdjustCursedLoc = false;
		CursedWeaponComboCurrentData = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("MinimapWnd_Expand.CursedComboBox");
		combocursedName = Class'NWindow.UIAPI_COMBOBOX'.static.GetString("MinimapWnd_Expand.CursedComboBox", CursedWeaponComboCurrentData);
		// End:0x343
		if(__NFUN_122__(combocursedName, cursedName1))
		{
			Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd_Expand.Minimap", CursedWeaponLoc1, false);			
		}
		else
		{
			// End:0x385
			if(__NFUN_122__(combocursedName, cursedName2))
			{
				Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd_Expand.Minimap", CursedWeaponLoc2, false);				
			}
			else
			{
				AdjustMapToPlayerPosition(true);
			}
		}		
	}
	else
	{
		// End:0x3D6
		if(__NFUN_154__(Num, 1))
		{
			DrawCursedWeapon("MinimapWnd_Expand.Minimap", itemID1, cursedName1, CursedWeaponLoc1, __NFUN_154__(isownded1, 0), true);			
		}
		else
		{
			// End:0x56E
			if(__NFUN_154__(Num, 2))
			{
				combined = Class'NWindow.UIAPI_MINIMAPCTRL'.static.IsOverlapped("MinimapWnd_Expand.Minimap", int(CursedWeaponLoc1.X), int(CursedWeaponLoc1.Y), int(CursedWeaponLoc2.X), int(CursedWeaponLoc2.Y));
				// End:0x4DB
				if(combined)
				{
					Class'NWindow.UIAPI_MINIMAPCTRL'.static.DrawGridIcon("MinimapWnd_Expand.Minimap", "L2UI_CH3.MiniMap.cursedmapicon00", "L2UI_CH3.MiniMap.cursedmapicon00", CursedWeaponLoc1, true, 0, -12, __NFUN_112__(__NFUN_112__(cursedName1, "\\n"), cursedName2));					
				}
				else
				{
					Debug(__NFUN_168__(__NFUN_168__("ownded:", string(isownded1)), string(isownded2)));
					DrawCursedWeapon("MinimapWnd_Expand.Minimap", itemID1, cursedName1, CursedWeaponLoc1, __NFUN_154__(isownded1, 0), true);
					DrawCursedWeapon("MinimapWnd_Expand.Minimap", itemID2, cursedName2, CursedWeaponLoc2, __NFUN_154__(isownded2, 0), false);
				}
			}
		}
	}
	return;
}

function DrawCursedWeapon(string WindowName, int itemID, string cursedName, Vector vecLoc, bool bDropped, bool bRefresh)
{
	local string itemIconName;

	// End:0x3A
	if(__NFUN_154__(itemID, 8190))
	{
		itemIconName = "L2UI_CH3.MiniMap.cursedmapicon01";		
	}
	else
	{
		// End:0x71
		if(__NFUN_154__(itemID, 8689))
		{
			itemIconName = "L2UI_CH3.MiniMap.cursedmapicon02";
		}
	}
	// End:0x8E
	if(bDropped)
	{
		itemIconName = __NFUN_112__(itemIconName, "_drop");
	}
	Class'NWindow.UIAPI_MINIMAPCTRL'.static.DrawGridIcon(WindowName, itemIconName, "L2UI_CH3.MiniMap.cursedmapicon00", vecLoc, bRefresh, 0, -12, cursedName);
	return;
}

function HandleUpdateGameTime(string a_Param)
{
	local int GameHour, GameMinute;
	local string GameTimeString;

	ParseInt(a_Param, "GameHour", GameHour);
	ParseInt(a_Param, "GameMinute", GameMinute);
	SelectSunOrMoon(GameHour);
	// End:0x64
	if(__NFUN_153__(GameHour, 12))
	{
		GameTimeString = "PM ";
		__NFUN_162__(GameHour, 12);		
	}
	else
	{
		GameTimeString = "AM ";
	}
	// End:0x82
	if(__NFUN_154__(GameHour, 0))
	{
		GameHour = 12;
	}
	// End:0xB1
	if(__NFUN_150__(GameHour, 10))
	{
		GameTimeString = __NFUN_112__(__NFUN_112__(__NFUN_112__(GameTimeString, "0"), string(GameHour)), " : ");		
	}
	else
	{
		GameTimeString = __NFUN_112__(__NFUN_112__(GameTimeString, string(GameHour)), " : ");
	}
	// End:0xF4
	if(__NFUN_150__(GameMinute, 10))
	{
		GameTimeString = __NFUN_112__(__NFUN_112__(GameTimeString, "0"), string(GameMinute));		
	}
	else
	{
		GameTimeString = __NFUN_112__(GameTimeString, string(GameMinute));
	}
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("MinimapWnd_Expand.txtGameTime", GameTimeString);
	return;
}

function SelectSunOrMoon(int GameHour)
{
	// End:0x70
	if(__NFUN_130__(__NFUN_153__(GameHour, 6), __NFUN_152__(GameHour, 24)))
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("MinimapWnd_Expand.texSun");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MinimapWnd_Expand.texMoon");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("MinimapWnd_Expand.texMoon");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MinimapWnd_Expand.texSun");
	}
	return;
}
