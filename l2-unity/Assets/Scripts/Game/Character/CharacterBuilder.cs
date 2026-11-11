using System;
using UnityEngine;
using VContainer;

public partial class CharacterBuilder : MonoBehaviour
{
    private static CharacterBuilder _instance;

    [System.Obsolete("Use [Inject] CharacterBuilder or CharacterPacketServices.Builder instead")]
    public static CharacterBuilder Instance { get { return _instance; } }

    [Inject] private ModelTable _models;

    private ModelTable Models
    {
        get { return _models != null ? _models : TablePacketServices.Models; }
    }


    // Load player animations, face and hair










}
