class we_97913df67462b7b2 extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter_0
         StaticMesh=StaticMesh'Oren_HEV_S.O_E_symbol_002'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         FadeOutStartTime=1.000000
         MaxParticles=1
         Name="MeshEmitter0"
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.300000,Max=0.300000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=1.200000,Max=1.200000),Y=(Min=1.200000,Max=1.200000),Z=(Min=1.200000,Max=1.200000))
         InitialParticlesPerSecond=500.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=10000.000000,Max=10000.000000)
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         Acceleration=(X=0.000000,Y=0.000000,Z=-12.599999)
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=226,R=200,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=241,G=238,R=99,A=255),RelativeTime=1.000000)
         Opacity=0.510000
         FadeOutStartTime=2.600000
         FadeOut=True
         FadeInEndTime=0.920000
         FadeIn=True
         MaxParticles=6
         Name="SpriteEmitter2"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=40.000000)
         StartLocationRange=(X=(Min=-10.000000,Max=10.000000),Y=(Min=-15.000000,Max=10.000000),Z=(Min=-30.000000,Max=-20.000000))
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.000000,Max=0.050000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.100000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.480000)
         SizeScale(2)=(RelativeSize=0.100000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=40.000000,Max=40.000000),Y=(Min=40.000000,Max=40.000000),Z=(Min=40.000000,Max=40.000000))
         UniformSize=True
         Texture=Texture'Oren_HEV_T.elf_vi_sy_ef'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
