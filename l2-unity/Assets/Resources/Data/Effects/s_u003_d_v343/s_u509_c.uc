class s_u509_c extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter9
         // Reference: MeshEmitter'lineageeffect.dec.s_u509_c.MeshEmitter9'
         begin object name="MeshEmitter9" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.wispray00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=254,G=162,B=78,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=33,G=2,B=249,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.9500000,Max=0.9500000))
         Opacity=0.1200000
         FadeOutStartTime=3.0000000
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Beam"
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0500000),Z=(Min=0.0000000,Max=0.0500000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.1800000,Max=0.1800000),Y=(Min=0.1800000,Max=0.1800000),Z=(Min=0.1800000,Max=0.1800000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=2.0000000,Max=2.0000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter9'
     Begin Object Class=MeshEmitter Name=MeshEmitter10
         // Reference: MeshEmitter'lineageeffect.dec.s_u509_c.MeshEmitter10'
         begin object name="MeshEmitter10" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windknifeball00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=-300.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=243,G=149,B=7,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.4000000
         FadeOutStartTime=0.0780000
         FadeOut=true
         CoordinateSystem=4
         RespawnDeadParticles=false
         Name="Wind"
         StartLocationOffset=(X=3.0000000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=3.0000000,Max=3.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
         InitialParticlesPerSecond=10.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter10'
     Begin Object Class=MeshEmitter Name=MeshEmitter17
         // Reference: MeshEmitter'lineageeffect.dec.s_u509_c.MeshEmitter17'
         begin object name="MeshEmitter17" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=-35.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.4000000
         FadeOut=true
         FadeInEndTime=0.2000000
         FadeIn=true
         MaxParticles=15
         RespawnDeadParticles=false
         Name="Elt"
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.2000000,Max=1.0000000),Y=(Min=0.2000000,Max=1.0000000),Z=(Min=0.2000000,Max=1.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
         InitialParticlesPerSecond=10.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter17'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter17
         // Reference: SpriteEmitter'lineageeffect.dec.s_u509_c.SpriteEmitter17'
         begin object name="SpriteEmitter17" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         Acceleration=(X=-10.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=232,G=0,B=0,A=255))
         Opacity=0.4000000
         FadeOutStartTime=0.1200000
         FadeOut=true
         FadeInEndTime=0.1200000
         FadeIn=true
         CoordinateSystem=4
         MaxParticles=40
         RespawnDeadParticles=false
         Name="SpriteEmitter17"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.1000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=10.0000000,Max=16.0000000),Y=(Min=10.0000000,Max=16.0000000),Z=(Min=10.0000000,Max=16.0000000))
         UniformSize=true
         InitialParticlesPerSecond=20.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1026'
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=-2.0000000,Max=-2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter17'
     // Reference: MeshEmitter'lineageeffect.dec.s_u509_c.MeshEmitter9'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.wispray00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=254,G=162,B=78,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=33,G=2,B=249,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.9500000,Max=0.9500000))
     Opacity=0.1200000
     FadeOutStartTime=3.0000000
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Beam"
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0500000),Z=(Min=0.0000000,Max=0.0500000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.1800000,Max=0.1800000),Y=(Min=0.1800000,Max=0.1800000),Z=(Min=0.1800000,Max=0.1800000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=2.0000000,Max=2.0000000)
     Emitters[0]=MeshEmitter9
     // Reference: MeshEmitter'lineageeffect.dec.s_u509_c.MeshEmitter10'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windknifeball00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=-300.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=243,G=149,B=7,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.4000000
     FadeOutStartTime=0.0780000
     FadeOut=true
     CoordinateSystem=4
     RespawnDeadParticles=false
     Name="Wind"
     StartLocationOffset=(X=3.0000000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=3.0000000,Max=3.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
     InitialParticlesPerSecond=10.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[1]=MeshEmitter10
     // Reference: MeshEmitter'lineageeffect.dec.s_u509_c.MeshEmitter17'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=-35.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.4000000
     FadeOut=true
     FadeInEndTime=0.2000000
     FadeIn=true
     MaxParticles=15
     RespawnDeadParticles=false
     Name="Elt"
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.2000000,Max=1.0000000),Y=(Min=0.2000000,Max=1.0000000),Z=(Min=0.2000000,Max=1.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
     InitialParticlesPerSecond=10.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     Emitters[2]=MeshEmitter17
     // Reference: SpriteEmitter'lineageeffect.dec.s_u509_c.SpriteEmitter17'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     Acceleration=(X=-10.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=232,G=0,B=0,A=255))
     Opacity=0.4000000
     FadeOutStartTime=0.1200000
     FadeOut=true
     FadeInEndTime=0.1200000
     FadeIn=true
     CoordinateSystem=4
     MaxParticles=40
     RespawnDeadParticles=false
     Name="SpriteEmitter17"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.1000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=10.0000000,Max=16.0000000),Y=(Min=10.0000000,Max=16.0000000),Z=(Min=10.0000000,Max=16.0000000))
     UniformSize=true
     InitialParticlesPerSecond=20.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1026'
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=-2.0000000,Max=-2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[3]=SpriteEmitter17
     AutoDestroy=true
     Physics=10
     bUseDynamicLights=false
     bNoDelete=false
     bTrailerSameRotation=true
     bTrailerPrePivot=true
     bTrailerNoOwnerDestroy=true
     bAcceptsProjectors=false
     bDirectional=true
}
