class LocalMessage extends Info
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var bool bComplexString;
var bool bIsSpecial;
var bool bIsUnique;
var bool bIsConsoleMessage;
var bool bFadeMessage;
var bool bBeep;
var bool bOffsetYPos;
var bool bCenter;
var bool bFromBottom;
var int LifeTime;
var(Message) Class<LocalMessage> ChildMessage;
var Color DrawColor;
var float XPos;
var float YPos;
var int FontSize;

static function RenderComplexMessage(Canvas Canvas, out float XL, out float YL, optional string MessageString, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	return;
}

static function string GetRelatedString(optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	return GetString(Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject);
	return;
}

static function string GetString(optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	// End:0x34
	if(__NFUN_119__(Class<Actor>(OptionalObject), none))
	{
		return Class<Actor>(OptionalObject).static.GetLocalString(Switch, RelatedPRI_1, RelatedPRI_2);
	}
	return "";
	return;
}

static function string AssembleString(HUD myHUD, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional string MessageString)
{
	return "";
	return;
}

static function ClientReceive(PlayerController P, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	// End:0x45
	if(__NFUN_119__(P.myHUD, none))
	{
		P.myHUD.LocalizedMessage(default.Class, Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject);
	}
	// End:0xDA
	if(__NFUN_130__(__NFUN_130__(default.bIsConsoleMessage, __NFUN_119__(P.Player, none)), __NFUN_119__(P.Player.Console, none)))
	{
		P.Player.InteractionMaster.Process_Message(GetString(Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject), 6.0000000, P.Player.LocalInteractions);
	}
	return;
}

static function Color GetConsoleColor(PlayerReplicationInfo RelatedPRI_1)
{
	return default.DrawColor;
	return;
}

static function Color GetColor(optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2)
{
	return default.DrawColor;
	return;
}

static function float GetOffset(int Switch, float YL, float ClipY)
{
	return default.YPos;
	return;
}

static function int GetFontSize(int Switch, PlayerReplicationInfo RelatedPRI1, PlayerReplicationInfo RelatedPRI2, PlayerReplicationInfo LocalPlayer)
{
	return default.FontSize;
	return;
}

static function float GetLifeTime(int Switch)
{
	return float(default.LifeTime);
	return;
}

defaultproperties
{
	bIsSpecial=true
	bIsConsoleMessage=true
	LifeTime=3
	DrawColor=(R=255,G=255,B=255,A=255)
	XPos=0.5000000
	YPos=0.7000000
}