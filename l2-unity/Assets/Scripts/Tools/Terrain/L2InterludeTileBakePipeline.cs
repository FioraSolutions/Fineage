#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Flags]
public enum L2InterludeBakeStep
{
    None = 0,
    ImportBrushTextures = 1 << 0,
    ImportStaticMeshes = 1 << 1,
    GenerateMaterials = 1 << 2,
    GenerateStaticMeshScene = 1 << 3,
    GenerateTerrain = 1 << 4,
    BuildBrushes = 1 << 5,
    ImportAmbientSounds = 1 << 6,
    BuildAmbientSounds = 1 << 7,
    ConvertTerrainToMesh = 1 << 8,
    GenerateDecoLayerMesh = 1 << 9,
    BuildCameras = 1 << 10,

    PerTileUnityPipeline =
        ImportBrushTextures |
        ImportStaticMeshes |
        GenerateStaticMeshScene |
        GenerateTerrain |
        BuildBrushes |
        ImportAmbientSounds |
        BuildAmbientSounds |
        ConvertTerrainToMesh |
        GenerateDecoLayerMesh,

    AllPerTile = PerTileUnityPipeline | BuildCameras
}

public class L2InterludeTileBakeResult
{
    public string tileId;
    public bool success;
    public List<string> log = new List<string>();
}

public static partial class L2InterludeTileBakePipeline
{
    public static L2InterludeBakeStep DefaultSteps =>
        L2InterludeBakeStep.ImportBrushTextures |
        L2InterludeBakeStep.ImportStaticMeshes |
        L2InterludeBakeStep.GenerateStaticMeshScene |
        L2InterludeBakeStep.GenerateTerrain |
        L2InterludeBakeStep.BuildBrushes |
        L2InterludeBakeStep.ImportAmbientSounds |
        L2InterludeBakeStep.BuildAmbientSounds |
        L2InterludeBakeStep.ConvertTerrainToMesh |
        L2InterludeBakeStep.GenerateDecoLayerMesh;

}
