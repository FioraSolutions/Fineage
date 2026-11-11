using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class ItemDropPresentationService
{
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

    public void Tick(float deltaTime)
    {
        TickThrows(deltaTime);
        TickGlows();
        TickCoinMeshReveals();
    }
}
