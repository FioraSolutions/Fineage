using UnityEngine;
using VContainer;

public partial class AbstractMeshManager : MonoBehaviour
{
    protected void SetTypeOtherGo(GameObject go, WeaponType weaponType, bool leftSlot, Transform[] allBone)
    {
        SetParentGo(go, weaponType, leftSlot, allBone);
    }

    void SetParentGo(GameObject go, WeaponType weaponType, bool leftSlot, Transform[] allBone)
    {
        if (go == null)
            return;

        // Shield and EquipShield (WeaponType.none) both bind to Shield_L_Bone.
        if (weaponType == WeaponType.none || weaponType == WeaponType.shield)
            go.transform.SetParent(allBone[0], false);
        else if (weaponType == WeaponType.bow)
            go.transform.SetParent(allBone[1], false);
        else if (weaponType == WeaponType.arrow)
        {
            go.transform.SetParent(allBone[3], false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }
        else if (leftSlot)
            go.transform.SetParent(allBone[2], false);
        else
            go.transform.SetParent(allBone[3], false);

        // Shield FBX meshes are two slots (face + rim). Rebuilt prefabs often keep
        // only slot 0, so the face draws black. Fist claws are one thin sheet:
        // the left copy is the right mesh mirrored, and both sides of the sheet show.
        RepairSubmeshMaterials(go);
        if (IsFistWeapon(weaponType))
        {
            Vector3 scale = go.transform.localScale;
            float x = Mathf.Abs(scale.x);
            if (x < 0.0001f)
                x = 1f;
            go.transform.localScale = new Vector3(leftSlot ? -x : x, scale.y, scale.z);
            ForceDoubleSided(go);
        }
        else if (weaponType == WeaponType.none || weaponType == WeaponType.shield)
        {
            ForceDoubleSided(go);
        }
    }

    static bool IsFistWeapon(WeaponType weaponType)
    {
        return weaponType == WeaponType.fist || weaponType == WeaponType.dualfist;
    }

    static void RepairSubmeshMaterials(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
                continue;
            int submeshes = SubmeshCount(renderer);
            Material[] current = renderer.sharedMaterials;
            if (submeshes <= 1 || current == null || current.Length >= submeshes)
                continue;

            Material fallback = null;
            for (int m = 0; m < current.Length; m++)
            {
                if (current[m] != null)
                {
                    fallback = current[m];
                    break;
                }
            }
            if (fallback == null)
                continue;

            var filled = new Material[submeshes];
            for (int m = 0; m < submeshes; m++)
                filled[m] = m < current.Length && current[m] != null ? current[m] : fallback;
            renderer.sharedMaterials = filled;
        }
    }

    static int SubmeshCount(Renderer renderer)
    {
        if (renderer is SkinnedMeshRenderer skin && skin.sharedMesh != null)
            return skin.sharedMesh.subMeshCount;
        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        if (filter != null && filter.sharedMesh != null)
            return filter.sharedMesh.subMeshCount;
        return 0;
    }

    static void ForceDoubleSided(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
                continue;
            // .materials instantiates every read. Copy once, then leave the instance
            // on the pooled renderer so the next equip does not allocate again.
            Material[] shared = renderer.sharedMaterials;
            bool needsCopy = false;
            for (int m = 0; m < shared.Length; m++)
            {
                Material mat = shared[m];
                if (mat != null && mat.HasProperty("_Cull") && mat.GetFloat("_Cull") > 0.5f)
                {
                    needsCopy = true;
                    break;
                }
            }
            if (!needsCopy)
                continue;

            var copy = new Material[shared.Length];
            for (int m = 0; m < shared.Length; m++)
            {
                if (shared[m] == null)
                    continue;
                copy[m] = new Material(shared[m]);
                if (copy[m].HasProperty("_Cull"))
                    copy[m].SetFloat("_Cull", 0f);
            }
            renderer.sharedMaterials = copy;
        }
    }

    public void GetDefaultGoWithArmorModel(
        ItemSlot slot,
        out Armor[] defaultArmor,
        out GameObject[] listArmorPiece,
        int raceId)
    {
        slot = ArmorBodySlots.Normalize(slot);
        defaultArmor = CharacterDefaultEquipment.GetDefaultArmorByItemSlot(slot);
        listArmorPiece = InstantiateArmorMeshes(defaultArmor, raceId);
    }
}
