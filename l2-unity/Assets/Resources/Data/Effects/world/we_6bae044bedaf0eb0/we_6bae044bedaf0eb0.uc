class we_6bae044bedaf0eb0 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Up
         Acceleration=(X=0.000000,Y=0.000000,Z=10.000000)
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=153,G=153,R=153,A=255),RelativeTime=1.000000)
         Opacity=0.490000
         FadeOutStartTime=5.900000
         FadeOut=True
         FadeInEndTime=0.800000
         FadeIn=True
         MaxParticles=20
         SpinParticles=True
         SpinsPerSecondRange=(X=(Min=0.000000,Max=0.050000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.300000,Max=0.600000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=100.000000,Max=200.000000),Y=(Min=100.000000,Max=200.000000),Z=(Min=100.000000,Max=200.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.smoke.fly_smoke'
         LifetimeRange=(Min=8.000000,Max=8.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=20.000000))
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=3.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         UseDirectionAs=PTDU_Up
         Acceleration=(X=0.000000,Y=0.000000,Z=30.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.450000
         FadeOutStartTime=2.480000
         FadeOut=True
         FadeInEndTime=1.560000
         FadeIn=True
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=2.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=1.000000,Max=100.000000),Y=(Min=3.000000,Max=300.000000),Z=(Min=1.000000,Max=100.000000))
         DrawStyle=PTDS_Darken
         Texture=Texture'FX_E_T.smoke.up_smoke'
         StartVelocityRange=(X=(Min=10.000000,Max=30.000000),Y=(Min=30.000000,Max=50.000000),Z=(Min=0.000000,Max=20.000000))
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=3.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
