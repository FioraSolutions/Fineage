class we_9361b894c7e39570 extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter_0
         StaticMesh=StaticMesh'FX_E_S.Etc.fx_rune_light_beam_s03'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.410000
         FadeOutStartTime=4.000000
         MaxParticles=1
         Disabled=True
         Name="MeshEmitter0"
         StartSizeRange=(X=(Min=0.800000,Max=0.800000),Y=(Min=0.800000,Max=0.800000),Z=(Min=1.200000,Max=1.200000))
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter_0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter_1
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Opacity=0.240000
         FadeOutStartTime=2.280000
         FadeOut=True
         FadeInEndTime=1.120000
         FadeIn=True
         MaxParticles=2
         WeatherSoundCheck=True
         Name="SpriteEmitter6"
         StartSizeRange=(X=(Min=80.000000,Max=80.000000),Y=(Min=80.000000,Max=80.000000),Z=(Min=80.000000,Max=80.000000))
         UniformSize=True
         Texture=Texture'FX_E_T.particles_etc.FX_e_light_map03'
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter_1'
}
