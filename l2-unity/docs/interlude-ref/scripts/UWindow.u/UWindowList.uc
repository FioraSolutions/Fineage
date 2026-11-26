class UWindowList extends UWindowBase;

var UWindowList Next;
var UWindowList Last;
var UWindowList Prev;
var UWindowList Sentinel;
var int InternalCount;
var bool bItemOrderChanged;
var bool bSuspendableSort;
var int CompareCount;
var bool bSortSuspended;
var UWindowList CurrentSortItem;
var bool bTreeSort;
var UWindowList BranchLeft;
var UWindowList BranchRight;
var UWindowList ParentNode;
var int Tag;

function UWindowList CreateItem(Class<UWindowList> C)
{
	local UWindowList NewElement;

	NewElement = new C;
	return NewElement;
	return;
}

function GraftLeft(UWindowList NewLeft)
{
	assert(Sentinel.bTreeSort);
	BranchLeft = NewLeft;
	// End:0x38
	if(__NFUN_119__(NewLeft, none))
	{
		NewLeft.ParentNode = self;
	}
	return;
}

function GraftRight(UWindowList NewRight)
{
	assert(Sentinel.bTreeSort);
	BranchRight = NewRight;
	// End:0x38
	if(__NFUN_119__(NewRight, none))
	{
		NewRight.ParentNode = self;
	}
	return;
}

function UWindowList RightMost()
{
	local UWindowList L;

	assert(Sentinel.bTreeSort);
	// End:0x1F
	if(__NFUN_114__(BranchRight, none))
	{
		return none;
	}
	L = self;
	J0x26:

	// End:0x51 [Loop If]
	if(__NFUN_119__(L.BranchRight, none))
	{
		L = L.BranchRight;
		// [Loop Continue]
		goto J0x26;
	}
	return L;
	return;
}

function UWindowList LeftMost()
{
	local UWindowList L;

	assert(Sentinel.bTreeSort);
	// End:0x1F
	if(__NFUN_114__(BranchLeft, none))
	{
		return none;
	}
	L = self;
	J0x26:

	// End:0x51 [Loop If]
	if(__NFUN_119__(L.BranchLeft, none))
	{
		L = L.BranchLeft;
		// [Loop Continue]
		goto J0x26;
	}
	return L;
	return;
}

function Remove()
{
	local UWindowList t;

	// End:0x1F
	if(__NFUN_119__(Next, none))
	{
		Next.Prev = Prev;
	}
	// End:0x3E
	if(__NFUN_119__(Prev, none))
	{
		Prev.Next = Next;
	}
	// End:0x1BD
	if(__NFUN_119__(Sentinel, none))
	{
		// End:0x15F
		if(__NFUN_130__(Sentinel.bTreeSort, __NFUN_119__(ParentNode, none)))
		{
			// End:0xFA
			if(__NFUN_119__(BranchLeft, none))
			{
				// End:0x9B
				if(__NFUN_114__(ParentNode.BranchLeft, self))
				{
					ParentNode.GraftLeft(BranchLeft);
				}
				// End:0xC3
				if(__NFUN_114__(ParentNode.BranchRight, self))
				{
					ParentNode.GraftRight(BranchLeft);
				}
				t = BranchLeft.RightMost();
				// End:0xF7
				if(__NFUN_119__(t, none))
				{
					t.GraftRight(BranchRight);
				}				
			}
			else
			{
				// End:0x122
				if(__NFUN_114__(ParentNode.BranchLeft, self))
				{
					ParentNode.GraftLeft(BranchRight);
				}
				// End:0x14A
				if(__NFUN_114__(ParentNode.BranchRight, self))
				{
					ParentNode.GraftRight(BranchRight);
				}
			}
			ParentNode = none;
			BranchLeft = none;
			BranchRight = none;
		}
		__NFUN_166__(Sentinel.InternalCount);
		Sentinel.bItemOrderChanged = true;
		// End:0x1A8
		if(__NFUN_114__(Sentinel.Last, self))
		{
			Sentinel.Last = Prev;
		}
		Prev = none;
		Next = none;
		Sentinel = none;
	}
	return;
}

function int Compare(UWindowList t, UWindowList B)
{
	return 0;
	return;
}

function UWindowList InsertBefore(Class<UWindowList> C)
{
	local UWindowList NewElement;

	NewElement = CreateItem(C);
	InsertItemBefore(NewElement);
	return NewElement;
	return;
}

function UWindowList InsertAfter(Class<UWindowList> C)
{
	local UWindowList NewElement;

	NewElement = CreateItem(C);
	InsertItemAfter(NewElement);
	return NewElement;
	return;
}

function InsertItemBefore(UWindowList NewElement)
{
	assert(__NFUN_119__(Sentinel, self));
	NewElement.BranchLeft = none;
	NewElement.BranchRight = none;
	NewElement.ParentNode = none;
	NewElement.Sentinel = Sentinel;
	NewElement.BranchLeft = none;
	NewElement.BranchRight = none;
	NewElement.ParentNode = none;
	NewElement.Prev = Prev;
	Prev.Next = NewElement;
	Prev = NewElement;
	NewElement.Next = self;
	// End:0xEA
	if(__NFUN_114__(Sentinel.Next, self))
	{
		Sentinel.Next = NewElement;
	}
	__NFUN_165__(Sentinel.InternalCount);
	Sentinel.bItemOrderChanged = true;
	return;
}

function InsertItemAfter(UWindowList NewElement, optional bool bCheckShowItem)
{
	local UWindowList N;

	N = Next;
	// End:0x4C
	if(bCheckShowItem)
	{
		J0x14:

		// End:0x4C [Loop If]
		if(__NFUN_130__(__NFUN_119__(N, none), __NFUN_129__(N.ShowThisItem())))
		{
			N = N.Next;
			// [Loop Continue]
			goto J0x14;
		}
	}
	// End:0x6E
	if(__NFUN_119__(N, none))
	{
		N.InsertItemBefore(NewElement);		
	}
	else
	{
		Sentinel.DoAppendItem(NewElement);
	}
	Sentinel.bItemOrderChanged = true;
	return;
}

function ContinueSort()
{
	local UWindowList N;

	CompareCount = 0;
	bSortSuspended = false;
	J0x0F:

	// End:0x6B [Loop If]
	if(__NFUN_119__(CurrentSortItem, none))
	{
		N = CurrentSortItem.Next;
		AppendItem(CurrentSortItem);
		CurrentSortItem = N;
		// End:0x68
		if(__NFUN_130__(__NFUN_153__(CompareCount, 10000), bSuspendableSort))
		{
			bSortSuspended = true;
			return;
		}
		// [Loop Continue]
		goto J0x0F;
	}
	return;
}

function Tick(float Delta)
{
	// End:0x0F
	if(bSortSuspended)
	{
		ContinueSort();
	}
	return;
}

function UWindowList Sort()
{
	local UWindowList S, CurrentItem, Previous, Best, BestPrev;

	// End:0x33
	if(bTreeSort)
	{
		// End:0x1A
		if(bSortSuspended)
		{
			ContinueSort();
			return self;
		}
		CurrentSortItem = Next;
		DisconnectList();
		ContinueSort();
		return self;
	}
	CurrentItem = self;
	J0x3A:

	// End:0x218 [Loop If]
	if(__NFUN_119__(CurrentItem, none))
	{
		S = CurrentItem.Next;
		Best = CurrentItem.Next;
		Previous = CurrentItem;
		BestPrev = CurrentItem;
		J0x83:

		// End:0xE5 [Loop If]
		if(__NFUN_119__(S, none))
		{
			// End:0xC3
			if(__NFUN_152__(CurrentItem.Compare(S, Best), 0))
			{
				Best = S;
				BestPrev = Previous;
			}
			Previous = S;
			S = S.Next;
			// [Loop Continue]
			goto J0x83;
		}
		// End:0x201
		if(__NFUN_119__(Best, CurrentItem.Next))
		{
			BestPrev.Next = Best.Next;
			// End:0x14B
			if(__NFUN_119__(BestPrev.Next, none))
			{
				BestPrev.Next.Prev = BestPrev;
			}
			Best.Prev = CurrentItem;
			Best.Next = CurrentItem.Next;
			CurrentItem.Next.Prev = Best;
			CurrentItem.Next = Best;
			// End:0x201
			if(__NFUN_114__(Sentinel.Last, Best))
			{
				Sentinel.Last = BestPrev;
				// End:0x201
				if(__NFUN_114__(Sentinel.Last, none))
				{
					Sentinel.Last = Sentinel;
				}
			}
		}
		CurrentItem = CurrentItem.Next;
		// [Loop Continue]
		goto J0x3A;
	}
	return self;
	return;
}

function DisconnectList()
{
	Next = none;
	Last = self;
	Prev = none;
	BranchLeft = none;
	BranchRight = none;
	ParentNode = none;
	InternalCount = 0;
	Sentinel.bItemOrderChanged = true;
	return;
}

function DestroyList()
{
	local UWindowList L, temp;

	L = Next;
	InternalCount = 0;
	// End:0x2E
	if(__NFUN_119__(Sentinel, none))
	{
		Sentinel.bItemOrderChanged = true;
	}
	J0x2E:

	// End:0x6A [Loop If]
	if(__NFUN_119__(L, none))
	{
		temp = L.Next;
		L.DestroyListItem();
		L = temp;
		// [Loop Continue]
		goto J0x2E;
	}
	DestroyListItem();
	return;
}

function DestroyListItem()
{
	Next = none;
	Last = self;
	Sentinel = none;
	Prev = none;
	BranchLeft = none;
	BranchRight = none;
	ParentNode = none;
	return;
}

function int CountShown()
{
	local int C;
	local UWindowList i;

	i = Next;
	J0x0B:

	// End:0x46 [Loop If]
	if(__NFUN_119__(i, none))
	{
		// End:0x2F
		if(i.ShowThisItem())
		{
			__NFUN_165__(C);
		}
		i = i.Next;
		// [Loop Continue]
		goto J0x0B;
	}
	return C;
	return;
}

function UWindowList CopyExistingListItem(Class<UWindowList> ItemClass, UWindowList SourceItem)
{
	local UWindowList i;

	i = Append(ItemClass);
	Sentinel.bItemOrderChanged = true;
	return i;
	return;
}

function bool ShowThisItem()
{
	return true;
	return;
}

function int Count()
{
	return InternalCount;
	return;
}

function MoveItemSorted(UWindowList item)
{
	local UWindowList L;

	// End:0x26
	if(bTreeSort)
	{
		item.Remove();
		AppendItem(item);		
	}
	else
	{
		L = Next;
		J0x31:

		// End:0x6C [Loop If]
		if(__NFUN_119__(L, none))
		{
			// End:0x55
			if(__NFUN_152__(Compare(item, L), 0))
			{
				// [Explicit Break]
				goto J0x6C;
			}
			L = L.Next;
			// [Loop Continue]
			goto J0x31;
		}
		J0x6C:

		// End:0xB7
		if(__NFUN_119__(L, item))
		{
			item.Remove();
			// End:0xA3
			if(__NFUN_114__(L, none))
			{
				AppendItem(item);				
			}
			else
			{
				L.InsertItemBefore(item);
			}
		}
	}
	return;
}

function SetupSentinel(optional bool bInTreeSort)
{
	Last = self;
	Next = none;
	Prev = none;
	BranchLeft = none;
	BranchRight = none;
	ParentNode = none;
	Sentinel = self;
	InternalCount = 0;
	bItemOrderChanged = true;
	bTreeSort = bInTreeSort;
	return;
}

function Validate()
{
	local UWindowList i, Previous;
	local int Count;

	// End:0x46
	if(__NFUN_119__(Sentinel, self))
	{
		__NFUN_231__(__NFUN_112__("Calling Sentinel.Validate() from ", string(self)));
		Sentinel.Validate();
		return;
	}
	__NFUN_231__(__NFUN_112__("BEGIN Validate(): ", string(Class)));
	Count = 0;
	Previous = self;
	i = Next;
	J0x7E:

	// End:0x1FD [Loop If]
	if(__NFUN_119__(i, none))
	{
		__NFUN_231__(__NFUN_112__("Checking item: ", string(Count)));
		// End:0xDE
		if(__NFUN_119__(i.Sentinel, self))
		{
			__NFUN_231__("   I.Sentinel reference is broken");
		}
		// End:0x117
		if(__NFUN_119__(i.Prev, Previous))
		{
			__NFUN_231__("   I.Prev reference is broken");
		}
		// End:0x170
		if(__NFUN_130__(__NFUN_114__(Last, i), __NFUN_119__(i.Next, none)))
		{
			__NFUN_231__("   Item is Sentinel.Last but Item has valid Next");
		}
		// End:0x1D4
		if(__NFUN_130__(__NFUN_114__(i.Next, none), __NFUN_119__(Last, i)))
		{
			__NFUN_231__("   Item is Item.Next is none, but Item is not Sentinel.Last");
		}
		Previous = i;
		__NFUN_165__(Count);
		i = i.Next;
		// [Loop Continue]
		goto J0x7E;
	}
	__NFUN_231__(__NFUN_112__("END Validate(): ", string(Class)));
	return;
}

function UWindowList Append(Class<UWindowList> C)
{
	local UWindowList NewElement;

	NewElement = CreateItem(C);
	AppendItem(NewElement);
	return NewElement;
	return;
}

function AppendItem(UWindowList NewElement)
{
	local UWindowList Node, OldNode, temp;
	local int test;

	// End:0x225
	if(bTreeSort)
	{
		// End:0xB8
		if(__NFUN_130__(__NFUN_119__(Next, none), __NFUN_119__(Last, self)))
		{
			// End:0x6D
			if(__NFUN_153__(Compare(NewElement, Last), 0))
			{
				Node = Last;
				Node.InsertItemAfter(NewElement, false);
				Node.GraftRight(NewElement);
				return;
			}
			// End:0xB8
			if(__NFUN_152__(Compare(NewElement, Next), 0))
			{
				Node = Next;
				Node.InsertItemBefore(NewElement);
				Node.GraftLeft(NewElement);
				return;
			}
		}
		Node = self;
		J0xBF:

		// End:0x222 [Loop If]
		if(true)
		{
			// End:0xD8
			if(__NFUN_114__(Node, self))
			{
				test = 1;				
			}
			else
			{
				test = Compare(NewElement, Node);
			}
			// End:0x113
			if(__NFUN_154__(test, 0))
			{
				Node.InsertItemAfter(NewElement, false);
				return;				
			}
			else
			{
				// End:0x1CB
				if(__NFUN_151__(test, 0))
				{
					OldNode = Node;
					Node = Node.BranchRight;
					// End:0x1C8
					if(__NFUN_114__(Node, none))
					{
						temp = OldNode;
						J0x153:

						// End:0x19D [Loop If]
						if(__NFUN_130__(__NFUN_119__(temp.Next, none), __NFUN_114__(temp.Next.ParentNode, none)))
						{
							temp = temp.Next;
							// [Loop Continue]
							goto J0x153;
						}
						temp.InsertItemAfter(NewElement, false);
						OldNode.GraftRight(NewElement);
						return;
					}					
				}
				else
				{
					OldNode = Node;
					Node = Node.BranchLeft;
					// End:0x21F
					if(__NFUN_114__(Node, none))
					{
						OldNode.InsertItemBefore(NewElement);
						OldNode.GraftLeft(NewElement);
						return;
					}
				}
			}
			// [Loop Continue]
			goto J0xBF;
		}		
	}
	else
	{
		DoAppendItem(NewElement);
	}
	return;
}

function DoAppendItem(UWindowList NewElement)
{
	NewElement.Next = none;
	Last.Next = NewElement;
	NewElement.Prev = Last;
	NewElement.Sentinel = self;
	NewElement.BranchLeft = none;
	NewElement.BranchRight = none;
	NewElement.ParentNode = none;
	Last = NewElement;
	__NFUN_165__(Sentinel.InternalCount);
	Sentinel.bItemOrderChanged = true;
	return;
}

function UWindowList Insert(Class<UWindowList> C)
{
	local UWindowList NewElement;

	NewElement = CreateItem(C);
	InsertItem(NewElement);
	return NewElement;
	return;
}

function InsertItem(UWindowList NewElement)
{
	NewElement.Next = Next;
	// End:0x33
	if(__NFUN_119__(Next, none))
	{
		Next.Prev = NewElement;
	}
	Next = NewElement;
	// End:0x54
	if(__NFUN_114__(Last, self))
	{
		Last = Next;
	}
	NewElement.Prev = self;
	NewElement.Sentinel = self;
	NewElement.BranchLeft = none;
	NewElement.BranchRight = none;
	NewElement.ParentNode = none;
	__NFUN_165__(Sentinel.InternalCount);
	Sentinel.bItemOrderChanged = true;
	return;
}

function UWindowList FindEntry(int Index)
{
	local UWindowList L;
	local int i;

	L = Next;
	i = 0;
	J0x12:

	// End:0x4C [Loop If]
	if(__NFUN_150__(i, Index))
	{
		L = L.Next;
		// End:0x42
		if(__NFUN_114__(L, none))
		{
			return none;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x12;
	}
	return L;
	return;
}

function AppendListCopy(UWindowList L)
{
	// End:0x0D
	if(__NFUN_114__(L, none))
	{
		return;
	}
	L = L.Next;
	J0x21:

	// End:0x5C [Loop If]
	if(__NFUN_119__(L, none))
	{
		CopyExistingListItem(L.Class, L);
		L = L.Next;
		// [Loop Continue]
		goto J0x21;
	}
	return;
}

function Clear()
{
	InternalCount = 0;
	ParentNode = none;
	BranchLeft = none;
	BranchRight = none;
	bItemOrderChanged = true;
	Next = none;
	Last = self;
	return;
}
