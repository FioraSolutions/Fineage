class UWindowScrollingDialogClient extends UWindowPageWindow;

var bool bShowHorizSB;
var bool bShowVertSB;
var UWindowDialogClientWindow ClientArea;
var UWindowDialogClientWindow FixedArea;
var Class<UWindowDialogClientWindow> ClientClass;
var Class<UWindowDialogClientWindow> FixedAreaClass;
var UWindowVScrollbar VertSB;
var UWindowHScrollbar HorizSB;
var UWindowBitmap BRBitmap;

function Created()
{
	super(UWindowWindow).Created();
	// End:0x54
	if(__NFUN_119__(FixedAreaClass, none))
	{
		FixedArea = UWindowDialogClientWindow(CreateWindow(FixedAreaClass, 0.0000000, 0.0000000, 100.0000000, 100.0000000, OwnerWindow));
		FixedArea.bAlwaysOnTop = true;		
	}
	else
	{
		FixedArea = none;
	}
	ClientArea = UWindowDialogClientWindow(CreateWindow(ClientClass, 0.0000000, 0.0000000, WinWidth, WinHeight, OwnerWindow));
	VertSB = UWindowVScrollbar(CreateWindow(Class'UWindow.UWindowVScrollbar', __NFUN_175__(WinWidth, float(12)), 0.0000000, 12.0000000, WinHeight));
	VertSB.bAlwaysOnTop = true;
	VertSB.HideWindow();
	HorizSB = UWindowHScrollbar(CreateWindow(Class'UWindow.UWindowHScrollbar', 0.0000000, __NFUN_175__(WinHeight, float(12)), WinWidth, 12.0000000));
	HorizSB.bAlwaysOnTop = true;
	HorizSB.HideWindow();
	BRBitmap = UWindowBitmap(CreateWindow(Class'UWindow.UWindowBitmap', __NFUN_175__(WinWidth, float(12)), __NFUN_175__(WinHeight, float(12)), 12.0000000, 12.0000000));
	BRBitmap.bAlwaysOnTop = true;
	BRBitmap.HideWindow();
	BRBitmap.bStretch = true;
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local float ClientWidth, ClientHeight, FixedHeight;

	// End:0x22
	if(__NFUN_119__(FixedArea, none))
	{
		FixedHeight = FixedArea.WinHeight;		
	}
	else
	{
		FixedHeight = 0.0000000;
	}
	ClientWidth = ClientArea.DesiredWidth;
	ClientHeight = ClientArea.DesiredHeight;
	// End:0x6F
	if(__NFUN_178__(ClientWidth, WinWidth))
	{
		ClientWidth = WinWidth;
	}
	// End:0x97
	if(__NFUN_178__(ClientHeight, __NFUN_175__(WinHeight, FixedHeight)))
	{
		ClientHeight = __NFUN_175__(WinHeight, FixedHeight);
	}
	ClientArea.SetSize(ClientWidth, ClientHeight);
	bShowVertSB = __NFUN_177__(ClientHeight, __NFUN_175__(WinHeight, FixedHeight));
	bShowHorizSB = __NFUN_177__(ClientWidth, WinWidth);
	// End:0x16C
	if(bShowHorizSB)
	{
		ClientHeight = ClientArea.DesiredHeight;
		// End:0x142
		if(__NFUN_178__(ClientHeight, __NFUN_175__(__NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth), FixedHeight)))
		{
			ClientHeight = __NFUN_175__(__NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth), FixedHeight);
		}
		bShowVertSB = __NFUN_177__(ClientHeight, __NFUN_175__(__NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth), FixedHeight));
	}
	// End:0x316
	if(bShowVertSB)
	{
		VertSB.ShowWindow();
		VertSB.WinTop = 0.0000000;
		VertSB.WinLeft = __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth);
		VertSB.WinWidth = LookAndFeel.Size_ScrollbarWidth;
		// End:0x2BD
		if(bShowHorizSB)
		{
			BRBitmap.ShowWindow();
			BRBitmap.WinWidth = LookAndFeel.Size_ScrollbarWidth;
			BRBitmap.WinHeight = LookAndFeel.Size_ScrollbarWidth;
			BRBitmap.WinTop = __NFUN_175__(__NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth), FixedHeight);
			BRBitmap.WinLeft = __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth);
			BRBitmap.t = GetLookAndFeelTexture();
			VertSB.WinHeight = __NFUN_175__(__NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth), FixedHeight);			
		}
		else
		{
			BRBitmap.HideWindow();
			VertSB.WinHeight = __NFUN_175__(WinHeight, FixedHeight);
		}
		VertSB.SetRange(0.0000000, ClientHeight, VertSB.WinHeight, 10.0000000);		
	}
	else
	{
		BRBitmap.HideWindow();
		VertSB.HideWindow();
		VertSB.pos = 0.0000000;
	}
	// End:0x42F
	if(bShowHorizSB)
	{
		HorizSB.ShowWindow();
		HorizSB.WinLeft = 0.0000000;
		HorizSB.WinTop = __NFUN_175__(__NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth), FixedHeight);
		HorizSB.WinHeight = LookAndFeel.Size_ScrollbarWidth;
		// End:0x3EC
		if(bShowVertSB)
		{
			HorizSB.WinWidth = __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth);			
		}
		else
		{
			HorizSB.WinWidth = WinWidth;
		}
		HorizSB.SetRange(0.0000000, ClientWidth, HorizSB.WinWidth, 10.0000000);		
	}
	else
	{
		HorizSB.HideWindow();
		HorizSB.pos = 0.0000000;
	}
	ClientArea.WinLeft = __NFUN_169__(HorizSB.pos);
	ClientArea.WinTop = __NFUN_169__(VertSB.pos);
	// End:0x504
	if(__NFUN_119__(FixedArea, none))
	{
		FixedArea.WinLeft = 0.0000000;
		FixedArea.WinTop = __NFUN_175__(WinHeight, FixedHeight);
		// End:0x504
		if(__NFUN_181__(FixedArea.WinWidth, WinWidth))
		{
			FixedArea.SetSize(WinWidth, FixedArea.WinHeight);
		}
	}
	super(UWindowWindow).BeforePaint(C, X, Y);
	return;
}

function GetDesiredDimensions(out float W, out float h)
{
	super(UWindowWindow).GetDesiredDimensions(W, h);
	return;
}

function Paint(Canvas C, float X, float Y)
{
	return;
}

defaultproperties
{
	ClientClass=Class'UWindow.UWindowDialogClientWindow'
}