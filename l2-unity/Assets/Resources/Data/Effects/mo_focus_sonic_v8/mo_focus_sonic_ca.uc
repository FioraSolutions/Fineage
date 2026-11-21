class mo_focus_sonic_ca extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.mo_focus_sonic_ca.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         UseDirectionAs=1
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.8000000
         FadeOutStartTime=0.1550000
         FadeOut=true
         FadeInEndTime=0.1350000
         FadeIn=true
         MaxParticles=50
         ForcedMaxParticles=true
         RespawnDeadParticles=false
         Name="RedIn"
         StartLocationShape=1
         SphereRadiusRange=(Min=45.0000000,Max=45.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.1000000)
         SizeScale[1]=(RelativeTime=0.8000000,RelativeSize=1.5000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
         StartSizeRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=2.0000000,Max=2.0000000))
         InitialParticlesPerSecond=120.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0132'
         TextureUSubdivisions=8
         TextureVSubdivisions=2
         SubdivisionStart=15
         SubdivisionEnd=16
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.2500000,Max=0.2500000)
         StartVelocityRange=(X=(Min=170.0000000,Max=170.0000000),Y=(Min=170.0000000,Max=170.0000000),Z=(Min=170.0000000,Max=170.0000000))
         GetVelocityDirectionFrom=1
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=MeshEmitter Name=MeshEmitter0
         // Reference: MeshEmitter'lineageeffect.dec.mo_focus_sonic_ca.MeshEmitter0'
         begin object name="MeshEmitter0" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.wispray00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorScale[1]=(RelativeTime=0.6464290,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=0.9250000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.5500000,Max=0.5500000),Z=(Min=0.2000000,Max=0.2000000))
         Opacity=0.3000000
         FadeOutStartTime=0.6000000
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Ray"
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.1000000),Z=(Min=0.0000000,Max=0.1000000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
         SizeScale[1]=(RelativeTime=0.6000000,RelativeSize=0.6000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         SizeScaleRepeats=6.0000000
         StartSizeRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.3000000,Max=0.5000000),Z=(Min=0.3000000,Max=0.5000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.6000000,Max=0.6000000)
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.mo_focus_sonic_ca.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.6250000,Color=(R=179,G=179,B=179,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=11.0000000
         Opacity=0.6000000
         FadeOutStartTime=0.5460000
         FadeOut=true
         FadeInEndTime=0.5040000
         FadeIn=true
         MaxParticles=1
         ForcedLifeTime=true
         ForcedFade=true
         RespawnDeadParticles=false
         Name="Core"
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
         SizeScale[1]=(RelativeTime=0.2400000,RelativeSize=1.8000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.8000000)
         StartSizeRange=(X=(Min=22.0000000,Max=22.0000000),Y=(Min=22.0000000,Max=22.0000000),Z=(Min=22.0000000,Max=22.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0002'
         LifetimeRange=(Min=0.6000000,Max=0.6000000)
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter1'
     // Reference: SpriteEmitter'lineageeffect.dec.mo_focus_sonic_ca.SpriteEmitter0'
     UseDirectionAs=1
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.8000000
     FadeOutStartTime=0.1550000
     FadeOut=true
     FadeInEndTime=0.1350000
     FadeIn=true
     MaxParticles=50
     ForcedMaxParticles=true
     RespawnDeadParticles=false
     Name="RedIn"
     StartLocationShape=1
     SphereRadiusRange=(Min=45.0000000,Max=45.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.1000000)
     SizeScale[1]=(RelativeTime=0.8000000,RelativeSize=1.5000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.0100000)
     StartSizeRange=(X=(Min=2.0000000,Max=2.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=2.0000000,Max=2.0000000))
     InitialParticlesPerSecond=120.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0132'
     TextureUSubdivisions=8
     TextureVSubdivisions=2
     SubdivisionStart=15
     SubdivisionEnd=16
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.2500000,Max=0.2500000)
     StartVelocityRange=(X=(Min=170.0000000,Max=170.0000000),Y=(Min=170.0000000,Max=170.0000000),Z=(Min=170.0000000,Max=170.0000000))
     GetVelocityDirectionFrom=1
     Emitters[0]=SpriteEmitter0
     // Reference: MeshEmitter'lineageeffect.dec.mo_focus_sonic_ca.MeshEmitter0'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.wispray00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorScale[1]=(RelativeTime=0.6464290,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=0.9250000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.5500000,Max=0.5500000),Z=(Min=0.2000000,Max=0.2000000))
     Opacity=0.3000000
     FadeOutStartTime=0.6000000
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Ray"
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.1000000),Y=(Min=0.0000000,Max=0.1000000),Z=(Min=0.0000000,Max=0.1000000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.0000000)
     SizeScale[1]=(RelativeTime=0.6000000,RelativeSize=0.6000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     SizeScaleRepeats=6.0000000
     StartSizeRange=(X=(Min=0.3000000,Max=0.5000000),Y=(Min=0.3000000,Max=0.5000000),Z=(Min=0.3000000,Max=0.5000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.6000000,Max=0.6000000)
     Emitters[1]=MeshEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.mo_focus_sonic_ca.SpriteEmitter1'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.6250000,Color=(R=179,G=179,B=179,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=11.0000000
     Opacity=0.6000000
     FadeOutStartTime=0.5460000
     FadeOut=true
     FadeInEndTime=0.5040000
     FadeIn=true
     MaxParticles=1
     ForcedLifeTime=true
     ForcedFade=true
     RespawnDeadParticles=false
     Name="Core"
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=2.0000000)
     SizeScale[1]=(RelativeTime=0.2400000,RelativeSize=1.8000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.8000000)
     StartSizeRange=(X=(Min=22.0000000,Max=22.0000000),Y=(Min=22.0000000,Max=22.0000000),Z=(Min=22.0000000,Max=22.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0002'
     LifetimeRange=(Min=0.6000000,Max=0.6000000)
     Emitters[2]=SpriteEmitter1
     AutoDestroy=true
     bRotEmitter=true
     bNoDelete=false
     DrawScale=0.2000000
     bDirectional=true
}
