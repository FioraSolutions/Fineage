class UWindowTabControlLeftButton extends UWindowButton;

function BeforePaint(Canvas C, float X, float Y)
{
	LookAndFeel.Tab_SetupLeftButton(self);
	return;
}

function LMouseDown(float X, float Y)
{
	super(UWindowWindow).LMouseDown(X, Y);
	// End:0x39
	if(__NFUN_129__(bDisabled))
	{
		__NFUN_166__(UWindowTabControl(ParentWindow).TabArea.TabOffset);
	}
	return;
}

defaultproperties
{
	bNoKeyboard=true
}