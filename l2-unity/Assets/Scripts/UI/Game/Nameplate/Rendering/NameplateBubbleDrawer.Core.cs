using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateBubbleDrawer
{
    public void EnsureResources()
    {
        if (!_enabled)
        {
            return;
        }

        if (_texNormal == null)
        {
            _texNormal = L2NameplateBubbles.LoadTexture(L2NameplateBubbles.NormalResourcePath);
        }

        if (_texTarget == null)
        {
            _texTarget = L2NameplateBubbles.LoadTexture(L2NameplateBubbles.TargetResourcePath);
        }

        if (_texAttack == null)
        {
            _texAttack = L2NameplateBubbles.LoadTexture(L2NameplateBubbles.AttackResourcePath);
        }

        if (_texNormal != null)
        {
            _batchNormal.EnsureMaterial(_texNormal);
        }

        if (_texTarget != null)
        {
            _batchTarget.EnsureMaterial(_texTarget);
        }

        if (_texAttack != null)
        {
            _batchAttack.EnsureMaterial(_texAttack);
        }
    }

    public void Draw(Camera cam, List<NameplatePaintItem> paintList)
    {
        if (!_enabled || cam == null || paintList == null)
        {
            return;
        }

        bool anyReady = _batchNormal.IsReady || _batchTarget.IsReady || _batchAttack.IsReady;
        if (!anyReady)
        {
            return;
        }

        bool anyNormal = false;
        bool anyTarget = false;
        bool anyAttack = false;

        for (int i = 0; i < paintList.Count; i++)
        {
            NameplatePaintItem item = paintList[i];
            if (!item.ScreenValid)
            {
                continue;
            }

            switch (item.BubbleType)
            {
                case L2TargetRenderType.Normal:
                    anyNormal = true;
                    break;
                case L2TargetRenderType.Target:
                    anyTarget = true;
                    break;
                case L2TargetRenderType.Attack:
                    anyAttack = true;
                    break;
            }
        }

        float screenH = cam.pixelHeight;

        if (anyNormal)
        {
            DrawForType(cam, screenH, paintList, L2TargetRenderType.Normal, _texNormal, _batchNormal);
        }

        if (anyTarget)
        {
            DrawForType(cam, screenH, paintList, L2TargetRenderType.Target, _texTarget, _batchTarget);
        }

        if (anyAttack)
        {
            DrawForType(cam, screenH, paintList, L2TargetRenderType.Attack, _texAttack, _batchAttack);
        }
    }
}
