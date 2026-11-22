class PathNode extends NavigationPoint
	native
	placeable
	hidecategories(Lighting,LightColor,Karma,Force);

cpptext
{
	virtual UBOOL ReviewPath(APawn* Scout);
	virtual void CheckSymmetry(ANavigationPoint* Other);
	virtual INT AddMyMarker(AActor *S);

}

defaultproperties
{
	Texture=Texture'Engine.S_Pickup'
}