using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;


/// <summary>
/// One frame of the texture the live combine actually samples, plus the graded result.
/// Raw layout matches the client capture: file row 0 is the top of the picture.
/// </summary>
public static partial class L2YebisLiveCapture
{
    public const string Root =
        @"C:\games\Lineage II HighElfes\ADEU-P464-D20240703-P-240117-240724-1\system\logs\YebisCapture\unity_live";

    static int _request;
    static int _gradeStage;

    public static int GradeStage => _gradeStage;

    struct Region
    {
        public readonly string Name;
        public readonly float X;
        public readonly float Y;

        public Region(string name, float x, float y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }
}
