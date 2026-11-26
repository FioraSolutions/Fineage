class UWindowLayoutControl extends UWindowLayoutBase;

var UWindowDialogClientWindow OwnerWindow;
var float WinTop;
var float WinLeft;
var float WinWidth;
var float WinHeight;
var float MinimumWidth;
var float MinimumHeight;
var UWindowLayoutRow RowList;

static function UWindowLayoutControl Create()
{
	local UWindowLayoutControl C;

	C = new Class'UWindow.UWindowLayoutControl';
	C.RowList = new Class'UWindow.UWindowLayoutRow';
	C.RowList.SetupSentinel();
	return C;
	return;
}

function PerformLayout()
{
	local UWindowLayoutRow R;
	local float TotalWidth, TotalHeight;

	R = UWindowLayoutRow(RowList.Next);
	J0x19:

	// End:0x56 [Loop If]
	if(__NFUN_119__(R, none))
	{
		__NFUN_184__(TotalHeight, R.CalcMinHeight());
		R = UWindowLayoutRow(R.Next);
		// [Loop Continue]
		goto J0x19;
	}
	__NFUN_184__(TotalWidth, R.CalcMinHeight());
	return;
}

function UWindowLayoutRow AddRow()
{
	return UWindowLayoutRow(RowList.Append(Class'UWindow.UWindowLayoutRow'));
	return;
}

function UWindowLayoutCell AddCell(optional int ColSpan, optional int RowSpan)
{
	return RowList.AddCell(ColSpan, RowSpan);
	return;
}
