class UWindowMenuBar extends UWindowListControl;

var UWindowMenuBarItem Selected;
var UWindowMenuBarItem Over;
var bool bAltDown;
var int Spacing;

function Created()
{
	ListClass = Class'UWindow.UWindowMenuBarItem';
	SetAcceptsHotKeys(true);
	super.Created();
	Spacing = 10;
	return;
}

function UWindowMenuBarItem AddHelpItem(string Caption)
{
	local UWindowMenuBarItem i;

	i = AddItem(Caption);
	i.SetHelp(true);
	return i;
	return;
}

function UWindowMenuBarItem AddItem(string Caption)
{
	local UWindowMenuBarItem i;

	i = UWindowMenuBarItem(Items.Append(Class'UWindow.UWindowMenuBarItem'));
	i.Owner = self;
	i.SetCaption(Caption);
	return i;
	return;
}

function ResolutionChanged(float W, float h)
{
	local UWindowMenuBarItem i;

	// End:0x81
	if(__NFUN_119__(Items, none))
	{
		i = UWindowMenuBarItem(Items.Next);
		J0x24:

		// End:0x81 [Loop If]
		if(__NFUN_119__(i, none))
		{
			// End:0x65
			if(__NFUN_119__(i.Menu, none))
			{
				i.Menu.ResolutionChanged(W, h);
			}
			i = UWindowMenuBarItem(i.Next);
			// [Loop Continue]
			goto J0x24;
		}
	}
	super(UWindowWindow).ResolutionChanged(W, h);
	return;
}

function Paint(Canvas C, float MouseX, float MouseY)
{
	local float X, W, h;
	local UWindowMenuBarItem i;

	DrawMenuBar(C);
	i = UWindowMenuBarItem(Items.Next);
	J0x24:

	// End:0x12D [Loop If]
	if(__NFUN_119__(i, none))
	{
		C.Font = Root.Fonts[0];
		TextSize(C, RemoveAmpersand(i.Caption), W, h);
		// End:0xC9
		if(i.bHelp)
		{
			DrawItem(C, i, __NFUN_175__(WinWidth, __NFUN_174__(W, float(Spacing))), 1.0000000, __NFUN_174__(W, float(Spacing)), 14.0000000);			
		}
		else
		{
			DrawItem(C, i, X, 1.0000000, __NFUN_174__(W, float(Spacing)), 14.0000000);
			X = __NFUN_174__(__NFUN_174__(X, W), float(Spacing));
		}
		i = UWindowMenuBarItem(i.Next);
		// [Loop Continue]
		goto J0x24;
	}
	return;
}

function MouseMove(float X, float Y)
{
	local UWindowMenuBarItem i;

	super(UWindowDialogControl).MouseMove(X, Y);
	Over = none;
	i = UWindowMenuBarItem(Items.Next);
	J0x30:

	// End:0xF5 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0xD9
		if(__NFUN_130__(__NFUN_179__(X, i.ItemLeft), __NFUN_178__(X, __NFUN_174__(i.ItemLeft, i.ItemWidth))))
		{
			// End:0xCE
			if(__NFUN_119__(Selected, none))
			{
				// End:0xCB
				if(__NFUN_119__(Selected, i))
				{
					Selected.DeSelect();
					Selected = i;
					Selected.Select();
					Select(Selected);
				}				
			}
			else
			{
				Over = i;
			}
		}
		i = UWindowMenuBarItem(i.Next);
		// [Loop Continue]
		goto J0x30;
	}
	return;
}

function MouseLeave()
{
	super(UWindowDialogControl).MouseLeave();
	Over = none;
	return;
}

function Select(UWindowMenuBarItem i)
{
	return;
}

function LMouseDown(float X, float Y)
{
	local UWindowMenuBarItem i;

	i = UWindowMenuBarItem(Items.Next);
	J0x19:

	// End:0xE7 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0xCB
		if(__NFUN_130__(__NFUN_179__(X, i.ItemLeft), __NFUN_178__(X, __NFUN_174__(i.ItemLeft, i.ItemWidth))))
		{
			// End:0x80
			if(__NFUN_119__(Selected, none))
			{
				Selected.DeSelect();
			}
			// End:0xA4
			if(__NFUN_114__(Selected, i))
			{
				Selected = none;
				Over = i;				
			}
			else
			{
				Selected = i;
				Selected.Select();
			}
			Select(Selected);
			return;
		}
		i = UWindowMenuBarItem(i.Next);
		// [Loop Continue]
		goto J0x19;
	}
	// End:0x101
	if(__NFUN_119__(Selected, none))
	{
		Selected.DeSelect();
	}
	Selected = none;
	Select(Selected);
	return;
}

function DrawItem(Canvas C, UWindowList item, float X, float Y, float W, float h)
{
	C.SetDrawColor(byte(255), byte(255), byte(255));
	UWindowMenuBarItem(item).ItemLeft = X;
	UWindowMenuBarItem(item).ItemWidth = W;
	LookAndFeel.Menu_DrawMenuBarItem(self, UWindowMenuBarItem(item), X, Y, W, h, C);
	return;
}

function DrawMenuBar(Canvas C)
{
	DrawStretchedTexture(C, 0.0000000, 0.0000000, WinWidth, 16.0000000, Texture'UWindow.Icons.MenuBar');
	return;
}

function CloseUp()
{
	// End:0x21
	if(__NFUN_119__(Selected, none))
	{
		Selected.DeSelect();
		Selected = none;
	}
	return;
}

function Close(optional bool bByParent)
{
	return;
}

function UWindowMenuBar GetMenuBar()
{
	return self;
	return;
}

function bool HotKeyDown(int Key, float X, float Y)
{
	local UWindowMenuBarItem i;

	// End:0x14
	if(__NFUN_154__(Key, 18))
	{
		bAltDown = true;
	}
	// End:0xC0
	if(bAltDown)
	{
		i = UWindowMenuBarItem(Items.Next);
		J0x36:

		// End:0xC0 [Loop If]
		if(__NFUN_119__(i, none))
		{
			// End:0xA4
			if(__NFUN_154__(Key, int(i.HotKey)))
			{
				// End:0x75
				if(__NFUN_119__(Selected, none))
				{
					Selected.DeSelect();
				}
				Selected = i;
				Selected.Select();
				Select(Selected);
				bAltDown = false;
				return true;
			}
			i = UWindowMenuBarItem(i.Next);
			// [Loop Continue]
			goto J0x36;
		}
	}
	return false;
	return;
}

function bool HotKeyUp(int Key, float X, float Y)
{
	// End:0x14
	if(__NFUN_154__(Key, 18))
	{
		bAltDown = false;
	}
	return false;
	return;
}

function KeyDown(int Key, float X, float Y)
{
	local UWindowMenuBarItem i;

	switch(Key)
	{
		// End:0x9C
		case 37:
			i = UWindowMenuBarItem(Selected.Prev);
			// End:0x5A
			if(__NFUN_132__(__NFUN_114__(i, none), __NFUN_114__(i, Items)))
			{
				i = UWindowMenuBarItem(Items.Last);
			}
			// End:0x74
			if(__NFUN_119__(Selected, none))
			{
				Selected.DeSelect();
			}
			Selected = i;
			Selected.Select();
			Select(Selected);
			// End:0x123
			break;
		// End:0x120
		case 39:
			i = UWindowMenuBarItem(Selected.Next);
			// End:0xDE
			if(__NFUN_114__(i, none))
			{
				i = UWindowMenuBarItem(Items.Next);
			}
			// End:0xF8
			if(__NFUN_119__(Selected, none))
			{
				Selected.DeSelect();
			}
			Selected = i;
			Selected.Select();
			Select(Selected);
			// End:0x123
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function MenuCmd(int Menu, int item)
{
	local UWindowMenuBarItem i;
	local int j;

	j = 0;
	i = UWindowMenuBarItem(Items.Next);
	J0x20:

	// End:0xD1 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0xAE
		if(__NFUN_130__(__NFUN_154__(j, Menu), __NFUN_119__(i.Menu, none)))
		{
			// End:0x6A
			if(__NFUN_119__(Selected, none))
			{
				Selected.DeSelect();
			}
			Selected = i;
			Selected.Select();
			Select(Selected);
			i.Menu.MenuCmd(item);
			return;
		}
		__NFUN_165__(j);
		i = UWindowMenuBarItem(i.Next);
		// [Loop Continue]
		goto J0x20;
	}
	return;
}

function MenuItemSelected(UWindowBase Sender, UWindowBase item)
{
	return;
}
