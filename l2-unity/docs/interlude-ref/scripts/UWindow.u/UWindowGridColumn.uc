class UWindowGridColumn extends UWindowWindow;

var UWindowGridColumn NextColumn;
var UWindowGridColumn PrevColumn;
var bool bSizing;
var string ColumnHeading;
var int ColumnNum;

function Created()
{
	super.Created();
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	super.BeforePaint(C, X, Y);
	// End:0x2D
	if(__NFUN_176__(WinWidth, float(1)))
	{
		WinWidth = 1.0000000;
	}
	return;
}

function LMouseDown(float X, float Y)
{
	super.LMouseDown(X, Y);
	// End:0x8E
	if(__NFUN_130__(__NFUN_177__(X, float(__NFUN_249__(int(__NFUN_175__(WinWidth, float(5))), int(__NFUN_175__(__NFUN_175__(ParentWindow.WinWidth, WinLeft), float(5)))))), __NFUN_176__(Y, float(12))))
	{
		bSizing = true;
		UWindowGrid(ParentWindow.ParentWindow).bSizingColumn = true;
		Root.CaptureMouse();
	}
	return;
}

function LMouseUp(float X, float Y)
{
	super.LMouseUp(X, Y);
	UWindowGrid(ParentWindow.ParentWindow).bSizingColumn = false;
	return;
}

function MouseMove(float X, float Y)
{
	// End:0x5F
	if(__NFUN_130__(__NFUN_177__(X, float(__NFUN_249__(int(__NFUN_175__(WinWidth, float(5))), int(__NFUN_175__(__NFUN_175__(ParentWindow.WinWidth, WinLeft), float(5)))))), __NFUN_176__(Y, float(12))))
	{
		Cursor = Root.HSplitCursor;		
	}
	else
	{
		Cursor = Root.NormalCursor;
	}
	// End:0xF1
	if(__NFUN_130__(bSizing, bMouseDown))
	{
		WinWidth = X;
		// End:0xAA
		if(__NFUN_176__(WinWidth, float(1)))
		{
			WinWidth = 1.0000000;
		}
		// End:0xEE
		if(__NFUN_177__(WinWidth, __NFUN_175__(__NFUN_175__(ParentWindow.WinWidth, WinLeft), float(1))))
		{
			WinWidth = __NFUN_175__(__NFUN_175__(ParentWindow.WinWidth, WinLeft), float(1));
		}		
	}
	else
	{
		bSizing = false;
		UWindowGrid(ParentWindow.ParentWindow).bSizingColumn = false;
	}
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local Texture t;
	local Color FC;

	UWindowGrid(ParentWindow.ParentWindow).PaintColumn(C, self, X, Y);
	// End:0x61
	if(IsActive())
	{
		t = LookAndFeel.active;
		FC = LookAndFeel.HeadingActiveTitleColor;		
	}
	else
	{
		t = LookAndFeel.Inactive;
		FC = LookAndFeel.HeadingInActiveTitleColor;
	}
	C.SetDrawColor(byte(255), byte(255), byte(255));
	DrawUpBevel(C, 0.0000000, 0.0000000, WinWidth, float(LookAndFeel.ColumnHeadingHeight), t);
	C.DrawColor = FC;
	ClipText(C, 2.0000000, 1.0000000, ColumnHeading);
	C.SetDrawColor(byte(255), byte(255), byte(255));
	return;
}

function Click(float X, float Y)
{
	local int Row;

	// End:0x67
	if(__NFUN_176__(Y, float(12)))
	{
		// End:0x64
		if(__NFUN_178__(X, float(__NFUN_249__(int(__NFUN_175__(WinWidth, float(5))), int(__NFUN_175__(__NFUN_175__(ParentWindow.WinWidth, WinLeft), float(5)))))))
		{
			UWindowGrid(ParentWindow.ParentWindow).SortColumn(self);
		}		
	}
	else
	{
		Row = __NFUN_146__(int(__NFUN_172__(float(int(__NFUN_175__(Y, float(12)))), UWindowGrid(ParentWindow.ParentWindow).RowHeight)), UWindowGrid(ParentWindow.ParentWindow).TopRow);
		UWindowGrid(ParentWindow.ParentWindow).SelectRow(Row);
	}
	return;
}

function RMouseDown(float X, float Y)
{
	local int Row;

	super.RMouseDown(X, Y);
	// End:0xCD
	if(__NFUN_177__(Y, float(12)))
	{
		Row = __NFUN_146__(int(__NFUN_172__(float(int(__NFUN_175__(Y, float(12)))), UWindowGrid(ParentWindow.ParentWindow).RowHeight)), UWindowGrid(ParentWindow.ParentWindow).TopRow);
		UWindowGrid(ParentWindow.ParentWindow).SelectRow(Row);
		UWindowGrid(ParentWindow.ParentWindow).RightClickRowDown(Row, __NFUN_174__(X, WinLeft), __NFUN_174__(Y, WinTop));
	}
	return;
}

function RMouseUp(float X, float Y)
{
	local int Row;

	super.RMouseUp(X, Y);
	// End:0xCD
	if(__NFUN_177__(Y, float(12)))
	{
		Row = __NFUN_146__(int(__NFUN_172__(float(int(__NFUN_175__(Y, float(12)))), UWindowGrid(ParentWindow.ParentWindow).RowHeight)), UWindowGrid(ParentWindow.ParentWindow).TopRow);
		UWindowGrid(ParentWindow.ParentWindow).SelectRow(Row);
		UWindowGrid(ParentWindow.ParentWindow).RightClickRow(Row, __NFUN_174__(X, WinLeft), __NFUN_174__(Y, WinTop));
	}
	return;
}

function DoubleClick(float X, float Y)
{
	local int Row;

	// End:0x21
	if(__NFUN_176__(Y, float(12)))
	{
		Click(X, Y);		
	}
	else
	{
		Row = __NFUN_146__(int(__NFUN_172__(float(int(__NFUN_175__(Y, float(12)))), UWindowGrid(ParentWindow.ParentWindow).RowHeight)), UWindowGrid(ParentWindow.ParentWindow).TopRow);
		UWindowGrid(ParentWindow.ParentWindow).DoubleClickRow(Row);
	}
	return;
}

function MouseLeave()
{
	super.MouseLeave();
	UWindowGrid(ParentWindow.ParentWindow).MouseLeaveColumn(self);
	return;
}
