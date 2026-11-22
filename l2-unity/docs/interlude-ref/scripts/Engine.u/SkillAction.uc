class SkillAction extends Object
	abstract
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	virtual AEmitter* Notify( AActor *BaseActor, AActor* DestActor );
	// UObject interface.
	virtual void PostEditChange();

}

var() Class<Emitter> EffectClass;
var() bool bOnMultiTarget;
