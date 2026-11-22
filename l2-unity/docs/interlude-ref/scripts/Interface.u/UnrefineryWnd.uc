class UnrefineryWnd extends UICommonAPI;

var WindowHandle m_UnRefineryWnd_Main;
var WindowHandle m_ItemtoUnRefineWnd;
var WindowHandle m_ItemtoUnRefineAnim;
var WindowHandle m_hSelectedItemHighlight;
var WindowHandle m_ResultAnimation1;
var AnimTextureHandle m_ResultAnim1;
var TextBoxHandle m_InstructionText;
var TextBoxHandle m_AdenaText;
var ButtonHandle m_hUnrefineButton;
var ButtonHandle m_OkBtn;
var ItemWindowHandle m_ItemDragBox;
var ItemInfo CurrentItem;
var bool procedure1stat;
var bool procedureopenstat;
var INT64 m_Adena;

function OnLoad()
{
	RegisterEvent(2810);
	RegisterEvent(2820);
	RegisterEvent(2830);
	procedure1stat = false;
	procedureopenstat = false;
	m_ResultAnimation1 = GetHandle("UnrefineryWnd.RefineResultAnimation01");
	m_ResultAnim1 = AnimTextureHandle(GetHandle("UnrefineryWnd.RefineResultAnimation01.RefineResult1"));
	m_UnRefineryWnd_Main = GetHandle("UnrefineryWnd");
	m_ItemtoUnRefineWnd = GetHandle("Itemtounrefine");
	m_ItemtoUnRefineAnim = GetHandle("ItemtounrefineAnim");
	m_hSelectedItemHighlight = GetHandle("SelectedItemHighlight");
	m_ItemDragBox = ItemWindowHandle(GetHandle("UnRefineryWnd.Itemtounrefine.ItemUnrefine"));
	m_InstructionText = TextBoxHandle(GetHandle("UnrefineryWnd.txtInstruction"));
	m_AdenaText = TextBoxHandle(GetHandle("UnrefineryWnd.txtAdenaInstruction"));
	m_hUnrefineButton = ButtonHandle(GetHandle("btnUnRefine"));
	m_OkBtn = ButtonHandle(GetHandle("btnClose"));
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.SetProgressTime("UnrefineryWnd.UnRefineryProgress", 2000);
	return;
}

function OnShow()
{
	ResetReady();
	return;
}

function ResetReady()
{
	procedure1stat = false;
	procedureopenstat = false;
	m_UnRefineryWnd_Main.ShowWindow();
	m_ItemtoUnRefineWnd.ShowWindow();
	m_ItemtoUnRefineAnim.ShowWindow();
	m_hSelectedItemHighlight.HideWindow();
	m_ResultAnimation1.HideWindow();
	m_ResultAnim1.Stop();
	m_hUnrefineButton.DisableWindow();
	m_ItemDragBox.Clear();
	m_InstructionText.SetText(GetSystemMessage(1963));
	SetAdenaText("0");
	m_AdenaText.SetTooltipString("");
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.SetProgressTime("UnrefineryWnd.UnRefineryProgress", 2000);
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.Reset("UnrefineryWnd.UnRefineryProgress");
	PlaySound("ItemSound2.smelting.Smelting_dragin");
	m_OkBtn.EnableWindow();
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x4F
		case 2810:
			// End:0x4C
			if(__NFUN_242__(procedureopenstat, false))
			{
				PlaySound("ItemSound2.smelting.Smelting_dragin");
				ResetReady();
			}
			// End:0xD7
			break;
		// End:0x90
		case 2820:
			PlaySound("ItemSound2.smelting.Smelting_dragin");
			OnTargetItemValidationResult(a_Param);
			// End:0xD7
			break;
		// End:0xD1
		case 2830:
			PlaySound("ItemSound2.smelting.smelting_finalA");
			OnUnRefineDoneResult(a_Param);
			// End:0xD7
			break;
		// End:0xFFFF
		default:
			// End:0xD7
			break;
			break;
	}
	return;
}

function OnDropItem(string a_WindowID, ItemInfo a_ItemInfo, int X, int Y)
{
	switch(a_WindowID)
	{
		// End:0x32
		case "ItemUnrefine":
			// End:0x2F
			if(__NFUN_242__(procedure1stat, false))
			{
				ValidateItem(a_ItemInfo);
			}
			// End:0x35
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ValidateItem(ItemInfo a_ItemInfo)
{
	local int TargetItemServerID;

	CurrentItem = a_ItemInfo;
	TargetItemServerID = a_ItemInfo.ServerID;
	Class'NWindow.RefineryAPI'.static.ConfirmCancelItem(TargetItemServerID);
	return;
}

function OnTargetItemValidationResult(string a_Param)
{
	local int ItemServerID, ItemClassID, Option1, Option2, ItemValidationResult;

	local string AdenaText;

	Debug("¾ÆÀÌÅÛ ¼ö¶ô °áÁ¤");
	ParseInt(a_Param, "CancelItemServerID", ItemServerID);
	ParseInt(a_Param, "CancelItemClassID", ItemClassID);
	ParseInt(a_Param, "Option1", Option1);
	ParseInt(a_Param, "Option2", Option2);
	ParseInt64(a_Param, "Adena", m_Adena);
	ParseInt(a_Param, "Result", ItemValidationResult);
	switch(ItemValidationResult)
	{
		// End:0x193
		case 1:
			m_hUnrefineButton.EnableWindow();
			// End:0x108
			if(__NFUN_129__(m_ItemDragBox.SetItem(0, CurrentItem)))
			{
				m_ItemDragBox.AddItem(CurrentItem);
			}
			AdenaText = MakeCostStringInt64(m_Adena);
			SetAdenaText(AdenaText);
			m_ItemtoUnRefineAnim.HideWindow();
			m_hSelectedItemHighlight.ShowWindow();
			m_InstructionText.SetText("");
			procedureopenstat = true;
			// End:0x190
			if(__NFUN_242__(CheckAdena(), false))
			{
				m_hUnrefineButton.DisableWindow();
				m_InstructionText.SetText(GetSystemMessage(279));
			}
			// End:0x19D
			break;
		// End:0x19A
		case 0:
			// End:0x19D
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function SetAdenaText(string a_AdenaText)
{
	local string AdenaText;

	AdenaText = ConvertNumToText(a_AdenaText);
	m_AdenaText.SetText(__NFUN_168__(a_AdenaText, GetSystemString(469)));
	m_AdenaText.SetTextColor(GetNumericColor(a_AdenaText));
	// End:0x6D
	if(__NFUN_154__(int(a_AdenaText), 0))
	{
		m_AdenaText.SetTooltipString("");		
	}
	else
	{
		m_AdenaText.SetTooltipString(AdenaText);
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x20
		case "btnUnRefine":
			OnClickUnRefineButton();
			// End:0x7E
			break;
		// End:0x7B
		case "btnClose":
			procedure1stat = false;
			procedureopenstat = false;
			PlaySound("Itemsound2.smelting.smelting_dragout");
			m_UnRefineryWnd_Main.HideWindow();
			// End:0x7E
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickUnRefineButton()
{
	local INT64 Diff, CurAdena;

	CurAdena.nLeft = 0;
	CurAdena.nRight = GetAdena();
	Diff = Int64SubtractBfromA(CurAdena, m_Adena);
	// End:0xD5
	if(__NFUN_132__(__NFUN_153__(Diff.nLeft, 0), __NFUN_153__(Diff.nRight, 0)))
	{
		m_hUnrefineButton.DisableWindow();
		m_ResultAnim1.SetLoopCount(1);
		m_ResultAnimation1.ShowWindow();
		PlaySound("ItemSound2.smelting.smelting_loding");
		m_ResultAnim1.Play();
		PlayProgressiveBar();
		m_OkBtn.DisableWindow();		
	}
	else
	{
		DialogShow(1, GetSystemMessage(279));
	}
	return;
}

function bool CheckAdena()
{
	local INT64 Diff, CurAdena;

	CurAdena.nLeft = 0;
	CurAdena.nRight = GetAdena();
	Diff = Int64SubtractBfromA(CurAdena, m_Adena);
	// End:0x5A
	if(__NFUN_132__(__NFUN_153__(Diff.nLeft, 0), __NFUN_153__(Diff.nRight, 0)))
	{
		return true;		
	}
	else
	{
		return false;
	}
	return;
}

function PlayProgressiveBar()
{
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.Start("UnrefineryWnd.UnRefineryProgress");
	return;
}

function OnProgressTimeUp(string aWindowID)
{
	switch(aWindowID)
	{
		// End:0x27
		case "UnRefineryProgress":
			OnUnRefineRequest();
			// End:0x2A
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnTextureAnimEnd(AnimTextureHandle a_WindowHandle)
{
	switch(a_WindowHandle)
	{
		// End:0x21
		case m_ResultAnim1:
			m_ResultAnimation1.HideWindow();
			// End:0x24
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnUnRefineRequest()
{
	Class'NWindow.RefineryAPI'.static.RequestRefineCancel(CurrentItem.ServerID);
	return;
}

function OnUnRefineDoneResult(string a_Param)
{
	local int UnRefineResult;

	ParseInt(a_Param, "Result", UnRefineResult);
	m_OkBtn.EnableWindow();
	Debug("¾ÆÀÌÅÛ Á¦·Ã ÇØÁ¦ ¿Ï·á");
	switch(UnRefineResult)
	{
		// End:0xDB
		case 1:
			CurrentItem.RefineryOp1 = 0;
			CurrentItem.RefineryOp2 = 0;
			// End:0x95
			if(__NFUN_129__(m_ItemDragBox.SetItem(0, CurrentItem)))
			{
				m_ItemDragBox.AddItem(CurrentItem);
			}
			m_InstructionText.SetText(MakeFullSystemMsg(GetSystemMessage(1965), CurrentItem.Name, ""));
			m_hUnrefineButton.DisableWindow();
			procedure1stat = true;
			// End:0x10B
			break;
		// End:0x108
		case 0:
			m_hUnrefineButton.EnableWindow();
			procedure1stat = false;
			m_UnRefineryWnd_Main.HideWindow();
			// End:0x10B
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}
