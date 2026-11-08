class wh_magic_barrier_ta extends Emitter;

defaultproperties
{
     Begin Object Class=VertMeshEmitter Name=VertMeshEmitter0
         // Reference: VertMeshEmitter'lineageeffect.dec.wh_magic_barrier_ta.VertMeshEmitter0'
         begin object name="VertMeshEmitter0" class=Engine.VertMeshEmitter
         VertexMesh=VertMesh'LineageEffectMeshes.magic2'
         UseMeshBlendMode=false
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=128,G=0,B=0,A=255))
         ColorScale[1]=(RelativeTime=0.2500000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=128,G=64,B=0,A=255))
         Opacity=0.6000000
         FadeOutStartTime=0.4000000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Main"
         StartLocationOffset=(X=-5.0000000,Y=2.0000000,Z=4.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=0.1000000,RelativeSize=1.5000000)
         SizeScale[2]=(RelativeTime=0.2300000,RelativeSize=1.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.1200000,Max=0.1200000),Y=(Min=0.1200000,Max=0.1200000),Z=(Min=0.1200000,Max=0.1200000))
         UniformSize=true
         InitialParticlesPerSecond=25.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=2.0000000,Max=2.0000000)
         InitialDelayRange=(Min=1.0000000,Max=1.0000000)
         UseVelocityScale=true
         VelocityScale=/* Array type was not detected. */
         end object
     End Object
     Emitters(0)=VertMeshEmitter'VertMeshEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.wh_magic_barrier_ta.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         Acceleration=(X=0.0000000,Y=0.0000000,Z=5.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=128,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5500000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.8000000
         FadeOutStartTime=0.4860000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Fir"
         StartLocationOffset=(X=-5.0000000,Y=1.0000000,Z=15.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=40.0000000,Max=40.0000000),Y=(Min=50.0000000,Max=50.0000000),Z=(Min=10.0000000,Max=10.0000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t_3031'
         TextureUSubdivisions=4
         TextureVSubdivisions=2
         BlendBetweenSubdivisions=true
         SubdivisionEnd=8
         LifetimeRange=(Min=0.7000000,Max=0.7000000)
         InitialDelayRange=(Min=1.4000000,Max=1.4000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=20.0000000,Max=20.0000000))
         VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=2.0000000,Max=2.0000000))
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter4
         // Reference: SpriteEmitter'lineageeffect.dec.wh_magic_barrier_ta.SpriteEmitter4'
         begin object name="SpriteEmitter4" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.0150000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="LB"
         StartLocationOffset=(X=-5.0000000,Y=0.0000000,Z=-2.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
         StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
         UniformSize=true
         InitialParticlesPerSecond=90.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t_3033'
         LifetimeRange=(Min=0.2500000,Max=0.2500000)
         InitialDelayRange=(Min=1.3200001,Max=1.3200001)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter4'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter5
         // Reference: SpriteEmitter'lineageeffect.dec.wh_magic_barrier_ta.SpriteEmitter5'
         begin object name="SpriteEmitter5" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=218,B=200,A=255))
         ColorScale[1]=(RelativeTime=0.4571430,Color=(R=201,G=201,B=201,A=255))
         ColorScale[2]=(RelativeTime=0.5785710,Color=(R=206,G=206,B=206,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=15.0000000
         Opacity=0.7000000
         FadeOutStartTime=0.2470000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="to"
         StartLocationOffset=(X=-5.0000000,Y=0.6000000,Z=5.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         StartSizeRange=(X=(Min=17.5000000,Max=17.5000000),Y=(Min=25.0000000,Max=25.0000000),Z=(Min=40.0000000,Max=40.0000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t_3013'
         LifetimeRange=(Min=1.3000000,Max=1.3000000)
         InitialDelayRange=(Min=1.4000000,Max=1.4000000)
         VelocityScale=/* Array type was not detected. */
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter5'
     Begin Object Class=MeshEmitter Name=MeshEmitter1
         // Reference: MeshEmitter'lineageeffect.dec.wh_magic_barrier_ta.MeshEmitter1'
         begin object name="MeshEmitter1" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_levelup00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5214290,Color=(R=195,G=195,B=195,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=11.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
         Opacity=0.1800000
         FadeOutStartTime=1.2150000
         FadeOut=true
         FadeInEndTime=0.1050000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="cir"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-62.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
         SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=1.3000000)
         SizeScale[2]=(RelativeTime=0.2900000,RelativeSize=1.2000000)
         SizeScale[3]=(RelativeTime=0.7900000,RelativeSize=1.2000000)
         SizeScale[4]=(RelativeTime=0.8300000,RelativeSize=0.4000000)
         SizeScale[5]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
         StartSizeRange=(X=(Min=0.2800000,Max=0.2800000),Y=(Min=0.2800000,Max=0.2800000),Z=(Min=0.3000000,Max=0.3000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=1.5000000,Max=1.5000000)
         end object
     End Object
     Emitters(4)=MeshEmitter'MeshEmitter1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter6
         // Reference: SpriteEmitter'lineageeffect.dec.wh_magic_barrier_ta.SpriteEmitter6'
         begin object name="SpriteEmitter6" class=Engine.SpriteEmitter
         UseDirectionAs=1
         Acceleration=(X=0.0000000,Y=0.0000000,Z=-1000.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.3740000,Max=1.0000000),Y=(Min=0.3740000,Max=1.0000000),Z=(Min=0.3740000,Max=1.0000000))
         Opacity=0.3000000
         FadeOutStartTime=0.1750000
         FadeOut=true
         FadeInEndTime=0.1100000
         FadeIn=true
         RespawnDeadParticles=false
         Name="Sh"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=50.0000000)
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=110.0000000,Max=150.0000000),Z=(Min=12.0000000,Max=12.0000000))
         StartSizeRange=(X=(Min=4.0000000,Max=10.0000000),Y=(Min=80.0000000,Max=80.0000000),Z=(Min=3.0000000,Max=3.0000000))
         InitialParticlesPerSecond=140.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
         TextureUSubdivisions=8
         TextureVSubdivisions=2
         SubdivisionStart=13
         SubdivisionEnd=14
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         InitialDelayRange=(Min=1.2000000,Max=1.2000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-120.0000000,Max=-120.0000000))
         end object
     End Object
     Emitters(5)=SpriteEmitter'SpriteEmitter6'
     // Reference: VertMeshEmitter'lineageeffect.dec.wh_magic_barrier_ta.VertMeshEmitter0'
     VertexMesh=VertMesh'LineageEffectMeshes.magic2'
     UseMeshBlendMode=false
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=128,G=0,B=0,A=255))
     ColorScale[1]=(RelativeTime=0.2500000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=128,G=64,B=0,A=255))
     Opacity=0.6000000
     FadeOutStartTime=0.4000000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Main"
     StartLocationOffset=(X=-5.0000000,Y=2.0000000,Z=4.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=0.1000000,RelativeSize=1.5000000)
     SizeScale[2]=(RelativeTime=0.2300000,RelativeSize=1.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.1200000,Max=0.1200000),Y=(Min=0.1200000,Max=0.1200000),Z=(Min=0.1200000,Max=0.1200000))
     UniformSize=true
     InitialParticlesPerSecond=25.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=2.0000000,Max=2.0000000)
     InitialDelayRange=(Min=1.0000000,Max=1.0000000)
     UseVelocityScale=true
     VelocityScale=/* Array type was not detected. */
     Emitters[0]=VertMeshEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.wh_magic_barrier_ta.SpriteEmitter0'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     Acceleration=(X=0.0000000,Y=0.0000000,Z=5.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=128,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5500000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.8000000
     FadeOutStartTime=0.4860000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Fir"
     StartLocationOffset=(X=-5.0000000,Y=1.0000000,Z=15.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=40.0000000,Max=40.0000000),Y=(Min=50.0000000,Max=50.0000000),Z=(Min=10.0000000,Max=10.0000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t_3031'
     TextureUSubdivisions=4
     TextureVSubdivisions=2
     BlendBetweenSubdivisions=true
     SubdivisionEnd=8
     LifetimeRange=(Min=0.7000000,Max=0.7000000)
     InitialDelayRange=(Min=1.4000000,Max=1.4000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=20.0000000,Max=20.0000000))
     VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=2.0000000,Max=2.0000000))
     Emitters[1]=SpriteEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.wh_magic_barrier_ta.SpriteEmitter4'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.0150000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="LB"
     StartLocationOffset=(X=-5.0000000,Y=0.0000000,Z=-2.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
     StartSizeRange=(X=(Min=30.0000000,Max=30.0000000),Y=(Min=30.0000000,Max=30.0000000),Z=(Min=30.0000000,Max=30.0000000))
     UniformSize=true
     InitialParticlesPerSecond=90.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t_3033'
     LifetimeRange=(Min=0.2500000,Max=0.2500000)
     InitialDelayRange=(Min=1.3200001,Max=1.3200001)
     Emitters[2]=SpriteEmitter4
     // Reference: SpriteEmitter'lineageeffect.dec.wh_magic_barrier_ta.SpriteEmitter5'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=218,B=200,A=255))
     ColorScale[1]=(RelativeTime=0.4571430,Color=(R=201,G=201,B=201,A=255))
     ColorScale[2]=(RelativeTime=0.5785710,Color=(R=206,G=206,B=206,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=15.0000000
     Opacity=0.7000000
     FadeOutStartTime=0.2470000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="to"
     StartLocationOffset=(X=-5.0000000,Y=0.6000000,Z=5.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     StartSizeRange=(X=(Min=17.5000000,Max=17.5000000),Y=(Min=25.0000000,Max=25.0000000),Z=(Min=40.0000000,Max=40.0000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t_3013'
     LifetimeRange=(Min=1.3000000,Max=1.3000000)
     InitialDelayRange=(Min=1.4000000,Max=1.4000000)
     VelocityScale=/* Array type was not detected. */
     Emitters[3]=SpriteEmitter5
     // Reference: MeshEmitter'lineageeffect.dec.wh_magic_barrier_ta.MeshEmitter1'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_levelup00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5214290,Color=(R=195,G=195,B=195,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=11.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
     Opacity=0.1800000
     FadeOutStartTime=1.2150000
     FadeOut=true
     FadeInEndTime=0.1050000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="cir"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-62.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.5000000)
     SizeScale[1]=(RelativeTime=0.1500000,RelativeSize=1.3000000)
     SizeScale[2]=(RelativeTime=0.2900000,RelativeSize=1.2000000)
     SizeScale[3]=(RelativeTime=0.7900000,RelativeSize=1.2000000)
     SizeScale[4]=(RelativeTime=0.8300000,RelativeSize=0.4000000)
     SizeScale[5]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
     StartSizeRange=(X=(Min=0.2800000,Max=0.2800000),Y=(Min=0.2800000,Max=0.2800000),Z=(Min=0.3000000,Max=0.3000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=1.5000000,Max=1.5000000)
     Emitters[4]=MeshEmitter1
     // Reference: SpriteEmitter'lineageeffect.dec.wh_magic_barrier_ta.SpriteEmitter6'
     UseDirectionAs=1
     Acceleration=(X=0.0000000,Y=0.0000000,Z=-1000.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.3740000,Max=1.0000000),Y=(Min=0.3740000,Max=1.0000000),Z=(Min=0.3740000,Max=1.0000000))
     Opacity=0.3000000
     FadeOutStartTime=0.1750000
     FadeOut=true
     FadeInEndTime=0.1100000
     FadeIn=true
     RespawnDeadParticles=false
     Name="Sh"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=50.0000000)
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=0.0000000,Max=360.0000000),Y=(Min=110.0000000,Max=150.0000000),Z=(Min=12.0000000,Max=12.0000000))
     StartSizeRange=(X=(Min=4.0000000,Max=10.0000000),Y=(Min=80.0000000,Max=80.0000000),Z=(Min=3.0000000,Max=3.0000000))
     InitialParticlesPerSecond=140.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
     TextureUSubdivisions=8
     TextureVSubdivisions=2
     SubdivisionStart=13
     SubdivisionEnd=14
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     InitialDelayRange=(Min=1.2000000,Max=1.2000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-120.0000000,Max=-120.0000000))
     Emitters[5]=SpriteEmitter6
     AutoDestroy=true
     bLightChanged=true
     bNoDelete=false
     bSunAffect=true
     Rotation=(Pitch=0,Yaw=16416,Roll=0)
     DrawScale=0.2000000
     SwayRotationOrig=(Pitch=0,Yaw=16416,Roll=0)
     bDirectional=true
}
