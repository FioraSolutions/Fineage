class e_u031_a extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter0
         // Reference: MeshEmitter'lineageeffect.dec.e_u031_a.MeshEmitter0'
         begin object name="MeshEmitter0" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.magiccircle.magiccircleblack02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0010000,Max=0.0010000),Z=(Min=0.0010000,Max=0.0010000))
         FadeOutStartTime=27.7199993
         FadeOut=true
         FadeInEndTime=1.6799999
         FadeIn=true
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="DarkMatter"
         StartSizeRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         LifetimeRange=(Min=28.0000000,Max=28.0000000)
         InitialDelayRange=(Min=2.0000000,Max=2.0000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter0'
     Begin Object Class=MeshEmitter Name=MeshEmitter1
         // Reference: MeshEmitter'lineageeffect.dec.e_u031_a.MeshEmitter1'
         begin object name="MeshEmitter1" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_spawn01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.7250000,Color=(R=183,G=183,B=183,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=300.0000000
         ColorMultiplierRange=(X=(Min=0.4000000,Max=0.4000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=0.9000000,Max=0.9000000))
         FadeOutStartTime=30.0000000
         FadeInEndTime=0.6000000
         FadeIn=true
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="MC"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=7.0000000)
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=10.0000000,Max=10.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.8000000)
         SizeScale[1]=(RelativeTime=0.5000000,RelativeSize=1.3000000)
         SizeScale[2]=(RelativeTime=0.9500000,RelativeSize=1.3000000)
         SizeScale[3]=(RelativeTime=0.9800000,RelativeSize=1.0000000)
         SizeScale[4]=(RelativeTime=0.9900000,RelativeSize=0.7000000)
         SizeScale[5]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
         StartSizeRange=(X=(Min=0.6000000,Max=0.6000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=0.6000000,Max=0.6000000))
         UniformSize=true
         InitialParticlesPerSecond=100000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=30.0000000,Max=30.0000000)
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter2
         // Reference: MeshEmitter'lineageeffect.dec.e_u031_a.MeshEmitter2'
         begin object name="MeshEmitter2" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_spawn00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.6857140,Color=(R=167,G=167,B=167,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=300.0000000
         ColorMultiplierRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.2500000,Max=0.2500000))
         Opacity=0.3900000
         FadeOutStartTime=28.0000000
         FadeInEndTime=1.6799999
         FadeIn=true
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Column"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-6.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.0500000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.9200000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
         StartSizeRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.6000000,Max=0.6000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=28.0000000,Max=28.0000000)
         InitialDelayRange=(Min=2.0000000,Max=2.0000000)
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.e_u031_a.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         UseDirectionAs=1
         Acceleration=(X=0.0000000,Y=0.0000000,Z=200.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.5000000,Max=0.7000000),Y=(Min=0.5000000,Max=0.7000000),Z=(Min=0.8000000,Max=1.0000000))
         FadeOutStartTime=0.3000000
         FadeOut=true
         MaxParticles=250
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="UpLine"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-20.0000000)
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=20.0000000,Max=20.0000000))
         SpinParticles=true
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.5100000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.3000000)
         StartSizeRange=(X=(Min=1.0000000,Max=3.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
         InitialParticlesPerSecond=10.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0059'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=2
         SubdivisionEnd=3
         LifetimeRange=(Min=1.2000000,Max=1.2000000)
         InitialDelayRange=(Min=3.0000000,Max=3.0000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=30.0000000))
         VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1.0000000,Max=1.0000000))
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter2'
     Begin Object Class=MeshEmitter Name=MeshEmitter3
         // Reference: MeshEmitter'lineageeffect.dec.e_u031_a.MeshEmitter3'
         begin object name="MeshEmitter3" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.castleob03'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=0.0000000,Y=0.0000000,Z=300.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.7000000,Max=0.7000000),Y=(Min=0.7000000,Max=0.7000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.3000000
         FadeOut=true
         MaxParticles=12
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="CylUp"
         StartSizeRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.2000000,Max=0.2000000),Z=(Min=0.2000000,Max=0.2000000))
         UniformSize=true
         InitialParticlesPerSecond=0.5000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.6000000,Max=0.6000000)
         InitialDelayRange=(Min=5.0000000,Max=5.0000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=50.0000000,Max=50.0000000))
         end object
     End Object
     Emitters(4)=MeshEmitter'MeshEmitter3'
     // Reference: MeshEmitter'lineageeffect.dec.e_u031_a.MeshEmitter0'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.magiccircle.magiccircleblack02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0010000,Max=0.0010000),Z=(Min=0.0010000,Max=0.0010000))
     FadeOutStartTime=27.7199993
     FadeOut=true
     FadeInEndTime=1.6799999
     FadeIn=true
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="DarkMatter"
     StartSizeRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     LifetimeRange=(Min=28.0000000,Max=28.0000000)
     InitialDelayRange=(Min=2.0000000,Max=2.0000000)
     Emitters[0]=MeshEmitter0
     // Reference: MeshEmitter'lineageeffect.dec.e_u031_a.MeshEmitter1'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_spawn01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.7250000,Color=(R=183,G=183,B=183,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=300.0000000
     ColorMultiplierRange=(X=(Min=0.4000000,Max=0.4000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=0.9000000,Max=0.9000000))
     FadeOutStartTime=30.0000000
     FadeInEndTime=0.6000000
     FadeIn=true
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="MC"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=7.0000000)
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=10.0000000,Max=10.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.8000000)
     SizeScale[1]=(RelativeTime=0.5000000,RelativeSize=1.3000000)
     SizeScale[2]=(RelativeTime=0.9500000,RelativeSize=1.3000000)
     SizeScale[3]=(RelativeTime=0.9800000,RelativeSize=1.0000000)
     SizeScale[4]=(RelativeTime=0.9900000,RelativeSize=0.7000000)
     SizeScale[5]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
     StartSizeRange=(X=(Min=0.6000000,Max=0.6000000),Y=(Min=0.6000000,Max=0.6000000),Z=(Min=0.6000000,Max=0.6000000))
     UniformSize=true
     InitialParticlesPerSecond=100000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=30.0000000,Max=30.0000000)
     Emitters[1]=MeshEmitter1
     // Reference: MeshEmitter'lineageeffect.dec.e_u031_a.MeshEmitter2'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_spawn00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.6857140,Color=(R=167,G=167,B=167,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=300.0000000
     ColorMultiplierRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.2500000,Max=0.2500000))
     Opacity=0.3900000
     FadeOutStartTime=28.0000000
     FadeInEndTime=1.6799999
     FadeIn=true
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Column"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-6.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.0500000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.9200000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
     StartSizeRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.6000000,Max=0.6000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=28.0000000,Max=28.0000000)
     InitialDelayRange=(Min=2.0000000,Max=2.0000000)
     Emitters[2]=MeshEmitter2
     // Reference: SpriteEmitter'lineageeffect.dec.e_u031_a.SpriteEmitter2'
     UseDirectionAs=1
     Acceleration=(X=0.0000000,Y=0.0000000,Z=200.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.5000000,Max=0.7000000),Y=(Min=0.5000000,Max=0.7000000),Z=(Min=0.8000000,Max=1.0000000))
     FadeOutStartTime=0.3000000
     FadeOut=true
     MaxParticles=250
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="UpLine"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-20.0000000)
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=20.0000000,Max=20.0000000))
     SpinParticles=true
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.5100000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.3000000)
     StartSizeRange=(X=(Min=1.0000000,Max=3.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
     InitialParticlesPerSecond=10.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0059'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionStart=2
     SubdivisionEnd=3
     LifetimeRange=(Min=1.2000000,Max=1.2000000)
     InitialDelayRange=(Min=3.0000000,Max=3.0000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=30.0000000))
     VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1.0000000,Max=1.0000000))
     Emitters[3]=SpriteEmitter2
     // Reference: MeshEmitter'lineageeffect.dec.e_u031_a.MeshEmitter3'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.castleob03'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=0.0000000,Y=0.0000000,Z=300.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.7000000,Max=0.7000000),Y=(Min=0.7000000,Max=0.7000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.3000000
     FadeOut=true
     MaxParticles=12
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="CylUp"
     StartSizeRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.2000000,Max=0.2000000),Z=(Min=0.2000000,Max=0.2000000))
     UniformSize=true
     InitialParticlesPerSecond=0.5000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.6000000,Max=0.6000000)
     InitialDelayRange=(Min=5.0000000,Max=5.0000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=50.0000000,Max=50.0000000))
     Emitters[4]=MeshEmitter3
     AutoDestroy=true
     AutoReplay=true
     bRotEmitter=true
     RotPerSecond=(Pitch=0,Yaw=2000,Roll=0)
     Physics=10
     bNoDelete=false
     bTrailerPrePivot=true
     bSelfRotation=true
}
