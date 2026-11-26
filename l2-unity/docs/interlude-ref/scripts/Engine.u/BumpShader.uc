class BumpShader extends RenderedMaterial
	native
	editinlinenew
	collapsecategories
	noexport
	hidecategories(Object);

var() editinlineuse Material Diffuse;
var() editinlineuse Material BumpMaterial;
var Material OldBumpMaterial;
var const transient byte BumpRawData;
var const transient int BumpTexInterface;

function Reset()
{
	return;
}

function Trigger(Actor Other, Actor EventInstigator)
{
	return;
}
