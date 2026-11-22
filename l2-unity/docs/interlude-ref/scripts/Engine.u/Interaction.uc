class Interaction extends Interactions
	native;

var bool bActive;
var bool bVisible;
var bool bRequiresTick;
var bool bNativeEvents;
var Player ViewportOwner;
var InteractionMaster Master;

// Export UInteraction::execInitialize(FFrame&, void* const)
native function Initialize();

// Export UInteraction::execConsoleCommand(FFrame&, void* const)
native function bool ConsoleCommand(coerce string S);

// Export UInteraction::execWorldToScreen(FFrame&, void* const)
native function Vector WorldToScreen(Vector Location, optional Vector CameraLocation, optional Rotator CameraRotation);

// Export UInteraction::execScreenToWorld(FFrame&, void* const)
native function Vector ScreenToWorld(Vector Location, optional Vector CameraLocation, optional Rotator CameraRotation);

event Initialized()
{
	return;
}

function Message(coerce string Msg, float MsgLife)
{
	return;
}

function bool KeyType(out Interactions.EInputKey Key, optional string Unicode)
{
	return false;
	return;
}

function bool KeyEvent(out Interactions.EInputKey Key, out Interactions.EInputAction Action, float Delta)
{
	return false;
	return;
}

function PreRender(Canvas Canvas)
{
	return;
}

function PostRender(Canvas Canvas)
{
	return;
}

function SetFocus()
{
	Master.SetFocusTo(self, ViewportOwner);
	return;
}

function Tick(float DeltaTime)
{
	return;
}

defaultproperties
{
	bActive=true
}