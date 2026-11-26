class ZoneInfo extends Info
	native
	placeable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var SkyZoneInfo SkyZone;
var() name ZoneTag;
var() localized string LocationName;
var() float KillZ;
var() Actor.eKillZType KillZType;
var() bool bSoftKillZ;
var() const bool bFogZone;
var() bool bTerrainZone;
var() bool bDistanceFog;
var() bool bClearToFogColor;
var const array<TerrainInfo> Terrains;
var Vector AmbientVector;
var(ZoneLight) byte AmbientBrightness;
var(ZoneLight) byte AmbientHue;
var(ZoneLight) byte AmbientSaturation;
var(ZoneLight) Color DistanceFogColor;
var(ZoneLight) float DistanceFogStart;
var(ZoneLight) float DistanceFogEnd;
var(ZoneLight) float DistanceFogBlendTime;
var(ZoneLight) const Texture EnvironmentMap;
var(ZoneLight) float TexUPanSpeed;
var(ZoneLight) float TexVPanSpeed;
var(ZoneSound) editinline I3DL2Listener ZoneEffect;
var(LensFlare) Texture LensFlare[12];
var(LensFlare) float LensFlareOffset[12];
var(LensFlare) float LensFlareScale[12];
var(ZoneVisibility) bool bLonelyZone;
var(ZoneVisibility) editinline array<editinline ZoneInfo> ManualExcludes;

// Export UZoneInfo::execZoneActors(FFrame&, void* const)
native(308) final iterator function ZoneActors(Class<Actor> BaseClass, out Actor Actor);

simulated function LinkToSkybox()
{
	local SkyZoneInfo TempSkyZone;

	// End:0x21
	foreach __NFUN_304__(Class'Engine.SkyZoneInfo', TempSkyZone, 'None')
	{
		SkyZone = TempSkyZone;		
	}	
	// End:0x8A
	if(__NFUN_154__(int(Level.DetailMode), int(0)))
	{
		// End:0x86
		foreach __NFUN_304__(Class'Engine.SkyZoneInfo', TempSkyZone, 'None')
		{
			// End:0x85
			if(__NFUN_130__(__NFUN_129__(TempSkyZone.bHighDetail), __NFUN_129__(TempSkyZone.bSuperHighDetail)))
			{
				SkyZone = TempSkyZone;
			}			
		}				
	}
	else
	{
		// End:0xDC
		if(__NFUN_154__(int(Level.DetailMode), int(1)))
		{
			// End:0xD8
			foreach __NFUN_304__(Class'Engine.SkyZoneInfo', TempSkyZone, 'None')
			{
				// End:0xD7
				if(__NFUN_129__(TempSkyZone.bSuperHighDetail))
				{
					SkyZone = TempSkyZone;
				}				
			}						
		}
		else
		{
			// End:0x117
			if(__NFUN_154__(int(Level.DetailMode), int(2)))
			{
				// End:0x116
				foreach __NFUN_304__(Class'Engine.SkyZoneInfo', TempSkyZone, 'None')
				{
					SkyZone = TempSkyZone;					
				}				
			}
		}
	}
	return;
}

simulated function PreBeginPlay()
{
	super(Actor).PreBeginPlay();
	LinkToSkybox();
	return;
}

event ActorEntered(Actor Other)
{
	return;
}

event ActorLeaving(Actor Other)
{
	return;
}

defaultproperties
{
	KillZ=-10000.0000000
	AmbientSaturation=255
	DistanceFogColor=(R=128,G=128,B=128,A=0)
	DistanceFogStart=3000.0000000
	DistanceFogEnd=8000.0000000
	DistanceFogBlendTime=1.0000000
	TexUPanSpeed=1.0000000
	TexVPanSpeed=1.0000000
	bStatic=true
	bNoDelete=true
	bSunAffect=true
	Texture=Texture'Engine.S_ZoneInfo'
}