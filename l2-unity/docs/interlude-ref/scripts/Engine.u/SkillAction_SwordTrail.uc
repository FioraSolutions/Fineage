class SkillAction_SwordTrail extends SkillAction
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	virtual AEmitter* Notify( AActor *BaseActor, AActor *DestActor);
	virtual void PostEditChange();

}

var() float DurationRatio;
var() bool bRightHand;

defaultproperties
{
	DurationRatio=0.8000000
	bRightHand=true
}