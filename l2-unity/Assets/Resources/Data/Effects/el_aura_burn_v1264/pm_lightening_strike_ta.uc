class pm_lightening_strike_ta extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter0
         // Reference: MeshEmitter'lineageeffect.dec.pm_lightening_strike_ta.MeshEmitter0'
         begin object name="MeshEmitter0" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.7142860,Color=(R=145,G=145,B=145,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=8.0000000
         ColorMultiplierRange=(X=(Min=0.6000000,Max=0.6000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.8800000
         FadeOut=true
         MaxParticles=8
         RespawnDeadParticles=false
         Name="Bodybolt"
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=10.0000000,Max=10.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.5000000,Max=1.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.5400000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
         StartSizeRange=(X=(Min=0.3000000,Max=0.6000000),Y=(Min=0.3000000,Max=0.6000000),Z=(Min=0.3000000,Max=0.6000000))
         InitialParticlesPerSecond=30.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.6000000,Max=1.2000000)
         InitialDelayRange=(Min=0.1000000,Max=0.1000000)
         StartVelocityRange=(X=(Min=-40.0000000,Max=40.0000000),Y=(Min=-40.0000000,Max=40.0000000),Z=(Min=-80.0000000,Max=80.0000000))
         VelocityLossRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter0'
     Begin Object Class=MeshEmitter Name=MeshEmitter1
         // Reference: MeshEmitter'lineageeffect.dec.pm_lightening_strike_ta.MeshEmitter1'
         begin object name="MeshEmitter1" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.7142860,Color=(R=145,G=145,B=145,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=4.0000000
         ColorMultiplierRange=(X=(Min=0.6000000,Max=0.6000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.8000000
         FadeOutStartTime=0.3600000
         FadeOut=true
         MaxParticles=6
         RespawnDeadParticles=false
         Name="Linebolt"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=100.0000000)
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=10.0000000,Max=10.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.6000000),Y=(Min=0.3000000,Max=0.6000000),Z=(Min=0.3000000,Max=0.6000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0900000,RelativeSize=1.3000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=0.8000000)
         SizeScale[2]=(RelativeTime=0.6000000,RelativeSize=0.8000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.3000000,Max=0.5000000),Z=(Min=0.3000000,Max=0.5000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.8000000)
         InitialDelayRange=(Min=0.1000000,Max=0.1000000)
         StartVelocityRange=(X=(Min=-30.0000000,Max=30.0000000),Y=(Min=-30.0000000,Max=30.0000000),Z=(Min=-250.0000000,Max=20.0000000))
         VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=3.0000000,Max=3.0000000),Z=(Min=1.0000000,Max=1.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter1'
     Begin Object Class=BeamEmitter Name=BeamEmitter0
         // Reference: BeamEmitter'lineageeffect.dec.pm_lightening_strike_ta.BeamEmitter0'
         begin object name="BeamEmitter0" class=Engine.BeamEmitter
         BeamEndPoints=/* Array type was not detected. */
         DetermineEndPointBy=2
         LowFrequencyNoiseRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=0.0000000,Max=0.0000000))
         HighFrequencyPoints=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.8000000
         FadeOutStartTime=0.2300000
         FadeOut=true
         FadeInEndTime=0.0175000
         FadeIn=true
         MaxParticles=7
         RespawnDeadParticles=false
         Name="Beam"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=100.0000000)
         StartSizeRange=(X=(Min=14.0000000,Max=14.0000000),Y=(Min=14.0000000,Max=14.0000000),Z=(Min=14.0000000,Max=14.0000000))
         InitialParticlesPerSecond=500.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Spark.fx_m_t0042'
         LifetimeRange=(Min=0.2000000,Max=0.4000000)
         end object
     End Object
     Emitters(2)=BeamEmitter'BeamEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.pm_lightening_strike_ta.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5392860,Color=(R=134,G=183,B=255,A=255))
         ColorScale[2]=(RelativeTime=0.8571430,Color=(R=26,G=0,B=255,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorMultiplierRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.5000000
         MaxParticles=5
         RespawnDeadParticles=false
         Name="Core"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=95.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=3.0000000)
         SizeScale[1]=(RelativeTime=0.1900000,RelativeSize=1.5000000)
         SizeScale[2]=(RelativeTime=0.7900000,RelativeSize=1.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
         StartSizeRange=(X=(Min=17.0000000,Max=17.0000000),Y=(Min=17.0000000,Max=17.0000000),Z=(Min=17.0000000,Max=17.0000000))
         UniformSize=true
         InitialParticlesPerSecond=20.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=3
         SubdivisionEnd=4
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=BeamEmitter Name=BeamEmitter1
         // Reference: BeamEmitter'lineageeffect.dec.pm_lightening_strike_ta.BeamEmitter1'
         begin object name="BeamEmitter1" class=Engine.BeamEmitter
         LowFrequencyNoiseRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
         HighFrequencyPoints=5
         NoiseDeterminesEndPoint=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.3178570,Color=(R=170,G=249,B=255,A=255))
         ColorScale[2]=(RelativeTime=0.6642860,Color=(R=146,G=142,B=255,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorMultiplierRange=(X=(Min=0.6000000,Max=0.6000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.8000000
         FadeOutStartTime=0.4000000
         MaxParticles=6
         RespawnDeadParticles=false
         Name="BodyBeam"
         StartLocationShape=2
         SphereRadiusRange=(Min=0.1000000,Max=0.1000000)
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=0.1000000,Max=0.1000000))
         UseSizeScale=true
         StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
         InitialParticlesPerSecond=30.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Spark.fx_m_t0042'
         LifetimeRange=(Min=0.2200000,Max=0.3200000)
         InitialDelayRange=(Min=0.1000000,Max=0.1000000)
         StartVelocityRange=(X=(Min=130.0000000,Max=130.0000000),Y=(Min=130.0000000,Max=130.0000000),Z=(Min=100.0000000,Max=100.0000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(4)=BeamEmitter'BeamEmitter1'
     // Reference: MeshEmitter'lineageeffect.dec.pm_lightening_strike_ta.MeshEmitter0'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.7142860,Color=(R=145,G=145,B=145,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=8.0000000
     ColorMultiplierRange=(X=(Min=0.6000000,Max=0.6000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.8800000
     FadeOut=true
     MaxParticles=8
     RespawnDeadParticles=false
     Name="Bodybolt"
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=10.0000000,Max=10.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.5000000,Max=1.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.5400000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
     StartSizeRange=(X=(Min=0.3000000,Max=0.6000000),Y=(Min=0.3000000,Max=0.6000000),Z=(Min=0.3000000,Max=0.6000000))
     InitialParticlesPerSecond=30.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.6000000,Max=1.2000000)
     InitialDelayRange=(Min=0.1000000,Max=0.1000000)
     StartVelocityRange=(X=(Min=-40.0000000,Max=40.0000000),Y=(Min=-40.0000000,Max=40.0000000),Z=(Min=-80.0000000,Max=80.0000000))
     VelocityLossRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
     Emitters[0]=MeshEmitter0
     // Reference: MeshEmitter'lineageeffect.dec.pm_lightening_strike_ta.MeshEmitter1'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.7142860,Color=(R=145,G=145,B=145,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=4.0000000
     ColorMultiplierRange=(X=(Min=0.6000000,Max=0.6000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.8000000
     FadeOutStartTime=0.3600000
     FadeOut=true
     MaxParticles=6
     RespawnDeadParticles=false
     Name="Linebolt"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=100.0000000)
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=10.0000000,Max=10.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.6000000),Y=(Min=0.3000000,Max=0.6000000),Z=(Min=0.3000000,Max=0.6000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0900000,RelativeSize=1.3000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=0.8000000)
     SizeScale[2]=(RelativeTime=0.6000000,RelativeSize=0.8000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.3000000,Max=0.5000000),Z=(Min=0.3000000,Max=0.5000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.8000000)
     InitialDelayRange=(Min=0.1000000,Max=0.1000000)
     StartVelocityRange=(X=(Min=-30.0000000,Max=30.0000000),Y=(Min=-30.0000000,Max=30.0000000),Z=(Min=-250.0000000,Max=20.0000000))
     VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=3.0000000,Max=3.0000000),Z=(Min=1.0000000,Max=1.0000000))
     Emitters[1]=MeshEmitter1
     // Reference: BeamEmitter'lineageeffect.dec.pm_lightening_strike_ta.BeamEmitter0'
     BeamEndPoints=/* Array type was not detected. */
     DetermineEndPointBy=2
     LowFrequencyNoiseRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=0.0000000,Max=0.0000000))
     HighFrequencyPoints=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.8000000
     FadeOutStartTime=0.2300000
     FadeOut=true
     FadeInEndTime=0.0175000
     FadeIn=true
     MaxParticles=7
     RespawnDeadParticles=false
     Name="Beam"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=100.0000000)
     StartSizeRange=(X=(Min=14.0000000,Max=14.0000000),Y=(Min=14.0000000,Max=14.0000000),Z=(Min=14.0000000,Max=14.0000000))
     InitialParticlesPerSecond=500.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Spark.fx_m_t0042'
     LifetimeRange=(Min=0.2000000,Max=0.4000000)
     Emitters[2]=BeamEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.pm_lightening_strike_ta.SpriteEmitter0'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5392860,Color=(R=134,G=183,B=255,A=255))
     ColorScale[2]=(RelativeTime=0.8571430,Color=(R=26,G=0,B=255,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorMultiplierRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.5000000
     MaxParticles=5
     RespawnDeadParticles=false
     Name="Core"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=95.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=3.0000000)
     SizeScale[1]=(RelativeTime=0.1900000,RelativeSize=1.5000000)
     SizeScale[2]=(RelativeTime=0.7900000,RelativeSize=1.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
     StartSizeRange=(X=(Min=17.0000000,Max=17.0000000),Y=(Min=17.0000000,Max=17.0000000),Z=(Min=17.0000000,Max=17.0000000))
     UniformSize=true
     InitialParticlesPerSecond=20.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=3
     SubdivisionEnd=4
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[3]=SpriteEmitter0
     // Reference: BeamEmitter'lineageeffect.dec.pm_lightening_strike_ta.BeamEmitter1'
     LowFrequencyNoiseRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
     HighFrequencyPoints=5
     NoiseDeterminesEndPoint=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.3178570,Color=(R=170,G=249,B=255,A=255))
     ColorScale[2]=(RelativeTime=0.6642860,Color=(R=146,G=142,B=255,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorMultiplierRange=(X=(Min=0.6000000,Max=0.6000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.8000000
     FadeOutStartTime=0.4000000
     MaxParticles=6
     RespawnDeadParticles=false
     Name="BodyBeam"
     StartLocationShape=2
     SphereRadiusRange=(Min=0.1000000,Max=0.1000000)
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=0.1000000,Max=0.1000000))
     UseSizeScale=true
     StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
     InitialParticlesPerSecond=30.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Spark.fx_m_t0042'
     LifetimeRange=(Min=0.2200000,Max=0.3200000)
     InitialDelayRange=(Min=0.1000000,Max=0.1000000)
     StartVelocityRange=(X=(Min=130.0000000,Max=130.0000000),Y=(Min=130.0000000,Max=130.0000000),Z=(Min=100.0000000,Max=100.0000000))
     GetVelocityDirectionFrom=2
     Emitters[4]=BeamEmitter1
     AutoDestroy=true
     bRotEmitter=true
     RotPerSecond=(Pitch=0,Yaw=-10000,Roll=0)
     bNoDelete=false
     DrawScale=0.4000000
     bDirectional=true
}
