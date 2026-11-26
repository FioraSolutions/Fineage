class AccessControl extends Info
	config
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var globalconfig string IPPolicies[50];
var localized string IPBanned;
var localized string WrongPassword;
var localized string NeedPassword;
var Class<Admin> AdminClass;
var private globalconfig string AdminPassword;
var private globalconfig string GamePassword;
var bool bDontAddDefaultAdmin;

function SetAdminPassword(string P)
{
	AdminPassword = P;
	return;
}

function SetGamePassword(string P)
{
	GamePassword = P;
	return;
}

function bool RequiresPassword()
{
	return __NFUN_123__(GamePassword, "");
	return;
}

function Kick(string S)
{
	local PlayerController P;

	// End:0x5C
	foreach __NFUN_313__(Class'Engine.PlayerController', P)
	{
		// End:0x5B
		if(__NFUN_130__(__NFUN_124__(P.PlayerReplicationInfo.PlayerName, S), __NFUN_119__(NetConnection(P.Player), none)))
		{
			P.__NFUN_279__();			
			return;
		}		
	}	
	return;
}

function KickBan(string S)
{
	local PlayerController P;
	local string IP;
	local int j;

	// End:0x10F
	foreach __NFUN_313__(Class'Engine.PlayerController', P)
	{
		// End:0x10E
		if(__NFUN_130__(__NFUN_124__(P.PlayerReplicationInfo.PlayerName, S), __NFUN_119__(NetConnection(P.Player), none)))
		{
			IP = P.GetPlayerNetworkAddress();
			// End:0xFF
			if(CheckIPPolicy(IP))
			{
				IP = __NFUN_128__(IP, __NFUN_126__(IP, ":"));
				__NFUN_231__(__NFUN_112__("Adding IP Ban for: ", IP));
				j = 0;
				J0xAB:

				// End:0xD6 [Loop If]
				if(__NFUN_150__(j, 50))
				{
					// End:0xCC
					if(__NFUN_122__(IPPolicies[j], ""))
					{
						// [Explicit Break]
						goto J0xD6;
					}
					__NFUN_165__(j);
					// [Loop Continue]
					goto J0xAB;
				}
				J0xD6:

				// End:0xFC
				if(__NFUN_150__(j, 50))
				{
					IPPolicies[j] = __NFUN_112__("DENY,", IP);
				}
				__NFUN_536__();
			}
			P.__NFUN_279__();			
			return;
		}		
	}	
	return;
}

function bool AdminLogin(PlayerController P, string Password)
{
	// End:0x0E
	if(__NFUN_122__(AdminPassword, ""))
	{
		return false;
	}
	// End:0x97
	if(__NFUN_122__(Password, AdminPassword))
	{
		__NFUN_231__("Administrator logged in.");
		Level.Game.Broadcast(P, __NFUN_112__(P.PlayerReplicationInfo.PlayerName, "logged in as a server administrator."));
		return true;
	}
	return false;
	return;
}

event PreLogin(string Options, string Address, out string Error, out string FailCode, bool bSpectator)
{
	local string InPassword;

	Error = "";
	InPassword = Level.Game.ParseOption(Options, "Password");
	// End:0x9A
	if(__NFUN_130__(__NFUN_155__(int(Level.NetMode), int(NM_Standalone)), Level.Game.AtCapacity(bSpectator)))
	{
		Error = Level.Game.GameMessageClass.default.MaxedOutMessage;		
	}
	else
	{
		// End:0x120
		if(__NFUN_130__(__NFUN_130__(__NFUN_123__(GamePassword, ""), __NFUN_123__(__NFUN_235__(InPassword), __NFUN_235__(GamePassword))), __NFUN_132__(__NFUN_122__(AdminPassword, ""), __NFUN_123__(__NFUN_235__(InPassword), __NFUN_235__(AdminPassword)))))
		{
			// End:0x106
			if(__NFUN_122__(InPassword, ""))
			{
				Error = NeedPassword;
				FailCode = "NEEDPW";				
			}
			else
			{
				Error = WrongPassword;
				FailCode = "WRONGPW";
			}
		}
	}
	// End:0x13B
	if(__NFUN_129__(CheckIPPolicy(Address)))
	{
		Error = IPBanned;
	}
	return;
}

function bool CheckIPPolicy(string Address)
{
	local int i, j, LastMatchingPolicy;
	local string Policy, Mask;
	local bool bAcceptAddress, bAcceptPolicy;

	j = __NFUN_126__(Address, ":");
	// End:0x31
	if(__NFUN_155__(j, -1))
	{
		Address = __NFUN_128__(Address, j);
	}
	bAcceptAddress = true;
	i = 0;
	J0x40:

	// End:0x17E [Loop If]
	if(__NFUN_130__(__NFUN_150__(i, 50), __NFUN_123__(IPPolicies[i], "")))
	{
		j = __NFUN_126__(IPPolicies[i], ",");
		// End:0x88
		if(__NFUN_154__(j, -1))
		{
			// [Explicit Continue]
			goto J0x174;
		}
		Policy = __NFUN_128__(IPPolicies[i], j);
		Mask = __NFUN_127__(IPPolicies[i], __NFUN_146__(j, 1));
		// End:0xD8
		if(__NFUN_124__(Policy, "ACCEPT"))
		{
			bAcceptPolicy = true;			
		}
		else
		{
			// End:0xF3
			if(__NFUN_124__(Policy, "DENY"))
			{
				bAcceptPolicy = false;				
			}
			else
			{
				// [Explicit Continue]
				goto J0x174;
			}
		}
		j = __NFUN_126__(Mask, "*");
		// End:0x14D
		if(__NFUN_155__(j, -1))
		{
			// End:0x14A
			if(__NFUN_122__(__NFUN_128__(Mask, j), __NFUN_128__(Address, j)))
			{
				bAcceptAddress = bAcceptPolicy;
				LastMatchingPolicy = i;
			}
			// [Explicit Continue]
			goto J0x174;
		}
		// End:0x174
		if(__NFUN_122__(Mask, Address))
		{
			bAcceptAddress = bAcceptPolicy;
			LastMatchingPolicy = i;
		}
		J0x174:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x40;
	}
	// End:0x1CB
	if(__NFUN_129__(bAcceptAddress))
	{
		__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Denied connection for ", Address), " with IP policy "), IPPolicies[LastMatchingPolicy]));
	}
	return bAcceptAddress;
	return;
}

defaultproperties
{
	IPPolicies="ACCEPT,*"
	IPBanned="Your IP address has been banned on this server."
	WrongPassword="The password you entered is incorrect."
	NeedPassword="You need to enter a password to join this game."
	AdminClass=Class'Engine.Admin'
}