class Info extends Actor
	abstract
	native
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

struct KeyValuePair
{
	var() string Key;
	var() string Value;
};

struct PlayerResponseLine
{
	var() int PlayerNum;
	var() int PlayerID;
	var() string PlayerName;
	var() int Ping;
	var() int Score;
	var() int StatsID;
	var() array<KeyValuePair> PlayerInfo;
};

struct ServerResponseLine
{
	var() int ServerID;
	var() string IP;
	var() int Port;
	var() int QueryPort;
	var() string ServerName;
	var() string MapName;
	var() string GameType;
	var() int CurrentPlayers;
	var() int MaxPlayers;
	var() int Ping;
	var() array<KeyValuePair> ServerInfo;
	var() array<PlayerResponseLine> PlayerInfo;
};

defaultproperties
{
	bHidden=true
	bSkipActorPropertyReplication=true
	bOnlyDirtyReplication=true
	RemoteRole=0
	NetUpdateFrequency=10.0000000
}