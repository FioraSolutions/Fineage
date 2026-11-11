using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class ItemDropPresentationService
{
    sealed class ThrowArcState
    {
        public int ItemObjectId;
        public ItemEntity Item;
        public Vector3 From;
        public Vector3 To;
        public float Elapsed;
        public float Duration;
        public ItemDropVisualKind Kind;
        public Vector3 TumbleAxis;
        public float TumbleDegPerSec;
        public Vector3 LandEuler;
        public bool StickOnLand;
        public float ArcHeight;
    }

    sealed class GroundGlowState
    {
        public GameObject GlowAnchor;
        public float EndTime;
    }

    sealed class CoinMeshRevealState
    {
        public ItemEntity Item;
        public float RevealTime;
    }

    readonly List<ThrowArcState> _throws = new List<ThrowArcState>(8);
    readonly Dictionary<int, GroundGlowState> _groundGlowByItemId = new Dictionary<int, GroundGlowState>();
    readonly Dictionary<int, CoinMeshRevealState> _coinRevealByItemId = new Dictionary<int, CoinMeshRevealState>();
    readonly Dictionary<int, GameObject> _landBurstByItemId = new Dictionary<int, GameObject>();
    readonly List<int> _expiredGlowIds = new List<int>(4);
    readonly List<int> _expiredCoinRevealIds = new List<int>(4);
    readonly ItemDropClickAreaService _clickArea;
    readonly ItemDropWeaponAligner _weapons;
    readonly ItemDropLayerService _layers;
    readonly ItemDropGrpCatalog _grp;
    readonly IAnimationManager _animations;
    readonly EffectManager _effects;


    private World GameWorld
    {
        get { return ItemPacketServices.GameWorld ?? GamePacketServices.GameWorld; }
    }

    private IAnimationManager Animations
    {
        get { return _animations != null ? _animations : ItemPacketServices.Animations; }
    }

    private EffectManager Effects
    {
        get { return _effects != null ? _effects : ItemPacketServices.Effects; }
    }





}
