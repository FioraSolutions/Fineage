class KConeLimit extends KConstraint
	native
	placeable;

cpptext
{
#ifdef WITH_KARMA
    virtual void KUpdateConstraintParams();
#endif

}

var(KarmaConstraint) float KHalfAngle;
var(KarmaConstraint) float KStiffness;
var(KarmaConstraint) float KDamping;

defaultproperties
{
	KHalfAngle=8200.0000000
	KStiffness=50.0000000
	Texture=Texture'Engine.S_KConeLimit'
	bDirectional=true
}