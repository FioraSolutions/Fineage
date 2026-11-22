class QuestTreeWnd extends UICommonAPI;

const QUESTTREEWND_MAX_COUNT = 25;

var string m_WindowName;
var int m_QuestNum;
var int m_OldQuestID;
var string m_CurNodeName;
var int m_DeleteQuestID;
var string m_DeleteNodeName;
var array<string> m_arrItemNodeName;
var array<string> m_arrItemString;
var array<int> m_arrItemClassID;
var TextureHandle m_QuestTooltip;

function OnLoad()
{
	RegisterEvent(700);
	RegisterEvent(710);
	RegisterEvent(720);
	RegisterEvent(730);
	RegisterEvent(1710);
	RegisterEvent(2600);
	RegisterEvent(2610);
	m_QuestTooltip = TextureHandle(GetHandle(__NFUN_112__(m_WindowName, ".QuestToolTip")));
	InitQuestTooltip();
	m_QuestNum = 0;
	return;
}

function OnShow()
{
	ShowQuestList();
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1D
		case "btnClose":
			HandleQuestCancel();
			// End:0x20
			break;
		// End:0xFFFF
		default:
			break;
	}
	// End:0x3A
	if(__NFUN_122__(__NFUN_128__(strID, 4), "root"))
	{
		UpdateTargetInfo();
	}
	return;
}

function OnClickCheckBox(string strID)
{
	switch(strID)
	{
		// End:0x21
		case "chkNpcPosBox":
			UpdateTargetInfo();
			// End:0x24
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function Clear()
{
	m_QuestNum = 0;
	UpdateQuestCount();
	m_OldQuestID = -1;
	m_CurNodeName = "";
	m_DeleteQuestID = 0;
	m_DeleteNodeName = "";
	m_arrItemNodeName.Remove(0, m_arrItemNodeName.Length);
	m_arrItemString.Remove(0, m_arrItemString.Length);
	m_arrItemClassID.Remove(0, m_arrItemClassID.Length);
	Class'NWindow.UIAPI_TREECTRL'.static.Clear(__NFUN_112__(m_WindowName, ".MainTree"));
	return;
}

function ShowQuestList()
{
	Class'NWindow.QuestAPI'.static.RequestQuestList();
	return;
}

function InitTree()
{
	local XMLTreeNodeInfo infNode;
	local string strRetName;

	Clear();
	infNode.strName = "root";
	infNode.nOffSetX = 3;
	infNode.nOffSetY = 5;
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode(__NFUN_112__(m_WindowName, ".MainTree"), "", infNode);
	// End:0xA3
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		__NFUN_231__(__NFUN_112__("ERROR: Can't insert root node. Name: ", infNode.strName));
		return;
	}
	return;
}

function HandleQuestListStart()
{
	InitTree();
	return;
}

function HandleQuestList(string a_Param)
{
	local int QuestID, Level, Completed;

	ParseInt(a_Param, "QuestID", QuestID);
	ParseInt(a_Param, "Level", Level);
	ParseInt(a_Param, "Completed", Completed);
	// End:0x7B
	if(__NFUN_155__(m_OldQuestID, QuestID))
	{
		__NFUN_165__(m_QuestNum);
		AddQuestInfo("", QuestID, Level, Completed);		
	}
	else
	{
		AddQuestInfo(m_CurNodeName, QuestID, Level, Completed);
	}
	m_OldQuestID = QuestID;
	return;
}

function HandleQuestListEnd()
{
	UpdateQuestCount();
	UpdateItemCount(0);
	return;
}

function OnEvent(int Event_ID, string param)
{
	local int ClassID;

	// End:0x18
	if(__NFUN_154__(Event_ID, 700))
	{
		HandleQuestListStart();		
	}
	else
	{
		// End:0x35
		if(__NFUN_154__(Event_ID, 710))
		{
			HandleQuestList(param);			
		}
		else
		{
			// End:0x4D
			if(__NFUN_154__(Event_ID, 720))
			{
				HandleQuestListEnd();				
			}
			else
			{
				// End:0x6A
				if(__NFUN_154__(Event_ID, 730))
				{
					HandleQuestSetCurrentID(param);					
				}
				else
				{
					// End:0xB1
					if(__NFUN_132__(__NFUN_154__(Event_ID, 2600), __NFUN_154__(Event_ID, 2610)))
					{
						ParseInt(param, "classID", ClassID);
						UpdateItemCount(ClassID);						
					}
					else
					{
						// End:0x127
						if(__NFUN_154__(Event_ID, 1710))
						{
							// End:0x127
							if(DialogIsMine())
							{
								// End:0x127
								if(__NFUN_154__(DialogGetID(), 0))
								{
									Class'NWindow.QuestAPI'.static.RequestDestroyQuest(m_DeleteQuestID);
									SetQuestOff();
									Class'NWindow.UIAPI_TREECTRL'.static.DeleteNode(__NFUN_112__(m_WindowName, ".MainTree"), m_DeleteNodeName);
									m_DeleteQuestID = 0;
									m_DeleteNodeName = "";									
								}
							}
						}
					}
				}
			}
		}
	}
	return;
}

function HandleQuestSetCurrentID(string param)
{
	local string strNodeName, strChildList;
	local int RecentlyAddedQuestID, SplitCount;
	local array<string> arrSplit;

	// End:0x20
	if(__NFUN_129__(ParseInt(param, "QuestID", RecentlyAddedQuestID)))
	{
		return;
	}
	// End:0xF0
	if(__NFUN_151__(RecentlyAddedQuestID, 0))
	{
		strNodeName = __NFUN_112__("root.", string(RecentlyAddedQuestID));
		Class'NWindow.UIAPI_TREECTRL'.static.SetExpandedNode(__NFUN_112__(m_WindowName, ".MainTree"), strNodeName, true);
		strChildList = Class'NWindow.UIAPI_TREECTRL'.static.GetChildNode(__NFUN_112__(m_WindowName, ".MainTree"), strNodeName);
		// End:0xEA
		if(__NFUN_151__(__NFUN_125__(strChildList), 0))
		{
			SplitCount = Split(strChildList, "|", arrSplit);
			Class'NWindow.UIAPI_TREECTRL'.static.SetExpandedNode(__NFUN_112__(m_WindowName, ".MainTree"), arrSplit[__NFUN_147__(SplitCount, 1)], true);
		}
		UpdateTargetInfo();
	}
	return;
}

function UpdateItemCount(int ClassID, optional int a_ItemCount)
{
	local int i, nPos, ItemCount;
	local string strTmp;

	i = 0;
	J0x07:

	// End:0x128 [Loop If]
	if(__NFUN_150__(i, m_arrItemClassID.Length))
	{
		// End:0x11E
		if(__NFUN_132__(__NFUN_154__(ClassID, 0), __NFUN_154__(ClassID, m_arrItemClassID[i])))
		{
			switch(a_ItemCount)
			{
				// End:0x52
				case -1:
					ItemCount = 0;
					// End:0x81
					break;
				// End:0x70
				case 0:
					ItemCount = GetInventoryItemCount(m_arrItemClassID[i]);
					// End:0x81
					break;
				// End:0xFFFF
				default:
					ItemCount = a_ItemCount;
					// End:0x81
					break;
					break;
			}
			nPos = __NFUN_126__(m_arrItemString[i], "%s");
			// End:0x11E
			if(__NFUN_151__(nPos, -1))
			{
				strTmp = __NFUN_112__(__NFUN_112__(__NFUN_128__(m_arrItemString[i], nPos), string(ItemCount)), __NFUN_127__(m_arrItemString[i], __NFUN_146__(nPos, 2)));
				Class'NWindow.UIAPI_TREECTRL'.static.SetNodeItemText(__NFUN_112__(m_WindowName, ".MainTree"), m_arrItemNodeName[i], __NFUN_147__(m_arrItemClassID.Length, i), strTmp);
			}
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function UpdateQuestCount()
{
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".txtQuestNum"), __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", string(m_QuestNum)), "/"), string(25)), ")"));
	return;
}

function UpdateTargetInfo()
{
	local int i;
	local array<string> arrSplit;
	local int SplitCount, QuestID, Level, Completed;
	local string strChildList, strTargetNode, strNodeName, strTargetName;
	local Vector vTargetPos;
	local bool bOnlyMinimap;

	// End:0x32
	if(__NFUN_129__(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked(__NFUN_112__(m_WindowName, ".chkNpcPosBox"))))
	{
		SetQuestOff();
		return;
	}
	strNodeName = GetExpandedNode();
	// End:0x53
	if(__NFUN_150__(__NFUN_125__(strNodeName), 1))
	{
		SetQuestOff();
		return;
	}
	strChildList = Class'NWindow.UIAPI_TREECTRL'.static.GetChildNode(__NFUN_112__(m_WindowName, ".MainTree"), strNodeName);
	// End:0xBC
	if(__NFUN_151__(__NFUN_125__(strChildList), 0))
	{
		SplitCount = Split(strChildList, "|", arrSplit);
		strTargetNode = arrSplit[__NFUN_147__(SplitCount, 1)];		
	}
	else
	{
		SetQuestOff();
		return;
	}
	arrSplit.Remove(0, arrSplit.Length);
	SplitCount = Split(strTargetNode, ".", arrSplit);
	i = 0;
	J0xF1:

	// End:0x16B [Loop If]
	if(__NFUN_150__(i, SplitCount))
	{
		switch(i)
		{
			// End:0x10E
			case 0:
				// End:0x161
				break;
			// End:0x128
			case 1:
				QuestID = int(arrSplit[i]);
				// End:0x161
				break;
			// End:0x143
			case 2:
				Level = int(arrSplit[i]);
				// End:0x161
				break;
			// End:0x15E
			case 2:
				Completed = int(arrSplit[i]);
				// End:0x161
				break;
			// End:0xFFFF
			default:
				break;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0xF1;
	}
	// End:0x252
	if(__NFUN_130__(__NFUN_151__(QuestID, 0), __NFUN_151__(Level, 0)))
	{
		strTargetName = Class'NWindow.UIDATA_QUEST'.static.GetTargetName(QuestID, Level);
		vTargetPos = Class'NWindow.UIDATA_QUEST'.static.GetTargetLoc(QuestID, Level);
		// End:0x24C
		if(__NFUN_130__(__NFUN_154__(Completed, 0), __NFUN_151__(__NFUN_125__(strTargetName), 0)))
		{
			bOnlyMinimap = Class'NWindow.UIDATA_QUEST'.static.IsMinimapOnly(QuestID, Level);
			// End:0x228
			if(bOnlyMinimap)
			{
				Class'NWindow.QuestAPI'.static.SetQuestTargetInfo(true, false, false, strTargetName, vTargetPos, QuestID);				
			}
			else
			{
				Class'NWindow.QuestAPI'.static.SetQuestTargetInfo(true, true, true, strTargetName, vTargetPos, QuestID);
			}			
		}
		else
		{
			SetQuestOff();
		}
	}
	return;
}

function SetQuestOff()
{
	local Vector vVector;

	Class'NWindow.QuestAPI'.static.SetQuestTargetInfo(false, false, false, "", vVector, 0);
	return;
}

function string GetExpandedNode()
{
	local array<string> arrSplit;
	local int SplitCount;
	local string strNodeName;

	strNodeName = Class'NWindow.UIAPI_TREECTRL'.static.GetExpandedNode(__NFUN_112__(m_WindowName, ".MainTree"), "root");
	SplitCount = Split(strNodeName, "|", arrSplit);
	// End:0x5E
	if(__NFUN_151__(SplitCount, 0))
	{
		strNodeName = arrSplit[0];
	}
	return strNodeName;
	return;
}

function HandleQuestCancel()
{
	local array<string> arrSplit;
	local int SplitCount;
	local string strNodeName;

	m_DeleteQuestID = 0;
	m_DeleteNodeName = "";
	strNodeName = GetExpandedNode();
	SplitCount = Split(strNodeName, "|", arrSplit);
	// End:0x97
	if(__NFUN_151__(SplitCount, 0))
	{
		strNodeName = arrSplit[0];
		arrSplit.Remove(0, arrSplit.Length);
		SplitCount = Split(strNodeName, ".", arrSplit);
		// End:0x97
		if(__NFUN_151__(SplitCount, 1))
		{
			m_DeleteQuestID = int(arrSplit[1]);
			m_DeleteNodeName = strNodeName;
		}
	}
	// End:0xC1
	if(__NFUN_150__(__NFUN_125__(m_DeleteNodeName), 1))
	{
		DialogShow(5, GetSystemMessage(1201));
		DialogSetID(1);		
	}
	else
	{
		DialogShow(4, GetSystemMessage(182));
		DialogSetID(0);
	}
	return;
}

function AddQuestInfo(string strParentName, int QuestID, int Level, int Completed)
{
	local XMLTreeNodeInfo infNode;
	local XMLTreeNodeItemInfo infNodeItem;
	local XMLTreeNodeInfo infNodeClear;
	local XMLTreeNodeItemInfo infNodeItemClear;
	local string strRetName, strTmp;
	local int QuestMaxLevel, QuestMinLevel, nQuestType;
	local string strTexture1, strTexture2;
	local int ItemCount;
	local array<int> arrItemIDList, arrItemNumList;
	local int i;
	local bool bShowCompletionItem, bShowCompletionJournal;

	bShowCompletionItem = Class'NWindow.UIDATA_QUEST'.static.IsShowableItemNumQuest(QuestID, Level);
	bShowCompletionJournal = Class'NWindow.UIDATA_QUEST'.static.IsShowableJournalQuest(QuestID, Level);
	// End:0x81F
	if(__NFUN_154__(Level, 1))
	{
		strTmp = Class'NWindow.UIDATA_QUEST'.static.GetQuestName(QuestID);
		infNode = infNodeClear;
		infNode.strName = __NFUN_112__("", string(QuestID));
		infNode.ToolTip = MakeTooltipSimpleText(strTmp);
		infNode.bFollowCursor = true;
		infNode.bShowButton = 1;
		infNode.nTexBtnWidth = 14;
		infNode.nTexBtnHeight = 14;
		infNode.strTexBtnExpand = "L2UI_CH3.QUESTWND.QuestWndPlusBtn";
		infNode.strTexBtnCollapse = "L2UI_CH3.QUESTWND.QuestWndMinusBtn";
		infNode.strTexBtnExpand_Over = "L2UI_CH3.QUESTWND.QuestWndPlusBtn_over";
		infNode.strTexBtnCollapse_Over = "L2UI_CH3.QUESTWND.QuestWndMinusBtn_over";
		infNode.nTexExpandedOffSetY = 1;
		infNode.nTexExpandedHeight = 13;
		infNode.nTexExpandedRightWidth = 32;
		infNode.nTexExpandedLeftUWidth = 16;
		infNode.nTexExpandedLeftUHeight = 13;
		infNode.nTexExpandedRightUWidth = 32;
		infNode.nTexExpandedRightUHeight = 13;
		infNode.strTexExpandedLeft = "L2UI_CH3.ListCtrl.TextSelect";
		infNode.strTexExpandedRight = "L2UI_CH3.ListCtrl.TextSelect2";
		strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode(__NFUN_112__(m_WindowName, ".MainTree"), "root", infNode);
		// End:0x2B1
		if(__NFUN_150__(__NFUN_125__(strRetName), 1))
		{
			__NFUN_231__(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
			return;
		}
		infNode.ToolTip.DrawList.Remove(0, infNode.ToolTip.DrawList.Length);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		infNodeItem.t_strText = strTmp;
		infNodeItem.nOffSetX = 5;
		infNodeItem.nOffSetY = 2;
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
		nQuestType = 0;
		nQuestType = Class'NWindow.UIDATA_QUEST'.static.GetQuestType(QuestID, Level);
		// End:0x3CB
		if(__NFUN_154__(nQuestType, 0))
		{
			strTexture1 = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_4";
			strTexture2 = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_1";			
		}
		else
		{
			// End:0x431
			if(__NFUN_154__(nQuestType, 1))
			{
				strTexture1 = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_4";
				strTexture2 = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_2";				
			}
			else
			{
				// End:0x498
				if(__NFUN_154__(nQuestType, 2))
				{
					strTexture1 = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_3";
					strTexture2 = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_1";					
				}
				else
				{
					// End:0x4FC
					if(__NFUN_154__(nQuestType, 3))
					{
						strTexture1 = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_3";
						strTexture2 = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_2";
					}
				}
			}
		}
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 2;
		infNodeItem.bStopMouseFocus = true;
		infNodeItem.nOffSetX = 5;
		infNodeItem.nOffSetY = 0;
		infNodeItem.u_nTextureWidth = 11;
		infNodeItem.u_nTextureHeight = 11;
		infNodeItem.u_strTexture = strTexture1;
		// End:0x59C
		if(__NFUN_151__(__NFUN_125__(strTexture1), 0))
		{
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
		}
		infNodeItem.nOffSetX = 0;
		infNodeItem.u_strTexture = strTexture2;
		// End:0x5F0
		if(__NFUN_151__(__NFUN_125__(strTexture2), 0))
		{
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
		}
		QuestMaxLevel = Class'NWindow.UIDATA_QUEST'.static.GetMaxLevel(QuestID, Level);
		QuestMinLevel = Class'NWindow.UIDATA_QUEST'.static.GetMinLevel(QuestID, Level);
		// End:0x680
		if(__NFUN_130__(__NFUN_151__(QuestMaxLevel, 0), __NFUN_151__(QuestMinLevel, 0)))
		{
			strTmp = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", GetSystemString(922)), ":"), string(QuestMinLevel)), "~"), string(QuestMaxLevel)), ")");			
		}
		else
		{
			// End:0x6C9
			if(__NFUN_151__(QuestMinLevel, 0))
			{
				strTmp = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", GetSystemString(922)), ":"), string(QuestMinLevel)), " "), GetSystemString(859)), ")");				
			}
			else
			{
				strTmp = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", GetSystemString(922)), ":"), GetSystemString(866)), ")");
			}
		}
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		infNodeItem.t_strText = strTmp;
		infNodeItem.t_color.R = 176;
		infNodeItem.t_color.G = 155;
		infNodeItem.t_color.B = 121;
		infNodeItem.t_color.A = byte(255);
		infNodeItem.bLineBreak = true;
		infNodeItem.nOffSetX = 22;
		infNodeItem.nOffSetY = 0;
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 0;
		infNodeItem.b_nHeight = 7;
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
		strParentName = strRetName;
		m_CurNodeName = strRetName;
	}
	strTmp = Class'NWindow.UIDATA_QUEST'.static.GetQuestJournalName(QuestID, Level);
	infNode = infNodeClear;
	infNode.strName = __NFUN_112__(__NFUN_112__(__NFUN_112__("", string(Level)), "."), string(Completed));
	infNode.ToolTip = MakeTooltipSimpleText(strTmp);
	infNode.bFollowCursor = true;
	infNode.nOffSetX = 7;
	infNode.bShowButton = 1;
	infNode.nTexBtnWidth = 14;
	infNode.nTexBtnHeight = 14;
	infNode.strTexBtnExpand = "L2UI_CH3.QUESTWND.QuestWndDownBtn";
	infNode.strTexBtnCollapse = "L2UI_CH3.QUESTWND.QuestWndUpBtn";
	infNode.strTexBtnExpand_Over = "L2UI_CH3.QUESTWND.QuestWndDownBtn_over";
	infNode.strTexBtnCollapse_Over = "L2UI_CH3.QUESTWND.QuestWndUpBtn_over";
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode(__NFUN_112__(m_WindowName, ".MainTree"), strParentName, infNode);
	// End:0x9F1
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		__NFUN_231__(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	infNode.ToolTip.DrawList.Remove(0, infNode.ToolTip.DrawList.Length);
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = strTmp;
	infNodeItem.nOffSetX = 5;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
	strTmp = Class'NWindow.UIDATA_QUEST'.static.GetTargetName(QuestID, Level);
	// End:0xB52
	if(__NFUN_151__(__NFUN_125__(strTmp), 0))
	{
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 2;
		infNodeItem.bStopMouseFocus = true;
		infNodeItem.nOffSetX = 5;
		infNodeItem.nOffSetY = 0;
		infNodeItem.u_nTextureWidth = 11;
		infNodeItem.u_nTextureHeight = 11;
		infNodeItem.u_strTexture = "L2UI_CH3.QUESTWND.QuestWndInfoIcon_5";
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
	}
	// End:0xC18
	if(__NFUN_130__(__NFUN_151__(Completed, 0), bShowCompletionJournal))
	{
		infNodeItem = infNodeItemClear;
		infNodeItem.eType = 1;
		infNodeItem.t_strText = GetSystemString(898);
		infNodeItem.t_color.R = 176;
		infNodeItem.t_color.G = 155;
		infNodeItem.t_color.B = 121;
		infNodeItem.t_color.A = byte(255);
		infNodeItem.nOffSetX = 5;
		Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 0;
	infNodeItem.b_nHeight = 5;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
	strTmp = Class'NWindow.UIDATA_QUEST'.static.GetQuestDescription(QuestID, Level);
	infNode = infNodeClear;
	infNode.strName = "desc";
	infNode.nOffSetX = 2;
	infNode.bDrawBackground = 1;
	infNode.bTexBackHighlight = 0;
	infNode.nTexBackWidth = 211;
	infNode.nTexBackUWidth = 211;
	infNode.nTexBackOffSetY = -2;
	infNode.nTexBackOffSetBottom = -2;
	strRetName = Class'NWindow.UIAPI_TREECTRL'.static.InsertNode(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNode);
	// End:0xD72
	if(__NFUN_150__(__NFUN_125__(strRetName), 1))
	{
		__NFUN_231__(__NFUN_112__("ERROR: Can't insert node. Name: ", infNode.strName));
		return;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 1;
	infNodeItem.t_strText = strTmp;
	infNodeItem.t_color.R = 140;
	infNodeItem.t_color.G = 140;
	infNodeItem.t_color.B = 140;
	infNodeItem.t_color.A = byte(255);
	infNodeItem.nOffSetX = 5;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
	strTmp = Class'NWindow.UIDATA_QUEST'.static.GetQuestItem(QuestID, Level);
	ParseInt(strTmp, "Max", ItemCount);
	arrItemIDList.Length = ItemCount;
	arrItemNumList.Length = ItemCount;
	i = 0;
	J0xE6F:

	// End:0xED9 [Loop If]
	if(__NFUN_150__(i, ItemCount))
	{
		ParseInt(strTmp, __NFUN_112__("ItemID_", string(i)), arrItemIDList[i]);
		ParseInt(strTmp, __NFUN_112__("ItemNum_", string(i)), arrItemNumList[i]);
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0xE6F;
	}
	i = 0;
	J0xEE0:

	// End:0x145E [Loop If]
	if(__NFUN_150__(i, ItemCount))
	{
		strTmp = Class'NWindow.UIDATA_ITEM'.static.GetItemTextureName(arrItemIDList[i]);
		// End:0x1454
		if(__NFUN_151__(__NFUN_125__(strTmp), 0))
		{
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 0;
			infNodeItem.b_nHeight = 4;
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 2;
			infNodeItem.nOffSetX = 4;
			infNodeItem.u_nTextureWidth = 34;
			infNodeItem.u_nTextureHeight = 34;
			infNodeItem.u_strTexture = "L2UI_CH3.Etc.menu_outline";
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 2;
			infNodeItem.nOffSetX = -33;
			infNodeItem.nOffSetY = 1;
			infNodeItem.u_nTextureWidth = 32;
			infNodeItem.u_nTextureHeight = 32;
			infNodeItem.u_strTexture = strTmp;
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
			strTmp = Class'NWindow.UIDATA_ITEM'.static.GetItemName(arrItemIDList[i]);
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 1;
			infNodeItem.t_strText = strTmp;
			infNodeItem.t_color.R = 176;
			infNodeItem.t_color.G = 155;
			infNodeItem.t_color.B = 121;
			infNodeItem.t_color.A = byte(255);
			infNodeItem.nOffSetX = 5;
			infNodeItem.nOffSetY = 1;
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
			infNodeItem = infNodeItemClear;
			infNodeItem.eType = 1;
			// End:0x1245
			if(__NFUN_151__(arrItemNumList[i], 0))
			{
				// End:0x11CC
				if(__NFUN_130__(__NFUN_151__(Completed, 0), bShowCompletionItem))
				{
					strTmp = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", GetSystemString(898)), "/"), string(arrItemNumList[i])), ")");					
				}
				else
				{
					strTmp = __NFUN_112__(__NFUN_112__("(%s/", string(arrItemNumList[i])), ")");
					m_arrItemNodeName.Insert(0, 1);
					m_arrItemNodeName[0] = strRetName;
					m_arrItemClassID.Insert(0, 1);
					m_arrItemClassID[0] = arrItemIDList[i];
					m_arrItemString.Insert(0, 1);
					m_arrItemString[0] = strTmp;
					infNodeItem.t_nTextID = m_arrItemClassID.Length;
				}				
			}
			else
			{
				// End:0x1313
				if(__NFUN_154__(arrItemNumList[i], 0))
				{
					// End:0x129C
					if(__NFUN_130__(__NFUN_151__(Completed, 0), bShowCompletionItem))
					{
						strTmp = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", GetSystemString(898)), "/"), GetSystemString(858)), ")");						
					}
					else
					{
						strTmp = __NFUN_112__(__NFUN_112__("(%s/", GetSystemString(858)), ")");
						m_arrItemNodeName.Insert(0, 1);
						m_arrItemNodeName[0] = strRetName;
						m_arrItemClassID.Insert(0, 1);
						m_arrItemClassID[0] = arrItemIDList[i];
						m_arrItemString.Insert(0, 1);
						m_arrItemString[0] = strTmp;
						infNodeItem.t_nTextID = m_arrItemClassID.Length;
					}					
				}
				else
				{
					// End:0x136A
					if(__NFUN_130__(__NFUN_151__(Completed, 0), bShowCompletionItem))
					{
						strTmp = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", GetSystemString(898)), "/"), string(__NFUN_143__(arrItemNumList[i]))), GetSystemString(859)), ")");						
					}
					else
					{
						strTmp = __NFUN_112__(__NFUN_112__(__NFUN_112__("(%s/", string(__NFUN_143__(arrItemNumList[i]))), GetSystemString(859)), ")");
						m_arrItemNodeName.Insert(0, 1);
						m_arrItemNodeName[0] = strRetName;
						m_arrItemClassID.Insert(0, 1);
						m_arrItemClassID[0] = arrItemIDList[i];
						m_arrItemString.Insert(0, 1);
						m_arrItemString[0] = strTmp;
						infNodeItem.t_nTextID = m_arrItemClassID.Length;
					}
				}
			}
			infNodeItem.t_strText = strTmp;
			infNodeItem.bLineBreak = true;
			infNodeItem.nOffSetX = 42;
			infNodeItem.nOffSetY = -16;
			Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0xEE0;
	}
	infNodeItem = infNodeItemClear;
	infNodeItem.eType = 0;
	infNodeItem.b_nHeight = 9;
	Class'NWindow.UIAPI_TREECTRL'.static.InsertNodeItem(__NFUN_112__(m_WindowName, ".MainTree"), strRetName, infNodeItem);
	return;
}

function InitQuestTooltip()
{
	local CustomTooltip TooltipInfo;

	TooltipInfo.DrawList.Length = 10;
	TooltipInfo.DrawList[0].eType = 2;
	TooltipInfo.DrawList[0].u_nTextureWidth = 16;
	TooltipInfo.DrawList[0].u_nTextureHeight = 16;
	TooltipInfo.DrawList[0].u_strTexture = "L2UI_CH3.QuestWnd.QuestWndInfoIcon_1";
	TooltipInfo.DrawList[1].eType = 1;
	TooltipInfo.DrawList[1].nOffSetX = 5;
	TooltipInfo.DrawList[1].t_bDrawOneLine = true;
	TooltipInfo.DrawList[1].t_strText = GetSystemString(861);
	TooltipInfo.DrawList[2].eType = 2;
	TooltipInfo.DrawList[2].nOffSetY = 2;
	TooltipInfo.DrawList[2].u_nTextureWidth = 16;
	TooltipInfo.DrawList[2].u_nTextureHeight = 16;
	TooltipInfo.DrawList[2].u_strTexture = "L2UI_CH3.QuestWnd.QuestWndInfoIcon_2";
	TooltipInfo.DrawList[2].bLineBreak = true;
	TooltipInfo.DrawList[3].eType = 1;
	TooltipInfo.DrawList[3].nOffSetY = 2;
	TooltipInfo.DrawList[3].nOffSetX = 5;
	TooltipInfo.DrawList[3].t_bDrawOneLine = true;
	TooltipInfo.DrawList[3].t_strText = GetSystemString(862);
	TooltipInfo.DrawList[4].eType = 2;
	TooltipInfo.DrawList[4].nOffSetY = 2;
	TooltipInfo.DrawList[4].u_nTextureWidth = 16;
	TooltipInfo.DrawList[4].u_nTextureHeight = 16;
	TooltipInfo.DrawList[4].u_strTexture = "L2UI_CH3.QuestWnd.QuestWndInfoIcon_3";
	TooltipInfo.DrawList[4].bLineBreak = true;
	TooltipInfo.DrawList[5].eType = 1;
	TooltipInfo.DrawList[5].nOffSetY = 2;
	TooltipInfo.DrawList[5].nOffSetX = 5;
	TooltipInfo.DrawList[5].t_bDrawOneLine = true;
	TooltipInfo.DrawList[5].t_strText = GetSystemString(863);
	TooltipInfo.DrawList[6].eType = 2;
	TooltipInfo.DrawList[6].nOffSetY = 2;
	TooltipInfo.DrawList[6].u_nTextureWidth = 16;
	TooltipInfo.DrawList[6].u_nTextureHeight = 16;
	TooltipInfo.DrawList[6].u_strTexture = "L2UI_CH3.QuestWnd.QuestWndInfoIcon_4";
	TooltipInfo.DrawList[6].bLineBreak = true;
	TooltipInfo.DrawList[7].eType = 1;
	TooltipInfo.DrawList[7].nOffSetY = 2;
	TooltipInfo.DrawList[7].nOffSetX = 5;
	TooltipInfo.DrawList[7].t_bDrawOneLine = true;
	TooltipInfo.DrawList[7].t_strText = GetSystemString(864);
	TooltipInfo.DrawList[8].eType = 2;
	TooltipInfo.DrawList[8].nOffSetY = 2;
	TooltipInfo.DrawList[8].u_nTextureWidth = 16;
	TooltipInfo.DrawList[8].u_nTextureHeight = 16;
	TooltipInfo.DrawList[8].u_strTexture = "L2UI_CH3.QuestWnd.QuestWndInfoIcon_5";
	TooltipInfo.DrawList[8].bLineBreak = true;
	TooltipInfo.DrawList[9].eType = 1;
	TooltipInfo.DrawList[9].nOffSetY = 2;
	TooltipInfo.DrawList[9].nOffSetX = 5;
	TooltipInfo.DrawList[9].t_bDrawOneLine = true;
	TooltipInfo.DrawList[9].t_strText = GetSystemString(865);
	m_QuestTooltip.SetTooltipCustomType(TooltipInfo);
	return;
}

defaultproperties
{
	m_WindowName="QuestTreeWnd"
}