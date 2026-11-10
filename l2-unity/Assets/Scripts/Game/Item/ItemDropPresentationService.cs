using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ItemDropPresentationService
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

    public ItemDropPresentationService(
        ItemDropClickAreaService clickArea,
        ItemDropWeaponAligner weapons,
        ItemDropLayerService layers,
        ItemDropGrpCatalog grp,
        IAnimationManager animations,
        EffectManager effects)
    {
        _clickArea = clickArea;
        _weapons = weapons;
        _layers = layers;
        _grp = grp;
        _animations = animations;
        _effects = effects;
    }

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

    public void Tick(float deltaTime)
    {
        TickThrows(deltaTime);
        TickGlows();
        TickCoinMeshReveals();
    }

    public void PlayDrop(ItemEntity item, Vector3 landPos, int dropperCharObjId)
    {
        if (item == null)
            return;

        int itemObjectId = item.Identity != null ? item.Identity.Id : 0;
        ItemDropVisualKind kind = ResolveVisualKind(item);
        Entity dropper = ResolveEntity(dropperCharObjId);
        landPos = GroundSnapHelper.SnapToGroundOrKeep(landPos);

        if (dropper != null)
            PlayActorAnimation(dropperCharObjId, ItemDropPresentationIds.DropAnimationTrigger);

        int fallEffectId = ResolveFallEffectId(kind);
        if (fallEffectId > 0 && fallEffectId != ItemDropPresentationIds.LandBurstEffectId)
        {
            PlayEffect(
                fallEffectId,
                dropper != null ? dropper.transform : item.transform,
                "fall-trail");
        }

        // Adena: FX toss, no mesh throw. Everything else flies from the hand
        // (or from a short hover if the packet has no dropper).
        bool throwMesh = kind != ItemDropVisualKind.Adena;
        if (throwMesh)
        {
            Vector3 from = ResolveThrowStart(dropper, landPos);
            StopThrow(itemObjectId);
            BeginThrow(item, from, landPos, itemObjectId, kind);
        }
        else
        {
            LandItem(item, landPos, itemObjectId, kind);
        }

    }

    public void StopItemPresentation(int itemObjectId)
    {
        StopThrow(itemObjectId);
        StopGroundGlow(itemObjectId);
        StopLandBurst(itemObjectId);
        CancelCoinMeshReveal(itemObjectId);
    }

    public void PlayPickup(int itemObjectId, int pickerCharObjId, Vector3 unityPos)
    {
        StopItemPresentation(itemObjectId);
        PlayActorCrossFade(pickerCharObjId, ItemDropPresentationIds.PickupAnimationState);
        PlayEffect(ItemDropPresentationIds.PickupBurstEffectId, unityPos, "pickup-burst");
    }

}
