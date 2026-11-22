class DynamicProjector extends Projector
	placeable;

function Tick(float DeltaTime)
{
	DetachProjector();
	AttachProjector();
	return;
}

defaultproperties
{
	bDynamicAttach=true
	bStatic=false
}