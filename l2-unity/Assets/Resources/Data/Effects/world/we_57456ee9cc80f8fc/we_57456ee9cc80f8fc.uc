class we_57456ee9cc80f8fc extends Emitter;

defaultproperties
{
     Begin Object Class=BeamEmitter Name=BeamEmitter_0
         LowFrequencyNoiseRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=-4000.000000,Max=-1000.000000))
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         Disabled=True
         Name="BeamEmitter0"
         StartLocationRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=1000.000000))
         Texture=Texture'FX_E_T.Etc.fx_e_posion_07'
         StartVelocityRange=(X=(Min=0.000000,Max=0.000000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=1000.000000))
     End Object
     Emitters(0)=BeamEmitter'BeamEmitter_0'
     Begin Object Class=MeshEmitter Name=MeshEmitter_1
         StaticMesh=StaticMesh'innadrile_eva_waterGarden_S.eva_dom_03'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=0.000000)
         ColorScale(1)=(Color=(B=255,G=255,R=255,A=255),RelativeTime=1.000000)
         FadeOutStartTime=2.800000
         FadeOut=True
         FadeInEndTime=1.040000
         FadeIn=True
         MaxParticles=1
         Disabled=True
         Name="MeshEmitter0"
         StartLocationOffset=(X=0.000000,Y=0.000000,Z=-100.000000)
         SpinParticles=True
         SpinCCWorCW=(X=1.000000,Y=1.000000,Z=0.500000)
         SpinsPerSecondRange=(X=(Min=0.003000,Max=0.003000),Y=(Min=0.000000,Max=0.000000),Z=(Min=0.000000,Max=0.000000))
         StartSizeRange=(X=(Min=0.500000,Max=0.500000),Y=(Min=0.500000,Max=0.500000),Z=(Min=0.500000,Max=0.500000))
         DrawStyle=PTDS_AlphaBlend
         WarmupTicksPerSecond=1.000000
         RelativeWarmupTime=4.000000
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter_1'
}
