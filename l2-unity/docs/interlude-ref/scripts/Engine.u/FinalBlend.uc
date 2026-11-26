class FinalBlend extends Modifier
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

enum EFrameBufferBlending
{
	FB_Overwrite,                   // 0
	FB_Modulate,                    // 1
	FB_AlphaBlend,                  // 2
	FB_AlphaModulate_MightNotFogCorrectly,// 3
	FB_Translucent,                 // 4
	FB_Darken,                      // 5
	FB_Brighten,                    // 6
	FB_Invisible,                   // 7
	FB_Add                          // 8
};

var() FinalBlend.EFrameBufferBlending FrameBufferBlending;
var() bool ZWrite;
var() bool ZTest;
var() bool AlphaTest;
var() bool TwoSided;
var() byte AlphaRef;
var() bool TreatAsTwoSided;

defaultproperties
{
	ZWrite=true
	ZTest=true
}