class UWindowHotkeyWindowList extends UWindowList;

var UWindowWindow Window;

function UWindowHotkeyWindowList FindWindow(UWindowWindow W)
{
	local UWindowHotkeyWindowList L;

	L = UWindowHotkeyWindowList(Next);
	J0x10:

	// End:0x55 [Loop If]
	if(__NFUN_119__(L, none))
	{
		// End:0x39
		if(__NFUN_114__(L.Window, W))
		{
			return L;
		}
		L = UWindowHotkeyWindowList(L.Next);
		// [Loop Continue]
		goto J0x10;
	}
	return none;
	return;
}
