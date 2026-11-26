class we_0d1a669fcb9afaa1 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.150000
         FadeOutStartTime=1.100000
         FadeOut=True
         FadeInEndTime=0.540000
         FadeIn=True
         MaxParticles=2
         Name="SpriteEmitter0"
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.005000,Max=0.005000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=130.000000,Max=130.000000),Y=(Min=130.000000,Max=130.000000),Z=(Min=130.000000,Max=130.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.Etc.fx_m_t0003_big'
         LifetimeRange=(Min=2.000000,Max=2.000000)
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=2.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Disabled=True
         Name="SpriteEmitter1"
         StartLocationRange=(X=(Min=-100.000000,Max=100.000000),Y=(Min=-100.000000,Max=100.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=50.000000,Max=100.000000),Y=(Min=50.000000,Max=100.000000),Z=(Min=50.000000,Max=100.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.Etc.fx_m_t0070_big'
         TextureUSubdivisions=3
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=True
         SubdivisionStart=8
         SubdivisionEnd=8
         UseRandomSubdivision=True
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=10.000000,Max=20.000000))
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
     Begin Object Class=MeshEmitter Name=MeshEmitter_2
         StaticMesh=StaticMesh'FX_E_S.Etc.magiccirclewhite01'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=0.500000,Max=0.800000),Y=(Min=0.700000,Max=0.700000),Z=(Min=0.500000,Max=0.500000))
         Opacity=0.430000
         FadeOutStartTime=0.390000
         FadeOut=True
         FadeInEndTime=0.300000
         FadeIn=True
         MaxParticles=30
         Disabled=True
         Name="MeshEmitter0"
         StartLocationRange=(X=(Min=-80.000000,Max=80.000000),Y=(Min=-80.000000,Max=80.000000),Z=(Min=-7.000000,Max=-7.000000))
         SpinParticles=True
         StartSpinRange=(X=(Min=0.000000,Max=1.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=0.200000,Max=1.000000),Y=(Min=0.200000,Max=1.000000),Z=(Min=0.200000,Max=0.500000))
         LifetimeRange=(Min=3.000000,Max=3.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=5.000000,Max=10.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=3.000000
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter_2'
     Begin Object Class=MeshEmitter Name=MeshEmitter_3
         StaticMesh=StaticMesh'FX_E_S.Etc.magiccirclewhite01'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.200000
         FadeOutStartTime=0.940000
         FadeOut=True
         FadeInEndTime=0.320000
         FadeIn=True
         MaxParticles=5
         Name="MeshEmitter3"
         SpinParticles=True
         SpinsPerSecondRange=(X=(Min=0.010000,Max=0.020000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=0.600000,Max=0.600000),Y=(Min=0.600000,Max=0.600000),Z=(Min=0.600000,Max=0.600000))
         LifetimeRange=(Min=2.000000,Max=2.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=4.000000,Max=10.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=2.000000
     End Object
     Emitters(3)=MeshEmitter'MeshEmitter_3'
}
