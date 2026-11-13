class s_u509_e extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter12
         // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter12'
         begin object name="SpriteEmitter12" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4714290,Color=(R=209,G=209,B=209,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=20.0000000
         ColorMultiplierRange=(X=(Min=0.8200000,Max=0.8200000),Y=(Min=0.7500000,Max=0.7500000),Z=(Min=0.7500000,Max=0.7500000))
         FadeOutStartTime=0.1500000
         FadeOut=true
         MaxParticles=8
         RespawnDeadParticles=false
         Name="fire_dust"
         StartLocationShape=2
         SphereRadiusRange=(Min=25.0000000,Max=25.0000000)
         StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=20.0000000,Max=20.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.4500000,RelativeSize=0.9700000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         SizeScaleRepeats=10.0000000
         StartSizeRange=(X=(Min=15.0000000,Max=20.0000000),Y=(Min=15.0000000,Max=20.0000000),Z=(Min=15.0000000,Max=20.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=5
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
         LifetimeRange=(Min=1.0000000,Max=2.0000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=100.0000000,Max=100.0000000),Z=(Min=100.0000000,Max=100.0000000))
         VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
         GetVelocityDirectionFrom=1
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter12'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter18
         // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter18'
         begin object name="SpriteEmitter18" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4821430,Color=(R=191,G=191,B=191,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=4.0000000
         Opacity=0.4600000
         FadeOutStartTime=0.2160000
         FadeOut=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="Circle_Sparks"
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=0.9000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=32.0000000,Max=32.0000000),Y=(Min=32.0000000,Max=32.0000000),Z=(Min=32.0000000,Max=32.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=5
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1017'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         BlendBetweenSubdivisions=true
         SubdivisionStart=1
         SubdivisionEnd=2
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter18'
     Begin Object Class=MeshEmitter Name=MeshEmitter4
         // Reference: MeshEmitter'lineageeffect.dec.s_u509_e.MeshEmitter4'
         begin object name="MeshEmitter4" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.spark00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=0.0000000,Y=0.0000000,Z=30.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.6000000
         FadeOutStartTime=0.1000000
         FadeOut=true
         FadeInEndTime=0.1000000
         FadeIn=true
         MaxParticles=8
         RespawnDeadParticles=false
         Name="Elt"
         StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.5000000,Max=1.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         StartSizeRange=(X=(Min=0.1000000,Max=0.2500000),Y=(Min=0.1000000,Max=0.2500000),Z=(Min=0.1000000,Max=0.2500000))
         InitialParticlesPerSecond=10.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.8000000)
         StartVelocityRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter4'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter19
         // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter19'
         begin object name="SpriteEmitter19" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5000000,Color=(R=200,G=200,B=200,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=15.0000000
         Opacity=0.4000000
         FadeOutStartTime=0.0802000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Rainbow"
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0500000)
         StartSizeRange=(X=(Min=35.0000000,Max=35.0000000),Y=(Min=35.0000000,Max=35.0000000),Z=(Min=35.0000000,Max=35.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
         LifetimeRange=(Min=0.8000000,Max=0.8000000)
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter19'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter21
         // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter21'
         begin object name="SpriteEmitter21" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.2000000
         FadeOutStartTime=0.0600000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="SpriteEmitter0"
         StartSizeRange=(X=(Min=35.0000000,Max=35.0000000),Y=(Min=35.0000000,Max=35.0000000),Z=(Min=35.0000000,Max=35.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0143'
         TextureUSubdivisions=1
         TextureVSubdivisions=2
         SubdivisionEnd=1
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         end object
     End Object
     Emitters(4)=SpriteEmitter'SpriteEmitter21'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter22
         // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter22'
         begin object name="SpriteEmitter22" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.2000000
         FadeOutStartTime=0.0240000
         FadeOut=true
         MaxParticles=5
         RespawnDeadParticles=false
         Name="Beam"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=15.0000000,Max=15.0000000),Y=(Min=15.0000000,Max=15.0000000),Z=(Min=15.0000000,Max=15.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1022'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         BlendBetweenSubdivisions=true
         SubdivisionEnd=1
         LifetimeRange=(Min=0.2000000,Max=0.2000000)
         end object
     End Object
     Emitters(5)=SpriteEmitter'SpriteEmitter22'
     // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter12'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4714290,Color=(R=209,G=209,B=209,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=20.0000000
     ColorMultiplierRange=(X=(Min=0.8200000,Max=0.8200000),Y=(Min=0.7500000,Max=0.7500000),Z=(Min=0.7500000,Max=0.7500000))
     FadeOutStartTime=0.1500000
     FadeOut=true
     MaxParticles=8
     RespawnDeadParticles=false
     Name="fire_dust"
     StartLocationShape=2
     SphereRadiusRange=(Min=25.0000000,Max=25.0000000)
     StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=20.0000000,Max=20.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.4500000,RelativeSize=0.9700000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     SizeScaleRepeats=10.0000000
     StartSizeRange=(X=(Min=15.0000000,Max=20.0000000),Y=(Min=15.0000000,Max=20.0000000),Z=(Min=15.0000000,Max=20.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=5
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
     LifetimeRange=(Min=1.0000000,Max=2.0000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=100.0000000,Max=100.0000000),Z=(Min=100.0000000,Max=100.0000000))
     VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
     GetVelocityDirectionFrom=1
     Emitters[0]=SpriteEmitter12
     // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter18'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4821430,Color=(R=191,G=191,B=191,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=4.0000000
     Opacity=0.4600000
     FadeOutStartTime=0.2160000
     FadeOut=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="Circle_Sparks"
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=0.9000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=32.0000000,Max=32.0000000),Y=(Min=32.0000000,Max=32.0000000),Z=(Min=32.0000000,Max=32.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=5
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1017'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     BlendBetweenSubdivisions=true
     SubdivisionStart=1
     SubdivisionEnd=2
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     Emitters[1]=SpriteEmitter18
     // Reference: MeshEmitter'lineageeffect.dec.s_u509_e.MeshEmitter4'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.spark00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=0.0000000,Y=0.0000000,Z=30.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.6000000
     FadeOutStartTime=0.1000000
     FadeOut=true
     FadeInEndTime=0.1000000
     FadeIn=true
     MaxParticles=8
     RespawnDeadParticles=false
     Name="Elt"
     StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.5000000,Max=1.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     StartSizeRange=(X=(Min=0.1000000,Max=0.2500000),Y=(Min=0.1000000,Max=0.2500000),Z=(Min=0.1000000,Max=0.2500000))
     InitialParticlesPerSecond=10.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.8000000)
     StartVelocityRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[2]=MeshEmitter4
     // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter19'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5000000,Color=(R=200,G=200,B=200,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=15.0000000
     Opacity=0.4000000
     FadeOutStartTime=0.0802000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Rainbow"
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0500000)
     StartSizeRange=(X=(Min=35.0000000,Max=35.0000000),Y=(Min=35.0000000,Max=35.0000000),Z=(Min=35.0000000,Max=35.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0056'
     LifetimeRange=(Min=0.8000000,Max=0.8000000)
     Emitters[3]=SpriteEmitter19
     // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter21'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.2000000
     FadeOutStartTime=0.0600000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="SpriteEmitter0"
     StartSizeRange=(X=(Min=35.0000000,Max=35.0000000),Y=(Min=35.0000000,Max=35.0000000),Z=(Min=35.0000000,Max=35.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0143'
     TextureUSubdivisions=1
     TextureVSubdivisions=2
     SubdivisionEnd=1
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     Emitters[4]=SpriteEmitter21
     // Reference: SpriteEmitter'lineageeffect.dec.s_u509_e.SpriteEmitter22'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.2000000
     FadeOutStartTime=0.0240000
     FadeOut=true
     MaxParticles=5
     RespawnDeadParticles=false
     Name="Beam"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=15.0000000,Max=15.0000000),Y=(Min=15.0000000,Max=15.0000000),Z=(Min=15.0000000,Max=15.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1022'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     BlendBetweenSubdivisions=true
     SubdivisionEnd=1
     LifetimeRange=(Min=0.2000000,Max=0.2000000)
     Emitters[5]=SpriteEmitter22
     AutoDestroy=true
     bNoDelete=false
     bDirectional=true
}
