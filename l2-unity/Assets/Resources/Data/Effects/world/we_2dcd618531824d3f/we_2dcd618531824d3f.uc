class we_2dcd618531824d3f extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=139,G=138,R=131,A=255),RelativeTime=1.000000)
         Opacity=0.880000
         FadeOutStartTime=2.640000
         FadeOut=True
         FadeInEndTime=1.320000
         FadeIn=True
         MaxParticles=100
         Name="SpriteEmitter2"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=200.000000))
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.000000,Max=0.300000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSpinRange=(X=(Min=-0.500000,Max=0.500000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.500000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.000000,RelativeTime=0.360000)
         SizeScale(2)=(RelativeSize=0.500000,RelativeTime=0.750000)
         SizeScale(3)=(RelativeSize=0.000000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=0.000000,Max=30.000000),Y=(Min=0.000000,Max=30.000000),Z=(Min=0.000000,Max=30.000000))
         UniformSize=True
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0069'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         SubdivisionStart=5
         SubdivisionEnd=6
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=100.000000))
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=MeshEmitter Name=MeshEmitter_1
         StaticMesh=StaticMesh'Oren_DEV_S.DE_effect'
         UseMeshBlendMode=False
         RenderTwoSided=True
         Acceleration=(X=0.000000,Y=0.000000,Z=50.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.720000
         MaxParticles=1
         Name="MeshEmitter2"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=50.000000)
         SpinParticles=True
         StartSpinRange=(X=(Min=0.250000,Max=0.250000),Y=(Min=0.250000,Max=0.250000),Z=(Min=0.250000,Max=0.250000))
         StartSizeRange=(X=(Min=1.000000,Max=1.000000),Y=(Min=1.200000,Max=1.200000),Z=(Min=1.000000,Max=1.000000))
         DrawStyle=PTDS_Brighten
         LifetimeRange=(Min=1.000000,Max=1.000000)
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter_1'
}
