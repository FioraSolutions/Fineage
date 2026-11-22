class MaterialSwitch extends Modifier
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material,Modifier);

cpptext
{
	virtual void PostEditChange();
	virtual UBOOL CheckCircularReferences( TArray<class UMaterial*>& History );

}

var() transient int Current;
var() editinlineuse array<editinlineuse Material> Materials;

function Reset()
{
	Current = 0;
	// End:0x23
	if(__NFUN_151__(Materials.Length, 0))
	{
		Material = Materials[0];		
	}
	else
	{
		Material = none;
	}
	// End:0x44
	if(__NFUN_119__(Material, none))
	{
		Material.Reset();
	}
	// End:0x5E
	if(__NFUN_119__(FallbackMaterial, none))
	{
		FallbackMaterial.Reset();
	}
	return;
}

function Trigger(Actor Other, Actor EventInstigator)
{
	__NFUN_165__(Current);
	// End:0x1E
	if(__NFUN_153__(Current, Materials.Length))
	{
		Current = 0;
	}
	// End:0x3E
	if(__NFUN_151__(Materials.Length, 0))
	{
		Material = Materials[Current];		
	}
	else
	{
		Material = none;
	}
	// End:0x69
	if(__NFUN_119__(Material, none))
	{
		Material.Trigger(Other, EventInstigator);
	}
	// End:0x8D
	if(__NFUN_119__(FallbackMaterial, none))
	{
		FallbackMaterial.Trigger(Other, EventInstigator);
	}
	return;
}
