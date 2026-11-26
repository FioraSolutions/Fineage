class we_574654b899c2c237 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         FadeOutStartTime=4.800000
         FadeOut=True
         MaxParticles=1
         RespawnDeadParticles=False
         Name="SpriteEmitter0"
         StartSizeRange=(X=(Min=1.000000,Max=1.000000),Y=(Min=1.000000,Max=1.000000),Z=(Min=1.000000,Max=1.000000))
         UniformSize=True
         InitialParticlesPerSecond=10000.000000
         AutomaticInitialSpawning=False
         Texture=Texture'FX_E_T.Flameset_a.FX_E_fire_ani6'
         LifetimeRange=(Min=7.000000,Max=7.000000)
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=MeshEmitter Name=MeshEmitter_1
         StaticMesh=StaticMesh'FX_E_S.FX_E_fire.fx_e_fire_ani06'
         UseMeshBlendMode=False
         RenderTwoSided=True
         Acceleration=(X=0.000000,Y=0.000000,Z=-60.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.820000
         FadeOutStartTime=0.110000
         FadeOut=True
         FadeInEndTime=0.020000
         FadeIn=True
         MaxParticles=5
         RespawnDeadParticles=False
         Name="MeshEmitter0"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=-30.000000)
         SpinParticles=True
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=2.000000,RelativeTime=0.280000)
         SizeScale(2)=(RelativeSize=3.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=2.400000,Max=3.000000),Y=(Min=2.400000,Max=3.000000),Z=(Min=2.400000,Max=3.000000))
         UniformSize=True
         InitialParticlesPerSecond=12.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_AlphaBlend
         LifetimeRange=(Min=1.000000,Max=1.000000)
         InitialDelayRange=(Min=1.000000,Max=1.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=100.000000,Max=100.000000))
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter_1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_2
         Acceleration=(X=0.000000,Y=0.000000,Z=-100.000000)
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=95,G=95,R=95,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=1.000000,Max=1.000000),Y=(Min=0.500000,Max=0.500000),Z=(Min=0.500000,Max=0.500000))
         Opacity=0.430000
         FadeOutStartTime=0.550000
         FadeOut=True
         FadeInEndTime=0.100000
         FadeIn=True
         RespawnDeadParticles=False
         Name="SpriteEmitter7"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=20.000000)
         StartLocationRange=(X=(Min=-16.000000,Max=16.000000),Y=(Min=-16.000000,Max=16.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=2.000000,RelativeTime=0.200000)
         SizeScale(2)=(RelativeSize=1.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=100.000000,Max=140.000000),Y=(Min=100.000000,Max=140.000000),Z=(Min=100.000000,Max=140.000000))
         UniformSize=True
         InitialParticlesPerSecond=12.000000
         AutomaticInitialSpawning=False
         Texture=Texture'FX_E_T.Flameset_a.FX_E_fire_ani5'
         LifetimeRange=(Min=5.000000,Max=5.000000)
         InitialDelayRange=(Min=1.000000,Max=1.000000)
         StartVelocityRange=(X=(Min=-100.000000,Max=100.000000),Y=(Min=-100.000000,Max=100.000000),Z=(Min=100.000000,Max=100.000000))
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter_2'
}
