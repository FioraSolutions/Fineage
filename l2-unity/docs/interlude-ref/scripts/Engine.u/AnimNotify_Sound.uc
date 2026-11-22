class AnimNotify_Sound extends AnimNotify
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner );
	// #ifdef __L2 // zodiac
	// Load Sound
	virtual USound *LoadSound(TCHAR *Name);
	virtual USound *GetSound(PMoveType MoveType, L2PawnSoundType type);
	void PostLoad();
	// #endif

}

enum L2PawnSoundType
{
	LPST_GRASS,                     // 0
	LPST_LAND,                      // 1
	LPST_WATER,                     // 2
	LPST_ACTOR                      // 3
};

var() Sound Sound;
var() float Volume;
var() int Radius;
var() int Random;
var Sound DefaultWalkSound[3];
var Sound DefaultRunSound[3];
var Sound GrassWalkSound[3];
var Sound GrassRunSound[3];
var Sound WaterWalkSound[3];
var Sound WaterRunSound[3];
var Sound DefaultActorWalkSound[3];
var Sound DefaultActorRunSound[3];

defaultproperties
{
	Volume=1.0000000
	Random=100
}