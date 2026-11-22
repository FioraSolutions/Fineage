using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Shipped DecoLayer tile. UE2 does not persist grass instances: <c>DecorationLayerData</c>
/// is native and transient, rebuilt from <c>TerrainInfo.DecoLayers</c> (density / scale / color
/// maps, seed, ranges). This file stores those inputs. Expanded matrices are not the source
/// and do not fit in git.
/// </summary>
public sealed class L2DecoTileFile
{
    public const int Version = 1;
    public const int Ue2TerrainSectorSizeDefault = 16;
    public const int MaxTileBytes = 4 * 1024 * 1024;

    public string TileId;
    public float OriginX;
    public float OriginY;
    public float OriginZ;
    public float ScaleX;
    public float ScaleY;
    public float ScaleZ;
    public int QuadsX;
    public int QuadsY;
    public float DecoLayerOffsetUu;
    public int SectorQuads;
    public int[] VisibilityWords;
    public L2DecoLayerInput[] Layers;
}

public sealed class L2DecoLayerInput
{
    public bool ShowOnTerrain;
    public bool AlignToTerrain;
    public bool RandomYaw;
    public bool ShowOnInvisibleTerrain;
    public bool LitDirectional;
    public bool DisregardTerrainLighting;
    public bool ForceRender;
    public byte DrawOrder;
    /// <summary>Write-time presence. Not stored; a file that reads back has these set.</summary>
    public bool HasShowOnTerrain = true;
    public bool HasFade = true;
    public bool HasDensity = true;
    public bool HasMaxPerQuad = true;
    public bool HasSeed = true;
    public bool HasScale = true;
    public bool HasRandomYaw = true;
    public bool HasDrawOrder = true;
    public bool HasAlign = true;
    public bool HasShowOnInvisible = true;
    public int Seed;
    public int MaxPerQuad;
    public float FadeMinUu;
    public float FadeMaxUu;
    public float DensityMin;
    public float DensityMax;
    public float ScaleXMin;
    public float ScaleXMax;
    public float ScaleYMin;
    public float ScaleYMax;
    public float ScaleZMin;
    public float ScaleZMax;
    public string MeshResourcePath;
    public string MeshRef;
    public string DensityRef;
    public string ScaleRef;
    public string ColorRef;
    public int DensityWidth;
    public int DensityHeight;
    public byte[] DensityAlpha;
    public int ScaleWidth;
    public int ScaleHeight;
    public byte[] ScaleAlpha;
    public int ColorWidth;
    public int ColorHeight;
    public byte[] ColorRgba;
}

public sealed class L2DecoArchiveResult
{
    public byte[] Bytes;
    public readonly List<string> Errors = new List<string>();

    public bool Ok
    {
        get { return Bytes != null && Errors.Count == 0; }
    }
}

public static class L2DecoLayerArchive
{
    public const string Magic = "L2DC";

    const ushort FlagShow = 1;
    const ushort FlagAlign = 2;
    const ushort FlagYaw = 4;
    const ushort FlagInvisible = 8;
    const ushort FlagLit = 16;
    const ushort FlagDisregard = 32;
    const ushort FlagForce = 64;
    const ushort FlagScaleMap = 128;
    const ushort FlagColorMap = 256;

    public static L2DecoArchiveResult Write(L2DecoTileFile tile)
    {
        var result = new L2DecoArchiveResult();
        if (tile == null)
        {
            result.Errors.Add("[Interlude deco] tile is null");
            return result;
        }

        string tileId = string.IsNullOrEmpty(tile.TileId) ? "(unknown tile)" : tile.TileId;
        if (string.IsNullOrEmpty(tile.TileId))
            result.Errors.Add("[Interlude deco] (unknown tile): missing tile id");
        if (tile.QuadsX <= 0 || tile.QuadsY <= 0)
            result.Errors.Add("[Interlude deco] " + tileId + ": quad grid is " + tile.QuadsX + "x" + tile.QuadsY);
        if (tile.ScaleX == 0f || tile.ScaleY == 0f)
            result.Errors.Add("[Interlude deco] " + tileId + ": TerrainScale X/Y is 0");
        int needWords = tile.QuadsX > 0 && tile.QuadsY > 0 ? (tile.QuadsX * tile.QuadsY + 31) / 32 : 0;
        if (tile.VisibilityWords == null || tile.VisibilityWords.Length != needWords)
        {
            result.Errors.Add(
                "[Interlude deco] " + tileId + ": QuadVisibilityBitmap has "
                + (tile.VisibilityWords == null ? 0 : tile.VisibilityWords.Length)
                + " words, need " + needWords);
        }

        var layers = new List<L2DecoLayerInput>();
        if (tile.Layers != null)
        {
            for (int i = 0; i < tile.Layers.Length; i++)
            {
                L2DecoLayerInput layer = tile.Layers[i];
                if (layer == null)
                {
                    result.Errors.Add("[Interlude deco] " + tileId + " layer " + i + ": null layer");
                    continue;
                }

                if (!LayerInputsPresent(tileId, i, layer, result.Errors))
                    continue;
                layers.Add(layer);
            }
        }

        if (result.Errors.Count > 0 && (tile.VisibilityWords == null || tile.QuadsX <= 0 || string.IsNullOrEmpty(tile.TileId)))
            return result;

        if (tile.VisibilityWords == null || tile.VisibilityWords.Length != needWords || tile.QuadsX <= 0)
            return result;

        var copy = new L2DecoTileFile();
        copy.TileId = tile.TileId;
        copy.OriginX = tile.OriginX;
        copy.OriginY = tile.OriginY;
        copy.OriginZ = tile.OriginZ;
        copy.ScaleX = tile.ScaleX;
        copy.ScaleY = tile.ScaleY;
        copy.ScaleZ = tile.ScaleZ;
        copy.QuadsX = tile.QuadsX;
        copy.QuadsY = tile.QuadsY;
        copy.DecoLayerOffsetUu = tile.DecoLayerOffsetUu;
        copy.SectorQuads = tile.SectorQuads > 0 ? tile.SectorQuads : L2DecoTileFile.Ue2TerrainSectorSizeDefault;
        copy.VisibilityWords = tile.VisibilityWords;
        copy.Layers = layers.ToArray();

        byte[] bytes = Encode(copy);
        if (bytes.Length > L2DecoTileFile.MaxTileBytes)
        {
            result.Errors.Add(
                "[Interlude deco] " + tileId + ": archive is " + bytes.Length
                + " bytes, cap is " + L2DecoTileFile.MaxTileBytes);
            return result;
        }

        result.Bytes = bytes;
        return result;
    }

    public static bool TryRead(byte[] bytes, out L2DecoTileFile tile, out string error)
    {
        tile = null;
        error = null;
        if (bytes == null || bytes.Length < 8)
        {
            error = "[Interlude deco] archive is empty";
            return false;
        }

        if (bytes.Length > L2DecoTileFile.MaxTileBytes)
        {
            error = "[Interlude deco] archive is " + bytes.Length + " bytes, cap is " + L2DecoTileFile.MaxTileBytes;
            return false;
        }

        try
        {
            using (var ms = new MemoryStream(bytes, false))
            using (var reader = new BinaryReader(ms, Encoding.UTF8, false))
            {
                string magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
                if (magic != Magic)
                {
                    error = "[Interlude deco] archive magic is '" + magic + "', expected " + Magic;
                    return false;
                }

                int version = reader.ReadUInt16();
                if (version != L2DecoTileFile.Version)
                {
                    error = "[Interlude deco] archive version " + version + ", expected " + L2DecoTileFile.Version;
                    return false;
                }

                tile = new L2DecoTileFile();
                tile.TileId = ReadString(reader);
                tile.OriginX = reader.ReadSingle();
                tile.OriginY = reader.ReadSingle();
                tile.OriginZ = reader.ReadSingle();
                tile.ScaleX = reader.ReadSingle();
                tile.ScaleY = reader.ReadSingle();
                tile.ScaleZ = reader.ReadSingle();
                tile.QuadsX = reader.ReadUInt16();
                tile.QuadsY = reader.ReadUInt16();
                tile.DecoLayerOffsetUu = reader.ReadSingle();
                tile.SectorQuads = reader.ReadUInt16();
                int words = reader.ReadUInt16();
                tile.VisibilityWords = new int[words];
                for (int i = 0; i < words; i++)
                    tile.VisibilityWords[i] = reader.ReadInt32();
                int layerCount = reader.ReadUInt16();
                tile.Layers = new L2DecoLayerInput[layerCount];
                for (int i = 0; i < layerCount; i++)
                    tile.Layers[i] = ReadLayer(reader);
                if (ms.Position != ms.Length)
                {
                    error = "[Interlude deco] " + tile.TileId + ": " + (ms.Length - ms.Position) + " trailing bytes";
                    tile = null;
                    return false;
                }
            }
        }
        catch (EndOfStreamException)
        {
            error = "[Interlude deco] archive truncated";
            tile = null;
            return false;
        }
        catch (IOException ex)
        {
            error = "[Interlude deco] archive unreadable: " + ex.Message;
            tile = null;
            return false;
        }

        return true;
    }

    static bool LayerInputsPresent(string tileId, int index, L2DecoLayerInput layer, List<string> errors)
    {
        string who = "[Interlude deco] " + tileId + " layer " + index
            + " (" + (string.IsNullOrEmpty(layer.MeshRef) ? "no StaticMesh" : layer.MeshRef) + ")";
        if (!layer.HasShowOnTerrain && !layer.ForceRender)
        {
            errors.Add(who + ": missing ShowOnTerrain");
            return false;
        }

        bool draw = layer.ShowOnTerrain || layer.ForceRender;
        if (!draw)
            return true;

        bool ok = true;
        if (!layer.HasFade)
        {
            errors.Add(who + ": missing FadeoutRadius");
            ok = false;
        }
        if (!layer.HasDensity)
        {
            errors.Add(who + ": missing DensityMultiplier");
            ok = false;
        }
        if (!layer.HasMaxPerQuad)
        {
            errors.Add(who + ": missing MaxPerQuad");
            ok = false;
        }
        if (!layer.HasSeed)
        {
            errors.Add(who + ": missing Seed");
            ok = false;
        }
        if (!layer.HasScale)
        {
            errors.Add(who + ": missing ScaleMultiplier");
            ok = false;
        }
        if (!layer.HasRandomYaw)
        {
            errors.Add(who + ": missing RandomYaw");
            ok = false;
        }
        if (!layer.HasDrawOrder)
        {
            errors.Add(who + ": missing DrawOrder");
            ok = false;
        }
        if (!layer.HasAlign)
        {
            errors.Add(who + ": missing AlignToTerrain");
            ok = false;
        }
        if (!layer.HasShowOnInvisible)
        {
            errors.Add(who + ": missing ShowOnInvisibleTerrain");
            ok = false;
        }
        if (string.IsNullOrEmpty(layer.MeshResourcePath))
        {
            errors.Add(who + ": missing StaticMesh resource");
            ok = false;
        }

        if (string.IsNullOrEmpty(layer.DensityRef))
        {
            errors.Add(who + ": missing DensityMap");
            ok = false;
        }
        else if (layer.DensityAlpha == null || layer.DensityWidth <= 0 || layer.DensityHeight <= 0
            || layer.DensityAlpha.Length != layer.DensityWidth * layer.DensityHeight)
        {
            errors.Add(who + ": DensityMap " + layer.DensityRef + " has no alpha samples");
            ok = false;
        }

        if (layer.MaxPerQuad < 0)
        {
            errors.Add(who + ": missing MaxPerQuad");
            ok = false;
        }

        if (!string.IsNullOrEmpty(layer.ScaleRef)
            && (layer.ScaleAlpha == null || layer.ScaleAlpha.Length != layer.ScaleWidth * layer.ScaleHeight))
        {
            errors.Add(who + ": ScaleMap " + layer.ScaleRef + " has no alpha samples");
            ok = false;
        }

        if (!string.IsNullOrEmpty(layer.ColorRef)
            && (layer.ColorRgba == null || layer.ColorRgba.Length != layer.ColorWidth * layer.ColorHeight * 4))
        {
            errors.Add(who + ": ColorMap " + layer.ColorRef + " has no samples");
            ok = false;
        }

        return ok;
    }

    static byte[] Encode(L2DecoTileFile tile)
    {
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms, Encoding.UTF8, true))
        {
            writer.Write(Encoding.ASCII.GetBytes(Magic));
            writer.Write((ushort)L2DecoTileFile.Version);
            WriteString(writer, tile.TileId);
            writer.Write(tile.OriginX);
            writer.Write(tile.OriginY);
            writer.Write(tile.OriginZ);
            writer.Write(tile.ScaleX);
            writer.Write(tile.ScaleY);
            writer.Write(tile.ScaleZ);
            writer.Write((ushort)tile.QuadsX);
            writer.Write((ushort)tile.QuadsY);
            writer.Write(tile.DecoLayerOffsetUu);
            writer.Write((ushort)tile.SectorQuads);
            writer.Write((ushort)tile.VisibilityWords.Length);
            for (int i = 0; i < tile.VisibilityWords.Length; i++)
                writer.Write(tile.VisibilityWords[i]);
            L2DecoLayerInput[] layers = tile.Layers ?? new L2DecoLayerInput[0];
            writer.Write((ushort)layers.Length);
            for (int i = 0; i < layers.Length; i++)
                WriteLayer(writer, layers[i]);
            writer.Flush();
            return ms.ToArray();
        }
    }

    static void WriteLayer(BinaryWriter writer, L2DecoLayerInput layer)
    {
        ushort flags = 0;
        if (layer.ShowOnTerrain) flags |= FlagShow;
        if (layer.AlignToTerrain) flags |= FlagAlign;
        if (layer.RandomYaw) flags |= FlagYaw;
        if (layer.ShowOnInvisibleTerrain) flags |= FlagInvisible;
        if (layer.LitDirectional) flags |= FlagLit;
        if (layer.DisregardTerrainLighting) flags |= FlagDisregard;
        if (layer.ForceRender) flags |= FlagForce;
        if (!string.IsNullOrEmpty(layer.ScaleRef)) flags |= FlagScaleMap;
        if (!string.IsNullOrEmpty(layer.ColorRef)) flags |= FlagColorMap;
        writer.Write(flags);
        writer.Write(layer.DrawOrder);
        writer.Write((byte)0);
        writer.Write(layer.Seed);
        writer.Write(layer.MaxPerQuad);
        writer.Write(layer.FadeMinUu);
        writer.Write(layer.FadeMaxUu);
        writer.Write(layer.DensityMin);
        writer.Write(layer.DensityMax);
        writer.Write(layer.ScaleXMin);
        writer.Write(layer.ScaleXMax);
        writer.Write(layer.ScaleYMin);
        writer.Write(layer.ScaleYMax);
        writer.Write(layer.ScaleZMin);
        writer.Write(layer.ScaleZMax);
        WriteString(writer, layer.MeshResourcePath);
        WriteString(writer, layer.MeshRef);
        WriteString(writer, layer.DensityRef);
        WriteString(writer, layer.ScaleRef);
        WriteString(writer, layer.ColorRef);
        WriteMap(writer, layer.DensityWidth, layer.DensityHeight, layer.DensityAlpha);
        if ((flags & FlagScaleMap) != 0)
            WriteMap(writer, layer.ScaleWidth, layer.ScaleHeight, layer.ScaleAlpha);
        if ((flags & FlagColorMap) != 0)
            WriteMap(writer, layer.ColorWidth, layer.ColorHeight, layer.ColorRgba);
    }

    static L2DecoLayerInput ReadLayer(BinaryReader reader)
    {
        var layer = new L2DecoLayerInput();
        ushort flags = reader.ReadUInt16();
        layer.ShowOnTerrain = (flags & FlagShow) != 0;
        layer.AlignToTerrain = (flags & FlagAlign) != 0;
        layer.RandomYaw = (flags & FlagYaw) != 0;
        layer.ShowOnInvisibleTerrain = (flags & FlagInvisible) != 0;
        layer.LitDirectional = (flags & FlagLit) != 0;
        layer.DisregardTerrainLighting = (flags & FlagDisregard) != 0;
        layer.ForceRender = (flags & FlagForce) != 0;
        layer.HasShowOnTerrain = true;
        layer.HasFade = true;
        layer.HasDensity = true;
        layer.HasMaxPerQuad = true;
        layer.HasSeed = true;
        layer.HasScale = true;
        layer.HasRandomYaw = true;
        layer.HasDrawOrder = true;
        layer.HasAlign = true;
        layer.HasShowOnInvisible = true;
        layer.DrawOrder = reader.ReadByte();
        reader.ReadByte();
        layer.Seed = reader.ReadInt32();
        layer.MaxPerQuad = reader.ReadInt32();
        layer.FadeMinUu = reader.ReadSingle();
        layer.FadeMaxUu = reader.ReadSingle();
        layer.DensityMin = reader.ReadSingle();
        layer.DensityMax = reader.ReadSingle();
        layer.ScaleXMin = reader.ReadSingle();
        layer.ScaleXMax = reader.ReadSingle();
        layer.ScaleYMin = reader.ReadSingle();
        layer.ScaleYMax = reader.ReadSingle();
        layer.ScaleZMin = reader.ReadSingle();
        layer.ScaleZMax = reader.ReadSingle();
        layer.MeshResourcePath = ReadString(reader);
        layer.MeshRef = ReadString(reader);
        layer.DensityRef = ReadString(reader);
        layer.ScaleRef = ReadString(reader);
        layer.ColorRef = ReadString(reader);
        ReadMap(reader, out layer.DensityWidth, out layer.DensityHeight, out layer.DensityAlpha);
        if ((flags & FlagScaleMap) != 0)
            ReadMap(reader, out layer.ScaleWidth, out layer.ScaleHeight, out layer.ScaleAlpha);
        if ((flags & FlagColorMap) != 0)
            ReadMap(reader, out layer.ColorWidth, out layer.ColorHeight, out layer.ColorRgba);
        return layer;
    }

    static void WriteMap(BinaryWriter writer, int width, int height, byte[] raw)
    {
        writer.Write((ushort)width);
        writer.Write((ushort)height);
        byte[] payload = EncodePayload(raw);
        writer.Write(payload.Length);
        writer.Write(payload);
    }

    static void ReadMap(BinaryReader reader, out int width, out int height, out byte[] raw)
    {
        width = reader.ReadUInt16();
        height = reader.ReadUInt16();
        int len = reader.ReadInt32();
        if (len < 1)
            throw new EndOfStreamException();
        byte[] payload = reader.ReadBytes(len);
        if (payload.Length != len)
            throw new EndOfStreamException();
        raw = DecodePayload(payload, width * height);
    }

    static byte[] EncodePayload(byte[] raw)
    {
        if (raw == null)
            raw = new byte[0];
        byte[] rle = Rle(raw);
        bool useRle = rle.Length < raw.Length;
        byte[] chosen = useRle ? rle : raw;
        var packed = new byte[1 + chosen.Length];
        packed[0] = useRle ? (byte)1 : (byte)0;
        Buffer.BlockCopy(chosen, 0, packed, 1, chosen.Length);
        return packed;
    }

    static byte[] DecodePayload(byte[] payload, int count)
    {
        if (payload == null || payload.Length < 1)
            throw new EndOfStreamException();
        if (payload[0] == 0)
        {
            if (payload.Length != 1 + count)
                throw new InvalidDataException("raw map length");
            var raw = new byte[count];
            Buffer.BlockCopy(payload, 1, raw, 0, count);
            return raw;
        }

        if (payload[0] != 1)
            throw new InvalidDataException("map codec");
        return Unrle(payload, 1, payload.Length - 1, count);
    }

    static byte[] Rle(byte[] raw)
    {
        var runs = new List<byte>(raw.Length / 8 + 8);
        int i = 0;
        while (i < raw.Length)
        {
            byte value = raw[i];
            int j = i + 1;
            int limit = i + 65535;
            if (limit > raw.Length)
                limit = raw.Length;
            while (j < limit && raw[j] == value)
                j++;
            int count = j - i;
            runs.Add((byte)(count & 0xFF));
            runs.Add((byte)((count >> 8) & 0xFF));
            runs.Add(value);
            i = j;
        }

        return runs.ToArray();
    }

    static byte[] Unrle(byte[] payload, int offset, int length, int count)
    {
        var raw = new byte[count];
        int o = 0;
        int p = offset;
        int end = offset + length;
        while (p + 3 <= end)
        {
            int run = payload[p] | (payload[p + 1] << 8);
            byte value = payload[p + 2];
            p += 3;
            if (run <= 0 || o + run > count)
                throw new InvalidDataException("rle overrun");
            for (int k = 0; k < run; k++)
                raw[o++] = value;
        }

        if (o != count || p != end)
            throw new InvalidDataException("rle length");
        return raw;
    }

    static void WriteString(BinaryWriter writer, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            writer.Write((ushort)0);
            return;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length > ushort.MaxValue)
            throw new InvalidDataException("string too long");
        writer.Write((ushort)bytes.Length);
        writer.Write(bytes);
    }

    static string ReadString(BinaryReader reader)
    {
        int len = reader.ReadUInt16();
        if (len == 0)
            return string.Empty;
        byte[] bytes = reader.ReadBytes(len);
        if (bytes.Length != len)
            throw new EndOfStreamException();
        return Encoding.UTF8.GetString(bytes);
    }
}
