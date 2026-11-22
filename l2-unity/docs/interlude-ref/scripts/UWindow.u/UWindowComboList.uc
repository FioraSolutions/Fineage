class UWindowComboList extends UWindowListControl;

var UWindowComboControl Owner;
var UWindowVScrollbar VertSB;
var UWindowComboListItem Selected;
var int ItemHeight;
var int VBorder;
var int HBorder;
var int TextBorder;
var int MaxVisible;

function Sort()
{
	Items.Sort();
	return;
}

function WindowShown()
{
	super(UWindowWindow).WindowShown();
	FocusWindow();
	return;
}

function Clear()
{
	Items.Clear();
	return;
}

function Texture GetLookAndFeelTexture()
{
	return LookAndFeel.active;
	return;
}

function Setup()
{
	VertSB = UWindowVScrollbar(CreateWindow(Class'UWindow.UWindowVScrollbar', 0.0000000, __NFUN_175__(WinWidth, float(16)), 16.0000000, WinHeight));
	return;
}

function Created()
{
	ListClass = Class'UWindow.UWindowComboListItem';
	bAlwaysOnTop = true;
	bTransient = true;
	super.Created();
	ItemHeight = 15;
	VBorder = 3;
	HBorder = 3;
	TextBorder = 9;
	super.Created();
	return;
}

function int FindItemIndex(string Value, optional bool bIgnoreCase)
{
	local UWindowComboListItem i;
	local int Count;

	i = UWindowComboListItem(Items.Next);
	Count = 0;
	J0x20:

	// End:0x95 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x54
		if(__NFUN_130__(bIgnoreCase, __NFUN_124__(i.Value, Value)))
		{
			return Count;
		}
		// End:0x72
		if(__NFUN_122__(i.Value, Value))
		{
			return Count;
		}
		__NFUN_165__(Count);
		i = UWindowComboListItem(i.Next);
		// [Loop Continue]
		goto J0x20;
	}
	return -1;
	return;
}

function int FindItemIndex2(string Value2, optional bool bIgnoreCase)
{
	local UWindowComboListItem i;
	local int Count;

	i = UWindowComboListItem(Items.Next);
	Count = 0;
	J0x20:

	// End:0x95 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x54
		if(__NFUN_130__(bIgnoreCase, __NFUN_124__(i.Value2, Value2)))
		{
			return Count;
		}
		// End:0x72
		if(__NFUN_122__(i.Value2, Value2))
		{
			return Count;
		}
		__NFUN_165__(Count);
		i = UWindowComboListItem(i.Next);
		// [Loop Continue]
		goto J0x20;
	}
	return -1;
	return;
}

function string GetItemValue(int Index)
{
	local UWindowComboListItem i;
	local int Count;

	i = UWindowComboListItem(Items.Next);
	Count = 0;
	J0x20:

	// End:0x6C [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x49
		if(__NFUN_154__(Count, Index))
		{
			return i.Value;
		}
		__NFUN_165__(Count);
		i = UWindowComboListItem(i.Next);
		// [Loop Continue]
		goto J0x20;
	}
	return "";
	return;
}

function RemoveItem(int Index)
{
	local UWindowComboListItem i;
	local int Count;

	// End:0x11
	if(__NFUN_154__(Index, -1))
	{
		return;
	}
	i = UWindowComboListItem(Items.Next);
	Count = 0;
	J0x31:

	// End:0x7F [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x5C
		if(__NFUN_154__(Count, Index))
		{
			i.Remove();
			return;
		}
		__NFUN_165__(Count);
		i = UWindowComboListItem(i.Next);
		// [Loop Continue]
		goto J0x31;
	}
	return;
}

function string GetItemValue2(int Index)
{
	local UWindowComboListItem i;
	local int Count;

	i = UWindowComboListItem(Items.Next);
	Count = 0;
	J0x20:

	// End:0x6C [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x49
		if(__NFUN_154__(Count, Index))
		{
			return i.Value2;
		}
		__NFUN_165__(Count);
		i = UWindowComboListItem(i.Next);
		// [Loop Continue]
		goto J0x20;
	}
	return "";
	return;
}

function AddItem(string Value, optional string Value2, optional int SortWeight)
{
	local UWindowComboListItem i;

	i = UWindowComboListItem(Items.Append(Class'UWindow.UWindowComboListItem'));
	i.Value = Value;
	i.Value2 = Value2;
	i.SortWeight = SortWeight;
	return;
}

function InsertItem(string Value, optional string Value2, optional int SortWeight)
{
	local UWindowComboListItem i;

	i = UWindowComboListItem(Items.Insert(Class'UWindow.UWindowComboListItem'));
	i.Value = Value;
	i.Value2 = Value2;
	i.SortWeight = SortWeight;
	return;
}

function SetSelected(float X, float Y)
{
	local UWindowComboListItem NewSelected, item;
	local int i, Count;

	Count = 0;
	item = UWindowComboListItem(Items.Next);
	J0x20:

	// End:0x4E [Loop If]
	if(__NFUN_119__(item, none))
	{
		__NFUN_165__(Count);
		item = UWindowComboListItem(item.Next);
		// [Loop Continue]
		goto J0x20;
	}
	i = int(__NFUN_174__(float(__NFUN_145__(int(__NFUN_175__(Y, float(VBorder))), ItemHeight)), VertSB.pos));
	// End:0x91
	if(__NFUN_150__(i, 0))
	{
		i = 0;
	}
	// End:0xE6
	if(__NFUN_179__(float(i), __NFUN_174__(VertSB.pos, float(__NFUN_249__(Count, MaxVisible)))))
	{
		i = int(__NFUN_175__(__NFUN_174__(VertSB.pos, float(__NFUN_249__(Count, MaxVisible))), float(1)));
	}
	NewSelected = UWindowComboListItem(Items.FindEntry(i));
	// End:0x134
	if(__NFUN_119__(NewSelected, Selected))
	{
		// End:0x129
		if(__NFUN_114__(NewSelected, none))
		{
			Selected = none;			
		}
		else
		{
			Selected = NewSelected;
		}
	}
	return;
}

function MouseMove(float X, float Y)
{
	super(UWindowDialogControl).MouseMove(X, Y);
	// End:0x33
	if(__NFUN_177__(Y, WinHeight))
	{
		VertSB.Scroll(1.0000000);
	}
	// End:0x54
	if(__NFUN_176__(Y, float(0)))
	{
		VertSB.Scroll(-1.0000000);
	}
	SetSelected(X, Y);
	FocusWindow();
	return;
}

function LMouseUp(float X, float Y)
{
	// End:0x36
	if(__NFUN_130__(__NFUN_130__(__NFUN_179__(Y, float(0)), __NFUN_178__(Y, WinHeight)), __NFUN_119__(Selected, none)))
	{
		ExecuteItem(Selected);
	}
	super(UWindowWindow).LMouseUp(X, Y);
	return;
}

function LMouseDown(float X, float Y)
{
	Root.CaptureMouse();
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local float W, h, MaxWidth;
	local int Count;
	local UWindowComboListItem i;
	local float ListX, ListY, ExtraWidth;

	C.Font = Root.Fonts[0];
	C.SetPos(0.0000000, 0.0000000);
	MaxWidth = Owner.EditBoxWidth;
	ExtraWidth = __NFUN_171__(float(__NFUN_146__(HBorder, TextBorder)), float(2));
	Count = Items.Count();
	// End:0xC3
	if(__NFUN_151__(Count, MaxVisible))
	{
		__NFUN_184__(ExtraWidth, LookAndFeel.Size_ScrollbarWidth);
		WinHeight = __NFUN_174__(float(__NFUN_144__(ItemHeight, MaxVisible)), float(__NFUN_144__(VBorder, 2)));		
	}
	else
	{
		VertSB.pos = 0.0000000;
		WinHeight = __NFUN_174__(float(__NFUN_144__(ItemHeight, Count)), float(__NFUN_144__(VBorder, 2)));
	}
	i = UWindowComboListItem(Items.Next);
	J0x111:

	// End:0x189 [Loop If]
	if(__NFUN_119__(i, none))
	{
		TextSize(C, RemoveAmpersand(i.Value), W, h);
		// End:0x16D
		if(__NFUN_177__(__NFUN_174__(W, ExtraWidth), MaxWidth))
		{
			MaxWidth = __NFUN_174__(W, ExtraWidth);
		}
		i = UWindowComboListItem(i.Next);
		// [Loop Continue]
		goto J0x111;
	}
	WinWidth = MaxWidth;
	ListX = __NFUN_175__(__NFUN_174__(Owner.EditAreaDrawX, Owner.EditBoxWidth), WinWidth);
	ListY = __NFUN_174__(Owner.Button.WinTop, Owner.Button.WinHeight);
	// End:0x2B9
	if(__NFUN_151__(Count, MaxVisible))
	{
		VertSB.ShowWindow();
		VertSB.SetRange(0.0000000, float(Count), float(MaxVisible));
		VertSB.WinLeft = __NFUN_175__(__NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth), float(HBorder));
		VertSB.WinTop = float(HBorder);
		VertSB.WinWidth = LookAndFeel.Size_ScrollbarWidth;
		VertSB.WinHeight = __NFUN_175__(WinHeight, float(__NFUN_144__(2, VBorder)));		
	}
	else
	{
		VertSB.HideWindow();
	}
	Owner.WindowToGlobal(ListX, ListY, WinLeft, WinTop);
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local int Count;
	local UWindowComboListItem i;

	DrawMenuBackground(C);
	Count = 0;
	i = UWindowComboListItem(Items.Next);
	J0x2B:

	// End:0x136 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0xCE
		if(VertSB.bWindowVisible)
		{
			// End:0xCB
			if(__NFUN_179__(float(Count), VertSB.pos))
			{
				DrawItem(C, i, float(HBorder), __NFUN_174__(float(VBorder), __NFUN_171__(float(ItemHeight), __NFUN_175__(float(Count), VertSB.pos))), __NFUN_175__(__NFUN_175__(WinWidth, float(__NFUN_144__(2, HBorder))), VertSB.WinWidth), float(ItemHeight));
			}			
		}
		else
		{
			DrawItem(C, i, float(HBorder), float(__NFUN_146__(VBorder, __NFUN_144__(ItemHeight, Count))), __NFUN_175__(WinWidth, float(__NFUN_144__(2, HBorder))), float(ItemHeight));
		}
		__NFUN_165__(Count);
		i = UWindowComboListItem(i.Next);
		// [Loop Continue]
		goto J0x2B;
	}
	return;
}

function DrawMenuBackground(Canvas C)
{
	LookAndFeel.ComboList_DrawBackground(self, C);
	return;
}

function DrawItem(Canvas C, UWindowList item, float X, float Y, float W, float h)
{
	LookAndFeel.ComboList_DrawItem(self, C, X, Y, W, h, UWindowComboListItem(item).Value, __NFUN_114__(Selected, item));
	return;
}

function ExecuteItem(UWindowComboListItem i)
{
	Owner.SetValue(i.Value, i.Value2);
	CloseUp();
	return;
}

function CloseUp()
{
	Owner.CloseUp();
	return;
}

function FocusOtherWindow(UWindowWindow W)
{
	super(UWindowWindow).FocusOtherWindow(W);
	// End:0x69
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(bWindowVisible, __NFUN_119__(W.ParentWindow.ParentWindow, self)), __NFUN_119__(W.ParentWindow, self)), __NFUN_119__(W.ParentWindow, Owner)))
	{
		CloseUp();
	}
	return;
}

defaultproperties
{
	MaxVisible=10
}