class m_u025_a extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter14
         // Reference: SpriteEmitter'lineageeffect.dec.m_u025_a.SpriteEmitter14'
         begin object name="SpriteEmitter14" class=Engine.SpriteEmitter
         UseDirectionAs=1
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.5710000,Max=0.5710000),Y=(Min=0.2600000,Max=0.2600000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.8000000
         FadeOutStartTime=2.7600000
         FadeOut=true
         FadeInEndTime=0.3300000
         FadeIn=true
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Flame"
         StartLocationOffset=(X=0.5000000,Y=0.0000000,Z=12.0000000)
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=4.0000000,Max=4.0000000))
         SpinParticles=true
         StartSizeRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=10.0000000,Max=10.0000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         Texture=Texture'FX_E_T.Flameset_a.flame_a00'
         LifetimeRange=(Min=3.5000000,Max=3.5000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.4000000,Max=-0.4000000))
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter14'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter5
         // Reference: SpriteEmitter'lineageeffect.dec.m_u025_a.SpriteEmitter5'
         begin object name="SpriteEmitter5" class=Engine.SpriteEmitter
         Acceleration=(X=0.0000000,Y=0.0000000,Z=100.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.5030000,Max=0.5030000),Z=(Min=0.7200000,Max=0.7200000))
         Opacity=0.5300000
         FadeOutStartTime=0.3500000
         FadeOut=true
         FadeInEndTime=0.1330000
         FadeIn=true
         MaxParticles=12
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="Blue"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-2.0000000)
         UseSizeScale=true
         StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
         UniformSize=true
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=3
         SubdivisionEnd=4
         LifetimeRange=(Min=0.7000000,Max=0.7000000)
         StartVelocityRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=20.0000000,Max=20.0000000))
         VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter5'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter15
         // Reference: SpriteEmitter'lineageeffect.dec.m_u025_a.SpriteEmitter15'
         begin object name="SpriteEmitter15" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.6250000,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=30.0000000
         ColorMultiplierRange=(X=(Min=0.6800000,Max=0.6800000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.6800000,Max=0.6800000))
         Opacity=0.6000000
         FadeOutStartTime=2.4000001
         FadeOut=true
         FadeInEndTime=0.6000000
         FadeIn=true
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Core"
         StartLocationOffset=(X=0.5000000,Y=0.0000000,Z=0.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.5100000,RelativeSize=0.9500000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         SizeScaleRepeats=30.0000000
         StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=5
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0066'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         SubdivisionEnd=1
         LifetimeRange=(Min=3.0000000,Max=3.0000000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter15'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter16
         // Reference: SpriteEmitter'lineageeffect.dec.m_u025_a.SpriteEmitter16'
         begin object name="SpriteEmitter16" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.5030000,Max=0.5030000),Y=(Min=0.5030000,Max=0.5030000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.4000000
         FadeOutStartTime=2.4000001
         FadeOut=true
         FadeInEndTime=0.6000000
         FadeIn=true
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Char"
         StartLocationOffset=(X=0.5000000,Y=0.0000000,Z=-0.5000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=4.0000000,Max=4.0000000),Z=(Min=4.0000000,Max=4.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0080'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         SubdivisionStart=1
         SubdivisionEnd=2
         LifetimeRange=(Min=3.0000000,Max=3.0000000)
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter16'
     // Reference: SpriteEmitter'lineageeffect.dec.m_u025_a.SpriteEmitter14'
     UseDirectionAs=1
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.5710000,Max=0.5710000),Y=(Min=0.2600000,Max=0.2600000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.8000000
     FadeOutStartTime=2.7600000
     FadeOut=true
     FadeInEndTime=0.3300000
     FadeIn=true
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Flame"
     StartLocationOffset=(X=0.5000000,Y=0.0000000,Z=12.0000000)
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=4.0000000,Max=4.0000000))
     SpinParticles=true
     StartSizeRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=10.0000000,Max=10.0000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     Texture=Texture'FX_E_T.Flameset_a.flame_a00'
     LifetimeRange=(Min=3.5000000,Max=3.5000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.4000000,Max=-0.4000000))
     Emitters[0]=SpriteEmitter14
     // Reference: SpriteEmitter'lineageeffect.dec.m_u025_a.SpriteEmitter5'
     Acceleration=(X=0.0000000,Y=0.0000000,Z=100.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.5030000,Max=0.5030000),Z=(Min=0.7200000,Max=0.7200000))
     Opacity=0.5300000
     FadeOutStartTime=0.3500000
     FadeOut=true
     FadeInEndTime=0.1330000
     FadeIn=true
     MaxParticles=12
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="Blue"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-2.0000000)
     UseSizeScale=true
     StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
     UniformSize=true
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=3
     SubdivisionEnd=4
     LifetimeRange=(Min=0.7000000,Max=0.7000000)
     StartVelocityRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=20.0000000,Max=20.0000000))
     VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
     Emitters[1]=SpriteEmitter5
     // Reference: SpriteEmitter'lineageeffect.dec.m_u025_a.SpriteEmitter15'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.6250000,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=30.0000000
     ColorMultiplierRange=(X=(Min=0.6800000,Max=0.6800000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.6800000,Max=0.6800000))
     Opacity=0.6000000
     FadeOutStartTime=2.4000001
     FadeOut=true
     FadeInEndTime=0.6000000
     FadeIn=true
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Core"
     StartLocationOffset=(X=0.5000000,Y=0.0000000,Z=0.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.5100000,RelativeSize=0.9500000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     SizeScaleRepeats=30.0000000
     StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=5
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0066'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     SubdivisionEnd=1
     LifetimeRange=(Min=3.0000000,Max=3.0000000)
     Emitters[2]=SpriteEmitter15
     // Reference: SpriteEmitter'lineageeffect.dec.m_u025_a.SpriteEmitter16'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.5030000,Max=0.5030000),Y=(Min=0.5030000,Max=0.5030000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.4000000
     FadeOutStartTime=2.4000001
     FadeOut=true
     FadeInEndTime=0.6000000
     FadeIn=true
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Char"
     StartLocationOffset=(X=0.5000000,Y=0.0000000,Z=-0.5000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=4.0000000,Max=4.0000000),Z=(Min=4.0000000,Max=4.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0080'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     SubdivisionStart=1
     SubdivisionEnd=2
     LifetimeRange=(Min=3.0000000,Max=3.0000000)
     Emitters[3]=SpriteEmitter16
     AutoDestroy=true
     AutoReplay=true
     bRotEmitter=true
     RotPerSecond=(Pitch=0,Yaw=100000,Roll=0)
     Physics=10
     bUseDynamicLights=false
     bNoDelete=false
     bTrailerPrePivot=true
     bRelativeTrail=true
     bSelfRotation=true
     bAcceptsProjectors=false
     DrawScale=0.1000000
}
