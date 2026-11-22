class ItemEnchantWnd extends UICommonAPI;

var WindowHandle Me;
var ItemWindowHandle ItemWnd;

function OnLoad()
{
	RegisterEvent(2860);
	RegisterEvent(2870);
	RegisterEvent(2880);
	RegisterEvent(2890);
	Me = GetHandle("ItemEnchantWnd");
	ItemWnd = ItemWindowHandle(GetHandle("ItemEnchantWnd.ItemWnd"));
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x1D
	if(__NFUN_154__(Event_ID, 2860))
	{
		HandleEnchantShow(param);		
	}
	else
	{
		// End:0x35
		if(__NFUN_154__(Event_ID, 2870))
		{
			HandleEnchantHide();			
		}
		else
		{
			// End:0x52
			if(__NFUN_154__(Event_ID, 2880))
			{
				HandleEnchantItemList(param);				
			}
			else
			{
				// End:0x6C
				if(__NFUN_154__(Event_ID, 2890))
				{
					HandleEnchantResult(param);
				}
			}
		}
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1A
		case "btnOK":
			OnOKClick();
			// End:0x34
			break;
		// End:0x31
		case "btnCancel":
			OnCancelClick();
			// End:0x34
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnOKClick()
{
	local ItemInfo infItem;

	ItemWnd.GetSelectedItem(infItem);
	// End:0x3D
	if(__NFUN_151__(infItem.ServerID, 0))
	{
		Class'NWindow.EnchantAPI'.static.RequestEnchantItem(infItem.ServerID);
	}
	return;
}

function OnCancelClick()
{
	Class'NWindow.EnchantAPI'.static.RequestEnchantItem(-1);
	Me.HideWindow();
	Clear();
	return;
}

function Clear()
{
	ItemWnd.Clear();
	return;
}

function HandleEnchantShow(string param)
{
	local int ClassID;

	Clear();
	ParseInt(param, "ClassID", ClassID);
	Me.SetWindowTitle(__NFUN_112__(__NFUN_112__(__NFUN_112__(GetSystemString(1220), "("), Class'NWindow.UIDATA_ITEM'.static.GetItemName(ClassID)), ")"));
	Me.ShowWindow();
	Me.SetFocus();
	return;
}

function HandleEnchantHide()
{
	Me.HideWindow();
	Clear();
	return;
}

function HandleEnchantItemList(string param)
{
	local ItemInfo infItem;

	ParamToItemInfo(param, infItem);
	ItemWnd.AddItem(infItem);
	return;
}

function HandleEnchantResult(string param)
{
	Me.HideWindow();
	Clear();
	return;
}
