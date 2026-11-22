class CheatManager extends Object within PlayerController
	native;

var Rotator LockedRotation;

exec function ReviewJumpSpots(name TestLabel)
{
	// End:0x1D
	if(__NFUN_254__(TestLabel, 'Transloc'))
	{
		TestLabel = 'Begin';		
	}
	else
	{
		// End:0x3A
		if(__NFUN_254__(TestLabel, 'Jump'))
		{
			TestLabel = 'Finished';			
		}
		else
		{
			// End:0x57
			if(__NFUN_254__(TestLabel, 'Combo'))
			{
				TestLabel = 'FinishedJumping';				
			}
			else
			{
				// End:0x71
				if(__NFUN_254__(TestLabel, 'LowGrav'))
				{
					TestLabel = 'FinishedComboJumping';
				}
			}
		}
	}
	__NFUN_231__(__NFUN_112__("TestLabel is ", string(TestLabel)));
	Outer.Level.Game.ReviewJumpSpots(TestLabel);
	return;
}

exec function ListDynamicActors()
{
	local Actor A;
	local int i;

	// End:0x33
	foreach Outer.__NFUN_313__(Class'Engine.Actor', A)
	{
		__NFUN_165__(i);
		__NFUN_231__(__NFUN_168__(string(i), string(A)));		
	}	
	__NFUN_231__(__NFUN_112__("Num dynamic actors: ", string(i)));
	return;
}

exec function FreezeFrame(float Delay)
{
	Outer.Level.Game.SetPause(true, Outer);
	Outer.Level.PauseDelay = __NFUN_174__(Outer.Level.TimeSeconds, Delay);
	return;
}

exec function WriteToLog()
{
	__NFUN_231__("NOW!");
	return;
}

exec function SetFlash(float f)
{
	Outer.FlashScale.X = f;
	return;
}

exec function SetFogR(float f)
{
	Outer.FlashFog.X = f;
	return;
}

exec function SetFogG(float f)
{
	Outer.FlashFog.Y = f;
	return;
}

exec function SetFogB(float f)
{
	Outer.FlashFog.Z = f;
	return;
}

exec function KillViewedActor()
{
	// End:0x99
	if(__NFUN_119__(Outer.ViewTarget, none))
	{
		// End:0x74
		if(__NFUN_130__(__NFUN_119__(Pawn(Outer.ViewTarget), none), __NFUN_119__(Pawn(Outer.ViewTarget).Controller, none)))
		{
			Pawn(Outer.ViewTarget).Controller.__NFUN_279__();
		}
		Outer.ViewTarget.__NFUN_279__();
		Outer.SetViewTarget(none);
	}
	return;
}

exec function LogScriptedSequences()
{
	local AIScript S;

	// End:0x3B
	foreach Outer.__NFUN_304__(Class'Engine.AIScript', S)
	{
		S.bLoggingEnabled = __NFUN_129__(S.bLoggingEnabled);		
	}	
	return;
}

exec function Teleport()
{
	local Actor HitActor;
	local Vector HitNormal, HitLocation;

	HitActor = Outer.__NFUN_277__(HitLocation, HitNormal, __NFUN_215__(Outer.ViewTarget.Location, __NFUN_213__(float(10000), Vector(Outer.Rotation))), Outer.ViewTarget.Location, true);
	// End:0xAC
	if(__NFUN_114__(HitActor, none))
	{
		HitLocation = __NFUN_215__(Outer.ViewTarget.Location, __NFUN_213__(float(10000), Vector(Outer.Rotation)));		
	}
	else
	{
		HitLocation = __NFUN_215__(HitLocation, __NFUN_213__(Outer.ViewTarget.CollisionRadius, HitNormal));
	}
	Outer.ViewTarget.__NFUN_267__(HitLocation);
	return;
}

exec function ChangeSize(float f)
{
	// End:0x9D
	if(Outer.Pawn.__NFUN_283__(__NFUN_171__(Outer.Pawn.default.CollisionRadius, f), __NFUN_171__(Outer.Pawn.default.CollisionHeight, f)))
	{
		Outer.Pawn.SetDrawScale(f);
		Outer.Pawn.__NFUN_267__(Outer.Pawn.Location);
	}
	return;
}

exec function LockCamera()
{
	local Vector LockedLocation;
	local Rotator LockedRot;
	local Actor LockedActor;

	// End:0x65
	if(__NFUN_129__(Outer.bCameraPositionLocked))
	{
		Outer.PlayerCalcView(LockedActor, LockedLocation, LockedRot);
		Outer.__NFUN_267__(LockedLocation);
		LockedRotation = LockedRot;
		Outer.SetViewTarget(Outer);		
	}
	else
	{
		Outer.SetViewTarget(Outer.Pawn);
	}
	Outer.bCameraPositionLocked = __NFUN_129__(Outer.bCameraPositionLocked);
	Outer.bBehindView = Outer.bCameraPositionLocked;
	Outer.bFreeCamera = false;
	return;
}

exec function SetCameraDist(float f)
{
	Outer.CameraDist = __NFUN_245__(f, 2.0000000);
	return;
}

exec function EndPath()
{
	return;
}

exec function FreeCamera(bool B)
{
	Outer.bFreeCamera = B;
	Outer.bBehindView = B;
	return;
}

exec function CauseEvent(name EventName)
{
	Outer.TriggerEvent(EventName, Outer.Pawn, Outer.Pawn);
	return;
}

exec function Amphibious()
{
	Outer.Pawn.UnderWaterTime = 999999.0000000;
	return;
}

exec function Fly()
{
	// End:0x16
	if(__NFUN_114__(Outer.Pawn, none))
	{
		return;
	}
	Outer.Pawn.UnderWaterTime = Outer.Pawn.default.UnderWaterTime;
	Outer.ClientMessage("You feel much lighter");
	Outer.Pawn.__NFUN_262__(true, true, true);
	Outer.Pawn.bCollideWorld = true;
	Outer.bCheatFlying = true;
	Outer.__NFUN_113__('PlayerFlying');
	return;
}

exec function Walk()
{
	// End:0xAC
	if(__NFUN_119__(Outer.Pawn, none))
	{
		Outer.bCheatFlying = false;
		Outer.Pawn.UnderWaterTime = Outer.Pawn.default.UnderWaterTime;
		Outer.Pawn.__NFUN_262__(true, true, true);
		Outer.Pawn.__NFUN_3970__(1);
		Outer.Pawn.bCollideWorld = true;
		Outer.ClientReStart();
	}
	return;
}

exec function Ghost()
{
	// End:0xCA
	if(__NFUN_130__(__NFUN_119__(Outer.Pawn, none), __NFUN_129__(Outer.Pawn.__NFUN_303__('KVehicle'))))
	{
		Outer.Pawn.UnderWaterTime = -1.0000000;
		Outer.ClientMessage("You feel ethereal");
		Outer.Pawn.__NFUN_262__(false, false, false);
		Outer.Pawn.bCollideWorld = false;
		Outer.bCheatFlying = true;
		Outer.__NFUN_113__('PlayerFlying');		
	}
	else
	{
		__NFUN_231__("Can't Ghost In Vehicles");
	}
	return;
}

exec function AllAmmo()
{
	local Inventory Inv;

	Inv = Outer.Pawn.Inventory;
	J0x1D:

	// End:0x81 [Loop If]
	if(__NFUN_119__(Inv, none))
	{
		// End:0x6A
		if(__NFUN_119__(Ammunition(Inv), none))
		{
			Ammunition(Inv).AmmoAmount = 999;
			Ammunition(Inv).MaxAmmo = 999;
		}
		Inv = Inv.Inventory;
		// [Loop Continue]
		goto J0x1D;
	}
	return;
}

exec function Invisible(bool B)
{
	Outer.Pawn.bHidden = B;
	// End:0x45
	if(B)
	{
		Outer.Pawn.Visibility = 0;		
	}
	else
	{
		Outer.Pawn.Visibility = Outer.Pawn.default.Visibility;
	}
	return;
}

exec function God()
{
	// End:0x42
	if(Outer.bGodMode)
	{
		Outer.bGodMode = false;
		Outer.ClientMessage("God mode off");
		return;
	}
	Outer.bGodMode = true;
	Outer.ClientMessage("God Mode on");
	return;
}

exec function SloMo(float t)
{
	Outer.Level.Game.SetGameSpeed(t);
	Outer.Level.Game.__NFUN_536__();
	Outer.Level.Game.GameReplicationInfo.__NFUN_536__();
	return;
}

exec function SetJumpZ(float f)
{
	Outer.Pawn.JumpZ = f;
	return;
}

exec function SetGravity(float f)
{
	Outer.PhysicsVolume.Gravity.Z = f;
	return;
}

exec function SetSpeed(float f)
{
	Outer.Pawn.GroundSpeed = __NFUN_171__(Outer.Pawn.default.GroundSpeed, f);
	Outer.Pawn.WaterSpeed = __NFUN_171__(Outer.Pawn.default.WaterSpeed, f);
	return;
}

exec function KillAll(Class<Actor> aClass)
{
	local Actor A;

	// End:0x53
	if(__NFUN_258__(aClass, Class'Engine.AIController'))
	{
		Outer.Level.Game.KillBots(Outer.Level.Game.NumBots);
		return;
	}
	// End:0x75
	if(__NFUN_258__(aClass, Class'Engine.Pawn'))
	{
		KillAllPawns(Class<Pawn>(aClass));
		return;
	}
	// End:0xB4
	foreach Outer.__NFUN_313__(Class'Engine.Actor', A)
	{
		// End:0xB3
		if(__NFUN_258__(A.Class, aClass))
		{
			A.__NFUN_279__();
		}		
	}	
	return;
}

function KillAllPawns(Class<Pawn> aClass)
{
	local Pawn P;

	Outer.Level.Game.KillBots(Outer.Level.Game.NumBots);
	// End:0xBF
	foreach Outer.__NFUN_313__(Class'Engine.Pawn', P)
	{
		// End:0xBE
		if(__NFUN_130__(__NFUN_258__(P.Class, aClass), __NFUN_129__(P.IsPlayerPawn())))
		{
			// End:0xB2
			if(__NFUN_119__(P.Controller, none))
			{
				P.Controller.__NFUN_279__();
			}
			P.__NFUN_279__();
		}		
	}	
	return;
}

exec function KillPawns()
{
	KillAllPawns(Class'Engine.Pawn');
	return;
}

exec function Avatar(string ClassName)
{
	local Class<Actor> NewClass;
	local Pawn P;

	NewClass = Class<Actor>(DynamicLoadObject(ClassName, Class'Core.Class'));
	// End:0xD6
	if(__NFUN_119__(NewClass, none))
	{
		// End:0xD5
		foreach Outer.__NFUN_313__(Class'Engine.Pawn', P)
		{
			// End:0xD4
			if(__NFUN_130__(__NFUN_114__(P.Class, NewClass), __NFUN_119__(P, Outer.Pawn)))
			{
				// End:0xBD
				if(__NFUN_119__(Outer.Pawn.Controller, none))
				{
					Outer.Pawn.Controller.PawnDied(Outer.Pawn);
				}
				Outer.Possess(P);
				// End:0xD5
				break;
			}			
		}		
	}
	return;
}

exec function Summon(string ClassName)
{
	local Class<Actor> NewClass;
	local Vector SpawnLoc;

	__NFUN_231__(__NFUN_112__("Fabricate ", ClassName));
	NewClass = Class<Actor>(DynamicLoadObject(ClassName, Class'Core.Class'));
	// End:0xC8
	if(__NFUN_119__(NewClass, none))
	{
		// End:0x6F
		if(__NFUN_119__(Outer.Pawn, none))
		{
			SpawnLoc = Outer.Pawn.Location;			
		}
		else
		{
			SpawnLoc = Outer.Location;
		}
		Outer.__NFUN_278__(NewClass,,, __NFUN_215__(__NFUN_215__(SpawnLoc, __NFUN_213__(float(72), Vector(Outer.Rotation))), __NFUN_212__(vect(0.0000000, 0.0000000, 1.0000000), float(15))));
	}
	return;
}

exec function PlayersOnly()
{
	Outer.Level.bPlayersOnly = __NFUN_129__(Outer.Level.bPlayersOnly);
	return;
}

exec function CheatView(Class<Actor> aClass, optional bool bQuiet)
{
	ViewClass(aClass, bQuiet, true);
	return;
}

exec function RememberSpot()
{
	// End:0x3D
	if(__NFUN_119__(Outer.Pawn, none))
	{
		Outer.Destination = Outer.Pawn.Location;		
	}
	else
	{
		Outer.Destination = Outer.Location;
	}
	return;
}

exec function ViewSelf(optional bool bQuiet)
{
	Outer.bBehindView = false;
	Outer.bViewBot = false;
	// End:0x56
	if(__NFUN_119__(Outer.Pawn, none))
	{
		Outer.SetViewTarget(Outer.Pawn);		
	}
	else
	{
		Outer.SetViewTarget(Outer);
	}
	// End:0x97
	if(__NFUN_129__(bQuiet))
	{
		Outer.ClientMessage(Outer.OwnCamera, 'Event');
	}
	Outer.FixFOV();
	return;
}

exec function ViewPlayer(string S)
{
	local Controller P;

	P = Outer.Level.ControllerList;
	J0x1D:

	// End:0x54 [Loop If]
	if(__NFUN_119__(P, none))
	{
		// End:0x3D
		if(P.bIsPlayer)
		{
			// [Explicit Break]
			goto J0x54;
		}
		P = P.nextController;
		// [Loop Continue]
		goto J0x1D;
	}
	J0x54:

	// End:0xC0
	if(__NFUN_119__(P.Pawn, none))
	{
		Outer.ClientMessage(__NFUN_168__(Outer.ViewingFrom, P.PlayerReplicationInfo.PlayerName), 'Event');
		Outer.SetViewTarget(P.Pawn);
	}
	Outer.bBehindView = true;
	// End:0xFB
	if(Outer.bBehindView)
	{
		Outer.ViewTarget.BecomeViewTarget();
	}
	return;
}

exec function ViewActor(name ActorName)
{
	local Actor A;

	// End:0x5A
	foreach Outer.__NFUN_304__(Class'Engine.Actor', A)
	{
		// End:0x59
		if(__NFUN_254__(A.Name, ActorName))
		{
			Outer.SetViewTarget(A);
			Outer.bBehindView = true;			
			return;
		}		
	}	
	return;
}

exec function ViewFlag()
{
	local Controller C;

	C = Outer.Level.ControllerList;
	J0x1D:

	// End:0xA7 [Loop If]
	if(__NFUN_119__(C, none))
	{
		// End:0x90
		if(__NFUN_130__(__NFUN_130__(C.__NFUN_303__('AIController'), __NFUN_119__(C.PlayerReplicationInfo, none)), __NFUN_119__(C.PlayerReplicationInfo.HasFlag, none)))
		{
			Outer.SetViewTarget(C.Pawn);
			return;
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x1D;
	}
	return;
}

exec function ViewBot()
{
	local Actor first;
	local bool bFound;
	local Controller C;

	Outer.bViewBot = true;
	C = Outer.Level.ControllerList;
	J0x2E:

	// End:0xD9 [Loop If]
	if(__NFUN_119__(C, none))
	{
		// End:0xC2
		if(__NFUN_130__(C.__NFUN_303__('AIController'), __NFUN_119__(C.Pawn, none)))
		{
			// End:0x99
			if(__NFUN_132__(bFound, __NFUN_114__(first, none)))
			{
				first = C.Pawn;
				// End:0x99
				if(bFound)
				{
					// [Explicit Break]
					goto J0xD9;
				}
			}
			// End:0xC2
			if(__NFUN_114__(C.Pawn, Outer.ViewTarget))
			{
				bFound = true;
			}
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x2E;
	}
	J0xD9:

	// End:0x133
	if(__NFUN_119__(first, none))
	{
		Outer.SetViewTarget(first);
		Outer.bBehindView = true;
		Outer.ViewTarget.BecomeViewTarget();
		Outer.FixFOV();		
	}
	else
	{
		ViewSelf(true);
	}
	return;
}

exec function ViewClass(Class<Actor> aClass, optional bool bQuiet, optional bool bCheat)
{
	local Actor Other, first;
	local bool bFound;

	// End:0x54
	if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bCheat), __NFUN_119__(Outer.Level.Game, none)), __NFUN_129__(Outer.Level.Game.bCanViewOthers)))
	{
		return;
	}
	first = none;
	// End:0xC2
	foreach Outer.__NFUN_304__(aClass, Other)
	{
		// End:0xA1
		if(__NFUN_132__(bFound, __NFUN_114__(first, none)))
		{
			first = Other;
			// End:0xA1
			if(bFound)
			{
				// End:0xC2
				break;
			}
		}
		// End:0xC1
		if(__NFUN_114__(Other, Outer.ViewTarget))
		{
			bFound = true;
		}		
	}	
	// End:0x1BF
	if(__NFUN_119__(first, none))
	{
		// End:0x14A
		if(__NFUN_129__(bQuiet))
		{
			// End:0x11F
			if(__NFUN_119__(Pawn(first), none))
			{
				Outer.ClientMessage(__NFUN_168__(Outer.ViewingFrom, first.GetHumanReadableName()), 'Event');				
			}
			else
			{
				Outer.ClientMessage(__NFUN_168__(Outer.ViewingFrom, string(first)), 'Event');
			}
		}
		Outer.SetViewTarget(first);
		Outer.bBehindView = __NFUN_119__(Outer.ViewTarget, Outer);
		// End:0x1AD
		if(Outer.bBehindView)
		{
			Outer.ViewTarget.BecomeViewTarget();
		}
		Outer.FixFOV();		
	}
	else
	{
		ViewSelf(bQuiet);
	}
	return;
}

exec function Loaded()
{
	// End:0x24
	if(__NFUN_155__(int(Outer.Level.NetMode), int(NM_Standalone)))
	{
		return;
	}
	return;
}

exec function AllWeapons()
{
	// End:0x3A
	if(__NFUN_132__(__NFUN_155__(int(Outer.Level.NetMode), int(NM_Standalone)), __NFUN_114__(Outer.Pawn, none)))
	{
		return;
	}
	Outer.Pawn.GiveWeapon("WarClassLight.WeapCOGAssaultRifle");
	Outer.Pawn.GiveWeapon("WarClassLight.WeapCOGLightPlasma");
	Outer.Pawn.GiveWeapon("WarClassLight.WeapCOGPistol");
	Outer.Pawn.GiveWeapon("WarClassHeavy.WeapCOGMinigun");
	Outer.Pawn.GiveWeapon("WarClassLight.WeapGeistSniperRifle");
	Outer.Pawn.GiveWeapon("WarClassLight.WeapGeistGrenadeLauncher");
	return;
}
