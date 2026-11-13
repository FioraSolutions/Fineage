class s_u806_cs extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter10
         // Reference: MeshEmitter'lineageeffect.dec.s_u806_cs.MeshEmitter10'
         begin object name="MeshEmitter10" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.swirl_ring_05'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7920000,Max=0.7920000),Z=(Min=0.6000000,Max=0.6000000))
         FadeOutStartTime=0.2450000
         FadeOut=true
         FadeInEndTime=0.1700000
         FadeIn=true
         MaxParticles=67
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="spread"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
         StartSizeRange=(X=(Min=0.3000000,Max=0.3000000),Y=(Min=0.3000000,Max=0.3000000),Z=(Min=0.1500000,Max=0.1500000))
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         InitialDelayRange=(Min=2.6500001,Max=2.6500001)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter10'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter20
         // Reference: SpriteEmitter'lineageeffect.dec.s_u806_cs.SpriteEmitter20'
         begin object name="SpriteEmitter20" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5142860,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=5.0000000
         Opacity=0.5000000
         FadeOutStartTime=0.3000000
         FadeOut=true
         FadeInEndTime=0.1000000
         FadeIn=true
         MaxParticles=52
         WeatherSoundCheck=true
         RespawnDeadParticles=false
         Name="Circle"
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
         UniformSize=true
         InitialParticlesPerSecond=4.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1026'
         TextureUSubdivisions=1
         TextureVSubdivisions=1
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         InitialDelayRange=(Min=2.6500001,Max=2.6500001)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=50.0000000,Max=50.0000000))
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter20'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter21
         // Reference: SpriteEmitter'lineageeffect.dec.s_u806_cs.SpriteEmitter21'
         begin object name="SpriteEmitter21" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5142860,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=5.0000000
         ColorMultiplierRange=(X=(Min=0.7400000,Max=0.7400000),Y=(Min=0.7390000,Max=0.7390000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.5400000
         FadeOutStartTime=0.3000000
         FadeOut=true
         FadeInEndTime=0.1000000
         FadeIn=true
         MaxParticles=52
         RespawnDeadParticles=false
         Name="circle2"
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.3000000)
         StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
         UniformSize=true
         InitialParticlesPerSecond=4.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t_3041'
         TextureUSubdivisions=1
         TextureVSubdivisions=1
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         InitialDelayRange=(Min=2.6500001,Max=2.6500001)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=50.0000000,Max=50.0000000))
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter21'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter4
         // Reference: SpriteEmitter'lineageeffect.dec.s_u806_cs.SpriteEmitter4'
         begin object name="SpriteEmitter4" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         MaxParticles=1
         WeatherSoundCheck=true
         RespawnDeadParticles=false
         Name="SpriteEmitter4"
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=none
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter4'
     // Reference: MeshEmitter'lineageeffect.dec.s_u806_cs.MeshEmitter10'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.swirl_ring_05'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7920000,Max=0.7920000),Z=(Min=0.6000000,Max=0.6000000))
     FadeOutStartTime=0.2450000
     FadeOut=true
     FadeInEndTime=0.1700000
     FadeIn=true
     MaxParticles=67
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="spread"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
     StartSizeRange=(X=(Min=0.3000000,Max=0.3000000),Y=(Min=0.3000000,Max=0.3000000),Z=(Min=0.1500000,Max=0.1500000))
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     InitialDelayRange=(Min=2.6500001,Max=2.6500001)
     Emitters[0]=MeshEmitter10
     // Reference: SpriteEmitter'lineageeffect.dec.s_u806_cs.SpriteEmitter20'
     UseDirectionAs=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5142860,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=5.0000000
     Opacity=0.5000000
     FadeOutStartTime=0.3000000
     FadeOut=true
     FadeInEndTime=0.1000000
     FadeIn=true
     MaxParticles=52
     WeatherSoundCheck=true
     RespawnDeadParticles=false
     Name="Circle"
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
     UniformSize=true
     InitialParticlesPerSecond=4.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1026'
     TextureUSubdivisions=1
     TextureVSubdivisions=1
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     InitialDelayRange=(Min=2.6500001,Max=2.6500001)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=50.0000000,Max=50.0000000))
     Emitters[1]=SpriteEmitter20
     // Reference: SpriteEmitter'lineageeffect.dec.s_u806_cs.SpriteEmitter21'
     UseDirectionAs=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5142860,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=5.0000000
     ColorMultiplierRange=(X=(Min=0.7400000,Max=0.7400000),Y=(Min=0.7390000,Max=0.7390000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.5400000
     FadeOutStartTime=0.3000000
     FadeOut=true
     FadeInEndTime=0.1000000
     FadeIn=true
     MaxParticles=52
     RespawnDeadParticles=false
     Name="circle2"
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.3000000)
     StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
     UniformSize=true
     InitialParticlesPerSecond=4.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t_3041'
     TextureUSubdivisions=1
     TextureVSubdivisions=1
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     InitialDelayRange=(Min=2.6500001,Max=2.6500001)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=50.0000000,Max=50.0000000))
     Emitters[2]=SpriteEmitter21
     // Reference: SpriteEmitter'lineageeffect.dec.s_u806_cs.SpriteEmitter4'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     MaxParticles=1
     WeatherSoundCheck=true
     RespawnDeadParticles=false
     Name="SpriteEmitter4"
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=none
     Emitters[3]=SpriteEmitter4
     AutoDestroy=true
     bLightChanged=true
     bNoDelete=false
     bSunAffect=true
     DrawScale=0.1000000
}
