class SuperPointInfo extends Info
	native
	placeable
	hidecategories(Collision,Lighting,LightColor,Karma,Force);

cpptext
{
	typedef TMap<int, TArray<FVector> > SuperPointPathMapType;
	SuperPointPathMapType* GetPaths() { return (SuperPointPathMapType*)Paths; }
	virtual void PostEditChange() 
	{
		for (int i=0; i<AbsPoint.Num(); i++)
		{
			DeltaPoint(i) = AbsPoint(i) - Location;
		}
	}

}

enum SuperPointMoveType
{
	Follow_Rail,                    // 0
	Move_Random                     // 1
};

var() string SuperPointName;
var() SuperPointInfo.SuperPointMoveType MoveType;
var() array<Vector> DeltaPoint;
var() array<Vector> AbsPoint;
var() array<int> Delay;
var() Color LineColor;
var() Color PathColor;
var() Color FontColor;
var int Paths;

defaultproperties
{
	LineColor=(R=10,G=10,B=255,A=255)
	PathColor=(R=10,G=255,B=10,A=255)
	FontColor=(R=10,G=255,B=10,A=255)
	bAlwaysVisible=true
	bStatic=true
	Texture=Texture'Engine.S_SuperPointInfo'
	bStaticLighting=true
}