class we_c29d5d0c95791b87 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.500000
         FadeOutStartTime=2.640000
         FadeOut=True
         FadeInEndTime=0.800000
         FadeIn=True
         MaxParticles=2
         WeatherSoundCheck=True
         Name="SpriteEmitter1"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=90.000000,Max=90.000000))
         StartSizeRange=(X=(Min=170.000000,Max=170.000000),Y=(Min=170.000000,Max=170.000000),Z=(Min=170.000000,Max=170.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.fallwater.anghel_fog04'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         Acceleration=(X=0.000000,Y=0.000000,Z=-20.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.400000
         FadeOutStartTime=2.960000
         FadeOut=True
         FadeInEndTime=0.440000
         FadeIn=True
         MaxParticles=5
         Name="SpriteEmitter2"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=100.000000,Max=100.000000))
         StartSizeRange=(X=(Min=70.000000,Max=70.000000),Y=(Min=70.000000,Max=70.000000),Z=(Min=70.000000,Max=70.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.smoke.fog03'
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=50.000000,Max=50.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
