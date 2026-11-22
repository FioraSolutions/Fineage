class MeshEmitter extends ParticleEmitter
	native
	editinlinenew;

var(Mesh) StaticMesh StaticMesh;
var(Mesh) bool UseMeshBlendMode;
var(Mesh) bool RenderTwoSided;
var(Mesh) bool UseParticleColor;
var transient Vector MeshExtent;

defaultproperties
{
	UseMeshBlendMode=true
	StartSizeRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
}