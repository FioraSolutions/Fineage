class we_c37375da6aa76331 extends Emitter;

defaultproperties
{
     Begin Object Class=BeamEmitter Name=BeamEmitter_0
         BeamEndPoints(0)=(offset=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=300.000000,Max=500.000000)),ActorTag="None",Weight=0.000000)
         DetermineEndPointBy=PTEP_Offset
         BeamTextureVScale=3.000000
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.200000,Max=0.300000),Y=(Min=0.220000,Max=0.400000),Z=(Min=0.280000,Max=0.450000))
         Opacity=0.200000
         FadeOutStartTime=5.670000
         FadeOut=True
         FadeInEndTime=1.470000
         FadeIn=True
         MaxParticles=15
         WeatherSoundCheck=True
         StartLocationRange=(X=(Min=-30.000000,Max=30.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=-10.000000,Max=10.000000))
         SphereRadiusRange=(Min=20.000000,Max=20.000000)
         StartSizeRange=(X=(Min=2.000000,Max=70.000000),Y=(Min=2.000000,Max=70.000000),Z=(Min=2.000000,Max=70.000000))
         Texture=Texture'FX_E_T.Etc.fx_light_s01'
         LifetimeRange=(Min=7.000000,Max=7.000000)
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=10.000000
     End Object
     Emitters(0)=BeamEmitter'BeamEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=70,G=70,R=70,A=255),RelativeTime=0.207143)
         ColorScale(2)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.400000)
         ColorScale(3)=(Color=(B=77,G=77,R=77,A=255),RelativeTime=0.596429)
         ColorScale(4)=(Color=(B=250,G=250,R=250,A=255),RelativeTime=0.782143)
         ColorScale(5)=(Color=(B=75,G=75,R=75,A=255),RelativeTime=1.000000)
         ColorScaleRepeats=5.000000
         ColorMultiplierRange=(X=(Min=0.800000,Max=1.000000),Y=(Min=0.800000,Max=1.000000),Z=(Min=0.800000,Max=1.000000))
         Opacity=0.320000
         MaxParticles=8
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=150.000000)
         StartLocationRange=(X=(Min=-20.000000,Max=20.000000),Y=(Min=-20.000000,Max=20.000000),Z=(Min=-20.000000,Max=20.000000))
         SpinParticles=True
         SpinsPerSecondRange=(X=(Min=0.100000,Max=0.100000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.480000)
         SizeScale(2)=(RelativeSize=0.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=5.000000,Max=50.000000),Y=(Min=5.000000,Max=50.000000),Z=(Min=5.000000,Max=50.000000))
         UniformSize=True
         Texture=Texture'FX_M_T.M_FX_particle.fx_head_texture011'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=2
         SubdivisionEnd=2
         UseRandomSubdivision=True
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=-10.000000,Max=10.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
