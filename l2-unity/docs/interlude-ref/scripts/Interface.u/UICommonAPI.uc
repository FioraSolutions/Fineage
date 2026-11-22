class UICommonAPI extends UIScript;

enum EDialogType
{
	DIALOG_OKCancel,                // 0
	DIALOG_OK,                      // 1
	DIALOG_OKCancelInput,           // 2
	DIALOG_OKInput,                 // 3
	DIALOG_Warning,                 // 4
	DIALOG_Notice,                  // 5
	DIALOG_NumberPad,               // 6
	DIALOG_Progress                 // 7
};

enum DialogDefaultAction
{
	EDefaultNone,                   // 0
	EDefaultOK,                     // 1
	EDefaultCancel                  // 2
};

function DialogShow(UICommonAPI.EDialogType dialogType, string strMessage)
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.ShowDialog(dialogType, strMessage, string(self));
	return;
}

function DialogHide()
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.HideDialog();
	return;
}

function DialogSetDefaultOK()
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.SetDefaultAction(1);
	return;
}

function bool DialogIsMine()
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	// End:0x35
	if(__NFUN_122__(script.GetTarget(), string(self)))
	{
		return true;
	}
	return false;
	return;
}

function DialogSetID(int Id)
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.SetID(Id);
	return;
}

function DialogSetEditType(string strType)
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.SetEditType(strType);
	return;
}

function string DialogGetString()
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	return script.GetEditMessage();
	return;
}

function DialogSetString(string strInput)
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.SetEditMessage(strInput);
	return;
}

function int DialogGetID()
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	return script.GetID();
	return;
}

function DialogSetParamInt(int param)
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.SetParamInt(param);
	return;
}

function DialogSetReservedInt(int Value)
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.SetReservedInt(Value);
	return;
}

function DialogSetReservedInt2(int Value)
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.SetReservedInt2(Value);
	return;
}

function DialogSetReservedInt3(int Value)
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.SetReservedInt3(Value);
	return;
}

function int DialogGetReservedInt()
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	return script.GetReservedInt();
	return;
}

function int DialogGetReservedInt2()
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	return script.GetReservedInt2();
	return;
}

function int DialogGetReservedInt3()
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	return script.GetReservedInt3();
	return;
}

function DialogSetEditBoxMaxLength(int MaxLength)
{
	local DialogBox script;

	script = DialogBox(GetScript("DialogBox"));
	script.SetEditBoxMaxLength(MaxLength);
	return;
}

function int Split(string strInput, string delim, out array<string> arrToken)
{
	local int arrSize;

	J0x00:
	// End:0x6F [Loop If]
	if(__NFUN_151__(__NFUN_126__(strInput, delim), 0))
	{
		arrToken.Insert(arrToken.Length, 1);
		arrToken[__NFUN_147__(arrToken.Length, 1)] = __NFUN_128__(strInput, __NFUN_126__(strInput, delim));
		strInput = __NFUN_127__(strInput, __NFUN_146__(__NFUN_126__(strInput, delim), 1));
		arrSize = __NFUN_146__(arrSize, 1);
		// [Loop Continue]
		goto J0x00;
	}
	arrToken.Insert(arrToken.Length, 1);
	arrToken[__NFUN_147__(arrToken.Length, 1)] = strInput;
	arrSize = __NFUN_146__(arrSize, 1);
	return arrSize;
	return;
}

function ShowWindow(string a_ControlID)
{
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow(a_ControlID);
	return;
}

function ShowWindowWithFocus(string a_ControlID)
{
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow(a_ControlID);
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus(a_ControlID);
	return;
}

function HideWindow(string a_ControlID)
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow(a_ControlID);
	return;
}

function bool IsShowWindow(string a_ControlID)
{
	return Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow(a_ControlID);
	return;
}

function ParamToItemInfo(string param, out ItemInfo Info)
{
	local int tmpInt;

	ParseInt(param, "classID", Info.ClassID);
	ParseInt(param, "level", Info.Level);
	ParseString(param, "name", Info.Name);
	ParseString(param, "additionalName", Info.AdditionalName);
	ParseString(param, "iconName", Info.IconName);
	ParseString(param, "description", Info.Description);
	ParseInt(param, "itemType", Info.ItemType);
	ParseInt(param, "serverID", Info.ServerID);
	ParseInt(param, "itemNum", Info.ItemNum);
	ParseInt(param, "slotBitType", Info.SlotBitType);
	ParseInt(param, "enchanted", Info.Enchanted);
	ParseInt(param, "blessed", Info.Blessed);
	ParseInt(param, "damaged", Info.Damaged);
	// End:0x1C5
	if(ParseInt(param, "equipped", tmpInt))
	{
		Info.bEquipped = bool(tmpInt);
	}
	ParseInt(param, "price", Info.Price);
	ParseInt(param, "reserved", Info.Reserved);
	ParseInt(param, "defaultPrice", Info.DefaultPrice);
	ParseInt(param, "refineryOp1", Info.RefineryOp1);
	ParseInt(param, "refineryOp2", Info.RefineryOp2);
	ParseInt(param, "currentDurability", Info.CurrentDurability);
	ParseInt(param, "weight", Info.Weight);
	ParseInt(param, "materialType", Info.MaterialType);
	ParseInt(param, "weaponType", Info.WeaponType);
	ParseInt(param, "physicalDamage", Info.PhysicalDamage);
	ParseInt(param, "magicalDamage", Info.MagicalDamage);
	ParseInt(param, "shieldDefense", Info.ShieldDefense);
	ParseInt(param, "shieldDefenseRate", Info.ShieldDefenseRate);
	ParseInt(param, "durability", Info.Durability);
	ParseInt(param, "crystalType", Info.CrystalType);
	ParseInt(param, "randomDamage", Info.RandomDamage);
	ParseInt(param, "critical", Info.Critical);
	ParseInt(param, "hitModify", Info.HitModify);
	ParseInt(param, "attackSpeed", Info.AttackSpeed);
	ParseInt(param, "mpConsume", Info.MpConsume);
	ParseInt(param, "avoidModify", Info.AvoidModify);
	ParseInt(param, "soulshotCount", Info.SoulshotCount);
	ParseInt(param, "spiritshotCount", Info.SpiritshotCount);
	ParseInt(param, "armorType", Info.ArmorType);
	ParseInt(param, "physicalDefense", Info.PhysicalDefense);
	ParseInt(param, "magicalDefense", Info.MagicalDefense);
	ParseInt(param, "mpBonus", Info.MpBonus);
	ParseInt(param, "consumeType", Info.ConsumeType);
	ParseInt(param, "ItemSubType", Info.ItemSubType);
	ParseString(param, "iconNameEx1", Info.IconNameEx1);
	ParseString(param, "iconNameEx2", Info.IconNameEx2);
	ParseString(param, "iconNameEx3", Info.IconNameEx3);
	ParseString(param, "iconNameEx4", Info.IconNameEx4);
	// End:0x65A
	if(ParseInt(param, "arrow", tmpInt))
	{
		Info.bArrow = bool(tmpInt);
	}
	// End:0x688
	if(ParseInt(param, "recipe", tmpInt))
	{
		Info.bRecipe = bool(tmpInt);
	}
	return;
}

function ParamToRecord(string param, out LVDataRecord Record)
{
	local int idx, MaxColumn;

	ParseString(param, "szReserved", Record.szReserved);
	ParseInt(param, "nReserved1", Record.nReserved1);
	ParseInt(param, "nReserved2", Record.nReserved2);
	ParseInt(param, "nReserved3", Record.nReserved3);
	ParseInt(param, "MaxColumn", MaxColumn);
	Record.LVDataList.Length = MaxColumn;
	idx = 0;
	J0xB7:

	// End:0x1DA [Loop If]
	if(__NFUN_150__(idx, MaxColumn))
	{
		ParseString(param, __NFUN_112__("szData_", string(idx)), Record.LVDataList[idx].szData);
		ParseString(param, __NFUN_112__("szReserved_", string(idx)), Record.LVDataList[idx].szReserved);
		ParseInt(param, __NFUN_112__("nReserved1_", string(idx)), Record.LVDataList[idx].nReserved1);
		ParseInt(param, __NFUN_112__("nReserved2_", string(idx)), Record.LVDataList[idx].nReserved2);
		ParseInt(param, __NFUN_112__("nReserved3_", string(idx)), Record.LVDataList[idx].nReserved3);
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0xB7;
	}
	return;
}

function CustomTooltip MakeTooltipSimpleText(string Text)
{
	local CustomTooltip ToolTip;
	local DrawItemInfo Info;

	ToolTip.DrawList.Length = 1;
	Info.eType = 1;
	Info.t_bDrawOneLine = true;
	Info.t_strText = Text;
	ToolTip.DrawList[0] = Info;
	return ToolTip;
	return;
}
