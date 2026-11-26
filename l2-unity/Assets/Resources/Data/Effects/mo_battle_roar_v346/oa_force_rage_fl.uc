class oa_force_rage_fl extends NSkillProjectile;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter1
         // Reference: MeshEmitter'lineageeffect.dec.oa_force_rage_fl.MeshEmitter1'
         begin object name="MeshEmitter1" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windknifeball00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=0.3000000,Max=0.3000000))
         FadeOutStartTime=2.0000000
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Screw"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.0000000,Z=0.0000000)
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=3.0000000,Max=3.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.6700000,RelativeSize=1.4000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=2.2000000)
         StartSizeRange=(X=(Min=0.3500000,Max=0.3500000),Y=(Min=0.3000000,Max=0.3000000),Z=(Min=0.3000000,Max=0.3000000))
         InitialParticlesPerSecond=100000000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         LifetimeRange=(Min=2.0000000,Max=2.0000000)
         VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter2
         // Reference: MeshEmitter'lineageeffect.dec.oa_force_rage_fl.MeshEmitter2'
         begin object name="MeshEmitter2" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Fire.fireball00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.8000000
         FadeOutStartTime=2.0000000
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="FlameMesh"
         StartLocationOffset=(X=-3.0000000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=0.5000000,Y=0.5000000,Z=0.0000000)
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=2.0000000,Max=2.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.9900000,Max=0.9900000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.1000000,RelativeSize=1.7000000)
         SizeScale[1]=(RelativeTime=0.1700000,RelativeSize=1.5000000)
         SizeScale[2]=(RelativeTime=0.2600000,RelativeSize=1.6000000)
         SizeScale[3]=(RelativeTime=0.5200000,RelativeSize=1.5400000)
         SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
         StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0800000,Max=0.0800000),Z=(Min=0.0800000,Max=0.0800000))
         InitialParticlesPerSecond=100000000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         LifetimeRange=(Min=2.0000000,Max=2.0000000)
         VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter3
         // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_fl.SpriteEmitter3'
         begin object name="SpriteEmitter3" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
         Opacity=0.5600000
         FadeOutStartTime=0.0110000
         FadeOut=true
         CoordinateSystem=4
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Flash"
         SphereRadiusRange=(Min=1.0000000,Max=1.0000000)
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=4.0000000,Max=4.0000000))
         SpinParticles=true
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=25.0000000,Max=25.0000000),Y=(Min=25.0000000,Max=25.0000000),Z=(Min=25.0000000,Max=25.0000000))
         UniformSize=true
         InitialParticlesPerSecond=100000000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionEnd=1
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.2500000,Max=0.2500000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter3'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter7
         // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_fl.SpriteEmitter7'
         begin object name="SpriteEmitter7" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.3000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[2]=(RelativeTime=0.6964290,Color=(R=128,G=128,B=128,A=255))
         ColorScale[3]=(RelativeTime=0.9250000,Color=(R=124,G=124,B=124,A=255))
         ColorScale[4]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=2.0000000
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Core"
         SphereRadiusRange=(Min=7.0000000,Max=7.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.4900000,RelativeSize=0.9000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         SizeScaleRepeats=20.0000000
         StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
         UniformSize=true
         InitialParticlesPerSecond=100000000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0066'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         SubdivisionEnd=1
         LifetimeRange=(Min=2.0000000,Max=2.0000000)
         VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter7'
     Speed=1000.0000000
     AccSpeed=3000.0000000
     // Reference: MeshEmitter'lineageeffect.dec.oa_force_rage_fl.MeshEmitter1'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windknifeball00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=0.3000000,Max=0.3000000))
     FadeOutStartTime=2.0000000
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Screw"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.0000000,Z=0.0000000)
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=3.0000000,Max=3.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.6700000,RelativeSize=1.4000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=2.2000000)
     StartSizeRange=(X=(Min=0.3500000,Max=0.3500000),Y=(Min=0.3000000,Max=0.3000000),Z=(Min=0.3000000,Max=0.3000000))
     InitialParticlesPerSecond=100000000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     LifetimeRange=(Min=2.0000000,Max=2.0000000)
     VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[0]=MeshEmitter1
     // Reference: MeshEmitter'lineageeffect.dec.oa_force_rage_fl.MeshEmitter2'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Fire.fireball00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.8000000
     FadeOutStartTime=2.0000000
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="FlameMesh"
     StartLocationOffset=(X=-3.0000000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=0.5000000,Y=0.5000000,Z=0.0000000)
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=2.0000000,Max=2.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.9900000,Max=0.9900000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.1000000,RelativeSize=1.7000000)
     SizeScale[1]=(RelativeTime=0.1700000,RelativeSize=1.5000000)
     SizeScale[2]=(RelativeTime=0.2600000,RelativeSize=1.6000000)
     SizeScale[3]=(RelativeTime=0.5200000,RelativeSize=1.5400000)
     SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
     StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0800000,Max=0.0800000),Z=(Min=0.0800000,Max=0.0800000))
     InitialParticlesPerSecond=100000000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     LifetimeRange=(Min=2.0000000,Max=2.0000000)
     VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[1]=MeshEmitter2
     // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_fl.SpriteEmitter3'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
     Opacity=0.5600000
     FadeOutStartTime=0.0110000
     FadeOut=true
     CoordinateSystem=4
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Flash"
     SphereRadiusRange=(Min=1.0000000,Max=1.0000000)
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=4.0000000,Max=4.0000000))
     SpinParticles=true
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=25.0000000,Max=25.0000000),Y=(Min=25.0000000,Max=25.0000000),Z=(Min=25.0000000,Max=25.0000000))
     UniformSize=true
     InitialParticlesPerSecond=100000000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionEnd=1
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.2500000,Max=0.2500000)
     Emitters[2]=SpriteEmitter3
     // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_fl.SpriteEmitter7'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.3000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[2]=(RelativeTime=0.6964290,Color=(R=128,G=128,B=128,A=255))
     ColorScale[3]=(RelativeTime=0.9250000,Color=(R=124,G=124,B=124,A=255))
     ColorScale[4]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=2.0000000
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Core"
     SphereRadiusRange=(Min=7.0000000,Max=7.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.4900000,RelativeSize=0.9000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     SizeScaleRepeats=20.0000000
     StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
     UniformSize=true
     InitialParticlesPerSecond=100000000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0066'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     SubdivisionEnd=1
     LifetimeRange=(Min=2.0000000,Max=2.0000000)
     VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[3]=SpriteEmitter7
     AutoDestroy=true
     bLightChanged=true
     bSunAffect=true
     DrawScale=0.0500000
     bDirectional=true
}
