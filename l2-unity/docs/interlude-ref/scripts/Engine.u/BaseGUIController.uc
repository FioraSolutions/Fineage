class BaseGUIController extends Interaction
	native;

var Material DefaultPens[3];
//var delegate<OnAdminReply> __OnAdminReply__Delegate;

delegate OnAdminReply(string Reply)
{
	return;
}

event bool OpenMenu(string NewMenuName, optional string Param1, optional string Param2)
{
	return false;
	return;
}

event bool ReplaceMenu(string NewMenuName, optional string Param1, optional string Param2)
{
	return false;
	return;
}

event bool CloseMenu(optional bool bCanceled)
{
	return true;
	return;
}

event CloseAll(bool bCancel)
{
	return;
}

function SetControllerStatus(bool On)
{
	bActive = On;
	bVisible = On;
	bRequiresTick = On;
	return;
}

event InitializeController()
{
	return;
}

event bool NeedsMenuResolution()
{
	return;
}

event SetRequiredGameResolution(string GameRes)
{
	return;
}

defaultproperties
{
	DefaultPens[0]=Texture'Engine.MenuWhite'
	DefaultPens[1]=Texture'Engine.MenuBlack'
	DefaultPens[2]=Texture'Engine.MenuGray'
	bActive=false
	bNativeEvents=true
}