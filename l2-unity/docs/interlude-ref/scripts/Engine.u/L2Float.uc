class L2Float extends Actor
	native
	placeable;

enum TagState
{
	L2TAG_NONE,                     // 0
	L2TAG_WAIT,                     // 1
	L2TAG_BATTLE                    // 2
};

var L2Float.TagState State;
var int FishType;
var bool Gut;
var bool Fake;
var float WaterEffectTimer;
var float FakeEffectTimer;
var Vector OldEffectLoc;
var float fEffectElapsedTime;
var int EffectType;
var Vector OrgLocation;
var Emitter BrightEffect;
var name WaitAnimName;
var name BattleAnimName;
var name BattleWaitAnimName[6];

defaultproperties
{
	WaitAnimName="Wait"
	BattleAnimName="battle"
	BattleWaitAnimName[0]="battlewait01"
	BattleWaitAnimName[1]="battlewait03"
	BattleWaitAnimName[2]="battlewait05"
	BattleWaitAnimName[3]="battlewait02"
	BattleWaitAnimName[4]="battlewait04"
	BattleWaitAnimName[5]="battlewait06"
	DrawType=2
	bCheckChangableLevel=true
}