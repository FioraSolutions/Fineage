class we_bcb26513dc570e49 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.120000
         FadeOutStartTime=2.640000
         FadeOut=True
         FadeInEndTime=0.800000
         FadeIn=True
         MaxParticles=4
         WeatherSoundCheck=True
         Name="SpriteEmitter1"
         StartLocationRange=(X=(Min=-100.000000,Max=100.000000),Y=(Min=-100.000000,Max=100.000000),Z=(Min=158.000000,Max=158.000000))
         StartSizeRange=(X=(Min=370.000000,Max=370.000000),Y=(Min=370.000000,Max=370.000000),Z=(Min=370.000000,Max=370.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.particles_etc.FX_e_light_map02'
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
         MaxParticles=13
         Name="SpriteEmitter2"
         StartLocationRange=(X=(Min=-200.000000,Max=200.000000),Y=(Min=-200.000000,Max=200.000000),Z=(Min=70.000000,Max=70.000000))
         StartSizeRange=(X=(Min=170.000000,Max=270.000000),Y=(Min=170.000000,Max=270.000000),Z=(Min=170.000000,Max=270.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.LightGlowSet.npc_2f_etc_G'
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=70.000000,Max=70.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_2
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.250000
         FadeOutStartTime=1.410000
         FadeOut=True
         FadeInEndTime=0.570000
         FadeIn=True
         Name="SpriteEmitter50"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=120.000000)
         StartLocationRange=(X=(Min=-200.000000,Max=200.000000),Y=(Min=-200.000000,Max=200.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=80.000000,Max=100.000000),Y=(Min=80.000000,Max=100.000000),Z=(Min=80.000000,Max=100.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.smoke.Fx_e_hotsmoke_t001'
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
