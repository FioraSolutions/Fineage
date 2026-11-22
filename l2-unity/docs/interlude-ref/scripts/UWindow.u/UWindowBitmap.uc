class UWindowBitmap extends UWindowWindow;

var Texture t;
var Region R;
var bool bStretch;
var bool bCenter;

function Paint(Canvas C, float X, float Y)
{
	// End:0x60
	if(bStretch)
	{
		DrawStretchedTextureSegment(C, 0.0000000, 0.0000000, WinWidth, WinHeight, float(R.X), float(R.Y), float(R.W), float(R.h), t);		
	}
	else
	{
		// End:0xF6
		if(bCenter)
		{
			DrawStretchedTextureSegment(C, __NFUN_172__(__NFUN_175__(WinWidth, float(R.W)), float(2)), __NFUN_172__(__NFUN_175__(WinHeight, float(R.h)), float(2)), float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);			
		}
		else
		{
			DrawStretchedTextureSegment(C, 0.0000000, 0.0000000, float(R.W), float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
		}
	}
	return;
}
