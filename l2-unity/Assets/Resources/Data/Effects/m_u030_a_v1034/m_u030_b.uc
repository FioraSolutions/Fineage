class m_u030_b extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter8
         // Reference: MeshEmitter'lineageeffect.dec.m_u030_b.MeshEmitter8'
         begin object name="MeshEmitter8" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.magiccircle.magiccirclewhite01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.4714290,Color=(R=191,G=191,B=191,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=90.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.6000000,Max=0.6000000))
         Opacity=0.4000000
         FadeOutStartTime=7.0000000
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Ring"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.0000000,Z=0.0000000)
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.0864000,Max=0.0864000),Y=(Min=0.0864000,Max=0.0864000),Z=(Min=0.2400000,Max=0.2400000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=7.0000000,Max=7.0000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter8'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.m_u030_b.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5714290,Color=(R=173,G=173,B=173,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=90.0000000
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=0.8000000,Max=0.8000000))
         Opacity=0.8000000
         FadeOutStartTime=6.5100002
         FadeOut=true
         FadeInEndTime=0.1400000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="MC"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         SpinsPerSecondRange=(X=(Min=0.0100000,Max=0.0100000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=14.4000006,Max=14.4000006),Y=(Min=14.4000006,Max=14.4000006),Z=(Min=14.4000006,Max=14.4000006))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0003'
         LifetimeRange=(Min=7.0000000,Max=7.0000000)
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=MeshEmitter Name=MeshEmitter7
         // Reference: MeshEmitter'lineageeffect.dec.m_u030_b.MeshEmitter7'
         begin object name="MeshEmitter7" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.wispray00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.4000000,Max=1.0000000),Y=(Min=0.3000000,Max=0.8000000),Z=(Min=0.2000000,Max=0.7000000))
         Opacity=0.2600000
         FadeOutStartTime=4.7300000
         FadeOut=true
         FadeInEndTime=0.3300000
         FadeIn=true
         MaxParticles=8
         RespawnDeadParticles=false
         Name="RayMesh"
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.0000000,Z=0.0000000)
         SpinsPerSecondRange=(X=(Min=0.0200000,Max=0.0200000),Y=(Min=0.0200000,Max=0.0200000),Z=(Min=0.0200000,Max=0.0200000))
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.5000000)
         StartSizeRange=(X=(Min=0.7200000,Max=0.7200000),Y=(Min=0.7200000,Max=0.7200000),Z=(Min=0.7200000,Max=0.7200000))
         UniformSize=true
         InitialParticlesPerSecond=8.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=5.5000000,Max=5.5000000)
         InitialDelayRange=(Min=0.5000000,Max=0.5000000)
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter7'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.m_u030_b.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         UseDirectionAs=1
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5500000,Color=(R=210,G=210,B=210,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=15.0000000
         Opacity=0.6000000
         FadeOutStartTime=0.6000000
         FadeOut=true
         FadeInEndTime=0.2000000
         FadeIn=true
         MaxParticles=32
         RespawnDeadParticles=false
         Name="RainbowNeedle"
         StartLocationShape=1
         SphereRadiusRange=(Min=1.0000000,Max=10.0000000)
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=1.4400001,Max=3.5999999),Y=(Min=28.8000011,Max=36.0000038),Z=(Min=1.4400001,Max=3.5999999))
         InitialParticlesPerSecond=8.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0050'
         TextureUSubdivisions=8
         TextureVSubdivisions=1
         SubdivisionEnd=1
         UseRandomSubdivision=true
         LifetimeRange=(Min=1.5000000,Max=1.5000000)
         InitialDelayRange=(Min=1.1000000,Max=1.1000000)
         StartVelocityRange=(X=(Min=36.0000038,Max=36.0000038),Y=(Min=36.0000038,Max=36.0000038),Z=(Min=36.0000038,Max=36.0000038))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(3)=SpriteEmitter'SpriteEmitter1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.m_u030_b.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=0.9000000,Max=0.9000000))
         Opacity=0.6000000
         FadeOutStartTime=0.1200000
         FadeOut=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Ini"
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=1.8000000)
         SizeScale[1]=(RelativeTime=0.2400000,RelativeSize=2.4000001)
         SizeScale[2]=(RelativeTime=0.5800000,RelativeSize=2.8000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=3.0999999)
         StartSizeRange=(X=(Min=9.6000004,Max=9.6000004),Y=(Min=9.6000004,Max=9.6000004),Z=(Min=9.6000004,Max=9.6000004))
         UniformSize=true
         InitialParticlesPerSecond=8.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=4
         SubdivisionEnd=5
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.7000000,Max=0.7000000)
         end object
     End Object
     Emitters(4)=SpriteEmitter'SpriteEmitter2'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter3
         // Reference: SpriteEmitter'lineageeffect.dec.m_u030_b.SpriteEmitter3'
         begin object name="SpriteEmitter3" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.2714290,Color=(R=128,G=128,B=128,A=255))
         ColorScale[2]=(RelativeTime=0.4678570,Color=(R=128,G=128,B=128,A=255))
         ColorScale[3]=(RelativeTime=0.8000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[4]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=80.0000000
         Opacity=0.1500000
         FadeOutStartTime=5.2800002
         FadeOut=true
         FadeInEndTime=0.7800000
         FadeIn=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Rainbow"
         StartSizeRange=(X=(Min=36.0000038,Max=36.0000038),Y=(Min=36.0000038,Max=36.0000038),Z=(Min=36.0000038,Max=36.0000038))
         UniformSize=true
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=2
         SubdivisionEnd=3
         UseRandomSubdivision=true
         LifetimeRange=(Min=6.0000000,Max=6.0000000)
         InitialDelayRange=(Min=1.0000000,Max=1.0000000)
         end object
     End Object
     Emitters(5)=SpriteEmitter'SpriteEmitter3'
     // Reference: MeshEmitter'lineageeffect.dec.m_u030_b.MeshEmitter8'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.magiccircle.magiccirclewhite01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.4714290,Color=(R=191,G=191,B=191,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=90.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.8000000,Max=0.8000000),Z=(Min=0.6000000,Max=0.6000000))
     Opacity=0.4000000
     FadeOutStartTime=7.0000000
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Ring"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.0000000,Z=0.0000000)
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.0864000,Max=0.0864000),Y=(Min=0.0864000,Max=0.0864000),Z=(Min=0.2400000,Max=0.2400000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=7.0000000,Max=7.0000000)
     Emitters[0]=MeshEmitter8
     // Reference: SpriteEmitter'lineageeffect.dec.m_u030_b.SpriteEmitter0'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5714290,Color=(R=173,G=173,B=173,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=90.0000000
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=0.8000000,Max=0.8000000))
     Opacity=0.8000000
     FadeOutStartTime=6.5100002
     FadeOut=true
     FadeInEndTime=0.1400000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="MC"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     SpinsPerSecondRange=(X=(Min=0.0100000,Max=0.0100000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=14.4000006,Max=14.4000006),Y=(Min=14.4000006,Max=14.4000006),Z=(Min=14.4000006,Max=14.4000006))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0003'
     LifetimeRange=(Min=7.0000000,Max=7.0000000)
     Emitters[1]=SpriteEmitter0
     // Reference: MeshEmitter'lineageeffect.dec.m_u030_b.MeshEmitter7'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Monster.wispray00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.4000000,Max=1.0000000),Y=(Min=0.3000000,Max=0.8000000),Z=(Min=0.2000000,Max=0.7000000))
     Opacity=0.2600000
     FadeOutStartTime=4.7300000
     FadeOut=true
     FadeInEndTime=0.3300000
     FadeIn=true
     MaxParticles=8
     RespawnDeadParticles=false
     Name="RayMesh"
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.0000000,Z=0.0000000)
     SpinsPerSecondRange=(X=(Min=0.0200000,Max=0.0200000),Y=(Min=0.0200000,Max=0.0200000),Z=(Min=0.0200000,Max=0.0200000))
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=1.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=0.5000000)
     StartSizeRange=(X=(Min=0.7200000,Max=0.7200000),Y=(Min=0.7200000,Max=0.7200000),Z=(Min=0.7200000,Max=0.7200000))
     UniformSize=true
     InitialParticlesPerSecond=8.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=5.5000000,Max=5.5000000)
     InitialDelayRange=(Min=0.5000000,Max=0.5000000)
     Emitters[2]=MeshEmitter7
     // Reference: SpriteEmitter'lineageeffect.dec.m_u030_b.SpriteEmitter1'
     UseDirectionAs=1
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5500000,Color=(R=210,G=210,B=210,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=15.0000000
     Opacity=0.6000000
     FadeOutStartTime=0.6000000
     FadeOut=true
     FadeInEndTime=0.2000000
     FadeIn=true
     MaxParticles=32
     RespawnDeadParticles=false
     Name="RainbowNeedle"
     StartLocationShape=1
     SphereRadiusRange=(Min=1.0000000,Max=10.0000000)
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=1.4400001,Max=3.5999999),Y=(Min=28.8000011,Max=36.0000038),Z=(Min=1.4400001,Max=3.5999999))
     InitialParticlesPerSecond=8.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0050'
     TextureUSubdivisions=8
     TextureVSubdivisions=1
     SubdivisionEnd=1
     UseRandomSubdivision=true
     LifetimeRange=(Min=1.5000000,Max=1.5000000)
     InitialDelayRange=(Min=1.1000000,Max=1.1000000)
     StartVelocityRange=(X=(Min=36.0000038,Max=36.0000038),Y=(Min=36.0000038,Max=36.0000038),Z=(Min=36.0000038,Max=36.0000038))
     GetVelocityDirectionFrom=2
     Emitters[3]=SpriteEmitter1
     // Reference: SpriteEmitter'lineageeffect.dec.m_u030_b.SpriteEmitter2'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=0.9000000,Max=0.9000000))
     Opacity=0.6000000
     FadeOutStartTime=0.1200000
     FadeOut=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Ini"
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0700000,RelativeSize=1.8000000)
     SizeScale[1]=(RelativeTime=0.2400000,RelativeSize=2.4000001)
     SizeScale[2]=(RelativeTime=0.5800000,RelativeSize=2.8000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=3.0999999)
     StartSizeRange=(X=(Min=9.6000004,Max=9.6000004),Y=(Min=9.6000004,Max=9.6000004),Z=(Min=9.6000004,Max=9.6000004))
     UniformSize=true
     InitialParticlesPerSecond=8.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=4
     SubdivisionEnd=5
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.7000000,Max=0.7000000)
     Emitters[4]=SpriteEmitter2
     // Reference: SpriteEmitter'lineageeffect.dec.m_u030_b.SpriteEmitter3'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.2714290,Color=(R=128,G=128,B=128,A=255))
     ColorScale[2]=(RelativeTime=0.4678570,Color=(R=128,G=128,B=128,A=255))
     ColorScale[3]=(RelativeTime=0.8000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[4]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=80.0000000
     Opacity=0.1500000
     FadeOutStartTime=5.2800002
     FadeOut=true
     FadeInEndTime=0.7800000
     FadeIn=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Rainbow"
     StartSizeRange=(X=(Min=36.0000038,Max=36.0000038),Y=(Min=36.0000038,Max=36.0000038),Z=(Min=36.0000038,Max=36.0000038))
     UniformSize=true
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=2
     SubdivisionEnd=3
     UseRandomSubdivision=true
     LifetimeRange=(Min=6.0000000,Max=6.0000000)
     InitialDelayRange=(Min=1.0000000,Max=1.0000000)
     Emitters[5]=SpriteEmitter3
     AutoDestroy=true
     Physics=10
     bNoDelete=false
     bTrailerPrePivot=true
     bSunAffect=true
}
