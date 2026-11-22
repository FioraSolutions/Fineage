class Mutator extends Info
	native
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var Mutator NextMutator;
var Class<Weapon> DefaultWeapon;
var string DefaultWeaponName;
var() string GroupName;
var() localized string FriendlyName;
var() localized string Description;
var bool bUserAdded;

event PreBeginPlay()
{
	// End:0x0E
	if(__NFUN_129__(MutatorIsAllowed()))
	{
		__NFUN_279__();
	}
	return;
}

function bool MutatorIsAllowed()
{
	return __NFUN_132__(__NFUN_129__(Level.IsDemoBuild()), __NFUN_114__(Class, Class'Engine.Mutator'));
	return;
}

function Destroyed()
{
	local Mutator M;

	// End:0x3D
	if(__NFUN_114__(Level.Game.BaseMutator, self))
	{
		Level.Game.BaseMutator = NextMutator;		
	}
	else
	{
		M = Level.Game.BaseMutator;
		J0x5A:

		// End:0xA7 [Loop If]
		if(__NFUN_119__(M, none))
		{
			// End:0x90
			if(__NFUN_114__(M.NextMutator, self))
			{
				M.NextMutator = NextMutator;
				// [Explicit Break]
				goto J0xA7;
			}
			M = M.NextMutator;
			// [Loop Continue]
			goto J0x5A;
		}
	}
	J0xA7:

	super(Actor).Destroyed();
	return;
}

function Mutate(string MutateString, PlayerController Sender)
{
	// End:0x24
	if(__NFUN_119__(NextMutator, none))
	{
		NextMutator.Mutate(MutateString, Sender);
	}
	return;
}

function ModifyLogin(out string Portal, out string Options)
{
	// End:0x24
	if(__NFUN_119__(NextMutator, none))
	{
		NextMutator.ModifyLogin(Portal, Options);
	}
	return;
}

function ModifyPlayer(Pawn Other)
{
	// End:0x1F
	if(__NFUN_119__(NextMutator, none))
	{
		NextMutator.ModifyPlayer(Other);
	}
	return;
}

function Class<Weapon> GetDefaultWeapon()
{
	local Class<Weapon> W;

	// End:0x3A
	if(__NFUN_119__(NextMutator, none))
	{
		W = NextMutator.GetDefaultWeapon();
		// End:0x37
		if(__NFUN_114__(W, none))
		{
			W = MyDefaultWeapon();
		}		
	}
	else
	{
		W = MyDefaultWeapon();
	}
	return W;
	return;
}

function Class<Inventory> GetInventoryClass(string InventoryClassName)
{
	InventoryClassName = GetInventoryClassOverride(InventoryClassName);
	return Class<Inventory>(DynamicLoadObject(InventoryClassName, Class'Core.Class'));
	return;
}

function string GetInventoryClassOverride(string InventoryClassName)
{
	// End:0x20
	if(__NFUN_119__(NextMutator, none))
	{
		return NextMutator.GetInventoryClassOverride(InventoryClassName);
	}
	return InventoryClassName;
	return;
}

function Class<Weapon> MyDefaultWeapon()
{
	// End:0x34
	if(__NFUN_130__(__NFUN_114__(DefaultWeapon, none), __NFUN_123__(DefaultWeaponName, "")))
	{
		DefaultWeapon = Class<Weapon>(DynamicLoadObject(DefaultWeaponName, Class'Core.Class'));
	}
	return DefaultWeapon;
	return;
}

function AddMutator(Mutator M)
{
	// End:0x19
	if(__NFUN_114__(NextMutator, none))
	{
		NextMutator = M;		
	}
	else
	{
		NextMutator.AddMutator(M);
	}
	return;
}

function bool ReplaceWith(Actor Other, string aClassName)
{
	local Actor A;
	local Class<Actor> aClass;

	// End:0x0E
	if(__NFUN_122__(aClassName, ""))
	{
		return true;
	}
	aClass = Class<Actor>(DynamicLoadObject(aClassName, Class'Core.Class'));
	// End:0x7A
	if(__NFUN_119__(aClass, none))
	{
		A = __NFUN_278__(aClass, Other.Owner, Other.Tag, Other.Location, Other.Rotation);
	}
	// End:0x193
	if(Other.__NFUN_303__('Pickup'))
	{
		// End:0x166
		if(__NFUN_119__(Pickup(Other).myMarker, none))
		{
			Pickup(Other).myMarker.markedItem = Pickup(A);
			// End:0x14E
			if(__NFUN_119__(Pickup(A), none))
			{
				Pickup(A).myMarker = Pickup(Other).myMarker;
				A.__NFUN_267__(__NFUN_215__(A.Location, __NFUN_213__(__NFUN_175__(A.CollisionHeight, Other.CollisionHeight), vect(0.0000000, 0.0000000, 1.0000000))));
			}
			Pickup(Other).myMarker = none;			
		}
		else
		{
			// End:0x193
			if(A.__NFUN_303__('Pickup'))
			{
				Pickup(A).RespawnTime = 0.0000000;
			}
		}
	}
	// End:0x1DA
	if(__NFUN_119__(A, none))
	{
		A.Event = Other.Event;
		A.Tag = Other.Tag;
		return true;
	}
	return false;
	return;
}

function bool AlwaysKeep(Actor Other)
{
	// End:0x20
	if(__NFUN_119__(NextMutator, none))
	{
		return NextMutator.AlwaysKeep(Other);
	}
	return false;
	return;
}

function bool IsRelevant(Actor Other, out byte bSuperRelevant)
{
	local bool bResult;

	bResult = CheckReplacement(Other, bSuperRelevant);
	// End:0x4D
	if(__NFUN_130__(bResult, __NFUN_119__(NextMutator, none)))
	{
		bResult = NextMutator.IsRelevant(Other, bSuperRelevant);
	}
	return bResult;
	return;
}

function bool CheckRelevance(Actor Other)
{
	local bool bResult;
	local byte bSuperRelevant;

	// End:0x10
	if(AlwaysKeep(Other))
	{
		return true;
	}
	bResult = IsRelevant(Other, bSuperRelevant);
	return bResult;
	return;
}

function bool CheckReplacement(Actor Other, out byte bSuperRelevant)
{
	return true;
	return;
}

function PlayerChangedClass(Controller aPlayer)
{
	NextMutator.PlayerChangedClass(aPlayer);
	return;
}

function GetServerDetails(out ServerResponseLine ServerState)
{
	local int i;

	i = ServerState.ServerInfo.Length;
	ServerState.ServerInfo.Length = __NFUN_146__(i, 1);
	ServerState.ServerInfo[i].Key = "Mutator";
	ServerState.ServerInfo[i].Value = GetHumanReadableName();
	return;
}

function GetServerPlayers(out ServerResponseLine ServerState)
{
	return;
}

function string ParseChatPercVar(Controller Who, string Cmd)
{
	// End:0x2A
	if(__NFUN_119__(NextMutator, none))
	{
		Cmd = NextMutator.ParseChatPercVar(Who, Cmd);
	}
	return Cmd;
	return;
}

defaultproperties
{
	FriendlyName="Mutator"
	Description="Description"
}