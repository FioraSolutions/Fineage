class UWindowTabControlTabArea extends UWindowWindow
	config;

var int TabOffset;
var bool bShowSelected;
var UWindowTabControlItem FirstShown;
var bool bDragging;
var UWindowTabControlItem DragTab;
var int TabRows;
var globalconfig bool bArrangeRowsLikeTimHates;
var float UnFlashTime;
var bool bFlashShown;

function Created()
{
	TabOffset = 0;
	super.Created();
	return;
}

function SizeTabsSingleLine(Canvas C)
{
	local UWindowTabControlItem i, Selected, LastHidden;
	local int Count, TabCount;
	local float ItemX, W, h;
	local bool bHaveMore;

	ItemX = LookAndFeel.Size_TabXOffset;
	TabCount = 0;
	i = UWindowTabControlItem(UWindowTabControl(ParentWindow).Items.Next);
	J0x42:

	// End:0xF4 [Loop If]
	if(__NFUN_119__(i, none))
	{
		LookAndFeel.Tab_GetTabSize(self, C, RemoveAmpersand(i.Caption), W, h);
		i.TabWidth = W;
		i.TabHeight = __NFUN_174__(h, float(1));
		i.TabTop = 0.0000000;
		i.RowNumber = 0;
		__NFUN_165__(TabCount);
		i = UWindowTabControlItem(i.Next);
		// [Loop Continue]
		goto J0x42;
	}
	Selected = UWindowTabControl(ParentWindow).SelectedTab;
	J0x10D:

	// End:0x2F9 [Loop If]
	if(true)
	{
		ItemX = LookAndFeel.Size_TabXOffset;
		Count = 0;
		LastHidden = none;
		FirstShown = none;
		i = UWindowTabControlItem(UWindowTabControl(ParentWindow).Items.Next);
		J0x161:

		// End:0x235 [Loop If]
		if(__NFUN_119__(i, none))
		{
			// End:0x19D
			if(__NFUN_150__(Count, TabOffset))
			{
				i.TabLeft = -1.0000000;
				LastHidden = i;				
			}
			else
			{
				// End:0x1B3
				if(__NFUN_114__(FirstShown, none))
				{
					FirstShown = i;
				}
				i.TabLeft = ItemX;
				// End:0x1FD
				if(__NFUN_179__(__NFUN_174__(i.TabLeft, i.TabWidth), __NFUN_174__(WinWidth, float(5))))
				{
					bHaveMore = true;
				}
				__NFUN_184__(ItemX, i.TabWidth);
			}
			__NFUN_165__(Count);
			i = UWindowTabControlItem(i.Next);
			// [Loop Continue]
			goto J0x161;
		}
		// End:0x27E
		if(__NFUN_130__(__NFUN_130__(__NFUN_151__(TabOffset, 0), __NFUN_119__(LastHidden, none)), __NFUN_176__(__NFUN_174__(LastHidden.TabWidth, float(5)), __NFUN_175__(WinWidth, ItemX))))
		{
			__NFUN_166__(TabOffset);			
		}
		else
		{
			// End:0x2F3
			if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(bShowSelected, __NFUN_150__(TabOffset, __NFUN_147__(TabCount, 1))), __NFUN_119__(Selected, none)), __NFUN_119__(Selected, FirstShown)), __NFUN_177__(__NFUN_174__(Selected.TabLeft, Selected.TabWidth), __NFUN_175__(WinWidth, float(5)))))
			{
				__NFUN_165__(TabOffset);				
			}
			else
			{
				// [Explicit Break]
				goto J0x2F9;
			}
		}
		// [Loop Continue]
		goto J0x10D;
	}
	J0x2F9:

	bShowSelected = false;
	UWindowTabControl(ParentWindow).LeftButton.bDisabled = __NFUN_152__(TabOffset, 0);
	UWindowTabControl(ParentWindow).RightButton.bDisabled = __NFUN_129__(bHaveMore);
	TabRows = 1;
	return;
}

function SizeTabsMultiLine(Canvas C)
{
	local UWindowTabControlItem i, Selected;
	local float W, h;
	local int MinRow;
	local float RowWidths[10];
	local int TabCounts[10], j;
	local bool bTryAnotherRow;

	TabOffset = 0;
	FirstShown = none;
	TabRows = 1;
	bTryAnotherRow = true;
	J0x1D:

	// End:0x1D7 [Loop If]
	if(__NFUN_130__(bTryAnotherRow, __NFUN_152__(TabRows, 10)))
	{
		bTryAnotherRow = false;
		j = 0;
		J0x43:

		// End:0x7A [Loop If]
		if(__NFUN_150__(j, TabRows))
		{
			RowWidths[j] = 0.0000000;
			TabCounts[j] = 0;
			__NFUN_165__(j);
			// [Loop Continue]
			goto J0x43;
		}
		i = UWindowTabControlItem(UWindowTabControl(ParentWindow).Items.Next);
		J0xA1:

		// End:0x1D4 [Loop If]
		if(__NFUN_119__(i, none))
		{
			LookAndFeel.Tab_GetTabSize(self, C, RemoveAmpersand(i.Caption), W, h);
			i.TabWidth = W;
			i.TabHeight = h;
			MinRow = 0;
			j = 1;
			J0x115:

			// End:0x154 [Loop If]
			if(__NFUN_150__(j, TabRows))
			{
				// End:0x14A
				if(__NFUN_176__(RowWidths[j], RowWidths[MinRow]))
				{
					MinRow = j;
				}
				__NFUN_165__(j);
				// [Loop Continue]
				goto J0x115;
			}
			// End:0x185
			if(__NFUN_177__(__NFUN_174__(RowWidths[MinRow], W), WinWidth))
			{
				__NFUN_165__(TabRows);
				bTryAnotherRow = true;
				// [Explicit Break]
				goto J0x1D4;				
			}
			else
			{
				__NFUN_184__(RowWidths[MinRow], W);
				__NFUN_165__(TabCounts[MinRow]);
				i.RowNumber = MinRow;
			}
			i = UWindowTabControlItem(i.Next);
			// [Loop Continue]
			goto J0xA1;
		}
		J0x1D4:

		// [Loop Continue]
		goto J0x1D;
	}
	Selected = UWindowTabControl(ParentWindow).SelectedTab;
	// End:0x28C
	if(__NFUN_151__(TabRows, 1))
	{
		i = UWindowTabControlItem(UWindowTabControl(ParentWindow).Items.Next);
		J0x222:

		// End:0x28C [Loop If]
		if(__NFUN_119__(i, none))
		{
			__NFUN_184__(i.TabWidth, __NFUN_172__(__NFUN_175__(WinWidth, RowWidths[i.RowNumber]), float(TabCounts[i.RowNumber])));
			i = UWindowTabControlItem(i.Next);
			// [Loop Continue]
			goto J0x222;
		}
	}
	j = 0;
	J0x293:

	// End:0x2BD [Loop If]
	if(__NFUN_150__(j, TabRows))
	{
		RowWidths[j] = 0.0000000;
		__NFUN_165__(j);
		// [Loop Continue]
		goto J0x293;
	}
	i = UWindowTabControlItem(UWindowTabControl(ParentWindow).Items.Next);
	J0x2E4:

	// End:0x3DF [Loop If]
	if(__NFUN_119__(i, none))
	{
		i.TabLeft = RowWidths[i.RowNumber];
		// End:0x370
		if(bArrangeRowsLikeTimHates)
		{
			i.TabTop = __NFUN_171__(__NFUN_173__(float(__NFUN_146__(i.RowNumber, __NFUN_147__(__NFUN_147__(TabRows, 1), Selected.RowNumber))), float(TabRows)), i.TabHeight);			
		}
		else
		{
			i.TabTop = __NFUN_171__(float(i.RowNumber), i.TabHeight);
		}
		__NFUN_184__(RowWidths[i.RowNumber], i.TabWidth);
		i = UWindowTabControlItem(i.Next);
		// [Loop Continue]
		goto J0x2E4;
	}
	return;
}

function LayoutTabs(Canvas C)
{
	// End:0x25
	if(UWindowTabControl(ParentWindow).bMultiLine)
	{
		SizeTabsMultiLine(C);		
	}
	else
	{
		SizeTabsSingleLine(C);
	}
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local UWindowTabControlItem i;
	local int Count, Row;
	local float t;

	t = GetEntryLevel().TimeSeconds;
	// End:0x63
	if(__NFUN_176__(UnFlashTime, t))
	{
		bFlashShown = __NFUN_129__(bFlashShown);
		// End:0x51
		if(bFlashShown)
		{
			UnFlashTime = __NFUN_174__(t, 0.5000000);			
		}
		else
		{
			UnFlashTime = __NFUN_174__(t, 0.3000000);
		}
	}
	Row = 0;
	J0x6A:

	// End:0x16D [Loop If]
	if(__NFUN_150__(Row, TabRows))
	{
		Count = 0;
		i = UWindowTabControlItem(UWindowTabControl(ParentWindow).Items.Next);
		J0xA7:

		// End:0x163 [Loop If]
		if(__NFUN_119__(i, none))
		{
			// End:0xCB
			if(__NFUN_150__(Count, TabOffset))
			{
				__NFUN_165__(Count);				
			}
			else
			{
				// End:0x147
				if(__NFUN_154__(i.RowNumber, Row))
				{
					DrawItem(C, i, i.TabLeft, i.TabTop, i.TabWidth, i.TabHeight, __NFUN_132__(__NFUN_129__(i.bFlash), bFlashShown));
				}
			}
			i = UWindowTabControlItem(i.Next);
			// [Loop Continue]
			goto J0xA7;
		}
		__NFUN_165__(Row);
		// [Loop Continue]
		goto J0x6A;
	}
	return;
}

function LMouseDown(float X, float Y)
{
	local UWindowTabControlItem i;
	local int Count;

	super.LMouseDown(X, Y);
	Count = 0;
	i = UWindowTabControlItem(UWindowTabControl(ParentWindow).Items.Next);
	J0x3E:

	// End:0x166 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x62
		if(__NFUN_150__(Count, TabOffset))
		{
			__NFUN_165__(Count);			
		}
		else
		{
			// End:0x14A
			if(__NFUN_130__(__NFUN_130__(__NFUN_179__(X, i.TabLeft), __NFUN_178__(X, __NFUN_174__(i.TabLeft, i.TabWidth))), __NFUN_132__(__NFUN_154__(TabRows, 1), __NFUN_130__(__NFUN_179__(Y, i.TabTop), __NFUN_178__(Y, __NFUN_174__(i.TabTop, i.TabHeight))))))
			{
				// End:0x130
				if(__NFUN_129__(UWindowTabControl(ParentWindow).bMultiLine))
				{
					bDragging = true;
					DragTab = i;
					Root.CaptureMouse();
				}
				UWindowTabControl(ParentWindow).GotoTab(i, true);
			}
		}
		i = UWindowTabControlItem(i.Next);
		// [Loop Continue]
		goto J0x3E;
	}
	return;
}

function MouseMove(float X, float Y)
{
	// End:0x72
	if(__NFUN_130__(bDragging, bMouseDown))
	{
		// End:0x33
		if(__NFUN_176__(X, DragTab.TabLeft))
		{
			__NFUN_165__(TabOffset);
		}
		// End:0x6F
		if(__NFUN_130__(__NFUN_177__(X, __NFUN_174__(DragTab.TabLeft, DragTab.TabWidth)), __NFUN_151__(TabOffset, 0)))
		{
			__NFUN_166__(TabOffset);
		}		
	}
	else
	{
		bDragging = false;
	}
	return;
}

function RMouseDown(float X, float Y)
{
	local UWindowTabControlItem i;
	local int Count;

	LMouseDown(X, Y);
	Count = 0;
	i = UWindowTabControlItem(UWindowTabControl(ParentWindow).Items.Next);
	J0x3E:

	// End:0xCF [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x62
		if(__NFUN_150__(Count, TabOffset))
		{
			__NFUN_165__(Count);			
		}
		else
		{
			// End:0xB3
			if(__NFUN_130__(__NFUN_179__(X, i.TabLeft), __NFUN_178__(X, __NFUN_174__(i.TabLeft, i.TabWidth))))
			{
				i.RightClickTab();
			}
		}
		i = UWindowTabControlItem(i.Next);
		// [Loop Continue]
		goto J0x3E;
	}
	return;
}

function DrawItem(Canvas C, UWindowList item, float X, float Y, float W, float h, bool bShowText)
{
	// End:0x6F
	if(__NFUN_114__(item, UWindowTabControl(ParentWindow).SelectedTab))
	{
		LookAndFeel.Tab_DrawTab(self, C, true, __NFUN_114__(FirstShown, item), X, Y, W, h, UWindowTabControlItem(item).Caption, bShowText);		
	}
	else
	{
		LookAndFeel.Tab_DrawTab(self, C, false, __NFUN_114__(FirstShown, item), X, Y, W, h, UWindowTabControlItem(item).Caption, bShowText);
	}
	return;
}

function bool CheckMousePassThrough(float X, float Y)
{
	return __NFUN_179__(Y, __NFUN_171__(LookAndFeel.Size_TabAreaHeight, float(TabRows)));
	return;
}
