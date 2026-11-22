class ObjectPool extends Object;

var array<Object> Objects;

simulated function Object AllocateObject(Class ObjectClass)
{
	local Object Result;
	local int ObjectIndex;

	ObjectIndex = 0;
	J0x07:

	// End:0x5F [Loop If]
	if(__NFUN_150__(ObjectIndex, Objects.Length))
	{
		// End:0x55
		if(__NFUN_114__(Objects[ObjectIndex].Class, ObjectClass))
		{
			Result = Objects[ObjectIndex];
			Objects.Remove(ObjectIndex, 1);
			// [Explicit Break]
			goto J0x5F;
		}
		__NFUN_165__(ObjectIndex);
		// [Loop Continue]
		goto J0x07;
	}
	J0x5F:

	// End:0x79
	if(__NFUN_114__(Result, none))
	{
		Result = new (none) ObjectClass;
	}
	return Result;
	return;
}

simulated function FreeObject(Object Obj)
{
	Objects.Length = __NFUN_146__(Objects.Length, 1);
	Objects[__NFUN_147__(Objects.Length, 1)] = Obj;
	return;
}

simulated function Shrink()
{
	J0x00:
	// End:0x1F [Loop If]
	if(__NFUN_151__(Objects.Length, 0))
	{
		Objects.Remove(__NFUN_147__(Objects.Length, 1), 1);
		// [Loop Continue]
		goto J0x00;
	}
	return;
}
