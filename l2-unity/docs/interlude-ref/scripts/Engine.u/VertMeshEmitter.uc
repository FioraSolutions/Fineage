class VertMeshEmitter extends ParticleEmitter
	native
	editinlinenew
	noexport;

var transient array<int> VertexStreams;
var transient array<int> IndexBuffer;
var transient array<float> AnimFrame;
var(VertMesh) array<float> AnimRate;
var(VertMesh) array<float> StartAnimFrame;
var(VertMesh) VertMesh VertexMesh;
var(VertMesh) bool UseMeshBlendMode;
var(VertMesh) bool RenderTwoSided;
var(VertMesh) bool UseParticleColor;
var transient Vector MeshExtent;

defaultproperties
{
	UseMeshBlendMode=true
	RenderTwoSided=true
	StartSizeRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
}