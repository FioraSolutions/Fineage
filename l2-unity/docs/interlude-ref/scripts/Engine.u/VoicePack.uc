class VoicePack extends Info
	abstract
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

function ClientInitialize(PlayerReplicationInfo Sender, PlayerReplicationInfo Recipient, name messagetype, byte messageIndex)
{
	return;
}

static function PlayerSpeech(name Type, int Index, string Callsign, Actor PackOwner)
{
	return;
}

static function byte GetMessageIndex(name PhraseName)
{
	return 0;
	return;
}

static function int PickRandomTauntFor(Controller C, bool bNoMature, bool bNoHumanOnly)
{
	return 0;
	return;
}

defaultproperties
{
	LifeSpan=10.0000000
}