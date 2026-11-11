using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class LobbyNameplatesManager
{
    private void BuildPaintList(Camera cam)
    {
        _paintList.Clear();

        CharacterSelector selector = LoginPacketServices.Characters;
        if (selector == null)
        {
            return;
        }

        IReadOnlyList<GameObject> pawns = selector.CharacterPawns;
        if (pawns == null)
        {
            return;
        }

        int count = Mathf.Min(pawns.Count, MaxSlots);
        for (int i = 0; i < count; i++)
        {
            GameObject pawn = pawns[i];
            if (pawn == null)
            {
                continue;
            }

            SelectableCharacterEntity entity = pawn.GetComponent<SelectableCharacterEntity>();
            if (entity == null || entity.CharacterInfoInterlude == null)
            {
                continue;
            }

            Transform t = entity.transform;
            if (!IsNameplateVisible(cam, t))
            {
                continue;
            }

            CharSelectInfoPackage info = entity.CharacterInfoInterlude;
            if (string.IsNullOrEmpty(info.Name))
            {
                continue;
            }

            _paintList.Add(new PaintItem
            {
                Slot = i,
                World = L2OverheadAnchor.GetWorldPos(t),
                Name = info.Name,
                Color = ResolveNameColor(info.Karma)
            });
        }
    }

    private Color ResolveNameColor(int karma)
    {
        if (karma <= 0)
        {
            return _defaultNameColor;
        }

        float t = Mathf.Clamp01(karma / 1000f);
        return Color.Lerp(Color.white, new Color(1f, 0.25f, 0.25f, 1f), t);
    }

    private bool IsNameplateVisible(Camera cam, Transform target)
    {
        if (target == null || cam == null)
        {
            return false;
        }

        return Vector3.Distance(cam.transform.position, target.position) <= _nameplateViewDistance;
    }
}
