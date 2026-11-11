using UnityEngine;
using System.Collections.Generic;

public partial class L2RibbonEmitter
{
    private void ResolveBladeAnchors()
    {
        if (_swordTip != null && _swordBase != null && _weaponRoot != null)
        {
            return;
        }

        Transform searchRoot = ResolveSearchRoot();
        Transform weapon = _weaponRoot;
        Entity owner = ResolveOwnerEntity(searchRoot);

        if (weapon == null && owner != null)
        {
            weapon = owner.GetWeaponTransform();
        }

        Transform[] points = owner != null ? owner.GetSwordBasePoints() : null;
        if (points != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == null)
                {
                    continue;
                }

                if (_swordBase == null && points[i].name == _swordBaseName)
                {
                    _swordBase = points[i];
                }

                if (_swordTip == null && points[i].name == _swordTipName)
                {
                    _swordTip = points[i];
                }
            }
        }

        if (weapon == null && searchRoot != null)
        {
            Transform bone = searchRoot.FindRecursive(_autoBoneName);
            if (bone != null)
            {
                // weapon_* usually under Weapon_R_Bone
                for (int i = 0; i < bone.childCount; i++)
                {
                    Transform child = bone.GetChild(i);
                    if (child.name.StartsWith("weapon_"))
                    {
                        weapon = child;
                        break;
                    }
                }

                if (weapon == null)
                {
                    weapon = bone;
                }
            }
        }

        Transform tipRoot = weapon != null ? weapon : searchRoot;
        if (tipRoot != null)
        {
            if (_swordTip == null)
            {
                _swordTip = tipRoot.Find(_swordTipName) ?? tipRoot.FindRecursive(_swordTipName);
            }

            if (_swordBase == null)
            {
                _swordBase = tipRoot.Find(_swordBaseName) ?? tipRoot.FindRecursive(_swordBaseName);
            }
        }

        if (_sampleBone == null && !string.IsNullOrEmpty(_autoBoneName) && searchRoot != null)
        {
            _sampleBone = searchRoot.FindRecursive(_autoBoneName);
        }

        if (_sampleBone == null && weapon != null)
        {
            _sampleBone = weapon;
        }

        _weaponRoot = weapon != null ? weapon : tipRoot;

        float markerLen = (_swordTip != null && _swordBase != null)
            ? Vector3.Distance(_swordTip.position, _swordBase.position)
            : 0f;
        float meshLen = 0f;
        if (_weaponRoot != null &&
            L2FxRibbonGetPoint.TryGetMeshBladeEnds(_weaponRoot, out Vector3 meshTip, out Vector3 meshHilt))
        {
            meshLen = Vector3.Distance(meshTip, meshHilt);
        }

        Log(
            $"ResolveBlade search='{(searchRoot != null ? searchRoot.name : "null")}' " +
            $"weapon='{(weapon != null ? weapon.name : "null")}' " +
            $"tip='{(_swordTip != null ? FullPath(_swordTip) : "null")}' " +
            $"base='{(_swordBase != null ? FullPath(_swordBase) : "null")}' " +
            $"bone='{(_sampleBone != null ? _sampleBone.name : "null")}' " +
            $"markerLen={markerLen:F3} meshLen={meshLen:F3}");
    }

    private static string FullPath(Transform t)
    {
        if (t == null)
        {
            return "null";
        }

        string path = t.name;
        Transform p = t.parent;
        int guard = 0;
        while (p != null && guard++ < 8)
        {
            path = p.name + "/" + path;
            p = p.parent;
        }

        return path;
    }

    private void ApplyOpacityToMaterial()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        UpdateShaderFloat("_Opacity", _opacity);
    }
}
