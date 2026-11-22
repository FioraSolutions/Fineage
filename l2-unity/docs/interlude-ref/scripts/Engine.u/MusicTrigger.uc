class MusicTrigger extends Triggers
	placeable;

var() string Song;
var() float FadeInTime;
var() float FadeOutTime;
var() bool FadeOutAllSongs;
var transient bool Triggered;
var transient int SongHandle;

function Trigger(Actor Other, Pawn EventInstigator)
{
	// End:0x40
	if(__NFUN_129__(Triggered))
	{
		Triggered = true;
		// End:0x27
		if(FadeOutAllSongs)
		{
			StopAllMusic(FadeOutTime);
		}
		SongHandle = PlayMusic(Song, FadeInTime);		
	}
	else
	{
		Triggered = false;
		// End:0x66
		if(__NFUN_155__(SongHandle, 0))
		{
			StopMusic(SongHandle, FadeOutTime);			
		}
		else
		{
			__NFUN_231__("WARNING: invalid song handle");
		}
	}
	return;
}

defaultproperties
{
	bCollideActors=false
}