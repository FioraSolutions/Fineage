class RadarContainerWnd extends UIScript;

function OnLoad()
{
	RegisterEvent(1730);
	RegisterEvent(1740);
	RegisterEvent(1750);
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 1730:
			HandleRadarAddTarget(a_Param);
			// End:0x47
			break;
		// End:0x33
		case 1740:
			HandleRadarDeleteTarget(a_Param);
			// End:0x47
			break;
		// End:0x44
		case 1750:
			HandleRadarDeleteAllTarget();
			// End:0x47
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleRadarAddTarget(string a_Param)
{
	local int X, Y, Z;

	// End:0x7E
	if(__NFUN_130__(__NFUN_130__(ParseInt(a_Param, "X", X), ParseInt(a_Param, "Y", Y)), ParseInt(a_Param, "Z", Z)))
	{
		Class'NWindow.UIAPI_RADAR'.static.AddTarget("RadarContainterWnd.Radar", X, Y, Z);
	}
	return;
}

function HandleRadarDeleteTarget(string a_Param)
{
	local int X, Y, Z;

	// End:0x7E
	if(__NFUN_130__(__NFUN_130__(ParseInt(a_Param, "X", X), ParseInt(a_Param, "Y", Y)), ParseInt(a_Param, "Z", Z)))
	{
		Class'NWindow.UIAPI_RADAR'.static.DeleteTarget("RadarContainterWnd.Radar", X, Y, Z);
	}
	return;
}

function HandleRadarDeleteAllTarget()
{
	Class'NWindow.UIAPI_RADAR'.static.DeleteAllTarget("RadarContainterWnd.Radar");
	return;
}
