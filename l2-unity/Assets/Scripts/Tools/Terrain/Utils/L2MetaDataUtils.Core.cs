#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class L2MetaDataUtils
{
    public static float ParseFloatFromInfo(string info) {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return float.Parse(valueString);
    }

    public static bool ParseBoolFromInfo(string info) {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return valueString.Equals("1");
    }

    public static int ParseIntFromInfo(string info) {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return int.Parse(valueString);
    }

    public static string[] GetFolderAndFileFromInfo(string value) {
        string textureName = value.Contains("=") ? value.Split('=')[1] : value;

        textureName = textureName.Replace("Texture'", string.Empty);
        textureName = textureName.Replace("StaticMesh'", string.Empty);
        /*textureName = textureName.Replace(".Texture", string.Empty);
		textureName = textureName.Replace("Height.", string.Empty);*/
        textureName = textureName.Replace("'", string.Empty);

        string[] result = textureName.Split('.');

        if (result.Length == 2) {
            return result;
        } else if (result.Length > 2) {
            return new string[2] { result[0], result[2] };
        }

        return result;
    }
}
#endif