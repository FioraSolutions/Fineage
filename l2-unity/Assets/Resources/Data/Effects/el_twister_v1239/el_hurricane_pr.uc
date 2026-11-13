class el_hurricane_pr extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_pr.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.1714290,Color=(R=255,G=255,B=255,A=255))
         ColorScale[2]=(RelativeTime=0.3321430,Color=(R=128,G=128,B=128,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorMultiplierRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=5.0000000
         MaxParticles=2
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Core"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=10.5600004,Max=10.5600004),Y=(Min=42.2400017,Max=42.2400017),Z=(Min=42.2400017,Max=42.2400017))
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
     Emitters(0)=SpriteEmitter'SpriteEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter0
         // Reference: MeshEmitter'lineageeffect.dec.el_hurricane_pr.MeshEmitter0'
         begin object name="MeshEmitter0" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windblowin03'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.6000000
         FadeOutFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
         FadeOutStartTime=6.0000000
         FadeInFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
         MaxParticles=1
         ResetAfterChange=true
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Wind"
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=0.0528000,Max=0.0528000),Y=(Min=0.0528000,Max=0.0528000),Z=(Min=0.0880000,Max=0.0880000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=1
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0001'
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_pr.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.7000000,Max=0.7000000),Y=(Min=0.7000000,Max=0.7000000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.1800000
         FadeOutStartTime=5.0000000
         MaxParticles=3
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Aura"
         StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.2640000,Max=0.2640000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=4.0000000)
         SizeScale[2]=(RelativeTime=0.6600000,RelativeSize=6.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=7.0000000)
         StartSizeRange=(X=(Min=13.2000017,Max=13.2000017),Y=(Min=13.2000017,Max=13.2000017),Z=(Min=13.2000017,Max=13.2000017))
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
     Emitters(2)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter24
         // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_pr.SpriteEmitter24'
         begin object name="SpriteEmitter24" class=Engine.SpriteEmitter
         UseDirectionAs=3
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.6000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.2000000
         FadeOut=true
         FadeInEndTime=0.1000000
         FadeIn=true
         MaxParticles=28
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="FlowIn"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
         StartSizeRange=(X=(Min=35.2000008,Max=44.0000000),Y=(Min=35.2000008,Max=44.0000000),Z=(Min=35.2000008,Max=44.0000000))
         UniformSize=true
         InitialParticlesPerSecond=8.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=2
         SubdivisionEnd=3
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         StartVelocityRange=(X=(Min=-0.0158400,Max=0.0158400),Y=(Min=-0.0158400,Max=0.0158400),Z=(Min=-0.0528000,Max=-0.0528000))
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter24'
     // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_pr.SpriteEmitter1'
     UseDirectionAs=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.1714290,Color=(R=255,G=255,B=255,A=255))
     ColorScale[2]=(RelativeTime=0.3321430,Color=(R=128,G=128,B=128,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorMultiplierRange=(X=(Min=0.8000000,Max=0.8000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=5.0000000
     MaxParticles=2
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Core"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=10.5600004,Max=10.5600004),Y=(Min=42.2400017,Max=42.2400017),Z=(Min=42.2400017,Max=42.2400017))
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
     Emitters[0]=SpriteEmitter1
     // Reference: MeshEmitter'lineageeffect.dec.el_hurricane_pr.MeshEmitter0'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windblowin03'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.6000000
     FadeOutFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
     FadeOutStartTime=6.0000000
     FadeInFactor=(W=1.0000000,X=0.3000000,Y=0.3000000,Z=0.3000000)
     MaxParticles=1
     ResetAfterChange=true
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Wind"
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=0.0528000,Max=0.0528000),Y=(Min=0.0528000,Max=0.0528000),Z=(Min=0.0880000,Max=0.0880000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=1
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0001'
     Emitters[1]=MeshEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_pr.SpriteEmitter0'
     UseDirectionAs=4
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.7000000,Max=0.7000000),Y=(Min=0.7000000,Max=0.7000000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.1800000
     FadeOutStartTime=5.0000000
     MaxParticles=3
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Aura"
     StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.2640000,Max=0.2640000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=4.0000000)
     SizeScale[2]=(RelativeTime=0.6600000,RelativeSize=6.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=7.0000000)
     StartSizeRange=(X=(Min=13.2000017,Max=13.2000017),Y=(Min=13.2000017,Max=13.2000017),Z=(Min=13.2000017,Max=13.2000017))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=12
     SubdivisionEnd=13
     LifetimeRange=(Min=3.8000000,Max=3.8000000)
     Emitters[2]=SpriteEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.el_hurricane_pr.SpriteEmitter24'
     UseDirectionAs=3
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.6000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.2000000
     FadeOut=true
     FadeInEndTime=0.1000000
     FadeIn=true
     MaxParticles=28
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="FlowIn"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
     StartSizeRange=(X=(Min=35.2000008,Max=44.0000000),Y=(Min=35.2000008,Max=44.0000000),Z=(Min=35.2000008,Max=44.0000000))
     UniformSize=true
     InitialParticlesPerSecond=8.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionStart=2
     SubdivisionEnd=3
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     StartVelocityRange=(X=(Min=-0.0158400,Max=0.0158400),Y=(Min=-0.0158400,Max=0.0158400),Z=(Min=-0.0528000,Max=-0.0528000))
     Emitters[3]=SpriteEmitter24
     AutoDestroy=true
     RotPerSecond=(Pitch=0,Yaw=1000,Roll=0)
     bNoDelete=false
     DrawScale=0.0500000
     bDirectional=true
}
