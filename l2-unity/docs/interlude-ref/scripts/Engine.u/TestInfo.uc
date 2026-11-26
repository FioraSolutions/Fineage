class TestInfo extends Info
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

const Pie = 3.14;
const Str = "Tim";
const Lotus = vect(1,2,3);

struct STest
{
	var bool b1;
	var int i;
	var bool b2;
	var bool b3;
};

var() bool bTrue1;
var() bool bFalse1;
var() bool bTrue2;
var() bool bFalse2;
var bool bBool1;
var bool bBool2;
var() int xnum;
var float ppp;
var string sxx;
var int MyArray[2];
var Vector v1;
var Vector v2;
var string TestRepStr;
var STest ST;

function TestQ()
{
	local Vector V;

	V.X = 2.0000000;
	V.Y = 3.0000000;
	V.Z = 4.0000000;
	assert(__NFUN_217__(V, vect(2.0000000, 3.0000000, 4.0000000)));
	assert(__NFUN_180__(V.Z, float(4)));
	assert(__NFUN_180__(V.Y, float(3)));
	assert(__NFUN_180__(V.X, float(2)));
	return;
}

static function test()
{
	Class'Engine.TestInfo'.default.v1 = vect(1.0000000, 2.0000000, 3.0000000);
	return;
}

function PostBeginPlay()
{
	__NFUN_231__("!!BEGIN");
	default.v1 = vect(5.0000000, 4.0000000, 3.0000000);
	assert(__NFUN_217__(default.v1, vect(5.0000000, 4.0000000, 3.0000000)));
	test();
	assert(__NFUN_217__(default.v1, vect(1.0000000, 2.0000000, 3.0000000)));
	assert(__NFUN_303__('Actor'));
	assert(__NFUN_303__('TestInfo'));
	assert(__NFUN_303__('Info'));
	assert(__NFUN_129__(__NFUN_303__('LevelInfo')));
	assert(__NFUN_129__(__NFUN_303__('Texture')));
	__NFUN_231__("!!END");
	return;
}

function TestStructBools()
{
	assert(__NFUN_242__(ST.b1, false));
	assert(__NFUN_242__(ST.b2, false));
	assert(__NFUN_242__(ST.b3, false));
	ST.b1 = true;
	assert(__NFUN_242__(ST.b1, true));
	assert(__NFUN_242__(ST.b2, false));
	assert(__NFUN_242__(ST.b3, false));
	ST.b2 = true;
	assert(__NFUN_242__(ST.b1, true));
	assert(__NFUN_242__(ST.b2, true));
	assert(__NFUN_242__(ST.b3, false));
	ST.b3 = true;
	assert(__NFUN_242__(ST.b1, true));
	assert(__NFUN_242__(ST.b2, true));
	assert(__NFUN_242__(ST.b3, true));
	ST.b1 = false;
	ST.b2 = false;
	ST.b3 = false;
	return;
}

function BeginPlay()
{
	local TestObj to;

	to = new Class'Engine.TestObj';
	to = new Class'Engine.TestObj';
	to = new (self) Class'Engine.TestObj';
	to = new (self, "") Class'Engine.TestObj';
	to = new (self, "", 0) Class'Engine.TestObj';
	to.test();
	TestStructBools();
	return;
}

function TestX(bool bResource)
{
	local int N;

	N = int(bResource);
	MyArray[int(bResource)] = 0;
	__NFUN_165__(MyArray[int(bResource)]);
	return;
}

function bool RecurseTest()
{
	bBool1 = true;
	return false;
	return;
}

function TestLimitor(Class C)
{
	local Class<Actor> NewClass;

	NewClass = Class<Actor>(C);
	return;
}

static function int OtherStatic(int i)
{
	assert(__NFUN_154__(i, 246));
	assert(__NFUN_154__(default.xnum, 777));
	return 555;
	return;
}

static function int TestStatic(int i)
{
	assert(__NFUN_154__(i, 123));
	assert(__NFUN_154__(default.xnum, 777));
	assert(__NFUN_154__(OtherStatic(__NFUN_144__(i, 2)), 555));
	return i;
	return;
}

function TestContinueFor()
{
	local int i;

	__NFUN_231__("TestContinue");
	i = 0;
	J0x17:

	// End:0x76 [Loop If]
	if(__NFUN_150__(i, 20))
	{
		__NFUN_231__(__NFUN_112__("iteration ", string(i)));
		// End:0x65
		if(__NFUN_132__(__NFUN_132__(__NFUN_154__(i, 7), __NFUN_154__(i, 9)), __NFUN_154__(i, 19)))
		{
			// [Explicit Continue]
			goto J0x6C;
		}
		__NFUN_231__("...");
		J0x6C:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x17;
	}
	__NFUN_231__("DoneContinue");
	return;
}

function TestContinueWhile()
{
	local int i;

	__NFUN_231__("TestContinue");
	J0x10:

	// End:0x5C [Loop If]
	if(__NFUN_152__(__NFUN_163__(i), 20))
	{
		__NFUN_231__(__NFUN_112__("iteration ", string(i)));
		// End:0x52
		if(__NFUN_132__(__NFUN_154__(i, 7), __NFUN_154__(i, 9)))
		{			
		}
		else
		{
			__NFUN_231__("...");
		}
		// [Loop Continue]
		goto J0x10;
	}
	__NFUN_231__("DoneContinue");
	return;
}

function TestContinueDoUntil()
{
	local int i;

	__NFUN_231__("TestContinue");
	J0x10:

	__NFUN_165__(i);
	__NFUN_231__(__NFUN_112__("iteration ", string(i)));
	// End:0x59
	if(__NFUN_132__(__NFUN_132__(__NFUN_154__(i, 7), __NFUN_154__(i, 9)), __NFUN_151__(i, 18)))
	{		
	}
	else
	{
		__NFUN_231__("...");
	}
	// End:0x10
	if(!(__NFUN_151__(i, 20)))
		goto J0x10;
	__NFUN_231__("DoneContinue");
	return;
}

function TestContinueForEach()
{
	local Actor A;

	__NFUN_231__("TestContinue");
	// End:0x4F
	foreach __NFUN_304__(Class'Engine.Actor', A)
	{
		__NFUN_231__(__NFUN_112__("actor ", string(A)));
		// End:0x47
		if(__NFUN_114__(Light(A), none))
		{
			continue;			
		}
		__NFUN_231__("...");		
	}	
	__NFUN_231__("DoneContinue");
	return;
}

function SubTestOptionalOut(optional out int A, optional out int B, optional out int C)
{
	__NFUN_159__(A, float(2));
	B = __NFUN_144__(B, 2);
	__NFUN_161__(C, C);
	return;
}

function TestOptionalOut()
{
	local int A, B, C;

	A = 1;
	B = 2;
	C = 3;
	SubTestOptionalOut(A, B, C);
	assert(__NFUN_154__(A, 2));
	assert(__NFUN_154__(B, 4));
	assert(__NFUN_154__(C, 6));
	SubTestOptionalOut(A, B);
	assert(__NFUN_154__(A, 4));
	assert(__NFUN_154__(B, 8));
	assert(__NFUN_154__(C, 6));
	SubTestOptionalOut(, B, C);
	assert(__NFUN_154__(A, 4));
	assert(__NFUN_154__(B, 16));
	assert(__NFUN_154__(C, 12));
	SubTestOptionalOut();
	assert(__NFUN_154__(A, 4));
	assert(__NFUN_154__(B, 16));
	assert(__NFUN_154__(C, 12));
	SubTestOptionalOut(A, B, C);
	assert(__NFUN_154__(A, 8));
	assert(__NFUN_154__(B, 32));
	assert(__NFUN_154__(C, 24));
	__NFUN_231__("TestOptionalOut ok!");
	return;
}

function TestNullContext(Actor A)
{
	bHidden = A.bHidden;
	A.bHidden = bHidden;
	return;
}

function TestSwitch()
{
	local string S;
	local int i;
	local bool B;

	S = "Tim";
	i = 2;
	switch(i)
	{
		// End:0x25
		case 0:
			assert(false);
			// End:0x3F
			break;
		// End:0x35
		case 2:
			B = true;
			// End:0x3F
			break;
		// End:0xFFFF
		default:
			assert(false);
			// End:0x3F
			break;
			break;
	}
	assert(B);
	switch(S)
	{
		// End:0x5B
		case "":
			assert(false);
			// End:0x7A
			break;
		// End:0x6C
		case "xyzzy":
			assert(false);
			// End:0x7A
			break;
		// End:0xFFFF
		default:
			B = false;
			// End:0x7A
			break;
			break;
	}
	assert(__NFUN_129__(B));
	__NFUN_231__("testswitch succeeded");
	return;
}

function Tick(float DeltaTime)
{
	local Class C;
	local Class<TestInfo> TC;

	__NFUN_231__(__NFUN_112__("time=", string(Level.TimeSeconds)));
	TestOptionalOut();
	TestNullContext(self);
	TestNullContext(none);
	TestSwitch();
	v1 = vect(1.0000000, 2.0000000, 3.0000000);
	v2 = vect(2.0000000, 4.0000000, 6.0000000);
	assert(__NFUN_218__(v1, v2));
	assert(__NFUN_129__(__NFUN_217__(v1, v2)));
	assert(__NFUN_217__(v1, vect(1.0000000, 2.0000000, 3.0000000)));
	assert(__NFUN_217__(v2, vect(2.0000000, 4.0000000, 6.0000000)));
	assert(__NFUN_218__(vect(1.0000000, 2.0000000, 5.0000000), v1));
	assert(__NFUN_217__(__NFUN_212__(v1, float(2)), v2));
	assert(__NFUN_217__(v1, __NFUN_214__(v2, float(2))));
	assert(__NFUN_180__(3.1400001, 3.1400001));
	assert(__NFUN_181__(3.1400001, float(2)));
	assert(__NFUN_122__("Tim", "Tim"));
	assert(__NFUN_123__("Tim", "Bob"));
	assert(__NFUN_217__(vect(1.0000000, 2.0000000, 3.0000000), vect(1.0000000, 2.0000000, 3.0000000)));
	assert(__NFUN_122__(GetPropertyText("sxx"), "Tim"));
	assert(__NFUN_123__(GetPropertyText("ppp"), "123"));
	assert(__NFUN_122__(GetPropertyText("bogus"), ""));
	xnum = 345;
	assert(__NFUN_122__(GetPropertyText("xnum"), "345"));
	SetPropertyText("xnum", "999");
	assert(__NFUN_154__(xnum, 999));
	assert(__NFUN_155__(xnum, 666));
	assert(__NFUN_242__(bTrue1, true));
	assert(__NFUN_242__(bFalse1, false));
	assert(__NFUN_242__(bTrue2, true));
	assert(__NFUN_242__(bFalse2, false));
	assert(__NFUN_242__(default.bTrue1, true));
	assert(__NFUN_242__(default.bFalse1, false));
	assert(__NFUN_242__(default.bTrue2, true));
	assert(__NFUN_242__(default.bFalse2, false));
	assert(__NFUN_242__(Class'Engine.TestInfo'.default.bTrue1, true));
	assert(__NFUN_242__(Class'Engine.TestInfo'.default.bFalse1, false));
	assert(__NFUN_242__(Class'Engine.TestInfo'.default.bTrue2, true));
	assert(__NFUN_242__(Class'Engine.TestInfo'.default.bFalse2, false));
	TC = Class;
	assert(__NFUN_242__(TC.default.bTrue1, true));
	assert(__NFUN_242__(TC.default.bFalse1, false));
	assert(__NFUN_242__(TC.default.bTrue2, true));
	assert(__NFUN_242__(TC.default.bFalse2, false));
	C = Class;
	assert(__NFUN_242__(Class<TestInfo>(C).default.bTrue1, true));
	assert(__NFUN_242__(Class<TestInfo>(C).default.bFalse1, false));
	assert(__NFUN_242__(Class<TestInfo>(C).default.bTrue2, true));
	assert(__NFUN_242__(Class<TestInfo>(C).default.bFalse2, false));
	assert(__NFUN_154__(default.xnum, 777));
	TestStatic(123);
	TC.static.TestStatic(123);
	Class<TestInfo>(C).static.TestStatic(123);
	bBool2 = RecurseTest();
	assert(__NFUN_242__(bBool2, false));
	TestStructBools();
	TestQ();
	__NFUN_231__("All tests passed");
	return;
}

function f()
{
	return;
}

function temp()
{
	temp();
	return;
}

state AA
{	stop;
}

state BB
{	stop;
}

state CCAA extends AA
{	stop;
}

state DDAA extends AA
{	stop;
}

state EEDDAA extends DDAA
{	stop;
}

defaultproperties
{
	bTrue1=true
	bTrue2=true
	xnum=777
	ppp=3.1400001
	sxx="Tim"
	bHidden=false
	bAlwaysRelevant=true
	RemoteRole=2
}