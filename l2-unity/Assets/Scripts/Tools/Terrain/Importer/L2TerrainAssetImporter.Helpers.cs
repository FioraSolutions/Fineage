#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract partial class L2TerrainAssetImporter
{
    protected static string GetParentFolder(string path) {
        string currentDirectory = path;
        if(Path.GetExtension(path) != string.Empty) {
            currentDirectory = Path.GetDirectoryName(path);
        }

        string parentFolderFullPath = Directory.GetParent(currentDirectory).FullName;
        string parentFolderRelativePath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, parentFolderFullPath));
        return parentFolderRelativePath;
    }

    protected static string GetFolderName(string path) {
        string folderName = Path.GetFileName(path);
        return folderName;
    }
}
#endif
