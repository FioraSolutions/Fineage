class HUD extends Actor
	transient
	native
	config(User)
	notplaceable;

struct HUDLocalizedMessage
{
	var Class<LocalMessage> Message;
	var int Switch;
	var PlayerReplicationInfo RelatedPRI;
	var Object OptionalObject;
	var float EndOfLife;
	var float LifeTime;
	var bool bDrawing;
	var int numLines;
	var string StringMessage;
	var Color DrawColor;
	var Font StringFont;
	var float XL;
	var float YL;
	var float YPos;
};

struct ConsoleMessage
{
	var string Text;
	var Color TextColor;
	var float MessageLife;
	var PlayerReplicationInfo PRI;
};

var Font SmallFont;
var Font MedFont;
var Font BigFont;
var Font LargeFont;
var string HUDConfigWindowType;
var HUD nextHUD;
var PlayerController PlayerOwner;
var localized string ProgressFontName;
var Font ProgressFontFont;
var float ProgressFadeTime;
var Color MOTDColor;
var ScoreBoard ScoreBoard;
var bool bHideHUD;
var bool bShowScores;
var bool bShowDebugInfo;
var bool bHideCenterMessages;
var bool bBadConnectionAlert;
var config bool bMessageBeep;
var globalconfig float HudCanvasScale;
var localized string LoadingMessage;
var localized string SavingMessage;
var localized string ConnectingMessage;
var localized string PausedMessage;
var localized string PrecachingMessage;
var Color ConsoleColor;
var globalconfig int ConsoleMessageCount;
var globalconfig int ConsoleFontSize;
var globalconfig int MessageFontOffset;
var ConsoleMessage TextMessages[8];
var() float ConsoleMessagePosX;
var() float ConsoleMessagePosY;
var localized string FontArrayNames[9];
var Font FontArrayFonts[9];
var int FontScreenWidthMedium[9];
var int FontScreenWidthSmall[9];

// Export UHUD::execDraw3DLine(FFrame&, void* const)
native final function Draw3DLine(Vector Start, Vector End, Color LineColor);

simulated event PostBeginPlay()
{
	super.PostBeginPlay();
	PlayerOwner = PlayerController(Owner);
	return;
}

function SpawnScoreBoard(Class<ScoreBoard> ScoringType)
{
	// End:0x1E
	if(__NFUN_119__(ScoringType, none))
	{
		ScoreBoard = __NFUN_278__(ScoringType, PlayerOwner);
	}
	return;
}

simulated event Destroyed()
{
	// End:0x1E
	if(__NFUN_119__(ScoreBoard, none))
	{
		ScoreBoard.__NFUN_279__();
		ScoreBoard = none;
	}
	super.Destroyed();
	return;
}

exec function ShowScores()
{
	bShowScores = __NFUN_129__(bShowScores);
	return;
}

exec function ShowDebug()
{
	bShowDebugInfo = __NFUN_129__(bShowDebugInfo);
	return;
}

simulated event WorldSpaceOverlays()
{
	// End:0x2A
	if(__NFUN_130__(bShowDebugInfo, __NFUN_119__(Pawn(PlayerOwner.ViewTarget), none)))
	{
		DrawRoute();
	}
	return;
}

function CheckCountdown(GameReplicationInfo GRI)
{
	return;
}

event ConnectFailure(string FailCode, string URL)
{
	PlayerOwner.ReceiveLocalizedMessage(Class'Engine.FailedConnect', Class'Engine.FailedConnect'.static.GetFailSwitch(FailCode));
	return;
}

event ShowUpgradeMenu()
{
	return;
}

function PlayStartupMessage(byte Stage)
{
	return;
}

function ClearMessage(out HUDLocalizedMessage M)
{
	M.Message = none;
	M.Switch = 0;
	M.RelatedPRI = none;
	M.OptionalObject = none;
	M.EndOfLife = 0.0000000;
	M.StringMessage = "";
	M.DrawColor = Class'Engine.Canvas'.static.MakeColor(byte(255), byte(255), byte(255));
	M.XL = 0.0000000;
	M.bDrawing = false;
	return;
}

function CopyMessage(out HUDLocalizedMessage M1, HUDLocalizedMessage M2)
{
	M1.Message = M2.Message;
	M1.Switch = M2.Switch;
	M1.RelatedPRI = M2.RelatedPRI;
	M1.OptionalObject = M2.OptionalObject;
	M1.EndOfLife = M2.EndOfLife;
	M1.StringMessage = M2.StringMessage;
	M1.DrawColor = M2.DrawColor;
	M1.XL = M2.XL;
	M1.YL = M2.YL;
	M1.YPos = M2.YPos;
	M1.bDrawing = M2.bDrawing;
	M1.LifeTime = M2.LifeTime;
	M1.numLines = M2.numLines;
	return;
}

simulated event PostRender(Canvas Canvas)
{
	local float XPos, YPos;
	local Pawn P;

	CheckCountdown(PlayerOwner.GameReplicationInfo);
	// End:0x7F
	if(__NFUN_129__(PlayerOwner.bBehindView))
	{
		P = Pawn(PlayerOwner.ViewTarget);
		// End:0x7F
		if(__NFUN_130__(__NFUN_119__(P, none), __NFUN_119__(P.Weapon, none)))
		{
			P.Weapon.RenderOverlays(Canvas);
		}
	}
	DisplayMessages(Canvas);
	bHideCenterMessages = DrawLevelAction(Canvas);
	// End:0xD5
	if(__NFUN_130__(__NFUN_129__(bHideCenterMessages), __NFUN_177__(PlayerOwner.ProgressTimeOut, Level.TimeSeconds)))
	{
		DisplayProgressMessage(Canvas);
	}
	// End:0xE4
	if(bBadConnectionAlert)
	{
		DisplayBadConnectionAlert();
	}
	// End:0x117
	if(__NFUN_130__(__NFUN_119__(P, none), P.bSpecialHUD))
	{
		P.DrawHUD(Canvas);
	}
	// End:0x189
	if(bShowDebugInfo)
	{
		Canvas.Font = GetConsoleFont(Canvas);
		Canvas.Style = 5;
		Canvas.DrawColor = ConsoleColor;
		PlayerOwner.ViewTarget.DisplayDebug(Canvas, XPos, YPos);		
	}
	else
	{
		// End:0x29F
		if(__NFUN_129__(bHideHUD))
		{
			// End:0x1DC
			if(bShowScores)
			{
				// End:0x1D9
				if(__NFUN_119__(ScoreBoard, none))
				{
					ScoreBoard.DrawScoreboard(Canvas);
					// End:0x1D9
					if(ScoreBoard.bDisplayMessages)
					{
						DisplayMessages(Canvas);
					}
				}				
			}
			else
			{
				// End:0x22C
				if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_114__(PlayerOwner, none), __NFUN_114__(P, none)), __NFUN_114__(P.PlayerReplicationInfo, none)), PlayerOwner.IsSpectating()))
				{
					DrawSpectatingHud(Canvas);					
				}
				else
				{
					// End:0x24B
					if(__NFUN_129__(P.bHideRegularHUD))
					{
						DrawHUD(Canvas);
					}
				}
				// End:0x294
				if(__NFUN_129__(DrawLevelAction(Canvas)))
				{
					// End:0x294
					if(__NFUN_130__(__NFUN_119__(PlayerOwner, none), __NFUN_177__(PlayerOwner.ProgressTimeOut, Level.TimeSeconds)))
					{
						DisplayProgressMessage(Canvas);
					}
				}
				DisplayMessages(Canvas);
			}
		}
	}
	PlayerOwner.RenderOverlays(Canvas);
	return;
}

function DrawSpectatingHud(Canvas C)
{
	return;
}

simulated function DrawRoute()
{
	local int i;
	local Controller C;
	local Vector Start, End, RealStart;
	local bool bPath;

	C = Pawn(PlayerOwner.ViewTarget).Controller;
	// End:0x2F
	if(__NFUN_114__(C, none))
	{
		return;
	}
	// End:0x6C
	if(__NFUN_119__(C.CurrentPath, none))
	{
		Start = C.CurrentPath.Start.Location;		
	}
	else
	{
		Start = PlayerOwner.ViewTarget.Location;
	}
	RealStart = Start;
	// End:0xFE
	if(C.bAdjusting)
	{
		Draw3DLine(C.Pawn.Location, C.AdjustLoc, Class'Engine.Canvas'.static.MakeColor(byte(255), 0, byte(255)));
		Start = C.AdjustLoc;
	}
	// End:0x2DC
	if(__NFUN_132__(__NFUN_114__(C, PlayerOwner), __NFUN_130__(__NFUN_114__(C.MoveTarget, C.RouteCache[0]), __NFUN_119__(C.MoveTarget, none))))
	{
		// End:0x1F8
		if(__NFUN_130__(__NFUN_114__(C, PlayerOwner), __NFUN_218__(C.Destination, vect(0.0000000, 0.0000000, 0.0000000))))
		{
			// End:0x1DE
			if(C.__NFUN_521__(C.Destination))
			{
				Draw3DLine(C.Pawn.Location, C.Destination, Class'Engine.Canvas'.static.MakeColor(byte(255), byte(255), byte(255)));
				return;
			}
			C.__NFUN_518__(C.Destination);
		}
		i = 0;
		J0x1FF:

		// End:0x29C [Loop If]
		if(__NFUN_150__(i, 16))
		{
			// End:0x228
			if(__NFUN_114__(C.RouteCache[i], none))
			{
				// [Explicit Break]
				goto J0x29C;
			}
			bPath = true;
			Draw3DLine(Start, C.RouteCache[i].Location, Class'Engine.Canvas'.static.MakeColor(0, byte(255), 0));
			Start = C.RouteCache[i].Location;
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x1FF;
		}
		J0x29C:

		// End:0x2D9
		if(bPath)
		{
			Draw3DLine(RealStart, C.Destination, Class'Engine.Canvas'.static.MakeColor(byte(255), byte(255), byte(255)));
		}		
	}
	else
	{
		// End:0x339
		if(__NFUN_218__(PlayerOwner.ViewTarget.Velocity, vect(0.0000000, 0.0000000, 0.0000000)))
		{
			Draw3DLine(RealStart, C.Destination, Class'Engine.Canvas'.static.MakeColor(byte(255), byte(255), byte(255)));
		}
	}
	// End:0x34A
	if(__NFUN_114__(C, PlayerOwner))
	{
		return;
	}
	// End:0x37E
	if(__NFUN_119__(C.Focus, none))
	{
		End = C.Focus.Location;		
	}
	else
	{
		End = C.FocalPoint;
	}
	Draw3DLine(__NFUN_215__(PlayerOwner.ViewTarget.Location, __NFUN_213__(Pawn(PlayerOwner.ViewTarget).BaseEyeHeight, vect(0.0000000, 0.0000000, 1.0000000))), End, Class'Engine.Canvas'.static.MakeColor(byte(255), 0, 0));
	return;
}

function DrawHUD(Canvas Canvas)
{
	return;
}

function PrintActionMessage(Canvas C, string BigMessage)
{
	local float XL, YL;

	// End:0x1C
	if(__NFUN_151__(__NFUN_125__(BigMessage), 10))
	{
		UseLargeFont(C);		
	}
	else
	{
		UseHugeFont(C);
	}
	C.bCenter = false;
	C.__NFUN_464__(BigMessage, XL, YL);
	C.SetPos(__NFUN_174__(__NFUN_171__(0.5000000, __NFUN_175__(C.ClipX, XL)), float(1)), __NFUN_174__(__NFUN_175__(__NFUN_171__(0.6600000, C.ClipY), __NFUN_171__(YL, 0.5000000)), float(1)));
	C.SetDrawColor(0, 0, 0);
	C.__NFUN_465__(BigMessage, false);
	C.SetPos(__NFUN_171__(0.5000000, __NFUN_175__(C.ClipX, XL)), __NFUN_175__(__NFUN_171__(0.6600000, C.ClipY), __NFUN_171__(YL, 0.5000000)));
	C.SetDrawColor(0, 0, byte(255));
	C.__NFUN_465__(BigMessage, false);
	return;
}

simulated function DisplayProgressMessage(Canvas C)
{
	local int i, LineCount;
	local GameReplicationInfo GRI;
	local float FontDX, FontDY, X, Y;
	local int Alpha;
	local float TimeLeft;

	TimeLeft = __NFUN_175__(PlayerOwner.ProgressTimeOut, Level.TimeSeconds);
	// End:0x3E
	if(__NFUN_179__(TimeLeft, ProgressFadeTime))
	{
		Alpha = 255;		
	}
	else
	{
		Alpha = int(__NFUN_172__(float(int(__NFUN_171__(float(255), TimeLeft))), ProgressFadeTime));
	}
	GRI = PlayerOwner.GameReplicationInfo;
	LineCount = 0;
	i = 0;
	J0x7E:

	// End:0xB9 [Loop If]
	if(__NFUN_150__(i, 4))
	{
		// End:0xA8
		if(__NFUN_122__(PlayerOwner.ProgressMessage[i], ""))
		{
			// [Explicit Continue]
			goto J0xAF;
		}
		__NFUN_165__(LineCount);
		J0xAF:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x7E;
	}
	// End:0x134
	if(__NFUN_119__(GRI, none))
	{
		// End:0xE0
		if(__NFUN_123__(GRI.MOTDLine1, ""))
		{
			__NFUN_165__(LineCount);
		}
		// End:0xFC
		if(__NFUN_123__(GRI.MOTDLine2, ""))
		{
			__NFUN_165__(LineCount);
		}
		// End:0x118
		if(__NFUN_123__(GRI.MOTDLine3, ""))
		{
			__NFUN_165__(LineCount);
		}
		// End:0x134
		if(__NFUN_123__(GRI.MOTDLine4, ""))
		{
			__NFUN_165__(LineCount);
		}
	}
	C.Font = LoadProgressFont();
	C.Style = 5;
	C.__NFUN_470__("A", FontDX, FontDY);
	X = __NFUN_174__(__NFUN_171__(__NFUN_171__(0.5000000, HudCanvasScale), float(C.SizeX)), __NFUN_171__(__NFUN_172__(__NFUN_175__(1.0000000, HudCanvasScale), 2.0000000), float(C.SizeX)));
	Y = __NFUN_174__(__NFUN_171__(__NFUN_171__(0.5000000, HudCanvasScale), float(C.SizeY)), __NFUN_171__(__NFUN_172__(__NFUN_175__(1.0000000, HudCanvasScale), 2.0000000), float(C.SizeY)));
	__NFUN_185__(Y, __NFUN_171__(FontDY, __NFUN_172__(float(LineCount), 2.0000000)));
	i = 0;
	J0x22C:

	// End:0x31B [Loop If]
	if(__NFUN_150__(i, 4))
	{
		// End:0x256
		if(__NFUN_122__(PlayerOwner.ProgressMessage[i], ""))
		{
			// [Explicit Continue]
			goto J0x311;
		}
		C.DrawColor = PlayerOwner.ProgressColor[i];
		C.DrawColor.A = byte(Alpha);
		C.__NFUN_470__(PlayerOwner.ProgressMessage[i], FontDX, FontDY);
		C.SetPos(__NFUN_175__(X, __NFUN_172__(FontDX, 2.0000000)), Y);
		C.__NFUN_465__(PlayerOwner.ProgressMessage[i]);
		__NFUN_184__(Y, FontDY);
		J0x311:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x22C;
	}
	// End:0x588
	if(__NFUN_130__(__NFUN_119__(GRI, none), __NFUN_155__(int(Level.NetMode), int(NM_Standalone))))
	{
		C.DrawColor = MOTDColor;
		C.DrawColor.A = byte(Alpha);
		// End:0x3F6
		if(__NFUN_123__(GRI.MOTDLine1, ""))
		{
			C.__NFUN_470__(GRI.MOTDLine1, FontDX, FontDY);
			C.SetPos(__NFUN_175__(X, __NFUN_172__(FontDX, 2.0000000)), Y);
			C.__NFUN_465__(GRI.MOTDLine1);
			__NFUN_184__(Y, FontDY);
		}
		// End:0x47C
		if(__NFUN_123__(GRI.MOTDLine2, ""))
		{
			C.__NFUN_470__(GRI.MOTDLine2, FontDX, FontDY);
			C.SetPos(__NFUN_175__(X, __NFUN_172__(FontDX, 2.0000000)), Y);
			C.__NFUN_465__(GRI.MOTDLine2);
			__NFUN_184__(Y, FontDY);
		}
		// End:0x502
		if(__NFUN_123__(GRI.MOTDLine3, ""))
		{
			C.__NFUN_470__(GRI.MOTDLine3, FontDX, FontDY);
			C.SetPos(__NFUN_175__(X, __NFUN_172__(FontDX, 2.0000000)), Y);
			C.__NFUN_465__(GRI.MOTDLine3);
			__NFUN_184__(Y, FontDY);
		}
		// End:0x588
		if(__NFUN_123__(GRI.MOTDLine4, ""))
		{
			C.__NFUN_470__(GRI.MOTDLine4, FontDX, FontDY);
			C.SetPos(__NFUN_175__(X, __NFUN_172__(FontDX, 2.0000000)), Y);
			C.__NFUN_465__(GRI.MOTDLine4);
			__NFUN_184__(Y, FontDY);
		}
	}
	return;
}

function bool DrawLevelAction(Canvas C)
{
	local string BigMessage;

	// End:0x72
	if(__NFUN_154__(int(Level.LevelAction), int(0)))
	{
		// End:0x65
		if(__NFUN_130__(__NFUN_119__(Level.Pauser, none), __NFUN_177__(Level.TimeSeconds, __NFUN_174__(Level.PauseDelay, 0.2000000))))
		{
			BigMessage = PausedMessage;			
		}
		else
		{
			BigMessage = "";
			return false;
		}		
	}
	else
	{
		// End:0x99
		if(__NFUN_154__(int(Level.LevelAction), int(1)))
		{
			BigMessage = LoadingMessage;			
		}
		else
		{
			// End:0xC0
			if(__NFUN_154__(int(Level.LevelAction), int(2)))
			{
				BigMessage = SavingMessage;				
			}
			else
			{
				// End:0xE7
				if(__NFUN_154__(int(Level.LevelAction), int(3)))
				{
					BigMessage = ConnectingMessage;					
				}
				else
				{
					// End:0x10B
					if(__NFUN_154__(int(Level.LevelAction), int(4)))
					{
						BigMessage = PrecachingMessage;
					}
				}
			}
		}
	}
	// End:0x145
	if(__NFUN_123__(BigMessage, ""))
	{
		C.Style = 1;
		UseLargeFont(C);
		PrintActionMessage(C, BigMessage);
		return true;
	}
	return false;
	return;
}

function DisplayBadConnectionAlert()
{
	return;
}

simulated function Message(PlayerReplicationInfo PRI, coerce string Msg, name MsgType)
{
	// End:0x18
	if(bMessageBeep)
	{
		PlayerOwner.PlayBeepSound();
	}
	// End:0x59
	if(__NFUN_132__(__NFUN_254__(MsgType, 'Say'), __NFUN_254__(MsgType, 'TeamSay')))
	{
		Msg = __NFUN_112__(__NFUN_112__(PRI.PlayerName, ": "), Msg);
	}
	AddTextMessage(Msg, Class'Engine.LocalMessage', PRI);
	return;
}

function DisplayPortrait(PlayerReplicationInfo PRI)
{
	return;
}

simulated function LocalizedMessage(Class<LocalMessage> Message, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject, optional string CriticalString)
{
	return;
}

simulated function PlayReceivedMessage(string S, string PName, ZoneInfo PZone)
{
	PlayerOwner.ClientMessage(S);
	// End:0x2C
	if(bMessageBeep)
	{
		PlayerOwner.PlayBeepSound();
	}
	return;
}

function bool ProcessKeyEvent(int Key, int Action, float Delta)
{
	// End:0x2A
	if(__NFUN_119__(nextHUD, none))
	{
		return nextHUD.ProcessKeyEvent(Key, Action, Delta);
	}
	return false;
	return;
}

function DisplayMessages(Canvas C)
{
	local int i, j, XPos, YPos, MessageCount;

	local float XL, YL;

	i = 0;
	J0x07:

	// End:0xE6 [Loop If]
	if(__NFUN_150__(i, ConsoleMessageCount))
	{
		// End:0x33
		if(__NFUN_122__(TextMessages[i].Text, ""))
		{
			// [Explicit Break]
			goto J0xE6;
			// [Explicit Continue]
			goto J0xDC;
		}
		// End:0xD5
		if(__NFUN_176__(TextMessages[i].MessageLife, Level.TimeSeconds))
		{
			TextMessages[i].Text = "";
			// End:0xBC
			if(__NFUN_150__(i, __NFUN_147__(ConsoleMessageCount, 1)))
			{
				j = i;
				J0x86:

				// End:0xBC [Loop If]
				if(__NFUN_150__(j, __NFUN_147__(ConsoleMessageCount, 1)))
				{
					TextMessages[j] = TextMessages[__NFUN_146__(j, 1)];
					__NFUN_165__(j);
					// [Loop Continue]
					goto J0x86;
				}
			}
			TextMessages[j].Text = "";
			// [Explicit Break]
			goto J0xE6;
			// [Explicit Continue]
			goto J0xDC;
		}
		__NFUN_165__(MessageCount);
		J0xDC:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	J0xE6:

	XPos = int(__NFUN_174__(float(int(__NFUN_171__(__NFUN_171__(ConsoleMessagePosX, HudCanvasScale), float(C.SizeX)))), __NFUN_171__(__NFUN_172__(__NFUN_175__(1.0000000, HudCanvasScale), 2.0000000), float(C.SizeX))));
	YPos = int(__NFUN_174__(float(int(__NFUN_171__(__NFUN_171__(ConsoleMessagePosY, HudCanvasScale), float(C.SizeY)))), __NFUN_171__(__NFUN_172__(__NFUN_175__(1.0000000, HudCanvasScale), 2.0000000), float(C.SizeY))));
	C.Font = GetConsoleFont(C);
	C.DrawColor = ConsoleColor;
	C.__NFUN_470__("A", XL, YL);
	__NFUN_162__(YPos, int(__NFUN_174__(__NFUN_171__(YL, float(MessageCount)), float(1))));
	__NFUN_162__(YPos, int(YL));
	i = 0;
	J0x200:

	// End:0x2C0 [Loop If]
	if(__NFUN_150__(i, MessageCount))
	{
		// End:0x229
		if(__NFUN_122__(TextMessages[i].Text, ""))
		{
			// [Explicit Break]
			goto J0x2C0;
		}
		C.__NFUN_464__(TextMessages[i].Text, XL, YL);
		C.SetPos(float(XPos), float(YPos));
		C.DrawColor = TextMessages[i].TextColor;
		C.__NFUN_465__(TextMessages[i].Text, false);
		__NFUN_161__(YPos, int(YL));
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x200;
	}
	J0x2C0:

	return;
}

function AddTextMessage(string M, Class<LocalMessage> MessageClass, PlayerReplicationInfo PRI)
{
	local int i;

	// End:0x2C
	if(__NFUN_130__(bMessageBeep, MessageClass.default.bBeep))
	{
		PlayerOwner.PlayBeepSound();
	}
	i = 0;
	J0x33:

	// End:0x66 [Loop If]
	if(__NFUN_150__(i, ConsoleMessageCount))
	{
		// End:0x5C
		if(__NFUN_122__(TextMessages[i].Text, ""))
		{
			// [Explicit Break]
			goto J0x66;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x33;
	}
	J0x66:

	// End:0xB2
	if(__NFUN_154__(i, ConsoleMessageCount))
	{
		i = 0;
		J0x7C:

		// End:0xB2 [Loop If]
		if(__NFUN_150__(i, __NFUN_147__(ConsoleMessageCount, 1)))
		{
			TextMessages[i] = TextMessages[__NFUN_146__(i, 1)];
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x7C;
		}
	}
	TextMessages[i].Text = M;
	TextMessages[i].MessageLife = __NFUN_174__(Level.TimeSeconds, float(MessageClass.default.LifeTime));
	TextMessages[i].TextColor = MessageClass.static.GetConsoleColor(PRI);
	TextMessages[i].PRI = PRI;
	return;
}

function UseSmallFont(Canvas Canvas)
{
	// End:0x31
	if(__NFUN_178__(Canvas.ClipX, float(640)))
	{
		Canvas.Font = SmallFont;		
	}
	else
	{
		Canvas.Font = MedFont;
	}
	return;
}

function UseMediumFont(Canvas Canvas)
{
	// End:0x31
	if(__NFUN_178__(Canvas.ClipX, float(640)))
	{
		Canvas.Font = MedFont;		
	}
	else
	{
		Canvas.Font = BigFont;
	}
	return;
}

function UseLargeFont(Canvas Canvas)
{
	// End:0x31
	if(__NFUN_178__(Canvas.ClipX, float(640)))
	{
		Canvas.Font = BigFont;		
	}
	else
	{
		Canvas.Font = LargeFont;
	}
	return;
}

function UseHugeFont(Canvas Canvas)
{
	Canvas.Font = LargeFont;
	return;
}

static function Font LoadFontStatic(int i)
{
	// End:0x90
	if(__NFUN_114__(default.FontArrayFonts[i], none))
	{
		default.FontArrayFonts[i] = Font(DynamicLoadObject(default.FontArrayNames[i], Class'Engine.Font'));
		// End:0x90
		if(__NFUN_114__(default.FontArrayFonts[i], none))
		{
			__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Warning: ", string(default.Class)), " Couldn't dynamically load font "), default.FontArrayNames[i]));
		}
	}
	return default.FontArrayFonts[i];
	return;
}

simulated function Font LoadFont(int i)
{
	// End:0x8C
	if(__NFUN_114__(FontArrayFonts[i], none))
	{
		FontArrayFonts[i] = Font(DynamicLoadObject(FontArrayNames[i], Class'Engine.Font'));
		// End:0x8C
		if(__NFUN_114__(FontArrayFonts[i], none))
		{
			__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Warning: ", string(self)), " Couldn't dynamically load font "), FontArrayNames[i]));
		}
	}
	return FontArrayFonts[i];
	return;
}

static function Font GetConsoleFont(Canvas C)
{
	local int FontSize;

	FontSize = default.ConsoleFontSize;
	// End:0x2C
	if(__NFUN_176__(C.ClipX, float(640)))
	{
		__NFUN_165__(FontSize);
	}
	// End:0x4D
	if(__NFUN_176__(C.ClipX, float(800)))
	{
		__NFUN_165__(FontSize);
	}
	// End:0x6E
	if(__NFUN_176__(C.ClipX, float(1024)))
	{
		__NFUN_165__(FontSize);
	}
	// End:0x8F
	if(__NFUN_176__(C.ClipX, float(1280)))
	{
		__NFUN_165__(FontSize);
	}
	// End:0xB0
	if(__NFUN_176__(C.ClipX, float(1600)))
	{
		__NFUN_165__(FontSize);
	}
	return LoadFontStatic(__NFUN_249__(8, FontSize));
	return;
}

static function Font GetMediumFontFor(Canvas Canvas)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x49 [Loop If]
	if(__NFUN_150__(i, 8))
	{
		// End:0x3F
		if(__NFUN_178__(float(default.FontScreenWidthMedium[i]), Canvas.ClipX))
		{
			return LoadFontStatic(i);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return LoadFontStatic(8);
	return;
}

static function Font LargerFontThan(Font aFont)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x45 [Loop If]
	if(__NFUN_150__(i, 7))
	{
		// End:0x3B
		if(__NFUN_114__(LoadFontStatic(i), aFont))
		{
			return LoadFontStatic(__NFUN_250__(0, __NFUN_147__(i, 4)));
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return LoadFontStatic(5);
	return;
}

simulated function Font LoadProgressFont()
{
	// End:0x79
	if(__NFUN_114__(ProgressFontFont, none))
	{
		ProgressFontFont = Font(DynamicLoadObject(ProgressFontName, Class'Engine.Font'));
		// End:0x79
		if(__NFUN_114__(ProgressFontFont, none))
		{
			__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Warning: ", string(self)), " Couldn't dynamically load font "), ProgressFontName));
			ProgressFontFont = Font'Engine.DefaultFont';
		}
	}
	return ProgressFontFont;
	return;
}

simulated function DrawTargeting(Canvas C)
{
	return;
}

defaultproperties
{
	SmallFont=Font'Engine.SmallFont'
	MedFont=Font'Engine.SmallFont'
	BigFont=Font'Engine.SmallFont'
	LargeFont=Font'Engine.SmallFont'
	ProgressFontName="UT2003Fonts.FontEurostile12"
	ProgressFadeTime=1.0000000
	MOTDColor=(R=255,G=255,B=255,A=255)
	bMessageBeep=true
	HudCanvasScale=0.9500000
	LoadingMessage="LOADING"
	SavingMessage="SAVING"
	ConnectingMessage="CONNECTING"
	PausedMessage="PAUSED"
	ConsoleColor=(R=153,G=216,B=253,A=255)
	ConsoleMessageCount=4
	ConsoleFontSize=5
	FontArrayNames[0]="Engine.DefaultFont"
	FontArrayNames[1]="Engine.DefaultFont"
	FontArrayNames[2]="Engine.DefaultFont"
	FontArrayNames[3]="Engine.DefaultFont"
	FontArrayNames[4]="Engine.DefaultFont"
	FontArrayNames[5]="Engine.DefaultFont"
	FontArrayNames[6]="Engine.DefaultFont"
	FontArrayNames[7]="Engine.DefaultFont"
	FontArrayNames[8]="Engine.DefaultFont"
	bHidden=true
	RemoteRole=0
}