class TeamInfo extends ReplicationInfo
	native
	nativereplication
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var localized string TeamName;
var int Size;
var float Score;
var int TeamIndex;
var Color TeamColor;
var Color AltTeamColor;
var Actor HomeBase;
var() Class<Pawn> DefaultPlayerClass;
var localized string ColorNames[4];
var Material TeamIcon;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(bNetDirty, __NFUN_154__(int(Role), int(ROLE_Authority))))
		HomeBase, Score;

	// Pos:0x018
	reliable if(__NFUN_130__(bNetInitial, __NFUN_154__(int(Role), int(ROLE_Authority))))
		AltTeamColor, TeamColor, 
		TeamIcon, TeamIndex, 
		TeamName;
}

function bool BelongsOnTeam(Class<Pawn> PawnClass)
{
	return true;
	return;
}

simulated function UpdatePrecacheMaterials()
{
	Level.AddPrecacheMaterial(TeamIcon);
	return;
}

simulated function string GetHumanReadableName()
{
	// End:0x36
	if(__NFUN_122__(TeamName, default.TeamName))
	{
		// End:0x27
		if(__NFUN_150__(TeamIndex, 4))
		{
			return ColorNames[TeamIndex];
		}
		return __NFUN_168__(TeamName, string(TeamIndex));
	}
	return TeamName;
	return;
}

function bool AddToTeam(Controller Other)
{
	local Controller P;
	local bool bSuccess;

	// End:0x26
	if(__NFUN_114__(Other, none))
	{
		__NFUN_231__("Added none to team!!!");
		return false;
	}
	// End:0x38
	if(__NFUN_119__(MessagingSpectator(Other), none))
	{
		return false;
	}
	__NFUN_165__(Size);
	Other.PlayerReplicationInfo.Team = self;
	bSuccess = false;
	// End:0x90
	if(Other.__NFUN_303__('PlayerController'))
	{
		Other.PlayerReplicationInfo.TeamID = 0;		
	}
	else
	{
		Other.PlayerReplicationInfo.TeamID = 1;
	}
	J0xA9:

	// End:0x1C7 [Loop If]
	if(__NFUN_129__(bSuccess))
	{
		bSuccess = true;
		P = Level.ControllerList;
		J0xD0:

		// End:0x187 [Loop If]
		if(__NFUN_119__(P, none))
		{
			// End:0x170
			if(__NFUN_130__(__NFUN_130__(__NFUN_130__(P.bIsPlayer, __NFUN_119__(P, Other)), __NFUN_114__(P.PlayerReplicationInfo.Team, Other.PlayerReplicationInfo.Team)), __NFUN_154__(P.PlayerReplicationInfo.TeamID, Other.PlayerReplicationInfo.TeamID)))
			{
				bSuccess = false;
			}
			P = P.nextController;
			// [Loop Continue]
			goto J0xD0;
		}
		// End:0x1C4
		if(__NFUN_129__(bSuccess))
		{
			Other.PlayerReplicationInfo.TeamID = __NFUN_146__(Other.PlayerReplicationInfo.TeamID, 1);
		}
		// [Loop Continue]
		goto J0xA9;
	}
	return true;
	return;
}

function RemoveFromTeam(Controller Other)
{
	__NFUN_166__(Size);
	return;
}

defaultproperties
{
	TeamName="Team"
	TeamColor=(R=255,G=0,B=0,A=255)
	AltTeamColor=(R=200,G=0,B=0,A=255)
	ColorNames[0]="Red"
	ColorNames[1]="Blue"
	ColorNames[2]="Green"
	ColorNames[3]="Gold"
}