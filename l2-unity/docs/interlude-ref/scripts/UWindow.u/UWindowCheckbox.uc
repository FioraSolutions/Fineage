class UWindowCheckbox extends UWindowButton;

var bool bChecked;

function BeforePaint(Canvas C, float X, float Y)
{
	LookAndFeel.Checkbox_SetupSizes(self, C);
	super.BeforePaint(C, X, Y);
	return;
}

function Paint(Canvas C, float X, float Y)
{
	LookAndFeel.Checkbox_Draw(self, C);
	super.Paint(C, X, Y);
	return;
}

function LMouseUp(float X, float Y)
{
	// End:0x22
	if(__NFUN_129__(bDisabled))
	{
		bChecked = __NFUN_129__(bChecked);
		Notify(1);
	}
	super(UWindowWindow).LMouseUp(X, Y);
	return;
}
