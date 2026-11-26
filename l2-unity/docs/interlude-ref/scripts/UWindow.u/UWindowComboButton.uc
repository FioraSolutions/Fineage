class UWindowComboButton extends UWindowButton;

var UWindowComboControl Owner;

function BeforePaint(Canvas C, float X, float Y)
{
	LookAndFeel.Combo_GetButtonBitmaps(self);
	bMouseDown = Owner.bListVisible;
	return;
}

function LMouseDown(float X, float Y)
{
	// End:0x5B
	if(__NFUN_129__(bDisabled))
	{
		// End:0x2F
		if(Owner.bListVisible)
		{
			Owner.CloseUp();			
		}
		else
		{
			Owner.DropDown();
			Root.CaptureMouse(Owner.List);
		}
	}
	return;
}

function Click(float X, float Y)
{
	return;
}

function FocusOtherWindow(UWindowWindow W)
{
	super(UWindowWindow).FocusOtherWindow(W);
	// End:0x95
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(Owner.bListVisible, __NFUN_119__(W.ParentWindow, Owner)), __NFUN_119__(W.ParentWindow, Owner.List)), __NFUN_119__(W.ParentWindow.ParentWindow, Owner.List)))
	{
		Owner.CloseUp();
	}
	return;
}

defaultproperties
{
	bNoKeyboard=true
}