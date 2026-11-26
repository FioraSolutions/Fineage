class mo_battle_roar_ta extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter3
         // Reference: MeshEmitter'lineageeffect.dec.mo_battle_roar_ta.MeshEmitter3'
         begin object name="MeshEmitter3" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7900000,Max=0.7900000),Z=(Min=0.4400000,Max=0.4400000))
         FadeOutStartTime=0.0150000
         FadeOut=true
         MaxParticles=1
         RespawnDeadParticles=false
         Name="1"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-32.7379990)
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=0.1100000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.4000000,RelativeSize=2.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.4200001)
         StartSizeRange=(X=(Min=0.7210000,Max=0.7210000),Y=(Min=0.1820000,Max=0.1820000),Z=(Min=0.1820000,Max=0.1820000))
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.3000000,Max=0.3000000)
         end object
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter3'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter3
         // Reference: SpriteEmitter'lineageeffect.dec.mo_battle_roar_ta.SpriteEmitter3'
         begin object name="SpriteEmitter3" class=Engine.SpriteEmitter
         Acceleration=(X=0.0000000,Y=0.0000000,Z=10.0000000)
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=220,G=131,B=22,A=255))
         ColorScale[1]=(RelativeTime=0.8214290,Color=(R=54,G=14,B=67,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=37,G=64,B=169,A=255))
         ColorMultiplierRange=(X=(Min=0.5620000,Max=0.5620000),Y=(Min=0.5620000,Max=0.5620000),Z=(Min=1.0000000,Max=1.0000000))
         Opacity=0.4000000
         FadeOutStartTime=0.0500000
         FadeOut=true
         RespawnDeadParticles=false
         Name="3"
         StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
         StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
         StartLocationShape=1
         SphereRadiusRange=(Min=7.0000000,Max=7.0000000)
         StartSizeRange=(X=(Min=24.8560009,Max=46.3320007),Y=(Min=24.8560009,Max=46.3320007),Z=(Min=24.8560009,Max=46.3320007))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         BlendBetweenSubdivisions=true
         SubdivisionEnd=3
         LifetimeRange=(Min=1.0000000,Max=1.0000000)
         StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=100.0000000,Max=100.0000000))
         VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
         GetVelocityDirectionFrom=2
         end object
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter3'
     // Reference: MeshEmitter'lineageeffect.dec.mo_battle_roar_ta.MeshEmitter3'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=0,G=0,B=0,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.7900000,Max=0.7900000),Z=(Min=0.4400000,Max=0.4400000))
     FadeOutStartTime=0.0150000
     FadeOut=true
     MaxParticles=1
     RespawnDeadParticles=false
     Name="1"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=-32.7379990)
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.2500000,Max=0.2500000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=0.1100000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.4000000,RelativeSize=2.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.4200001)
     StartSizeRange=(X=(Min=0.7210000,Max=0.7210000),Y=(Min=0.1820000,Max=0.1820000),Z=(Min=0.1820000,Max=0.1820000))
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.3000000,Max=0.3000000)
     Emitters[0]=MeshEmitter3
     // Reference: SpriteEmitter'lineageeffect.dec.mo_battle_roar_ta.SpriteEmitter3'
     Acceleration=(X=0.0000000,Y=0.0000000,Z=10.0000000)
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=220,G=131,B=22,A=255))
     ColorScale[1]=(RelativeTime=0.8214290,Color=(R=54,G=14,B=67,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=37,G=64,B=169,A=255))
     ColorMultiplierRange=(X=(Min=0.5620000,Max=0.5620000),Y=(Min=0.5620000,Max=0.5620000),Z=(Min=1.0000000,Max=1.0000000))
     Opacity=0.4000000
     FadeOutStartTime=0.0500000
     FadeOut=true
     RespawnDeadParticles=false
     Name="3"
     StartLocationOffset=(X=0.0000000,Y=0.0000000,Z=10.0000000)
     StartLocationRange=(X=(Min=-10.0000000,Max=10.0000000),Y=(Min=-10.0000000,Max=10.0000000),Z=(Min=-10.0000000,Max=10.0000000))
     StartLocationShape=1
     SphereRadiusRange=(Min=7.0000000,Max=7.0000000)
     StartSizeRange=(X=(Min=24.8560009,Max=46.3320007),Y=(Min=24.8560009,Max=46.3320007),Z=(Min=24.8560009,Max=46.3320007))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1002'
     TextureUSubdivisions=2
     TextureVSubdivisions=4
     BlendBetweenSubdivisions=true
     SubdivisionEnd=3
     LifetimeRange=(Min=1.0000000,Max=1.0000000)
     StartVelocityRange=(X=(Min=200.0000000,Max=200.0000000),Y=(Min=200.0000000,Max=200.0000000),Z=(Min=100.0000000,Max=100.0000000))
     VelocityLossRange=(X=(Min=5.0000000,Max=5.0000000),Y=(Min=5.0000000,Max=5.0000000),Z=(Min=5.0000000,Max=5.0000000))
     GetVelocityDirectionFrom=2
     Emitters[1]=SpriteEmitter3
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.2000000
     bDirectional=true
}
