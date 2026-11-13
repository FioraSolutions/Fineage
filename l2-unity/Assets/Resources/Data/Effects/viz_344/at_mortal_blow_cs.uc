class at_mortal_blow_cs extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.at_mortal_blow_cs.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
         FadeOutStartTime=0.4800000
         FadeOut=true
         RespawnDeadParticles=false
         Name="Particle"
         StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.3000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         StartSizeRange=(X=(Min=2.0000000,Max=3.0000000),Y=(Min=20.0000000,Max=30.0000000),Z=(Min=2.0000000,Max=3.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=14
         SubdivisionEnd=16
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.7000000,Max=1.2000000)
         StartVelocityRange=(X=(Min=100.0000000,Max=300.0000000),Y=(Min=-200.0000000,Max=200.0000000),Z=(Min=-200.0000000,Max=200.0000000))
         VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=MeshEmitter Name=MeshEmitter0
         // Reference: MeshEmitter'lineageeffect.dec.at_mortal_blow_cs.MeshEmitter0'
         begin object name="MeshEmitter0" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=120.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.6000000
         FadeOutStartTime=0.1200000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Slash"
         StartLocationOffset=(X=-20.0000000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         UseSizeScale=true
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=0.1600000,Max=0.1600000),Y=(Min=0.1500000,Max=0.1500000),Z=(Min=0.1500000,Max=0.1500000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter0'
     Begin Object Class=MeshEmitter Name=MeshEmitter1
         // Reference: MeshEmitter'lineageeffect.dec.at_mortal_blow_cs.MeshEmitter1'
         begin object name="MeshEmitter1" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power04'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.0642860,Color=(R=55,G=153,B=200,A=255))
         ColorScale[2]=(RelativeTime=0.3178570,Color=(R=49,G=49,B=153,A=255))
         ColorScale[3]=(RelativeTime=0.6071430,Color=(R=43,G=43,B=104,A=255))
         ColorScale[4]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         Opacity=0.4000000
         FadeOutStartTime=0.4000000
         MaxParticles=4
         RespawnDeadParticles=false
         Name="MeshWave"
         StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=2.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0600000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.1900000,RelativeSize=2.5000000)
         SizeScale[2]=(RelativeTime=0.3700000,RelativeSize=3.0000000)
         SizeScale[3]=(RelativeTime=0.6600000,RelativeSize=3.5000000)
         SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=4.0000000)
         StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.0700000,Max=0.2000000))
         UniformSize=true
         InitialParticlesPerSecond=30.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3500000,Max=0.4000000)
         StartVelocityRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter1'
     // Reference: SpriteEmitter'lineageeffect.dec.at_mortal_blow_cs.SpriteEmitter0'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
     FadeOutStartTime=0.4800000
     FadeOut=true
     RespawnDeadParticles=false
     Name="Particle"
     StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.3000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     StartSizeRange=(X=(Min=2.0000000,Max=3.0000000),Y=(Min=20.0000000,Max=30.0000000),Z=(Min=2.0000000,Max=3.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=14
     SubdivisionEnd=16
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.7000000,Max=1.2000000)
     StartVelocityRange=(X=(Min=100.0000000,Max=300.0000000),Y=(Min=-200.0000000,Max=200.0000000),Z=(Min=-200.0000000,Max=200.0000000))
     VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
     Emitters[0]=SpriteEmitter0
     // Reference: MeshEmitter'lineageeffect.dec.at_mortal_blow_cs.MeshEmitter0'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=120.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.6000000
     FadeOutStartTime=0.1200000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Slash"
     StartLocationOffset=(X=-20.0000000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     UseSizeScale=true
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=0.1600000,Max=0.1600000),Y=(Min=0.1500000,Max=0.1500000),Z=(Min=0.1500000,Max=0.1500000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[1]=MeshEmitter0
     // Reference: MeshEmitter'lineageeffect.dec.at_mortal_blow_cs.MeshEmitter1'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power04'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.0642860,Color=(R=55,G=153,B=200,A=255))
     ColorScale[2]=(RelativeTime=0.3178570,Color=(R=49,G=49,B=153,A=255))
     ColorScale[3]=(RelativeTime=0.6071430,Color=(R=43,G=43,B=104,A=255))
     ColorScale[4]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     Opacity=0.4000000
     FadeOutStartTime=0.4000000
     MaxParticles=4
     RespawnDeadParticles=false
     Name="MeshWave"
     StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=2.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0600000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.1900000,RelativeSize=2.5000000)
     SizeScale[2]=(RelativeTime=0.3700000,RelativeSize=3.0000000)
     SizeScale[3]=(RelativeTime=0.6600000,RelativeSize=3.5000000)
     SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=4.0000000)
     StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.0700000,Max=0.2000000))
     UniformSize=true
     InitialParticlesPerSecond=30.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3500000,Max=0.4000000)
     StartVelocityRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[2]=MeshEmitter1
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.0500000
     bDirectional=true
}
