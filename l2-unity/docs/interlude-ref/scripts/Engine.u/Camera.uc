class Camera extends PlayerController
	native
	config(User)
	notplaceable;

defaultproperties
{
	Location=(X=-500.0000000,Y=-300.0000000,Z=300.0000000)
	Texture=Texture'Engine.S_Camera'
	CollisionRadius=16.0000000
	CollisionHeight=39.0000000
	bDirectional=true
}