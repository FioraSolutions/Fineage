class MapList extends Info
	abstract
	config
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var(Maps) config array<string> Maps;
var config int MapNum;

function string GetNextMap()
{
	local string CurrentMap;
	local int i;

	CurrentMap = __NFUN_547__();
	// End:0x8E
	if(__NFUN_123__(CurrentMap, ""))
	{
		// End:0x37
		if(__NFUN_124__(__NFUN_234__(CurrentMap, 4), ".unr"))
		{
			CurrentMap = CurrentMap;			
		}
		else
		{
			CurrentMap = __NFUN_112__(CurrentMap, ".unr");
		}
		i = 0;
		J0x51:

		// End:0x8E [Loop If]
		if(__NFUN_150__(i, Maps.Length))
		{
			// End:0x84
			if(__NFUN_124__(CurrentMap, Maps[i]))
			{
				MapNum = i;
				// [Explicit Break]
				goto J0x8E;
			}
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x51;
		}
	}
	J0x8E:

	__NFUN_165__(MapNum);
	// End:0xAF
	if(__NFUN_151__(MapNum, __NFUN_147__(Maps.Length, 1)))
	{
		MapNum = 0;
	}
	// End:0xC8
	if(__NFUN_122__(Maps[MapNum], ""))
	{
		MapNum = 0;
	}
	__NFUN_536__();
	return Maps[MapNum];
	return;
}
