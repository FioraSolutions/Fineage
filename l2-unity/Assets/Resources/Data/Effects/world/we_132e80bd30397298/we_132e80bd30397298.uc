class we_132e80bd30397298 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.240000
         MaxParticles=1
         Name="SpriteEmitter5"
         StartSizeRange=(X=(Min=180.000000,Max=180.000000),Y=(Min=180.000000,Max=180.000000),Z=(Min=180.000000,Max=180.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.Flameset_a.FX_E_fire_light02'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         Acceleration=(X=0.000000,Y=0.000000,Z=5.000000)
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=110,G=110,R=110,A=255),RelativeTime=0.117857)
         ColorScale(2)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.242857)
         ColorScale(3)=(Color=(B=110,G=110,R=110,A=255),RelativeTime=0.375000)
         ColorScale(4)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.514286)
         ColorScale(5)=(Color=(B=110,G=110,R=110,A=255),RelativeTime=0.646429)
         ColorScale(6)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.782143)
         ColorScale(7)=(Color=(B=110,G=110,R=110,A=255),RelativeTime=0.889286)
         ColorScale(8)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorScaleRepeats=1.000000
         ColorMultiplierRange=(X=(Min=0.700000,Max=1.000000),Y=(Min=0.700000,Max=1.000000),Z=(Min=0.700000,Max=1.000000))
         Opacity=0.470000
         FadeOutStartTime=3.920000
         FadeOut=True
         FadeInEndTime=1.050000
         FadeIn=True
         MaxParticles=20
         Name="SpriteEmitter2"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=80.000000)
         StartLocationRange=(X=(Min=-40.000000,Max=40.000000),Y=(Min=-40.000000,Max=40.000000),Z=(Min=0.000000,Max=40.000000))
         SpinParticles=True
         SpinsPerSecondRange=(X=(Min=0.100000,Max=0.300000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=2.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.500000)
         SizeScale(2)=(RelativeSize=0.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=10.000000,Max=20.000000),Y=(Min=10.000000,Max=20.000000),Z=(Min=10.000000,Max=20.000000))
         UniformSize=True
         DrawStyle=PTDS_Brighten
         Texture=Texture'FX_E_T.particles_etc.elf_particleC_03'
         LifetimeRange=(Min=7.000000,Max=7.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=10.000000,Max=30.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=7.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
