using UnityEngine;
using System;

public partial class UserGear : Gear
{
    const string ModelChildName = "model";

    protected SkinnedMeshSync _skinnedMeshSync;
    [SerializeField] private GameObject _container;

    [System.NonSerialized] public GameObject Hair1;
    [System.NonSerialized] public GameObject Hair2;
    [System.NonSerialized] public GameObject Face;

    private CharacterArmorDresser _armorDresser;
    ArmorVisualResolver _armorVisuals;

    ArmorVisualResolver ArmorVisuals =>
        _armorVisuals ??= ArmorVisualResolver.FromTables();
}
