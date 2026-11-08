class su_summon_a_ca extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter4
         // Reference: MeshEmitter'lineageeffect.dec.su_summon_a_ca.MeshEmitter4'
         begin object name="MeshEmitter4" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Summon.summon00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5714290,Color=(R=205,G=211,B=226,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=50.0000000
         Opacity=0.6000000
         FadeOutStartTime=4.3000002
         FadeOut=true
         FadeInEndTime=1.0000000
         FadeIn=true
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="MC"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.0400000,Max=0.0400000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=0.2870000,Max=0.2870000),Y=(Min=0.2870000,Max=0.2870000),Z=(Min=0.2870000,Max=0.2870000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=5.0000000,Max=5.0000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter4'
     Begin Object Class=MeshEmitter Name=MeshEmitter3
         // Reference: MeshEmitter'lineageeffect.dec.su_summon_a_ca.MeshEmitter3'
         begin object name="MeshEmitter3" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Support.supportenchant02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.3100000
         FadeOutStartTime=0.2800000
         FadeOut=true
         FadeInEndTime=0.1100000
         FadeIn=true
         MaxParticles=20
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="RingsFlow"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=5.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=2.2000000)
         SizeScale[1]=(RelativeTime=0.2200000,RelativeSize=3.0000000)
         SizeScale[2]=(RelativeTime=0.4800000,RelativeSize=3.8000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=4.5000000)
         StartSizeRange=(X=(Min=0.4500000,Max=0.4500000),Y=(Min=0.4500000,Max=0.4500000),Z=(Min=2.0000000,Max=2.0000000))
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.8000000,Max=0.8000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-40.0000000,Max=-40.0000000))
         VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=2.0000000,Max=2.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter3'
     // Reference: MeshEmitter'lineageeffect.dec.su_summon_a_ca.MeshEmitter4'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Summon.summon00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5714290,Color=(R=205,G=211,B=226,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=50.0000000
     Opacity=0.6000000
     FadeOutStartTime=4.3000002
     FadeOut=true
     FadeInEndTime=1.0000000
     FadeIn=true
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="MC"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.0400000,Max=0.0400000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=0.2870000,Max=0.2870000),Y=(Min=0.2870000,Max=0.2870000),Z=(Min=0.2870000,Max=0.2870000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=5.0000000,Max=5.0000000)
     Emitters[0]=MeshEmitter4
     // Reference: MeshEmitter'lineageeffect.dec.su_summon_a_ca.MeshEmitter3'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Support.supportenchant02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.3100000
     FadeOutStartTime=0.2800000
     FadeOut=true
     FadeInEndTime=0.1100000
     FadeIn=true
     MaxParticles=20
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="RingsFlow"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=5.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=2.2000000)
     SizeScale[1]=(RelativeTime=0.2200000,RelativeSize=3.0000000)
     SizeScale[2]=(RelativeTime=0.4800000,RelativeSize=3.8000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=4.5000000)
     StartSizeRange=(X=(Min=0.4500000,Max=0.4500000),Y=(Min=0.4500000,Max=0.4500000),Z=(Min=2.0000000,Max=2.0000000))
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.8000000,Max=0.8000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=-40.0000000,Max=-40.0000000))
     VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=2.0000000,Max=2.0000000))
     Emitters[1]=MeshEmitter3
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.2500000
}
