#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public partial class L2T3DStaticMeshImporter
{
    static List<string> GetTextureNames(string inputText, string pattern) {

        List<string> items = new List<string>();

        Regex regex = new Regex(pattern);

        MatchCollection matches = regex.Matches(inputText);

        foreach(Match match in matches) {
            if(!items.Contains(match.Value)) {
                items.Add(match.Value
                    .Replace("Texture'", string.Empty)
                    .Replace("Shader'", string.Empty));
            }
        }

        return items;
    }

    static string FixPath(string baseFolder, string fileName, bool shader) {
        string pathToTest = string.Empty;
        string origFileName = fileName;
        if(shader) {
            fileName = "Materials/" + fileName;
        }

        if(fileName.ToLower().StartsWith("interior_")) {
            string[] interiorParts = fileName.Split('_');
            string folder = interiorParts[0] + "_" + interiorParts[1] + (interiorParts[2].StartsWith("ch") ? "CH_T" : "_T");
            pathToTest = Path.Combine(baseFolder, folder, fileName);
        }

        if(!File.Exists(pathToTest)) {
            pathToTest = Path.Combine(baseFolder, "FX_E_T", fileName);
        }
        if(!File.Exists(pathToTest)) {
            pathToTest = Path.Combine(baseFolder, "SI_V_T", fileName);
        }
        if(!File.Exists(pathToTest)) {
            pathToTest = Path.Combine(baseFolder, "speakingfighter_t", fileName);
        }
        if(!File.Exists(pathToTest)) {
            pathToTest = Path.Combine(baseFolder, "Gludio_Port_T", fileName);
        }
        if(!File.Exists(pathToTest)) {
            string searchedFile = FindInSubDirectories(baseFolder, origFileName);
            //Debug.LogWarning("Searched and found " + searchedFile);

            if (searchedFile == null && shader) {
                // Debug.LogWarning("Could not find material props at path " + pathToTest);
            }

            pathToTest = searchedFile;
        }

        return pathToTest;
    }
}
#endif
