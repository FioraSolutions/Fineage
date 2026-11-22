class GameInfo extends Info
	native
	config
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

struct VoiceChatterInfo
{
	var Controller Controller;
	var int IpAddr;
	var int Handle;
};

var bool bRestartLevel;
var bool bPauseable;
var config bool bWeaponStay;
var bool bCanChangeSkin;
var bool bTeamGame;
var bool bGameEnded;
var bool bOverTime;
var localized bool bAlternateMode;
var bool bCanViewOthers;
var bool bDelayedStart;
var bool bWaitingToStartMatch;
var globalconfig bool bChangeLevels;
var bool bAlreadyChanged;
var bool bLoggingGame;
var globalconfig bool bEnableStatLogging;
var config bool bAllowWeaponThrowing;
var globalconfig float GameDifficulty;
var globalconfig int GoreLevel;
var globalconfig float AutoAim;
var globalconfig float GameSpeed;
var float StartTime;
var string DefaultPlayerClassName;
var string ScoreBoardType;
var string BotMenuType;
var string RulesMenuType;
var string SettingsMenuType;
var string GameUMenuType;
var string MultiplayerUMenuType;
var string GameOptionsMenuType;
var string HUDType;
var string MapListType;
var string MapPrefix;
var string BeaconName;
var globalconfig int MaxSpectators;
var int NumSpectators;
var globalconfig int MaxPlayers;
var int NumPlayers;
var int NumBots;
var int CurrentID;
var localized string DefaultPlayerName;
var localized string GameName;
var float FearCostFallOff;
var config int GoalScore;
var config int MaxLives;
var config int TimeLimit;
var Class<LocalMessage> DeathMessageClass;
var Class<GameMessage> GameMessageClass;
var name OtherMesgGroup;
var string MutatorClass;
var Mutator BaseMutator;
var globalconfig string AccessControlClass;
var AccessControl AccessControl;
var GameRules GameRulesModifiers;
var string BroadcastHandlerClass;
var BroadcastHandler BroadcastHandler;
var Class<PlayerController> PlayerControllerClass;
var string PlayerControllerClassName;
var() Class<GameReplicationInfo> GameReplicationInfoClass;
var GameReplicationInfo GameReplicationInfo;
var bool bWelcomePending;
var GameStats GameStats;
var Class<GameStats> GameStatsClass;
var array<VoiceChatterInfo> VoiceChatters;
var Class<Security> SecurityClass;
var() string ScreenShotName;
var() string DecoTextName;
var() string Acronym;
var private localized string GIPropsDisplayText[11];
var private localized string GIPropsExtras[2];

function PreBeginPlay()
{
	StartTime = 0.0000000;
	SetGameSpeed(GameSpeed);
	GameReplicationInfo = __NFUN_278__(GameReplicationInfoClass);
	InitGameReplicationInfo();
	InitLogging();
	return;
}

function UpdatePrecacheMaterials()
{
	PrecacheGameTextures(Level);
	return;
}

function UpdatePrecacheStaticMeshes()
{
	PrecacheGameStaticMeshes(Level);
	return;
}

static function PrecacheGameTextures(LevelInfo myLevel)
{
	return;
}

static function PrecacheGameStaticMeshes(LevelInfo myLevel)
{
	return;
}

function string FindPlayerByID(int PlayerID)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x6F [Loop If]
	if(__NFUN_150__(i, GameReplicationInfo.PRIArray.Length))
	{
		// End:0x65
		if(__NFUN_154__(GameReplicationInfo.PRIArray[i].PlayerID, PlayerID))
		{
			return GameReplicationInfo.PRIArray[i].PlayerName;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return "";
	return;
}

static function bool UseLowGore()
{
	return __NFUN_132__(default.bAlternateMode, __NFUN_151__(default.GoreLevel, 0));
	return;
}

function PostBeginPlay()
{
	// End:0x29
	if(__NFUN_119__(GameStats, none))
	{
		GameStats.NewGame();
		GameStats.ServerInfo();
	}
	return;
}

function Reset()
{
	super(Actor).Reset();
	bGameEnded = false;
	bOverTime = false;
	bWaitingToStartMatch = true;
	InitGameReplicationInfo();
	return;
}

function InitLogging()
{
	// End:0x50
	if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_129__(bEnableStatLogging), __NFUN_129__(bLoggingGame)), __NFUN_154__(int(Level.NetMode), int(NM_Standalone))), __NFUN_154__(int(Level.NetMode), int(NM_ListenServer))))
	{
		return;
	}
	GameStats = __NFUN_278__(GameStatsClass);
	return;
}

function Timer()
{
	local NavigationPoint N;
	local int i;

	// End:0xF6
	if(bWelcomePending)
	{
		bWelcomePending = false;
		// End:0xF6
		if(__NFUN_155__(int(Level.NetMode), int(NM_Standalone)))
		{
			i = 0;
			J0x31:

			// End:0xF6 [Loop If]
			if(__NFUN_150__(i, GameReplicationInfo.PRIArray.Length))
			{
				// End:0xEC
				if(__NFUN_130__(__NFUN_119__(GameReplicationInfo.PRIArray[i], none), __NFUN_129__(GameReplicationInfo.PRIArray[i].bWelcomed)))
				{
					GameReplicationInfo.PRIArray[i].bWelcomed = true;
					// End:0xEC
					if(__NFUN_129__(GameReplicationInfo.PRIArray[i].bOnlySpectator))
					{
						BroadcastLocalizedMessage(GameMessageClass, 1, GameReplicationInfo.PRIArray[i]);
					}
				}
				__NFUN_165__(i);
				// [Loop Continue]
				goto J0x31;
			}
		}
	}
	BroadcastHandler.UpdateSentText();
	N = Level.NavigationPointList;
	J0x119:

	// End:0x150 [Loop If]
	if(__NFUN_119__(N, none))
	{
		__NFUN_159__(N.FearCost, FearCostFallOff);
		N = N.nextNavigationPoint;
		// [Loop Continue]
		goto J0x119;
	}
	return;
}

event GameEnding()
{
	EndLogging("serverquit");
	return;
}

function InitGameReplicationInfo()
{
	GameReplicationInfo.bTeamGame = bTeamGame;
	GameReplicationInfo.GameName = GameName;
	GameReplicationInfo.GameClass = string(Class);
	GameReplicationInfo.MaxLives = MaxLives;
	return;
}

// Export UGameInfo::execGetNetworkNumber(FFrame&, void* const)
native function string GetNetworkNumber();

function GetServerInfo(out ServerResponseLine ServerState)
{
	ServerState.ServerName = GameReplicationInfo.ServerName;
	ServerState.MapName = __NFUN_128__(string(Level), __NFUN_126__(string(Level), "."));
	ServerState.GameType = __NFUN_127__(string(Class), __NFUN_146__(__NFUN_126__(string(Class), "."), 1));
	ServerState.CurrentPlayers = GetNumPlayers();
	ServerState.MaxPlayers = MaxPlayers;
	ServerState.IP = "";
	ServerState.Port = GetServerPort();
	ServerState.ServerInfo.Length = 0;
	ServerState.PlayerInfo.Length = 0;
	return;
}

function int GetNumPlayers()
{
	return NumPlayers;
	return;
}

function GetServerDetails(out ServerResponseLine ServerState)
{
	local int i;
	local Mutator M;

	i = ServerState.ServerInfo.Length;
	ServerState.ServerInfo.Length = __NFUN_146__(i, 1);
	ServerState.ServerInfo[i].Key = "servermode";
	// End:0x8A
	if(__NFUN_154__(int(Level.NetMode), int(NM_ListenServer)))
	{
		ServerState.ServerInfo[__NFUN_165__(i)].Value = "non-dedicated";		
	}
	else
	{
		ServerState.ServerInfo[__NFUN_165__(i)].Value = "dedicated";
	}
	ServerState.ServerInfo.Length = __NFUN_146__(i, 1);
	ServerState.ServerInfo[i].Key = "adminname";
	ServerState.ServerInfo[__NFUN_165__(i)].Value = GameReplicationInfo.AdminName;
	ServerState.ServerInfo.Length = __NFUN_146__(i, 1);
	ServerState.ServerInfo[i].Key = "adminemail";
	ServerState.ServerInfo[__NFUN_165__(i)].Value = GameReplicationInfo.AdminEmail;
	// End:0x1C8
	if(AccessControl.RequiresPassword())
	{
		ServerState.ServerInfo.Length = __NFUN_146__(i, 1);
		ServerState.ServerInfo[i].Key = "password";
		ServerState.ServerInfo[__NFUN_165__(i)].Value = "true";
	}
	ServerState.ServerInfo.Length = __NFUN_146__(i, 1);
	ServerState.ServerInfo[i].Key = "gamestats";
	// End:0x229
	if(__NFUN_119__(GameStats, none))
	{
		ServerState.ServerInfo[__NFUN_165__(i)].Value = "true";		
	}
	else
	{
		ServerState.ServerInfo[__NFUN_165__(i)].Value = "false";
	}
	// End:0x2BC
	if(__NFUN_181__(GameSpeed, 1.0000000))
	{
		ServerState.ServerInfo.Length = __NFUN_146__(i, 1);
		ServerState.ServerInfo[i].Key = "gamespeed";
		ServerState.ServerInfo[__NFUN_165__(i)].Value = string(__NFUN_172__(float(int(__NFUN_171__(GameSpeed, float(100)))), 100.0000000));
	}
	M = BaseMutator.NextMutator;
	J0x2D0:

	// End:0x306 [Loop If]
	if(__NFUN_119__(M, none))
	{
		M.GetServerDetails(ServerState);
		M = M.NextMutator;
		// [Loop Continue]
		goto J0x2D0;
	}
	return;
}

function GetServerPlayers(out ServerResponseLine ServerState)
{
	local Mutator M;
	local Controller C;
	local PlayerReplicationInfo PRI;
	local int i;

	i = ServerState.PlayerInfo.Length;
	C = Level.ControllerList;
	J0x25:

	// End:0x13B [Loop If]
	if(__NFUN_119__(C, none))
	{
		PRI = C.PlayerReplicationInfo;
		// End:0x124
		if(__NFUN_130__(__NFUN_130__(__NFUN_119__(PRI, none), __NFUN_129__(PRI.bBot)), __NFUN_114__(MessagingSpectator(C), none)))
		{
			ServerState.PlayerInfo.Length = __NFUN_146__(i, 1);
			ServerState.PlayerInfo[i].PlayerNum = C.PlayerNum;
			ServerState.PlayerInfo[i].PlayerName = PRI.PlayerName;
			ServerState.PlayerInfo[i].Score = int(PRI.Score);
			ServerState.PlayerInfo[i].Ping = PRI.Ping;
			__NFUN_165__(i);
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x25;
	}
	M = BaseMutator.NextMutator;
	J0x14F:

	// End:0x185 [Loop If]
	if(__NFUN_119__(M, none))
	{
		M.GetServerPlayers(ServerState);
		M = M.NextMutator;
		// [Loop Continue]
		goto J0x14F;
	}
	return;
}

function int GetServerPort()
{
	local string S;
	local int i;

	S = Level.GetAddressURL();
	i = __NFUN_126__(S, ":");
	assert(__NFUN_153__(i, 0));
	return int(__NFUN_127__(S, __NFUN_146__(i, 1)));
	return;
}

function bool SetPause(bool bPause, PlayerController P)
{
	// End:0x95
	if(__NFUN_132__(__NFUN_132__(__NFUN_132__(bPauseable, P.__NFUN_303__('Admin')), P.PlayerReplicationInfo.bAdmin), __NFUN_154__(int(Level.NetMode), int(NM_Standalone))))
	{
		// End:0x80
		if(bPause)
		{
			Level.Pauser = P.PlayerReplicationInfo;			
		}
		else
		{
			Level.Pauser = none;
		}
		return true;		
	}
	else
	{
		return false;
	}
	return;
}

function ChangeVoiceChatter(Controller Client, int IpAddr, int Handle, bool Add)
{
	local int Index;
	local PlayerController P;

	// End:0x66
	if(Add)
	{
		Index = VoiceChatters.Length;
		VoiceChatters.Insert(Index, 1);
		VoiceChatters[Index].Controller = Client;
		VoiceChatters[Index].IpAddr = IpAddr;
		VoiceChatters[Index].Handle = Handle;		
	}
	else
	{
		Index = 0;
		J0x6D:

		// End:0xC9 [Loop If]
		if(__NFUN_150__(Index, VoiceChatters.Length))
		{
			// End:0xBF
			if(__NFUN_130__(__NFUN_154__(VoiceChatters[Index].IpAddr, IpAddr), __NFUN_154__(VoiceChatters[Index].Handle, Handle)))
			{
				VoiceChatters.Remove(Index, 1);
			}
			__NFUN_165__(Index);
			// [Loop Continue]
			goto J0x6D;
		}
	}
	// End:0x108
	foreach __NFUN_313__(Class'Engine.PlayerController', P)
	{
		// End:0x107
		if(__NFUN_119__(P, Client))
		{
			P.ClientChangeVoiceChatter(IpAddr, Handle, Add);
		}		
	}	
	return;
}

function SetGameSpeed(float t)
{
	local float OldSpeed;

	OldSpeed = GameSpeed;
	GameSpeed = __NFUN_245__(t, 0.1000000);
	Level.TimeDilation = GameSpeed;
	// End:0x43
	if(__NFUN_181__(GameSpeed, OldSpeed))
	{
		__NFUN_536__();
	}
	__NFUN_280__(Level.TimeDilation, true);
	return;
}

event DetailChange()
{
	local Actor A;
	local ZoneInfo Z;

	// End:0x76
	if(__NFUN_154__(int(Level.DetailMode), int(0)))
	{
		// End:0x72
		foreach __NFUN_313__(Class'Engine.Actor', A)
		{
			// End:0x71
			if(__NFUN_130__(__NFUN_132__(A.bHighDetail, A.bSuperHighDetail), __NFUN_129__(A.bGameRelevant)))
			{
				A.__NFUN_279__();
			}			
		}				
	}
	else
	{
		// End:0xD5
		if(__NFUN_154__(int(Level.DetailMode), int(1)))
		{
			// End:0xD4
			foreach __NFUN_313__(Class'Engine.Actor', A)
			{
				// End:0xD3
				if(__NFUN_130__(A.bSuperHighDetail, __NFUN_129__(A.bGameRelevant)))
				{
					A.__NFUN_279__();
				}				
			}			
		}
	}
	// End:0xF5
	foreach __NFUN_304__(Class'Engine.ZoneInfo', Z)
	{
		Z.LinkToSkybox();		
	}	
	return;
}

function bool GrabOption(out string Options, out string Result)
{
	// End:0x8A
	if(__NFUN_122__(__NFUN_128__(Options, 1), "?"))
	{
		Result = __NFUN_127__(Options, 1);
		// End:0x45
		if(__NFUN_153__(__NFUN_126__(Result, "?"), 0))
		{
			Result = __NFUN_128__(Result, __NFUN_126__(Result, "?"));
		}
		Options = __NFUN_127__(Options, 1);
		// End:0x7D
		if(__NFUN_153__(__NFUN_126__(Options, "?"), 0))
		{
			Options = __NFUN_127__(Options, __NFUN_126__(Options, "?"));			
		}
		else
		{
			Options = "";
		}
		return true;		
	}
	else
	{
		return false;
	}
	return;
}

function GetKeyValue(string Pair, out string Key, out string Value)
{
	// End:0x44
	if(__NFUN_153__(__NFUN_126__(Pair, "="), 0))
	{
		Key = __NFUN_128__(Pair, __NFUN_126__(Pair, "="));
		Value = __NFUN_127__(Pair, __NFUN_146__(__NFUN_126__(Pair, "="), 1));		
	}
	else
	{
		Key = Pair;
		Value = "";
	}
	return;
}

function string ParseOption(string Options, string InKey)
{
	local string Pair, Key, Value;

	J0x00:
	// End:0x40 [Loop If]
	if(GrabOption(Options, Pair))
	{
		GetKeyValue(Pair, Key, Value);
		// End:0x3D
		if(__NFUN_124__(Key, InKey))
		{
			return Value;
		}
		// [Loop Continue]
		goto J0x00;
	}
	return "";
	return;
}

event InitGame(string Options, out string Error)
{
	local string InOpt, LeftOpt;
	local int pos;
	local Class<AccessControl> ACClass;
	local Class<GameRules> GRClass;
	local Class<BroadcastHandler> BHClass;

	__NFUN_231__(__NFUN_168__("InitGame:", Options));
	MaxPlayers = __NFUN_251__(GetIntOption(Options, "MaxPlayers", MaxPlayers), 0, 32);
	GameDifficulty = __NFUN_245__(0.0000000, float(GetIntOption(Options, "Difficulty", int(GameDifficulty))));
	InOpt = ParseOption(Options, "GameSpeed");
	// End:0xB1
	if(__NFUN_123__(InOpt, ""))
	{
		__NFUN_231__(__NFUN_168__("GameSpeed", InOpt));
		SetGameSpeed(float(InOpt));
	}
	AddMutator(MutatorClass);
	BHClass = Class<BroadcastHandler>(DynamicLoadObject(BroadcastHandlerClass, Class'Core.Class'));
	BroadcastHandler = __NFUN_278__(BHClass);
	InOpt = ParseOption(Options, "AccessControl");
	// End:0x12C
	if(__NFUN_123__(InOpt, ""))
	{
		ACClass = Class<AccessControl>(DynamicLoadObject(InOpt, Class'Core.Class'));
	}
	// End:0x168
	if(__NFUN_114__(ACClass, none))
	{
		ACClass = Class<AccessControl>(DynamicLoadObject(AccessControlClass, Class'Core.Class'));
		// End:0x168
		if(__NFUN_114__(ACClass, none))
		{
			ACClass = Class'Engine.AccessControl';
		}
	}
	LeftOpt = ParseOption(Options, "AdminName");
	InOpt = ParseOption(Options, "AdminPassword");
	// End:0x1CF
	if(__NFUN_130__(__NFUN_123__(LeftOpt, ""), __NFUN_123__(InOpt, "")))
	{
		ACClass.default.bDontAddDefaultAdmin = true;
	}
	AccessControl = __NFUN_278__(ACClass);
	InOpt = ParseOption(Options, "GameRules");
	// End:0x2FD
	if(__NFUN_123__(InOpt, ""))
	{
		__NFUN_231__(__NFUN_168__("Game Rules", InOpt));
		J0x21A:

		// End:0x2FD [Loop If]
		if(__NFUN_123__(InOpt, ""))
		{
			pos = __NFUN_126__(InOpt, ",");
			// End:0x274
			if(__NFUN_151__(pos, 0))
			{
				LeftOpt = __NFUN_128__(InOpt, pos);
				InOpt = __NFUN_234__(InOpt, __NFUN_147__(__NFUN_147__(__NFUN_125__(InOpt), pos), 1));				
			}
			else
			{
				LeftOpt = InOpt;
				InOpt = "";
			}
			__NFUN_231__(__NFUN_112__("Add game rules ", LeftOpt));
			GRClass = Class<GameRules>(DynamicLoadObject(LeftOpt, Class'Core.Class'));
			// End:0x2FA
			if(__NFUN_119__(GRClass, none))
			{
				// End:0x2E3
				if(__NFUN_114__(GameRulesModifiers, none))
				{
					GameRulesModifiers = __NFUN_278__(GRClass);					
				}
				else
				{
					GameRulesModifiers.AddGameRules(__NFUN_278__(GRClass));
				}
			}
			// [Loop Continue]
			goto J0x21A;
		}
	}
	__NFUN_231__(__NFUN_112__("Base Mutator is ", string(BaseMutator)));
	InOpt = ParseOption(Options, "Mutator");
	// End:0x3E6
	if(__NFUN_123__(InOpt, ""))
	{
		__NFUN_231__(__NFUN_168__("Mutators", InOpt));
		J0x353:

		// End:0x3E6 [Loop If]
		if(__NFUN_123__(InOpt, ""))
		{
			pos = __NFUN_126__(InOpt, ",");
			// End:0x3AD
			if(__NFUN_151__(pos, 0))
			{
				LeftOpt = __NFUN_128__(InOpt, pos);
				InOpt = __NFUN_234__(InOpt, __NFUN_147__(__NFUN_147__(__NFUN_125__(InOpt), pos), 1));				
			}
			else
			{
				LeftOpt = InOpt;
				InOpt = "";
			}
			__NFUN_231__(__NFUN_112__("Add mutator ", LeftOpt));
			AddMutator(LeftOpt, true);
			// [Loop Continue]
			goto J0x353;
		}
	}
	InOpt = ParseOption(Options, "GamePassword");
	// End:0x43C
	if(__NFUN_123__(InOpt, ""))
	{
		AccessControl.SetGamePassword(InOpt);
		__NFUN_231__(__NFUN_168__("GamePassword", InOpt));
	}
	InOpt = ParseOption(Options, "AllowThrowing");
	// End:0x476
	if(__NFUN_123__(InOpt, ""))
	{
		bAllowWeaponThrowing = bool(InOpt);
	}
	InOpt = ParseOption(Options, "GameStats");
	// End:0x4AF
	if(__NFUN_123__(InOpt, ""))
	{
		bEnableStatLogging = bool(InOpt);		
	}
	else
	{
		bEnableStatLogging = false;
	}
	__NFUN_231__(__NFUN_168__("GameInfo::InitGame : bEnableStatLogging", string(bEnableStatLogging)));
	return;
}

function AddMutator(string mutname, optional bool bUserAdded)
{
	local Class<Mutator> mutClass;
	local Mutator mut;

	mutClass = Class<Mutator>(DynamicLoadObject(mutname, Class'Core.Class'));
	// End:0x28
	if(__NFUN_114__(mutClass, none))
	{
		return;
	}
	// End:0x9A
	if(__NFUN_130__(__NFUN_123__(mutClass.default.GroupName, ""), __NFUN_119__(BaseMutator, none)))
	{
		mut = BaseMutator;
		J0x55:

		// End:0x9A [Loop If]
		if(__NFUN_119__(mut, none))
		{
			// End:0x83
			if(__NFUN_122__(mut.GroupName, mutClass.default.GroupName))
			{
				return;
			}
			mut = mut.NextMutator;
			// [Loop Continue]
			goto J0x55;
		}
	}
	mut = __NFUN_278__(mutClass);
	// End:0xB5
	if(__NFUN_114__(mut, none))
	{
		return;
	}
	mut.bUserAdded = bUserAdded;
	// End:0xE4
	if(__NFUN_114__(BaseMutator, none))
	{
		BaseMutator = mut;		
	}
	else
	{
		BaseMutator.AddMutator(mut);
	}
	return;
}

event string GetBeaconText()
{
	return __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(Level.ComputerName, " "), __NFUN_128__(Level.Title, 24)), "\\t"), BeaconName), "\\t"), string(GetNumPlayers())), "/"), string(MaxPlayers));
	return;
}

function ProcessServerTravel(string URL, bool bItems)
{
	local PlayerController P, LocalPlayer;

	EndLogging("mapchange");
	__NFUN_231__(__NFUN_168__("ProcessServerTravel:", URL));
	// End:0x93
	foreach __NFUN_313__(Class'Engine.PlayerController', P)
	{
		// End:0x78
		if(__NFUN_119__(NetConnection(P.Player), none))
		{
			P.ClientTravel(URL, 2, bItems);
			// End:0x92
			continue;
		}
		LocalPlayer = P;
		P.PreClientTravel();		
	}	
	// End:0x167
	if(__NFUN_130__(__NFUN_154__(int(Level.NetMode), int(NM_ListenServer)), __NFUN_119__(LocalPlayer, none)))
	{
		Level.NextURL = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(Level.NextURL, "?Team="), LocalPlayer.GetDefaultURL("Team")), "?Name="), LocalPlayer.GetDefaultURL("Name")), "?Class="), LocalPlayer.GetDefaultURL("Class")), "?Character="), LocalPlayer.GetDefaultURL("Character"));
	}
	// End:0x1AF
	if(__NFUN_130__(__NFUN_155__(int(Level.NetMode), int(NM_DedicatedServer)), __NFUN_155__(int(Level.NetMode), int(NM_ListenServer))))
	{
		Level.NextSwitchCountdown = 0.0000000;
	}
	return;
}

event PreLogin(string Options, string Address, out string Error, out string FailCode)
{
	local bool bSpectator;

	bSpectator = __NFUN_124__(ParseOption(Options, "SpectatorOnly"), "true");
	AccessControl.PreLogin(Options, Address, Error, FailCode, bSpectator);
	return;
}

function int GetIntOption(string Options, string ParseString, int CurrentValue)
{
	local string InOpt;

	InOpt = ParseOption(Options, ParseString);
	// End:0x38
	if(__NFUN_123__(InOpt, ""))
	{
		__NFUN_231__(__NFUN_168__(ParseString, InOpt));
		return int(InOpt);
	}
	return CurrentValue;
	return;
}

function bool AtCapacity(bool bSpectator)
{
	// End:0x1B
	if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
	{
		return false;
	}
	// End:0x5C
	if(bSpectator)
	{
		return __NFUN_130__(__NFUN_153__(NumSpectators, MaxSpectators), __NFUN_132__(__NFUN_155__(int(Level.NetMode), int(NM_ListenServer)), __NFUN_151__(NumPlayers, 0)));		
	}
	else
	{
		return __NFUN_130__(__NFUN_151__(MaxPlayers, 0), __NFUN_153__(NumPlayers, MaxPlayers));
	}
	return;
}

event PlayerController Login(string Portal, string Options, out string Error)
{
	local NavigationPoint StartSpot;
	local PlayerController NewPlayer;
	local Pawn TestPawn;
	local string InName, InAdminName, InPassword, InChecksum, InClass, InCharacter;

	local byte InTeam;
	local bool bSpectator, bAdmin;

	bSpectator = __NFUN_124__(ParseOption(Options, "SpectatorOnly"), "true");
	// End:0x5B
	if(__NFUN_130__(__NFUN_129__(bAdmin), AtCapacity(bSpectator)))
	{
		Error = GameMessageClass.default.MaxedOutMessage;
		return none;
	}
	// End:0x78
	if(__NFUN_130__(bAdmin, AtCapacity(false)))
	{
		bSpectator = true;
	}
	BaseMutator.ModifyLogin(Portal, Options);
	InName = __NFUN_128__(ParseOption(Options, "Name"), 20);
	InTeam = byte(GetIntOption(Options, "Team", 255));
	InAdminName = ParseOption(Options, "AdminName");
	InPassword = ParseOption(Options, "Password");
	InChecksum = ParseOption(Options, "Checksum");
	__NFUN_231__(__NFUN_168__("Login:", InName));
	// End:0x149
	if(__NFUN_123__(InPassword, ""))
	{
		__NFUN_231__(__NFUN_168__("Password", InPassword));
	}
	InTeam = PickTeam(InTeam, none);
	StartSpot = FindPlayerStart(none, InTeam, Portal);
	// End:0x193
	if(__NFUN_114__(StartSpot, none))
	{
		Error = GameMessageClass.default.FailedPlaceMessage;
		return none;
	}
	// End:0x1DF
	if(__NFUN_114__(PlayerControllerClass, none))
	{
		PlayerControllerClass = Class<PlayerController>(DynamicLoadObject("ini:Engine.Engine.DefaultPlayerController", Class'Core.Class'));
	}
	NewPlayer = __NFUN_278__(PlayerControllerClass,,, StartSpot.Location, StartSpot.Rotation);
	// End:0x263
	if(__NFUN_114__(NewPlayer, none))
	{
		__NFUN_231__(__NFUN_112__("Couldn't spawn player controller of class ", string(PlayerControllerClass)));
		Error = GameMessageClass.default.FailedSpawnMessage;
		return none;
	}
	NewPlayer.StartSpot = StartSpot;
	NewPlayer.GameReplicationInfo = GameReplicationInfo;
	NewPlayer.__NFUN_113__('Spectating');
	NewPlayer.PlayerSecurity = __NFUN_278__(SecurityClass, self);
	// End:0x2FD
	if(__NFUN_114__(NewPlayer.PlayerSecurity, none))
	{
		__NFUN_231__(__NFUN_112__("Could not spawn security for player ", string(NewPlayer)), 'Security');
	}
	// End:0x329
	if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
	{
		NewPlayer.__NFUN_113__('AttractMode');		
	}
	else
	{
		NewPlayer.__NFUN_113__('Spectating');
	}
	// End:0x350
	if(__NFUN_122__(InName, ""))
	{
		InName = DefaultPlayerName;
	}
	// End:0x39D
	if(__NFUN_132__(__NFUN_155__(int(Level.NetMode), int(NM_Standalone)), __NFUN_122__(NewPlayer.PlayerReplicationInfo.PlayerName, DefaultPlayerName)))
	{
		ChangeName(NewPlayer, InName, false);
	}
	// End:0x404
	if(__NFUN_132__(bSpectator, NewPlayer.PlayerReplicationInfo.bOnlySpectator))
	{
		NewPlayer.PlayerReplicationInfo.bOnlySpectator = true;
		NewPlayer.PlayerReplicationInfo.bIsSpectator = true;
		__NFUN_165__(NumSpectators);
		return NewPlayer;
	}
	// End:0x432
	if(__NFUN_129__(ChangeTeam(NewPlayer, int(InTeam), false)))
	{
		Error = GameMessageClass.default.FailedTeamMessage;
		return none;
	}
	NewPlayer.StartSpot = StartSpot;
	NewPlayer.PlayerReplicationInfo.PlayerID = __NFUN_165__(CurrentID);
	InClass = ParseOption(Options, "Class");
	// End:0x494
	if(__NFUN_122__(InClass, ""))
	{
		InClass = DefaultPlayerClassName;
	}
	InCharacter = ParseOption(Options, "Character");
	NewPlayer.SetPawnClass(InClass, InCharacter);
	__NFUN_165__(NumPlayers);
	bWelcomePending = true;
	// End:0x4F7
	if(bDelayedStart)
	{
		NewPlayer.__NFUN_113__('PlayerWaiting');
		return NewPlayer;
	}
	// End:0x5E6
	foreach __NFUN_313__(Class'Engine.Pawn', TestPawn)
	{
		// End:0x5E5
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(TestPawn, none), __NFUN_119__(PlayerController(TestPawn.Controller), none)), __NFUN_114__(PlayerController(TestPawn.Controller).Player, none)), __NFUN_151__(TestPawn.Health, 0)), __NFUN_124__(TestPawn.OwnerName, InName)))
		{
			NewPlayer.__NFUN_279__();
			TestPawn.__NFUN_299__(TestPawn.Controller.Rotation);
			TestPawn.bInitializeAnimation = false;
			TestPawn.PlayWaiting();			
			return PlayerController(TestPawn.Controller);
		}		
	}	
	return NewPlayer;
	return;
}

event PlayerController L2NetLogin(string Portal, string Options, out string Error)
{
	local PlayerController NewPlayer;
	local NavigationPoint StartSpot;
	local Pawn TestPawn;
	local string InName, InPassword, InChecksum, InClass;
	local byte InTeam;
	local bool bSpectator;

	bSpectator = __NFUN_123__(ParseOption(Options, "SpectatorOnly"), "");
	// End:0x4A
	if(AtCapacity(bSpectator))
	{
		Error = GameMessageClass.default.MaxedOutMessage;
		return none;
	}
	BaseMutator.ModifyLogin(Portal, Options);
	InName = __NFUN_128__(ParseOption(Options, "Name"), 20);
	InTeam = byte(GetIntOption(Options, "Team", 255));
	InPassword = ParseOption(Options, "Password");
	InChecksum = ParseOption(Options, "Checksum");
	InTeam = PickTeam(InTeam, none);
	StartSpot = FindPlayerStart(none, InTeam, Portal);
	// End:0x119
	if(__NFUN_114__(StartSpot, none))
	{
		Error = GameMessageClass.default.FailedPlaceMessage;
		return none;
	}
	// End:0x175
	if(AccessControl.AdminLogin(NewPlayer, InPassword))
	{
		NewPlayer = __NFUN_278__(AccessControl.AdminClass,,, StartSpot.Location, StartSpot.Rotation);
		bSpectator = true;		
	}
	else
	{
		// End:0x1C1
		if(__NFUN_114__(PlayerControllerClass, none))
		{
			PlayerControllerClass = Class<PlayerController>(DynamicLoadObject("ini:Engine.Engine.DefaultPlayerController", Class'Core.Class'));
		}
		NewPlayer = __NFUN_278__(PlayerControllerClass,,, StartSpot.Location, StartSpot.Rotation);
	}
	// End:0x245
	if(__NFUN_114__(NewPlayer, none))
	{
		__NFUN_231__(__NFUN_112__("Couldn't spawn player controller of class ", string(PlayerControllerClass)));
		Error = GameMessageClass.default.FailedSpawnMessage;
		return none;
	}
	NewPlayer.StartSpot = StartSpot;
	// End:0x270
	if(__NFUN_122__(InName, ""))
	{
		InName = DefaultPlayerName;
	}
	// End:0x2D3
	if(__NFUN_132__(__NFUN_155__(int(Level.NetMode), int(NM_Standalone)), __NFUN_130__(__NFUN_119__(none, NewPlayer.PlayerReplicationInfo), __NFUN_122__(NewPlayer.PlayerReplicationInfo.PlayerName, DefaultPlayerName))))
	{
		ChangeName(NewPlayer, InName, false);
	}
	NewPlayer.GameReplicationInfo = GameReplicationInfo;
	NewPlayer.__NFUN_113__('Spectating');
	// End:0x327
	if(bSpectator)
	{
		NewPlayer.PlayerReplicationInfo.bOnlySpectator = true;
		__NFUN_165__(NumSpectators);
		return NewPlayer;
	}
	// End:0x35A
	if(__NFUN_119__(none, NewPlayer.PlayerReplicationInfo))
	{
		NewPlayer.PlayerReplicationInfo.PlayerID = __NFUN_165__(CurrentID);
	}
	InClass = ParseOption(Options, "Class");
	// End:0x3C0
	if(__NFUN_132__(__NFUN_154__(int(Level.NetMode), int(NM_DedicatedServer)), __NFUN_154__(int(Level.NetMode), int(NM_ListenServer))))
	{
		BroadcastLocalizedMessage(GameMessageClass, 1, NewPlayer.PlayerReplicationInfo);
	}
	// End:0x3DF
	if(bDelayedStart)
	{
		NewPlayer.__NFUN_113__('PlayerWaiting');
		return NewPlayer;
	}
	// End:0x49A
	foreach __NFUN_313__(Class'Engine.Pawn', TestPawn)
	{
		// End:0x499
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(TestPawn, none), __NFUN_119__(PlayerController(TestPawn.Controller), none)), __NFUN_114__(PlayerController(TestPawn.Controller).Player, none)), __NFUN_151__(TestPawn.Health, 0)), __NFUN_124__(TestPawn.OwnerName, InName)))
		{
			TestPawn.Controller.__NFUN_279__();
			NewPlayer.Possess(TestPawn);			
			return NewPlayer;
		}		
	}	
	bRestartLevel = false;
	// End:0x4B5
	if(bWaitingToStartMatch)
	{
		L2NetStartMatch();		
	}
	else
	{
		RestartPlayer(NewPlayer);
	}
	bRestartLevel = default.bRestartLevel;
	return NewPlayer;
	return;
}

function StartMatch()
{
	local Controller P;
	local Actor A;

	// End:0x1A
	if(__NFUN_119__(GameStats, none))
	{
		GameStats.StartGame();
	}
	// End:0x3A
	foreach __NFUN_304__(Class'Engine.Actor', A)
	{
		A.MatchStarting();		
	}	
	P = Level.ControllerList;
	J0x4F:

	// End:0xCB [Loop If]
	if(__NFUN_119__(P, none))
	{
		// End:0xB4
		if(__NFUN_130__(P.__NFUN_303__('PlayerController'), __NFUN_114__(P.Pawn, none)))
		{
			// End:0x92
			if(bGameEnded)
			{
				return;				
			}
			else
			{
				// End:0xB4
				if(PlayerController(P).CanRestartPlayer())
				{
					RestartPlayer(P);
				}
			}
		}
		P = P.nextController;
		// [Loop Continue]
		goto J0x4F;
	}
	P = Level.ControllerList;
	J0xDF:

	// End:0x167 [Loop If]
	if(__NFUN_119__(P, none))
	{
		// End:0x150
		if(__NFUN_130__(P.bIsPlayer, __NFUN_129__(P.__NFUN_303__('PlayerController'))))
		{
			// End:0x13B
			if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
			{
				RestartPlayer(P);				
			}
			else
			{
				P.__NFUN_113__('Dead', 'MPStart');
			}
		}
		P = P.nextController;
		// [Loop Continue]
		goto J0xDF;
	}
	bWaitingToStartMatch = false;
	GameReplicationInfo.bMatchHasBegun = true;
	return;
}

function L2NetStartMatch()
{
	local Controller P;
	local Actor A;

	// End:0x1A
	if(__NFUN_119__(GameStats, none))
	{
		GameStats.StartGame();
	}
	// End:0x3A
	foreach __NFUN_304__(Class'Engine.Actor', A)
	{
		A.MatchStarting();		
	}	
	P = Level.ControllerList;
	J0x4F:

	// End:0xA6 [Loop If]
	if(__NFUN_119__(P, none))
	{
		// End:0x8F
		if(__NFUN_130__(P.__NFUN_303__('PlayerController'), __NFUN_114__(P.Pawn, none)))
		{
			// End:0x8F
			if(bGameEnded)
			{
				return;
			}
		}
		P = P.nextController;
		// [Loop Continue]
		goto J0x4F;
	}
	P = Level.ControllerList;
	J0xBA:

	// End:0x111 [Loop If]
	if(__NFUN_119__(P, none))
	{
		// End:0xFA
		if(__NFUN_130__(P.bIsPlayer, __NFUN_129__(P.__NFUN_303__('PlayerController'))))
		{
			RestartPlayer(P);
		}
		P = P.nextController;
		// [Loop Continue]
		goto J0xBA;
	}
	bWaitingToStartMatch = false;
	return;
}

event SpawnDefaultPlayerPawn(Controller aPlayer)
{
	local NavigationPoint StartSpot;
	local Class<Pawn> DefaultPlayerClass;

	StartSpot = FindPlayerStart(aPlayer, byte(255));
	// End:0x40
	if(__NFUN_114__(StartSpot, none))
	{
		__NFUN_231__(" Player start not found!!!");
		return;
	}
	// End:0x92
	if(__NFUN_119__(aPlayer.PawnClass, none))
	{
		aPlayer.Pawn = __NFUN_278__(aPlayer.PawnClass,,, StartSpot.Location, StartSpot.Rotation);
	}
	// End:0xEC
	if(__NFUN_114__(aPlayer.Pawn, none))
	{
		DefaultPlayerClass = GetDefaultPlayerClass(aPlayer);
		aPlayer.Pawn = __NFUN_278__(DefaultPlayerClass,,, StartSpot.Location, StartSpot.Rotation);
	}
	// End:0x157
	if(__NFUN_114__(aPlayer.Pawn, none))
	{
		__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Couldn't spawn player of type ", string(aPlayer.PawnClass)), " at "), string(StartSpot)));
		aPlayer.__NFUN_113__('Dead');
		return;
	}
	aPlayer.Possess(aPlayer.Pawn);
	aPlayer.PawnClass = aPlayer.Pawn.Class;
	aPlayer.Pawn.PlayTeleportEffect(true, true);
	aPlayer.ClientSetRotation(aPlayer.Pawn.Rotation);
	AddDefaultInventory(aPlayer.Pawn);
	return;
}

event SpawnPlayerPawn(Controller aPlayer, string PawnClassName, Vector StartLocation, Rotator StartRotation)
{
	local int TeamNum;
	local Class<Pawn> DefaultPlayerClass;

	// End:0x3E
	if(__NFUN_132__(__NFUN_114__(aPlayer.PlayerReplicationInfo, none), __NFUN_114__(aPlayer.PlayerReplicationInfo.Team, none)))
	{
		TeamNum = 255;		
	}
	else
	{
		TeamNum = aPlayer.PlayerReplicationInfo.Team.TeamIndex;
	}
	// End:0xB7
	if(__NFUN_114__(aPlayer.Pawn, none))
	{
		DefaultPlayerClass = Class<Pawn>(DynamicLoadObject(PawnClassName, Class'Core.Class'));
		aPlayer.Pawn = __NFUN_278__(DefaultPlayerClass,,, StartLocation, StartRotation, true);
	}
	// End:0xFE
	if(__NFUN_114__(aPlayer.Pawn, none))
	{
		__NFUN_231__("Couldn't spawn player of type");
		aPlayer.__NFUN_113__('Dead');
		return;
	}
	aPlayer.Possess(aPlayer.Pawn);
	aPlayer.PawnClass = aPlayer.Pawn.Class;
	aPlayer.PlayTeleportEffect(true, true);
	aPlayer.ClientSetRotation(aPlayer.Pawn.Rotation);
	AddDefaultInventory(aPlayer.Pawn);
	return;
}

function RestartPlayer(Controller aPlayer)
{
	local NavigationPoint StartSpot;
	local int TeamNum;
	local Class<Pawn> DefaultPlayerClass;

	// End:0x41
	if(__NFUN_130__(__NFUN_130__(bRestartLevel, __NFUN_155__(int(Level.NetMode), int(NM_DedicatedServer))), __NFUN_155__(int(Level.NetMode), int(NM_ListenServer))))
	{
		return;
	}
	// End:0x7F
	if(__NFUN_132__(__NFUN_114__(aPlayer.PlayerReplicationInfo, none), __NFUN_114__(aPlayer.PlayerReplicationInfo.Team, none)))
	{
		TeamNum = 255;		
	}
	else
	{
		TeamNum = aPlayer.PlayerReplicationInfo.Team.TeamIndex;
	}
	StartSpot = FindPlayerStart(aPlayer, byte(TeamNum));
	// End:0xE8
	if(__NFUN_114__(StartSpot, none))
	{
		__NFUN_231__(" Player start not found!!!");
		return;
	}
	// End:0x133
	if(__NFUN_130__(__NFUN_119__(aPlayer.PreviousPawnClass, none), __NFUN_119__(aPlayer.PawnClass, aPlayer.PreviousPawnClass)))
	{
		BaseMutator.PlayerChangedClass(aPlayer);
	}
	// End:0x185
	if(__NFUN_119__(aPlayer.PawnClass, none))
	{
		aPlayer.Pawn = __NFUN_278__(aPlayer.PawnClass,,, StartSpot.Location, StartSpot.Rotation);
	}
	// End:0x1DF
	if(__NFUN_114__(aPlayer.Pawn, none))
	{
		DefaultPlayerClass = GetDefaultPlayerClass(aPlayer);
		aPlayer.Pawn = __NFUN_278__(DefaultPlayerClass,,, StartSpot.Location, StartSpot.Rotation);
	}
	// End:0x24A
	if(__NFUN_114__(aPlayer.Pawn, none))
	{
		__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Couldn't spawn player of type ", string(aPlayer.PawnClass)), " at "), string(StartSpot)));
		aPlayer.__NFUN_113__('Dead');
		return;
	}
	aPlayer.Pawn.Anchor = StartSpot;
	aPlayer.Pawn.LastStartSpot = PlayerStart(StartSpot);
	aPlayer.Pawn.LastStartTime = Level.TimeSeconds;
	aPlayer.PreviousPawnClass = aPlayer.Pawn.Class;
	aPlayer.Possess(aPlayer.Pawn);
	aPlayer.PawnClass = aPlayer.Pawn.Class;
	aPlayer.Pawn.PlayTeleportEffect(true, true);
	aPlayer.ClientSetRotation(aPlayer.Pawn.Rotation);
	AddDefaultInventory(aPlayer.Pawn);
	TriggerEvent(StartSpot.Event, StartSpot, aPlayer.Pawn);
	return;
}

function Class<Pawn> GetDefaultPlayerClass(Controller C)
{
	local PlayerController PC;
	local string PawnClassName;
	local Class<Pawn> PawnClass;

	PC = PlayerController(C);
	// End:0x63
	if(__NFUN_119__(PC, none))
	{
		PawnClassName = PC.GetDefaultURL("Class");
		PawnClass = Class<Pawn>(DynamicLoadObject(PawnClassName, Class'Core.Class'));
		// End:0x63
		if(__NFUN_119__(PawnClass, none))
		{
			return PawnClass;
		}
	}
	return Class<Pawn>(DynamicLoadObject(DefaultPlayerClassName, Class'Core.Class'));
	return;
}

event PostLogin(PlayerController NewPlayer)
{
	local Class<HUD> HudClass;
	local Class<ScoreBoard> ScoreboardClass;
	local string SongName;

	// End:0x68
	if(__NFUN_119__(GameStats, none))
	{
		GameStats.ConnectEvent(NewPlayer.PlayerReplicationInfo);
		GameStats.GameEvent("NameChange", NewPlayer.PlayerReplicationInfo.PlayerName, NewPlayer.PlayerReplicationInfo);
	}
	// End:0xA5
	if(__NFUN_129__(bDelayedStart))
	{
		bRestartLevel = false;
		// End:0x8D
		if(bWaitingToStartMatch)
		{
			StartMatch();			
		}
		else
		{
			RestartPlayer(NewPlayer);
		}
		bRestartLevel = default.bRestartLevel;
	}
	SongName = Level.Song;
	// End:0xED
	if(__NFUN_130__(__NFUN_123__(SongName, ""), __NFUN_123__(SongName, "None")))
	{
		NewPlayer.ClientSetMusic(SongName, 3);
	}
	// End:0x114
	if(__NFUN_123__(HUDType, ""))
	{
		HudClass = Class<HUD>(DynamicLoadObject(HUDType, Class'Core.Class'));
	}
	// End:0x13B
	if(__NFUN_123__(ScoreBoardType, ""))
	{
		ScoreboardClass = Class<ScoreBoard>(DynamicLoadObject(ScoreBoardType, Class'Core.Class'));
	}
	NewPlayer.ClientSetHUD(HudClass, ScoreboardClass);
	// End:0x197
	if(__NFUN_119__(NewPlayer.Pawn, none))
	{
		NewPlayer.Pawn.ClientSetRotation(NewPlayer.Pawn.Rotation);
	}
	return;
}

event L2NetPostLogin(PlayerController NewPlayer)
{
	// End:0x2F
	if(__NFUN_129__(bDelayedStart))
	{
		bRestartLevel = false;
		// End:0x22
		if(bWaitingToStartMatch)
		{
			L2NetStartMatch();
		}
		bRestartLevel = default.bRestartLevel;
	}
	NewPlayer.ClientSetMusic(Level.Song, 3);
	// End:0x91
	if(__NFUN_119__(NewPlayer.Pawn, none))
	{
		NewPlayer.Pawn.ClientSetRotation(NewPlayer.Pawn.Rotation);
	}
	return;
}

function Logout(Controller Exiting)
{
	local bool bMessage;
	local int Index;

	bMessage = true;
	// End:0x103
	if(__NFUN_119__(PlayerController(Exiting), none))
	{
		// End:0x4A
		if(PlayerController(Exiting).PlayerReplicationInfo.bOnlySpectator)
		{
			bMessage = false;
			__NFUN_166__(NumSpectators);			
		}
		else
		{
			__NFUN_166__(NumPlayers);
		}
		// End:0xD6
		if(__NFUN_155__(int(Level.NetMode), int(NM_Standalone)))
		{
			Index = 0;
			J0x71:

			// End:0xD6 [Loop If]
			if(__NFUN_150__(Index, VoiceChatters.Length))
			{
				// End:0xCC
				if(__NFUN_114__(VoiceChatters[Index].Controller, Exiting))
				{
					ChangeVoiceChatter(PlayerController(Exiting), VoiceChatters[Index].IpAddr, VoiceChatters[Index].Handle, false);
				}
				__NFUN_165__(Index);
				// [Loop Continue]
				goto J0x71;
			}
		}
		// End:0x103
		if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
		{
			PlayerController(Exiting).ClientLeaveVoiceChat();
		}
	}
	// End:0x15D
	if(__NFUN_130__(bMessage, __NFUN_132__(__NFUN_154__(int(Level.NetMode), int(NM_DedicatedServer)), __NFUN_154__(int(Level.NetMode), int(NM_ListenServer)))))
	{
		BroadcastLocalizedMessage(GameMessageClass, 4, Exiting.PlayerReplicationInfo);
	}
	// End:0x185
	if(__NFUN_119__(GameStats, none))
	{
		GameStats.DisconnectEvent(Exiting.PlayerReplicationInfo);
	}
	return;
}

event AcceptInventory(Pawn PlayerPawn)
{
	return;
}

function AddGameSpecificInventory(Pawn P)
{
	local Weapon newWeapon;
	local Class<Weapon> WeapClass;
	local Inventory Inv;

	WeapClass = BaseMutator.GetDefaultWeapon();
	// End:0xF1
	if(__NFUN_130__(__NFUN_119__(WeapClass, none), __NFUN_114__(P.FindInventoryType(WeapClass), none)))
	{
		newWeapon = __NFUN_278__(WeapClass,,, P.Location);
		// End:0xF1
		if(__NFUN_119__(newWeapon, none))
		{
			Inv = none;
			Inv = Inventory;
			J0x77:

			// End:0xCC [Loop If]
			if(__NFUN_119__(Inv, none))
			{
				// End:0xB5
				if(__NFUN_130__(Inv.__NFUN_303__('Weapon'), __NFUN_242__(Weapon(Inv).bCanThrow, false)))
				{
					// [Explicit Break]
					goto J0xCC;
				}
				Inv = Inv.Inventory;
				// [Loop Continue]
				goto J0x77;
			}
			J0xCC:

			newWeapon.GiveTo(P);
			newWeapon.bCanThrow = false;
		}
	}
	return;
}

function AddDefaultInventory(Pawn PlayerPawn)
{
	local Weapon newWeapon;
	local Class<Weapon> WeapClass;

	WeapClass = BaseMutator.GetDefaultWeapon();
	// End:0x8A
	if(__NFUN_130__(__NFUN_119__(WeapClass, none), __NFUN_114__(PlayerPawn.FindInventoryType(WeapClass), none)))
	{
		newWeapon = __NFUN_278__(WeapClass,,, PlayerPawn.Location);
		// End:0x8A
		if(__NFUN_119__(newWeapon, none))
		{
			newWeapon.GiveTo(PlayerPawn);
			newWeapon.bCanThrow = false;
		}
	}
	SetPlayerDefaults(PlayerPawn);
	return;
}

function SetPlayerDefaults(Pawn PlayerPawn)
{
	PlayerPawn.AirControl = PlayerPawn.default.AirControl;
	PlayerPawn.GroundSpeed = PlayerPawn.default.GroundSpeed;
	PlayerPawn.WaterSpeed = PlayerPawn.default.WaterSpeed;
	PlayerPawn.AirSpeed = PlayerPawn.default.AirSpeed;
	PlayerPawn.Acceleration = PlayerPawn.default.Acceleration;
	PlayerPawn.JumpZ = PlayerPawn.default.JumpZ;
	BaseMutator.ModifyPlayer(PlayerPawn);
	return;
}

function NotifyKilled(Controller Killer, Controller Killed, Pawn KilledPawn)
{
	return;
}

function Killed(Controller Killer, Controller Killed, Pawn KilledPawn, Class<DamageType> DamageType)
{
	// End:0x189
	if(__NFUN_130__(__NFUN_119__(Killed, none), Killed.bIsPlayer))
	{
		__NFUN_184__(Killed.PlayerReplicationInfo.Deaths, float(1));
		BroadcastDeathMessage(Killer, Killed, DamageType);
		// End:0x189
		if(__NFUN_119__(GameStats, none))
		{
			// End:0xE1
			if(__NFUN_132__(__NFUN_114__(Killer, Killed), __NFUN_114__(Killer, none)))
			{
				// End:0xAB
				if(__NFUN_114__(Killer, none))
				{
					GameStats.KillEvent("K", none, Killed.PlayerReplicationInfo, DamageType);					
				}
				else
				{
					GameStats.KillEvent("K", Killer.PlayerReplicationInfo, Killed.PlayerReplicationInfo, DamageType);
				}				
			}
			else
			{
				// End:0x156
				if(__NFUN_130__(bTeamGame, __NFUN_114__(Killer.PlayerReplicationInfo.Team, Killed.PlayerReplicationInfo.Team)))
				{
					GameStats.KillEvent("TK", Killer.PlayerReplicationInfo, Killed.PlayerReplicationInfo, DamageType);					
				}
				else
				{
					GameStats.KillEvent("K", Killer.PlayerReplicationInfo, Killed.PlayerReplicationInfo, DamageType);
				}
			}
		}
	}
	// End:0x1A4
	if(__NFUN_119__(Killed, none))
	{
		ScoreKill(Killer, Killed);
	}
	DiscardInventory(KilledPawn);
	NotifyKilled(Killer, Killed, KilledPawn);
	return;
}

function bool PreventDeath(Pawn Killed, Controller Killer, Class<DamageType> DamageType, Vector HitLocation)
{
	// End:0x0D
	if(__NFUN_114__(GameRulesModifiers, none))
	{
		return false;
	}
	return GameRulesModifiers.PreventDeath(Killed, Killer, DamageType, HitLocation);
	return;
}

function BroadcastDeathMessage(Controller Killer, Controller Other, Class<DamageType> DamageType)
{
	// End:0x40
	if(__NFUN_132__(__NFUN_114__(Killer, Other), __NFUN_114__(Killer, none)))
	{
		BroadcastLocalized(self, DeathMessageClass, 1, none, Other.PlayerReplicationInfo, DamageType);		
	}
	else
	{
		BroadcastLocalized(self, DeathMessageClass, 0, Killer.PlayerReplicationInfo, Other.PlayerReplicationInfo, DamageType);
	}
	return;
}

// Export UGameInfo::execParseKillMessage(FFrame&, void* const)
native static function string ParseKillMessage(string KillerName, string VictimName, string DeathMessage);

function Kick(string S)
{
	AccessControl.Kick(S);
	return;
}

function KickBan(string S)
{
	AccessControl.KickBan(S);
	return;
}

function bool IsOnTeam(Controller Other, int TeamNum)
{
	// End:0x77
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(bTeamGame, __NFUN_119__(Other, none)), Other.bIsPlayer), __NFUN_119__(Other.PlayerReplicationInfo.Team, none)), __NFUN_154__(Other.PlayerReplicationInfo.Team.TeamIndex, TeamNum)))
	{
		return true;
	}
	return false;
	return;
}

function bool CanSpectate(PlayerController Viewer, bool bOnlySpectator, Actor ViewTarget)
{
	return true;
	return;
}

function int ReduceDamage(int Damage, Pawn injured, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	return 0;
	return;
}

function bool ShouldRespawn(Pickup Other)
{
	// End:0x1B
	if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
	{
		return false;
	}
	return __NFUN_181__(Other.RespawnTime, 0.0000000);
	return;
}

function bool PickupQuery(Pawn Other, Pickup item)
{
	local byte bAllowPickup;

	// End:0x39
	if(__NFUN_130__(__NFUN_119__(GameRulesModifiers, none), GameRulesModifiers.OverridePickupQuery(Other, item, bAllowPickup)))
	{
		return __NFUN_154__(int(bAllowPickup), 1);
	}
	// End:0x52
	if(__NFUN_114__(Other.Inventory, none))
	{
		return true;		
	}
	else
	{
		return __NFUN_129__(Other.Inventory.HandlePickupQuery(item));
	}
	return;
}

function DiscardInventory(Pawn Other)
{
	Other.Weapon = none;
	Other.SelectedItem = none;
	J0x20:

	// End:0x4C [Loop If]
	if(__NFUN_119__(Other.Inventory, none))
	{
		Other.Inventory.__NFUN_279__();
		// [Loop Continue]
		goto J0x20;
	}
	return;
}

function ChangeName(Controller Other, coerce string S, bool bNameChange)
{
	local Controller C;

	// End:0x0E
	if(__NFUN_122__(S, ""))
	{
		return;
	}
	Other.PlayerReplicationInfo.SetPlayerName(S);
	// End:0xC3
	if(bNameChange)
	{
		C = Level.ControllerList;
		J0x48:

		// End:0xC3 [Loop If]
		if(__NFUN_119__(C, none))
		{
			// End:0xAC
			if(__NFUN_130__(__NFUN_119__(PlayerController(C), none), __NFUN_119__(Viewport(PlayerController(C).Player), none)))
			{
				PlayerController(C).ReceiveLocalizedMessage(Class'Engine.GameMessage', 2, Other.PlayerReplicationInfo);
			}
			C = C.nextController;
			// [Loop Continue]
			goto J0x48;
		}
	}
	return;
}

function bool ChangeTeam(Controller Other, int N, bool bNewTeam)
{
	return true;
	return;
}

function byte PickTeam(byte Current, Controller C)
{
	return Current;
	return;
}

function SendPlayer(PlayerController aPlayer, string URL)
{
	aPlayer.ClientTravel(URL, 2, true);
	return;
}

function RestartGame()
{
	local string NextMap;
	local MapList MyList;

	// End:0x21
	if(__NFUN_130__(__NFUN_119__(GameRulesModifiers, none), GameRulesModifiers.HandleRestartGame()))
	{
		return;
	}
	// End:0xCD
	if(__NFUN_130__(__NFUN_130__(bChangeLevels, __NFUN_129__(bAlreadyChanged)), __NFUN_123__(MapListType, "")))
	{
		bAlreadyChanged = true;
		MyList = GetMapList(MapListType);
		// End:0x8A
		if(__NFUN_119__(MyList, none))
		{
			NextMap = MyList.GetNextMap();
			MyList.__NFUN_279__();
		}
		// End:0xAA
		if(__NFUN_122__(NextMap, ""))
		{
			NextMap = __NFUN_539__(MapPrefix, NextMap, 1);
		}
		// End:0xCD
		if(__NFUN_123__(NextMap, ""))
		{
			Level.ServerTravel(NextMap, false);
			return;
		}
	}
	Level.ServerTravel("?Restart", false);
	return;
}

function MapList GetMapList(string MapListType)
{
	local Class<MapList> MapListClass;

	// End:0x3B
	if(__NFUN_123__(MapListType, ""))
	{
		MapListClass = Class<MapList>(DynamicLoadObject(MapListType, Class'Core.Class'));
		// End:0x3B
		if(__NFUN_119__(MapListClass, none))
		{
			return __NFUN_278__(MapListClass);
		}
	}
	return none;
	return;
}

event Broadcast(Actor Sender, coerce string Msg, optional name Type)
{
	BroadcastHandler.Broadcast(Sender, Msg, Type);
	return;
}

function BroadcastTeam(Controller Sender, coerce string Msg, optional name Type)
{
	BroadcastHandler.BroadcastTeam(Sender, Msg, Type);
	return;
}

event BroadcastLocalized(Actor Sender, Class<LocalMessage> Message, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	BroadcastHandler.AllowBroadcastLocalized(Sender, Message, Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject);
	return;
}

function bool CheckEndGame(PlayerReplicationInfo Winner, string Reason)
{
	local Controller P;

	// End:0x2D
	if(__NFUN_130__(__NFUN_119__(GameRulesModifiers, none), __NFUN_129__(GameRulesModifiers.CheckEndGame(Winner, Reason))))
	{
		return false;
	}
	P = Level.ControllerList;
	J0x41:

	// End:0x81 [Loop If]
	if(__NFUN_119__(P, none))
	{
		P.ClientGameEnded();
		P.GameHasEnded();
		P = P.nextController;
		// [Loop Continue]
		goto J0x41;
	}
	return true;
	return;
}

function EndGame(PlayerReplicationInfo Winner, string Reason)
{
	// End:0x1F
	if(__NFUN_129__(CheckEndGame(Winner, Reason)))
	{
		bOverTime = true;
		return;
	}
	bGameEnded = true;
	TriggerEvent('EndGame', self, none);
	EndLogging(Reason);
	return;
}

function EndLogging(string Reason)
{
	// End:0x0D
	if(__NFUN_114__(GameStats, none))
	{
		return;
	}
	GameStats.EndGame(Reason);
	GameStats.__NFUN_279__();
	GameStats = none;
	return;
}

function NavigationPoint FindPlayerStart(Controller Player, optional byte InTeam, optional string incomingName)
{
	local NavigationPoint N, BestStart;
	local Teleporter Tel;
	local float BestRating, NewRating;
	local byte Team;

	// End:0x89
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(Player, none), __NFUN_119__(Player.StartSpot, none)), __NFUN_154__(int(Level.NetMode), int(NM_Standalone))), __NFUN_132__(bWaitingToStartMatch, __NFUN_130__(__NFUN_119__(Player.PlayerReplicationInfo, none), Player.PlayerReplicationInfo.bWaitingPlayer))))
	{
		return Player.StartSpot;
	}
	// End:0xC9
	if(__NFUN_119__(GameRulesModifiers, none))
	{
		N = GameRulesModifiers.FindPlayerStart(Player, InTeam, incomingName);
		// End:0xC9
		if(__NFUN_119__(N, none))
		{
			return N;
		}
	}
	// End:0x108
	if(__NFUN_123__(incomingName, ""))
	{
		// End:0x107
		foreach __NFUN_304__(Class'Engine.Teleporter', Tel)
		{
			// End:0x106
			if(__NFUN_124__(string(Tel.Tag), incomingName))
			{				
				return Tel;
			}			
		}		
	}
	// End:0x17C
	if(__NFUN_130__(__NFUN_119__(Player, none), __NFUN_119__(Player.PlayerReplicationInfo, none)))
	{
		// End:0x171
		if(__NFUN_119__(Player.PlayerReplicationInfo.Team, none))
		{
			Team = byte(Player.PlayerReplicationInfo.Team.TeamIndex);			
		}
		else
		{
			Team = 0;
		}		
	}
	else
	{
		Team = InTeam;
	}
	N = Level.NavigationPointList;
	J0x19B:

	// End:0x1FD [Loop If]
	if(__NFUN_119__(N, none))
	{
		NewRating = RatePlayerStart(N, InTeam, Player);
		// End:0x1E6
		if(__NFUN_177__(NewRating, BestRating))
		{
			BestRating = NewRating;
			BestStart = N;
		}
		N = N.nextNavigationPoint;
		// [Loop Continue]
		goto J0x19B;
	}
	// End:0x288
	if(__NFUN_114__(BestStart, none))
	{
		__NFUN_231__("Warning - PATHS NOT DEFINED or NO PLAYERSTART");
		// End:0x287
		foreach __NFUN_304__(Class'Engine.NavigationPoint', N)
		{
			NewRating = RatePlayerStart(N, 0, Player);
			// End:0x286
			if(__NFUN_177__(NewRating, BestRating))
			{
				BestRating = NewRating;
				BestStart = N;
			}			
		}		
	}
	return BestStart;
	return;
}

function float RatePlayerStart(NavigationPoint N, byte Team, Controller Player)
{
	local PlayerStart P;

	P = PlayerStart(N);
	// End:0x51
	if(__NFUN_119__(P, none))
	{
		// End:0x4B
		if(P.bSinglePlayerStart)
		{
			// End:0x45
			if(P.bEnabled)
			{
				return 1000.0000000;
			}
			return 20.0000000;
		}
		return 10.0000000;
	}
	return 0.0000000;
	return;
}

function ScoreObjective(PlayerReplicationInfo Scorer, int Score)
{
	// End:0x22
	if(__NFUN_119__(Scorer, none))
	{
		__NFUN_184__(Scorer.Score, float(Score));
	}
	// End:0x46
	if(__NFUN_119__(GameRulesModifiers, none))
	{
		GameRulesModifiers.ScoreObjective(Scorer, Score);
	}
	CheckScore(Scorer);
	return;
}

function CheckScore(PlayerReplicationInfo Scorer)
{
	// End:0x26
	if(__NFUN_130__(__NFUN_119__(GameRulesModifiers, none), GameRulesModifiers.CheckScore(Scorer)))
	{
		return;
	}
	return;
}

function ScoreKill(Controller Killer, Controller Other)
{
	// End:0x73
	if(__NFUN_132__(__NFUN_114__(Killer, Other), __NFUN_114__(Killer, none)))
	{
		__NFUN_185__(Other.PlayerReplicationInfo.Score, float(1));
		// End:0x70
		if(__NFUN_119__(GameStats, none))
		{
			GameStats.ScoreEvent(Other.PlayerReplicationInfo, -1.0000000, "self_frag");
		}		
	}
	else
	{
		// End:0xEF
		if(__NFUN_119__(Killer.PlayerReplicationInfo, none))
		{
			__NFUN_184__(Killer.PlayerReplicationInfo.Score, float(1));
			__NFUN_165__(Killer.PlayerReplicationInfo.Kills);
			// End:0xEF
			if(__NFUN_119__(GameStats, none))
			{
				GameStats.ScoreEvent(Killer.PlayerReplicationInfo, 1.0000000, "frag");
			}
		}
	}
	// End:0x113
	if(__NFUN_119__(GameRulesModifiers, none))
	{
		GameRulesModifiers.ScoreKill(Killer, Other);
	}
	// End:0x13F
	if(__NFUN_132__(__NFUN_119__(Killer, none), __NFUN_151__(MaxLives, 0)))
	{
		CheckScore(Killer.PlayerReplicationInfo);
	}
	return;
}

function bool TooManyBots()
{
	return false;
	return;
}

static function string FindTeamDesignation(GameReplicationInfo GRI, Actor A)
{
	return "";
	return;
}

static function string ParseMessageString(Mutator BaseMutator, Controller Who, string Message)
{
	return Message;
	return;
}

function ReviewJumpSpots(name TestLabel)
{
	return;
}

function TeamInfo OtherTeam(TeamInfo Requester)
{
	return none;
	return;
}

exec function KillBots(int Num)
{
	return;
}

exec function AdminSay(string Msg)
{
	local Controller C;

	C = Level.ControllerList;
	J0x14:

	// End:0xAC [Loop If]
	if(__NFUN_119__(C, none))
	{
		// End:0x95
		if(C.__NFUN_303__('PlayerController'))
		{
			PlayerController(C).ClearProgressMessages();
			PlayerController(C).SetProgressTime(6.0000000);
			PlayerController(C).SetProgressMessage(0, Msg, Class'Engine.Canvas'.static.MakeColor(byte(255), byte(255), byte(255)));
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x14;
	}
	return;
}

defaultproperties
{
	bRestartLevel=true
	bPauseable=true
	bCanChangeSkin=true
	bCanViewOthers=true
	bDelayedStart=true
	bChangeLevels=true
	bAllowWeaponThrowing=true
	GameDifficulty=1.0000000
	AutoAim=0.9300000
	GameSpeed=1.0000000
	HUDType="Engine.HUD"
	MaxSpectators=2
	MaxPlayers=16
	DefaultPlayerName="Player"
	GameName="Game"
	FearCostFallOff=0.9500000
	DeathMessageClass=Class'Engine.LocalMessage'
	GameMessageClass=Class'Engine.GameMessage'
	MutatorClass="Engine.Mutator"
	AccessControlClass="Engine.AccessControl"
	BroadcastHandlerClass="Engine.BroadcastHandler"
	PlayerControllerClassName="Engine.PlayerController"
	GameReplicationInfoClass=Class'Engine.GameReplicationInfo'
	GameStatsClass=Class'Engine.GameStats'
	SecurityClass=Class'Engine.Security'
	Acronym="???"
}