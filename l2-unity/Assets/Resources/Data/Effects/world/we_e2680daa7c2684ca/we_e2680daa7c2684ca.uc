class we_e2680daa7c2684ca extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.160000
         FadeOutStartTime=0.800000
         FadeOut=True
         FadeInEndTime=0.400000
         FadeIn=True
         MaxParticles=3
         Name="SpriteEmitter10"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=200.000000)
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.000000,Max=0.300000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=3000.000000,Max=4000.000000),Y=(Min=3000.000000,Max=4000.000000),Z=(Min=3000.000000,Max=4000.000000))
         UniformSize=True
         InitialParticlesPerSecond=20.000000
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.smoke.up_fog'
         TextureUSubdivisions=1
         TextureVSubdivisions=1
         UseRandomSubdivision=True
         LifetimeRange=(Min=20.000000,Max=20.000000)
         StartVelocityRange=(X=(Min=-38.000000,Max=38.000000),Y=(Min=-38.000000,Max=38.000000),Z=(Min=-20.000000,Max=20.000000))
         VelocityLossRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         WarmupTicksPerSecond=2.000000
         RelativeWarmupTime=20.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         UseDirectionAs=PTDU_Normal
         Acceleration=(X=0.000000,Y=0.000000,Z=30.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.860000
         FadeOutStartTime=2.760000
         FadeOut=True
         FadeInEndTime=0.390000
         FadeIn=True
         MaxParticles=8
         Name="SpriteEmitter11"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=100.000000)
         SpinParticles=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.865000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=900.000000,Max=1000.000000),Y=(Min=900.000000,Max=1000.000000),Z=(Min=900.000000,Max=1000.000000))
         UniformSize=True
         InitialParticlesPerSecond=1000.000000
         DrawStyle=PTDS_Darken
         Texture=Texture'FX_E_T.smoke.fog03'
         TextureUSubdivisions=1
         TextureVSubdivisions=1
         UseRandomSubdivision=True
         LifetimeRange=(Min=3.000000,Max=3.000000)
         StartVelocityRange=(X=(Min=-38.000000,Max=38.000000),Y=(Min=-38.000000,Max=38.000000),Z=(Min=-81.360001,Max=81.360001))
         VelocityLossRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=3.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
