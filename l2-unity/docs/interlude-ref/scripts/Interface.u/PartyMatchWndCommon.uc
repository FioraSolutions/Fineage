class PartyMatchWndCommon extends UIScript;

function string GetAmbiguousLevelString(int a_Level, bool a_HasSpace)
{
	local string AmbiguousLevelString;

	// End:0x33
	if(__NFUN_151__(10, a_Level))
	{
		// End:0x25
		if(a_HasSpace)
		{
			AmbiguousLevelString = "1 ~ 9";			
		}
		else
		{
			AmbiguousLevelString = "1~9";
		}		
	}
	else
	{
		// End:0xAE
		if(__NFUN_151__(70, a_Level))
		{
			// End:0x7C
			if(a_HasSpace)
			{
				AmbiguousLevelString = __NFUN_112__(__NFUN_112__(string(__NFUN_144__(__NFUN_145__(a_Level, 10), 10)), " ~ "), string(__NFUN_146__(__NFUN_144__(__NFUN_145__(a_Level, 10), 10), 9)));				
			}
			else
			{
				AmbiguousLevelString = __NFUN_112__(__NFUN_112__(string(__NFUN_144__(__NFUN_145__(a_Level, 10), 10)), "~"), string(__NFUN_146__(__NFUN_144__(__NFUN_145__(a_Level, 10), 10), 9)));
			}			
		}
		else
		{
			// End:0xCD
			if(a_HasSpace)
			{
				AmbiguousLevelString = __NFUN_112__("70 ~ ", string(80));				
			}
			else
			{
				AmbiguousLevelString = __NFUN_112__("70~", string(80));
			}
		}
	}
	return AmbiguousLevelString;
	return;
}
