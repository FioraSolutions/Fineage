class we_f8314ed55cf2337e extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter_0
         StaticMesh=StaticMesh'Rune_Castle_S.runecastle_posione_01'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         FadeOutStartTime=12.000000
         MaxParticles=2
         WeatherSoundCheck=True
         Name="MeshEmitter0"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=2.000000,Max=2.000000))
         InitialParticlesPerSecond=1000000.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_AlphaBlend
         LifetimeRange=(Min=12.000000,Max=12.000000)
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter_0'
     Begin Object Class=MeshEmitter Name=MeshEmitter_1
         StaticMesh=StaticMesh'Rune_Castle_S.runecastle_posione_01'
         UseMeshBlendMode=False
         RenderTwoSided=True
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=113,G=113,R=113,A=255),RelativeTime=1.000000)
         Opacity=0.450000
         FadeOutStartTime=0.780000
         FadeOut=True
         FadeInEndTime=0.780000
         FadeIn=True
         MaxParticles=1
         Name="MeshEmitter1"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=2.000000,Max=2.000000))
         InitialParticlesPerSecond=10000.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=2.000000,Max=2.000000)
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter_1'
     Begin Object Class=MeshEmitter Name=MeshEmitter_2
         StaticMesh=StaticMesh'FX_E_S.Etc.aden_poison6'
         UseMeshBlendMode=False
         RenderTwoSided=True
         Acceleration=(X=0.000000,Y=0.000000,Z=70.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.260000
         FadeOutStartTime=0.250000
         FadeOut=True
         FadeInEndTime=0.250000
         FadeIn=True
         MaxParticles=5
         Name="MeshEmitter8"
         StartLocationRange=(X=(Min=-90.000000,Max=90.000000),Y=(Min=-95.000000,Max=95.000000),Z=(Min=0.000000,Max=0.000000))
         SpinParticles=True
         StartSizeRange=(X=(Min=0.100000,Max=0.300000),Y=(Min=0.100000,Max=0.300000),Z=(Min=0.500000,Max=0.500000))
         InitialParticlesPerSecond=10.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=0.800000,Max=1.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=4.000000,Max=4.000000))
         GetVelocityDirectionFrom=PTVD_StartPositionAndOwner
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter_2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_3
         UseDirectionAs=PTDU_Up
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         FadeOutStartTime=0.105000
         FadeOut=True
         FadeInEndTime=0.105000
         FadeIn=True
         Name="SpriteEmitter0"
         StartLocationRange=(X=(Min=-90.000000,Max=90.000000),Y=(Min=-95.000000,Max=95.000000),Z=(Min=10.000000,Max=10.000000))
         StartSizeRange=(X=(Min=2.000000,Max=3.000000),Y=(Min=20.000000,Max=30.000000),Z=(Min=20.000000,Max=30.000000))
         InitialParticlesPerSecond=10000.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.particles_etc.21_18_posion9'
         LifetimeRange=(Min=0.500000,Max=0.700000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=35.000000,Max=40.000000))
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter_3'
}
