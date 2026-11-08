class wh_sec_heal_ca extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter10
         // Reference: MeshEmitter'lineageeffect.dec.wh_sec_heal_ca.MeshEmitter10'
         begin object name="MeshEmitter10" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.White.white_center01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=125,G=125,B=125,A=0))
         ColorScale[1]=(RelativeTime=0.5071430,Color=(R=168,G=168,B=168,A=0))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=125,G=125,B=125,A=0))
         ColorScaleRepeats=40.0000000
         Opacity=0.8000000
         FadeOutStartTime=2.6040001
         FadeOut=true
         FadeInEndTime=0.5880000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Base"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=1.5000000)
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         SpinsPerSecondRange=(X=(Min=0.0200000,Max=0.0200000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=0.0700000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0500000)
         StartSizeRange=(X=(Min=0.2300000,Max=0.2300000),Y=(Min=0.2300000,Max=0.2300000),Z=(Min=0.2300000,Max=0.2300000))
         InitialParticlesPerSecond=500.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=4.3000002,Max=4.3000002)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter10'
     Begin Object Class=MeshEmitter Name=MeshEmitter13
         // Reference: MeshEmitter'lineageeffect.dec.wh_sec_heal_ca.MeshEmitter13'
         begin object name="MeshEmitter13" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.White.white_sideRing'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=231,G=231,B=231,A=255))
         ColorScale[1]=(RelativeTime=0.4857140,Color=(R=200,G=200,B=200,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=30.0000000
         Opacity=0.8000000
         FadeOutStartTime=2.3180001
         FadeOut=true
         FadeInEndTime=1.0260000
         FadeIn=true
         MaxParticles=1
         WeatherSoundCheck=true
         RespawnDeadParticles=false
         Name="swirl1"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=0.2500000)
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         SpinsPerSecondRange=(X=(Min=0.0200000,Max=0.0200000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=0.2100000,Max=0.2100000),Y=(Min=0.2100000,Max=0.2100000),Z=(Min=0.2100000,Max=0.2100000))
         InitialParticlesPerSecond=500.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=3.8000000,Max=3.8000000)
         InitialDelayRange=(Min=0.3000000,Max=0.3000000)
         VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter13'
     Begin Object Class=MeshEmitter Name=MeshEmitter14
         // Reference: MeshEmitter'lineageeffect.dec.wh_sec_heal_ca.MeshEmitter14'
         begin object name="MeshEmitter14" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.White.white_centerRing'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4821430,Color=(R=167,G=167,B=167,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=40.0000000
         Opacity=0.5000000
         FadeOutStartTime=1.7400000
         FadeOut=true
         FadeInEndTime=0.3480000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="shield"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=0.5000000)
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         SpinsPerSecondRange=(X=(Min=-0.0200000,Max=-0.0200000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=0.1720000,Max=0.1720000),Y=(Min=0.1720000,Max=0.1720000),Z=(Min=0.1720000,Max=0.1720000))
         InitialParticlesPerSecond=100.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=2.9000001,Max=2.9000001)
         InitialDelayRange=(Min=0.3800000,Max=0.3800000)
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter14'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.wh_sec_heal_ca.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.5000000
         FadeOutStartTime=1.3300000
         FadeOut=true
         FadeInEndTime=0.3500000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Ring"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=5.5000000)
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.0200000,Max=0.0200000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=19.0000000,Max=19.0000000),Y=(Min=19.0000000,Max=19.0000000),Z=(Min=19.0000000,Max=19.0000000))
         UniformSize=true
         InitialParticlesPerSecond=500.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         Texture=Texture'LineageEffectsTextures.Particles3.fx_m_t7009'
         LifetimeRange=(Min=3.5000000,Max=3.5000000)
         InitialDelayRange=(Min=0.3500000,Max=0.3500000)
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter2'
     // Reference: MeshEmitter'lineageeffect.dec.wh_sec_heal_ca.MeshEmitter10'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.White.white_center01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=125,G=125,B=125,A=0))
     ColorScale[1]=(RelativeTime=0.5071430,Color=(R=168,G=168,B=168,A=0))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=125,G=125,B=125,A=0))
     ColorScaleRepeats=40.0000000
     Opacity=0.8000000
     FadeOutStartTime=2.6040001
     FadeOut=true
     FadeInEndTime=0.5880000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Base"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=1.5000000)
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     SpinsPerSecondRange=(X=(Min=0.0200000,Max=0.0200000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=0.0700000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0500000)
     StartSizeRange=(X=(Min=0.2300000,Max=0.2300000),Y=(Min=0.2300000,Max=0.2300000),Z=(Min=0.2300000,Max=0.2300000))
     InitialParticlesPerSecond=500.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=4.3000002,Max=4.3000002)
     Emitters[0]=MeshEmitter10
     // Reference: MeshEmitter'lineageeffect.dec.wh_sec_heal_ca.MeshEmitter13'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.White.white_sideRing'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=231,G=231,B=231,A=255))
     ColorScale[1]=(RelativeTime=0.4857140,Color=(R=200,G=200,B=200,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=30.0000000
     Opacity=0.8000000
     FadeOutStartTime=2.3180001
     FadeOut=true
     FadeInEndTime=1.0260000
     FadeIn=true
     MaxParticles=1
     WeatherSoundCheck=true
     RespawnDeadParticles=false
     Name="swirl1"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=0.2500000)
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     SpinsPerSecondRange=(X=(Min=0.0200000,Max=0.0200000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=0.2100000,Max=0.2100000),Y=(Min=0.2100000,Max=0.2100000),Z=(Min=0.2100000,Max=0.2100000))
     InitialParticlesPerSecond=500.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=3.8000000,Max=3.8000000)
     InitialDelayRange=(Min=0.3000000,Max=0.3000000)
     VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
     Emitters[1]=MeshEmitter13
     // Reference: MeshEmitter'lineageeffect.dec.wh_sec_heal_ca.MeshEmitter14'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.White.white_centerRing'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4821430,Color=(R=167,G=167,B=167,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=40.0000000
     Opacity=0.5000000
     FadeOutStartTime=1.7400000
     FadeOut=true
     FadeInEndTime=0.3480000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="shield"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=0.5000000)
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     SpinsPerSecondRange=(X=(Min=-0.0200000,Max=-0.0200000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=0.1720000,Max=0.1720000),Y=(Min=0.1720000,Max=0.1720000),Z=(Min=0.1720000,Max=0.1720000))
     InitialParticlesPerSecond=100.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=2.9000001,Max=2.9000001)
     InitialDelayRange=(Min=0.3800000,Max=0.3800000)
     Emitters[2]=MeshEmitter14
     // Reference: SpriteEmitter'lineageeffect.dec.wh_sec_heal_ca.SpriteEmitter2'
     UseDirectionAs=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.5000000
     FadeOutStartTime=1.3300000
     FadeOut=true
     FadeInEndTime=0.3500000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Ring"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=5.5000000)
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.0200000,Max=0.0200000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=19.0000000,Max=19.0000000),Y=(Min=19.0000000,Max=19.0000000),Z=(Min=19.0000000,Max=19.0000000))
     UniformSize=true
     InitialParticlesPerSecond=500.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     Texture=Texture'LineageEffectsTextures.Particles3.fx_m_t7009'
     LifetimeRange=(Min=3.5000000,Max=3.5000000)
     InitialDelayRange=(Min=0.3500000,Max=0.3500000)
     Emitters[3]=SpriteEmitter2
     AutoDestroy=true
     bLightChanged=true
     bNoDelete=false
     bSunAffect=true
     DrawScale=0.1000000
}
