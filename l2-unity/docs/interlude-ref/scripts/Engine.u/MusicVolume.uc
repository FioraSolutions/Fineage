class MusicVolume extends Volume
	native
	nativereplication
	notplaceable;

var(Music) int nMusicID;
var(Music) bool bForcePlayMusic;
var(Music) bool bLoopMusic;

simulated function PostBeginPlay()
{
	super.PostBeginPlay();
	return;
}

defaultproperties
{
	nMusicID=-1
	DrawType=14
}