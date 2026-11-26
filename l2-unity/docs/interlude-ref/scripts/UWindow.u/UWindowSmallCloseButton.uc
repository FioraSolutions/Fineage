class UWindowSmallCloseButton extends UWindowSmallButton;

var localized string CloseText;

function Created()
{
	super.Created();
	SetText(CloseText);
	return;
}

function Click(float X, float Y)
{
	UWindowFramedWindow(GetParent(Class'UWindow.UWindowFramedWindow')).Close();
	return;
}

defaultproperties
{
	CloseText="Close"
}