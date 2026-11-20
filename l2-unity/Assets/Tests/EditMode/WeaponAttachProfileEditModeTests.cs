using NUnit.Framework;
using UnityEngine;

public sealed class WeaponAttachProfileEditModeTests
{
    [Test]
    public void DualFistSameMesh_MirrorsLeftAtSocket()
    {
        var grp = new Weapongrp
        {
            Model = "LineageWeapons.demon_splinter_m00_wp",
            ModelLeft = "LineageWeapons.demon_splinter_m00_wp",
            BodyPart = ItemSlot.lrhand,
            WeaponType = WeaponType.dualfist,
        };

        var profile = WeaponAttachProfileResolver.Resolve(grp, WeaponAttachEquipMode.BothHands);
        Assert.AreEqual(2, profile.Mounts.Count);
        Assert.AreEqual(WeaponAttachSocket.RightHand, profile.Mounts[0].Socket);
        Assert.IsFalse(profile.Mounts[0].MirrorScaleX);
        Assert.AreEqual(WeaponAttachSocket.LeftHand, profile.Mounts[1].Socket);
        Assert.IsTrue(profile.Mounts[1].MirrorScaleX);
    }

    [Test]
    public void DualSwords_DistinctMeshes_NoMirror()
    {
        var grp = new Weapongrp
        {
            Model = "LineageWeapons.saber_m00_wp",
            ModelLeft = "LineageWeapons.bastard_sword_m00_wp",
            BodyPart = ItemSlot.lrhand,
            WeaponType = WeaponType.dual,
        };

        var profile = WeaponAttachProfileResolver.Resolve(grp, WeaponAttachEquipMode.BothHands);
        Assert.AreEqual("LineageWeapons.bastard_sword_m00_wp", profile.Mounts[1].ModelKey);
        Assert.IsFalse(profile.Mounts[1].MirrorScaleX);
    }

    [Test]
    public void Apply_SnapsIdentityAtSocket()
    {
        var socket = new GameObject("Weapon_R_Bone").transform;
        var weapon = new GameObject("weapon");
        weapon.transform.position = new Vector3(9f, 0f, 0f);
        weapon.transform.rotation = Quaternion.Euler(15f, 30f, 45f);

        var mount = new WeaponAttachMount(WeaponAttachSocket.RightHand, "LineageWeapons.test_m00_wp");
        WeaponAttachApplicator.Apply(weapon, socket, mount);

        Assert.AreEqual(socket, weapon.transform.parent);
        Assert.AreEqual(Vector3.zero, weapon.transform.localPosition);
        Assert.AreEqual(Quaternion.identity, weapon.transform.localRotation);
        Object.DestroyImmediate(socket.gameObject);
        Object.DestroyImmediate(weapon);
    }
}