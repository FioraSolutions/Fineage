class UWindowClientWindow extends UWindowWindow;

function Close(optional bool bByParent)
{
	// End:0x20
	if(__NFUN_129__(bByParent))
	{
		ParentWindow.Close(bByParent);
	}
	super.Close(bByParent);
	return;
}
