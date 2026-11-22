class GametipWnd extends UIScript;

var array<GameTipData> TipData;
var int CountRecord;
var UserInfo userinfofortip;
var string CurrentTip;
var int numb;

function OnLoad()
{
	LoadGameTipData();
	RegisterEvent(1900);
	return;
}

function OnEventWithStr(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x18
		case 1900:
			LoadGameTipData();
			// End:0x1B
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function LoadGameTipData()
{
	local int i;
	local bool gamedataloaded;
	local GameTipData TipData1;

	CountRecord = Class'NWindow.UIDATA_GAMETIP'.static.GetDataCount();
	i = 0;
	J0x1C:

	// End:0x66 [Loop If]
	if(__NFUN_150__(i, CountRecord))
	{
		gamedataloaded = Class'NWindow.UIDATA_GAMETIP'.static.GetDataByIndex(i, TipData1);
		TipData[i] = TipData1;
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1C;
	}
	return;
}

function OnShow()
{
	local int RandomVal, PrioritySelect, UserLevelData;
	local bool userinfoloaded;
	local array<string> SelectedCondition;
	local int i, j, UserLevel, UserArrange, NumberSelect;

	j = 0;
	userinfoloaded = GetPlayerInfo(userinfofortip);
	// End:0x28
	if(__NFUN_242__(userinfoloaded, false))
	{		
	}
	UserLevelData = userinfofortip.nLevel;
	// End:0x5B
	if(__NFUN_130__(__NFUN_153__(UserLevelData, 1), __NFUN_152__(UserLevelData, 20)))
	{
		UserLevel = 1;		
	}
	else
	{
		// End:0x80
		if(__NFUN_130__(__NFUN_153__(UserLevelData, 21), __NFUN_152__(UserLevelData, 40)))
		{
			UserLevel = 20;			
		}
		else
		{
			// End:0xA5
			if(__NFUN_130__(__NFUN_153__(UserLevelData, 41), __NFUN_152__(UserLevelData, 60)))
			{
				UserLevel = 40;				
			}
			else
			{
				// End:0xCA
				if(__NFUN_130__(__NFUN_153__(UserLevelData, 61), __NFUN_152__(UserLevelData, 74)))
				{
					UserLevel = 60;					
				}
				else
				{
					// End:0xEC
					if(__NFUN_130__(__NFUN_153__(UserLevelData, 75), __NFUN_152__(UserLevelData, 80)))
					{
						UserLevel = 80;
					}
				}
			}
		}
	}
	// End:0x103
	if(__NFUN_150__(UserLevelData, 40))
	{
		UserArrange = 101;		
	}
	else
	{
		UserArrange = 102;
	}
	RandomVal = __NFUN_146__(__NFUN_167__(99), 1);
	// End:0x13B
	if(__NFUN_130__(__NFUN_153__(RandomVal, 1), __NFUN_152__(RandomVal, 50)))
	{
		PrioritySelect = 1;		
	}
	else
	{
		// End:0x160
		if(__NFUN_130__(__NFUN_153__(RandomVal, 51), __NFUN_152__(RandomVal, 75)))
		{
			PrioritySelect = 2;			
		}
		else
		{
			// End:0x185
			if(__NFUN_130__(__NFUN_153__(RandomVal, 76), __NFUN_152__(RandomVal, 90)))
			{
				PrioritySelect = 3;				
			}
			else
			{
				// End:0x1A7
				if(__NFUN_130__(__NFUN_153__(RandomVal, 91), __NFUN_152__(RandomVal, 100)))
				{
					PrioritySelect = 4;
				}
			}
		}
	}
	i = 0;
	J0x1AE:

	// End:0x283 [Loop If]
	if(__NFUN_150__(i, TipData.Length))
	{
		// End:0x279
		if(__NFUN_123__(TipData[i].TipMsg, ""))
		{
			// End:0x279
			if(__NFUN_130__(__NFUN_154__(TipData[i].Priority, PrioritySelect), __NFUN_242__(TipData[i].Validity, true)))
			{
				// End:0x279
				if(__NFUN_132__(__NFUN_132__(__NFUN_154__(TipData[i].TargetLevel, UserLevel), __NFUN_154__(TipData[i].TargetLevel, 0)), __NFUN_154__(TipData[i].TargetLevel, UserArrange)))
				{
					SelectedCondition[j] = TipData[i].TipMsg;
					__NFUN_163__(j);
				}
			}
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x1AE;
	}
	NumberSelect = __NFUN_167__(SelectedCondition.Length);
	// End:0x2A8
	if(__NFUN_154__(SelectedCondition.Length, 0))
	{
		CurrentTip = "";		
	}
	else
	{
		CurrentTip = SelectedCondition[NumberSelect];
	}
	// End:0x403
	if(__NFUN_242__(GetOptionBool("Game", "ShowGameTipMsg"), false))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText1", GetSystemString(1455));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText1-1", GetSystemString(1455));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText1-2", GetSystemString(1455));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText", CurrentTip);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText-1", CurrentTip);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText-2", CurrentTip);		
	}
	else
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText1", "");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText1-1", "");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText1-2", "");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText", "");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText-1", "");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("GametipWnd.GameTipText-2", "");
	}
	return;
}
