class L2ColorModifier extends Modifier
	native
	noteditinlinenew
	collapsecategories
	hidecategories(Object,Material);

enum EL2TextureOp
{
	P_DISABLE,                      // 0
	P_SELECTARG1,                   // 1
	P_SELECTARG2,                   // 2
	P_MODULATE,                     // 3
	P_MODULATE2X,                   // 4
	P_MODULATE4X,                   // 5
	P_ADD,                          // 6
	P_ADDSIGNED,                    // 7
	P_ADDSIGNED2X,                  // 8
	P_SUBTRACT,                     // 9
	P_ADDSMOOTH,                    // 10
	P_BLENDDIFFUSEALPHA,            // 11
	P_BLENDTEXTUREALPHA,            // 12
	P_BLENDFACTORALPHA,             // 13
	P_BLENDTEXTUREALPHAPM,          // 14
	P_BLENDCURRENTALPHA,            // 15
	P_PREMODULATE,                  // 16
	P_MODULATEALPHA_ADDCOLOR,       // 17
	P_MODULATECOLOR_ADDALPHA,       // 18
	P_MODULATEINVALPHA_ADDCOLOR,    // 19
	P_MODULATEINVCOLOR_ADDALPHA,    // 20
	P_BUMPENVMAP,                   // 21
	P_BUMPENVMAPLUMINANCE,          // 22
	P_DOTPRODUCT3,                  // 23
	P_MULTIPLYADD                   // 24
};

var() Color Color;
var() bool RenderTwoSided;
var() bool AlphaBlend;
var() L2ColorModifier.EL2TextureOp AlphaOp;
var() L2ColorModifier.EL2TextureOp ColorOp;

defaultproperties
{
	Color=(R=255,G=255,B=255,A=255)
	RenderTwoSided=true
	AlphaBlend=true
}