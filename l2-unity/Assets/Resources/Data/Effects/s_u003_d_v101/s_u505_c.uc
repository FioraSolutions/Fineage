class s_u505_c extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter9
         // Reference: MeshEmitter'lineageeffect.dec.s_u505_c.MeshEmitter9'
         begin object name="MeshEmitter9" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=48,G=62,B=67,A=255))
         ColorMultiplierRange=(X=(Min=0.8930000,Max=0.8930000),Y=(Min=0.8510000,Max=0.8510000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.0240000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="sp"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.5000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.4100000,RelativeSize=1.5000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.8000000)
         StartSizeRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.2500000,Max=0.2500000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter9'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter10
         // Reference: SpriteEmitter'lineageeffect.dec.s_u505_c.SpriteEmitter10'
         begin object name="SpriteEmitter10" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.3857140,Color=(R=185,G=185,B=185,A=255))
         ColorScale[2]=(RelativeTime=0.6750000,Color=(R=142,G=142,B=142,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=90,G=90,B=90,A=255))
         Opacity=0.5400000
         FadeOutStartTime=0.0300000
         FadeOut=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Center"
         StartLocationOffset=(X=8.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationRange=(X=(Min=-3.5999999,Max=3.5999999),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=24.0000000,Max=24.0000000),Y=(Min=24.0000000,Max=24.0000000),Z=(Min=24.0000000,Max=24.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
         LifetimeRange=(Min=0.3010000,Max=0.3010000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter10'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter11
         // Reference: SpriteEmitter'lineageeffect.dec.s_u505_c.SpriteEmitter11'
         begin object name="SpriteEmitter11" class=Engine.SpriteEmitter
         UseDirectionAs=1
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.8700000,Max=0.8700000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.0840000
         FadeOut=true
         RespawnDeadParticles=false
         Name="Needle"
         StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationShape=2
         SphereRadiusRange=(Min=15.0000000,Max=25.0000000)
         StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.1500000,Max=0.7000000),Y=(Min=13.8319998,Max=15.3520002),Z=(Min=0.1500000,Max=0.7000000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=2
         SubdivisionEnd=3
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=130.0000000,Max=160.0000000),Y=(Min=130.0000000,Max=160.0000000),Z=(Min=130.0000000,Max=160.0000000))
         VelocityLossRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=0.5000000,Max=0.5000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter11'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter16
         // Reference: SpriteEmitter'lineageeffect.dec.s_u505_c.SpriteEmitter16'
         begin object name="SpriteEmitter16" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5714290,Color=(R=178,G=178,B=178,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=12.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8500000,Max=0.8500000),Z=(Min=0.7200000,Max=0.7200000))
         FadeOutStartTime=0.2600000
         FadeOut=true
         MaxParticles=16
         RespawnDeadParticles=false
         Name="Particle"
         StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=-140.0000000,Max=140.0000000),Z=(Min=10.0000000,Max=20.0000000))
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
         SizeScaleRepeats=10.0000000
         StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0085'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         BlendBetweenSubdivisions=true
         SubdivisionScale=/* Array type was not detected. */
         SubdivisionEnd=2
         LifetimeRange=(Min=0.6000000,Max=1.2470000)
         StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=200.0000000,Max=200.0000000))
         VelocityLossRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=6.0000000,Max=6.0000000),Z=(Min=6.0000000,Max=6.0000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter16'
     Begin Object Class=MeshEmitter Name=MeshEmitter10
         // Reference: MeshEmitter'lineageeffect.dec.s_u505_c.MeshEmitter10'
         begin object name="MeshEmitter10" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.7142860,Color=(R=145,G=145,B=145,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=12.0000000
         ColorMultiplierRange=(X=(Min=0.7110000,Max=0.7110000),Y=(Min=0.7110000,Max=0.7110000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.4440000
         FadeOut=true
         MaxParticles=9
         RespawnDeadParticles=false
         Name="Bodybolt"
         StartLocationOffset=(X=10.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=10.0000000,Max=10.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.2000000),Y=(Min=0.1000000,Max=0.2000000),Z=(Min=0.1000000,Max=0.2000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.5400000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
         StartSizeRange=(X=(Min=0.2940000,Max=0.5000000),Y=(Min=0.2940000,Max=0.5000000),Z=(Min=0.2940000,Max=0.5000000))
         InitialParticlesPerSecond=30.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.4560000,Max=1.2000000)
         StartVelocityRange=(X=(Min=-33.0000000,Max=33.0000000),Y=(Min=-33.0000000,Max=33.0000000),Z=(Min=-63.0000000,Max=63.0000000))
         VelocityLossRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
         end object
     End Object
     Emitters(4)=MeshEmitter'MeshEmitter10'
     Begin Object Class=MeshEmitter Name=MeshEmitter11
         // Reference: MeshEmitter'lineageeffect.dec.s_u505_c.MeshEmitter11'
         begin object name="MeshEmitter11" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5428570,Color=(R=193,G=193,B=193,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=10.0000000
         ColorMultiplierRange=(X=(Min=0.7200000,Max=0.7200000),Y=(Min=0.7900000,Max=0.7900000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.2100000
         FadeOut=true
         RespawnDeadParticles=false
         Name="MeshEmitter0"
         StartLocationRange=(X=(Min=3.0000000,Max=20.0000000),Y=(Min=-15.0000000,Max=15.0000000),Z=(Min=-15.0000000,Max=10.0000000))
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.4000000,Max=0.5000000),Y=(Min=0.4000000,Max=0.5000000),Z=(Min=0.4000000,Max=0.5000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         StartSizeRange=(X=(Min=0.1500000,Max=0.3000000),Y=(Min=0.1500000,Max=0.3000000),Z=(Min=0.1500000,Max=0.3000000))
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=1.0000000)
         InitialDelayRange=(Min=1.0000000,Max=1.0000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=20.0000000),Y=(Min=-50.0000000,Max=50.0000000),Z=(Min=-50.0000000,Max=50.0000000))
         VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
         end object
     End Object
     Emitters(5)=MeshEmitter'MeshEmitter11'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter17
         // Reference: SpriteEmitter'lineageeffect.dec.s_u505_c.SpriteEmitter17'
         begin object name="SpriteEmitter17" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4964290,Color=(R=221,G=221,B=221,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=17.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.6440000,Max=0.6440000),Z=(Min=0.4580000,Max=0.4580000))
         Opacity=0.4000000
         FadeOutStartTime=0.0300000
         FadeOut=true
         MaxParticles=5
         RespawnDeadParticles=false
         Name="SpriteEmitter9"
         StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-2.0000000,Max=2.0000000),Z=(Min=-2.0000000,Max=2.0000000))
         StartSizeRange=(X=(Min=25.0000000,Max=30.0000000),Y=(Min=25.0000000,Max=30.0000000),Z=(Min=25.0000000,Max=30.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=true
         SubdivisionEnd=3
         LifetimeRange=(Min=1.5000000,Max=1.5000000)
         StartVelocityRange=(X=(Min=80.0000000,Max=80.0000000),Y=(Min=-50.0000000,Max=50.0000000),Z=(Min=-50.0000000,Max=50.0000000))
         VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
         end object
     End Object
     Emitters(6)=SpriteEmitter'SpriteEmitter17'
     // Reference: MeshEmitter'lineageeffect.dec.s_u505_c.MeshEmitter9'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=48,G=62,B=67,A=255))
     ColorMultiplierRange=(X=(Min=0.8930000,Max=0.8930000),Y=(Min=0.8510000,Max=0.8510000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.0240000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="sp"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.5000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.4100000,RelativeSize=1.5000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.8000000)
     StartSizeRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.2500000,Max=0.2500000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[0]=MeshEmitter9
     // Reference: SpriteEmitter'lineageeffect.dec.s_u505_c.SpriteEmitter10'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.3857140,Color=(R=185,G=185,B=185,A=255))
     ColorScale[2]=(RelativeTime=0.6750000,Color=(R=142,G=142,B=142,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=90,G=90,B=90,A=255))
     Opacity=0.5400000
     FadeOutStartTime=0.0300000
     FadeOut=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Center"
     StartLocationOffset=(X=8.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationRange=(X=(Min=-3.5999999,Max=3.5999999),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=24.0000000,Max=24.0000000),Y=(Min=24.0000000,Max=24.0000000),Z=(Min=24.0000000,Max=24.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
     LifetimeRange=(Min=0.3010000,Max=0.3010000)
     Emitters[1]=SpriteEmitter10
     // Reference: SpriteEmitter'lineageeffect.dec.s_u505_c.SpriteEmitter11'
     UseDirectionAs=1
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.8700000,Max=0.8700000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.0840000
     FadeOut=true
     RespawnDeadParticles=false
     Name="Needle"
     StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationShape=2
     SphereRadiusRange=(Min=15.0000000,Max=25.0000000)
     StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.1500000,Max=0.7000000),Y=(Min=13.8319998,Max=15.3520002),Z=(Min=0.1500000,Max=0.7000000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionStart=2
     SubdivisionEnd=3
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=130.0000000,Max=160.0000000),Y=(Min=130.0000000,Max=160.0000000),Z=(Min=130.0000000,Max=160.0000000))
     VelocityLossRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=0.5000000,Max=0.5000000))
     GetVelocityDirectionFrom=2
     Emitters[2]=SpriteEmitter11
     // Reference: SpriteEmitter'lineageeffect.dec.s_u505_c.SpriteEmitter16'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5714290,Color=(R=178,G=178,B=178,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=12.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8500000,Max=0.8500000),Z=(Min=0.7200000,Max=0.7200000))
     FadeOutStartTime=0.2600000
     FadeOut=true
     MaxParticles=16
     RespawnDeadParticles=false
     Name="Particle"
     StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-5.0000000,Max=5.0000000),Z=(Min=-5.0000000,Max=5.0000000))
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=-140.0000000,Max=140.0000000),Z=(Min=10.0000000,Max=20.0000000))
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.2000000,Max=0.2000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.2000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
     SizeScaleRepeats=10.0000000
     StartSizeRange=(X=(Min=3.0000000,Max=5.0000000),Y=(Min=3.0000000,Max=5.0000000),Z=(Min=3.0000000,Max=5.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0085'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     BlendBetweenSubdivisions=true
     SubdivisionScale=/* Array type was not detected. */
     SubdivisionEnd=2
     LifetimeRange=(Min=0.6000000,Max=1.2470000)
     StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=200.0000000,Max=200.0000000))
     VelocityLossRange=(X=(Min=6.0000000,Max=6.0000000),Y=(Min=6.0000000,Max=6.0000000),Z=(Min=6.0000000,Max=6.0000000))
     GetVelocityDirectionFrom=2
     Emitters[3]=SpriteEmitter16
     // Reference: MeshEmitter'lineageeffect.dec.s_u505_c.MeshEmitter10'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.7142860,Color=(R=145,G=145,B=145,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=12.0000000
     ColorMultiplierRange=(X=(Min=0.7110000,Max=0.7110000),Y=(Min=0.7110000,Max=0.7110000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.4440000
     FadeOut=true
     MaxParticles=9
     RespawnDeadParticles=false
     Name="Bodybolt"
     StartLocationOffset=(X=10.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=10.0000000,Max=10.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.2000000),Y=(Min=0.1000000,Max=0.2000000),Z=(Min=0.1000000,Max=0.2000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.5400000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
     StartSizeRange=(X=(Min=0.2940000,Max=0.5000000),Y=(Min=0.2940000,Max=0.5000000),Z=(Min=0.2940000,Max=0.5000000))
     InitialParticlesPerSecond=30.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.4560000,Max=1.2000000)
     StartVelocityRange=(X=(Min=-33.0000000,Max=33.0000000),Y=(Min=-33.0000000,Max=33.0000000),Z=(Min=-63.0000000,Max=63.0000000))
     VelocityLossRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
     Emitters[4]=MeshEmitter10
     // Reference: MeshEmitter'lineageeffect.dec.s_u505_c.MeshEmitter11'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5428570,Color=(R=193,G=193,B=193,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=10.0000000
     ColorMultiplierRange=(X=(Min=0.7200000,Max=0.7200000),Y=(Min=0.7900000,Max=0.7900000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.2100000
     FadeOut=true
     RespawnDeadParticles=false
     Name="MeshEmitter0"
     StartLocationRange=(X=(Min=3.0000000,Max=20.0000000),Y=(Min=-15.0000000,Max=15.0000000),Z=(Min=-15.0000000,Max=10.0000000))
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.4000000,Max=0.5000000),Y=(Min=0.4000000,Max=0.5000000),Z=(Min=0.4000000,Max=0.5000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     StartSizeRange=(X=(Min=0.1500000,Max=0.3000000),Y=(Min=0.1500000,Max=0.3000000),Z=(Min=0.1500000,Max=0.3000000))
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=1.0000000)
     InitialDelayRange=(Min=1.0000000,Max=1.0000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=20.0000000),Y=(Min=-50.0000000,Max=50.0000000),Z=(Min=-50.0000000,Max=50.0000000))
     VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
     Emitters[5]=MeshEmitter11
     // Reference: SpriteEmitter'lineageeffect.dec.s_u505_c.SpriteEmitter17'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4964290,Color=(R=221,G=221,B=221,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=17.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.6440000,Max=0.6440000),Z=(Min=0.4580000,Max=0.4580000))
     Opacity=0.4000000
     FadeOutStartTime=0.0300000
     FadeOut=true
     MaxParticles=5
     RespawnDeadParticles=false
     Name="SpriteEmitter9"
     StartLocationRange=(X=(Min=-5.0000000,Max=5.0000000),Y=(Min=-2.0000000,Max=2.0000000),Z=(Min=-2.0000000,Max=2.0000000))
     StartSizeRange=(X=(Min=25.0000000,Max=30.0000000),Y=(Min=25.0000000,Max=30.0000000),Z=(Min=25.0000000,Max=30.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     BlendBetweenSubdivisions=true
     SubdivisionEnd=3
     LifetimeRange=(Min=1.5000000,Max=1.5000000)
     StartVelocityRange=(X=(Min=80.0000000,Max=80.0000000),Y=(Min=-50.0000000,Max=50.0000000),Z=(Min=-50.0000000,Max=50.0000000))
     VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
     Emitters[6]=SpriteEmitter17
     AutoDestroy=true
     bNoDelete=false
     bDirectional=true
}
