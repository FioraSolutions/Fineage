class we_b540d3a794d83b5c extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.800000,Max=0.800000),Y=(Min=0.800000,Max=0.800000),Z=(Min=0.800000,Max=0.800000))
         Opacity=0.180000
         FadeOutStartTime=6.800000
         FadeOut=True
         FadeInEndTime=1.500000
         FadeIn=True
         WeatherSoundCheck=True
         Name="SpriteEmitter1"
         StartLocationRange=(X=(Min=-30.000000,Max=30.000000),Y=(Min=-30.000000,Max=30.000000),Z=(Min=0.000000,Max=0.000000))
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.020000,Max=0.040000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.410000)
         SizeScale(2)=(RelativeSize=2.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=500.000000,Max=700.000000),Y=(Min=500.000000,Max=700.000000),Z=(Min=500.000000,Max=700.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.fallwater.fallwater_04'
         LifetimeRange=(Min=8.000000,Max=10.000000)
         StartVelocityRange=(X=(Min=10.000000,Max=10.000000),Y=(Min=10.000000,Max=10.000000),Z=(Min=0.000000,Max=0.000000))
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=12.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.300000
         FadeOutStartTime=2.480000
         FadeOut=True
         FadeInEndTime=0.960000
         FadeIn=True
         MaxParticles=3
         Name="SpriteEmitter2"
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=800.000000,Max=1200.000000),Y=(Min=800.000000,Max=1200.000000),Z=(Min=800.000000,Max=1200.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.Flameset_a.FX_E_fire_light02'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
