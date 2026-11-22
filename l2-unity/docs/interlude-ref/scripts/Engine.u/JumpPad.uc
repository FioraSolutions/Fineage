class JumpPad extends NavigationPoint
	native
	notplaceable
	hidecategories(Lighting,LightColor,Karma,Force);

cpptext
{
	void addReachSpecs(APawn * Scout, UBOOL bOnlyChanged);
	void RenderEditorSelected(FLevelSceneNode* SceneNode,FRenderInterface* RI, FDynamicActor* FDA);

}

var Vector JumpVelocity;
var Actor JumpTarget;
var() float JumpZModifier;
var() Sound JumpSound;

function PostBeginPlay()
{
	local NavigationPoint N;

	super.PostBeginPlay();
	// End:0x50
	foreach __NFUN_304__(Class'Engine.NavigationPoint', N)
	{
		// End:0x4F
		if(__NFUN_130__(__NFUN_119__(N, self), NearSpot(N.Location)))
		{
			__NFUN_161__(N.ExtraCost, 1000);
		}		
	}	
	return;
}

event Touch(Actor Other)
{
	// End:0x12
	if(__NFUN_114__(Pawn(Other), none))
	{
		return;
	}
	PendingTouch = Other.PendingTouch;
	Other.PendingTouch = self;
	return;
}

simulated event PostTouch(Actor Other)
{
	local Pawn P;

	P = Pawn(Other);
	// End:0x1D
	if(__NFUN_114__(P, none))
	{
		return;
	}
	// End:0x38
	if(__NFUN_150__(int(P.Role), int(ROLE_AutonomousProxy)))
	{
		return;
	}
	// End:0x70
	if(__NFUN_130__(__NFUN_154__(int(P.Role), int(ROLE_AutonomousProxy)), __NFUN_177__(P.Velocity.Z, float(0))))
	{
		return;
	}
	// End:0xFD
	if(__NFUN_119__(AIController(P.Controller), none))
	{
		P.Controller.MoveTarget = JumpTarget;
		P.Controller.Focus = JumpTarget;
		P.Controller.MoveTimer = 2.0000000;
		P.DestinationOffset = JumpTarget.CollisionRadius;
	}
	// End:0x124
	if(__NFUN_154__(int(P.Physics), int(1)))
	{
		P.__NFUN_3970__(2);
	}
	P.Velocity = JumpVelocity;
	P.Acceleration = vect(0.0000000, 0.0000000, 0.0000000);
	// End:0x170
	if(__NFUN_119__(JumpSound, none))
	{
		P.__NFUN_264__(JumpSound);
	}
	return;
}

defaultproperties
{
	JumpVelocity=(X=0.0000000,Y=0.0000000,Z=1200.0000000)
	JumpZModifier=1.0000000
	bDestinationOnly=true
	bCollideActors=true
}