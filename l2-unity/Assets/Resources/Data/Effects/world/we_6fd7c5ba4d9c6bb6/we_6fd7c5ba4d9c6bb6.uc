class we_6fd7c5ba4d9c6bb6 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.200000
         FadeOutStartTime=2.640000
         FadeOut=True
         FadeInEndTime=0.800000
         FadeIn=True
         MaxParticles=3
         WeatherSoundCheck=True
         Name="SpriteEmitter1"
         StartLocationRange=(X=(Min=-100.000000,Max=100.000000),Y=(Min=-100.000000,Max=100.000000),Z=(Min=158.000000,Max=158.000000))
         StartSizeRange=(X=(Min=110.000000,Max=110.000000),Y=(Min=110.000000,Max=110.000000),Z=(Min=110.000000,Max=110.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.smoke.fog03'
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
         StartLocationRange=(X=(Min=-100.000000,Max=100.000000),Y=(Min=-100.000000,Max=100.000000),Z=(Min=70.000000,Max=70.000000))
         StartSizeRange=(X=(Min=150.000000,Max=150.000000),Y=(Min=150.000000,Max=150.000000),Z=(Min=150.000000,Max=150.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.smoke.fog03'
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=50.000000,Max=50.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
