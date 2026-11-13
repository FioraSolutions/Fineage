class el_twister_fl extends NSkillProjectile;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter3
         // Reference: MeshEmitter'lineageeffect.dec.el_twister_fl.MeshEmitter3'
         begin object name="MeshEmitter3" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windblowin02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
         FadeOutStartTime=6.0000000
         FadeInFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
         MaxParticles=1
         ResetAfterChange=true
         RespawnDeadParticles=false
         Name="Wind"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=0.1020000,Max=0.1020000),Y=(Min=0.1020000,Max=0.1020000),Z=(Min=0.1020000,Max=0.1020000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0001'
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter3'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter15
         // Reference: SpriteEmitter'lineageeffect.dec.el_twister_fl.SpriteEmitter15'
         begin object name="SpriteEmitter15" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.7000000,Max=0.7000000),Y=(Min=0.7000000,Max=0.7000000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.1800000
         FadeOutStartTime=5.0000000
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Aura"
         StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.4250000,Max=0.4250000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=4.0000000)
         SizeScale[2]=(RelativeTime=0.6600000,RelativeSize=6.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=7.0000000)
         StartSizeRange=(X=(Min=12.7500000,Max=21.2500000),Y=(Min=12.7500000,Max=21.2500000),Z=(Min=12.7500000,Max=21.2500000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=12
         SubdivisionEnd=13
         LifetimeRange=(Min=3.8000000,Max=3.8000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter15'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter16
         // Reference: SpriteEmitter'lineageeffect.dec.el_twister_fl.SpriteEmitter16'
         begin object name="SpriteEmitter16" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.1714290,Color=(R=255,G=255,B=255,A=255))
         ColorScale[2]=(RelativeTime=0.3321430,Color=(R=128,G=128,B=128,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorMultiplierRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.8000000
         FadeOutStartTime=5.0000000
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Core"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=10.2000008,Max=10.2000008),Y=(Min=68.0000000,Max=68.0000000),Z=(Min=68.0000000,Max=68.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=5
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionEnd=1
         UseRandomSubdivision=true
         MaxAbsVelocity=(X=6400.0000000,Y=10000.0000000,Z=8000.0000000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter16'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter17
         // Reference: SpriteEmitter'lineageeffect.dec.el_twister_fl.SpriteEmitter17'
         begin object name="SpriteEmitter17" class=Engine.SpriteEmitter
         Acceleration=(X=-85.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=94,G=145,B=251,A=0))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=0))
         ColorScaleRepeats=5.0000000
         ColorMultiplierRange=(X=(Min=0.3070000,Max=0.3070000),Y=(Min=0.6100000,Max=0.6100000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.6300000
         FadeOutStartTime=0.1350000
         FadeOut=true
         MaxParticles=100
         RespawnDeadParticles=false
         Name="Smog"
         StartLocationOffset=(X=-10.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationShape=1
         SphereRadiusRange=(Min=5.0000000,Max=5.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=17.0000000,Max=17.0000000),Y=(Min=68.0000000,Max=68.0000000),Z=(Min=68.0000000,Max=68.0000000))
         UniformSize=true
         InitialParticlesPerSecond=30.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0067'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         SubdivisionStart=2
         SubdivisionEnd=4
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.2000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=-255.0000000,Max=-255.0000000),Y=(Min=-8.5000000,Max=8.5000000),Z=(Min=-8.5000000,Max=8.5000000))
         MaxAbsVelocity=(X=6400.0000000,Y=10000.0000000,Z=8000.0000000)
         WarmupTicksPerSecond=10.0000000
         RelativeWarmupTime=0.1000000
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter17'
     Speed=1000.0000000
     AccSpeed=3000.0000000
     // Reference: MeshEmitter'lineageeffect.dec.el_twister_fl.MeshEmitter3'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windblowin02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
     FadeOutStartTime=6.0000000
     FadeInFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
     MaxParticles=1
     ResetAfterChange=true
     RespawnDeadParticles=false
     Name="Wind"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=0.1020000,Max=0.1020000),Y=(Min=0.1020000,Max=0.1020000),Z=(Min=0.1020000,Max=0.1020000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0001'
     Emitters[0]=MeshEmitter3
     // Reference: SpriteEmitter'lineageeffect.dec.el_twister_fl.SpriteEmitter15'
     UseDirectionAs=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.7000000,Max=0.7000000),Y=(Min=0.7000000,Max=0.7000000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.1800000
     FadeOutStartTime=5.0000000
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Aura"
     StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.4250000,Max=0.4250000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=4.0000000)
     SizeScale[2]=(RelativeTime=0.6600000,RelativeSize=6.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=7.0000000)
     StartSizeRange=(X=(Min=12.7500000,Max=21.2500000),Y=(Min=12.7500000,Max=21.2500000),Z=(Min=12.7500000,Max=21.2500000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=12
     SubdivisionEnd=13
     LifetimeRange=(Min=3.8000000,Max=3.8000000)
     Emitters[1]=SpriteEmitter15
     // Reference: SpriteEmitter'lineageeffect.dec.el_twister_fl.SpriteEmitter16'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.1714290,Color=(R=255,G=255,B=255,A=255))
     ColorScale[2]=(RelativeTime=0.3321430,Color=(R=128,G=128,B=128,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorMultiplierRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.8000000
     FadeOutStartTime=5.0000000
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Core"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=10.2000008,Max=10.2000008),Y=(Min=68.0000000,Max=68.0000000),Z=(Min=68.0000000,Max=68.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=5
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionEnd=1
     UseRandomSubdivision=true
     MaxAbsVelocity=(X=6400.0000000,Y=10000.0000000,Z=8000.0000000)
     Emitters[2]=SpriteEmitter16
     // Reference: SpriteEmitter'lineageeffect.dec.el_twister_fl.SpriteEmitter17'
     Acceleration=(X=-85.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=94,G=145,B=251,A=0))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=0))
     ColorScaleRepeats=5.0000000
     ColorMultiplierRange=(X=(Min=0.3070000,Max=0.3070000),Y=(Min=0.6100000,Max=0.6100000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.6300000
     FadeOutStartTime=0.1350000
     FadeOut=true
     MaxParticles=100
     RespawnDeadParticles=false
     Name="Smog"
     StartLocationOffset=(X=-10.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationShape=1
     SphereRadiusRange=(Min=5.0000000,Max=5.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=17.0000000,Max=17.0000000),Y=(Min=68.0000000,Max=68.0000000),Z=(Min=68.0000000,Max=68.0000000))
     UniformSize=true
     InitialParticlesPerSecond=30.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0067'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     SubdivisionStart=2
     SubdivisionEnd=4
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.2000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=-255.0000000,Max=-255.0000000),Y=(Min=-8.5000000,Max=8.5000000),Z=(Min=-8.5000000,Max=8.5000000))
     MaxAbsVelocity=(X=6400.0000000,Y=10000.0000000,Z=8000.0000000)
     WarmupTicksPerSecond=10.0000000
     RelativeWarmupTime=0.1000000
     Emitters[3]=SpriteEmitter17
     AutoDestroy=true
     bRotEmitter=true
     RotPerSecond=(Pitch=0,Yaw=0,Roll=30000)
     bUseDynamicLights=false
     bSelfRotation=true
     bAcceptsProjectors=false
     DrawScale=0.1000000
     bDirectional=true
}
