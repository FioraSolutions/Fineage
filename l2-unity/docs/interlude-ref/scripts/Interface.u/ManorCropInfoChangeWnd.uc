class ManorCropInfoChangeWnd extends UICommonAPI;

var int m_MinCropPrice;
var int m_MaxCropPrice;
var int m_TomorrowLimit;

function OnLoad()
{
	RegisterEvent(2670);
	return;
}

function OnEvent(int Event_ID, string a_Param)
{
	Debug(__NFUN_112__("", string(Event_ID)));
	switch(Event_ID)
	{
		// End:0x2E
		case 2670:
			HandleShow(a_Param);
			// End:0x31
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleShow(string a_Param)
{
	local string CropName;
	local int TomorrowVolumeOfBuy, TomorrowLimit, TomorrowPrice, TomorrowProcure, MinCropPrice, MaxCropPrice;

	local string TomorrowLimitString;

	ParseString(a_Param, "CropName", CropName);
	ParseInt(a_Param, "TomorrowVolumeOfBuy", TomorrowVolumeOfBuy);
	ParseInt(a_Param, "TomorrowLimit", TomorrowLimit);
	ParseInt(a_Param, "TomorrowPrice", TomorrowPrice);
	ParseInt(a_Param, "TomorrowProcure", TomorrowProcure);
	ParseInt(a_Param, "MinCropPrice", MinCropPrice);
	ParseInt(a_Param, "MaxCropPrice", MaxCropPrice);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorCropInfoChangeWnd.txtCropName", CropName);
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("ManorCropInfoChangeWnd.ebTomorrowAmountOfPurchase", string(TomorrowVolumeOfBuy));
	m_TomorrowLimit = TomorrowLimit;
	TomorrowLimitString = MakeCostString(string(TomorrowLimit));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorCropInfoChangeWnd.txtVarTomorrowPurchaseLimit", TomorrowLimitString);
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("ManorCropInfoChangeWnd.ebTomorrowPurchasePrice", string(TomorrowPrice));
	// End:0x219
	if(__NFUN_154__(TomorrowProcure, 0))
	{
		TomorrowProcure = 1;
	}
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("ManorCropInfoChangeWnd.cbTomorrowReward", __NFUN_147__(TomorrowProcure, 1));
	m_MinCropPrice = MinCropPrice;
	m_MaxCropPrice = MaxCropPrice;
	ShowWindowWithFocus("ManorCropInfoChangeWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("ManorCropInfoChangeWnd.ebTomorrowAmountOfPurchase");
	return;
}

function OnClickButton(string strID)
{
	Debug(__NFUN_112__(" ", strID));
	switch(strID)
	{
		// End:0x2A
		case "btnOk":
			OnClickBtnOk();
			// End:0x5C
			break;
		// End:0x59
		case "btnCancel":
			HideWindow("ManorCropInfoChangeWnd");
			// End:0x5C
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickBtnOk()
{
	local int InputTomorrowAmountOfPurchase, InputTomorrowPurchasePrice, InputTomorrowProcure;
	local string Procure;
	local int SelectedNum;
	local string ParamString;

	InputTomorrowAmountOfPurchase = int(Class'NWindow.UIAPI_EDITBOX'.static.GetString("ManorCropInfoChangeWnd.ebTomorrowAmountOfPurchase"));
	InputTomorrowPurchasePrice = int(Class'NWindow.UIAPI_EDITBOX'.static.GetString("ManorCropInfoChangeWnd.ebTomorrowPurchasePrice"));
	// End:0xC0
	if(__NFUN_132__(__NFUN_150__(InputTomorrowAmountOfPurchase, 0), __NFUN_151__(InputTomorrowAmountOfPurchase, m_TomorrowLimit)))
	{
		ShowErrorDialog(0, m_TomorrowLimit, 1560);
		return;
	}
	// End:0x104
	if(__NFUN_130__(__NFUN_155__(InputTomorrowAmountOfPurchase, 0), __NFUN_132__(__NFUN_150__(InputTomorrowPurchasePrice, m_MinCropPrice), __NFUN_151__(InputTomorrowPurchasePrice, m_MaxCropPrice))))
	{
		ShowErrorDialog(m_MinCropPrice, m_MaxCropPrice, 1559);
		return;
	}
	SelectedNum = Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("ManorCropInfoChangeWnd.cbTomorrowReward");
	Procure = Class'NWindow.UIAPI_COMBOBOX'.static.GetString("ManorCropInfoChangeWnd.cbTomorrowReward", SelectedNum);
	InputTomorrowProcure = int(Procure);
	ParamAdd(ParamString, "TomorrowAmountOfPurchase", string(InputTomorrowAmountOfPurchase));
	ParamAdd(ParamString, "TomorrowPurchasePrice", string(InputTomorrowPurchasePrice));
	ParamAdd(ParamString, "TomorrowProcure", string(InputTomorrowProcure));
	ExecuteEvent(2668, ParamString);
	HideWindow("ManorCropInfoChangeWnd");
	return;
}

function ShowErrorDialog(int MinValue, int MaxValue, int SystemStringIdx)
{
	local string ParamString, Message;

	ParamAdd(ParamString, "Type", string(int(1)));
	ParamAdd(ParamString, "param1", string(MinValue));
	AddSystemMessageParam(ParamString);
	ParamString = "";
	ParamAdd(ParamString, "Type", string(int(1)));
	ParamAdd(ParamString, "param1", string(MaxValue));
	AddSystemMessageParam(ParamString);
	Message = EndSystemMessageParam(SystemStringIdx, true);
	DialogShow(5, Message);
	return;
}
