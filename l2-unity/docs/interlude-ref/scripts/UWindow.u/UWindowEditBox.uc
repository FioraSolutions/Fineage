class UWindowEditBox extends UWindowDialogControl;

var string Value;
var string Value2;
var int CaretOffset;
var int MaxLength;
var float LastDrawTime;
var bool bShowCaret;
var float offset;
var UWindowDialogControl NotifyOwner;
var bool bNumericOnly;
var bool bNumericFloat;
var bool bCanEdit;
var bool bAllSelected;
var bool bSelectOnFocus;
var bool bDelayedNotify;
var bool bChangePending;
var bool bControlDown;
var bool bShiftDown;
var bool bHistory;
var bool bKeyDown;
var UWindowEditBoxHistory HistoryList;
var UWindowEditBoxHistory CurrentHistory;

function Created()
{
	super.Created();
	bCanEdit = true;
	bControlDown = false;
	bShiftDown = false;
	MaxLength = 255;
	CaretOffset = 0;
	offset = 0.0000000;
	LastDrawTime = 0.0000000;
	return;
}

function SetHistory(bool bInHistory)
{
	bHistory = bInHistory;
	// End:0x4B
	if(__NFUN_130__(bHistory, __NFUN_114__(HistoryList, none)))
	{
		HistoryList = new (none) Class'UWindow.UWindowEditBoxHistory';
		HistoryList.SetupSentinel();
		CurrentHistory = none;		
	}
	else
	{
		// End:0x71
		if(__NFUN_130__(__NFUN_129__(bHistory), __NFUN_119__(HistoryList, none)))
		{
			HistoryList = none;
			CurrentHistory = none;
		}
	}
	return;
}

function SetEditable(bool bEditable)
{
	bCanEdit = bEditable;
	return;
}

function SetValue(string NewValue, optional string NewValue2)
{
	Value = NewValue;
	Value2 = NewValue2;
	// End:0x34
	if(__NFUN_151__(CaretOffset, __NFUN_125__(Value)))
	{
		CaretOffset = __NFUN_125__(Value);
	}
	Notify(1);
	return;
}

function Clear()
{
	CaretOffset = 0;
	Value = "";
	Value2 = "";
	bAllSelected = false;
	// End:0x33
	if(bDelayedNotify)
	{
		bChangePending = true;		
	}
	else
	{
		Notify(1);
	}
	return;
}

function SelectAll()
{
	// End:0x2C
	if(__NFUN_130__(bCanEdit, __NFUN_123__(Value, "")))
	{
		CaretOffset = __NFUN_125__(Value);
		bAllSelected = true;
	}
	return;
}

function string GetValue()
{
	return Value;
	return;
}

function string GetValue2()
{
	return Value2;
	return;
}

function Notify(byte E)
{
	// End:0x22
	if(__NFUN_119__(NotifyOwner, none))
	{
		NotifyOwner.Notify(E);		
	}
	else
	{
		super.Notify(E);
	}
	return;
}

function InsertText(string Text)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x39 [Loop If]
	if(__NFUN_150__(i, __NFUN_125__(Text)))
	{
		Insert(byte(__NFUN_237__(__NFUN_127__(Text, i, 1))));
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function bool Insert(byte C)
{
	local string NewValue;

	NewValue = __NFUN_112__(__NFUN_112__(__NFUN_128__(Value, CaretOffset), __NFUN_236__(int(C))), __NFUN_127__(Value, CaretOffset));
	// End:0x3E
	if(__NFUN_151__(__NFUN_125__(NewValue), MaxLength))
	{
		return false;
	}
	__NFUN_165__(CaretOffset);
	Value = NewValue;
	// End:0x64
	if(bDelayedNotify)
	{
		bChangePending = true;		
	}
	else
	{
		Notify(1);
	}
	return true;
	return;
}

function bool Backspace()
{
	local string NewValue;

	// End:0x0D
	if(__NFUN_154__(CaretOffset, 0))
	{
		return false;
	}
	NewValue = __NFUN_112__(__NFUN_128__(Value, __NFUN_147__(CaretOffset, 1)), __NFUN_127__(Value, CaretOffset));
	__NFUN_166__(CaretOffset);
	Value = NewValue;
	// End:0x56
	if(bDelayedNotify)
	{
		bChangePending = true;		
	}
	else
	{
		Notify(1);
	}
	return true;
	return;
}

function bool Delete()
{
	local string NewValue;

	// End:0x13
	if(__NFUN_154__(CaretOffset, __NFUN_125__(Value)))
	{
		return false;
	}
	NewValue = __NFUN_112__(__NFUN_128__(Value, CaretOffset), __NFUN_127__(Value, __NFUN_146__(CaretOffset, 1)));
	Value = NewValue;
	Notify(1);
	return true;
	return;
}

function bool WordLeft()
{
	J0x00:
	// End:0x2F [Loop If]
	if(__NFUN_130__(__NFUN_151__(CaretOffset, 0), __NFUN_122__(__NFUN_127__(Value, __NFUN_147__(CaretOffset, 1), 1), " ")))
	{
		__NFUN_166__(CaretOffset);
		// [Loop Continue]
		goto J0x00;
	}
	J0x2F:

	// End:0x5E [Loop If]
	if(__NFUN_130__(__NFUN_151__(CaretOffset, 0), __NFUN_123__(__NFUN_127__(Value, __NFUN_147__(CaretOffset, 1), 1), " ")))
	{
		__NFUN_166__(CaretOffset);
		// [Loop Continue]
		goto J0x2F;
	}
	LastDrawTime = GetLevel().TimeSeconds;
	bShowCaret = true;
	return true;
	return;
}

function bool MoveLeft()
{
	// End:0x0D
	if(__NFUN_154__(CaretOffset, 0))
	{
		return false;
	}
	__NFUN_166__(CaretOffset);
	LastDrawTime = GetLevel().TimeSeconds;
	bShowCaret = true;
	return true;
	return;
}

function bool MoveRight()
{
	// End:0x13
	if(__NFUN_154__(CaretOffset, __NFUN_125__(Value)))
	{
		return false;
	}
	__NFUN_165__(CaretOffset);
	LastDrawTime = GetLevel().TimeSeconds;
	bShowCaret = true;
	return true;
	return;
}

function bool WordRight()
{
	J0x00:
	// End:0x32 [Loop If]
	if(__NFUN_130__(__NFUN_150__(CaretOffset, __NFUN_125__(Value)), __NFUN_123__(__NFUN_127__(Value, CaretOffset, 1), " ")))
	{
		__NFUN_165__(CaretOffset);
		// [Loop Continue]
		goto J0x00;
	}
	J0x32:

	// End:0x64 [Loop If]
	if(__NFUN_130__(__NFUN_150__(CaretOffset, __NFUN_125__(Value)), __NFUN_122__(__NFUN_127__(Value, CaretOffset, 1), " ")))
	{
		__NFUN_165__(CaretOffset);
		// [Loop Continue]
		goto J0x32;
	}
	LastDrawTime = GetLevel().TimeSeconds;
	bShowCaret = true;
	return true;
	return;
}

function bool MoveHome()
{
	CaretOffset = 0;
	LastDrawTime = GetLevel().TimeSeconds;
	bShowCaret = true;
	return true;
	return;
}

function bool MoveEnd()
{
	CaretOffset = __NFUN_125__(Value);
	LastDrawTime = GetLevel().TimeSeconds;
	bShowCaret = true;
	return true;
	return;
}

function EditCopy()
{
	// End:0x2B
	if(__NFUN_132__(bAllSelected, __NFUN_129__(bCanEdit)))
	{
		GetPlayerOwner().CopyToClipboard(Value);
	}
	return;
}

function EditPaste()
{
	// End:0x2E
	if(bCanEdit)
	{
		// End:0x18
		if(bAllSelected)
		{
			Clear();
		}
		InsertText(GetPlayerOwner().PasteFromClipboard());
	}
	return;
}

function EditCut()
{
	// End:0x38
	if(bCanEdit)
	{
		// End:0x35
		if(bAllSelected)
		{
			GetPlayerOwner().CopyToClipboard(Value);
			bAllSelected = false;
			Clear();
		}		
	}
	else
	{
		EditCopy();
	}
	return;
}

function KeyPressed(int Key, float MouseX, float MouseY)
{
	// End:0x90
	if(__NFUN_130__(bCanEdit, bKeyDown))
	{
		// End:0x90
		if(__NFUN_129__(bControlDown))
		{
			// End:0x2E
			if(bAllSelected)
			{
				Clear();
			}
			bAllSelected = false;
			// End:0x69
			if(bNumericOnly)
			{
				// End:0x66
				if(__NFUN_130__(__NFUN_153__(Key, 48), __NFUN_152__(Key, 57)))
				{
					Insert(byte(Key));
				}				
			}
			else
			{
				// End:0x90
				if(__NFUN_130__(__NFUN_153__(Key, 32), __NFUN_150__(Key, 128)))
				{
					Insert(byte(Key));
				}
			}
		}
	}
	return;
}

function KeyUp(int Key, float X, float Y)
{
	local PlayerController P;

	bKeyDown = false;
	P = GetPlayerOwner();
	switch(Key)
	{
		// End:0x48
		case int(P.Player.Console.17):
			bControlDown = false;
			// End:0x78
			break;
		// End:0x75
		case int(P.Player.Console.16):
			bShiftDown = false;
			// End:0x78
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function KeyDown(int Key, float X, float Y)
{
	bKeyDown = true;
	switch(Key)
	{
		// End:0x1F
		case 17:
			bControlDown = true;
			// End:0x3AF
			break;
		// End:0x2F
		case 16:
			bShiftDown = true;
			// End:0x3AF
			break;
		// End:0x37
		case 27:
			// End:0x3AF
			break;
		// End:0xA3
		case 13:
			// End:0xA0
			if(bCanEdit)
			{
				// End:0x98
				if(bHistory)
				{
					// End:0x8D
					if(__NFUN_123__(Value, ""))
					{
						CurrentHistory = UWindowEditBoxHistory(HistoryList.Insert(Class'UWindow.UWindowEditBoxHistory'));
						CurrentHistory.HistoryText = Value;
					}
					CurrentHistory = HistoryList;
				}
				Notify(7);
			}
			// End:0x3AF
			break;
		// End:0xBC
		case 236:
			// End:0xB9
			if(bCanEdit)
			{
				Notify(14);
			}
			// End:0x3AF
			break;
		// End:0xD5
		case 237:
			// End:0xD2
			if(bCanEdit)
			{
				Notify(15);
			}
			// End:0x3AF
			break;
		// End:0x106
		case 39:
			// End:0xFB
			if(bCanEdit)
			{
				// End:0xF5
				if(bControlDown)
				{
					WordRight();					
				}
				else
				{
					MoveRight();
				}
			}
			bAllSelected = false;
			// End:0x3AF
			break;
		// End:0x137
		case 37:
			// End:0x12C
			if(bCanEdit)
			{
				// End:0x126
				if(bControlDown)
				{
					WordLeft();					
				}
				else
				{
					MoveLeft();
				}
			}
			bAllSelected = false;
			// End:0x3AF
			break;
		// End:0x1AF
		case 38:
			// End:0x1AC
			if(__NFUN_130__(bCanEdit, bHistory))
			{
				bAllSelected = false;
				// End:0x1AC
				if(__NFUN_130__(__NFUN_119__(CurrentHistory, none), __NFUN_119__(CurrentHistory.Next, none)))
				{
					CurrentHistory = UWindowEditBoxHistory(CurrentHistory.Next);
					SetValue(CurrentHistory.HistoryText);
					MoveEnd();
				}
			}
			// End:0x3AF
			break;
		// End:0x227
		case 40:
			// End:0x224
			if(__NFUN_130__(bCanEdit, bHistory))
			{
				bAllSelected = false;
				// End:0x224
				if(__NFUN_130__(__NFUN_119__(CurrentHistory, none), __NFUN_119__(CurrentHistory.Prev, none)))
				{
					CurrentHistory = UWindowEditBoxHistory(CurrentHistory.Prev);
					SetValue(CurrentHistory.HistoryText);
					MoveEnd();
				}
			}
			// End:0x3AF
			break;
		// End:0x246
		case 36:
			// End:0x23B
			if(bCanEdit)
			{
				MoveHome();
			}
			bAllSelected = false;
			// End:0x3AF
			break;
		// End:0x265
		case 35:
			// End:0x25A
			if(bCanEdit)
			{
				MoveEnd();
			}
			bAllSelected = false;
			// End:0x3AF
			break;
		// End:0x296
		case 8:
			// End:0x28B
			if(bCanEdit)
			{
				// End:0x285
				if(bAllSelected)
				{
					Clear();					
				}
				else
				{
					Backspace();
				}
			}
			bAllSelected = false;
			// End:0x3AF
			break;
		// End:0x2C7
		case 46:
			// End:0x2BC
			if(bCanEdit)
			{
				// End:0x2B6
				if(bAllSelected)
				{
					Clear();					
				}
				else
				{
					Delete();
				}
			}
			bAllSelected = false;
			// End:0x3AF
			break;
		// End:0x2CC
		case 190:
		// End:0x2EA
		case 110:
			// End:0x2E7
			if(bNumericFloat)
			{
				Insert(byte(__NFUN_237__(".")));
			}
			// End:0x3AF
			break;
		// End:0xFFFF
		default:
			// End:0x36B
			if(bControlDown)
			{
				// End:0x31C
				if(__NFUN_132__(__NFUN_154__(Key, __NFUN_237__("c")), __NFUN_154__(Key, __NFUN_237__("C"))))
				{
					EditCopy();
				}
				// End:0x342
				if(__NFUN_132__(__NFUN_154__(Key, __NFUN_237__("v")), __NFUN_154__(Key, __NFUN_237__("V"))))
				{
					EditPaste();
				}
				// End:0x368
				if(__NFUN_132__(__NFUN_154__(Key, __NFUN_237__("x")), __NFUN_154__(Key, __NFUN_237__("X"))))
				{
					EditCut();
				}				
			}
			else
			{
				// End:0x397
				if(__NFUN_119__(NotifyOwner, none))
				{
					NotifyOwner.KeyDown(Key, X, Y);					
				}
				else
				{
					super.KeyDown(Key, X, Y);
				}
			}
			// End:0x3AF
			break;
			break;
	}
	return;
}

function Click(float X, float Y)
{
	Notify(2);
	return;
}

function LMouseDown(float X, float Y)
{
	super(UWindowWindow).LMouseDown(X, Y);
	Notify(10);
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local float W, h, TextY;

	C.Font = Root.Fonts[Font];
	TextSize(C, "A", W, h);
	TextY = __NFUN_172__(__NFUN_175__(WinHeight, h), float(2));
	TextSize(C, __NFUN_128__(Value, CaretOffset), W, h);
	C.SetDrawColor(byte(255), byte(255), byte(255));
	// End:0xB0
	if(__NFUN_176__(__NFUN_174__(W, offset), float(0)))
	{
		offset = __NFUN_169__(W);
	}
	// End:0xFC
	if(__NFUN_177__(__NFUN_174__(W, offset), __NFUN_175__(WinWidth, float(2))))
	{
		offset = __NFUN_175__(__NFUN_175__(WinWidth, float(2)), W);
		// End:0xFC
		if(__NFUN_177__(offset, float(0)))
		{
			offset = 0.0000000;
		}
	}
	C.DrawColor = TextColor;
	// End:0x1A2
	if(bAllSelected)
	{
		DrawStretchedTexture(C, __NFUN_174__(offset, float(1)), TextY, W, h, Texture'UWindow.WhiteTexture');
		C.SetDrawColor(byte(__NFUN_157__(255, int(C.DrawColor.R))), byte(__NFUN_157__(255, int(C.DrawColor.G))), byte(__NFUN_157__(255, int(C.DrawColor.B))));
	}
	ClipText(C, __NFUN_174__(offset, float(1)), TextY, Value);
	// End:0x1E4
	if(__NFUN_132__(__NFUN_129__(bHasKeyboardFocus), __NFUN_129__(bCanEdit)))
	{
		bShowCaret = false;		
	}
	else
	{
		// End:0x243
		if(__NFUN_132__(__NFUN_177__(GetLevel().TimeSeconds, __NFUN_174__(LastDrawTime, 0.3000000)), __NFUN_176__(GetLevel().TimeSeconds, LastDrawTime)))
		{
			LastDrawTime = GetLevel().TimeSeconds;
			bShowCaret = __NFUN_129__(bShowCaret);
		}
	}
	// End:0x270
	if(bShowCaret)
	{
		ClipText(C, __NFUN_175__(__NFUN_174__(offset, W), float(1)), TextY, "|");
	}
	return;
}

function Close(optional bool bByParent)
{
	// End:0x19
	if(bChangePending)
	{
		bChangePending = false;
		Notify(1);
	}
	bKeyDown = false;
	super(UWindowWindow).Close(bByParent);
	return;
}

function FocusOtherWindow(UWindowWindow W)
{
	// End:0x19
	if(bChangePending)
	{
		bChangePending = false;
		Notify(1);
	}
	// End:0x3B
	if(__NFUN_119__(NotifyOwner, none))
	{
		NotifyOwner.FocusOtherWindow(W);		
	}
	else
	{
		super(UWindowWindow).FocusOtherWindow(W);
	}
	return;
}

function KeyFocusEnter()
{
	// End:0x1C
	if(__NFUN_130__(bSelectOnFocus, __NFUN_129__(bHasKeyboardFocus)))
	{
		SelectAll();
	}
	super.KeyFocusEnter();
	return;
}

function DoubleClick(float X, float Y)
{
	super(UWindowWindow).DoubleClick(X, Y);
	SelectAll();
	return;
}

function KeyFocusExit()
{
	bAllSelected = false;
	super.KeyFocusExit();
	return;
}
