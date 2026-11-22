class GuideWnd extends UIScript;

const MAX_QUEST_NUM = 2000;
const MAX_HUNTINGZONE_NUM = 500;
const MAX_RAID_NUM = 2000;
const TIMER_ID = 1;
const TIMER_DELAY = 5000;
const HUNTING_ZONE_TYPE = 0;
const HUNTING_ZONE_FIELDHUTINGZONE = 1;
const HUNTING_ZONE_DUNGEON = 2;
const HUNTING_ZONE_CASTLEVILLE = 3;
const HUNTING_ZONE_HARBOR = 4;
const HUNTING_ZONE_AZIT = 5;
const HUNTING_ZONE_COLOSSEUM = 6;
const HUNTING_ZONE_ETCERA = 7;
const ANTARASMONID1 = 29066;
const ANTARASMONID2 = 29067;
const ANTARASMONID3 = 29068;

struct RAIDRECORD
{
	var int A;
	var int B;
	var int C;
};

var bool block;
var array<RAIDRECORD> RaidRecordList;
var ListCtrlHandle m_hQuestListCtrl;
var ListCtrlHandle m_hHuntingZoneListCtrl;
var ListCtrlHandle m_hRaidListCtrl;
var ListCtrlHandle m_hAreaInfoListCtrl;
var TabHandle m_hTabCtrl;
var ComboBoxHandle m_hQuestComboBox;

function OnLoad()
{
	RegisterEvent(120);
	RegisterEvent(130);
	m_hQuestListCtrl = ListCtrlHandle(GetHandle("QuestListCtrl"));
	m_hHuntingZoneListCtrl = ListCtrlHandle(GetHandle("HuntingZoneListCtrl"));
	m_hRaidListCtrl = ListCtrlHandle(GetHandle("RaidListCtrl"));
	m_hAreaInfoListCtrl = ListCtrlHandle(GetHandle("AreaInfoListCtrl"));
	m_hTabCtrl = TabHandle(GetHandle("TabCtrl"));
	m_hQuestComboBox = ComboBoxHandle(GetHandle("QuestComboBox"));
	return;
}

function OnShow()
{
	block = false;
	m_hHuntingZoneListCtrl.DeleteAllItem();
	m_hRaidListCtrl.DeleteAllItem();
	m_hAreaInfoListCtrl.DeleteAllItem();
	m_hTabCtrl.InitTabCtrl();
	LoadQuestList();
	m_hOwnerWnd.SetTimer(1, 5000);
	return;
}

function OnHide()
{
	Class'NWindow.UIAPI_WINDOW'.static.KillUITimer("GuideWnd.RaidTab", 1);
	block = false;
	return;
}

function OnTimer(int TimerID)
{
	// End:0x13
	if(__NFUN_154__(TimerID, 1))
	{
		block = false;
	}
	return;
}

function OnClickButton(string Id)
{
	local int QuestComboCurrentData, QuestComboCurrentReservedData, HuntingZoneComboboxCurrentData, HuntingZoneComboboxCurrentReservedData, RaidCurrentComboboxCurrentReservedData, AreaInfoComboBoxCurrentData,
		AreaInfoComboBoxCurrentReservedData;

	// End:0x2D
	if(__NFUN_122__(Id, "TabCtrl0"))
	{
		LoadQuestList();
		m_hQuestComboBox.SetSelectedNum(0);		
	}
	else
	{
		// End:0x78
		if(__NFUN_122__(Id, "TabCtrl1"))
		{
			LoadHuntingZoneList();
			Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("GuideWnd.HuntingZoneComboBox", 0);			
		}
		else
		{
			// End:0xA6
			if(__NFUN_122__(Id, "TabCtrl2"))
			{
				// End:0xA6
				if(__NFUN_242__(block, false))
				{
					block = true;
					RequestRaidRecord();
				}
			}
		}
	}
	// End:0xEB
	if(__NFUN_122__(Id, "TabCtrl3"))
	{
		LoadAreaInfoList();
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("GuideWnd.AreaInfoComboBox", 0);
	}
	// End:0x13C
	if(__NFUN_122__(Id, "btn_search1"))
	{
		QuestComboCurrentData = m_hQuestComboBox.GetSelectedNum();
		QuestComboCurrentReservedData = m_hQuestComboBox.GetReserved(QuestComboCurrentData);
		LoadQuestSearchResult(QuestComboCurrentReservedData);
	}
	// End:0x1C9
	if(__NFUN_122__(Id, "btn_search2"))
	{
		HuntingZoneComboboxCurrentData = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("GuideWnd.HuntingZoneComboBox");
		HuntingZoneComboboxCurrentReservedData = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("GuideWnd.HuntingZoneComboBox", HuntingZoneComboboxCurrentData);
		LoadHuntingZoneListSearchResult(HuntingZoneComboboxCurrentReservedData);
	}
	// End:0x1EB
	if(__NFUN_122__(Id, "btn_search3"))
	{
		LoadRaidSearchResult(RaidCurrentComboboxCurrentReservedData);
	}
	// End:0x272
	if(__NFUN_122__(Id, "btn_search4"))
	{
		AreaInfoComboBoxCurrentData = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("GuideWnd.AreaInfoComboBox");
		AreaInfoComboBoxCurrentReservedData = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("GuideWnd.AreaInfoComboBox", AreaInfoComboBoxCurrentData);
		LoadAreaInfoListSearchResult(AreaInfoComboBoxCurrentReservedData);
	}
	// End:0x2C5
	if(__NFUN_122__(Id, "QuestComboBox"))
	{
		QuestComboCurrentData = m_hQuestComboBox.GetSelectedNum();
		QuestComboCurrentReservedData = m_hQuestComboBox.GetReserved(QuestComboCurrentData);
		LoadQuestSearchResult(QuestComboCurrentReservedData);
	}
	// End:0x35A
	if(__NFUN_122__(Id, "HuntingZoneComboBox"))
	{
		HuntingZoneComboboxCurrentData = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("GuideWnd.HuntingZoneComboBox");
		HuntingZoneComboboxCurrentReservedData = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("GuideWnd.HuntingZoneComboBox", HuntingZoneComboboxCurrentData);
		LoadHuntingZoneListSearchResult(HuntingZoneComboboxCurrentReservedData);
	}
	// End:0x395
	if(__NFUN_122__(Id, "CloseButton"))
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MinimapWnd.GuideWnd");
	}
	return;
}

function OnComboBoxItemSelected(string sName, int Index)
{
	local int QuestComboCurrentReservedData, HuntingZoneComboboxCurrentReservedData, AreaInfoComboBoxCurrentReservedData;

	// End:0x3E
	if(__NFUN_122__(sName, "QuestComboBox"))
	{
		QuestComboCurrentReservedData = m_hQuestComboBox.GetReserved(Index);
		LoadQuestSearchResult(QuestComboCurrentReservedData);
	}
	// End:0xA0
	if(__NFUN_122__(sName, "HuntingZoneComboBox"))
	{
		HuntingZoneComboboxCurrentReservedData = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("GuideWnd.HuntingZoneComboBox", Index);
		LoadHuntingZoneListSearchResult(HuntingZoneComboboxCurrentReservedData);
	}
	// End:0xFC
	if(__NFUN_122__(sName, "AreaInfoComboBox"))
	{
		AreaInfoComboBoxCurrentReservedData = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("GuideWnd.AreaInfoComboBox", Index);
		LoadAreaInfoListSearchResult(AreaInfoComboBoxCurrentReservedData);
	}
	return;
}

function OnClickListCtrlRecord(string Id)
{
	local LVDataRecord Record;
	local Vector Loc;

	// End:0xC9
	if(__NFUN_122__(Id, "QuestListCtrl"))
	{
		Record = m_hQuestListCtrl.GetSelectedRecord();
		Loc = Class'NWindow.UIDATA_QUEST'.static.GetStartNPCLoc(Record.LVDataList[0].nReserved1, 1);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd.Minimap", Loc, false);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.DeleteAllTarget("MinimapWnd.Minimap");
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AddTarget("MinimapWnd.Minimap", Loc);
	}
	// End:0x197
	if(__NFUN_122__(Id, "HuntingZoneListCtrl"))
	{
		Record = m_hHuntingZoneListCtrl.GetSelectedRecord();
		Loc = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneLoc(Record.LVDataList[0].nReserved1);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd.Minimap", Loc, false);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.DeleteAllTarget("MinimapWnd.Minimap");
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AddTarget("MinimapWnd.Minimap", Loc);
	}
	// End:0x25E
	if(__NFUN_122__(Id, "RaidListCtrl"))
	{
		Record = m_hRaidListCtrl.GetSelectedRecord();
		Loc = Class'NWindow.UIDATA_RAID'.static.GetRaidLoc(Record.LVDataList[0].nReserved1);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd.Minimap", Loc, false);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.DeleteAllTarget("MinimapWnd.Minimap");
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AddTarget("MinimapWnd.Minimap", Loc);
	}
	// End:0x329
	if(__NFUN_122__(Id, "AreaInfoListCtrl"))
	{
		Record = m_hAreaInfoListCtrl.GetSelectedRecord();
		Loc = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneLoc(Record.LVDataList[0].nReserved1);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AdjustMapView("MinimapWnd.Minimap", Loc, false);
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.DeleteAllTarget("MinimapWnd.Minimap");
		Class'NWindow.UIAPI_MINIMAPCTRL'.static.AddTarget("MinimapWnd.Minimap", Loc);
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1E
		case 130:
			m_hOwnerWnd.ShowWindow();
			// End:0x37
			break;
		// End:0x31
		case 120:
			LoadRaidList(a_Param);
			// End:0x37
			break;
		// End:0xFFFF
		default:
			// End:0x37
			break;
			break;
	}
	return;
}

function LoadHuntingZoneList()
{
	local string HuntingZoneName;
	local int MinLevel, MaxLevel;
	local string LevelLimit;
	local int FieldType;
	local string FieldType_Name;
	local int Zone;
	local LVDataRecord Record;
	local LVData data1, data2, data3, data4;
	local int i;

	m_hHuntingZoneListCtrl.DeleteAllItem();
	comboxFiller("HuntingZoneComboBox");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("GuideWnd.HuntingZoneComboBox", 0);
	i = 0;
	J0x5F:

	// End:0x263 [Loop If]
	if(__NFUN_150__(i, 500))
	{
		// End:0x259
		if(Class'NWindow.UIDATA_HUNTINGZONE'.static.IsValidData(i))
		{
			FieldType = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneType(i);
			// End:0x259
			if(__NFUN_132__(__NFUN_154__(FieldType, 1), __NFUN_154__(FieldType, 2)))
			{
				HuntingZoneName = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneName(i);
				MinLevel = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetMinLevel(i);
				MaxLevel = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetMaxLevel(i);
				Zone = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZone(i);
				// End:0x156
				if(__NFUN_130__(__NFUN_151__(MinLevel, 0), __NFUN_151__(MaxLevel, 0)))
				{
					LevelLimit = __NFUN_112__(__NFUN_112__(string(MinLevel), "~"), string(MaxLevel));					
				}
				else
				{
					// End:0x183
					if(__NFUN_151__(MinLevel, 0))
					{
						LevelLimit = __NFUN_112__(__NFUN_112__(string(MinLevel), " "), GetSystemString(859));						
					}
					else
					{
						LevelLimit = GetSystemString(866);
					}
				}
				FieldType_Name = conv_zoneType(FieldType);
				data1.nReserved1 = i;
				data1.szData = HuntingZoneName;
				Record.LVDataList[0] = data1;
				data2.szData = FieldType_Name;
				Record.LVDataList[1] = data2;
				data3.szData = conv_zoneName(Zone);
				Record.LVDataList[2] = data3;
				data4.szData = LevelLimit;
				Record.LVDataList[3] = data4;
				m_hHuntingZoneListCtrl.InsertRecord(Record);
			}
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x5F;
	}
	return;
}

function LoadHuntingZoneListSearchResult(int SearchZone)
{
	local string HuntingZoneName;
	local int MinLevel, MaxLevel;
	local string LevelLimit;
	local int FieldType;
	local string FieldType_Name;
	local int Zone;
	local string Description;
	local LVDataRecord Record;
	local LVData data1, data2, data3, data4;
	local int i;

	// End:0x18
	if(__NFUN_154__(SearchZone, 9999))
	{
		LoadHuntingZoneList();		
	}
	else
	{
		m_hHuntingZoneListCtrl.DeleteAllItem();
		i = 0;
		J0x2E:

		// End:0x26A [Loop If]
		if(__NFUN_150__(i, 500))
		{
			// End:0x260
			if(Class'NWindow.UIDATA_HUNTINGZONE'.static.IsValidData(i))
			{
				// End:0x260
				if(__NFUN_154__(Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZone(i), SearchZone))
				{
					FieldType = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneType(i);
					// End:0x260
					if(__NFUN_132__(__NFUN_154__(FieldType, 1), __NFUN_154__(FieldType, 2)))
					{
						HuntingZoneName = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneName(i);
						MinLevel = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetMinLevel(i);
						MaxLevel = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetMaxLevel(i);
						Zone = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZone(i);
						Description = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingDescription(i);
						// End:0x15D
						if(__NFUN_130__(__NFUN_151__(MinLevel, 0), __NFUN_151__(MaxLevel, 0)))
						{
							LevelLimit = __NFUN_112__(__NFUN_112__(string(MinLevel), "~"), string(MaxLevel));							
						}
						else
						{
							// End:0x18A
							if(__NFUN_151__(MinLevel, 0))
							{
								LevelLimit = __NFUN_112__(__NFUN_112__(string(MinLevel), " "), GetSystemString(859));								
							}
							else
							{
								LevelLimit = GetSystemString(866);
							}
						}
						FieldType_Name = conv_zoneType(FieldType);
						data1.nReserved1 = i;
						data1.szData = HuntingZoneName;
						Record.LVDataList[0] = data1;
						data2.szData = FieldType_Name;
						Record.LVDataList[1] = data2;
						data3.szData = conv_zoneName(Zone);
						Record.LVDataList[2] = data3;
						data4.szData = LevelLimit;
						Record.LVDataList[3] = data4;
						m_hHuntingZoneListCtrl.InsertRecord(Record);
					}
				}
			}
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x2E;
		}
	}
	return;
}

function LoadRaidRanking()
{
	return;
}

function LoadRaidList(string a_Param)
{
	local int i, j, RaidMonsterID, RaidMonsterLevel, RaidMonsterZone;

	local string RaidPointStr, RaidMonsterPrefferedLevel, RaidMonsterName, RaidDescription;
	local LVDataRecord Record;
	local LVData data1, data2, data3, data4;
	local int RaidRanking, RaidSeasonPoint, RaidNum, ClearRaidMonsterID, ClearSeasonPoint, ClearTotalPoint,
		AntarasPoint;

	local string SeasonTotalString;

	m_hRaidListCtrl.DeleteAllItem();
	AntarasPoint = 0;
	ParseInt(a_Param, "RaidRank", RaidRanking);
	ParseInt(a_Param, "SeasonPoint", RaidSeasonPoint);
	// End:0x87
	if(__NFUN_154__(RaidRanking, 0))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GuideWnd.Ranking", GetSystemString(1454));		
	}
	else
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("GuideWnd.Ranking", RaidRanking);
	}
	SeasonTotalString = __NFUN_168__(__NFUN_112__("", string(RaidSeasonPoint)), GetSystemString(1442));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GuideWnd.SeasonTotalPoint", SeasonTotalString);
	ParseInt(a_Param, "Count", RaidNum);
	RaidRecordList.Remove(0, RaidRecordList.Length);
	RaidRecordList.Insert(0, RaidNum);
	i = 0;
	J0x131:

	// End:0x1FC [Loop If]
	if(__NFUN_150__(i, RaidNum))
	{
		ParseInt(a_Param, __NFUN_112__("MonsterID", string(i)), ClearRaidMonsterID);
		ParseInt(a_Param, __NFUN_112__("CurrentPoint", string(i)), ClearSeasonPoint);
		ParseInt(a_Param, __NFUN_112__("TotalPoint", string(i)), ClearTotalPoint);
		RaidRecordList[i].A = ClearRaidMonsterID;
		RaidRecordList[i].B = ClearSeasonPoint;
		RaidRecordList[i].C = ClearTotalPoint;
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x131;
	}
	i = 0;
	J0x203:

	// End:0x5F9 [Loop If]
	if(__NFUN_150__(i, 2000))
	{
		// End:0x5EF
		if(Class'NWindow.UIDATA_RAID'.static.IsValidData(i))
		{
			RaidMonsterID = Class'NWindow.UIDATA_RAID'.static.GetRaidMonsterID(i);
			RaidMonsterLevel = Class'NWindow.UIDATA_RAID'.static.GetRaidMonsterLevel(i);
			RaidMonsterZone = Class'NWindow.UIDATA_RAID'.static.GetRaidMonsterZone(i);
			RaidDescription = Class'NWindow.UIDATA_RAID'.static.GetRaidDescription(i);
			RaidMonsterName = Class'NWindow.UIDATA_NPC'.static.GetNPCName(RaidMonsterID);
			// End:0x2CF
			if(__NFUN_154__(RaidMonsterID, 29066))
			{
				Debug("¾ÈÅ¸¶ó½º 1");
				// [Explicit Continue]
				goto J0x5EF;
			}
			// End:0x2F3
			if(__NFUN_154__(RaidMonsterID, 29067))
			{
				Debug("¾ÈÅ¸¶ó½º 2");
				// [Explicit Continue]
				goto J0x5EF;
			}
			// End:0x48E
			if(__NFUN_154__(RaidMonsterID, 29068))
			{
				Debug("¾ÈÅ¸¶ó½º 3");
				// End:0x341
				if(__NFUN_151__(RaidMonsterLevel, 0))
				{
					RaidMonsterPrefferedLevel = __NFUN_112__(__NFUN_112__(GetSystemString(537), " "), string(RaidMonsterLevel));					
				}
				else
				{
					RaidMonsterPrefferedLevel = GetSystemString(1415);
				}
				data1.nReserved1 = i;
				data1.szData = RaidMonsterName;
				Record.LVDataList[0] = data1;
				data2.szData = RaidMonsterPrefferedLevel;
				Record.LVDataList[1] = data2;
				data3.szData = conv_zoneName(RaidMonsterZone);
				Record.LVDataList[2] = data3;
				RaidPointStr = "0";
				j = 0;
				J0x3DF:

				// End:0x42E [Loop If]
				if(__NFUN_150__(j, RaidNum))
				{
					// End:0x424
					if(__NFUN_154__(RaidRecordList[j].A, RaidMonsterID))
					{
						RaidPointStr = __NFUN_112__(string(RaidRecordList[j].B), "");
					}
					__NFUN_165__(j);
					// [Loop Continue]
					goto J0x3DF;
				}
				AntarasPoint = __NFUN_146__(int(RaidPointStr), AntarasPoint);
				data4.szData = string(AntarasPoint);
				Record.LVDataList[3] = data4;
				Record.szReserved = RaidDescription;
				m_hRaidListCtrl.InsertRecord(Record);
				// [Explicit Continue]
				goto J0x5EF;
			}
			// End:0x4BB
			if(__NFUN_151__(RaidMonsterLevel, 0))
			{
				RaidMonsterPrefferedLevel = __NFUN_112__(__NFUN_112__(GetSystemString(537), " "), string(RaidMonsterLevel));				
			}
			else
			{
				RaidMonsterPrefferedLevel = GetSystemString(1415);
			}
			data1.nReserved1 = i;
			data1.szData = RaidMonsterName;
			Record.LVDataList[0] = data1;
			data2.szData = RaidMonsterPrefferedLevel;
			Record.LVDataList[1] = data2;
			data3.szData = conv_zoneName(RaidMonsterZone);
			Record.LVDataList[2] = data3;
			RaidPointStr = "0";
			j = 0;
			J0x559:

			// End:0x5A8 [Loop If]
			if(__NFUN_150__(j, RaidNum))
			{
				// End:0x59E
				if(__NFUN_154__(RaidRecordList[j].A, RaidMonsterID))
				{
					RaidPointStr = __NFUN_112__(string(RaidRecordList[j].B), "");
				}
				__NFUN_165__(j);
				// [Loop Continue]
				goto J0x559;
			}
			data4.szData = RaidPointStr;
			Record.LVDataList[3] = data4;
			Record.szReserved = RaidDescription;
			m_hRaidListCtrl.InsertRecord(Record);
		}
		J0x5EF:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x203;
	}
	return;
}

function LoadRaidList2()
{
	local int i, RaidMonsterID, RaidMonsterLevel, RaidMonsterZone;
	local string RaidPointStr, RaidMonsterPrefferedLevel, RaidMonsterName, RaidDescription;
	local LVDataRecord Record;
	local LVData data1, data2, data3, data4;
	local int RaidNum, RaidCount;

	m_hRaidListCtrl.DeleteAllItem();
	i = 0;
	J0x16:

	// End:0x21A [Loop If]
	if(__NFUN_150__(i, 2000))
	{
		// End:0x210
		if(Class'NWindow.UIDATA_RAID'.static.IsValidData(i))
		{
			RaidMonsterID = Class'NWindow.UIDATA_RAID'.static.GetRaidMonsterID(i);
			RaidMonsterLevel = Class'NWindow.UIDATA_RAID'.static.GetRaidMonsterLevel(i);
			RaidMonsterZone = Class'NWindow.UIDATA_RAID'.static.GetRaidMonsterZone(i);
			RaidDescription = Class'NWindow.UIDATA_RAID'.static.GetRaidDescription(i);
			RaidMonsterName = Class'NWindow.UIDATA_NPC'.static.GetNPCName(RaidMonsterID);
			// End:0xEB
			if(__NFUN_151__(RaidMonsterLevel, 0))
			{
				RaidMonsterPrefferedLevel = __NFUN_112__(__NFUN_112__(GetSystemString(537), " "), string(RaidMonsterLevel));				
			}
			else
			{
				RaidMonsterPrefferedLevel = GetSystemString(1415);
			}
			data1.nReserved1 = i;
			data1.szData = RaidMonsterName;
			Record.LVDataList[0] = data1;
			data2.szData = RaidMonsterPrefferedLevel;
			Record.LVDataList[1] = data2;
			data3.szData = conv_zoneName(RaidMonsterZone);
			Record.LVDataList[2] = data3;
			RaidPointStr = "0";
			// End:0x1C9
			if(__NFUN_150__(RaidCount, RaidNum))
			{
				// End:0x1C9
				if(__NFUN_154__(RaidRecordList[RaidCount].A, RaidMonsterID))
				{
					RaidPointStr = __NFUN_112__(string(RaidRecordList[__NFUN_165__(RaidCount)].B), "");
				}
			}
			data4.szData = RaidPointStr;
			Record.LVDataList[3] = data4;
			Record.szReserved = RaidDescription;
			m_hRaidListCtrl.InsertRecord(Record);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x16;
	}
	return;
}

function LoadRaidSearchResult(int SearchZone)
{
	local int i, RaidMonsterID, RaidMonsterLevel, RaidMonsterZone;
	local string RaidPointStr, RaidMonsterPrefferedLevel, RaidMonsterName, RaidDescription;
	local LVDataRecord Record;
	local LVData data1, data2, data3, data4;
	local int RaidNum, RaidCount;

	// End:0x18
	if(__NFUN_154__(SearchZone, 9999))
	{
		LoadRaidList2();		
	}
	else
	{
		m_hRaidListCtrl.DeleteAllItem();
		RaidNum = RaidRecordList.Length;
		RaidCount = 0;
		i = 0;
		J0x41:

		// End:0x25C [Loop If]
		if(__NFUN_150__(i, 2000))
		{
			// End:0x252
			if(Class'NWindow.UIDATA_RAID'.static.IsValidData(i))
			{
				RaidMonsterZone = Class'NWindow.UIDATA_RAID'.static.GetRaidMonsterZone(i);
				// End:0x252
				if(__NFUN_154__(SearchZone, RaidMonsterZone))
				{
					RaidMonsterID = Class'NWindow.UIDATA_RAID'.static.GetRaidMonsterID(i);
					RaidMonsterLevel = Class'NWindow.UIDATA_RAID'.static.GetRaidMonsterLevel(i);
					RaidDescription = Class'NWindow.UIDATA_RAID'.static.GetRaidDescription(i);
					RaidMonsterName = Class'NWindow.UIDATA_NPC'.static.GetNPCName(RaidMonsterID);
					// End:0x125
					if(__NFUN_151__(RaidMonsterLevel, 0))
					{
						RaidMonsterPrefferedLevel = __NFUN_112__(__NFUN_112__(GetSystemString(537), " "), string(RaidMonsterLevel));						
					}
					else
					{
						RaidMonsterPrefferedLevel = "-";
					}
					data1.nReserved1 = i;
					data1.szData = RaidMonsterName;
					Record.LVDataList[0] = data1;
					data2.szData = RaidMonsterPrefferedLevel;
					Record.LVDataList[1] = data2;
					data3.szData = conv_zoneName(RaidMonsterZone);
					Record.LVDataList[2] = data3;
					RaidPointStr = "0";
					// End:0x1FB
					if(__NFUN_150__(RaidCount, RaidNum))
					{
						// End:0x1FB
						if(__NFUN_154__(RaidRecordList[RaidCount].A, RaidMonsterID))
						{
							RaidPointStr = __NFUN_112__(string(RaidRecordList[__NFUN_165__(RaidCount)].B), "");
						}
					}
					data4.szData = RaidPointStr;
					Record.LVDataList[3] = data4;
					Record.szReserved = RaidDescription;
					Record.nReserved1 = RaidMonsterZone;
					m_hRaidListCtrl.InsertRecord(Record);
				}
			}
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x41;
		}
	}
	return;
}

function LoadQuestList()
{
	local string QuestName;
	local int MinLevel, MaxLevel, Type, Npc;
	local string LevelLimit, NpcName;
	local int Id;
	local LVDataRecord Record;
	local LVData data1, data2, data3, data4;
	local string TypeText[5];

	m_hQuestListCtrl.DeleteAllItem();
	comboxFiller("QuestComboBox");
	m_hQuestComboBox.SetSelectedNum(0);
	TypeText[0] = GetSystemString(861);
	TypeText[1] = GetSystemString(862);
	TypeText[2] = GetSystemString(861);
	TypeText[3] = GetSystemString(862);
	TypeText[4] = GetSystemString(861);
	Id = Class'NWindow.UIDATA_QUEST'.static.GetFirstID();
	J0xAB:

	// End:0x2B3 [Loop If]
	if(__NFUN_155__(-1, Id))
	{
		QuestName = Class'NWindow.UIDATA_QUEST'.static.GetQuestName(Id);
		MinLevel = Class'NWindow.UIDATA_QUEST'.static.GetMinLevel(Id, 1);
		MaxLevel = Class'NWindow.UIDATA_QUEST'.static.GetMaxLevel(Id, 1);
		Type = Class'NWindow.UIDATA_QUEST'.static.GetQuestType(Id, 1);
		Npc = Class'NWindow.UIDATA_QUEST'.static.GetStartNPCID(Id, 1);
		NpcName = Class'NWindow.UIDATA_NPC'.static.GetNPCName(Npc);
		// End:0x190
		if(__NFUN_130__(__NFUN_151__(MinLevel, 0), __NFUN_151__(MaxLevel, 0)))
		{
			LevelLimit = __NFUN_112__(__NFUN_112__(string(MinLevel), "~"), string(MaxLevel));			
		}
		else
		{
			// End:0x1BD
			if(__NFUN_151__(MinLevel, 0))
			{
				LevelLimit = __NFUN_112__(__NFUN_112__(string(MinLevel), " "), GetSystemString(859));				
			}
			else
			{
				LevelLimit = GetSystemString(866);
			}
		}
		data1.nReserved1 = Id;
		data1.szData = QuestName;
		Record.LVDataList[0] = data1;
		// End:0x22F
		if(__NFUN_130__(__NFUN_152__(0, Type), __NFUN_152__(Type, 4)))
		{
			data2.szData = TypeText[Type];
		}
		Record.LVDataList[1] = data2;
		data3.szData = NpcName;
		Record.LVDataList[2] = data3;
		data4.szData = LevelLimit;
		Record.LVDataList[3] = data4;
		m_hQuestListCtrl.InsertRecord(Record);
		Id = Class'NWindow.UIDATA_QUEST'.static.GetNextID();
		// [Loop Continue]
		goto J0xAB;
	}
	return;
}

function LoadQuestSearchResult(int SearchZone)
{
	local string QuestName, Condition;
	local int MinLevel, MaxLevel, Type, Zone, Npc;

	local string Description, LevelLimit, NpcName;
	local int i;
	local LVDataRecord Record;
	local LVData data1, data2, data3, data4;
	local string TypeText;

	// End:0x18
	if(__NFUN_154__(SearchZone, 9999))
	{
		LoadQuestList();		
	}
	else
	{
		m_hQuestListCtrl.DeleteAllItem();
		i = 0;
		J0x2E:

		// End:0x315 [Loop If]
		if(__NFUN_150__(i, 2000))
		{
			// End:0x30B
			if(Class'NWindow.UIDATA_QUEST'.static.IsValidData(i))
			{
				// End:0x30B
				if(__NFUN_154__(Class'NWindow.UIDATA_QUEST'.static.GetQuestZone(i, 1), SearchZone))
				{
					QuestName = Class'NWindow.UIDATA_QUEST'.static.GetQuestName(i);
					Condition = Class'NWindow.UIDATA_QUEST'.static.GetRequirement(i, 1);
					MinLevel = Class'NWindow.UIDATA_QUEST'.static.GetMinLevel(i, 1);
					MaxLevel = Class'NWindow.UIDATA_QUEST'.static.GetMaxLevel(i, 1);
					Type = Class'NWindow.UIDATA_QUEST'.static.GetQuestType(i, 1);
					Zone = Class'NWindow.UIDATA_QUEST'.static.GetQuestZone(i, 1);
					Description = Class'NWindow.UIDATA_QUEST'.static.GetIntro(i, 1);
					Npc = Class'NWindow.UIDATA_QUEST'.static.GetStartNPCID(i, 1);
					NpcName = Class'NWindow.UIDATA_NPC'.static.GetNPCName(Npc);
					// End:0x19A
					if(__NFUN_130__(__NFUN_151__(MinLevel, 0), __NFUN_151__(MaxLevel, 0)))
					{
						LevelLimit = __NFUN_112__(__NFUN_112__(string(MinLevel), "~"), string(MaxLevel));						
					}
					else
					{
						// End:0x1C7
						if(__NFUN_151__(MinLevel, 0))
						{
							LevelLimit = __NFUN_112__(__NFUN_112__(string(MinLevel), " "), GetSystemString(859));							
						}
						else
						{
							LevelLimit = GetSystemString(866);
						}
					}
					switch(Type)
					{
						// End:0x1F7
						case 0:
							TypeText = GetSystemString(861);
							// End:0x25D
							break;
						// End:0x20F
						case 1:
							TypeText = GetSystemString(862);
							// End:0x25D
							break;
						// End:0x228
						case 2:
							TypeText = GetSystemString(861);
							// End:0x25D
							break;
						// End:0x241
						case 3:
							TypeText = GetSystemString(862);
							// End:0x25D
							break;
						// End:0x25A
						case 4:
							TypeText = GetSystemString(861);
							// End:0x25D
							break;
						// End:0xFFFF
						default:
							break;
					}
					data1.nReserved1 = i;
					data1.szData = QuestName;
					Record.LVDataList[0] = data1;
					data2.szData = TypeText;
					Record.LVDataList[1] = data2;
					data3.szData = NpcName;
					Record.LVDataList[2] = data3;
					data4.szData = LevelLimit;
					Record.LVDataList[3] = data4;
					m_hQuestListCtrl.InsertRecord(Record);
				}
			}
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x2E;
		}
	}
	return;
}

function LoadAreaInfoList()
{
	local string AreaName;
	local int Type, Zone;
	local string ZoneName, ZoneType, Description;
	local int i;
	local LVDataRecord Record;
	local LVData data1, data2, data3;

	m_hAreaInfoListCtrl.DeleteAllItem();
	comboxFiller("AreaInfoComboBox");
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("GuideWnd.AreaInfoComboBox", 0);
	i = 0;
	J0x59:

	// End:0x1CA [Loop If]
	if(__NFUN_150__(i, 500))
	{
		// End:0x1C0
		if(Class'NWindow.UIDATA_HUNTINGZONE'.static.IsValidData(i))
		{
			Type = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneType(i);
			// End:0x1C0
			if(__NFUN_151__(Type, 2))
			{
				AreaName = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneName(i);
				Zone = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZone(i);
				Description = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingDescription(i);
				ZoneName = conv_dom(Zone);
				ZoneType = conv_zoneType(Type);
				data1.nReserved1 = i;
				data1.szData = AreaName;
				Record.LVDataList[0] = data1;
				data2.szData = ZoneType;
				Record.LVDataList[1] = data2;
				data3.szData = ZoneName;
				Record.LVDataList[2] = data3;
				Record.nReserved1 = Zone;
				Record.szReserved = Description;
				m_hAreaInfoListCtrl.InsertRecord(Record);
			}
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x59;
	}
	return;
}

function LoadAreaInfoListSearchResult(int SearchZone)
{
	local string AreaName;
	local int Type, Zone;
	local string ZoneName, ZoneType, Description;
	local int i;
	local LVDataRecord Record;
	local LVData data1, data2, data3;

	// End:0x18
	if(__NFUN_154__(SearchZone, 9999))
	{
		LoadAreaInfoList();		
	}
	else
	{
		m_hAreaInfoListCtrl.DeleteAllItem();
		i = 0;
		J0x2E:

		// End:0x19D [Loop If]
		if(__NFUN_150__(i, 2000))
		{
			// End:0x193
			if(Class'NWindow.UIDATA_HUNTINGZONE'.static.IsValidData(i))
			{
				// End:0x193
				if(__NFUN_154__(Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZone(i), SearchZone))
				{
					Type = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneType(i);
					// End:0x193
					if(__NFUN_151__(Type, 2))
					{
						AreaName = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneName(i);
						Zone = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZone(i);
						Description = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingDescription(i);
						ZoneName = conv_dom(Zone);
						ZoneType = conv_zoneType(Type);
						data1.nReserved1 = i;
						data1.szData = AreaName;
						Record.LVDataList[0] = data1;
						data2.szData = ZoneType;
						Record.LVDataList[1] = data2;
						data3.szData = ZoneName;
						Record.LVDataList[2] = data3;
						m_hAreaInfoListCtrl.InsertRecord(Record);
					}
				}
			}
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x2E;
		}
	}
	return;
}

function comboxFiller(string ComboboxName)
{
	switch(ComboboxName)
	{
		// End:0x31
		case "QuestComboBox":
			proc_combox("QuestComboBox");
			// End:0x9A
			break;
		// End:0x67
		case "HuntingZoneComboBox":
			proc_combox("HuntingZoneComboBox");
			// End:0x9A
			break;
		// End:0x97
		case "AreaInfoComboBox":
			proc_combox("AreaInfoComboBox");
			// End:0x9A
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function proc_combox(string ComboboxName)
{
	local string ComboboxNameFull, ZoneName;
	local int Zone, i;

	ComboboxNameFull = __NFUN_112__("GuideWnd.", ComboboxName);
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear(ComboboxNameFull);
	Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved(ComboboxNameFull, GetSystemString(144), 9999);
	i = 0;
	J0x54:

	// End:0xF0 [Loop If]
	if(__NFUN_150__(i, 500))
	{
		// End:0xE6
		if(Class'NWindow.UIDATA_HUNTINGZONE'.static.IsValidData(i))
		{
			// End:0xE6
			if(__NFUN_154__(Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneType(i), 0))
			{
				ZoneName = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneName(i);
				Zone = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZone(i);
				Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved(ComboboxNameFull, ZoneName, Zone);
			}
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x54;
	}
	return;
}

function string conv_dom(int ZoneNameNum)
{
	local string ZoneNameStr;
	local int i;

	i = 0;
	J0x07:

	// End:0x89 [Loop If]
	if(__NFUN_150__(i, 500))
	{
		// End:0x7F
		if(Class'NWindow.UIDATA_HUNTINGZONE'.static.IsValidData(i))
		{
			// End:0x7F
			if(__NFUN_154__(Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneType(i), 0))
			{
				// End:0x7F
				if(__NFUN_154__(Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZone(i), ZoneNameNum))
				{
					ZoneNameStr = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneName(i);
				}
			}
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return ZoneNameStr;
	return;
}

function string conv_zoneType(int ZoneTypeNum)
{
	local string ZoneTypeStr;

	switch(ZoneTypeNum)
	{
		// End:0x1F
		case 1:
			ZoneTypeStr = GetSystemString(1313);
			// End:0xB8
			break;
		// End:0x38
		case 2:
			ZoneTypeStr = GetSystemString(1314);
			// End:0xB8
			break;
		// End:0x51
		case 3:
			ZoneTypeStr = GetSystemString(1315);
			// End:0xB8
			break;
		// End:0x6A
		case 4:
			ZoneTypeStr = GetSystemString(1316);
			// End:0xB8
			break;
		// End:0x83
		case 5:
			ZoneTypeStr = GetSystemString(1317);
			// End:0xB8
			break;
		// End:0x9C
		case 6:
			ZoneTypeStr = GetSystemString(1318);
			// End:0xB8
			break;
		// End:0xB5
		case 7:
			ZoneTypeStr = GetSystemString(1319);
			// End:0xB8
			break;
		// End:0xFFFF
		default:
			break;
	}
	return ZoneTypeStr;
	return;
}

function string conv_zoneName(int search_zoneid)
{
	local string HuntingZoneName;
	local int i;

	i = 0;
	J0x07:

	// End:0x89 [Loop If]
	if(__NFUN_150__(i, 500))
	{
		// End:0x7F
		if(Class'NWindow.UIDATA_HUNTINGZONE'.static.IsValidData(i))
		{
			// End:0x7F
			if(__NFUN_154__(Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneType(i), 0))
			{
				// End:0x7F
				if(__NFUN_154__(Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZone(i), search_zoneid))
				{
					HuntingZoneName = Class'NWindow.UIDATA_HUNTINGZONE'.static.GetHuntingZoneName(i);
				}
			}
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return HuntingZoneName;
	return;
}
