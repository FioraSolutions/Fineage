class ManorSeedInfoSettingWnd extends UICommonAPI;

const SEED_NAME = 0;
const TODAY_VOLUME_OF_SALES = 1;
const TODAY_PRICE = 2;
const TOMORROW_VOLUME_OF_SALES = 3;
const TOMORROW_PRICE = 4;
const MINIMUM_CROP_PRICE = 5;
const MAXIMUM_CROP_PRICE = 6;
const SEED_LEVEL = 7;
const REWARD_TYPE_1 = 8;
const REWARD_TYPE_2 = 9;
const COLUMN_CNT = 10;
const DIALOG_ID_STOP = 555;
const DIALOG_ID_SETTODAY = 666;

var int m_ManorID;
var int m_SumOfDefaultPrice;

function OnLoad()
{
	RegisterEvent(2656);
	RegisterEvent(2657);
	RegisterEvent(2658);
	RegisterEvent(2659);
	RegisterEvent(1710);
	m_ManorID = -1;
	m_SumOfDefaultPrice = 0;
	return;
}

function OnEvent(int Event_ID, string a_Param)
{
	switch(Event_ID)
	{
		// End:0x1D
		case 2656:
			HandleShow(a_Param);
			// End:0x8D
			break;
		// End:0x33
		case 2657:
			HandleAddItem(a_Param);
			// End:0x8D
			break;
		// End:0x63
		case 2658:
			CalculateSumOfDefaultPrice();
			ShowWindowWithFocus("ManorSeedInfoSettingWnd");
			// End:0x8D
			break;
		// End:0x79
		case 2659:
			HandleChangeValue(a_Param);
			// End:0x8D
			break;
		// End:0x8A
		case 1710:
			HandleDialogOK();
			// End:0x8D
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleDialogOK()
{
	local int dialogID;

	// End:0x0D
	if(__NFUN_129__(DialogIsMine()))
	{
		return;
	}
	dialogID = DialogGetID();
	switch(dialogID)
	{
		// End:0x31
		case 555:
			HandleStop();
			// End:0x45
			break;
		// End:0x42
		case 666:
			HandleSetToday();
			// End:0x45
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleStop()
{
	local int i, RecordCnt;
	local LVDataRecord Record, recordClear;

	RecordCnt = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl");
	Debug(__NFUN_112__("Ä«¿îÆ®:", string(RecordCnt)));
	i = 0;
	J0x6A:

	// End:0x159 [Loop If]
	if(__NFUN_150__(i, RecordCnt))
	{
		Record = recordClear;
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl", i);
		Record.LVDataList[3].szData = "0";
		Record.LVDataList[4].szData = "0";
		Class'NWindow.UIAPI_LISTCTRL'.static.ModifyRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl", i, Record);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x6A;
	}
	CalculateSumOfDefaultPrice();
	return;
}

function HandleSetToday()
{
	local int i, RecordCnt;
	local LVDataRecord Record, recordClear;

	RecordCnt = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl");
	i = 0;
	J0x52:

	// End:0x15E [Loop If]
	if(__NFUN_150__(i, RecordCnt))
	{
		Record = recordClear;
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl", i);
		Record.LVDataList[3].szData = Record.LVDataList[1].szData;
		Record.LVDataList[4].szData = Record.LVDataList[2].szData;
		Class'NWindow.UIAPI_LISTCTRL'.static.ModifyRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl", i, Record);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x52;
	}
	CalculateSumOfDefaultPrice();
	return;
}

function HandleShow(string a_Param)
{
	local int ManorID;
	local string ManorName;

	ParseInt(a_Param, "ManorID", ManorID);
	ParseString(a_Param, "ManorName", ManorName);
	m_ManorID = ManorID;
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorSeedInfoSettingWnd.txtManorName", ManorName);
	DeleteAll();
	return;
}

function HandleChangeValue(string a_Param)
{
	local int TomorrowSalesVolume, TomorrowPrice;
	local LVDataRecord Record;
	local int SelectedIndex;

	ParseInt(a_Param, "TomorrowSalesVolume", TomorrowSalesVolume);
	ParseInt(a_Param, "TomorrowPrice", TomorrowPrice);
	SelectedIndex = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl");
	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl");
	Record.LVDataList[3].szData = string(TomorrowSalesVolume);
	Record.LVDataList[4].szData = string(TomorrowPrice);
	Class'NWindow.UIAPI_LISTCTRL'.static.ModifyRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl", SelectedIndex, Record);
	CalculateSumOfDefaultPrice();
	return;
}

function DeleteAll()
{
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl");
	return;
}

function OnDBClickListCtrlRecord(string strID)
{
	switch(strID)
	{
		// End:0x31
		case "ManorSeedInfoSettingListCtrl":
			OnChangeBtn();
			// End:0x34
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickButton(string strID)
{
	Debug(__NFUN_112__(" ", strID));
	switch(strID)
	{
		// End:0x32
		case "btnChangeSell":
			OnChangeBtn();
			// End:0xD6
			break;
		// End:0x63
		case "btnSetToday":
			DialogSetID(666);
			DialogShow(4, GetSystemMessage(1601));
			// End:0xD6
			break;
		// End:0x90
		case "btnStop":
			DialogSetID(555);
			DialogShow(4, GetSystemMessage(1600));
			// End:0xD6
			break;
		// End:0xA3
		case "btnOk":
			OnOk();
			// End:0xD6
			break;
		// End:0xD3
		case "btnCancel":
			HideWindow("ManorSeedInfoSettingWnd");
			// End:0xD6
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnOk()
{
	local int RecordCount;
	local LVDataRecord Record, recordClear;
	local int i;
	local string param;

	RecordCount = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl");
	ParamAdd(param, "ManorID", string(m_ManorID));
	ParamAdd(param, "SeedCnt", string(RecordCount));
	i = 0;
	J0x88:

	// End:0x194 [Loop If]
	if(__NFUN_150__(i, RecordCount))
	{
		Record = recordClear;
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl", i);
		ParamAdd(param, __NFUN_112__("SeedID", string(i)), string(Record.nReserved1));
		ParamAdd(param, __NFUN_112__("TomorrowSalesVolume", string(i)), Record.LVDataList[3].szData);
		ParamAdd(param, __NFUN_112__("TomorrowPrice", string(i)), Record.LVDataList[4].szData);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x88;
	}
	RequestSetSeed(param);
	HideWindow("ManorSeedInfoSettingWnd");
	return;
}

function OnChangeBtn()
{
	local LVDataRecord Record;
	local string param;

	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl");
	ParamAdd(param, "SeedName", Record.LVDataList[0].szData);
	ParamAdd(param, "TomorrowVolumeOfSales", Record.LVDataList[3].szData);
	ParamAdd(param, "TomorrowLimit", string(Record.nReserved2));
	ParamAdd(param, "TomorrowPrice", Record.LVDataList[4].szData);
	ParamAdd(param, "MinCropPrice", Record.LVDataList[5].szData);
	ParamAdd(param, "MaxCropPrice", Record.LVDataList[6].szData);
	ExecuteEvent(2660, param);
	return;
}

function HandleAddItem(string a_Param)
{
	local LVDataRecord Record;
	local int SeedID;
	local string SeedName;
	local int TodaySeedTotalCnt, TodaySeedPrice, NextSeedTotalCnt, NextSeedPrice, MinCropPrice, MaxCropPrice,
		SeedLevel;

	local string RewardType1, RewardType2;
	local int MaxSeedTotalCnt, DefaultSeedPrice;

	ParseInt(a_Param, "SeedID", SeedID);
	ParseString(a_Param, "SeedName", SeedName);
	ParseInt(a_Param, "TodaySeedTotalCnt", TodaySeedTotalCnt);
	ParseInt(a_Param, "TodaySeedPrice", TodaySeedPrice);
	ParseInt(a_Param, "TodayNextSeedTotalCnt", NextSeedTotalCnt);
	ParseInt(a_Param, "NextSeedPrice", NextSeedPrice);
	ParseInt(a_Param, "MinCropPrice", MinCropPrice);
	ParseInt(a_Param, "MaxCropPrice", MaxCropPrice);
	ParseInt(a_Param, "SeedLevel", SeedLevel);
	ParseString(a_Param, "RewardType1", RewardType1);
	ParseString(a_Param, "RewardType2", RewardType2);
	ParseInt(a_Param, "MaxSeedTotalCnt", MaxSeedTotalCnt);
	ParseInt(a_Param, "DefaultSeedPrice", DefaultSeedPrice);
	Record.LVDataList.Length = 10;
	Record.LVDataList[0].szData = SeedName;
	Record.LVDataList[1].szData = string(TodaySeedTotalCnt);
	Record.LVDataList[2].szData = string(TodaySeedPrice);
	Record.LVDataList[3].szData = string(NextSeedTotalCnt);
	Record.LVDataList[4].szData = string(NextSeedPrice);
	Record.LVDataList[5].szData = string(MinCropPrice);
	Record.LVDataList[6].szData = string(MaxCropPrice);
	Record.LVDataList[7].szData = string(SeedLevel);
	Record.LVDataList[8].szData = RewardType1;
	Record.LVDataList[9].szData = RewardType2;
	Record.nReserved1 = SeedID;
	Record.nReserved2 = MaxSeedTotalCnt;
	Record.nReserved3 = DefaultSeedPrice;
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl", Record);
	return;
}

function CalculateSumOfDefaultPrice()
{
	local LVDataRecord Record, recordClear;
	local int ItemCnt, i, tmpMulti;
	local string adenaString;

	m_SumOfDefaultPrice = 0;
	ItemCnt = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl");
	i = 0;
	J0x59:

	// End:0xFF [Loop If]
	if(__NFUN_150__(i, ItemCnt))
	{
		Record = recordClear;
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorSeedInfoSettingWnd.ManorSeedInfoSettingListCtrl", i);
		tmpMulti = __NFUN_144__(Record.nReserved3, int(Record.LVDataList[3].szData));
		__NFUN_161__(m_SumOfDefaultPrice, tmpMulti);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x59;
	}
	adenaString = MakeCostString(string(m_SumOfDefaultPrice));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorSeedInfoSettingWnd.txtVarNextTotalExpense", adenaString);
	return;
}
