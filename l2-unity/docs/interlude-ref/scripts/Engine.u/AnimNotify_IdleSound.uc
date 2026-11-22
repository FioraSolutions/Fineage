class AnimNotify_IdleSound extends AnimNotify
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner );
	// Load Sound
	virtual USound* LoadSound( TCHAR* Name );
	virtual USound* GetSound( INT ItemClassID );

}

var() Sound Sound;
var() float Volume;
var() int Radius;
var() int Random;
var Sound IdleSound[3];

defaultproperties
{
	Volume=1.0000000
	Random=100
}