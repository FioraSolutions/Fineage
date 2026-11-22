class we_c730a00ae69ac84c extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.290000
         FadeOutStartTime=3.400000
         FadeOut=True
         FadeInEndTime=1.000000
         FadeIn=True
         Name="SpriteEmitter1"
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=5.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=150.000000,Max=150.000000),Y=(Min=150.000000,Max=150.000000),Z=(Min=150.000000,Max=150.000000))
         DrawStyle=PTDS_Brighten
         Texture=Texture'FX_E_T.fallwater.fallwater_04'
         LifetimeRange=(Min=5.000000,Max=5.000000)
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.330000
         FadeOutStartTime=0.610000
         FadeOut=True
         FadeInEndTime=0.230000
         FadeIn=True
         MaxParticles=4
         Name="SpriteEmitter2"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=600.000000,Max=600.000000))
         SpinParticles=True
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=2.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=2.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=700.000000,Max=700.000000),Y=(Min=700.000000,Max=700.000000),Z=(Min=700.000000,Max=700.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.fallwater.fog04'
         LifetimeRange=(Min=1.000000,Max=1.000000)
         StartVelocityRange=(X=(Min=-50.000000,Max=50.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=-100.000000,Max=-100.000000))
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=2.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
