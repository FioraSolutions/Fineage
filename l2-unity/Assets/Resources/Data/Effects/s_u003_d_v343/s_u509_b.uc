class s_u509_b extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter13
         // Reference: SpriteEmitter'lineageeffect.dec.s_u509_b.SpriteEmitter13'
         begin object name="SpriteEmitter13" class=Engine.SpriteEmitter
         Acceleration=(X=-10.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4964290,Color=(R=180,G=180,B=180,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=15.0000000
         FadeOutStartTime=0.2000000
         FadeOut=true
         MaxParticles=20
         RespawnDeadParticles=false
         Name="Spark"
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=5.0000000,Max=5.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0100000,Max=0.1230000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
         StartSizeRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.5000000,Max=1.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=14
         SubdivisionEnd=16
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.8000000,Max=1.2000000)
         StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=200.0000000,Max=200.0000000))
         VelocityLossRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
         GetVelocityDirectionFrom=1
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter13'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter16
         // Reference: SpriteEmitter'lineageeffect.dec.s_u509_b.SpriteEmitter16'
         begin object name="SpriteEmitter16" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4821430,Color=(R=180,G=180,B=180,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=10.0000000
         Opacity=0.6000000
         FadeOutStartTime=0.1602000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Rainbow_out"
         StartLocationOffset=(X=-1.0000000,Y=0.0000000,Z=0.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
         StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
         LifetimeRange=(Min=0.8010000,Max=0.8010000)
         StartVelocityRange=(X=(Min=-10.0000000,Max=-10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter16'
     Begin Object Class=MeshEmitter Name=MeshEmitter16
         // Reference: MeshEmitter'lineageeffect.dec.s_u509_b.MeshEmitter16'
         begin object name="MeshEmitter16" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windknifeball00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=-300.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=243,G=149,B=7,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.0780000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Wind"
         StartLocationOffset=(X=10.0000000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=3.0000000,Max=3.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter16'
     // Reference: SpriteEmitter'lineageeffect.dec.s_u509_b.SpriteEmitter13'
     Acceleration=(X=-10.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4964290,Color=(R=180,G=180,B=180,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=15.0000000
     FadeOutStartTime=0.2000000
     FadeOut=true
     MaxParticles=20
     RespawnDeadParticles=false
     Name="Spark"
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=5.0000000,Max=5.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0100000,Max=0.1230000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
     StartSizeRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.5000000,Max=1.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=14
     SubdivisionEnd=16
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.8000000,Max=1.2000000)
     StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=200.0000000,Max=200.0000000))
     VelocityLossRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
     GetVelocityDirectionFrom=1
     Emitters[0]=SpriteEmitter13
     // Reference: SpriteEmitter'lineageeffect.dec.s_u509_b.SpriteEmitter16'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4821430,Color=(R=180,G=180,B=180,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=10.0000000
     Opacity=0.6000000
     FadeOutStartTime=0.1602000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Rainbow_out"
     StartLocationOffset=(X=-1.0000000,Y=0.0000000,Z=0.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
     StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
     LifetimeRange=(Min=0.8010000,Max=0.8010000)
     StartVelocityRange=(X=(Min=-10.0000000,Max=-10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[1]=SpriteEmitter16
     // Reference: MeshEmitter'lineageeffect.dec.s_u509_b.MeshEmitter16'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windknifeball00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=-300.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=243,G=149,B=7,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.0780000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Wind"
     StartLocationOffset=(X=10.0000000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=3.0000000,Max=3.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[2]=MeshEmitter16
     AutoDestroy=true
     bNoDelete=false
     bDirectional=true
}
