class AnimNotify_Effect extends AnimNotify
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner );

#if defined(__L2)	// 2006/07/26 NeverDie
	virtual AActor* GetLastSpawnedEffect();
	virtual void SetLastSpawnedEffect( AActor* a_pLastSpawnedEffect );
#endif

}

var() Class<Actor> EffectClass;
var() name Bone;
var() Vector OffsetLocation;
var() Rotator OffsetRotation;
var() bool Attach;
var() name Tag;
var() float DrawScale;
var() Vector DrawScale3D;
var() bool TrailCamera;
var() bool IndependentRotation;
var() float EffectScale;
var private transient Actor LastSpawnedEffect;

defaultproperties
{
	DrawScale=1.0000000
	DrawScale3D=(X=1.0000000,Y=1.0000000,Z=1.0000000)
	EffectScale=1.0000000
}