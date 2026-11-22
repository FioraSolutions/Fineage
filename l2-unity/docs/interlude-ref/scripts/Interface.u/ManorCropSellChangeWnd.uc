class ManorCropSellChangeWnd extends UICommonAPI;

const MANOR_NAME = 0;
const CROP_REMAIN_CNT = 1;
const CROP_PRICE = 2;
const PROCURE_TYPE = 3;
const COLUMN_CNT = 4;

function OnLoad()
{
	RegisterEvent(2647);
	RegisterEvent(2648);
	RegisterEvent(2649);
	return;
}

function OnEvent(int Event_ID, string a_Param)
{
	switch(Event_ID)
	{
		// End:0x78
		case 2647:
			// End:0x51
			if(IsShowWindow("ManorCropSellChangeWnd"))
			{
				HideWindow("ManorCropSellChangeWnd");				
			}
			else
			{
				Clear();
				ShowWindowWithFocus("ManorCropSellChangeWnd");
			}
			// End:0xA7
			break;
		// End:0x8E
		case 2648:
			HandleAddItem(a_Param);
			// End:0xA7
			break;
		// End:0xA4
		case 2649:
			HandleSetCropNameAndRewardType(a_Param);
			// End:0xA7
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ManorCropSellChangeWnd.ManorCropSellChangeListCtrl");
	Class'NWindow.UIAPI_COMBOBOX'.static.Clear("ManorCropSellChangeWnd.cbPurchasePlace");
	Class'NWindow.UIAPI_COMBOBOX'.static.SYS_AddStringWithReserved("ManorCropSellChangeWnd.cbPurchasePlace", 1276, -1);
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("ManorCropSellChangeWnd.cbPurchasePlace", 0);
	Class'NWindow.UIAPI_EDITBOX'.static.Clear("ManorCropSellChangeWnd.ebSalesVolume");
	return;
}

function HandleSetCropNameAndRewardType(string a_Param)
{
	local string CropName, RewardType1, RewardType2;

	ParseString(a_Param, "CropName", CropName);
	ParseString(a_Param, "RewardType1", RewardType1);
	ParseString(a_Param, "RewardType2", RewardType2);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorCropSellChangeWnd.txtVarCropName", CropName);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorCropSellChangeWnd.txtVarRewardType1", RewardType1);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorCropSellChangeWnd.txtVarRewardType2", RewardType2);
	return;
}

function HandleAddItem(string a_Param)
{
	local LVDataRecord Record;
	local string ManorName;
	local int CropRemainCnt, CropPrice, ProcureType, ManorID;

	Record.LVDataList.Length = 4;
	ParseString(a_Param, "ManorName", ManorName);
	ParseInt(a_Param, "CropRemainCnt", CropRemainCnt);
	ParseInt(a_Param, "CropPrice", CropPrice);
	ParseInt(a_Param, "ProcureType", ProcureType);
	ParseInt(a_Param, "ManorID", ManorID);
	Record.LVDataList[0].szData = ManorName;
	Record.LVDataList[1].szData = string(CropRemainCnt);
	Record.LVDataList[2].szData = string(CropPrice);
	Record.LVDataList[3].szData = string(ProcureType);
	Record.nReserved1 = ManorID;
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ManorCropSellChangeWnd.ManorCropSellChangeListCtrl", Record);
	Class'NWindow.UIAPI_COMBOBOX'.static.AddStringWithReserved("ManorCropSellChangeWnd.cbPurchasePlace", ManorName, ManorID);
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1B
		case "btnMax":
			HandleMaxButton();
			// End:0x60
			break;
		// End:0x2E
		case "btnOk":
			HandleOkBtn();
			// End:0x60
			break;
		// End:0x5D
		case "btnCancel":
			HideWindow("ManorCropSellChangeWnd");
			// End:0x60
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleMaxButton()
{
	local LVDataRecord Record;
	local int ManorID, MyCropCnt, CropRemainCnt, MinValue;
	local string MinValueString;

	Record = GetComboBoxSelectedRecord();
	CropRemainCnt = int(Record.LVDataList[1].szData);
	ManorID = GetComboBoxSelectedManorID();
	// End:0x42
	if(__NFUN_154__(ManorID, -1))
	{
		return;
	}
	MyCropCnt = GetMyCropCnt(ManorID);
	MinValue = __NFUN_249__(MyCropCnt, CropRemainCnt);
	// End:0x80
	if(__NFUN_154__(MinValue, -1))
	{
		MinValueString = "0";		
	}
	else
	{
		MinValueString = string(MinValue);
	}
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("ManorCropSellChangeWnd.ebSalesVolume", MinValueString);
	return;
}

function HandleOkBtn()
{
	local LVDataRecord Record;
	local int ManorID;
	local string SellCntString, param;

	Record = GetComboBoxSelectedRecord();
	SellCntString = Class'NWindow.UIAPI_EDITBOX'.static.GetString("ManorCropSellChangeWnd.ebSalesVolume");
	ManorID = Record.nReserved1;
	ParamAdd(param, "SellCntString", SellCntString);
	ParamAdd(param, "ManorID", string(ManorID));
	ParamAdd(param, "ManorName", Record.LVDataList[0].szData);
	ParamAdd(param, "CropRemainCntString", Record.LVDataList[1].szData);
	ParamAdd(param, "CropPriceString", Record.LVDataList[2].szData);
	ParamAdd(param, "ProcureTypeString", Record.LVDataList[3].szData);
	ExecuteEvent(2646, param);
	HideWindow("ManorCropSellChangeWnd");
	return;
}

function int GetComboBoxSelectedManorID()
{
	local int ManorID, cbSelectedIndex;

	cbSelectedIndex = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("ManorCropSellChangeWnd.cbPurchasePlace");
	ManorID = Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("ManorCropSellChangeWnd.cbPurchasePlace", cbSelectedIndex);
	return ManorID;
	return;
}

function LVDataRecord GetComboBoxSelectedRecord()
{
	local LVDataRecord Record;
	local int ManorID, RecordCount, i;

	ManorID = GetComboBoxSelectedManorID();
	RecordCount = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("ManorCropSellChangeWnd.ManorCropSellChangeListCtrl");
	i = 0;
	J0x5C:

	// End:0xDA [Loop If]
	if(__NFUN_150__(i, RecordCount))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ManorCropSellChangeWnd.ManorCropSellChangeListCtrl", i);
		// End:0xD0
		if(__NFUN_154__(Record.nReserved1, ManorID))
		{
			// [Explicit Break]
			goto J0xDA;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x5C;
	}
	J0xDA:

	return Record;
	return;
}

function int GetMyCropCnt(int ManorID)
{
	local int MyCropCnt;
	local LVDataRecord Record;

	MyCropCnt = -1;
	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("ManorCropSellWnd.ManorCropSellListCtrl");
	MyCropCnt = int(Record.LVDataList[5].szData);
	return MyCropCnt;
	return;
}
