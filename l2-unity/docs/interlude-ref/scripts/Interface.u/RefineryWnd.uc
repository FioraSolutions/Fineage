class RefineryWnd extends UICommonAPI;

const DIALOGID_GemstoneCount = 0;
const C_ANIMLOOPCOUNT = 1;
const C_ANIMLOOPCOUNT1 = 1;
const C_ANIMLOOPCOUNT2 = 1;
const C_ANIMLOOPCOUNT3 = 1;

var bool procedure1stat;
var bool procedure2stat;
var bool procedure3stat;
var bool procedure4stat;
var ItemInfo RefineItemInfo;
var ItemInfo RefinerItemInfo;
var ItemInfo GemstoneItemInfo;
var ItemInfo RefinedITemInfo;
var WindowHandle m_RefineryWnd_Main;
var WindowHandle m_RefineResultBackPattern;
var WindowHandle m_Highlight1;
var WindowHandle m_Highlight2;
var WindowHandle m_Highlight3;
var WindowHandle m_SeletedItemHighlight1;
var WindowHandle m_SeletedItemHighlight2;
var WindowHandle m_SeletedItemHighlight3;
var WindowHandle m_DragBox1;
var WindowHandle m_DragBox2;
var WindowHandle m_DragBox3;
var WindowHandle m_DragBoxResult;
var WindowHandle m_RefineAnimation;
var WindowHandle m_ResultAnimation1;
var WindowHandle m_ResultAnimation2;
var WindowHandle m_ResultAnimation3;
var AnimTextureHandle m_RefineAnim;
var AnimTextureHandle m_ResultAnim1;
var AnimTextureHandle m_ResultAnim2;
var AnimTextureHandle m_ResultAnim3;
var ButtonHandle m_OkBtn;
var ButtonHandle m_RefineryBtn;
var ItemWindowHandle m_DragboxItem1;
var ItemWindowHandle m_DragBoxItem2;
var ItemWindowHandle m_DragBoxItem3;
var ItemWindowHandle m_ResultBoxItem;
var TextBoxHandle m_InstructionText;
var TextBoxHandle m_hGemstoneNameTextBox;
var TextBoxHandle m_hGemstoneCountTextBox;
var int m_TargetItemServerID;
var int m_RefineItemServerID;
var int m_GemStoneServerID;
var int m_GemStoneClassID;
var int m_NecessaryGemstoneCount;
var int m_GemstoneCount;
var string m_GemstoneName;
var InventoryWnd InventoryWndScript;

function OnLoad()
{
	RegisterEvent(2760);
	RegisterEvent(2770);
	RegisterEvent(2780);
	RegisterEvent(2790);
	RegisterEvent(2800);
	RegisterEvent(1710);
	procedure1stat = false;
	procedure2stat = false;
	procedure3stat = false;
	procedure4stat = false;
	m_RefineryWnd_Main = GetHandle("RefineryWnd");
	m_RefineResultBackPattern = GetHandle("RefineryWnd.BackPattern");
	m_Highlight1 = GetHandle("RefineryWnd.ItemDragBox1Wnd.DropHighlight");
	m_Highlight2 = GetHandle("RefineryWnd.ItemDragBox2Wnd.DropHighlight");
	m_Highlight3 = GetHandle("RefineryWnd.ItemDragBox3Wnd.DropHighlight");
	m_SeletedItemHighlight1 = GetHandle("RefineryWnd.ItemDragBox1Wnd.SelectedItemHighlight");
	m_SeletedItemHighlight2 = GetHandle("RefineryWnd.ItemDragBox2Wnd.SelectedItemHighlight");
	m_SeletedItemHighlight3 = GetHandle("RefineryWnd.ItemDragBox3Wnd.SelectedItemHighlight");
	m_DragBox1 = GetHandle("RefineryWnd.ItemDragBox1Wnd");
	m_DragBox2 = GetHandle("RefineryWnd.ItemDragBox2Wnd");
	m_DragBox3 = GetHandle("RefineryWnd.ItemDragBox3Wnd");
	m_DragBoxResult = GetHandle("RefineryWnd.ItemDragBoxResultWnd");
	m_RefineAnimation = GetHandle("RefineryWnd.RefineLoadingAnimation");
	m_ResultAnimation1 = GetHandle("RefineryWnd.RefineResultAnimation01");
	m_ResultAnimation2 = GetHandle("RefineryWnd.RefineResultAnimation02");
	m_ResultAnimation3 = GetHandle("RefineryWnd.RefineResultAnimation03");
	m_RefineAnim = AnimTextureHandle(GetHandle("RefineryWnd.RefineLoadingAnimation.RefineLoadingAnim"));
	m_ResultAnim1 = AnimTextureHandle(GetHandle("RefineryWnd.RefineResultAnimation01.RefineResult1"));
	m_ResultAnim2 = AnimTextureHandle(GetHandle("RefineryWnd.RefineResultAnimation02.RefineResult2"));
	m_ResultAnim3 = AnimTextureHandle(GetHandle("RefineryWnd.RefineResultAnimation03.RefineResult3"));
	m_DragboxItem1 = ItemWindowHandle(GetHandle("RefineryWnd.ItemDragBox1Wnd.ItemDragBox1"));
	m_DragBoxItem2 = ItemWindowHandle(GetHandle("RefineryWnd.ItemDragBox2Wnd.ItemDragBox2"));
	m_DragBoxItem3 = ItemWindowHandle(GetHandle("RefineryWnd.ItemDragBox3Wnd.ItemDragBox3"));
	m_ResultBoxItem = ItemWindowHandle(GetHandle("RefineryWnd.ItemDragBoxResultWnd.ItemRefined"));
	m_RefineryBtn = ButtonHandle(GetHandle("RefineryWnd.btnRefine"));
	m_OkBtn = ButtonHandle(GetHandle("RefineryWnd.btnClose"));
	m_InstructionText = TextBoxHandle(GetHandle("RefineryWnd.txtInstruction"));
	m_hGemstoneNameTextBox = TextBoxHandle(GetHandle("txtGemstoneName"));
	m_hGemstoneCountTextBox = TextBoxHandle(GetHandle("txtGemstoneCount"));
	m_RefineAnim.SetLoopCount(1);
	m_ResultAnim1.SetLoopCount(1);
	m_ResultAnim2.SetLoopCount(1);
	m_ResultAnim3.SetLoopCount(1);
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.SetProgressTime("RefineryWnd.RefineryProgress", 1900);
	InventoryWndScript = InventoryWnd(GetScript("InventoryWnd"));
	return;
}

function OnShow()
{
	ResetReady();
	InventoryWndScript.HandleOpenWindow();
	return;
}

function ResetReady()
{
	procedure1stat = false;
	procedure2stat = false;
	procedure3stat = false;
	procedure4stat = false;
	m_GemstoneName = "";
	m_RefineryWnd_Main.ShowWindow();
	m_RefineResultBackPattern.HideWindow();
	m_Highlight1.ShowWindow();
	m_Highlight2.HideWindow();
	m_Highlight3.HideWindow();
	m_SeletedItemHighlight1.HideWindow();
	m_SeletedItemHighlight2.HideWindow();
	m_SeletedItemHighlight3.HideWindow();
	m_DragBox1.ShowWindow();
	m_DragBox2.ShowWindow();
	m_DragBox3.ShowWindow();
	m_DragBoxResult.HideWindow();
	m_RefineAnimation.HideWindow();
	m_ResultAnimation1.HideWindow();
	m_ResultAnimation2.HideWindow();
	m_ResultAnimation3.HideWindow();
	m_RefineAnim.Stop();
	m_ResultAnim1.Stop();
	m_ResultAnim2.Stop();
	m_ResultAnim3.Stop();
	m_InstructionText.SetText(GetSystemMessage(1957));
	m_hGemstoneNameTextBox.SetText("");
	m_hGemstoneCountTextBox.SetText("");
	m_hGemstoneCountTextBox.SetTooltipString("");
	m_DragboxItem1.Clear();
	m_DragBoxItem2.Clear();
	m_DragBoxItem3.Clear();
	m_RefineryBtn.DisableWindow();
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.Reset("RefineryWnd.RefineryProgress");
	MoveItemBoxes(true);
	m_DragboxItem1.EnableWindow();
	m_DragBoxItem2.DisableWindow();
	m_DragBoxItem3.DisableWindow();
	PlaySound("ItemSound2.smelting.Smelting_dragin");
	m_OkBtn.EnableWindow();
	return;
}

function OnRefineryConfirmTargetItemResult()
{
	m_RefineResultBackPattern.HideWindow();
	m_Highlight1.HideWindow();
	m_Highlight2.ShowWindow();
	m_Highlight3.HideWindow();
	m_SeletedItemHighlight1.ShowWindow();
	m_SeletedItemHighlight2.HideWindow();
	m_SeletedItemHighlight3.HideWindow();
	m_DragBox1.ShowWindow();
	m_DragBox2.ShowWindow();
	m_DragBox3.ShowWindow();
	m_DragBoxResult.HideWindow();
	m_RefineAnimation.HideWindow();
	m_ResultAnimation1.HideWindow();
	m_ResultAnimation2.HideWindow();
	m_ResultAnimation3.HideWindow();
	procedure1stat = true;
	procedure2stat = false;
	procedure3stat = false;
	procedure4stat = false;
	m_InstructionText.SetText(GetSystemMessage(1958));
	m_hGemstoneNameTextBox.SetText("");
	m_hGemstoneCountTextBox.SetText("");
	m_hGemstoneCountTextBox.SetTooltipString("");
	m_DragboxItem1.EnableWindow();
	m_DragBoxItem2.EnableWindow();
	m_DragBoxItem3.DisableWindow();
	m_RefineryBtn.DisableWindow();
	return;
}

function OnRefineryConfirmRefinerItemResult()
{
	local string GemstoneName, Instruction;

	m_RefineResultBackPattern.HideWindow();
	m_Highlight1.HideWindow();
	m_Highlight2.HideWindow();
	m_Highlight3.ShowWindow();
	m_SeletedItemHighlight1.ShowWindow();
	m_SeletedItemHighlight2.ShowWindow();
	m_SeletedItemHighlight3.HideWindow();
	m_DragBox1.ShowWindow();
	m_DragBox2.ShowWindow();
	m_DragBox3.ShowWindow();
	m_DragBoxResult.HideWindow();
	m_RefineAnimation.HideWindow();
	m_ResultAnimation1.HideWindow();
	m_ResultAnimation2.HideWindow();
	m_ResultAnimation3.HideWindow();
	procedure1stat = true;
	procedure2stat = true;
	procedure3stat = false;
	procedure4stat = false;
	GemstoneName = Class'NWindow.UIDATA_ITEM'.static.GetItemName(m_GemStoneClassID);
	m_GemstoneName = GemstoneName;
	Instruction = MakeFullSystemMsg(GetSystemMessage(1959), GemstoneName, string(m_NecessaryGemstoneCount));
	m_InstructionText.SetText(Instruction);
	m_hGemstoneNameTextBox.SetText(GemstoneName);
	m_hGemstoneCountTextBox.SetText(MakeCostString(string(m_NecessaryGemstoneCount)));
	m_hGemstoneCountTextBox.SetTooltipString(ConvertNumToTextNoAdena(string(m_NecessaryGemstoneCount)));
	m_DragboxItem1.EnableWindow();
	m_DragBoxItem2.EnableWindow();
	m_DragBoxItem3.EnableWindow();
	m_RefineryBtn.DisableWindow();
	return;
}

function OnRefineryConfirmGemStoneResult()
{
	m_RefineResultBackPattern.HideWindow();
	m_Highlight1.HideWindow();
	m_Highlight2.HideWindow();
	m_Highlight3.HideWindow();
	m_SeletedItemHighlight1.ShowWindow();
	m_SeletedItemHighlight2.ShowWindow();
	m_SeletedItemHighlight3.ShowWindow();
	m_DragBox1.ShowWindow();
	m_DragBox2.ShowWindow();
	m_DragBox3.ShowWindow();
	m_DragBoxResult.HideWindow();
	m_RefineAnimation.HideWindow();
	m_ResultAnimation1.HideWindow();
	m_ResultAnimation2.HideWindow();
	m_ResultAnimation3.HideWindow();
	procedure1stat = true;
	procedure2stat = true;
	procedure3stat = true;
	procedure4stat = false;
	m_InstructionText.SetText(GetSystemMessage(1984));
	m_hGemstoneNameTextBox.SetText("");
	m_hGemstoneCountTextBox.SetText("");
	m_hGemstoneCountTextBox.SetTooltipString("");
	m_RefineryBtn.EnableWindow();
	m_hGemstoneCountTextBox.SetTooltipString("");
	return;
}

function OnRefineryRefineResult()
{
	m_RefineResultBackPattern.HideWindow();
	m_Highlight1.HideWindow();
	m_Highlight2.HideWindow();
	m_Highlight3.HideWindow();
	m_DragBox1.HideWindow();
	m_DragBox2.HideWindow();
	m_DragBox3.HideWindow();
	MoveItemBoxes(true);
	m_DragBoxResult.ShowWindow();
	m_RefineAnimation.HideWindow();
	m_ResultAnimation1.HideWindow();
	m_ResultAnimation2.HideWindow();
	m_ResultAnimation3.HideWindow();
	procedure1stat = true;
	procedure2stat = true;
	procedure3stat = true;
	procedure4stat = true;
	m_InstructionText.SetText(GetSystemMessage(1962));
	m_hGemstoneNameTextBox.SetText("");
	m_hGemstoneCountTextBox.SetText("");
	m_hGemstoneCountTextBox.SetTooltipString("");
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x24
		case 2760:
			// End:0x21
			if(__NFUN_242__(procedure1stat, false))
			{
				ShowRefineryInterface();
			}
			// End:0x114
			break;
		// End:0x65
		case 2770:
			PlaySound("ItemSound2.smelting.Smelting_dragin");
			OnTargetItemValidationResult(a_Param);
			// End:0x114
			break;
		// End:0xA6
		case 2780:
			PlaySound("ItemSound2.smelting.Smelting_dragin");
			OnRefinerItemValidationResult(a_Param);
			// End:0x114
			break;
		// End:0xE7
		case 2790:
			PlaySound("ItemSound2.smelting.Smelting_dragin");
			OnGemstoneValidationResult(a_Param);
			// End:0x114
			break;
		// End:0xFD
		case 2800:
			OnRefineDoneResult(a_Param);
			// End:0x114
			break;
		// End:0x10E
		case 1710:
			HandleDialogOK();
			// End:0x114
			break;
		// End:0xFFFF
		default:
			// End:0x114
			break;
			break;
	}
	return;
}

function ShowRefineryInterface()
{
	ResetReady();
	return;
}

function OnDropItem(string a_WindowID, ItemInfo a_ItemInfo, int X, int Y)
{
	switch(a_WindowID)
	{
		// End:0x97
		case "ItemDragBox1":
			Debug(__NFUN_168__(__NFUN_168__(__NFUN_168__("µå·¡±× ¹Ú½º 1¿¡ ¾ÆÀÌÅÛ ¿Ã·Á ³õ¾ÒÀ½.", string(procedure1stat)), string(procedure2stat)), string(procedure3stat)));
			// End:0x94
			if(__NFUN_130__(__NFUN_130__(__NFUN_242__(procedure1stat, false), __NFUN_242__(procedure2stat, false)), __NFUN_242__(procedure3stat, false)))
			{
				ValidateFirstItem(a_ItemInfo);
			}
			// End:0x1BA
			break;
		// End:0x127
		case "ItemDragBox2":
			Debug(__NFUN_168__(__NFUN_168__(__NFUN_168__("µå·¡±× ¹Ú½º 2¿¡ ¾ÆÀÌÅÛ ¿Ã·Á ³õ¾ÒÀ½.", string(procedure1stat)), string(procedure2stat)), string(procedure3stat)));
			// End:0x124
			if(__NFUN_130__(__NFUN_130__(__NFUN_242__(procedure1stat, true), __NFUN_242__(procedure2stat, false)), __NFUN_242__(procedure3stat, false)))
			{
				ValidateSecondItem(a_ItemInfo);
			}
			// End:0x1BA
			break;
		// End:0x1B7
		case "ItemDragBox3":
			Debug(__NFUN_168__(__NFUN_168__(__NFUN_168__("µå·¡±× ¹Ú½º 3¿¡ ¾ÆÀÌÅÛ ¿Ã·Á ³õ¾ÒÀ½.", string(procedure1stat)), string(procedure2stat)), string(procedure3stat)));
			// End:0x1B4
			if(__NFUN_130__(__NFUN_130__(__NFUN_242__(procedure1stat, true), __NFUN_242__(procedure2stat, true)), __NFUN_242__(procedure3stat, false)))
			{
				ValidateGemstoneItem(a_ItemInfo);
			}
			// End:0x1BA
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ValidateFirstItem(ItemInfo a_ItemInfo)
{
	RefineItemInfo = a_ItemInfo;
	m_TargetItemServerID = a_ItemInfo.ServerID;
	Class'NWindow.RefineryAPI'.static.ConfirmTargetItem(m_TargetItemServerID);
	return;
}

function OnTargetItemValidationResult(string a_Param)
{
	local int Item1ServerID, Item1ClassID, ItemValidationResult1;

	ParseInt(a_Param, "TargetItemServerID", Item1ServerID);
	ParseInt(a_Param, "TargetItemClassID", Item1ClassID);
	ParseInt(a_Param, "Result", ItemValidationResult1);
	switch(ItemValidationResult1)
	{
		// End:0xB5
		case 1:
			// End:0xB2
			if(__NFUN_154__(Item1ServerID, RefineItemInfo.ServerID))
			{
				// End:0xAC
				if(__NFUN_129__(m_DragboxItem1.SetItem(0, RefineItemInfo)))
				{
					m_DragboxItem1.AddItem(RefineItemInfo);
				}
				OnRefineryConfirmTargetItemResult();
			}
			// End:0xBF
			break;
		// End:0xBC
		case 0:
			// End:0xBF
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ValidateSecondItem(ItemInfo a_ItemInfo)
{
	RefinerItemInfo = a_ItemInfo;
	m_RefineItemServerID = a_ItemInfo.ServerID;
	Class'NWindow.RefineryAPI'.static.ConfirmRefinerItem(m_TargetItemServerID, m_RefineItemServerID);
	return;
}

function OnRefinerItemValidationResult(string a_Param)
{
	local int Item2ServerID, Item2ClassID, ItemValidationResult2, RequiredGemstoneAmount, RequiredGemstoneClassID;

	Debug("µÎ¹øÂ° ÀÌº¥Æ® ¹ÞÀ½:Á¦·ÃÁ¦");
	ParseInt(a_Param, "RefinerItemServerID", Item2ServerID);
	ParseInt(a_Param, "RefinerItemClassID", Item2ClassID);
	ParseInt(a_Param, "Result", ItemValidationResult2);
	ParseInt(a_Param, "GemStoneCount", RequiredGemstoneAmount);
	ParseInt(a_Param, "GemStoneClassID", RequiredGemstoneClassID);
	m_GemStoneClassID = RequiredGemstoneClassID;
	m_NecessaryGemstoneCount = RequiredGemstoneAmount;
	switch(ItemValidationResult2)
	{
		// End:0x12E
		case 1:
			// End:0x12B
			if(__NFUN_154__(Item2ServerID, RefinerItemInfo.ServerID))
			{
				// End:0x125
				if(__NFUN_129__(m_DragBoxItem2.SetItem(0, RefinerItemInfo)))
				{
					m_DragBoxItem2.AddItem(RefinerItemInfo);
				}
				OnRefineryConfirmRefinerItemResult();
			}
			// End:0x138
			break;
		// End:0x135
		case 0:
			// End:0x138
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ValidateGemstoneItem(ItemInfo a_ItemInfo)
{
	GemstoneItemInfo = a_ItemInfo;
	m_GemStoneServerID = a_ItemInfo.ServerID;
	// End:0x61
	if(__NFUN_151__(a_ItemInfo.AllItemCount, 0))
	{
		m_GemstoneCount = a_ItemInfo.AllItemCount;
		Class'NWindow.RefineryAPI'.static.ConfirmGemStone(m_TargetItemServerID, m_RefineItemServerID, m_GemStoneServerID, m_GemstoneCount);		
	}
	else
	{
		DialogSetID(0);
		DialogSetParamInt(a_ItemInfo.ItemNum);
		DialogShow(6, MakeFullSystemMsg(GetSystemMessage(72), a_ItemInfo.Name, ""));
	}
	return;
}

function OnGemstoneValidationResult(string a_Param)
{
	local int Item3ServerID, Item3ClassID, ItemValidationResult3, RequiredMoreGemstoneAmount, GemstoneAmountChecked;

	Debug("¼¼¹øÂ° ÀÌº¥Æ® ¹ÞÀ½:Áª½ºÅæ");
	ParseInt(a_Param, "GemStoneServerID", Item3ServerID);
	ParseInt(a_Param, "GemStoneClassID", Item3ClassID);
	ParseInt(a_Param, "Result", ItemValidationResult3);
	ParseInt(a_Param, "NecessaryGemStoneCount", RequiredMoreGemstoneAmount);
	ParseInt(a_Param, "GemStoneCount", GemstoneAmountChecked);
	m_GemStoneClassID = Item3ClassID;
	m_NecessaryGemstoneCount = GemstoneAmountChecked;
	switch(ItemValidationResult3)
	{
		// End:0x13F
		case 1:
			// End:0x13C
			if(__NFUN_154__(Item3ServerID, GemstoneItemInfo.ServerID))
			{
				// End:0x136
				if(__NFUN_129__(m_DragBoxItem3.SetItem(0, GemstoneItemInfo)))
				{
					GemstoneItemInfo.ItemNum = GemstoneAmountChecked;
					m_DragBoxItem3.AddItem(GemstoneItemInfo);
				}
				OnRefineryConfirmGemStoneResult();
			}
			// End:0x149
			break;
		// End:0x146
		case 0:
			// End:0x149
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x5E
		case "btnRefine":
			Debug("Button ´­·ÈÀ½");
			PlaySound("Itemsound2.smelting.smelting_loding");
			OnClickRefineButton();
			// End:0x77
			break;
		// End:0x74
		case "btnClose":
			OnClickCancelButton();
			// End:0x77
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickCancelButton()
{
	m_RefineryWnd_Main.HideWindow();
	PlaySound("Itemsound2.smelting.smelting_dragout");
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.Stop("RefineryWnd.RefineryProgress");
	m_RefineAnim.Stop();
	m_RefineAnim.SetLoopCount(1);
	procedure1stat = false;
	procedure2stat = false;
	procedure3stat = false;
	procedure4stat = false;
	return;
}

function OnClickRefineButton()
{
	m_RefineResultBackPattern.HideWindow();
	m_Highlight1.HideWindow();
	m_Highlight2.HideWindow();
	m_Highlight3.HideWindow();
	m_DragBoxResult.HideWindow();
	m_RefineAnimation.ShowWindow();
	m_ResultAnimation1.HideWindow();
	m_ResultAnimation2.HideWindow();
	m_ResultAnimation3.HideWindow();
	m_RefineryBtn.DisableWindow();
	m_OkBtn.DisableWindow();
	procedure1stat = true;
	procedure2stat = true;
	procedure3stat = true;
	procedure4stat = true;
	PlayRefineAnimation();
	MoveItemBoxes(false);
	return;
}

function PlayRefineAnimation()
{
	m_InstructionText.SetText("");
	m_RefineAnim.Stop();
	m_RefineAnim.SetLoopCount(1);
	m_RefineAnim.Play();
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.Start("RefineryWnd.RefineryProgress");
	return;
}

function OnTextureAnimEnd(AnimTextureHandle a_WindowHandle)
{
	switch(a_WindowHandle)
	{
		// End:0x54
		case m_RefineAnim:
			m_RefineAnimation.HideWindow();
			m_DragBox1.HideWindow();
			m_DragBox2.HideWindow();
			m_DragBox3.HideWindow();
			OnRefineRequest();
			// End:0x78
			break;
		// End:0x5C
		case m_ResultAnim1:
		// End:0x64
		case m_ResultAnim2:
		// End:0x75
		case m_ResultAnim3:
			OnResultAnimEnd();
			// End:0x78
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnResultAnimEnd()
{
	m_ResultAnimation1.HideWindow();
	m_ResultAnimation2.HideWindow();
	m_ResultAnimation3.HideWindow();
	return;
}

function OnRefineRequest()
{
	Class'NWindow.RefineryAPI'.static.RequestRefine(m_TargetItemServerID, m_RefineItemServerID, m_GemStoneServerID, m_NecessaryGemstoneCount);
	return;
}

function OnRefineDoneResult(string a_Param)
{
	local int Option1, Option2, RefineResult, Quality;

	Debug("Á¦·Ã¿Ï·á: °á°ú È®ÀÎ");
	ParseInt(a_Param, "Option1", Option1);
	ParseInt(a_Param, "Option2", Option2);
	ParseInt(a_Param, "Result", RefineResult);
	m_OkBtn.EnableWindow();
	switch(RefineResult)
	{
		// End:0x159
		case 1:
			RefineItemInfo.RefineryOp1 = Option1;
			RefineItemInfo.RefineryOp2 = Option2;
			// End:0xCD
			if(__NFUN_129__(m_ResultBoxItem.SetItem(0, RefineItemInfo)))
			{
				m_ResultBoxItem.AddItem(RefineItemInfo);
			}
			OnRefineryRefineResult();
			Quality = Class'NWindow.UIDATA_REFINERYOPTION'.static.GetQuality(Option2);
			// End:0x102
			if(__NFUN_153__(0, Quality))
			{
				Quality = 1;				
			}
			else
			{
				// End:0x116
				if(__NFUN_150__(4, Quality))
				{
					Quality = 4;
				}
			}
			m_RefineResultBackPattern.ShowWindow();
			m_RefineResultBackPattern.SetAlpha(0);
			m_RefineResultBackPattern.SetAlpha(255, 1.0000000);
			PlayResultAnimation(Quality);
			// End:0x169
			break;
		// End:0x166
		case 0:
			OnClickCancelButton();
			// End:0x169
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleDialogOK()
{
	local int Id;

	// End:0x57
	if(DialogIsMine())
	{
		Id = DialogGetID();
		switch(Id)
		{
			// End:0x54
			case 0:
				m_GemstoneCount = int(DialogGetString());
				Class'NWindow.RefineryAPI'.static.ConfirmGemStone(m_TargetItemServerID, m_RefineItemServerID, m_GemStoneServerID, m_GemstoneCount);
				// End:0x57
				break;
			// End:0xFFFF
			default:
				break;
		}
	}
	return;
}

function PlayResultAnimation(int grade)
{
	m_ResultAnim1.SetLoopCount(1);
	m_ResultAnim2.SetLoopCount(1);
	m_ResultAnim3.SetLoopCount(1);
	switch(grade)
	{
		// End:0x87
		case 1:
			m_ResultAnimation1.ShowWindow();
			PlaySound("ItemSound2.smelting.smelting_finalB");
			m_ResultAnim1.Play();
			// End:0x1B9
			break;
		// End:0xD8
		case 2:
			m_ResultAnimation2.ShowWindow();
			PlaySound("ItemSound2.smelting.smelting_finalC");
			m_ResultAnim2.Play();
			// End:0x1B9
			break;
		// End:0x129
		case 3:
			m_ResultAnimation3.ShowWindow();
			PlaySound("ItemSound2.smelting.smelting_finalD");
			m_ResultAnim3.Play();
			// End:0x1B9
			break;
		// End:0x1B6
		case 4:
			m_ResultAnimation1.ShowWindow();
			m_ResultAnimation2.ShowWindow();
			m_ResultAnimation3.ShowWindow();
			PlaySound("ItemSound2.smelting.smelting_finalD");
			m_ResultAnim1.Play();
			m_ResultAnim2.Play();
			m_ResultAnim3.Play();
			// End:0x1B9
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function MoveItemBoxes(bool a_Origin)
{
	local Rect Item1Rect, Item2Rect, Item3Rect, ResultRect;

	// End:0xCF
	if(a_Origin)
	{
		m_DragBox1.SetAnchor("RefineryWnd", "TopLeft", "TopLeft", 77, 51);
		m_DragBox1.ClearAnchor();
		m_DragBox2.SetAnchor("RefineryWnd", "TopLeft", "TopLeft", 157, 51);
		m_DragBox2.ClearAnchor();
		m_DragBox3.SetAnchor("RefineryWnd", "TopLeft", "TopLeft", 117, 91);
		m_DragBox3.ClearAnchor();		
	}
	else
	{
		Item1Rect = m_DragBox1.GetRect();
		Item2Rect = m_DragBox2.GetRect();
		Item3Rect = m_DragBox3.GetRect();
		ResultRect = m_DragBoxResult.GetRect();
		m_DragBox1.Move(__NFUN_147__(ResultRect.nX, Item1Rect.nX), __NFUN_147__(ResultRect.nY, Item1Rect.nY), 1.5000000);
		m_DragBox2.Move(__NFUN_147__(ResultRect.nX, Item2Rect.nX), __NFUN_147__(ResultRect.nY, Item2Rect.nY), 1.5000000);
		m_DragBox3.Move(__NFUN_147__(ResultRect.nX, Item3Rect.nX), __NFUN_147__(ResultRect.nY, Item3Rect.nY), 1.5000000);
	}
	return;
}
