class SkillAction_LocateEffect extends SkillAction
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	virtual AEmitter* Notify( AActor *BaseActor, AActor *DestActor);
	virtual void PostEditChange();

}

enum EAttachMethod
{
	EAM_None,                       // 0
	EAM_RH,                         // 1
	EAM_LH,                         // 2
	EAM_BoneSpecified,              // 3
	EAM_AliasSpecified,             // 4
	EAM_Trail,                      // 5
	EAM_RF,                         // 6
	EAM_LF                          // 7
};

var(Attach) SkillAction_LocateEffect.EAttachMethod AttachOn;
var(Attach) name AttachBoneName;
var(Attach) bool bAbsolute;
var(Time) float SpawnDelay;
var(positioning) bool bUseCharacterRotation;
var(positioning) Vector offset;
var(positioning) bool bRelativeToCylinder;
var(positioning) bool bSpawnOnTarget;
var(positioning) bool bSizeScale;

defaultproperties
{
	bRelativeToCylinder=true
}