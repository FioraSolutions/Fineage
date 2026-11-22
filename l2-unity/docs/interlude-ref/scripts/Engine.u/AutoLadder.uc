class AutoLadder extends Ladder
	native
	notplaceable
	hidecategories(Lighting,LightColor,Karma,Force);

cpptext
{
	virtual UBOOL IsIdentifiedAs(FName ActorName);

}

defaultproperties
{
	bCollideWhenPlacing=false
}