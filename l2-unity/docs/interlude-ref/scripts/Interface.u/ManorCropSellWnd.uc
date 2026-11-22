class ManorCropSellWnd extends UICommonAPI;

const CROP_NAME = 0;
const MANOR_NAME = 1;
const CROP_REMAIN_CNT = 2;
const CROP_PRICE = 3;
const PROCURE_TYPE = 4;
const MY_CROP_CNT = 5;
const SELL_CNT = 6;
const CROP_LEVEL = 7;
const REWARD_TYPE_1 = 8;
const REWARD_TYPE_2 = 9;
const COLUMN_CNT = 10;

function OnLoad()
{
	RegisterEvent(2640);
	RegisterEvent(2645);
	RegisterEvent(2646);
	return;
}

function OnEvent(int Event_ID, string a_Param)
{
	switch(Event_ID)
	{
		// End:0x66
		case 2640:
			// End:0x45
			if(IsShowWindow("ManorCropSellWnd"))
			{
				HideWindow("ManorCropSellWnd");				
			}
			else
			{
				DeleteAll();
				ShowWindowWithFocus("ManorCropSellWnd");
			}
			// End:0x95
			break;
		// End:0x7C
		case 2645:
			HandleAddItem(a_Param);
			// End:0x95
			break;
		// End:0x92
		case 2646:
			HandleSetCropSell(a_Param);
			// End:0x95
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function DeleteAll()
{
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ManorCropSellWnd.ManorCropSellListCtrl");
	return;
}

function OnDBClickListCtrlRecord(string strID)
{
	switch(strID)
	{
		// End:0x2A
		case "ManorCropSellListCtrl":
			OnChangeBtn();
			// End:0x2D
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
			// End:0x73
			break;
		// End:0x47
		case "btnSell":
			OnSellBtn();
			// End:0x73
			break;
		// End:0x70
		case "btnCancel":
			HideWindow("ManorCropSellWnd");
			// End:0x73
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnSellBtn()
{
	local int RecordCount;
	local LVDataRecord Record;
	local int SellCnt, CropCnt, CropNum, i;
	local string param;

	RecordCount = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorCropSellWnd.ManorCropSellListCtrl");
	CropCnt = 0;
	i = 0;
	J0x4B:

	// End:0xD2 [Loop If]
	if(__NFUN_150__(i, RecordCount))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorCropSellWnd.ManorCropSellListCtrl", i);
		SellCnt = int(Record.LVDataList[6].szData);
		// End:0xC8
		if(__NFUN_151__(SellCnt, 0))
		{
			__NFUN_165__(CropCnt);
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x4B;
	}
	ParamAdd(param, "CropCnt", string(CropCnt));
	CropNum = 0;
	i = 0;
	J0xFB:

	// End:0x235 [Loop If]
	if(__NFUN_150__(i, RecordCount))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorCropSellWnd.ManorCropSellListCtrl", i);
		SellCnt = int(Record.LVDataList[6].szData);
		// End:0x174
		if(__NFUN_152__(SellCnt, 0))
		{
			// [Explicit Continue]
			goto J0x22B;
		}
		ParamAdd(param, __NFUN_112__("CropServerID", string(CropNum)), string(Record.nReserved3));
		ParamAdd(param, __NFUN_112__("CropID", string(CropNum)), string(Record.nReserved2));
		ParamAdd(param, __NFUN_112__("ManorID", string(CropNum)), string(Record.nReserved1));
		ParamAdd(param, __NFUN_112__("SellCount", string(CropNum)), Record.LVDataList[6].szData);
		__NFUN_165__(CropNum);
		J0x22B:

		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xFB;
	}
	RequestProcureCropList(param);
	HideWindow("ManorCropSellWnd");
	return;
}

function OnChangeBtn()
{
	local LVDataRecord Record;
	local int SelectedIndex, CropID;
	local string ManorCropSellChangeWndString, param;

	SelectedIndex = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ManorCropSellWnd.ManorCropSellListCtrl");
	// End:0x4E
	if(__NFUN_154__(SelectedIndex, -1))
	{
		return;
	}
	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("ManorCropSellWnd.ManorCropSellListCtrl");
	CropID = Record.nReserved2;
	ManorCropSellChangeWndString = __NFUN_112__(__NFUN_112__("manor_menu_select?ask=9&state=", string(CropID)), "&time=0");
	RequestBypassToServer(ManorCropSellChangeWndString);
	ParamAdd(param, "CropName", Record.LVDataList[0].szData);
	ParamAdd(param, "RewardType1", Record.LVDataList[8].szData);
	ParamAdd(param, "RewardType2", Record.LVDataList[9].szData);
	ExecuteEvent(2649, param);
	return;
}

function HandleAddItem(string a_Param)
{
	local LVDataRecord Record;
	local string CropName, ManorName;
	local int CropRemainCnt, CropPrice, ProcureType, MyCropCnt, CropLevel;

	local string RewardType1, RewardType2;
	local int ManorID, CropID, CropServerID;

	Record.LVDataList.Length = 10;
	ParseString(a_Param, "CropName", CropName);
	ParseString(a_Param, "ManorName", ManorName);
	ParseInt(a_Param, "CropRemainCnt", CropRemainCnt);
	ParseInt(a_Param, "CropPrice", CropPrice);
	ParseInt(a_Param, "ProcureType", ProcureType);
	ParseInt(a_Param, "MyCropCnt", MyCropCnt);
	ParseInt(a_Param, "CropLevel", CropLevel);
	ParseString(a_Param, "RewardType1", RewardType1);
	ParseString(a_Param, "RewardType2", RewardType2);
	ParseInt(a_Param, "ManorID", ManorID);
	ParseInt(a_Param, "CropID", CropID);
	ParseInt(a_Param, "CropServerID", CropServerID);
	Record.LVDataList[0].szData = CropName;
	Record.LVDataList[1].szData = ManorName;
	Record.LVDataList[2].szData = string(CropRemainCnt);
	Record.LVDataList[3].szData = string(CropPrice);
	Record.LVDataList[4].szData = string(ProcureType);
	Record.LVDataList[5].szData = string(MyCropCnt);
	Record.LVDataList[6].szData = "0";
	Record.LVDataList[7].szData = string(CropLevel);
	Record.LVDataList[8].szData = RewardType1;
	Record.LVDataList[9].szData = RewardType2;
	Record.nReserved1 = ManorID;
	Record.nReserved2 = CropID;
	Record.nReserved3 = CropServerID;
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ManorCropSellWnd.ManorCropSellListCtrl", Record);
	return;
}

function HandleSetCropSell(string a_Param)
{
	local string SellCntString;
	local int ManorID;
	local string ManorName, CropRemainCntString, CropPriceString, ProcureTypeString;
	local int SelectedIndex;
	local LVDataRecord Record;

	ParseString(a_Param, "SellCntString", SellCntString);
	ParseInt(a_Param, "ManorID", ManorID);
	ParseString(a_Param, "ManorName", ManorName);
	ParseString(a_Param, "CropRemainCntString", CropRemainCntString);
	ParseString(a_Param, "CropPriceString", CropPriceString);
	ParseString(a_Param, "ProcureTypeString", ProcureTypeString);
	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("ManorCropSellWnd.ManorCropSellListCtrl");
	Record.LVDataList[1].szData = ManorName;
	Record.LVDataList[2].szData = CropRemainCntString;
	Record.LVDataList[3].szData = CropPriceString;
	Record.LVDataList[4].szData = ProcureTypeString;
	Record.LVDataList[6].szData = SellCntString;
	Record.nReserved1 = ManorID;
	SelectedIndex = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ManorCropSellWnd.ManorCropSellListCtrl");
	Class'NWindow.UIAPI_LISTCTRL'.static.ModifyRecord("ManorCropSellWnd.ManorCropSellListCtrl", SelectedIndex, Record);
	return;
}
