class UWindowSBDownButton extends UWindowButton;

var float NextClickTime;

function Created()
{
	bNoKeyboard = true;
	super.Created();
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	LookAndFeel.SB_SetupDownButton(self);
	return;
}

function LMouseDown(float X, float Y)
{
	super(UWindowWindow).LMouseDown(X, Y);
	// End:0x1B
	if(bDisabled)
	{
		return;
	}
	UWindowVScrollbar(ParentWindow).Scroll(UWindowVScrollbar(ParentWindow).ScrollAmount);
	NextClickTime = __NFUN_174__(Root.GetPlayerOwner().Level.TimeSeconds, 0.5000000);
	return;
}

function Tick(float Delta)
{
	// End:0x9A
	if(__NFUN_130__(__NFUN_130__(bMouseDown, __NFUN_177__(NextClickTime, float(0))), __NFUN_176__(NextClickTime, Root.GetPlayerOwner().Level.TimeSeconds)))
	{
		UWindowVScrollbar(ParentWindow).Scroll(UWindowVScrollbar(ParentWindow).ScrollAmount);
		NextClickTime = __NFUN_174__(Root.GetPlayerOwner().Level.TimeSeconds, 0.1000000);
	}
	// End:0xB0
	if(__NFUN_129__(bMouseDown))
	{
		NextClickTime = 0.0000000;
	}
	return;
}
