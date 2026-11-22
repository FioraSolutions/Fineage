class UWindowPulldownMenu extends UWindowListControl;

var UWindowPulldownMenuItem Selected;
var UWindowList Owner;
var int ItemHeight;
var int VBorder;
var int HBorder;
var int TextBorder;
var UWindowMenuBar MyMenuBar;

function UWindowPulldownMenuItem AddMenuItem(string C, Texture G)
{
	local UWindowPulldownMenuItem i;

	i = UWindowPulldownMenuItem(Items.Append(Class'UWindow.UWindowPulldownMenuItem'));
	i.Owner = self;
	i.SetCaption(C);
	i.Graphic = G;
	i.Tag = Items.Count();
	return i;
	return;
}

function Created()
{
	ListClass = Class'UWindow.UWindowPulldownMenuItem';
	SetAcceptsFocus();
	super.Created();
	ItemHeight = int(LookAndFeel.Pulldown_ItemHeight);
	VBorder = int(LookAndFeel.Pulldown_VBorder);
	HBorder = int(LookAndFeel.Pulldown_HBorder);
	TextBorder = int(LookAndFeel.Pulldown_TextBorder);
	return;
}

function Clear()
{
	Items.Clear();
	Selected = none;
	return;
}

function DeSelect()
{
	// End:0x21
	if(__NFUN_119__(Selected, none))
	{
		Selected.DeSelect();
		Selected = none;
	}
	return;
}

function Select(UWindowPulldownMenuItem i)
{
	return;
}

function PerformSelect(UWindowPulldownMenuItem NewSelected)
{
	// End:0x2B
	if(__NFUN_130__(__NFUN_119__(Selected, none), __NFUN_119__(NewSelected, Selected)))
	{
		Selected.DeSelect();
	}
	// End:0x40
	if(__NFUN_114__(NewSelected, none))
	{
		Selected = none;		
	}
	else
	{
		// End:0x8F
		if(__NFUN_130__(__NFUN_130__(__NFUN_119__(Selected, NewSelected), __NFUN_123__(NewSelected.Caption, "-")), __NFUN_129__(NewSelected.bDisabled)))
		{
			LookAndFeel.PlayMenuSound(self, 2);
		}
		Selected = NewSelected;
		// End:0xBF
		if(__NFUN_119__(Selected, none))
		{
			Selected.Select();
			Select(Selected);
		}
	}
	return;
}

function SetSelected(float X, float Y)
{
	local UWindowPulldownMenuItem NewSelected;

	NewSelected = UWindowPulldownMenuItem(Items.FindEntry(__NFUN_145__(int(__NFUN_175__(Y, float(VBorder))), ItemHeight)));
	PerformSelect(NewSelected);
	return;
}

function ShowWindow()
{
	super(UWindowWindow).ShowWindow();
	PerformSelect(none);
	FocusWindow();
	return;
}

function MouseMove(float X, float Y)
{
	super(UWindowDialogControl).MouseMove(X, Y);
	SetSelected(X, Y);
	FocusWindow();
	return;
}

function LMouseUp(float X, float Y)
{
	// End:0x4F
	if(__NFUN_130__(__NFUN_130__(__NFUN_119__(Selected, none), __NFUN_123__(Selected.Caption, "-")), __NFUN_129__(Selected.bDisabled)))
	{
		BeforeExecuteItem(Selected);
		ExecuteItem(Selected);
	}
	super(UWindowWindow).LMouseUp(X, Y);
	return;
}

function LMouseDown(float X, float Y)
{
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local float W, h, MaxWidth;
	local int Count;
	local UWindowPulldownMenuItem i;

	MaxWidth = 100.0000000;
	Count = 0;
	C.Font = Root.Fonts[0];
	C.SetPos(0.0000000, 0.0000000);
	i = UWindowPulldownMenuItem(Items.Next);
	J0x63:

	// End:0xD4 [Loop If]
	if(__NFUN_119__(i, none))
	{
		__NFUN_165__(Count);
		TextSize(C, RemoveAmpersand(i.Caption), W, h);
		// End:0xB8
		if(__NFUN_177__(W, MaxWidth))
		{
			MaxWidth = W;
		}
		i = UWindowPulldownMenuItem(i.Next);
		// [Loop Continue]
		goto J0x63;
	}
	WinWidth = __NFUN_174__(MaxWidth, float(__NFUN_144__(__NFUN_146__(HBorder, TextBorder), 2)));
	WinHeight = __NFUN_174__(float(__NFUN_144__(ItemHeight, Count)), float(__NFUN_144__(VBorder, 2)));
	// End:0x179
	if(__NFUN_132__(__NFUN_130__(__NFUN_119__(UWindowMenuBarItem(Owner), none), UWindowMenuBarItem(Owner).bHelp), __NFUN_177__(__NFUN_174__(WinLeft, WinWidth), ParentWindow.WinWidth)))
	{
		WinLeft = __NFUN_175__(ParentWindow.WinWidth, WinWidth);
	}
	// End:0x1F7
	if(__NFUN_119__(UWindowPulldownMenuItem(Owner), none))
	{
		i = UWindowPulldownMenuItem(Owner);
		// End:0x1F7
		if(__NFUN_177__(__NFUN_174__(WinWidth, WinLeft), ParentWindow.WinWidth))
		{
			WinLeft = __NFUN_175__(__NFUN_174__(i.Owner.WinLeft, float(i.Owner.HBorder)), WinWidth);
		}
	}
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local int Count;
	local UWindowPulldownMenuItem i;

	DrawMenuBackground(C);
	Count = 0;
	i = UWindowPulldownMenuItem(Items.Next);
	J0x2B:

	// End:0x9E [Loop If]
	if(__NFUN_119__(i, none))
	{
		DrawItem(C, i, float(HBorder), float(__NFUN_146__(VBorder, __NFUN_144__(ItemHeight, Count))), __NFUN_175__(WinWidth, float(__NFUN_144__(2, HBorder))), float(ItemHeight));
		__NFUN_165__(Count);
		i = UWindowPulldownMenuItem(i.Next);
		// [Loop Continue]
		goto J0x2B;
	}
	return;
}

function DrawMenuBackground(Canvas C)
{
	LookAndFeel.Menu_DrawPulldownMenuBackground(self, C);
	return;
}

function DrawItem(Canvas C, UWindowList item, float X, float Y, float W, float h)
{
	LookAndFeel.Menu_DrawPulldownMenuItem(self, UWindowPulldownMenuItem(item), C, X, Y, W, h, __NFUN_114__(Selected, item));
	return;
}

function BeforeExecuteItem(UWindowPulldownMenuItem i)
{
	LookAndFeel.PlayMenuSound(self, 3);
	return;
}

function ExecuteItem(UWindowPulldownMenuItem i)
{
	MyMenuBar.MenuItemSelected(self, i);
	CloseUp();
	return;
}

function CloseUp(optional bool bByOwner)
{
	local UWindowPulldownMenuItem i;

	// End:0x53
	if(__NFUN_129__(bByOwner))
	{
		// End:0x2F
		if(__NFUN_119__(UWindowPulldownMenuItem(Owner), none))
		{
			UWindowPulldownMenuItem(Owner).CloseUp();
		}
		// End:0x53
		if(__NFUN_119__(UWindowMenuBarItem(Owner), none))
		{
			UWindowMenuBarItem(Owner).CloseUp();
		}
	}
	i = UWindowPulldownMenuItem(Items.Next);
	J0x6C:

	// End:0xC0 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0xA4
		if(__NFUN_119__(i.SubMenu, none))
		{
			i.SubMenu.CloseUp(true);
		}
		i = UWindowPulldownMenuItem(i.Next);
		// [Loop Continue]
		goto J0x6C;
	}
	return;
}

function UWindowMenuBar GetMenuBar()
{
	// End:0x25
	if(__NFUN_119__(UWindowPulldownMenuItem(Owner), none))
	{
		return UWindowPulldownMenuItem(Owner).GetMenuBar();
	}
	// End:0x4A
	if(__NFUN_119__(UWindowMenuBarItem(Owner), none))
	{
		return UWindowMenuBarItem(Owner).GetMenuBar();
	}
	return;
}

function FocusOtherWindow(UWindowWindow W)
{
	super(UWindowWindow).FocusOtherWindow(W);
	// End:0x30
	if(__NFUN_119__(Selected, none))
	{
		// End:0x30
		if(__NFUN_114__(W, Selected.SubMenu))
		{
			return;
		}
	}
	// End:0x5F
	if(__NFUN_119__(UWindowPulldownMenuItem(Owner), none))
	{
		// End:0x5F
		if(__NFUN_114__(UWindowPulldownMenuItem(Owner).Owner, W))
		{
			return;
		}
	}
	// End:0x6E
	if(bWindowVisible)
	{
		CloseUp();
	}
	return;
}

function KeyDown(int Key, float X, float Y)
{
	local UWindowPulldownMenuItem i;

	i = Selected;
	switch(Key)
	{
		// End:0x20
		case 27:
			CloseUp();
			// End:0x445
			break;
		// End:0x129
		case 38:
			// End:0x66
			if(__NFUN_132__(__NFUN_114__(i, none), __NFUN_114__(i, Items.Next)))
			{
				i = UWindowPulldownMenuItem(Items.Last);				
			}
			else
			{
				i = UWindowPulldownMenuItem(i.Prev);
			}
			// End:0xA6
			if(__NFUN_114__(i, none))
			{
				i = UWindowPulldownMenuItem(Items.Last);				
			}
			else
			{
				// End:0xD5
				if(__NFUN_122__(i.Caption, "-"))
				{
					i = UWindowPulldownMenuItem(i.Prev);
				}
			}
			// End:0xF9
			if(__NFUN_114__(i, none))
			{
				i = UWindowPulldownMenuItem(Items.Last);
			}
			// End:0x11B
			if(__NFUN_114__(i.SubMenu, none))
			{
				PerformSelect(i);				
			}
			else
			{
				Selected = i;
			}
			// End:0x445
			break;
		// End:0x218
		case 40:
			// End:0x155
			if(__NFUN_114__(i, none))
			{
				i = UWindowPulldownMenuItem(Items.Next);				
			}
			else
			{
				i = UWindowPulldownMenuItem(i.Next);
			}
			// End:0x195
			if(__NFUN_114__(i, none))
			{
				i = UWindowPulldownMenuItem(Items.Next);				
			}
			else
			{
				// End:0x1C4
				if(__NFUN_122__(i.Caption, "-"))
				{
					i = UWindowPulldownMenuItem(i.Next);
				}
			}
			// End:0x1E8
			if(__NFUN_114__(i, none))
			{
				i = UWindowPulldownMenuItem(Items.Next);
			}
			// End:0x20A
			if(__NFUN_114__(i.SubMenu, none))
			{
				PerformSelect(i);				
			}
			else
			{
				Selected = i;
			}
			// End:0x445
			break;
		// End:0x2B1
		case 37:
			// End:0x272
			if(__NFUN_119__(UWindowPulldownMenuItem(Owner), none))
			{
				UWindowPulldownMenuItem(Owner).Owner.PerformSelect(none);
				UWindowPulldownMenuItem(Owner).Owner.Selected = UWindowPulldownMenuItem(Owner);
			}
			// End:0x2AE
			if(__NFUN_119__(UWindowMenuBarItem(Owner), none))
			{
				UWindowMenuBarItem(Owner).Owner.KeyDown(Key, X, Y);
			}
			// End:0x445
			break;
		// End:0x3C2
		case 39:
			// End:0x329
			if(__NFUN_130__(__NFUN_119__(i, none), __NFUN_119__(i.SubMenu, none)))
			{
				Selected = none;
				PerformSelect(i);
				i.SubMenu.Selected = UWindowPulldownMenuItem(i.SubMenu.Items.Next);				
			}
			else
			{
				// End:0x383
				if(__NFUN_119__(UWindowPulldownMenuItem(Owner), none))
				{
					UWindowPulldownMenuItem(Owner).Owner.PerformSelect(none);
					UWindowPulldownMenuItem(Owner).Owner.KeyDown(Key, X, Y);
				}
				// End:0x3BF
				if(__NFUN_119__(UWindowMenuBarItem(Owner), none))
				{
					UWindowMenuBarItem(Owner).Owner.KeyDown(Key, X, Y);
				}
			}
			// End:0x445
			break;
		// End:0x442
		case 13:
			// End:0x3F0
			if(__NFUN_119__(i.SubMenu, none))
			{
				Selected = none;
				PerformSelect(i);				
			}
			else
			{
				// End:0x43F
				if(__NFUN_130__(__NFUN_130__(__NFUN_119__(Selected, none), __NFUN_123__(Selected.Caption, "-")), __NFUN_129__(Selected.bDisabled)))
				{
					BeforeExecuteItem(Selected);
					ExecuteItem(Selected);
				}
			}
			// End:0x445
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function KeyUp(int Key, float X, float Y)
{
	local UWindowPulldownMenuItem i;

	// End:0xCE
	if(__NFUN_130__(__NFUN_153__(Key, 65), __NFUN_152__(Key, 96)))
	{
		i = UWindowPulldownMenuItem(Items.Next);
		J0x33:

		// End:0xCE [Loop If]
		if(__NFUN_119__(i, none))
		{
			// End:0xB2
			if(__NFUN_154__(Key, int(i.HotKey)))
			{
				PerformSelect(i);
				// End:0xB2
				if(__NFUN_130__(__NFUN_130__(__NFUN_119__(i, none), __NFUN_123__(i.Caption, "-")), __NFUN_129__(i.bDisabled)))
				{
					BeforeExecuteItem(i);
					ExecuteItem(i);
				}
			}
			i = UWindowPulldownMenuItem(i.Next);
			// [Loop Continue]
			goto J0x33;
		}
	}
	return;
}

function MenuCmd(int item)
{
	local int j;
	local UWindowPulldownMenuItem i;

	i = UWindowPulldownMenuItem(Items.Next);
	J0x19:

	// End:0xA5 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x82
		if(__NFUN_154__(j, item))
		{
			PerformSelect(i);
			// End:0x80
			if(__NFUN_130__(__NFUN_123__(i.Caption, "-"), __NFUN_129__(i.bDisabled)))
			{
				BeforeExecuteItem(i);
				ExecuteItem(i);
			}
			return;
		}
		__NFUN_165__(j);
		i = UWindowPulldownMenuItem(i.Next);
		// [Loop Continue]
		goto J0x19;
	}
	return;
}

defaultproperties
{
	bAlwaysOnTop=true
}