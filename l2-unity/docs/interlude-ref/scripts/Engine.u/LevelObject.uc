class LevelObject extends Object
	abstract
	native
	nativereplication;

struct PointRegion
{
	var ZoneInfo Zone;
	var int iLeaf;
	var byte ZoneNumber;
};

var bool bDeleteMe;
var bool bSelected;
var(Advanced) bool bHidden;
var(Advanced) bool bHiddenEd;
var(Advanced) bool bHiddenEdGroup;
var(Advanced) bool bLockUndelete;
var(Advanced) bool bLockLocation;
var bool bPendingDelete;
var const transient bool bTempEditor;
var(Object) name Group;
var(Movement) Vector Location;
var(Display) Material Texture;
var(Display) float DrawScale;
var(Advanced) const PointRegion Region;
var const transient Level SWXLevel;
var const transient Level XLevel;
var LevelObject Deleted;

defaultproperties
{
	bHidden=true
	DrawScale=1.0000000
}