class UWindowComboListItem extends UWindowList;

var string Value;
var string Value2;
var int SortWeight;
var float ItemTop;

function int Compare(UWindowList t, UWindowList B)
{
	local UWindowComboListItem TI, BI;
	local string TS, BS;

	TI = UWindowComboListItem(t);
	BI = UWindowComboListItem(B);
	// End:0x98
	if(__NFUN_154__(TI.SortWeight, BI.SortWeight))
	{
		TS = __NFUN_235__(TI.Value);
		BS = __NFUN_235__(BI.Value);
		// End:0x7E
		if(__NFUN_122__(TS, BS))
		{
			return 0;
		}
		// End:0x93
		if(__NFUN_115__(TS, BS))
		{
			return -1;
		}
		return 1;		
	}
	else
	{
		return __NFUN_147__(TI.SortWeight, BI.SortWeight);
	}
	return;
}
