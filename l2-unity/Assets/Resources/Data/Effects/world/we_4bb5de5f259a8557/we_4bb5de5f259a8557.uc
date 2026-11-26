class we_4bb5de5f259a8557 extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter_0
         StaticMesh=StaticMesh'FX_E_S.Etc.magiccircleblack02'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.001000,Max=0.001000),Y=(Min=0.001000,Max=0.001000),Z=(Min=0.200000,Max=0.200000))
         Opacity=0.200000
         FadeOutStartTime=12.500000
         MaxParticles=1
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=5.000000)
         SpinsPerSecondRange=(X=(Min=0.100000,Max=0.100000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.950000,RelativeTime=0.500000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=1.000000)
         SizeScaleRepeats=500.000000
         StartSizeRange=(X=(Min=1.920000,Max=1.920000),Y=(Min=1.920000,Max=1.920000),Z=(Min=0.480000,Max=0.480000))
         InitialParticlesPerSecond=1000.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_AlphaBlend
         LifetimeRange=(Min=12.500000,Max=12.500000)
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter_0'
     Begin Object Class=MeshEmitter Name=MeshEmitter_1
         StaticMesh=StaticMesh'FX_E_S.Etc.summon00'
         UseMeshBlendMode=False
         RenderTwoSided=True
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=227,G=214,R=208,A=255),RelativeTime=0.789286)
         ColorScale(2)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorScaleRepeats=50.000000
         Opacity=0.600000
         FadeOutStartTime=12.500000
         MaxParticles=1
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=-10.000000)
         SpinParticles=True
         SpinCCWorCW=(X=0.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.020000,Max=0.020000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.999000,RelativeTime=0.460000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=1.000000)
         SizeScaleRepeats=50.000000
         StartSizeRange=(X=(Min=1.088000,Max=1.088000),Y=(Min=1.088000,Max=1.088000),Z=(Min=1.088000,Max=1.088000))
         InitialParticlesPerSecond=10000.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_Brighten
         LifetimeRange=(Min=12.500000,Max=12.500000)
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter_1'
}
