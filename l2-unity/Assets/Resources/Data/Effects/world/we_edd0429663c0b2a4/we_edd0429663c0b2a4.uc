class we_edd0429663c0b2a4 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=105,G=105,R=105,A=255),RelativeTime=0.125000)
         ColorScale(2)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.289286)
         ColorScale(3)=(Color=(B=97,G=97,R=97,A=255),RelativeTime=0.439286)
         ColorScale(4)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.607143)
         ColorScale(5)=(Color=(B=100,G=100,R=100,A=255),RelativeTime=0.800000)
         ColorScale(6)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.420000
         FadeOutStartTime=3.000000
         FadeOut=True
         FadeInEndTime=0.760000
         FadeIn=True
         MaxParticles=20
         Name="SpriteEmitter0"
         StartLocationRange=(X=(Min=-30.000000,Max=30.000000),Y=(Min=-30.000000,Max=30.000000),Z=(Min=-30.000000,Max=30.000000))
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.020000,Max=0.030000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.500000)
         SizeScale(2)=(RelativeSize=0.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=3.000000,Max=6.000000),Y=(Min=3.000000,Max=6.000000),Z=(Min=3.000000,Max=6.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.particles_etc.25_19_17'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=True
         SubdivisionEnd=15
         UseRandomSubdivision=True
         StartVelocityRange=(X=(Min=-10.000000,Max=10.000000),Y=(Min=-10.000000,Max=10.000000),Z=(Min=-10.000000,Max=10.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=5.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.420000
         FadeOutStartTime=1.720000
         FadeOut=True
         FadeInEndTime=0.760000
         FadeIn=True
         MaxParticles=2
         Name="SpriteEmitter3"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=-20.000000)
         StartSizeRange=(X=(Min=150.000000,Max=150.000000),Y=(Min=150.000000,Max=150.000000),Z=(Min=150.000000,Max=150.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.smoke.fog03'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
