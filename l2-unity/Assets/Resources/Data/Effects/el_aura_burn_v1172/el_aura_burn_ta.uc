class el_aura_burn_ta extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter15
         // Reference: SpriteEmitter'lineageeffect.dec.el_aura_burn_ta.SpriteEmitter15'
         begin object name="SpriteEmitter15" class=Engine.SpriteEmitter
         Acceleration=(X=20.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5107140,Color=(R=182,G=182,B=182,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=10.0000000
         FadeOutStartTime=0.0660000
         FadeOut=true
         FadeInEndTime=0.0480000
         FadeIn=true
         MaxParticles=4
         RespawnDeadParticles=false
         Name="Particle"
         SpinsPerSecondRange=(X=(Min=-0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=1.7000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=2.2000000)
         SizeScale[2]=(RelativeTime=0.4300000,RelativeSize=2.5999999)
         SizeScale[3]=(RelativeTime=0.7700000,RelativeSize=2.9000001)
         SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
         StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0069'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         SubdivisionStart=5
         SubdivisionEnd=7
         UseRandomSubdivision=true
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         StartVelocityRange=(X=(Min=10.0000000,Max=60.0000000),Y=(Min=-80.0000000,Max=80.0000000),Z=(Min=-80.0000000,Max=80.0000000))
         VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=4.0000000,Max=4.0000000),Z=(Min=4.0000000,Max=4.0000000))
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter15'
     Begin Object Class=MeshEmitter Name=MeshEmitter18
         // Reference: MeshEmitter'lineageeffect.dec.el_aura_burn_ta.MeshEmitter18'
         begin object name="MeshEmitter18" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.5200000
         FadeOutStartTime=0.0900000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Wave"
         StartLocationOffset=(X=-7.0000000,Y=0.0000000,Z=0.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0900000,RelativeSize=1.8000000)
         SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=2.2000000)
         SizeScale[2]=(RelativeTime=0.5600000,RelativeSize=2.5000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.8000000)
         StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
         InitialParticlesPerSecond=13.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter18'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter16
         // Reference: SpriteEmitter'lineageeffect.dec.el_aura_burn_ta.SpriteEmitter16'
         begin object name="SpriteEmitter16" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.3430000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Core"
         StartLocationOffset=(X=-5.0000000,Y=0.0000000,Z=0.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=1.2000000)
         SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=0.9000000)
         SizeScale[2]=(RelativeTime=0.5400000,RelativeSize=0.8000000)
         SizeScale[3]=(RelativeTime=0.8400000,RelativeSize=0.6000000)
         SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
         StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=3
         SubdivisionEnd=4
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         StartVelocityRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter16'
     Begin Object Class=MeshEmitter Name=MeshEmitter19
         // Reference: MeshEmitter'lineageeffect.dec.el_aura_burn_ta.MeshEmitter19'
         begin object name="MeshEmitter19" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Aura.auraburn00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.0650000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="AuraFlow"
         StartLocationOffset=(X=-5.0000000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.0500000,Max=0.0500000))
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=1.7000000)
         SizeScale[1]=(RelativeTime=0.2200000,RelativeSize=2.0999999)
         SizeScale[2]=(RelativeTime=0.5500000,RelativeSize=2.4000001)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.8000000)
         StartSizeRange=(X=(Min=0.0700000,Max=0.0700000),Y=(Min=0.0700000,Max=0.0700000),Z=(Min=0.0700000,Max=0.0700000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         end object
     End Object
     Emitters(3)=MeshEmitter'MeshEmitter19'
     // Reference: SpriteEmitter'lineageeffect.dec.el_aura_burn_ta.SpriteEmitter15'
     Acceleration=(X=20.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5107140,Color=(R=182,G=182,B=182,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=10.0000000
     FadeOutStartTime=0.0660000
     FadeOut=true
     FadeInEndTime=0.0480000
     FadeIn=true
     MaxParticles=4
     RespawnDeadParticles=false
     Name="Particle"
     SpinsPerSecondRange=(X=(Min=-0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=1.7000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=2.2000000)
     SizeScale[2]=(RelativeTime=0.4300000,RelativeSize=2.5999999)
     SizeScale[3]=(RelativeTime=0.7700000,RelativeSize=2.9000001)
     SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
     StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0069'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     SubdivisionStart=5
     SubdivisionEnd=7
     UseRandomSubdivision=true
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     StartVelocityRange=(X=(Min=10.0000000,Max=60.0000000),Y=(Min=-80.0000000,Max=80.0000000),Z=(Min=-80.0000000,Max=80.0000000))
     VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=4.0000000,Max=4.0000000),Z=(Min=4.0000000,Max=4.0000000))
     Emitters[0]=SpriteEmitter15
     // Reference: MeshEmitter'lineageeffect.dec.el_aura_burn_ta.MeshEmitter18'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.5200000
     FadeOutStartTime=0.0900000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Wave"
     StartLocationOffset=(X=-7.0000000,Y=0.0000000,Z=0.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0900000,RelativeSize=1.8000000)
     SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=2.2000000)
     SizeScale[2]=(RelativeTime=0.5600000,RelativeSize=2.5000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.8000000)
     StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
     InitialParticlesPerSecond=13.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[1]=MeshEmitter18
     // Reference: SpriteEmitter'lineageeffect.dec.el_aura_burn_ta.SpriteEmitter16'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.3430000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Core"
     StartLocationOffset=(X=-5.0000000,Y=0.0000000,Z=0.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=1.2000000)
     SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=0.9000000)
     SizeScale[2]=(RelativeTime=0.5400000,RelativeSize=0.8000000)
     SizeScale[3]=(RelativeTime=0.8400000,RelativeSize=0.6000000)
     SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=0.1000000)
     StartSizeRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=20.0000000,Max=20.0000000),Z=(Min=20.0000000,Max=20.0000000))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=3
     SubdivisionEnd=4
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     StartVelocityRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[2]=SpriteEmitter16
     // Reference: MeshEmitter'lineageeffect.dec.el_aura_burn_ta.MeshEmitter19'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Aura.auraburn00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.0650000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="AuraFlow"
     StartLocationOffset=(X=-5.0000000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-0.0500000,Max=0.0500000))
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=1.7000000)
     SizeScale[1]=(RelativeTime=0.2200000,RelativeSize=2.0999999)
     SizeScale[2]=(RelativeTime=0.5500000,RelativeSize=2.4000001)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.8000000)
     StartSizeRange=(X=(Min=0.0700000,Max=0.0700000),Y=(Min=0.0700000,Max=0.0700000),Z=(Min=0.0700000,Max=0.0700000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     Emitters[3]=MeshEmitter19
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.0500000
     bDirectional=true
}
