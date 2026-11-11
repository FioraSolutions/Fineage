using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    static void EnsureBaked17_25()
    {
        if (_baked17Tried)
            return;
        _baked17Tried = true;
        _gridTried = true;
        if (!ReadIntensityGrid(Baked17_25IntensityFileName, IntensitySlices))
            Debug.LogWarning("[L2EnvWorldLut] 17_25 baked intensity grid was not loaded.");
    }

    static void EnsureIntensityGrid()
    {
        if (_gridTried)
            return;
        _gridTried = true;
        string fileName = UseStoredClientIntensityGrid ? StoredIntensityGridFileName : IntensityGridFileName;
        string path = Path.Combine(Application.streamingAssetsPath, "Data/Meta", fileName);
        if (!File.Exists(path))
        {
            // Debug.LogWarning("[L2EnvWorldLut] Intensity grid missing (" + fileName + ")");
            return;
        }

        byte[] file = File.ReadAllBytes(path);
        if (file.Length < 16 || file[0] != (byte)'L' || file[1] != (byte)'2' || file[2] != (byte)'I' || file[3] != (byte)'G')
            return;
        int version = BitConverter.ToInt32(file, 4);
        int n = BitConverter.ToInt32(file, 8);
        int count = BitConverter.ToInt32(file, 12);
        if (version != 2 || n != IntensityGridSize || count <= 0)
            return;

        int cursor = 16;
        int plane = n * n;
        for (int s = 0; s < count; s++)
        {
            if (cursor + 4 + plane * 2 > file.Length)
                break;
            float hours = BitConverter.ToSingle(file, cursor);
            cursor += 4;
            var intensity = new byte[plane];
            var valid = new byte[plane];
            System.Buffer.BlockCopy(file, cursor, intensity, 0, plane);
            cursor += plane;
            System.Buffer.BlockCopy(file, cursor, valid, 0, plane);
            cursor += plane;
            IntensitySlices.Add(new IntensitySlice { tHours = hours, intensity = intensity, valid = valid });
        }

        IntensitySlices.Sort((x, y) => x.tHours.CompareTo(y.tHours));
        // Debug.Log("[L2EnvWorldLut] Loaded " + IntensitySlices.Count + " intensity slices from " + fileName);
    }

    static void EnsureNeighborGrids()
    {
        for (int i = 0; i < NeighborGrids.Length; i++)
        {
            NeighborIntensityGrid grid = NeighborGrids[i];
            if (grid.tried)
                continue;
            grid.tried = true;
            if (!ReadIntensityGrid(grid.fileName, grid.slices))
                Shader.SetGlobalVector(grid.originId, Vector4.zero);
        }
    }

    static bool ReadIntensityGrid(string fileName, List<IntensitySlice> destination)
    {
        destination.Clear();
        string path = Path.Combine(Application.streamingAssetsPath, "Data/Meta", fileName);
        if (!File.Exists(path))
        {
            // Debug.LogWarning("[L2EnvWorldLut] Intensity grid missing (" + fileName + ")");
            return false;
        }

        byte[] file = File.ReadAllBytes(path);
        if (file.Length < 16 || file[0] != (byte)'L' || file[1] != (byte)'2' || file[2] != (byte)'I' || file[3] != (byte)'G')
            return false;
        int version = BitConverter.ToInt32(file, 4);
        int n = BitConverter.ToInt32(file, 8);
        int count = BitConverter.ToInt32(file, 12);
        if (version != 2 || n != IntensityGridSize || count <= 0)
            return false;

        int cursor = 16;
        int plane = n * n;
        for (int s = 0; s < count; s++)
        {
            if (cursor + 4 + plane * 2 > file.Length)
                break;
            float hours = BitConverter.ToSingle(file, cursor);
            cursor += 4;
            var intensity = new byte[plane];
            var valid = new byte[plane];
            System.Buffer.BlockCopy(file, cursor, intensity, 0, plane);
            cursor += plane;
            System.Buffer.BlockCopy(file, cursor, valid, 0, plane);
            cursor += plane;
            destination.Add(new IntensitySlice { tHours = hours, intensity = intensity, valid = valid });
        }

        destination.Sort((x, y) => x.tHours.CompareTo(y.tHours));
        // Debug.Log("[L2EnvWorldLut] Loaded " + destination.Count + " intensity slices from " + fileName);
        return destination.Count > 0;
    }
}
