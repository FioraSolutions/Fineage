using System;

[Serializable]
public sealed class DetailedMapCatalogJson
{
    public int version;
    public string source;
    public int worldMapPixelWidth;
    public int worldMapPixelHeight;
    public DetailedMapZoneRecord[] zones;
}