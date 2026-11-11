using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateBubbleDrawer
{
    public void Dispose()
    {
        _batchNormal.Dispose();
        _batchTarget.Dispose();
        _batchAttack.Dispose();
    }

    private static void DrawForType(
        Camera cam,
        float screenH,
        List<NameplatePaintItem> paintList,
        L2TargetRenderType type,
        Texture2D tex,
        L2NameplateScreenBatch batch)
    {
        if (tex == null || batch == null)
        {
            return;
        }

        batch.EnsureMaterial(tex);
        batch.BeginFrame();

        for (int i = 0; i < paintList.Count; i++)
        {
            NameplatePaintItem item = paintList[i];
            if (!item.ScreenValid || item.BubbleType != type)
            {
                continue;
            }

            L2NameplateBubbles.AppendPair(
                batch,
                item.XName,
                item.NameW,
                item.YNameTop,
                item.Depth,
                screenH,
                0f,
                Color.white);
        }

        batch.UploadAndDraw(cam);
    }
}
