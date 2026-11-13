class el_hurricane_fl extends NSkillProjectile;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter3
         // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_fl.SpriteEmitter3'
         begin object name="SpriteEmitter3" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.1714290,Color=(R=255,G=255,B=255,A=255))
         ColorScale[2]=(RelativeTime=0.3321430,Color=(R=128,G=128,B=128,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorMultiplierRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=5.0000000
         MaxParticles=2
         RespawnDeadParticles=false
         Name="Core"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=17.6000004,Max=17.6000004),Y=(Min=64.0000000,Max=64.0000000),Z=(Min=64.0000000,Max=64.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=5
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionEnd=1
         UseRandomSubdivision=true
         MaxAbsVelocity=(X=6400.0000000,Y=10000.0000000,Z=8000.0000000)
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter3'
     Begin Object Class=MeshEmitter Name=MeshEmitter3
         // Reference: MeshEmitter'lineageeffect.dec.el_hurricane_fl.MeshEmitter3'
         begin object name="MeshEmitter3" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windblowin03'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.8000000
         FadeOutFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
         FadeOutStartTime=6.0000000
         FadeInFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
         MaxParticles=1
         ResetAfterChange=true
         RespawnDeadParticles=false
         Name="Wind"
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=0.0800000,Max=0.0800000),Y=(Min=0.0800000,Max=0.0800000),Z=(Min=0.0800000,Max=0.0800000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0001'
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter3'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter10
         // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_fl.SpriteEmitter10'
         begin object name="SpriteEmitter10" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.7000000,Max=0.7000000),Y=(Min=0.7000000,Max=0.7000000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.1800000
         FadeOutStartTime=5.0000000
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Aura"
         StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.4000000,Max=0.4000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=4.0000000)
         SizeScale[2]=(RelativeTime=0.6600000,RelativeSize=6.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=7.0000000)
         StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=12
         SubdivisionEnd=13
         LifetimeRange=(Min=3.8000000,Max=3.8000000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter10'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter12
         // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_fl.SpriteEmitter12'
         begin object name="SpriteEmitter12" class=Engine.SpriteEmitter
         UseDirectionAs=3
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.6000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=4.0000000
         MaxParticles=6
         RespawnDeadParticles=false
         Name="FlowIn"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
         StartSizeRange=(X=(Min=12.0000000,Max=40.0000000),Y=(Min=12.0000000,Max=40.0000000),Z=(Min=12.0000000,Max=40.0000000))
         UniformSize=true
         InitialParticlesPerSecond=100000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=2
         SubdivisionEnd=3
         UseRandomSubdivision=true
         StartVelocityRange=(X=(Min=-0.0240000,Max=0.0240000),Y=(Min=-0.0240000,Max=0.0240000),Z=(Min=-0.0800000,Max=-0.0800000))
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter12'
     Speed=1000.0000000
     AccSpeed=3000.0000000
     // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_fl.SpriteEmitter3'
     UseDirectionAs=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.1714290,Color=(R=255,G=255,B=255,A=255))
     ColorScale[2]=(RelativeTime=0.3321430,Color=(R=128,G=128,B=128,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorMultiplierRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=5.0000000
     MaxParticles=2
     RespawnDeadParticles=false
     Name="Core"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=17.6000004,Max=17.6000004),Y=(Min=64.0000000,Max=64.0000000),Z=(Min=64.0000000,Max=64.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=5
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionEnd=1
     UseRandomSubdivision=true
     MaxAbsVelocity=(X=6400.0000000,Y=10000.0000000,Z=8000.0000000)
     Emitters[0]=SpriteEmitter3
     // Reference: MeshEmitter'lineageeffect.dec.el_hurricane_fl.MeshEmitter3'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windblowin03'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.8000000
     FadeOutFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
     FadeOutStartTime=6.0000000
     FadeInFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
     MaxParticles=1
     ResetAfterChange=true
     RespawnDeadParticles=false
     Name="Wind"
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=0.0800000,Max=0.0800000),Y=(Min=0.0800000,Max=0.0800000),Z=(Min=0.0800000,Max=0.0800000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0001'
     Emitters[1]=MeshEmitter3
     // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_fl.SpriteEmitter10'
     UseDirectionAs=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.7000000,Max=0.7000000),Y=(Min=0.7000000,Max=0.7000000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.1800000
     FadeOutStartTime=5.0000000
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Aura"
     StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.4000000,Max=0.4000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=4.0000000)
     SizeScale[2]=(RelativeTime=0.6600000,RelativeSize=6.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=7.0000000)
     StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=12
     SubdivisionEnd=13
     LifetimeRange=(Min=3.8000000,Max=3.8000000)
     Emitters[2]=SpriteEmitter10
     // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_fl.SpriteEmitter12'
     UseDirectionAs=3
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.6000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=4.0000000
     MaxParticles=6
     RespawnDeadParticles=false
     Name="FlowIn"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
     StartSizeRange=(X=(Min=12.0000000,Max=40.0000000),Y=(Min=12.0000000,Max=40.0000000),Z=(Min=12.0000000,Max=40.0000000))
     UniformSize=true
     InitialParticlesPerSecond=100000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionStart=2
     SubdivisionEnd=3
     UseRandomSubdivision=true
     StartVelocityRange=(X=(Min=-0.0240000,Max=0.0240000),Y=(Min=-0.0240000,Max=0.0240000),Z=(Min=-0.0800000,Max=-0.0800000))
     Emitters[3]=SpriteEmitter12
     AutoDestroy=true
     RotPerSecond=(Pitch=0,Yaw=1000,Roll=0)
     bUseDynamicLights=false
     bAcceptsProjectors=false
     DrawScale=0.0500000
     bDirectional=true
}
