class ScoreBoard extends Info
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var() GameReplicationInfo GRI;
var() Class<HUD> HudClass;
var bool bDisplayMessages;

simulated function PostBeginPlay()
{
	super(Actor).PostBeginPlay();
	InitGRI();
	Init();
	return;
}

function Font GetSmallerFontFor(Canvas Canvas, int offset)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x69 [Loop If]
	if(__NFUN_150__(i, __NFUN_147__(8, offset)))
	{
		// End:0x5F
		if(__NFUN_178__(float(HudClass.default.FontScreenWidthMedium[i]), Canvas.ClipX))
		{
			return HudClass.static.LoadFontStatic(__NFUN_146__(i, offset));
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return HudClass.static.LoadFontStatic(8);
	return;
}

function bool HaveHalfFont(Canvas Canvas, int offset)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x4F [Loop If]
	if(__NFUN_150__(i, __NFUN_147__(9, offset)))
	{
		// End:0x45
		if(__NFUN_178__(float(HudClass.default.FontScreenWidthSmall[i]), Canvas.ClipX))
		{
			return true;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return false;
	return;
}

function Font GetSmallFontFor(int ScreenWidth, int offset)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x5E [Loop If]
	if(__NFUN_150__(i, __NFUN_147__(8, offset)))
	{
		// End:0x54
		if(__NFUN_152__(HudClass.default.FontScreenWidthSmall[i], ScreenWidth))
		{
			return HudClass.static.LoadFontStatic(__NFUN_146__(i, offset));
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return HudClass.static.LoadFontStatic(8);
	return;
}

simulated function InitGRI()
{
	GRI = PlayerController(Owner).GameReplicationInfo;
	return;
}

simulated function string InitTitle()
{
	return __NFUN_235__(GRI.GameName);
	return;
}

simulated function Init()
{
	return;
}

simulated event DrawScoreboard(Canvas C)
{
	UpdateGRI();
	UpdateScoreBoard(C);
	return;
}

function bool UpdateGRI()
{
	// End:0x1E
	if(__NFUN_114__(GRI, none))
	{
		InitGRI();
		// End:0x1E
		if(__NFUN_114__(GRI, none))
		{
			return false;
		}
	}
	SortPRIArray();
	return true;
	return;
}

simulated function string FormatTime(int Seconds)
{
	local int Minutes, Hours;
	local string Time;

	// End:0x46
	if(__NFUN_151__(Seconds, 3600))
	{
		Hours = __NFUN_145__(Seconds, 3600);
		__NFUN_162__(Seconds, __NFUN_144__(Hours, 3600));
		Time = __NFUN_112__(string(Hours), ":");
	}
	Minutes = __NFUN_145__(Seconds, 60);
	__NFUN_162__(Seconds, __NFUN_144__(Minutes, 60));
	// End:0x8D
	if(__NFUN_153__(Minutes, 10))
	{
		Time = __NFUN_112__(__NFUN_112__(Time, string(Minutes)), ":");		
	}
	else
	{
		Time = __NFUN_112__(__NFUN_112__(__NFUN_112__(Time, "0"), string(Minutes)), ":");
	}
	// End:0xCE
	if(__NFUN_153__(Seconds, 10))
	{
		Time = __NFUN_112__(Time, string(Seconds));		
	}
	else
	{
		Time = __NFUN_112__(__NFUN_112__(Time, "0"), string(Seconds));
	}
	return Time;
	return;
}

simulated function UpdateScoreBoard(Canvas Canvas)
{
	return;
}

simulated function bool InOrder(PlayerReplicationInfo P1, PlayerReplicationInfo P2)
{
	// End:0x2E
	if(P1.bOnlySpectator)
	{
		// End:0x29
		if(P2.bOnlySpectator)
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
		// End:0x42
		if(P2.bOnlySpectator)
		{
			return true;
		}
	}
	// End:0x65
	if(__NFUN_176__(P1.Score, P2.Score))
	{
		return false;
	}
	// End:0x110
	if(__NFUN_180__(P1.Score, P2.Score))
	{
		// End:0xA9
		if(__NFUN_177__(P1.Deaths, P2.Deaths))
		{
			return false;
		}
		// End:0x110
		if(__NFUN_130__(__NFUN_130__(__NFUN_180__(P1.Deaths, P2.Deaths), __NFUN_119__(PlayerController(P2.Owner), none)), __NFUN_119__(Viewport(PlayerController(P2.Owner).Player), none)))
		{
			return false;
		}
	}
	return true;
	return;
}

simulated function SortPRIArray()
{
	local int i, j;
	local PlayerReplicationInfo tmp;

	i = 0;
	J0x07:

	// End:0xEE [Loop If]
	if(__NFUN_150__(i, __NFUN_147__(GRI.PRIArray.Length, 1)))
	{
		j = __NFUN_146__(i, 1);
		J0x31:

		// End:0xE4 [Loop If]
		if(__NFUN_150__(j, GRI.PRIArray.Length))
		{
			// End:0xDA
			if(__NFUN_129__(InOrder(GRI.PRIArray[i], GRI.PRIArray[j])))
			{
				tmp = GRI.PRIArray[i];
				GRI.PRIArray[i] = GRI.PRIArray[j];
				GRI.PRIArray[j] = tmp;
			}
			__NFUN_165__(j);
			// [Loop Continue]
			goto J0x31;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

defaultproperties
{
	HudClass=Class'Engine.HUD'
}