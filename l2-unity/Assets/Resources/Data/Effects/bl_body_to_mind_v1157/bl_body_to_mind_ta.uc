class bl_body_to_mind_ta extends Emitter;

defaultproperties
{
     Begin Object Class=BeamEmitter Name=BeamEmitter1
         // Reference: BeamEmitter'lineageeffect.dec.bl_body_to_mind_ta.BeamEmitter1'
         begin object name="BeamEmitter1" class=Engine.BeamEmitter
         BeamEndPoints=/* Array type was not detected. */
         DetermineEndPointBy=2
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5214290,Color=(R=231,G=231,B=231,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=17.0000000
         ColorMultiplierRange=(X=(Min=0.3500000,Max=0.3500000),Y=(Min=0.3500000,Max=0.3500000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.1700000
         FadeOutStartTime=0.9900000
         FadeOut=true
         FadeInEndTime=0.4200000
         FadeIn=true
         RespawnDeadParticles=false
         Name="Beams"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=150.0000000)
         StartLocationRange=(X=(Min=-20.0000000,Max=10.0000000),Y=(Min=-15.0000000,Max=15.0000000),Z=(Min=40.0000000,Max=40.0000000))
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=70.0000000,Max=110.0000000),Z=(Min=13.0000000,Max=13.0000000))
         StartSizeRange=(X=(Min=6.0000000,Max=10.0000000),Y=(Min=100.0000000,Max=100.0000000),Z=(Min=100.0000000,Max=100.0000000))
         InitialParticlesPerSecond=30.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0004'
         LifetimeRange=(Min=1.5000000,Max=1.5000000)
         end object
     End Object
     Emitters(0)=BeamEmitter'BeamEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter4
         // Reference: MeshEmitter'lineageeffect.dec.bl_body_to_mind_ta.MeshEmitter4'
         begin object name="MeshEmitter4" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Aura.auracharge01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4714290,Color=(R=191,G=191,B=191,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=20.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.3390000,Max=0.3390000))
         Opacity=0.5300000
         FadeOutStartTime=1.0400000
         FadeOut=true
         FadeInEndTime=0.3800000
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
         DrawStyle=5
         LifetimeRange=(Min=1.7000000,Max=1.7000000)
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter4'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.bl_body_to_mind_ta.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         UseDirectionAs=1
         Acceleration=(X=0.0000000,Y=0.0000000,Z=100.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.7920000
         FadeOut=true
         FadeInEndTime=0.2520000
         FadeIn=true
         MaxParticles=15
         ResetAfterChange=true
         RespawnDeadParticles=false
         Name="Mana"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-40.0000000)
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=17.0000000,Max=17.0000000))
         UseRotationFrom=1
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.2100000,RelativeSize=0.9000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
         StartSizeRange=(X=(Min=1.0000000,Max=2.0000000),Y=(Min=30.0000000,Max=50.0000000),Z=(Min=30.0000000,Max=50.0000000))
         InitialParticlesPerSecond=15.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=3
         SubdivisionEnd=4
         UseRandomSubdivision=true
         LifetimeRange=(Min=1.2000000,Max=1.6000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=800.0000000,Max=1100.0000000))
         VelocityLossRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter1'
     // Reference: BeamEmitter'lineageeffect.dec.bl_body_to_mind_ta.BeamEmitter1'
     BeamEndPoints=/* Array type was not detected. */
     DetermineEndPointBy=2
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5214290,Color=(R=231,G=231,B=231,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=17.0000000
     ColorMultiplierRange=(X=(Min=0.3500000,Max=0.3500000),Y=(Min=0.3500000,Max=0.3500000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.1700000
     FadeOutStartTime=0.9900000
     FadeOut=true
     FadeInEndTime=0.4200000
     FadeIn=true
     RespawnDeadParticles=false
     Name="Beams"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=150.0000000)
     StartLocationRange=(X=(Min=-20.0000000,Max=10.0000000),Y=(Min=-15.0000000,Max=15.0000000),Z=(Min=40.0000000,Max=40.0000000))
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=70.0000000,Max=110.0000000),Z=(Min=13.0000000,Max=13.0000000))
     StartSizeRange=(X=(Min=6.0000000,Max=10.0000000),Y=(Min=100.0000000,Max=100.0000000),Z=(Min=100.0000000,Max=100.0000000))
     InitialParticlesPerSecond=30.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0004'
     LifetimeRange=(Min=1.5000000,Max=1.5000000)
     Emitters[0]=BeamEmitter1
     // Reference: MeshEmitter'lineageeffect.dec.bl_body_to_mind_ta.MeshEmitter4'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Aura.auracharge01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4714290,Color=(R=191,G=191,B=191,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=20.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.3390000,Max=0.3390000))
     Opacity=0.5300000
     FadeOutStartTime=1.0400000
     FadeOut=true
     FadeInEndTime=0.3800000
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
     DrawStyle=5
     LifetimeRange=(Min=1.7000000,Max=1.7000000)
     Emitters[1]=MeshEmitter4
     // Reference: SpriteEmitter'lineageeffect.dec.bl_body_to_mind_ta.SpriteEmitter1'
     UseDirectionAs=1
     Acceleration=(X=0.0000000,Y=0.0000000,Z=100.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.7920000
     FadeOut=true
     FadeInEndTime=0.2520000
     FadeIn=true
     MaxParticles=15
     ResetAfterChange=true
     RespawnDeadParticles=false
     Name="Mana"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-40.0000000)
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=17.0000000,Max=17.0000000))
     UseRotationFrom=1
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.2100000,RelativeSize=0.9000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
     StartSizeRange=(X=(Min=1.0000000,Max=2.0000000),Y=(Min=30.0000000,Max=50.0000000),Z=(Min=30.0000000,Max=50.0000000))
     InitialParticlesPerSecond=15.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=3
     SubdivisionEnd=4
     UseRandomSubdivision=true
     LifetimeRange=(Min=1.2000000,Max=1.6000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=800.0000000,Max=1100.0000000))
     VelocityLossRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
     Emitters[2]=SpriteEmitter1
     AutoDestroy=true
     bNoDelete=false
     Rotation=(Pitch=0,Yaw=224,Roll=0)
     DrawScale=0.2500000
     bDirectional=true
}
