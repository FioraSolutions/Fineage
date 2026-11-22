class UWindowPulldownMenuItem extends UWindowList;

var string Caption;
var Texture Graphic;
var byte HotKey;
var UWindowPulldownMenu SubMenu;
var bool bChecked;
var bool bDisabled;
var UWindowPulldownMenu Owner;
var float ItemTop;

function UWindowPulldownMenu CreateSubMenu(Class<UWindowPulldownMenu> MenuClass, optional UWindowWindow InOwnerWindow)
{
	SubMenu = UWindowPulldownMenu(Owner.ParentWindow.CreateWindow(MenuClass, 0.0000000, 0.0000000, 100.0000000, 100.0000000, InOwnerWindow));
	SubMenu.HideWindow();
	SubMenu.Owner = self;
	return SubMenu;
	return;
}

function Select()
{
	// End:0x7F
	if(__NFUN_119__(SubMenu, none))
	{
		SubMenu.WinLeft = __NFUN_175__(__NFUN_174__(Owner.WinLeft, Owner.WinWidth), float(Owner.HBorder));
		SubMenu.WinTop = __NFUN_175__(ItemTop, float(Owner.VBorder));
		SubMenu.ShowWindow();
	}
	return;
}

function SetCaption(string C)
{
	local string Junk, Junk2;

	Caption = C;
	HotKey = Owner.ParseAmpersand(C, Junk, Junk2, false);
	return;
}

function DeSelect()
{
	// End:0x29
	if(__NFUN_119__(SubMenu, none))
	{
		SubMenu.DeSelect();
		SubMenu.HideWindow();
	}
	return;
}

function CloseUp()
{
	Owner.CloseUp();
	return;
}

function UWindowMenuBar GetMenuBar()
{
	return Owner.GetMenuBar();
	return;
}
