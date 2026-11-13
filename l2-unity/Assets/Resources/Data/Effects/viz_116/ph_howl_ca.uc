class ph_howl_ca extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter45
         // Reference: SpriteEmitter'lineageeffect.dec.ph_howl_ca.SpriteEmitter45'
         begin object name="SpriteEmitter45" class=Engine.SpriteEmitter
         Acceleration=(X=0.0000000,Y=0.0000000,Z=50.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4571430,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=11.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7740000,Max=0.7740000),Z=(Min=0.5760000,Max=0.5760000))
         Opacity=0.1100000
         FadeOutStartTime=0.9360000
         FadeOut=true
         FadeInEndTime=0.1430000
         FadeIn=true
         MaxParticles=60
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="paticle"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=5.0000000)
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=60.0000000,Max=60.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=10.0000000,Max=30.0000000),Y=(Min=10.0000000,Max=30.0000000),Z=(Min=10.0000000,Max=30.0000000))
         UniformSize=true
         InitialParticlesPerSecond=40.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         SubdivisionEnd=7
         UseRandomSubdivision=true
         LifetimeRange=(Min=1.0000000,Max=1.3000000)
         StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=20.0000000,Max=20.0000000))
         VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=3.0000000,Max=3.0000000),Z=(Min=0.0000000,Max=0.0000000))
         GetVelocityDirectionFrom=1
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter45'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter46
         // Reference: SpriteEmitter'lineageeffect.dec.ph_howl_ca.SpriteEmitter46'
         begin object name="SpriteEmitter46" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.1100000
         FadeOutStartTime=1.6200000
         FadeOut=true
         FadeInEndTime=0.8200000
         FadeIn=true
         MaxParticles=3
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Center"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=25.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
         LifetimeRange=(Min=2.0000000,Max=2.0000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter46'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter49
         // Reference: SpriteEmitter'lineageeffect.dec.ph_howl_ca.SpriteEmitter49'
         begin object name="SpriteEmitter49" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.5000000
         FadeOutStartTime=0.1200000
         FadeOut=true
         FadeInEndTime=0.0240000
         FadeIn=true
         MaxParticles=17
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="Center2"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=25.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=3.0000000)
         SizeScale[1]=(RelativeTime=0.3900000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0000000)
         StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
         UniformSize=true
         InitialParticlesPerSecond=9.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0064'
         LifetimeRange=(Min=0.2730000,Max=0.2730000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter49'
     // Reference: SpriteEmitter'lineageeffect.dec.ph_howl_ca.SpriteEmitter45'
     Acceleration=(X=0.0000000,Y=0.0000000,Z=50.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4571430,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=11.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7740000,Max=0.7740000),Z=(Min=0.5760000,Max=0.5760000))
     Opacity=0.1100000
     FadeOutStartTime=0.9360000
     FadeOut=true
     FadeInEndTime=0.1430000
     FadeIn=true
     MaxParticles=60
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="paticle"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=5.0000000)
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=60.0000000,Max=60.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=10.0000000,Max=30.0000000),Y=(Min=10.0000000,Max=30.0000000),Z=(Min=10.0000000,Max=30.0000000))
     UniformSize=true
     InitialParticlesPerSecond=40.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     SubdivisionEnd=7
     UseRandomSubdivision=true
     LifetimeRange=(Min=1.0000000,Max=1.3000000)
     StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=20.0000000,Max=20.0000000))
     VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=3.0000000,Max=3.0000000),Z=(Min=0.0000000,Max=0.0000000))
     GetVelocityDirectionFrom=1
     Emitters[0]=SpriteEmitter45
     // Reference: SpriteEmitter'lineageeffect.dec.ph_howl_ca.SpriteEmitter46'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.1100000
     FadeOutStartTime=1.6200000
     FadeOut=true
     FadeInEndTime=0.8200000
     FadeIn=true
     MaxParticles=3
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Center"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=25.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
     LifetimeRange=(Min=2.0000000,Max=2.0000000)
     Emitters[1]=SpriteEmitter46
     // Reference: SpriteEmitter'lineageeffect.dec.ph_howl_ca.SpriteEmitter49'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.5000000
     FadeOutStartTime=0.1200000
     FadeOut=true
     FadeInEndTime=0.0240000
     FadeIn=true
     MaxParticles=17
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="Center2"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=25.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=3.0000000)
     SizeScale[1]=(RelativeTime=0.3900000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0000000)
     StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
     UniformSize=true
     InitialParticlesPerSecond=9.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0064'
     LifetimeRange=(Min=0.2730000,Max=0.2730000)
     Emitters[2]=SpriteEmitter49
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.1000000
     bDirectional=true
}
