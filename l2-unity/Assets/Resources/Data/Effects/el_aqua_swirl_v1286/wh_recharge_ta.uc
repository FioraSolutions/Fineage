class wh_recharge_ta extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter8
         // Reference: SpriteEmitter'lineageeffect.dec.wh_recharge_ta.SpriteEmitter8'
         begin object name="SpriteEmitter8" class=Engine.SpriteEmitter
         Acceleration=(X=0.0000000,Y=0.0000000,Z=40.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5500000,Color=(R=73,G=73,B=73,A=255))
         ColorScale[2]=(RelativeTime=0.7821430,Color=(R=69,G=69,B=69,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=20.0000000
         ColorMultiplierRange=(X=(Min=0.8500000,Max=1.0000000),Y=(Min=0.8500000,Max=1.0000000),Z=(Min=0.7310000,Max=0.8590000))
         Opacity=0.9600000
         FadeOutStartTime=1.5000000
         FadeOut=true
         FadeInEndTime=0.6300000
         FadeIn=true
         MaxParticles=12
         RespawnDeadParticles=false
         Name="BlueDust"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
         StartLocationRange=(X=(Min=-27.0000000,Max=27.0000000),Y=(Min=-27.0000000,Max=27.0000000),Z=(Min=0.0000000,Max=0.0000000))
         SphereRadiusRange=(Min=8.0000000,Max=8.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.1400000,RelativeSize=0.9000000)
         SizeScale[1]=(RelativeTime=0.3800000,RelativeSize=0.8000000)
         SizeScale[2]=(RelativeTime=0.7800000,RelativeSize=0.3000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
         StartSizeRange=(X=(Min=7.0000000,Max=7.0000000),Y=(Min=7.0000000,Max=7.0000000),Z=(Min=7.0000000,Max=7.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0005'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=6
         SubdivisionEnd=8
         UseRandomSubdivision=true
         LifetimeRange=(Min=2.5000000,Max=3.0000000)
         InitialDelayRange=(Min=0.6500000,Max=0.6500000)
         StartVelocityRange=(X=(Min=-20.0000000,Max=-20.0000000),Y=(Min=-20.0000000,Max=-20.0000000),Z=(Min=10.0000000,Max=10.0000000))
         VelocityLossRange=(X=(Min=0.0000000,Max=1.3680000),Y=(Min=0.0000000,Max=1.3680000),Z=(Min=0.0000000,Max=0.0000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter8'
     Begin Object Class=BeamEmitter Name=BeamEmitter2
         // Reference: BeamEmitter'lineageeffect.dec.wh_recharge_ta.BeamEmitter2'
         begin object name="BeamEmitter2" class=Engine.BeamEmitter
         BeamEndPoints=/* Array type was not detected. */
         DetermineEndPointBy=2
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5214290,Color=(R=231,G=231,B=231,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=10.0000000
         ColorMultiplierRange=(X=(Min=0.5000000,Max=0.7040000),Y=(Min=0.5000000,Max=0.7040000),Z=(Min=0.5000000,Max=0.7040000))
         Opacity=0.3300000
         FadeOutStartTime=1.0000000
         FadeOut=true
         FadeInEndTime=0.5000000
         FadeIn=true
         RespawnDeadParticles=false
         Name="Beams"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=150.0000000)
         StartLocationRange=(X=(Min=-20.0000000,Max=10.0000000),Y=(Min=-15.0000000,Max=15.0000000),Z=(Min=40.0000000,Max=40.0000000))
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=70.0000000,Max=110.0000000),Z=(Min=13.0000000,Max=13.0000000))
         StartSizeRange=(X=(Min=6.0000000,Max=10.0000000),Y=(Min=100.0000000,Max=100.0000000),Z=(Min=100.0000000,Max=100.0000000))
         InitialParticlesPerSecond=7.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0004'
         LifetimeRange=(Min=3.0000000,Max=3.0000000)
         end object
     End Object
     Emitters(1)=BeamEmitter'BeamEmitter2'
     Begin Object Class=MeshEmitter Name=MeshEmitter8
         // Reference: MeshEmitter'lineageeffect.dec.wh_recharge_ta.MeshEmitter8'
         begin object name="MeshEmitter8" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Aura.auracharge01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4714290,Color=(R=191,G=191,B=191,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=20.0000000
         ColorMultiplierRange=(X=(Min=0.4000000,Max=0.4000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.4000000
         FadeOutStartTime=1.3600000
         FadeOut=true
         FadeInEndTime=0.5000000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="MeshBlue"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-20.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=0.9900000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         SizeScaleRepeats=20.0000000
         StartSizeRange=(X=(Min=0.3800000,Max=0.3800000),Y=(Min=0.3800000,Max=0.3800000),Z=(Min=3.0000000,Max=3.0000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=3.0000000,Max=3.0000000)
         InitialDelayRange=(Min=0.5000000,Max=0.5000000)
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter8'
     // Reference: SpriteEmitter'lineageeffect.dec.wh_recharge_ta.SpriteEmitter8'
     Acceleration=(X=0.0000000,Y=0.0000000,Z=40.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5500000,Color=(R=73,G=73,B=73,A=255))
     ColorScale[2]=(RelativeTime=0.7821430,Color=(R=69,G=69,B=69,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=20.0000000
     ColorMultiplierRange=(X=(Min=0.8500000,Max=1.0000000),Y=(Min=0.8500000,Max=1.0000000),Z=(Min=0.7310000,Max=0.8590000))
     Opacity=0.9600000
     FadeOutStartTime=1.5000000
     FadeOut=true
     FadeInEndTime=0.6300000
     FadeIn=true
     MaxParticles=12
     RespawnDeadParticles=false
     Name="BlueDust"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
     StartLocationRange=(X=(Min=-27.0000000,Max=27.0000000),Y=(Min=-27.0000000,Max=27.0000000),Z=(Min=0.0000000,Max=0.0000000))
     SphereRadiusRange=(Min=8.0000000,Max=8.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.1400000,RelativeSize=0.9000000)
     SizeScale[1]=(RelativeTime=0.3800000,RelativeSize=0.8000000)
     SizeScale[2]=(RelativeTime=0.7800000,RelativeSize=0.3000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
     StartSizeRange=(X=(Min=7.0000000,Max=7.0000000),Y=(Min=7.0000000,Max=7.0000000),Z=(Min=7.0000000,Max=7.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0005'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=6
     SubdivisionEnd=8
     UseRandomSubdivision=true
     LifetimeRange=(Min=2.5000000,Max=3.0000000)
     InitialDelayRange=(Min=0.6500000,Max=0.6500000)
     StartVelocityRange=(X=(Min=-20.0000000,Max=-20.0000000),Y=(Min=-20.0000000,Max=-20.0000000),Z=(Min=10.0000000,Max=10.0000000))
     VelocityLossRange=(X=(Min=0.0000000,Max=1.3680000),Y=(Min=0.0000000,Max=1.3680000),Z=(Min=0.0000000,Max=0.0000000))
     GetVelocityDirectionFrom=2
     Emitters[0]=SpriteEmitter8
     // Reference: BeamEmitter'lineageeffect.dec.wh_recharge_ta.BeamEmitter2'
     BeamEndPoints=/* Array type was not detected. */
     DetermineEndPointBy=2
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5214290,Color=(R=231,G=231,B=231,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=10.0000000
     ColorMultiplierRange=(X=(Min=0.5000000,Max=0.7040000),Y=(Min=0.5000000,Max=0.7040000),Z=(Min=0.5000000,Max=0.7040000))
     Opacity=0.3300000
     FadeOutStartTime=1.0000000
     FadeOut=true
     FadeInEndTime=0.5000000
     FadeIn=true
     RespawnDeadParticles=false
     Name="Beams"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=150.0000000)
     StartLocationRange=(X=(Min=-20.0000000,Max=10.0000000),Y=(Min=-15.0000000,Max=15.0000000),Z=(Min=40.0000000,Max=40.0000000))
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=70.0000000,Max=110.0000000),Z=(Min=13.0000000,Max=13.0000000))
     StartSizeRange=(X=(Min=6.0000000,Max=10.0000000),Y=(Min=100.0000000,Max=100.0000000),Z=(Min=100.0000000,Max=100.0000000))
     InitialParticlesPerSecond=7.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0004'
     LifetimeRange=(Min=3.0000000,Max=3.0000000)
     Emitters[1]=BeamEmitter2
     // Reference: MeshEmitter'lineageeffect.dec.wh_recharge_ta.MeshEmitter8'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Aura.auracharge01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4714290,Color=(R=191,G=191,B=191,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=20.0000000
     ColorMultiplierRange=(X=(Min=0.4000000,Max=0.4000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.4000000
     FadeOutStartTime=1.3600000
     FadeOut=true
     FadeInEndTime=0.5000000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="MeshBlue"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-20.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=0.9900000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     SizeScaleRepeats=20.0000000
     StartSizeRange=(X=(Min=0.3800000,Max=0.3800000),Y=(Min=0.3800000,Max=0.3800000),Z=(Min=3.0000000,Max=3.0000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=3.0000000,Max=3.0000000)
     InitialDelayRange=(Min=0.5000000,Max=0.5000000)
     Emitters[2]=MeshEmitter8
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.2500000
}
