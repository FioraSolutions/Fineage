class UWindowLayoutRow extends UWindowLayoutBase;

var UWindowLayoutCell CellList;

function SetupSentinel(optional bool bInTreeSort)
{
	super(UWindowList).SetupSentinel(bInTreeSort);
	CellList = new Class'UWindow.UWindowLayoutCell';
	CellList.SetupSentinel();
	return;
}

function UWindowLayoutCell AddCell(optional int ColSpan, optional int RowSpan)
{
	local UWindowLayoutCell C;

	C = UWindowLayoutCell(CellList.Append(Class'UWindow.UWindowLayoutCell'));
	C.ColSpan = ColSpan;
	C.RowSpan = RowSpan;
	return C;
	return;
}

function float CalcMinHeight()
{
	return 0.0000000;
	return;
}
