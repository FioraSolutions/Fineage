using UnityEngine;
using VContainer;

public partial class AbstractMeshManager : MonoBehaviour
{
    protected void SetTypeOtherGo(GameObject go, WeaponAttachMount mount, Transform[] allBone)
    {
        ApplyWeaponMount(go, mount, allBone);
    }

    protected void SetType(WeaponAttachEquipMode mode, Weapongrp grp, Transform[] allBone)
    {
        if (_go == null || grp == null)
            return;
        WeaponAttachProfile profile = WeaponAttachProfileResolver.Resolve(grp, mode);
        if (profile.Mounts.Count == 0)
            return;
        ApplyWeaponMount(_go, profile.Mounts[0], allBone);
    }

    public static void ApplyWeaponMount(GameObject go, WeaponAttachMount mount, Transform[] allBone)
    {
        if (go == null || allBone == null)
            return;
        int socketIndex = (int)mount.Socket;
        if (socketIndex < 0 || socketIndex >= allBone.Length)
            return;
        Transform socket = allBone[socketIndex];
        if (socket == null)
            return;
        WeaponAttachApplicator.Apply(go, socket, mount);
    }

    protected void SetTypeFromMount(WeaponAttachMount mount, Transform[] allBone)
    {
        ApplyWeaponMount(_go, mount, allBone);
    }
}