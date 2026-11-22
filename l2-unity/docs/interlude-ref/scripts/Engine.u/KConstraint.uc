class KConstraint extends KActor
	abstract
	native
	placeable;

cpptext
{
#ifdef WITH_KARMA
    virtual MdtConstraintID getKConstraint() const;
    virtual McdModelID getKModel() const;

	virtual void physKarma(FLOAT DeltaTime);

	virtual void PostEditChange();
	virtual void PostEditMove();

    virtual void KUpdateConstraintParams();
	
	virtual void CheckForErrors(); // used for checking that this constraint is valid buring map build
	virtual void RenderEditorSelected(FLevelSceneNode* SceneNode,FRenderInterface* RI, FDynamicActor* FDA);
	virtual UBOOL CheckOwnerUpdated();

	virtual void preKarmaStep(FLOAT DeltaTime) {};
	virtual void postKarmaStep() {};
#endif

}

var const transient int KConstraintData;
var(KarmaConstraint) edfindable Actor KConstraintActor1;
var(KarmaConstraint) edfindable Actor KConstraintActor2;
var(KarmaConstraint) name KConstraintBone1;
var(KarmaConstraint) name KConstraintBone2;
var(KarmaConstraint) const bool bKDisableCollision;
var Vector KPos1;
var Vector KPriAxis1;
var Vector KSecAxis1;
var Vector KPos2;
var Vector KPriAxis2;
var Vector KSecAxis2;
var const bool bKForceFrameUpdate;
var(KarmaConstraint) float KForceThreshold;

// Export UKConstraint::execKUpdateConstraintParams(FFrame&, void* const)
native function KUpdateConstraintParams();

// Export UKConstraint::execKGetConstraintForce(FFrame&, void* const)
native final function KGetConstraintForce(out Vector Force);

// Export UKConstraint::execKGetConstraintTorque(FFrame&, void* const)
native final function KGetConstraintTorque(out Vector Torque);

event KForceExceed(float forceMag)
{
	return;
}

defaultproperties
{
	bKDisableCollision=true
	KPriAxis1=(X=1.0000000,Y=0.0000000,Z=0.0000000)
	KSecAxis1=(X=0.0000000,Y=1.0000000,Z=0.0000000)
	KPriAxis2=(X=1.0000000,Y=0.0000000,Z=0.0000000)
	KSecAxis2=(X=0.0000000,Y=1.0000000,Z=0.0000000)
	DrawType=1
	bHidden=true
	Texture=Texture'Engine.S_KConstraint'
	bCollideActors=false
	bBlockActors=false
	bBlockPlayers=false
	bProjTarget=false
	bBlockKarma=false
}