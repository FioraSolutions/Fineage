class KarmaParamsRBFull extends KarmaParams
	native
	editinlinenew;

var() float KInertiaTensor[6];
var() Vector KCOMOffset;

defaultproperties
{
	KInertiaTensor[0]=0.4000000
	KInertiaTensor[3]=0.4000000
	KInertiaTensor[5]=0.4000000
}