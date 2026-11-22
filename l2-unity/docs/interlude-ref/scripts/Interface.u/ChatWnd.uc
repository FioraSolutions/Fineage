class ChatWnd extends UICommonAPI;

const CHAT_WINDOW_NORMAL = 0;
const CHAT_WINDOW_TRADE = 1;
const CHAT_WINDOW_PARTY = 2;
const CHAT_WINDOW_CLAN = 3;
const CHAT_WINDOW_ALLY = 4;
const CHAT_WINDOW_COUNT = 5;
const CHAT_WINDOW_SYSTEM = 5;

struct ChatFilterInfo
{
	var int bSystem;
	var int bChat;
	var int bDamage;
	var int bNormal;
	var int bShout;
	var int bClan;
	var int bParty;
	var int bTrade;
	var int bWhisper;
	var int bAlly;
	var int bUseitem;
	var int bHero;
	var int bUnion;
};

var int m_NoUnionCommanderMessage;
var array<ChatFilterInfo> m_filterInfo;
var array<string> m_sectionName;
var int m_chatType;
var ChatWindowHandle NormalChat;
var ChatWindowHandle TradeChat;
var ChatWindowHandle PartyChat;
var ChatWindowHandle ClanChat;
var ChatWindowHandle AllyChat;
var ChatWindowHandle SystemMsg;
var TabHandle ChatTabCtrl;
var EditBoxHandle ChatEditBox;

function OnLoad()
{
	m_filterInfo.Length = __NFUN_146__(5, 1);
	RegisterEvent(540);
	RegisterEvent(1500);
	RegisterEvent(550);
	RegisterEvent(560);
	RegisterEvent(570);
	RegisterEvent(571);
	RegisterEvent(572);
	m_sectionName.Length = 5;
	m_sectionName[0] = "entire_tab";
	m_sectionName[1] = "pledge_tab";
	m_sectionName[2] = "party_tab";
	m_sectionName[3] = "market_tab";
	m_sectionName[4] = "ally_tab";
	RegisterState("ChatWnd", "OlympiadObserverState");
	InitHandle();
	InitFilterInfo();
	InitGlobalSetting();
	InitScrollBarPosition();
	return;
}

function OnDefaultPosition()
{
	ChatTabCtrl.MergeTab(1);
	ChatTabCtrl.MergeTab(2);
	ChatTabCtrl.MergeTab(3);
	ChatTabCtrl.MergeTab(4);
	ChatTabCtrl.SetTopOrder(0, true);
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("ChatWnd", "", "BottomLeft", "BottomLeft", 0, 0);
	HandleTabClick("ChatTabCtrl0");
	return;
}

function InitGlobalSetting()
{
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxCommand", bool(m_NoUnionCommanderMessage));
	return;
}

function InitHandle()
{
	NormalChat = ChatWindowHandle(GetHandle("ChatWnd.NormalChat"));
	TradeChat = ChatWindowHandle(GetHandle("ChatWnd.TradeChat"));
	PartyChat = ChatWindowHandle(GetHandle("ChatWnd.PartyChat"));
	ClanChat = ChatWindowHandle(GetHandle("ChatWnd.ClanChat"));
	AllyChat = ChatWindowHandle(GetHandle("ChatWnd.AllyChat"));
	SystemMsg = ChatWindowHandle(GetHandle("SystemMsgWnd.SystemMsgList"));
	ChatTabCtrl = TabHandle(GetHandle("ChatWnd.ChatTabCtrl"));
	ChatEditBox = EditBoxHandle(GetHandle("ChatWnd.ChatEditBox"));
	return;
}

function InitScrollBarPosition()
{
	NormalChat.SetScrollBarPosition(5, 10, -25);
	TradeChat.SetScrollBarPosition(5, 10, -25);
	PartyChat.SetScrollBarPosition(5, 10, -25);
	ClanChat.SetScrollBarPosition(5, 10, -25);
	AllyChat.SetScrollBarPosition(5, 10, -25);
	return;
}

function OnCompleteEditBox(string strID)
{
	local string strInput;
	local UIEventManager.EChatType Type;

	// End:0xE1
	if(__NFUN_122__(strID, "ChatEditBox"))
	{
		strInput = ChatEditBox.GetString();
		// End:0x3B
		if(__NFUN_150__(__NFUN_125__(strInput), 1))
		{
			return;
		}
		ProcessChatMessage(strInput, m_chatType);
		ChatEditBox.SetString("");
		// End:0xB1
		if(__NFUN_242__(GetOptionBool("Game", "OldChatting"), true))
		{
			Type = GetChatTypeByTabIndex(m_chatType);
			// End:0xB1
			if(__NFUN_155__(m_chatType, 0))
			{
				ChatEditBox.AddString(GetChatPrefix(Type));
			}
		}
		// End:0xE1
		if(__NFUN_242__(GetOptionBool("Game", "EnterChatting"), true))
		{
			ChatEditBox.ReleaseFocus();
		}
	}
	return;
}

function Clear()
{
	ChatEditBox.super(ChatWnd).Clear();
	NormalChat.super(ChatWnd).Clear();
	PartyChat.super(ChatWnd).Clear();
	ClanChat.super(ChatWnd).Clear();
	TradeChat.super(ChatWnd).Clear();
	AllyChat.super(ChatWnd).Clear();
	SystemMsg.super(ChatWnd).Clear();
	return;
}

function OnShow()
{
	// End:0x34
	if(GetOptionBool("Game", "SystemMsgWnd"))
	{
		ShowWindow("SystemMsgWnd");		
	}
	else
	{
		HideWindow("SystemMsgWnd");
	}
	HandleIMEStatusChange();
	return;
}

function OnClickButton(string strID)
{
	local PartyMatchWnd script;

	script = PartyMatchWnd(GetScript("PartyMatchWnd"));
	switch(strID)
	{
		// End:0x38
		case "ChatTabCtrl0":
		// End:0x49
		case "ChatTabCtrl1":
		// End:0x5A
		case "ChatTabCtrl2":
		// End:0x6B
		case "ChatTabCtrl3":
		// End:0x8A
		case "ChatTabCtrl4":
			HandleTabClick(strID);
			// End:0x1A0
			break;
		// End:0x105
		case "ChatFilterBtn":
			// End:0xDE
			if(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("ChatFilterWnd"))
			{
				Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ChatFilterWnd");				
			}
			else
			{
				SetChatFilterButton();
				Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ChatFilterWnd");
			}
			// End:0x1A0
			break;
		// End:0x11F
		case "MessengerBtn":
			ToggleMsnWindow();
			// End:0x1A0
			break;
		// End:0x19A
		case "PartyMatchingBtn":
			// End:0x188
			if(__NFUN_242__(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("PartyMatchWnd"), true))
			{
				Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchWnd");
				script.OnSendPacketWhenHiding();				
			}
			else
			{
				Class'NWindow.PartyMatchAPI'.static.RequestOpenPartyMatch();
			}
			// End:0x1A0
			break;
		// End:0xFFFF
		default:
			// End:0x1A0
			break;
			break;
	}
	return;
}

function OnTabSplit(string sTabButton)
{
	local ChatWindowHandle Handle;

	switch(sTabButton)
	{
		// End:0x31
		case "ChatTabCtrl0":
			Handle = NormalChat;
			HandleTabClick(sTabButton);
			// End:0xDF
			break;
		// End:0x5B
		case "ChatTabCtrl1":
			Handle = TradeChat;
			HandleTabClick(sTabButton);
			// End:0xDF
			break;
		// End:0x85
		case "ChatTabCtrl2":
			Handle = PartyChat;
			HandleTabClick(sTabButton);
			// End:0xDF
			break;
		// End:0xAF
		case "ChatTabCtrl3":
			Handle = ClanChat;
			HandleTabClick(sTabButton);
			// End:0xDF
			break;
		// End:0xD9
		case "ChatTabCtrl4":
			Handle = AllyChat;
			HandleTabClick(sTabButton);
			// End:0xDF
			break;
		// End:0xFFFF
		default:
			// End:0xDF
			break;
			break;
	}
	// End:0x125
	if(__NFUN_119__(Handle, none))
	{
		Handle.SetWindowSizeRel(-1.0000000, -1.0000000, 0, 0);
		Handle.SetSettledWnd(true);
		Handle.EnableTexture(true);
	}
	return;
}

function OnTabMerge(string sTabButton)
{
	local ChatWindowHandle Handle;
	local int Width, Height;
	local Rect rectWnd;

	switch(sTabButton)
	{
		// End:0x26
		case "ChatTabCtrl0":
			Handle = NormalChat;
			// End:0xA8
			break;
		// End:0x45
		case "ChatTabCtrl1":
			Handle = TradeChat;
			// End:0xA8
			break;
		// End:0x64
		case "ChatTabCtrl2":
			Handle = PartyChat;
			// End:0xA8
			break;
		// End:0x83
		case "ChatTabCtrl3":
			Handle = ClanChat;
			// End:0xA8
			break;
		// End:0xA2
		case "ChatTabCtrl4":
			Handle = AllyChat;
			// End:0xA8
			break;
		// End:0xFFFF
		default:
			// End:0xA8
			break;
			break;
	}
	// End:0x160
	if(__NFUN_119__(Handle, none))
	{
		rectWnd = NormalChat.GetRect();
		NormalChat.GetWindowSize(Width, Height);
		Handle.SetSettledWnd(false);
		Handle.MoveTo(rectWnd.nX, rectWnd.nY);
		Handle.SetWindowSize(Width, __NFUN_147__(Height, 46));
		Handle.SetWindowSizeRel(1.0000000, 1.0000000, 0, -46);
		Handle.EnableTexture(false);
	}
	return;
}

function HandleTabClick(string strID)
{
	local string strInput, strPrefix;
	local int StrLen;

	m_chatType = ChatTabCtrl.GetTopIndex();
	SetChatFilterButton();
	// End:0x10D
	if(__NFUN_242__(GetOptionBool("Game", "OldChatting"), true))
	{
		strInput = ChatEditBox.GetString();
		StrLen = __NFUN_125__(strInput);
		strPrefix = __NFUN_128__(strInput, 1);
		// End:0xC5
		if(__NFUN_132__(__NFUN_132__(__NFUN_132__(IsSameChatPrefix(8, strPrefix), IsSameChatPrefix(3, strPrefix)), IsSameChatPrefix(4, strPrefix)), IsSameChatPrefix(9, strPrefix)))
		{
			strInput = __NFUN_234__(strInput, __NFUN_147__(StrLen, 1));
		}
		// End:0xF9
		if(__NFUN_155__(m_chatType, 0))
		{
			strPrefix = GetChatPrefix(GetChatTypeByTabIndex(m_chatType));
			strInput = __NFUN_112__(strPrefix, strInput);
		}
		ChatEditBox.SetString(strInput);
	}
	return;
}

function OnEnterState(name a_PrevStateName)
{
	// End:0x15
	if(__NFUN_254__(a_PrevStateName, 'LoadingState'))
	{
		Clear();
	}
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x1A
		case 540:
			HandleChatmessage(param);
		// End:0x2B
		case 1500:
			HandleIMEStatusChange();
			// End:0x95
			break;
		// End:0x3C
		case 550:
			HandleChatWndStatusChange();
			// End:0x95
			break;
		// End:0x4D
		case 570:
			HandleSetFocus();
			// End:0x95
			break;
		// End:0x63
		case 560:
			HandleSetString(param);
			// End:0x95
			break;
		// End:0x79
		case 571:
			HandleMsnStatus(param);
			// End:0x95
			break;
		// End:0x8F
		case 572:
			HandleChatWndMacroCommand(param);
			// End:0x95
			break;
		// End:0xFFFF
		default:
			// End:0x95
			break;
			break;
	}
	return;
}

function HandleChatWndMacroCommand(string param)
{
	local string Command;

	// End:0x20
	if(__NFUN_129__(ParseString(param, "Command", Command)))
	{
		return;
	}
	ProcessChatMessage(Command, m_chatType);
	return;
}

function HandleChatmessage(string param)
{
	local int nTmp;
	local UIEventManager.EChatType Type;
	local UIEventManager.ESystemMsgType systemType;
	local string Text;
	local Color Color;

	ParseInt(param, "Type", nTmp);
	Type = byte(nTmp);
	ParseString(param, "Msg", Text);
	ParseInt(param, "ColorR", nTmp);
	Color.R = byte(nTmp);
	ParseInt(param, "ColorG", nTmp);
	Color.G = byte(nTmp);
	ParseInt(param, "ColorB", nTmp);
	Color.B = byte(nTmp);
	Color.A = byte(255);
	// End:0xFE
	if(__NFUN_154__(int(Type), int(5)))
	{
		ParseInt(param, "SysType", nTmp);
		systemType = byte(nTmp);		
	}
	else
	{
		systemType = 0;
	}
	// End:0x133
	if(CheckFilter(Type, 0, systemType))
	{
		NormalChat.AddString(Text, Color);
	}
	// End:0x161
	if(CheckFilter(Type, 2, systemType))
	{
		PartyChat.AddString(Text, Color);
	}
	// End:0x18F
	if(CheckFilter(Type, 3, systemType))
	{
		ClanChat.AddString(Text, Color);
	}
	// End:0x1BC
	if(CheckFilter(Type, 1, systemType))
	{
		TradeChat.AddString(Text, Color);
	}
	// End:0x1EA
	if(CheckFilter(Type, 4, systemType))
	{
		AllyChat.AddString(Text, Color);
	}
	// End:0x218
	if(CheckFilter(Type, 5, systemType))
	{
		SystemMsg.AddString(Text, Color);
	}
	// End:0x240
	if(__NFUN_130__(__NFUN_154__(int(Type), int(15)), __NFUN_154__(m_NoUnionCommanderMessage, 0)))
	{
		ShowUnionCommanderMessgage(Text);
	}
	return;
}

function ShowUnionCommanderMessgage(string Msg)
{
	local string strParam, MsgTemp, MsgTemp2;
	local int MaxLength;

	MaxLength = __NFUN_125__(Msg);
	// End:0x55
	if(__NFUN_151__(MaxLength, 20))
	{
		MsgTemp = __NFUN_128__(Msg, 20);
		MsgTemp2 = __NFUN_234__(Msg, __NFUN_147__(MaxLength, 20));
		Msg = __NFUN_112__(__NFUN_112__(MsgTemp, "#"), MsgTemp2);
	}
	Debug(Msg);
	// End:0x17E
	if(__NFUN_151__(__NFUN_125__(Msg), 0))
	{
		ParamAdd(strParam, "MsgType", string(1));
		ParamAdd(strParam, "WindowType", string(8));
		ParamAdd(strParam, "FontType", string(0));
		ParamAdd(strParam, "BackgroundType", string(0));
		ParamAdd(strParam, "LifeTime", string(5000));
		ParamAdd(strParam, "AnimationType", string(1));
		ParamAdd(strParam, "Msg", Msg);
		ParamAdd(strParam, "MsgColorR", string(255));
		ParamAdd(strParam, "MsgColorG", string(150));
		ParamAdd(strParam, "MsgColorB", string(149));
		ExecuteEvent(140, strParam);
	}
	return;
}

function HandleIMEStatusChange()
{
	local string Texture;
	local UIEventManager.EIMEType imeType;

	imeType = GetCurrentIMELang();
	switch(imeType)
	{
		// End:0x36
		case 1:
			Texture = "L2UI.ChatWnd.IME_kr";
			// End:0x204
			break;
		// End:0x59
		case 2:
			Texture = "L2UI.ChatWnd.IME_en";
			// End:0x204
			break;
		// End:0x7C
		case 3:
			Texture = "L2UI.ChatWnd.IME_jp";
			// End:0x204
			break;
		// End:0x9F
		case 4:
			Texture = "L2UI.ChatWnd.IME_jp";
			// End:0x204
			break;
		// End:0xC3
		case 5:
			Texture = "L2UI.ChatWnd.IME_tw2";
			// End:0x204
			break;
		// End:0xE7
		case 6:
			Texture = "L2UI.ChatWnd.IME_tw3";
			// End:0x204
			break;
		// End:0x10B
		case 7:
			Texture = "L2UI.ChatWnd.IME_tw1";
			// End:0x204
			break;
		// End:0x12F
		case 9:
			Texture = "L2UI.ChatWnd.IME_cn1";
			// End:0x204
			break;
		// End:0x153
		case 10:
			Texture = "L2UI.ChatWnd.IME_cn2";
			// End:0x204
			break;
		// End:0x177
		case 11:
			Texture = "L2UI.ChatWnd.IME_cn3";
			// End:0x204
			break;
		// End:0x19B
		case 12:
			Texture = "L2UI.ChatWnd.IME_cn4";
			// End:0x204
			break;
		// End:0x1BF
		case 13:
			Texture = "L2UI.ChatWnd.IME_cn4";
			// End:0x204
			break;
		// End:0x1E2
		case 14:
			Texture = "L2UI.ChatWnd.IME_th";
			// End:0x204
			break;
		// End:0xFFFF
		default:
			Texture = "Default.BlackTexture";
			// End:0x204
			break;
			break;
	}
	Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ChatWnd.LanguageTexture", Texture);
	return;
}

function bool CheckFilter(UIEventManager.EChatType Type, int WindowType, UIEventManager.ESystemMsgType systemType)
{
	// End:0x5E
	if(__NFUN_130__(__NFUN_129__(__NFUN_130__(__NFUN_153__(WindowType, 0), __NFUN_150__(WindowType, 5))), __NFUN_155__(WindowType, 5)))
	{
		Debug(__NFUN_112__("ChatWnd: Error invalid windowType ", string(WindowType)));
		return false;
	}
	// End:0x8B
	if(__NFUN_130__(__NFUN_154__(int(Type), int(8)), __NFUN_155__(m_filterInfo[WindowType].bTrade, 0)))
	{
		return true;		
	}
	else
	{
		// End:0xB8
		if(__NFUN_130__(__NFUN_154__(int(Type), int(0)), __NFUN_155__(m_filterInfo[WindowType].bNormal, 0)))
		{
			return true;			
		}
		else
		{
			// End:0xE5
			if(__NFUN_130__(__NFUN_154__(int(Type), int(4)), __NFUN_155__(m_filterInfo[WindowType].bClan, 0)))
			{
				return true;				
			}
			else
			{
				// End:0x112
				if(__NFUN_130__(__NFUN_154__(int(Type), int(3)), __NFUN_155__(m_filterInfo[WindowType].bParty, 0)))
				{
					return true;					
				}
				else
				{
					// End:0x13F
					if(__NFUN_130__(__NFUN_154__(int(Type), int(1)), __NFUN_155__(m_filterInfo[WindowType].bShout, 0)))
					{
						return true;						
					}
					else
					{
						// End:0x16C
						if(__NFUN_130__(__NFUN_154__(int(Type), int(2)), __NFUN_155__(m_filterInfo[WindowType].bWhisper, 0)))
						{
							return true;							
						}
						else
						{
							// End:0x199
							if(__NFUN_130__(__NFUN_154__(int(Type), int(9)), __NFUN_155__(m_filterInfo[WindowType].bAlly, 0)))
							{
								return true;								
							}
							else
							{
								// End:0x1C6
								if(__NFUN_130__(__NFUN_154__(int(Type), int(17)), __NFUN_155__(m_filterInfo[WindowType].bHero, 0)))
								{
									return true;									
								}
								else
								{
									// End:0x211
									if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_154__(int(Type), int(10)), __NFUN_154__(int(Type), int(18))), __NFUN_154__(int(Type), int(6))), __NFUN_154__(int(Type), int(7))))
									{
										return true;										
									}
									else
									{
										// End:0x250
										if(__NFUN_130__(__NFUN_132__(__NFUN_154__(int(Type), int(16)), __NFUN_154__(int(Type), int(15))), __NFUN_155__(m_filterInfo[WindowType].bUnion, 0)))
										{
											return true;											
										}
										else
										{
											// End:0x3B5
											if(__NFUN_154__(int(Type), int(5)))
											{
												// End:0x287
												if(__NFUN_132__(__NFUN_154__(int(systemType), int(2)), __NFUN_154__(int(systemType), int(6))))
												{
													return true;													
												}
												else
												{
													// End:0x33E
													if(__NFUN_154__(WindowType, 5))
													{
														// End:0x2D0
														if(__NFUN_154__(int(systemType), int(3)))
														{
															// End:0x2CB
															if(GetOptionBool("Game", "SystemMsgWndDamage"))
															{
																return true;																
															}
															else
															{
																return false;
															}															
														}
														else
														{
															// End:0x315
															if(__NFUN_154__(int(systemType), int(7)))
															{
																// End:0x310
																if(GetOptionBool("Game", "SystemMsgWndExpendableItem"))
																{
																	return true;																	
																}
																else
																{
																	return false;
																}																
															}
															else
															{
																// End:0x339
																if(__NFUN_132__(__NFUN_154__(int(systemType), int(1)), __NFUN_154__(int(systemType), int(0))))
																{
																	return true;
																}
															}
														}
														return false;														
													}
													else
													{
														// End:0x3B3
														if(__NFUN_155__(m_filterInfo[WindowType].bSystem, 0))
														{
															// End:0x384
															if(__NFUN_154__(int(systemType), int(3)))
															{
																// End:0x37F
																if(__NFUN_155__(m_filterInfo[WindowType].bDamage, 0))
																{
																	return true;																	
																}
																else
																{
																	return false;
																}																
															}
															else
															{
																// End:0x3B1
																if(__NFUN_154__(int(systemType), int(7)))
																{
																	// End:0x3AF
																	if(__NFUN_155__(m_filterInfo[WindowType].bUseitem, 0))
																	{
																		return true;																		
																	}
																	else
																	{
																		return false;
																	}
																}
															}
															return true;
														}
													}
												}
												return false;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
	return false;
	return;
}

function InitFilterInfo()
{
	local int i, tempVal;

	SetDefaultFilterValue();
	i = 0;
	J0x0D:

	// End:0x3B5 [Loop If]
	if(__NFUN_150__(i, 5))
	{
		// End:0x60
		if(GetINIBool(m_sectionName[i], "system", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bSystem = tempVal;
		}
		// End:0xA5
		if(GetINIBool(m_sectionName[i], "chat", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bChat = tempVal;
		}
		// End:0xEC
		if(GetINIBool(m_sectionName[i], "normal", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bNormal = tempVal;
		}
		// End:0x132
		if(GetINIBool(m_sectionName[i], "shout", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bShout = tempVal;
		}
		// End:0x179
		if(GetINIBool(m_sectionName[i], "pledge", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bClan = tempVal;
		}
		// End:0x1BF
		if(GetINIBool(m_sectionName[i], "party", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bParty = tempVal;
		}
		// End:0x206
		if(GetINIBool(m_sectionName[i], "market", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bTrade = tempVal;
		}
		// End:0x24B
		if(GetINIBool(m_sectionName[i], "tell", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bWhisper = tempVal;
		}
		// End:0x292
		if(GetINIBool(m_sectionName[i], "damage", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bDamage = tempVal;
		}
		// End:0x2D7
		if(GetINIBool(m_sectionName[i], "ally", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bAlly = tempVal;
		}
		// End:0x320
		if(GetINIBool(m_sectionName[i], "useitems", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bUseitem = tempVal;
		}
		// End:0x365
		if(GetINIBool(m_sectionName[i], "hero", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bHero = tempVal;
		}
		// End:0x3AB
		if(GetINIBool(m_sectionName[i], "union", tempVal, "chatfilter.ini"))
		{
			m_filterInfo[i].bUnion = tempVal;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x0D;
	}
	SetDefaultFilterOn();
	// End:0x3F5
	if(GetINIBool("global", "command", tempVal, "chatfilter.ini"))
	{
		m_NoUnionCommanderMessage = tempVal;
	}
	return;
}

function SetDefaultFilterOn()
{
	m_filterInfo[1].bTrade = 1;
	m_filterInfo[2].bParty = 1;
	m_filterInfo[3].bClan = 1;
	m_filterInfo[4].bAlly = 1;
	return;
}

function SetDefaultFilterValue()
{
	m_filterInfo[0].bSystem = 1;
	m_filterInfo[0].bChat = 1;
	m_filterInfo[0].bNormal = 1;
	m_filterInfo[0].bShout = 1;
	m_filterInfo[0].bClan = 1;
	m_filterInfo[0].bParty = 1;
	m_filterInfo[0].bTrade = 0;
	m_filterInfo[0].bWhisper = 1;
	m_filterInfo[0].bDamage = 1;
	m_filterInfo[0].bAlly = 0;
	m_filterInfo[0].bUseitem = 0;
	m_filterInfo[0].bHero = 0;
	m_filterInfo[0].bUnion = 1;
	m_filterInfo[1].bSystem = 1;
	m_filterInfo[1].bChat = 1;
	m_filterInfo[1].bNormal = 0;
	m_filterInfo[1].bShout = 1;
	m_filterInfo[1].bClan = 0;
	m_filterInfo[1].bParty = 0;
	m_filterInfo[1].bTrade = 1;
	m_filterInfo[1].bWhisper = 1;
	m_filterInfo[1].bDamage = 1;
	m_filterInfo[1].bAlly = 0;
	m_filterInfo[1].bUseitem = 0;
	m_filterInfo[1].bHero = 0;
	m_filterInfo[1].bUnion = 0;
	m_filterInfo[2].bSystem = 1;
	m_filterInfo[2].bChat = 1;
	m_filterInfo[2].bNormal = 0;
	m_filterInfo[2].bShout = 1;
	m_filterInfo[2].bClan = 0;
	m_filterInfo[2].bParty = 1;
	m_filterInfo[2].bTrade = 0;
	m_filterInfo[2].bWhisper = 1;
	m_filterInfo[2].bDamage = 1;
	m_filterInfo[2].bAlly = 0;
	m_filterInfo[2].bUseitem = 0;
	m_filterInfo[2].bHero = 0;
	m_filterInfo[2].bUnion = 0;
	m_filterInfo[3].bSystem = 1;
	m_filterInfo[3].bChat = 1;
	m_filterInfo[3].bNormal = 0;
	m_filterInfo[3].bShout = 1;
	m_filterInfo[3].bClan = 1;
	m_filterInfo[3].bParty = 0;
	m_filterInfo[3].bTrade = 0;
	m_filterInfo[3].bWhisper = 1;
	m_filterInfo[3].bDamage = 1;
	m_filterInfo[3].bAlly = 0;
	m_filterInfo[3].bUseitem = 0;
	m_filterInfo[3].bHero = 0;
	m_filterInfo[3].bUnion = 0;
	m_filterInfo[4].bSystem = 1;
	m_filterInfo[4].bChat = 1;
	m_filterInfo[4].bNormal = 0;
	m_filterInfo[4].bShout = 1;
	m_filterInfo[4].bClan = 0;
	m_filterInfo[4].bParty = 0;
	m_filterInfo[4].bTrade = 0;
	m_filterInfo[4].bWhisper = 1;
	m_filterInfo[4].bDamage = 1;
	m_filterInfo[4].bAlly = 1;
	m_filterInfo[4].bUseitem = 0;
	m_filterInfo[4].bHero = 0;
	m_filterInfo[4].bUnion = 0;
	m_filterInfo[5].bSystem = 0;
	m_filterInfo[5].bChat = 0;
	m_filterInfo[5].bNormal = 0;
	m_filterInfo[5].bShout = 0;
	m_filterInfo[5].bClan = 0;
	m_filterInfo[5].bParty = 0;
	m_filterInfo[5].bTrade = 0;
	m_filterInfo[5].bWhisper = 0;
	m_filterInfo[5].bDamage = 0;
	m_filterInfo[5].bAlly = 0;
	m_filterInfo[5].bUseitem = 0;
	m_filterInfo[5].bHero = 0;
	m_filterInfo[5].bUnion = 0;
	m_NoUnionCommanderMessage = 0;
	return;
}

function SetChatFilterButton()
{
	local bool bSystemMsgWnd, bOption;

	bSystemMsgWnd = GetOptionBool("Game", "SystemMsgWnd");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.SystemMsgBox", bSystemMsgWnd);
	bOption = GetOptionBool("Game", "SystemMsgWndDamage");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.DamageBox", bOption);
	bOption = GetOptionBool("Game", "SystemMsgWndExpendableItem");
	Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.ItemBox", bOption);
	// End:0x8F5
	if(__NFUN_130__(__NFUN_153__(m_chatType, 0), __NFUN_150__(m_chatType, 5)))
	{
		switch(m_chatType)
		{
			// End:0x16E
			case 0:
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ChatFilterWnd.CurrentText", MakeFullSystemMsg(GetSystemMessage(1995), GetSystemString(144), ""));
				// End:0x2AA
				break;
			// End:0x1BD
			case 1:
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ChatFilterWnd.CurrentText", MakeFullSystemMsg(GetSystemMessage(1995), GetSystemString(355), ""));
				// End:0x2AA
				break;
			// End:0x20A
			case 2:
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ChatFilterWnd.CurrentText", MakeFullSystemMsg(GetSystemMessage(1995), GetSystemString(188), ""));
				// End:0x2AA
				break;
			// End:0x257
			case 3:
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ChatFilterWnd.CurrentText", MakeFullSystemMsg(GetSystemMessage(1995), GetSystemString(128), ""));
				// End:0x2AA
				break;
			// End:0x2A7
			case 4:
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("ChatFilterWnd.CurrentText", MakeFullSystemMsg(GetSystemMessage(1995), GetSystemString(559), ""));
				// End:0x2AA
				break;
			// End:0xFFFF
			default:
				break;
		}
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxSystem", bool(m_filterInfo[m_chatType].bSystem));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxNormal", bool(m_filterInfo[m_chatType].bNormal));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxShout", bool(m_filterInfo[m_chatType].bShout));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxPledge", bool(m_filterInfo[m_chatType].bClan));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxParty", bool(m_filterInfo[m_chatType].bParty));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxTrade", bool(m_filterInfo[m_chatType].bTrade));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxWhisper", bool(m_filterInfo[m_chatType].bWhisper));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxDamage", bool(m_filterInfo[m_chatType].bDamage));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxAlly", bool(m_filterInfo[m_chatType].bAlly));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxItem", bool(m_filterInfo[m_chatType].bUseitem));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxHero", bool(m_filterInfo[m_chatType].bHero));
		Class'NWindow.UIAPI_CHECKBOX'.static.SetCheck("ChatFilterWnd.CheckBoxUnion", bool(m_filterInfo[m_chatType].bUnion));
		// End:0x624
		if(__NFUN_129__(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxSystem")))
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxDamage", true);
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxItem", true);			
		}
		else
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxDamage", false);
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxItem", false);
		}
		Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxNormal", false);
		Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxShout", false);
		Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxPledge", false);
		Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxParty", false);
		Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxTrade", false);
		Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxWhisper", false);
		Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxAlly", false);
		Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxHero", false);
		Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxUnion", false);
		switch(m_chatType)
		{
			// End:0x850
			case 1:
				Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxTrade", true);
				// End:0x8F5
				break;
			// End:0x885
			case 2:
				Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxParty", true);
				// End:0x8F5
				break;
			// End:0x8BB
			case 3:
				Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxPledge", true);
				// End:0x8F5
				break;
			// End:0x8EF
			case 4:
				Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxAlly", true);
				// End:0x8F5
				break;
			// End:0xFFFF
			default:
				// End:0x8F5
				break;
				break;
		}
	}
	return;
}

function HandleChatWndStatusChange()
{
	local UserInfo UserInfo;

	GetPlayerInfo(UserInfo);
	// End:0x30
	if(__NFUN_151__(UserInfo.nClanID, 0))
	{
		ChatTabCtrl.SetDisable(3, false);		
	}
	else
	{
		ChatTabCtrl.SetDisable(3, true);
	}
	// End:0x67
	if(__NFUN_151__(UserInfo.nAllianceID, 0))
	{
		ChatTabCtrl.SetDisable(4, false);		
	}
	else
	{
		ChatTabCtrl.SetDisable(4, true);
	}
	return;
}

function HandleSetString(string a_Param)
{
	local string tmpString;

	// End:0x2F
	if(ParseString(a_Param, "String", tmpString))
	{
		ChatEditBox.SetString(tmpString);
	}
	return;
}

function HandleSetFocus()
{
	// End:0x23
	if(__NFUN_129__(ChatEditBox.IsFocused()))
	{
		ChatEditBox.SetFocus();
	}
	return;
}

function Print(int Index)
{
	Debug(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Print type(", string(Index)), "), system :"), string(m_filterInfo[Index].bSystem)), ", chat:"), string(m_filterInfo[Index].bChat)), ",Normal:"), string(m_filterInfo[Index].bNormal)), ", shout:"), string(m_filterInfo[Index].bShout)), ",pledge:"), string(m_filterInfo[Index].bClan)), ", party:"), string(m_filterInfo[Index].bParty)), ", trade:"), string(m_filterInfo[Index].bTrade)), ", whisper:"), string(m_filterInfo[Index].bWhisper)), ", damage:"), string(m_filterInfo[Index].bDamage)), ", ally:"), string(m_filterInfo[Index].bAlly)), ",useitem:"), string(m_filterInfo[Index].bUseitem)), ", hero:"), string(m_filterInfo[Index].bHero)));
	return;
}

function HandleMsnStatus(string param)
{
	local string Status;
	local ButtonHandle Handle;

	Handle = ButtonHandle(GetHandle("Chatwnd.MessengerBtn"));
	ParseString(param, "status", Status);
	// End:0xA2
	if(__NFUN_122__(Status, "online"))
	{
		Handle.SetTexture("L2UI_CH3.Msn.chatting_msn1", "L2UI_CH3.Msn.chatting_msn1_down", "");		
	}
	else
	{
		// End:0x141
		if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_122__(Status, "berightback"), __NFUN_122__(Status, "idle")), __NFUN_122__(Status, "away")), __NFUN_122__(Status, "lunch")))
		{
			Handle.SetTexture("L2UI_CH3.Msn.chatting_msn2", "L2UI_CH3.Msn.chatting_msn2_down", "");			
		}
		else
		{
			// End:0x1BA
			if(__NFUN_132__(__NFUN_122__(Status, "busy"), __NFUN_122__(Status, "onthephone")))
			{
				Handle.SetTexture("L2UI_CH3.Msn.chatting_msn3", "L2UI_CH3.Msn.chatting_msn3_down", "");				
			}
			else
			{
				// End:0x235
				if(__NFUN_132__(__NFUN_122__(Status, "offline"), __NFUN_122__(Status, "invisible")))
				{
					Handle.SetTexture("L2UI_CH3.Msn.chatting_msn4", "L2UI_CH3.Msn.chatting_msn4_down", "");					
				}
				else
				{
					// End:0x293
					if(__NFUN_122__(Status, "none"))
					{
						Handle.SetTexture("L2UI_CH3.Msn.chatting_msn5", "L2UI_CH3.Msn.chatting_msn5_down", "");
					}
				}
			}
		}
	}
	return;
}

function UIEventManager.EChatType GetChatTypeByTabIndex(int Index)
{
	local UIEventManager.EChatType Type;

	Type = 0;
	switch(m_chatType)
	{
		// End:0x1E
		case 0:
			Type = 0;
			// End:0x63
			break;
		// End:0x2D
		case 1:
			Type = 8;
			// End:0x63
			break;
		// End:0x3D
		case 2:
			Type = 3;
			// End:0x63
			break;
		// End:0x4D
		case 3:
			Type = 4;
			// End:0x63
			break;
		// End:0x5D
		case 4:
			Type = 9;
			// End:0x63
			break;
		// End:0xFFFF
		default:
			// End:0x63
			break;
			break;
	}
	return Type;
	return;
}
