class CouponEventWnd extends UIScript;

var string CurrentInput;
var bool completeEditbox;
var array<int> completebox;

function OnLoad()
{
	RegisterEvent(530);
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("CouponEventWnd");
	completebox.Length = 5;
	initValue();
	return;
}

function initValue()
{
	CurrentInput = "";
	completeEditbox = false;
	completebox[1] = 0;
	completebox[2] = 0;
	completebox[3] = 0;
	completebox[4] = 0;
	completebox[5] = 0;
	return;
}

function resetEditBox()
{
	local int i;

	i = 1;
	J0x07:

	// End:0x4D [Loop If]
	if(__NFUN_152__(i, 5))
	{
		Class'NWindow.UIAPI_EDITBOX'.static.SetString(__NFUN_112__("CouponEventWnd.input", string(i)), "");
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function OnShow()
{
	initValue();
	resetEditBox();
	Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("CouponEventWnd.inputnumber");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("CouponEventWnd.input1");
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x2E
	if(__NFUN_154__(Event_ID, 530))
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("CouponEventWnd");
	}
	return;
}

function OnClickButton(string strID)
{
	// End:0x48
	if(__NFUN_122__(strID, "inputnumber"))
	{
		Proc_Delivery();
		resetEditBox();
		initValue();
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("CouponEventWnd");
	}
	// End:0x8E
	if(__NFUN_122__(strID, "resetbtn"))
	{
		resetEditBox();
		initValue();
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("CouponEventWnd.input1");
	}
	return;
}

function Proc_Delivery()
{
	RequestPCCafeCouponUse(CurrentInput);
	return;
}

function OnChangeEditBox(string EditBoxID)
{
	// End:0x1D
	if(__NFUN_122__(EditBoxID, "input1"))
	{
		count_editBox(EditBoxID);
	}
	// End:0x3A
	if(__NFUN_122__(EditBoxID, "input2"))
	{
		count_editBox(EditBoxID);
	}
	// End:0x57
	if(__NFUN_122__(EditBoxID, "input3"))
	{
		count_editBox(EditBoxID);
	}
	// End:0x74
	if(__NFUN_122__(EditBoxID, "input4"))
	{
		count_editBox(EditBoxID);
	}
	// End:0x91
	if(__NFUN_122__(EditBoxID, "input5"))
	{
		count_editBox(EditBoxID);
	}
	return;
}

function count_editBox(string currentboxnum)
{
	local array<string> inputtxt;
	local int currentboxNumint, i;

	inputtxt.Length = 5;
	currentboxNumint = int(__NFUN_234__(currentboxnum, 1));
	inputtxt[currentboxNumint] = __NFUN_112__("", Class'NWindow.UIAPI_EDITBOX'.static.GetString(__NFUN_112__("CouponEventWnd.input", string(currentboxNumint))));
	// End:0xB8
	if(__NFUN_154__(__NFUN_125__(inputtxt[currentboxNumint]), 4))
	{
		completebox[currentboxNumint] = 1;
		// End:0xB5
		if(__NFUN_155__(currentboxNumint, 5))
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetFocus(__NFUN_112__("CouponEventWnd.input", string(__NFUN_146__(currentboxNumint, 1))));
		}		
	}
	else
	{
		completebox[currentboxNumint] = 0;
	}
	i = 1;
	J0xCC:

	// End:0xE2 [Loop If]
	if(__NFUN_152__(i, 5))
	{
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xCC;
	}
	// End:0x235
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_154__(completebox[1], 1), __NFUN_154__(completebox[2], 1)), __NFUN_154__(completebox[3], 1)), __NFUN_154__(completebox[4], 1)), __NFUN_154__(completebox[5], 1)))
	{
		completeEditbox = true;
		CurrentInput = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("", Class'NWindow.UIAPI_EDITBOX'.static.GetString("CouponEventWnd.input1")), Class'NWindow.UIAPI_EDITBOX'.static.GetString("CouponEventWnd.input2")), Class'NWindow.UIAPI_EDITBOX'.static.GetString("CouponEventWnd.input3")), Class'NWindow.UIAPI_EDITBOX'.static.GetString("CouponEventWnd.input4")), Class'NWindow.UIAPI_EDITBOX'.static.GetString("CouponEventWnd.input5"));
		Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("CouponEventWnd.inputnumber");		
	}
	else
	{
		completeEditbox = false;
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("CouponEventWnd.inputnumber");
	}
	return;
}
