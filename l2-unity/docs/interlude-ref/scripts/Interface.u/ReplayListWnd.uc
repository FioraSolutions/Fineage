class ReplayListWnd extends UIScript;

const REPLAY_DIR = "..\\REPLAY";
const REPLAY_EXTENSION = ".L2R";

var array<string> m_StrFileList;

function OnLoad()
{
	return;
}

function OnShow()
{
	InitReplayList();
	return;
}

function InitReplayList()
{
	local array<string> strReplayFileList;
	local int i, iLength;
	local string strFileName;

	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("ReplayListWnd.ReplayListCtrl");
	GetFileList(strReplayFileList, "..\\REPLAY", ".L2R");
	i = 0;
	J0x50:

	// End:0xAF [Loop If]
	if(__NFUN_150__(i, strReplayFileList.Length))
	{
		iLength = __NFUN_147__(__NFUN_125__(strReplayFileList[i]), __NFUN_125__(".L2R"));
		strFileName = __NFUN_128__(strReplayFileList[i], iLength);
		AddItem(i, strFileName);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x50;
	}
	return;
}

function OnEvent(int Event_ID, string param)
{
	return;
}

function AddItem(int iNum, string strFileName)
{
	local LVDataRecord Record;
	local LVData Data;

	Data.szData = string(iNum);
	Record.LVDataList[0] = Data;
	Data.szData = strFileName;
	Record.LVDataList[1] = Data;
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("ReplayListWnd.ReplayListCtrl", Record);
	return;
}

function string GetSelectedFileName()
{
	local int Index;
	local LVDataRecord Record;
	local string strFileName;

	Index = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("ReplayListWnd.ReplayListCtrl");
	// End:0x8D
	if(__NFUN_153__(Index, 0))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("ReplayListWnd.ReplayListCtrl", Index);
		strFileName = Record.LVDataList[1].szData;
	}
	return strFileName;
	return;
}

function OnDBClickListCtrlRecord(string ListCtrlID)
{
	OnOk();
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1A
		case "btnOK":
			OnOk();
			// End:0x5A
			break;
		// End:0x34
		case "btnDel":
			OnDelete();
			InitReplayList();
			// End:0x5A
			break;
		// End:0x57
		case "btnCancel":
			SetUIState("LoginState");
			// End:0x5A
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnOk()
{
	local string strFileName;
	local bool bLoadCameraInst, bLoadChatData;

	strFileName = GetSelectedFileName();
	// End:0x1A
	if(__NFUN_122__(strFileName, ""))
	{
		return;
	}
	bLoadCameraInst = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ReplayListWnd.chkLoadCamInst");
	bLoadChatData = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ReplayListWnd.chkLoadChatData");
	BeginReplay(strFileName, bLoadCameraInst, bLoadChatData);
	return;
}

function OnDelete()
{
	local string strFileName;

	strFileName = GetSelectedFileName();
	// End:0x1A
	if(__NFUN_122__(strFileName, ""))
	{
		return;
	}
	EraseReplayFile(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("..\\REPLAY", "\\"), strFileName), ""), ".L2R"));
	return;
}
