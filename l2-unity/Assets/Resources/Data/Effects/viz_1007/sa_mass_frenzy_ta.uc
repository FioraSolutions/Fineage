class sa_mass_frenzy_ta extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter3
         // Reference: MeshEmitter'lineageeffect.dec.sa_mass_frenzy_ta.MeshEmitter3'
         begin object name="MeshEmitter3" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.orc.orc_magic00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.8750000,Color=(R=188,G=188,B=188,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=12.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.7000000,Max=0.7000000))
         FadeOutStartTime=0.8850000
         FadeOut=true
         FadeInEndTime=0.0900000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="MC"
         SpinParticles=true
         SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
         StartSizeRange=(X=(Min=0.1200000,Max=0.1200000),Y=(Min=0.1200000,Max=0.1200000),Z=(Min=0.1200000,Max=0.1200000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=1.5000000,Max=1.5000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter3'
     Begin Object Class=MeshEmitter Name=MeshEmitter4
         // Reference: MeshEmitter'lineageeffect.dec.sa_mass_frenzy_ta.MeshEmitter4'
         begin object name="MeshEmitter4" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Summon.summon01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7990000,Max=0.7990000),Z=(Min=0.5690000,Max=0.5690000))
         Opacity=0.4000000
         FadeOutStartTime=0.2010000
         FadeOut=true
         FadeInEndTime=0.0330000
         FadeIn=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="ParticleMesh"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-100.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.9500000)
         StartSizeRange=(X=(Min=0.2800000,Max=0.2800000),Y=(Min=0.2800000,Max=0.2800000),Z=(Min=1.2000000,Max=1.2000000))
         InitialParticlesPerSecond=10.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1000.0000000,Max=1000.0000000))
         VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=8.0000000,Max=8.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter4'
     // Reference: MeshEmitter'lineageeffect.dec.sa_mass_frenzy_ta.MeshEmitter3'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.orc.orc_magic00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.8750000,Color=(R=188,G=188,B=188,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=12.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.7000000,Max=0.7000000))
     FadeOutStartTime=0.8850000
     FadeOut=true
     FadeInEndTime=0.0900000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="MC"
     SpinParticles=true
     SpinCCWorCW=(X=1.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=1.2000000)
     StartSizeRange=(X=(Min=0.1200000,Max=0.1200000),Y=(Min=0.1200000,Max=0.1200000),Z=(Min=0.1200000,Max=0.1200000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=1.5000000,Max=1.5000000)
     Emitters[0]=MeshEmitter3
     // Reference: MeshEmitter'lineageeffect.dec.sa_mass_frenzy_ta.MeshEmitter4'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Summon.summon01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7990000,Max=0.7990000),Z=(Min=0.5690000,Max=0.5690000))
     Opacity=0.4000000
     FadeOutStartTime=0.2010000
     FadeOut=true
     FadeInEndTime=0.0330000
     FadeIn=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="ParticleMesh"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-100.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.9500000)
     StartSizeRange=(X=(Min=0.2800000,Max=0.2800000),Y=(Min=0.2800000,Max=0.2800000),Z=(Min=1.2000000,Max=1.2000000))
     InitialParticlesPerSecond=10.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1000.0000000,Max=1000.0000000))
     VelocityLossRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=8.0000000,Max=8.0000000))
     Emitters[1]=MeshEmitter4
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.1000000
     bDirectional=true
}
