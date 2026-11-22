class we_4354a5ed4111ee92 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         Acceleration=(X=0.000000,Y=0.000000,Z=5.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.884000,Max=1.000000),Y=(Min=0.884000,Max=1.000000),Z=(Min=0.884000,Max=1.000000))
         Opacity=0.600000
         FadeOutStartTime=1.040000
         FadeOut=True
         FadeInEndTime=1.040000
         FadeIn=True
         MaxParticles=1200
         CheckLevelOfWeather=2
         WeatherEffect=True
         WeatherRangeTime(0)=(Min=0.000000,Max=2400.000000)
         WeatherSoundCheck=True
         Name="Dust"
         StartLocationRange=(X=(Min=-3000.000000,Max=3000.000000),Y=(Min=-3000.000000,Max=3000.000000),Z=(Min=0.000000,Max=200.000000))
         SpinParticles=True
         SpinsPerSecondRange=(X=(Min=0.000000,Max=0.030000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=-1.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=35.000000,Max=55.000000),Y=(Min=35.000000,Max=55.000000),Z=(Min=35.000000,Max=55.000000))
         UniformSize=True
         InitialParticlesPerSecond=200.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_Brighten
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1071'
         LifetimeRange=(Min=2.000000,Max=2.000000)
         StartVelocityRange=(X=(Min=-350.000000,Max=-150.000000),Y=(Min=-30.000000,Max=30.000000),Z=(Min=0.000000,Max=50.000000))
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         Acceleration=(X=0.000000,Y=0.000000,Z=5.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.900000,Max=1.000000),Y=(Min=0.900000,Max=1.000000),Z=(Min=0.900000,Max=1.000000))
         Opacity=0.200000
         FadeOutStartTime=2.080000
         FadeOut=True
         FadeInEndTime=1.960000
         FadeIn=True
         MaxParticles=80
         Name="Dust2"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=80.000000)
         StartLocationRange=(X=(Min=-1400.000000,Max=1400.000000),Y=(Min=-1400.000000,Max=1400.000000),Z=(Min=0.000000,Max=100.000000))
         SpinParticles=True
         SpinsPerSecondRange=(X=(Min=0.000000,Max=0.100000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=-1.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=290.000000,Max=350.000000),Y=(Min=290.000000,Max=350.000000),Z=(Min=290.000000,Max=350.000000))
         UniformSize=True
         InitialParticlesPerSecond=8.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_Brighten
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         SubdivisionEnd=8
         UseRandomSubdivision=True
         LifetimeRange=(Min=5.000000,Max=5.000000)
         StartVelocityRange=(X=(Min=-450.000000,Max=-250.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         VelocityLossRange=(X=(Min=0.200000,Max=0.200000),Y=(Min=0.200000,Max=0.200000),Z=(Min=0.000000,Max=0.000000))
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
