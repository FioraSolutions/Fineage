class DemoRecSpectator extends MessagingSpectator
	config(User)
	notplaceable;

var PlayerController PlaybackActor;
var GameReplicationInfo PlaybackGRI;

replication
{
	// Pos:0x000
	reliable if(bDemoRecording)
		RepClientMessage, RepClientVoiceMessage, 
		RepReceiveLocalizedMessage;
}

function ClientMessage(coerce string S, optional name Type)
{
	RepClientMessage(S, Type);
	return;
}

function ClientVoiceMessage(PlayerReplicationInfo Sender, PlayerReplicationInfo Recipient, name messagetype, byte MessageID)
{
	RepClientVoiceMessage(Sender, Recipient, messagetype, MessageID);
	return;
}

function ReceiveLocalizedMessage(Class<LocalMessage> Message, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	RepReceiveLocalizedMessage(Message, Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject);
	return;
}

simulated function RepClientMessage(coerce string S, optional name Type)
{
	return;
}

simulated function RepClientVoiceMessage(PlayerReplicationInfo Sender, PlayerReplicationInfo Recipient, name messagetype, byte MessageID)
{
	return;
}

simulated function RepReceiveLocalizedMessage(Class<LocalMessage> Message, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	return;
}
