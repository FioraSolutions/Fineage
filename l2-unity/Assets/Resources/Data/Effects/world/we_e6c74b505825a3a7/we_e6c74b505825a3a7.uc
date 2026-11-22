class we_e6c74b505825a3a7 extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_0
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.540000
         MaxParticles=2
         Name="SpriteEmitter2"
         SpinParticles=True
         StartSpinRange=(X=(Min=0.280000,Max=0.280000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=50.000000,Max=50.000000),Y=(Min=50.000000,Max=50.000000),Z=(Min=50.000000,Max=50.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'Rion_Orc_Village_T.rion_alpha01'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         UseDirectionAs=PTDU_Normal
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.150000
         FadeOutStartTime=2.120000
         FadeOut=True
         FadeInEndTime=0.920000
         FadeIn=True
         MaxParticles=2
         Name="SpriteEmitter4"
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000,RelativeTime=0.000000)
         SizeScale(1)=(RelativeSize=1.500000,RelativeTime=1.000000)
         StartSizeRange=(X=(Min=90.000000,Max=90.000000),Y=(Min=90.000000,Max=90.000000),Z=(Min=90.000000,Max=90.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.Flameset_a.FX_E_fire_light02'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
     Begin Object Class=MeshEmitter Name=MeshEmitter_2
         StaticMesh=StaticMesh'FX_E_S.Etc.magiccirclewhite01'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.180000
         FadeOutStartTime=3.000000
         FadeOut=True
         FadeInEndTime=1.000000
         FadeIn=True
         MaxParticles=2
         Name="MeshEmitter0"
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=1.000000,Z=1.000000)
         SpinsPerSecondRange=(X=(Min=0.003000,Max=0.004000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=0.300000,Max=0.300000),Y=(Min=0.300000,Max=0.300000),Z=(Min=0.300000,Max=0.300000))
         DrawStyle=PTDS_Brighten
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.500000,Max=0.500000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter_2'
}
