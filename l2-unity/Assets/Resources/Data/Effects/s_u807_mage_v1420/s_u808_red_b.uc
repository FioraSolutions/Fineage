class s_u808_red_b extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter2
         // Reference: MeshEmitter'lineageeffect.dec.s_u808_red_b.MeshEmitter2'
         begin object name="MeshEmitter2" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.membrane_01'
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOut=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="dome2"
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=17.0000000,Max=17.0000000),Y=(Min=17.0000000,Max=17.0000000),Z=(Min=7.5000000,Max=7.5000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=300.0000000,Max=300.0000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter2'
     // Reference: MeshEmitter'lineageeffect.dec.s_u808_red_b.MeshEmitter2'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.membrane_01'
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOut=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="dome2"
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=17.0000000,Max=17.0000000),Y=(Min=17.0000000,Max=17.0000000),Z=(Min=7.5000000,Max=7.5000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=300.0000000,Max=300.0000000)
     Emitters[0]=MeshEmitter2
     AutoDestroy=true
     bLightChanged=true
     bNoDelete=false
     bSunAffect=true
}
