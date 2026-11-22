class Brush extends Actor
	native
	notplaceable;

enum ECsgOper
{
	CSG_Active,                     // 0
	CSG_Add,                        // 1
	CSG_Subtract,                   // 2
	CSG_Intersect,                  // 3
	CSG_Deintersect                 // 4
};

var() Brush.ECsgOper CsgOper;
var const Object UnusedLightMesh;
var Vector PostPivot;
var Scale MainScale;
var Scale PostScale;
var Scale TempScale;
var() Color BrushColor;
var() int PolyFlags;
var() bool bColored;

defaultproperties
{
	MainScale=(Scale=(X=1.0000000,Y=1.0000000,Z=1.0000000),SheerRate=0.0000000,SheerAxis=0)
	PostScale=(Scale=(X=1.0000000,Y=1.0000000,Z=1.0000000),SheerRate=0.0000000,SheerAxis=0)
	TempScale=(Scale=(X=1.0000000,Y=1.0000000,Z=1.0000000),SheerRate=0.0000000,SheerAxis=0)
	DrawType=3
	bStatic=true
	bHidden=true
	bNoDelete=true
	bFixedRotationDir=true
	bEdShouldSnap=true
}