class WindowConsole extends Console
	config;

const MaxLines = 64;
const TextMsgSize = 128;

var Viewport Viewport;
var int Scrollback;
var int numLines;
var int TopLine;
var int TextLines;
var float MsgTime;
var float MsgTickTime;
var string MsgText[64];
var float MsgTick[64];
var int ConsoleLines;
var float ConsolePos;
var float ConsoleDest;
var bool bNoStuff;
var bool bTyping;
var bool bNoDrawWorld;
var UWindowRootWindow Root;
var() config string RootWindow;
var float OldClipX;
var float OldClipY;
var bool bCreatedRoot;
var float MouseX;
var float MouseY;
var Class<UWindowConsoleWindow> ConsoleClass;
var config float MouseScale;
var config bool ShowDesktop;
var config bool bShowConsole;
var bool bBlackout;
var bool bUWindowType;
var bool bUWindowActive;
var bool bQuickKeyEnable;
var bool bLocked;
var bool bLevelChange;
var string OldLevel;
var config Interactions.EInputKey UWindowKey;
var UWindowConsoleWindow ConsoleWindow;

function ResetUWindow()
{
	// End:0x1A
	if(__NFUN_119__(Root, none))
	{
		Root.Close();
	}
	Root = none;
	bCreatedRoot = false;
	ConsoleWindow = none;
	bShowConsole = false;
	CloseUWindow();
	return;
}

function bool KeyEvent(Interactions.EInputKey Key, Interactions.EInputAction Action, float Delta)
{
	local byte k;

	k = Key;
	switch(Action)
	{
		// End:0x78
		case 1:
			switch(k)
			{
				// End:0x3E
				case 27:
					// End:0x2E
					if(bLocked)
					{
						return true;
					}
					bQuickKeyEnable = false;
					LaunchUWindow();
					return true;
				// End:0x72
				case ConsoleKey:
					// End:0x51
					if(bLocked)
					{
						return true;
					}
					bQuickKeyEnable = true;
					LaunchUWindow();
					// End:0x70
					if(__NFUN_129__(bShowConsole))
					{
						ShowConsole();
					}
					return true;
				// End:0xFFFF
				default:
					// End:0x7B
					break;
					break;
			}
		// End:0xFFFF
		default:
			return false;
			break;
	}
	return;
}

function ShowConsole()
{
	bShowConsole = true;
	// End:0x20
	if(bCreatedRoot)
	{
		ConsoleWindow.ShowWindow();
	}
	return;
}

function HideConsole()
{
	ConsoleLines = 0;
	bShowConsole = false;
	// End:0x29
	if(__NFUN_119__(ConsoleWindow, none))
	{
		ConsoleWindow.HideWindow();
	}
	return;
}

event Tick(float Delta)
{
	super(Interaction).Tick(Delta);
	// End:0xB0
	if(__NFUN_130__(__NFUN_130__(bLevelChange, __NFUN_119__(Root, none)), __NFUN_123__(string(Viewport.Actor.Level), OldLevel)))
	{
		OldLevel = string(Viewport.Actor.Level);
		// End:0xA1
		if(__NFUN_119__(Viewport.Actor.Level, Viewport.Actor.GetEntryLevel()))
		{
			bLevelChange = false;
		}
		Root.NotifyAfterLevelChange();
	}
	return;
}

function ToggleUWindow()
{
	return;
}

function LaunchUWindow()
{
	Viewport.bSuspendPrecaching = true;
	bUWindowActive = __NFUN_129__(bQuickKeyEnable);
	Viewport.bShowWindowsMouse = true;
	// End:0x45
	if(bQuickKeyEnable)
	{
		bNoDrawWorld = false;		
	}
	else
	{
		// End:0x89
		if(__NFUN_154__(int(Viewport.Actor.Level.NetMode), int(NM_Standalone)))
		{
			Viewport.Actor.SetPause(true);
		}
		bNoDrawWorld = ShowDesktop;
	}
	// End:0xB2
	if(__NFUN_119__(Root, none))
	{
		Root.bWindowVisible = true;
	}
	__NFUN_113__('UWindow');
	return;
}

function CloseUWindow()
{
	// End:0x24
	if(__NFUN_129__(bQuickKeyEnable))
	{
		Viewport.Actor.SetPause(false);
	}
	bNoDrawWorld = false;
	bQuickKeyEnable = false;
	bUWindowActive = false;
	Viewport.bShowWindowsMouse = false;
	// End:0x69
	if(__NFUN_119__(Root, none))
	{
		Root.bWindowVisible = false;
	}
	__NFUN_113__('None');
	Viewport.bSuspendPrecaching = false;
	return;
}

function CreateRootWindow(Canvas Canvas)
{
	local int i;

	// End:0x36
	if(__NFUN_119__(Canvas, none))
	{
		OldClipX = Canvas.ClipX;
		OldClipY = Canvas.ClipY;		
	}
	else
	{
		OldClipX = 0.0000000;
		OldClipY = 0.0000000;
	}
	__NFUN_231__(__NFUN_112__("Creating root window: ", RootWindow));
	Root = new (none) Class<UWindowRootWindow>(DynamicLoadObject(RootWindow, Class'Core.Class'));
	Root.BeginPlay();
	Root.WinTop = 0.0000000;
	Root.WinLeft = 0.0000000;
	// End:0x165
	if(__NFUN_119__(Canvas, none))
	{
		Root.WinWidth = __NFUN_172__(Canvas.ClipX, Root.GUIScale);
		Root.WinHeight = __NFUN_172__(Canvas.ClipY, Root.GUIScale);
		Root.RealWidth = Canvas.ClipX;
		Root.RealHeight = Canvas.ClipY;		
	}
	else
	{
		Root.WinWidth = 0.0000000;
		Root.WinHeight = 0.0000000;
		Root.RealWidth = 0.0000000;
		Root.RealHeight = 0.0000000;
	}
	Root.ClippingRegion.X = 0;
	Root.ClippingRegion.Y = 0;
	Root.ClippingRegion.W = int(Root.WinWidth);
	Root.ClippingRegion.h = int(Root.WinHeight);
	Root.bUWindowActive = bUWindowActive;
	Root.Created();
	bCreatedRoot = true;
	ConsoleWindow = UWindowConsoleWindow(Root.CreateWindow(ConsoleClass, 100.0000000, 100.0000000, 200.0000000, 200.0000000));
	// End:0x298
	if(__NFUN_129__(bShowConsole))
	{
		HideConsole();
	}
	UWindowConsoleClientWindow(ConsoleWindow.ClientArea).TextArea.AddText(" ");
	i = 0;
	J0x2C8:

	// End:0x30F [Loop If]
	if(__NFUN_150__(i, 4))
	{
		UWindowConsoleClientWindow(ConsoleWindow.ClientArea).TextArea.AddText(MsgText[i]);
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x2C8;
	}
	return;
}

function RenderUWindow(Canvas Canvas)
{
	local UWindowWindow NewFocusWindow;

	Canvas.bNoSmooth = true;
	Canvas.Z = 1.0000000;
	Canvas.Style = 1;
	Canvas.DrawColor.R = byte(255);
	Canvas.DrawColor.G = byte(255);
	Canvas.DrawColor.B = byte(255);
	// End:0xE5
	if(__NFUN_130__(Viewport.bWindowsMouseAvailable, __NFUN_119__(Root, none)))
	{
		MouseX = __NFUN_172__(Viewport.WindowsMouseX, Root.GUIScale);
		MouseY = __NFUN_172__(Viewport.WindowsMouseY, Root.GUIScale);
	}
	// End:0xFB
	if(__NFUN_129__(bCreatedRoot))
	{
		CreateRootWindow(Canvas);
	}
	Root.bWindowVisible = true;
	Root.bUWindowActive = bUWindowActive;
	Root.bQuickKeyEnable = bQuickKeyEnable;
	// End:0x2CF
	if(__NFUN_132__(__NFUN_181__(Canvas.ClipX, OldClipX), __NFUN_181__(Canvas.ClipY, OldClipY)))
	{
		OldClipX = Canvas.ClipX;
		OldClipY = Canvas.ClipY;
		Root.WinTop = 0.0000000;
		Root.WinLeft = 0.0000000;
		Root.WinWidth = __NFUN_172__(Canvas.ClipX, Root.GUIScale);
		Root.WinHeight = __NFUN_172__(Canvas.ClipY, Root.GUIScale);
		Root.RealWidth = Canvas.ClipX;
		Root.RealHeight = Canvas.ClipY;
		Root.ClippingRegion.X = 0;
		Root.ClippingRegion.Y = 0;
		Root.ClippingRegion.W = int(Root.WinWidth);
		Root.ClippingRegion.h = int(Root.WinHeight);
		Root.Resized();
	}
	// End:0x2FB
	if(__NFUN_177__(MouseX, Root.WinWidth))
	{
		MouseX = Root.WinWidth;
	}
	// End:0x327
	if(__NFUN_177__(MouseY, Root.WinHeight))
	{
		MouseY = Root.WinHeight;
	}
	// End:0x33F
	if(__NFUN_176__(MouseX, float(0)))
	{
		MouseX = 0.0000000;
	}
	// End:0x357
	if(__NFUN_176__(MouseY, float(0)))
	{
		MouseY = 0.0000000;
	}
	NewFocusWindow = Root.CheckKeyFocusWindow();
	// End:0x3C8
	if(__NFUN_119__(NewFocusWindow, Root.KeyFocusWindow))
	{
		Root.KeyFocusWindow.KeyFocusExit();
		Root.KeyFocusWindow = NewFocusWindow;
		Root.KeyFocusWindow.KeyFocusEnter();
	}
	Root.MoveMouse(MouseX, MouseY);
	Root.WindowEvent(9, Canvas, MouseX, MouseY, 0);
	// End:0x42A
	if(__NFUN_132__(bUWindowActive, bQuickKeyEnable))
	{
		Root.DrawMouse(Canvas);
	}
	return;
}

event Message(coerce string Msg, float MsgLife)
{
	super.Message(Msg, MsgLife);
	// End:0x26
	if(__NFUN_114__(Viewport.Actor, none))
	{
		return;
	}
	// End:0x70
	if(__NFUN_130__(__NFUN_123__(Msg, ""), __NFUN_119__(ConsoleWindow, none)))
	{
		UWindowConsoleClientWindow(ConsoleWindow.ClientArea).TextArea.AddText(MsgText[TopLine]);
	}
	return;
}

function UpdateHistory()
{
	History[int(__NFUN_173__(float(__NFUN_165__(HistoryCur)), float(16)))] = TypedStr;
	// End:0x33
	if(__NFUN_151__(HistoryCur, HistoryBot))
	{
		__NFUN_165__(HistoryBot);
	}
	// End:0x58
	if(__NFUN_153__(__NFUN_147__(HistoryCur, HistoryTop), 16))
	{
		HistoryTop = __NFUN_146__(__NFUN_147__(HistoryCur, 16), 1);
	}
	return;
}

function HistoryUp()
{
	// End:0x47
	if(__NFUN_151__(HistoryCur, HistoryTop))
	{
		History[int(__NFUN_173__(float(HistoryCur), float(16)))] = TypedStr;
		TypedStr = History[int(__NFUN_173__(float(__NFUN_164__(HistoryCur)), float(16)))];
	}
	return;
}

function HistoryDown()
{
	History[int(__NFUN_173__(float(HistoryCur), float(16)))] = TypedStr;
	// End:0x4A
	if(__NFUN_150__(HistoryCur, HistoryBot))
	{
		TypedStr = History[int(__NFUN_173__(float(__NFUN_163__(HistoryCur)), float(16)))];		
	}
	else
	{
		TypedStr = "";
	}
	return;
}

function NotifyLevelChange()
{
	bLevelChange = true;
	// End:0x22
	if(__NFUN_119__(Root, none))
	{
		Root.NotifyBeforeLevelChange();
	}
	return;
}

state UWindow
{
	event Tick(float Delta)
	{
		global.Tick(Delta);
		// End:0x2A
		if(__NFUN_119__(Root, none))
		{
			Root.DoTick(Delta);
		}
		return;
	}

	function PostRender(Canvas Canvas)
	{
		// End:0x1C
		if(__NFUN_119__(Root, none))
		{
			Root.bUWindowActive = true;
		}
		RenderUWindow(Canvas);
		return;
	}

	function bool KeyType(Interactions.EInputKey Key, optional string Unicode)
	{
		// End:0x2E
		if(__NFUN_119__(Root, none))
		{
			Root.WindowEvent(8, none, MouseX, MouseY, int(Key));
		}
		return true;
		return;
	}

	function bool KeyEvent(Interactions.EInputKey Key, Interactions.EInputAction Action, float Delta)
	{
		local byte k;

		k = Key;
		switch(Action)
		{
			// End:0xF7
			case 3:
				switch(k)
				{
					// End:0x54
					case 1:
						// End:0x51
						if(__NFUN_119__(Root, none))
						{
							Root.WindowEvent(1, none, MouseX, MouseY, int(k));
						}
						// End:0xF4
						break;
					// End:0x8A
					case 2:
						// End:0x87
						if(__NFUN_119__(Root, none))
						{
							Root.WindowEvent(5, none, MouseX, MouseY, int(k));
						}
						// End:0xF4
						break;
					// End:0xC0
					case 4:
						// End:0xBD
						if(__NFUN_119__(Root, none))
						{
							Root.WindowEvent(3, none, MouseX, MouseY, int(k));
						}
						// End:0xF4
						break;
					// End:0xFFFF
					default:
						// End:0xF1
						if(__NFUN_119__(Root, none))
						{
							Root.WindowEvent(6, none, MouseX, MouseY, int(k));
						}
						// End:0xF4
						break;
						break;
				}
				goto J0x2DD;
			// End:0x286
			case 1:
				switch(k)
				{
					// End:0x121
					case 120:
						return global.KeyEvent(Key, Action, Delta);
						// End:0x283
						break;
					// End:0x18B
					case ConsoleKey:
						// End:0x14A
						if(bShowConsole)
						{
							HideConsole();
							// End:0x147
							if(bQuickKeyEnable)
							{
								CloseUWindow();
							}							
						}
						else
						{
							// End:0x165
							if(Root.bAllowConsole)
							{
								ShowConsole();								
							}
							else
							{
								Root.WindowEvent(7, none, MouseX, MouseY, int(k));
							}
						}
						// End:0x283
						break;
					// End:0x1AD
					case 27:
						// End:0x1AA
						if(__NFUN_119__(Root, none))
						{
							Root.CloseActiveWindow();
						}
						// End:0x283
						break;
					// End:0x1E3
					case 1:
						// End:0x1E0
						if(__NFUN_119__(Root, none))
						{
							Root.WindowEvent(0, none, MouseX, MouseY, int(k));
						}
						// End:0x283
						break;
					// End:0x219
					case 2:
						// End:0x216
						if(__NFUN_119__(Root, none))
						{
							Root.WindowEvent(4, none, MouseX, MouseY, int(k));
						}
						// End:0x283
						break;
					// End:0x24F
					case 4:
						// End:0x24C
						if(__NFUN_119__(Root, none))
						{
							Root.WindowEvent(2, none, MouseX, MouseY, int(k));
						}
						// End:0x283
						break;
					// End:0xFFFF
					default:
						// End:0x280
						if(__NFUN_119__(Root, none))
						{
							Root.WindowEvent(7, none, MouseX, MouseY, int(k));
						}
						// End:0x283
						break;
						break;
				}
				goto J0x2DD;
			// End:0x2D7
			case 4:
				switch(Key)
				{
					// End:0x2B3
					case 228:
						MouseX = __NFUN_174__(MouseX, __NFUN_171__(MouseScale, Delta));
						// End:0x2D7
						break;
					// End:0x2D4
					case 229:
						MouseY = __NFUN_175__(MouseY, __NFUN_171__(MouseScale, Delta));
						// End:0x2D7
						break;
					// End:0xFFFF
					default:
						break;
				}
			// End:0xFFFF
			default:
				// End:0x2DD
				break;
				break;
		}
		J0x2DD:

		return true;
		return;
	}
Begin:

	stop;			
}

defaultproperties
{
	RootWindow="UWindow.UWindowRootWindow"
	ConsoleClass=Class'UWindow.UWindowConsoleWindow'
	MouseScale=0.6000000
	ConsoleKey=192
}