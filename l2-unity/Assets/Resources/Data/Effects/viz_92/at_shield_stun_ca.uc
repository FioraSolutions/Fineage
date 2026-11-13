class at_shield_stun_ca extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter16
         // Reference: MeshEmitter'lineageeffect.dec.at_shield_stun_ca.MeshEmitter16'
         begin object name="MeshEmitter16" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4821430,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=24.0000000
         Opacity=0.8000000
         FadeOutStartTime=1.2000000
         FadeOut=true
         FadeInEndTime=0.4000000
         FadeIn=true
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="Light"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.7800000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0000000)
         StartSizeRange=(X=(Min=0.0960000,Max=0.2400000),Y=(Min=0.0960000,Max=0.2400000),Z=(Min=0.0960000,Max=0.2400000))
         InitialParticlesPerSecond=8.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter16'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter18
         // Reference: SpriteEmitter'lineageeffect.dec.at_shield_stun_ca.SpriteEmitter18'
         begin object name="SpriteEmitter18" class=Engine.SpriteEmitter
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.1600000
         FadeOut=true
         FadeInEndTime=0.0800000
         FadeIn=true
         MaxParticles=11
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="Circle"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=15.6000004,Max=15.6000004),Y=(Min=15.6000004,Max=15.6000004),Z=(Min=15.6000004,Max=15.6000004))
         UniformSize=true
         InitialParticlesPerSecond=8.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=true
         SubdivisionStart=4
         SubdivisionEnd=8
         LifetimeRange=(Min=0.5000000,Max=0.5000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter18'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter19
         // Reference: SpriteEmitter'lineageeffect.dec.at_shield_stun_ca.SpriteEmitter19'
         begin object name="SpriteEmitter19" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4714290,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=23.0000000
         Opacity=0.2700000
         FadeOutStartTime=0.3600000
         FadeOut=true
         FadeInEndTime=0.1560000
         FadeIn=true
         MaxParticles=7
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="Center"
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.5000000)
         StartSizeRange=(X=(Min=24.6480007,Max=24.6480007),Y=(Min=24.6480007,Max=24.6480007),Z=(Min=24.6480007,Max=24.6480007))
         UniformSize=true
         InitialParticlesPerSecond=5.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
         LifetimeRange=(Min=0.6000000,Max=0.6000000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter19'
     Begin Object Class=MeshEmitter Name=MeshEmitter17
         // Reference: MeshEmitter'lineageeffect.dec.at_shield_stun_ca.MeshEmitter17'
         begin object name="MeshEmitter17" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.5550000,Max=0.5550000),Z=(Min=0.2380000,Max=0.2380000))
         FadeOutStartTime=3.0400000
         FadeOut=true
         FadeInEndTime=0.2660000
         FadeIn=true
         MaxParticles=4
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="elet"
         StartLocationRange=(X=(Min=-18.7199993,Max=18.7199993),Y=(Min=-18.7199993,Max=18.7199993),Z=(Min=-18.7199993,Max=18.7199993))
         StartLocationShape=1
         SphereRadiusRange=(Min=0.0000000,Max=4.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=-0.5000000,Max=0.5000000),Y=(Min=-0.5000000,Max=0.5000000),Z=(Min=-0.5000000,Max=0.5000000))
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.9100000,RelativeSize=0.8000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.3000000)
         StartSizeRange=(X=(Min=0.2808000,Max=0.4680000),Y=(Min=0.2808000,Max=0.4680000),Z=(Min=0.2808000,Max=0.4680000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=1.5000000,Max=1.5000000)
         end object
     End Object
     Emitters(3)=MeshEmitter'MeshEmitter17'
     // Reference: MeshEmitter'lineageeffect.dec.at_shield_stun_ca.MeshEmitter16'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4821430,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=24.0000000
     Opacity=0.8000000
     FadeOutStartTime=1.2000000
     FadeOut=true
     FadeInEndTime=0.4000000
     FadeIn=true
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="Light"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.7800000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0000000)
     StartSizeRange=(X=(Min=0.0960000,Max=0.2400000),Y=(Min=0.0960000,Max=0.2400000),Z=(Min=0.0960000,Max=0.2400000))
     InitialParticlesPerSecond=8.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     Emitters[0]=MeshEmitter16
     // Reference: SpriteEmitter'lineageeffect.dec.at_shield_stun_ca.SpriteEmitter18'
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.1600000
     FadeOut=true
     FadeInEndTime=0.0800000
     FadeIn=true
     MaxParticles=11
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="Circle"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=15.6000004,Max=15.6000004),Y=(Min=15.6000004,Max=15.6000004),Z=(Min=15.6000004,Max=15.6000004))
     UniformSize=true
     InitialParticlesPerSecond=8.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0033'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     BlendBetweenSubdivisions=true
     SubdivisionStart=4
     SubdivisionEnd=8
     LifetimeRange=(Min=0.5000000,Max=0.5000000)
     Emitters[1]=SpriteEmitter18
     // Reference: SpriteEmitter'lineageeffect.dec.at_shield_stun_ca.SpriteEmitter19'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4714290,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=23.0000000
     Opacity=0.2700000
     FadeOutStartTime=0.3600000
     FadeOut=true
     FadeInEndTime=0.1560000
     FadeIn=true
     MaxParticles=7
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="Center"
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.5000000)
     StartSizeRange=(X=(Min=24.6480007,Max=24.6480007),Y=(Min=24.6480007,Max=24.6480007),Z=(Min=24.6480007,Max=24.6480007))
     UniformSize=true
     InitialParticlesPerSecond=5.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
     LifetimeRange=(Min=0.6000000,Max=0.6000000)
     Emitters[2]=SpriteEmitter19
     // Reference: MeshEmitter'lineageeffect.dec.at_shield_stun_ca.MeshEmitter17'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge02'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.5550000,Max=0.5550000),Z=(Min=0.2380000,Max=0.2380000))
     FadeOutStartTime=3.0400000
     FadeOut=true
     FadeInEndTime=0.2660000
     FadeIn=true
     MaxParticles=4
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="elet"
     StartLocationRange=(X=(Min=-18.7199993,Max=18.7199993),Y=(Min=-18.7199993,Max=18.7199993),Z=(Min=-18.7199993,Max=18.7199993))
     StartLocationShape=1
     SphereRadiusRange=(Min=0.0000000,Max=4.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=-0.5000000,Max=0.5000000),Y=(Min=-0.5000000,Max=0.5000000),Z=(Min=-0.5000000,Max=0.5000000))
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=-1.0000000,Max=1.0000000),Z=(Min=-1.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.9100000,RelativeSize=0.8000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.3000000)
     StartSizeRange=(X=(Min=0.2808000,Max=0.4680000),Y=(Min=0.2808000,Max=0.4680000),Z=(Min=0.2808000,Max=0.4680000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=1.5000000,Max=1.5000000)
     Emitters[3]=MeshEmitter17
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.2000000
     bDirectional=true
}
