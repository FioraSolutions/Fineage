class we_28ed4a0e3338af1b extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         Acceleration=(X=0.000000,Y=0.000000,Z=-10.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.690000
         FadeOutStartTime=3.010000
         FadeOut=True
         FadeInEndTime=0.490000
         FadeIn=True
         MaxParticles=3
         EffectAxis=PTEA_PositiveZ
         Disabled=True
         Name="SpriteEmitter45"
         StartLocationRange=(X=(Min=-200.000000,Max=200.000000),Y=(Min=-200.000000,Max=200.000000),Z=(Min=50.000000,Max=50.000000))
         StartSizeRange=(X=(Min=200.000000,Max=300.000000),Y=(Min=200.000000,Max=300.000000),Z=(Min=200.000000,Max=300.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.Env_Particles.Mist01'
         LifetimeRange=(Min=7.000000,Max=7.000000)
         StartVelocityRange=(X=(Min=-20.000000,Max=20.000000),Y=(Min=-20.000000,Max=20.000000),Z=(Min=-20.000000,Max=20.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=7.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.230000
         FadeOutStartTime=3.150000
         FadeOut=True
         FadeInEndTime=0.770000
         FadeIn=True
         MaxParticles=5
         Name="SpriteEmitter3"
         StartLocationRange=(X=(Min=-50.000000,Max=50.000000),Y=(Min=-50.000000,Max=50.000000),Z=(Min=0.000000,Max=0.000000))
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.010000,Max=0.020000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=2.000000,RelativeTime=0.410000)
         SizeScale(2)=(RelativeSize=3.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=300.000000,Max=400.000000),Y=(Min=300.000000,Max=400.000000),Z=(Min=300.000000,Max=400.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.fallwater.fallwater_04'
         LifetimeRange=(Min=4.000000,Max=7.000000)
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=7.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
