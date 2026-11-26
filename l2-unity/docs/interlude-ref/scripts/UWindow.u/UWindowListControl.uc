class UWindowListControl extends UWindowDialogControl;

var Class<UWindowList> ListClass;
var UWindowList Items;
var Color SelectionBkgColor;
var Color SelectionColor;
var bool bNoSelectionBox;

function DrawItem(Canvas C, UWindowList item, float X, float Y, float W, float h)
{
	return;
}

function Created()
{
	super.Created();
	Items = new ListClass;
	Items.Last = Items;
	Items.Next = none;
	Items.Prev = none;
	Items.Sentinel = Items;
	return;
}

defaultproperties
{
	SelectionBkgColor=(R=0,G=0,B=192,A=255)
	SelectionColor=(R=255,G=255,B=255,A=255)
}