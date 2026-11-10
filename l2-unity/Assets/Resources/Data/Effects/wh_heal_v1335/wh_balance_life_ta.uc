class wh_balance_life_ta extends Emitter;

defaultproperties
{
     Begin Object Class=BeamEmitter Name=BeamEmitter1
         // Reference: BeamEmitter'lineageeffect.dec.wh_balance_life_ta.BeamEmitter1'
         begin object name="BeamEmitter1" class=Engine.BeamEmitter
         BeamEndPoints=/* Array type was not detected. */
         DetermineEndPointBy=2
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5214290,Color=(R=231,G=231,B=231,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=10.0000000
         ColorMultiplierRange=(X=(Min=0.5000000,Max=0.7040000),Y=(Min=0.5000000,Max=0.7040000),Z=(Min=0.5000000,Max=0.7040000))
         Opacity=0.1400000
         FadeOutStartTime=1.0000000
         FadeOut=true
         FadeInEndTime=0.5000000
         FadeIn=true
         MaxParticles=9
         RespawnDeadParticles=false
         Name="Beam"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=150.0000000)
         StartLocationRange=(X=(Min=-11.0000000,Max=11.0000000),Y=(Min=-11.0000000,Max=11.0000000),Z=(Min=-30.0000000,Max=30.0000000))
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=70.0000000,Max=110.0000000),Z=(Min=13.0000000,Max=13.0000000))
         StartSizeRange=(X=(Min=6.0000000,Max=10.0000000),Y=(Min=100.0000000,Max=100.0000000),Z=(Min=100.0000000,Max=100.0000000))
         InitialParticlesPerSecond=10.3500004
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0004'
         LifetimeRange=(Min=2.0000000,Max=3.0000000)
         end object
     End Object
     Emitters(0)=BeamEmitter'BeamEmitter1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5392860,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=20.0000000
         ColorMultiplierRange=(X=(Min=0.8500000,Max=1.0000000),Y=(Min=0.8500000,Max=1.0000000),Z=(Min=0.7310000,Max=0.8590000))
         Opacity=0.6000000
         FadeOutStartTime=0.7800000
         FadeOut=true
         FadeInEndTime=0.3640000
         FadeIn=true
         MaxParticles=15
         RespawnDeadParticles=false
         Name="BlueDust"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
         StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-20.0000000,Max=20.0000000))
         SphereRadiusRange=(Min=8.0000000,Max=8.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=-0.2000000,Max=0.2000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=3.0000000)
         SizeScale[1]=(RelativeTime=0.2400000,RelativeSize=6.0000000)
         SizeScale[2]=(RelativeTime=0.5200000,RelativeSize=8.5000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=10.0000000)
         StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
         UniformSize=true
         InitialParticlesPerSecond=15.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0005'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=6
         SubdivisionEnd=8
         UseRandomSubdivision=true
         LifetimeRange=(Min=1.0000000,Max=1.3000000)
         StartVelocityRange=(X=(Min=-6.0000000,Max=6.0000000),Y=(Min=-6.0000000,Max=6.0000000),Z=(Min=-12.1990004,Max=-5.0479999))
         VelocityLossRange=(X=(Min=0.0000000,Max=1.3680000),Y=(Min=0.0000000,Max=1.3680000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         UseDirectionAs=4
         Acceleration=(X=0.0000000,Y=0.0000000,Z=40.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.5000000
         FadeOutStartTime=1.0400000
         FadeOut=true
         FadeInEndTime=0.6890000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Down"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-10.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
         LifetimeRange=(Min=1.3000000,Max=1.3000000)
         InitialDelayRange=(Min=0.2500000,Max=0.2500000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=10.0000000,Max=10.0000000))
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter3
         // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter3'
         begin object name="SpriteEmitter3" class=Engine.SpriteEmitter
         UseDirectionAs=4
         Acceleration=(X=0.0000000,Y=0.0000000,Z=-40.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.5000000
         FadeOutStartTime=1.0400000
         FadeOut=true
         FadeInEndTime=0.7150000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Up"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=75.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
         LifetimeRange=(Min=1.3000000,Max=1.3000000)
         InitialDelayRange=(Min=0.2500000,Max=0.2500000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-10.0000000,Max=-10.0000000))
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter3'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter4
         // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter4'
         begin object name="SpriteEmitter4" class=Engine.SpriteEmitter
         UseDirectionAs=4
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=11,G=23,B=255,A=255))
         Opacity=0.8000000
         FadeOutStartTime=0.1000000
         FadeOut=true
         FadeInEndTime=0.0400000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Final_Circle"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.2800000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.4800000,RelativeSize=1.6000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.8000000)
         StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         InitialDelayRange=(Min=1.3000000,Max=1.3000000)
         end object
     End Object
     Emitters(4)=SpriteEmitter'SpriteEmitter4'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter5
         // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter5'
         begin object name="SpriteEmitter5" class=Engine.SpriteEmitter
         Acceleration=(X=0.0000000,Y=0.0000000,Z=5.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5142860,Color=(R=201,G=201,B=201,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=10.0000000
         ColorMultiplierRange=(X=(Min=0.8500000,Max=1.0000000),Y=(Min=0.8500000,Max=1.0000000),Z=(Min=0.7310000,Max=0.8590000))
         FadeOutStartTime=0.2000000
         FadeOut=true
         MaxParticles=25
         RespawnDeadParticles=false
         Name="fire_dust"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
         StartLocationShape=2
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0005'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=6
         SubdivisionEnd=9
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.8000000,Max=1.0000000)
         InitialDelayRange=(Min=1.4000000,Max=1.4000000)
         StartVelocityRange=(X=(Min=300.0000000,Max=300.0000000),Y=(Min=300.0000000,Max=300.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=0.0000000,Max=0.0000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(5)=SpriteEmitter'SpriteEmitter5'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter6
         // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter6'
         begin object name="SpriteEmitter6" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.2000000
         FadeOutStartTime=0.1000000
         FadeOut=true
         MaxParticles=5
         RespawnDeadParticles=false
         Name="Center_b"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.8800000,Max=0.8800000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=0.1400000,RelativeSize=1.7800000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
         StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         InitialDelayRange=(Min=1.4000000,Max=1.4000000)
         end object
     End Object
     Emitters(6)=SpriteEmitter'SpriteEmitter6'
     Begin Object Class=MeshEmitter Name=MeshEmitter4
         // Reference: MeshEmitter'lineageeffect.dec.wh_balance_life_ta.MeshEmitter4'
         begin object name="MeshEmitter4" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.wispray00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.1000000
         FadeOutStartTime=0.2000000
         FadeOut=true
         MaxParticles=5
         RespawnDeadParticles=false
         Name="0"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=-0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.3000000)
         SizeScale[1]=(RelativeTime=0.1700000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
         StartSizeRange=(X=(Min=0.4000000,Max=0.5400000),Y=(Min=0.4000000,Max=0.5400000),Z=(Min=0.4000000,Max=0.5400000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         InitialDelayRange=(Min=1.4000000,Max=1.4000000)
         end object
     End Object
     Emitters(7)=MeshEmitter'MeshEmitter4'
     // Reference: BeamEmitter'lineageeffect.dec.wh_balance_life_ta.BeamEmitter1'
     BeamEndPoints=/* Array type was not detected. */
     DetermineEndPointBy=2
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5214290,Color=(R=231,G=231,B=231,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=10.0000000
     ColorMultiplierRange=(X=(Min=0.5000000,Max=0.7040000),Y=(Min=0.5000000,Max=0.7040000),Z=(Min=0.5000000,Max=0.7040000))
     Opacity=0.1400000
     FadeOutStartTime=1.0000000
     FadeOut=true
     FadeInEndTime=0.5000000
     FadeIn=true
     MaxParticles=9
     RespawnDeadParticles=false
     Name="Beam"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=150.0000000)
     StartLocationRange=(X=(Min=-11.0000000,Max=11.0000000),Y=(Min=-11.0000000,Max=11.0000000),Z=(Min=-30.0000000,Max=30.0000000))
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=70.0000000,Max=110.0000000),Z=(Min=13.0000000,Max=13.0000000))
     StartSizeRange=(X=(Min=6.0000000,Max=10.0000000),Y=(Min=100.0000000,Max=100.0000000),Z=(Min=100.0000000,Max=100.0000000))
     InitialParticlesPerSecond=10.3500004
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0004'
     LifetimeRange=(Min=2.0000000,Max=3.0000000)
     Emitters[0]=BeamEmitter1
     // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter0'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5392860,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=20.0000000
     ColorMultiplierRange=(X=(Min=0.8500000,Max=1.0000000),Y=(Min=0.8500000,Max=1.0000000),Z=(Min=0.7310000,Max=0.8590000))
     Opacity=0.6000000
     FadeOutStartTime=0.7800000
     FadeOut=true
     FadeInEndTime=0.3640000
     FadeIn=true
     MaxParticles=15
     RespawnDeadParticles=false
     Name="BlueDust"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
     StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-20.0000000,Max=20.0000000))
     SphereRadiusRange=(Min=8.0000000,Max=8.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=-0.2000000,Max=0.2000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=3.0000000)
     SizeScale[1]=(RelativeTime=0.2400000,RelativeSize=6.0000000)
     SizeScale[2]=(RelativeTime=0.5200000,RelativeSize=8.5000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=10.0000000)
     StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
     UniformSize=true
     InitialParticlesPerSecond=15.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0005'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=6
     SubdivisionEnd=8
     UseRandomSubdivision=true
     LifetimeRange=(Min=1.0000000,Max=1.3000000)
     StartVelocityRange=(X=(Min=-6.0000000,Max=6.0000000),Y=(Min=-6.0000000,Max=6.0000000),Z=(Min=-12.1990004,Max=-5.0479999))
     VelocityLossRange=(X=(Min=0.0000000,Max=1.3680000),Y=(Min=0.0000000,Max=1.3680000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[1]=SpriteEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter2'
     UseDirectionAs=4
     Acceleration=(X=0.0000000,Y=0.0000000,Z=40.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.5000000
     FadeOutStartTime=1.0400000
     FadeOut=true
     FadeInEndTime=0.6890000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Down"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-10.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
     LifetimeRange=(Min=1.3000000,Max=1.3000000)
     InitialDelayRange=(Min=0.2500000,Max=0.2500000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=10.0000000,Max=10.0000000))
     Emitters[2]=SpriteEmitter2
     // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter3'
     UseDirectionAs=4
     Acceleration=(X=0.0000000,Y=0.0000000,Z=-40.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.5000000
     FadeOutStartTime=1.0400000
     FadeOut=true
     FadeInEndTime=0.7150000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Up"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=75.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
     LifetimeRange=(Min=1.3000000,Max=1.3000000)
     InitialDelayRange=(Min=0.2500000,Max=0.2500000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-10.0000000,Max=-10.0000000))
     Emitters[3]=SpriteEmitter3
     // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter4'
     UseDirectionAs=4
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=11,G=23,B=255,A=255))
     Opacity=0.8000000
     FadeOutStartTime=0.1000000
     FadeOut=true
     FadeInEndTime=0.0400000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Final_Circle"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.2800000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.4800000,RelativeSize=1.6000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.8000000)
     StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     InitialDelayRange=(Min=1.3000000,Max=1.3000000)
     Emitters[4]=SpriteEmitter4
     // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter5'
     Acceleration=(X=0.0000000,Y=0.0000000,Z=5.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5142860,Color=(R=201,G=201,B=201,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=10.0000000
     ColorMultiplierRange=(X=(Min=0.8500000,Max=1.0000000),Y=(Min=0.8500000,Max=1.0000000),Z=(Min=0.7310000,Max=0.8590000))
     FadeOutStartTime=0.2000000
     FadeOut=true
     MaxParticles=25
     RespawnDeadParticles=false
     Name="fire_dust"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
     StartLocationShape=2
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0005'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=6
     SubdivisionEnd=9
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.8000000,Max=1.0000000)
     InitialDelayRange=(Min=1.4000000,Max=1.4000000)
     StartVelocityRange=(X=(Min=300.0000000,Max=300.0000000),Y=(Min=300.0000000,Max=300.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=0.0000000,Max=0.0000000))
     GetVelocityDirectionFrom=2
     Emitters[5]=SpriteEmitter5
     // Reference: SpriteEmitter'lineageeffect.dec.wh_balance_life_ta.SpriteEmitter6'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.2000000
     FadeOutStartTime=0.1000000
     FadeOut=true
     MaxParticles=5
     RespawnDeadParticles=false
     Name="Center_b"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.8800000,Max=0.8800000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=0.1400000,RelativeSize=1.7800000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
     StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     InitialDelayRange=(Min=1.4000000,Max=1.4000000)
     Emitters[6]=SpriteEmitter6
     // Reference: MeshEmitter'lineageeffect.dec.wh_balance_life_ta.MeshEmitter4'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.wispray00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.1000000
     FadeOutStartTime=0.2000000
     FadeOut=true
     MaxParticles=5
     RespawnDeadParticles=false
     Name="0"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=30.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=-0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.3000000)
     SizeScale[1]=(RelativeTime=0.1700000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
     StartSizeRange=(X=(Min=0.4000000,Max=0.5400000),Y=(Min=0.4000000,Max=0.5400000),Z=(Min=0.4000000,Max=0.5400000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     InitialDelayRange=(Min=1.4000000,Max=1.4000000)
     Emitters[7]=MeshEmitter4
     AutoDestroy=true
     bLightChanged=true
     bNoDelete=false
     bSunAffect=true
}
