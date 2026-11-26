class GameReplicationInfo extends ReplicationInfo
	native
	nativereplication
	config
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

enum ECarriedObjectState
{
	COS_Home,                       // 0
	COS_HeldFriendly,               // 1
	COS_HeldEnemy,                  // 2
	COS_Down                        // 3
};

var string GameName;
var string GameClass;
var bool bTeamGame;
var bool bStopCountDown;
var bool bMatchHasBegun;
var bool bTeamSymbolsUpdated;
var int RemainingTime;
var int ElapsedTime;
var int RemainingMinute;
var float SecondCount;
var int GoalScore;
var int TimeLimit;
var int MaxLives;
var TeamInfo Teams[2];
var() globalconfig string ServerName;
var() globalconfig string ShortName;
var() globalconfig string AdminName;
var() globalconfig string AdminEmail;
var() globalconfig int ServerRegion;
var() globalconfig string MOTDLine1;
var() globalconfig string MOTDLine2;
var() globalconfig string MOTDLine3;
var() globalconfig string MOTDLine4;
var Actor Winner;
var() array<PlayerReplicationInfo> PRIArray;
var Vector FlagPos;
var GameReplicationInfo.ECarriedObjectState CarriedObjectState[2];
var int MatchID;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(bNetDirty, __NFUN_154__(int(Role), int(ROLE_Authority))))
		CarriedObjectState, FlagPos, 
		MatchID, RemainingMinute, 
		Teams, Winner, 
		bMatchHasBegun, bStopCountDown;

	// Pos:0x018
	reliable if(__NFUN_130__(bNetInitial, __NFUN_154__(int(Role), int(ROLE_Authority))))
		AdminEmail, AdminName, 
		ElapsedTime, GameClass, 
		GameName, GoalScore, 
		MOTDLine1, MOTDLine2, 
		MOTDLine3, MOTDLine4, 
		MaxLives, RemainingTime, 
		ServerName, ServerRegion, 
		ShortName, TimeLimit, 
		bTeamGame;
}

simulated function SetCarriedObjectState(int Team, name NewState)
{
	switch(NewState)
	{
		// End:0x20
		case 'Down':
			CarriedObjectState[Team] = 3;
			// End:0x6E
			break;
		// End:0x39
		case 'HeldEnemy ':
			CarriedObjectState[Team] = 2;
			// End:0x6E
			break;
		// End:0x52
		case 'Home ':
			CarriedObjectState[Team] = 0;
			// End:0x6E
			break;
		// End:0x6B
		case 'HeldFriendly ':
			CarriedObjectState[Team] = 1;
			// End:0x6E
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

simulated function name GetCarriedObjectState(int Team)
{
	switch(CarriedObjectState[Team])
	{
		// End:0x18
		case 3:
			return 'Down';
		// End:0x23
		case 2:
			return 'HeldEnemy';
		// End:0x2E
		case 0:
			return 'home';
		// End:0x39
		case 1:
			return 'HeldFriendly';
		// End:0xFFFF
		default:
			return 'None';
			break;
	}
	return;
}

simulated function PostNetBeginPlay()
{
	local PlayerReplicationInfo PRI;

	// End:0x1C
	foreach __NFUN_313__(Class'Engine.PlayerReplicationInfo', PRI)
	{
		AddPRI(PRI);		
	}	
	// End:0x3C
	if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
	{
		TeamSymbolNotify();
	}
	return;
}

simulated function TeamSymbolNotify()
{
	local Actor A;

	// End:0x4E
	if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_114__(Teams[0], none), __NFUN_114__(Teams[1], none)), __NFUN_114__(Teams[0].TeamIcon, none)), __NFUN_114__(Teams[1].TeamIcon, none)))
	{
		return;
	}
	bTeamSymbolsUpdated = true;
	// End:0x77
	foreach __NFUN_304__(Class'Engine.Actor', A)
	{
		A.SetGRI(self);		
	}	
	return;
}

simulated function PostBeginPlay()
{
	// End:0x51
	if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
	{
		ServerName = "";
		AdminName = "";
		AdminEmail = "";
		MOTDLine1 = "";
		MOTDLine2 = "";
		MOTDLine3 = "";
		MOTDLine4 = "";
	}
	SecondCount = Level.TimeSeconds;
	__NFUN_280__(1.0000000, true);
	return;
}

function Reset()
{
	super(Actor).Reset();
	Winner = none;
	return;
}

simulated function Timer()
{
	// End:0xAA
	if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
	{
		// End:0x99
		if(__NFUN_179__(__NFUN_175__(Level.TimeSeconds, SecondCount), Level.TimeDilation))
		{
			__NFUN_165__(ElapsedTime);
			// End:0x65
			if(__NFUN_155__(RemainingMinute, 0))
			{
				RemainingTime = RemainingMinute;
				RemainingMinute = 0;
			}
			// End:0x84
			if(__NFUN_130__(__NFUN_151__(RemainingTime, 0), __NFUN_129__(bStopCountDown)))
			{
				__NFUN_166__(RemainingTime);
			}
			__NFUN_184__(SecondCount, Level.TimeDilation);
		}
		// End:0xAA
		if(__NFUN_129__(bTeamSymbolsUpdated))
		{
			TeamSymbolNotify();
		}
	}
	return;
}

simulated function AddPRI(PlayerReplicationInfo PRI)
{
	PRIArray[PRIArray.Length] = PRI;
	return;
}

simulated function RemovePRI(PlayerReplicationInfo PRI)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x39 [Loop If]
	if(__NFUN_150__(i, PRIArray.Length))
	{
		// End:0x2F
		if(__NFUN_114__(PRIArray[i], PRI))
		{
			// [Explicit Break]
			goto J0x39;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	J0x39:

	// End:0x91
	if(__NFUN_154__(i, PRIArray.Length))
	{
		__NFUN_231__(__NFUN_112__(__NFUN_112__("GameReplicationInfo::RemovePRI() pri=", string(PRI)), " not found."), 'Error');
		return;
	}
	PRIArray.Remove(i, 1);
	return;
}

simulated function GetPRIArray(out array<PlayerReplicationInfo> pris)
{
	local int i, Num;

	pris.Remove(0, pris.Length);
	i = 0;
	J0x14:

	// End:0x58 [Loop If]
	if(__NFUN_150__(i, PRIArray.Length))
	{
		// End:0x4E
		if(__NFUN_119__(PRIArray[i], none))
		{
			pris[__NFUN_165__(Num)] = PRIArray[i];
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x14;
	}
	return;
}

defaultproperties
{
	bStopCountDown=true
	ServerName="Another Server"
	ShortName="Server"
	bNetNotify=true
}