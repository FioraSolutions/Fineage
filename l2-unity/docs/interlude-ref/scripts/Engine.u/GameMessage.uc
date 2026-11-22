class GameMessage extends LocalMessage
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var localized string SwitchLevelMessage;
var localized string LeftMessage;
var localized string FailedTeamMessage;
var localized string FailedPlaceMessage;
var localized string FailedSpawnMessage;
var localized string EnteredMessage;
var localized string MaxedOutMessage;
var localized string OvertimeMessage;
var localized string GlobalNameChange;
var localized string NewTeamMessage;
var localized string NewTeamMessageTrailer;
var localized string NoNameChange;
var localized string VoteStarted;
var localized string VotePassed;
var localized string MustHaveStats;
var localized string NewPlayerMessage;

static function string GetString(optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	switch(Switch)
	{
		// End:0x14
		case 0:
			return default.OvertimeMessage;
			// End:0x176
			break;
		// End:0x42
		case 1:
			// End:0x29
			if(__NFUN_114__(RelatedPRI_1, none))
			{
				return default.NewPlayerMessage;
			}
			return __NFUN_112__(RelatedPRI_1.PlayerName, default.EnteredMessage);
			// End:0x176
			break;
		// End:0x7E
		case 2:
			// End:0x55
			if(__NFUN_114__(RelatedPRI_1, none))
			{
				return "";
			}
			return __NFUN_168__(__NFUN_168__(RelatedPRI_1.OldName, default.GlobalNameChange), RelatedPRI_1.PlayerName);
			// End:0x176
			break;
		// End:0xD5
		case 3:
			// End:0x91
			if(__NFUN_114__(RelatedPRI_1, none))
			{
				return "";
			}
			// End:0x9F
			if(__NFUN_114__(OptionalObject, none))
			{
				return "";
			}
			return __NFUN_112__(__NFUN_168__(__NFUN_168__(RelatedPRI_1.PlayerName, default.NewTeamMessage), TeamInfo(OptionalObject).GetHumanReadableName()), default.NewTeamMessageTrailer);
			// End:0x176
			break;
		// End:0x101
		case 4:
			// End:0xE8
			if(__NFUN_114__(RelatedPRI_1, none))
			{
				return "";
			}
			return __NFUN_112__(RelatedPRI_1.PlayerName, default.LeftMessage);
			// End:0x176
			break;
		// End:0x10F
		case 5:
			return default.SwitchLevelMessage;
			// End:0x176
			break;
		// End:0x11D
		case 6:
			return default.FailedTeamMessage;
			// End:0x176
			break;
		// End:0x12B
		case 7:
			return default.MaxedOutMessage;
			// End:0x176
			break;
		// End:0x139
		case 8:
			return default.NoNameChange;
			// End:0x176
			break;
		// End:0x157
		case 9:
			return __NFUN_168__(RelatedPRI_1.PlayerName, default.VoteStarted);
			// End:0x176
			break;
		// End:0x165
		case 10:
			return default.VotePassed;
			// End:0x176
			break;
		// End:0x173
		case 11:
			return default.MustHaveStats;
			// End:0x176
			break;
		// End:0xFFFF
		default:
			break;
	}
	return "";
	return;
}

defaultproperties
{
	SwitchLevelMessage="Switching Levels"
	LeftMessage=" left the game."
	FailedTeamMessage="Could not find team for player"
	FailedPlaceMessage="Could not find a starting spot"
	FailedSpawnMessage="Could not spawn player"
	EnteredMessage=" entered the game."
	MaxedOutMessage="Server is already at capacity."
	OvertimeMessage="Score tied at the end of regulation. Sudden Death Overtime!!!"
	GlobalNameChange="changed name to"
	NewTeamMessage="is now on"
	NoNameChange="Name is already in use."
	VoteStarted="started a vote."
	VotePassed="Vote passed."
	MustHaveStats="Must have stats enabled to join this server."
	NewPlayerMessage="A new player entered the game."
	bIsSpecial=false
}