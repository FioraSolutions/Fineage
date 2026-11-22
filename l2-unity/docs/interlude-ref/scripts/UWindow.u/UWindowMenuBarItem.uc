class UWindowMenuBarItem extends UWindowList;

var string Caption;
var UWindowMenuBar Owner;
var UWindowPulldownMenu Menu;
var float ItemLeft;
var float ItemWidth;
var bool bHelp;
var byte HotKey;

function SetHelp(bool B)
{
	bHelp = B;
	return;
}

function SetCaption(string C)
{
	local string Junk, Junk2;

	Caption = C;
	HotKey = Owner.ParseAmpersand(C, Junk, Junk2, false);
	return;
}

function UWindowPulldownMenu CreateMenu(Class<UWindowPulldownMenu> MenuClass)
{
	Menu = UWindowPulldownMenu(Owner.ParentWindow.CreateWindow(MenuClass, 0.0000000, 0.0000000, 100.0000000, 100.0000000));
	Menu.HideWindow();
	Menu.Owner = self;
	return Menu;
	return;
}

function DeSelect()
{
	Owner.LookAndFeel.PlayMenuSound(Owner, 1);
	Menu.DeSelect();
	Menu.HideWindow();
	return;
}

function Select()
{
	Owner.LookAndFeel.PlayMenuSound(Owner, 0);
	Menu.ShowWindow();
	Menu.WinLeft = __NFUN_174__(ItemLeft, Owner.WinLeft);
	Menu.WinTop = 14.0000000;
	Menu.WinWidth = 100.0000000;
	Menu.WinHeight = 100.0000000;
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
