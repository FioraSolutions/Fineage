class el_ice_bolt_ta extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter0
         // Reference: MeshEmitter'lineageeffect.dec.el_ice_bolt_ta.MeshEmitter0'
         begin object name="MeshEmitter0" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Black.black_poison00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.6800000,Max=0.6800000),Y=(Min=0.8500000,Max=0.8500000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.1750000
         FadeOut=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="whiteclaw"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0200000),Z=(Min=0.0000000,Max=0.0200000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.0700000,RelativeSize=3.0000000)
         SizeScale[2]=(RelativeTime=0.3700000,RelativeSize=4.5000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=5.8000002)
         StartSizeRange=(X=(Min=0.0399000,Max=0.0399000),Y=(Min=0.0399000,Max=0.0399000),Z=(Min=0.0684000,Max=0.0684000))
         InitialParticlesPerSecond=14.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         InitialDelayRange=(Min=1.0000000,Max=1.0000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter0'
     Begin Object Class=MeshEmitter Name=MeshEmitter3
         // Reference: MeshEmitter'lineageeffect.dec.el_ice_bolt_ta.MeshEmitter3'
         begin object name="MeshEmitter3" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.water.watericebolt00'
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=4.0000000
         MaxParticles=3
         RespawnDeadParticles=false
         Name="iceberg"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.0150000,Max=0.0150000),Y=(Min=-0.0100000,Max=0.0100000),Z=(Min=-0.0100000,Max=0.0100000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.1000000,Max=0.4000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=0.0700000,RelativeSize=1.1000000)
         SizeScale[2]=(RelativeTime=0.0700000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.0605000,Max=0.0990000),Y=(Min=0.0605000,Max=0.0990000),Z=(Min=0.0605000,Max=0.0990000))
         UniformSize=true
         InitialParticlesPerSecond=16.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         LifetimeRange=(Min=2.0000000,Max=2.0000000)
         InitialDelayRange=(Min=1.0000000,Max=1.0000000)
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter3'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.el_ice_bolt_ta.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.2000000
         FadeOutStartTime=1.0000000
         FadeInEndTime=0.1500000
         FadeIn=true
         MaxParticles=12
         RespawnDeadParticles=false
         Name="Steam"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=1.0000000)
         StartLocationRange=(X=(Min=-25.0000000,Max=25.0000000),Y=(Min=-25.0000000,Max=25.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=85.0000000,Max=95.0000000),Z=(Min=16.0000000,Max=16.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0500000,Max=0.1000000),Z=(Min=0.0500000,Max=0.1000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
         StartSizeRange=(X=(Min=12.0000000,Max=12.0000000),Y=(Min=12.0000000,Max=12.0000000),Z=(Min=12.0000000,Max=12.0000000))
         UniformSize=true
         InitialParticlesPerSecond=4.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0035'
         TextureUSubdivisions=8
         TextureVSubdivisions=8
         BlendBetweenSubdivisions=true
         SubdivisionStart=4
         SubdivisionEnd=15
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         InitialDelayRange=(Min=0.2000000,Max=0.2000000)
         StartVelocityRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=-40.0000000,Max=-20.0000000))
         GetVelocityDirectionFrom=1
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter6
         // Reference: MeshEmitter'lineageeffect.dec.el_ice_bolt_ta.MeshEmitter6'
         begin object name="MeshEmitter6" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.water.watericebolt01'
         Acceleration=(X=0.0000000,Y=0.0000000,Z=-350.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.4000000
         FadeOut=true
         CoordinateSystem=0
         RespawnDeadParticles=false
         Name="icefrag"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=4.0000000)
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=30.0000000,Max=150.0000000),Z=(Min=10.0000000,Max=10.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=2.0000000),Z=(Min=1.0000000,Max=2.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         StartSizeRange=(X=(Min=0.0600000,Max=0.1900000),Y=(Min=0.0300000,Max=0.1500000),Z=(Min=0.0300000,Max=0.1500000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         InitialDelayRange=(Min=3.0000000,Max=3.0000000)
         StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=150.0000000,Max=240.0000000))
         VelocityLossRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
         GetVelocityDirectionFrom=1
         end object
     End Object
     Emitters(3)=MeshEmitter'MeshEmitter6'
     Begin Object Class=MeshEmitter Name=MeshEmitter7
         // Reference: MeshEmitter'lineageeffect.dec.el_ice_bolt_ta.MeshEmitter7'
         begin object name="MeshEmitter7" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etcpotion02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.6330000,Max=0.8160000),Y=(Min=0.6120000,Max=0.8380000),Z=(Min=0.6760000,Max=1.0000000))
         Opacity=0.3400000
         FadeOutStartTime=3.2800000
         FadeOut=true
         FadeInEndTime=0.5200000
         FadeIn=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="Wind"
         StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-2.4000001,Max=2.4000001))
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0100000),Z=(Min=0.0000000,Max=0.0100000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=0.1400000,RelativeSize=2.0000000)
         SizeScale[2]=(RelativeTime=0.3400000,RelativeSize=3.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=3.5000000)
         StartSizeRange=(X=(Min=0.0804000,Max=0.0804000),Y=(Min=0.0804000,Max=0.0804000),Z=(Min=-0.0360000,Max=0.0360000))
         InitialParticlesPerSecond=3.0000000
         AutomaticInitialSpawning=false
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-1.2000000,Max=1.2000000))
         WarmupTicksPerSecond=3.0000000
         RelativeWarmupTime=0.3000000
         end object
     End Object
     Emitters(4)=MeshEmitter'MeshEmitter7'
     // Reference: MeshEmitter'lineageeffect.dec.el_ice_bolt_ta.MeshEmitter0'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Black.black_poison00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.6800000,Max=0.6800000),Y=(Min=0.8500000,Max=0.8500000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.1750000
     FadeOut=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="whiteclaw"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0200000),Z=(Min=0.0000000,Max=0.0200000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.0700000,RelativeSize=3.0000000)
     SizeScale[2]=(RelativeTime=0.3700000,RelativeSize=4.5000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=5.8000002)
     StartSizeRange=(X=(Min=0.0399000,Max=0.0399000),Y=(Min=0.0399000,Max=0.0399000),Z=(Min=0.0684000,Max=0.0684000))
     InitialParticlesPerSecond=14.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     InitialDelayRange=(Min=1.0000000,Max=1.0000000)
     Emitters[0]=MeshEmitter0
     // Reference: MeshEmitter'lineageeffect.dec.el_ice_bolt_ta.MeshEmitter3'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.water.watericebolt00'
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=4.0000000
     MaxParticles=3
     RespawnDeadParticles=false
     Name="iceberg"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.0150000,Max=0.0150000),Y=(Min=-0.0100000,Max=0.0100000),Z=(Min=-0.0100000,Max=0.0100000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.1000000,Max=0.4000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=0.0700000,RelativeSize=1.1000000)
     SizeScale[2]=(RelativeTime=0.0700000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.0605000,Max=0.0990000),Y=(Min=0.0605000,Max=0.0990000),Z=(Min=0.0605000,Max=0.0990000))
     UniformSize=true
     InitialParticlesPerSecond=16.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     LifetimeRange=(Min=2.0000000,Max=2.0000000)
     InitialDelayRange=(Min=1.0000000,Max=1.0000000)
     Emitters[1]=MeshEmitter3
     // Reference: SpriteEmitter'lineageeffect.dec.el_ice_bolt_ta.SpriteEmitter1'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.2000000
     FadeOutStartTime=1.0000000
     FadeInEndTime=0.1500000
     FadeIn=true
     MaxParticles=12
     RespawnDeadParticles=false
     Name="Steam"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=1.0000000)
     StartLocationRange=(X=(Min=-25.0000000,Max=25.0000000),Y=(Min=-25.0000000,Max=25.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=85.0000000,Max=95.0000000),Z=(Min=16.0000000,Max=16.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0500000,Max=0.1000000),Z=(Min=0.0500000,Max=0.1000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
     StartSizeRange=(X=(Min=12.0000000,Max=12.0000000),Y=(Min=12.0000000,Max=12.0000000),Z=(Min=12.0000000,Max=12.0000000))
     UniformSize=true
     InitialParticlesPerSecond=4.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0035'
     TextureUSubdivisions=8
     TextureVSubdivisions=8
     BlendBetweenSubdivisions=true
     SubdivisionStart=4
     SubdivisionEnd=15
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     InitialDelayRange=(Min=0.2000000,Max=0.2000000)
     StartVelocityRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=-40.0000000,Max=-20.0000000))
     GetVelocityDirectionFrom=1
     Emitters[2]=SpriteEmitter1
     // Reference: MeshEmitter'lineageeffect.dec.el_ice_bolt_ta.MeshEmitter6'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.water.watericebolt01'
     Acceleration=(X=0.0000000,Y=0.0000000,Z=-350.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.4000000
     FadeOut=true
     CoordinateSystem=0
     RespawnDeadParticles=false
     Name="icefrag"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=4.0000000)
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=30.0000000,Max=150.0000000),Z=(Min=10.0000000,Max=10.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=2.0000000),Z=(Min=1.0000000,Max=2.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     StartSizeRange=(X=(Min=0.0600000,Max=0.1900000),Y=(Min=0.0300000,Max=0.1500000),Z=(Min=0.0300000,Max=0.1500000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     InitialDelayRange=(Min=3.0000000,Max=3.0000000)
     StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=150.0000000,Max=240.0000000))
     VelocityLossRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
     GetVelocityDirectionFrom=1
     Emitters[3]=MeshEmitter6
     // Reference: MeshEmitter'lineageeffect.dec.el_ice_bolt_ta.MeshEmitter7'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etcpotion02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.6330000,Max=0.8160000),Y=(Min=0.6120000,Max=0.8380000),Z=(Min=0.6760000,Max=1.0000000))
     Opacity=0.3400000
     FadeOutStartTime=3.2800000
     FadeOut=true
     FadeInEndTime=0.5200000
     FadeIn=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="Wind"
     StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-2.4000001,Max=2.4000001))
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0100000),Z=(Min=0.0000000,Max=0.0100000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=0.1400000,RelativeSize=2.0000000)
     SizeScale[2]=(RelativeTime=0.3400000,RelativeSize=3.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=3.5000000)
     StartSizeRange=(X=(Min=0.0804000,Max=0.0804000),Y=(Min=0.0804000,Max=0.0804000),Z=(Min=-0.0360000,Max=0.0360000))
     InitialParticlesPerSecond=3.0000000
     AutomaticInitialSpawning=false
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-1.2000000,Max=1.2000000))
     WarmupTicksPerSecond=3.0000000
     RelativeWarmupTime=0.3000000
     Emitters[4]=MeshEmitter7
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.0500000
     bDirectional=true
}
