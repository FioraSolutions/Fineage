class UWindowConsoleClientWindow extends UWindowDialogClientWindow;

var UWindowConsoleTextAreaControl TextArea;
var UWindowEditControl EditControl;

function Created()
{
	TextArea = UWindowConsoleTextAreaControl(CreateWindow(Class'UWindow.UWindowConsoleTextAreaControl', 0.0000000, 0.0000000, WinWidth, WinHeight));
	EditControl = UWindowEditControl(CreateControl(Class'UWindow.UWindowEditControl', 0.0000000, __NFUN_175__(WinHeight, float(16)), WinWidth, 16.0000000));
	EditControl.SetFont(0);
	EditControl.SetValue("Test");
	EditControl.SetNumericOnly(false);
	EditControl.SetMaxLength(400);
	EditControl.SetHistory(true);
	Cursor = Root.NormalCursor;
	return;
}

function Notify(UWindowDialogControl C, byte E)
{
	local string S;

	super.Notify(C, E);
	switch(E)
	{
		// End:0xC2
		case 7:
			switch(C)
			{
				// End:0xBC
				case EditControl:
					// End:0xB9
					if(__NFUN_123__(EditControl.GetValue(), ""))
					{
						S = EditControl.GetValue();
						Message(__NFUN_112__("> ", S), 6.0000000);
						EditControl.Clear();
						// End:0xB9
						if(__NFUN_129__(Root.ConsoleCommand(S)))
						{
							Message(Localize("Errors", "Exec", "Core"), 6.0000000);
						}
					}
					// End:0xBF
					break;
				// End:0xFFFF
				default:
					break;
			}
			// End:0x139
			break;
		// End:0xFC
		case 14:
			switch(C)
			{
				// End:0xF6
				case EditControl:
					TextArea.VertSB.Scroll(-1.0000000);
					// End:0xF9
					break;
				// End:0xFFFF
				default:
					break;
			}
			// End:0x139
			break;
		// End:0x136
		case 15:
			switch(C)
			{
				// End:0x130
				case EditControl:
					TextArea.VertSB.Scroll(1.0000000);
					// End:0x133
					break;
				// End:0xFFFF
				default:
					break;
			}
			// End:0x139
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	super(UWindowWindow).BeforePaint(C, X, Y);
	EditControl.SetSize(WinWidth, 17.0000000);
	EditControl.WinLeft = 0.0000000;
	EditControl.WinTop = __NFUN_175__(WinHeight, EditControl.WinHeight);
	EditControl.EditBoxWidth = WinWidth;
	TextArea.SetSize(WinWidth, __NFUN_175__(WinHeight, EditControl.WinHeight));
	return;
}

function Paint(Canvas C, float X, float Y)
{
	DrawStretchedTexture(C, 0.0000000, 0.0000000, WinWidth, WinHeight, Texture'UWindow.BlackTexture');
	return;
}

function Message(coerce string Msg, float MsgLife)
{
	super(Interaction).Message(Msg, MsgLife);
	// End:0x30
	if(__NFUN_123__(Msg, ""))
	{
		TextArea.AddText(Msg);
	}
	return;
}
