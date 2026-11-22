class FailedConnect extends LocalMessage
	abstract
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var localized string FailMessage[4];

static function int GetFailSwitch(string FailString)
{
	// End:0x14
	if(__NFUN_124__(FailString, "NEEDPW"))
	{
		return 0;
	}
	// End:0x29
	if(__NFUN_124__(FailString, "WRONGPW"))
	{
		return 1;
	}
	// End:0x43
	if(__NFUN_124__(FailString, "GAMESTARTED"))
	{
		return 2;
	}
	return 3;
	return;
}

static function string GetString(optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	return default.FailMessage[__NFUN_251__(Switch, 0, 3)];
	return;
}

defaultproperties
{
	FailMessage[0]="FAILED TO JOIN GAME.  NEED PASSWORD."
	FailMessage[1]="FAILED TO JOIN GAME.  WRONG PASSWORD."
	FailMessage[2]="FAILED TO JOIN GAME.  GAME HAS STARTED."
	FailMessage[3]="FAILED TO JOIN GAME."
	bIsUnique=true
	bFadeMessage=true
	DrawColor=(R=255,G=0,B=128,A=255)
	FontSize=1
}