class we_f4c4cc2b4685759e extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.680000
         FadeOutStartTime=2.250000
         FadeOut=True
         FadeInEndTime=1.050000
         FadeIn=True
         MaxParticles=8
         Name="SpriteEmitter7"
         StartLocationRange=(X=(Min=-10.000000,Max=10.000000),Y=(Min=-350.000000,Max=350.000000),Z=(Min=50.000000,Max=50.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=0.800000,RelativeTime=0.220000)
         SizeScale(2)=(RelativeSize=0.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=400.000000,Max=500.000000),Y=(Min=400.000000,Max=500.000000),Z=(Min=400.000000,Max=500.000000))
         UniformSize=True
         DrawStyle=PTDS_Brighten
         Texture=Texture'FX_E_T.smoke.anghel_fog02'
         LifetimeRange=(Min=3.000000,Max=3.000000)
         StartVelocityRange=(X=(Min=-200.000000,Max=-200.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=30.000000,Max=30.000000))
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
         Opacity=0.800000
         FadeOutStartTime=3.000000
         FadeOut=True
         FadeInEndTime=1.120000
         FadeIn=True
         MaxParticles=1
         Name="MeshEmitter4"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=10.000000,Max=10.000000))
         SpinParticles=True
         StartSpinRange=(X=(Min=0.500000,Max=0.500000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         DrawStyle=PTDS_AlphaBlend
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter_1'
}
