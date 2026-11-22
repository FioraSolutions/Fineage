class UWindowSmallOKButton extends UWindowSmallCloseButton;

var localized string OKText;

function Created()
{
	super.Created();
	SetText(OKText);
	return;
}

defaultproperties
{
	OKText="OK"
}