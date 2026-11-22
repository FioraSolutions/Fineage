class Note extends Actor
	native
	placeable;

var() string Text;

defaultproperties
{
	bStatic=true
	bHidden=true
	bNoDelete=true
	Texture=Texture'Engine.S_Note'
	bMovable=false
}