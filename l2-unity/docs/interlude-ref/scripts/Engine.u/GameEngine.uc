class GameEngine extends Engine
	transient
	native
	config
	noexport;

struct URL
{
	var string Protocol;
	var string Host;
	var int Port;
	var string Map;
	var array<string> Op;
	var string Portal;
	var int Valid;
};

struct FWind
{
	var float LifeTime;
	var Box Area;
	var Vector Direction;
	var float Force;
	var float Speed;
	var float AccelSpeed;
	var float DeltaSeconds;
};

var Level GLevel;
var Level GEntry;
var Level GSkyLevel;
var PendingLevel GPendingLevel;
var URL LastURL;
var config array<string> ServerActors;
var config array<string> ServerPackages;
var array<Object> DummyArray;
var Object DummyObject;
var int LastHitObject;
var bool bClicked;
var float ClickedMouseX;
var float ClickedMouseY;
var Vector ClickLocation;
var Plane ClickPlane;
var Actor ClickActor;
var int StartTempNpcID;
var int EndTempNpcID;
var int CurrentTempNpcID;
var float AvgFPS;
var float LastDeltaTime;
var float LastDeltaSeconds[30];
var int CurTickIndex;
var array<FWind> Wind;
var array<int> LoadingMap;
var array<int> CommandMacro;
var int LandMark;
var bool bUseUnderWaterEffect;
var float FadeTime;
var bool bFadeOut;
var float FadeElaspedTime;
var bool bFadeIn;
var Color FadeColor;
var int nFadeType;
var bool bPlayerObserverMode;
var Vector TeleportLoc;
var bool bTimeCheck;
var int RequestedTime;
var int ElapsedTimeCheck;
var float MusicMaxTime;
var float DefaultMusicMaxTime;
var float BlackOutDuration;
var float FadeInTime;
var bool bLazyMode;
var int UnderWaterEffect;
var bool bAttachedUnderWaterEffect;
var int InspectorCount;
var int InspectorParams;
var Class InspectorClass;
var array<Object> CameraEffects;
var int AuthData[4];
var bool bConnectionClosed;

defaultproperties
{
	MusicMaxTime=60.0000000
	DefaultMusicMaxTime=30.0000000
}