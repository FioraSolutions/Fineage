class GameRules extends Info
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var GameRules NextGameRules;

function AddGameRules(GameRules GR)
{
	// End:0x19
	if(__NFUN_114__(NextGameRules, none))
	{
		NextGameRules = GR;		
	}
	else
	{
		NextGameRules.AddGameRules(GR);
	}
	return;
}

function NavigationPoint FindPlayerStart(Controller Player, optional byte InTeam, optional string incomingName)
{
	// End:0x2A
	if(__NFUN_119__(NextGameRules, none))
	{
		return NextGameRules.FindPlayerStart(Player, InTeam, incomingName);
	}
	return none;
	return;
}

function string GetRules()
{
	local string ResultSet;

	// End:0x27
	if(__NFUN_114__(NextGameRules, none))
	{
		ResultSet = __NFUN_112__(ResultSet, NextGameRules.GetRules());
	}
	return ResultSet;
	return;
}

function bool HandleRestartGame()
{
	// End:0x21
	if(__NFUN_130__(__NFUN_119__(NextGameRules, none), NextGameRules.HandleRestartGame()))
	{
		return true;
	}
	return false;
	return;
}

function bool CheckEndGame(PlayerReplicationInfo Winner, string Reason)
{
	// End:0x25
	if(__NFUN_119__(NextGameRules, none))
	{
		return NextGameRules.CheckEndGame(Winner, Reason);
	}
	return true;
	return;
}

function bool CheckScore(PlayerReplicationInfo Scorer)
{
	// End:0x20
	if(__NFUN_119__(NextGameRules, none))
	{
		return NextGameRules.CheckScore(Scorer);
	}
	return false;
	return;
}

function bool OverridePickupQuery(Pawn Other, Pickup item, out byte bAllowPickup)
{
	// End:0x30
	if(__NFUN_130__(__NFUN_119__(NextGameRules, none), NextGameRules.OverridePickupQuery(Other, item, bAllowPickup)))
	{
		return true;
	}
	return false;
	return;
}

function bool PreventDeath(Pawn Killed, Controller Killer, Class<DamageType> DamageType, Vector HitLocation)
{
	// End:0x35
	if(__NFUN_130__(__NFUN_119__(NextGameRules, none), NextGameRules.PreventDeath(Killed, Killer, DamageType, HitLocation)))
	{
		return true;
	}
	return false;
	return;
}

function ScoreObjective(PlayerReplicationInfo Scorer, int Score)
{
	// End:0x24
	if(__NFUN_119__(NextGameRules, none))
	{
		NextGameRules.ScoreObjective(Scorer, Score);
	}
	return;
}

function ScoreKill(Controller Killer, Controller Killed)
{
	// End:0x24
	if(__NFUN_119__(NextGameRules, none))
	{
		NextGameRules.ScoreKill(Killer, Killed);
	}
	return;
}

function bool CriticalPlayer(Controller Other)
{
	// End:0x26
	if(__NFUN_130__(__NFUN_119__(NextGameRules, none), NextGameRules.CriticalPlayer(Other)))
	{
		return true;
	}
	return false;
	return;
}

function int NetDamage(int OriginalDamage, int Damage, Pawn injured, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	// End:0x3E
	if(__NFUN_119__(NextGameRules, none))
	{
		return NextGameRules.NetDamage(OriginalDamage, Damage, injured, instigatedBy, HitLocation, Momentum, DamageType);
	}
	return Damage;
	return;
}
