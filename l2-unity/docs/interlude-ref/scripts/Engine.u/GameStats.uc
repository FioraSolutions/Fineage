class GameStats extends Info
	native
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var FileLog TempLog;
var GameReplicationInfo GRI;
var bool bShowBots;

// Export UGameStats::execGetStatsIdentifier(FFrame&, void* const)
native final function string GetStatsIdentifier(Controller C);

// Export UGameStats::execGetMapFileName(FFrame&, void* const)
native final function string GetMapFileName();

function Init()
{
	TempLog = __NFUN_278__(Class'Engine.FileLog');
	// End:0x55
	if(__NFUN_119__(TempLog, none))
	{
		TempLog.OpenLog("Stats");
		__NFUN_231__("Output Game stats to: STATS.TXT");		
	}
	else
	{
		__NFUN_231__("Could not spawn Temporary Stats log");
		__NFUN_279__();
	}
	return;
}

function Shutdown()
{
	// End:0x17
	if(__NFUN_119__(TempLog, none))
	{
		TempLog.__NFUN_279__();
	}
	return;
}

function Logf(string LogString)
{
	// End:0x1F
	if(__NFUN_119__(TempLog, none))
	{
		TempLog.super(GameStats).Logf(LogString);
	}
	return;
}

event PostBeginPlay()
{
	super(Actor).PostBeginPlay();
	Init();
	return;
}

event Destroyed()
{
	Shutdown();
	super(Actor).Destroyed();
	return;
}

function string TimeStamp()
{
	local string Seconds;

	Seconds = __NFUN_112__("", string(Level.TimeSeconds));
	// End:0x45
	if(__NFUN_155__(__NFUN_126__(Seconds, "."), -1))
	{
		Seconds = __NFUN_128__(Seconds, __NFUN_126__(Seconds, "."));
	}
	return Seconds;
	return;
}

function string Header()
{
	return __NFUN_112__(__NFUN_112__("", TimeStamp()), __NFUN_236__(9));
	return;
}

function string FullTimeDate()
{
	return __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("", string(Level.Year)), "-"), string(Level.Month)), "-"), string(Level.Day)), " "), string(Level.Hour)), ":"), string(Level.Minute)), ":"), string(Level.Second));
	return;
}

function string TimeZone()
{
	return "0";
	return;
}

function string MapName()
{
	local string MapName, tab;

	tab = __NFUN_112__("", __NFUN_236__(9));
	MapName = __NFUN_112__("", GetMapFileName());
	// End:0x4F
	if(__NFUN_155__(__NFUN_126__(MapName, ".ut2"), -1))
	{
		MapName = __NFUN_128__(MapName, __NFUN_126__(MapName, ".ut2"));
	}
	ReplaceText(MapName, tab, "_");
	return MapName;
	return;
}

function NewGame()
{
	local string Out, tmp, tab, ngTitle, ngAuthor, ngGameGameName;

	local int i;
	local Mutator MyMutie;
	local GameRules MyRules;

	tab = __NFUN_112__("", __NFUN_236__(9));
	ngTitle = Level.Title;
	ngAuthor = Level.Author;
	ngGameGameName = Level.Game.GameName;
	ReplaceText(ngTitle, tab, "_");
	ReplaceText(ngAuthor, tab, "_");
	ReplaceText(ngGameGameName, tab, "_");
	GRI = Level.Game.GameReplicationInfo;
	Out = __NFUN_112__(__NFUN_112__(Header(), "NG"), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, FullTimeDate()), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, TimeZone()), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, MapName()), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, ngTitle), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, ngAuthor), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, string(Level.Game.Class)), __NFUN_236__(9));
	Out = __NFUN_112__(Out, ngGameGameName);
	tmp = "";
	i = 0;
	// End:0x1EC
	foreach __NFUN_304__(Class'Engine.Mutator', MyMutie)
	{
		// End:0x1CA
		if(__NFUN_123__(tmp, ""))
		{
			tmp = __NFUN_112__(__NFUN_112__(tmp, "|"), string(MyMutie.Class));			
		}
		else
		{
			tmp = __NFUN_112__("", string(MyMutie.Class));
		}
		__NFUN_165__(i);		
	}	
	// End:0x250
	foreach __NFUN_304__(Class'Engine.GameRules', MyRules)
	{
		// End:0x22E
		if(__NFUN_123__(tmp, ""))
		{
			tmp = __NFUN_112__(__NFUN_112__(tmp, "|"), string(MyRules.Class));			
		}
		else
		{
			tmp = __NFUN_112__("", string(MyRules.Class));
		}
		__NFUN_165__(i);		
	}	
	// End:0x294
	if(__NFUN_151__(i, 0))
	{
		ReplaceText(tmp, tab, "_");
		Out = __NFUN_112__(__NFUN_112__(__NFUN_112__(Out, __NFUN_236__(9)), "Mutators="), tmp);
	}
	Logf(Out);
	return;
}

function ServerInfo()
{
	local string Out, flags, tab, siServerName, siAdminName, siAdminEmail;

	local ServerResponseLine ServerState;
	local int i;

	tab = __NFUN_112__("", __NFUN_236__(9));
	siServerName = GRI.ServerName;
	siAdminName = GRI.AdminName;
	siAdminEmail = GRI.AdminEmail;
	ReplaceText(siServerName, tab, "_");
	ReplaceText(siAdminName, tab, "_");
	ReplaceText(siAdminEmail, tab, "_");
	Out = __NFUN_112__(__NFUN_112__(Header(), "SI"), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, siServerName), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, TimeZone()), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, siAdminName), __NFUN_236__(9));
	Out = __NFUN_112__(__NFUN_112__(Out, siAdminEmail), __NFUN_236__(9));
	Out = __NFUN_112__(Out, __NFUN_236__(9));
	flags = "";
	Level.Game.GetServerDetails(ServerState);
	i = 0;
	J0x139:

	// End:0x19B [Loop If]
	if(__NFUN_150__(i, ServerState.ServerInfo.Length))
	{
		flags = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(flags, "\\"), ServerState.ServerInfo[i].Key), "\\"), ServerState.ServerInfo[i].Value);
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x139;
	}
	ReplaceText(flags, tab, "_");
	Out = __NFUN_112__(Out, flags);
	Logf(Out);
	return;
}

function StartGame()
{
	Logf(__NFUN_112__(__NFUN_112__("", Header()), "SG"));
	return;
}

function EndGame(string Reason)
{
	local string Out;
	local int i, j;
	local GameReplicationInfo GRI;
	local array<PlayerReplicationInfo> pris;
	local PlayerReplicationInfo PRI, t;

	Out = __NFUN_112__(__NFUN_112__(__NFUN_112__(Header(), "EG"), __NFUN_236__(9)), Reason);
	GRI = Level.Game.GameReplicationInfo;
	i = 0;
	J0x43:

	// End:0x17F [Loop If]
	if(__NFUN_150__(i, GRI.PRIArray.Length))
	{
		PRI = GRI.PRIArray[i];
		// End:0x175
		if(__NFUN_129__(PRI.bOnlySpectator))
		{
			pris.Length = __NFUN_146__(pris.Length, 1);
			j = 0;
			J0xA1:

			// End:0x164 [Loop If]
			if(__NFUN_150__(j, __NFUN_147__(pris.Length, 1)))
			{
				// End:0x15A
				if(__NFUN_132__(__NFUN_176__(pris[j].Score, PRI.Score), __NFUN_130__(__NFUN_180__(pris[j].Score, PRI.Score), __NFUN_177__(pris[j].Deaths, PRI.Deaths))))
				{
					t = pris[j];
					pris[j] = PRI;
					PRI = t;
				}
				__NFUN_165__(j);
				// [Loop Continue]
				goto J0xA1;
			}
			pris[j] = PRI;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x43;
	}
	i = 0;
	J0x186:

	// End:0x1D7 [Loop If]
	if(__NFUN_150__(i, pris.Length))
	{
		Out = __NFUN_112__(__NFUN_112__(Out, __NFUN_236__(9)), string(Controller(pris[i].Owner).PlayerNum));
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x186;
	}
	Logf(Out);
	return;
}

function ConnectEvent(PlayerReplicationInfo Who)
{
	local string Out;

	// End:0x35
	if(__NFUN_132__(__NFUN_130__(Who.bBot, __NFUN_129__(bShowBots)), Who.bOnlySpectator))
	{
		return;
	}
	Out = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("", Header()), "C"), __NFUN_236__(9)), string(Controller(Who.Owner).PlayerNum)), __NFUN_236__(9));
	Out = __NFUN_112__(Out, GetStatsIdentifier(Controller(Who.Owner)));
	Logf(Out);
	return;
}

function DisconnectEvent(PlayerReplicationInfo Who)
{
	local string Out;

	// End:0x35
	if(__NFUN_132__(__NFUN_130__(Who.bBot, __NFUN_129__(bShowBots)), Who.bOnlySpectator))
	{
		return;
	}
	Out = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("", Header()), "D"), __NFUN_236__(9)), string(Controller(Who.Owner).PlayerNum));
	Logf(Out);
	return;
}

function ScoreEvent(PlayerReplicationInfo Who, float Points, string Desc)
{
	// End:0x35
	if(__NFUN_132__(__NFUN_130__(Who.bBot, __NFUN_129__(bShowBots)), Who.bOnlySpectator))
	{
		return;
	}
	Logf(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("", Header()), "S"), __NFUN_236__(9)), string(Controller(Who.Owner).PlayerNum)), __NFUN_236__(9)), string(Points)), __NFUN_236__(9)), Desc));
	return;
}

function TeamScoreEvent(int Team, float Points, string Desc)
{
	Logf(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("", Header()), "T"), __NFUN_236__(9)), string(Team)), __NFUN_236__(9)), string(Points)), __NFUN_236__(9)), Desc));
	return;
}

function KillEvent(string KillType, PlayerReplicationInfo Killer, PlayerReplicationInfo Victim, Class<DamageType> Damage)
{
	local string Out;

	// End:0x49
	if(__NFUN_132__(__NFUN_132__(__NFUN_130__(Victim.bBot, __NFUN_129__(bShowBots)), Killer.bOnlySpectator), Victim.bOnlySpectator))
	{
		return;
	}
	Out = __NFUN_112__(__NFUN_112__(__NFUN_112__("", Header()), KillType), __NFUN_236__(9));
	// End:0xE6
	if(__NFUN_119__(Killer, none))
	{
		// End:0x92
		if(__NFUN_130__(Killer.bBot, __NFUN_129__(bShowBots)))
		{
			return;
		}
		Out = __NFUN_112__(__NFUN_112__(Out, string(Controller(Killer.Owner).PlayerNum)), __NFUN_236__(9));
		Out = __NFUN_112__(__NFUN_112__(Out, GetItemName(string(Damage))), __NFUN_236__(9));		
	}
	else
	{
		Out = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(Out, "-1"), __NFUN_236__(9)), GetItemName(string(Damage))), __NFUN_236__(9));
	}
	Out = __NFUN_112__(__NFUN_112__(__NFUN_112__(Out, string(Controller(Victim.Owner).PlayerNum)), __NFUN_236__(9)), GetItemName(string(Controller(Victim.Owner).GetLastWeapon())));
	// End:0x1E6
	if(__NFUN_130__(__NFUN_119__(PlayerController(Victim.Owner), none), PlayerController(Victim.Owner).bIsTyping))
	{
		// End:0x1E6
		if(__NFUN_119__(PlayerController(Killer.Owner), PlayerController(Victim.Owner)))
		{
			SpecialEvent(Killer, "type_kill");
		}
	}
	Logf(Out);
	return;
}

function SpecialEvent(PlayerReplicationInfo Who, string Desc)
{
	local string Out;

	// End:0x6B
	if(__NFUN_119__(Who, none))
	{
		// End:0x40
		if(__NFUN_132__(__NFUN_130__(Who.bBot, __NFUN_129__(bShowBots)), Who.bOnlySpectator))
		{
			return;
		}
		Out = __NFUN_112__("", string(Controller(Who.Owner).PlayerNum));		
	}
	else
	{
		Out = "-1";
	}
	Logf(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("", Header()), "P"), __NFUN_236__(9)), Out), __NFUN_236__(9)), Desc));
	return;
}

function GameEvent(string GEvent, string Desc, PlayerReplicationInfo Who)
{
	local string Out, tab, geDesc;

	// End:0x6B
	if(__NFUN_119__(Who, none))
	{
		// End:0x40
		if(__NFUN_132__(__NFUN_130__(Who.bBot, __NFUN_129__(bShowBots)), Who.bOnlySpectator))
		{
			return;
		}
		Out = __NFUN_112__("", string(Controller(Who.Owner).PlayerNum));		
	}
	else
	{
		Out = "-1";
	}
	geDesc = Desc;
	tab = __NFUN_112__("", __NFUN_236__(9));
	ReplaceText(geDesc, tab, "_");
	Logf(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("", Header()), "G"), __NFUN_236__(9)), GEvent), __NFUN_236__(9)), Out), __NFUN_236__(9)), geDesc));
	return;
}
