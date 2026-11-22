class Projector extends Actor
	native
	placeable;

enum EProjectorBlending
{
	PB_None,                        // 0
	PB_Modulate,                    // 1
	PB_AlphaBlend,                  // 2
	PB_Add                          // 3
};

var() Projector.EProjectorBlending MaterialBlendingOp;
var() Projector.EProjectorBlending FrameBufferBlendingOp;
var() Material ProjTexture;
var() int FOV;
var() int MaxTraceDistance;
var() bool bProjectBSP;
var() bool bProjectTerrain;
var() bool bProjectStaticMesh;
var() bool bProjectParticles;
var() bool bProjectActor;
var() bool bLevelStatic;
var() bool bClipBSP;
var() bool bClipStaticMesh;
var() bool bProjectOnUnlit;
var() bool bGradient;
var() bool bProjectOnBackfaces;
var() bool bProjectOnAlpha;
var() bool bProjectOnParallelBSP;
var() name ProjectTag;
var() bool bDynamicAttach;
var const transient Plane FrustumPlanes[6];
var const transient Vector FrustumVertices[8];
var const transient Box Box;
var const transient ProjectorRenderInfoPtr RenderInfo;
var Texture GradientTexture;
var transient Matrix GradientMatrix;
var transient Matrix Matrix;
var transient Vector OldLocation;
var float ProjectorRadius;

// Export UProjector::execAttachProjector(FFrame&, void* const)
native function AttachProjector();

// Export UProjector::execDetachProjector(FFrame&, void* const)
native function DetachProjector(optional bool Force);

// Export UProjector::execAbandonProjector(FFrame&, void* const)
native function AbandonProjector(optional float LifeTime);

// Export UProjector::execAttachActor(FFrame&, void* const)
native function AttachActor(Actor A);

// Export UProjector::execDetachActor(FFrame&, void* const)
native function DetachActor(Actor A);

simulated event PostBeginPlay()
{
	AttachProjector();
	// End:0x18
	if(bLevelStatic)
	{
		AbandonProjector();
		__NFUN_279__();
	}
	// End:0x27
	if(bProjectActor)
	{
		__NFUN_262__(true, false, false);
	}
	return;
}

simulated event Touch(Actor Other)
{
	// End:0x0D
	if(__NFUN_114__(Other, none))
	{
		return;
	}
	// End:0x76
	if(__NFUN_130__(__NFUN_130__(Other.bAcceptsProjectors, __NFUN_132__(__NFUN_254__(ProjectTag, 'None'), __NFUN_254__(Other.Tag, ProjectTag))), __NFUN_132__(bProjectStaticMesh, __NFUN_114__(Other.StaticMesh, none))))
	{
		AttachActor(Other);
	}
	return;
}

simulated event UnTouch(Actor Other)
{
	DetachActor(Other);
	return;
}

defaultproperties
{
	FrameBufferBlendingOp=1
	MaxTraceDistance=1000
	bProjectBSP=true
	bProjectTerrain=true
	bProjectStaticMesh=true
	bProjectParticles=true
	bProjectActor=true
	GradientTexture=Texture'Engine.GRADIENT_Fade'
	ProjectorRadius=-1.0000000
	bStatic=true
	bHidden=true
	bCheckChangableLevel=true
	RemoteRole=0
	Texture=Texture'Engine.Proj_Icon'
	bDirectional=true
}