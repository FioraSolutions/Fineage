class at_sting_ta extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter4
         // Reference: MeshEmitter'lineageeffect.dec.at_sting_ta.MeshEmitter4'
         begin object name="MeshEmitter4" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_music_case01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=199,G=58,B=18,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=90,G=75,B=35,A=255))
         FadeOutStartTime=0.0600000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="Circle"
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.1000000)
         SizeScale[1]=(RelativeTime=0.0800000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.2900000,RelativeSize=2.3000000)
         SizeScale[3]=(RelativeTime=0.4000000,RelativeSize=2.5000000)
         SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=2.5999999)
         StartSizeRange=(X=(Min=0.1547000,Max=0.1547000),Y=(Min=0.1547000,Max=0.1547000),Z=(Min=0.1547000,Max=0.1547000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=70.0000000,Max=70.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter4'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter0
         // Reference: SpriteEmitter'lineageeffect.dec.at_sting_ta.SpriteEmitter0'
         begin object name="SpriteEmitter0" class=Engine.SpriteEmitter
         UseDirectionAs=1
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.9000000,Max=0.9000000),Y=(Min=0.4700000,Max=0.4700000),Z=(Min=0.2600000,Max=0.2600000))
         FadeOutStartTime=0.0840000
         FadeOut=true
         MaxParticles=14
         RespawnDeadParticles=false
         Name="Needle"
         StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationShape=2
         SphereRadiusRange=(Min=15.0000000,Max=25.0000000)
         StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.1050000,Max=0.4900000),Y=(Min=12.5999994,Max=17.5000000),Z=(Min=0.1050000,Max=0.4900000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=2
         SubdivisionEnd=3
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=105.0000000,Max=126.0000000),Y=(Min=105.0000000,Max=126.0000000),Z=(Min=105.0000000,Max=126.0000000))
         VelocityLossRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=0.5000000,Max=0.5000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter0'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         // Reference: SpriteEmitter'lineageeffect.dec.at_sting_ta.SpriteEmitter1'
         begin object name="SpriteEmitter1" class=Engine.SpriteEmitter
         UseDirectionAs=1
         Acceleration=(X=0.0000000,Y=0.0000000,Z=-140.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         FadeOutStartTime=0.1150000
         FadeOut=true
         MaxParticles=20
         RespawnDeadParticles=false
         Name="ttt"
         StartLocationShape=2
         StartLocationPolarRange=(X=(Min=-45.0000000,Max=45.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=4.0000000,Max=4.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.5000000)
         SizeScale[1]=(RelativeTime=0.3700000,RelativeSize=2.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.3000000)
         StartSizeRange=(X=(Min=0.1400000,Max=0.2800000),Y=(Min=1.2600000,Max=2.4500000),Z=(Min=70.0000000,Max=70.0000000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
         TextureUSubdivisions=8
         TextureVSubdivisions=8
         SubdivisionStart=17
         SubdivisionEnd=20
         UseRandomSubdivision=true
         LifetimeRange=(Min=0.6000000,Max=0.6000000)
         StartVelocityRange=(X=(Min=35.0000000,Max=84.0000000),Y=(Min=-56.0000000,Max=56.0000000),Z=(Min=-56.0000000,Max=56.0000000))
         end object
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter6
         // Reference: MeshEmitter'lineageeffect.dec.at_sting_ta.MeshEmitter6'
         begin object name="MeshEmitter6" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.sparkredcone00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=203,G=101,B=33,A=255))
         ColorScale[1]=(RelativeTime=0.0571430,Color=(R=255,G=255,B=255,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=55,G=15,B=6,A=255))
         FadeOutStartTime=0.0802000
         FadeOut=true
         FadeInEndTime=0.0120300
         FadeIn=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Beam"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.8000000)
         SizeScale[1]=(RelativeTime=0.1800000,RelativeSize=2.0000000)
         SizeScale[2]=(RelativeTime=0.3500000,RelativeSize=4.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=5.0000000)
         StartSizeRange=(X=(Min=0.1365000,Max=0.1365000),Y=(Min=0.1365000,Max=0.1365000),Z=(Min=0.1365000,Max=0.1365000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         end object
     End Object
     Emitters(3)=MeshEmitter'MeshEmitter6'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         // Reference: SpriteEmitter'lineageeffect.dec.at_sting_ta.SpriteEmitter2'
         begin object name="SpriteEmitter2" class=Engine.SpriteEmitter
         UseDirectionAs=4
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.6900000,Max=0.6900000),Y=(Min=0.6800000,Max=0.6800000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.4400000
         FadeOutStartTime=0.0600000
         FadeOut=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="Black_sphere"
         StartLocationOffset=(X=5.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1.4000000,Max=3.5000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
         SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=2.0000000)
         SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
         StartSizeRange=(X=(Min=16.1343002,Max=16.1343002),Y=(Min=16.1343002,Max=16.1343002),Z=(Min=16.1343002,Max=16.1343002))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         DrawStyle=6
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=35.0000000,Max=35.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(4)=SpriteEmitter'SpriteEmitter2'
     // Reference: MeshEmitter'lineageeffect.dec.at_sting_ta.MeshEmitter4'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Etc.etc_music_case01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=199,G=58,B=18,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=90,G=75,B=35,A=255))
     FadeOutStartTime=0.0600000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="Circle"
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.1000000)
     SizeScale[1]=(RelativeTime=0.0800000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.2900000,RelativeSize=2.3000000)
     SizeScale[3]=(RelativeTime=0.4000000,RelativeSize=2.5000000)
     SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=2.5999999)
     StartSizeRange=(X=(Min=0.1547000,Max=0.1547000),Y=(Min=0.1547000,Max=0.1547000),Z=(Min=0.1547000,Max=0.1547000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=70.0000000,Max=70.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[0]=MeshEmitter4
     // Reference: SpriteEmitter'lineageeffect.dec.at_sting_ta.SpriteEmitter0'
     UseDirectionAs=1
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.9000000,Max=0.9000000),Y=(Min=0.4700000,Max=0.4700000),Z=(Min=0.2600000,Max=0.2600000))
     FadeOutStartTime=0.0840000
     FadeOut=true
     MaxParticles=14
     RespawnDeadParticles=false
     Name="Needle"
     StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationShape=2
     SphereRadiusRange=(Min=15.0000000,Max=25.0000000)
     StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.1050000,Max=0.4900000),Y=(Min=12.5999994,Max=17.5000000),Z=(Min=0.1050000,Max=0.4900000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionStart=2
     SubdivisionEnd=3
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=105.0000000,Max=126.0000000),Y=(Min=105.0000000,Max=126.0000000),Z=(Min=105.0000000,Max=126.0000000))
     VelocityLossRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=0.5000000,Max=0.5000000))
     GetVelocityDirectionFrom=2
     Emitters[1]=SpriteEmitter0
     // Reference: SpriteEmitter'lineageeffect.dec.at_sting_ta.SpriteEmitter1'
     UseDirectionAs=1
     Acceleration=(X=0.0000000,Y=0.0000000,Z=-140.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     FadeOutStartTime=0.1150000
     FadeOut=true
     MaxParticles=20
     RespawnDeadParticles=false
     Name="ttt"
     StartLocationShape=2
     StartLocationPolarRange=(X=(Min=-45.0000000,Max=45.0000000),Y=(Min=60.0000000,Max=120.0000000),Z=(Min=4.0000000,Max=4.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=1.5000000)
     SizeScale[1]=(RelativeTime=0.3700000,RelativeSize=2.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=0.3000000)
     StartSizeRange=(X=(Min=0.1400000,Max=0.2800000),Y=(Min=1.2600000,Max=2.4500000),Z=(Min=70.0000000,Max=70.0000000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
     TextureUSubdivisions=8
     TextureVSubdivisions=8
     SubdivisionStart=17
     SubdivisionEnd=20
     UseRandomSubdivision=true
     LifetimeRange=(Min=0.6000000,Max=0.6000000)
     StartVelocityRange=(X=(Min=35.0000000,Max=84.0000000),Y=(Min=-56.0000000,Max=56.0000000),Z=(Min=-56.0000000,Max=56.0000000))
     Emitters[2]=SpriteEmitter1
     // Reference: MeshEmitter'lineageeffect.dec.at_sting_ta.MeshEmitter6'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.sparkredcone00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=203,G=101,B=33,A=255))
     ColorScale[1]=(RelativeTime=0.0571430,Color=(R=255,G=255,B=255,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=55,G=15,B=6,A=255))
     FadeOutStartTime=0.0802000
     FadeOut=true
     FadeInEndTime=0.0120300
     FadeIn=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Beam"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.8000000)
     SizeScale[1]=(RelativeTime=0.1800000,RelativeSize=2.0000000)
     SizeScale[2]=(RelativeTime=0.3500000,RelativeSize=4.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=5.0000000)
     StartSizeRange=(X=(Min=0.1365000,Max=0.1365000),Y=(Min=0.1365000,Max=0.1365000),Z=(Min=0.1365000,Max=0.1365000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     Emitters[3]=MeshEmitter6
     // Reference: SpriteEmitter'lineageeffect.dec.at_sting_ta.SpriteEmitter2'
     UseDirectionAs=4
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.6900000,Max=0.6900000),Y=(Min=0.6800000,Max=0.6800000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.4400000
     FadeOutStartTime=0.0600000
     FadeOut=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="Black_sphere"
     StartLocationOffset=(X=5.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=1.4000000,Max=3.5000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.0000000)
     SizeScale[1]=(RelativeTime=0.4000000,RelativeSize=2.0000000)
     SizeScale[2]=(RelativeTime=1.0000000,RelativeSize=3.0000000)
     StartSizeRange=(X=(Min=16.1343002,Max=16.1343002),Y=(Min=16.1343002,Max=16.1343002),Z=(Min=16.1343002,Max=16.1343002))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     DrawStyle=6
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0105'
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=35.0000000,Max=35.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     VelocityLossRange=(X=(Min=3.0000000,Max=3.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[4]=SpriteEmitter2
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.2000000
     bDirectional=true
}
