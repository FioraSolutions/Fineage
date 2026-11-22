class TexEnvMap extends TexModifier
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material);

cpptext
{
	// UTexModifier interface
	virtual FMatrix* GetMatrix(FLOAT TimeSeconds);

}

enum ETexEnvMapType
{
	EM_WorldSpace,                  // 0
	EM_CameraSpace,                 // 1
	EM_SphereMap,                   // 2
	EM_SphereMapModulateOpacity     // 3
};

var() TexEnvMap.ETexEnvMapType EnvMapType;

defaultproperties
{
	EnvMapType=1
	TexCoordCount=1
}