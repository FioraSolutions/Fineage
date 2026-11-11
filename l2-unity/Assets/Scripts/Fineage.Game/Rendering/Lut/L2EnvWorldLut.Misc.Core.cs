using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    static bool NeighborsClean(float hours, byte fill)
    {
        for (int i = 0; i < NeighborGrids.Length; i++)
        {
            NeighborIntensityGrid grid = NeighborGrids[i];
            if (!grid.tried)
                return false;
            if (grid.slices.Count == 0)
                continue;
            int a;
            int b;
            float u;
            PickIntensitySlices(grid.slices, hours, out a, out b, out u);
            if (grid.tex == null || a != grid.sliceA || b != grid.sliceB || fill != grid.fill)
                return false;
        }
        return true;
    }
}
