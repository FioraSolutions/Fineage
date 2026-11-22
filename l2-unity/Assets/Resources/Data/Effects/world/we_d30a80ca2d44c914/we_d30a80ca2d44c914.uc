class we_d30a80ca2d44c914 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         FadeOutStartTime=3.360000
         FadeOut=True
         FadeInEndTime=0.720000
         FadeIn=True
         MaxParticles=2
         Name="SpriteEmitter0"
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.200000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=15.000000,Max=15.000000),Y=(Min=15.000000,Max=15.000000),Z=(Min=15.000000,Max=15.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.LightGlowSet.LIGHT_GLOW01'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.410000
         MaxParticles=2
         Name="SpriteEmitter397"
         StartSizeRange=(X=(Min=30.000000,Max=30.000000),Y=(Min=30.000000,Max=30.000000),Z=(Min=30.000000,Max=30.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.particles_etc.o_fx_fire03'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_2
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=106,G=106,R=106,A=255),RelativeTime=0.214286)
         ColorScale(2)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.428571)
         ColorScale(3)=(Color=(B=103,G=103,R=103,A=255),RelativeTime=0.617857)
         ColorScale(4)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.810714)
         ColorScale(5)=(Color=(B=110,G=110,R=110,A=255),RelativeTime=1.000000)
         ColorScaleRepeats=2.000000
         ColorMultiplierRange=(X=(Min=0.500000,Max=1.000000),Y=(Min=0.500000,Max=1.000000),Z=(Min=0.500000,Max=1.000000))
         FadeOutStartTime=2.600000
         FadeOut=True
         FadeInEndTime=0.880000
         FadeIn=True
         Name="SpriteEmitter398"
         StartLocationShape=PTLS_Sphere
         SphereRadiusRange=(Min=10.000000,Max=15.000000)
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.100000,Max=0.200000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.500000)
         SizeScale(2)=(RelativeSize=0.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=5.000000,Max=12.000000),Y=(Min=5.000000,Max=12.000000),Z=(Min=5.000000,Max=12.000000))
         UniformSize=True
         InitialParticlesPerSecond=5.000000
         AutomaticInitialSpawning=False
         Texture=Texture'FX_E_T.particles_etc.elf_particleA_044'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         BlendBetweenSubdivisions=True
         SubdivisionStart=1
         SubdivisionEnd=1
         UseRandomSubdivision=True
         StartVelocityRange=(X=(Min=-5.000000,Max=5.000000),Y=(Min=-5.000000,Max=5.000000),Z=(Min=-5.000000,Max=5.000000))
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter_2'
}
