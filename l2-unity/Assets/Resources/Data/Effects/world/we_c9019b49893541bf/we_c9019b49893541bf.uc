class we_c9019b49893541bf extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Up
         Acceleration=(X=0.000000,Y=-0.500000,Z=0.000000)
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=131,G=131,R=131,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.020000,Max=0.020000),Y=(Min=0.020000,Max=0.020000),Z=(Min=0.020000,Max=0.020000))
         Opacity=0.370000
         FadeOutStartTime=2.050000
         FadeOut=True
         MaxParticles=30
         Name="SpriteEmitter0"
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=-0.031000,Max=0.031000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=-1.000000,Max=0.500000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.300000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.300000,RelativeTime=0.620000)
         SizeScale(2)=(RelativeSize=1.800000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=80.000000,Max=80.000000),Y=(Min=80.000000,Max=80.000000),Z=(Min=80.000000,Max=80.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.smoke.fly_smoke'
         LifetimeRange=(Min=35.000000,Max=35.000000)
         StartVelocityRange=(X=(Min=-1.333000,Max=1.333000),Y=(Min=-1.333000,Max=1.333000),Z=(Min=14.000000,Max=16.200001))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=5.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         Acceleration=(X=0.000000,Y=0.000000,Z=3.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.370000
         FadeOutStartTime=1.360000
         FadeOut=True
         FadeInEndTime=1.040000
         FadeIn=True
         MaxParticles=20
         Name="SpriteEmitter7"
         StartLocationRange=(X=(Min=-3.950000,Max=3.950000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.000000,Max=0.030000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=-0.325000,Max=0.325000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.397000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=0.612000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=50.000000,Max=50.000000),Y=(Min=50.000000,Max=50.000000),Z=(Min=50.000000,Max=50.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.smoke.fly_smoke'
         LifetimeRange=(Min=5.000000,Max=5.000000)
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=3.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
