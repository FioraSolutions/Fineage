class we_8aae900f8fdfdf4a extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter_0
         StaticMesh=StaticMesh'Dion_Cruma_Tower_S.curuma_tower_effect'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.460000
         FadeOutStartTime=1.000000
         FadeOut=True
         FadeInEndTime=1.000000
         FadeIn=True
         MaxParticles=2
         Name="MeshEmitter1"
         SpinParticles=True
         StartSpinRange=(X=(Min=0.417000,Max=0.417000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=0.920000,Max=0.920000),Y=(Min=0.920000,Max=0.920000),Z=(Min=0.920000,Max=0.920000))
         LifetimeRange=(Min=2.000000,Max=2.000000)
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=2.000000
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter_0'
     Begin Object Class=MeshEmitter Name=MeshEmitter_1
         StaticMesh=StaticMesh'Dion_Cruma_Tower_S.curuma_tower_effect2'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.240000
         FadeOutStartTime=1.440000
         FadeOut=True
         FadeInEndTime=1.440000
         FadeIn=True
         MaxParticles=2
         Name="MeshEmitter0"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=50.000000)
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.000000,Max=0.010000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         DrawStyle=PTDS_AlphaBlend
         LifetimeRange=(Min=3.000000,Max=3.000000)
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=3.000000
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter_1'
}
