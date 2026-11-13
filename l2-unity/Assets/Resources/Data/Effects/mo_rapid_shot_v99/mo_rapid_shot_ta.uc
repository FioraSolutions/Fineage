class mo_rapid_shot_ta extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter3
         // Reference: MeshEmitter'lineageeffect.dec.mo_rapid_shot_ta.MeshEmitter3'
         begin object name="MeshEmitter3" class=Engine.MeshEmitter
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.0964290,Color=(R=37,G=218,B=87,A=255))
         ColorScale[2]=(RelativeTime=0.3142860,Color=(R=255,G=255,B=255,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.1800000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="last2"
         StartLocationOffset=(X=-0.0100000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseRegularSizeScale=false
         StartSizeRange=(X=(Min=0.5136000,Max=0.5136000),Y=(Min=0.5136000,Max=0.5136000),Z=(Min=0.5136000,Max=0.5136000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=0.8000000,Max=0.8000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter3'
     Begin Object Class=MeshEmitter Name=MeshEmitter8
         // Reference: MeshEmitter'lineageeffect.dec.mo_rapid_shot_ta.MeshEmitter8'
         begin object name="MeshEmitter8" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         Acceleration=(X=-250.0000000,Y=0.0000000,Z=0.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=241,G=173,B=90,A=255))
         ColorScale[1]=(RelativeTime=0.5000000,Color=(R=158,G=158,B=158,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=198,G=198,B=198,A=255))
         ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.5710000,Max=0.5710000),Z=(Min=0.5500000,Max=0.5500000))
         FadeOutStartTime=0.0120000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="windy_end"
         StartLocationOffset=(X=3.5860000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         StartSpinRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=0.0600000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.2400000,RelativeSize=1.5000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=5.2810001)
         StartSizeRange=(X=(Min=0.1600000,Max=0.1600000),Y=(Min=0.1600000,Max=0.1600000),Z=(Min=0.1600000,Max=0.1600000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3870000,Max=0.3870000)
         InitialDelayRange=(Min=0.0300000,Max=0.0300000)
         StartVelocityRange=(X=(Min=-2.4456000,Max=-2.4456000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter8'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter13
         // Reference: SpriteEmitter'lineageeffect.dec.mo_rapid_shot_ta.SpriteEmitter13'
         begin object name="SpriteEmitter13" class=Engine.SpriteEmitter
         Acceleration=(X=-14.4000006,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=218,G=174,B=44,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=222,G=123,B=33,A=255))
         Opacity=0.4000000
         FadeOutStartTime=0.1002000
         FadeOut=true
         MaxParticles=18
         RespawnDeadParticles=false
         Name="SpriteEmitter0"
         StartLocationOffset=(X=-10.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=85.0000000,Max=95.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
         SpinParticles=true
         SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
         StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         StartSizeRange=(X=(Min=4.8800001,Max=5.9200001),Y=(Min=4.8800001,Max=5.9200001),Z=(Min=4.8800001,Max=5.9200001))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         SubdivisionStart=1
         SubdivisionEnd=2
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.3010000,Max=0.7010000)
         InitialDelayRange=(Min=0.0300000,Max=0.0300000)
         StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=160.0000000,Max=160.0000000),Z=(Min=160.0000000,Max=160.0000000))
         VelocityLossRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=10.0000000,Max=10.0000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter13'
     // Reference: MeshEmitter'lineageeffect.dec.mo_rapid_shot_ta.MeshEmitter3'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.0964290,Color=(R=37,G=218,B=87,A=255))
     ColorScale[2]=(RelativeTime=0.3142860,Color=(R=255,G=255,B=255,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.1800000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="last2"
     StartLocationOffset=(X=-0.0100000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.2500000,Max=0.2500000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseRegularSizeScale=false
     StartSizeRange=(X=(Min=0.5136000,Max=0.5136000),Y=(Min=0.5136000,Max=0.5136000),Z=(Min=0.5136000,Max=0.5136000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=0.8000000,Max=0.8000000)
     Emitters[0]=MeshEmitter3
     // Reference: MeshEmitter'lineageeffect.dec.mo_rapid_shot_ta.MeshEmitter8'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     Acceleration=(X=-250.0000000,Y=0.0000000,Z=0.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=241,G=173,B=90,A=255))
     ColorScale[1]=(RelativeTime=0.5000000,Color=(R=158,G=158,B=158,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=198,G=198,B=198,A=255))
     ColorScale[3]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.5710000,Max=0.5710000),Z=(Min=0.5500000,Max=0.5500000))
     FadeOutStartTime=0.0120000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="windy_end"
     StartLocationOffset=(X=3.5860000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     StartSpinRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=0.0600000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.2400000,RelativeSize=1.5000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=5.2810001)
     StartSizeRange=(X=(Min=0.1600000,Max=0.1600000),Y=(Min=0.1600000,Max=0.1600000),Z=(Min=0.1600000,Max=0.1600000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3870000,Max=0.3870000)
     InitialDelayRange=(Min=0.0300000,Max=0.0300000)
     StartVelocityRange=(X=(Min=-2.4456000,Max=-2.4456000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[1]=MeshEmitter8
     // Reference: SpriteEmitter'lineageeffect.dec.mo_rapid_shot_ta.SpriteEmitter13'
     Acceleration=(X=-14.4000006,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=218,G=174,B=44,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=222,G=123,B=33,A=255))
     Opacity=0.4000000
     FadeOutStartTime=0.1002000
     FadeOut=true
     MaxParticles=18
     RespawnDeadParticles=false
     Name="SpriteEmitter0"
     StartLocationOffset=(X=-10.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=85.0000000,Max=95.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
     SpinParticles=true
     SpinCCWorCW=(X=0.0000000,Y=0.5000000,Z=0.5000000)
     StartSpinRange=(X=(Min=0.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     StartSizeRange=(X=(Min=4.8800001,Max=5.9200001),Y=(Min=4.8800001,Max=5.9200001),Z=(Min=4.8800001,Max=5.9200001))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0063'
     TextureUSubdivisions=4
     TextureVSubdivisions=4
     SubdivisionStart=1
     SubdivisionEnd=2
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.3010000,Max=0.7010000)
     InitialDelayRange=(Min=0.0300000,Max=0.0300000)
     StartVelocityRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=160.0000000,Max=160.0000000),Z=(Min=160.0000000,Max=160.0000000))
     VelocityLossRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=10.0000000,Max=10.0000000),Z=(Min=10.0000000,Max=10.0000000))
     GetVelocityDirectionFrom=2
     Emitters[2]=SpriteEmitter13
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.2000000
     bDirectional=true
}
