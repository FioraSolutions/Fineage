class oa_force_rage_ta extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter20
         // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_ta.SpriteEmitter20'
         begin object name="SpriteEmitter20" class=Engine.SpriteEmitter
         Acceleration=(X=0.0000000,Y=0.0000000,Z=80.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.7000000
         FadeOutStartTime=0.3600000
         FadeOut=true
         MaxParticles=7
         RespawnDeadParticles=false
         Name="smoke"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
         StartLocationRange=(X=(Min=-12.0000000,Max=12.0000000),Y=(Min=-12.0000000,Max=12.0000000),Z=(Min=0.0000000,Max=0.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.6000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.1000000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
         StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
         UniformSize=true
         InitialParticlesPerSecond=100.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0072'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=1
         SubdivisionEnd=15
         LifetimeRange=(Min=0.6000000,Max=0.8000000)
         StartVelocityRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=10.0000000,Max=40.0000000))
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter20'
     Begin Object Class=MeshEmitter Name=MeshEmitter8
         // Reference: MeshEmitter'lineageeffect.dec.oa_force_rage_ta.MeshEmitter8'
         begin object name="MeshEmitter8" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power04'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=0.0000000,Y=0.0000000,Z=-50.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.6000000,Max=0.6000000))
         Opacity=0.6000000
         FadeOutStartTime=0.1200000
         FadeOut=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="RingWave"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=3.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1.0000000,Max=2.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0600000,RelativeSize=1.8000000)
         SizeScale[1]=(RelativeTime=0.1600000,RelativeSize=2.5000000)
         SizeScale[2]=(RelativeTime=0.4600000,RelativeSize=3.4000001)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=4.0000000)
         StartSizeRange=(X=(Min=0.1300000,Max=0.1300000),Y=(Min=0.1300000,Max=0.1300000),Z=(Min=0.1300000,Max=0.1300000))
         InitialParticlesPerSecond=30.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         StartVelocityRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter8'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter21
         // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_ta.SpriteEmitter21'
         begin object name="SpriteEmitter21" class=Engine.SpriteEmitter
         UseDirectionAs=1
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.9000000,Max=0.9000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.2320000
         FadeOut=true
         MaxParticles=5
         RespawnDeadParticles=false
         Name="Flame"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=20.0000000)
         StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-5.0000000,Max=5.0000000))
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=10.0000000,Max=10.0000000))
         SpinParticles=true
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.3500000,RelativeSize=2.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
         StartSizeRange=(X=(Min=8.0000000,Max=10.0000000),Y=(Min=-22.0000000,Max=-12.0000000),Z=(Min=8.0000000,Max=10.0000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'FX_E_T.Flameset_a.flame_a00'
         LifetimeRange=(Min=0.3000000,Max=0.6000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=10.0000000,Max=10.0000000))
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter21'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter22
         // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_ta.SpriteEmitter22'
         begin object name="SpriteEmitter22" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.3000000
         FadeOutStartTime=0.1980000
         FadeOut=true
         FadeInEndTime=0.0720000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Corona"
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0900000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.4900000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.5000000)
         StartSizeRange=(X=(Min=15.0000000,Max=15.0000000),Y=(Min=15.0000000,Max=15.0000000),Z=(Min=15.0000000,Max=15.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0081'
         LifetimeRange=(Min=0.6000000,Max=0.6000000)
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter22'
     Begin Object Class=MeshEmitter Name=MeshEmitter10
         // Reference: MeshEmitter'lineageeffect.dec.oa_force_rage_ta.MeshEmitter10'
         begin object name="MeshEmitter10" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.sparkredcone00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.6000000
         FadeOutStartTime=0.1000000
         FadeOut=true
         MaxParticles=3
         RespawnDeadParticles=false
         Disabled=true
         Name="NeedleMesh"
         SpinParticles=true
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0600000,RelativeSize=3.0000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=0.6000000,Max=0.6000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.2000000,Max=0.2000000)
         end object
     End Object
     Emitters(4)=MeshEmitter'MeshEmitter10'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_ta.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.5000000
         FadeOutStartTime=0.1000000
         FadeOut=true
         MaxParticles=12
         RespawnDeadParticles=false
         Name="FlameOver"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-10.0000000)
         StartLocationRange=(X=(Min=-3.0000000,Max=3.0000000),Y=(Min=-3.0000000,Max=3.0000000),Z=(Min=0.0000000,Max=0.0000000))
         SphereRadiusRange=(Min=10.0000000,Max=10.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.6000000,RelativeSize=0.9500000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         SizeScaleRepeats=3.0000000
         StartSizeRange=(X=(Min=15.0000000,Max=30.0000000),Y=(Min=15.0000000,Max=30.0000000),Z=(Min=15.0000000,Max=30.0000000))
         UniformSize=true
         InitialParticlesPerSecond=70.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0034'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionEnd=2
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.3000000,Max=0.5000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=60.0000000,Max=130.0000000))
         end object
     End Object
     Emitters(5)=SpriteEmitter'SpriteEmitter0'
     // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_ta.SpriteEmitter20'
     Acceleration=(X=0.0000000,Y=0.0000000,Z=80.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.7000000
     FadeOutStartTime=0.3600000
     FadeOut=true
     MaxParticles=7
     RespawnDeadParticles=false
     Name="smoke"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
     StartLocationRange=(X=(Min=-12.0000000,Max=12.0000000),Y=(Min=-12.0000000,Max=12.0000000),Z=(Min=0.0000000,Max=0.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.6000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.1000000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
     StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
     UniformSize=true
     InitialParticlesPerSecond=100.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0072'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=1
     SubdivisionEnd=15
     LifetimeRange=(Min=0.6000000,Max=0.8000000)
     StartVelocityRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=10.0000000,Max=40.0000000))
     Emitters[0]=SpriteEmitter20
     // Reference: MeshEmitter'lineageeffect.dec.oa_force_rage_ta.MeshEmitter8'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power04'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=0.0000000,Y=0.0000000,Z=-50.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.6000000,Max=0.6000000))
     Opacity=0.6000000
     FadeOutStartTime=0.1200000
     FadeOut=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="RingWave"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=3.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1.0000000,Max=2.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0600000,RelativeSize=1.8000000)
     SizeScale[1]=(RelativeTime=0.1600000,RelativeSize=2.5000000)
     SizeScale[2]=(RelativeTime=0.4600000,RelativeSize=3.4000001)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=4.0000000)
     StartSizeRange=(X=(Min=0.1300000,Max=0.1300000),Y=(Min=0.1300000,Max=0.1300000),Z=(Min=0.1300000,Max=0.1300000))
     InitialParticlesPerSecond=30.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     StartVelocityRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[1]=MeshEmitter8
     // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_ta.SpriteEmitter21'
     UseDirectionAs=1
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.9000000,Max=0.9000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.2320000
     FadeOut=true
     MaxParticles=5
     RespawnDeadParticles=false
     Name="Flame"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=20.0000000)
     StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-5.0000000,Max=5.0000000))
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=10.0000000,Max=10.0000000))
     SpinParticles=true
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.3500000,RelativeSize=2.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
     StartSizeRange=(X=(Min=8.0000000,Max=10.0000000),Y=(Min=-22.0000000,Max=-12.0000000),Z=(Min=8.0000000,Max=10.0000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'FX_E_T.Flameset_a.flame_a00'
     LifetimeRange=(Min=0.3000000,Max=0.6000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=10.0000000,Max=10.0000000))
     Emitters[2]=SpriteEmitter21
     // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_ta.SpriteEmitter22'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.3000000
     FadeOutStartTime=0.1980000
     FadeOut=true
     FadeInEndTime=0.0720000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Corona"
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0900000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.4900000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.5000000)
     StartSizeRange=(X=(Min=15.0000000,Max=15.0000000),Y=(Min=15.0000000,Max=15.0000000),Z=(Min=15.0000000,Max=15.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0081'
     LifetimeRange=(Min=0.6000000,Max=0.6000000)
     Emitters[3]=SpriteEmitter22
     // Reference: MeshEmitter'lineageeffect.dec.oa_force_rage_ta.MeshEmitter10'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.sparkredcone00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.6000000
     FadeOutStartTime=0.1000000
     FadeOut=true
     MaxParticles=3
     RespawnDeadParticles=false
     Disabled=true
     Name="NeedleMesh"
     SpinParticles=true
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0600000,RelativeSize=3.0000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=0.6000000,Max=0.6000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.2000000,Max=0.2000000)
     Emitters[4]=MeshEmitter10
     // Reference: SpriteEmitter'lineageeffect.dec.oa_force_rage_ta.SpriteEmitter0'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.5000000
     FadeOutStartTime=0.1000000
     FadeOut=true
     MaxParticles=12
     RespawnDeadParticles=false
     Name="FlameOver"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-10.0000000)
     StartLocationRange=(X=(Min=-3.0000000,Max=3.0000000),Y=(Min=-3.0000000,Max=3.0000000),Z=(Min=0.0000000,Max=0.0000000))
     SphereRadiusRange=(Min=10.0000000,Max=10.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.6000000,RelativeSize=0.9500000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     SizeScaleRepeats=3.0000000
     StartSizeRange=(X=(Min=15.0000000,Max=30.0000000),Y=(Min=15.0000000,Max=30.0000000),Z=(Min=15.0000000,Max=30.0000000))
     UniformSize=true
     InitialParticlesPerSecond=70.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0034'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionEnd=2
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.3000000,Max=0.5000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=60.0000000,Max=130.0000000))
     Emitters[5]=SpriteEmitter0
     AutoDestroy=true
     bLightChanged=true
     bNoDelete=false
     bSunAffect=true
     DrawScale=0.2000000
     bDirectional=true
}
