class Weapon extends Inventory
	abstract
	native
	nativereplication
	notplaceable;

var Class<Ammunition> AmmoName;
var int PickupAmmoCount;
var travel Ammunition AmmoType;
var byte ReloadCount;
var bool bPointing;
var bool bWeaponUp;
var bool bChangeWeapon;
var bool bCanThrow;
var bool bRapidFire;
var bool bForceReload;
var float StopFiringTime;
var int AutoSwitchPriority;
var Vector FireOffset;
var float ShakeMag;
var float ShakeTime;
var Vector ShakeVert;
var Vector ShakeSpeed;
var Texture CrossHair;
var Powerups Affector;
var float TraceAccuracy;
var bool bMeleeWeapon;
var bool bSniping;
var float aimerror;
var float AIRating;
var float CurrentRating;
var float TraceDist;
var float MaxRange;
var Rotator AdjustedAim;
var() Sound FireSound;
var() Sound SelectSound;
var() localized string MessageNoAmmo;
var localized string WeaponDescription;
var Color NameColor;
var bool bSteadyToggle;
var bool bForceFire;
var bool bForceAltFire;
var string LeftHandedMesh;
var float DisplayFOV;
var float FlashTime;
var(MuzzleFlash) float MuzzleScale;
var(MuzzleFlash) float FlashOffsetY;
var(MuzzleFlash) float FlashOffsetX;
var(MuzzleFlash) float FlashLength;
var(MuzzleFlash) float MuzzleFlashSize;
var Texture MFTexture;
var byte FlashCount;
var bool bAutoFire;
var bool bMuzzleFlash;
var bool bSetFlashTime;
var bool bDrawMuzzleFlash;
var bool bMuzzleFlashParticles;
var Mesh MuzzleFlashMesh;
var float MuzzleFlashScale;
var Actor.ERenderStyle MuzzleFlashStyle;
var Texture MuzzleFlashTexture;
var float FireAdjust;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(__NFUN_130__(bNetOwner, bNetDirty), __NFUN_154__(int(Role), int(ROLE_Authority))))
		Affector, AmmoType, 
		ReloadCount;

	// Pos:0x023
	reliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		ClientForceReload, ClientWeaponEvent, 
		ClientWeaponSet;

	// Pos:0x030
	reliable if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		ServerAltFire, ServerFire, 
		ServerForceReload, ServerRapidFire, 
		ServerStopFiring;
}

function float RangedAttackTime()
{
	return 0.0000000;
	return;
}

function bool RecommendRangedAttack()
{
	return false;
	return;
}

function bool SplashDamage()
{
	return AmmoType.bSplashDamage;
	return;
}

function float GetDamageRadius()
{
	return AmmoType.GetDamageRadius();
	return;
}

function bool FocusOnLeader(bool bLeaderFiring)
{
	return false;
	return;
}

function float RefireRate()
{
	return AmmoType.RefireRate;
	return;
}

function bool RecommendSplashDamage()
{
	return AmmoType.bRecommendSplashDamage;
	return;
}

function FireHack(byte Mode)
{
	return;
}

function bool IsFiring()
{
	return __NFUN_130__(__NFUN_130__(__NFUN_119__(Instigator, none), __NFUN_119__(Instigator.Controller, none)), __NFUN_132__(__NFUN_155__(int(Instigator.Controller.bFire), 0), __NFUN_155__(int(Instigator.Controller.bAltFire), 0)));
	return;
}

simulated function IncrementFlashCount()
{
	__NFUN_139__(FlashCount);
	// End:0x44
	if(__NFUN_119__(WeaponAttachment(ThirdPersonActor), none))
	{
		WeaponAttachment(ThirdPersonActor).FlashCount = FlashCount;
		WeaponAttachment(ThirdPersonActor).ThirdPersonEffects();
	}
	return;
}

simulated event PostNetBeginPlay()
{
	// End:0x12
	if(__NFUN_154__(int(Role), int(ROLE_Authority)))
	{
		return;
	}
	// End:0x41
	if(__NFUN_132__(__NFUN_114__(Instigator, none), __NFUN_114__(Instigator.Controller, none)))
	{
		SetHand(0.0000000);		
	}
	else
	{
		SetHand(Instigator.Controller.Handedness);
	}
	return;
}

simulated function ForceReload()
{
	return;
}

function ServerForceReload()
{
	bForceReload = true;
	return;
}

simulated function ClientForceReload()
{
	bForceReload = true;
	__NFUN_113__('Reloading');
	return;
}

simulated function DisplayDebug(Canvas Canvas, out float YL, out float YPos)
{
	local string t;
	local name Anim;
	local float frame, Rate;

	Canvas.SetDrawColor(0, byte(255), 0);
	Canvas.__NFUN_465__(__NFUN_112__("WEAPON ", GetItemName(string(self))));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	t = __NFUN_112__(__NFUN_112__(__NFUN_112__("     STATE: ", string(__NFUN_284__())), " Timer: "), string(TimerCounter));
	// End:0xC0
	if(__NFUN_151__(int(default.ReloadCount), 0))
	{
		t = __NFUN_112__(__NFUN_112__(t, " Reload Count: "), string(ReloadCount));
	}
	Canvas.__NFUN_465__(t, false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	// End:0x1E4
	if(__NFUN_119__(Mesh, none))
	{
		GetAnimParams(0, Anim, frame, Rate);
		t = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("     AnimSequence ", string(Anim)), " Frame "), string(frame)), " Rate "), string(Rate));
		// End:0x1AD
		if(bAnimByOwner)
		{
			t = __NFUN_112__(t, " Anim by Owner");
		}
		Canvas.__NFUN_465__(t, false);
		__NFUN_184__(YPos, YL);
		Canvas.SetPos(4.0000000, YPos);
	}
	// End:0x23A
	if(__NFUN_114__(AmmoType, none))
	{
		Canvas.__NFUN_465__("ERROR - NO AMMUNITION");
		__NFUN_184__(YPos, YL);
		Canvas.SetPos(4.0000000, YPos);		
	}
	else
	{
		AmmoType.DisplayDebug(Canvas, YL, YPos);
	}
	return;
}

event TravelPostAccept()
{
	super.TravelPostAccept();
	// End:0x18
	if(__NFUN_114__(Pawn(Owner), none))
	{
		return;
	}
	// End:0x9D
	if(__NFUN_119__(AmmoName, none))
	{
		AmmoType = Ammunition(Pawn(Owner).FindInventoryType(AmmoName));
		// End:0x9D
		if(__NFUN_114__(AmmoType, none))
		{
			AmmoType = __NFUN_278__(AmmoName);
			Pawn(Owner).AddInventory(AmmoType);
			AmmoType.AmmoAmount = PickupAmmoCount;
			AmmoType.__NFUN_113__('None');
		}
	}
	// End:0xBF
	if(__NFUN_114__(self, Pawn(Owner).Weapon))
	{
		BringUp();		
	}
	else
	{
		__NFUN_113__('None');
	}
	return;
}

function SetAITarget(Actor t)
{
	return;
}

function Destroyed()
{
	super.Destroyed();
	// End:0x46
	if(__NFUN_130__(__NFUN_119__(Pawn(Owner), none), __NFUN_114__(Pawn(Owner).Weapon, self)))
	{
		Pawn(Owner).Weapon = none;
	}
	return;
}

function GiveTo(Pawn Other)
{
	super.GiveTo(Other);
	bTossedOut = false;
	Instigator = Other;
	GiveAmmo(Other);
	ClientWeaponSet(true);
	return;
}

simulated event RenderOverlays(Canvas Canvas)
{
	local Rotator newRot;
	local bool bPlayerOwner;
	local int hand;
	local PlayerController PlayerOwner;
	local float ScaledFlash;

	DrawCrossHair(Canvas);
	// End:0x18
	if(__NFUN_114__(Instigator, none))
	{
		return;
	}
	PlayerOwner = PlayerController(Instigator.Controller);
	// End:0x68
	if(__NFUN_119__(PlayerOwner, none))
	{
		bPlayerOwner = true;
		hand = int(PlayerOwner.Handedness);
		// End:0x68
		if(__NFUN_154__(hand, 2))
		{
			return;
		}
	}
	// End:0x19F
	if(__NFUN_130__(__NFUN_130__(bMuzzleFlash, bDrawMuzzleFlash), __NFUN_119__(MFTexture, none)))
	{
		// End:0xBA
		if(__NFUN_129__(bSetFlashTime))
		{
			bSetFlashTime = true;
			FlashTime = __NFUN_174__(Level.TimeSeconds, FlashLength);			
		}
		else
		{
			// End:0xDA
			if(__NFUN_176__(FlashTime, Level.TimeSeconds))
			{
				bMuzzleFlash = false;
			}
		}
		// End:0x19C
		if(bMuzzleFlash)
		{
			ScaledFlash = __NFUN_172__(__NFUN_171__(__NFUN_171__(__NFUN_171__(0.5000000, MuzzleFlashSize), MuzzleScale), Canvas.ClipX), 640.0000000);
			Canvas.SetPos(__NFUN_174__(__NFUN_175__(__NFUN_171__(0.5000000, Canvas.ClipX), ScaledFlash), __NFUN_171__(__NFUN_171__(Canvas.ClipX, float(hand)), FlashOffsetX)), __NFUN_174__(__NFUN_175__(__NFUN_171__(0.5000000, Canvas.ClipY), ScaledFlash), __NFUN_171__(Canvas.ClipY, FlashOffsetY)));
			DrawMuzzleFlash(Canvas);
		}		
	}
	else
	{
		bSetFlashTime = false;
	}
	__NFUN_267__(__NFUN_215__(Instigator.Location, Instigator.CalcDrawOffset(self)));
	newRot = Instigator.GetViewRotation();
	// End:0x206
	if(__NFUN_154__(hand, 0))
	{
		newRot.Roll = __NFUN_144__(2, default.Rotation.Roll);		
	}
	else
	{
		newRot.Roll = __NFUN_144__(default.Rotation.Roll, hand);
	}
	__NFUN_299__(newRot);
	Canvas.__NFUN_467__(self, false, false, DisplayFOV);
	return;
}

simulated function DrawCrossHair(Canvas Canvas)
{
	local float XLength;

	// End:0x0D
	if(__NFUN_114__(CrossHair, none))
	{
		return;
	}
	XLength = 32.0000000;
	Canvas.bNoSmooth = false;
	Canvas.SetPos(__NFUN_171__(0.5030000, __NFUN_175__(Canvas.ClipX, XLength)), __NFUN_171__(0.5040000, __NFUN_175__(Canvas.ClipY, XLength)));
	Canvas.Style = 3;
	Canvas.SetDrawColor(byte(255), byte(255), byte(255));
	Canvas.__NFUN_466__(CrossHair, XLength, XLength, 0.0000000, 0.0000000, 32.0000000, 32.0000000);
	Canvas.bNoSmooth = true;
	Canvas.Style = Style;
	return;
}

simulated function DrawMuzzleFlash(Canvas Canvas)
{
	local float Scale, ULength, VLength, UStart, VStart;

	Scale = __NFUN_172__(__NFUN_171__(MuzzleScale, Canvas.ClipX), 640.0000000);
	Canvas.Style = 3;
	ULength = float(MFTexture.USize);
	// End:0x72
	if(__NFUN_176__(__NFUN_195__(), 0.5000000))
	{
		UStart = ULength;
		ULength = __NFUN_171__(-1.0000000, ULength);
	}
	VLength = float(MFTexture.VSize);
	// End:0xB1
	if(__NFUN_176__(__NFUN_195__(), 0.5000000))
	{
		VStart = VLength;
		VLength = __NFUN_171__(-1.0000000, VLength);
	}
	Canvas.__NFUN_466__(MFTexture, __NFUN_171__(Scale, float(MFTexture.USize)), __NFUN_171__(Scale, float(MFTexture.VSize)), UStart, VStart, ULength, VLength);
	Canvas.Style = 1;
	return;
}

function bool CanAttack(Actor Other)
{
	local float MaxDist, CheckDist;
	local Vector HitLocation, HitNormal, X, Y, Z, projStart;

	local Actor HitActor;

	// End:0x23
	if(__NFUN_132__(__NFUN_114__(Instigator, none), __NFUN_114__(Instigator.Controller, none)))
	{
		return false;
	}
	MaxDist = MaxRange;
	// End:0x4B
	if(AmmoType.bInstantHit)
	{
		MaxDist = TraceDist;
	}
	// End:0x77
	if(__NFUN_177__(__NFUN_225__(__NFUN_216__(Instigator.Location, Other.Location)), MaxDist))
	{
		return false;
	}
	// End:0x98
	if(__NFUN_129__(Instigator.Controller.__NFUN_514__(Other)))
	{
		return false;
	}
	__NFUN_229__(Instigator.Controller.Rotation, X, Y, Z);
	projStart = GetFireStart(X, Y, Z);
	// End:0x136
	if(AmmoType.bInstantHit)
	{
		HitActor = __NFUN_277__(HitLocation, HitNormal, __NFUN_215__(Other.Location, __NFUN_213__(Other.CollisionHeight, vect(0.0000000, 0.0000000, 0.7000000))), projStart, true);		
	}
	else
	{
		CheckDist = __NFUN_246__(AmmoType.ProjectileClass.default.Speed, 600.0000000, __NFUN_225__(__NFUN_216__(Other.Location, Location)));
		HitActor = __NFUN_277__(HitLocation, HitNormal, __NFUN_215__(projStart, __NFUN_213__(CheckDist, __NFUN_226__(__NFUN_216__(__NFUN_215__(Other.Location, __NFUN_213__(Other.CollisionHeight, vect(0.0000000, 0.0000000, 0.7000000))), Location)))), projStart, true);
	}
	// End:0x230
	if(__NFUN_132__(__NFUN_132__(__NFUN_114__(HitActor, none), __NFUN_114__(HitActor, Other)), __NFUN_130__(__NFUN_119__(Pawn(HitActor), none), __NFUN_129__(Instigator.Controller.SameTeamAs(Pawn(HitActor).Controller)))))
	{
		return true;
	}
	return false;
	return;
}

function float AmmoStatus()
{
	return __NFUN_172__(float(AmmoType.AmmoAmount), float(AmmoType.MaxAmmo));
	return;
}

simulated function bool HasAmmo()
{
	return AmmoType.HasAmmo();
	return;
}

function bool SplashJump()
{
	return false;
	return;
}

function float GetRating()
{
	return RateSelf();
	return;
}

simulated function float RateSelf()
{
	// End:0x19
	if(__NFUN_129__(HasAmmo()))
	{
		CurrentRating = -2.0000000;		
	}
	else
	{
		// End:0x36
		if(__NFUN_114__(Instigator.Controller, none))
		{
			return 0.0000000;			
		}
		else
		{
			CurrentRating = Instigator.Controller.RateWeapon(self);
		}
	}
	return CurrentRating;
	return;
}

function float GetAIRating()
{
	return AIRating;
	return;
}

function float SuggestAttackStyle()
{
	return 0.0000000;
	return;
}

function float SuggestDefenseStyle()
{
	return 0.0000000;
	return;
}

function ClientWeaponEvent(name EventType)
{
	return;
}

function bool HandlePickupQuery(Pickup item)
{
	local int OldAmmo, NewAmmo;
	local Pawn P;

	// End:0x160
	if(__NFUN_114__(item.InventoryType, Class))
	{
		// End:0x64
		if(__NFUN_130__(WeaponPickup(item).bWeaponStay, __NFUN_132__(__NFUN_114__(item.Inventory, none), item.Inventory.bTossedOut)))
		{
			return true;
		}
		P = Pawn(Owner);
		// End:0x145
		if(__NFUN_119__(AmmoType, none))
		{
			OldAmmo = AmmoType.AmmoAmount;
			// End:0xCC
			if(__NFUN_119__(item.Inventory, none))
			{
				NewAmmo = Weapon(item.Inventory).PickupAmmoCount;				
			}
			else
			{
				NewAmmo = Class<Weapon>(item.InventoryType).default.PickupAmmoCount;
			}
			// End:0x145
			if(__NFUN_130__(__NFUN_130__(AmmoType.AddAmmo(NewAmmo), __NFUN_154__(OldAmmo, 0)), __NFUN_119__(P.Weapon.Class, item.InventoryType)))
			{
				ClientWeaponSet(true);
			}
		}
		item.AnnouncePickup(Pawn(Owner));
		return true;
	}
	// End:0x16D
	if(__NFUN_114__(Inventory, none))
	{
		return false;
	}
	return Inventory.HandlePickupQuery(item);
	return;
}

simulated function SetHand(float hand)
{
	// End:0x30
	if(__NFUN_180__(hand, float(2)))
	{
		PlayerViewOffset.Y = 0.0000000;
		FireOffset.Y = 0.0000000;
		return;
	}
	LinkMesh(default.Mesh);
	// End:0x9F
	if(__NFUN_180__(hand, float(0)))
	{
		PlayerViewOffset.X = __NFUN_171__(default.PlayerViewOffset.X, 0.8800000);
		PlayerViewOffset.Y = __NFUN_171__(-0.2000000, default.PlayerViewOffset.Y);
		PlayerViewOffset.Z = __NFUN_171__(default.PlayerViewOffset.Z, 1.1200000);		
	}
	else
	{
		PlayerViewOffset.X = default.PlayerViewOffset.X;
		PlayerViewOffset.Y = __NFUN_171__(default.PlayerViewOffset.Y, hand);
		PlayerViewOffset.Z = default.PlayerViewOffset.Z;
		// End:0x11F
		if(__NFUN_130__(__NFUN_180__(hand, float(-1)), __NFUN_123__(LeftHandedMesh, "")))
		{
			LinkMesh(Mesh(DynamicLoadObject(LeftHandedMesh, Class'Engine.Mesh')));
		}
	}
	FireOffset.Y = __NFUN_171__(default.FireOffset.Y, hand);
	return;
}

simulated function Weapon WeaponChange(byte f)
{
	local Weapon newWeapon;

	// End:0x9E
	if(__NFUN_154__(int(InventoryGroup), int(f)))
	{
		// End:0x99
		if(__NFUN_129__(AmmoType.HasAmmo()))
		{
			// End:0x3C
			if(__NFUN_114__(Inventory, none))
			{
				newWeapon = none;				
			}
			else
			{
				newWeapon = Inventory.WeaponChange(f);
			}
			// End:0x90
			if(__NFUN_130__(__NFUN_114__(newWeapon, none), Instigator.IsHumanControlled()))
			{
				Instigator.ClientMessage(__NFUN_112__(ItemName, MessageNoAmmo));
			}
			return newWeapon;			
		}
		else
		{
			return self;
		}		
	}
	else
	{
		// End:0xAE
		if(__NFUN_114__(Inventory, none))
		{
			return none;			
		}
		else
		{
			return Inventory.WeaponChange(f);
		}
	}
	return;
}

simulated function Weapon PrevWeapon(Weapon CurrentChoice, Weapon CurrentWeapon)
{
	// End:0x187
	if(AmmoType.HasAmmo())
	{
		// End:0x32
		if(__NFUN_114__(CurrentChoice, none))
		{
			// End:0x2F
			if(__NFUN_119__(CurrentWeapon, self))
			{
				CurrentChoice = self;
			}			
		}
		else
		{
			// End:0xD4
			if(__NFUN_154__(int(InventoryGroup), int(CurrentChoice.InventoryGroup)))
			{
				// End:0xAE
				if(__NFUN_154__(int(InventoryGroup), int(CurrentWeapon.InventoryGroup)))
				{
					// End:0xAB
					if(__NFUN_130__(__NFUN_150__(int(GroupOffset), int(CurrentWeapon.GroupOffset)), __NFUN_151__(int(GroupOffset), int(CurrentChoice.GroupOffset))))
					{
						CurrentChoice = self;
					}					
				}
				else
				{
					// End:0xD1
					if(__NFUN_151__(int(GroupOffset), int(CurrentChoice.GroupOffset)))
					{
						CurrentChoice = self;
					}
				}				
			}
			else
			{
				// End:0x13D
				if(__NFUN_151__(int(InventoryGroup), int(CurrentChoice.InventoryGroup)))
				{
					// End:0x13A
					if(__NFUN_132__(__NFUN_150__(int(InventoryGroup), int(CurrentWeapon.InventoryGroup)), __NFUN_151__(int(CurrentChoice.InventoryGroup), int(CurrentWeapon.InventoryGroup))))
					{
						CurrentChoice = self;
					}					
				}
				else
				{
					// End:0x187
					if(__NFUN_130__(__NFUN_151__(int(CurrentChoice.InventoryGroup), int(CurrentWeapon.InventoryGroup)), __NFUN_150__(int(InventoryGroup), int(CurrentWeapon.InventoryGroup))))
					{
						CurrentChoice = self;
					}
				}
			}
		}
	}
	// End:0x19B
	if(__NFUN_114__(Inventory, none))
	{
		return CurrentChoice;		
	}
	else
	{
		return Inventory.PrevWeapon(CurrentChoice, CurrentWeapon);
	}
	return;
}

simulated function Weapon NextWeapon(Weapon CurrentChoice, Weapon CurrentWeapon)
{
	// End:0x187
	if(AmmoType.HasAmmo())
	{
		// End:0x32
		if(__NFUN_114__(CurrentChoice, none))
		{
			// End:0x2F
			if(__NFUN_119__(CurrentWeapon, self))
			{
				CurrentChoice = self;
			}			
		}
		else
		{
			// End:0xD4
			if(__NFUN_154__(int(InventoryGroup), int(CurrentChoice.InventoryGroup)))
			{
				// End:0xAE
				if(__NFUN_154__(int(InventoryGroup), int(CurrentWeapon.InventoryGroup)))
				{
					// End:0xAB
					if(__NFUN_130__(__NFUN_151__(int(GroupOffset), int(CurrentWeapon.GroupOffset)), __NFUN_150__(int(GroupOffset), int(CurrentChoice.GroupOffset))))
					{
						CurrentChoice = self;
					}					
				}
				else
				{
					// End:0xD1
					if(__NFUN_150__(int(GroupOffset), int(CurrentChoice.GroupOffset)))
					{
						CurrentChoice = self;
					}
				}				
			}
			else
			{
				// End:0x13D
				if(__NFUN_150__(int(InventoryGroup), int(CurrentChoice.InventoryGroup)))
				{
					// End:0x13A
					if(__NFUN_132__(__NFUN_151__(int(InventoryGroup), int(CurrentWeapon.InventoryGroup)), __NFUN_150__(int(CurrentChoice.InventoryGroup), int(CurrentWeapon.InventoryGroup))))
					{
						CurrentChoice = self;
					}					
				}
				else
				{
					// End:0x187
					if(__NFUN_130__(__NFUN_150__(int(CurrentChoice.InventoryGroup), int(CurrentWeapon.InventoryGroup)), __NFUN_151__(int(InventoryGroup), int(CurrentWeapon.InventoryGroup))))
					{
						CurrentChoice = self;
					}
				}
			}
		}
	}
	// End:0x19B
	if(__NFUN_114__(Inventory, none))
	{
		return CurrentChoice;		
	}
	else
	{
		return Inventory.NextWeapon(CurrentChoice, CurrentWeapon);
	}
	return;
}

simulated function AnimEnd(int Channel)
{
	// End:0x1F
	if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
	{
		PlayIdleAnim();
	}
	return;
}

function GiveAmmo(Pawn Other)
{
	// End:0x0D
	if(__NFUN_114__(AmmoName, none))
	{
		return;
	}
	AmmoType = Ammunition(Other.FindInventoryType(AmmoName));
	// End:0x4E
	if(__NFUN_119__(AmmoType, none))
	{
		AmmoType.AddAmmo(PickupAmmoCount);		
	}
	else
	{
		AmmoType = __NFUN_278__(AmmoName);
		Other.AddInventory(AmmoType);
		AmmoType.AmmoAmount = PickupAmmoCount;
	}
	return;
}

simulated function float SwitchPriority()
{
	// End:0x1E
	if(__NFUN_129__(Instigator.IsHumanControlled()))
	{
		return RateSelf();		
	}
	else
	{
		// End:0x5D
		if(__NFUN_129__(AmmoType.HasAmmo()))
		{
			// End:0x54
			if(__NFUN_114__(Pawn(Owner).Weapon, self))
			{
				return -0.5000000;				
			}
			else
			{
				return -1.0000000;
			}			
		}
		else
		{
			return float(default.AutoSwitchPriority);
		}
	}
	return;
}

simulated function ClientWeaponSet(bool bOptionalSet)
{
	local Weapon W;

	Instigator = Pawn(Owner);
	// End:0x27
	if(__NFUN_114__(Instigator, none))
	{
		__NFUN_113__('PendingClientWeaponSet');
		return;		
	}
	else
	{
		// End:0x39
		if(__NFUN_281__('PendingClientWeaponSet'))
		{
			__NFUN_113__('None');
		}
	}
	// End:0x4F
	if(__NFUN_114__(Instigator.Weapon, self))
	{
		return;
	}
	// End:0x84
	if(__NFUN_114__(Instigator.Weapon, none))
	{
		Instigator.PendingWeapon = self;
		Instigator.ChangedWeapon();
		return;
	}
	// End:0xC5
	if(__NFUN_130__(bOptionalSet, __NFUN_130__(Instigator.IsHumanControlled(), PlayerController(Instigator.Controller).bNeverSwitchOnPickup)))
	{
		return;
	}
	// End:0x146
	if(__NFUN_176__(Instigator.Weapon.SwitchPriority(), SwitchPriority()))
	{
		W = Instigator.PendingWeapon;
		Instigator.PendingWeapon = self;
		__NFUN_113__('None');
		// End:0x144
		if(__NFUN_129__(Instigator.Weapon.PutDown()))
		{
			Instigator.PendingWeapon = W;
		}
		return;
	}
	__NFUN_113__('None');
	return;
}

simulated function Weapon RecommendWeapon(out float rating)
{
	local Weapon Recommended;
	local float oldRating;

	// End:0x21
	if(Instigator.IsHumanControlled())
	{
		rating = SwitchPriority();		
	}
	else
	{
		rating = RateSelf();
		// End:0x80
		if(__NFUN_130__(__NFUN_130__(__NFUN_114__(self, Instigator.Weapon), __NFUN_119__(Instigator.Controller.Enemy, none)), AmmoType.HasAmmo()))
		{
			__NFUN_184__(rating, 0.2100000);
		}
		__NFUN_184__(rating, Instigator.Controller.WeaponPreference(self));
	}
	// End:0xF2
	if(__NFUN_119__(Inventory, none))
	{
		Recommended = Inventory.RecommendWeapon(oldRating);
		// End:0xF2
		if(__NFUN_130__(__NFUN_119__(Recommended, none), __NFUN_177__(oldRating, rating)))
		{
			rating = oldRating;
			return Recommended;
		}
	}
	return self;
	return;
}

function DropFrom(Vector StartLocation)
{
	AIRating = default.AIRating;
	bMuzzleFlash = false;
	// End:0x42
	if(__NFUN_119__(AmmoType, none))
	{
		PickupAmmoCount = AmmoType.AmmoAmount;
		AmmoType.AmmoAmount = 0;
	}
	super.DropFrom(StartLocation);
	return;
}

simulated function bool RepeatFire()
{
	return bRapidFire;
	return;
}

function ServerStopFiring()
{
	StopFiringTime = Level.TimeSeconds;
	return;
}

function ServerRapidFire()
{
	ServerFire();
	// End:0x2C
	if(__NFUN_281__('NormalFire'))
	{
		StopFiringTime = __NFUN_174__(Level.TimeSeconds, 0.6000000);
	}
	return;
}

function ServerFire()
{
	// End:0x3F
	if(__NFUN_114__(AmmoType, none))
	{
		__NFUN_231__(__NFUN_112__(__NFUN_112__("WARNING ", string(self)), " HAS NO AMMO!!!"));
		GiveAmmo(Pawn(Owner));
	}
	// End:0x95
	if(AmmoType.HasAmmo())
	{
		__NFUN_113__('NormalFire');
		__NFUN_140__(ReloadCount);
		// End:0x89
		if(AmmoType.bInstantHit)
		{
			TraceFire(TraceAccuracy, 0.0000000, 0.0000000);			
		}
		else
		{
			ProjectileFire();
		}
		LocalFire();
	}
	return;
}

simulated function Fire(float Value)
{
	// End:0x16
	if(__NFUN_129__(AmmoType.HasAmmo()))
	{
		return;
	}
	// End:0x2A
	if(__NFUN_129__(RepeatFire()))
	{
		ServerFire();		
	}
	else
	{
		// End:0x6A
		if(__NFUN_176__(StopFiringTime, __NFUN_174__(Level.TimeSeconds, 0.3000000)))
		{
			StopFiringTime = __NFUN_174__(Level.TimeSeconds, 0.6000000);
			ServerRapidFire();
		}
	}
	// End:0x8E
	if(__NFUN_150__(int(Role), int(ROLE_Authority)))
	{
		__NFUN_140__(ReloadCount);
		LocalFire();
		__NFUN_113__('ClientFiring');
	}
	return;
}

simulated function LocalFire()
{
	local PlayerController P;

	bPointing = true;
	// End:0x78
	if(__NFUN_130__(__NFUN_119__(Instigator, none), Instigator.IsLocallyControlled()))
	{
		P = PlayerController(Instigator.Controller);
		// End:0x78
		if(__NFUN_119__(P, none))
		{
			P.ShakeView(ShakeTime, ShakeMag, ShakeVert, 120000.0000000, ShakeSpeed, 1.0000000);
		}
	}
	// End:0x92
	if(__NFUN_119__(Affector, none))
	{
		Affector.FireEffect();
	}
	PlayFiring();
	return;
}

function ServerAltFire()
{
	// End:0x14
	if(__NFUN_129__(__NFUN_281__('Idle')))
	{
		__NFUN_113__('Idle');
	}
	return;
}

simulated function AltFire(float Value)
{
	// End:0x14
	if(__NFUN_129__(__NFUN_281__('Idle')))
	{
		__NFUN_113__('Idle');
	}
	ServerAltFire();
	return;
}

simulated function PlayFiring()
{
	return;
}

simulated function Vector GetFireStart(Vector X, Vector Y, Vector Z)
{
	return __NFUN_215__(__NFUN_215__(__NFUN_215__(__NFUN_215__(Instigator.Location, Instigator.EyePosition()), __NFUN_213__(FireOffset.X, X)), __NFUN_213__(FireOffset.Y, Y)), __NFUN_213__(FireOffset.Z, Z));
	return;
}

function ProjectileFire()
{
	local Vector Start, X, Y, Z;

	Owner.__NFUN_512__(1.0000000);
	__NFUN_229__(Instigator.GetViewRotation(), X, Y, Z);
	Start = GetFireStart(X, Y, Z);
	AdjustedAim = Instigator.AdjustAim(AmmoType, Start, int(aimerror));
	AmmoType.SpawnProjectile(Start, AdjustedAim);
	return;
}

function TraceFire(float Accuracy, float YOffset, float ZOffset)
{
	local Vector HitLocation, HitNormal, StartTrace, EndTrace, X, Y,
		Z;

	local Actor Other;

	Owner.__NFUN_512__(1.0000000);
	__NFUN_229__(Instigator.GetViewRotation(), X, Y, Z);
	StartTrace = GetFireStart(X, Y, Z);
	AdjustedAim = Instigator.AdjustAim(AmmoType, StartTrace, int(__NFUN_171__(float(2), aimerror)));
	EndTrace = __NFUN_215__(__NFUN_215__(StartTrace, __NFUN_212__(__NFUN_213__(__NFUN_174__(YOffset, __NFUN_171__(Accuracy, __NFUN_175__(__NFUN_195__(), 0.5000000))), Y), float(1000))), __NFUN_212__(__NFUN_213__(__NFUN_174__(ZOffset, __NFUN_171__(Accuracy, __NFUN_175__(__NFUN_195__(), 0.5000000))), Z), float(1000)));
	X = Vector(AdjustedAim);
	__NFUN_223__(EndTrace, __NFUN_213__(TraceDist, X));
	Other = __NFUN_277__(HitLocation, HitNormal, EndTrace, StartTrace, true);
	AmmoType.ProcessTraceHit(self, Other, HitLocation, HitNormal, X, Y, Z);
	return;
}

simulated function bool NeedsToReload()
{
	return __NFUN_132__(bForceReload, __NFUN_130__(__NFUN_151__(int(default.ReloadCount), 0), __NFUN_154__(int(ReloadCount), 0)));
	return;
}

function bool BotFire(bool bFinished, optional name FiringMode)
{
	// End:0x1A
	if(__NFUN_130__(__NFUN_129__(bFinished), __NFUN_129__(IsIdle())))
	{
		return false;
	}
	Instigator.Controller.bFire = 1;
	Instigator.Controller.bAltFire = 0;
	// End:0x62
	if(__NFUN_129__(RepeatFire()))
	{
		global.ServerFire();		
	}
	else
	{
		// End:0xA2
		if(__NFUN_176__(StopFiringTime, __NFUN_174__(Level.TimeSeconds, 0.3000000)))
		{
			StopFiringTime = __NFUN_174__(Level.TimeSeconds, 0.6000000);
			global.ServerRapidFire();
		}
	}
	return true;
	return;
}

function bool IsIdle()
{
	return false;
	return;
}

function Finish()
{
	local bool bForce, bForceAlt;

	// End:0x26
	if(__NFUN_130__(NeedsToReload(), AmmoType.HasAmmo()))
	{
		__NFUN_113__('Reloading');
		return;
	}
	bForce = bForceFire;
	bForceAlt = bForceAltFire;
	bForceFire = false;
	bForceAltFire = false;
	// End:0x62
	if(bChangeWeapon)
	{
		__NFUN_113__('DownWeapon');
		return;
	}
	// End:0x8C
	if(__NFUN_132__(__NFUN_114__(Instigator, none), __NFUN_114__(Instigator.Controller, none)))
	{
		__NFUN_113__('None');
		return;
	}
	// End:0x14E
	if(__NFUN_129__(Instigator.IsHumanControlled()))
	{
		// End:0xE8
		if(__NFUN_129__(AmmoType.HasAmmo()))
		{
			Instigator.Controller.SwitchToBestWeapon();
			// End:0xDF
			if(bChangeWeapon)
			{
				__NFUN_113__('DownWeapon');				
			}
			else
			{
				__NFUN_113__('Idle');
			}
			return;
		}
		// End:0x14E
		if(__NFUN_119__(AIController(Instigator.Controller), none))
		{
			// End:0x14C
			if(__NFUN_129__(AIController(Instigator.Controller).WeaponFireAgain(AmmoType.RefireRate, true)))
			{
				// End:0x145
				if(bChangeWeapon)
				{
					__NFUN_113__('DownWeapon');					
				}
				else
				{
					__NFUN_113__('Idle');
				}
			}
			return;
		}
	}
	// End:0x17E
	if(__NFUN_130__(__NFUN_129__(AmmoType.HasAmmo()), Instigator.IsLocallyControlled()))
	{
		SwitchToWeaponWithAmmo();
		return;
	}
	// End:0x19C
	if(__NFUN_119__(Instigator.Weapon, self))
	{
		__NFUN_113__('Idle');		
	}
	else
	{
		// End:0x1DC
		if(__NFUN_132__(__NFUN_132__(__NFUN_177__(StopFiringTime, Level.TimeSeconds), bForce), Instigator.PressingFire()))
		{
			global.ServerFire();			
		}
		else
		{
			// End:0x202
			if(__NFUN_132__(bForceAlt, Instigator.PressingAltFire()))
			{
				CauseAltFire();				
			}
			else
			{
				__NFUN_113__('Idle');
			}
		}
	}
	return;
}

function SwitchToWeaponWithAmmo()
{
	Instigator.Controller.SwitchToBestWeapon();
	// End:0x2D
	if(bChangeWeapon)
	{
		__NFUN_113__('DownWeapon');
		return;		
	}
	else
	{
		__NFUN_113__('Idle');
	}
	return;
}

function CauseAltFire()
{
	global.ServerAltFire();
	return;
}

simulated function ClientFinish()
{
	// End:0x2A
	if(__NFUN_132__(__NFUN_114__(Instigator, none), __NFUN_114__(Instigator.Controller, none)))
	{
		__NFUN_113__('None');
		return;
	}
	// End:0x50
	if(__NFUN_130__(NeedsToReload(), AmmoType.HasAmmo()))
	{
		__NFUN_113__('Reloading');
		return;
	}
	// End:0x96
	if(__NFUN_129__(AmmoType.HasAmmo()))
	{
		Instigator.Controller.SwitchToBestWeapon();
		// End:0x96
		if(__NFUN_129__(bChangeWeapon))
		{
			PlayIdleAnim();
			__NFUN_113__('Idle');
			return;
		}
	}
	// End:0xA9
	if(bChangeWeapon)
	{
		__NFUN_113__('DownWeapon');		
	}
	else
	{
		// End:0xC9
		if(Instigator.PressingFire())
		{
			global.Fire(0.0000000);			
		}
		else
		{
			// End:0xE9
			if(Instigator.PressingAltFire())
			{
				global.AltFire(0.0000000);				
			}
			else
			{
				PlayIdleAnim();
				__NFUN_113__('Idle');
			}
		}
	}
	return;
}

function CheckAnimating()
{
	return;
}

simulated function BringUp()
{
	// End:0x51
	if(Instigator.IsHumanControlled())
	{
		SetHand(PlayerController(Instigator.Controller).Handedness);
		PlayerController(Instigator.Controller).EndZoom();
	}
	bWeaponUp = false;
	PlaySelect();
	__NFUN_113__('active');
	return;
}

simulated function bool PutDown()
{
	bChangeWeapon = true;
	return true;
	return;
}

simulated function TweenDown()
{
	local name Anim;
	local float frame, Rate;

	// End:0x44
	if(__NFUN_130__(__NFUN_282__(), AnimIsInGroup(0, 'Select')))
	{
		GetAnimParams(0, Anim, frame, Rate);
		__NFUN_294__(Anim, __NFUN_171__(frame, 0.4000000));		
	}
	else
	{
		__NFUN_259__('Down', 1.0000000, 0.0500000);
	}
	return;
}

simulated function PlayReloading()
{
	AnimEnd(0);
	return;
}

simulated function PlaySelect()
{
	bForceFire = false;
	bForceAltFire = false;
	// End:0x3D
	if(__NFUN_132__(__NFUN_129__(__NFUN_282__()), __NFUN_129__(AnimIsInGroup(0, 'Select'))))
	{
		__NFUN_259__('Select', 1.0000000, 0.0000000);
	}
	Owner.__NFUN_264__(SelectSound, 1, 1.0000000);
	return;
}

simulated function PlayPostSelect()
{
	__NFUN_113__('Idle');
	AnimEnd(0);
	return;
}

simulated function PlayIdleAnim()
{
	return;
}

simulated function zoom()
{
	return;
}

state NormalFire
{
	function CheckAnimating()
	{
		// End:0x38
		if(__NFUN_129__(__NFUN_282__()))
		{
			__NFUN_232__(__NFUN_112__(string(self), " stuck in NormalFire and not animating!"));
		}
		return;
	}

	function ServerFire()
	{
		bForceFire = true;
		return;
	}

	function ServerAltFire()
	{
		bForceAltFire = true;
		return;
	}

	function Fire(float f)
	{
		return;
	}

	function AltFire(float f)
	{
		return;
	}

	function AnimEnd(int Channel)
	{
		Finish();
		CheckAnimating();
		return;
	}

	function EndState()
	{
		StopFiringTime = Level.TimeSeconds;
		return;
	}
Begin:

	__NFUN_256__(0.0000000);
	stop;			
}

state Idle
{
	function bool IsIdle()
	{
		return true;
		return;
	}

	simulated function ForceReload()
	{
		ServerForceReload();
		return;
	}

	function ServerForceReload()
	{
		// End:0x19
		if(AmmoType.HasAmmo())
		{
			__NFUN_113__('Reloading');
		}
		return;
	}

	simulated function AnimEnd(int Channel)
	{
		PlayIdleAnim();
		return;
	}

	simulated function bool PutDown()
	{
		__NFUN_113__('DownWeapon');
		return true;
		return;
	}
Begin:

	bPointing = false;
	// End:0x2C
	if(__NFUN_130__(NeedsToReload(), AmmoType.HasAmmo()))
	{
		__NFUN_113__('Reloading');
	}
	// End:0x58
	if(__NFUN_129__(AmmoType.HasAmmo()))
	{
		Instigator.Controller.SwitchToBestWeapon();
	}
	// End:0x75
	if(Instigator.PressingFire())
	{
		Fire(0.0000000);
	}
	// End:0x92
	if(Instigator.PressingAltFire())
	{
		AltFire(0.0000000);
	}
	PlayIdleAnim();
	stop;			
}

state Reloading
{
	function ServerForceReload()
	{
		return;
	}

	function ClientForceReload()
	{
		return;
	}

	function Fire(float Value)
	{
		return;
	}

	function AltFire(float Value)
	{
		return;
	}

	function ServerFire()
	{
		bForceFire = true;
		return;
	}

	function ServerAltFire()
	{
		bForceAltFire = true;
		return;
	}

	simulated function bool PutDown()
	{
		bChangeWeapon = true;
		return true;
		return;
	}

	simulated function BeginState()
	{
		// End:0x2A
		if(__NFUN_129__(bForceReload))
		{
			// End:0x24
			if(__NFUN_150__(int(Role), int(ROLE_Authority)))
			{
				ServerForceReload();				
			}
			else
			{
				ClientForceReload();
			}
		}
		bForceReload = false;
		PlayReloading();
		return;
	}

	simulated function AnimEnd(int Channel)
	{
		ReloadCount = default.ReloadCount;
		// End:0x24
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ClientFinish();			
		}
		else
		{
			Finish();
		}
		CheckAnimating();
		return;
	}

	function CheckAnimating()
	{
		// End:0x37
		if(__NFUN_129__(__NFUN_282__()))
		{
			__NFUN_232__(__NFUN_112__(string(self), " stuck in Reloading and not animating!"));
		}
		return;
	}
	stop;
}

state active
{
	function Fire(float Value)
	{
		return;
	}

	function AltFire(float Value)
	{
		return;
	}

	function ServerFire()
	{
		bForceFire = true;
		return;
	}

	function ServerAltFire()
	{
		bForceAltFire = true;
		return;
	}

	simulated function bool PutDown()
	{
		local name Anim;
		local float frame, Rate;

		GetAnimParams(0, Anim, frame, Rate);
		// End:0x3A
		if(__NFUN_132__(bWeaponUp, __NFUN_176__(frame, 0.7500000)))
		{
			__NFUN_113__('DownWeapon');			
		}
		else
		{
			bChangeWeapon = true;
		}
		return true;
		return;
	}

	simulated function BeginState()
	{
		Instigator = Pawn(Owner);
		bForceFire = false;
		bForceAltFire = false;
		bWeaponUp = false;
		bChangeWeapon = false;
		return;
	}

	simulated function EndState()
	{
		bForceFire = false;
		bForceAltFire = false;
		return;
	}

	simulated function AnimEnd(int Channel)
	{
		// End:0x10
		if(bChangeWeapon)
		{
			__NFUN_113__('DownWeapon');
		}
		// End:0x3E
		if(__NFUN_114__(Owner, none))
		{
			__NFUN_231__(__NFUN_112__(string(self), " no owner"));
			global.AnimEnd(0);
			__NFUN_113__('None');			
		}
		else
		{
			// End:0x69
			if(bWeaponUp)
			{
				// End:0x60
				if(__NFUN_154__(int(Role), int(ROLE_Authority)))
				{
					Finish();					
				}
				else
				{
					ClientFinish();
				}				
			}
			else
			{
				PlayPostSelect();
				bWeaponUp = true;
			}
		}
		CheckAnimating();
		return;
	}

	function CheckAnimating()
	{
		// End:0x34
		if(__NFUN_129__(__NFUN_282__()))
		{
			__NFUN_232__(__NFUN_112__(string(self), " stuck in Active and not animating!"));
		}
		return;
	}
	stop;
}

state DownWeapon
{
	function Fire(float Value)
	{
		return;
	}

	function AltFire(float Value)
	{
		return;
	}

	function ServerFire()
	{
		return;
	}

	function ServerAltFire()
	{
		return;
	}

	simulated function bool PutDown()
	{
		return true;
		return;
	}

	simulated function AnimEnd(int Channel)
	{
		Pawn(Owner).ChangedWeapon();
		return;
	}

	simulated function BeginState()
	{
		bChangeWeapon = false;
		bMuzzleFlash = false;
		TweenDown();
		return;
	}
	stop;
}

state ClientFiring
{
	function Fire(float Value)
	{
		return;
	}

	function AltFire(float Value)
	{
		return;
	}

	simulated function AnimEnd(int Channel)
	{
		ClientFinish();
		CheckAnimating();
		return;
	}

	function CheckAnimating()
	{
		// End:0x3A
		if(__NFUN_129__(__NFUN_282__()))
		{
			__NFUN_232__(__NFUN_112__(string(self), " stuck in ClientFiring and not animating!"));
		}
		return;
	}

	simulated function EndState()
	{
		// End:0x1C
		if(__NFUN_130__(RepeatFire(), __NFUN_129__(bPendingDelete)))
		{
			ServerStopFiring();
		}
		return;
	}
	stop;
}

state PendingClientWeaponSet
{
	simulated function Timer()
	{
		// End:0x17
		if(__NFUN_119__(Pawn(Owner), none))
		{
			ClientWeaponSet(false);
		}
		return;
	}

	simulated function BeginState()
	{
		__NFUN_280__(0.0500000, true);
		return;
	}

	simulated function EndState()
	{
		__NFUN_280__(0.0000000, false);
		return;
	}
	stop;
}

defaultproperties
{
	bCanThrow=true
	AutoSwitchPriority=1
	ShakeMag=300.0000000
	ShakeTime=0.1000000
	ShakeVert=(X=0.0000000,Y=0.0000000,Z=5.0000000)
	ShakeSpeed=(X=100.0000000,Y=100.0000000,Z=100.0000000)
	aimerror=550.0000000
	AIRating=0.5000000
	CurrentRating=0.5000000
	TraceDist=10000.0000000
	MaxRange=8000.0000000
	MessageNoAmmo=" has no ammo."
	NameColor=(R=255,G=255,B=255,A=255)
	DisplayFOV=85.0000000
	MuzzleScale=1.0000000
	FlashLength=0.1000000
	FireAdjust=1.0000000
	InventoryGroup=1
	PlayerViewOffset=(X=30.0000000,Y=0.0000000,Z=-5.0000000)
	AttachmentClass=Class'Engine.WeaponAttachment'
	Icon=Texture'Engine.S_Weapon'
	ItemName="Weapon"
	DrawType=2
	bReplicateInstigator=true
	bCheckChangableLevel=true
}