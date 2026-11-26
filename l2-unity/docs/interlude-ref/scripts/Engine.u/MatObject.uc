class MatObject extends Object
	abstract
	native;

struct Orientation
{
	var() Object.ECamOrientation CamOrientation;
	var() Actor LookAt;
	var() float EaseIntime;
	var() int bReversePitch;
	var() int bReverseYaw;
	var() int bReverseRoll;
	var int MA;
	var float PctInStart;
	var float PctInEnd;
	var float PctInDuration;
	var Rotator StartingRotation;
};
