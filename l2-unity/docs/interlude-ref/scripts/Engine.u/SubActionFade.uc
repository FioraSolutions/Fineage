class SubActionFade extends MatSubAction
	native
	editinlinenew;

var(Fade) Color FadeColor;
var(Fade) bool bFadeOut;

defaultproperties
{
	bFadeOut=true
	Icon=Texture'Engine.SubActionFade'
	Desc="Fade"
}