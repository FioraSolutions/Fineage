class we_0bbf81f3190be993 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         FadeOutStartTime=0.480000
         FadeOut=True
         FadeInEndTime=0.210000
         FadeIn=True
         MaxParticles=12
         WeatherSoundCheck=True
         Name="SpriteEmitter0"
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=0.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=15.000000,Max=20.000000),Y=(Min=15.000000,Max=20.000000),Z=(Min=15.000000,Max=20.000000))
         UniformSize=True
         DrawStyle=PTDS_Brighten
         Texture=Texture'SSQ_dungeon_T.ssq_fx.SSQ_FX_tex01'
         LifetimeRange=(Min=3.000000,Max=3.000000)
         StartVelocityRange=(X=(Min=-3.000000,Max=3.000000),Y=(Min=-3.000000,Max=3.000000),Z=(Min=6.000000,Max=10.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=3.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=1.000000,Max=1.000000),Z=(Min=1.000000,Max=1.000000))
         FadeOutStartTime=0.380000
         FadeOut=True
         FadeInEndTime=0.140000
         FadeIn=True
         Name="SpriteEmitter2"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=3.000000,Max=3.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=0.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=5.000000,Max=15.000000),Y=(Min=5.000000,Max=15.000000),Z=(Min=5.000000,Max=15.000000))
         UniformSize=True
         DrawStyle=PTDS_Brighten
         Texture=Texture'FX_E_T.LightGlowSet.BlueFlare01'
         LifetimeRange=(Min=1.000000,Max=1.000000)
         StartVelocityRange=(X=(Min=-10.000000,Max=10.000000),Y=(Min=-10.000000,Max=10.000000),Z=(Min=10.000000,Max=25.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=1.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
