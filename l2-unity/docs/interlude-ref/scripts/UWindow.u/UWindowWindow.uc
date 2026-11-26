class UWindowWindow extends UWindowBase;

const DE_Created = 0;
const DE_Change = 1;
const DE_Click = 2;
const DE_Enter = 3;
const DE_Exit = 4;
const DE_MClick = 5;
const DE_RClick = 6;
const DE_EnterPressed = 7;
const DE_MouseMove = 8;
const DE_MouseLeave = 9;
const DE_LMouseDown = 10;
const DE_DoubleClick = 11;
const DE_MouseEnter = 12;
const DE_HelpChanged = 13;
const DE_WheelUpPressed = 14;
const DE_WheelDownPressed = 15;

enum WinMessage
{
	WM_LMouseDown,                  // 0
	WM_LMouseUp,                    // 1
	WM_MMouseDown,                  // 2
	WM_MMouseUp,                    // 3
	WM_RMouseDown,                  // 4
	WM_RMouseUp,                    // 5
	WM_KeyUp,                       // 6
	WM_KeyDown,                     // 7
	WM_KeyType,                     // 8
	WM_Paint                        // 9
};

struct MouseCursor
{
	var Texture Tex;
	var int HotX;
	var int HotY;
	var byte WindowsCursor;
};

var float WinLeft;
var float WinTop;
var float WinWidth;
var float WinHeight;
var UWindowWindow ParentWindow;
var UWindowWindow FirstChildWindow;
var UWindowWindow LastChildWindow;
var UWindowWindow NextSiblingWindow;
var UWindowWindow PrevSiblingWindow;
var UWindowWindow ActiveWindow;
var UWindowRootWindow Root;
var UWindowWindow OwnerWindow;
var UWindowWindow ModalWindow;
var bool bWindowVisible;
var bool bNoClip;
var bool bMouseDown;
var bool bRMouseDown;
var bool bMMouseDown;
var bool bAlwaysBehind;
var bool bAcceptsFocus;
var bool bAlwaysOnTop;
var bool bLeaveOnscreen;
var bool bUWindowActive;
var bool bTransient;
var bool bAcceptsHotKeys;
var bool bIgnoreLDoubleClick;
var bool bIgnoreMDoubleClick;
var bool bIgnoreRDoubleClick;
var float ClickTime;
var float MClickTime;
var float RClickTime;
var float ClickX;
var float ClickY;
var float MClickX;
var float MClickY;
var float RClickX;
var float RClickY;
var UWindowLookAndFeel LookAndFeel;
var Region ClippingRegion;
var MouseCursor Cursor;

function WindowEvent(UWindowWindow.WinMessage Msg, Canvas C, float X, float Y, int Key)
{
	switch(Msg)
	{
		// End:0x39
		case 9:
			Paint(C, X, Y);
			PaintClients(C, X, Y);
			// End:0x292
			break;
		// End:0x89
		case 0:
			// End:0x86
			if(__NFUN_129__(Root.CheckCaptureMouseDown()))
			{
				// End:0x86
				if(__NFUN_129__(MessageClients(Msg, C, X, Y, Key)))
				{
					LMouseDown(X, Y);
				}
			}
			// End:0x292
			break;
		// End:0xD9
		case 1:
			// End:0xD6
			if(__NFUN_129__(Root.CheckCaptureMouseUp()))
			{
				// End:0xD6
				if(__NFUN_129__(MessageClients(Msg, C, X, Y, Key)))
				{
					LMouseUp(X, Y);
				}
			}
			// End:0x292
			break;
		// End:0x115
		case 4:
			// End:0x112
			if(__NFUN_129__(MessageClients(Msg, C, X, Y, Key)))
			{
				RMouseDown(X, Y);
			}
			// End:0x292
			break;
		// End:0x151
		case 5:
			// End:0x14E
			if(__NFUN_129__(MessageClients(Msg, C, X, Y, Key)))
			{
				RMouseUp(X, Y);
			}
			// End:0x292
			break;
		// End:0x18D
		case 2:
			// End:0x18A
			if(__NFUN_129__(MessageClients(Msg, C, X, Y, Key)))
			{
				MMouseDown(X, Y);
			}
			// End:0x292
			break;
		// End:0x1C9
		case 3:
			// End:0x1C6
			if(__NFUN_129__(MessageClients(Msg, C, X, Y, Key)))
			{
				MMouseUp(X, Y);
			}
			// End:0x292
			break;
		// End:0x20A
		case 7:
			// End:0x207
			if(__NFUN_129__(PropagateKey(Msg, C, X, Y, Key)))
			{
				KeyDown(Key, X, Y);
			}
			// End:0x292
			break;
		// End:0x24B
		case 6:
			// End:0x248
			if(__NFUN_129__(PropagateKey(Msg, C, X, Y, Key)))
			{
				KeyUp(Key, X, Y);
			}
			// End:0x292
			break;
		// End:0x28C
		case 8:
			// End:0x289
			if(__NFUN_129__(PropagateKey(Msg, C, X, Y, Key)))
			{
				KeyPressed(Key, X, Y);
			}
			// End:0x292
			break;
		// End:0xFFFF
		default:
			// End:0x292
			break;
			break;
	}
	return;
}

function SaveConfigs()
{
	return;
}

final function PlayerController GetPlayerOwner()
{
	return Root.ViewportOwner.Actor;
	return;
}

final function LevelInfo GetLevel()
{
	return Root.ViewportOwner.Actor.Level;
	return;
}

final function LevelInfo GetEntryLevel()
{
	return Root.ViewportOwner.Actor.super(UWindowWindow).GetEntryLevel();
	return;
}

function Resized()
{
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	return;
}

function AfterPaint(Canvas C, float X, float Y)
{
	return;
}

function Paint(Canvas C, float X, float Y)
{
	return;
}

function Click(float X, float Y)
{
	return;
}

function MClick(float X, float Y)
{
	return;
}

function RClick(float X, float Y)
{
	return;
}

function DoubleClick(float X, float Y)
{
	return;
}

function MDoubleClick(float X, float Y)
{
	return;
}

function RDoubleClick(float X, float Y)
{
	return;
}

function BeginPlay()
{
	return;
}

function BeforeCreate()
{
	return;
}

function Created()
{
	return;
}

function AfterCreate()
{
	return;
}

function MouseEnter()
{
	return;
}

function Activated()
{
	return;
}

function Deactivated()
{
	return;
}

function MouseLeave()
{
	bMouseDown = false;
	bMMouseDown = false;
	bRMouseDown = false;
	return;
}

function MouseMove(float X, float Y)
{
	return;
}

function KeyUp(int Key, float X, float Y)
{
	return;
}

function KeyDown(int Key, float X, float Y)
{
	return;
}

function bool HotKeyDown(int Key, float X, float Y)
{
	return false;
	return;
}

function bool HotKeyUp(int Key, float X, float Y)
{
	return false;
	return;
}

function KeyPressed(int Key, float X, float Y)
{
	return;
}

function ProcessMenuKey(int Key, string KeyName)
{
	return;
}

function KeyFocusEnter()
{
	return;
}

function KeyFocusExit()
{
	return;
}

function RMouseDown(float X, float Y)
{
	ActivateWindow(0, false);
	bRMouseDown = true;
	return;
}

function RMouseUp(float X, float Y)
{
	// End:0xBF
	if(bRMouseDown)
	{
		// End:0x84
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_129__(bIgnoreRDoubleClick), __NFUN_178__(__NFUN_186__(__NFUN_175__(X, RClickX)), float(1))), __NFUN_178__(__NFUN_186__(__NFUN_175__(Y, RClickY)), float(1))), __NFUN_176__(GetLevel().TimeSeconds, __NFUN_174__(RClickTime, 0.6000000))))
		{
			RDoubleClick(X, Y);
			RClickTime = 0.0000000;			
		}
		else
		{
			RClickTime = GetLevel().TimeSeconds;
			RClickX = X;
			RClickY = Y;
			RClick(X, Y);
		}
	}
	bRMouseDown = false;
	return;
}

function MMouseDown(float X, float Y)
{
	ActivateWindow(0, false);
	bMMouseDown = true;
	return;
}

function MMouseUp(float X, float Y)
{
	// End:0xBD
	if(bMMouseDown)
	{
		// End:0x82
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_129__(bIgnoreMDoubleClick), __NFUN_178__(__NFUN_186__(__NFUN_175__(X, MClickX)), float(1))), __NFUN_178__(__NFUN_175__(Y, MClickY), float(1))), __NFUN_176__(GetLevel().TimeSeconds, __NFUN_174__(MClickTime, 0.6000000))))
		{
			MDoubleClick(X, Y);
			MClickTime = 0.0000000;			
		}
		else
		{
			MClickTime = GetLevel().TimeSeconds;
			MClickX = X;
			MClickY = Y;
			MClick(X, Y);
		}
	}
	bMMouseDown = false;
	return;
}

function LMouseDown(float X, float Y)
{
	ActivateWindow(0, false);
	bMouseDown = true;
	return;
}

function LMouseUp(float X, float Y)
{
	// End:0xBD
	if(bMouseDown)
	{
		// End:0x82
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_129__(bIgnoreLDoubleClick), __NFUN_178__(__NFUN_186__(__NFUN_175__(X, ClickX)), float(1))), __NFUN_178__(__NFUN_175__(Y, ClickY), float(1))), __NFUN_176__(GetLevel().TimeSeconds, __NFUN_174__(ClickTime, 0.6000000))))
		{
			DoubleClick(X, Y);
			ClickTime = 0.0000000;			
		}
		else
		{
			ClickTime = GetLevel().TimeSeconds;
			ClickX = X;
			ClickY = Y;
			Click(X, Y);
		}
	}
	bMouseDown = false;
	return;
}

function FocusWindow()
{
	// End:0x43
	if(__NFUN_130__(__NFUN_119__(Root.FocusedWindow, none), __NFUN_119__(Root.FocusedWindow, self)))
	{
		Root.FocusedWindow.FocusOtherWindow(self);
	}
	Root.FocusedWindow = self;
	return;
}

function FocusOtherWindow(UWindowWindow W)
{
	return;
}

function EscClose()
{
	Close();
	return;
}

function Close(optional bool bByParent)
{
	local UWindowWindow Prev, Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x48 [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Prev = Child.PrevSiblingWindow;
		Child.Close(true);
		Child = Prev;
		// [Loop Continue]
		goto J0x0B;
	}
	SaveConfigs();
	// End:0x5F
	if(__NFUN_129__(bByParent))
	{
		HideWindow();
	}
	return;
}

final function SetSize(float W, float h)
{
	// End:0x3C
	if(__NFUN_132__(__NFUN_181__(WinWidth, W), __NFUN_181__(WinHeight, h)))
	{
		WinWidth = W;
		WinHeight = h;
		Resized();
	}
	return;
}

function Tick(float Delta)
{
	return;
}

final function DoTick(float Delta)
{
	local UWindowWindow Child;

	Tick(Delta);
	Child = FirstChildWindow;
	J0x16:

	// End:0x99 [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.bUWindowActive = bUWindowActive;
		// End:0x51
		if(bLeaveOnscreen)
		{
			Child.bLeaveOnscreen = true;
		}
		// End:0x82
		if(__NFUN_132__(bUWindowActive, Child.bLeaveOnscreen))
		{
			Child.super(UWindowWindow).DoTick(Delta);
		}
		Child = Child.NextSiblingWindow;
		// [Loop Continue]
		goto J0x16;
	}
	return;
}

final function PaintClients(Canvas C, float X, float Y)
{
	local float OrgX, OrgY, ClipX, ClipY;
	local UWindowWindow Child;

	OrgX = C.OrgX;
	OrgY = C.OrgY;
	ClipX = C.ClipX;
	ClipY = C.ClipY;
	Child = FirstChildWindow;
	J0x5B:

	// End:0x5B0 [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.bUWindowActive = bUWindowActive;
		C.SetPos(0.0000000, 0.0000000);
		C.Style = GetPlayerOwner().1;
		C.SetDrawColor(byte(255), byte(255), byte(255));
		C.SpaceX = 0.0000000;
		C.SpaceY = 0.0000000;
		Child.BeforePaint(C, __NFUN_175__(X, Child.WinLeft), __NFUN_175__(Y, Child.WinTop));
		// End:0x14B
		if(bLeaveOnscreen)
		{
			Child.bLeaveOnscreen = true;
		}
		// End:0x599
		if(__NFUN_132__(bUWindowActive, Child.bLeaveOnscreen))
		{
			C.OrgX = __NFUN_174__(C.OrgX, __NFUN_171__(Child.WinLeft, Root.GUIScale));
			C.OrgY = __NFUN_174__(C.OrgY, __NFUN_171__(Child.WinTop, Root.GUIScale));
			// End:0x4BE
			if(__NFUN_129__(Child.bNoClip))
			{
				C.ClipX = __NFUN_171__(__NFUN_244__(__NFUN_175__(WinWidth, Child.WinLeft), Child.WinWidth), Root.GUIScale);
				C.ClipY = __NFUN_171__(__NFUN_244__(__NFUN_175__(WinHeight, Child.WinTop), Child.WinHeight), Root.GUIScale);
				Child.ClippingRegion.X = int(__NFUN_175__(float(ClippingRegion.X), Child.WinLeft));
				Child.ClippingRegion.Y = int(__NFUN_175__(float(ClippingRegion.Y), Child.WinTop));
				Child.ClippingRegion.W = ClippingRegion.W;
				Child.ClippingRegion.h = ClippingRegion.h;
				// End:0x374
				if(__NFUN_150__(Child.ClippingRegion.X, 0))
				{
					__NFUN_161__(Child.ClippingRegion.W, Child.ClippingRegion.X);
					Child.ClippingRegion.X = 0;
				}
				// End:0x3CA
				if(__NFUN_150__(Child.ClippingRegion.Y, 0))
				{
					__NFUN_161__(Child.ClippingRegion.h, Child.ClippingRegion.Y);
					Child.ClippingRegion.Y = 0;
				}
				// End:0x444
				if(__NFUN_177__(float(Child.ClippingRegion.W), __NFUN_175__(Child.WinWidth, float(Child.ClippingRegion.X))))
				{
					Child.ClippingRegion.W = int(__NFUN_175__(Child.WinWidth, float(Child.ClippingRegion.X)));
				}
				// End:0x4BE
				if(__NFUN_177__(float(Child.ClippingRegion.h), __NFUN_175__(Child.WinHeight, float(Child.ClippingRegion.Y))))
				{
					Child.ClippingRegion.h = int(__NFUN_175__(Child.WinHeight, float(Child.ClippingRegion.Y)));
				}
			}
			// End:0x571
			if(__NFUN_130__(__NFUN_151__(Child.ClippingRegion.W, 0), __NFUN_151__(Child.ClippingRegion.h, 0)))
			{
				Child.WindowEvent(9, C, __NFUN_175__(X, Child.WinLeft), __NFUN_175__(Y, Child.WinTop), 0);
				Child.AfterPaint(C, __NFUN_175__(X, Child.WinLeft), __NFUN_175__(Y, Child.WinTop));
			}
			C.OrgX = OrgX;
			C.OrgY = OrgY;
		}
		Child = Child.NextSiblingWindow;
		// [Loop Continue]
		goto J0x5B;
	}
	C.ClipX = ClipX;
	C.ClipY = ClipY;
	return;
}

final function UWindowWindow FindWindowUnder(float X, float Y)
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x17A [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.bUWindowActive = bUWindowActive;
		// End:0x46
		if(bLeaveOnscreen)
		{
			Child.bLeaveOnscreen = true;
		}
		// End:0x163
		if(__NFUN_132__(bUWindowActive, Child.bLeaveOnscreen))
		{
			// End:0x163
			if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_179__(X, Child.WinLeft), __NFUN_178__(X, __NFUN_174__(Child.WinLeft, Child.WinWidth))), __NFUN_179__(Y, Child.WinTop)), __NFUN_178__(Y, __NFUN_174__(Child.WinTop, Child.WinHeight))), __NFUN_129__(Child.CheckMousePassThrough(__NFUN_175__(X, Child.WinLeft), __NFUN_175__(Y, Child.WinTop)))))
			{
				return Child.super(UWindowWindow).FindWindowUnder(__NFUN_175__(X, Child.WinLeft), __NFUN_175__(Y, Child.WinTop));
			}
		}
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return self;
	return;
}

final function bool PropagateKey(UWindowWindow.WinMessage Msg, Canvas C, float X, float Y, int Key)
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	// End:0x48
	if(__NFUN_130__(__NFUN_130__(__NFUN_119__(ActiveWindow, none), __NFUN_119__(Child, ActiveWindow)), __NFUN_129__(Child.bTransient)))
	{
		Child = ActiveWindow;
	}
	J0x48:

	// End:0x115 [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.bUWindowActive = bUWindowActive;
		// End:0x83
		if(bLeaveOnscreen)
		{
			Child.bLeaveOnscreen = true;
		}
		// End:0xFE
		if(__NFUN_130__(__NFUN_132__(bUWindowActive, Child.bLeaveOnscreen), Child.bAcceptsFocus))
		{
			Child.WindowEvent(Msg, C, __NFUN_175__(X, Child.WinLeft), __NFUN_175__(Y, Child.WinTop), Key);
			return true;
		}
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x48;
	}
	return false;
	return;
}

final function bool PropagateMessage(coerce string Msg, float MsgLife)
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	// End:0x48
	if(__NFUN_130__(__NFUN_130__(__NFUN_119__(ActiveWindow, none), __NFUN_119__(Child, ActiveWindow)), __NFUN_129__(Child.bTransient)))
	{
		Child = ActiveWindow;
	}
	J0x48:

	// End:0xFF [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.bUWindowActive = bUWindowActive;
		// End:0x83
		if(bLeaveOnscreen)
		{
			Child.bLeaveOnscreen = true;
		}
		// End:0xE8
		if(__NFUN_130__(__NFUN_132__(bUWindowActive, Child.bLeaveOnscreen), Child.bAcceptsFocus))
		{
			Child.Message(Msg, MsgLife);
			Child.super(UWindowWindow).PropagateMessage(Msg, MsgLife);
			return true;
		}
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x48;
	}
	return false;
	return;
}

final function UWindowWindow CheckKeyFocusWindow()
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	// End:0x48
	if(__NFUN_130__(__NFUN_130__(__NFUN_119__(ActiveWindow, none), __NFUN_119__(Child, ActiveWindow)), __NFUN_129__(Child.bTransient)))
	{
		Child = ActiveWindow;
	}
	J0x48:

	// End:0xD9 [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.bUWindowActive = bUWindowActive;
		// End:0x83
		if(bLeaveOnscreen)
		{
			Child.bLeaveOnscreen = true;
		}
		// End:0xC2
		if(__NFUN_132__(bUWindowActive, Child.bLeaveOnscreen))
		{
			// End:0xC2
			if(Child.bAcceptsFocus)
			{
				return Child.super(UWindowWindow).CheckKeyFocusWindow();
			}
		}
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x48;
	}
	return self;
	return;
}

final function bool MessageClients(UWindowWindow.WinMessage Msg, Canvas C, float X, float Y, int Key)
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x18A [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.bUWindowActive = bUWindowActive;
		// End:0x46
		if(bLeaveOnscreen)
		{
			Child.bLeaveOnscreen = true;
		}
		// End:0x173
		if(__NFUN_132__(bUWindowActive, Child.bLeaveOnscreen))
		{
			// End:0x173
			if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_179__(X, Child.WinLeft), __NFUN_178__(X, __NFUN_174__(Child.WinLeft, Child.WinWidth))), __NFUN_179__(Y, Child.WinTop)), __NFUN_178__(Y, __NFUN_174__(Child.WinTop, Child.WinHeight))), __NFUN_129__(Child.CheckMousePassThrough(__NFUN_175__(X, Child.WinLeft), __NFUN_175__(Y, Child.WinTop)))))
			{
				Child.WindowEvent(Msg, C, __NFUN_175__(X, Child.WinLeft), __NFUN_175__(Y, Child.WinTop), Key);
				return true;
			}
		}
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return false;
	return;
}

final function ActivateWindow(int Depth, bool bTransientNoDeactivate)
{
	// End:0x1E
	if(__NFUN_114__(self, Root))
	{
		// End:0x1C
		if(__NFUN_154__(Depth, 0))
		{
			FocusWindow();
		}
		return;
	}
	// End:0x29
	if(WaitModal())
	{
		return;
	}
	// End:0x54
	if(__NFUN_129__(bAlwaysBehind))
	{
		ParentWindow.HideChildWindow(self);
		ParentWindow.ShowChildWindow(self);
	}
	// End:0xDD
	if(__NFUN_129__(__NFUN_132__(bTransient, bTransientNoDeactivate)))
	{
		// End:0xAC
		if(__NFUN_130__(__NFUN_119__(ParentWindow.ActiveWindow, none), __NFUN_119__(ParentWindow.ActiveWindow, self)))
		{
			ParentWindow.ActiveWindow.Deactivated();
		}
		ParentWindow.ActiveWindow = self;
		ParentWindow.super(UWindowWindow).ActivateWindow(__NFUN_146__(Depth, 1), false);
		Activated();		
	}
	else
	{
		ParentWindow.super(UWindowWindow).ActivateWindow(__NFUN_146__(Depth, 1), true);
	}
	// End:0x106
	if(__NFUN_154__(Depth, 0))
	{
		FocusWindow();
	}
	return;
}

final function BringToFront()
{
	// End:0x0D
	if(__NFUN_114__(self, Root))
	{
		return;
	}
	// End:0x45
	if(__NFUN_130__(__NFUN_129__(bAlwaysBehind), __NFUN_129__(WaitModal())))
	{
		ParentWindow.HideChildWindow(self);
		ParentWindow.ShowChildWindow(self);
	}
	ParentWindow.super(UWindowWindow).BringToFront();
	return;
}

final function SendToBack()
{
	ParentWindow.HideChildWindow(self);
	ParentWindow.ShowChildWindow(self, true);
	return;
}

final function HideChildWindow(UWindowWindow Child)
{
	local UWindowWindow Window;

	// End:0x16
	if(__NFUN_129__(Child.bWindowVisible))
	{
		return;
	}
	Child.bWindowVisible = false;
	// End:0x4D
	if(Child.bAcceptsHotKeys)
	{
		Root.RemoveHotkeyWindow(Child);
	}
	// End:0x98
	if(__NFUN_114__(LastChildWindow, Child))
	{
		LastChildWindow = Child.PrevSiblingWindow;
		// End:0x8E
		if(__NFUN_119__(LastChildWindow, none))
		{
			LastChildWindow.NextSiblingWindow = none;			
		}
		else
		{
			FirstChildWindow = none;
		}		
	}
	else
	{
		// End:0xE3
		if(__NFUN_114__(FirstChildWindow, Child))
		{
			FirstChildWindow = Child.NextSiblingWindow;
			// End:0xD9
			if(__NFUN_119__(FirstChildWindow, none))
			{
				FirstChildWindow.PrevSiblingWindow = none;				
			}
			else
			{
				LastChildWindow = none;
			}			
		}
		else
		{
			Window = FirstChildWindow;
			J0xEE:

			// End:0x165 [Loop If]
			if(__NFUN_119__(Window, none))
			{
				// End:0x14E
				if(__NFUN_114__(Window.NextSiblingWindow, Child))
				{
					Window.NextSiblingWindow = Child.NextSiblingWindow;
					Window.NextSiblingWindow.PrevSiblingWindow = Window;
					// [Explicit Break]
					goto J0x165;
				}
				Window = Window.NextSiblingWindow;
				// [Loop Continue]
				goto J0xEE;
			}
		}
	}
	J0x165:

	ActiveWindow = none;
	Window = LastChildWindow;
	J0x177:

	// End:0x1BB [Loop If]
	if(__NFUN_119__(Window, none))
	{
		// End:0x1A4
		if(__NFUN_129__(Window.bAlwaysOnTop))
		{
			ActiveWindow = Window;
			// [Explicit Break]
			goto J0x1BB;
		}
		Window = Window.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x177;
	}
	J0x1BB:

	// End:0x1D1
	if(__NFUN_114__(ActiveWindow, none))
	{
		ActiveWindow = LastChildWindow;
	}
	return;
}

final function SetAcceptsFocus()
{
	// End:0x0B
	if(bAcceptsFocus)
	{
		return;
	}
	bAcceptsFocus = true;
	// End:0x2D
	if(__NFUN_119__(self, Root))
	{
		ParentWindow.super(UWindowWindow).SetAcceptsFocus();
	}
	return;
}

final function CancelAcceptsFocus()
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x3C [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.super(UWindowWindow).CancelAcceptsFocus();
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	bAcceptsFocus = false;
	return;
}

final function GetMouseXY(out float X, out float Y)
{
	local UWindowWindow P;

	X = float(int(Root.MouseX));
	Y = float(int(Root.MouseY));
	P = self;
	J0x37:

	// End:0x93 [Loop If]
	if(__NFUN_119__(P, Root))
	{
		X = __NFUN_175__(X, P.WinLeft);
		Y = __NFUN_175__(Y, P.WinTop);
		P = P.ParentWindow;
		// [Loop Continue]
		goto J0x37;
	}
	return;
}

final function GlobalToWindow(float GlobalX, float GlobalY, out float WinX, out float WinY)
{
	local UWindowWindow P;

	WinX = GlobalX;
	WinY = GlobalY;
	P = self;
	J0x1D:

	// End:0x6D [Loop If]
	if(__NFUN_119__(P, Root))
	{
		__NFUN_185__(WinX, P.WinLeft);
		__NFUN_185__(WinY, P.WinTop);
		P = P.ParentWindow;
		// [Loop Continue]
		goto J0x1D;
	}
	return;
}

final function WindowToGlobal(float WinX, float WinY, out float GlobalX, out float GlobalY)
{
	local UWindowWindow P;

	GlobalX = WinX;
	GlobalY = WinY;
	P = self;
	J0x1D:

	// End:0x6D [Loop If]
	if(__NFUN_119__(P, Root))
	{
		__NFUN_184__(GlobalX, P.WinLeft);
		__NFUN_184__(GlobalY, P.WinTop);
		P = P.ParentWindow;
		// [Loop Continue]
		goto J0x1D;
	}
	return;
}

final function ShowChildWindow(UWindowWindow Child, optional bool bAtBack)
{
	local UWindowWindow W;

	// End:0x1F
	if(__NFUN_129__(Child.bTransient))
	{
		ActiveWindow = Child;
	}
	// End:0x33
	if(Child.bWindowVisible)
	{
		return;
	}
	Child.bWindowVisible = true;
	// End:0x6A
	if(Child.bAcceptsHotKeys)
	{
		Root.AddHotkeyWindow(Child);
	}
	// End:0xFD
	if(bAtBack)
	{
		// End:0xB7
		if(__NFUN_114__(FirstChildWindow, none))
		{
			Child.NextSiblingWindow = none;
			Child.PrevSiblingWindow = none;
			LastChildWindow = Child;
			FirstChildWindow = Child;			
		}
		else
		{
			FirstChildWindow.PrevSiblingWindow = Child;
			Child.NextSiblingWindow = FirstChildWindow;
			Child.PrevSiblingWindow = none;
			FirstChildWindow = Child;
		}		
	}
	else
	{
		W = LastChildWindow;
		J0x108:

		// End:0x274 [Loop If]
		if(true)
		{
			// End:0x25D
			if(__NFUN_132__(__NFUN_132__(Child.bAlwaysOnTop, __NFUN_114__(W, none)), __NFUN_129__(W.bAlwaysOnTop)))
			{
				// End:0x1D6
				if(__NFUN_114__(W, none))
				{
					// End:0x190
					if(__NFUN_114__(LastChildWindow, none))
					{
						Child.NextSiblingWindow = none;
						Child.PrevSiblingWindow = none;
						LastChildWindow = Child;
						FirstChildWindow = Child;						
					}
					else
					{
						Child.NextSiblingWindow = FirstChildWindow;
						Child.PrevSiblingWindow = none;
						FirstChildWindow.PrevSiblingWindow = Child;
						FirstChildWindow = Child;
					}					
				}
				else
				{
					Child.NextSiblingWindow = W.NextSiblingWindow;
					Child.PrevSiblingWindow = W;
					// End:0x23B
					if(__NFUN_119__(W.NextSiblingWindow, none))
					{
						W.NextSiblingWindow.PrevSiblingWindow = Child;						
					}
					else
					{
						LastChildWindow = Child;
					}
					W.NextSiblingWindow = Child;
				}
				// [Explicit Break]
				goto J0x274;
			}
			W = W.PrevSiblingWindow;
			// [Loop Continue]
			goto J0x108;
		}
	}
	J0x274:

	return;
}

function ShowWindow()
{
	ParentWindow.ShowChildWindow(self);
	WindowShown();
	return;
}

function HideWindow()
{
	WindowHidden();
	ParentWindow.HideChildWindow(self);
	return;
}

final function UWindowWindow CreateWindow(Class<UWindowWindow> WndClass, float X, float Y, float W, float h, optional UWindowWindow OwnerW, optional bool bUnique, optional name ObjectName)
{
	local UWindowWindow Child;

	// End:0x53
	if(bUnique)
	{
		Child = Root.FindChildWindow(WndClass, true);
		// End:0x53
		if(__NFUN_119__(Child, none))
		{
			Child.ShowWindow();
			Child.BringToFront();
			return Child;
		}
	}
	// End:0x7A
	if(__NFUN_255__(ObjectName, 'None'))
	{
		Child = new (none, string(ObjectName)) WndClass;		
	}
	else
	{
		Child = new (none) WndClass;
	}
	Child.BeginPlay();
	Child.WinTop = Y;
	Child.WinLeft = X;
	Child.WinWidth = W;
	Child.WinHeight = h;
	Child.Root = Root;
	Child.ParentWindow = self;
	Child.OwnerWindow = OwnerW;
	// End:0x144
	if(__NFUN_114__(Child.OwnerWindow, none))
	{
		Child.OwnerWindow = self;
	}
	Child.Cursor = Cursor;
	Child.bAlwaysBehind = false;
	Child.LookAndFeel = LookAndFeel;
	Child.BeforeCreate();
	Child.Created();
	ShowChildWindow(Child);
	Child.AfterCreate();
	return Child;
	return;
}

final function Tile(Canvas C, Texture t)
{
	local int X, Y;

	X = 0;
	Y = 0;
	J0x0E:

	// End:0x85 [Loop If]
	if(__NFUN_176__(float(X), WinWidth))
	{
		J0x1F:

		// End:0x66 [Loop If]
		if(__NFUN_176__(float(Y), WinHeight))
		{
			DrawClippedTexture(C, float(X), float(Y), t);
			__NFUN_161__(Y, t.VSize);
			// [Loop Continue]
			goto J0x1F;
		}
		__NFUN_161__(X, t.USize);
		Y = 0;
		// [Loop Continue]
		goto J0x0E;
	}
	return;
}

final function DrawHorizTiledPieces(Canvas C, float DestX, float DestY, float DestW, float DestH, TexRegion T1, TexRegion T2, TexRegion T3, TexRegion T4, TexRegion T5, float Scale)
{
	local TexRegion Pieces[5], R;
	local int PieceCount, j;
	local float X, L;

	Pieces[0] = T1;
	// End:0x24
	if(__NFUN_119__(T1.t, none))
	{
		PieceCount = 1;
	}
	Pieces[1] = T2;
	// End:0x49
	if(__NFUN_119__(T2.t, none))
	{
		PieceCount = 2;
	}
	Pieces[2] = T3;
	// End:0x6F
	if(__NFUN_119__(T3.t, none))
	{
		PieceCount = 3;
	}
	Pieces[3] = T4;
	// End:0x95
	if(__NFUN_119__(T4.t, none))
	{
		PieceCount = 4;
	}
	Pieces[4] = T5;
	// End:0xBB
	if(__NFUN_119__(T5.t, none))
	{
		PieceCount = 5;
	}
	j = 0;
	X = DestX;
	J0xCD:

	// End:0x1D6 [Loop If]
	if(__NFUN_176__(X, __NFUN_174__(DestX, DestW)))
	{
		L = __NFUN_175__(DestW, __NFUN_175__(X, DestX));
		R = Pieces[j];
		DrawStretchedTextureSegment(C, X, DestY, __NFUN_244__(__NFUN_171__(float(R.W), Scale), L), __NFUN_171__(float(R.h), Scale), float(R.X), float(R.Y), __NFUN_244__(float(R.W), __NFUN_172__(L, Scale)), float(R.h), R.t);
		__NFUN_184__(X, __NFUN_244__(__NFUN_171__(float(R.W), Scale), L));
		j = int(__NFUN_173__(float(__NFUN_146__(j, 1)), float(PieceCount)));
		// [Loop Continue]
		goto J0xCD;
	}
	return;
}

final function DrawVertTiledPieces(Canvas C, float DestX, float DestY, float DestW, float DestH, TexRegion T1, TexRegion T2, TexRegion T3, TexRegion T4, TexRegion T5, float Scale)
{
	local TexRegion Pieces[5], R;
	local int PieceCount, j;
	local float Y, L;

	Pieces[0] = T1;
	// End:0x24
	if(__NFUN_119__(T1.t, none))
	{
		PieceCount = 1;
	}
	Pieces[1] = T2;
	// End:0x49
	if(__NFUN_119__(T2.t, none))
	{
		PieceCount = 2;
	}
	Pieces[2] = T3;
	// End:0x6F
	if(__NFUN_119__(T3.t, none))
	{
		PieceCount = 3;
	}
	Pieces[3] = T4;
	// End:0x95
	if(__NFUN_119__(T4.t, none))
	{
		PieceCount = 4;
	}
	Pieces[4] = T5;
	// End:0xBB
	if(__NFUN_119__(T5.t, none))
	{
		PieceCount = 5;
	}
	j = 0;
	Y = DestY;
	J0xCD:

	// End:0x1D6 [Loop If]
	if(__NFUN_176__(Y, __NFUN_174__(DestY, DestH)))
	{
		L = __NFUN_175__(DestH, __NFUN_175__(Y, DestY));
		R = Pieces[j];
		DrawStretchedTextureSegment(C, DestX, Y, __NFUN_171__(float(R.W), Scale), __NFUN_244__(__NFUN_171__(float(R.h), Scale), L), float(R.X), float(R.Y), float(R.W), __NFUN_244__(float(R.h), __NFUN_172__(L, Scale)), R.t);
		__NFUN_184__(Y, __NFUN_244__(__NFUN_171__(float(R.h), Scale), L));
		j = int(__NFUN_173__(float(__NFUN_146__(j, 1)), float(PieceCount)));
		// [Loop Continue]
		goto J0xCD;
	}
	return;
}

final function DrawClippedTexture(Canvas C, float X, float Y, Texture Tex)
{
	DrawStretchedTextureSegment(C, X, Y, float(Tex.USize), float(Tex.VSize), 0.0000000, 0.0000000, float(Tex.USize), float(Tex.VSize), Tex);
	return;
}

final function DrawStretchedTexture(Canvas C, float X, float Y, float W, float h, Texture Tex)
{
	DrawStretchedTextureSegment(C, X, Y, W, h, 0.0000000, 0.0000000, float(Tex.USize), float(Tex.VSize), Tex);
	return;
}

final function DrawStretchedTextureSegment(Canvas C, float X, float Y, float W, float h, float tX, float tY, float tW, float tH, Texture Tex)
{
	local float OrgX, OrgY, ClipX, ClipY;

	OrgX = C.OrgX;
	OrgY = C.OrgY;
	ClipX = C.ClipX;
	ClipY = C.ClipY;
	C.SetOrigin(__NFUN_174__(OrgX, __NFUN_171__(float(ClippingRegion.X), Root.GUIScale)), __NFUN_174__(OrgY, __NFUN_171__(float(ClippingRegion.Y), Root.GUIScale)));
	C.SetClip(__NFUN_171__(float(ClippingRegion.W), Root.GUIScale), __NFUN_171__(float(ClippingRegion.h), Root.GUIScale));
	C.SetPos(__NFUN_171__(__NFUN_175__(X, float(ClippingRegion.X)), Root.GUIScale), __NFUN_171__(__NFUN_175__(Y, float(ClippingRegion.Y)), Root.GUIScale));
	C.__NFUN_468__(Tex, __NFUN_171__(W, Root.GUIScale), __NFUN_171__(h, Root.GUIScale), tX, tY, tW, tH);
	C.SetClip(ClipX, ClipY);
	C.SetOrigin(OrgX, OrgY);
	return;
}

final function ClipText(Canvas C, float X, float Y, coerce string S, optional bool bCheckHotKey)
{
	local float OrgX, OrgY, ClipX, ClipY;

	OrgX = C.OrgX;
	OrgY = C.OrgY;
	ClipX = C.ClipX;
	ClipY = C.ClipY;
	C.SetOrigin(__NFUN_174__(OrgX, __NFUN_171__(float(ClippingRegion.X), Root.GUIScale)), __NFUN_174__(OrgY, __NFUN_171__(float(ClippingRegion.Y), Root.GUIScale)));
	C.SetClip(__NFUN_171__(float(ClippingRegion.W), Root.GUIScale), __NFUN_171__(float(ClippingRegion.h), Root.GUIScale));
	C.SetPos(__NFUN_171__(__NFUN_175__(X, float(ClippingRegion.X)), Root.GUIScale), __NFUN_171__(__NFUN_175__(Y, float(ClippingRegion.Y)), Root.GUIScale));
	C.__NFUN_469__(S, bCheckHotKey);
	C.SetClip(ClipX, ClipY);
	C.SetOrigin(OrgX, OrgY);
	return;
}

final function int WrapClipText(Canvas C, float X, float Y, coerce string S, optional bool bCheckHotKey, optional int Length, optional int PaddingLength, optional bool bNoDraw)
{
	local float W, h;
	local int SpacePos, CRPos, WordPos, TotalPos;
	local string Out, temp, Padding;
	local bool bCR, bSentry;
	local int i, numLines;
	local float pW, pH;

	i = __NFUN_126__(S, "\\n");
	J0x11:

	// End:0x5E [Loop If]
	if(__NFUN_155__(i, -1))
	{
		S = __NFUN_112__(__NFUN_112__(__NFUN_128__(S, i), __NFUN_236__(13)), __NFUN_127__(S, __NFUN_146__(i, 2)));
		i = __NFUN_126__(S, "\\n");
		// [Loop Continue]
		goto J0x11;
	}
	i = 0;
	bSentry = true;
	Out = "";
	numLines = 1;
	J0x7C:

	// End:0x33B [Loop If]
	if(__NFUN_130__(bSentry, __NFUN_176__(Y, WinHeight)))
	{
		// End:0xD4
		if(__NFUN_122__(Out, ""))
		{
			__NFUN_165__(i);
			// End:0xC9
			if(__NFUN_151__(Length, 0))
			{
				Out = __NFUN_128__(S, Length);				
			}
			else
			{
				Out = S;
			}
		}
		SpacePos = __NFUN_126__(Out, " ");
		CRPos = __NFUN_126__(Out, __NFUN_236__(13));
		bCR = false;
		// End:0x144
		if(__NFUN_130__(__NFUN_155__(CRPos, -1), __NFUN_132__(__NFUN_150__(CRPos, SpacePos), __NFUN_154__(SpacePos, -1))))
		{
			WordPos = CRPos;
			bCR = true;			
		}
		else
		{
			WordPos = SpacePos;
		}
		C.SetPos(0.0000000, 0.0000000);
		// End:0x185
		if(__NFUN_154__(WordPos, -1))
		{
			temp = Out;			
		}
		else
		{
			temp = __NFUN_112__(__NFUN_128__(Out, WordPos), " ");
		}
		__NFUN_161__(TotalPos, WordPos);
		TextSize(C, temp, W, h);
		// End:0x262
		if(__NFUN_130__(__NFUN_122__(__NFUN_127__(Out, __NFUN_125__(temp)), ""), __NFUN_151__(PaddingLength, 0)))
		{
			Padding = __NFUN_127__(S, Length, PaddingLength);
			TextSize(C, Padding, pW, pH);
			// End:0x25F
			if(__NFUN_130__(__NFUN_177__(__NFUN_174__(__NFUN_174__(W, X), pW), WinWidth), __NFUN_177__(X, float(0))))
			{
				X = 0.0000000;
				__NFUN_184__(Y, h);
				__NFUN_165__(numLines);
			}			
		}
		else
		{
			// End:0x2A5
			if(__NFUN_130__(__NFUN_177__(__NFUN_174__(W, X), WinWidth), __NFUN_177__(X, float(0))))
			{
				X = 0.0000000;
				__NFUN_184__(Y, h);
				__NFUN_165__(numLines);
			}
		}
		// End:0x2D0
		if(__NFUN_129__(bNoDraw))
		{
			ClipText(C, X, Y, temp, bCheckHotKey);
		}
		__NFUN_184__(X, W);
		// End:0x303
		if(bCR)
		{
			X = 0.0000000;
			__NFUN_184__(Y, h);
			__NFUN_165__(numLines);
		}
		Out = __NFUN_127__(Out, __NFUN_125__(temp));
		// End:0x338
		if(__NFUN_130__(__NFUN_122__(Out, ""), __NFUN_151__(i, 0)))
		{
			bSentry = false;
		}
		// [Loop Continue]
		goto J0x7C;
	}
	return numLines;
	return;
}

final function ClipTextWidth(Canvas C, float X, float Y, coerce string S, float W)
{
	ClipText(C, X, Y, S);
	return;
}

final function DrawClippedActor(Canvas C, float X, float Y, Actor A, bool Wireframe, Rotator RotOffset, Vector LocOffset)
{
	local Vector MeshLoc;
	local float FOV;

	FOV = __NFUN_172__(__NFUN_171__(GetPlayerOwner().FovAngle, 3.1415927), float(180));
	MeshLoc.X = __NFUN_172__(4.0000000, __NFUN_189__(__NFUN_172__(FOV, float(2))));
	MeshLoc.Y = 0.0000000;
	MeshLoc.Z = 0.0000000;
	A.__NFUN_299__(RotOffset);
	A.__NFUN_267__(__NFUN_215__(MeshLoc, LocOffset));
	return;
}

final function DrawUpBevel(Canvas C, float X, float Y, float W, float h, Texture t)
{
	local Region R;

	R = LookAndFeel.BevelUpTL;
	DrawStretchedTextureSegment(C, X, Y, float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.BevelUpT;
	DrawStretchedTextureSegment(C, __NFUN_174__(X, float(LookAndFeel.BevelUpTL.W)), Y, __NFUN_175__(__NFUN_175__(W, float(LookAndFeel.BevelUpTL.W)), float(LookAndFeel.BevelUpTR.W)), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.BevelUpTR;
	DrawStretchedTextureSegment(C, __NFUN_175__(__NFUN_174__(X, W), float(R.W)), Y, float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.BevelUpL;
	DrawStretchedTextureSegment(C, X, __NFUN_174__(Y, float(LookAndFeel.BevelUpTL.h)), float(R.W), __NFUN_175__(__NFUN_175__(h, float(LookAndFeel.BevelUpTL.h)), float(LookAndFeel.BevelUpBL.h)), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.BevelUpR;
	DrawStretchedTextureSegment(C, __NFUN_175__(__NFUN_174__(X, W), float(R.W)), __NFUN_174__(Y, float(LookAndFeel.BevelUpTL.h)), float(R.W), __NFUN_175__(__NFUN_175__(h, float(LookAndFeel.BevelUpTL.h)), float(LookAndFeel.BevelUpBL.h)), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.BevelUpBL;
	DrawStretchedTextureSegment(C, X, __NFUN_175__(__NFUN_174__(Y, h), float(R.h)), float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.BevelUpB;
	DrawStretchedTextureSegment(C, __NFUN_174__(X, float(LookAndFeel.BevelUpBL.W)), __NFUN_175__(__NFUN_174__(Y, h), float(R.h)), __NFUN_175__(__NFUN_175__(W, float(LookAndFeel.BevelUpBL.W)), float(LookAndFeel.BevelUpBR.W)), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.BevelUpBR;
	DrawStretchedTextureSegment(C, __NFUN_175__(__NFUN_174__(X, W), float(R.W)), __NFUN_175__(__NFUN_174__(Y, h), float(R.h)), float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.BevelUpArea;
	DrawStretchedTextureSegment(C, __NFUN_174__(X, float(LookAndFeel.BevelUpTL.W)), __NFUN_174__(Y, float(LookAndFeel.BevelUpTL.h)), __NFUN_175__(__NFUN_175__(W, float(LookAndFeel.BevelUpBL.W)), float(LookAndFeel.BevelUpBR.W)), __NFUN_175__(__NFUN_175__(h, float(LookAndFeel.BevelUpTL.h)), float(LookAndFeel.BevelUpBL.h)), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	return;
}

final function DrawMiscBevel(Canvas C, float X, float Y, float W, float h, Texture t, int BevelType)
{
	local Region R;

	R = LookAndFeel.MiscBevelTL[BevelType];
	DrawStretchedTextureSegment(C, X, Y, float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.MiscBevelT[BevelType];
	DrawStretchedTextureSegment(C, __NFUN_174__(X, float(LookAndFeel.MiscBevelTL[BevelType].W)), Y, __NFUN_175__(__NFUN_175__(W, float(LookAndFeel.MiscBevelTL[BevelType].W)), float(LookAndFeel.MiscBevelTR[BevelType].W)), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.MiscBevelTR[BevelType];
	DrawStretchedTextureSegment(C, __NFUN_175__(__NFUN_174__(X, W), float(R.W)), Y, float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.MiscBevelL[BevelType];
	DrawStretchedTextureSegment(C, X, __NFUN_174__(Y, float(LookAndFeel.MiscBevelTL[BevelType].h)), float(R.W), __NFUN_175__(__NFUN_175__(h, float(LookAndFeel.MiscBevelTL[BevelType].h)), float(LookAndFeel.MiscBevelBL[BevelType].h)), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.MiscBevelR[BevelType];
	DrawStretchedTextureSegment(C, __NFUN_175__(__NFUN_174__(X, W), float(R.W)), __NFUN_174__(Y, float(LookAndFeel.MiscBevelTL[BevelType].h)), float(R.W), __NFUN_175__(__NFUN_175__(h, float(LookAndFeel.MiscBevelTL[BevelType].h)), float(LookAndFeel.MiscBevelBL[BevelType].h)), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.MiscBevelBL[BevelType];
	DrawStretchedTextureSegment(C, X, __NFUN_175__(__NFUN_174__(Y, h), float(R.h)), float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.MiscBevelB[BevelType];
	DrawStretchedTextureSegment(C, __NFUN_174__(X, float(LookAndFeel.MiscBevelBL[BevelType].W)), __NFUN_175__(__NFUN_174__(Y, h), float(R.h)), __NFUN_175__(__NFUN_175__(W, float(LookAndFeel.MiscBevelBL[BevelType].W)), float(LookAndFeel.MiscBevelBR[BevelType].W)), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.MiscBevelBR[BevelType];
	DrawStretchedTextureSegment(C, __NFUN_175__(__NFUN_174__(X, W), float(R.W)), __NFUN_175__(__NFUN_174__(Y, h), float(R.h)), float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	R = LookAndFeel.MiscBevelArea[BevelType];
	DrawStretchedTextureSegment(C, __NFUN_174__(X, float(LookAndFeel.MiscBevelTL[BevelType].W)), __NFUN_174__(Y, float(LookAndFeel.MiscBevelTL[BevelType].h)), __NFUN_175__(__NFUN_175__(W, float(LookAndFeel.MiscBevelBL[BevelType].W)), float(LookAndFeel.MiscBevelBR[BevelType].W)), __NFUN_175__(__NFUN_175__(h, float(LookAndFeel.MiscBevelTL[BevelType].h)), float(LookAndFeel.MiscBevelBL[BevelType].h)), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	return;
}

final function string RemoveAmpersand(string S)
{
	local string Result, Underline;

	ParseAmpersand(S, Result, Underline, false);
	return Result;
	return;
}

final function byte ParseAmpersand(string S, out string Result, out string Underline, bool bCalcUnderline)
{
	local string temp;
	local int pos, NewPos, i;
	local byte HotKey;

	HotKey = 0;
	pos = 0;
	Result = "";
	Underline = "";
	J0x1F:

	// End:0x154 [Loop If]
	if(true)
	{
		temp = __NFUN_127__(S, pos);
		NewPos = __NFUN_126__(temp, "&");
		// End:0x57
		if(__NFUN_154__(NewPos, -1))
		{
			// [Explicit Break]
			goto J0x154;
		}
		__NFUN_161__(pos, NewPos);
		// End:0xBC
		if(__NFUN_122__(__NFUN_127__(temp, __NFUN_146__(NewPos, 1), 1), "&"))
		{
			Result = __NFUN_112__(__NFUN_112__(Result, __NFUN_128__(temp, NewPos)), "&");
			// End:0xB2
			if(bCalcUnderline)
			{
				Underline = __NFUN_112__(Underline, " ");
			}
			__NFUN_165__(pos);
			// [Explicit Continue]
			goto J0x14A;
		}
		// End:0xE5
		if(__NFUN_154__(int(HotKey), 0))
		{
			HotKey = byte(__NFUN_237__(__NFUN_235__(__NFUN_127__(temp, __NFUN_146__(NewPos, 1), 1))));
		}
		Result = __NFUN_112__(Result, __NFUN_128__(temp, NewPos));
		// End:0x14A
		if(bCalcUnderline)
		{
			i = 0;
			J0x10E:

			// End:0x13A [Loop If]
			if(__NFUN_150__(i, __NFUN_147__(NewPos, 1)))
			{
				Underline = __NFUN_112__(Underline, " ");
				__NFUN_165__(i);
				// [Loop Continue]
				goto J0x10E;
			}
			Underline = __NFUN_112__(Underline, "_");
		}
		J0x14A:

		__NFUN_165__(pos);
		// [Loop Continue]
		goto J0x1F;
	}
	J0x154:

	Result = __NFUN_112__(Result, temp);
	return HotKey;
	return;
}

final function bool MouseIsOver()
{
	return __NFUN_114__(Root.MouseWindow, self);
	return;
}

function ToolTip(string strTip)
{
	// End:0x23
	if(__NFUN_119__(ParentWindow, Root))
	{
		ParentWindow.ToolTip(strTip);
	}
	return;
}

final function SetMouseWindow()
{
	Root.MouseWindow = self;
	return;
}

function Texture GetLookAndFeelTexture()
{
	return ParentWindow.GetLookAndFeelTexture();
	return;
}

function bool IsActive()
{
	return ParentWindow.IsActive();
	return;
}

function SetAcceptsHotKeys(bool bNewAccpetsHotKeys)
{
	// End:0x31
	if(__NFUN_130__(__NFUN_130__(bNewAccpetsHotKeys, __NFUN_129__(bAcceptsHotKeys)), bWindowVisible))
	{
		Root.AddHotkeyWindow(self);
	}
	// End:0x62
	if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bNewAccpetsHotKeys), bAcceptsHotKeys), bWindowVisible))
	{
		Root.RemoveHotkeyWindow(self);
	}
	bAcceptsHotKeys = bNewAccpetsHotKeys;
	return;
}

final function UWindowWindow GetParent(Class<UWindowWindow> ParentClass, optional bool bExactClass)
{
	local UWindowWindow P;

	P = ParentWindow;
	J0x0B:

	// End:0x7A [Loop If]
	if(__NFUN_119__(P, Root))
	{
		// End:0x44
		if(bExactClass)
		{
			// End:0x41
			if(__NFUN_114__(P.Class, ParentClass))
			{
				return P;
			}			
		}
		else
		{
			// End:0x63
			if(__NFUN_258__(P.Class, ParentClass))
			{
				return P;
			}
		}
		P = P.ParentWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return none;
	return;
}

final function UWindowWindow FindChildWindow(Class<UWindowWindow> ChildClass, optional bool bExactClass)
{
	local UWindowWindow Child, Found;

	Child = LastChildWindow;
	J0x0B:

	// End:0xA1 [Loop If]
	if(__NFUN_119__(Child, none))
	{
		// End:0x40
		if(bExactClass)
		{
			// End:0x3D
			if(__NFUN_114__(Child.Class, ChildClass))
			{
				return Child;
			}			
		}
		else
		{
			// End:0x5F
			if(__NFUN_258__(Child.Class, ChildClass))
			{
				return Child;
			}
		}
		Found = Child.super(UWindowWindow).FindChildWindow(ChildClass);
		// End:0x8A
		if(__NFUN_119__(Found, none))
		{
			return Found;
		}
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return none;
	return;
}

function GetDesiredDimensions(out float W, out float h)
{
	local float MaxW, MaxH, tW, tH;
	local UWindowWindow Child;

	MaxW = 0.0000000;
	MaxH = 0.0000000;
	Child = LastChildWindow;
	J0x21:

	// End:0x90 [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.GetDesiredDimensions(tW, tH);
		// End:0x5F
		if(__NFUN_177__(tW, MaxW))
		{
			MaxW = tW;
		}
		// End:0x79
		if(__NFUN_177__(tH, MaxH))
		{
			MaxH = tH;
		}
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x21;
	}
	W = MaxW;
	h = MaxH;
	return;
}

final function TextSize(Canvas C, string Text, out float W, out float h)
{
	C.SetPos(0.0000000, 0.0000000);
	C.__NFUN_470__(Text, W, h);
	W = __NFUN_172__(W, Root.GUIScale);
	h = __NFUN_172__(h, Root.GUIScale);
	return;
}

function ResolutionChanged(float W, float h)
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x46 [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.ResolutionChanged(W, h);
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return;
}

function ShowModal(UWindowWindow W)
{
	ModalWindow = W;
	W.ShowWindow();
	W.BringToFront();
	return;
}

function bool WaitModal()
{
	// End:0x21
	if(__NFUN_130__(__NFUN_119__(ModalWindow, none), ModalWindow.bWindowVisible))
	{
		return true;
	}
	ModalWindow = none;
	return false;
	return;
}

function WindowHidden()
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x3C [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.WindowHidden();
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return;
}

function WindowShown()
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x3C [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.WindowShown();
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return;
}

function bool CheckMousePassThrough(float X, float Y)
{
	return false;
	return;
}

final function bool WindowIsVisible()
{
	// End:0x0D
	if(__NFUN_114__(self, Root))
	{
		return true;
	}
	// End:0x1A
	if(__NFUN_129__(bWindowVisible))
	{
		return false;
	}
	return ParentWindow.super(UWindowWindow).WindowIsVisible();
	return;
}

function SetParent(UWindowWindow NewParent)
{
	HideWindow();
	ParentWindow = NewParent;
	ShowWindow();
	return;
}

function UWindowMessageBox MessageBox(string Title, string Message, UWindowBase.MessageBoxButtons Buttons, UWindowBase.MessageBoxResult ESCResult, optional UWindowBase.MessageBoxResult EnterResult, optional int TimeOut)
{
	local UWindowMessageBox W;
	local UWindowFramedWindow f;

	W = UWindowMessageBox(Root.CreateWindow(Class'UWindow.UWindowMessageBox', 100.0000000, 100.0000000, 100.0000000, 100.0000000, self));
	W.SetupMessageBox(Title, Message, Buttons, ESCResult, EnterResult, TimeOut);
	f = UWindowFramedWindow(GetParent(Class'UWindow.UWindowFramedWindow'));
	// End:0x99
	if(__NFUN_119__(f, none))
	{
		f.ShowModal(W);		
	}
	else
	{
		Root.ShowModal(W);
	}
	return W;
	return;
}

function MessageBoxDone(UWindowMessageBox W, UWindowBase.MessageBoxResult Result)
{
	return;
}

function NotifyQuitUnreal()
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x3C [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.NotifyQuitUnreal();
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return;
}

function NotifyBeforeLevelChange()
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x3C [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.NotifyBeforeLevelChange();
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return;
}

function SetCursor(MouseCursor C)
{
	local UWindowWindow Child;

	Cursor = C;
	Child = LastChildWindow;
	J0x16:

	// End:0x4C [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.SetCursor(C);
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x16;
	}
	return;
}

function NotifyAfterLevelChange()
{
	local UWindowWindow Child;

	Child = LastChildWindow;
	J0x0B:

	// End:0x3C [Loop If]
	if(__NFUN_119__(Child, none))
	{
		Child.NotifyAfterLevelChange();
		Child = Child.PrevSiblingWindow;
		// [Loop Continue]
		goto J0x0B;
	}
	return;
}

final function ReplaceText(out string Text, string Replace, string With)
{
	local int i;
	local string Input;

	Input = Text;
	Text = "";
	i = __NFUN_126__(Input, Replace);
	J0x25:

	// End:0x84 [Loop If]
	if(__NFUN_155__(i, -1))
	{
		Text = __NFUN_112__(__NFUN_112__(Text, __NFUN_128__(Input, i)), With);
		Input = __NFUN_127__(Input, __NFUN_146__(i, __NFUN_125__(Replace)));
		i = __NFUN_126__(Input, Replace);
		// [Loop Continue]
		goto J0x25;
	}
	Text = __NFUN_112__(Text, Input);
	return;
}

function StripCRLF(out string Text)
{
	ReplaceText(Text, __NFUN_112__(__NFUN_236__(13), __NFUN_236__(10)), "");
	ReplaceText(Text, __NFUN_236__(13), "");
	ReplaceText(Text, __NFUN_236__(10), "");
	return;
}
