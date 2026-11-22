class ManorSeedInfoChangeWnd extends UICommonAPI;

var int m_MinCropPrice;
var int m_MaxCropPrice;
var int m_TomorrowLimit;

function OnLoad()
{
	RegisterEvent(2660);
	return;
}

function OnEvent(int Event_ID, string a_Param)
{
	switch(Event_ID)
	{
		// End:0x1D
		case 2660:
			HandleShow(a_Param);
			// End:0x20
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleShow(string a_Param)
{
	local string SeedName;
	local int TomorrowVolumeOfSales, TomorrowLimit, TomorrowPrice, MinCropPrice, MaxCropPrice;

	local string TomorrowLimitString;

	ParseString(a_Param, "SeedName", SeedName);
	ParseInt(a_Param, "TomorrowVolumeOfSales", TomorrowVolumeOfSales);
	ParseInt(a_Param, "TomorrowLimit", TomorrowLimit);
	ParseInt(a_Param, "TomorrowPrice", TomorrowPrice);
	ParseInt(a_Param, "MinCropPrice", MinCropPrice);
	ParseInt(a_Param, "MaxCropPrice", MaxCropPrice);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorSeedInfoChangeWnd.txtSeedName", SeedName);
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("ManorSeedInfoChangeWnd.ebTomorrowSalesVolume", string(TomorrowVolumeOfSales));
	m_TomorrowLimit = TomorrowLimit;
	TomorrowLimitString = MakeCostString(string(TomorrowLimit));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ManorSeedInfoChangeWnd.txtVarTomorrowLimit", TomorrowLimitString);
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("ManorSeedInfoChangeWnd.ebTomorrowPrice", string(TomorrowPrice));
	m_MinCropPrice = MinCropPrice;
	m_MaxCropPrice = MaxCropPrice;
	ShowWindowWithFocus("ManorSeedInfoChangeWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("ManorSeedInfoChangeWnd.ebTomorrowSalesVolume");
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
			HideWindow("ManorSeedInfoChangeWnd");
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
	local int InputTomorrowSalesVolume, InputTomorrowPrice;
	local string ParamString;

	InputTomorrowSalesVolume = int(Class'NWindow.UIAPI_EDITBOX'.static.GetString("ManorSeedInfoChangeWnd.ebTomorrowSalesVolume"));
	InputTomorrowPrice = int(Class'NWindow.UIAPI_EDITBOX'.static.GetString("ManorSeedInfoChangeWnd.ebTomorrowPrice"));
	// End:0xB3
	if(__NFUN_132__(__NFUN_150__(InputTomorrowSalesVolume, 0), __NFUN_151__(InputTomorrowSalesVolume, m_TomorrowLimit)))
	{
		ShowErrorDialog(0, m_TomorrowLimit, 1558);
		return;
	}
	// End:0xF7
	if(__NFUN_130__(__NFUN_155__(InputTomorrowSalesVolume, 0), __NFUN_132__(__NFUN_150__(InputTomorrowPrice, m_MinCropPrice), __NFUN_151__(InputTomorrowPrice, m_MaxCropPrice))))
	{
		ShowErrorDialog(m_MinCropPrice, m_MaxCropPrice, 1557);
		return;
	}
	ParamAdd(ParamString, "TomorrowSalesVolume", string(InputTomorrowSalesVolume));
	ParamAdd(ParamString, "TomorrowPrice", string(InputTomorrowPrice));
	ExecuteEvent(2659, ParamString);
	HideWindow("ManorSeedInfoChangeWnd");
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
