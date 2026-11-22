class we_95880f3337918828 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Up
         ProjectionNormal=(X=1.000000,Y=0.000000,Z=0.000000)
         ColorScale(0)=(Color=(B=0,G=128,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=0,G=128,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=1.000000,Max=1.000000),Y=(Min=0.800000,Max=0.800000),Z=(Min=0.300000,Max=0.300000))
         Opacity=0.450000
         FadeOutStartTime=0.960000
         FadeOut=True
         FadeInEndTime=0.180000
         FadeIn=True
         MaxParticles=5
         WeatherSoundCheck=True
         Name="Glow"
         StartLocationOffset=(X=-2.000000,Y=-5.000000,Z=20.000000)
         StartLocationRange=(X=(Min=-5.000000,Max=5.000000),Y=(Min=-5.000000,Max=5.000000),Z=(Min=0.000000,Max=0.000000))
         SpinParticles=True
         StartSpinRange=(X=(Min=-0.010000,Max=0.010000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseRegularSizeScale=False
         StartSizeRange=(X=(Min=120.000000,Max=220.000000),Y=(Min=220.000000,Max=220.000000),Z=(Min=220.000000,Max=220.000000))
         UniformSize=True
         InitialParticlesPerSecond=4.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_Brighten
         Texture=None
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=True
         SubdivisionEnd=15
         LifetimeRange=(Min=1.000000,Max=1.200000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=3.000000,Max=3.000000))
         VelocityLossRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=1.000000,Max=1.000000))
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.100000
         FadeOutStartTime=0.680000
         FadeOut=True
         FadeInEndTime=0.620000
         FadeIn=True
         MaxParticles=50
         Name="smoke"
         StartLocationOffset=(X=2.000000,Y=10.000000,Z=0.000000)
         SpinParticles=True
         StartSpinRange=(X=(Min=-1.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=220.000000,Max=220.000000),Y=(Min=220.000000,Max=220.000000),Z=(Min=220.000000,Max=220.000000))
         UniformSize=True
         InitialParticlesPerSecond=0.800000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_Darken
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=True
         SubdivisionStart=4
         SubdivisionEnd=7
         LifetimeRange=(Min=2.000000,Max=2.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=200.000000,Max=200.000000))
         VelocityLossRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.500000,Max=0.500000))
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
