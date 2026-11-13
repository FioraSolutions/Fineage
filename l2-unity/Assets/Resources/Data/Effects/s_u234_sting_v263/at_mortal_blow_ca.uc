class at_mortal_blow_ca extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter2
         // Reference: MeshEmitter'lineageeffect.dec.at_mortal_blow_ca.MeshEmitter2'
         begin object name="MeshEmitter2" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.3571430,Color=(R=255,G=255,B=255,A=191))
         ColorScale[2]=(RelativeTime=0.7892860,Color=(R=128,G=128,B=128,A=255))
         ColorScale[3]=(RelativeTime=0.9142860,Color=(R=255,G=255,B=255,A=191))
         ColorScale[4]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=15.0000000
         Opacity=0.7200000
         FadeOutStartTime=0.2520000
         FadeOut=true
         FadeInEndTime=0.1845000
         FadeIn=true
         MaxParticles=11
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="MeshEmitter2"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.7000000,RelativeSize=0.5000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
         StartSizeRange=(X=(Min=0.1100000,Max=0.2200000),Y=(Min=0.1100000,Max=0.2200000),Z=(Min=0.1100000,Max=0.2200000))
         InitialParticlesPerSecond=9.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.4500000,Max=0.4500000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.at_mortal_blow_ca.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.3000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[2]=(RelativeTime=0.8571430,Color=(R=93,G=93,B=93,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=20.0000000
         Opacity=0.8000000
         FadeOutStartTime=1.2000000
         FadeOut=true
         FadeInEndTime=0.4350000
         FadeIn=true
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="SpriteEmitter2"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.2500000,RelativeSize=1.4000000)
         SizeScale[1]=(RelativeTime=0.6500000,RelativeSize=1.7000000)
         SizeScale[2]=(RelativeTime=0.9000000,RelativeSize=1.9000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.9000000)
         StartSizeRange=(X=(Min=4.1799998,Max=4.1799998),Y=(Min=4.1799998,Max=4.1799998),Z=(Min=4.1799998,Max=4.1799998))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0002'
         LifetimeRange=(Min=1.5000000,Max=1.5000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter2'
     Begin Object Class=MeshEmitter Name=MeshEmitter6
         // Reference: MeshEmitter'lineageeffect.dec.at_mortal_blow_ca.MeshEmitter6'
         begin object name="MeshEmitter6" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorScaleRepeats=1.0000000
         FadeOutStartTime=0.0690000
         FadeOut=true
         FadeInEndTime=0.0390000
         FadeIn=true
         MaxParticles=7
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="MeshEmitter5"
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.3000000),Y=(Min=0.3000000,Max=0.3000000),Z=(Min=0.3000000,Max=0.3000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         StartSizeRange=(X=(Min=0.2750000,Max=0.2750000),Y=(Min=0.2750000,Max=0.2750000),Z=(Min=0.2750000,Max=0.2750000))
         UniformSize=true
         InitialParticlesPerSecond=10.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         InitialDelayRange=(Min=0.5000000,Max=0.5000000)
         StartVelocityRange=(X=(Min=-33.0000000,Max=33.0000000),Y=(Min=-33.0000000,Max=33.0000000),Z=(Min=-33.0000000,Max=33.0000000))
         VelocityLossRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter6'
     // Reference: MeshEmitter'lineageeffect.dec.at_mortal_blow_ca.MeshEmitter2'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.3571430,Color=(R=255,G=255,B=255,A=191))
     ColorScale[2]=(RelativeTime=0.7892860,Color=(R=128,G=128,B=128,A=255))
     ColorScale[3]=(RelativeTime=0.9142860,Color=(R=255,G=255,B=255,A=191))
     ColorScale[4]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=15.0000000
     Opacity=0.7200000
     FadeOutStartTime=0.2520000
     FadeOut=true
     FadeInEndTime=0.1845000
     FadeIn=true
     MaxParticles=11
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="MeshEmitter2"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.7000000,RelativeSize=0.5000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
     StartSizeRange=(X=(Min=0.1100000,Max=0.2200000),Y=(Min=0.1100000,Max=0.2200000),Z=(Min=0.1100000,Max=0.2200000))
     InitialParticlesPerSecond=9.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.4500000,Max=0.4500000)
     Emitters[0]=MeshEmitter2
     // Reference: SpriteEmitter'lineageeffect.dec.at_mortal_blow_ca.SpriteEmitter2'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.3000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[2]=(RelativeTime=0.8571430,Color=(R=93,G=93,B=93,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=20.0000000
     Opacity=0.8000000
     FadeOutStartTime=1.2000000
     FadeOut=true
     FadeInEndTime=0.4350000
     FadeIn=true
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="SpriteEmitter2"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.2500000,RelativeSize=1.4000000)
     SizeScale[1]=(RelativeTime=0.6500000,RelativeSize=1.7000000)
     SizeScale[2]=(RelativeTime=0.9000000,RelativeSize=1.9000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=1.9000000)
     StartSizeRange=(X=(Min=4.1799998,Max=4.1799998),Y=(Min=4.1799998,Max=4.1799998),Z=(Min=4.1799998,Max=4.1799998))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0002'
     LifetimeRange=(Min=1.5000000,Max=1.5000000)
     Emitters[1]=SpriteEmitter2
     // Reference: MeshEmitter'lineageeffect.dec.at_mortal_blow_ca.MeshEmitter6'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorScaleRepeats=1.0000000
     FadeOutStartTime=0.0690000
     FadeOut=true
     FadeInEndTime=0.0390000
     FadeIn=true
     MaxParticles=7
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="MeshEmitter5"
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.3000000),Y=(Min=0.3000000,Max=0.3000000),Z=(Min=0.3000000,Max=0.3000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     StartSizeRange=(X=(Min=0.2750000,Max=0.2750000),Y=(Min=0.2750000,Max=0.2750000),Z=(Min=0.2750000,Max=0.2750000))
     UniformSize=true
     InitialParticlesPerSecond=10.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     InitialDelayRange=(Min=0.5000000,Max=0.5000000)
     StartVelocityRange=(X=(Min=-33.0000000,Max=33.0000000),Y=(Min=-33.0000000,Max=33.0000000),Z=(Min=-33.0000000,Max=33.0000000))
     VelocityLossRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
     Emitters[2]=MeshEmitter6
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.0500000
}
