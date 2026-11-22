class UWindowFramedWindow extends UWindowWindow;

var Class<UWindowWindow> ClientClass;
var UWindowWindow ClientArea;
var localized string WindowTitle;
var string StatusBarText;
var float MoveX;
var float MoveY;
var float MinWinWidth;
var float MinWinHeight;
var bool bTLSizing;
var bool bTSizing;
var bool bTRSizing;
var bool bLSizing;
var bool bRSizing;
var bool bBLSizing;
var bool bBSizing;
var bool bBRSizing;
var bool bMoving;
var bool bSizable;
var bool bStatusBar;
var UWindowFrameCloseBox CloseBox;

function Created()
{
	super.Created();
	MinWinWidth = 50.0000000;
	MinWinHeight = 50.0000000;
	ClientArea = CreateWindow(ClientClass, 4.0000000, 16.0000000, __NFUN_175__(WinWidth, float(8)), __NFUN_175__(WinHeight, float(20)), OwnerWindow);
	CloseBox = UWindowFrameCloseBox(CreateWindow(Class'UWindow.UWindowFrameCloseBox', __NFUN_175__(WinWidth, float(20)), __NFUN_175__(WinHeight, float(20)), 11.0000000, 10.0000000));
	return;
}

function Texture GetLookAndFeelTexture()
{
	return LookAndFeel.GetTexture(self);
	return;
}

function bool IsActive()
{
	return __NFUN_114__(ParentWindow.ActiveWindow, self);
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	super.BeforePaint(C, X, Y);
	Resized();
	LookAndFeel.FW_SetupFrameButtons(self, C);
	return;
}

function Paint(Canvas C, float X, float Y)
{
	LookAndFeel.FW_DrawWindowFrame(self, C);
	return;
}

function LMouseDown(float X, float Y)
{
	local UWindowBase.FrameHitTest h;

	h = LookAndFeel.FW_HitTest(self, X, Y);
	super.LMouseDown(X, Y);
	// End:0x6F
	if(__NFUN_154__(int(h), int(8)))
	{
		MoveX = X;
		MoveY = Y;
		bMoving = true;
		Root.CaptureMouse();
		return;
	}
	// End:0x172
	if(bSizable)
	{
		switch(h)
		{
			// End:0x9D
			case 0:
				bTLSizing = true;
				Root.CaptureMouse();
				return;
			// End:0xBB
			case 2:
				bTRSizing = true;
				Root.CaptureMouse();
				return;
			// End:0xD9
			case 5:
				bBLSizing = true;
				Root.CaptureMouse();
				return;
			// End:0xF7
			case 7:
				bBRSizing = true;
				Root.CaptureMouse();
				return;
			// End:0x115
			case 1:
				bTSizing = true;
				Root.CaptureMouse();
				return;
			// End:0x133
			case 6:
				bBSizing = true;
				Root.CaptureMouse();
				return;
			// End:0x151
			case 3:
				bLSizing = true;
				Root.CaptureMouse();
				return;
			// End:0x16F
			case 4:
				bRSizing = true;
				Root.CaptureMouse();
				return;
			// End:0xFFFF
			default:
				break;
			}
	}
	else
	{
		return;
	}
}

function Resized()
{
	local Region R;

	// End:0x2E
	if(__NFUN_114__(ClientArea, none))
	{
		__NFUN_231__(__NFUN_112__("Client Area is None for ", string(self)));
		return;
	}
	R = LookAndFeel.FW_GetClientArea(self);
	ClientArea.WinLeft = float(R.X);
	ClientArea.WinTop = float(R.Y);
	// End:0xE1
	if(__NFUN_132__(__NFUN_181__(float(R.W), ClientArea.WinWidth), __NFUN_181__(float(R.h), ClientArea.WinHeight)))
	{
		ClientArea.SetSize(float(R.W), float(R.h));
	}
	return;
}

function MouseMove(float X, float Y)
{
	local float OldW, OldH;
	local UWindowBase.FrameHitTest h;

	h = LookAndFeel.FW_HitTest(self, X, Y);
	// End:0x71
	if(__NFUN_130__(bMoving, bMouseDown))
	{
		WinLeft = float(int(__NFUN_175__(__NFUN_174__(WinLeft, X), MoveX)));
		WinTop = float(int(__NFUN_175__(__NFUN_174__(WinTop, Y), MoveY)));		
	}
	else
	{
		bMoving = false;
	}
	Cursor = Root.NormalCursor;
	// End:0x131
	if(__NFUN_130__(bSizable, __NFUN_129__(bMoving)))
	{
		switch(h)
		{
			// End:0xAF
			case 0:
			// End:0xCB
			case 7:
				Cursor = Root.DiagCursor1;
				// End:0x131
				break;
			// End:0xD0
			case 2:
			// End:0xEC
			case 5:
				Cursor = Root.DiagCursor2;
				// End:0x131
				break;
			// End:0xF1
			case 3:
			// End:0x10D
			case 4:
				Cursor = Root.WECursor;
				// End:0x131
				break;
			// End:0x112
			case 1:
			// End:0x12E
			case 6:
				Cursor = Root.NSCursor;
				// End:0x131
				break;
			// End:0xFFFF
			default:
				break;
		}
	}
	// End:0x1E4
	if(__NFUN_130__(bTLSizing, bMouseDown))
	{
		Cursor = Root.DiagCursor1;
		OldW = WinWidth;
		OldH = WinHeight;
		SetSize(float(__NFUN_250__(int(MinWinWidth), int(__NFUN_175__(WinWidth, X)))), float(__NFUN_250__(int(MinWinHeight), int(__NFUN_175__(WinHeight, Y)))));
		WinLeft = float(int(__NFUN_175__(__NFUN_174__(WinLeft, OldW), WinWidth)));
		WinTop = float(int(__NFUN_175__(__NFUN_174__(WinTop, OldH), WinHeight)));		
	}
	else
	{
		bTLSizing = false;
	}
	// End:0x263
	if(__NFUN_130__(bTSizing, bMouseDown))
	{
		Cursor = Root.NSCursor;
		OldH = WinHeight;
		SetSize(WinWidth, float(__NFUN_250__(int(MinWinHeight), int(__NFUN_175__(WinHeight, Y)))));
		WinTop = float(int(__NFUN_175__(__NFUN_174__(WinTop, OldH), WinHeight)));		
	}
	else
	{
		bTSizing = false;
	}
	// End:0x2EF
	if(__NFUN_130__(bTRSizing, bMouseDown))
	{
		Cursor = Root.DiagCursor2;
		OldH = WinHeight;
		SetSize(float(__NFUN_250__(int(MinWinWidth), int(X))), float(__NFUN_250__(int(MinWinHeight), int(__NFUN_175__(WinHeight, Y)))));
		WinTop = float(int(__NFUN_175__(__NFUN_174__(WinTop, OldH), WinHeight)));		
	}
	else
	{
		bTRSizing = false;
	}
	// End:0x36E
	if(__NFUN_130__(bLSizing, bMouseDown))
	{
		Cursor = Root.WECursor;
		OldW = WinWidth;
		SetSize(float(__NFUN_250__(int(MinWinWidth), int(__NFUN_175__(WinWidth, X)))), WinHeight);
		WinLeft = float(int(__NFUN_175__(__NFUN_174__(WinLeft, OldW), WinWidth)));		
	}
	else
	{
		bLSizing = false;
	}
	// End:0x3BE
	if(__NFUN_130__(bRSizing, bMouseDown))
	{
		Cursor = Root.WECursor;
		SetSize(float(__NFUN_250__(int(MinWinWidth), int(X))), WinHeight);		
	}
	else
	{
		bRSizing = false;
	}
	// End:0x44A
	if(__NFUN_130__(bBLSizing, bMouseDown))
	{
		Cursor = Root.DiagCursor2;
		OldW = WinWidth;
		SetSize(float(__NFUN_250__(int(MinWinWidth), int(__NFUN_175__(WinWidth, X)))), float(__NFUN_250__(int(MinWinHeight), int(Y))));
		WinLeft = float(int(__NFUN_175__(__NFUN_174__(WinLeft, OldW), WinWidth)));		
	}
	else
	{
		bBLSizing = false;
	}
	// End:0x49A
	if(__NFUN_130__(bBSizing, bMouseDown))
	{
		Cursor = Root.NSCursor;
		SetSize(WinWidth, float(__NFUN_250__(int(MinWinHeight), int(Y))));		
	}
	else
	{
		bBSizing = false;
	}
	// End:0x4F7
	if(__NFUN_130__(bBRSizing, bMouseDown))
	{
		Cursor = Root.DiagCursor1;
		SetSize(float(__NFUN_250__(int(MinWinWidth), int(X))), float(__NFUN_250__(int(MinWinHeight), int(Y))));		
	}
	else
	{
		bBRSizing = false;
	}
	return;
}

function ToolTip(string strTip)
{
	StatusBarText = strTip;
	return;
}

function WindowEvent(UWindowWindow.WinMessage Msg, Canvas C, float X, float Y, int Key)
{
	// End:0x3C
	if(__NFUN_132__(__NFUN_154__(int(Msg), int(9)), __NFUN_129__(WaitModal())))
	{
		super.WindowEvent(Msg, C, X, Y, Key);
	}
	return;
}

function WindowHidden()
{
	super.WindowHidden();
	LookAndFeel.PlayMenuSound(self, 4);
	return;
}

defaultproperties
{
	ClientClass=Class'UWindow.UWindowClientWindow'
}