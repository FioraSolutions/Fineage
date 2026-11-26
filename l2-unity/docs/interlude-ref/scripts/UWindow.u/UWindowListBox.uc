class UWindowListBox extends UWindowListControl;

var float ItemHeight;
var UWindowVScrollbar VertSB;
var UWindowListBoxItem SelectedItem;
var bool bCanDrag;
var bool bCanDragExternal;
var string DefaultHelpText;
var bool bDragging;
var float DragY;
var UWindowListBox DoubleClickList;
var bool bHotTrack;

function Created()
{
	super.Created();
	VertSB = UWindowVScrollbar(CreateWindow(Class'UWindow.UWindowVScrollbar', __NFUN_175__(WinWidth, float(12)), 0.0000000, 12.0000000, WinHeight));
	return;
}

function UWindowListBoxItem AddItem(string C)
{
	local UWindowListBoxItem i;

	i = UWindowListBoxItem(Items.Append(ListClass));
	i.Caption = C;
	return i;
	return;
}

function BeforePaint(Canvas C, float MouseX, float MouseY)
{
	local UWindowListBoxItem OverItem;
	local string NewHelpText;

	VertSB.SetRange(0.0000000, float(Items.CountShown()), float(int(__NFUN_172__(WinHeight, ItemHeight))));
	NewHelpText = DefaultHelpText;
	// End:0x9B
	if(__NFUN_119__(SelectedItem, none))
	{
		OverItem = GetItemAt(MouseX, MouseY);
		// End:0x9B
		if(__NFUN_130__(__NFUN_114__(OverItem, SelectedItem), __NFUN_123__(OverItem.HelpText, "")))
		{
			NewHelpText = OverItem.HelpText;
		}
	}
	// End:0xBD
	if(__NFUN_123__(NewHelpText, HelpText))
	{
		HelpText = NewHelpText;
		Notify(13);
	}
	return;
}

function SetHelpText(string t)
{
	super(UWindowDialogControl).SetHelpText(t);
	DefaultHelpText = t;
	return;
}

function Sort()
{
	Items.Sort();
	return;
}

function Paint(Canvas C, float MouseX, float MouseY)
{
	local float Y;
	local UWindowList CurItem;
	local int i;
	local Color OldColor;

	CurItem = Items.Next;
	i = 0;
	J0x1B:

	// End:0x72 [Loop If]
	if(__NFUN_130__(__NFUN_119__(CurItem, none), __NFUN_176__(float(i), VertSB.pos)))
	{
		// End:0x5B
		if(CurItem.ShowThisItem())
		{
			__NFUN_165__(i);
		}
		CurItem = CurItem.Next;
		// [Loop Continue]
		goto J0x1B;
	}
	Y = 3.0000000;
	J0x7D:

	// End:0x1EC [Loop If]
	if(__NFUN_130__(__NFUN_176__(Y, WinHeight), __NFUN_119__(CurItem, none)))
	{
		// End:0x1D5
		if(CurItem.ShowThisItem())
		{
			OldColor = TextColor;
			// End:0x155
			if(bHotTrack)
			{
				// End:0x155
				if(__NFUN_130__(__NFUN_114__(GetItemAt(MouseX, MouseY), CurItem), __NFUN_119__(CurItem, SelectedItem)))
				{
					TextColor = SelectionColor;
					TextColor.R = byte(__NFUN_250__(0, __NFUN_147__(int(TextColor.R), 96)));
					TextColor.G = byte(__NFUN_250__(0, __NFUN_147__(int(TextColor.G), 96)));
					TextColor.B = byte(__NFUN_250__(0, __NFUN_147__(int(TextColor.B), 96)));
				}
			}
			// End:0x194
			if(VertSB.bWindowVisible)
			{
				DrawItem(C, CurItem, 5.0000000, Y, __NFUN_175__(WinWidth, float(12)), ItemHeight);				
			}
			else
			{
				DrawItem(C, CurItem, 5.0000000, Y, WinWidth, ItemHeight);
			}
			Y = __NFUN_174__(Y, ItemHeight);
			TextColor = OldColor;
		}
		CurItem = CurItem.Next;
		// [Loop Continue]
		goto J0x7D;
	}
	return;
}

function Resized()
{
	super(UWindowWindow).Resized();
	VertSB.WinLeft = __NFUN_175__(WinWidth, float(12));
	VertSB.WinTop = 0.0000000;
	VertSB.SetSize(12.0000000, WinHeight);
	return;
}

function UWindowListBoxItem GetItemAt(float MouseX, float MouseY)
{
	local float Y;
	local UWindowList CurItem;
	local int i;

	// End:0x20
	if(__NFUN_132__(__NFUN_176__(MouseX, float(0)), __NFUN_177__(MouseX, WinWidth)))
	{
		return none;
	}
	CurItem = Items.Next;
	i = 0;
	J0x3B:

	// End:0x92 [Loop If]
	if(__NFUN_130__(__NFUN_119__(CurItem, none), __NFUN_176__(float(i), VertSB.pos)))
	{
		// End:0x7B
		if(CurItem.ShowThisItem())
		{
			__NFUN_165__(i);
		}
		CurItem = CurItem.Next;
		// [Loop Continue]
		goto J0x3B;
	}
	Y = 0.0000000;
	J0x9D:

	// End:0x126 [Loop If]
	if(__NFUN_130__(__NFUN_176__(Y, WinHeight), __NFUN_119__(CurItem, none)))
	{
		// End:0x10F
		if(CurItem.ShowThisItem())
		{
			// End:0xFD
			if(__NFUN_130__(__NFUN_179__(MouseY, Y), __NFUN_178__(MouseY, __NFUN_174__(Y, ItemHeight))))
			{
				return UWindowListBoxItem(CurItem);
			}
			Y = __NFUN_174__(Y, ItemHeight);
		}
		CurItem = CurItem.Next;
		// [Loop Continue]
		goto J0x9D;
	}
	return none;
	return;
}

function MakeSelectedVisible()
{
	local UWindowList CurItem;
	local int i;

	VertSB.SetRange(0.0000000, float(Items.CountShown()), float(int(__NFUN_172__(WinHeight, ItemHeight))));
	// End:0x42
	if(__NFUN_114__(SelectedItem, none))
	{
		return;
	}
	i = 0;
	CurItem = Items.Next;
	J0x5D:

	// End:0xAA [Loop If]
	if(__NFUN_119__(CurItem, none))
	{
		// End:0x7A
		if(__NFUN_114__(CurItem, SelectedItem))
		{
			// [Explicit Break]
			goto J0xAA;
		}
		// End:0x93
		if(CurItem.ShowThisItem())
		{
			__NFUN_165__(i);
		}
		CurItem = CurItem.Next;
		// [Loop Continue]
		goto J0x5D;
	}
	J0xAA:

	VertSB.Show(float(i));
	return;
}

function SetSelectedItem(UWindowListBoxItem NewSelected)
{
	// End:0x67
	if(__NFUN_130__(__NFUN_119__(NewSelected, none), __NFUN_119__(SelectedItem, NewSelected)))
	{
		// End:0x38
		if(__NFUN_119__(SelectedItem, none))
		{
			SelectedItem.bSelected = false;
		}
		SelectedItem = NewSelected;
		// End:0x5F
		if(__NFUN_119__(SelectedItem, none))
		{
			SelectedItem.bSelected = true;
		}
		Notify(2);
	}
	return;
}

function SetSelected(float X, float Y)
{
	local UWindowListBoxItem NewSelected;

	NewSelected = GetItemAt(X, Y);
	SetSelectedItem(NewSelected);
	return;
}

function LMouseDown(float X, float Y)
{
	super(UWindowWindow).LMouseDown(X, Y);
	SetSelected(X, Y);
	// End:0x56
	if(__NFUN_132__(bCanDrag, bCanDragExternal))
	{
		bDragging = true;
		Root.CaptureMouse();
		DragY = Y;
	}
	return;
}

function DoubleClick(float X, float Y)
{
	super(UWindowWindow).DoubleClick(X, Y);
	// End:0x35
	if(__NFUN_114__(GetItemAt(X, Y), SelectedItem))
	{
		DoubleClickItem(SelectedItem);
	}
	return;
}

function ReceiveDoubleClickItem(UWindowListBox L, UWindowListBoxItem i)
{
	i.Remove();
	Items.AppendItem(i);
	SetSelectedItem(i);
	L.SelectedItem = none;
	L.Notify(1);
	Notify(1);
	return;
}

function DoubleClickItem(UWindowListBoxItem i)
{
	// End:0x2D
	if(__NFUN_130__(__NFUN_119__(DoubleClickList, none), __NFUN_119__(i, none)))
	{
		DoubleClickList.ReceiveDoubleClickItem(self, i);
	}
	return;
}

function MouseMove(float X, float Y)
{
	local UWindowListBoxItem OverItem;

	super(UWindowDialogControl).MouseMove(X, Y);
	// End:0xFA
	if(__NFUN_130__(bDragging, bMouseDown))
	{
		OverItem = GetItemAt(X, Y);
		// End:0xCE
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(bCanDrag, __NFUN_119__(OverItem, SelectedItem)), __NFUN_119__(OverItem, none)), __NFUN_119__(SelectedItem, none)))
		{
			SelectedItem.Remove();
			// End:0xA3
			if(__NFUN_176__(Y, DragY))
			{
				OverItem.InsertItemBefore(SelectedItem);				
			}
			else
			{
				OverItem.InsertItemAfter(SelectedItem, true);
			}
			Notify(1);
			DragY = Y;			
		}
		else
		{
			// End:0xF7
			if(__NFUN_130__(bCanDragExternal, __NFUN_119__(CheckExternalDrag(X, Y), none)))
			{
				bDragging = false;
			}
		}		
	}
	else
	{
		bDragging = false;
	}
	return;
}

function bool ExternalDragOver(UWindowDialogControl ExternalControl, float X, float Y)
{
	local UWindowListBox B;
	local UWindowListBoxItem OverItem;

	B = UWindowListBox(ExternalControl);
	// End:0x134
	if(__NFUN_130__(__NFUN_119__(B, none), __NFUN_119__(B.SelectedItem, none)))
	{
		OverItem = GetItemAt(X, Y);
		B.SelectedItem.Remove();
		// End:0x8A
		if(__NFUN_119__(OverItem, none))
		{
			OverItem.InsertItemBefore(B.SelectedItem);			
		}
		else
		{
			Items.AppendItem(B.SelectedItem);
		}
		SetSelectedItem(B.SelectedItem);
		B.SelectedItem = none;
		B.Notify(1);
		Notify(1);
		// End:0x132
		if(__NFUN_132__(bCanDrag, bCanDragExternal))
		{
			Root.CancelCapture();
			bDragging = true;
			bMouseDown = true;
			Root.CaptureMouse(self);
			DragY = Y;
		}
		return true;
	}
	return false;
	return;
}

function DrawItem(Canvas C, UWindowList item, float X, float Y, float W, float h)
{
	local float tX, XL, YL;

	// End:0x76
	if(UWindowListBoxItem(item).bSelected)
	{
		// End:0x5F
		if(__NFUN_129__(bNoSelectionBox))
		{
			C.DrawColor = SelectionBkgColor;
			DrawStretchedTexture(C, X, Y, W, __NFUN_175__(h, float(1)), Texture'UWindow.WhiteTexture');
		}
		C.DrawColor = SelectionColor;		
	}
	else
	{
		C.DrawColor = TextColor;
	}
	C.Font = Root.Fonts[Font];
	C.__NFUN_464__(UWindowListBoxItem(item).Caption, XL, YL);
	switch(Align)
	{
		// End:0xF3
		case 0:
			tX = __NFUN_174__(X, float(2));
		// End:0x119
		case 2:
			tX = __NFUN_172__(__NFUN_175__(__NFUN_175__(W, float(4)), XL), float(2));
			// End:0x143
			break;
		// End:0x140
		case 1:
			tX = __NFUN_175__(__NFUN_175__(__NFUN_175__(W, XL), W), float(2));
			// End:0x143
			break;
		// End:0xFFFF
		default:
			break;
	}
	ClipText(C, tX, Y, UWindowListBoxItem(item).Caption);
	return;
}

function KeyUp(int Key, float X, float Y)
{
	// End:0x53
	if(__NFUN_132__(__NFUN_154__(Key, 200), __NFUN_154__(Key, 78)))
	{
		// End:0x50
		if(__NFUN_119__(SelectedItem.Prev.Prev, none))
		{
			SetSelectedItem(UWindowListBoxItem(SelectedItem.Prev));
		}		
	}
	else
	{
		// End:0x9D
		if(__NFUN_132__(__NFUN_154__(Key, 201), __NFUN_154__(Key, 80)))
		{
			// End:0x9A
			if(__NFUN_119__(SelectedItem.Next, none))
			{
				SetSelectedItem(UWindowListBoxItem(SelectedItem.Next));
			}			
		}
		else
		{
			// End:0xEF
			if(__NFUN_132__(__NFUN_154__(Key, 205), __NFUN_154__(Key, 13)))
			{
				// End:0xEF
				if(__NFUN_130__(__NFUN_119__(OwnerWindow, none), OwnerWindow.__NFUN_303__('UWindowDialogClientWindow')))
				{
					UWindowDialogClientWindow(OwnerWindow).Notify(self, 1);
				}
			}
		}
	}
	super(UWindowWindow).KeyUp(Key, X, Y);
	return;
}

defaultproperties
{
	ItemHeight=10.0000000
	ListClass=Class'UWindow.UWindowListBoxItem'
	TextColor=(R=255,G=255,B=255,A=255)
}