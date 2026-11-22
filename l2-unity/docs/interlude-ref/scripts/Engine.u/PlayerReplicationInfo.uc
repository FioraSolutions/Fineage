class PlayerReplicationInfo extends ReplicationInfo
	native
	nativereplication
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var float Score;
var float Deaths;
var CarriedObject HasFlag;
var int Ping;
var Volume PlayerVolume;
var ZoneInfo PlayerZone;
var int NumLives;
var string PlayerName;
var string CharacterName;
var string OldCharacterName;
var string OldName;
var string PreviousName;
var int PlayerID;
var TeamInfo Team;
var int TeamID;
var Class<VoicePack> VoiceType;
var bool bAdmin;
var bool bIsFemale;
var bool bIsSpectator;
var bool bOnlySpectator;
var bool bWaitingPlayer;
var bool bReadyToPlay;
var bool bOutOfLives;
var bool bBot;
var bool bWelcomed;
var bool bReceivedPing;
var bool bHasFlag;
var int StartTime;
var localized string StringSpectating;
var localized string StringUnknown;
var int GoalsScored;
var int Kills;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(bNetDirty, __NFUN_154__(int(Role), int(ROLE_Authority))))
		CharacterName, Deaths, 
		PlayerName, PlayerVolume, 
		PlayerZone, Score, 
		Team, TeamID, 
		VoiceType, bAdmin, 
		bHasFlag, bIsFemale, 
		bIsSpectator, bOnlySpectator, 
		bOutOfLives, bReadyToPlay, 
		bWaitingPlayer;

	// Pos:0x018
	reliable if(__NFUN_130__(__NFUN_130__(bNetDirty, __NFUN_129__(bNetOwner)), __NFUN_154__(int(Role), int(ROLE_Authority))))
		Ping;

	// Pos:0x03D
	reliable if(__NFUN_130__(bNetInitial, __NFUN_154__(int(Role), int(ROLE_Authority))))
		StartTime, bBot;
}

function PostBeginPlay()
{
	// End:0x12
	if(__NFUN_150__(int(Role), int(ROLE_Authority)))
	{
		return;
	}
	// End:0x2A
	if(__NFUN_119__(AIController(Owner), none))
	{
		bBot = true;
	}
	StartTime = Level.Game.GameReplicationInfo.ElapsedTime;
	Timer();
	__NFUN_280__(__NFUN_174__(1.5000000, __NFUN_195__()), true);
	return;
}

simulated function PostNetBeginPlay()
{
	local GameReplicationInfo GRI;

	// End:0x24
	foreach __NFUN_313__(Class'Engine.GameReplicationInfo', GRI)
	{
		GRI.AddPRI(self);
		// End:0x24
		break;		
	}	
	return;
}

simulated function Destroyed()
{
	local GameReplicationInfo GRI;

	// End:0x21
	foreach __NFUN_313__(Class'Engine.GameReplicationInfo', GRI)
	{
		GRI.RemovePRI(self);		
	}	
	super(Actor).Destroyed();
	return;
}

function SetCharacterName(string S)
{
	CharacterName = S;
	return;
}

function Reset()
{
	super(Actor).Reset();
	Score = 0.0000000;
	Deaths = 0.0000000;
	SetFlag(none);
	bReadyToPlay = false;
	NumLives = 0;
	bOutOfLives = false;
	return;
}

function SetFlag(CarriedObject NewFlag)
{
	HasFlag = NewFlag;
	bHasFlag = __NFUN_119__(HasFlag, none);
	return;
}

simulated function string GetHumanReadableName()
{
	return PlayerName;
	return;
}

simulated function string GetLocationName()
{
	// End:0x1E
	if(__NFUN_130__(__NFUN_114__(PlayerVolume, none), __NFUN_114__(PlayerZone, none)))
	{
		return StringSpectating;
	}
	// End:0x5E
	if(__NFUN_130__(__NFUN_119__(PlayerVolume, none), __NFUN_123__(PlayerVolume.LocationName, Class'Engine.Volume'.default.LocationName)))
	{
		return PlayerVolume.LocationName;		
	}
	else
	{
		// End:0x92
		if(__NFUN_130__(__NFUN_119__(PlayerZone, none), __NFUN_123__(PlayerZone.LocationName, "")))
		{
			return PlayerZone.LocationName;			
		}
		else
		{
			// End:0xC5
			if(__NFUN_123__(Level.Title, Level.default.Title))
			{
				return Level.Title;				
			}
			else
			{
				return StringUnknown;
			}
		}
	}
	return;
}

simulated function Material GetPortrait()
{
	return;
}

event UpdateCharacter()
{
	return;
}

function UpdatePlayerLocation()
{
	local Volume V, Best;
	local Pawn P;
	local Controller C;

	C = Controller(Owner);
	// End:0x2F
	if(__NFUN_119__(C, none))
	{
		P = C.Pawn;
	}
	// End:0x4A
	if(__NFUN_114__(P, none))
	{
		PlayerVolume = none;
		PlayerZone = none;
		return;
	}
	// End:0x80
	if(__NFUN_119__(PlayerZone, P.Region.Zone))
	{
		PlayerZone = P.Region.Zone;
	}
	// End:0x107
	foreach P.__NFUN_307__(Class'Engine.Volume', V)
	{
		// End:0xB2
		if(__NFUN_122__(V.LocationName, ""))
		{
			continue;			
		}
		// End:0xE4
		if(__NFUN_130__(__NFUN_119__(Best, none), __NFUN_152__(V.LocationPriority, Best.LocationPriority)))
		{
			continue;			
		}
		// End:0x106
		if(V.Encompasses(P))
		{
			Best = V;
		}		
	}	
	// End:0x122
	if(__NFUN_119__(PlayerVolume, Best))
	{
		PlayerVolume = Best;
	}
	return;
}

simulated function DisplayDebug(Canvas Canvas, out float YL, out float YPos)
{
	// End:0x65
	if(__NFUN_119__(Team, none))
	{
		Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("     PlayerName ", PlayerName), " Team "), Team.GetHumanReadableName()), " has flag "), string(HasFlag)));		
	}
	else
	{
		Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__("     PlayerName ", PlayerName), " NO Team"));
	}
	return;
}

event ClientNameChange()
{
	local PlayerController PC;

	// End:0x28
	foreach __NFUN_313__(Class'Engine.PlayerController', PC)
	{
		PC.ReceiveLocalizedMessage(Class'Engine.GameMessage', 2, self);		
	}	
	return;
}

function Timer()
{
	local Controller C;

	UpdatePlayerLocation();
	__NFUN_280__(__NFUN_174__(1.5000000, __NFUN_195__()), true);
	// End:0x21
	if(__NFUN_176__(__NFUN_195__(), 0.6500000))
	{
		return;
	}
	// End:0x67
	if(__NFUN_129__(bBot))
	{
		C = Controller(Owner);
		// End:0x67
		if(__NFUN_129__(bReceivedPing))
		{
			Ping = int(C.ConsoleCommand("GETPING"));
		}
	}
	return;
}

function SetPlayerName(string S)
{
	OldName = PlayerName;
	PlayerName = S;
	return;
}

function SetWaitingPlayer(bool B)
{
	bIsSpectator = B;
	bWaitingPlayer = B;
	return;
}

defaultproperties
{
	StringSpectating="Spectating"
	StringUnknown="Unknown"
	NetUpdateFrequency=5.0000000
}