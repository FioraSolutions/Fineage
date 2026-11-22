class FluidSurfaceOscillator extends Actor
	native
	placeable;

cpptext
{
	void UpdateOscillation( FLOAT DeltaTime );
	virtual void PostEditChange();
	virtual void Destroy();

}

var() edfindable FluidSurfaceInfo FluidInfo;
var() float Frequency;
var() byte Phase;
var() float Strength;
var() float Radius;
var const transient float OscTime;

defaultproperties
{
	Frequency=1.0000000
	Strength=10.0000000
	bHidden=true
	Texture=Texture'Engine.S_FluidSurfOsc'
}