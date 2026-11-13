class s_u009_a extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter11
         // Reference: MeshEmitter'lineageeffect.dec.s_u009_a.MeshEmitter11'
         begin object name="MeshEmitter11" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Elf.sword_mesh00'
         UseMeshBlendMode=false
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=10.3400002
         FadeOut=true
         FadeInEndTime=0.4400000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="MeshEmitter6"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=50.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.1800000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.7600000,RelativeSize=0.8000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.5000000)
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=11.0000000,Max=11.0000000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=30.0000000,Max=30.0000000))
         VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1.5000000,Max=1.5000000))
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter11'
     Begin Object Class=MeshEmitter Name=MeshEmitter12
         // Reference: MeshEmitter'lineageeffect.dec.s_u009_a.MeshEmitter12'
         begin object name="MeshEmitter12" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power04'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4964290,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=30.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.6000000,Max=0.6000000))
         Opacity=0.2000000
         FadeOutStartTime=0.8000000
         FadeOut=true
         FadeInEndTime=0.2200000
         FadeIn=true
         MaxParticles=7
         RespawnDeadParticles=false
         Name="MeshEmitter8"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=70.0000000)
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=9.0000000)
         StartSizeRange=(X=(Min=0.0300000,Max=0.0300000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
         InitialParticlesPerSecond=1.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=3.0000000,Max=3.0000000)
         InitialDelayRange=(Min=1.5000000,Max=1.5000000)
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter12'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter13
         // Reference: SpriteEmitter'lineageeffect.dec.s_u009_a.SpriteEmitter13'
         begin object name="SpriteEmitter13" class=Engine.SpriteEmitter
         UseDirectionAs=4
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4714290,Color=(R=223,G=123,B=32,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=102,G=0,B=0,A=255))
         Opacity=0.4000000
         FadeOutStartTime=3.0000000
         FadeInEndTime=0.1200000
         FadeIn=true
         MaxParticles=7
         RespawnDeadParticles=false
         Name="SpriteEmitter7"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=70.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=9.0000000)
         StartSizeRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=10.0000000,Max=10.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=true
         SubdivisionStart=4
         SubdivisionEnd=6
         LifetimeRange=(Min=3.0000000,Max=3.0000000)
         InitialDelayRange=(Min=1.5000000,Max=1.5000000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter13'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter14
         // Reference: SpriteEmitter'lineageeffect.dec.s_u009_a.SpriteEmitter14'
         begin object name="SpriteEmitter14" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
         Opacity=0.8000000
         FadeOutStartTime=4.5500002
         FadeOut=true
         MaxParticles=7
         RespawnDeadParticles=false
         Name="SpriteEmitter8"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=70.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=8.0000000)
         SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=4.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
         StartSizeRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=8
         SubdivisionEnd=9
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         InitialDelayRange=(Min=1.4000000,Max=1.4000000)
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter14'
     Begin Object Class=MeshEmitter Name=MeshEmitter13
         // Reference: MeshEmitter'lineageeffect.dec.s_u009_a.MeshEmitter13'
         begin object name="MeshEmitter13" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_levelup00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.3142860,Color=(R=194,G=194,B=194,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=70.0000000
         Opacity=0.0200000
         FadeOutStartTime=7.3950000
         FadeOut=true
         FadeInEndTime=0.5950000
         FadeIn=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="MeshEmitter9"
         StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.6000000,Max=0.6000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=11.0000000,Max=11.0000000)
         end object
     End Object
     Emitters(4)=MeshEmitter'MeshEmitter13'
     // Reference: MeshEmitter'lineageeffect.dec.s_u009_a.MeshEmitter11'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Elf.sword_mesh00'
     UseMeshBlendMode=false
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=10.3400002
     FadeOut=true
     FadeInEndTime=0.4400000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="MeshEmitter6"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=50.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.1800000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.7600000,RelativeSize=0.8000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=0.5000000)
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=11.0000000,Max=11.0000000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=30.0000000,Max=30.0000000))
     VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1.5000000,Max=1.5000000))
     Emitters[0]=MeshEmitter11
     // Reference: MeshEmitter'lineageeffect.dec.s_u009_a.MeshEmitter12'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power04'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4964290,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=30.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.6000000,Max=0.6000000))
     Opacity=0.2000000
     FadeOutStartTime=0.8000000
     FadeOut=true
     FadeInEndTime=0.2200000
     FadeIn=true
     MaxParticles=7
     RespawnDeadParticles=false
     Name="MeshEmitter8"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=70.0000000)
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=9.0000000)
     StartSizeRange=(X=(Min=0.0300000,Max=0.0300000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.1000000,Max=0.1000000))
     InitialParticlesPerSecond=1.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=3.0000000,Max=3.0000000)
     InitialDelayRange=(Min=1.5000000,Max=1.5000000)
     Emitters[1]=MeshEmitter12
     // Reference: SpriteEmitter'lineageeffect.dec.s_u009_a.SpriteEmitter13'
     UseDirectionAs=4
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4714290,Color=(R=223,G=123,B=32,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=102,G=0,B=0,A=255))
     Opacity=0.4000000
     FadeOutStartTime=3.0000000
     FadeInEndTime=0.1200000
     FadeIn=true
     MaxParticles=7
     RespawnDeadParticles=false
     Name="SpriteEmitter7"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=70.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=9.0000000)
     StartSizeRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=10.0000000,Max=10.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     BlendBetweenSubdivisions=true
     SubdivisionStart=4
     SubdivisionEnd=6
     LifetimeRange=(Min=3.0000000,Max=3.0000000)
     InitialDelayRange=(Min=1.5000000,Max=1.5000000)
     Emitters[2]=SpriteEmitter13
     // Reference: SpriteEmitter'lineageeffect.dec.s_u009_a.SpriteEmitter14'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.8000000,Max=0.8000000))
     Opacity=0.8000000
     FadeOutStartTime=4.5500002
     FadeOut=true
     MaxParticles=7
     RespawnDeadParticles=false
     Name="SpriteEmitter8"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=70.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=8.0000000)
     SizeScale[1]=(RelativeTime=0.2000000,RelativeSize=4.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.2000000)
     StartSizeRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=2.0000000,Max=2.0000000),Z=(Min=2.0000000,Max=2.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=8
     SubdivisionEnd=9
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     InitialDelayRange=(Min=1.4000000,Max=1.4000000)
     Emitters[3]=SpriteEmitter14
     // Reference: MeshEmitter'lineageeffect.dec.s_u009_a.MeshEmitter13'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_levelup00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.3142860,Color=(R=194,G=194,B=194,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=70.0000000
     Opacity=0.0200000
     FadeOutStartTime=7.3950000
     FadeOut=true
     FadeInEndTime=0.5950000
     FadeIn=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="MeshEmitter9"
     StartSizeRange=(X=(Min=0.1000000,Max=0.1000000),Y=(Min=0.1000000,Max=0.1000000),Z=(Min=0.6000000,Max=0.6000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=11.0000000,Max=11.0000000)
     Emitters[4]=MeshEmitter13
     AutoDestroy=true
     Physics=10
     bUseDynamicLights=false
     bNoDelete=false
     bTrailerPrePivot=true
     bAcceptsProjectors=false
     DrawScale=0.1000000
}
