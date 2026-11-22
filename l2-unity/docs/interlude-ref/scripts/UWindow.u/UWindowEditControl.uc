class UWindowEditControl extends UWindowDialogControl;

var float EditBoxWidth;
var float EditAreaDrawX;
var float EditAreaDrawY;
var UWindowEditBox EditBox;

function Created()
{
	super.Created();
	EditBox = UWindowEditBox(CreateWindow(Class'UWindow.UWindowEditBox', 0.0000000, 0.0000000, WinWidth, WinHeight));
	EditBox.NotifyOwner = self;
	EditBox.bSelectOnFocus = true;
	EditBoxWidth = __NFUN_172__(WinWidth, float(2));
	SetEditTextColor(LookAndFeel.EditBoxTextColor);
	return;
}

function SetNumericOnly(bool bNumericOnly)
{
	EditBox.bNumericOnly = bNumericOnly;
	return;
}

function SetNumericFloat(bool bNumericFloat)
{
	EditBox.bNumericFloat = bNumericFloat;
	return;
}

function SetFont(int NewFont)
{
	super.SetFont(NewFont);
	EditBox.SetFont(NewFont);
	return;
}

function SetHistory(bool bInHistory)
{
	EditBox.SetHistory(bInHistory);
	return;
}

function SetEditTextColor(Color NewColor)
{
	EditBox.SetTextColor(NewColor);
	return;
}

function Clear()
{
	EditBox.Clear();
	return;
}

function string GetValue()
{
	return EditBox.GetValue();
	return;
}

function SetValue(string NewValue)
{
	EditBox.SetValue(NewValue);
	return;
}

function SetMaxLength(int MaxLength)
{
	EditBox.MaxLength = MaxLength;
	return;
}

function Paint(Canvas C, float X, float Y)
{
	LookAndFeel.Editbox_Draw(self, C);
	super(UWindowWindow).Paint(C, X, Y);
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	super.BeforePaint(C, X, Y);
	LookAndFeel.Editbox_SetupSizes(self, C);
	return;
}

function SetDelayedNotify(bool bDelayedNotify)
{
	EditBox.bDelayedNotify = bDelayedNotify;
	return;
}
