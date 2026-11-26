class UWindowPageControl extends UWindowTabControl;

function ResolutionChanged(float W, float h)
{
	local UWindowPageControlPage i;

	i = UWindowPageControlPage(Items.Next);
	J0x19:

	// End:0x87 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x6B
		if(__NFUN_130__(__NFUN_119__(i.Page, none), __NFUN_119__(i, SelectedTab)))
		{
			i.Page.ResolutionChanged(W, h);
		}
		i = UWindowPageControlPage(i.Next);
		// [Loop Continue]
		goto J0x19;
	}
	// End:0xB9
	if(__NFUN_119__(SelectedTab, none))
	{
		UWindowPageControlPage(SelectedTab).Page.ResolutionChanged(W, h);
	}
	return;
}

function NotifyQuitUnreal()
{
	local UWindowPageControlPage i;

	i = UWindowPageControlPage(Items.Next);
	J0x19:

	// End:0x6C [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x50
		if(__NFUN_119__(i.Page, none))
		{
			i.Page.NotifyQuitUnreal();
		}
		i = UWindowPageControlPage(i.Next);
		// [Loop Continue]
		goto J0x19;
	}
	return;
}

function NotifyBeforeLevelChange()
{
	local UWindowPageControlPage i;

	i = UWindowPageControlPage(Items.Next);
	J0x19:

	// End:0x6C [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x50
		if(__NFUN_119__(i.Page, none))
		{
			i.Page.NotifyBeforeLevelChange();
		}
		i = UWindowPageControlPage(i.Next);
		// [Loop Continue]
		goto J0x19;
	}
	return;
}

function NotifyAfterLevelChange()
{
	local UWindowPageControlPage i;

	i = UWindowPageControlPage(Items.Next);
	J0x19:

	// End:0x6C [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x50
		if(__NFUN_119__(i.Page, none))
		{
			i.Page.NotifyAfterLevelChange();
		}
		i = UWindowPageControlPage(i.Next);
		// [Loop Continue]
		goto J0x19;
	}
	return;
}

function GetDesiredDimensions(out float W, out float h)
{
	local float MaxW, MaxH, tW, tH;
	local UWindowPageControlPage i;

	MaxW = 0.0000000;
	MaxH = 0.0000000;
	i = UWindowPageControlPage(Items.Next);
	J0x2F:

	// End:0xC0 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x70
		if(__NFUN_119__(i.Page, none))
		{
			i.Page.GetDesiredDimensions(tW, tH);
		}
		// End:0x8A
		if(__NFUN_177__(tW, MaxW))
		{
			MaxW = tW;
		}
		// End:0xA4
		if(__NFUN_177__(tH, MaxH))
		{
			MaxH = tH;
		}
		i = UWindowPageControlPage(i.Next);
		// [Loop Continue]
		goto J0x2F;
	}
	W = MaxW;
	h = __NFUN_174__(MaxH, TabArea.WinHeight);
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local float OldWinHeight;
	local UWindowPageControlPage i;

	OldWinHeight = WinHeight;
	super.BeforePaint(C, X, Y);
	WinHeight = OldWinHeight;
	i = UWindowPageControlPage(Items.Next);
	J0x44:

	// End:0x89 [Loop If]
	if(__NFUN_119__(i, none))
	{
		LookAndFeel.Tab_SetTabPageSize(self, i.Page);
		i = UWindowPageControlPage(i.Next);
		// [Loop Continue]
		goto J0x44;
	}
	return;
}

function Paint(Canvas C, float X, float Y)
{
	super.Paint(C, X, Y);
	LookAndFeel.Tab_DrawTabPageArea(self, C, UWindowPageControlPage(SelectedTab).Page);
	return;
}

function UWindowPageControlPage AddPage(string Caption, Class<UWindowPageWindow> PageClass, optional name ObjectName)
{
	local UWindowPageControlPage P;

	P = UWindowPageControlPage(AddTab(Caption));
	P.Page = UWindowPageWindow(CreateWindow(PageClass, 0.0000000, __NFUN_175__(TabArea.WinHeight, float(__NFUN_147__(LookAndFeel.TabSelectedM.h, LookAndFeel.TabUnselectedM.h))), WinWidth, __NFUN_175__(WinHeight, __NFUN_175__(TabArea.WinHeight, float(__NFUN_147__(LookAndFeel.TabSelectedM.h, LookAndFeel.TabUnselectedM.h)))),,, ObjectName));
	P.Page.OwnerTab = P;
	// End:0x108
	if(__NFUN_119__(P, SelectedTab))
	{
		P.Page.HideWindow();		
	}
	else
	{
		// End:0x15D
		if(__NFUN_130__(__NFUN_119__(UWindowPageControlPage(SelectedTab), none), WindowIsVisible()))
		{
			UWindowPageControlPage(SelectedTab).Page.ShowWindow();
			UWindowPageControlPage(SelectedTab).Page.BringToFront();
		}
	}
	return P;
	return;
}

function UWindowPageControlPage InsertPage(UWindowPageControlPage BeforePage, string Caption, Class<UWindowPageWindow> PageClass, optional name ObjectName)
{
	local UWindowPageControlPage P;

	// End:0x1C
	if(__NFUN_114__(BeforePage, none))
	{
		return AddPage(Caption, PageClass);
	}
	P = UWindowPageControlPage(InsertTab(BeforePage, Caption));
	P.Page = UWindowPageWindow(CreateWindow(PageClass, 0.0000000, __NFUN_175__(TabArea.WinHeight, float(__NFUN_147__(LookAndFeel.TabSelectedM.h, LookAndFeel.TabUnselectedM.h))), WinWidth, __NFUN_175__(WinHeight, __NFUN_175__(TabArea.WinHeight, float(__NFUN_147__(LookAndFeel.TabSelectedM.h, LookAndFeel.TabUnselectedM.h)))),,, ObjectName));
	P.Page.OwnerTab = P;
	// End:0x129
	if(__NFUN_119__(P, SelectedTab))
	{
		P.Page.HideWindow();		
	}
	else
	{
		// End:0x17E
		if(__NFUN_130__(__NFUN_119__(UWindowPageControlPage(SelectedTab), none), WindowIsVisible()))
		{
			UWindowPageControlPage(SelectedTab).Page.ShowWindow();
			UWindowPageControlPage(SelectedTab).Page.BringToFront();
		}
	}
	return P;
	return;
}

function UWindowPageControlPage GetPage(string Caption)
{
	return UWindowPageControlPage(GetTab(Caption));
	return;
}

function DeletePage(UWindowPageControlPage P)
{
	P.Page.Close(true);
	P.Page.HideWindow();
	DeleteTab(P);
	return;
}

function Close(optional bool bByParent)
{
	local UWindowPageControlPage i;

	i = UWindowPageControlPage(Items.Next);
	J0x19:

	// End:0x6D [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x51
		if(__NFUN_119__(i.Page, none))
		{
			i.Page.Close(true);
		}
		i = UWindowPageControlPage(i.Next);
		// [Loop Continue]
		goto J0x19;
	}
	super(UWindowWindow).Close(bByParent);
	return;
}

function GotoTab(UWindowTabControlItem NewSelected, optional bool bByUser)
{
	local UWindowPageControlPage i;

	super.GotoTab(NewSelected, bByUser);
	i = UWindowPageControlPage(Items.Next);
	J0x2A:

	// End:0x78 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x5C
		if(__NFUN_119__(i, NewSelected))
		{
			i.Page.HideWindow();
		}
		i = UWindowPageControlPage(i.Next);
		// [Loop Continue]
		goto J0x2A;
	}
	// End:0xA5
	if(__NFUN_119__(UWindowPageControlPage(NewSelected), none))
	{
		UWindowPageControlPage(NewSelected).Page.ShowWindow();
	}
	return;
}

function UWindowPageControlPage FirstPage()
{
	return UWindowPageControlPage(Items.Next);
	return;
}

defaultproperties
{
	ListClass=Class'UWindow.UWindowPageControlPage'
}