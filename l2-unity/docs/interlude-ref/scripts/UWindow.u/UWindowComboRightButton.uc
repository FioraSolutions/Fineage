class UWindowComboRightButton extends UWindowButton;

function BeforePaint(Canvas C, float X, float Y)
{
	LookAndFeel.Combo_SetupRightButton(self);
	return;
}

function LMouseDown(float X, float Y)
{
	local int i;

	super(UWindowWindow).LMouseDown(X, Y);
	// End:0x8C
	if(__NFUN_129__(bDisabled))
	{
		i = UWindowComboControl(OwnerWindow).GetSelectedIndex();
		__NFUN_165__(i);
		// End:0x73
		if(__NFUN_153__(i, UWindowComboControl(OwnerWindow).List.Items.Count()))
		{
			i = 0;
		}
		UWindowComboControl(OwnerWindow).SetSelectedIndex(i);
	}
	return;
}

defaultproperties
{
	bNoKeyboard=true
}