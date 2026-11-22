class VehicleRoutePoint extends Vehicle
	native
	config(User)
	placeable;

cpptext
{
	typedef TMap<int, TArray<FVector> > VehicleRoutePointPathMapType;
	VehicleRoutePointPathMapType* GetPaths() { return (VehicleRoutePointPathMapType*)Paths; }
	virtual void PostEditChange() 
	{
		for (int i=0; i<AbsPoint.Num(); i++)
		{
			DeltaPoint(i) = AbsPoint(i) - Location;
		}
	}

}

var() string RouteName;
var() array<Vector> DeltaPoint;
var() array<Vector> AbsPoint;
var() array<int> MovingSpeed;
var() array<int> RotatingSpeed;
var() array<int> TimeToNextAction;
var() array<int> SpeakerID;
var() array<int> WaitingMessageID;
var() array<int> StationID;
var() Color LineColor;
var() Color PathColor;
var() Color FontColor;
var int Paths;

defaultproperties
{
	LineColor=(R=10,G=10,B=255,A=255)
	PathColor=(R=10,G=255,B=10,A=255)
	FontColor=(R=10,G=255,B=10,A=255)
	DrawType=8
}