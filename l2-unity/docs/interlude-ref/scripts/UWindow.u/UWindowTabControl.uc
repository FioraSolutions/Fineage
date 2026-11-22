class UWindowTabControl extends UWindowListControl;

var UWindowTabControlLeftButton LeftButton;
var UWindowTabControlRightButton RightButton;
var UWindowTabControlTabArea TabArea;
var UWindowTabControlItem SelectedTab;
var bool bMultiLine;
var bool bSelectNearestTabOnRemove;

function Created()
{
	super.Created();
	SelectedTab = none;
	TabArea = UWindowTabControlTabArea(CreateWindow(Class'UWindow.UWindowTabControlTabArea', 0.0000000, 0.0000000, __NFUN_175__(__NFUN_175__(__NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth), LookAndFeel.Size_ScrollbarWidth), float(10)), __NFUN_174__(LookAndFeel.Size_TabAreaHeight, LookAndFeel.Size_TabAreaOverhangHeight)));
	TabArea.bAlwaysOnTop = true;
	LeftButton = UWindowTabControlLeftButton(CreateWindow(Class'UWindow.UWindowTabControlLeftButton', __NFUN_175__(WinWidth, float(20)), 0.0000000, 10.0000000, 12.0000000));
	RightButton = UWindowTabControlRightButton(CreateWindow(Class'UWindow.UWindowTabControlRightButton', __NFUN_175__(WinWidth, float(10)), 0.0000000, 10.0000000, 12.0000000));
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	TabArea.WinTop = 0.0000000;
	TabArea.WinLeft = 0.0000000;
	// End:0x48
	if(bMultiLine)
	{
		TabArea.WinWidth = WinWidth;		
	}
	else
	{
		TabArea.WinWidth = __NFUN_175__(__NFUN_175__(__NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth), LookAndFeel.Size_ScrollbarWidth), float(10));
	}
	TabArea.LayoutTabs(C);
	WinHeight = __NFUN_174__(__NFUN_171__(LookAndFeel.Size_TabAreaHeight, float(TabArea.TabRows)), LookAndFeel.Size_TabAreaOverhangHeight);
	TabArea.WinHeight = WinHeight;
	super(UWindowDialogControl).BeforePaint(C, X, Y);
	return;
}

function SetMultiLine(bool InMultiLine)
{
	bMultiLine = InMultiLine;
	// End:0x37
	if(bMultiLine)
	{
		LeftButton.HideWindow();
		RightButton.HideWindow();		
	}
	else
	{
		LeftButton.ShowWindow();
		RightButton.ShowWindow();
	}
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local Region R;
	local Texture t;

	t = GetLookAndFeelTexture();
	R = LookAndFeel.TabBackground;
	DrawStretchedTextureSegment(C, 0.0000000, 0.0000000, WinWidth, __NFUN_171__(LookAndFeel.Size_TabAreaHeight, float(TabArea.TabRows)), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	return;
}

function UWindowTabControlItem AddTab(string Caption)
{
	local UWindowTabControlItem i;

	i = UWindowTabControlItem(Items.Append(ListClass));
	i.Owner = self;
	i.SetCaption(Caption);
	// End:0x59
	if(__NFUN_114__(SelectedTab, none))
	{
		SelectedTab = i;
	}
	return i;
	return;
}

function UWindowTabControlItem InsertTab(UWindowTabControlItem BeforeTab, string Caption)
{
	local UWindowTabControlItem i;

	i = UWindowTabControlItem(BeforeTab.InsertBefore(ListClass));
	i.Owner = self;
	i.SetCaption(Caption);
	// End:0x59
	if(__NFUN_114__(SelectedTab, none))
	{
		SelectedTab = i;
	}
	return i;
	return;
}

function GotoTab(UWindowTabControlItem NewSelected, optional bool bByUser)
{
	// End:0x2C
	if(__NFUN_130__(__NFUN_119__(SelectedTab, NewSelected), bByUser))
	{
		LookAndFeel.PlayMenuSound(self, 5);
	}
	SelectedTab = NewSelected;
	TabArea.bShowSelected = true;
	return;
}

function UWindowTabControlItem GetTab(string Caption)
{
	local UWindowTabControlItem i;

	i = UWindowTabControlItem(Items.Next);
	J0x19:

	// End:0x5E [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x42
		if(__NFUN_122__(i.Caption, Caption))
		{
			return i;
		}
		i = UWindowTabControlItem(i.Next);
		// [Loop Continue]
		goto J0x19;
	}
	return none;
	return;
}

function DeleteTab(UWindowTabControlItem tab)
{
	local UWindowTabControlItem NextTab, PrevTab;

	NextTab = UWindowTabControlItem(tab.Next);
	PrevTab = UWindowTabControlItem(tab.Prev);
	tab.Remove();
	// End:0xA1
	if(__NFUN_114__(SelectedTab, tab))
	{
		// End:0x88
		if(bSelectNearestTabOnRemove)
		{
			tab = NextTab;
			// End:0x7A
			if(__NFUN_114__(tab, none))
			{
				tab = PrevTab;
			}
			GotoTab(tab);			
		}
		else
		{
			GotoTab(UWindowTabControlItem(Items.Next));
		}
	}
	return;
}

defaultproperties
{
	ListClass=Class'UWindow.UWindowTabControlItem'
}