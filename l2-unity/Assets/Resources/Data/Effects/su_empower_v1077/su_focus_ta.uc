class su_focus_ta extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter0
         // Reference: MeshEmitter'lineageeffect.dec.su_focus_ta.MeshEmitter0'
         begin object name="MeshEmitter0" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Aura.auracharge01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4857140,Color=(R=180,G=180,B=180,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=20.0000000
         ColorMultiplierRange=(X=(Min=0.3000000,Max=0.3000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.6000000
         FadeOutStartTime=1.1160001
         FadeOut=true
         FadeInEndTime=0.1800000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="MeshBase"
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=0.4000000,Max=0.4000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=1.3000000,Max=1.3000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=1.8000000,Max=1.8000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter0'
     Begin Object Class=MeshEmitter Name=MeshEmitter2
         // Reference: MeshEmitter'lineageeffect.dec.su_focus_ta.MeshEmitter2'
         begin object name="MeshEmitter2" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Support.supportenchant03'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7550000,Max=0.7550000),Z=(Min=0.7550000,Max=0.7550000))
         FadeOutStartTime=0.2520000
         FadeOut=true
         FadeInEndTime=0.0420000
         FadeIn=true
         RespawnDeadParticles=false
         Name="Lines"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-60.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=0.3500000,Max=0.3500000),Y=(Min=0.3500000,Max=0.3500000),Z=(Min=0.7000000,Max=0.7000000))
         InitialParticlesPerSecond=18.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.6000000,Max=0.6000000)
         InitialDelayRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=280.0000000,Max=280.0000000))
         VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.5000000,Max=0.5000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.su_focus_ta.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         UseDirectionAs=1
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=0))
         ColorScale[1]=(RelativeTime=0.2142860,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=0))
         ColorScaleRepeats=8.0000000
         ColorMultiplierRange=(X=(Min=0.4000000,Max=0.6000000),Y=(Min=0.2000000,Max=0.4000000),Z=(Min=0.8000000,Max=1.0000000))
         Opacity=0.6000000
         FadeOutStartTime=0.3500000
         FadeOut=true
         FadeInEndTime=0.0770000
         FadeIn=true
         CoordinateSystem=0
         MaxParticles=20
         ResetAfterChange=true
         RespawnDeadParticles=false
         Name="Particle"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-50.0000000)
         StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
         StartLocationShape=2
         SphereRadiusRange=(Min=3.0000000,Max=3.0000000)
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=16.0000000,Max=16.0000000))
         SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.2000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.6000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
         StartSizeRange=(X=(Min=0.6000000,Max=1.2000000),Y=(Min=30.0000000,Max=60.0000000),Z=(Min=30.0000000,Max=60.0000000))
         InitialParticlesPerSecond=12.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=8
         TextureVSubdivisions=8
         SubdivisionStart=3
         SubdivisionEnd=4
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.5000000,Max=0.7000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=400.0000000,Max=400.0000000))
         VelocityLossRange=(X=(Min=2.0000000,Max=5.0000000),Y=(Min=2.0000000,Max=5.0000000),Z=(Min=2.0000000,Max=2.0000000))
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter1'
     // Reference: MeshEmitter'lineageeffect.dec.su_focus_ta.MeshEmitter0'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Aura.auracharge01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4857140,Color=(R=180,G=180,B=180,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=20.0000000
     ColorMultiplierRange=(X=(Min=0.3000000,Max=0.3000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.6000000
     FadeOutStartTime=1.1160001
     FadeOut=true
     FadeInEndTime=0.1800000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="MeshBase"
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=0.4000000,Max=0.4000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=1.3000000,Max=1.3000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=1.8000000,Max=1.8000000)
     Emitters[0]=MeshEmitter0
     // Reference: MeshEmitter'lineageeffect.dec.su_focus_ta.MeshEmitter2'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Support.supportenchant03'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7550000,Max=0.7550000),Z=(Min=0.7550000,Max=0.7550000))
     FadeOutStartTime=0.2520000
     FadeOut=true
     FadeInEndTime=0.0420000
     FadeIn=true
     RespawnDeadParticles=false
     Name="Lines"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-60.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=0.3500000,Max=0.3500000),Y=(Min=0.3500000,Max=0.3500000),Z=(Min=0.7000000,Max=0.7000000))
     InitialParticlesPerSecond=18.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.6000000,Max=0.6000000)
     InitialDelayRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=280.0000000,Max=280.0000000))
     VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.5000000,Max=0.5000000))
     Emitters[1]=MeshEmitter2
     // Reference: SpriteEmitter'lineageeffect.dec.su_focus_ta.SpriteEmitter1'
     UseDirectionAs=1
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=0))
     ColorScale[1]=(RelativeTime=0.2142860,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=0))
     ColorScaleRepeats=8.0000000
     ColorMultiplierRange=(X=(Min=0.4000000,Max=0.6000000),Y=(Min=0.2000000,Max=0.4000000),Z=(Min=0.8000000,Max=1.0000000))
     Opacity=0.6000000
     FadeOutStartTime=0.3500000
     FadeOut=true
     FadeInEndTime=0.0770000
     FadeIn=true
     CoordinateSystem=0
     MaxParticles=20
     ResetAfterChange=true
     RespawnDeadParticles=false
     Name="Particle"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-50.0000000)
     StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
     StartLocationShape=2
     SphereRadiusRange=(Min=3.0000000,Max=3.0000000)
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=16.0000000,Max=16.0000000))
     SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.2000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.6000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
     StartSizeRange=(X=(Min=0.6000000,Max=1.2000000),Y=(Min=30.0000000,Max=60.0000000),Z=(Min=30.0000000,Max=60.0000000))
     InitialParticlesPerSecond=12.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
     TextureUSubdivisions=8
     TextureVSubdivisions=8
     SubdivisionStart=3
     SubdivisionEnd=4
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.5000000,Max=0.7000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=400.0000000,Max=400.0000000))
     VelocityLossRange=(X=(Min=2.0000000,Max=5.0000000),Y=(Min=2.0000000,Max=5.0000000),Z=(Min=2.0000000,Max=2.0000000))
     Emitters[2]=SpriteEmitter1
     AutoDestroy=true
     bRotEmitter=true
     RotPerSecond=(Pitch=0,Yaw=65536,Roll=0)
     bNoDelete=false
     DrawScale=0.2500000
     bDirectional=true
}
