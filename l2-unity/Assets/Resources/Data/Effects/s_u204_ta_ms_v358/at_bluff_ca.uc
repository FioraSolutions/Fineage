class at_bluff_ca extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter3
         // Reference: MeshEmitter'lineageeffect.dec.at_bluff_ca.MeshEmitter3'
         begin object name="MeshEmitter3" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Support.supportenchant01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5000000,Color=(R=170,G=170,B=170,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=30.0000000
         ColorMultiplierRange=(X=(Min=0.2020000,Max=1.0000000),Y=(Min=0.5570000,Max=0.6320000),Z=(Min=0.2040000,Max=0.2040000))
         Opacity=0.9000000
         FadeOutStartTime=1.2000000
         FadeOut=true
         FadeInEndTime=0.4000000
         FadeIn=true
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="Light"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.8000000,RelativeSize=0.2000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0000000)
         StartSizeRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.3000000,Max=0.5000000),Z=(Min=0.3000000,Max=0.5000000))
         InitialParticlesPerSecond=8.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter3'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.at_bluff_ca.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.2000000
         FadeOutStartTime=0.1600000
         FadeOut=true
         FadeInEndTime=0.0750000
         FadeIn=true
         MaxParticles=11
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="fire_swirl"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=-15.0000000,Max=-15.0000000),Y=(Min=-10.0000000,Max=-10.0000000),Z=(Min=10.0000000,Max=10.0000000))
         UniformSize=true
         InitialParticlesPerSecond=11.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Fire.fx_m_t2022'
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter5
         // Reference: SpriteEmitter'lineageeffect.dec.at_bluff_ca.SpriteEmitter5'
         begin object name="SpriteEmitter5" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4714290,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=30.0000000
         Opacity=0.2400000
         FadeOutStartTime=0.3000000
         FadeOut=true
         FadeInEndTime=0.1500000
         FadeIn=true
         MaxParticles=7
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="Center"
         StartLocationRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
         SpinParticles=true
         SpinCCWorCW=(X=0.3700000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.5000000)
         StartSizeRange=(X=(Min=24.6480007,Max=24.6480007),Y=(Min=24.6480007,Max=24.6480007),Z=(Min=24.6480007,Max=24.6480007))
         UniformSize=true
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
         LifetimeRange=(Min=0.6000000,Max=0.6000000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter5'
     Begin Object Class=MeshEmitter Name=MeshEmitter4
         // Reference: MeshEmitter'lineageeffect.dec.at_bluff_ca.MeshEmitter4'
         begin object name="MeshEmitter4" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.White.white_radiation00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.4900000,Max=0.4900000),Z=(Min=0.1390000,Max=0.1390000))
         Opacity=0.5000000
         FadeOutStartTime=3.0400000
         FadeOut=true
         FadeInEndTime=0.2660000
         FadeIn=true
         MaxParticles=4
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="elet"
         StartLocationRange=(X=(Min=-18.7199993,Max=18.7199993),Y=(Min=-18.7199993,Max=18.7199993),Z=(Min=-18.7199993,Max=18.7199993))
         StartLocationShape=1
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.5000000,Max=1.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.9100000,RelativeSize=0.8000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.3000000)
         StartSizeRange=(X=(Min=0.1000000,Max=0.1500000),Y=(Min=0.1000000,Max=0.1500000),Z=(Min=0.1000000,Max=0.1500000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=1.5000000,Max=1.5000000)
         end object
     End Object
     Emitters(3)=MeshEmitter'MeshEmitter4'
     Begin Object Class=MeshEmitter Name=MeshEmitter9
         // Reference: MeshEmitter'lineageeffect.dec.at_bluff_ca.MeshEmitter9'
         begin object name="MeshEmitter9" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.twist_m02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5214290,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=60.0000000
         ColorMultiplierRange=(X=(Min=0.5250000,Max=1.0000000),Y=(Min=0.7600000,Max=1.0000000),Z=(Min=0.8600000,Max=1.0000000))
         Opacity=0.5000000
         FadeOutStartTime=0.4800000
         FadeInEndTime=0.1152000
         FadeIn=true
         MaxParticles=5
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="swirl"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-5.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         SpinsPerSecondRange=(X=(Min=2.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=0.0500000,Max=0.0500000),Y=(Min=0.0500000,Max=0.0500000),Z=(Min=0.0500000,Max=0.0500000))
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.4800000,Max=0.4800000)
         end object
     End Object
     Emitters(4)=MeshEmitter'MeshEmitter9'
     // Reference: MeshEmitter'lineageeffect.dec.at_bluff_ca.MeshEmitter3'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Support.supportenchant01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5000000,Color=(R=170,G=170,B=170,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=30.0000000
     ColorMultiplierRange=(X=(Min=0.2020000,Max=1.0000000),Y=(Min=0.5570000,Max=0.6320000),Z=(Min=0.2040000,Max=0.2040000))
     Opacity=0.9000000
     FadeOutStartTime=1.2000000
     FadeOut=true
     FadeInEndTime=0.4000000
     FadeIn=true
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="Light"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.8000000,RelativeSize=0.2000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0000000)
     StartSizeRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.3000000,Max=0.5000000),Z=(Min=0.3000000,Max=0.5000000))
     InitialParticlesPerSecond=8.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     Emitters[0]=MeshEmitter3
     // Reference: SpriteEmitter'lineageeffect.dec.at_bluff_ca.SpriteEmitter2'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.2000000
     FadeOutStartTime=0.1600000
     FadeOut=true
     FadeInEndTime=0.0750000
     FadeIn=true
     MaxParticles=11
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="fire_swirl"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=-15.0000000,Max=-15.0000000),Y=(Min=-10.0000000,Max=-10.0000000),Z=(Min=10.0000000,Max=10.0000000))
     UniformSize=true
     InitialParticlesPerSecond=11.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Fire.fx_m_t2022'
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     Emitters[1]=SpriteEmitter2
     // Reference: SpriteEmitter'lineageeffect.dec.at_bluff_ca.SpriteEmitter5'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4714290,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=30.0000000
     Opacity=0.2400000
     FadeOutStartTime=0.3000000
     FadeOut=true
     FadeInEndTime=0.1500000
     FadeIn=true
     MaxParticles=7
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="Center"
     StartLocationRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
     SpinParticles=true
     SpinCCWorCW=(X=0.3700000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.5000000)
     StartSizeRange=(X=(Min=24.6480007,Max=24.6480007),Y=(Min=24.6480007,Max=24.6480007),Z=(Min=24.6480007,Max=24.6480007))
     UniformSize=true
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
     LifetimeRange=(Min=0.6000000,Max=0.6000000)
     Emitters[2]=SpriteEmitter5
     // Reference: MeshEmitter'lineageeffect.dec.at_bluff_ca.MeshEmitter4'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.White.white_radiation00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.4900000,Max=0.4900000),Z=(Min=0.1390000,Max=0.1390000))
     Opacity=0.5000000
     FadeOutStartTime=3.0400000
     FadeOut=true
     FadeInEndTime=0.2660000
     FadeIn=true
     MaxParticles=4
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="elet"
     StartLocationRange=(X=(Min=-18.7199993,Max=18.7199993),Y=(Min=-18.7199993,Max=18.7199993),Z=(Min=-18.7199993,Max=18.7199993))
     StartLocationShape=1
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.5000000,Max=1.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.9100000,RelativeSize=0.8000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.3000000)
     StartSizeRange=(X=(Min=0.1000000,Max=0.1500000),Y=(Min=0.1000000,Max=0.1500000),Z=(Min=0.1000000,Max=0.1500000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=1.5000000,Max=1.5000000)
     Emitters[3]=MeshEmitter4
     // Reference: MeshEmitter'lineageeffect.dec.at_bluff_ca.MeshEmitter9'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.twist_m02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5214290,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=60.0000000
     ColorMultiplierRange=(X=(Min=0.5250000,Max=1.0000000),Y=(Min=0.7600000,Max=1.0000000),Z=(Min=0.8600000,Max=1.0000000))
     Opacity=0.5000000
     FadeOutStartTime=0.4800000
     FadeInEndTime=0.1152000
     FadeIn=true
     MaxParticles=5
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="swirl"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-5.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     SpinsPerSecondRange=(X=(Min=2.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=0.0500000,Max=0.0500000),Y=(Min=0.0500000,Max=0.0500000),Z=(Min=0.0500000,Max=0.0500000))
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.4800000,Max=0.4800000)
     Emitters[4]=MeshEmitter9
     AutoDestroy=true
     bLightChanged=true
     bNoDelete=false
     bSunAffect=true
     bDirectional=true
}
