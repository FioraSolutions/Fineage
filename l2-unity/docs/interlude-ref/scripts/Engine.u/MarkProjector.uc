class MarkProjector extends Projector
	native
	placeable;

var() Vector DesireLocation;
var() Vector OffsetDesireLocation;
var() bool bAttachMark;
var() Actor ProjectedActor;

// Export UMarkProjector::execUpdateDesireLocation(FFrame&, void* const)
native final function bool UpdateDesireLocation();

simulated event PostBeginPlay()
{
	super.PostBeginPlay();
	__NFUN_117__('Tick');
	return;
}

simulated event Destroyed()
{
	super(Actor).Destroyed();
	return;
}

simulated function Timer()
{
	DetachProjector(true);
	return;
}

function UpdateMarkProjector()
{
	// End:0x5F
	if(bAttachMark)
	{
		DetachProjector(true);
		__NFUN_262__(false, false, false);
		FOV = 1;
		UpdateDesireLocation();
		__NFUN_267__(DesireLocation);
		__NFUN_299__(Rotator(__NFUN_211__(OffsetDesireLocation)));
		SetDrawScale(0.1000000);
		AttachProjector();
		__NFUN_262__(true, false, false);
		bAttachMark = false;
		__NFUN_280__(10.0000000, false);
	}
	return;
}

simulated function Tick(float DeltaTime)
{
	super(Actor).Tick(DeltaTime);
	UpdateMarkProjector();
	return;
}

simulated event Touch(Actor Other)
{
	// End:0x69
	if(__NFUN_130__(__NFUN_130__(Other.bAcceptsProjectors, __NFUN_132__(__NFUN_254__(ProjectTag, 'None'), __NFUN_254__(Other.Tag, ProjectTag))), __NFUN_130__(bProjectStaticMesh, __NFUN_119__(Other.StaticMesh, none))))
	{
		AttachActor(Other);
	}
	return;
}

defaultproperties
{
	MaterialBlendingOp=2
	FrameBufferBlendingOp=2
	ProjTexture=Texture'Engine.Gui021'
	bProjectBSP=false
	bProjectStaticMesh=false
	bProjectParticles=false
	bProjectActor=false
	bClipBSP=true
	bGradient=true
	bProjectOnAlpha=true
	bProjectOnParallelBSP=true
	bStatic=false
}