class StatLog extends Info
	native
	config
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var int Context;
var bool bWorld;
var float TimeStamp;
var() string LocalStandard;
var() string WorldStandard;
var() string LogVersion;
var() string LogInfoURL;
var() string GameName;
var() string GameCreator;
var() string GameCreatorURL;
var() string DecoderRingURL;
var() globalconfig string LocalBatcherURL;
var() globalconfig string LocalBatcherParams;
var() globalconfig string LocalStatsURL;
var() globalconfig string WorldBatcherURL;
var() globalconfig string WorldBatcherParams;
var() globalconfig string WorldStatsURL;
var() globalconfig string LocalLogDir;
var() globalconfig string WorldLogDir;
var globalconfig bool bWorldBatcherError;
var globalconfig bool bBatchLocal;
var StatLog LocalLog;

function BeginPlay()
{
	__NFUN_280__(30.0000000, true);
	return;
}

function Destroyed()
{
	// End:0x17
	if(__NFUN_119__(LocalLog, none))
	{
		LocalLog.__NFUN_279__();
	}
	return;
}

function Timer()
{
	LogPings();
	return;
}

function GenerateLogs(bool bLogLocal, bool bLogWorld)
{
	// End:0x28
	if(bLogWorld)
	{
		bWorld = true;
		// End:0x28
		if(bLogLocal)
		{
			LocalLog = __NFUN_278__(Class);
		}
	}
	return;
}

function StartLog()
{
	return;
}

function StopLog()
{
	return;
}

function FlushLog()
{
	return;
}

function LogEventString(string EventString)
{
	__NFUN_231__(EventString);
	// End:0x22
	if(__NFUN_119__(LocalLog, none))
	{
		LocalLog.__NFUN_231__(EventString);
	}
	return;
}

function LogWorldEventString(string EventString)
{
	__NFUN_231__(EventString);
	return;
}

// Export UStatLog::execExecuteLocalLogBatcher(FFrame&, void* const)
native final function ExecuteLocalLogBatcher();

// Export UStatLog::execExecuteSilentLogBatcher(FFrame&, void* const)
native final function ExecuteSilentLogBatcher();

// Export UStatLog::execBatchLocal(FFrame&, void* const)
native static final function BatchLocal();

// Export UStatLog::execExecuteWorldLogBatcher(FFrame&, void* const)
native final function ExecuteWorldLogBatcher();

// Export UStatLog::execBrowseRelativeLocalURL(FFrame&, void* const)
native static function BrowseRelativeLocalURL(string URL);

// Export UStatLog::execInitialCheck(FFrame&, void* const)
native final function InitialCheck(GameInfo Game);

// Export UStatLog::execLogMutator(FFrame&, void* const)
native final function LogMutator(Mutator M);

// Export UStatLog::execGetPlayerChecksum(FFrame&, void* const)
native static function GetPlayerChecksum(PlayerController P, out string Checksum);

// Export UStatLog::execGetGMTRef(FFrame&, void* const)
native final function string GetGMTRef();

function string GetAbsoluteTime()
{
	local string AbsoluteTime, GMTRef;

	AbsoluteTime = string(Level.Year);
	// End:0x51
	if(__NFUN_150__(Level.Month, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Month));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Month));
	}
	// End:0xAE
	if(__NFUN_150__(Level.Day, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Day));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Day));
	}
	// End:0x10B
	if(__NFUN_150__(Level.Hour, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Hour));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Hour));
	}
	// End:0x168
	if(__NFUN_150__(Level.Minute, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Minute));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Minute));
	}
	// End:0x1C5
	if(__NFUN_150__(Level.Second, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Second));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Second));
	}
	// End:0x222
	if(__NFUN_150__(Level.Millisecond, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Millisecond));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Millisecond));
	}
	GMTRef = GetGMTRef();
	AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), GMTRef);
	TimeStamp = 0.0000000;
	return AbsoluteTime;
	return;
}

function string GetShortAbsoluteTime()
{
	local string AbsoluteTime;

	AbsoluteTime = string(Level.Year);
	// End:0x51
	if(__NFUN_150__(Level.Month, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Month));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Month));
	}
	// End:0xAE
	if(__NFUN_150__(Level.Day, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Day));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Day));
	}
	// End:0x10B
	if(__NFUN_150__(Level.Hour, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Hour));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Hour));
	}
	// End:0x168
	if(__NFUN_150__(Level.Minute, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Minute));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Minute));
	}
	// End:0x1C5
	if(__NFUN_150__(Level.Second, 10))
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, ".0"), string(Level.Second));		
	}
	else
	{
		AbsoluteTime = __NFUN_112__(__NFUN_112__(AbsoluteTime, "."), string(Level.Second));
	}
	TimeStamp = 0.0000000;
	return AbsoluteTime;
	return;
}

function string GetTimeStamp()
{
	local string Time;

	Time = string(TimeStamp);
	Time = __NFUN_128__(Time, __NFUN_146__(__NFUN_126__(Time, "."), 3));
	return Time;
	return;
}

event string GetLocalLogFileName()
{
	return "";
	return;
}

function Tick(float Delta)
{
	__NFUN_184__(TimeStamp, Delta);
	return;
}

function LogStandardInfo()
{
	// End:0x9A
	if(bWorld)
	{
		LogWorldEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Log_Standard"), __NFUN_236__(9)), WorldStandard));
		// End:0x97
		if(__NFUN_119__(LocalLog, none))
		{
			LocalLog.LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Log_Standard"), __NFUN_236__(9)), LocalStandard));
		}		
	}
	else
	{
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Log_Standard"), __NFUN_236__(9)), LocalStandard));
	}
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Log_Version"), __NFUN_236__(9)), LogVersion));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Log_Info_URL"), __NFUN_236__(9)), LogInfoURL));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Game_Name"), __NFUN_236__(9)), GameName));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Game_Version"), __NFUN_236__(9)), Level.EngineVersion));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Game_Creator"), __NFUN_236__(9)), GameCreator));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Game_Creator_URL"), __NFUN_236__(9)), GameCreatorURL));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Game_Decoder_Ring_URL"), __NFUN_236__(9)), DecoderRingURL));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Absolute_Time"), __NFUN_236__(9)), GetAbsoluteTime()));
	// End:0x38C
	if(bWorld)
	{
		// End:0x350
		if(__NFUN_124__(Level.ConsoleCommand("get UdpServerUplink douplink"), string(true)))
		{
			LogWorldEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_Public"), __NFUN_236__(9)), "1"));			
		}
		else
		{
			LogWorldEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_Public"), __NFUN_236__(9)), "0"));
		}
	}
	return;
}

function LogServerInfo()
{
	local string NetworkNumber;

	NetworkNumber = Level.Game.GetNetworkNumber();
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_ServerName"), __NFUN_236__(9)), Level.Game.GameReplicationInfo.ServerName));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_AdminName"), __NFUN_236__(9)), Level.Game.GameReplicationInfo.AdminName));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_AdminEmail"), __NFUN_236__(9)), Level.Game.GameReplicationInfo.AdminEmail));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_Region"), __NFUN_236__(9)), string(Level.Game.GameReplicationInfo.ServerRegion)));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_MOTDLine1"), __NFUN_236__(9)), Level.Game.GameReplicationInfo.MOTDLine1));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_MOTDLine2"), __NFUN_236__(9)), Level.Game.GameReplicationInfo.MOTDLine2));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_MOTDLine3"), __NFUN_236__(9)), Level.Game.GameReplicationInfo.MOTDLine3));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_MOTDLine4"), __NFUN_236__(9)), Level.Game.GameReplicationInfo.MOTDLine4));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_IP"), __NFUN_236__(9)), NetworkNumber));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "info"), __NFUN_236__(9)), "Server_Port"), __NFUN_236__(9)), string(Level.Game.GetServerPort())));
	return;
}

final event LogGameSpecial(string SpecialID, string SpecialParam)
{
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "game"), __NFUN_236__(9)), SpecialID), __NFUN_236__(9)), SpecialParam));
	return;
}

final event LogGameSpecial2(string SpecialID, string SpecialParam, string SpecialParam2)
{
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "game"), __NFUN_236__(9)), SpecialID), __NFUN_236__(9)), SpecialParam), __NFUN_236__(9)), SpecialParam2));
	return;
}

// Export UStatLog::execGetMapFileName(FFrame&, void* const)
native final function string GetMapFileName();

function LogMapParameters()
{
	local string MapName;

	MapName = GetMapFileName();
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "map"), __NFUN_236__(9)), "Name"), __NFUN_236__(9)), MapName));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "map"), __NFUN_236__(9)), "Title"), __NFUN_236__(9)), Level.Title));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "map"), __NFUN_236__(9)), "Author"), __NFUN_236__(9)), Level.Author));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "map"), __NFUN_236__(9)), "IdealPlayerCount"), __NFUN_236__(9)), string(Level.Summary.IdealPlayerCount)));
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "map"), __NFUN_236__(9)), "LevelEnterText"), __NFUN_236__(9)), Level.LevelEnterText));
	return;
}

function LogPlayerConnect(Controller Player, optional string Checksum)
{
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "Connect"), __NFUN_236__(9)), Player.PlayerReplicationInfo.PlayerName), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.PlayerID)), __NFUN_236__(9)), string(Player.__NFUN_303__('Admin'))));
	LogPlayerInfo(Player);
	return;
}

function LogPlayerInfo(Controller Player)
{
	// End:0x177
	if(__NFUN_119__(Player.PlayerReplicationInfo.Team, none))
	{
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "TeamName"), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.PlayerID)), __NFUN_236__(9)), Player.PlayerReplicationInfo.Team.TeamName));
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "Team"), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.PlayerID)), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.Team.TeamIndex)));
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "TeamID"), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.PlayerID)), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.TeamID)));
	}
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "Ping"), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.PlayerID)), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.Ping)));
	return;
}

function LogPlayerDisconnect(Controller Player)
{
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "Disconnect"), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.PlayerID)));
	return;
}

function LogKill(PlayerReplicationInfo KillerPRI, PlayerReplicationInfo VictimPRI, string KillerWeaponName, string VictimWeaponName, Class<DamageType> DamageType)
{
	local string KillType;

	// End:0x0D
	if(__NFUN_114__(VictimPRI, none))
	{
		return;
	}
	// End:0x7D
	if(__NFUN_114__(KillerPRI, VictimPRI))
	{
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "suicide"), __NFUN_236__(9)), string(KillerPRI.PlayerID)), __NFUN_236__(9)), KillerWeaponName), __NFUN_236__(9)), string(DamageType)), __NFUN_236__(9)), "None"));
		return;
	}
	// End:0xB1
	if(__NFUN_114__(KillerPRI.Team, VictimPRI.Team))
	{
		KillType = "teamkill";		
	}
	else
	{
		KillType = "kill";
	}
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), KillType), __NFUN_236__(9)), string(KillerPRI.PlayerID)), __NFUN_236__(9)), KillerWeaponName), __NFUN_236__(9)), string(VictimPRI.PlayerID)), __NFUN_236__(9)), VictimWeaponName), __NFUN_236__(9)), string(DamageType)));
	return;
}

function LogNameChange(Controller Other)
{
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "Rename"), __NFUN_236__(9)), Other.PlayerReplicationInfo.PlayerName), __NFUN_236__(9)), string(Other.PlayerReplicationInfo.PlayerID)));
	return;
}

function LogTeamChange(Controller Other)
{
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "Teamchange"), __NFUN_236__(9)), string(Other.PlayerReplicationInfo.PlayerID)), __NFUN_236__(9)), string(Other.PlayerReplicationInfo.Team)));
	return;
}

function LogTypingEvent(bool bTyping, Controller Other)
{
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "typing"), __NFUN_236__(9)), string(bTyping)), __NFUN_236__(9)), string(Other.PlayerReplicationInfo.PlayerID)));
	return;
}

function LogPickup(Pickup item, Pawn Other)
{
	// End:0x95
	if(__NFUN_130__(__NFUN_119__(item.InventoryType, none), __NFUN_123__(item.InventoryType.default.ItemName, "")))
	{
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "item_get"), __NFUN_236__(9)), item.InventoryType.default.ItemName), __NFUN_236__(9)), string(Other.PlayerReplicationInfo.PlayerID)));		
	}
	else
	{
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "item_get"), __NFUN_236__(9)), string(item)), __NFUN_236__(9)), string(Other.PlayerReplicationInfo.PlayerID)));
	}
	return;
}

function LogItemActivate(Inventory item, Pawn Other)
{
	// End:0x30
	if(__NFUN_132__(__NFUN_132__(__NFUN_114__(Other, none), __NFUN_114__(Other.PlayerReplicationInfo, none)), __NFUN_114__(item, none)))
	{
		return;
	}
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "item_activate"), __NFUN_236__(9)), item.ItemName), __NFUN_236__(9)), string(Other.PlayerReplicationInfo.PlayerID)));
	return;
}

function LogItemDeactivate(Inventory item, Pawn Other)
{
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "item_deactivate"), __NFUN_236__(9)), item.ItemName), __NFUN_236__(9)), string(Other.PlayerReplicationInfo.PlayerID)));
	return;
}

function LogSpecialEvent(string EventType, coerce optional string Arg1, coerce optional string Arg2, coerce optional string Arg3, coerce optional string Arg4)
{
	local string Event;

	Event = EventType;
	// End:0x2F
	if(__NFUN_123__(Arg1, ""))
	{
		Event = __NFUN_112__(__NFUN_112__(Event, __NFUN_236__(9)), Arg1);
	}
	// End:0x53
	if(__NFUN_123__(Arg2, ""))
	{
		Event = __NFUN_112__(__NFUN_112__(Event, __NFUN_236__(9)), Arg2);
	}
	// End:0x77
	if(__NFUN_123__(Arg3, ""))
	{
		Event = __NFUN_112__(__NFUN_112__(Event, __NFUN_236__(9)), Arg3);
	}
	// End:0x9B
	if(__NFUN_123__(Arg4, ""))
	{
		Event = __NFUN_112__(__NFUN_112__(Event, __NFUN_236__(9)), Arg4);
	}
	LogEventString(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), Event));
	return;
}

function LogPings()
{
	local PlayerReplicationInfo PRI;

	// End:0x6B
	foreach __NFUN_313__(Class'Engine.PlayerReplicationInfo', PRI)
	{
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "Ping"), __NFUN_236__(9)), string(PRI.PlayerID)), __NFUN_236__(9)), string(PRI.Ping)));		
	}	
	return;
}

function LogGameStart()
{
	LogEventString(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "game_start"));
	return;
}

function LogGameEnd(string Reason)
{
	LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "game_end"), __NFUN_236__(9)), Reason));
	return;
}

defaultproperties
{
	LocalStandard="ngLog"
	WorldStandard="ngLog"
	LogVersion="1.2"
	LogInfoURL="http://www.netgamesusa.com/ngLog/"
	GameName="Unreal"
	GameCreator="Epic MegaGames, Inc."
	GameCreatorURL="http://www.epicgames.com/"
	DecoderRingURL="http://unreal.epicgames.com/Unreal_Log_Decoder_Ring.html"
	LocalBatcherURL="../NetGamesUSA.com/ngStats/ngStatsUT.exe"
	LocalStatsURL="../NetGamesUSA.com/ngStats/html/ngStats_Main.html"
	WorldBatcherURL="../NetGamesUSA.com/ngWorldStats/bin/ngWorldStats.exe"
	WorldBatcherParams="-d ../NetGamesUSA.com/ngWorldStats/logs -g UT"
	WorldStatsURL="http://www.netgamesusa.com"
	LocalLogDir="../Logs"
	WorldLogDir="../NetGamesUSA.com/ngWorldStats/logs"
}