class MatSubAction extends MatObject
	abstract
	native
	editinlinenew;

enum ESAStatus
{
	SASTATUS_Waiting,               // 0
	SASTATUS_Running,               // 1
	SASTATUS_Ending,                // 2
	SASTATUS_Expired                // 3
};

var(Time) float Delay;
var(Time) float Duration;
var Texture Icon;
var MatSubAction.ESAStatus Status;
var localized string Desc;
var transient float PctStarting;
var transient float PctEnding;
var transient float PctDuration;

defaultproperties
{
	Desc="N/A"
}