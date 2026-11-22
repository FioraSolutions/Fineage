class we_39dc63896674a365 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.100000,Max=0.100000),Y=(Min=0.500000,Max=0.500000),Z=(Min=1.000000,Max=1.000000))
         MaxParticles=1
         Name="SpriteEmitter8"
         StartSizeRange=(X=(Min=70.000000,Max=70.000000),Y=(Min=70.000000,Max=70.000000),Z=(Min=70.000000,Max=70.000000))
         UniformSize=True
         InitialParticlesPerSecond=100.000000
         AutomaticInitialSpawning=False
         Texture=Texture'FX_E_T.Etc.fx_m_t0006_big'
         TextureUSubdivisions=4
         TextureVSubdivisions=2
         SubdivisionStart=4
         SubdivisionEnd=4
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=90,G=90,R=90,A=255),RelativeTime=0.150000)
         ColorScale(2)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.303571)
         ColorScale(3)=(Color=(B=82,G=82,R=82,A=255),RelativeTime=0.471429)
         ColorScale(4)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.667857)
         ColorScale(5)=(Color=(B=110,G=110,R=110,A=255),RelativeTime=0.850000)
         ColorScale(6)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.500000,Max=0.800000),Y=(Min=0.500000,Max=0.800000),Z=(Min=1.000000,Max=1.000000))
         Opacity=0.670000
         FadeOutStartTime=3.280000
         FadeOut=True
         FadeInEndTime=0.600000
         FadeIn=True
         Name="SpriteEmitter205"
         StartLocationRange=(X=(Min=-20.000000,Max=20.000000),Y=(Min=-20.000000,Max=20.000000),Z=(Min=-20.000000,Max=20.000000))
         StartLocationShape=PTLS_Sphere
         SphereRadiusRange=(Min=20.000000,Max=40.000000)
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.020000,Max=0.020000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.500000)
         SizeScale(2)=(RelativeSize=0.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=5.000000,Max=8.000000),Y=(Min=5.000000,Max=8.000000),Z=(Min=5.000000,Max=8.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.particles_etc.elf_particleA_03'
         StartVelocityRange=(X=(Min=-7.000000,Max=7.000000),Y=(Min=-7.000000,Max=7.000000),Z=(Min=-7.000000,Max=7.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
