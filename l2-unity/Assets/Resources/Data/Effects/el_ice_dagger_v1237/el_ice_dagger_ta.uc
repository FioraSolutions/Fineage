class el_ice_dagger_ta extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter5
         // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_ta.MeshEmitter5'
         begin object name="MeshEmitter5" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.water.watericebolt01'
         Acceleration=(X=0.0000000,Y=0.0000000,Z=-300.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         MaxParticles=8
         RespawnDeadParticles=false
         Name="Frag"
         StartLocationOffset=(X=-15.0000000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=3.0000000),Z=(Min=0.0000000,Max=3.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.6000000,RelativeSize=0.2000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
         StartSizeRange=(X=(Min=0.0080000,Max=0.1000000),Y=(Min=0.0080000,Max=0.1000000),Z=(Min=0.1200000,Max=0.2000000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=0
         LifetimeRange=(Min=1.4000000,Max=1.4000000)
         StartVelocityRange=(X=(Min=100.0000000,Max=200.0000000),Y=(Min=-150.0000000,Max=150.0000000),Z=(Min=-150.0000000,Max=150.0000000))
         VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter5'
     Begin Object Class=MeshEmitter Name=MeshEmitter7
         // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_ta.MeshEmitter7'
         begin object name="MeshEmitter7" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=80.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7980000,Max=0.7980000),Z=(Min=0.6870000,Max=0.6870000))
         FadeOutStartTime=0.1200000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Slash"
         StartLocationOffset=(X=-30.0000000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=0.5000000,Y=0.5000000,Z=0.0000000)
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.3000000,Max=0.3000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.2500000,Max=0.2500000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter7'
     Begin Object Class=MeshEmitter Name=MeshEmitter10
         // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_ta.MeshEmitter10'
         begin object name="MeshEmitter10" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power04'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.0642860,Color=(R=55,G=153,B=200,A=255))
         ColorScale[2]=(RelativeTime=0.3142860,Color=(R=49,G=49,B=153,A=255))
         ColorScale[3]=(RelativeTime=0.6071430,Color=(R=43,G=43,B=104,A=255))
         ColorScale[4]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         FadeOutStartTime=0.4000000
         MaxParticles=3
         RespawnDeadParticles=false
         Name="MeshWave"
         StartLocationOffset=(X=-8.0000000,Y=0.0000000,Z=0.0000000)
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
         StartSizeRange=(X=(Min=0.1300000,Max=0.1300000),Y=(Min=0.1300000,Max=0.1300000),Z=(Min=0.0700000,Max=0.2000000))
         UniformSize=true
         InitialParticlesPerSecond=30.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.5000000)
         StartVelocityRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter10'
     Begin Object Class=MeshEmitter Name=MeshEmitter11
         // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_ta.MeshEmitter11'
         begin object name="MeshEmitter11" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Black.black_poison00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.7070000,Max=0.7070000),Y=(Min=0.7070000,Max=0.7070000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.8000000
         FadeOutStartTime=0.0500000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Ray"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.2500000,Max=0.2500000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.1100000,RelativeSize=1.5000000)
         SizeScale[1]=(RelativeTime=0.4400000,RelativeSize=2.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.5000000)
         StartSizeRange=(X=(Min=0.1700000,Max=0.1700000),Y=(Min=0.1700000,Max=0.1700000),Z=(Min=0.1700000,Max=0.1700000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         end object
     End Object
     Emitters(3)=MeshEmitter'MeshEmitter11'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter6
         // Reference: SpriteEmitter'lineageeffect.dec.el_ice_dagger_ta.SpriteEmitter6'
         begin object name="SpriteEmitter6" class=Engine.SpriteEmitter
         Acceleration=(X=0.0000000,Y=0.0000000,Z=-50.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.6000000,Max=1.0000000))
         FadeOutStartTime=0.9100000
         FadeOut=true
         CoordinateSystem=0
         MaxParticles=15
         RespawnDeadParticles=false
         Name="Kira"
         StartLocationOffset=(X=15.0000000,Y=0.0000000,Z=10.0000000)
         StartLocationRange=(X=(Min=0.0000000,Max=30.0000000),Y=(Min=-20.0000000,Max=20.0000000),Z=(Min=-20.0000000,Max=20.0000000))
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0300000,Max=0.0300000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.1200000,RelativeSize=3.0000000)
         SizeScale[1]=(RelativeTime=0.5000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.7900000,RelativeSize=0.1000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
         StartSizeRange=(X=(Min=1.5000000,Max=2.5000000),Y=(Min=1.5000000,Max=2.5000000),Z=(Min=1.5000000,Max=2.5000000))
         UniformSize=true
         InitialParticlesPerSecond=20.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0085'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         BlendBetweenSubdivisions=true
         SubdivisionEnd=2
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         InitialDelayRange=(Min=0.1000000,Max=0.1000000)
         StartVelocityRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
         VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
         end object
     End Object
     Emitters(4)=SpriteEmitter'SpriteEmitter6'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter9
         // Reference: SpriteEmitter'lineageeffect.dec.el_ice_dagger_ta.SpriteEmitter9'
         begin object name="SpriteEmitter9" class=Engine.SpriteEmitter
         UseDirectionAs=1
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.9000000,Max=0.9000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.1410000
         FadeOut=true
         RespawnDeadParticles=false
         Name="BlueNeedle"
         StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationShape=2
         SphereRadiusRange=(Min=15.0000000,Max=25.0000000)
         StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.2000000,Max=0.8000000),Y=(Min=10.0000000,Max=19.0440006),Z=(Min=0.2000000,Max=0.8000000))
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
     Emitters(5)=SpriteEmitter'SpriteEmitter9'
     // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_ta.MeshEmitter5'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.water.watericebolt01'
     Acceleration=(X=0.0000000,Y=0.0000000,Z=-300.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     MaxParticles=8
     RespawnDeadParticles=false
     Name="Frag"
     StartLocationOffset=(X=-15.0000000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=3.0000000),Z=(Min=0.0000000,Max=3.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.6000000,RelativeSize=0.2000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
     StartSizeRange=(X=(Min=0.0080000,Max=0.1000000),Y=(Min=0.0080000,Max=0.1000000),Z=(Min=0.1200000,Max=0.2000000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=0
     LifetimeRange=(Min=1.4000000,Max=1.4000000)
     StartVelocityRange=(X=(Min=100.0000000,Max=200.0000000),Y=(Min=-150.0000000,Max=150.0000000),Z=(Min=-150.0000000,Max=150.0000000))
     VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
     Emitters[0]=MeshEmitter5
     // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_ta.MeshEmitter7'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=80.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7980000,Max=0.7980000),Z=(Min=0.6870000,Max=0.6870000))
     FadeOutStartTime=0.1200000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Slash"
     StartLocationOffset=(X=-30.0000000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=0.5000000,Y=0.5000000,Z=0.0000000)
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.3000000,Max=0.3000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.2500000,Max=0.2500000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[1]=MeshEmitter7
     // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_ta.MeshEmitter10'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power04'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.0642860,Color=(R=55,G=153,B=200,A=255))
     ColorScale[2]=(RelativeTime=0.3142860,Color=(R=49,G=49,B=153,A=255))
     ColorScale[3]=(RelativeTime=0.6071430,Color=(R=43,G=43,B=104,A=255))
     ColorScale[4]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     FadeOutStartTime=0.4000000
     MaxParticles=3
     RespawnDeadParticles=false
     Name="MeshWave"
     StartLocationOffset=(X=-8.0000000,Y=0.0000000,Z=0.0000000)
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
     StartSizeRange=(X=(Min=0.1300000,Max=0.1300000),Y=(Min=0.1300000,Max=0.1300000),Z=(Min=0.0700000,Max=0.2000000))
     UniformSize=true
     InitialParticlesPerSecond=30.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.5000000)
     StartVelocityRange=(X=(Min=20.0000000,Max=20.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[2]=MeshEmitter10
     // Reference: MeshEmitter'lineageeffect.dec.el_ice_dagger_ta.MeshEmitter11'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Black.black_poison00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.7070000,Max=0.7070000),Y=(Min=0.7070000,Max=0.7070000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.8000000
     FadeOutStartTime=0.0500000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Ray"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.2500000,Max=0.2500000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.1100000,RelativeSize=1.5000000)
     SizeScale[1]=(RelativeTime=0.4400000,RelativeSize=2.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.5000000)
     StartSizeRange=(X=(Min=0.1700000,Max=0.1700000),Y=(Min=0.1700000,Max=0.1700000),Z=(Min=0.1700000,Max=0.1700000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     Emitters[3]=MeshEmitter11
     // Reference: SpriteEmitter'lineageeffect.dec.el_ice_dagger_ta.SpriteEmitter6'
     Acceleration=(X=0.0000000,Y=0.0000000,Z=-50.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.5000000,Max=1.0000000),Y=(Min=0.5000000,Max=1.0000000),Z=(Min=0.6000000,Max=1.0000000))
     FadeOutStartTime=0.9100000
     FadeOut=true
     CoordinateSystem=0
     MaxParticles=15
     RespawnDeadParticles=false
     Name="Kira"
     StartLocationOffset=(X=15.0000000,Y=0.0000000,Z=10.0000000)
     StartLocationRange=(X=(Min=0.0000000,Max=30.0000000),Y=(Min=-20.0000000,Max=20.0000000),Z=(Min=-20.0000000,Max=20.0000000))
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0300000,Max=0.0300000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.1200000,RelativeSize=3.0000000)
     SizeScale[1]=(RelativeTime=0.5000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.7900000,RelativeSize=0.1000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
     StartSizeRange=(X=(Min=1.5000000,Max=2.5000000),Y=(Min=1.5000000,Max=2.5000000),Z=(Min=1.5000000,Max=2.5000000))
     UniformSize=true
     InitialParticlesPerSecond=20.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0085'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     BlendBetweenSubdivisions=true
     SubdivisionEnd=2
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     InitialDelayRange=(Min=0.1000000,Max=0.1000000)
     StartVelocityRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
     VelocityLossRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
     Emitters[4]=SpriteEmitter6
     // Reference: SpriteEmitter'lineageeffect.dec.el_ice_dagger_ta.SpriteEmitter9'
     UseDirectionAs=1
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.9000000,Max=0.9000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.1410000
     FadeOut=true
     RespawnDeadParticles=false
     Name="BlueNeedle"
     StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationShape=2
     SphereRadiusRange=(Min=15.0000000,Max=25.0000000)
     StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.2000000,Max=0.8000000),Y=(Min=10.0000000,Max=19.0440006),Z=(Min=0.2000000,Max=0.8000000))
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
     Emitters[5]=SpriteEmitter9
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.0500000
     bDirectional=true
}
