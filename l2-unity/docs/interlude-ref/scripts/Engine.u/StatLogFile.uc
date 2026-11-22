class StatLogFile extends StatLog
	native
	config
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var bool bWatermark;
var int LogAr;
var string StatLogFile;
var string StatLogFinal;

// Export UStatLogFile::execOpenLog(FFrame&, void* const)
native final function OpenLog();

// Export UStatLogFile::execCloseLog(FFrame&, void* const)
native final function CloseLog();

// Export UStatLogFile::execWatermark(FFrame&, void* const)
native final function Watermark(string EventString);

// Export UStatLogFile::execGetChecksum(FFrame&, void* const)
native final function GetChecksum(out string Checksum);

// Export UStatLogFile::execFileFlush(FFrame&, void* const)
native final function FileFlush();

// Export UStatLogFile::execFileLog(FFrame&, void* const)
native final function FileLog(string EventString);

function StartLog()
{
	local string Filename, AbsoluteTime;

	__NFUN_536__();
	AbsoluteTime = GetShortAbsoluteTime();
	// End:0x93
	if(__NFUN_129__(bWorld))
	{
		Filename = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(LocalLogDir, "/"), GameName), "."), LocalStandard), "."), AbsoluteTime), "."), string(Level.Game.GetServerPort()));
		StatLogFile = __NFUN_112__(Filename, ".tmp");
		StatLogFinal = __NFUN_112__(Filename, ".log");		
	}
	else
	{
		Filename = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(WorldLogDir, "/"), GameName), "."), WorldStandard), "."), AbsoluteTime), "."), string(Level.Game.GetServerPort()));
		StatLogFile = __NFUN_112__(Filename, ".tmp");
		StatLogFinal = __NFUN_112__(Filename, ".log");
		bWatermark = true;
	}
	OpenLog();
	// End:0x131
	if(__NFUN_119__(LocalLog, none))
	{
		LocalLog.StartLog();
	}
	LogStandardInfo();
	LogServerInfo();
	LogMapParameters();
	return;
}

function StopLog()
{
	FileFlush();
	CloseLog();
	// End:0x1B
	if(bBatchLocal)
	{
		ExecuteSilentLogBatcher();
	}
	// End:0x35
	if(__NFUN_119__(LocalLog, none))
	{
		LocalLog.StopLog();
	}
	return;
}

function FlushLog()
{
	FileFlush();
	// End:0x20
	if(__NFUN_119__(LocalLog, none))
	{
		LocalLog.FlushLog();
	}
	return;
}

function LogEventString(string EventString)
{
	// End:0x14
	if(bWatermark)
	{
		Watermark(EventString);
	}
	FileLog(EventString);
	FileFlush();
	// End:0x44
	if(__NFUN_119__(LocalLog, none))
	{
		LocalLog.LogEventString(EventString);
	}
	return;
}

function LogWorldEventString(string EventString)
{
	// End:0x14
	if(bWatermark)
	{
		Watermark(EventString);
	}
	FileLog(EventString);
	FileFlush();
	return;
}

event string GetLocalLogFileName()
{
	// End:0x33
	if(bWorld)
	{
		// End:0x30
		if(__NFUN_119__(StatLogFile(LocalLog), none))
		{
			return StatLogFile(LocalLog).StatLogFinal;			
		}
		else
		{
			return "";
		}
	}
	return StatLogFinal;
	return;
}

function LogPlayerConnect(Controller Player, optional string Checksum)
{
	// End:0xAC
	if(bWorld)
	{
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "player"), __NFUN_236__(9)), "Connect"), __NFUN_236__(9)), Player.PlayerReplicationInfo.PlayerName), __NFUN_236__(9)), string(Player.PlayerReplicationInfo.PlayerID)), __NFUN_236__(9)), string(Player.__NFUN_303__('Admin'))), __NFUN_236__(9)), Checksum));
		LogPlayerInfo(Player);		
	}
	else
	{
		super.LogPlayerConnect(Player, Checksum);
	}
	return;
}

function LogGameEnd(string Reason)
{
	local string Checksum;

	// End:0x5B
	if(bWorld)
	{
		bWatermark = false;
		GetChecksum(Checksum);
		LogEventString(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetTimeStamp(), __NFUN_236__(9)), "game_end"), __NFUN_236__(9)), Reason), __NFUN_236__(9)), Checksum), ""));		
	}
	else
	{
		super.LogGameEnd(Reason);
	}
	return;
}

defaultproperties
{
	StatLogFile="../Logs/unreal.ngStats.Unknown.log"
}