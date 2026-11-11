using System.Collections.Generic;
using UnityEngine;

public sealed partial class NpcDecoCatalog
{
    public bool TryLoadPieces(string decoEffectName, List<NpcDecoPiece> pieces)
    {
        pieces.Clear();
        if (string.IsNullOrWhiteSpace(decoEffectName))
            return false;

        string shortName = ShortName(decoEffectName);
        string folder = ResourcesFolder + "/" + shortName;
        Object[] folderAssets = Resources.LoadAll(folder, typeof(GameObject));
        if (folderAssets != null && folderAssets.Length > 0)
        {
            for (int i = 0; i < folderAssets.Length; i++)
            {
                GameObject go = folderAssets[i] as GameObject;
                if (go == null)
                    continue;

                if (go.GetComponent<CompositePrefabEffect>() != null)
                    continue;

                BaseEffect effect = go.GetComponent<BaseEffect>();
                if (effect == null)
                    continue;

                pieces.Add(new NpcDecoPiece
                {
                    Prefab = effect,
                    Attach = NpcDecoAttachment.FromPieceName(go.name),
                    Label = go.name
                });
            }

            if (pieces.Count > 0)
                return true;
        }

        BaseEffect single = LoadAt(ResourcesFolder + "/" + shortName);
        if (single == null)
            single = LoadAt(folder + "/" + shortName);
        if (single == null)
            return false;

        pieces.Add(new NpcDecoPiece
        {
            Prefab = single,
            Attach = NpcDecoAttachment.FromPieceName(single.name),
            Label = single.name
        });
        return true;
    }
}
