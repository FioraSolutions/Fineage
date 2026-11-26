class ManorCropInfoSettingWnd extends UICommonAPI;

const CROP_NAME = 0;
const TODAY_CROP_TOTOAL_CNT = 1;
const TODAY_CROP_PRICE = 2;
const TODAY_PROCURE_TYPE = 3;
const NEXT_CROP_TOTAL_CNT = 4;
const NEXT_CROP_PRICE = 5;
const NEXT_PROCURE_TYPE = 6;
const MIN_CROP_PRICE = 7;
const MAX_CROP_PRICE = 8;
const CROP_LELEL = 9;
const REWARD_TYPE_1 = 10;
const REWARD_TYPE_2 = 11;
const COLUMN_CNT = 12;
const DIALOG_ID_STOP = 777;
const DIALOG_ID_SETTODAY = 888;

var int m_ManorID;
var int m_SumOfDefaultPrice;

function OnLoad()
{
	RegisterEvent(2665);
	RegisterEvent(2666);
	RegisterEvent(2667);
	RegisterEvent(2668);
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
		case 2665:
			HandleShow(a_Param);
			// End:0x8D
			break;
		// End:0x33
		case 2666:
			HandleAddItem(a_Param);
			// End:0x8D
			break;
		// End:0x63
		case 2667:
			CalculateSumOfDefaultPrice();
			ShowWindowWithFocus("ManorCropInfoSettingWnd");
			// End:0x8D
			break;
		// End:0x79
		case 2668:
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
		case 777:
			HandleStop();
			// End:0x45
			break;
		// End:0x42
		case 888:
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

	RecordCnt = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl");
	i = 0;
	J0x52:

	// End:0x157 [Loop If]
	if(__NFUN_150__(i, RecordCnt))
	{
		Record = recordClear;
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl", i);
		Record.LVDataList[4].szData = "0";
		Record.LVDataList[5].szData = "0";
		Record.LVDataList[6].szData = "0";
		Class'NWindow.UIAPI_LISTCTRL'.static.ModifyRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl", i, Record);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x52;
	}
	CalculateSumOfDefaultPrice();
	return;
}

function HandleSetToday()
{
	local int i, RecordCnt;
	local LVDataRecord Record, recordClear;

	RecordCnt = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl");
	i = 0;
	J0x52:

	// End:0x183 [Loop If]
	if(__NFUN_150__(i, RecordCnt))
	{
		Record = recordClear;
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl", i);
		Record.LVDataList[4].szData = Record.LVDataList[1].szData;
		Record.LVDataList[5].szData = Record.LVDataList[2].szData;
		Record.LVDataList[6].szData = Record.LVDataList[3].szData;
		Class'NWindow.UIAPI_LISTCTRL'.static.ModifyRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl", i, Record);
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
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorCropInfoSettingWnd.txtManorName", ManorName);
	DeleteAll();
	return;
}

function DeleteAll()
{
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl");
	return;
}

function OnDBClickListCtrlRecord(string strID)
{
	switch(strID)
	{
		// End:0x31
		case "ManorCropInfoSettingListCtrl":
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
			// End:0xF5
			break;
		// End:0x63
		case "btnSetToday":
			DialogSetID(888);
			DialogShow(4, GetSystemMessage(1601));
			// End:0xF5
			break;
		// End:0x90
		case "btnStop":
			DialogSetID(777);
			DialogShow(4, GetSystemMessage(1600));
			// End:0xF5
			break;
		// End:0xC2
		case "btnOk":
			OnOk();
			HideWindow("ManorCropInfoSettingWnd");
			// End:0xF5
			break;
		// End:0xF2
		case "btnCancel":
			HideWindow("ManorCropInfoSettingWnd");
			// End:0xF5
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

	RecordCount = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl");
	ParamAdd(param, "ManorID", string(m_ManorID));
	ParamAdd(param, "CropCnt", string(RecordCount));
	i = 0;
	J0x88:

	// End:0x1B2 [Loop If]
	if(__NFUN_150__(i, RecordCount))
	{
		Record = recordClear;
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl", i);
		ParamAdd(param, __NFUN_112__("CropID", string(i)), string(Record.nReserved1));
		ParamAdd(param, __NFUN_112__("BuyCnt", string(i)), Record.LVDataList[4].szData);
		ParamAdd(param, __NFUN_112__("Price", string(i)), Record.LVDataList[5].szData);
		ParamAdd(param, __NFUN_112__("ProcureType", string(i)), Record.LVDataList[6].szData);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x88;
	}
	RequestSetCrop(param);
	return;
}

function HandleAddItem(string a_Param)
{
	local LVDataRecord Record;
	local int CropID;
	local string CropName;
	local int TodayCropTotalCnt, TodayCropPrice, TodayProcureType, NextCropTotalCnt, NextCropPrice, NextProcureType,
		MinCropPrice, MaxCropPrice, CropLevel;

	local string RewardType1, RewardType2;
	local int MaxCropTotalCnt, DefaultCropPrice;

	ParseInt(a_Param, "CropID", CropID);
	ParseString(a_Param, "CropName", CropName);
	ParseInt(a_Param, "TodayCropTotalCnt", TodayCropTotalCnt);
	ParseInt(a_Param, "TodayCropPrice", TodayCropPrice);
	ParseInt(a_Param, "TodayProcureType", TodayProcureType);
	ParseInt(a_Param, "NextCropTotalCnt", NextCropTotalCnt);
	ParseInt(a_Param, "NextCropPrice", NextCropPrice);
	ParseInt(a_Param, "NextProcureType", NextProcureType);
	ParseInt(a_Param, "MinCropPrice", MinCropPrice);
	ParseInt(a_Param, "MaxCropPrice", MaxCropPrice);
	ParseInt(a_Param, "CropLevel", CropLevel);
	ParseString(a_Param, "RewardType1", RewardType1);
	ParseString(a_Param, "RewardType2", RewardType2);
	ParseInt(a_Param, "MaxCropTotalCnt", MaxCropTotalCnt);
	ParseInt(a_Param, "DefaultCropPrice", DefaultCropPrice);
	Record.LVDataList.Length = 12;
	Record.LVDataList[0].szData = CropName;
	Record.LVDataList[1].szData = string(TodayCropTotalCnt);
	Record.LVDataList[2].szData = string(TodayCropPrice);
	Record.LVDataList[3].szData = string(TodayProcureType);
	Record.LVDataList[4].szData = string(NextCropTotalCnt);
	Record.LVDataList[5].szData = string(NextCropPrice);
	Record.LVDataList[6].szData = string(NextProcureType);
	Record.LVDataList[7].szData = string(MinCropPrice);
	Record.LVDataList[8].szData = string(MaxCropPrice);
	Record.LVDataList[9].szData = string(CropLevel);
	Record.LVDataList[10].szData = RewardType1;
	Record.LVDataList[11].szData = RewardType2;
	Record.nReserved1 = CropID;
	Record.nReserved2 = MaxCropTotalCnt;
	Record.nReserved3 = DefaultCropPrice;
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl", Record);
	return;
}

function CalculateSumOfDefaultPrice()
{
	local LVDataRecord Record, recordClear;
	local int ItemCnt, i, tmpMulti;
	local string adenaString;

	m_SumOfDefaultPrice = 0;
	ItemCnt = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl");
	i = 0;
	J0x59:

	// End:0x109 [Loop If]
	if(__NFUN_150__(i, ItemCnt))
	{
		Record = recordClear;
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl", i);
		tmpMulti = __NFUN_144__(int(Record.LVDataList[5].szData), int(Record.LVDataList[4].szData));
		__NFUN_161__(m_SumOfDefaultPrice, tmpMulti);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x59;
	}
	adenaString = MakeCostString(string(m_SumOfDefaultPrice));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorCropInfoSettingWnd.txtVarNextTotalExpense", adenaString);
	return;
}

function OnChangeBtn()
{
	local LVDataRecord Record;
	local string param;

	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl");
	ParamAdd(param, "CropName", Record.LVDataList[0].szData);
	ParamAdd(param, "TomorrowVolumeOfBuy", Record.LVDataList[4].szData);
	ParamAdd(param, "TomorrowLimit", string(Record.nReserved2));
	ParamAdd(param, "TomorrowPrice", Record.LVDataList[5].szData);
	ParamAdd(param, "TomorrowProcure", Record.LVDataList[6].szData);
	ParamAdd(param, "MinCropPrice", Record.LVDataList[7].szData);
	ParamAdd(param, "MaxCropPrice", Record.LVDataList[8].szData);
	ExecuteEvent(2670, param);
	return;
}

function HandleChangeValue(string a_Param)
{
	local int TomorrowAmountOfPurchase, TomorrowPurchasePrice, TomorrowProcure;
	local LVDataRecord Record;
	local int SelectedIndex;

	ParseInt(a_Param, "TomorrowAmountOfPurchase", TomorrowAmountOfPurchase);
	ParseInt(a_Param, "TomorrowPurchasePrice", TomorrowPurchasePrice);
	ParseInt(a_Param, "TomorrowProcure", TomorrowProcure);
	SelectedIndex = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl");
	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl");
	Record.LVDataList[4].szData = string(TomorrowAmountOfPurchase);
	Record.LVDataList[5].szData = string(TomorrowPurchasePrice);
	Record.LVDataList[6].szData = string(TomorrowProcure);
	Class'NWindow.UIAPI_LISTCTRL'.static.ModifyRecord("ManorCropInfoSettingWnd.ManorCropInfoSettingListCtrl", SelectedIndex, Record);
	CalculateSumOfDefaultPrice();
	return;
}
