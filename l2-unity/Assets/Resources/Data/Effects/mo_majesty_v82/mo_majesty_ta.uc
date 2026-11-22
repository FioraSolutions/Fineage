class mo_majesty_ta extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter8
         // Reference: SpriteEmitter'lineageeffect.dec.mo_majesty_ta.SpriteEmitter8'
         begin object name="SpriteEmitter8" class=Engine.SpriteEmitter
         UseColorScale=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=0.5392860,Color=(R=137,G=129,B=156,A=255))
         ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScaleRepeats=13.0000000
         Opacity=0.6000000
         FadeOutStartTime=0.0800000
         FadeOut=true
         MaxParticles=2
         RespawnDeadParticles=false
         Name="Fireon"
         SpinParticles=true
         SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0100000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=0.0600000,RelativeSize=1.5000000)
         SizeScale[2]=(RelativeTime=0.1100000,RelativeSize=2.5000000)
         SizeScale[3]=(RelativeTime=0.2200000,RelativeSize=2.5500000)
         SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=2.5000000)
         StartSizeRange=(X=(Min=14.0000000,Max=14.0000000),Y=(Min=14.0000000,Max=14.0000000),Z=(Min=14.0000000,Max=14.0000000))
         UniformSize=true
         InitialParticlesPerSecond=1000.0000000
         AutomaticInitialSpawning=false
         Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1017'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         BlendBetweenSubdivisions=true
         SubdivisionEnd=3
         LifetimeRange=(Min=0.8000000,Max=0.8000000)
         end object
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter8'
     Begin Object Class=MeshEmitter Name=MeshEmitter8
         // Reference: MeshEmitter'lineageeffect.dec.mo_majesty_ta.MeshEmitter8'
         begin object name="MeshEmitter8" class=Engine.MeshEmitter
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Support.supportenchant01'
         UseMeshBlendMode=false
         RenderTwoSided=true
         ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
         ColorScale[1]=(RelativeTime=1.0000000,Color=(R=54,G=1,B=50,A=255))
         ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=0.1000000,Max=0.1000000))
         FadeOutStartTime=0.0720000
         FadeOut=true
         MaxParticles=3
         RespawnDeadParticles=false
         Name="Lightbeam"
         SpinParticles=true
         StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
         UseSizeScale=true
         UseRegularSizeScale=false
         SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
         SizeScale[1]=(RelativeTime=0.0500000,RelativeSize=1.0000000)
         SizeScale[2]=(RelativeTime=0.1700000,RelativeSize=2.0000000)
         SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.2000000)
         StartSizeRange=(X=(Min=0.2800000,Max=0.2800000),Y=(Min=0.2800000,Max=0.2800000),Z=(Min=0.2800000,Max=0.2800000))
         InitialParticlesPerSecond=10000.0000000
         AutomaticInitialSpawning=false
         LifetimeRange=(Min=0.4000000,Max=0.4000000)
         end object
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter8'
     // Reference: SpriteEmitter'lineageeffect.dec.mo_majesty_ta.SpriteEmitter8'
     UseColorScale=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=0.5392860,Color=(R=137,G=129,B=156,A=255))
     ColorScale[2]=(RelativeTime=1.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScaleRepeats=13.0000000
     Opacity=0.6000000
     FadeOutStartTime=0.0800000
     FadeOut=true
     MaxParticles=2
     RespawnDeadParticles=false
     Name="Fireon"
     SpinParticles=true
     SpinsPerSecondRange=(X=(Min=0.0000000,Max=0.0100000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=0.0600000,RelativeSize=1.5000000)
     SizeScale[2]=(RelativeTime=0.1100000,RelativeSize=2.5000000)
     SizeScale[3]=(RelativeTime=0.2200000,RelativeSize=2.5500000)
     SizeScale[4]=(RelativeTime=1.0000000,RelativeSize=2.5000000)
     StartSizeRange=(X=(Min=14.0000000,Max=14.0000000),Y=(Min=14.0000000,Max=14.0000000),Z=(Min=14.0000000,Max=14.0000000))
     UniformSize=true
     InitialParticlesPerSecond=1000.0000000
     AutomaticInitialSpawning=false
     Texture=Texture'LineageEffectsTextures.Particles2.fx_m_t1017'
     TextureUSubdivisions=2
     TextureVSubdivisions=2
     BlendBetweenSubdivisions=true
     SubdivisionEnd=3
     LifetimeRange=(Min=0.8000000,Max=0.8000000)
     Emitters[0]=SpriteEmitter8
     // Reference: MeshEmitter'lineageeffect.dec.mo_majesty_ta.MeshEmitter8'
     StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Support.supportenchant01'
     UseMeshBlendMode=false
     RenderTwoSided=true
     ColorScale[0]=(RelativeTime=0.0000000,Color=(R=255,G=255,B=255,A=255))
     ColorScale[1]=(RelativeTime=1.0000000,Color=(R=54,G=1,B=50,A=255))
     ColorMultiplierRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=0.4000000,Max=0.4000000),Z=(Min=0.1000000,Max=0.1000000))
     FadeOutStartTime=0.0720000
     FadeOut=true
     MaxParticles=3
     RespawnDeadParticles=false
     Name="Lightbeam"
     SpinParticles=true
     StartSpinRange=(X=(Min=-1.0000000,Max=1.0000000),Y=(Min=0.0000000,Max=0.0000000),Z=(Min=0.0000000,Max=0.0000000))
     UseSizeScale=true
     UseRegularSizeScale=false
     SizeScale[0]=(RelativeTime=0.0000000,RelativeSize=0.2000000)
     SizeScale[1]=(RelativeTime=0.0500000,RelativeSize=1.0000000)
     SizeScale[2]=(RelativeTime=0.1700000,RelativeSize=2.0000000)
     SizeScale[3]=(RelativeTime=1.0000000,RelativeSize=2.2000000)
     StartSizeRange=(X=(Min=0.2800000,Max=0.2800000),Y=(Min=0.2800000,Max=0.2800000),Z=(Min=0.2800000,Max=0.2800000))
     InitialParticlesPerSecond=10000.0000000
     AutomaticInitialSpawning=false
     LifetimeRange=(Min=0.4000000,Max=0.4000000)
     Emitters[1]=MeshEmitter8
     AutoDestroy=true
     bNoDelete=false
     DrawScale=0.2000000
     bDirectional=true
}
