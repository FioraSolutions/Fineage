class at_fatal_strike_ta extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter9
         // Reference: SpriteEmitter'lineageeffect.dec.at_fatal_strike_ta.SpriteEmitter9'
         begin object name="SpriteEmitter9" class=Engine.SpriteEmitter
         UseDirectionAs=1
         ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorMultiplierRange=(X=(Min=0.9000000,Max=0.9000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
         FadeOutStartTime=0.0840000
         FadeOut=true
         MaxParticles=8
         RespawnDeadParticles=false
         Name="BlueNeedle"
         StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
         StartLocationShape=2
         SphereRadiusRange=(Min=15.0000000,Max=25.0000000)
         StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
         SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
         StartSizeRange=(X=(Min=0.1500000,Max=0.7000000),Y=(Min=10.0000000,Max=19.0440006),Z=(Min=0.1500000,Max=0.7000000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=2
         SubdivisionEnd=3
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=130.0000000,Max=160.0000000),Y=(Min=130.0000000,Max=160.0000000),Z=(Min=130.0000000,Max=160.0000000))
         VelocityLossRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=0.5000000,Max=0.5000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter9'
     Begin Object Class=MeshEmitter Name=MeshEmitter15
         // Reference: MeshEmitter'lineageeffect.dec.at_fatal_strike_ta.MeshEmitter15'
         begin object name="MeshEmitter15" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.4900000
         FadeOutStartTime=0.0375000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="ShockWave"
         SpinParticles=true
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0900000,RelativeSize=1.4000000)
         SizeScale[1]=(RelativeTime=0.2300000,RelativeSize=1.7000000)
         SizeScale[2]=(RelativeTime=0.6200000,RelativeSize=1.9000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
         StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.2000000,Max=0.2000000),Z=(Min=0.2000000,Max=0.2000000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.2000000,Max=0.2000000)
         StartVelocityRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter15'
     Begin Object Class=MeshEmitter Name=MeshEmitter16
         // Reference: MeshEmitter'lineageeffect.dec.at_fatal_strike_ta.MeshEmitter16'
         begin object name="MeshEmitter16" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.sparkredcone00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         Opacity=0.4000000
         FadeOutStartTime=0.0390000
         FadeOut=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="SparkCone"
         StartLocationOffset=(X=-3.0000000,Y=0.0000000,Z=0.0000000)
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.2000000))
         StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         StartVelocityRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         end object
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter16'
     // Reference: SpriteEmitter'lineageeffect.dec.at_fatal_strike_ta.SpriteEmitter9'
     UseDirectionAs=1
     ProjectionNormal=(X=1.0000000,Y=0.0000000,Z=0.0000000)
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorMultiplierRange=(X=(Min=0.9000000,Max=0.9000000),Y=(Min=0.9000000,Max=0.9000000),Z=(Min=1.0000000,Max=1.0000000))
     FadeOutStartTime=0.0840000
     FadeOut=true
     MaxParticles=8
     RespawnDeadParticles=false
     Name="BlueNeedle"
     StartLocationOffset=(X=2.0000000,Y=0.0000000,Z=0.0000000)
     StartLocationShape=2
     SphereRadiusRange=(Min=15.0000000,Max=25.0000000)
     StartLocationPolarRange=(X=(Min=90.0000000,Max=90.0000000),Y=(Min=0.0000000,Max=360.0000000),Z=(Min=10.0000000,Max=15.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.4600000,RelativeSize=1.5000000)
     SizeScale[1]=(RelativeTime=1.0000000,RelativeSize=1.0000000)
     StartSizeRange=(X=(Min=0.1500000,Max=0.7000000),Y=(Min=10.0000000,Max=19.0440006),Z=(Min=0.1500000,Max=0.7000000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     SubdivisionStart=2
     SubdivisionEnd=3
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=130.0000000,Max=160.0000000),Y=(Min=130.0000000,Max=160.0000000),Z=(Min=130.0000000,Max=160.0000000))
     VelocityLossRange=(X=(Min=0.5000000,Max=0.5000000),Y=(Min=0.5000000,Max=0.5000000),Z=(Min=0.5000000,Max=0.5000000))
     GetVelocityDirectionFrom=2
     Emitters[0]=SpriteEmitter9
     // Reference: MeshEmitter'lineageeffect.dec.at_fatal_strike_ta.MeshEmitter15'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.4900000
     FadeOutStartTime=0.0375000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="ShockWave"
     SpinParticles=true
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0900000,RelativeSize=1.4000000)
     SizeScale[1]=(RelativeTime=0.2300000,RelativeSize=1.7000000)
     SizeScale[2]=(RelativeTime=0.6200000,RelativeSize=1.9000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.0000000)
     StartSizeRange=(X=(Min=0.1500000,Max=0.1500000),Y=(Min=0.2000000,Max=0.2000000),Z=(Min=0.2000000,Max=0.2000000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.2000000,Max=0.2000000)
     StartVelocityRange=(X=(Min=4.0000000,Max=4.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[1]=MeshEmitter15
     // Reference: MeshEmitter'lineageeffect.dec.at_fatal_strike_ta.MeshEmitter16'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.sparkredcone00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     Opacity=0.4000000
     FadeOutStartTime=0.0390000
     FadeOut=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="SparkCone"
     StartLocationOffset=(X=-3.0000000,Y=0.0000000,Z=0.0000000)
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.2000000))
     StartSpinRange=(X=(Min=0.0000000,Max=0.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=1.0000000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     StartVelocityRange=(X=(Min=10.0000000,Max=10.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     Emitters[2]=MeshEmitter16
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.0200000
     bDirectional=true
}
