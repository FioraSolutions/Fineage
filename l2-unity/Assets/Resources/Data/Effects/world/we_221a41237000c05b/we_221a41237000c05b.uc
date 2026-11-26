class we_221a41237000c05b extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter_0
         StaticMesh=StaticMesh'Schtgart_SilentMona_S.FX.SilentMona_2f_LightFX04'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.370000
         FadeOutStartTime=4.000000
         MaxParticles=1
         WeatherSoundCheck=True
         Name="MeshEmitter21"
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=1.000000,Z=0.500000)
         StartSizeRange=(X=(Min=1.200000,Max=1.200000),Y=(Min=1.200000,Max=1.200000),Z=(Min=0.800000,Max=0.800000))
         LifetimeRange=(Min=50.000000,Max=50.000000)
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=50.000000
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=98,G=98,R=98,A=255),RelativeTime=0.500000)
         ColorScale(2)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorScaleRepeats=2.000000
         Opacity=0.370000
         FadeOutStartTime=1.890000
         FadeOut=True
         FadeInEndTime=0.660000
         FadeIn=True
         MaxParticles=9
         Name="SpriteEmitter0"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=550.000000)
         StartLocationRange=(X=(Min=-50.000000,Max=50.000000),Y=(Min=-50.000000,Max=50.000000),Z=(Min=-70.000000,Max=200.000000))
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.005000,Max=0.010000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=50.000000,Max=90.000000),Y=(Min=50.000000,Max=90.000000),Z=(Min=50.000000,Max=90.000000))
         UniformSize=True
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1071'
         LifetimeRange=(Min=3.000000,Max=3.000000)
         StartVelocityRange=(X=(Min=-1.000000,Max=1.000000),Y=(Min=-1.000000,Max=1.000000),Z=(Min=-5.000000,Max=5.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=3.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
     Begin Object Class=BeamEmitter Name=BeamEmitter_2
         LowFrequencyNoiseRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=1200.000000,Max=1200.000000))
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.270000
         FadeOutStartTime=4.640000
         FadeOut=True
         FadeInEndTime=3.440000
         FadeIn=True
         Name="BeamEmitter0"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=200.000000)
         StartLocationRange=(X=(Min=-50.000000,Max=50.000000),Y=(Min=-50.000000,Max=50.000000),Z=(Min=-10.000000,Max=10.000000))
         StartSizeRange=(X=(Min=10.000000,Max=70.000000),Y=(Min=10.000000,Max=70.000000),Z=(Min=10.000000,Max=70.000000))
         Texture=Texture'FX_E_T.fx_teleport.fx_teleport10'
         BlendBetweenSubdivisions=True
         LifetimeRange=(Min=7.000000,Max=8.000000)
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(2)=BeamEmitter'BeamEmitter_2'
}
