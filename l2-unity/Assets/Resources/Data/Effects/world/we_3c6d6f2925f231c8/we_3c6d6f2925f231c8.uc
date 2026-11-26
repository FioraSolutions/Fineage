class we_3c6d6f2925f231c8 extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter_0
         StaticMesh=StaticMesh'FX_E_S.Etc.fx_e_box_light_s00'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorMultiplierRange=(X=(Min=1.000000,Max=1.000000),Y=(Min=0.500000,Max=0.500000),Z=(Min=0.500000,Max=0.500000))
         Opacity=0.150000
         FadeOutStartTime=1.640000
         FadeOut=True
         FadeInEndTime=1.120000
         FadeIn=True
         MaxParticles=2
         Name="MeshEmitter0"
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         FadeOutStartTime=1.760000
         FadeOut=True
         FadeInEndTime=1.400000
         FadeIn=True
         MaxParticles=3
         Name="SpriteEmitter12"
         StartLocationOffset=(X=0.000000,Y=-50.000000,Z=0.000000)
         SpinParticles=True
         SpinCCWorCW=(X=0.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.050000,Max=0.050000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.200000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=70.000000,Max=70.000000),Y=(Min=70.000000,Max=70.000000),Z=(Min=70.000000,Max=70.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.smoke.fog03'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_2
         UseDirectionAs=PTDU_Normal
         ProjectionNormal=(X=0.000000,Y=1.000000,Z=0.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.760000
         FadeOutStartTime=2.560000
         FadeOut=True
         FadeInEndTime=1.400000
         FadeIn=True
         MaxParticles=2
         Name="SpriteEmitter14"
         SpinParticles=True
         SpinCCWorCW=(X=0.000000,Y=0.500000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.010000,Max=0.010000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=140.000000,Max=140.000000),Y=(Min=140.000000,Max=140.000000),Z=(Min=140.000000,Max=140.000000))
         UniformSize=True
         DrawStyle=PTDS_Brighten
         Texture=Texture'FX_E_T.broadcasting_middletower_t.argos_fe_001'
         BlendBetweenSubdivisions=True
         SubdivisionEnd=1
         UseRandomSubdivision=True
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter_2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_3
         UseDirectionAs=PTDU_Normal
         ProjectionNormal=(X=0.000000,Y=1.000000,Z=0.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=95,G=95,R=95,A=255),RelativeTime=0.471429)
         ColorScale(2)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         ColorScaleRepeats=1.000000
         ColorMultiplierRange=(X=(Min=1.000000,Max=1.000000),Y=(Min=0.500000,Max=0.500000),Z=(Min=0.500000,Max=0.500000))
         Opacity=0.230000
         FadeOutStartTime=2.400000
         FadeOut=True
         FadeInEndTime=1.160000
         FadeIn=True
         MaxParticles=4
         Name="SpriteEmitter15"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=-90.000000)
         SpinParticles=True
         StartSpinRange=(X=(Min=0.250000,Max=0.250000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=70.000000,Max=70.000000),Y=(Min=70.000000,Max=70.000000),Z=(Min=70.000000,Max=70.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.Etc.fx_e_t001_13'
         LifetimeRange=(Min=7.000000,Max=7.000000)
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=30.000000,Max=30.000000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=7.000000
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter_3'
}
