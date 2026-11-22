class BroadcastHandler extends Info
	config
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var int SentText;
var config bool bMuteSpectators;

function UpdateSentText()
{
	SentText = 0;
	return;
}

function bool AllowsBroadcast(Actor broadcaster, int Len)
{
	// End:0x3F
	if(__NFUN_130__(__NFUN_130__(bMuteSpectators, __NFUN_119__(PlayerController(broadcaster), none)), PlayerController(broadcaster).PlayerReplicationInfo.bOnlySpectator))
	{
		return false;
	}
	__NFUN_161__(SentText, Len);
	return __NFUN_132__(__NFUN_119__(Level.Pauser, none), __NFUN_150__(SentText, 260));
	return;
}

function BroadcastText(PlayerReplicationInfo SenderPRI, PlayerController Receiver, coerce string Msg, optional name Type)
{
	Receiver.TeamMessage(SenderPRI, Msg, Type);
	return;
}

function BroadcastLocalized(Actor Sender, PlayerController Receiver, Class<LocalMessage> Message, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	Receiver.ReceiveLocalizedMessage(Message, Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject);
	return;
}

function Broadcast(Actor Sender, coerce string Msg, optional name Type)
{
	local Controller C;
	local PlayerController P;
	local PlayerReplicationInfo PRI;

	// End:0x19
	if(__NFUN_129__(AllowsBroadcast(Sender, __NFUN_125__(Msg))))
	{
		return;
	}
	// End:0x45
	if(__NFUN_119__(Pawn(Sender), none))
	{
		PRI = Pawn(Sender).PlayerReplicationInfo;		
	}
	else
	{
		// End:0x6E
		if(__NFUN_119__(Controller(Sender), none))
		{
			PRI = Controller(Sender).PlayerReplicationInfo;
		}
	}
	C = Level.ControllerList;
	J0x82:

	// End:0xD9 [Loop If]
	if(__NFUN_119__(C, none))
	{
		P = PlayerController(C);
		// End:0xC2
		if(__NFUN_119__(P, none))
		{
			BroadcastText(PRI, P, Msg, Type);
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x82;
	}
	return;
}

function BroadcastTeam(Controller Sender, coerce string Msg, optional name Type)
{
	local Controller C;
	local PlayerController P;

	// End:0x19
	if(__NFUN_129__(AllowsBroadcast(Sender, __NFUN_125__(Msg))))
	{
		return;
	}
	C = Level.ControllerList;
	J0x2D:

	// End:0xC2 [Loop If]
	if(__NFUN_119__(C, none))
	{
		P = PlayerController(C);
		// End:0xAB
		if(__NFUN_130__(__NFUN_119__(P, none), __NFUN_114__(P.PlayerReplicationInfo.Team, Sender.PlayerReplicationInfo.Team)))
		{
			BroadcastText(Sender.PlayerReplicationInfo, P, Msg, Type);
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x2D;
	}
	return;
}

event AllowBroadcastLocalized(Actor Sender, Class<LocalMessage> Message, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	local Controller C;
	local PlayerController P;

	C = Level.ControllerList;
	J0x14:

	// End:0x7A [Loop If]
	if(__NFUN_119__(C, none))
	{
		P = PlayerController(C);
		// End:0x63
		if(__NFUN_119__(P, none))
		{
			BroadcastLocalized(Sender, P, Message, Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject);
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x14;
	}
	return;
}
