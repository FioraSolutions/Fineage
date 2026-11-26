class we_dcab79234ad79727 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=253,G=253,R=253,A=255),RelativeTime=0.171429)
         ColorScale(2)=(Color=(B=68,G=68,R=68,A=255),RelativeTime=0.278571)
         ColorScale(3)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.457143)
         ColorScale(4)=(Color=(B=75,G=75,R=75,A=255),RelativeTime=0.685714)
         ColorScale(5)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.460000
         FadeOutStartTime=0.910000
         FadeOut=True
         FadeInEndTime=0.420000
         FadeIn=True
         Name="SpriteEmitter4"
         SpinParticles=True
         StartSpinRange=(X=(Min=0.500000,Max=0.500000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         SizeScale(0)=(RelativeSize=0.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.200000,RelativeTime=0.110000)
         SizeScale(2)=(RelativeSize=1.500000,RelativeTime=0.440000)
         SizeScale(3)=(RelativeSize=0.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=400.000000,Max=500.000000),Y=(Min=100.000000,Max=100.000000),Z=(Min=100.000000,Max=100.000000))
         DrawStyle=PTDS_Brighten
         Texture=Texture'FX_E_T.fallwater.angel_fall_waterwave04'
         LifetimeRange=(Min=7.000000,Max=7.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=50.000000,Max=50.000000),Z=(Min=0.000000,Max=0.000000))
         WarmupTicksPerSecond=4.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.630000
         FadeOutStartTime=0.330000
         FadeOut=True
         MaxParticles=30
         Disabled=True
         Name="SpriteEmitter8"
         StartLocationRange=(X=(Min=-200.000000,Max=200.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         SpinParticles=True
         SpinsPerSecondRange=(X=(Min=0.010000,Max=0.050000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=0.500000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=100.000000,Max=200.000000),Y=(Min=100.000000,Max=200.000000),Z=(Min=100.000000,Max=200.000000))
         UniformSize=True
         DrawStyle=PTDS_Brighten
         Texture=Texture'FX_E_T.smoke.fog03'
         LifetimeRange=(Min=1.000000,Max=1.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=20.000000,Max=50.000000),Z=(Min=10.000000,Max=20.000000))
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
     Begin Object Class=MeshEmitter Name=MeshEmitter_2
         StaticMesh=StaticMesh'FX_E_S.fallwater.anghel_waterfall_wave01'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.510000
         FadeOutStartTime=0.640000
         FadeOut=True
         MaxParticles=5
         Disabled=True
         Name="MeshEmitter2"
         StartLocationRange=(X=(Min=-100.000000,Max=100.000000),Y=(Min=-100.000000,Max=100.000000),Z=(Min=0.000000,Max=0.000000))
         SpinParticles=True
         StartSpinRange=(X=(Min=0.750000,Max=0.750000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.250000,Max=0.250000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.140000)
         SizeScale(2)=(RelativeSize=1.500000,RelativeTime=1.000000)
         DrawStyle=PTDS_Brighten
         LifetimeRange=(Min=3.000000,Max=3.000000)
         StartVelocityRange=(X=(Min=-100.000000,Max=100.000000),Y=(Min=300.000000,Max=300.000000),Z=(Min=0.000000,Max=0.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=3.000000
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter_2'
}
