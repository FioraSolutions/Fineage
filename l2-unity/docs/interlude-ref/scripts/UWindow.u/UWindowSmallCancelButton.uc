class UWindowSmallCancelButton extends UWindowButton;

var localized string CancelText;

function Created()
{
	super.Created();
	SetText(CancelText);
	return;
}

defaultproperties
{
	CancelText="Cancel"
}