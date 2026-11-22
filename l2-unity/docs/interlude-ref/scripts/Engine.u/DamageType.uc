class DamageType extends Actor
	abstract
	native
	notplaceable;

var() localized string DeathString;
var() localized string FemaleSuicide;
var() localized string MaleSuicide;
var() float ViewFlash;
var() Vector ViewFog;
var() Class<Effects> DamageEffect;
var() string DamageWeaponName;
var() bool bArmorStops;
var() bool bInstantHit;
var() bool bFastInstantHit;
var() bool bAlwaysGibs;
var() bool bNoSpecificLocation;
var() bool bSkeletize;
var() bool bCausesBlood;
var() bool bKUseOwnDeathVel;
var() float GibModifier;
var() Class<Effects> PawnDamageEffect;
var() Class<Emitter> PawnDamageEmitter;
var() array<Sound> PawnDamageSounds;
var() Class<Effects> LowGoreDamageEffect;
var() Class<Emitter> LowGoreDamageEmitter;
var() array<Sound> LowGoreDamageSounds;
var() Class<Effects> LowDetailEffect;
var() Class<Emitter> LowDetailEmitter;
var() float FlashScale;
var() Vector FlashFog;
var() int DamageDesc;
var() int DamageThreshold;
var() Vector DamageKick;
var(Karma) float KDamageImpulse;
var(Karma) float KDeathVel;
var(Karma) float KDeathUpKick;

static function IncrementKills(Controller Killer)
{
	return;
}

static function string DeathMessage(PlayerReplicationInfo Killer, PlayerReplicationInfo Victim)
{
	return default.DeathString;
	return;
}

static function string SuicideMessage(PlayerReplicationInfo Victim)
{
	// End:0x1B
	if(Victim.bIsFemale)
	{
		return default.FemaleSuicide;		
	}
	else
	{
		return default.MaleSuicide;
	}
	return;
}

static function Class<Effects> GetPawnDamageEffect(Vector HitLocation, float Damage, Vector Momentum, Pawn Victim, bool bLowDetail)
{
	// End:0x38
	if(Class'Engine.GameInfo'.static.UseLowGore())
	{
		// End:0x26
		if(__NFUN_119__(default.LowGoreDamageEffect, none))
		{
			return default.LowGoreDamageEffect;			
		}
		else
		{
			return Victim.LowGoreBlood;
		}		
	}
	else
	{
		// End:0x67
		if(bLowDetail)
		{
			// End:0x55
			if(__NFUN_119__(default.LowDetailEffect, none))
			{
				return default.LowDetailEffect;				
			}
			else
			{
				return Victim.BloodEffect;
			}			
		}
		else
		{
			// End:0x7B
			if(__NFUN_119__(default.PawnDamageEffect, none))
			{
				return default.PawnDamageEffect;				
			}
			else
			{
				return Victim.BloodEffect;
			}
		}
	}
	return;
}

static function Class<Emitter> GetPawnDamageEmitter(Vector HitLocation, float Damage, Vector Momentum, Pawn Victim, bool bLowDetail)
{
	// End:0x2B
	if(Class'Engine.GameInfo'.static.UseLowGore())
	{
		// End:0x26
		if(__NFUN_119__(default.LowGoreDamageEmitter, none))
		{
			return default.LowGoreDamageEmitter;			
		}
		else
		{
			return none;
		}		
	}
	else
	{
		// End:0x4D
		if(bLowDetail)
		{
			// End:0x48
			if(__NFUN_119__(default.LowDetailEffect, none))
			{
				return default.LowDetailEmitter;				
			}
			else
			{
				return none;
			}			
		}
		else
		{
			// End:0x61
			if(__NFUN_119__(default.PawnDamageEmitter, none))
			{
				return default.PawnDamageEmitter;				
			}
			else
			{
				return none;
			}
		}
	}
	return;
}

static function Sound GetPawnDamageSound()
{
	// End:0x35
	if(Class'Engine.GameInfo'.static.UseLowGore())
	{
		// End:0x30
		if(__NFUN_151__(default.LowGoreDamageSounds.Length, 0))
		{
			return default.LowGoreDamageSounds[__NFUN_167__(default.LowGoreDamageSounds.Length)];			
		}
		else
		{
			return none;
		}		
	}
	else
	{
		// End:0x53
		if(__NFUN_151__(default.PawnDamageSounds.Length, 0))
		{
			return default.PawnDamageSounds[__NFUN_167__(default.PawnDamageSounds.Length)];			
		}
		else
		{
			return none;
		}
	}
	return;
}

static function bool IsOfType(int Description)
{
	local int Result;

	Result = __NFUN_156__(Description, default.DamageDesc);
	return __NFUN_154__(Result, Description);
	return;
}

static function string GetWeaponClass()
{
	return "";
	return;
}

defaultproperties
{
	DeathString="%o was killed by %k."
	FemaleSuicide="%o killed herself."
	MaleSuicide="%o killed himself."
	bArmorStops=true
	bCausesBlood=true
	GibModifier=1.0000000
	FlashScale=0.3000000
	FlashFog=(X=900.0000000,Y=0.0000000,Z=0.0000000)
	DamageDesc=1
	KDamageImpulse=8000.0000000
}