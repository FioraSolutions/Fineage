class su_death_whisper_ta extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.su_death_whisper_ta.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.6900000,Max=0.6900000))
         FadeOutStartTime=0.2500000
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Rings"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=25.0000000,Max=25.0000000),Y=(Min=25.0000000,Max=25.0000000),Z=(Min=25.0000000,Max=25.0000000))
         UniformSize=true
         InitialParticlesPerSecond=4.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=true
         SubdivisionStart=4
         SubdivisionEnd=7
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.su_death_whisper_ta.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.6000000
         FadeOutStartTime=0.2075000
         FadeOut=true
         FadeInEndTime=0.0500000
         FadeIn=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Spark"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=25.0000000,Max=25.0000000),Y=(Min=25.0000000,Max=25.0000000),Z=(Min=25.0000000,Max=25.0000000))
         UniformSize=true
         InitialParticlesPerSecond=4.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter0
         // Reference: MeshEmitter'lineageeffect.dec.su_death_whisper_ta.MeshEmitter0'
         begin object name="MeshEmitter0" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.1000000
         FadeOut=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Splashes"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=0.8000000)
         SizeScale[2]=(RelativeTime=0.4100000,RelativeSize=1.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.3000000)
         StartSizeRange=(X=(Min=0.2000000,Max=0.4000000),Y=(Min=0.2000000,Max=0.4000000),Z=(Min=0.2000000,Max=0.4000000))
         InitialParticlesPerSecond=4.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter4
         // Reference: SpriteEmitter'lineageeffect.dec.su_death_whisper_ta.SpriteEmitter4'
         begin object name="SpriteEmitter4" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=90,G=186,B=226,A=255))
         ColorScale[1]=(RelativeTime=0.5678570,Color=(R=223,G=133,B=32,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.4300000
         FadeOutStartTime=0.6700000
         FadeOut=true
         FadeInEndTime=0.2300000
         FadeIn=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Smokes"
         StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=30.0000000,Max=35.0000000),Y=(Min=30.0000000,Max=35.0000000),Z=(Min=30.0000000,Max=35.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=true
         SubdivisionEnd=3
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter4'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.su_death_whisper_ta.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4714290,Color=(R=132,G=132,B=132,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=10.0000000
         ColorMultiplierRange=(X=(Min=0.6040000,Max=0.6040000),Y=(Min=0.7070000,Max=0.7070000),Z=(Min=0.8100000,Max=0.8100000))
         FadeOutStartTime=0.4000000
         FadeOut=true
         RespawnDeadParticles=false
         Name="Paticles"
         StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
         StartLocationShape=1
         SphereRadiusRange=(Min=11.6999998,Max=11.6999998)
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=5.0000000,Max=9.0000000),Y=(Min=5.0000000,Max=9.0000000),Z=(Min=5.0000000,Max=9.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0005'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=6
         SubdivisionEnd=9
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.5010000,Max=1.0000000)
         InitialDelayRange=(Min=0.7500000,Max=0.7500000)
         StartVelocityRange=(X=(Min=350.6419983,Max=350.6419983),Y=(Min=350.6419983,Max=350.6419983),Z=(Min=350.6419983,Max=350.6419983))
         VelocityLossRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
         GetVelocityDirectionFrom=1
         end object
     End Object
     Emitters(4)=SpriteEmitter'SpriteEmitter2'
     // Reference: SpriteEmitter'lineageeffect.dec.su_death_whisper_ta.SpriteEmitter0'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.6900000,Max=0.6900000))
     FadeOutStartTime=0.2500000
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Rings"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=25.0000000,Max=25.0000000),Y=(Min=25.0000000,Max=25.0000000),Z=(Min=25.0000000,Max=25.0000000))
     UniformSize=true
     InitialParticlesPerSecond=4.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     BlendBetweenSubdivisions=true
     SubdivisionStart=4
     SubdivisionEnd=7
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[0]=SpriteEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.su_death_whisper_ta.SpriteEmitter1'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.6000000
     FadeOutStartTime=0.2075000
     FadeOut=true
     FadeInEndTime=0.0500000
     FadeIn=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Spark"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=25.0000000,Max=25.0000000),Y=(Min=25.0000000,Max=25.0000000),Z=(Min=25.0000000,Max=25.0000000))
     UniformSize=true
     InitialParticlesPerSecond=4.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[1]=SpriteEmitter1
     // Reference: MeshEmitter'lineageeffect.dec.su_death_whisper_ta.MeshEmitter0'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.1000000
     FadeOut=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Splashes"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=0.8000000)
     SizeScale[2]=(RelativeTime=0.4100000,RelativeSize=1.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.3000000)
     StartSizeRange=(X=(Min=0.2000000,Max=0.4000000),Y=(Min=0.2000000,Max=0.4000000),Z=(Min=0.2000000,Max=0.4000000))
     InitialParticlesPerSecond=4.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[2]=MeshEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.su_death_whisper_ta.SpriteEmitter4'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=90,G=186,B=226,A=255))
     ColorScale[1]=(RelativeTime=0.5678570,Color=(R=223,G=133,B=32,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.4300000
     FadeOutStartTime=0.6700000
     FadeOut=true
     FadeInEndTime=0.2300000
     FadeIn=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Smokes"
     StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=30.0000000,Max=35.0000000),Y=(Min=30.0000000,Max=35.0000000),Z=(Min=30.0000000,Max=35.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     BlendBetweenSubdivisions=true
     SubdivisionEnd=3
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     Emitters[3]=SpriteEmitter4
     // Reference: SpriteEmitter'lineageeffect.dec.su_death_whisper_ta.SpriteEmitter2'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4714290,Color=(R=132,G=132,B=132,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=10.0000000
     ColorMultiplierRange=(X=(Min=0.6040000,Max=0.6040000),Y=(Min=0.7070000,Max=0.7070000),Z=(Min=0.8100000,Max=0.8100000))
     FadeOutStartTime=0.4000000
     FadeOut=true
     RespawnDeadParticles=false
     Name="Paticles"
     StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
     StartLocationShape=1
     SphereRadiusRange=(Min=11.6999998,Max=11.6999998)
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=5.0000000,Max=9.0000000),Y=(Min=5.0000000,Max=9.0000000),Z=(Min=5.0000000,Max=9.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0005'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=6
     SubdivisionEnd=9
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.5010000,Max=1.0000000)
     InitialDelayRange=(Min=0.7500000,Max=0.7500000)
     StartVelocityRange=(X=(Min=350.6419983,Max=350.6419983),Y=(Min=350.6419983,Max=350.6419983),Z=(Min=350.6419983,Max=350.6419983))
     VelocityLossRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
     GetVelocityDirectionFrom=1
     Emitters[4]=SpriteEmitter2
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.0500000
}
