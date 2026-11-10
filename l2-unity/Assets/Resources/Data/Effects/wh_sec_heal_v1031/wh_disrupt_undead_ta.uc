class wh_disrupt_undead_ta extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter12
         // Reference: SpriteEmitter'lineageeffect.dec.wh_disrupt_undead_ta.SpriteEmitter12'
         begin object name="SpriteEmitter12" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.4810000,Max=0.4810000),Y=(Min=0.5050000,Max=0.5050000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.6000000
         FadeOutStartTime=0.4130000
         FadeOut=true
         FadeInEndTime=0.0210000
         FadeIn=true
         MaxParticles=5
         RespawnDeadParticles=false
         Name="Rings"
         SpinParticles=true
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0500000,RelativeSize=2.5000000)
         SizeScale[1]=(RelativeTime=0.1900000,RelativeSize=3.5000000)
         SizeScale[2]=(RelativeTime=0.3400000,RelativeSize=4.5000000)
         SizeScale[3]=(RelativeTime=0.7500000,RelativeSize=5.5000000)
         SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=6.0000000)
         StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
         InitialParticlesPerSecond=6.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=4
         SubdivisionEnd=5
         LifetimeRange=(Min=0.7000000,Max=0.7000000)
         InitialDelayRange=(Min=1.6000000,Max=1.6000000)
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter12'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.wh_disrupt_undead_ta.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5714290,Color=(R=113,G=113,B=113,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=25.0000000
         ColorMultiplierRange=(X=(Min=0.8500000,Max=1.0000000),Y=(Min=0.8500000,Max=1.0000000),Z=(Min=0.8500000,Max=1.0000000))
         FadeOutStartTime=1.8200001
         FadeOut=true
         MaxParticles=7
         RespawnDeadParticles=false
         Name="Detornator"
         StartLocationShape=2
         SphereRadiusRange=(Min=16.0000000,Max=16.0000000)
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=30.0000000,Max=30.0000000))
         SpinsPerSecondRange=(X=(Min=-0.2000000,Max=0.2000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.6600000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.7300000,RelativeSize=30.0000000)
         SizeScale[2]=(RelativeTime=0.8600000,RelativeSize=25.0000000)
         SizeScale[3]=(RelativeTime=0.9600000,RelativeSize=15.0000000)
         SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.6000000,Max=0.9000000),Y=(Min=0.6000000,Max=0.9000000),Z=(Min=0.6000000,Max=0.9000000))
         UniformSize=true
         InitialParticlesPerSecond=12.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionEnd=1
         UseRandomSubdivision=true
         LifetimeRange=(Min=2.5000000,Max=2.5000000)
         StartVelocityRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
         VelocityLossRange=(X=(Min=1.5000000,Max=2.5000000),Y=(Min=1.5000000,Max=2.5000000),Z=(Min=1.5000000,Max=2.5000000))
         GetVelocityDirectionFrom=1
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter1'
     Begin Object Class=BeamEmitter Name=BeamEmitter0
         // Reference: BeamEmitter'lineageeffect.dec.wh_disrupt_undead_ta.BeamEmitter0'
         begin object name="BeamEmitter0" class=Engine.BeamEmitter
         BeamEndPoints=/* Array type was not detected. */
         DetermineEndPointBy=2
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.6250000,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=10.0000000
         ColorMultiplierRange=(X=(Min=0.7000000,Max=1.0000000),Y=(Min=0.7000000,Max=1.0000000),Z=(Min=0.9000000,Max=1.0000000))
         Opacity=0.1400000
         FadeOutStartTime=0.5200000
         FadeOut=true
         FadeInEndTime=0.0600000
         FadeIn=true
         MaxParticles=12
         RespawnDeadParticles=false
         Name="Beam"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=100.0000000)
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=8.0000000,Max=8.0000000))
         StartSizeRange=(X=(Min=15.0000000,Max=15.0000000),Y=(Min=15.0000000,Max=15.0000000),Z=(Min=15.0000000,Max=15.0000000))
         InitialParticlesPerSecond=60.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0004'
         LifetimeRange=(Min=0.6000000,Max=1.2000000)
         InitialDelayRange=(Min=1.7000000,Max=1.7000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-100.0000000,Max=-100.0000000))
         end object
     End Object
     Emitters(2)=BeamEmitter'BeamEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.wh_disrupt_undead_ta.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.6964290,Color=(R=77,G=77,B=77,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=10.0000000
         FadeOutStartTime=0.6000000
         FadeOut=true
         MaxParticles=6
         RespawnDeadParticles=false
         Name="Particle"
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=15.0000000,Max=15.0000000))
         SpinParticles=true
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
         StartSizeRange=(X=(Min=7.0000000,Max=7.0000000),Y=(Min=7.0000000,Max=7.0000000),Z=(Min=7.0000000,Max=7.0000000))
         UniformSize=true
         InitialParticlesPerSecond=100.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=1
         SubdivisionEnd=3
         UseRandomSubdivision=true
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         InitialDelayRange=(Min=1.7000000,Max=1.7000000)
         StartVelocityRange=(X=(Min=50.0000000,Max=50.0000000),Y=(Min=50.0000000,Max=50.0000000),Z=(Min=50.0000000,Max=50.0000000))
         VelocityLossRange=(X=(Min=2.0000000,Max=3.0000000),Y=(Min=2.0000000,Max=3.0000000),Z=(Min=2.0000000,Max=3.0000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter2'
     // Reference: SpriteEmitter'lineageeffect.dec.wh_disrupt_undead_ta.SpriteEmitter12'
     UseDirectionAs=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.4810000,Max=0.4810000),Y=(Min=0.5050000,Max=0.5050000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.6000000
     FadeOutStartTime=0.4130000
     FadeOut=true
     FadeInEndTime=0.0210000
     FadeIn=true
     MaxParticles=5
     RespawnDeadParticles=false
     Name="Rings"
     SpinParticles=true
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0500000,RelativeSize=2.5000000)
     SizeScale[1]=(RelativeTime=0.1900000,RelativeSize=3.5000000)
     SizeScale[2]=(RelativeTime=0.3400000,RelativeSize=4.5000000)
     SizeScale[3]=(RelativeTime=0.7500000,RelativeSize=5.5000000)
     SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=6.0000000)
     StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
     InitialParticlesPerSecond=6.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=4
     SubdivisionEnd=5
     LifetimeRange=(Min=0.7000000,Max=0.7000000)
     InitialDelayRange=(Min=1.6000000,Max=1.6000000)
     Emitters[0]=SpriteEmitter12
     // Reference: SpriteEmitter'lineageeffect.dec.wh_disrupt_undead_ta.SpriteEmitter1'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5714290,Color=(R=113,G=113,B=113,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=25.0000000
     ColorMultiplierRange=(X=(Min=0.8500000,Max=1.0000000),Y=(Min=0.8500000,Max=1.0000000),Z=(Min=0.8500000,Max=1.0000000))
     FadeOutStartTime=1.8200001
     FadeOut=true
     MaxParticles=7
     RespawnDeadParticles=false
     Name="Detornator"
     StartLocationShape=2
     SphereRadiusRange=(Min=16.0000000,Max=16.0000000)
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=30.0000000,Max=30.0000000))
     SpinsPerSecondRange=(X=(Min=-0.2000000,Max=0.2000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.6600000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.7300000,RelativeSize=30.0000000)
     SizeScale[2]=(RelativeTime=0.8600000,RelativeSize=25.0000000)
     SizeScale[3]=(RelativeTime=0.9600000,RelativeSize=15.0000000)
     SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.6000000,Max=0.9000000),Y=(Min=0.6000000,Max=0.9000000),Z=(Min=0.6000000,Max=0.9000000))
     UniformSize=true
     InitialParticlesPerSecond=12.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionEnd=1
     UseRandomSubdivision=true
     LifetimeRange=(Min=2.5000000,Max=2.5000000)
     StartVelocityRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
     VelocityLossRange=(X=(Min=1.5000000,Max=2.5000000),Y=(Min=1.5000000,Max=2.5000000),Z=(Min=1.5000000,Max=2.5000000))
     GetVelocityDirectionFrom=1
     Emitters[1]=SpriteEmitter1
     // Reference: BeamEmitter'lineageeffect.dec.wh_disrupt_undead_ta.BeamEmitter0'
     BeamEndPoints=/* Array type was not detected. */
     DetermineEndPointBy=2
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.6250000,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=10.0000000
     ColorMultiplierRange=(X=(Min=0.7000000,Max=1.0000000),Y=(Min=0.7000000,Max=1.0000000),Z=(Min=0.9000000,Max=1.0000000))
     Opacity=0.1400000
     FadeOutStartTime=0.5200000
     FadeOut=true
     FadeInEndTime=0.0600000
     FadeIn=true
     MaxParticles=12
     RespawnDeadParticles=false
     Name="Beam"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=100.0000000)
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=8.0000000,Max=8.0000000))
     StartSizeRange=(X=(Min=15.0000000,Max=15.0000000),Y=(Min=15.0000000,Max=15.0000000),Z=(Min=15.0000000,Max=15.0000000))
     InitialParticlesPerSecond=60.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0004'
     LifetimeRange=(Min=0.6000000,Max=1.2000000)
     InitialDelayRange=(Min=1.7000000,Max=1.7000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-100.0000000,Max=-100.0000000))
     Emitters[2]=BeamEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.wh_disrupt_undead_ta.SpriteEmitter2'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.6964290,Color=(R=77,G=77,B=77,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=10.0000000
     FadeOutStartTime=0.6000000
     FadeOut=true
     MaxParticles=6
     RespawnDeadParticles=false
     Name="Particle"
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=15.0000000,Max=15.0000000))
     SpinParticles=true
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
     StartSizeRange=(X=(Min=7.0000000,Max=7.0000000),Y=(Min=7.0000000,Max=7.0000000),Z=(Min=7.0000000,Max=7.0000000))
     UniformSize=true
     InitialParticlesPerSecond=100.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=1
     SubdivisionEnd=3
     UseRandomSubdivision=true
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     InitialDelayRange=(Min=1.7000000,Max=1.7000000)
     StartVelocityRange=(X=(Min=50.0000000,Max=50.0000000),Y=(Min=50.0000000,Max=50.0000000),Z=(Min=50.0000000,Max=50.0000000))
     VelocityLossRange=(X=(Min=2.0000000,Max=3.0000000),Y=(Min=2.0000000,Max=3.0000000),Z=(Min=2.0000000,Max=3.0000000))
     GetVelocityDirectionFrom=2
     Emitters[3]=SpriteEmitter2
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.2500000
     bDirectional=true
}
