class UWindowGrid extends UWindowWindow;

var UWindowGridColumn FirstColumn;
var UWindowGridColumn LastColumn;
var UWindowGridClient ClientArea;
var int TopRow;
var float RowHeight;
var UWindowVScrollbar VertSB;
var UWindowHScrollbar HorizSB;
var bool bShowHorizSB;
var bool bSizingColumn;
var bool bNoKeyboard;

function Created()
{
	ClientArea = UWindowGridClient(CreateWindow(Class'UWindow.UWindowGridClient', 0.0000000, 0.0000000, __NFUN_175__(WinWidth, float(12)), WinHeight));
	VertSB = UWindowVScrollbar(CreateWindow(Class'UWindow.UWindowVScrollbar', __NFUN_175__(WinWidth, float(12)), 0.0000000, 12.0000000, WinHeight));
	VertSB.bAlwaysOnTop = true;
	HorizSB = UWindowHScrollbar(CreateWindow(Class'UWindow.UWindowHScrollbar', 0.0000000, __NFUN_175__(WinHeight, float(12)), WinWidth, 12.0000000));
	HorizSB.bAlwaysOnTop = true;
	HorizSB.HideWindow();
	bShowHorizSB = false;
	// End:0xDA
	if(__NFUN_129__(bNoKeyboard))
	{
		SetAcceptsFocus();
	}
	super.Created();
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	super.BeforePaint(C, X, Y);
	Resized();
	return;
}

function Resized()
{
	local float offset;
	local UWindowGridColumn colColumn;
	local float TotalWidth;

	TotalWidth = 0.0000000;
	colColumn = FirstColumn;
	J0x16:

	// End:0x53 [Loop If]
	if(__NFUN_119__(colColumn, none))
	{
		TotalWidth = __NFUN_174__(TotalWidth, colColumn.WinWidth);
		colColumn = colColumn.NextColumn;
		// [Loop Continue]
		goto J0x16;
	}
	// End:0x91
	if(__NFUN_129__(bSizingColumn))
	{
		HorizSB.SetRange(0.0000000, TotalWidth, __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth), 10.0000000);
	}
	// End:0xBF
	if(__NFUN_129__(HorizSB.bDisabled))
	{
		HorizSB.ShowWindow();
		bShowHorizSB = true;		
	}
	else
	{
		HorizSB.HideWindow();
		bShowHorizSB = false;
		HorizSB.pos = 0.0000000;
	}
	ClientArea.WinTop = 0.0000000;
	ClientArea.WinLeft = 0.0000000;
	ClientArea.WinWidth = __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth);
	// End:0x166
	if(bShowHorizSB)
	{
		ClientArea.WinHeight = __NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth);		
	}
	else
	{
		ClientArea.WinHeight = WinHeight;
	}
	// End:0x1FC
	if(bShowHorizSB)
	{
		HorizSB.WinTop = __NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth);
		HorizSB.WinLeft = 0.0000000;
		HorizSB.WinWidth = __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth);
		HorizSB.WinHeight = LookAndFeel.Size_ScrollbarWidth;
	}
	VertSB.WinTop = 0.0000000;
	VertSB.WinLeft = __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth);
	VertSB.WinWidth = LookAndFeel.Size_ScrollbarWidth;
	// End:0x281
	if(bShowHorizSB)
	{
		VertSB.WinHeight = __NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth);		
	}
	else
	{
		VertSB.WinHeight = WinHeight;
	}
	// End:0x2BC
	if(bShowHorizSB)
	{
		offset = __NFUN_175__(1.0000000, HorizSB.pos);		
	}
	else
	{
		offset = 1.0000000;
	}
	colColumn = FirstColumn;
	J0x2D2:

	// End:0x34B [Loop If]
	if(__NFUN_119__(colColumn, none))
	{
		colColumn.WinLeft = offset;
		colColumn.WinTop = 0.0000000;
		colColumn.WinHeight = WinHeight;
		offset = __NFUN_174__(offset, colColumn.WinWidth);
		colColumn = colColumn.NextColumn;
		// [Loop Continue]
		goto J0x2D2;
	}
	return;
}

function UWindowGridColumn AddColumn(string ColumnHeading, float DefaultWidth)
{
	local UWindowGridColumn NewColumn, OldLastColumn;

	OldLastColumn = LastColumn;
	// End:0x67
	if(__NFUN_114__(LastColumn, none))
	{
		NewColumn = UWindowGridColumn(ClientArea.CreateWindow(Class'UWindow.UWindowGridColumn', 0.0000000, 0.0000000, DefaultWidth, WinHeight));
		FirstColumn = NewColumn;
		NewColumn.ColumnNum = 0;		
	}
	else
	{
		NewColumn = UWindowGridColumn(ClientArea.CreateWindow(Class'UWindow.UWindowGridColumn', __NFUN_174__(LastColumn.WinLeft, LastColumn.WinWidth), 0.0000000, DefaultWidth, WinHeight));
		LastColumn.NextColumn = NewColumn;
		NewColumn.ColumnNum = __NFUN_146__(LastColumn.ColumnNum, 1);
	}
	LastColumn = NewColumn;
	NewColumn.NextColumn = none;
	NewColumn.PrevColumn = OldLastColumn;
	NewColumn.ColumnHeading = ColumnHeading;
	return NewColumn;
	return;
}

function Paint(Canvas C, float MouseX, float MouseY)
{
	local float X;
	local Texture t;
	local Region R;

	X = __NFUN_174__(LastColumn.WinWidth, LastColumn.WinLeft);
	t = GetLookAndFeelTexture();
	DrawUpBevel(C, X, 0.0000000, __NFUN_175__(WinWidth, X), float(LookAndFeel.ColumnHeadingHeight), t);
	// End:0xF5
	if(bShowHorizSB)
	{
		DrawStretchedTextureSegment(C, __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth), __NFUN_175__(WinHeight, LookAndFeel.Size_ScrollbarWidth), LookAndFeel.Size_ScrollbarWidth, LookAndFeel.Size_ScrollbarWidth, float(R.X), float(R.Y), float(R.W), float(R.h), t);
	}
	return;
}

function PaintColumn(Canvas C, UWindowGridColumn Column, float MouseX, float MouseY)
{
	return;
}

function SortColumn(UWindowGridColumn Column)
{
	return;
}

function SelectRow(int Row)
{
	return;
}

function RightClickRow(int Row, float X, float Y)
{
	return;
}

function RightClickRowDown(int Row, float X, float Y)
{
	return;
}

function DoubleClickRow(int Row)
{
	return;
}

function MouseLeaveColumn(UWindowGridColumn Column)
{
	return;
}

function KeyDown(int Key, float X, float Y)
{
	switch(Key)
	{
		// End:0x0C
		case 38:
		// End:0x28
		case 236:
			VertSB.Scroll(-1.0000000);
			// End:0xA2
			break;
		// End:0x2D
		case 40:
		// End:0x49
		case 237:
			VertSB.Scroll(1.0000000);
			// End:0xA2
			break;
		// End:0x75
		case 33:
			VertSB.Scroll(__NFUN_169__(__NFUN_175__(VertSB.MaxVisible, float(1))));
			// End:0xA2
			break;
		// End:0x9F
		case 34:
			VertSB.Scroll(__NFUN_175__(VertSB.MaxVisible, float(1)));
			// End:0xA2
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

defaultproperties
{
	RowHeight=10.0000000
}