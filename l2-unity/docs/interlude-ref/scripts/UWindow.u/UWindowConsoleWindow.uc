class UWindowConsoleWindow extends UWindowFramedWindow;

var float OldParentWidth;
var float OldParentHeight;

function Created()
{
	super.Created();
	bSizable = true;
	bStatusBar = true;
	bLeaveOnscreen = true;
	OldParentWidth = ParentWindow.WinWidth;
	OldParentHeight = ParentWindow.WinHeight;
	SetDimensions();
	SetAcceptsFocus();
	return;
}

function ShowWindow()
{
	super(UWindowWindow).ShowWindow();
	// End:0x66
	if(__NFUN_132__(__NFUN_181__(ParentWindow.WinWidth, OldParentWidth), __NFUN_181__(ParentWindow.WinHeight, OldParentHeight)))
	{
		SetDimensions();
		OldParentWidth = ParentWindow.WinWidth;
		OldParentHeight = ParentWindow.WinHeight;
	}
	return;
}

function ResolutionChanged(float W, float h)
{
	SetDimensions();
	return;
}

function SetDimensions()
{
	// End:0x2D
	if(__NFUN_176__(ParentWindow.WinWidth, float(500)))
	{
		SetSize(200.0000000, 150.0000000);		
	}
	else
	{
		SetSize(410.0000000, 310.0000000);
	}
	WinLeft = __NFUN_175__(__NFUN_172__(ParentWindow.WinWidth, float(2)), __NFUN_172__(WinWidth, float(2)));
	WinTop = __NFUN_175__(__NFUN_172__(ParentWindow.WinHeight, float(2)), __NFUN_172__(WinHeight, float(2)));
	return;
}

function Close(optional bool bByParent)
{
	ClientArea.Close(true);
	Root.__NFUN_113__('None');
	return;
}

defaultproperties
{
	ClientClass=Class'UWindow.UWindowConsoleClientWindow'
	WindowTitle="Game Console"
}