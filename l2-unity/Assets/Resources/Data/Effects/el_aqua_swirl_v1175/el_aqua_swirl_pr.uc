class el_aqua_swirl_pr extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.el_aqua_swirl_pr.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.4000000
         MaxParticles=25
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="Core"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=0.5000000)
         StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=10.0000000))
         SpinParticles=true
         StartSpinRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.5000000,RelativeSize=1.0500000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=6.0000000,Max=6.0000000),Z=(Min=6.0000000,Max=6.0000000))
         UniformSize=true
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0035'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=true
         SubdivisionStart=4
         SubdivisionEnd=7
         LifetimeRange=(Min=0.2000000,Max=0.2000000)
         InitialDelayRange=(Min=0.7000000,Max=0.7000000)
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.el_aqua_swirl_pr.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.7000000
         MaxParticles=1
         RespawnDeadParticles=false
         Name="INICore"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=0.5000000)
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=18.0000000,Max=18.0000000))
         SpinParticles=true
         StartSpinRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.1000000)
         SizeScale[1]=(RelativeTime=0.4600000,RelativeSize=0.7000000)
         SizeScale[2]=(RelativeTime=0.7800000,RelativeSize=0.9500000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=6.0000000,Max=6.0000000),Z=(Min=6.0000000,Max=6.0000000))
         UniformSize=true
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0035'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=true
         SubdivisionStart=4
         SubdivisionEnd=7
         LifetimeRange=(Min=0.7000000,Max=0.7000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter7
         // Reference: SpriteEmitter'lineageeffect.dec.el_aqua_swirl_pr.SpriteEmitter7'
         begin object name="SpriteEmitter7" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.6000000
         FadeOutStartTime=3.4400001
         FadeOut=true
         FadeInEndTime=0.1600000
         FadeIn=true
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Bubble"
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=4.5000000,Max=4.5000000))
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.3000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=4.0000000,Max=5.0000000),Y=(Min=4.0000000,Max=5.0000000),Z=(Min=4.0000000,Max=5.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0099'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionEnd=3
         UseRandomSubdivision=true
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter7'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter21
         // Reference: SpriteEmitter'lineageeffect.dec.el_aqua_swirl_pr.SpriteEmitter21'
         begin object name="SpriteEmitter21" class=Engine.SpriteEmitter
         UseDirectionAs=3
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.6000000
         FadeOutStartTime=3.4800000
         FadeOut=true
         FadeInEndTime=0.4000000
         FadeIn=true
         MaxParticles=6
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Ring"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.7000000)
         StartSizeRange=(X=(Min=10.0000000,Max=15.0000000),Y=(Min=10.0000000,Max=15.0000000),Z=(Min=10.0000000,Max=15.0000000))
         UniformSize=true
         InitialParticlesPerSecond=6.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=2
         SubdivisionEnd=3
         UseRandomSubdivision=true
         StartVelocityRange=(X=(Min=-0.0100000,Max=0.0100000),Y=(Min=-0.0100000,Max=0.0100000),Z=(Min=-0.0600000,Max=-0.0600000))
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter21'
     // Reference: SpriteEmitter'lineageeffect.dec.el_aqua_swirl_pr.SpriteEmitter2'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.4000000
     MaxParticles=25
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="Core"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=0.5000000)
     StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=10.0000000))
     SpinParticles=true
     StartSpinRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.5000000,RelativeSize=1.0500000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=6.0000000,Max=6.0000000),Z=(Min=6.0000000,Max=6.0000000))
     UniformSize=true
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0035'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     BlendBetweenSubdivisions=true
     SubdivisionStart=4
     SubdivisionEnd=7
     LifetimeRange=(Min=0.2000000,Max=0.2000000)
     InitialDelayRange=(Min=0.7000000,Max=0.7000000)
     Emitters[0]=SpriteEmitter2
     // Reference: SpriteEmitter'lineageeffect.dec.el_aqua_swirl_pr.SpriteEmitter1'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.7000000
     MaxParticles=1
     RespawnDeadParticles=false
     Name="INICore"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=0.5000000)
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=18.0000000,Max=18.0000000))
     SpinParticles=true
     StartSpinRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.1000000)
     SizeScale[1]=(RelativeTime=0.4600000,RelativeSize=0.7000000)
     SizeScale[2]=(RelativeTime=0.7800000,RelativeSize=0.9500000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=6.0000000,Max=6.0000000),Z=(Min=6.0000000,Max=6.0000000))
     UniformSize=true
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0035'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     BlendBetweenSubdivisions=true
     SubdivisionStart=4
     SubdivisionEnd=7
     LifetimeRange=(Min=0.7000000,Max=0.7000000)
     Emitters[1]=SpriteEmitter1
     // Reference: SpriteEmitter'lineageeffect.dec.el_aqua_swirl_pr.SpriteEmitter7'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.6000000
     FadeOutStartTime=3.4400001
     FadeOut=true
     FadeInEndTime=0.1600000
     FadeIn=true
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Bubble"
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=4.5000000,Max=4.5000000))
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.3000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=4.0000000,Max=5.0000000),Y=(Min=4.0000000,Max=5.0000000),Z=(Min=4.0000000,Max=5.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0099'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionEnd=3
     UseRandomSubdivision=true
     Emitters[2]=SpriteEmitter7
     // Reference: SpriteEmitter'lineageeffect.dec.el_aqua_swirl_pr.SpriteEmitter21'
     UseDirectionAs=3
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.6000000
     FadeOutStartTime=3.4800000
     FadeOut=true
     FadeInEndTime=0.4000000
     FadeIn=true
     MaxParticles=6
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Ring"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.7000000)
     StartSizeRange=(X=(Min=10.0000000,Max=15.0000000),Y=(Min=10.0000000,Max=15.0000000),Z=(Min=10.0000000,Max=15.0000000))
     UniformSize=true
     InitialParticlesPerSecond=6.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionStart=2
     SubdivisionEnd=3
     UseRandomSubdivision=true
     StartVelocityRange=(X=(Min=-0.0100000,Max=0.0100000),Y=(Min=-0.0100000,Max=0.0100000),Z=(Min=-0.0600000,Max=-0.0600000))
     Emitters[3]=SpriteEmitter21
     AutoDestroy=true
     bRotEmitter=true
     RotPerSecond=(Pitch=0,Yaw=-50000,Roll=0)
     bNoDelete=false
     DrawScale=0.0200000
     bDirectional=true
}
