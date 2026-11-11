
using UnityEngine;

public sealed partial class ItemDropVisualService
{
    const float CoinDropScale = 3f * 1.8f;
    const float DropPropWorldSize = 0.18f;
    const float HerbDropScale = 2f;

    readonly ItemDropPrefabLoader _prefabs;
    readonly ItemDropGrpCatalog _grp;
    readonly ItemDropMaterialService _materials;
    readonly ItemDropWeaponAligner _weapons;
    readonly ItemDropClickAreaService _clickArea;




}
