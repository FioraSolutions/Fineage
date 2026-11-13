class mo_dash_ta extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter8
         // Reference: SpriteEmitter'lineageeffect.dec.mo_dash_ta.SpriteEmitter8'
         begin object name="SpriteEmitter8" class=Engine.SpriteEmitter
         UseDirectionAs=1
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5964290,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=8.0000000
         FadeOutStartTime=0.1400000
         FadeOut=true
         MaxParticles=12
         RespawnDeadParticles=false
         Name="EndTick"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-30.0000000)
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=70.0000000,Max=110.0000000),Z=(Min=25.0000000,Max=25.0000000))
         SpinParticles=true
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
         StartSizeRange=(X=(Min=1.0000000,Max=1.5000000),Y=(Min=40.0000000,Max=50.0000000),Z=(Min=40.0000000,Max=50.0000000))
         InitialParticlesPerSecond=50.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionEnd=2
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.5000000,Max=1.0000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=300.0000000,Max=500.0000000))
         VelocityLossRange=(X=(Min=4.5000000,Max=4.5000000),Y=(Min=4.5000000,Max=4.5000000),Z=(Min=4.5000000,Max=4.5000000))
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter8'
     Begin Object Class=MeshEmitter Name=MeshEmitter37
         // Reference: MeshEmitter'lineageeffect.dec.mo_dash_ta.MeshEmitter37'
         begin object name="MeshEmitter37" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windknifewave00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=0.0000000,Y=0.0000000,Z=540.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.3142860,Color=(R=255,G=230,B=162,A=255))
         ColorScale[2]=(RelativeTime=0.5892860,Color=(R=255,G=159,B=0,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=227,G=0,B=0,A=255))
         Opacity=0.7000000
         FadeOutStartTime=0.1290000
         FadeOut=true
         FadeInEndTime=0.0600000
         FadeIn=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="EndWave"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=25.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.7500000,Max=0.7500000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.3100000,RelativeSize=1.5000000)
         SizeScale[1]=(RelativeTime=0.6500000,RelativeSize=2.2000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=3.5000000)
         StartSizeRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=0.4000000,Max=0.4000000))
         InitialParticlesPerSecond=20.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-240.0000153,Max=-240.0000153))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter37'
     // Reference: SpriteEmitter'lineageeffect.dec.mo_dash_ta.SpriteEmitter8'
     UseDirectionAs=1
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5964290,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=8.0000000
     FadeOutStartTime=0.1400000
     FadeOut=true
     MaxParticles=12
     RespawnDeadParticles=false
     Name="EndTick"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-30.0000000)
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=70.0000000,Max=110.0000000),Z=(Min=25.0000000,Max=25.0000000))
     SpinParticles=true
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
     StartSizeRange=(X=(Min=1.0000000,Max=1.5000000),Y=(Min=40.0000000,Max=50.0000000),Z=(Min=40.0000000,Max=50.0000000))
     InitialParticlesPerSecond=50.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionEnd=2
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.5000000,Max=1.0000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=300.0000000,Max=500.0000000))
     VelocityLossRange=(X=(Min=4.5000000,Max=4.5000000),Y=(Min=4.5000000,Max=4.5000000),Z=(Min=4.5000000,Max=4.5000000))
     Emitters[0]=SpriteEmitter8
     // Reference: MeshEmitter'lineageeffect.dec.mo_dash_ta.MeshEmitter37'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windknifewave00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=0.0000000,Y=0.0000000,Z=540.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.3142860,Color=(R=255,G=230,B=162,A=255))
     ColorScale[2]=(RelativeTime=0.5892860,Color=(R=255,G=159,B=0,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=227,G=0,B=0,A=255))
     Opacity=0.7000000
     FadeOutStartTime=0.1290000
     FadeOut=true
     FadeInEndTime=0.0600000
     FadeIn=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="EndWave"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=25.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.7500000,Max=0.7500000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.3100000,RelativeSize=1.5000000)
     SizeScale[1]=(RelativeTime=0.6500000,RelativeSize=2.2000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=3.5000000)
     StartSizeRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=0.4000000,Max=0.4000000))
     InitialParticlesPerSecond=20.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-240.0000153,Max=-240.0000153))
     Emitters[1]=MeshEmitter37
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.2000000
     bDirectional=true
}
