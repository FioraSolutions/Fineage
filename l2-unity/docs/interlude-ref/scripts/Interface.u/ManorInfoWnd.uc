class ManorInfoWnd extends UICommonAPI;

const SEED_NAME = 0;
const SEED_REMAIN_CNT = 1;
const SEED_TOTLAL_CNT = 2;
const SEED_PRICE = 3;
const SEED_LEVEL = 4;
const SEED_REWARD_TYPE_1 = 5;
const SEED_REWARD_TYPE_2 = 6;
const SEED_MAX_COLUMN = 7;
const CROP_NAME = 0;
const CROP_REMAIN_CNT = 1;
const CROP_TOTLAL_CNT = 2;
const CROP_PRICE = 3;
const CROP_PROCURE_TYPE = 4;
const CROP_LEVEL = 5;
const CROP_REWARD_TYPE_1 = 6;
const CROP_REWARD_TYPE_2 = 7;
const CROP_MAX_COLUMN = 8;
const DEFAULT_CROP_NAME = 0;
const DEFAULT_CROP_LEVEL = 1;
const DEFAULT_SEED_PRICE = 2;
const DEFAULT_CROP_PRICE = 3;
const DEFAULT_REWARD_TYPE_1 = 4;
const DEFAULT_REWARD_TYPE_2 = 5;
const DEFAULT_MAX_COLUMN = 6;

var bool m_bTime;
var int m_MerchantOrChamberlain;
var int m_ManorID;
var int m_MyManorID;

function OnLoad()
{
	RegisterEvent(2650);
	RegisterEvent(2652);
	RegisterEvent(2654);
	RegisterEvent(2651);
	RegisterEvent(2653);
	RegisterEvent(2655);
	m_MerchantOrChamberlain = 0;
	m_ManorID = -1;
	m_MyManorID = -1;
	m_bTime = false;
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 2650:
			HandleSeedInfoShow(a_Param);
			// End:0x8E
			break;
		// End:0x33
		case 2651:
			HandleSeedAdd(a_Param);
			// End:0x8E
			break;
		// End:0x49
		case 2652:
			HandleCropInfoShow(a_Param);
			// End:0x8E
			break;
		// End:0x5F
		case 2653:
			HandleCropAdd(a_Param);
			// End:0x8E
			break;
		// End:0x75
		case 2654:
			HandleDefaultInfoShow(a_Param);
			// End:0x8E
			break;
		// End:0x8B
		case 2655:
			HandleDefaultAdd(a_Param);
			// End:0x8E
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleSeedInfoShow(string a_Param)
{
	local int MerchantOrChamberlain, NumOfManor, i, ManorID;
	local string ManorName, ParamString, Message;
	local int ManorCnt;
	local string MyManorName;

	ParseInt(a_Param, "MerchantOrChamberlain", MerchantOrChamberlain);
	ParseInt(a_Param, "ManorID", ManorID);
	ParseString(a_Param, "ManorName", ManorName);
	m_MerchantOrChamberlain = MerchantOrChamberlain;
	m_ManorID = ManorID;
	Debug(__NFUN_112__("HandleSeedInfoShow - m_ManorID:", string(m_ManorID)));
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ManorInfoWnd.SeedInfoWnd.SeedInfoListCtrl");
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("ManorInfoWnd.SeedInfoWnd.cbManorSelectInSeedInfo");
	i = 0;
	J0x123:

	// End:0x19F [Loop If]
	if(__NFUN_150__(i, GetManorCount()))
	{
		ManorID = GetManorIDInManorList(i);
		Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("ManorInfoWnd.SeedInfoWnd.cbManorSelectInSeedInfo", GetManorNameInManorList(i), ManorID);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x123;
	}
	NumOfManor = Class'NWindow.UIAPI_COMBOBOX'.static.GetNumOfItems("ManorInfoWnd.SeedInfoWnd.cbManorSelectInSeedInfo");
	i = 0;
	J0x1ED:

	// End:0x29F [Loop If]
	if(__NFUN_150__(i, NumOfManor))
	{
		// End:0x295
		if(__NFUN_154__(m_ManorID, Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("ManorInfoWnd.SeedInfoWnd.cbManorSelectInSeedInfo", i)))
		{
			Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("ManorInfoWnd.SeedInfoWnd.cbManorSelectInSeedInfo", i);
			// [Explicit Break]
			goto J0x29F;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1ED;
	}
	J0x29F:

	// End:0x2C3
	if(__NFUN_129__(IsShowWindow("ManorInfoWnd")))
	{
		m_MyManorID = m_ManorID;
	}
	// End:0x2FC
	if(__NFUN_154__(MerchantOrChamberlain, 1))
	{
		HideWindow("ManorInfoWnd.SeedInfoWnd.btnBuySeed");		
	}
	else
	{
		ShowWindow("ManorInfoWnd.SeedInfoWnd.btnBuySeed");
		ManorCnt = GetManorCount();
		i = 0;
		J0x33A:

		// End:0x37C [Loop If]
		if(__NFUN_150__(i, ManorCnt))
		{
			// End:0x372
			if(__NFUN_154__(m_MyManorID, GetManorIDInManorList(i)))
			{
				MyManorName = GetManorNameInManorList(i);
				// [Explicit Break]
				goto J0x37C;
			}
			__NFUN_163__(i);
			// [Loop Continue]
			goto J0x33A;
		}
		J0x37C:

		ParamAdd(ParamString, "Type", string(int(0)));
		ParamAdd(ParamString, "param1", MyManorName);
		AddSystemMessageParam(ParamString);
		Message = EndSystemMessageParam(1605, true);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorInfoWnd.SeedInfoWnd.txtNotice", Message);
	}
	ShowWindowWithFocus("ManorInfoWnd");
	HideWindow("ManorInfoWnd.CropInfoWnd");
	HideWindow("ManorInfoWnd.DefaultInfoWnd");
	Class'NWindow.UIAPI_TABCTRL'.static.SetTopOrder("ManorInfoWnd.ManorInfoTabCtrl", 0, false);
	// End:0x56D
	if(__NFUN_129__(IsShowWindow("ManorInfoWnd.SeedInfoWnd")))
	{
		Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("ManorInfoWnd.CropInfoWnd.cbManorSelectInCropInfo", -1);
		ShowWindowWithFocus("ManorInfoWnd.SeedInfoWnd");
		Debug(__NFUN_112__(__NFUN_112__(__NFUN_112__("HandleSeedInfoShow  !IsShowWindow ¾È - m_ManorID:", string(m_ManorID)), "m_MyManorID:"), string(m_MyManorID)));
	}
	return;
}

function HandleSeedAdd(string a_Param)
{
	local int SeedID;
	local string SeedName;
	local int SeedRemainCnt, SeedTotalCnt, SeedPrice, SeedLevel;
	local string RewardType1, RewardType2;
	local LVDataRecord Record;

	ParseInt(a_Param, "SeedID", SeedID);
	ParseString(a_Param, "SeedName", SeedName);
	ParseInt(a_Param, "SeedRemainCnt", SeedRemainCnt);
	ParseInt(a_Param, "SeedTotalCnt", SeedTotalCnt);
	ParseInt(a_Param, "SeedPrice", SeedPrice);
	ParseInt(a_Param, "SeedLevel", SeedLevel);
	ParseString(a_Param, "RewardType1", RewardType1);
	ParseString(a_Param, "RewardType2", RewardType2);
	Record.LVDataList.Length = 7;
	Record.LVDataList[0].szData = SeedName;
	Record.LVDataList[1].szData = string(SeedRemainCnt);
	Record.LVDataList[2].szData = string(SeedTotalCnt);
	Record.LVDataList[3].szData = string(SeedPrice);
	Record.LVDataList[4].szData = string(SeedLevel);
	Record.LVDataList[5].szData = RewardType1;
	Record.LVDataList[6].szData = RewardType2;
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ManorInfoWnd.SeedInfoWnd.SeedInfoListCtrl", Record);
	return;
}

function HandleCropInfoShow(string a_Param)
{
	local int MerchantOrChamberlain, NumOfManor, i, ManorID;

	ParseInt(a_Param, "MerchantOrChamberlain", MerchantOrChamberlain);
	ParseInt(a_Param, "ManorID", ManorID);
	m_MerchantOrChamberlain = MerchantOrChamberlain;
	m_ManorID = ManorID;
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ManorInfoWnd.CropInfoWnd.CropInfoListCtrl");
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("ManorInfoWnd.CropInfoWnd.cbManorSelectInCropInfo");
	i = 0;
	J0xD8:

	// End:0x154 [Loop If]
	if(__NFUN_150__(i, GetManorCount()))
	{
		ManorID = GetManorIDInManorList(i);
		Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("ManorInfoWnd.CropInfoWnd.cbManorSelectInCropInfo", GetManorNameInManorList(i), ManorID);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xD8;
	}
	NumOfManor = Class'NWindow.UIAPI_COMBOBOX'.static.GetNumOfItems("ManorInfoWnd.CropInfoWnd.cbManorSelectInCropInfo");
	i = 0;
	J0x1A2:

	// End:0x254 [Loop If]
	if(__NFUN_150__(i, NumOfManor))
	{
		// End:0x24A
		if(__NFUN_154__(m_ManorID, Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("ManorInfoWnd.CropInfoWnd.cbManorSelectInCropInfo", i)))
		{
			Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("ManorInfoWnd.CropInfoWnd.cbManorSelectInCropInfo", i);
			// [Explicit Break]
			goto J0x254;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1A2;
	}
	J0x254:

	// End:0x28E
	if(__NFUN_154__(MerchantOrChamberlain, 1))
	{
		HideWindow("ManorInfoWnd.CropInfoWnd.btnSellCrop");		
	}
	else
	{
		ShowWindow("ManorInfoWnd.CropInfoWnd.btnSellCrop");
	}
	// End:0x36F
	if(__NFUN_129__(IsShowWindow("ManorInfoWnd.CropInfoWnd")))
	{
		ShowWindowWithFocus("ManorInfoWnd.CropInfoWnd");
		// End:0x32C
		if(__NFUN_129__(IsShowWindow("ManorInfoWnd")))
		{
			ShowWindow("ManorInfoWnd");
		}
		HideWindow("ManorInfoWnd.SeedInfoWnd");
		HideWindow("ManorInfoWnd.DefaultInfoWnd");
	}
	return;
}

function HandleCropAdd(string a_Param)
{
	local int CropID;
	local string CropName;
	local int CropRemainCnt, CropTotalCnt, CropPrice, ProcureType, CropLevel;

	local string RewardType1, RewardType2;
	local LVDataRecord Record;

	ParseInt(a_Param, "CropID", CropID);
	ParseString(a_Param, "CropName", CropName);
	ParseInt(a_Param, "CropRemainCnt", CropRemainCnt);
	ParseInt(a_Param, "CropTotalCnt", CropTotalCnt);
	ParseInt(a_Param, "CropPrice", CropPrice);
	ParseInt(a_Param, "ProcureType", ProcureType);
	ParseInt(a_Param, "CropLevel", CropLevel);
	ParseString(a_Param, "RewardType1", RewardType1);
	ParseString(a_Param, "RewardType2", RewardType2);
	Record.LVDataList.Length = 8;
	Record.LVDataList[0].szData = CropName;
	Record.LVDataList[1].szData = string(CropRemainCnt);
	Record.LVDataList[2].szData = string(CropTotalCnt);
	Record.LVDataList[3].szData = string(CropPrice);
	Record.LVDataList[4].szData = string(ProcureType);
	Record.LVDataList[5].szData = string(CropLevel);
	Record.LVDataList[6].szData = RewardType1;
	Record.LVDataList[7].szData = RewardType2;
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ManorInfoWnd.CropInfoWnd.CropInfoListCtrl", Record);
	return;
}

function HandleDefaultInfoShow(string a_Param)
{
	local int MerchantOrChamberlain;

	ParseInt(a_Param, "MerchantOrChamberlain", MerchantOrChamberlain);
	m_MerchantOrChamberlain = MerchantOrChamberlain;
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ManorInfoWnd.DefaultInfoWnd.DefaultInfoListCtrl");
	// End:0xDD
	if(__NFUN_154__(MerchantOrChamberlain, 1))
	{
		HideWindow("ManorInfoWnd.DefaultInfoWnd.btnSellCrop");
		HideWindow("ManorInfoWnd.DefaultInfoWnd.btnBuySeed");		
	}
	else
	{
		ShowWindow("ManorInfoWnd.DefaultInfoWnd.btnSellCrop");
		ShowWindow("ManorInfoWnd.DefaultInfoWnd.btnBuySeed");
	}
	ShowWindowWithFocus("ManorInfoWnd");
	ShowWindowWithFocus("ManorInfoWnd.DefaultInfoWnd");
	Class'NWindow.UIAPI_TABCTRL'.static.SetTopOrder("ManorInfoWnd.ManorInfoTabCtrl", 2, false);
	HideWindow("ManorInfoWnd.SeedInfoWnd");
	HideWindow("ManorInfoWnd.CropInfoWnd");
	return;
}

function HandleDefaultAdd(string a_Param)
{
	local int CropID;
	local string CropName;
	local int CropLevel, SeedPrice, CropPrice;
	local string RewardType1, RewardType2;
	local LVDataRecord Record;

	ParseInt(a_Param, "CropID", CropID);
	ParseString(a_Param, "CropName", CropName);
	ParseInt(a_Param, "CropLevel", CropLevel);
	ParseInt(a_Param, "SeedPrice", SeedPrice);
	ParseInt(a_Param, "CropPrice", CropPrice);
	ParseString(a_Param, "RewardType1", RewardType1);
	ParseString(a_Param, "RewardType2", RewardType2);
	Record.LVDataList.Length = 6;
	Record.LVDataList[0].szData = CropName;
	Record.LVDataList[1].szData = string(CropLevel);
	Record.LVDataList[2].szData = string(SeedPrice);
	Record.LVDataList[3].szData = string(CropPrice);
	Record.LVDataList[4].szData = RewardType1;
	Record.LVDataList[5].szData = RewardType2;
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ManorInfoWnd.DefaultInfoWnd.DefaultInfoListCtrl", Record);
	return;
}

function OnClickButton(string strID)
{
	Debug(strID);
	switch(strID)
	{
		// End:0x53
		case "btnBuySeed":
			RequestBypassToServer("manor_menu_select?ask=1&state=-1&time=0");
			// End:0x132
			break;
		// End:0x95
		case "btnSellCrop":
			RequestBypassToServer("manor_menu_select?ask=2&state=-1&time=0");
			// End:0x132
			break;
		// End:0xBE
		case "ManorInfoTabCtrl0":
			OnClickInfoTab("SeedInfo");
			// End:0x132
			break;
		// End:0xE7
		case "ManorInfoTabCtrl1":
			OnClickInfoTab("CropInfo");
			// End:0x132
			break;
		// End:0x12F
		case "ManorInfoTabCtrl2":
			RequestBypassToServer("manor_menu_select?ask=5&state=-1&time=0");
			// End:0x132
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickInfoTab(string TabName)
{
	local int SelectedManorID, SelectedTime;
	local string RequestString, PreString;
	local int Index;
	local string ManorComboBoxName, TimeComboBoxName;

	switch(TabName)
	{
		// End:0xA6
		case "SeedInfo":
			PreString = "manor_menu_select?ask=3&state=";
			ManorComboBoxName = "ManorInfoWnd.SeedInfoWnd.cbManorSelectInSeedInfo";
			TimeComboBoxName = "ManorInfoWnd.SeedInfoWnd.cbTimeInSeedInfo";
			// End:0x148
			break;
		// End:0x145
		case "CropInfo":
			PreString = "manor_menu_select?ask=4&state=";
			ManorComboBoxName = "ManorInfoWnd.CropInfoWnd.cbManorSelectInCropInfo";
			TimeComboBoxName = "ManorInfoWnd.CropInfoWnd.cbTimeInCropInfo";
			// End:0x148
			break;
		// End:0xFFFF
		default:
			break;
	}
	Index = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum(ManorComboBoxName);
	SelectedManorID = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved(ManorComboBoxName, Index);
	SelectedTime = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum(TimeComboBoxName);
	Debug(__NFUN_112__("ID:", string(SelectedManorID)));
	// End:0x1DF
	if(__NFUN_130__(__NFUN_122__(TabName, "CropInfo"), __NFUN_154__(SelectedManorID, -1)))
	{
		SelectedManorID = m_MyManorID;
	}
	// End:0x23C
	if(__NFUN_155__(SelectedManorID, 0))
	{
		Debug(__NFUN_112__("Àå¿øID - ID:", string(SelectedManorID)));
		RequestString = __NFUN_112__(__NFUN_112__(__NFUN_112__(PreString, string(SelectedManorID)), "&time="), string(SelectedTime));
		RequestBypassToServer(RequestString);		
	}
	else
	{
		Debug(__NFUN_112__("Àå¿øID ¿À·ù - ID:", string(SelectedManorID)));
	}
	return;
}

function OnComboBoxItemSelected(string sName, int Index)
{
	Debug(__NFUN_112__(__NFUN_112__(sName, ", index:"), string(Index)));
	switch(sName)
	{
		// End:0x63
		case "cbManorSelectInSeedInfo":
			m_bTime = false;
			RequestSelectedData("SeedInfo", Index);
			// End:0x10C
			break;
		// End:0x98
		case "cbTimeInSeedInfo":
			m_bTime = true;
			RequestSelectedData("SeedInfo", Index);
			// End:0x10C
			break;
		// End:0xD4
		case "cbManorSelectInCropInfo":
			m_bTime = false;
			RequestSelectedData("CropInfo", Index);
			// End:0x10C
			break;
		// End:0x109
		case "cbTimeInCropInfo":
			m_bTime = true;
			RequestSelectedData("CropInfo", Index);
			// End:0x10C
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function RequestSelectedData(string WindowName, int Index)
{
	local int ManorID;
	local string RequestString, PreString;
	local int Time;
	local string ManorComboBoxName, TimeComboBoxName;

	// End:0xA6
	if(__NFUN_122__(WindowName, "SeedInfo"))
	{
		PreString = "manor_menu_select?ask=3&state=";
		ManorComboBoxName = "ManorInfoWnd.SeedInfoWnd.cbManorSelectInSeedInfo";
		TimeComboBoxName = "ManorInfoWnd.SeedInfoWnd.cbTimeInSeedInfo";		
	}
	else
	{
		PreString = "manor_menu_select?ask=4&state=";
		ManorComboBoxName = "ManorInfoWnd.CropInfoWnd.cbManorSelectInCropInfo";
		TimeComboBoxName = "ManorInfoWnd.CropInfoWnd.cbTimeInCropInfo";
	}
	// End:0x166
	if(m_bTime)
	{
		ManorID = m_ManorID;
		Time = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum(TimeComboBoxName);		
	}
	else
	{
		ManorID = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved(ManorComboBoxName, Index);
		Time = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum(TimeComboBoxName);
	}
	// End:0x1EB
	if(__NFUN_151__(ManorID, 0))
	{
		Debug(__NFUN_112__("Àå¿øID", string(ManorID)));
		RequestString = __NFUN_112__(__NFUN_112__(__NFUN_112__(PreString, string(ManorID)), "&time="), string(Time));		
	}
	else
	{
		Debug(__NFUN_112__("Àå¿øID¿À·ù : ", string(ManorID)));
	}
	RequestBypassToServer(RequestString);
	return;
}
