class we_a987b8e2e26feb4f extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter_0
         StaticMesh=StaticMesh'FX_E_S.FX_E_fire.fx_e_fire_ani05'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.550000
         FadeOutStartTime=2.080000
         FadeOut=True
         FadeInEndTime=1.200000
         FadeIn=True
         MaxParticles=20
         Disabled=True
         Name="MeshEmitter0"
         StartLocationRange=(X=(Min=-300.000000,Max=300.000000),Y=(Min=-300.000000,Max=300.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=0.200000,Max=0.200000),Y=(Min=0.200000,Max=0.200000),Z=(Min=0.200000,Max=0.200000))
         DrawStyle=PTDS_AlphaBlend
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.460000
         MaxParticles=1
         Name="SpriteEmitter6"
         SpinParticles=True
         StartSpinRange=(X=(Min=0.500000,Max=0.500000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         UniformSize=True
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'FX_E_T.Flameset_a.FX_E_fire_ani3_001'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
