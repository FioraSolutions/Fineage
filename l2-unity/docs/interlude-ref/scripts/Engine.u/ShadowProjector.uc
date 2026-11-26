class ShadowProjector extends Projector
	native
	placeable;

var() Actor ShadowActor;
var() Vector LightDirection;
var() float LightDistance;
var() bool RootMotion;
var() bool bBlobShadow;
var() bool bShadowActive;
var ShadowBitmapMaterial ShadowTexture;
var() bool L2Hidden;
var() bool L2DefaultShadow;

// Export UShadowProjector::execUpdateLightInfo(FFrame&, void* const)
native final function bool UpdateLightInfo();

// Export UShadowProjector::execCheckVisible(FFrame&, void* const)
native final function bool CheckVisible();

event PostBeginPlay()
{
	super(Actor).PostBeginPlay();
	return;
}

event Destroyed()
{
	// End:0x61
	if(__NFUN_119__(ShadowTexture, none))
	{
		ShadowTexture.ShadowActor = none;
		// End:0x4C
		if(__NFUN_129__(ShadowTexture.Invalid))
		{
			Level.ObjectPool.FreeObject(ShadowTexture);
		}
		ShadowTexture = none;
		ProjTexture = none;
		ShadowActor = none;
	}
	super(Actor).Destroyed();
	return;
}

function bool IsUnderWaterCamera()
{
	local Viewport Viewport;

	Viewport = GetViewport();
	// End:0x3D
	if(__NFUN_119__(Viewport, none))
	{
		// End:0x3D
		if(Viewport.Actor.PhysicsVolume.bL2WaterVolume)
		{
			return true;
		}
	}
	return false;
	return;
}

function InitShadow()
{
	local Plane BoundingSphere;

	// End:0x1A9
	if(__NFUN_119__(ShadowActor, none))
	{
		BoundingSphere = ShadowActor.GetRenderBoundingSphere();
		FOV = int(__NFUN_172__(__NFUN_171__(__NFUN_190__(__NFUN_174__(__NFUN_171__(BoundingSphere.W, float(2)), float(160)), LightDistance), float(180)), 3.1415927));
		ShadowTexture = ShadowBitmapMaterial(Level.ObjectPool.AllocateObject(Class'Engine.ShadowBitmapMaterial'));
		ProjTexture = ShadowTexture;
		// End:0x172
		if(__NFUN_119__(ShadowTexture, none))
		{
			SetDrawScale(__NFUN_172__(__NFUN_171__(LightDistance, __NFUN_189__(__NFUN_172__(__NFUN_171__(__NFUN_171__(0.5000000, float(FOV)), 3.1415927), float(180)))), __NFUN_171__(0.5000000, float(ShadowTexture.USize))));
			ShadowTexture.Invalid = false;
			ShadowTexture.bBlobShadow = bBlobShadow;
			ShadowTexture.ShadowActor = ShadowActor;
			ShadowTexture.LightDirection = __NFUN_226__(LightDirection);
			ShadowTexture.LightDistance = LightDistance;
			ShadowTexture.LightFOV = float(FOV);
			ShadowTexture.CullDistance = CullDistance;
			__NFUN_117__('Tick');
			UpdateShadow();			
		}
		else
		{
			__NFUN_231__(__NFUN_112__(string(Name), ".InitShadow: Failed to allocate texture"));
		}		
	}
	else
	{
		__NFUN_231__(__NFUN_112__(string(Name), ".InitShadow: No actor"));
	}
	return;
}

function UpdateShadow()
{
	local Vector ShadowLocation;
	local Plane BoundingSphere;

	DetachProjector(true);
	// End:0x22F
	if(__NFUN_130__(__NFUN_130__(__NFUN_129__(L2Hidden), __NFUN_119__(ShadowActor, none)), __NFUN_129__(ShadowActor.bHidden)))
	{
		UpdateLightInfo();
		// End:0x98
		if(L2DefaultShadow)
		{
			__NFUN_267__(ShadowActor.Location);
			__NFUN_299__(Rotator(__NFUN_211__(vect(0.0000000, 0.0000000, 10.0000000))));
			SetDrawScale(0.0500000);
			MaterialBlendingOp = 0;
			FrameBufferBlendingOp = 2;
			ShadowTexture.bDefaultShadow = true;			
		}
		else
		{
			BoundingSphere = ShadowActor.GetRenderBoundingSphere();
			FOV = int(__NFUN_174__(__NFUN_172__(__NFUN_171__(__NFUN_190__(__NFUN_174__(__NFUN_171__(BoundingSphere.W, float(2)), float(160)), LightDistance), float(180)), 3.1415927), float(5)));
			// End:0x136
			if(__NFUN_130__(__NFUN_154__(int(ShadowActor.DrawType), int(2)), __NFUN_119__(ShadowActor.Mesh, none)))
			{
				ShadowLocation = ShadowActor.GetBoneCoords('None').Origin;				
			}
			else
			{
				ShadowLocation = ShadowActor.Location;
			}
			__NFUN_267__(ShadowLocation);
			__NFUN_299__(Rotator(__NFUN_226__(__NFUN_211__(LightDirection))));
			SetDrawScale(__NFUN_172__(__NFUN_171__(LightDistance, __NFUN_189__(__NFUN_172__(__NFUN_171__(__NFUN_171__(0.5000000, float(FOV)), 3.1415927), float(180)))), __NFUN_171__(0.5000000, float(ShadowTexture.USize))));
			ShadowTexture.ShadowActor = ShadowActor;
			ShadowTexture.LightDirection = __NFUN_226__(LightDirection);
			ShadowTexture.LightDistance = LightDistance;
			ShadowTexture.LightFOV = float(FOV);
			ShadowTexture.Dirty = true;
			MaterialBlendingOp = 0;
			FrameBufferBlendingOp = 2;
			ShadowTexture.bDefaultShadow = false;
		}
		AttachProjector();
	}
	return;
}

function Tick(float DeltaTime)
{
	super(Actor).Tick(DeltaTime);
	CheckVisible();
	UpdateShadow();
	return;
}

defaultproperties
{
	bShadowActive=true
	bProjectActor=false
	bClipBSP=true
	bProjectOnParallelBSP=true
	bDynamicAttach=true
	bNeedCleanup=false
	bStatic=false
	L2NeedTick=false
	CullDistance=1200.0000000
	bOwnerNoSee=true
}