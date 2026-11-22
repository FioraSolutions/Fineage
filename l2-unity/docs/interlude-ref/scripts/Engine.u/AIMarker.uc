class AIMarker extends SmallNavigationPoint
	native
	notplaceable
	hidecategories(Lighting,LightColor,Karma,Force);

cpptext
{
	virtual UBOOL IsIdentifiedAs(FName ActorName);

}

var AIScript markedScript;

defaultproperties
{
	bCollideWhenPlacing=false
	bHiddenEd=true
}