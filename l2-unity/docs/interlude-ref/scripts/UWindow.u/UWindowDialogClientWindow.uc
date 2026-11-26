class UWindowDialogClientWindow extends UWindowClientWindow;

var float DesiredWidth;
var float DesiredHeight;
var UWindowDialogControl TabLast;

function OKPressed()
{
	return;
}

function Notify(UWindowDialogControl C, byte E)
{
	return;
}

function UWindowDialogControl CreateControl(Class<UWindowDialogControl> ControlClass, float X, float Y, float W, float h, optional UWindowWindow OwnerWindow)
{
	local UWindowDialogControl C;

	C = UWindowDialogControl(CreateWindow(ControlClass, X, Y, W, h, OwnerWindow));
	C.Register(self);
	C.Notify(C.0);
	// End:0x9A
	if(__NFUN_114__(TabLast, none))
	{
		TabLast = C;
		C.TabNext = C;
		C.TabPrev = C;		
	}
	else
	{
		C.TabNext = TabLast.TabNext;
		C.TabPrev = TabLast;
		TabLast.TabNext.TabPrev = C;
		TabLast.TabNext = C;
		TabLast = C;
	}
	return C;
	return;
}

function Paint(Canvas C, float X, float Y)
{
	super(UWindowWindow).Paint(C, X, Y);
	LookAndFeel.DrawClientArea(self, C);
	return;
}

function GetDesiredDimensions(out float W, out float h)
{
	W = DesiredWidth;
	h = DesiredHeight;
	return;
}
