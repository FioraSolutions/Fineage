class we_c12982a4f7f9b6ce extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.300000
         FadeOutStartTime=1.680000
         FadeOut=True
         FadeInEndTime=0.960000
         FadeIn=True
         MaxParticles=5
         WeatherSoundCheck=True
         Name="SpriteEmitter62"
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.050000)
         StartSizeRange=(X=(Min=35.000000,Max=65.000000),Y=(Min=8.000000,Max=8.000000),Z=(Min=8.000000,Max=8.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.fallwater.angel_fall_waterwave04'
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=-30.000000,Max=-25.000000),Z=(Min=0.000000,Max=0.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         Acceleration=(X=0.000000,Y=0.000000,Z=-15.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.800000,Max=0.800000),Y=(Min=0.900000,Max=0.900000),Z=(Min=0.900000,Max=0.900000))
         Opacity=0.370000
         FadeOutStartTime=1.560000
         FadeOut=True
         FadeInEndTime=0.520000
         FadeIn=True
         Name="SpriteEmitter63"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=-15.000000)
         StartLocationRange=(X=(Min=-30.000000,Max=30.000000),Y=(Min=-30.000000,Max=30.000000),Z=(Min=20.000000,Max=20.000000))
         StartSizeRange=(X=(Min=35.000000,Max=55.000000),Y=(Min=35.000000,Max=55.000000),Z=(Min=35.000000,Max=55.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.smoke.anghel_fog'
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=15.000000,Max=15.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
