class s_u806_channeling_mesh extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter15
         // Reference: MeshEmitter'lineageeffect.dec.s_u806_channeling_mesh.MeshEmitter15'
         begin object name="MeshEmitter15" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_rapidshot'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5571430,Color=(R=102,G=102,B=102,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=40.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.6000000,Max=1.0000000),Z=(Min=0.6000000,Max=1.0000000))
         Opacity=0.7000000
         FadeOutStartTime=13.3500004
         FadeOut=true
         FadeInEndTime=0.1500000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="MeshEmitter15"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.2000000,Max=0.2000000),Z=(Min=0.2000000,Max=0.2000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=15.0000000,Max=15.0000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter15'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter18
         // Reference: SpriteEmitter'lineageeffect.dec.s_u806_channeling_mesh.SpriteEmitter18'
         begin object name="SpriteEmitter18" class=Engine.SpriteEmitter
         Acceleration=(X=16.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=249,G=217,B=164,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.9160000,Max=0.9160000),Y=(Min=0.9440000,Max=0.9440000),Z=(Min=0.8300000,Max=0.8300000))
         Opacity=0.2800000
         FadeOutStartTime=0.8100000
         FadeOut=true
         FadeInEndTime=0.1200000
         FadeIn=true
         MaxParticles=65
         RespawnDeadParticles=false
         Name="aura1"
         StartLocationRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=15.0000000,Max=15.0000000))
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=0.8000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
         StartSizeRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=16.0000000,Max=16.0000000),Z=(Min=16.0000000,Max=16.0000000))
         UniformSize=true
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0111'
         TextureUSubdivisions=1
         TextureVSubdivisions=2
         SubdivisionEnd=2
         UseRandomSubdivision=true
         LifetimeRange=(Min=1.3000000,Max=1.5000000)
         StartVelocityRange=(X=(Min=8.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter18'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter19
         // Reference: SpriteEmitter'lineageeffect.dec.s_u806_channeling_mesh.SpriteEmitter19'
         begin object name="SpriteEmitter19" class=Engine.SpriteEmitter
         Acceleration=(X=16.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=249,G=217,B=164,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.9690000,Max=0.9690000),Y=(Min=0.7670000,Max=0.7670000),Z=(Min=0.6260000,Max=0.6260000))
         Opacity=0.4200000
         FadeOutStartTime=0.7500000
         FadeOut=true
         FadeInEndTime=0.1200000
         FadeIn=true
         MaxParticles=65
         RespawnDeadParticles=false
         Name="aura2"
         StartLocationRange=(X=(Min=-3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=15.0000000,Max=15.0000000))
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=0.8000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
         StartSizeRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=16.0000000,Max=16.0000000),Z=(Min=16.0000000,Max=16.0000000))
         UniformSize=true
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles3.fx_m_t5011'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=true
         SubdivisionEnd=4
         LifetimeRange=(Min=1.3000000,Max=1.5000000)
         StartVelocityRange=(X=(Min=8.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter19'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter22
         // Reference: SpriteEmitter'lineageeffect.dec.s_u806_channeling_mesh.SpriteEmitter22'
         begin object name="SpriteEmitter22" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         MaxParticles=1
         WeatherSoundCheck=true
         Name="SpriteEmitter22"
         UniformSize=true
         InitialParticlesPerSecond=1.0000000
         AutomaticInitialSpawning=false
         Texture=none
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter22'
     // Reference: MeshEmitter'lineageeffect.dec.s_u806_channeling_mesh.MeshEmitter15'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_rapidshot'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5571430,Color=(R=102,G=102,B=102,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=40.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.6000000,Max=1.0000000),Z=(Min=0.6000000,Max=1.0000000))
     Opacity=0.7000000
     FadeOutStartTime=13.3500004
     FadeOut=true
     FadeInEndTime=0.1500000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="MeshEmitter15"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.2000000,Max=0.2000000),Z=(Min=0.2000000,Max=0.2000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=15.0000000,Max=15.0000000)
     Emitters[0]=MeshEmitter15
     // Reference: SpriteEmitter'lineageeffect.dec.s_u806_channeling_mesh.SpriteEmitter18'
     Acceleration=(X=16.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=249,G=217,B=164,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.9160000,Max=0.9160000),Y=(Min=0.9440000,Max=0.9440000),Z=(Min=0.8300000,Max=0.8300000))
     Opacity=0.2800000
     FadeOutStartTime=0.8100000
     FadeOut=true
     FadeInEndTime=0.1200000
     FadeIn=true
     MaxParticles=65
     RespawnDeadParticles=false
     Name="aura1"
     StartLocationRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=15.0000000,Max=15.0000000))
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=0.8000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
     StartSizeRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=16.0000000,Max=16.0000000),Z=(Min=16.0000000,Max=16.0000000))
     UniformSize=true
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0111'
     TextureUSubdivisions=1
     TextureVSubdivisions=2
     SubdivisionEnd=2
     UseRandomSubdivision=true
     LifetimeRange=(Min=1.3000000,Max=1.5000000)
     StartVelocityRange=(X=(Min=8.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[1]=SpriteEmitter18
     // Reference: SpriteEmitter'lineageeffect.dec.s_u806_channeling_mesh.SpriteEmitter19'
     Acceleration=(X=16.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=249,G=217,B=164,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.9690000,Max=0.9690000),Y=(Min=0.7670000,Max=0.7670000),Z=(Min=0.6260000,Max=0.6260000))
     Opacity=0.4200000
     FadeOutStartTime=0.7500000
     FadeOut=true
     FadeInEndTime=0.1200000
     FadeIn=true
     MaxParticles=65
     RespawnDeadParticles=false
     Name="aura2"
     StartLocationRange=(X=(Min=-3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=90.0000000,Max=90.0000000),Z=(Min=15.0000000,Max=15.0000000))
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=0.8000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
     StartSizeRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=16.0000000,Max=16.0000000),Z=(Min=16.0000000,Max=16.0000000))
     UniformSize=true
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles3.fx_m_t5011'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     BlendBetweenSubdivisions=true
     SubdivisionEnd=4
     LifetimeRange=(Min=1.3000000,Max=1.5000000)
     StartVelocityRange=(X=(Min=8.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[2]=SpriteEmitter19
     // Reference: SpriteEmitter'lineageeffect.dec.s_u806_channeling_mesh.SpriteEmitter22'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     MaxParticles=1
     WeatherSoundCheck=true
     Name="SpriteEmitter22"
     UniformSize=true
     InitialParticlesPerSecond=1.0000000
     AutomaticInitialSpawning=false
     Texture=none
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     Emitters[3]=SpriteEmitter22
     AutoDestroy=true
     bLightChanged=true
     bNoDelete=false
     bSunAffect=true
     DrawScale=0.1000000
}
