class Console extends Interaction
	native
	config;

const MaxHistory = 16;

var globalconfig byte ConsoleKey;
var int HistoryTop;
var int HistoryBot;
var int HistoryCur;
var string TypedStr;
var string History[16];
var bool bTyping;
var bool bIgnoreKeys;

exec function Type()
{
	TypedStr = "";
	__NFUN_113__('Typing');
	return;
}

exec function Talk()
{
	TypedStr = "Say ";
	__NFUN_113__('Typing');
	return;
}

exec function TeamTalk()
{
	TypedStr = "TeamSay ";
	__NFUN_113__('Typing');
	return;
}

event Message(coerce string Msg, float MsgLife)
{
	return;
}

function bool KeyEvent(Interactions.EInputKey Key, Interactions.EInputAction Action, float Delta)
{
	// End:0x15
	if(__NFUN_155__(int(Action), int(1)))
	{
		return false;		
	}
	else
	{
		// End:0x34
		if(__NFUN_154__(int(Key), int(ConsoleKey)))
		{
			__NFUN_113__('Typing');
			return true;			
		}
		else
		{
			return false;
		}
	}
	return;
}

state Typing
{
	exec function Type()
	{
		TypedStr = "";
		__NFUN_113__('None');
		return;
	}

	function bool KeyType(Interactions.EInputKey Key, optional string Unicode)
	{
		// End:0x0B
		if(bIgnoreKeys)
		{
			return true;
		}
		// End:0x8B
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_153__(int(Key), 32), __NFUN_150__(int(Key), 256)), __NFUN_155__(int(Key), __NFUN_237__("~"))), __NFUN_155__(int(Key), __NFUN_237__("`"))))
		{
			// End:0x73
			if(__NFUN_123__(Unicode, ""))
			{
				TypedStr = __NFUN_112__(TypedStr, Unicode);				
			}
			else
			{
				TypedStr = __NFUN_112__(TypedStr, __NFUN_236__(int(Key)));
			}
			return true;
		}
		return;
	}

	function bool KeyEvent(Interactions.EInputKey Key, Interactions.EInputAction Action, float Delta)
	{
		local string temp;

		// End:0x18
		if(__NFUN_154__(int(Action), int(1)))
		{
			bIgnoreKeys = false;
		}
		// End:0x56
		if(__NFUN_154__(int(Key), int(27)))
		{
			// End:0x4C
			if(__NFUN_123__(TypedStr, ""))
			{
				TypedStr = "";
				HistoryCur = HistoryTop;
				return true;				
			}
			else
			{
				__NFUN_113__('None');
			}			
		}
		else
		{
			// End:0x73
			if(global.KeyEvent(Key, Action, Delta))
			{
				return true;				
			}
			else
			{
				// End:0x88
				if(__NFUN_155__(int(Action), int(1)))
				{
					return false;					
				}
				else
				{
					// End:0x18B
					if(__NFUN_154__(int(Key), int(13)))
					{
						// End:0x17F
						if(__NFUN_123__(TypedStr, ""))
						{
							Message(TypedStr, 6.0000000);
							History[HistoryTop] = TypedStr;
							HistoryTop = int(__NFUN_173__(float(__NFUN_146__(HistoryTop, 1)), float(16)));
							// End:0x115
							if(__NFUN_132__(__NFUN_154__(HistoryBot, -1), __NFUN_154__(HistoryBot, HistoryTop)))
							{
								HistoryBot = int(__NFUN_173__(float(__NFUN_146__(HistoryBot, 1)), float(16)));
							}
							HistoryCur = HistoryTop;
							temp = TypedStr;
							TypedStr = "";
							// End:0x168
							if(__NFUN_129__(ConsoleCommand(temp)))
							{
								Message(Localize("Errors", "Exec", "Core"), 6.0000000);
							}
							Message("", 6.0000000);
							__NFUN_113__('None');							
						}
						else
						{
							__NFUN_113__('None');
						}
						return true;						
					}
					else
					{
						// End:0x1F6
						if(__NFUN_154__(int(Key), int(38)))
						{
							// End:0x1F1
							if(__NFUN_153__(HistoryBot, 0))
							{
								// End:0x1C3
								if(__NFUN_154__(HistoryCur, HistoryBot))
								{
									HistoryCur = HistoryTop;									
								}
								else
								{
									__NFUN_166__(HistoryCur);
									// End:0x1E0
									if(__NFUN_150__(HistoryCur, 0))
									{
										HistoryCur = __NFUN_147__(16, 1);
									}
								}
								TypedStr = History[HistoryCur];
							}
							return true;							
						}
						else
						{
							// End:0x25A
							if(__NFUN_154__(int(Key), int(40)))
							{
								// End:0x257
								if(__NFUN_153__(HistoryBot, 0))
								{
									// End:0x22E
									if(__NFUN_154__(HistoryCur, HistoryTop))
									{
										HistoryCur = HistoryBot;										
									}
									else
									{
										HistoryCur = int(__NFUN_173__(float(__NFUN_146__(HistoryCur, 1)), float(16)));
									}
									TypedStr = History[HistoryCur];
								}								
							}
							else
							{
								// End:0x2A2
								if(__NFUN_132__(__NFUN_154__(int(Key), int(8)), __NFUN_154__(int(Key), int(37))))
								{
									// End:0x2A0
									if(__NFUN_151__(__NFUN_125__(TypedStr), 0))
									{
										TypedStr = __NFUN_128__(TypedStr, __NFUN_147__(__NFUN_125__(TypedStr), 1));
									}
									return true;
								}
							}
						}
					}
				}
			}
		}
		return true;
		return;
	}

	function PostRender(Canvas Canvas)
	{
		local float XL, YL;
		local string OutStr;

		Canvas.Style = 1;
		Canvas.Font = Font'Engine.ConsoleFont';
		OutStr = __NFUN_112__(__NFUN_168__("(>", TypedStr), "_");
		Canvas.__NFUN_464__(OutStr, XL, YL);
		Canvas.SetPos(0.0000000, __NFUN_175__(__NFUN_175__(Canvas.ClipY, float(6)), YL));
		Canvas.__NFUN_466__(Texture'Engine.ConsoleBK', Canvas.ClipX, __NFUN_174__(YL, float(6)), 0.0000000, 0.0000000, 32.0000000, 32.0000000);
		Canvas.SetPos(0.0000000, __NFUN_175__(__NFUN_175__(Canvas.ClipY, float(8)), YL));
		Canvas.SetDrawColor(0, byte(255), 0);
		Canvas.__NFUN_466__(Texture'Engine.ConsoleBdr', Canvas.ClipX, 2.0000000, 0.0000000, 0.0000000, 32.0000000, 32.0000000);
		Canvas.SetPos(0.0000000, __NFUN_175__(__NFUN_175__(Canvas.ClipY, float(3)), YL));
		Canvas.bCenter = false;
		Canvas.__NFUN_465__(OutStr, false);
		return;
	}

	function BeginState()
	{
		bTyping = true;
		bVisible = true;
		bIgnoreKeys = true;
		HistoryCur = HistoryTop;
		return;
	}

	function EndState()
	{
		bTyping = false;
		bVisible = false;
		return;
	}
	stop;
}

defaultproperties
{
	HistoryBot=-1
	bRequiresTick=true
}