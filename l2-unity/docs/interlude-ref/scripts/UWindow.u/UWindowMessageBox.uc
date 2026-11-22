class UWindowMessageBox extends UWindowFramedWindow;

var UWindowBase.MessageBoxResult Result;
var float TimeOutTime;
var int TimeOut;
var bool bSetupSize;
var int FrameCount;

function SetupMessageBox(string Title, string Message, UWindowBase.MessageBoxButtons Buttons, UWindowBase.MessageBoxResult InESCResult, optional UWindowBase.MessageBoxResult InEnterResult, optional int InTimeOut)
{
	WindowTitle = Title;
	UWindowMessageBoxCW(ClientArea).SetupMessageBoxClient(Message, Buttons, InEnterResult);
	Result = InESCResult;
	TimeOutTime = 0.0000000;
	TimeOut = InTimeOut;
	FrameCount = 0;
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local Region R;

	// End:0xBC
	if(__NFUN_129__(bSetupSize))
	{
		SetSize(200.0000000, WinHeight);
		R = LookAndFeel.FW_GetClientArea(self);
		SetSize(200.0000000, __NFUN_174__(__NFUN_175__(WinHeight, float(R.h)), UWindowMessageBoxCW(ClientArea).GetHeight(C)));
		WinLeft = float(int(__NFUN_172__(__NFUN_175__(Root.WinWidth, WinWidth), float(2))));
		WinTop = float(int(__NFUN_172__(__NFUN_175__(Root.WinHeight, WinHeight), float(2))));
		bSetupSize = true;
	}
	super.BeforePaint(C, X, Y);
	return;
}

function AfterPaint(Canvas C, float X, float Y)
{
	super(UWindowWindow).AfterPaint(C, X, Y);
	// End:0x58
	if(__NFUN_155__(TimeOut, 0))
	{
		__NFUN_165__(FrameCount);
		// End:0x58
		if(__NFUN_153__(FrameCount, 5))
		{
			TimeOutTime = __NFUN_174__(GetEntryLevel().TimeSeconds, float(TimeOut));
			TimeOut = 0;
		}
	}
	// End:0x91
	if(__NFUN_130__(__NFUN_181__(TimeOutTime, float(0)), __NFUN_177__(GetEntryLevel().TimeSeconds, TimeOutTime)))
	{
		TimeOutTime = 0.0000000;
		Close();
	}
	return;
}

function Close(optional bool bByParent)
{
	super(UWindowWindow).Close(bByParent);
	OwnerWindow.MessageBoxDone(self, Result);
	return;
}

defaultproperties
{
	ClientClass=Class'UWindow.UWindowMessageBoxCW'
}