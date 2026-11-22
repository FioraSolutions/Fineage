class InteractionMaster extends Interactions
	transient
	native;

var transient Client Client;
var const transient Interaction BaseMenu;
var const transient Interaction Console;
var transient array<Interaction> GlobalInteractions;
var transient bool bRequireRawJoystick;

// Export UInteractionMaster::execTravel(FFrame&, void* const)
native function Travel(string URL);

event Interaction AddInteraction(string InteractionName, optional Player AttachTo)
{
	local Interaction NewInteraction;
	local Class<Interaction> NewInteractionClass;

	NewInteractionClass = Class<Interaction>(DynamicLoadObject(InteractionName, Class'Core.Class'));
	// End:0x12E
	if(__NFUN_119__(NewInteractionClass, none))
	{
		NewInteraction = new NewInteractionClass;
		// End:0xF8
		if(__NFUN_119__(NewInteraction, none))
		{
			// End:0xAB
			if(__NFUN_119__(AttachTo, none))
			{
				AttachTo.LocalInteractions.Length = __NFUN_146__(AttachTo.LocalInteractions.Length, 1);
				AttachTo.LocalInteractions[__NFUN_147__(AttachTo.LocalInteractions.Length, 1)] = NewInteraction;
				NewInteraction.ViewportOwner = AttachTo;				
			}
			else
			{
				GlobalInteractions.Length = __NFUN_146__(GlobalInteractions.Length, 1);
				GlobalInteractions[__NFUN_147__(GlobalInteractions.Length, 1)] = NewInteraction;
			}
			NewInteraction.Initialize();
			NewInteraction.Master = self;
			return NewInteraction;			
		}
		else
		{
			__NFUN_231__(__NFUN_112__(__NFUN_112__("Could not create interaction [", InteractionName), "]"), 'IMaster');
		}		
	}
	else
	{
		__NFUN_231__(__NFUN_112__(__NFUN_112__("Could not load interaction [", InteractionName), "]"), 'IMaster');
	}
	return none;
	return;
}

event RemoveInteraction(Interaction RemoveMe)
{
	local int Index;

	// End:0x91
	if(__NFUN_119__(RemoveMe.ViewportOwner, none))
	{
		Index = 0;
		J0x1B:

		// End:0x8E [Loop If]
		if(__NFUN_150__(Index, RemoveMe.ViewportOwner.LocalInteractions.Length))
		{
			// End:0x84
			if(__NFUN_114__(RemoveMe.ViewportOwner.LocalInteractions[Index], RemoveMe))
			{
				RemoveMe.ViewportOwner.LocalInteractions.Remove(Index, 1);
				return;
			}
			__NFUN_165__(Index);
			// [Loop Continue]
			goto J0x1B;
		}		
	}
	else
	{
		Index = 0;
		J0x98:

		// End:0xD5 [Loop If]
		if(__NFUN_150__(Index, GlobalInteractions.Length))
		{
			// End:0xCB
			if(__NFUN_114__(GlobalInteractions[Index], RemoveMe))
			{
				GlobalInteractions.Remove(Index, 1);
				return;
			}
			__NFUN_165__(Index);
			// [Loop Continue]
			goto J0x98;
		}
	}
	__NFUN_231__(__NFUN_112__(__NFUN_112__("Could not remove interaction [", string(RemoveMe)), "] (Not Found)"), 'IMaster');
	return;
}

event SetFocusTo(Interaction Inter, optional Player ViewportOwner)
{
	local array<Interaction> InteractionArray;
	local Interaction temp;
	local int i, iIndex;

	// End:0x22
	if(__NFUN_119__(ViewportOwner, none))
	{
		InteractionArray = ViewportOwner.LocalInteractions;		
	}
	else
	{
		InteractionArray = GlobalInteractions;
	}
	// End:0x6A
	if(__NFUN_154__(InteractionArray.Length, 0))
	{
		__NFUN_231__("Attempt to SetFocus on an empty Array.", 'IMaster');
		return;
	}
	iIndex = -1;
	i = 0;
	J0x7C:

	// End:0xB9 [Loop If]
	if(__NFUN_150__(i, InteractionArray.Length))
	{
		// End:0xAF
		if(__NFUN_114__(InteractionArray[i], Inter))
		{
			iIndex = i;
			// [Explicit Break]
			goto J0xB9;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x7C;
	}
	J0xB9:

	// End:0x104
	if(__NFUN_150__(iIndex, 0))
	{
		__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Interaction ", string(Inter)), " is not in "), string(ViewportOwner)), "."), 'IMaster');
		return;		
	}
	else
	{
		// End:0x111
		if(__NFUN_154__(iIndex, 0))
		{
			return;
		}
	}
	temp = InteractionArray[iIndex];
	i = 0;
	J0x129:

	// End:0x15C [Loop If]
	if(__NFUN_150__(i, iIndex))
	{
		InteractionArray[__NFUN_146__(i, 1)] = InteractionArray[i];
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x129;
	}
	InteractionArray[0] = temp;
	InteractionArray[0].bActive = true;
	InteractionArray[0].bVisible = true;
	return;
}

event bool Process_KeyType(array<Interaction> InteractionArray, out Interactions.EInputKey Key, optional string Unicode)
{
	local int Index;

	Index = 0;
	J0x07:

	// End:0x7B [Loop If]
	if(__NFUN_150__(Index, InteractionArray.Length))
	{
		// End:0x71
		if(__NFUN_130__(__NFUN_130__(InteractionArray[Index].bActive, __NFUN_129__(InteractionArray[Index].bNativeEvents)), InteractionArray[Index].KeyType(Key, Unicode)))
		{
			return true;
		}
		__NFUN_165__(Index);
		// [Loop Continue]
		goto J0x07;
	}
	return false;
	return;
}

event bool Process_KeyEvent(array<Interaction> InteractionArray, out Interactions.EInputKey Key, out Interactions.EInputAction Action, float Delta)
{
	local int Index;

	Index = 0;
	J0x07:

	// End:0x80 [Loop If]
	if(__NFUN_150__(Index, InteractionArray.Length))
	{
		// End:0x76
		if(__NFUN_130__(__NFUN_130__(InteractionArray[Index].bActive, __NFUN_129__(InteractionArray[Index].bNativeEvents)), InteractionArray[Index].KeyEvent(Key, Action, Delta)))
		{
			return true;
		}
		__NFUN_165__(Index);
		// [Loop Continue]
		goto J0x07;
	}
	return false;
	return;
}

event Process_PreRender(array<Interaction> InteractionArray, Canvas Canvas)
{
	local int Index;

	Index = InteractionArray.Length;
	J0x0C:

	// End:0x78 [Loop If]
	if(__NFUN_151__(Index, 0))
	{
		// End:0x6E
		if(__NFUN_130__(InteractionArray[__NFUN_147__(Index, 1)].bVisible, __NFUN_129__(InteractionArray[__NFUN_147__(Index, 1)].bNativeEvents)))
		{
			InteractionArray[__NFUN_147__(Index, 1)].PreRender(Canvas);
		}
		__NFUN_166__(Index);
		// [Loop Continue]
		goto J0x0C;
	}
	return;
}

event Process_PostRender(array<Interaction> InteractionArray, Canvas Canvas)
{
	local int Index;

	Index = InteractionArray.Length;
	J0x0C:

	// End:0x78 [Loop If]
	if(__NFUN_151__(Index, 0))
	{
		// End:0x6E
		if(__NFUN_130__(InteractionArray[__NFUN_147__(Index, 1)].bVisible, __NFUN_129__(InteractionArray[__NFUN_147__(Index, 1)].bNativeEvents)))
		{
			InteractionArray[__NFUN_147__(Index, 1)].PostRender(Canvas);
		}
		__NFUN_166__(Index);
		// [Loop Continue]
		goto J0x0C;
	}
	return;
}

event Process_Tick(array<Interaction> InteractionArray, float DeltaTime)
{
	local int Index;

	Index = 0;
	J0x07:

	// End:0x6F [Loop If]
	if(__NFUN_150__(Index, InteractionArray.Length))
	{
		// End:0x65
		if(__NFUN_130__(InteractionArray[Index].bRequiresTick, __NFUN_129__(InteractionArray[Index].bNativeEvents)))
		{
			InteractionArray[Index].Tick(DeltaTime);
		}
		__NFUN_165__(Index);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

event Process_Message(coerce string Msg, float MsgLife, array<Interaction> InteractionArray)
{
	local int Index;

	Index = 0;
	J0x07:

	// End:0x40 [Loop If]
	if(__NFUN_150__(Index, InteractionArray.Length))
	{
		InteractionArray[Index].Message(Msg, MsgLife);
		__NFUN_165__(Index);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}
