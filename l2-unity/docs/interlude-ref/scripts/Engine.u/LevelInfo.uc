class LevelInfo extends ZoneInfo
	native
	nativereplication
	config
	placeable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

enum EPhysicsDetailLevel
{
	PDL_Low,                        // 0
	PDL_Medium,                     // 1
	PDL_High                        // 2
};

enum ELevelAction
{
	LEVACT_None,                    // 0
	LEVACT_Loading,                 // 1
	LEVACT_Saving,                  // 2
	LEVACT_Connecting,              // 3
	LEVACT_Precaching               // 4
};

enum ENetMode
{
	NM_Standalone,                  // 0
	NM_DedicatedServer,             // 1
	NM_ListenServer,                // 2
	NM_Client                       // 3
};

var() float TimeDilation;
var float TimeSeconds;
var transient int Year;
var transient int Month;
var transient int Day;
var transient int DayOfWeek;
var transient int Hour;
var transient int Minute;
var transient int Second;
var transient int Millisecond;
var float PauseDelay;
var config float DecalStayScale;
var() config LevelInfo.EPhysicsDetailLevel PhysicsDetailLevel;
var(Karma) float KarmaTimeScale;
var(Karma) float RagdollTimeScale;
var(Karma) int MaxRagdolls;
var(Karma) float KarmaGravScale;
var(Karma) bool bKStaticFriction;
var() bool bKNoInit;
var() localized string Title;
var() string Author;
var() localized string LevelEnterText;
var() string LocalizedPkg;
var PlayerReplicationInfo Pauser;
var() int RecommendedNumPlayers;
var LevelSummary Summary;
var string VisibleGroups;
var transient string SelectedGroups;
var() bool bLonePlayer;
var bool bBegunPlay;
var bool bPlayersOnly;
var const Actor.EDetailMode DetailMode;
var bool bDropDetail;
var bool bAggressiveLOD;
var bool bStartup;
var bool bPathsRebuilt;
var bool bHasPathNodes;
var globalconfig bool bCapFramerate;
var config bool bNeverPrecache;
var() Vector CameraLocationDynamic;
var() Vector CameraLocationTop;
var() Vector CameraLocationFront;
var() Vector CameraLocationSide;
var() Rotator CameraRotationDynamic;
var(Audio) string Song;
var(Audio) float PlayerDoppler;
var(Audio) float MusicVolumeOverride;
var() float Brightness;
var() Texture Screenshot;
var Texture DefaultTexture;
var Texture WireframeTexture;
var Texture WhiteSquareTexture;
var Texture LargeVertex;
var int HubStackLevel;
var transient LevelInfo.ELevelAction LevelAction;
var transient GameReplicationInfo GRI;
var LevelInfo.ENetMode NetMode;
var string ComputerName;
var string EngineVersion;
var string MinNetVersion;
var() string DefaultGameType;
var() string PreCacheGame;
var GameInfo Game;
var float DefaultGravity;
var const NavigationPoint NavigationPointList;
var const Controller ControllerList;
var private PlayerController LocalPlayerController;
var string NextURL;
var bool bNextItems;
var float NextSwitchCountdown;
var transient ObjectPool ObjectPool;
var transient array<Material> PrecacheMaterials;
var transient array<StaticMesh> PrecacheStaticMeshes;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(bNetDirty, __NFUN_154__(int(Role), int(ROLE_Authority))))
		DefaultGravity, Pauser, 
		TimeDilation;

	// Pos:0x018
	reliable if(__NFUN_130__(bNetInitial, __NFUN_154__(int(Role), int(ROLE_Authority))))
		KarmaGravScale, KarmaTimeScale, 
		RagdollTimeScale;
}

// Export ULevelInfo::execDetailChange(FFrame&, void* const)
native simulated function DetailChange(Actor.EDetailMode NewDetailMode);

// Export ULevelInfo::execIsEntry(FFrame&, void* const)
native simulated function bool IsEntry();

simulated function PostBeginPlay()
{
	super(Actor).PostBeginPlay();
	DecalStayScale = __NFUN_246__(DecalStayScale, 0.0000000, 2.0000000);
	return;
}

simulated function Class<GameInfo> GetGameClass()
{
	local Class<GameInfo> G;

	// End:0x2C
	if(__NFUN_119__(Level.Game, none))
	{
		return Level.Game.Class;
	}
	// End:0x72
	if(__NFUN_130__(__NFUN_119__(GRI, none), __NFUN_123__(GRI.GameClass, "")))
	{
		G = Class<GameInfo>(DynamicLoadObject(GRI.GameClass, Class'Core.Class'));
	}
	// End:0x83
	if(__NFUN_119__(G, none))
	{
		return G;
	}
	// End:0xAA
	if(__NFUN_123__(DefaultGameType, ""))
	{
		G = Class<GameInfo>(DynamicLoadObject(DefaultGameType, Class'Core.Class'));
	}
	return G;
	return;
}

simulated event FillPrecacheMaterialsArray()
{
	local Actor A;
	local Class<GameInfo> G;

	// End:0x12
	if(__NFUN_154__(int(NetMode), int(NM_DedicatedServer)))
	{
		return;
	}
	// End:0xE1
	if(__NFUN_114__(Level.Game, none))
	{
		// End:0x6C
		if(__NFUN_130__(__NFUN_119__(GRI, none), __NFUN_123__(GRI.GameClass, "")))
		{
			G = Class<GameInfo>(DynamicLoadObject(GRI.GameClass, Class'Core.Class'));
		}
		// End:0xA0
		if(__NFUN_130__(__NFUN_114__(G, none), __NFUN_123__(DefaultGameType, "")))
		{
			G = Class<GameInfo>(DynamicLoadObject(DefaultGameType, Class'Core.Class'));
		}
		// End:0xC6
		if(__NFUN_114__(G, none))
		{
			G = Class<GameInfo>(DynamicLoadObject(PreCacheGame, Class'Core.Class'));
		}
		// End:0xE1
		if(__NFUN_119__(G, none))
		{
			G.static.PrecacheGameTextures(self);
		}
	}
	// End:0x101
	foreach __NFUN_304__(Class'Engine.Actor', A)
	{
		A.UpdatePrecacheMaterials();		
	}	
	return;
}

simulated event FillPrecacheStaticMeshesArray()
{
	local Actor A;
	local Class<GameInfo> G;

	// End:0x12
	if(__NFUN_154__(int(NetMode), int(NM_DedicatedServer)))
	{
		return;
	}
	// End:0xE1
	if(__NFUN_114__(Level.Game, none))
	{
		// End:0x6C
		if(__NFUN_130__(__NFUN_119__(GRI, none), __NFUN_123__(GRI.GameClass, "")))
		{
			G = Class<GameInfo>(DynamicLoadObject(GRI.GameClass, Class'Core.Class'));
		}
		// End:0xA0
		if(__NFUN_130__(__NFUN_114__(G, none), __NFUN_123__(DefaultGameType, "")))
		{
			G = Class<GameInfo>(DynamicLoadObject(DefaultGameType, Class'Core.Class'));
		}
		// End:0xC6
		if(__NFUN_114__(G, none))
		{
			G = Class<GameInfo>(DynamicLoadObject(PreCacheGame, Class'Core.Class'));
		}
		// End:0xE1
		if(__NFUN_119__(G, none))
		{
			G.static.PrecacheGameStaticMeshes(self);
		}
	}
	// End:0x101
	foreach __NFUN_304__(Class'Engine.Actor', A)
	{
		A.UpdatePrecacheStaticMeshes();		
	}	
	return;
}

simulated function AddPrecacheMaterial(Material mat)
{
	local int Index;

	// End:0x12
	if(__NFUN_154__(int(NetMode), int(NM_DedicatedServer)))
	{
		return;
	}
	// End:0x1F
	if(__NFUN_114__(mat, none))
	{
		return;
	}
	Index = Level.PrecacheMaterials.Length;
	PrecacheMaterials.Insert(Index, 1);
	PrecacheMaterials[Index] = mat;
	return;
}

simulated function AddPrecacheStaticMesh(StaticMesh stat)
{
	local int Index;

	// End:0x12
	if(__NFUN_154__(int(NetMode), int(NM_DedicatedServer)))
	{
		return;
	}
	// End:0x1F
	if(__NFUN_114__(stat, none))
	{
		return;
	}
	Index = Level.PrecacheStaticMeshes.Length;
	PrecacheStaticMeshes.Insert(Index, 1);
	PrecacheStaticMeshes[Index] = stat;
	return;
}

// Export ULevelInfo::execGetLocalURL(FFrame&, void* const)
native simulated function string GetLocalURL();

// Export ULevelInfo::execIsDemoBuild(FFrame&, void* const)
native final simulated function bool IsDemoBuild();

// Export ULevelInfo::execGetAddressURL(FFrame&, void* const)
native simulated function string GetAddressURL();

event ServerTravel(string URL, bool bItems)
{
	// End:0x57
	if(__NFUN_122__(NextURL, ""))
	{
		bNextItems = bItems;
		NextURL = URL;
		// End:0x4C
		if(__NFUN_119__(Game, none))
		{
			Game.ProcessServerTravel(URL, bItems);			
		}
		else
		{
			NextSwitchCountdown = 0.0000000;
		}
	}
	return;
}

function ThisIsNeverExecuted()
{
	local DefaultPhysicsVolume P;

	P = none;
	return;
}

function Reset()
{
	ConsoleCommand("OBJ GARBAGE");
	super(Actor).Reset();
	return;
}

simulated event PreBeginPlay()
{
	ObjectPool = new (none) Class'Engine.ObjectPool';
	return;
}

simulated function PlayerController GetLocalPlayerController()
{
	local PlayerController PC;

	// End:0x1B
	if(__NFUN_154__(int(Level.NetMode), int(NM_DedicatedServer)))
	{
		return none;
	}
	// End:0x2C
	if(__NFUN_119__(LocalPlayerController, none))
	{
		return LocalPlayerController;
	}
	// End:0x64
	foreach __NFUN_313__(Class'Engine.PlayerController', PC)
	{
		// End:0x63
		if(__NFUN_119__(Viewport(PC.Player), none))
		{
			LocalPlayerController = PC;
			// End:0x64
			break;
		}		
	}	
	return LocalPlayerController;
	return;
}

defaultproperties
{
	TimeDilation=1.0000000
	PhysicsDetailLevel=1
	KarmaTimeScale=0.9000000
	RagdollTimeScale=1.0000000
	MaxRagdolls=4
	KarmaGravScale=1.0000000
	bKStaticFriction=true
	Title="Untitled"
	VisibleGroups="None"
	DetailMode=2
	bCapFramerate=true
	Brightness=1.0000000
	DefaultTexture=Texture'Engine.DefaultTexture'
	WireframeTexture=Texture'Engine.WireframeTexture'
	WhiteSquareTexture=Texture'Engine.WhiteSquareTexture'
	LargeVertex=Texture'Engine.LargeVertex'
	PreCacheGame="Engine.GameInfo"
	DefaultGravity=-1500.0000000
	bWorldGeometry=true
	bAlwaysRelevant=true
	RemoteRole=1
	bHiddenEd=true
}