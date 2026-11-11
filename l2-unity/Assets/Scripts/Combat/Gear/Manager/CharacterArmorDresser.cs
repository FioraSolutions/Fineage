using System;
using UnityEngine;

public sealed partial class CharacterArmorDresser
{
    struct Worn
    {
        public Armor Armor;
        public GameObject Mesh;
    }

    readonly Transform _container;

    Worn _chest;
    Worn _legs;
    Worn _gloves;
    Worn _feet;
    bool _fullBody;

    public Action<GameObject> OnDestroyGameObject;
    public Action OnSyncMesh;






}
