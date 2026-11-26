class AnimNotify_ViewShake extends AnimNotify
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner );

}

enum EViewShakeType
{
	VST_DAMAGE,                     // 0
	VST_VIBRATION,                  // 1
	VST_USER,                       // 2
	VST_UP,                         // 3
	VST_DOWN,                       // 4
	VST_UPDOWN,                     // 5
	VST_DOWNUP                      // 6
};

var() AnimNotify_ViewShake.EViewShakeType ShakeType;
var() float ShakeIntensity;
var() Vector ShakeVector;
var() float ShakeRange;
var() int ShakeCount;
