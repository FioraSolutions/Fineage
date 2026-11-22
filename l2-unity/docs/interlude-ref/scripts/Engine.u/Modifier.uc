class Modifier extends Material
	abstract
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material);

var() editinlineuse Material Material;

function Reset()
{
	// End:0x1A
	if(__NFUN_119__(Material, none))
	{
		Material.Reset();
	}
	// End:0x34
	if(__NFUN_119__(FallbackMaterial, none))
	{
		FallbackMaterial.Reset();
	}
	return;
}

function Trigger(Actor Other, Actor EventInstigator)
{
	// End:0x24
	if(__NFUN_119__(Material, none))
	{
		Material.Trigger(Other, EventInstigator);
	}
	// End:0x48
	if(__NFUN_119__(FallbackMaterial, none))
	{
		FallbackMaterial.Trigger(Other, EventInstigator);
	}
	return;
}
