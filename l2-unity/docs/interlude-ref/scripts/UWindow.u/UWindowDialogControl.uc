class UWindowDialogControl extends UWindowWindow;

var UWindowDialogClientWindow NotifyWindow;
var string Text;
var int Font;
var Color TextColor;
var UWindowBase.TextAlign Align;
var float TextX;
var float TextY;
var bool bHasKeyboardFocus;
var bool bNoKeyboard;
var bool bAcceptExternalDragDrop;
var string HelpText;
var float MinWidth;
var float MinHeight;
var UWindowDialogControl TabNext;
var UWindowDialogControl TabPrev;

function Created()
{
	// End:0x11
	if(__NFUN_129__(bNoKeyboard))
	{
		SetAcceptsFocus();
	}
	return;
}

function KeyFocusEnter()
{
	super.KeyFocusEnter();
	bHasKeyboardFocus = true;
	return;
}

function KeyFocusExit()
{
	super.KeyFocusExit();
	bHasKeyboardFocus = false;
	return;
}

function SetHelpText(string NewHelpText)
{
	HelpText = NewHelpText;
	return;
}

function SetText(string NewText)
{
	Text = NewText;
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	super.BeforePaint(C, X, Y);
	C.Font = Root.Fonts[Font];
	return;
}

function SetFont(int NewFont)
{
	Font = NewFont;
	return;
}

function SetTextColor(Color NewColor)
{
	TextColor = NewColor;
	return;
}

function Register(UWindowDialogClientWindow W)
{
	NotifyWindow = W;
	Notify(0);
	return;
}

function Notify(byte E)
{
	// End:0x20
	if(__NFUN_119__(NotifyWindow, none))
	{
		NotifyWindow.Notify(self, E);
	}
	return;
}

function bool ExternalDragOver(UWindowDialogControl ExternalControl, float X, float Y)
{
	return false;
	return;
}

function UWindowDialogControl CheckExternalDrag(float X, float Y)
{
	local float RootX, RootY, ExtX, ExtY;
	local UWindowWindow W;
	local UWindowDialogControl C;

	WindowToGlobal(X, Y, RootX, RootY);
	W = Root.FindWindowUnder(RootX, RootY);
	C = UWindowDialogControl(W);
	// End:0xBB
	if(__NFUN_130__(__NFUN_130__(__NFUN_119__(W, self), __NFUN_119__(C, none)), C.bAcceptExternalDragDrop))
	{
		W.GlobalToWindow(RootX, RootY, ExtX, ExtY);
		// End:0xBB
		if(C.ExternalDragOver(self, ExtX, ExtY))
		{
			return C;
		}
	}
	return none;
	return;
}

function KeyDown(int Key, float X, float Y)
{
	local PlayerController P;
	local UWindowDialogControl N;

	P = Root.GetPlayerOwner();
	switch(Key)
	{
		// End:0xA0
		case int(P.Player.Console.9):
			// End:0x9D
			if(__NFUN_119__(TabNext, none))
			{
				N = TabNext;
				J0x54:

				// End:0x8C [Loop If]
				if(__NFUN_130__(__NFUN_119__(N, self), __NFUN_129__(N.bWindowVisible)))
				{
					N = N.TabNext;
					// [Loop Continue]
					goto J0x54;
				}
				N.ActivateWindow(0, false);
			}
			// End:0xBB
			break;
		// End:0xFFFF
		default:
			super.KeyDown(Key, X, Y);
			// End:0xBB
			break;
			break;
	}
	return;
}

function MouseMove(float X, float Y)
{
	super.MouseMove(X, Y);
	Notify(8);
	return;
}

function MouseEnter()
{
	super.MouseEnter();
	Notify(12);
	return;
}

function MouseLeave()
{
	super.MouseLeave();
	Notify(9);
	return;
}

defaultproperties
{
	TextColor=(R=0,G=0,B=0,A=255)
}