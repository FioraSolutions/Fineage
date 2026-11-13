class el_ice_dagger_fl extends NSkillProjectile;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.el_ice_dagger_fl.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.1200000
         FadeOut=true
         MaxParticles=43
         RespawnDeadParticles=false
         Name="Steam"
         StartLocationOffset=(X=5.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-2.0000000,Max=2.0000000),Z=(Min=-2.0000000,Max=2.0000000))
         StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=5.0000000,Max=5.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.5000000)
         StartSizeRange=(X=(Min=12.0000000,Max=16.0000000),Y=(Min=12.0000000,Max=16.0000000),Z=(Min=12.0000000,Max=16.0000000))
         UniformSize=true
         InitialParticlesPerSecond=20.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0071'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=10
         SubdivisionEnd=13
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=-300.0000000,Max=-300.0000000),Y=(Min=-50.0000000,Max=50.0000000),Z=(Min=-50.0000000,Max=50.0000000))
         WarmupTicksPerSecond=8.0000000
         RelativeWarmupTime=0.5000000
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=MeshEmitter Name=MeshEmitter6
         // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_fl.MeshEmitter6'
         begin object name="MeshEmitter6" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.water.watericedagger00'
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Icedagger"
         SpinParticles=true
         SpinCCWorCW=(X=0.5000000,Y=0.0000000,Z=0.0000000)
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=3.0000000,Max=3.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.5000000,Max=0.5000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.3500000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.3500000)
         StartSizeRange=(X=(Min=0.1200000,Max=0.1200000),Y=(Min=0.1200000,Max=0.1200000),Z=(Min=0.1200000,Max=0.1200000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=0
         LifetimeRange=(Min=2.0000000,Max=2.0000000)
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter6'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter3
         // Reference: SpriteEmitter'lineageeffect.dec.el_ice_dagger_fl.SpriteEmitter3'
         begin object name="SpriteEmitter3" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.6000000,Max=1.0000000))
         FadeOutStartTime=0.9100000
         FadeOut=true
         MaxParticles=40
         RespawnDeadParticles=false
         Name="Kira"
         StartLocationOffset=(X=5.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationRange=(X=(Min=-15.0000000,Max=15.0000000),Y=(Min=-6.0000000,Max=6.0000000),Z=(Min=-6.0000000,Max=6.0000000))
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0300000,Max=0.0300000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.1200000,RelativeSize=3.0000000)
         SizeScale[1]=(RelativeTime=0.5000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.7900000,RelativeSize=0.1000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
         StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
         UniformSize=true
         InitialParticlesPerSecond=20.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0085'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=1
         SubdivisionEnd=2
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         StartVelocityRange=(X=(Min=-200.0000000,Max=-200.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         WarmupTicksPerSecond=8.0000000
         RelativeWarmupTime=0.5000000
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter3'
     Speed=1000.0000000
     AccSpeed=3000.0000000
     // Reference: SpriteEmitter'lineageeffect.dec.el_ice_dagger_fl.SpriteEmitter0'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.1200000
     FadeOut=true
     MaxParticles=43
     RespawnDeadParticles=false
     Name="Steam"
     StartLocationOffset=(X=5.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-2.0000000,Max=2.0000000),Z=(Min=-2.0000000,Max=2.0000000))
     StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=5.0000000,Max=5.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.5000000)
     StartSizeRange=(X=(Min=12.0000000,Max=16.0000000),Y=(Min=12.0000000,Max=16.0000000),Z=(Min=12.0000000,Max=16.0000000))
     UniformSize=true
     InitialParticlesPerSecond=20.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0071'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=10
     SubdivisionEnd=13
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=-300.0000000,Max=-300.0000000),Y=(Min=-50.0000000,Max=50.0000000),Z=(Min=-50.0000000,Max=50.0000000))
     WarmupTicksPerSecond=8.0000000
     RelativeWarmupTime=0.5000000
     Emitters[0]=SpriteEmitter0
     // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_fl.MeshEmitter6'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.water.watericedagger00'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Icedagger"
     SpinParticles=true
     SpinCCWorCW=(X=0.5000000,Y=0.0000000,Z=0.0000000)
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=3.0000000,Max=3.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.5000000,Max=0.5000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.3500000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.3500000)
     StartSizeRange=(X=(Min=0.1200000,Max=0.1200000),Y=(Min=0.1200000,Max=0.1200000),Z=(Min=0.1200000,Max=0.1200000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=0
     LifetimeRange=(Min=2.0000000,Max=2.0000000)
     Emitters[1]=MeshEmitter6
     // Reference: SpriteEmitter'lineageeffect.dec.el_ice_dagger_fl.SpriteEmitter3'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.6000000,Max=1.0000000))
     FadeOutStartTime=0.9100000
     FadeOut=true
     MaxParticles=40
     RespawnDeadParticles=false
     Name="Kira"
     StartLocationOffset=(X=5.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationRange=(X=(Min=-15.0000000,Max=15.0000000),Y=(Min=-6.0000000,Max=6.0000000),Z=(Min=-6.0000000,Max=6.0000000))
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0300000,Max=0.0300000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.1200000,RelativeSize=3.0000000)
     SizeScale[1]=(RelativeTime=0.5000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.7900000,RelativeSize=0.1000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
     StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
     UniformSize=true
     InitialParticlesPerSecond=20.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0085'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionStart=1
     SubdivisionEnd=2
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     StartVelocityRange=(X=(Min=-200.0000000,Max=-200.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     WarmupTicksPerSecond=8.0000000
     RelativeWarmupTime=0.5000000
     Emitters[2]=SpriteEmitter3
     AutoDestroy=true
     RotPerSecond=(Pitch=0,Yaw=0,Roll=30000)
     bUseDynamicLights=false
     bAcceptsProjectors=false
     DrawScale=0.0200000
     bDirectional=true
}
