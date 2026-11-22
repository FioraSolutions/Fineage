class NCubics extends Emitter
	native
	placeable;

enum ECubicType
{
	ECT_STORM,                      // 0
	ECT_VAMPIRIC,                   // 1
	ECT_LIFE,                       // 2
	ECT_VIPER,                      // 3
	ECT_DEBUFF                      // 4
};

enum ECubicMovementType
{
	ECMT_FOLLOW,                    // 0
	ECMT_FLOAT,                     // 1
	ECMT_SKILLUSE,                  // 2
	ECMT_BUFF,                      // 3
	ECMT_FLOATSTART,                // 4
	ECMT_ONVEHICLE                  // 5
};

var NCubics.ECubicType CubicType;
var NCubics.ECubicMovementType CubicMovementType;
var Vector DestLocation;
var int CubicIndex;
var int SkillID;
var Pawn TargetPawn;
var float SkillActiveTime;
var Rotator RotPerSecond;
var transient NMagicInfo MagicInfo;

defaultproperties
{
	CubicIndex=-1
	SkillID=-1
	NoCheatCollision=true
	bNoDelete=false
	bAlwaysRelevant=true
	NetUpdateFrequency=8.0000000
	NetPriority=1.4000000
	CollisionRadius=0.1000000
	CollisionHeight=0.1000000
	bFixedRotationDir=true
}