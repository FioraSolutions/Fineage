class Ladder extends SmallNavigationPoint
	native
	placeable
	hidecategories(Lighting,LightColor,Karma,Force);

var LadderVolume MyLadder;
var Ladder LadderList;

event bool SuggestMovePreparation(Pawn Other)
{
	// End:0x0D
	if(__NFUN_114__(MyLadder, none))
	{
		return false;
	}
	// End:0x3C
	if(__NFUN_129__(MyLadder.InUse(Other)))
	{
		MyLadder.PendingClimber = Other;
		return false;
	}
	Other.Controller.bPreparingMove = true;
	Other.Acceleration = vect(0.0000000, 0.0000000, 0.0000000);
	return true;
	return;
}

defaultproperties
{
	bSpecialMove=true
	bNotBased=true
	Texture=Texture'Engine.S_Ladder'
	bDirectional=true
}