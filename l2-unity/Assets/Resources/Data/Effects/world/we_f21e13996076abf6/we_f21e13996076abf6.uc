class we_f21e13996076abf6 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.300000
         FadeOutStartTime=2.160000
         FadeOut=True
         FadeInEndTime=0.400000
         FadeIn=True
         MaxParticles=7
         WeatherSoundCheck=True
         Name="SpriteEmitter0"
         StartLocationRange=(X=(Min=-10.000000,Max=10.000000),Y=(Min=-10.000000,Max=10.000000),Z=(Min=0.000000,Max=0.000000))
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.010000,Max=0.040000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.500000,RelativeTime=0.370000)
         SizeScale(2)=(RelativeSize=3.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=50.000000,Max=70.000000),Y=(Min=50.000000,Max=70.000000),Z=(Min=50.000000,Max=70.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.fallwater.fallwater_04'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.240000
         FadeOutStartTime=2.640000
         FadeOut=True
         FadeInEndTime=0.800000
         FadeIn=True
         MaxParticles=2
         Name="SpriteEmitter1"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=30.000000,Max=30.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.fallwater.anghel_fog04'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
