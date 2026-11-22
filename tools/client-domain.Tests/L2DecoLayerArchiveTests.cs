using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Fineage.Client.Domain.Tests;

public class L2DecoLayerArchiveTests
{
    [Fact]
    public void RoundTrip_is_byte_identical_and_keeps_official_fields()
    {
        L2DecoTileFile tile = Synthetic();
        L2DecoArchiveResult written = L2DecoLayerArchive.Write(tile);
        Assert.True(written.Ok, string.Join("\n", written.Errors));
        Assert.True(L2DecoLayerArchive.TryRead(written.Bytes, out L2DecoTileFile back, out string error), error);
        L2DecoArchiveResult again = L2DecoLayerArchive.Write(back);
        Assert.True(again.Ok, string.Join("\n", again.Errors));
        Assert.Equal(written.Bytes, again.Bytes);
        Assert.Equal("syn", back.TileId);
        Assert.Equal(2, back.QuadsX);
        Assert.Equal(128f, back.ScaleX);
        Assert.Equal(-1, back.VisibilityWords[0]);
        L2DecoLayerInput layer = back.Layers[0];
        Assert.True(layer.ShowOnTerrain);
        Assert.True(layer.RandomYaw);
        Assert.Equal(1, layer.DrawOrder);
        Assert.Equal(0, layer.Seed);
        Assert.Equal(20, layer.MaxPerQuad);
        Assert.Equal(300f, layer.FadeMinUu);
        Assert.Equal(1500f, layer.FadeMaxUu);
        Assert.Equal(10f, layer.DensityMin);
        Assert.Equal(50f, layer.DensityMax);
        Assert.Equal(3f, layer.ScaleZMax);
        Assert.Equal(new byte[] { 255, 0, 0, 0 }, layer.DensityAlpha);
    }

    [Fact]
    public void Generate_is_deterministic_and_alpha_zero_places_nothing()
    {
        L2DecoTileFile tile = Synthetic();
        var first = new List<L2DecoInstance>();
        var second = new List<L2DecoInstance>();
        L2DecoGenerator.AppendSector(tile, 0, 0, 0, first);
        L2DecoGenerator.AppendSector(tile, 0, 0, 0, second);
        Assert.Equal(20, first.Count);
        Assert.Equal(first.Count, second.Count);
        for (int i = 0; i < first.Count; i++)
        {
            Assert.Equal(first[i].UnityX, second[i].UnityX);
            Assert.Equal(first[i].UnityZ, second[i].UnityZ);
            Assert.Equal(first[i].YawDegrees, second[i].YawDegrees);
            Assert.Equal(first[i].UnityScaleX, second[i].UnityScaleX);
            Assert.Equal(first[i].UnityScaleY, second[i].UnityScaleY);
            Assert.Equal(first[i].UnityScaleZ, second[i].UnityScaleZ);
        }

        tile.Layers[0].DensityAlpha = new byte[] { 0, 0, 0, 0 };
        var none = new List<L2DecoInstance>();
        L2DecoGenerator.AppendSector(tile, 0, 0, 0, none);
        Assert.Empty(none);
    }

    [Fact]
    public void CountForDensity_follows_the_official_lerp_and_cap()
    {
        Assert.Equal(0, L2DecoGenerator.CountForDensity(10f, 50f, 20, 0));
        Assert.Equal(20, L2DecoGenerator.CountForDensity(10f, 50f, 20, 255));
        Assert.Equal(10, L2DecoGenerator.CountForDensity(10f, 50f, 20, 1));
    }

    [Fact]
    public void Hidden_quad_is_skipped_unless_ShowOnInvisibleTerrain()
    {
        L2DecoTileFile tile = Synthetic();
        tile.VisibilityWords = new int[] { 0 };
        var hidden = new List<L2DecoInstance>();
        L2DecoGenerator.AppendSector(tile, 0, 0, 0, hidden);
        Assert.Empty(hidden);

        tile.Layers[0].ShowOnInvisibleTerrain = true;
        var forced = new List<L2DecoInstance>();
        L2DecoGenerator.AppendSector(tile, 0, 0, 0, forced);
        Assert.Equal(20, forced.Count);
    }

    [Fact]
    public void Missing_fade_on_a_shown_layer_names_the_tile_and_layer()
    {
        L2DecoTileFile tile = Synthetic();
        tile.Layers[0].HasFade = false;
        L2DecoArchiveResult result = L2DecoLayerArchive.Write(tile);
        Assert.False(result.Ok);
        string text = string.Join("\n", result.Errors);
        Assert.Contains("syn", text);
        Assert.Contains("layer 0", text);
        Assert.Contains("FadeoutRadius", text);
        Assert.True(L2DecoLayerArchive.TryRead(result.Bytes, out L2DecoTileFile back, out string error), error);
        Assert.Empty(back.Layers);
    }

    [Fact]
    public void Missing_visibility_does_not_write_a_file()
    {
        L2DecoTileFile tile = Synthetic();
        tile.VisibilityWords = null;
        L2DecoArchiveResult result = L2DecoLayerArchive.Write(tile);
        Assert.Null(result.Bytes);
        Assert.Contains("QuadVisibilityBitmap", string.Join("\n", result.Errors));
    }

    [Fact]
    public void Cap_is_4mb_and_under_the_git_file_limit()
    {
        Assert.Equal(4 * 1024 * 1024, L2DecoTileFile.MaxTileBytes);
        Assert.True(L2DecoTileFile.MaxTileBytes < 100 * 1024 * 1024);
    }

    [Fact]
    public void All_zero_density_stays_rle_and_incompressible_stays_raw_under_the_cap()
    {
        L2DecoTileFile zeros = FullDensity(new byte[512 * 512]);
        L2DecoArchiveResult packed = L2DecoLayerArchive.Write(zeros);
        Assert.True(packed.Ok, string.Join("\n", packed.Errors));
        Assert.True(packed.Bytes.Length < 20 * 1024, "zero map was " + packed.Bytes.Length);

        byte[] noise = new byte[512 * 512];
        for (int i = 0; i < noise.Length; i++)
            noise[i] = (byte)(i & 1);
        L2DecoArchiveResult raw = L2DecoLayerArchive.Write(FullDensity(noise));
        Assert.True(raw.Ok, string.Join("\n", raw.Errors));
        Assert.True(raw.Bytes.Length > 512 * 512);
        Assert.True(raw.Bytes.Length < L2DecoTileFile.MaxTileBytes);
        int payloadLen = 1 + 512 * 512;
        Assert.Equal(0, raw.Bytes[raw.Bytes.Length - payloadLen]);
        Assert.Equal(payloadLen, BitConverter.ToInt32(raw.Bytes, raw.Bytes.Length - payloadLen - 4));
    }

    [Fact]
    public void Talking_island_quad_origin_matches_the_terrain_corner()
    {
        var tile = new L2DecoTileFile();
        tile.OriginX = -81920f;
        tile.OriginY = 245760f;
        tile.ScaleX = 128f;
        tile.ScaleY = 128f;
        tile.QuadsX = 256;
        tile.QuadsY = 256;
        L2DecoGenerator.QuadToUnityMeters(tile, 0, 0, 0f, 0f, out float unityX, out float unityZ);
        Assert.Equal(229248f / 52.5f, unityX, 3);
        Assert.Equal(-98432f / 52.5f, unityZ, 3);
    }

    [Fact]
    public void AppFrand_first_step_from_zero_is_the_windows_lcg()
    {
        uint state = 0;
        float sample = L2DecoGenerator.AppFrand(ref state);
        Assert.Equal(38f / 32767f, sample, 6);
    }

    [Fact]
    public void Scale_map_replaces_the_scale_draw()
    {
        L2DecoTileFile tile = Synthetic();
        tile.Layers[0].ScaleWidth = 2;
        tile.Layers[0].ScaleHeight = 2;
        tile.Layers[0].ScaleAlpha = new byte[] { 255, 255, 255, 255 };
        var spawned = new List<L2DecoInstance>();
        L2DecoGenerator.AppendSector(tile, 0, 0, 0, spawned);
        Assert.Equal(20, spawned.Count);
        Assert.Equal(3f, spawned[0].UnityScaleX);
        Assert.Equal(3f, spawned[0].UnityScaleY);
        Assert.Equal(3f, spawned[0].UnityScaleZ);
    }

    [Fact]
    public void Synthetic_fixture_matches_the_writer()
    {
        L2DecoArchiveResult written = L2DecoLayerArchive.Write(Synthetic());
        Assert.True(written.Ok, string.Join("\n", written.Errors));
        string path = FixturePath();
        if (Environment.GetEnvironmentVariable("FINEAGE_WRITE_DECO_FIXTURE") == "1")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, written.Bytes);
        }

        Assert.True(File.Exists(path), path);
        Assert.Equal(written.Bytes, File.ReadAllBytes(path));
    }

    static L2DecoTileFile FullDensity(byte[] alpha)
    {
        L2DecoTileFile tile = Synthetic();
        tile.QuadsX = 256;
        tile.QuadsY = 256;
        tile.VisibilityWords = new int[2048];
        for (int i = 0; i < tile.VisibilityWords.Length; i++)
            tile.VisibilityWords[i] = -1;
        tile.Layers[0].DensityWidth = 512;
        tile.Layers[0].DensityHeight = 512;
        tile.Layers[0].DensityAlpha = alpha;
        return tile;
    }

    static L2DecoTileFile Synthetic()
    {
        var tile = new L2DecoTileFile();
        tile.TileId = "syn";
        tile.ScaleX = 128f;
        tile.ScaleY = 128f;
        tile.ScaleZ = 76f;
        tile.QuadsX = 2;
        tile.QuadsY = 2;
        tile.SectorQuads = 16;
        tile.VisibilityWords = new int[] { -1 };
        tile.Layers = new[] { Grass() };
        return tile;
    }

    static L2DecoLayerInput Grass()
    {
        var layer = new L2DecoLayerInput();
        layer.ShowOnTerrain = true;
        layer.RandomYaw = true;
        layer.DrawOrder = 1;
        layer.Seed = 0;
        layer.MaxPerQuad = 20;
        layer.FadeMinUu = 300f;
        layer.FadeMaxUu = 1500f;
        layer.DensityMin = 10f;
        layer.DensityMax = 50f;
        layer.ScaleXMin = 1f;
        layer.ScaleXMax = 3f;
        layer.ScaleYMin = 1f;
        layer.ScaleYMax = 3f;
        layer.ScaleZMin = 1f;
        layer.ScaleZMax = 3f;
        layer.MeshResourcePath = "Data/StaticMeshes/field_deco_S/Prefab/Grass009";
        layer.MeshRef = "StaticMesh'field_deco_S.grass_flower.Grass009'";
        layer.DensityRef = "Texture'L2DecoLayer.Talking_Island.syn'";
        layer.DensityWidth = 2;
        layer.DensityHeight = 2;
        layer.DensityAlpha = new byte[] { 255, 0, 0, 0 };
        return layer;
    }

    static string FixturePath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            string csproj = Path.Combine(dir.FullName, "Fineage.Client.Domain.Tests.csproj");
            if (File.Exists(csproj))
                return Path.Combine(dir.FullName, "Fixtures", "synthetic-deco.l2dc");
            dir = dir.Parent;
        }

        throw new InvalidOperationException("Fineage.Client.Domain.Tests.csproj not found from " + AppContext.BaseDirectory);
    }
}
