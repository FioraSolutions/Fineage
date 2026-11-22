class we_2ad95491381c534b extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.500000
         FadeOutStartTime=2.190000
         FadeOut=True
         FadeInEndTime=0.750000
         FadeIn=True
         Name="SpriteEmitter5"
         StartLocationRange=(X=(Min=-40.000000,Max=40.000000),Y=(Min=-750.000000,Max=750.000000),Z=(Min=-20.000000,Max=-20.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=0.800000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=200.000000,Max=500.000000),Y=(Min=200.000000,Max=500.000000),Z=(Min=200.000000,Max=500.000000))
         UniformSize=True
         DrawStyle=PTDS_Brighten
         Texture=Texture'FX_E_T.smoke.anghel_fog02'
         LifetimeRange=(Min=3.000000,Max=3.000000)
         StartVelocityRange=(X=(Min=-300.000000,Max=-300.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=30.000000,Max=30.000000))
         WarmupTicksPerSecond=3.000000
         RelativeWarmupTime=2.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=MeshEmitter Name=MeshEmitter_1
         StaticMesh=StaticMesh'FX_E_S.fallwater.anghel_fog_mesh'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.600000
         MaxParticles=1
         Name="MeshEmitter5"
         StartLocationRange=(X=(Min=-400.000000,Max=-400.000000),Y=(Min=300.000000,Max=300.000000),Z=(Min=0.000000,Max=0.000000))
         SpinParticles=True
         StartSpinRange=(X=(Min=0.600000,Max=0.600000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=1.500000,Max=1.500000),Y=(Min=1.500000,Max=1.500000),Z=(Min=1.000000,Max=1.000000))
         DrawStyle=PTDS_AlphaBlend
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter_1'
}
