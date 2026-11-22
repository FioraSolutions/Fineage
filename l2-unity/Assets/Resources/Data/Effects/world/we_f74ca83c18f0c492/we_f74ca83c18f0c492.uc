class we_f74ca83c18f0c492 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.150000
         FadeOutStartTime=2.640000
         FadeOut=True
         FadeInEndTime=0.800000
         FadeIn=True
         MaxParticles=5
         WeatherSoundCheck=True
         Name="SpriteEmitter1"
         StartLocationRange=(X=(Min=-150.000000,Max=150.000000),Y=(Min=-150.000000,Max=150.000000),Z=(Min=158.000000,Max=158.000000))
         StartSizeRange=(X=(Min=320.000000,Max=320.000000),Y=(Min=320.000000,Max=320.000000),Z=(Min=320.000000,Max=320.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.LightGlowSet.npc_2f_etc_R'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         Acceleration=(X=0.000000,Y=0.000000,Z=-20.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.080000
         FadeOutStartTime=2.960000
         FadeOut=True
         FadeInEndTime=0.440000
         FadeIn=True
         MaxParticles=15
         Name="SpriteEmitter2"
         StartLocationRange=(X=(Min=-200.000000,Max=200.000000),Y=(Min=-200.000000,Max=200.000000),Z=(Min=70.000000,Max=70.000000))
         StartSizeRange=(X=(Min=220.000000,Max=270.000000),Y=(Min=220.000000,Max=270.000000),Z=(Min=220.000000,Max=270.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.LightGlowSet.npc_2f_etc_R'
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=70.000000,Max=70.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_2
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.190000
         FadeOutStartTime=1.470000
         FadeOut=True
         FadeInEndTime=0.420000
         FadeIn=True
         MaxParticles=20
         Name="SpriteEmitter50"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=120.000000)
         StartLocationRange=(X=(Min=-170.000000,Max=170.000000),Y=(Min=-170.000000,Max=170.000000),Z=(Min=0.000000,Max=0.000000))
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=0.700000,RelativeTime=1.000000)
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=None
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=True
         SubdivisionStart=8
         SubdivisionEnd=12
         UseRandomSubdivision=True
         LifetimeRange=(Min=2.000000,Max=3.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=15.000000,Max=30.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter_2'
}
