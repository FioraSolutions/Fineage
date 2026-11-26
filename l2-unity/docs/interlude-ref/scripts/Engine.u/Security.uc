class Security extends Info
	native
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var int LastType;
var string LastParams[2];

replication
{
	// Pos:0x000
	reliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		ClientMessage, ClientPerform;

	// Pos:0x00D
	reliable if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		ServerCallback;
}

// Export USecurity::execNativePerform(FFrame&, void* const)
native function NativePerform(int SecType, string Param1, string Param2);

simulated function ClientPerform(int SecType, string Param1, string Param2)
{
	NativePerform(SecType, Param1, Param2);
	return;
}

event ServerCallback(int SecType, string Data)
{
	__NFUN_280__(0.0000000, false);
	__NFUN_113__('None');
	return;
}

function Perform(int SecType, string Param1, string Param2, float TimeOut)
{
	LastType = SecType;
	LastParams[0] = Param1;
	LastParams[1] = Param2;
	ClientPerform(SecType, Param1, Param2);
	__NFUN_280__(TimeOut, false);
	__NFUN_113__('Probation');
	return;
}

function BadClient(int Code, string Data)
{
	ClientMessage(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("The Server has determined that your client is illegal and you have been removed! Code: ", string(Code)), " ["), Data), "]"));
	Owner.__NFUN_279__();
	__NFUN_279__();
	return;
}

simulated function ClientMessage(string S)
{
	__NFUN_231__(S, 'Security');
	return;
}

state Probation
{
	function Timer()
	{
		BadClient(LastType, __NFUN_112__(__NFUN_112__(LastParams[0], ","), LastParams[1]));
		return;
	}
	stop;
}
