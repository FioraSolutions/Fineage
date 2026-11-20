using System;

[Serializable]
public sealed class DetailedMapZoneRecord
{
    public string id;
    public int[] zoneIds;
    public string name;
    public string originalPackage;
    public string originalTextureGroup;
    public string originalTexture;
    public string importedResource;
    public float originL2X;
    public float originL2Y;
    public float scale;
    public int mapPixelWidth;
    public int mapPixelHeight;
    public int hotspotMapX;
    public int hotspotMapY;
    public bool needsCoordsFromZonenameDat;

    public bool HasWorldMapHotspot => hotspotMapX >= 0 && hotspotMapY >= 0;

    public float WorldMinL2X => originL2X;
    public float WorldMaxL2X => originL2X + mapPixelWidth / scale;
    public float WorldMinL2Y => originL2Y;
    public float WorldMaxL2Y => originL2Y + mapPixelHeight / scale;

    public string ExpectedTextureLabel =>
        originalTextureGroup != null
            ? $"{originalPackage}:{originalTextureGroup}.{originalTexture}"
            : $"{originalPackage}:{originalTexture}";
}