class sa_mass_frenzy_cs extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter10
         // Reference: SpriteEmitter'lineageeffect.dec.sa_mass_frenzy_cs.SpriteEmitter10'
         begin object name="SpriteEmitter10" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.8000000
         FadeOutStartTime=0.1500000
         FadeOut=true
         MaxParticles=15
         RespawnDeadParticles=false
         Name="Ticks"
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=10.0000000,Max=10.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.2000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         StartSizeRange=(X=(Min=2.0000000,Max=3.0000000),Y=(Min=2.0000000,Max=3.0000000),Z=(Min=2.0000000,Max=3.0000000))
         UniformSize=true
         InitialParticlesPerSecond=100.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
         TextureUSubdivisions=4
         TextureVSubdivisions=8
         SubdivisionEnd=4
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=200.0000000,Max=200.0000000))
         VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=4.0000000,Max=4.0000000),Z=(Min=4.0000000,Max=4.0000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter10'
     Begin Object Class=MeshEmitter Name=MeshEmitter11
         // Reference: MeshEmitter'lineageeffect.dec.sa_mass_frenzy_cs.MeshEmitter11'
         begin object name="MeshEmitter11" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.1285710,Color=(R=255,G=255,B=255,A=255))
         ColorScale[2]=(RelativeTime=0.4250000,Color=(R=128,G=128,B=128,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7630000,Max=0.7630000),Z=(Min=0.5300000,Max=0.5300000))
         FadeOutStartTime=0.3000000
         MaxParticles=5
         RespawnDeadParticles=false
         Name="NeedleOut"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=6.0000000)
         StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.1500000,Max=0.1500000),Z=(Min=0.0800000,Max=0.0800000))
         InitialParticlesPerSecond=50.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.2500000,Max=0.2500000)
         VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=4.0000000,Max=4.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter11'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter11
         // Reference: SpriteEmitter'lineageeffect.dec.sa_mass_frenzy_cs.SpriteEmitter11'
         begin object name="SpriteEmitter11" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8710000,Max=0.8710000),Z=(Min=0.6550000,Max=0.6550000))
         Opacity=0.6000000
         FadeOutStartTime=0.0400000
         FadeOut=true
         FadeInEndTime=0.0140000
         FadeIn=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Rings"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=4.0000000)
         SizeScale[2]=(RelativeTime=0.6600000,RelativeSize=6.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=7.0000000)
         StartSizeRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
         UniformSize=true
         InitialParticlesPerSecond=30.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionEnd=1
         LifetimeRange=(Min=0.2000000,Max=0.2000000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter11'
     // Reference: SpriteEmitter'lineageeffect.dec.sa_mass_frenzy_cs.SpriteEmitter10'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.8000000
     FadeOutStartTime=0.1500000
     FadeOut=true
     MaxParticles=15
     RespawnDeadParticles=false
     Name="Ticks"
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=80.0000000,Max=100.0000000),Z=(Min=10.0000000,Max=10.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.2000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     StartSizeRange=(X=(Min=2.0000000,Max=3.0000000),Y=(Min=2.0000000,Max=3.0000000),Z=(Min=2.0000000,Max=3.0000000))
     UniformSize=true
     InitialParticlesPerSecond=100.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
     TextureUSubdivisions=4
     TextureVSubdivisions=8
     SubdivisionEnd=4
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=200.0000000,Max=200.0000000))
     VelocityLossRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=4.0000000,Max=4.0000000),Z=(Min=4.0000000,Max=4.0000000))
     GetVelocityDirectionFrom=2
     Emitters[0]=SpriteEmitter10
     // Reference: MeshEmitter'lineageeffect.dec.sa_mass_frenzy_cs.MeshEmitter11'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.1285710,Color=(R=255,G=255,B=255,A=255))
     ColorScale[2]=(RelativeTime=0.4250000,Color=(R=128,G=128,B=128,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7630000,Max=0.7630000),Z=(Min=0.5300000,Max=0.5300000))
     FadeOutStartTime=0.3000000
     MaxParticles=5
     RespawnDeadParticles=false
     Name="NeedleOut"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=6.0000000)
     StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.1500000,Max=0.1500000),Z=(Min=0.0800000,Max=0.0800000))
     InitialParticlesPerSecond=50.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.2500000,Max=0.2500000)
     VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=4.0000000,Max=4.0000000))
     Emitters[1]=MeshEmitter11
     // Reference: SpriteEmitter'lineageeffect.dec.sa_mass_frenzy_cs.SpriteEmitter11'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8710000,Max=0.8710000),Z=(Min=0.6550000,Max=0.6550000))
     Opacity=0.6000000
     FadeOutStartTime=0.0400000
     FadeOut=true
     FadeInEndTime=0.0140000
     FadeIn=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Rings"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0800000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.2700000,RelativeSize=4.0000000)
     SizeScale[2]=(RelativeTime=0.6600000,RelativeSize=6.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=7.0000000)
     StartSizeRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
     UniformSize=true
     InitialParticlesPerSecond=30.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionEnd=1
     LifetimeRange=(Min=0.2000000,Max=0.2000000)
     Emitters[2]=SpriteEmitter11
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.1000000
     bDirectional=true
}
