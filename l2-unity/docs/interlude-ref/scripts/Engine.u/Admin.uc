class Admin extends PlayerController
	config(User)
	notplaceable;

replication
{
	// Pos:0x000
	reliable if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		Admin, Kick, 
		KickBan, RestartMap, 
		Switch;
}

event PostBeginPlay()
{
	super.PostBeginPlay();
	AddCheats();
	return;
}

exec function Admin(string CommandLine)
{
	local string Result;

	Result = ConsoleCommand(CommandLine);
	// End:0x28
	if(__NFUN_123__(Result, ""))
	{
		ClientMessage(Result);
	}
	return;
}

exec function KickBan(string S)
{
	Level.Game.KickBan(S);
	return;
}

exec function Say(string Msg)
{
	local Controller C;

	// End:0xD5
	if(__NFUN_122__(__NFUN_128__(Msg, 1), "#"))
	{
		Msg = __NFUN_234__(Msg, __NFUN_147__(__NFUN_125__(Msg), 1));
		C = Level.ControllerList;
		J0x3B:

		// End:0xD3 [Loop If]
		if(__NFUN_119__(C, none))
		{
			// End:0xBC
			if(C.__NFUN_303__('PlayerController'))
			{
				PlayerController(C).ClearProgressMessages();
				PlayerController(C).SetProgressTime(6.0000000);
				PlayerController(C).SetProgressMessage(0, Msg, Class'Engine.Canvas'.static.MakeColor(byte(255), byte(255), byte(255)));
			}
			C = C.nextController;
			// [Loop Continue]
			goto J0x3B;
		}
		return;
	}
	super.Say(Msg);
	return;
}

exec function Kick(string S)
{
	Level.Game.Kick(S);
	return;
}

exec function PlayerList()
{
	local PlayerReplicationInfo PRI;

	__NFUN_231__("Player List:");
	// End:0x52
	foreach __NFUN_313__(Class'Engine.PlayerReplicationInfo', PRI)
	{
		__NFUN_231__(__NFUN_112__(__NFUN_168__(__NFUN_168__(PRI.PlayerName, "( ping"), string(PRI.Ping)), ")"));		
	}	
	return;
}

exec function RestartMap()
{
	ClientTravel("?restart", 2, false);
	return;
}

exec function Switch(string URL)
{
	Level.ServerTravel(URL, false);
	return;
}
