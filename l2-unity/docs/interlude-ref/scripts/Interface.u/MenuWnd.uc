class MenuWnd extends UICommonAPI;

function OnLoad()
{
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x20
		case "BtnCharInfo":
			ToggleOpenCharInfoWnd();
			// End:0x6C
			break;
		// End:0x3A
		case "BtnInventory":
			ToggleOpenInventoryWnd();
			// End:0x6C
			break;
		// End:0x4E
		case "BtnMap":
			ToggleOpenMinimapWnd();
			// End:0x6C
			break;
		// End:0x69
		case "BtnSystemMenu":
			ToggleOpenSystemMenuWnd();
			// End:0x6C
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function ToggleOpenCharInfoWnd()
{
	// End:0x4C
	if(IsShowWindow("MainWnd"))
	{
		HideWindow("MainWnd");
		PlaySound("InterfaceSound.charstat_close_01");		
	}
	else
	{
		ShowWindowWithFocus("MainWnd");
		PlaySound("InterfaceSound.charstat_open_01");
	}
	return;
}

function ToggleOpenInventoryWnd()
{
	// End:0x57
	if(IsShowWindow("InventoryWnd"))
	{
		HideWindow("InventoryWnd");
		PlaySound("InterfaceSound.inventory_close_01");		
	}
	else
	{
		ShowWindowWithFocus("InventoryWnd");
		PlaySound("InterfaceSound.inventory_open_01");
	}
	return;
}

function ToggleOpenMinimapWnd()
{
	RequestOpenMinimap();
	return;
}

function ToggleOpenSystemMenuWnd()
{
	// End:0x56
	if(IsShowWindow("SystemMenuWnd"))
	{
		HideWindow("SystemMenuWnd");
		PlaySound("InterfaceSound.system_close_01");		
	}
	else
	{
		ShowWindowWithFocus("SystemMenuWnd");
		PlaySound("InterfaceSound.system_open_01");
	}
	return;
}
