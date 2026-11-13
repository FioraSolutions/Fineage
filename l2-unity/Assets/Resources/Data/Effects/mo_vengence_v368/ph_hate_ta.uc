class ph_hate_ta extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.ph_hate_ta.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.7700000,Max=0.7700000))
         Opacity=0.6000000
         FadeOutStartTime=0.3750000
         FadeOut=true
         FadeInEndTime=0.0150000
         FadeIn=true
         MaxParticles=15
         RespawnDeadParticles=false
         Name="smoke"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=6.0000000,Max=8.0000000),Y=(Min=6.0000000,Max=8.0000000),Z=(Min=6.0000000,Max=8.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         SubdivisionEnd=7
         UseRandomSubdivision=true
         InitialTimeRange=(Min=0.0000000,Max=0.2000000)
         LifetimeRange=(Min=2.0000000,Max=3.0000000)
         StartVelocityRange=(X=(Min=-30.0000000,Max=30.0000000),Y=(Min=-30.0000000,Max=30.0000000),Z=(Min=-30.0000000,Max=30.0000000))
         VelocityLossRange=(X=(Min=5.5000000,Max=5.5000000),Y=(Min=5.5000000,Max=5.5000000),Z=(Min=5.5000000,Max=5.5000000))
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.ph_hate_ta.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=243,G=96,B=7,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.1140000
         FadeOut=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="Spark"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
         StartLocationShape=2
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=0.2900000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.5000000)
         StartSizeRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=10.0000000,Max=10.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1003'
         BlendBetweenSubdivisions=true
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter1
         // Reference: MeshEmitter'lineageeffect.dec.ph_hate_ta.MeshEmitter1'
         begin object name="MeshEmitter1" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.6020000,Max=0.6020000),Z=(Min=0.4140000,Max=0.4140000))
         Opacity=0.6000000
         FadeOutStartTime=0.1600000
         FadeOut=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="Beamlight"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.3000000)
         SizeScale[1]=(RelativeTime=0.5100000,RelativeSize=0.8000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.1500000,Max=0.1500000),Z=(Min=0.1500000,Max=0.1500000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter3
         // Reference: SpriteEmitter'lineageeffect.dec.ph_hate_ta.SpriteEmitter3'
         begin object name="SpriteEmitter3" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.4670000,Max=0.4670000),Z=(Min=0.0000000,Max=0.0000000))
         Opacity=0.4000000
         FadeOutStartTime=0.0700000
         FadeOut=true
         RespawnDeadParticles=false
         Name="Redsmoke"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
         StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
         StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures2.balakas.fx_m_t2011'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionEnd=4
         UseRandomSubdivision=true
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         StartVelocityRange=(X=(Min=-4.0000000,Max=4.0000000),Y=(Min=-4.0000000,Max=4.0000000),Z=(Min=-4.0000000,Max=4.0000000))
         VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter3'
     // Reference: SpriteEmitter'lineageeffect.dec.ph_hate_ta.SpriteEmitter0'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=0.7700000,Max=0.7700000))
     Opacity=0.6000000
     FadeOutStartTime=0.3750000
     FadeOut=true
     FadeInEndTime=0.0150000
     FadeIn=true
     MaxParticles=15
     RespawnDeadParticles=false
     Name="smoke"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=6.0000000,Max=8.0000000),Y=(Min=6.0000000,Max=8.0000000),Z=(Min=6.0000000,Max=8.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     SubdivisionEnd=7
     UseRandomSubdivision=true
     InitialTimeRange=(Min=0.0000000,Max=0.2000000)
     LifetimeRange=(Min=2.0000000,Max=3.0000000)
     StartVelocityRange=(X=(Min=-30.0000000,Max=30.0000000),Y=(Min=-30.0000000,Max=30.0000000),Z=(Min=-30.0000000,Max=30.0000000))
     VelocityLossRange=(X=(Min=5.5000000,Max=5.5000000),Y=(Min=5.5000000,Max=5.5000000),Z=(Min=5.5000000,Max=5.5000000))
     Emitters[0]=SpriteEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.ph_hate_ta.SpriteEmitter1'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=243,G=96,B=7,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.1140000
     FadeOut=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="Spark"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
     StartLocationShape=2
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=0.2900000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.5000000)
     StartSizeRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=10.0000000,Max=10.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1003'
     BlendBetweenSubdivisions=true
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     Emitters[1]=SpriteEmitter1
     // Reference: MeshEmitter'lineageeffect.dec.ph_hate_ta.MeshEmitter1'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.6020000,Max=0.6020000),Z=(Min=0.4140000,Max=0.4140000))
     Opacity=0.6000000
     FadeOutStartTime=0.1600000
     FadeOut=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="Beamlight"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.3000000)
     SizeScale[1]=(RelativeTime=0.5100000,RelativeSize=0.8000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.1500000,Max=0.1500000),Z=(Min=0.1500000,Max=0.1500000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[2]=MeshEmitter1
     // Reference: SpriteEmitter'lineageeffect.dec.ph_hate_ta.SpriteEmitter3'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.4670000,Max=0.4670000),Z=(Min=0.0000000,Max=0.0000000))
     Opacity=0.4000000
     FadeOutStartTime=0.0700000
     FadeOut=true
     RespawnDeadParticles=false
     Name="Redsmoke"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
     StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
     StartSizeRange=(X=(Min=8.0000000,Max=8.0000000),Y=(Min=8.0000000,Max=8.0000000),Z=(Min=8.0000000,Max=8.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures2.balakas.fx_m_t2011'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionEnd=4
     UseRandomSubdivision=true
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     StartVelocityRange=(X=(Min=-4.0000000,Max=4.0000000),Y=(Min=-4.0000000,Max=4.0000000),Z=(Min=-4.0000000,Max=4.0000000))
     VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
     Emitters[3]=SpriteEmitter3
     AutoDestroy=true
     bNoDelete=false
     bSunAffect=true
     bDirectional=true
}
