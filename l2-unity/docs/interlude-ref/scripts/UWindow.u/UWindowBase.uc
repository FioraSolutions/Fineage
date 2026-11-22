class UWindowBase extends Interaction;

const F_Normal = 0;
const F_Bold = 1;
const F_Large = 2;
const F_LargeBold = 3;

enum TextAlign
{
	TA_Left,                        // 0
	TA_Right,                       // 1
	TA_Center                       // 2
};

enum FrameHitTest
{
	HT_NW,                          // 0
	HT_N,                           // 1
	HT_NE,                          // 2
	HT_W,                           // 3
	HT_E,                           // 4
	HT_SW,                          // 5
	HT_S,                           // 6
	HT_SE,                          // 7
	HT_TitleBar,                    // 8
	HT_DragHandle,                  // 9
	HT_None                         // 10
};

enum MenuSound
{
	MS_MenuPullDown,                // 0
	MS_MenuCloseUp,                 // 1
	MS_MenuItem,                    // 2
	MS_WindowOpen,                  // 3
	MS_WindowClose,                 // 4
	MS_ChangeTab                    // 5
};

enum MessageBoxButtons
{
	MB_YesNo,                       // 0
	MB_OKCancel,                    // 1
	MB_OK,                          // 2
	MB_YesNoCancel                  // 3
};

enum MessageBoxResult
{
	MR_None,                        // 0
	MR_Yes,                         // 1
	MR_No,                          // 2
	MR_OK,                          // 3
	MR_Cancel                       // 4
};

enum PropertyCondition
{
	PC_None,                        // 0
	PC_LessThan,                    // 1
	PC_Equal,                       // 2
	PC_GreaterThan,                 // 3
	PC_NotEqual,                    // 4
	PC_Contains,                    // 5
	PC_NotContains                  // 6
};

struct Region
{
	var() int X;
	var() int Y;
	var() int W;
	var() int h;
};

struct TexRegion
{
	var() int X;
	var() int Y;
	var() int W;
	var() int h;
	var() Texture t;
};

struct HTMLStyle
{
	var int BulletLevel;
	var string LinkDestination;
	var Color TextColor;
	var Color BGColor;
	var bool bCenter;
	var bool bLink;
	var bool bUnderline;
	var bool bNoBR;
	var bool bHeading;
	var bool bBold;
	var bool bBlink;
};

function Region NewRegion(float X, float Y, float W, float h)
{
	local Region R;

	R.X = int(X);
	R.Y = int(Y);
	R.W = int(W);
	R.h = int(h);
	return R;
	return;
}

function TexRegion NewTexRegion(float X, float Y, float W, float h, Texture t)
{
	local TexRegion R;

	R.X = int(X);
	R.Y = int(Y);
	R.W = int(W);
	R.h = int(h);
	R.t = t;
	return R;
	return;
}

function Region GetRegion(TexRegion t)
{
	local Region R;

	R.X = t.X;
	R.Y = t.Y;
	R.W = t.W;
	R.h = t.h;
	return R;
	return;
}

static function int InStrAfter(string Text, string Match, int pos)
{
	local int i;

	i = __NFUN_126__(__NFUN_127__(Text, pos), Match);
	// End:0x35
	if(__NFUN_155__(i, -1))
	{
		return __NFUN_146__(i, pos);
	}
	return -1;
	return;
}

static function Object BuildObjectWithProperties(string Text)
{
	local int i;
	local string ObjectClass, PropertyName, PropertyValue, temp;
	local Class C;
	local Object O;

	i = __NFUN_126__(Text, ",");
	// End:0x35
	if(__NFUN_154__(i, -1))
	{
		ObjectClass = Text;
		Text = "";		
	}
	else
	{
		ObjectClass = __NFUN_128__(Text, i);
		Text = __NFUN_127__(Text, __NFUN_146__(i, 1));
	}
	C = Class<Object>(DynamicLoadObject(ObjectClass, Class'Core.Class'));
	O = new C;
	J0x86:

	// End:0x296 [Loop If]
	if(__NFUN_123__(Text, ""))
	{
		i = __NFUN_126__(Text, "=");
		// End:0xF9
		if(__NFUN_154__(i, -1))
		{
			__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Missing value for property ", ObjectClass), "."), Text));
			PropertyName = Text;
			PropertyValue = "";			
		}
		else
		{
			PropertyName = __NFUN_128__(Text, i);
			Text = __NFUN_127__(Text, __NFUN_146__(i, 1));
		}
		// End:0x21E
		if(__NFUN_122__(__NFUN_128__(Text, 1), "\""))
		{
			i = InStrAfter(Text, "\"", 1);
			// End:0x183
			if(__NFUN_154__(i, -1))
			{
				__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Missing quote for ", ObjectClass), "."), PropertyName));
				return O;
			}
			PropertyValue = __NFUN_127__(Text, 1, __NFUN_147__(i, 1));
			temp = __NFUN_127__(Text, __NFUN_146__(i, 1), 1);
			// End:0x205
			if(__NFUN_130__(__NFUN_123__(temp, ""), __NFUN_123__(temp, ",")))
			{
				__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Missing comma after close quote for ", ObjectClass), "."), PropertyName));
			}
			Text = __NFUN_127__(Text, __NFUN_146__(i, 2));			
		}
		else
		{
			i = __NFUN_126__(Text, ",");
			// End:0x253
			if(__NFUN_154__(i, -1))
			{
				PropertyValue = Text;
				Text = "";				
			}
			else
			{
				PropertyValue = __NFUN_128__(Text, i);
				Text = __NFUN_127__(Text, __NFUN_146__(i, 1));
			}
		}
		O.SetPropertyText(PropertyName, PropertyValue);
		// [Loop Continue]
		goto J0x86;
	}
	return O;
	return;
}
