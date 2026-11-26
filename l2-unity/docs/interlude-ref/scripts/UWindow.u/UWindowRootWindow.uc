class UWindowRootWindow extends UWindowWindow
	config;

var UWindowWindow MouseWindow;
var bool bMouseCapture;
var float OldMouseX;
var float OldMouseY;
var UWindowWindow FocusedWindow;
var UWindowWindow KeyFocusWindow;
var MouseCursor NormalCursor;
var MouseCursor MoveCursor;
var MouseCursor DiagCursor1;
var MouseCursor HandCursor;
var MouseCursor HSplitCursor;
var MouseCursor VSplitCursor;
var MouseCursor DiagCursor2;
var MouseCursor NSCursor;
var MouseCursor WECursor;
var MouseCursor WaitCursor;
var bool bQuickKeyEnable;
var UWindowHotkeyWindowList HotkeyWindows;
var config float GUIScale;
var float RealWidth;
var float RealHeight;
var Font Fonts[10];
var UWindowLookAndFeel LooksAndFeels[20];
var config string LookAndFeelClass;
var bool bRequestQuit;
var float QuitTime;
var bool bAllowConsole;
var config float MouseScale;
var float MouseX;
var float MouseY;
var float OldClipX;
var float OldClipY;
var Class<UWindowConsoleWindow> ConsoleClass;
var UWindowConsoleWindow ConsoleWindow;

function BeginPlay()
{
	Root = self;
	MouseWindow = self;
	KeyFocusWindow = self;
	return;
}

function UWindowLookAndFeel GetLookAndFeel(string LFClassName)
{
	local int i;
	local Class<UWindowLookAndFeel> LFClass;

	LFClass = Class<UWindowLookAndFeel>(DynamicLoadObject(LFClassName, Class'Core.Class'));
	i = 0;
	J0x22:

	// End:0xA9 [Loop If]
	if(__NFUN_150__(i, 20))
	{
		// End:0x75
		if(__NFUN_114__(LooksAndFeels[i], none))
		{
			LooksAndFeels[i] = new LFClass;
			LooksAndFeels[i].Setup();
			return LooksAndFeels[i];
		}
		// End:0x9F
		if(__NFUN_114__(LooksAndFeels[i].Class, LFClass))
		{
			return LooksAndFeels[i];
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x22;
	}
	__NFUN_231__("Out of LookAndFeel array space!!");
	return none;
	return;
}

function Created()
{
	LookAndFeel = GetLookAndFeel(LookAndFeelClass);
	SetupFonts();
	NormalCursor.Tex = Texture'UWindow.Icons.MouseCursor';
	NormalCursor.HotX = 0;
	NormalCursor.HotY = 0;
	NormalCursor.WindowsCursor = ViewportOwner.0;
	MoveCursor.Tex = Texture'UWindow.Icons.MouseMove';
	MoveCursor.HotX = 8;
	MoveCursor.HotY = 8;
	MoveCursor.WindowsCursor = ViewportOwner.1;
	DiagCursor1.Tex = Texture'UWindow.Icons.MouseDiag1';
	DiagCursor1.HotX = 8;
	DiagCursor1.HotY = 8;
	DiagCursor1.WindowsCursor = ViewportOwner.4;
	HandCursor.Tex = Texture'UWindow.Icons.MouseHand';
	HandCursor.HotX = 11;
	HandCursor.HotY = 1;
	HandCursor.WindowsCursor = ViewportOwner.0;
	HSplitCursor.Tex = Texture'UWindow.Icons.MouseHSplit';
	HSplitCursor.HotX = 9;
	HSplitCursor.HotY = 9;
	HSplitCursor.WindowsCursor = ViewportOwner.5;
	VSplitCursor.Tex = Texture'UWindow.Icons.MouseVSplit';
	VSplitCursor.HotX = 9;
	VSplitCursor.HotY = 9;
	VSplitCursor.WindowsCursor = ViewportOwner.3;
	DiagCursor2.Tex = Texture'UWindow.Icons.MouseDiag2';
	DiagCursor2.HotX = 7;
	DiagCursor2.HotY = 7;
	DiagCursor2.WindowsCursor = ViewportOwner.2;
	NSCursor.Tex = Texture'UWindow.Icons.MouseNS';
	NSCursor.HotX = 3;
	NSCursor.HotY = 7;
	NSCursor.WindowsCursor = ViewportOwner.3;
	WECursor.Tex = Texture'UWindow.Icons.MouseWE';
	WECursor.HotX = 7;
	WECursor.HotY = 3;
	WECursor.WindowsCursor = ViewportOwner.5;
	WaitCursor.Tex = Texture'UWindow.Icons.MouseWait';
	WECursor.HotX = 6;
	WECursor.HotY = 9;
	WECursor.WindowsCursor = ViewportOwner.6;
	HotkeyWindows = new Class'UWindow.UWindowHotkeyWindowList';
	HotkeyWindows.Last = HotkeyWindows;
	HotkeyWindows.Next = none;
	HotkeyWindows.Sentinel = HotkeyWindows;
	Cursor = NormalCursor;
	// End:0x32A
	if(__NFUN_119__(ConsoleClass, none))
	{
		ConsoleWindow = UWindowConsoleWindow(CreateWindow(ConsoleClass, 100.0000000, 100.0000000, 200.0000000, 200.0000000));
		ConsoleWindow.HideWindow();
	}
	return;
}

function MoveMouse(float X, float Y)
{
	local UWindowWindow NewMouseWindow;
	local float tX, tY;

	MouseX = X;
	MouseY = Y;
	// End:0x3A
	if(__NFUN_129__(bMouseCapture))
	{
		NewMouseWindow = FindWindowUnder(X, Y);		
	}
	else
	{
		NewMouseWindow = MouseWindow;
	}
	// End:0x7D
	if(__NFUN_119__(NewMouseWindow, MouseWindow))
	{
		MouseWindow.MouseLeave();
		NewMouseWindow.MouseEnter();
		MouseWindow = NewMouseWindow;
	}
	// End:0xE5
	if(__NFUN_132__(__NFUN_181__(MouseX, OldMouseX), __NFUN_181__(MouseY, OldMouseY)))
	{
		OldMouseX = MouseX;
		OldMouseY = MouseY;
		MouseWindow.GetMouseXY(tX, tY);
		MouseWindow.MouseMove(tX, tY);
	}
	return;
}

function DrawMouse(Canvas C)
{
	// End:0x37
	if(ViewportOwner.bWindowsMouseAvailable)
	{
		ViewportOwner.SelectedCursor = MouseWindow.Cursor.WindowsCursor;		
	}
	else
	{
		C.DrawColor.R = byte(255);
		C.DrawColor.G = byte(255);
		C.DrawColor.B = byte(255);
		C.bNoSmooth = true;
		C.Style = 2;
		C.SetPos(__NFUN_175__(__NFUN_171__(MouseX, GUIScale), float(MouseWindow.Cursor.HotX)), __NFUN_175__(__NFUN_171__(MouseY, GUIScale), float(MouseWindow.Cursor.HotY)));
		C.DrawIcon(MouseWindow.Cursor.Tex, 1.0000000);
		C.Style = 1;
	}
	return;
}

function bool CheckCaptureMouseUp()
{
	local float X, Y;

	// End:0x45
	if(bMouseCapture)
	{
		MouseWindow.GetMouseXY(X, Y);
		MouseWindow.LMouseUp(X, Y);
		bMouseCapture = false;
		return true;
	}
	return false;
	return;
}

function bool CheckCaptureMouseDown()
{
	local float X, Y;

	// End:0x45
	if(bMouseCapture)
	{
		MouseWindow.GetMouseXY(X, Y);
		MouseWindow.LMouseDown(X, Y);
		bMouseCapture = false;
		return true;
	}
	return false;
	return;
}

function CancelCapture()
{
	bMouseCapture = false;
	return;
}

function CaptureMouse(optional UWindowWindow W)
{
	bMouseCapture = true;
	// End:0x1E
	if(__NFUN_119__(W, none))
	{
		MouseWindow = W;
	}
	return;
}

function Texture GetLookAndFeelTexture()
{
	return LookAndFeel.active;
	return;
}

function bool IsActive()
{
	return true;
	return;
}

function AddHotkeyWindow(UWindowWindow W)
{
	UWindowHotkeyWindowList(HotkeyWindows.Insert(Class'UWindow.UWindowHotkeyWindowList')).Window = W;
	return;
}

function RemoveHotkeyWindow(UWindowWindow W)
{
	local UWindowHotkeyWindowList L;

	L = HotkeyWindows.FindWindow(W);
	// End:0x34
	if(__NFUN_119__(L, none))
	{
		L.Remove();
	}
	return;
}

function WindowEvent(UWindowWindow.WinMessage Msg, Canvas C, float X, float Y, int Key)
{
	switch(Msg)
	{
		// End:0x29
		case 7:
			// End:0x26
			if(HotKeyDown(Key, X, Y))
			{
				return;
			}
			// End:0x4E
			break;
		// End:0x4B
		case 6:
			// End:0x48
			if(HotKeyUp(Key, X, Y))
			{
				return;
			}
			// End:0x4E
			break;
		// End:0xFFFF
		default:
			break;
	}
	super.WindowEvent(Msg, C, X, Y, Key);
	return;
}

function bool HotKeyDown(int Key, float X, float Y)
{
	local UWindowHotkeyWindowList L;

	L = UWindowHotkeyWindowList(HotkeyWindows.Next);
	J0x19:

	// End:0x82 [Loop If]
	if(__NFUN_119__(L, none))
	{
		// End:0x66
		if(__NFUN_130__(__NFUN_119__(L.Window, self), L.Window.HotKeyDown(Key, X, Y)))
		{
			return true;
		}
		L = UWindowHotkeyWindowList(L.Next);
		// [Loop Continue]
		goto J0x19;
	}
	return false;
	return;
}

function bool HotKeyUp(int Key, float X, float Y)
{
	local UWindowHotkeyWindowList L;

	L = UWindowHotkeyWindowList(HotkeyWindows.Next);
	J0x19:

	// End:0x82 [Loop If]
	if(__NFUN_119__(L, none))
	{
		// End:0x66
		if(__NFUN_130__(__NFUN_119__(L.Window, self), L.Window.HotKeyUp(Key, X, Y)))
		{
			return true;
		}
		L = UWindowHotkeyWindowList(L.Next);
		// [Loop Continue]
		goto J0x19;
	}
	return false;
	return;
}

function CloseActiveWindow()
{
	// End:0x1A
	if(__NFUN_119__(ActiveWindow, none))
	{
		ActiveWindow.EscClose();
	}
	return;
}

function Resized()
{
	ResolutionChanged(WinWidth, WinHeight);
	return;
}

function SetScale(float NewScale)
{
	WinWidth = __NFUN_172__(RealWidth, NewScale);
	WinHeight = __NFUN_172__(RealHeight, NewScale);
	GUIScale = NewScale;
	ClippingRegion.X = 0;
	ClippingRegion.Y = 0;
	ClippingRegion.W = int(WinWidth);
	ClippingRegion.h = int(WinHeight);
	SetupFonts();
	Resized();
	return;
}

function SetupFonts()
{
	// End:0xD1
	if(__NFUN_180__(GUIScale, float(2)))
	{
		Fonts[0] = Font(DynamicLoadObject("UWindowFonts.Tahoma20", Class'Engine.Font'));
		Fonts[1] = Font(DynamicLoadObject("UWindowFonts.TahomaB20", Class'Engine.Font'));
		Fonts[2] = Font(DynamicLoadObject("UWindowFonts.Tahoma30", Class'Engine.Font'));
		Fonts[3] = Font(DynamicLoadObject("UWindowFonts.TahomaB30", Class'Engine.Font'));		
	}
	else
	{
		Fonts[0] = Font(DynamicLoadObject("UWindowFonts.Tahoma10", Class'Engine.Font'));
		Fonts[1] = Font(DynamicLoadObject("UWindowFonts.TahomaB10", Class'Engine.Font'));
		Fonts[2] = Font(DynamicLoadObject("UWindowFonts.Tahoma20", Class'Engine.Font'));
		Fonts[3] = Font(DynamicLoadObject("UWindowFonts.TahomaB20", Class'Engine.Font'));
	}
	return;
}

function ChangeLookAndFeel(string NewLookAndFeel)
{
	LookAndFeelClass = NewLookAndFeel;
	__NFUN_536__();
	return;
}

function HideWindow()
{
	return;
}

function SetMousePos(float X, float Y)
{
	MouseX = X;
	MouseY = Y;
	return;
}

function QuitGame()
{
	bRequestQuit = true;
	QuitTime = 0.0000000;
	NotifyQuitUnreal();
	return;
}

function DoQuitGame()
{
	__NFUN_536__();
	ViewportOwner.Actor.ConsoleCommand("exit");
	return;
}

function Tick(float Delta)
{
	// End:0x2A
	if(bRequestQuit)
	{
		// End:0x1E
		if(__NFUN_177__(QuitTime, 0.2500000))
		{
			DoQuitGame();
		}
		__NFUN_184__(QuitTime, Delta);
	}
	super.Tick(Delta);
	return;
}

event Initialized()
{
	Root = self;
	MouseWindow = self;
	KeyFocusWindow = self;
	Created();
	return;
}

function bool KeyEvent(out Interactions.EInputKey Key, out Interactions.EInputAction Action, float Delta)
{
	// End:0x33
	if(__NFUN_130__(__NFUN_154__(int(Action), int(1)), __NFUN_154__(int(Key), int(192))))
	{
		bAllowConsole = true;
		__NFUN_113__('UWindows');
		return true;
	}
	return super(Interaction).KeyEvent(Key, Action, Delta);
	return;
}

function PostRender(Canvas Canvas)
{
	local UWindowWindow NewFocusWindow;

	Canvas.bNoSmooth = true;
	Canvas.Z = 1.0000000;
	Canvas.Style = 1;
	Canvas.DrawColor.R = byte(255);
	Canvas.DrawColor.G = byte(255);
	Canvas.DrawColor.B = byte(255);
	// End:0xE5
	if(__NFUN_130__(ViewportOwner.bWindowsMouseAvailable, __NFUN_119__(Root, none)))
	{
		MouseX = __NFUN_172__(ViewportOwner.WindowsMouseX, Root.GUIScale);
		MouseY = __NFUN_172__(ViewportOwner.WindowsMouseY, Root.GUIScale);
	}
	// End:0x219
	if(__NFUN_132__(__NFUN_181__(Canvas.ClipX, OldClipX), __NFUN_181__(Canvas.ClipY, OldClipY)))
	{
		OldClipX = Canvas.ClipX;
		OldClipY = Canvas.ClipY;
		WinTop = 0.0000000;
		WinLeft = 0.0000000;
		WinWidth = __NFUN_172__(Canvas.ClipX, Root.GUIScale);
		WinHeight = __NFUN_172__(Canvas.ClipY, Root.GUIScale);
		RealWidth = Canvas.ClipX;
		RealHeight = Canvas.ClipY;
		ClippingRegion.X = 0;
		ClippingRegion.Y = 0;
		ClippingRegion.W = int(Root.WinWidth);
		ClippingRegion.h = int(Root.WinHeight);
		Resized();
	}
	// End:0x233
	if(__NFUN_177__(MouseX, WinWidth))
	{
		MouseX = WinWidth;
	}
	// End:0x24D
	if(__NFUN_177__(MouseY, WinHeight))
	{
		MouseY = WinHeight;
	}
	// End:0x265
	if(__NFUN_176__(MouseX, float(0)))
	{
		MouseX = 0.0000000;
	}
	// End:0x27D
	if(__NFUN_176__(MouseY, float(0)))
	{
		MouseY = 0.0000000;
	}
	NewFocusWindow = CheckKeyFocusWindow();
	// End:0x2C1
	if(__NFUN_119__(NewFocusWindow, KeyFocusWindow))
	{
		KeyFocusWindow.KeyFocusExit();
		KeyFocusWindow = NewFocusWindow;
		KeyFocusWindow.KeyFocusEnter();
	}
	MoveMouse(MouseX, MouseY);
	WindowEvent(9, Canvas, MouseX, MouseY, 0);
	DrawMouse(Canvas);
	return;
}

function Message(coerce string Msg, float MsgLife)
{
	PropagateMessage(Msg, MsgLife);
	// End:0x34
	if(__NFUN_119__(ConsoleWindow, none))
	{
		ConsoleWindow.PropagateMessage(Msg, MsgLife);
	}
	return;
}

state UWindows
{
	function BeginState()
	{
		bVisible = true;
		bRequiresTick = true;
		bWindowVisible = true;
		bUWindowActive = true;
		ViewportOwner.bShowWindowsMouse = true;
		ViewportOwner.bSuspendPrecaching = true;
		// End:0x86
		if(__NFUN_154__(int(ViewportOwner.Actor.Level.NetMode), int(NM_Standalone)))
		{
			ViewportOwner.Actor.SetPause(true);
		}
		// End:0x9E
		if(bAllowConsole)
		{
			ConsoleWindow.ShowWindow();
		}
		return;
	}

	function EndState()
	{
		bVisible = false;
		bRequiresTick = false;
		bUWindowActive = false;
		bWindowVisible = false;
		ViewportOwner.bShowWindowsMouse = false;
		ViewportOwner.bSuspendPrecaching = false;
		// End:0x86
		if(__NFUN_154__(int(ViewportOwner.Actor.Level.NetMode), int(NM_Standalone)))
		{
			ViewportOwner.Actor.SetPause(false);
		}
		ConsoleWindow.HideWindow();
		return;
	}

	function bool KeyType(out Interactions.EInputKey Key, optional string Unicode)
	{
		WindowEvent(8, none, MouseX, MouseY, int(Key));
		return true;
		return;
	}

	function bool KeyEvent(Interactions.EInputKey Key, Interactions.EInputAction Action, float Delta)
	{
		local byte k;

		k = Key;
		switch(Action)
		{
			// End:0xB5
			case 3:
				switch(k)
				{
					// End:0x2C
					case 27:
						CloseActiveWindow();
						// End:0xB2
						break;
					// End:0x4E
					case 1:
						WindowEvent(1, none, MouseX, MouseY, int(k));
						// End:0xB2
						break;
					// End:0x70
					case 2:
						WindowEvent(5, none, MouseX, MouseY, int(k));
						// End:0xB2
						break;
					// End:0x92
					case 4:
						WindowEvent(3, none, MouseX, MouseY, int(k));
						// End:0xB2
						break;
					// End:0xFFFF
					default:
						WindowEvent(6, none, MouseX, MouseY, int(k));
						// End:0xB2
						break;
						break;
				}
				goto J0x1A1;
			// End:0x14A
			case 1:
				switch(k)
				{
					// End:0xE3
					case 1:
						WindowEvent(0, none, MouseX, MouseY, int(k));
						// End:0x147
						break;
					// End:0x105
					case 2:
						WindowEvent(4, none, MouseX, MouseY, int(k));
						// End:0x147
						break;
					// End:0x127
					case 4:
						WindowEvent(2, none, MouseX, MouseY, int(k));
						// End:0x147
						break;
					// End:0xFFFF
					default:
						WindowEvent(7, none, MouseX, MouseY, int(k));
						// End:0x147
						break;
						break;
				}
				goto J0x1A1;
			// End:0x19B
			case 4:
				switch(Key)
				{
					// End:0x177
					case 228:
						MouseX = __NFUN_174__(MouseX, __NFUN_171__(MouseScale, Delta));
						// End:0x19B
						break;
					// End:0x198
					case 229:
						MouseY = __NFUN_175__(MouseY, __NFUN_171__(MouseScale, Delta));
						// End:0x19B
						break;
					// End:0xFFFF
					default:
						break;
				}
			// End:0xFFFF
			default:
				// End:0x1A1
				break;
				break;
		}
		J0x1A1:

		return true;
		return;
	}
	stop;
}

defaultproperties
{
	GUIScale=1.0000000
	bAllowConsole=true
	MouseScale=0.6000000
	ConsoleClass=Class'UWindow.UWindowConsoleWindow'
}